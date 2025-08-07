using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class ChatPopup : MonoBehaviour
{
	private Image mChatBubble;

	private Text mChatText;

	[SerializeField]
	private CurvimationTemplate mFadeInAnim;

	[SerializeField]
	private CurvimationTemplate mRefreshAnim;

	[SerializeField]
	private CurvimationTemplate mFadeOutAnim;

	private Curvimate mCurvimate;

	private float mlastMessageTime = float.MinValue;

	private EventInstance mTalkQuietSoundEvent;

	private EventInstance mTalkLoudSoundEvent;

	[Range(0f, 10f)]
	public float mFadeTime;

	private NoiseSpawner mTalkDirtyToMe;

	private NoiseSpawner mTalkVeryDirtyToMe;

	private NoiseSpawner P_TalkDirtyToMe
	{
		get
		{
			if (mTalkDirtyToMe == null)
			{
				mTalkDirtyToMe = P_ChatBubble.gameObject.transform.Find("TalkDirtyToMe").GetComponent<NoiseSpawner>();
			}
			return mTalkDirtyToMe;
		}
	}

	private NoiseSpawner P_TalkVeryDirtyToMe
	{
		get
		{
			if (mTalkVeryDirtyToMe == null)
			{
				mTalkVeryDirtyToMe = P_ChatBubble.gameObject.transform.Find("TalkVeryDirtyToMe").GetComponent<NoiseSpawner>();
			}
			if (mTalkDirtyToMe == null)
			{
				Debug.Log("Lägg till noiseSpawners under chat popup");
			}
			return mTalkVeryDirtyToMe;
		}
	}

	private Image P_ChatBubble
	{
		get
		{
			if (mChatBubble == null)
			{
				mChatBubble = GetComponentInChildren<Image>();
			}
			return mChatBubble;
		}
	}

	private Text P_ChatText
	{
		get
		{
			if (mChatText == null)
			{
				mChatText = GetComponentInChildren<Text>();
			}
			return mChatText;
		}
	}

	private Curvimate P_Curvimate
	{
		get
		{
			if (mCurvimate == null)
			{
				mCurvimate = P_ChatBubble.GetComponent<Curvimate>();
			}
			return mCurvimate;
		}
	}

	private void Awake()
	{
		SoundEventsManager soundEventsManager = Object.FindObjectOfType<SoundEventsManager>();
		if (!string.IsNullOrEmpty(soundEventsManager.TalkQuiet))
		{
			mTalkQuietSoundEvent = RuntimeManager.CreateInstance(soundEventsManager.TalkQuiet);
		}
		if (!string.IsNullOrEmpty(soundEventsManager.TalkLoudEvent))
		{
			mTalkLoudSoundEvent = RuntimeManager.CreateInstance(soundEventsManager.TalkLoudEvent);
		}
	}

	private void Start()
	{
	}

	private void FadeInMessage()
	{
		P_Curvimate.PlayCurvimation(mFadeInAnim);
		Debug.Log("InMessage");
	}

	private void UpdateMessage()
	{
		P_Curvimate.PlayCurvimation(mRefreshAnim);
		Debug.Log("UpdateMessage");
	}

	private void FadeMessage()
	{
		if (P_Curvimate.P_ActiveTemplate != mFadeOutAnim)
		{
			P_Curvimate.PlayCurvimation(mFadeOutAnim);
			Debug.Log("FadeMessage");
		}
	}

	private static bool CheckUpper(string input, float percent = 0.5f)
	{
		int num = 0;
		for (int i = 0; i < input.Length; i++)
		{
			if (char.IsUpper(input[i]))
			{
				num++;
			}
		}
		if ((float)num / (float)input.Length >= percent)
		{
			return true;
		}
		return false;
	}

	public void ShowText(string text)
	{
		P_ChatText.text = text;
		if (CheckUpper(text))
		{
			P_Curvimate.P_Multiplier = 2f;
			P_TalkVeryDirtyToMe.Trigger();
			TalkLoud();
		}
		else
		{
			P_Curvimate.P_Multiplier = 1f;
			P_TalkDirtyToMe.Trigger();
			TalkQuiet();
		}
		if (Time.timeSinceLevelLoad - mlastMessageTime < mFadeTime)
		{
			mlastMessageTime = Time.timeSinceLevelLoad;
			UpdateMessage();
		}
		else
		{
			mlastMessageTime = Time.timeSinceLevelLoad;
			FadeInMessage();
		}
	}

	private void Update()
	{
		if (Time.timeSinceLevelLoad - mlastMessageTime >= mFadeTime)
		{
			FadeMessage();
		}
	}

	private void TalkLoud()
	{
		mTalkLoudSoundEvent.set3DAttributes(base.transform.To3DAttributes());
		mTalkLoudSoundEvent.start();
	}

	private void TalkQuiet()
	{
		mTalkQuietSoundEvent.set3DAttributes(base.transform.To3DAttributes());
		mTalkQuietSoundEvent.start();
	}
}
