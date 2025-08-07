using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class NetworkStatistics : MonoBehaviour
{
	private static Dictionary<string, uint> mMessageSentDictionary = new Dictionary<string, uint>();

	private static Dictionary<string, uint> mMessageSentDictionaryTotal = new Dictionary<string, uint>();

	private float mInterval = 1f;

	private float mTimer;

	private uint mSeconds;

	private static uint mMessageSentCountLastInterval = 0u;

	private static uint mMessageSentCountTotal = 0u;

	public Rect statsRect = new Rect(0f, 100f, 200f, 500f);

	public int WindowId = 100;

	private bool mShowStats;

	private string totalMessages;

	private string messagesLastInterval;

	private string avgMessageCount;

	private string detailedStatistics;

	private string detailedStatisticsTotal;

	private string networkTime;

	private string serverName;

	private string photonRoom;

	private string steamRoom;

	private string spawnedZombies;

	private void Awake()
	{
		mTimer = mInterval;
	}

	private void Start()
	{
		serverName = SteamMatchmaking.GetLobbyData(ServerSearcher.CurrentSelectedServer, MainMenuHandler.SERVER_NAME_PARAM);
		steamRoom = ServerSearcher.CurrentSelectedServer.ToString();
		photonRoom = PhotonNetwork.room.Name;
	}

	private void Update()
	{
		spawnedZombies = NetworkZombieSpawner.SpawnedZombies.ToString();
		TickTimer();
		if (Input.GetKeyDown(KeyCode.Q))
		{
			mShowStats = !mShowStats;
		}
	}

	private void TickTimer()
	{
		mTimer -= Time.deltaTime;
		if (mTimer < 0f)
		{
			PrintMessagesLastInterval();
			PrintDetailedStatistics();
			ResetMessages();
			ResetTimer();
		}
	}

	public static void MessageSent(string methodName)
	{
		mMessageSentCountLastInterval++;
		mMessageSentCountTotal++;
		if (!mMessageSentDictionary.ContainsKey(methodName))
		{
			mMessageSentDictionary.Add(methodName, 1u);
		}
		else
		{
			mMessageSentDictionary[methodName]++;
		}
	}

	private void PrintMessagesLastInterval()
	{
		messagesLastInterval = mMessageSentCountLastInterval.ToString();
		avgMessageCount = ((float)mMessageSentCountTotal / Mathf.Clamp(mSeconds, 1f, 4.2949673E+09f)).ToString();
		totalMessages = mMessageSentCountTotal.ToString();
	}

	private void PrintDetailedStatistics()
	{
		string text = string.Empty;
		string text2 = string.Empty;
		foreach (KeyValuePair<string, uint> item in mMessageSentDictionary)
		{
			if (!mMessageSentDictionaryTotal.ContainsKey(item.Key))
			{
				mMessageSentDictionaryTotal.Add(item.Key, item.Value);
			}
			else
			{
				mMessageSentDictionaryTotal[item.Key] += item.Value;
			}
			string text3 = text2;
			text2 = text3 + "TYPE: " + item.Key + "SENT: " + item.Value + " Times!\n";
		}
		foreach (KeyValuePair<string, uint> item2 in mMessageSentDictionaryTotal)
		{
			string text3 = text;
			text = text3 + "TYPE: " + item2.Key + "SENT: " + item2.Value + " Times!\n";
		}
		detailedStatistics = text2;
		detailedStatisticsTotal = text;
	}

	private void ResetMessages()
	{
		mMessageSentCountLastInterval = 0u;
		mMessageSentDictionary.Clear();
	}

	private void ResetTimer()
	{
		mTimer = mInterval;
		mSeconds++;
	}

	public void OnGUI()
	{
		if (mShowStats)
		{
			statsRect = GUILayout.Window(WindowId, statsRect, StatsWindow, "Toggle (Q)");
		}
	}

	private void StatsWindow(int window)
	{
		GUILayout.Label("SERVER: " + serverName);
		GUILayout.Label("Photon Room: " + photonRoom);
		GUILayout.Label("steamRoom: " + steamRoom);
		GUILayout.Label("Zombies: " + spawnedZombies);
		GUILayout.Label("AVG msg/s: " + avgMessageCount);
		GUILayout.Space(5f);
		string text = "Players in Room: " + PhotonNetwork.room.PlayerCount;
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		foreach (PhotonPlayer photonPlayer in playerList)
		{
			string text2 = ((!string.IsNullOrEmpty(photonPlayer.NickName)) ? photonPlayer.NickName : "[NO NAME]");
			text = text + "\n" + text2;
		}
		GUILayout.Label("[PLAYERS]\n" + text);
	}
}
