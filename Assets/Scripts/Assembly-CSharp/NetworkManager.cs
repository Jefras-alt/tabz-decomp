using Photon;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : PunBehaviour, IPunCallbacks
{
	private static PhotonView mMasterPhotonView;

	private static PhotonView mLocalPlayerPhotonView;

	private int mZombiesToSpawn = 5;

	private Vector3[] mSpawnPoints = new Vector3[10]
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(1f, 1f, 1f),
		new Vector3(0f, 0f, 0f),
		new Vector3(1f, 1f, 1f),
		new Vector3(0f, 0f, 0f),
		new Vector3(1f, 1f, 1f),
		new Vector3(0f, 0f, 0f),
		new Vector3(1f, 1f, 1f),
		new Vector3(0f, 0f, 0f),
		new Vector3(1f, 1f, 1f)
	};

	private static NetworkZombieSpawner mZombieSpawner;

	public static PhotonView MasterPhotonView
	{
		get
		{
			return mMasterPhotonView;
		}
	}

	public static PhotonView LocalPlayerPhotonView
	{
		get
		{
			return mLocalPlayerPhotonView;
		}
	}

	private void Awake()
	{
		mMasterPhotonView = GetComponent<PhotonView>();
		mZombieSpawner = Object.FindObjectOfType<NetworkZombieSpawner>();
	}

	private void Start()
	{
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
	}

	private void Update()
	{
	}

	public static void SpawnZombies(byte[] zombieTypes, Vector3[] positions, Quaternion[] rotations = null, bool ignoreChunkSystem = false)
	{
		mZombieSpawner.RequestSpawnZombies(zombieTypes, positions, rotations, ignoreChunkSystem, PhotonNetwork.player);
	}

	public static void DestroyZombie(PhotonView[] zombieViews)
	{
		for (ushort num = 0; num < zombieViews.Length; num++)
		{
			PhotonView photonView = zombieViews[num];
			photonView.RPC("NetworkDestroy", photonView.owner);
		}
	}

	public static GameObject SpawnLoot(GameObject prefab, Vector3 position, int index, float decayTime = -1f)
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return null;
		}
		if (decayTime <= 0f)
		{
			return PhotonNetwork.Instantiate("Items/" + prefab.name, position, Quaternion.identity, 0, new object[1] { index });
		}
		return PhotonNetwork.Instantiate("Items/" + prefab.name, position, Quaternion.identity, 0, new object[2] { index, decayTime });
	}

	public override void OnDisconnectedFromPhoton()
	{
		base.OnDisconnectedFromPhoton();
		SteamMatchmaking.LeaveLobby(ServerSearcher.CurrentSelectedServer);
		Debug.LogError("Disconnected!");
		SceneManager.LoadScene(0);
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		base.OnPhotonPlayerConnected(newPlayer);
		if (PhotonNetwork.isMasterClient)
		{
			SteamMatchmaking.SetLobbyData(ServerSearcher.CurrentSelectedServer, MainMenuHandler.SERVER_PLAYERS_PARAM, PhotonNetwork.room.PlayerCount.ToString());
		}
	}

	public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{
		base.OnPhotonPlayerDisconnected(otherPlayer);
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		PhotonNetwork.RemoveRPCs(otherPlayer);
		ZombieBlackboard[] array = Object.FindObjectsOfType<ZombieBlackboard>();
		for (ushort num = 0; num < array.Length; num++)
		{
			PhotonView component = array[num].GetComponent<PhotonView>();
			if (component.owner == null)
			{
				Debug.Log("Found Stray Zombie with no owner!");
				component.TransferOwnership(PhotonNetwork.masterClient);
			}
		}
		NetworkPlayer[] array2 = Object.FindObjectsOfType<NetworkPlayer>();
		for (ushort num2 = 0; num2 < array2.Length; num2++)
		{
			NetworkPlayer networkPlayer = array2[num2];
			PhotonView component2 = networkPlayer.GetComponent<PhotonView>();
			if (component2.isMine && component2.viewID != mLocalPlayerPhotonView.viewID)
			{
				Debug.Log("Network Removing: ", networkPlayer);
				PhotonNetwork.Destroy(component2);
			}
		}
	}

	public override void OnOwnershipTransfered(object[] viewAndPlayers)
	{
		base.OnOwnershipTransfered(viewAndPlayers);
		ZombieBlackboard component = (viewAndPlayers[0] as PhotonView).GetComponent<ZombieBlackboard>();
		if ((bool)component)
		{
			component.Init();
		}
	}

	public override void OnOwnershipRequest(object[] viewAndPlayer)
	{
		base.OnOwnershipRequest(viewAndPlayer);
	}

	public static void OnPlayerSpawned(PhotonView playerView)
	{
		mLocalPlayerPhotonView = playerView;
	}

	public static GameObject SpawnLootCheat(GameObject prefab, Vector3 position, int index, float decayTime = -1f)
	{
		if (decayTime <= 0f)
		{
			return PhotonNetwork.Instantiate("Items/" + prefab.name, position, Quaternion.identity, 0, new object[1] { index });
		}
		return PhotonNetwork.Instantiate("Items/" + prefab.name, position, Quaternion.identity, 0, new object[2] { index, decayTime });
	}
}
