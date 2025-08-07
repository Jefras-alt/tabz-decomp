using UnityEngine;

public class InventoryItemWeapon : InventoryItem
{
	[SerializeField]
	private InventoryService.AmmoType m_ammoType;

	[SerializeField]
	private int m_bulletsInMagazine = 10;

	public InventoryService.AmmoType AmmoType
	{
		get
		{
			return m_ammoType;
		}
	}

	public int BulletsInMagazine
	{
		get
		{
			return m_bulletsInMagazine;
		}
		set
		{
			m_bulletsInMagazine = value;
		}
	}
}
