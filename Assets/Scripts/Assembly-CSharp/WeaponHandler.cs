using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using Photon;
using UnityEngine;

public class WeaponHandler : PunBehaviour
{
	[Serializable]
	public class WeaponWrapper
	{
		public InventoryItem m_item;

		public Weapon m_weapon;
	}

	private const string GANGSTA_PARAM = "Gangsta";

	public WeaponWrapper[] m_weaponList;

	private EventInstance mPunchSwing;

	private EventInstance mPunchHit;

	private EventInstance mGangstaYoSound;

	public Rigidbody weapon;

	private Rigidbody rightArm;

	private Rigidbody leftArm;

	public float weaponRotationForce;

	public float weaponPositionForce;

	private bool grabbingWeapon;

	private FixedJoint fixedJoint;

	private FixedJoint fixedJoint2;

	private CharacterJoint charJoint;

	private ConfigurableJoint configJoint;

	private ConfigurableJoint configJoint2;

	private PhysicsAmimationController mPhysicsController;

	private Transform gangstaAim;

	private Transform rightAim;

	private Transform leftAim;

	private Transform rightIdle;

	private Transform leftIdle;

	private Transform reloadAim;

	public float sinceShotMultiplier = 1f;

	private PhotonView mPhotonView;

	private ZombieBlackboard blackBoard;

	private float hitCdRight;

	private float hitCdLeft;

	public AnimationCurve punchCurve;

	public float punchForce;

	public float punchTime;

	private Transform leftHandTag;

	private bool hasGrabbedRight;

	private Transform activeAim;

	private Transform mCameraTransform;

	private byte mHoldingArmsState;

	private byte mLastHoldingArmsState;

	private bool mStartTicking;

	private float mChangedWeaponTimer = 0.3f;

	private float mCurrentChangedWeaponTimer;

	private bool mStartTickingGangsta;

	private float mCurrentGangstaTimer;

	private float mChangeGangstaTimer = 1f;

	private float mCurrentGangstaParamValue;

	private InventoryItem m_currentEquip;

	public Transform weaponAimTransform;

	private bool currentlyReloading;

	private float waitSwitchWeapon;

	public bool ganstaMode;

	private bool usingMeeleWeapon;

	private Coroutine mCurrentReloadCoroutine;

	public byte HoldingArmsState
	{
		get
		{
			return mHoldingArmsState;
		}
	}

	private void Awake()
	{
		mPhotonView = GetComponent<PhotonView>();
		blackBoard = GetComponentInParent<ZombieBlackboard>();
		mPhysicsController = GetComponent<PhysicsAmimationController>();
		gangstaAim = GetComponentInChildren<GangstaAimTag>().transform;
		rightAim = GetComponentInChildren<RightFistAimTag>().transform;
		leftAim = GetComponentInChildren<LeftFistAimTag>().transform;
		rightIdle = GetComponentInChildren<RightIdle>().transform;
		leftIdle = GetComponentInChildren<LeftIdle>().transform;
		reloadAim = GetComponentInChildren<ReloadAimTag>().transform;
		rightArm = GetComponentInChildren<RightArmTag>().GetComponent<Rigidbody>();
		leftArm = GetComponentInChildren<LeftArmTag>().GetComponent<Rigidbody>();
		mLastHoldingArmsState = mHoldingArmsState;
		mCurrentChangedWeaponTimer = mChangedWeaponTimer;
	}

	private void Start()
	{
		mCameraTransform = GetComponentInChildren<CameraMovement>(true).CameraRotationTransform;
		SoundEventsManager soundEventsManager = UnityEngine.Object.FindObjectOfType<SoundEventsManager>();
		if (!string.IsNullOrEmpty(soundEventsManager.PunchHitEvent))
		{
			mPunchHit = RuntimeManager.CreateInstance(soundEventsManager.PunchHitEvent);
		}
		if (!string.IsNullOrEmpty(soundEventsManager.PunchSwingEvent))
		{
			mPunchSwing = RuntimeManager.CreateInstance(soundEventsManager.PunchSwingEvent);
		}
		if (!string.IsNullOrEmpty(soundEventsManager.GangstaEvent))
		{
			mGangstaYoSound = RuntimeManager.CreateInstance(soundEventsManager.GangstaEvent);
			mGangstaYoSound.start();
		}
		ServiceLocator.GetService<InventoryService>().CurrentUI.OnEquipItemE += OnEquipItem;
	}

