using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
	public delegate void OnEquipItem(InventoryItem item);

	public InventoryItem[] m_testItems;

	[SerializeField]
	private UIInventoryItem m_uiItemPrefab;

	[SerializeField]
	private UIInventoryTooltip m_tooltip;

	[SerializeField]
	private CodeStateAnimation m_itemsAnimation;

	[SerializeField]
	private CodeStateAnimation m_hotbarAnimation;

	[SerializeField]
	private RectTransform m_itemContainer;

	[SerializeField]
	private RectTransform m_hotbarContainer;

	[SerializeField]
	private RectTransform m_hotbarSelection;

	private InventoryService m_inventory;

	private UIInventoryItem[] m_items;

	private UIInventoryItem[] m_hotbar;

	private UIInventoryItem m_dragItem;

	private UIInventoryItem m_dragCurrent;

	private Canvas m_canvas;

	private RectTransform m_transform;

	private Text m_textAmmoInfo;

	private int m_selectedHotbarIndex;

	private bool m_hasNetworkControl;

	private bool m_isOpen;

	private float m_timeLastUsedHotbar;

	private bool m_droppedOnItem;

	private EventInstance mClickSound;

	private Weapon mWeaponRef;

	public bool IsOpen
	{
		get
		{
			return m_isOpen;
		}
	}

	public event OnEquipItem OnEquipItemE;

	private void Awake()
	{
		m_textAmmoInfo = base.transform.parent.GetComponentInChildren<AmmoUITAG>().GetComponent<Text>();
		PhotonView componentInParent = GetComponentInParent<PhotonView>();
		m_hasNetworkControl = componentInParent == null || componentInParent.isMine;
		m_canvas = GetComponentInParent<Canvas>();
		if (!m_hasNetworkControl)
		{
			m_canvas.gameObject.SetActive(false);
			base.enabled = false;
			return;
		}
		string inventoryClick = Object.FindObjectOfType<SoundEventsManager>().InventoryClick;
		if (!string.IsNullOrEmpty(inventoryClick))
		{
			mClickSound = RuntimeManager.CreateInstance(inventoryClick);
		}
		m_transform = base.transform as RectTransform;
		m_inventory = ServiceLocator.GetService<InventoryService>();
		if (m_inventory == null)
		{
			Debug.LogWarning("Inventory service cannot be found!!!");
			base.enabled = false;
		}
		else
		{
			Initialize();
			StartCoroutine(LateInitialize());
		}
	}

	private void PlayClickSound()
	{
		mClickSound.start();
	}

	private void Start()
	{
		for (int i = 0; i < m_testItems.Length; i++)
		{
			if (!(m_testItems[i] == null))
			{
				InventoryItem component = PhotonNetwork.Instantiate("Items/" + m_testItems[i].name, Vector2.zero, Quaternion.identity, 0).GetComponent<InventoryItem>();
				if (component != null)
				{
					component.TryPickup();
				}
			}
		}
	}

	public InventoryItemAmmo GetAmmoType(InventoryService.AmmoType type)
	{
		for (int i = 0; i < m_items.Length; i++)
		{
			InventoryItem currentItem = m_items[i].CurrentItem;
			if (!(currentItem == null) && currentItem.ItemType == InventoryService.ItemType.AMMUNITION)
			{
				InventoryItemAmmo inventoryItemAmmo = currentItem as InventoryItemAmmo;
				if (inventoryItemAmmo.AmmoType == type)
				{
					return inventoryItemAmmo;
				}
			}
		}
		return null;
	}

	public void UpdateAmmoUI(InventoryItemAmmo itemAmmo)
	{
		for (int i = 0; i < m_items.Length; i++)
		{
			InventoryItem currentItem = m_items[i].CurrentItem;
			if (!(currentItem == null) && currentItem == itemAmmo)
			{
				m_items[i].UpdateUI();
				break;
			}
		}
	}

	public void UpdateTooltip(UIInventoryItem item)
	{
		m_tooltip.UpdateInfo(item);
	}

	internal void AddReferenceToThis(Weapon weapon)
	{
		mWeaponRef = weapon;
	}

	private void Initialize()
	{
		m_inventory.RegisterUI(this);
		m_inventory.OnItemAddedE += OnItemAdded;
		int sIZE_INVENTORY = InventoryService.SIZE_INVENTORY;
		m_items = new UIInventoryItem[sIZE_INVENTORY];
		for (int i = 0; i < sIZE_INVENTORY; i++)
		{
			UIInventoryItem uIInventoryItem = Object.Instantiate(m_uiItemPrefab, m_itemContainer, false);
			m_items[i] = uIInventoryItem;
			if (uIInventoryItem != null)
			{
				uIInventoryItem.Initialize(this, i, InventoryService.InventoryType.ITEM);
			}
		}
		m_hotbar = new UIInventoryItem[6];
		for (int j = 0; j < 6; j++)
		{
			UIInventoryItem uIInventoryItem2 = Object.Instantiate(m_uiItemPrefab, m_hotbarContainer, false);
			m_hotbar[j] = uIInventoryItem2;
			if (uIInventoryItem2 != null)
			{
				uIInventoryItem2.Initialize(this, j, InventoryService.InventoryType.HOTBAR);
			}
		}
		m_dragItem = Object.Instantiate(m_uiItemPrefab, base.transform, false);
		m_dragItem.Initialize(this, -1, InventoryService.InventoryType.ITEM, InventoryService.ItemType.ITEM, true);
		m_dragItem.gameObject.name = "InventoryItemDrag";
		m_isOpen = true;
		ToggleOpen();
	}

	private IEnumerator LateInitialize()
	{
		yield return new WaitForEndOfFrame();
		UpdateHotbarSelection();
	}

	public InventoryItem RemoveItem(int index)
	{
		if (m_items[index].IsEmpty)
		{
			return null;
		}
		return m_items[index].ClearItem();
	}

	private void OnItemAdded(InventoryItem item, InventoryService.InventoryType type, int index)
	{
		switch (type)
		{
		case InventoryService.InventoryType.ITEM:
			m_items[index].UpdateItem(item);
			if (!(item == null) && item.ItemType == InventoryService.ItemType.AMMUNITION && mWeaponRef != null)
			{
				mWeaponRef.UpdateUI();
			}
			break;
		case InventoryService.InventoryType.HOTBAR:
			m_hotbar[index].UpdateItem(item);
			if (m_selectedHotbarIndex == index && this.OnEquipItemE != null)
			{
				this.OnEquipItemE(m_hotbar[m_selectedHotbarIndex].CurrentItem);
			}
			break;
		}
	}

	public void DropItem(InventoryItem item, Vector3 positionOffset, bool destoyItem = false, bool forceDrop = false)
	{
		if (item != null && (m_inventory.RemoveItem(item) || forceDrop))
		{
			if (destoyItem)
			{
				PhotonNetwork.Destroy(item.PhotonView);
				return;
			}
			Vector3 position = base.transform.root.GetComponent<PhysicsAmimationController>().mainRig.position + positionOffset;
			item.Drop(position);
		}
	}

	public void DropItem(UIInventoryItem item, Vector3 positionOffset, bool destroyItem = false, bool forceDrop = false)
	{
		DropItem(item.CurrentItem, positionOffset, destroyItem, forceDrop);
	}

	public bool OnBeginDrag(UIInventoryItem item)
	{
		if (!m_isOpen)
		{
			return false;
		}
		m_dragCurrent = item;
		m_dragItem.UpdateItem(item.CurrentItem);
		m_dragItem.gameObject.SetActive(true);
		PlayClickSound();
		return true;
	}

	public void OnEndDrag(UIInventoryItem droppedItem, bool droppedOnSelf = false)
	{
		if (!(m_dragCurrent == null))
		{
			if (!m_droppedOnItem && !droppedOnSelf)
			{
				DropItem(m_dragCurrent, Vector3.zero);
			}
			m_droppedOnItem = false;
			ClearDrag();
			PlayClickSound();
		}
	}

	public void OnDrop(UIInventoryItem droppedOn)
	{
		m_droppedOnItem = true;
		if (m_dragCurrent == null)
		{
			return;
		}
		InventoryItem currentItem = m_dragCurrent.CurrentItem;
		InventoryItem currentItem2 = droppedOn.CurrentItem;
		if (currentItem == currentItem2)
		{
			ClearDrag();
			return;
		}
		m_inventory.SetItem(currentItem, droppedOn.InventoryType, droppedOn.Index);
		m_inventory.SetItem(currentItem2, m_dragCurrent.InventoryType, m_dragCurrent.Index);
		if (droppedOn.InventoryType == InventoryService.InventoryType.HOTBAR && currentItem.ItemType == InventoryService.ItemType.WEAPON)
		{
			m_inventory.SetItem(currentItem, droppedOn.InventoryType, droppedOn.Index);
		}
	}

	private void ClearDrag()
	{
		m_dragCurrent = null;
		m_dragItem.ClearItem();
		m_dragItem.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (m_hasNetworkControl && !TABZChat.DisableInput)
		{
			CheckInput();
			if (m_isOpen)
			{
				m_hotbarAnimation.state1 = true;
			}
			else if (Time.unscaledTime - m_timeLastUsedHotbar > 0.5f)
			{
				m_hotbarAnimation.state1 = false;
			}
		}
	}

	private void LateUpdate()
	{
		if (m_dragCurrent != null)
		{
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(m_transform, Input.mousePosition, m_canvas.worldCamera, out localPoint);
			m_dragItem.transform.position = m_canvas.transform.TransformPoint(localPoint);
		}
	}

	private void CheckInput()
	{
		if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
		{
			ToggleOpen();
		}
		for (int i = 0; i < InventoryService.SIZE_HOTBAR; i++)
		{
			if (Input.GetKeyDown((KeyCode)(49 + i)))
			{
				m_selectedHotbarIndex = i;
				UpdateHotbarSelection();
			}
		}
	}

	private void UpdateHotbarSelection()
	{
		m_hotbarAnimation.state1 = true;
		m_timeLastUsedHotbar = Time.unscaledTime;
		Vector3 vector = new Vector3(0f, m_hotbarSelection.sizeDelta.y * 0.5f, 0f);
		m_hotbarSelection.localPosition = m_hotbar[m_selectedHotbarIndex].transform.localPosition + vector;
		InventoryItem currentItem = m_hotbar[m_selectedHotbarIndex].CurrentItem;
		if (currentItem == null || currentItem.ItemType != InventoryService.ItemType.WEAPON)
		{
			m_textAmmoInfo.text = string.Empty;
		}
		if (this.OnEquipItemE != null)
		{
			this.OnEquipItemE(currentItem);
		}
	}

	private void ToggleOpen()
	{
		if (m_isOpen && m_dragCurrent != null)
		{
			m_dragCurrent.ClearDrag();
			ClearDrag();
		}
		m_isOpen = !m_isOpen;
		m_itemsAnimation.state1 = m_isOpen;
		Cursor.visible = m_isOpen;
		Cursor.lockState = ((!m_isOpen) ? CursorLockMode.Locked : CursorLockMode.None);
		m_timeLastUsedHotbar = Time.unscaledTime;
	}

	public void Die(bool destroyItems = false)
	{
		if (m_inventory != null)
		{
			Vector3 positionOffset = new Vector3(0f, 0f, 0f);
			List<InventoryItem> list = m_inventory.ClearInventory();
			for (int i = 0; i < list.Count; i++)
			{
				DropItem(list[i], positionOffset, false, true);
			}
			m_inventory.OnItemAddedE -= OnItemAdded;
			m_inventory.UnregisterUI(this);
		}
	}
}
