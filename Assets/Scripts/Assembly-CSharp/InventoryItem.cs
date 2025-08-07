using FMOD.Studio;
using FMODUnity;
using Photon;
using UnityEngine;

public class InventoryItem : PunBehaviour
{
	[SerializeField]
	private string m_displayName = "Item";

	[SerializeField]
	private InventoryService.ItemType m_type;

	[SerializeField]
	private Sprite m_itemIcon;

	[SerializeField]
	private string m_flavourText = string.Empty;

	private InventoryService m_inventory;

	private PhotonView mPhotonView;

	private int m_worldItemIndex = -1;

	private bool m_isPickedUp;

	private Vector3 m_droppedPos;

	private string m_dropData = string.Empty;

	private float m_decayTime = -1f;

	private float m_initialDecayTime = -1f;

	private bool m_doDecay;

	private EventInstance mPickedUpEvent;

	private static SoundEventsManager mSoundEventsManager;

	public string DisplayName
	{
		get
		{
			return m_displayName;
		}
	}

	public string FlavourText
	{
		get
		{
			return m_flavourText;
		}
	}

	public InventoryService.ItemType ItemType
	{
		get
		{
			return m_type;
		}
	}

	public Sprite ItemIcon
	{
		get
		{
			return m_itemIcon;
		}
	}

	public PhotonView PhotonView
	{
		get
		{
			return mPhotonView;
		}
	}

	private void Awake()
	{
		if (mSoundEventsManager == null)
		{
			mSoundEventsManager = Object.FindObjectOfType<SoundEventsManager>();
		}
		m_inventory = ServiceLocator.GetService<InventoryService>();
		mPhotonView = GetComponent<PhotonView>();
		if (mPhotonView.instantiationData != null)
		{
			m_worldItemIndex = (int)mPhotonView.instantiationData[0];
			if (mPhotonView.instantiationData.Length > 1)
			{
				m_doDecay = true;
				m_decayTime = (float)mPhotonView.instantiationData[1];
				m_initialDecayTime = m_decayTime;
			}
			if (PhotonNetwork.isMasterClient && m_worldItemIndex >= 0)
			{
				mPhotonView.RPC("SpawnSuccess", PhotonTargets.All, m_worldItemIndex);
			}
			StickToGround(base.transform.position);
			if (m_inventory.IsInSave(mPhotonView.viewID))
			{
				TryPickup();
			}
			mPickedUpEvent = RuntimeManager.CreateInstance(mSoundEventsManager.PickUpItem);
		}
	}

	private void Update()
	{
		if (!m_doDecay)
		{
			return;
		}
		m_decayTime -= Time.deltaTime;
		if (!PhotonNetwork.isMasterClient)
		{
			if (m_decayTime <= 0f)
			{
				base.gameObject.SetActive(false);
			}
		}
		else if (m_decayTime <= 0f)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	private void OnEnable()
	{
		if (m_doDecay)
		{
			m_decayTime = m_initialDecayTime;
		}
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (m_isPickedUp)
			{
				if (m_dropData == string.Empty)
				{
					mPhotonView.RPC("Taken", newPlayer);
				}
				else
				{
					mPhotonView.RPC("Dropped", PhotonTargets.All, m_droppedPos, m_dropData);
				}
			}
			else
			{
				mPhotonView.RPC("SpawnSuccess", newPlayer, m_worldItemIndex);
			}
		}
		base.OnPhotonPlayerConnected(newPlayer);
	}

	private void PlayPickUpSound()
	{
		if (!(mPickedUpEvent == null))
		{
			mPickedUpEvent.set3DAttributes(base.transform.To3DAttributes());
			mPickedUpEvent.start();
		}
	}