	public void OnReloading(float time)
	{
		if (!weapon)
		{
			Debug.LogError("No weapon Attached but reloading?");
		}
		else
		{
			mCurrentReloadCoroutine = StartCoroutine(ReloadMoveRigCoroutine(time));
		}
	}

	private IEnumerator ReloadMoveRigCoroutine(float time)
	{
		currentlyReloading = true;
		float curr = time;
		while (curr > 0f)
		{
			MoveRig(weapon, reloadAim, 5f, 100f, true, true);
			curr -= Time.deltaTime;
			yield return null;
		}
		currentlyReloading = false;
	}

	private void OnDestroy()
	{
		mGangstaYoSound.stop(STOP_MODE.IMMEDIATE);
		mGangstaYoSound.release();
		try
		{
			ServiceLocator.GetService<InventoryService>().CurrentUI.OnEquipItemE -= OnEquipItem;
		}
		catch
		{
			Debug.LogError("wtf david");
		}
	}

	private void OnEquipItem(InventoryItem item)
	{
		if (item != null && item.ItemType == InventoryService.ItemType.WEAPON)
		{
			int num = m_weaponList.Length;
			for (int i = 0; i < num; i++)
			{
				if (m_weaponList[i].m_item.DisplayName == item.DisplayName)
				{
					if (m_currentEquip == item)
					{
						SwitchWeapon(null, null);
						m_currentEquip = null;
					}
					else
					{
						InventoryItemWeapon weaponItem = item as InventoryItemWeapon;
						SwitchWeapon(m_weaponList[i].m_weapon.GetComponent<Rigidbody>(), weaponItem);
						m_currentEquip = item;
					}
					break;
				}
			}
		}
		else
		{
			SwitchWeapon(null, null);
			m_currentEquip = null;
		}
	}

	private void Update()
	{
		if (sinceShotMultiplier < 1f)
		{
			sinceShotMultiplier += Time.deltaTime * 5f;
		}
		else
		{
			sinceShotMultiplier = 1f;
		}
		waitSwitchWeapon -= Time.deltaTime;
		hitCdLeft += Time.deltaTime;
		hitCdRight += Time.deltaTime;
		FadeGangstaMusic();
		if (mPhotonView.isMine)
		{
			if (mStartTicking)
			{
				TickChangedWeaponTimer();
			}
			if (mStartTickingGangsta)
			{
				TickGangstaWaitTimer();
			}
			if (Input.GetKeyDown(KeyCode.G) && !TABZChat.DisableInput && weapon != null && !usingMeeleWeapon)
			{
				ganstaMode = !ganstaMode;
				mStartTickingGangsta = true;
				ResetChangedGangstaTimer();
			}
		}
	}

	private void FadeGangstaMusic()
	{
		if (!(mGangstaYoSound == null))
		{
			float num = ((!ganstaMode) ? (0f - Time.deltaTime) : Time.deltaTime);
			mCurrentGangstaParamValue += num;
			mCurrentGangstaParamValue = Mathf.Clamp(mCurrentGangstaParamValue, 0f, 1f);
			mGangstaYoSound.set3DAttributes(mPhysicsController.mainRig.transform.To3DAttributes());
			mGangstaYoSound.setParameterValue("Gangsta", mCurrentGangstaParamValue);
		}
	}

	[PunRPC]
	public void GangstaAimRPC(bool newGangsta)
	{
		ganstaMode = newGangsta;
	}

	private void TickGangstaWaitTimer()
	{
		mCurrentGangstaTimer -= Time.deltaTime;
		if (mCurrentGangstaTimer < 0f)
		{
			mPhotonView.RPC("GangstaAimRPC", PhotonTargets.Others, ganstaMode);
			mStartTickingGangsta = false;
		}
	}

	private void TickChangedWeaponTimer()
	{
		mCurrentChangedWeaponTimer -= Time.deltaTime;
		if (mCurrentChangedWeaponTimer < 0f)
		{
			byte weaponIndexOf = GetWeaponIndexOf(weapon);
			if (mPhotonView.isMine)
			{
				mPhotonView.RPC("WeaponSwitched", PhotonTargets.Others, weaponIndexOf);
			}
			Debug.Log("Sending RPC weapon changed to: " + weaponIndexOf);
			mStartTicking = false;
		}
	}

	private void ResetChangedWeaponTimer()
	{
		mCurrentChangedWeaponTimer = mChangedWeaponTimer;
	}

