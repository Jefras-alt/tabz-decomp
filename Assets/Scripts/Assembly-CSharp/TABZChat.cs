using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TABZChat : MonoBehaviour
{
	private static bool mDisableInput;

	private GameObject mChatInput;

	private EventSystem mEventSystem;

	private InputField mInputField;

	private ChatPopup mChatPopup;

	private CodeStateAnimation mChatAnim;

	private bool mSubmittedThisFrame;

	private string[] mGangstaPhrases = new string[6] { "YO!", "yo", "Yo", "DAWG!", "word", "AIGHT!" };

	private PhotonView mPhotonView;

	private Curvimate mCurvimate;

	[SerializeField]
	private CurvimationTemplate mFadeInAnim;

	[SerializeField]
	private CurvimationTemplate mUpdateAnim;

	[SerializeField]
	private CurvimationTemplate mFadeOutAnim;

	private WeaponHandler mWeaponHandler;

	private bool grassToggle = true;

	public static bool cheatToggle;

	public static bool motionBlur;

	public static bool fogToggle;

	public static bool DisableInput
	{
		get
		{
			return mDisableInput;
		}
		set
		{
			mDisableInput = value;
		}
	}

	private CodeStateAnimation P_ChatAnim
	{
		get
		{
			if (mChatAnim == null)
			{
				mChatAnim = P_ChatInput.GetComponent<CodeStateAnimation>();
			}
			return mChatAnim;
		}
	}

	private Curvimate P_Curvimate
	{
		get
		{
			if (mCurvimate == null)
			{
				mCurvimate = GetComponentInChildren<Curvimate>();
			}
			return mCurvimate;
		}
	}

	private ChatPopup P_ChatPopup
	{
		get
		{
			if (mChatPopup == null)
			{
				mChatPopup = GetComponentInChildren<ChatPopup>();
			}
			return mChatPopup;
		}
	}

	private InputField P_InputField
	{
		get
		{
			if (mInputField == null)
			{
				mInputField = P_ChatInput.GetComponent<InputField>();
			}
			return mInputField;
		}
	}

	private GameObject P_ChatInput
	{
		get
		{
			if (mChatInput == null)
			{
				mChatInput = GetComponentInChildren<ChatInputFieldTAG>().gameObject;
			}
			return mChatInput;
		}
	}

	private WeaponHandler P_WeaponHandler
	{
		get
		{
			if (mWeaponHandler == null)
			{
				mWeaponHandler = base.transform.root.GetComponent<WeaponHandler>();
			}
			return mWeaponHandler;
		}
	}

	private EventSystem P_ES
	{
		get
		{
			return EventSystem.current;
		}
	}

	private void SelectChat()
	{
		if (!P_ES.alreadySelecting || !P_ES.currentSelectedGameObject.Equals(P_ChatInput))
		{
			ShowInput();
		}
	}

	private void Awake()
	{
		mPhotonView = GetComponent<PhotonView>();
		P_InputField.onEndEdit.AddListener(Enter);
		P_InputField.onValueChanged.AddListener(UpdateText);
	}

	private void Start()
	{
		if (mPhotonView.isMine)
		{
			HideInput();
		}
	}

	private void ShowInput()
	{
		P_Curvimate.PlayCurvimation(mFadeInAnim);
		P_ES.SetSelectedGameObject(null);
		P_ES.SetSelectedGameObject(P_ChatInput, null);
		mDisableInput = true;
	}

	public void UpdateText(string s)
	{
		P_Curvimate.PlayCurvimation(mUpdateAnim);
	}

	private void HideInput()
	{
		P_Curvimate.PlayCurvimation(mFadeOutAnim);
		P_ES.SetSelectedGameObject(null);
		mDisableInput = false;
	}

	private void Update()
	{
		if (!mPhotonView.isMine)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (P_Curvimate.P_ActiveTemplate != mFadeOutAnim)
			{
				HideInput();
			}
			return;
		}
		if (Input.GetKeyDown(KeyCode.Return) && !mSubmittedThisFrame)
		{
			SelectChat();
		}
		if (mSubmittedThisFrame)
		{
			mSubmittedThisFrame = false;
		}
	}

	private string Gangstify()
	{
		if (string.Equals(SteamFriends.GetPersonaName(), "brodal") || Random.Range(0, 100) >= 98)
		{
			return "BRAPP!";
		}
		return mGangstaPhrases[Random.Range(0, mGangstaPhrases.Length)];
	}

	private string RemoveHaxxFromString(string s)
	{
		string text = string.Empty;
		for (int i = 0; i < s.Length; i++)
		{
			char c = s[i];
			if (c != '<')
			{
				text += c;
			}
		}
		return text;
	}

	public void Enter(string s)
	{
		string text = P_InputField.text;
		if (text.StartsWith("/"))
		{
			text = text.ToLower();
			text = text.TrimStart('/');
			if (text == "minimap" && (bool)NetworkConnector.backgroundImage && (bool)NetworkConnector.markerImage)
			{
				NetworkConnector.backgroundImage.enabled = !NetworkConnector.backgroundImage.enabled;
				NetworkConnector.markerImage.enabled = !NetworkConnector.markerImage.enabled;
			}
			else if (text == "grass" && (bool)NetworkConnector.terrain)
			{
				ToggleGrass();
			}
			else if (text == "bluron" && (bool)NetworkConnector.terrain)
			{
				ToggleBlurOn();
			}
			else if (text == "bluroff" && (bool)NetworkConnector.terrain)
			{
				ToggleBlurOff();
			}
			else if (text == "fogon" && (bool)NetworkConnector.terrain)
			{
				ToggleFogOn();
			}
			else if (text == "fogoff" && (bool)NetworkConnector.terrain)
			{
				ToggleFogOff();
			}
		}
		else
		{
			text = RemoveHaxxFromString(text);
			if (P_WeaponHandler.ganstaMode)
			{
				text = text + " " + Gangstify();
			}
			Debug.Log("wrote " + text);
			if (text.Length > 0)
			{
				mPhotonView.RPC("SendChatMessage", PhotonTargets.Others, text);
				P_ChatPopup.ShowText(text);
			}
		}
		P_InputField.text = string.Empty;
		HideInput();
		mSubmittedThisFrame = true;
	}

	[PunRPC]
	public void SendChatMessage(string message)
	{
		P_ChatPopup.ShowText(message);
	}

	static TABZChat()
	{
		fogToggle = true;
	}

	private void ToggleCheatOff()
	{
		cheatToggle = false;
	}

	private void ToggleCheatOn()
	{
		cheatToggle = true;
	}

	private void ToggleBlurOff()
	{
		motionBlur = false;
	}

	private void ToggleBlurOn()
	{
		motionBlur = true;
	}

	private void ToggleFogOff()
	{
		fogToggle = false;
	}

	private void ToggleFogOn()
	{
		fogToggle = true;
	}

	private void ToggleGrass()
	{
		if (!NetworkConnector.terrain || NetworkConnector.origTallGrassMap == null || NetworkConnector.origNormalGrassMap == null)
		{
			return;
		}
		if (grassToggle)
		{
			int[,] detailLayer = NetworkConnector.terrain.terrainData.GetDetailLayer(0, 0, NetworkConnector.terrain.terrainData.detailWidth, NetworkConnector.terrain.terrainData.detailHeight, 0);
			for (int i = 0; i < NetworkConnector.terrain.terrainData.detailHeight; i++)
			{
				for (int j = 0; j < NetworkConnector.terrain.terrainData.detailWidth; j++)
				{
					detailLayer[j, i] = 0;
				}
			}
			int[,] detailLayer2 = NetworkConnector.terrain.terrainData.GetDetailLayer(0, 0, NetworkConnector.terrain.terrainData.detailWidth, NetworkConnector.terrain.terrainData.detailHeight, 1);
			for (int k = 0; k < NetworkConnector.terrain.terrainData.detailHeight; k++)
			{
				for (int l = 0; l < NetworkConnector.terrain.terrainData.detailWidth; l++)
				{
					detailLayer2[l, k] = 0;
				}
			}
			NetworkConnector.terrain.terrainData.SetDetailLayer(0, 0, 0, detailLayer);
			NetworkConnector.terrain.terrainData.SetDetailLayer(0, 0, 1, detailLayer2);
			grassToggle = false;
		}
		else
		{
			NetworkConnector.terrain.terrainData.SetDetailLayer(0, 0, 0, NetworkConnector.origNormalGrassMap);
			NetworkConnector.terrain.terrainData.SetDetailLayer(0, 0, 1, NetworkConnector.origTallGrassMap);
			grassToggle = true;
		}
	}
}