	public bool TryPickup()
	{
		bool flag = false;
		if (m_type == InventoryService.ItemType.WEAPON)
		{
			if (m_inventory.AddItem(this, InventoryService.InventoryType.HOTBAR))
			{
				flag = true;
			}
			else if (m_inventory.AddItem(this, InventoryService.InventoryType.ITEM))
			{
				flag = true;
			}
		}
		else
		{
			flag = m_inventory.AddItem(this, InventoryService.InventoryType.ITEM);
		}
		if (flag)
		{
			mPhotonView.RPC("Taken", PhotonTargets.All);
			PlayPickUpSound();
			m_isPickedUp = true;
			m_dropData = string.Empty;
			return true;
		}
		m_isPickedUp = false;
		return false;
	}

	public void Drop(Vector3 position)
	{
		position = StickToGround(position);
		string text = string.Empty;
		if (ItemType == InventoryService.ItemType.WEAPON)
		{
			InventoryItemWeapon inventoryItemWeapon = this as InventoryItemWeapon;
			text = inventoryItemWeapon.BulletsInMagazine.ToString();
			Debug.Log("Dropping Weapon: " + inventoryItemWeapon.name + " With: " + inventoryItemWeapon.BulletsInMagazine);
		}
		else if (ItemType == InventoryService.ItemType.AMMUNITION)
		{
			InventoryItemAmmo inventoryItemAmmo = this as InventoryItemAmmo;
			text = inventoryItemAmmo.Amount.ToString();
		}
		m_isPickedUp = false;
		mPhotonView.RPC("Dropped", PhotonTargets.All, position, text);
	}

	private Vector3 StickToGround(Vector3 position)
	{
		position.y += 0.1f;
		Ray ray = new Ray(position, Vector3.down);
		if (ItemType == InventoryService.ItemType.WEAPON)
		{
			base.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 90f);
		}
		else
		{
			base.transform.rotation = Quaternion.identity;
		}
		int layerMask = ~LayerMask.GetMask("Item", "PlayerCollider", "PlayerColliderOther");
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, float.MaxValue, layerMask, QueryTriggerInteraction.Ignore))
		{
			Vector3 point = hitInfo.point;
			if (ItemType == InventoryService.ItemType.WEAPON)
			{
				point += hitInfo.normal * 0.1f;
			}
			position = point;
			base.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal) * base.transform.rotation;
		}
		base.transform.position = position;
		return position;
	}

	[PunRPC]
	public void Taken()
	{
		if (m_worldItemIndex == -1)
		{
			Debug.LogWarning("Item has an index of -1, invalid index!");
		}
		else
		{
			ChunkManager component = GameObject.FindGameObjectWithTag("ChunkManager").GetComponent<ChunkManager>();
			component.PickedUpItem(m_worldItemIndex);
		}
		base.gameObject.SetActive(false);
	}

	[PunRPC]
	public void SpawnSuccess(int index)
	{
		ChunkManager component = GameObject.FindGameObjectWithTag("ChunkManager").GetComponent<ChunkManager>();
		component.SuccessSpawnItem(index);
	}

	[PunRPC]
	public void Dropped(Vector3 pos, string data)
	{
		m_doDecay = true;
		m_decayTime = 500f;
		m_initialDecayTime = m_decayTime;
		base.transform.position = pos;
		if (ItemType == InventoryService.ItemType.WEAPON)
		{
			InventoryItemWeapon inventoryItemWeapon = this as InventoryItemWeapon;
			int num = (inventoryItemWeapon.BulletsInMagazine = int.Parse(data));
			Debug.Log(base.name + " dropped with: " + num + " Bullets left");
		}
		else if (ItemType == InventoryService.ItemType.AMMUNITION)
		{
			int num3 = int.Parse(data);
			InventoryItemAmmo inventoryItemAmmo = this as InventoryItemAmmo;
			inventoryItemAmmo.Amount = num3;
			Debug.Log(base.name + " dropped with: " + num3 + " Bullets left");
		}
		m_droppedPos = pos;
		m_dropData = data;
		base.gameObject.SetActive(true);
	}
}
