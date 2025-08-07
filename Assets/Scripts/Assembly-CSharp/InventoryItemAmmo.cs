using UnityEngine;

public class InventoryItemAmmo : InventoryItem
{
	[SerializeField]
	private int m_amount = 10;

	[SerializeField]
	private InventoryService.AmmoType m_Type;

	public int Amount
	{
		get
		{
			return m_amount;
		}
		set
		{
			m_amount = value;
		}
	}

	public InventoryService.AmmoType AmmoType
	{
		get
		{
			return m_Type;
		}
		set
		{
			m_Type = value;
		}
	}
}
