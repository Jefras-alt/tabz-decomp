using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuHandler : MonoBehaviour
{
	[SerializeField]
	private GameObject mPauseScreenObject;

	private static bool mIsPaused;

	public static bool IsPaused
	{
		get
		{
			return mIsPaused;
		}
	}

	private void Start()
	{
		mIsPaused = false;
	}

	private void Update()
	{
		CheckInput();
	}

	private void CheckInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			mIsPaused = !mIsPaused;
			OnPauseValueChanged();
		}
	}

	private void OnPauseValueChanged()
	{
		TABZChat.DisableInput = mIsPaused;
		mPauseScreenObject.SetActive(mIsPaused);
		Cursor.visible = mIsPaused;
		Cursor.lockState = ((!mIsPaused) ? CursorLockMode.Locked : CursorLockMode.None);
	}

	public void OnQuitClicked()
	{
		if (!PhotonNetwork.LeaveRoom())
		{
			PhotonNetwork.Disconnect();
		}
		SceneManager.LoadScene(0);
		SteamMatchmaking.LeaveLobby(ServerSearcher.CurrentSelectedServer);
	}

	public void OnResumeClicked()
	{
		mIsPaused = false;
		OnPauseValueChanged();
	}

	static PauseMenuHandler()
	{
	}
}
