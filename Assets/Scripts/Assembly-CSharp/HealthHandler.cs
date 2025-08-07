using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using Photon;
using UnityEngine;
using UnityEngine.UI;

public class HealthHandler : PunBehaviour
{
	private const string FMOD_IS_ZOMBIE = "Zombie";

	public float MaxHealth = 100f;

	[HideInInspector]
	public float currentHealth;

	public UnityEngine.MonoBehaviour[] dontDisableTheseScipts;

	public GameObject[] dontDisableTheSciptsOnThisObject;

	public bool dead;

	private PhotonView mPhotonView;

	private FeedbackHandler feedbackHandler;

	private bool mIsAi;

	[SerializeField]
	private Transform mHipRigidBody;

	private const string BREATH_PARAM = "amountDamage";

	private EventInstance mDamageEvent;

	private EventInstance mDeathEvent;

	private EventInstance mBreathSoundEvent;

	private GooglyEye[] eyes;

	private Rigidbody mMainRig;

	public Text healthText;

	private static SoundEventsManager mSoundEventHandler;

	private void Awake()
	{
		mPhotonView = GetComponent<PhotonView>();
		mIsAi = GetComponent<PlayerHandler>().AI;
		feedbackHandler = GetComponent<FeedbackHandler>();
		if (mSoundEventHandler == null)
		{
			mSoundEventHandler = UnityEngine.Object.FindObjectOfType<SoundEventsManager>();
		}
		string text = ((!mIsAi) ? mSoundEventHandler.DamageEvent : mSoundEventHandler.ZombieDamageEvent);
		if (!string.IsNullOrEmpty(text))
		{
			mDamageEvent = RuntimeManager.CreateInstance(text);
		}
		string text2 = ((!mIsAi) ? string.Empty : mSoundEventHandler.ZombieDeathEvent);
		if (!string.IsNullOrEmpty(text2))
		{
			mDeathEvent = RuntimeManager.CreateInstance(text2);
		}
		string breathEvent = mSoundEventHandler.BreathEvent;
		if (!string.IsNullOrEmpty(breathEvent) && !mIsAi && mPhotonView.isMine)
		{
			mBreathSoundEvent = RuntimeManager.CreateInstance(breathEvent);
			mBreathSoundEvent.start();
		}
		eyes = GetComponentsInChildren<GooglyEye>();
		mMainRig = base.transform.root.GetComponent<PhysicsAmimationController>().mainRig;
	}

	private void Start()
	{
		currentHealth = MaxHealth;
		ApplyNewBreathParameter();
	}

	private void Update()
	{
		if (!mIsAi && currentHealth < MaxHealth)
		{
			currentHealth += Time.deltaTime * 0.3f;
			if (mPhotonView.isMine)
			{
				healthText.text = currentHealth.ToString("F0") + " HP";
				ApplyNewBreathParameter();
			}
		}
		if (mMainRig.position.y < -25f)
		{
			Die(true);
		}
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		if (mPhotonView.isMine)
		{
			base.OnPhotonPlayerConnected(newPlayer);
			mPhotonView.RPC("RecieveHealth", newPlayer, currentHealth);
		}
	}

	[PunRPC]
	public void RecieveHealth(float health)
	{
		currentHealth = health;
		TakeDamage(0f);
	}

	[PunRPC]
	public void TakeDamage(float dmg, PhotonPlayer player = null, bool killingBlow = false)
	{
		for (int i = 0; i < eyes.Length; i++)
		{
			eyes[i].TakeDamage(dmg * 0.02f);
		}
		currentHealth -= dmg;
		ApplyNewBreathParameter();
		if (currentHealth <= 0f || killingBlow)
		{
			Die();
		}
		if (mIsAi || (!mIsAi && mPhotonView.isMine))
		{
			PlayDamageSound();
		}
		if (!mIsAi && mPhotonView.isMine)
		{
			feedbackHandler.DamageFeedback(dmg);
		}
	}

	private void ApplyNewBreathParameter()
	{
		if (!(mBreathSoundEvent == null) && (mIsAi || mPhotonView.isMine) && !mIsAi)
		{
			float value = (1f - currentHealth / 100f) * 0.5f;
			mBreathSoundEvent.setParameterValue("amountDamage", value);
		}
	}

	private void PlayDamageSound()
	{
		if (!(mDamageEvent == null))
		{
			mDamageEvent.set3DAttributes(mHipRigidBody.To3DAttributes());
			mDamageEvent.start();
		}
	}

	private void OnDestroy()
	{
		if (mDamageEvent != null)
		{
			mDamageEvent.release();
		}
		if (mBreathSoundEvent != null)
		{
			mBreathSoundEvent.release();
		}
	}

