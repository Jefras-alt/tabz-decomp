using Photon;
using UnityEngine;

public class ZombieSpawner : Photon.MonoBehaviour
{
	private void Start()
	{
	}

	public void OnJoinedRoom()
	{
		NetworkManager.SpawnZombies(new byte[1], new Vector3[1] { base.transform.position });
	}
}
