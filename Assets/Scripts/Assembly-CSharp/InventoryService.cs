using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryService : IService
{
	public enum ItemType
	{
		ITEM = 0,
		WEAPON = 1,
		AMMUNITION = 2
	}

	public enum AmmoType : byte
	{
		Small = 0,
		Medium = 1,
		Big = 2,
		Shell = 3,
		None = 4
	}

	public enum InventoryType
	{
		ITEM = 0,
		HOTBAR = 1
	}

	[Serializable]
	public class InventorySave
	{
		public string m_roomName;

		public int[] m_items;

		public int[] m_hotbar;
	}

	public delegate void OnItemAdded(InventoryItem item, InventoryType type, int i);

	private static string INVENTORY_KEY = "INVENTORY";

	public static int SIZE_INVENTORY = 16;

	public static int SIZE_HOTBAR = 6;

	private InventoryItem[] m_items = new InventoryItem[SIZE_INVENTORY];

	private InventoryItem[] m_hotbar = new InventoryItem[SIZE_HOTBAR];

	private InventorySave m_currentSave;

	private UIInventory m_uiInventory;

	public UIInventory CurrentUI
	{
		get
		{
			return m_uiInventory;
		}
	}

	public event OnItemAdded OnItemAddedE;

	public void Initialize()
	{
		LoadInventoryState();
	}

	public void Update()
	{
	}

	public void LateUpdate()
	{
	}

	public void FixedUpdate()
	{
	}

	public void Destroy()
	{
	}

	public void RegisterUI(UIInventory ui)
	{
		m_uiInventory = ui;
	}

	public void UnregisterUI(UIInventory ui)
	{
		if (!(m_uiInventory != ui))
		{
			m_uiInventory = null;
		}
	}

	public bool AddItem(InventoryItem item, InventoryType type)
	{
		bool flag = false;
		InventoryType type2;
		int index;
		if (Contains(item, out type2, out index))
		{
			if (type == type2)
			{
				return false;
			}
			flag = true;
		}
		switch (type)
		{
		case InventoryType.ITEM:
		{
			if (item.ItemType == ItemType.AMMUNITION)
			{
				InventoryItemAmmo inventoryItemAmmo = null;
				int i2 = 0;
				for (int j = 0; j < SIZE_INVENTORY; j++)
				{
					if (m_items[j] != null && m_items[j].DisplayName == item.DisplayName)
					{
						inventoryItemAmmo = m_items[j] as InventoryItemAmmo;
						i2 = j;
						break;
					}
				}
				if (inventoryItemAmmo != null)
				{
					InventoryItemAmmo inventoryItemAmmo2 = item as InventoryItemAmmo;
					if (inventoryItemAmmo2 != null)
					{
						inventoryItemAmmo.Amount += inventoryItemAmmo2.Amount;
						if (this.OnItemAddedE != null)
						{
							this.OnItemAddedE(inventoryItemAmmo, InventoryType.ITEM, i2);
						}
						return true;
					}
					return false;
				}
			}
			for (int k = 0; k < SIZE_INVENTORY; k++)
			{
				if (m_items[k] == null)
				{
					if (flag)
					{
						InternalSet(null, type2, index);
					}
					InternalSet(item, type, k);
					return true;
				}
			}
			return false;
		}
		case InventoryType.HOTBAR:
		{
			for (int i = 0; i < SIZE_HOTBAR; i++)
			{
				if (m_hotbar[i] == null)
				{
					if (flag)
					{
						InternalSet(null, type2, index);
					}
					InternalSet(item, type, i);
					return true;
				}
			}
			return false;
		}
		default:
			return false;
		}
	}

	public void SetItem(InventoryItem item, InventoryType type, int index)
	{
		InventoryItem[] list = GetList(type);
		InventoryType type2;
		int index2;
		if (Contains(item, out type2, out index2))
		{
			InternalSet(null, type2, index2);
		}
		InternalSet(item, type, index);
	}

	public bool RemoveItem(InventoryItem item)
	{
		InventoryType type;
		int index;
		if (Contains(item, out type, out index))
		{
			InternalSet(null, type, index);
			return true;
		}
		return false;
	}

	public List<InventoryItem> ClearInventory()
	{
		List<InventoryItem> list = new List<InventoryItem>();
		Array values = Enum.GetValues(typeof(InventoryType));
		foreach (InventoryType item in values)
		{
			InventoryItem[] list2 = GetList(item);
			int num = list2.Length;
			for (int i = 0; i < num; i++)
			{
				if (list2[i] != null)
				{
					list.Add(list2[i]);
					InternalSet(null, item, i, false);
				}
			}
		}
		PlayerPrefs.DeleteKey(INVENTORY_KEY);
		return list;
	}

	private void InternalSet(InventoryItem item, InventoryType type, int index, bool saveState = true)
	{
		GetList(type)[index] = item;
		if (this.OnItemAddedE != null)
		{
			this.OnItemAddedE(item, type, index);
		}
		if (saveState)
		{
			SaveInventoryState();
		}
	}

	private void SaveInventoryState()
	{
		if (PhotonNetwork.room == null)
		{
			return;
		}
		string name = PhotonNetwork.room.Name;
		InventorySave inventorySave = new InventorySave();
		inventorySave.m_roomName = name;
		int num = m_items.Length;
		inventorySave.m_items = new int[num];
		for (int i = 0; i < num; i++)
		{
			if (!(m_items[i] == null))
			{
				inventorySave.m_items[i] = m_items[i].PhotonView.viewID;
			}
		}
		num = m_hotbar.Length;
		inventorySave.m_hotbar = new int[num];
		for (int j = 0; j < num; j++)
		{
			if (!(m_hotbar[j] == null))
			{
				inventorySave.m_hotbar[j] = m_hotbar[j].PhotonView.viewID;
			}
		}
		string value = JsonUtility.ToJson(inventorySave);
		PlayerPrefs.SetString(INVENTORY_KEY, value);
	}

	private void LoadInventoryState()
	{
		string text = PlayerPrefs.GetString(INVENTORY_KEY, string.Empty);
		if (!string.IsNullOrEmpty(text))
		{
			m_currentSave = JsonUtility.FromJson<InventorySave>(text);
		}
	}

	public bool IsInSave(int viewID)
	{
		if (viewID == 0 || m_currentSave == null)
		{
			return false;
		}
		if (m_currentSave.m_roomName != PhotonNetwork.room.Name)
		{
			return false;
		}
		int num = m_items.Length;
		for (int i = 0; i < num; i++)
		{
			if (m_currentSave.m_items[i] != 0 && m_currentSave.m_items[i] == viewID)
			{
				return true;
			}
		}
		num = m_hotbar.Length;
		for (int j = 0; j < num; j++)
		{
			if (m_currentSave.m_hotbar[j] != 0 && m_currentSave.m_hotbar[j] == viewID)
			{
				return true;
			}
		}
		return false;
	}

	public bool Contains(InventoryItem item)
	{
		InventoryType type;
		int index;
		return Contains(item, out type, out index);
	}

	public bool Contains(InventoryItem item, out InventoryType type)
	{
		int index;
		return Contains(item, out type, out index);
	}

	public bool Contains(InventoryItem item, out InventoryType type, out int index)
	{
		Array values = Enum.GetValues(typeof(InventoryType));
		foreach (InventoryType item2 in values)
		{
			InventoryItem[] list = GetList(item2);
			int num = list.Length;
			for (int i = 0; i < num; i++)
			{
				if (list[i] == item)
				{
					index = i;
					type = item2;
					return true;
				}
			}
		}
		index = -1;
		type = InventoryType.ITEM;
		return false;
	}

	public bool Contains(InventoryItem item, InventoryType type)
	{
		int index;
		return Contains(item, type, out index);
	}

	public bool Contains(InventoryItem item, InventoryType type, out int index)
	{
		InventoryItem[] list = GetList(type);
		int num = list.Length;
		for (int i = 0; i < num; i++)
		{
			if (list[i] == item)
			{
				index = i;
				return true;
			}
		}
		index = -1;
		return false;
	}

	public InventoryItem[] GetList(InventoryType type)
	{
		switch (type)
		{
		case InventoryType.ITEM:
			return m_items;
		case InventoryType.HOTBAR:
			return m_hotbar;
		default:
			return m_items;
		}
	}

	public InventoryItem GetItem(InventoryType type, int index)
	{
		return GetList(type)[index];
	}

	public float UpdateTime()
	{
		return 0f;
	}

	public float LateUpdateTime()
	{
		return 0f;
	}

	public float FixedUpdateTime()
	{
		return 0f;
	}
}
