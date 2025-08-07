using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FMOD.Studio;
using FMODUnity;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
	[Serializable]
	public class RandomServerNameTier
	{
		public string[] values;
	}

	private static CSteamID mCurrentSteamLobby;

	private static string mVersionString;

	private static string PLAYER_NAME_KEY;

	private static string PLAYER_COLOR_KEY;

	[SerializeField]
	private List<RandomServerNameTier> mRandomLobbyTexts;

	private static byte mMaxPlayers;

	[SerializeField]
	private Button m_QuickMatchButton;

	[SerializeField]
	private Button m_QuitButton;

	[SerializeField]
	private Button m_OptionsButton;

	[SerializeField]
	private Button m_ServerListButton;

	[SerializeField]
	private Text m_PlayerNameText;

	[SerializeField]
	private Text m_TotalPlayersText;

	[SerializeField]
	private GameObject m_ServerListObject;

	[SerializeField]
	private GameObject m_EnterNamePrompt;

	[SerializeField]
	private Toggle[] m_mainColorToggles;

	[SerializeField]
	private Toggle[] m_promptColorToggles;

	private CodeStateAnimation m_MenuButtonCodeAnimation;

	private CodeStateAnimation m_PromptCodeAnimation;

	private MenuTimeHandler mTimeHandler;

	private EventInstance mClickSound;

	private static Image mFadeToBlack;

	private static bool mStartTicking;

	private const float TIME_OUT_AFTER = 10f;

	private static float mTimeOutTimer;

	private const string FIRT_TIME_TAG = "FirstTime";

	protected Callback<GameLobbyJoinRequested_t> m_LobbyJoinRequest;

	protected Callback<LobbyInvite_t> m_LobbyInvite;

	protected CallResult<LobbyCreated_t> mLobbyCreated;

	protected CallResult<LobbyEnter_t> mLobbyEntered;

	protected CallResult<LobbyMatchList_t> mLobbyListRecieved;

	public static Color[] colors;

	public static int colorIndex;

	public static RoomInfo[] roomInfos;

	private bool once = true;

	public static CSteamID CURRENT_STEAM_LOBBY
	{
		get
		{
			return mCurrentSteamLobby;
		}
	}

	public static string VERSION_NUMBER
	{
		get
		{
			return mVersionString;
		}
	}

	public static string SERVER_NAME_PARAM
	{
		get
		{
			return "Name";
		}
	}

	public static string SERVER_VERSION_PARAM
	{
		get
		{
			return "Version";
		}
	}

	public static string SERVER_PLAYERS_PARAM
	{
		get
		{
			return "Players";
		}
	}

	public static byte MaxPlayers
	{
		get
		{
			return mMaxPlayers;
		}
	}

	public static bool IS_RED
	{
		get
		{
			return PlayerPrefs.GetInt(PLAYER_COLOR_KEY, 0) == 0;
		}
		set
		{
			PlayerPrefs.SetInt(PLAYER_COLOR_KEY, (!value) ? 1 : 0);
		}
	}

	private bool FirstTime
	{
		get
		{
			return PlayerPrefs.GetInt("FirstTime", 0) == 0;
		}
		set
		{
			PlayerPrefs.SetInt("FirstTime", 1);
		}
	}

	public string PlayerName
	{
		get
		{
			if (!File.Exists("name.txt"))
			{
				File.WriteAllText("name.txt", "DefaultName");
			}
			return File.ReadAllText("name.txt");
		}
	}

	public MainMenuHandler()
	{
		mRandomLobbyTexts = new List<RandomServerNameTier>();
	}

	private void Awake()
	{
		if (!PhotonNetwork.connected)
		{
			PhotonNetwork.ConnectUsingSettings(VERSION_NUMBER);
		}
		PhotonNetwork.autoJoinLobby = true;
		PhotonNetwork.player.NickName = PlayerName;
		Toggle[] array = new Toggle[colors.Length];
		array[0] = m_mainColorToggles[0];
		array[1] = m_mainColorToggles[1];
		m_mainColorToggles = array;
		Vector3 position3 = m_mainColorToggles[1].transform.position;
		Vector3 position = array[0].transform.position;
		Vector3 vector = position - array[1].transform.position;
		for (int i = 2; i < colors.Length; i++)
		{
			Vector3 position2 = position + vector * ((float)i + 1f);
			position2 += array[0].transform.TransformDirection(Vector3.down) * 0.1f;
			Toggle component = UnityEngine.Object.Instantiate(m_mainColorToggles[0].gameObject, position2, array[0].transform.rotation, m_mainColorToggles[0].transform.parent).GetComponent<Toggle>();
			component.enabled = true;
			component.interactable = true;
			component.group = m_mainColorToggles[0].group;
			component.colors = new ColorBlock
			{
				colorMultiplier = 1f,
				disabledColor = colors[i],
				highlightedColor = colors[i],
				normalColor = colors[i],
				pressedColor = colors[i]
			};
			m_mainColorToggles[i] = component;
		}
		Time.timeScale = 0f;
		mTimeHandler = UnityEngine.Object.FindObjectOfType<MenuTimeHandler>();
		mStartTicking = false;
		mFadeToBlack = UnityEngine.Object.FindObjectOfType<FadeToBlackTAG>().GetComponent<Image>();
		if (FirstTime)
		{
			m_PlayerNameText.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			mTimeHandler.Done();
		}
		m_QuickMatchButton.onClick.AddListener(OnQuickMatchClicked);
		m_QuitButton.onClick.AddListener(OnQuitClicked);
		m_ServerListButton.onClick.AddListener(OnServerListClicked);
		m_QuickMatchButton.onClick.AddListener(OnClickEvent);
		m_QuitButton.onClick.AddListener(OnClickEvent);
		m_ServerListButton.onClick.AddListener(OnClickEvent);
		int num = PlayerPrefs.GetInt(PLAYER_COLOR_KEY, 0);
		m_mainColorToggles[num].isOn = true;
		m_promptColorToggles[num].isOn = true;
		m_PromptCodeAnimation = m_EnterNamePrompt.GetComponent<CodeStateAnimation>();
		m_MenuButtonCodeAnimation = m_QuickMatchButton.GetComponentInParent<CodeStateAnimation>();
		string menuClick = UnityEngine.Object.FindObjectOfType<SoundEventsManager>().MenuClick;
		if (!string.IsNullOrEmpty(menuClick))
		{
			mClickSound = RuntimeManager.CreateInstance(menuClick);
		}
	}

	private void OnClickEvent()
	{
		if (!(mClickSound == null))
		{
			mClickSound.start();
		}
	}

	private void OnQuitClicked()
	{
		Application.Quit();
	}

	private void Start()
	{
		m_PlayerNameText.text = PlayerName;
		if (FirstTime)
		{
			PromptForPlayerName();
		}
	}

	private void OnEnable()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		mLobbyCreated = CallResult<LobbyCreated_t>.Create(OnLobbyCreatedCallresult);
		mLobbyEntered = CallResult<LobbyEnter_t>.Create(OnLobbyEnteredCallResult);
		mLobbyListRecieved = CallResult<LobbyMatchList_t>.Create(OnLobbyMatchList);
		m_LobbyInvite = Callback<LobbyInvite_t>.Create(OnLobbyInvite);
		m_LobbyJoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyRequest);
		PhotonNetwork.playerName = PlayerName;
	}

	private void OnGameLobbyRequest(GameLobbyJoinRequested_t param)
	{
		Debug.Log("Game lobby request!");
		SteamAPICall_t hAPICall = SteamMatchmaking.JoinLobby(param.m_steamIDLobby);
		mLobbyEntered.Set(hAPICall);
	}

	private void OnLobbyInvite(LobbyInvite_t param)
	{
	}

	private void OnLobbyMatchList(LobbyMatchList_t param, bool bIOFailure)
	{
		if (bIOFailure)
		{
			Debug.Log("Biofail!");
			return;
		}
		if (param.m_nLobbiesMatching == 0)
		{
			Debug.Log("No lobbies was found: Hosting random steam server");
			HostRandomSteamLobby();
			return;
		}
		List<CSteamID> list = new List<CSteamID>();
		for (int i = 0; i < param.m_nLobbiesMatching; i++)
		{
			CSteamID lobbyByIndex = SteamMatchmaking.GetLobbyByIndex(i);
			if (IsLobbyValid(lobbyByIndex))
			{
				list.Add(lobbyByIndex);
			}
		}
		if (list.Count == 0)
		{
			Debug.Log("No lobbies was found: Hosting random steam server");
			HostRandomSteamLobby();
			return;
		}
		int index = UnityEngine.Random.Range(0, list.Count);
		CSteamID cSteamID = list[index];
		string text = cSteamID.ToString();
		Debug.Log("Got Steam Match list, Joining random server: " + text);
		ServerSearcher.CurrentSelectedServer = cSteamID;
		SteamMatchmaking.JoinLobby(cSteamID);
		PhotonNetwork.JoinOrCreateRoom(text, new RoomOptions
		{
			MaxPlayers = mMaxPlayers,
			CleanupCacheOnLeave = false
		}, null);
	}

	private bool IsLobbyValid(CSteamID currLobby)
	{
		string lobbyData = SteamMatchmaking.GetLobbyData(currLobby, SERVER_VERSION_PARAM);
		bool flag = lobbyData.Equals(VERSION_NUMBER) || (lobbyData == string.Empty && VERSION_NUMBER == "STEAM TABZ v1.1");
		return currLobby.IsValid() && flag;
	}

	private void OnLobbyEnteredCallResult(LobbyEnter_t param, bool bIOFailure)
	{
		if (bIOFailure)
		{
			Debug.LogError("Bio fail!! LobbyEntered");
			return;
		}
		CSteamID steamIDLobby = new CSteamID(param.m_ulSteamIDLobby);
		string text = steamIDLobby.m_SteamID.ToString();
		Debug.Log("Got Steam Lobby from Friend " + text);
		SteamMatchmaking.JoinLobby(steamIDLobby);
		PhotonNetwork.JoinOrCreateRoom(text, new RoomOptions
		{
			MaxPlayers = mMaxPlayers,
			CleanupCacheOnLeave = false
		}, null);
	}

	private void OnLobbyCreatedCallresult(LobbyCreated_t param, bool bIOFailure)
	{
		if (bIOFailure)
		{
			Debug.LogError("Bio fail!! LobbyCreated");
			return;
		}
		Debug.Log("Hosted Steam Room! " + param.m_eResult.ToString() + " : " + param.m_ulSteamIDLobby);
		if (param.m_eResult != EResult.k_EResultOK)
		{
			Debug.LogError("Host Steam Lobby result is not OK: Exiting...");
			return;
		}
		CSteamID cSteamID = new CSteamID(param.m_ulSteamIDLobby);
		string pchValue = RecieveRandomLobbyName();
		SteamMatchmaking.SetLobbyData(cSteamID, SERVER_NAME_PARAM, pchValue);
		SteamMatchmaking.SetLobbyData(cSteamID, SERVER_VERSION_PARAM, mVersionString);
		SteamMatchmaking.SetLobbyData(cSteamID, SERVER_PLAYERS_PARAM, "1");
		Debug.Log(string.Concat("PLayers in room: ", cSteamID, " : ", SteamMatchmaking.GetLobbyData(cSteamID, SERVER_PLAYERS_PARAM)));
		ServerSearcher.CurrentSelectedServer = cSteamID;
		PhotonNetwork.CreateRoom(param.m_ulSteamIDLobby.ToString(), new RoomOptions
		{
			MaxPlayers = mMaxPlayers,
			CleanupCacheOnLeave = false
		}, null);
	}

	private string RecieveRandomLobbyName()
	{
		string text = string.Empty;
		for (int i = 0; i < mRandomLobbyTexts.Count; i++)
		{
			int num = UnityEngine.Random.Range(0, mRandomLobbyTexts[i].values.Length);
			string text2 = mRandomLobbyTexts[i].values[num];
			text = text + text2 + " ";
		}
		return text;
	}

	private void Update()
	{
		m_TotalPlayersText.text = ((!PhotonNetwork.connected) ? "Connecting..." : ("Players Connected: " + Mathf.Clamp(PhotonNetwork.countOfPlayers, 1, int.MaxValue)));
		if (mStartTicking)
		{
			TickTimeOutTimer();
		}
	}

	private void TickTimeOutTimer()
	{
		mTimeOutTimer -= Time.unscaledDeltaTime;
		Debug.Log("Ticking time out timer! " + mTimeOutTimer);
		if (mTimeOutTimer < 0f)
		{
			Debug.Log("TIME OUT!");
			FadeToBlack(false);
		}
	}

	public virtual void OnConnectedToMaster()
	{
		Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
	}

	public virtual void OnJoinedLobby()
	{
		Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
	}

	public virtual void OnPhotonRandomJoinFailed()
	{
		Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
		PhotonNetwork.JoinOrCreateRoom("pew pew land", new RoomOptions
		{
			PlayerTtl = 30,
			CleanupCacheOnLeave = false
		}, null);
	}

	private void HostRandomSteamLobby()
	{
		SteamAPICall_t hAPICall = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, mMaxPlayers);
		mLobbyCreated.Set(hAPICall);
	}

	public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		Debug.LogError("Cause: " + cause);
	}

	public void OnJoinedRoom()
	{
		Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
		Debug.Log("JOINED ROOM: " + PhotonNetwork.room.MaxPlayers);
		IS_RED = m_mainColorToggles[0].isOn;
		for (int i = 0; i < m_mainColorToggles.Length; i++)
		{
			if (m_mainColorToggles[i].isOn)
			{
				colorIndex = i;
			}
		}
		PhotonNetwork.LoadLevel(1);
	}

	private void OnQuickMatchClicked()
	{
		Debug.Log("Quick Match Clicked!");
		if (!PhotonNetwork.connected)
		{
			return;
		}
		FadeToBlack(true);
		IS_RED = m_mainColorToggles[0].isOn;
		for (int i = 0; i < m_mainColorToggles.Length; i++)
		{
			if (m_mainColorToggles[i].isOn)
			{
				colorIndex = i;
			}
		}
		PhotonNetwork.JoinRandomRoom();
	}

	private void OnServerListClicked()
	{
		Debug.Log("serverlist clicked!");
		if (PhotonNetwork.connected)
		{
			m_ServerListObject.SetActive(!m_ServerListObject.activeInHierarchy);
		}
	}

	private void OnPromptOkClicked()
	{
		mTimeHandler.Done();
		IS_RED = m_promptColorToggles[0].isOn;
		FirstTime = false;
		m_EnterNamePrompt.SetActive(false);
	}

	private void OnPromptCancelClicked()
	{
		SetMenuButtonsActive(true);
	}

	private void JoinRandomRoom()
	{
		SteamAPICall_t hAPICall = SteamMatchmaking.RequestLobbyList();
		mLobbyListRecieved.Set(hAPICall);
	}

	private void PromptForPlayerName()
	{
		Button[] componentsInChildren = m_EnterNamePrompt.GetComponentsInChildren<Button>(true);
		foreach (Button button in componentsInChildren)
		{
			if (button.name == "Done")
			{
				button.onClick.AddListener(OnPromptOkClicked);
			}
			else if (button.name == "Cancel")
			{
				button.onClick.AddListener(OnPromptCancelClicked);
			}
			else
			{
				Debug.LogError("Button with invalid name: " + button.name);
			}
		}
		SetMenuButtonsActive(false);
	}

	private void SetMenuButtonsActive(bool active)
	{
		m_MenuButtonCodeAnimation.state1 = active;
		if (!m_EnterNamePrompt.activeInHierarchy)
		{
			m_EnterNamePrompt.SetActive(!active);
		}
		else
		{
			m_PromptCodeAnimation.state1 = !active;
		}
		if (!active && m_PlayerNameText.gameObject.activeInHierarchy)
		{
			m_PlayerNameText.transform.parent.GetComponent<CodeStateAnimation>().state1 = false;
		}
		Debug.Log("Setting Menubuttons: " + active);
	}

	public void FadeToBlack(bool black)
	{
		if (black)
		{
			Debug.Log("Fade to black!");
			StartCoroutine(FadeToValue(mFadeToBlack.color, new Color(mFadeToBlack.color.r, mFadeToBlack.color.g, mFadeToBlack.color.b, 255f), 1f, true));
			mTimeOutTimer = 10f;
		}
		else
		{
			Debug.Log("Fade to white!");
			StartCoroutine(FadeToValue(mFadeToBlack.color, new Color(mFadeToBlack.color.r, mFadeToBlack.color.g, mFadeToBlack.color.b, 0f), 1f, false));
		}
	}

	private IEnumerator FadeToValue(Color startColor, Color endColor, float duration, bool startTick)
	{
		float start = Time.unscaledTime;
		float elapsed = 0f;
		while (elapsed < duration)
		{
			elapsed = Time.unscaledTime - start;
			float t = Mathf.Clamp(elapsed / duration, 0f, 1f);
			mFadeToBlack.color = Color.Lerp(startColor, endColor, t);
			yield return new WaitForEndOfFrame();
		}
		mStartTicking = startTick;
	}

	static MainMenuHandler()
	{
		mVersionString = "TABZ v1.21";
		PLAYER_NAME_KEY = "Name";
		PLAYER_COLOR_KEY = "Color";
		mMaxPlayers = 10;
		colors = new Color[14]
		{
			new Color(0.574f, 0.257f, 0.257f, 1f),
			new Color(0.257f, 0.364f, 0.574f, 1f),
			new Color(1f, 0.8431f, 0f, 1f),
			Color.blue,
			Color.red,
			new Color(0.545f, 0.27f, 0.074f, 1f),
			new Color(0.965f, 0.671f, 0.741f, 1f),
			Color.black,
			Color.cyan,
			Color.green,
			Color.grey,
			Color.magenta,
			Color.yellow,
			Color.white
		};
	}

	public void OnReceivedRoomListUpdate()
	{
		roomInfos = PhotonNetwork.GetRoomList();
	}
}