	private void ResetChangedGangstaTimer()
	{
		mCurrentChangedWeaponTimer = mChangeGangstaTimer;
	}

	private void FixedUpdate()
	{
		if (!weapon)
		{
			if (mPhotonView.isMine)
			{
				if (Input.GetKey(KeyCode.Mouse1))
				{
					mHoldingArmsState = 1;
					MoveArms();
				}
				else
				{
					mHoldingArmsState = 0;
					MoveArmsIdle();
				}
			}
			if (Input.GetKeyDown(KeyCode.Mouse0) && mPhotonView.isMine)
			{
				if (hitCdRight > 0.7f && hitCdLeft > 0.1f)
				{
					hitCdRight = 0f;
					mPhotonView.RPC("PunchRPC", PhotonTargets.All, true);
				}
				if (hitCdLeft > 0.7f && hitCdRight > 0.1f)
				{
					hitCdLeft = 0f;
					mPhotonView.RPC("PunchRPC", PhotonTargets.All, false);
				}
			}
			if (!mPhotonView.isMine)
			{
				ApplyNewArmsState();
			}
		}
		else
		{
			if (!configJoint && !grabbingWeapon)
			{
				MoveRig(rightArm, rightIdle, 0f, 100f, true);
			}
			if (!configJoint2 && !grabbingWeapon)
			{
				MoveRig(leftArm, leftIdle, 0f, 100f, true);
			}
			if (waitSwitchWeapon > 0f)
			{
				MoveRig(rightArm, rightIdle, 5f, 100f, true);
				MoveRig(leftArm, leftIdle, 5f, 100f, true);
			}
			if (Vector3.Angle(mCameraTransform.forward, weapon.transform.forward) > 120f)
			{
				Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
				foreach (Rigidbody rigidbody in componentsInChildren)
				{
				}
			}
		}
		if (!grabbingWeapon && (bool)weapon)
		{
			weapon.constraints = RigidbodyConstraints.None;
		}
		if (Time.deltaTime > 0.5f)
		{
			return;
		}
		if (grabbingWeapon)
		{
			GrabWeapon();
			weaponAimTransform.localRotation = Quaternion.Euler(new Vector3(0f - weaponAimTransform.parent.parent.localRotation.eulerAngles.x, 0f, 0f));
			return;
		}
		if ((bool)weapon && blackBoard.Fallen < -1f && !currentlyReloading)
		{
			MoveWeapon();
		}
		weaponAimTransform.localRotation = Quaternion.identity;
	}

	private void ApplyNewArmsState()
	{
		if (mHoldingArmsState == 1)
		{
			MoveArms();
		}
		else
		{
			MoveArmsIdle();
		}
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		base.OnPhotonPlayerConnected(newPlayer);
		byte weaponIndexOf = GetWeaponIndexOf(weapon);
		if (mPhotonView.isMine)
		{
			mPhotonView.RPC("WeaponSwitched", newPlayer, weaponIndexOf);
		}
	}

	[PunRPC]
	public void WeaponSwitched(byte newWeapon)
	{
		Rigidbody newWeapon2 = ((newWeapon != byte.MaxValue) ? m_weaponList[newWeapon].m_weapon.GetComponent<Rigidbody>() : null);
		SwitchWeapon(newWeapon2, null, true);
	}

	[PunRPC]
	public void PunchRPC(bool right)
	{
		if (right)
		{
			StartCoroutine(Punch(rightArm));
		}
		else
		{
			StartCoroutine(Punch(leftArm));
		}
		PlayPunchSound();
	}

	private void PlayPunchSound()
	{
		if (!(mPunchSwing == null))
		{
			mPunchSwing.set3DAttributes(rightArm.transform.To3DAttributes());
			mPunchSwing.start();
		}
	}

	private IEnumerator Punch(Rigidbody rig)
	{
		if (rig == null)
		{
			yield return null;
		}
		MeleeWeapon meeleWeapon = rig.GetComponent<MeleeWeapon>();
		float t = 0f;
		while (t < punchTime)
		{
			t += Time.deltaTime;
			rig.AddForce(mCameraTransform.forward * punchForce * 60f * Time.deltaTime * punchCurve.Evaluate(t / punchTime), ForceMode.Acceleration);
			meeleWeapon.multiplier = t / punchTime;
			yield return null;
		}
		meeleWeapon.multiplier = 0f;
	}

