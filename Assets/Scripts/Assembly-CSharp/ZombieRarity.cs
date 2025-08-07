using System;
using UnityEngine;

[Serializable]
public class ZombieRarity
{
	public GameObject Prefab;

	[Range(0f, 100f)]
	public int Rarity;
}
