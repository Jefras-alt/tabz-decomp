using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ServerSearcher : MonoBehaviour
{
	public struct ServerCellParams
	{
		public CSteamID LobbyID;

		public string LobbyName;

		public string PlayersInside;

		public int PlayersMax;
	}

	public static bool JoinedThroughServerList;

	private CallResult<LobbyMatchList_t> m_OnLobbyListRecievedCallBack;

	[SerializeField]
	private GameObject mServerCell;

	[SerializeField]
	private Button mJoinLobbyButton;

	[SerializeField]
	private Transform mServerListContent;

	protected CallResult<LobbyEnter_t> mOnLobbyEnteredCallResult;

	private static CSteamID mCurrentSelectedServer;

	public CodeStateAnimation anim;

	private MainMenuHandler mMainMenuHandler;

	private static string selectedRoom;

	private static string SERVER_NAME_PARAM
	{
		get
		{
			return MainMenuHandler.SERVER_NAME_PARAM;
		}
	}

	private static string SERVER_VERSION_PARAM
	{
		get
		{
			return MainMenuHandler.SERVER_VERSION_PARAM;
		}
	}

	private static string SERVER_PLAYERS_PARAM
	{
		get
		{
			return MainMenuHandler.SERVER_PLAYERS_PARAM;
		}
	}

	public static CSteamID CurrentSelectedServer
	{
		get
		{
			return mCurrentSelectedServer;
		}
		set
		{
			mCurrentSelectedServer = value;
		}
	}

	private void OnEnable()
	{
		mMainMenuHandler = Object.FindObjectOfType<MainMenuHandler>();
		mJoinLobbyButton.gameObject.SetActive(selectedRoom != "");
		PopulateServerListWithRooms();
	}

	private void OnDisable()
	{
		mCurrentSelectedServer = (CSteamID)0uL;
		selectedRoom = "";
		anim.SetState(false);
	}

	private void SearchForServers()
	{
		SteamAPICall_t hAPICall = SteamMatchmaking.RequestLobbyList();
		m_OnLobbyListRecievedCallBack.Set(hAPICall);
	}

	private void OnLobbyListRecieved(LobbyMatchList_t param, bool bIOFailure)
	{
		if (bIOFailure)
		{
			Debug.LogError("Bio failure!");
			return;
		}
		List<ServerCellParams> list = new List<ServerCellParams>();
		Debug.Log("Lobbies returned: " + param.m_nLobbiesMatching);
		for (int i = 0; i < param.m_nLobbiesMatching; i++)
		{
			CSteamID lobbyByIndex = SteamMatchmaking.GetLobbyByIndex(i);
			if (!IsLobbyValid(lobbyByIndex))
			{
				Debug.Log("Lobby is not valid!" + lobbyByIndex);
				continue;
			}
			list.Add(new ServerCellParams
			{
				LobbyID = lobbyByIndex,
				LobbyName = SteamMatchmaking.GetLobbyData(lobbyByIndex, SERVER_NAME_PARAM),
				PlayersInside = SteamMatchmaking.GetLobbyData(lobbyByIndex, SERVER_PLAYERS_PARAM),
				PlayersMax = SteamMatchmaking.GetLobbyMemberLimit(lobbyByIndex)
			});
		}
		PopulateServerListWithServers(list);
	}

	private bool IsLobbyValid(CSteamID currLobby)
	{
		string lobbyData = SteamMatchmaking.GetLobbyData(currLobby, SERVER_VERSION_PARAM);
		bool flag = lobbyData.Equals(MainMenuHandler.VERSION_NUMBER) || (lobbyData == string.Empty && MainMenuHandler.VERSION_NUMBER == "STEAM TABZ v1.1");
		return currLobby.IsValid() && flag;
	}

	private void PopulateServerListWithServers(List<ServerCellParams> validLobbies)
	{
		ClearServerList();
		for (int i = 0; i < validLobbies.Count; i++)
		{
			ServerCellParams serverParams = validLobbies[i];
			GameObject obj = Object.Instantiate(mServerCell, mServerListContent, true);
			obj.GetComponent<SetServerTextValues>().InitServerValues(serverParams);
			obj.SetActive(true);
		}
		if (validLobbies.Count > 0)
		{
			anim.state1 = false;
		}
		else
		{
			anim.state1 = true;
		}
	}

	private void ClearServerList()
	{
		for (int i = 0; i < mServerListContent.childCount; i++)
		{
			Object.Destroy(mServerListContent.GetChild(i).gameObject);
		}
	}

	public void OnSubmit(BaseEventData data)
	{
		PhotonNetwork.JoinOrCreateRoom(data.selectedObject.GetComponent<SetServerTextValues>().ServerNickName, new RoomOptions
		{
			MaxPlayers = MainMenuHandler.MaxPlayers,
			CleanupCacheOnLeave = false
		}, null);
	}

	public void NewServerClicked(CSteamID id)
	{
		if (!id.IsValid())
		{
			Debug.Log("SelectedServer Is null!!");
			return;
		}
		if (mCurrentSelectedServer == id)
		{
			JoinServer(id);
			return;
		}
		mCurrentSelectedServer = id;
		mJoinLobbyButton.gameObject.SetActive(mCurrentSelectedServer != (CSteamID)0uL);
	}

	public void JoinButtonClicked()
	{
		if (mCurrentSelectedServer.IsValid())
		{
			JoinServer(mCurrentSelectedServer);
		}
	}

	private void JoinServer(CSteamID id)
	{
		if (mJoinLobbyButton.gameObject.activeInHierarchy)
		{
			mMainMenuHandler.FadeToBlack(true);
			SteamAPICall_t hAPICall = SteamMatchmaking.JoinLobby(id);
			mOnLobbyEnteredCallResult.Set(hAPICall);
		}
	}

	private void OnLobbyEntered(LobbyEnter_t param, bool bIOFailure)
	{
		if (bIOFailure)
		{
			Debug.LogError("BioFail!");
			return;
		}
		string text = param.m_ulSteamIDLobby.ToString();
		Debug.Log("Joined STeam Room: " + text + " joining Photon Room");
		JoinedThroughServerList = true;
		CurrentSelectedServer = new CSteamID(param.m_ulSteamIDLobby);
		PhotonNetwork.JoinOrCreateRoom(text, new RoomOptions
		{
			MaxPlayers = MainMenuHandler.MaxPlayers,
			CleanupCacheOnLeave = false
		}, null);
	}

	static ServerSearcher()
	{
		selectedRoom = "";
		JoinedThroughServerList = false;
		mCurrentSelectedServer = (CSteamID)0uL;
	}

	private void PopulateServerListWithRooms()
	{
		anim.state1 = true;
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		if (roomList == null || roomList.Length == 0)
		{
			anim.state1 = true;
			return;
		}
		anim.state1 = false;
		ClearServerList();
		for (int i = 0; i < roomList.Length; i++)
		{
			GameObject obj = Object.Instantiate(mServerCell, mServerListContent, true);
			obj.GetComponent<SetServerTextValues>().InitServerValues(new ServerCellParams
			{
				LobbyID = (CSteamID)0uL,
				LobbyName = roomList[i].Name,
				PlayersInside = roomList[i].PlayerCount.ToString(),
				PlayersMax = roomList[i].MaxPlayers
			});
			obj.SetActive(true);
		}
	}

	public void OnReceivedRoomListUpdate()
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		if (roomList == null || roomList.Length == 0)
		{
			anim.state1 = true;
			return;
		}
		anim.state1 = false;
		ClearServerList();
		for (int i = 0; i < roomList.Length; i++)
		{
			GameObject obj = Object.Instantiate(mServerCell, mServerListContent, true);
			obj.GetComponent<SetServerTextValues>().InitServerValues(new ServerCellParams
			{
				LobbyID = (CSteamID)0uL,
				LobbyName = roomList[i].Name,
				PlayersInside = roomList[i].PlayerCount.ToString(),
				PlayersMax = roomList[i].MaxPlayers
			});
			obj.SetActive(true);
		}
	}
}