	private void MoveRig(Rigidbody rig, Transform target, float force, float torque, bool fixer, bool fixedTime = false)
	{
		float num = 0.0166f;
		if (fixedTime)
		{
			num = Time.deltaTime;
		}
		Vector3 forward = rig.transform.forward;
		if (fixer)
		{
			forward = rig.transform.GetChild(0).forward;
		}
		float num2 = Vector3.Angle(forward, target.forward);
		Vector3 vector = Vector3.Cross(forward, target.forward);
		rig.AddTorque(vector * num2 * torque * num * sinceShotMultiplier, ForceMode.VelocityChange);
		Vector3 vector2 = target.position - rig.position;
		rig.AddForce(vector2 * force, ForceMode.VelocityChange);
	}

	private void MoveArms()
	{
		if (hitCdRight > punchTime)
		{
			MoveRig(rightArm, rightAim, 5f, 200f, true);
			mHoldingArmsState = 1;
		}
		if (hitCdLeft > punchTime)
		{
			MoveRig(leftArm, leftAim, 5f, 200f, true);
			mHoldingArmsState = 1;
		}
	}

	private void MoveArmsIdle()
	{
		if (hitCdRight > punchTime)
		{
			MoveRig(rightArm, rightIdle, 0f, 100f, true);
		}
		if (hitCdLeft > punchTime)
		{
			MoveRig(leftArm, leftIdle, 0f, 100f, true);
		}
	}

	private void MoveWeapon()
	{
		if (ganstaMode)
		{
			float num = Vector3.Angle(weapon.transform.up, gangstaAim.up);
			Vector3 vector = Vector3.Cross(weapon.transform.up, gangstaAim.up);
			weapon.AddTorque(vector * num * weaponRotationForce * 0.0166f * sinceShotMultiplier, ForceMode.VelocityChange);
			MoveRig(weapon, gangstaAim, 3f, weaponRotationForce, false);
		}
		else
		{
			float num2 = Vector3.Angle(weapon.transform.up, activeAim.up);
			Vector3 vector2 = Vector3.Cross(weapon.transform.up, activeAim.up);
			weapon.AddTorque(vector2 * num2 * weaponRotationForce * 0.0166f * sinceShotMultiplier, ForceMode.VelocityChange);
			MoveRig(weapon, activeAim, weaponPositionForce, weaponRotationForce, false);
		}
	}

	public void SwitchWeapon(Rigidbody newWeapon, InventoryItemWeapon weaponItem, bool networkForce = false)
	{
		if (!mPhotonView.isMine && !networkForce)
		{
			return;
		}
		currentlyReloading = false;
		mStartTicking = true;
		ResetChangedWeaponTimer();
		if ((bool)weapon)
		{
			if (newWeapon == weapon)
			{
				weapon.GetComponent<Weapon>().SetWeaponInventoryItem(weaponItem);
				return;
			}
			weapon.gameObject.SetActive(false);
		}
		else if (newWeapon == null)
		{
			return;
		}
		if ((bool)configJoint)
		{
			UnityEngine.Object.Destroy(configJoint);
		}
		if ((bool)configJoint2)
		{
			UnityEngine.Object.Destroy(configJoint2);
		}
		if (newWeapon != null)
		{
			newWeapon.gameObject.SetActive(true);
			SetWeapon(newWeapon, weaponItem);
			GetComponentInChildren<CameraMovement>().currentWeapon = newWeapon.GetComponent<Weapon>();
		}
		else
		{
			if (weapon != null)
			{
				Weapon component = weapon.GetComponent<Weapon>();
				component.SetWeaponInventoryItem(null);
			}
			weapon = null;
			GetComponentInChildren<CameraMovement>().currentWeapon = null;
		}
		if (!networkForce)
		{
		}
	}

	private void SetWeapon(Rigidbody weaponToPickUp, InventoryItemWeapon weaponItem)
	{
		Weapon component = weaponToPickUp.GetComponent<Weapon>();
		usingMeeleWeapon = false;
		if (component.melee)
		{
			usingMeeleWeapon = true;
		}
		if (usingMeeleWeapon)
		{
			ganstaMode = false;
			ResetChangedGangstaTimer();
		}
		activeAim = component.aimTarget;
		weaponRotationForce = component.weaponRotationForce;
		hasGrabbedRight = false;
		grabbingWeapon = true;
		waitSwitchWeapon = 0.2f;
		if ((bool)fixedJoint)
		{
			UnityEngine.Object.Destroy(fixedJoint);
		}
		component.SetWeaponInventoryItem(weaponItem);
		weapon = weaponToPickUp;
		weapon.transform.position = activeAim.position;
		WeaponLeftHandTag componentInChildren = weapon.GetComponentInChildren<WeaponLeftHandTag>();
		if ((bool)componentInChildren)
		{
			leftHandTag = componentInChildren.transform;
		}
		else
		{
			leftHandTag = null;
		}
	}

