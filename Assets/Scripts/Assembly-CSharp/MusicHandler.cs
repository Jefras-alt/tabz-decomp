using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
	private const string MUSIC_STATE_PARAM = "MusicState";

	private static EventInstance mMusicEvent;

	private int mState;

	public void StartMusic()
	{
		if (mMusicEvent != null)
		{
			mMusicEvent.release();
		}
		string musicEvent = GetComponent<SoundEventsManager>().MusicEvent;
		mMusicEvent = RuntimeManager.CreateInstance(musicEvent);
		mMusicEvent.setParameterValue("MusicState", mState);
		mMusicEvent.start();
	}

	public void ChangeMusicState(int newState)
	{
		if (newState != mState)
		{
			mMusicEvent.setParameterValue("MusicState", newState);
		}
	}

	public static void OnGameStarted()
	{
		StopMusic();
	}

	public static void StopMusic()
	{
		if (!(mMusicEvent == null))
		{
			mMusicEvent.stop(STOP_MODE.ALLOWFADEOUT);
			mMusicEvent.release();
		}
	}
}
