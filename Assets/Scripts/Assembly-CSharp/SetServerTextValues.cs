using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SetServerTextValues : MonoBehaviour
{
	private CSteamID mServerID;

	private string mServerNickName;

	private string playersInLobby;

	private int playersMax;

	[SerializeField]
	private Text mServerText;

	public CSteamID ServerID
	{
		get
		{
			return mServerID;
		}
	}

	public string ServerNickName
	{
		get
		{
			return mServerNickName;
		}
	}

	public void InitServerValues(ServerSearcher.ServerCellParams serverParams)
	{
		mServerID = serverParams.LobbyID;
		mServerNickName = serverParams.LobbyName;
		playersInLobby = serverParams.PlayersInside;
		playersMax = serverParams.PlayersMax;
		mServerText.text = mServerNickName + "      : " + playersInLobby + "/" + playersMax;
	}
}