	private byte GetWeaponIndexOf(Rigidbody weapon)
	{
		if (weapon == null)
		{
			return byte.MaxValue;
		}
		for (byte b = 0; b < m_weaponList.Length; b++)
		{
			if (m_weaponList[b].m_weapon.name == weapon.name)
			{
				return b;
			}
		}
		throw new Exception("Weapon: " + weapon.name + " Not found in weaponlist!");
	}

	private void GrabWeapon()
	{
		if (weapon == null)
		{
			return;
		}
		Transform transform = weapon.GetComponentInChildren<WeaponRightHandTag>().transform;
		bool flag = true;
		weapon.transform.position = activeAim.position;
		weapon.transform.rotation = activeAim.rotation;
		weapon.constraints = RigidbodyConstraints.FreezeRotation;
		if (waitSwitchWeapon < 0f)
		{
			if ((bool)leftHandTag)
			{
				flag = false;
				if (!flag && hasGrabbedRight)
				{
					leftArm.GetComponentInChildren<Collider>().enabled = false;
					leftArm.velocity += (leftHandTag.position - leftArm.transform.GetChild(1).position).normalized * 10f;
					if (Vector3.Distance(leftHandTag.position, leftArm.transform.GetChild(1).position) < 0.1f)
					{
						leftArm.GetComponentInChildren<Collider>().enabled = true;
						leftArm.transform.position = leftHandTag.position + (leftArm.transform.position - leftArm.transform.GetChild(1).position);
						configJoint2 = leftArm.gameObject.AddComponent<ConfigurableJoint>();
						configJoint2.xMotion = ConfigurableJointMotion.Locked;
						configJoint2.yMotion = ConfigurableJointMotion.Locked;
						configJoint2.zMotion = ConfigurableJointMotion.Locked;
						configJoint2.angularXMotion = ConfigurableJointMotion.Locked;
						configJoint2.angularYMotion = ConfigurableJointMotion.Locked;
						configJoint2.angularZMotion = ConfigurableJointMotion.Locked;
						configJoint2.projectionMode = JointProjectionMode.PositionAndRotation;
						configJoint2.connectedBody = weapon;
						flag = true;
					}
				}
			}
			if (!hasGrabbedRight)
			{
				rightArm.GetComponentInChildren<Collider>().enabled = false;
				rightArm.velocity += (transform.position - rightArm.transform.GetChild(1).position).normalized * 10f;
				if (Vector3.Distance(transform.position, rightArm.transform.GetChild(1).position) < 0.1f)
				{
					rightArm.GetComponentInChildren<Collider>().enabled = true;
					rightArm.transform.position = transform.position + (rightArm.transform.position - rightArm.transform.GetChild(1).position);
					configJoint = rightArm.gameObject.AddComponent<ConfigurableJoint>();
					configJoint.xMotion = ConfigurableJointMotion.Locked;
					configJoint.yMotion = ConfigurableJointMotion.Locked;
					configJoint.zMotion = ConfigurableJointMotion.Locked;
					configJoint.angularXMotion = ConfigurableJointMotion.Locked;
					configJoint.angularYMotion = ConfigurableJointMotion.Locked;
					configJoint.angularZMotion = ConfigurableJointMotion.Locked;
					configJoint.projectionMode = JointProjectionMode.PositionAndRotation;
					configJoint.connectedBody = weapon;
					hasGrabbedRight = true;
				}
			}
		}
		weapon.velocity *= 0f;
		if (flag && hasGrabbedRight)
		{
			grabbingWeapon = false;
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(mHoldingArmsState);
			mLastHoldingArmsState = mHoldingArmsState;
		}
		else if (stream.isReading)
		{
			byte armsState = (byte)stream.ReceiveNext();
			ApplyArmsState(armsState);
		}
	}

	private void ApplyArmsState(byte armsState)
	{
		mHoldingArmsState = armsState;
	}
}
