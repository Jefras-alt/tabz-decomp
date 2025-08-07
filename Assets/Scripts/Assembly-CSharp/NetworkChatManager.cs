using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkChatManager : MonoBehaviour
{
	public class NetworkFeedMessage
	{
		private string mMessage;

		private float mTimeAlive;

		private float mTimeLeft;

		public string Message
		{
			get
			{
				return mMessage;
			}
		}

		public float TimeLeft
		{
			get
			{
				return mTimeLeft;
			}
		}

		public NetworkFeedMessage(float startTime, string message)
		{
			mTimeAlive = startTime;
			mTimeLeft = mTimeAlive;
			mMessage = message;
		}

		public void Tick()
		{
			mTimeLeft -= Time.unscaledDeltaTime;
		}
	}

	private const float TIME_MESSAGE_ALIVE = 5f;

	private Text mFeedText;

	private List<NetworkFeedMessage> mMessages = new List<NetworkFeedMessage>();

	private void Start()
	{
		mFeedText = Object.FindObjectOfType<FeedTextTAG>().GetComponent<Text>();
	}

	private void Update()
	{
		TickMessages();
	}

	private void TickMessages()
	{
		for (ushort num = 0; num < mMessages.Count; num++)
		{
			NetworkFeedMessage networkFeedMessage = mMessages[num];
			networkFeedMessage.Tick();
			if (networkFeedMessage.TimeLeft <= 0f)
			{
				mMessages.Remove(networkFeedMessage);
				UpdateFeed();
			}
		}
	}

	[PunRPC]
	public void AddFeedMessage(string message)
	{
		NetworkFeedMessage item = new NetworkFeedMessage(5f, message);
		mMessages.Add(item);
		UpdateFeed();
	}

	private void UpdateFeed()
	{
		mFeedText.text = string.Empty;
		for (ushort num = 0; num < mMessages.Count; num++)
		{
			NetworkFeedMessage networkFeedMessage = mMessages[num];
			Text text = mFeedText;
			text.text = text.text + networkFeedMessage.Message + "\n";
		}
	}
}
