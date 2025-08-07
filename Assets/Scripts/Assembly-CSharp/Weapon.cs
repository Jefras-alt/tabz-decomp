using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
	public enum WeaponStates
	{
		Fire = 0,
		NoAmmo = 1,
		Reload = 2
	}

	private static string FMOD_OUT_OF_AMMO_PARAM;

	private static string FMOD_RELOAD_STATE_PARAM;

	private static string mWeaponModeParam;

	[Header("Weapon")]
	public GameObject projectile;

	public float fireRate;

	public bool automatic;

	public float fov = 60f;

	public Transform currentADS;

	private float counter;

	[SerializeField]
	[Range(1f, 50f)]
	private int m_MagazineSize;

	[SerializeField]
	[Range(0f, 5f)]
	private float mReloadTime;

	[SerializeField]
	private bool mIsMelee;

	private InventoryItemAmmo m_AmmoInInventory;

	private InventoryItemWeapon m_WeaponInInventory;

	private bool mReloading;

	[Header("Feedback")]
	public float screenshakeAmount;

	public float screenshakeTime;

	public float distortAmount;

	public float AberrationAmount;

	private string mWeaponSoundString;

	private EventInstance mWeaponEvent;

	private EventInstance mReloadEvent;

	private EventInstance mHitEvent;

	private EventInstance mHolsterWeaponEvent;

	private EventInstance mRaiseWeaponEvent;

	private FeedbackHandler feedbackHandler;

	public float weaponRotationForce = 300f;

	public float recoilMultiplierIncreasePerShot;

	public float recoilMultiplierDecrese = 0.95f;

	private float recoilMultiplier;

	public Rigidbody bodyRig;

	public float bodyRecoil;

	public Vector3 recoil;

	public Vector3 angularRecoil;

	public Vector3 forceShake;

	public float forceShakeTime = 0.3f;

	private Rigidbody rig;

	private PhysicsAmimationController anim;

	private WeaponHandler weaponHandler;

	private PhotonView mPhotonView;

	private static SoundEffectsManager mSoundHandler;

	private SoundEventsManager.WeaponSoundWrapper mWeaponSound;

	private NoiseSpawner m_noiseSpawner;

	private static UIInventory mCurrentInventory;

	private Text mAmmoUIText;

	private FirePoint firePoint;

	public Transform aimTarget;

	private CameraMovement cameraMove;

	[Header("Melee")]
	public bool melee;

	public AnimationCurve punchCurve;

	public float punchForce;

	public float punchTime;

	public float punchRate = 0.7f;

	private float punchCounter;

	private static SoundEventsManager mSoundEventManager;

	public static string WEAPON_MODE_PARAM
	{
		get
		{
			return mWeaponModeParam;
		}
	}

	public bool HasAmmoInMagazine
	{
		get
		{
			return BulletsInMag > 0;
		}
	}

	public bool HasFullMagazine
	{
		get
		{
			return BulletsInMag == m_MagazineSize;
		}
	}

	public int BulletsInMag
	{
		get
		{
			if (!mPhotonView.isMine)
			{
				return 1;
			}
			if (m_WeaponInInventory == null)
			{
				Debug.Log("Break mothe");
			}
			return m_WeaponInInventory.BulletsInMagazine;
		}
		set
		{
			m_WeaponInInventory.BulletsInMagazine = value;
		}
	}

	private void Awake()
	{
		cameraMove = base.transform.parent.GetComponentInChildren<CameraMovement>();
		mPhotonView = GetComponent<PhotonView>();
		mSoundHandler = Object.FindObjectOfType<SoundEffectsManager>();
		firePoint = GetComponentInChildren<FirePoint>();
		rig = GetComponent<Rigidbody>();
		if (mSoundEventManager == null)
		{
			mSoundEventManager = Object.FindObjectOfType<SoundEventsManager>();
		}
		mWeaponSound = mSoundEventManager.GetWeaponSoundEvent(base.gameObject.name);
		if (!string.IsNullOrEmpty(mWeaponSound.WeaponEvent))
		{
			mWeaponSoundString = mWeaponSound.WeaponEvent;
			mWeaponEvent = RuntimeManager.CreateInstance(mWeaponSoundString);
		}
		if (!string.IsNullOrEmpty(mWeaponSound.HitEvent))
		{
			mHitEvent = RuntimeManager.CreateInstance(mWeaponSound.HitEvent);
		}
		if (!string.IsNullOrEmpty(mWeaponSound.ReloadEvent))
		{
			mReloadEvent = RuntimeManager.CreateInstance(mWeaponSound.ReloadEvent);
		}
		if (!string.IsNullOrEmpty(mSoundEventManager.RaiseEvent))
		{
			mRaiseWeaponEvent = RuntimeManager.CreateInstance(mSoundEventManager.RaiseEvent);
		}
		if (!string.IsNullOrEmpty(mSoundEventManager.HolsterEvent))
		{
			mHolsterWeaponEvent = RuntimeManager.CreateInstance(mSoundEventManager.HolsterEvent);
		}
		if (mPhotonView.isMine)
		{
			mAmmoUIText = base.transform.parent.GetComponentInChildren<AmmoUITAG>().GetComponent<Text>();
			mCurrentInventory = base.transform.parent.GetComponentInChildren<UIInventory>();
			mCurrentInventory.AddReferenceToThis(this);
			ADS componentInChildren = GetComponentInChildren<ADS>();
			if ((bool)componentInChildren)
			{
				currentADS = componentInChildren.transform;
			}
		}
	}

	private void Start()
	{
		m_noiseSpawner = GetComponent<NoiseSpawner>();
		feedbackHandler = base.transform.root.GetComponent<FeedbackHandler>();
		anim = base.transform.root.GetComponent<PhysicsAmimationController>();
		weaponHandler = base.transform.root.GetComponent<WeaponHandler>();
	}

	public void SetWeaponInventoryItem(InventoryItemWeapon weaponItem)
	{
		if (mPhotonView.isMine)
		{
			m_WeaponInInventory = weaponItem;
			if (!(m_WeaponInInventory == null))
			{
				mCurrentInventory.AddReferenceToThis(this);
				UpdateUI();
			}
		}
	}

	public void UpdateUI()
	{
		if (!mPhotonView.isMine || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (mIsMelee)
		{
			mAmmoUIText.text = string.Empty;
			return;
		}
		CheckAmmo();
		string displayName = m_WeaponInInventory.DisplayName;
		string text = BulletsInMag.ToString();
		if (m_AmmoInInventory == null)
		{
			mAmmoUIText.text = displayName + "\n" + text + " \\ 0";
			return;
		}
		string text2 = m_AmmoInInventory.Amount.ToString();
		mAmmoUIText.text = displayName + "\n" + text + " \\ " + text2;
	}

	private void CheckAmmo()
	{
		InventoryItemAmmo ammoType = mCurrentInventory.GetAmmoType(m_WeaponInInventory.AmmoType);
		if (ammoType == null)
		{
			if (mIsMelee)
			{
				mAmmoUIText.text = string.Empty;
			}
		}
		else
		{
			m_AmmoInInventory = ammoType;
		}
	}

	private void Reload()
	{
		if (TABZChat.cheatToggle)
		{
			BulletsInMag += 1000;
			UpdateUI();
			return;
		}
		int num = m_MagazineSize - BulletsInMag;
		if (num > 0)
		{
			int amount = m_AmmoInInventory.Amount;
			if (amount > 0)
			{
				mReloading = true;
				weaponHandler.OnReloading(mReloadTime);
				StartCoroutine(ReloadCoroutine(amount, num));
			}
		}
	}

	private IEnumerator ReloadCoroutine(int ammo, int bulletsToFill)
	{
		PlayReloadSound(0);
		yield return new WaitForSeconds(mReloadTime);
		if (ammo >= bulletsToFill)
		{
			BulletsInMag += bulletsToFill;
			m_AmmoInInventory.Amount -= bulletsToFill;
		}
		else
		{
			BulletsInMag += ammo;
			m_AmmoInInventory.Amount -= ammo;
		}
		mCurrentInventory.UpdateAmmoUI(m_AmmoInInventory);
		mReloading = false;
		PlayReloadSound(1);
		if (mWeaponEvent != null)
		{
			mWeaponEvent.setParameterValue(FMOD_OUT_OF_AMMO_PARAM, 0f);
		}
		UpdateUI();
	}

	private void FixedUpdate()
	{
		recoilMultiplier *= recoilMultiplierDecrese;
	}

	private void Update()
	{
		if (mPhotonView.isMine)
		{
			punchCounter += Time.deltaTime;
			counter += Time.deltaTime;
			if (!melee && CanFire())
			{
				mPhotonView.RPC("Fire", PhotonTargets.All, PhotonNetwork.player);
				AddFeedback();
				counter = 0f;
			}
			if (punchCounter > punchRate && melee && Input.GetKeyDown(KeyCode.Mouse0))
			{
				mPhotonView.RPC("Swing", PhotonTargets.All);
				punchCounter = 0f;
				PlayFireSound();
			}
			if (Input.GetKeyDown(KeyCode.R) && CanReload())
			{
				Reload();
			}
		}
	}

	private bool CanReload()
	{
		CheckAmmo();
		if (!TABZChat.cheatToggle)
		{
			if (!(m_AmmoInInventory == null) && !mIsMelee && !HasFullMagazine && !mReloading)
			{
				return !TABZChat.DisableInput;
			}
			return false;
		}
		return true;
	}

	private void OnEnable()
	{
		PlayRaiseEvent();
	}

	private void OnDisable()
	{
		PlayHolsterEvent();
		if (mPhotonView.isMine)
		{
			mReloading = false;
			StopReloadSound();
			mAmmoUIText.text = string.Empty;
		}
	}

	private void PlayHolsterEvent()
	{
		if (!(mHolsterWeaponEvent == null))
		{
			mHolsterWeaponEvent.set3DAttributes(base.transform.To3DAttributes());
			mHolsterWeaponEvent.start();
		}
	}

	private void PlayRaiseEvent()
	{
		if (!(mRaiseWeaponEvent == null))
		{
			mRaiseWeaponEvent.set3DAttributes(base.transform.To3DAttributes());
			mRaiseWeaponEvent.start();
		}
	}

	private bool CanFire()
	{
		if (mReloading || TABZChat.DisableInput)
		{
			return false;
		}
		if (!HasAmmoInMagazine)
		{
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				PlayFireSound();
			}
			return false;
		}
		InventoryService service = ServiceLocator.GetService<InventoryService>();
		if (service != null && !(service.CurrentUI == null) && (service == null || !(service.CurrentUI != null) || service.CurrentUI.IsOpen))
		{
			return false;
		}
		if ((Input.GetKeyDown(KeyCode.Mouse0) || (Input.GetKey(KeyCode.Mouse0) && automatic)) && counter > fireRate)
		{
			return true;
		}
		return false;
	}

	[PunRPC]
	public void Swing()
	{
		StartCoroutine(Punch());
	}

	private IEnumerator Punch()
	{
		MeleeWeapon meeleWeapon = GetComponent<MeleeWeapon>();
		float t = 0f;
		while (t < punchTime)
		{
			t += Time.deltaTime;
			rig.AddForce(Camera.main.transform.forward * punchForce * 60f * Time.deltaTime * punchCurve.Evaluate(t / punchTime), ForceMode.Acceleration);
			meeleWeapon.multiplier = t / punchTime;
			yield return null;
		}
	}

	[PunRPC]
	public void Fire(PhotonPlayer sender)
	{
		GameObject gameObject = Object.Instantiate(projectile, firePoint.transform.position, base.transform.rotation);
		RaycastProjectile[] componentsInChildren = gameObject.GetComponentsInChildren<RaycastProjectile>();
		for (byte b = 0; b < componentsInChildren.Length; b++)
		{
			componentsInChildren[b].InitSender(sender);
		}
		ProjectileHit[] componentsInChildren2 = gameObject.GetComponentsInChildren<ProjectileHit>();
		if (AssignHitEvent(componentsInChildren2))
		{
			gameObject.SetActive(true);
		}
		RaycastProjectile[] componentsInChildren3 = gameObject.GetComponentsInChildren<RaycastProjectile>();
		foreach (RaycastProjectile raycastProjectile in componentsInChildren3)
		{
			raycastProjectile.shooterRoot = base.transform.root;
		}
		AddFeedback();
		rig.AddForce(base.transform.forward * (0f - recoil.z) * (1f + recoilMultiplier), ForceMode.VelocityChange);
		rig.AddForce(base.transform.up * recoil.y * (1f + recoilMultiplier), ForceMode.VelocityChange);
		rig.AddForce(base.transform.right * recoil.x * (1f + recoilMultiplier), ForceMode.VelocityChange);
		rig.AddTorque(base.transform.forward * angularRecoil.z * (1f + recoilMultiplier), ForceMode.VelocityChange);
		rig.AddTorque(base.transform.up * angularRecoil.y * (1f + recoilMultiplier), ForceMode.VelocityChange);
		rig.AddTorque(anim.right.forward * angularRecoil.x * (1f + recoilMultiplier), ForceMode.VelocityChange);
		if (bodyRecoil > 0f)
		{
			bodyRig.AddForce(bodyRecoil * -base.transform.forward, ForceMode.VelocityChange);
		}
		cameraMove.AddCameraForce(forceShake, forceShakeTime);
		weaponHandler.sinceShotMultiplier = 0.2f;
		m_noiseSpawner.Trigger();
		PlayFireSound();
		recoilMultiplier += recoilMultiplierIncreasePerShot;
		if (mPhotonView.isMine)
		{
			BulletsInMag--;
			if (BulletsInMag <= 0 && mWeaponEvent != null)
			{
				mWeaponEvent.setParameterValue(FMOD_OUT_OF_AMMO_PARAM, 1f);
			}
			UpdateUI();
		}
	}

	private bool AssignHitEvent(ProjectileHit[] hits)
	{
		for (byte b = 0; b < hits.Length; b++)
		{
			ProjectileHit projectileHit = hits[b];
			projectileHit.InitHitSoundEvent(mHitEvent);
		}
		return true;
	}

	private void PlayReloadSound(int mode)
	{
		if (!(mReloadEvent == null))
		{
			mReloadEvent.setParameterValue(FMOD_RELOAD_STATE_PARAM, mode);
			mReloadEvent.set3DAttributes(base.transform.To3DAttributes());
			mReloadEvent.start();
		}
	}

	private void StopReloadSound()
	{
		if (mReloadEvent != null)
		{
			mReloadEvent.stop(STOP_MODE.IMMEDIATE);
		}
	}

	private void PlayFireSound()
	{
		if (!(mWeaponEvent == null))
		{
			int num = 0;
			mWeaponEvent = RuntimeManager.CreateInstance(mWeaponSoundString);
			if (BulletsInMag > 0)
			{
				mWeaponEvent.setParameterValue(FMOD_OUT_OF_AMMO_PARAM, 0f);
			}
			else
			{
				mWeaponEvent.setParameterValue(FMOD_OUT_OF_AMMO_PARAM, 1f);
			}
			mWeaponEvent.set3DAttributes(base.transform.To3DAttributes());
			mWeaponEvent.start();
			mWeaponEvent.release();
		}
	}

	private void OnDestroy()
	{
		if (mWeaponEvent != null)
		{
			mWeaponEvent.release();
		}
		if (mHitEvent != null)
		{
			mHitEvent.release();
		}
	}

	private void AddFeedback()
	{
		if (mPhotonView.isMine)
		{
			feedbackHandler.AddAber(AberrationAmount);
			feedbackHandler.AddDistortion(distortAmount);
			screenShake.AddShake(screenshakeAmount, screenshakeTime);
		}
	}

	internal void SetBulletsInMagazine(int bulletsLeft)
	{
		BulletsInMag = bulletsLeft;
	}

	static Weapon()
	{
		FMOD_OUT_OF_AMMO_PARAM = "OutOfAmmo";
		FMOD_RELOAD_STATE_PARAM = "Position";
		mWeaponModeParam = "Mode";
	}
}
