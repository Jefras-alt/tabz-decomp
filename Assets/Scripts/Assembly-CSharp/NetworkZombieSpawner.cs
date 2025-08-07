using System.Collections;
using Photon;
using UnityEngine;

public class NetworkZombieSpawner : PunBehaviour
{
	[SerializeField]
	private GameObject[] mZombieObjects;

	[SerializeField]
	[Range(0f, 100f)]
	private int[] m_spawnChances;

	private const ushort mMaxZombies = 100;

	private static ushort mSpawnedZombies;

	public int[] SpawnChances
	{
		get
		{
			return m_spawnChances;
		}
	}

	public static ushort SpawnedZombies
	{
		get
		{
			return mSpawnedZombies;
		}
	}

	private void Start()
	{
		mSpawnedZombies = 0;
	}

	private void Update()
	{
	}

	public byte GetRandomZombieIndex()
	{
		return (byte)Random.Range(1, mZombieObjects.Length);
	}

	public GameObject GetPrefabfromIndex(int index)
	{
		return mZombieObjects[index];
	}

	public byte GetIndexForPrefab(GameObject prefab)
	{
		for (byte b = 0; b < mZombieObjects.Length; b++)
		{
			if (mZombieObjects[b].name == prefab.name)
			{
				return b;
			}
		}
		return byte.MaxValue;
	}

	public void RequestSpawnZombies(byte[] zombieTypes, Vector3[] spawnPoints, Quaternion[] rotations, bool ignoreChunkSystem, PhotonPlayer player)
	{
		int num = spawnPoints.Length;
		ushort num2 = (ushort)(mSpawnedZombies + num);
		if (num2 <= 100)
		{
			mSpawnedZombies = num2;
			StartCoroutine(SpawnZombies(zombieTypes, spawnPoints, rotations, ignoreChunkSystem, player));
		}
	}

	[PunRPC]
	public void RequestDestroyZombies(PhotonView[] zombieViews, PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient)
		{
			Debug.Log("Zombie Destroy Request from Player: " + player.ID);
			StartCoroutine(DestroyZombies(zombieViews, player));
		}
	}

	private IEnumerator DestroyZombies(PhotonView[] zombieViews, PhotonPlayer player)
	{
		for (ushort i = 0; i < zombieViews.Length; i++)
		{
			PhotonView zombieView = zombieViews[i];
			PhotonNetwork.Destroy(zombieView);
			yield return new WaitForSeconds(1f);
		}
	}

	private IEnumerator SpawnZombies(byte[] zombieTypes, Vector3[] spawnPoints, Quaternion[] rotations, bool ignoreChunkSystem, PhotonPlayer player)
	{
		for (ushort i = 0; i < spawnPoints.Length; i++)
		{
			GameObject spawnedZombie = PhotonNetwork.Instantiate(position: spawnPoints[i], prefabName: mZombieObjects[zombieTypes[i]].name, rotation: (rotations != null) ? rotations[i] : Quaternion.identity, group: 0, data: new object[1] { ignoreChunkSystem });
			yield return new WaitForSeconds(2f);
		}
	}

	public static void OnZombieSpawned()
	{
		if (mSpawnedZombies > 100)
		{
		}
		mSpawnedZombies++;
	}

	public static void OnZombieDied()
	{
		if (mSpawnedZombies != 0)
		{
			mSpawnedZombies--;
		}
	}
}
