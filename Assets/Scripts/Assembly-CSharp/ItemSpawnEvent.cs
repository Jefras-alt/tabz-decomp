using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnEvent : MonoBehaviour
{
	public Item[] Item;

	private bool m_initialSpawn;

	public Vector3 Position { get; set; }

	private void Start()
	{
	}

	public void Trigger()
	{
		base.transform.position += Random.insideUnitSphere;
		PickItemToSpawn();
	}

	private void PickItemToSpawn()
	{
		int num = Random.Range(0, 101);
		List<Item> list = new List<Item>();
		for (int i = 0; i < Item.Length; i++)
		{
			if (num >= Item[i].Rarity)
			{
				list.Add(Item[i]);
			}
		}
		if (list.Count != 0)
		{
			int index = Random.Range(0, list.Count);
			GameObject itemPrefab = list[index].ItemPrefab;
			InventoryItem component = itemPrefab.GetComponent<InventoryItem>();
			Vector3 position = StickToGround(base.transform.position + Random.insideUnitSphere * 0.5f, component.ItemType);
			NetworkManager.SpawnLoot(list[index].ItemPrefab, position, -1, 15f);
		}
	}

	private Vector3 StickToGround(Vector3 position, InventoryService.ItemType type)
	{
		position.y += 0.1f;
		Ray ray = new Ray(position, Vector3.down);
		if (type == InventoryService.ItemType.WEAPON)
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
			if (type == InventoryService.ItemType.WEAPON)
			{
				point += hitInfo.normal * 0.1f;
			}
			position = point;
			base.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal) * base.transform.rotation;
		}
		base.transform.position = position;
		return position;
	}
}
