using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour, IDragHandler, IDropHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	[SerializeField]
	private Image m_itemBackground;

	[SerializeField]
	private Image m_itemIcon;

	[SerializeField]
	private Text m_textAmount;

	private UIInventory m_uiInventory;

	private InventoryItem m_item;

	private Sprite m_emptyIcon;

	private int m_index = -1;

	private InventoryService.InventoryType m_inventoryType;

	private InventoryService.ItemType m_itemType;

	private bool m_pointerOver;

	public Image ItemIcon
	{
		get
		{
			return m_itemIcon;
		}
	}

	public InventoryItem CurrentItem
	{
		get
		{
			return m_item;
		}
	}

	public bool IsEmpty
	{
		get
		{
			return m_item == null;
		}
	}

	public InventoryService.InventoryType InventoryType
	{
		get
		{
			return m_inventoryType;
		}
	}

	public InventoryService.ItemType InventoryTypeItem
	{
		get
		{
			return m_itemType;
		}
	}

	public int Index
	{
		get
		{
			return m_index;
		}
	}

	public void Initialize(UIInventory inventory, int index, InventoryService.InventoryType inventoryType, InventoryService.ItemType itemType = InventoryService.ItemType.ITEM, bool drag = false)
	{
		m_uiInventory = inventory;
		m_emptyIcon = m_itemIcon.sprite;
		m_index = index;
		m_inventoryType = inventoryType;
		m_itemType = itemType;
		if (drag)
		{
			m_itemBackground.color = new Color(1f, 1f, 1f, 0f);
			m_itemBackground.raycastTarget = false;
			base.gameObject.SetActive(false);
		}
		UpdateUI();
	}

	public void UpdateItem(InventoryItem item)
	{
		m_item = item;
		UpdateUI();
	}

	public InventoryItem ClearItem()
	{
		InventoryItem item = m_item;
		m_item = null;
		UpdateUI();
		return item;
	}

	private void Update()
	{
		if (!(m_item == null) && m_item.ItemType == InventoryService.ItemType.WEAPON && m_pointerOver && Input.GetKeyDown(KeyCode.Mouse1))
		{
			InventoryService service = ServiceLocator.GetService<InventoryService>();
			switch (m_inventoryType)
			{
			case InventoryService.InventoryType.ITEM:
				service.AddItem(m_item, InventoryService.InventoryType.HOTBAR);
				break;
			}
		}
	}

	public void UpdateUI()
	{
		if (!IsEmpty)
		{
			m_itemIcon.sprite = m_item.ItemIcon;
			m_itemIcon.color = new Color(1f, 1f, 1f, 1f);
			if (m_item.ItemType == InventoryService.ItemType.AMMUNITION)
			{
				InventoryItemAmmo inventoryItemAmmo = m_item as InventoryItemAmmo;
				if (inventoryItemAmmo != null)
				{
					m_textAmount.text = inventoryItemAmmo.Amount.ToString();
				}
			}
			else
			{
				m_textAmount.text = string.Empty;
			}
		}
		else
		{
			m_itemIcon.sprite = m_emptyIcon;
			m_itemIcon.color = new Color(1f, 1f, 1f, 0f);
			m_uiInventory.UpdateTooltip(null);
			m_textAmount.text = string.Empty;
		}
		if (m_pointerOver && m_item != null)
		{
			m_uiInventory.UpdateTooltip(this);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
	}

	public void OnDrop(PointerEventData eventData)
	{
		m_uiInventory.OnDrop(this);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!IsEmpty && m_uiInventory.OnBeginDrag(this))
		{
			m_itemIcon.enabled = false;
			m_textAmount.enabled = false;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		m_uiInventory.OnEndDrag(this, m_pointerOver);
		m_itemIcon.enabled = true;
		m_textAmount.enabled = true;
	}

	public void ClearDrag()
	{
		m_itemIcon.enabled = true;
		m_textAmount.enabled = true;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		m_pointerOver = true;
		if (m_item == null)
		{
			m_uiInventory.UpdateTooltip(null);
		}
		else
		{
			m_uiInventory.UpdateTooltip(this);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		m_pointerOver = false;
		m_uiInventory.UpdateTooltip(null);
	}
}