	private void Die(bool destroyItems = false)
	{
		if (dead)
		{
			return;
		}
		dead = true;
		StopBreath();
		for (int i = 0; i < eyes.Length; i++)
		{
			eyes[i].Die();
		}
		if (mPhotonView.isMine && !mIsAi)
		{
			feedbackHandler.DamageFeedback(60f);
		}
		PlayDeathSound();
		UnityEngine.MonoBehaviour[] componentsInChildren = GetComponentsInChildren<UnityEngine.MonoBehaviour>();
		foreach (UnityEngine.MonoBehaviour monoBehaviour in componentsInChildren)
		{
			bool flag = true;
			UnityEngine.MonoBehaviour[] array = dontDisableTheseScipts;
			foreach (UnityEngine.MonoBehaviour monoBehaviour2 in array)
			{
				if (monoBehaviour2 == monoBehaviour)
				{
					flag = false;
				}
			}
			GameObject[] array2 = dontDisableTheSciptsOnThisObject;
			foreach (GameObject gameObject in array2)
			{
				if (gameObject == monoBehaviour.gameObject)
				{
					flag = false;
				}
			}
			if (flag)
			{
				monoBehaviour.enabled = false;
			}
		}
		Rigidbody[] componentsInChildren2 = GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody rigidbody in componentsInChildren2)
		{
			rigidbody.constraints = RigidbodyConstraints.None;
			rigidbody.drag = 0f;
			rigidbody.angularDrag = 0f;
		}
		Collider[] componentsInChildren3 = GetComponentsInChildren<Collider>();
		foreach (Collider collider in componentsInChildren3)
		{
			collider.material = null;
		}
		if (mIsAi)
		{
			if (mPhotonView.isMine)
			{
				CustomEventTrigger[] componentsInChildren4 = GetComponentsInChildren<CustomEventTrigger>(true);
				CustomEventTrigger[] array3 = componentsInChildren4;
				foreach (CustomEventTrigger customEventTrigger in array3)
				{
					customEventTrigger.Trigger();
				}
				StartCoroutine(WaitForSecondsThenSink(15));
			}
		}
		else if (mPhotonView.isMine)
		{
			PhotonNetwork.RemoveRPCs(mPhotonView);
			StartCoroutine(WaitForSecondsThenSpawn(5f));
			ServiceLocator.GetService<InventoryService>().CurrentUI.Die(destroyItems);
		}
	}

	private void StopBreath()
	{
		if (!(mBreathSoundEvent == null))
		{
			mBreathSoundEvent.stop(STOP_MODE.ALLOWFADEOUT);
		}
	}

	private IEnumerator WaitForSecondsThenSpawn(float v)
	{
		yield return new WaitForSeconds(v);
		PhotonNetwork.Destroy(base.gameObject);
		UnityEngine.Object.FindObjectOfType<NetworkConnector>().SpawnPlayer();
	}

	private void PlayDeathSound()
	{
		if (!(mDeathEvent == null))
		{
			mDeathEvent.set3DAttributes(mHipRigidBody.To3DAttributes());
			mDeathEvent.start();
			mDeathEvent.release();
		}
	}

	[PunRPC]
	[Obsolete]
	public void NetworkZombieDied()
	{
		dead = true;
		UnityEngine.MonoBehaviour[] componentsInChildren = GetComponentsInChildren<UnityEngine.MonoBehaviour>();
		foreach (UnityEngine.MonoBehaviour monoBehaviour in componentsInChildren)
		{
			bool flag = true;
			UnityEngine.MonoBehaviour[] array = dontDisableTheseScipts;
			foreach (UnityEngine.MonoBehaviour monoBehaviour2 in array)
			{
				if (monoBehaviour2 == monoBehaviour)
				{
					flag = false;
				}
			}
			GameObject[] array2 = dontDisableTheSciptsOnThisObject;
			foreach (GameObject gameObject in array2)
			{
				if (gameObject == monoBehaviour.gameObject)
				{
					flag = false;
				}
			}
			if (flag)
			{
				monoBehaviour.enabled = false;
			}
		}
		Rigidbody[] componentsInChildren2 = GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody rigidbody in componentsInChildren2)
		{
			rigidbody.constraints = RigidbodyConstraints.None;
			rigidbody.drag = 0f;
			rigidbody.angularDrag = 0f;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			if (mPhotonView.isMine)
			{
				PhotonNetwork.Destroy(mPhotonView);
			}
		}
		else if (mPhotonView.isMine)
		{
			StartCoroutine(WaitForSecondsThenSink(5));
		}
	}

	private IEnumerator WaitForSecondsThenSink(int v)
	{
		yield return new WaitForSeconds(v);
		PhotonNetwork.Destroy(mPhotonView);
	}
}
