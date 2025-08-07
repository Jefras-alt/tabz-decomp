using System;
using UnityEngine;

[Serializable]
public class Item
{
	public GameObject ItemPrefab;

	[Range(0f, 100f)]
	public int Rarity;
}
