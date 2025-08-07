using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
	private enum StandState : byte
	{
		Prone = 10,
		Crouch = 2,
		Stand = 1
	}

	private class UpdateMarkerPos : MonoBehaviour
	{
		public GameObject toFollow;

		private void Update()
		{
			if (toFollow != null)
			{
				base.gameObject.transform.position = toFollow.transform.position;
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private static string FMOD_MOVEMENT_TYPE_PARAM;

	private static string FMOD_ISATTACKING_PARAM;

	public bool AI;

	public Material[] m_materials;

	public Renderer m_modelRenderer;

	private MovementHandler movement;

	private PhysicsAmimationController animController;

	[HideInInspector]
	public float jumpCD = 1f;

	private Transform target;

	[HideInInspector]
	public Vector3 playerDirection;

	private ZombieBlackboard m_blackboard;

	private PhotonView mPhotonView;

	private RotationHandler m_rotation;

	private byte mMovementSoundParameter;

	public byte mMovementState = 4;

	private byte mPreviousMovementState;

	private bool addingForces;

	private StandHandler mStandHandler;

	private bool mIsStrafing;

	private bool mIsAttacking;

	private EventInstance mMovementSoundEvent;

	private EventInstance mScreamsSoundEvent;

	private EventInstance mDamageSoundEvent;

	private EventInstance mBreathSoundEvent;

	private static SoundEventsManager mSoundEventManager;

	private PlayerSoundHandler mPlayerSoundHandler;

	public byte CurrentMovementState
	{
		get
		{
			return mMovementState;
		}
	}

	public byte MovementSoundParameter
	{
		get
		{
			return mMovementSoundParameter;
		}
	}

	private void Awake()
	{
		if (mSoundEventManager == null)
		{
			mSoundEventManager = Object.FindObjectOfType<SoundEventsManager>();
		}
		m_blackboard = GetComponent<ZombieBlackboard>();
		movement = GetComponent<MovementHandler>();
		animController = GetComponent<PhysicsAmimationController>();
		m_rotation = GetComponent<RotationHandler>();
		mStandHandler = GetComponent<StandHandler>();
		mPlayerSoundHandler = GetComponent<PlayerSoundHandler>();
		if (AI)
		{
			mScreamsSoundEvent = RuntimeManager.CreateInstance(mSoundEventManager.GetScreamEvent(base.gameObject));
			mScreamsSoundEvent.start();
			RuntimeManager.AttachInstanceToGameObject(mScreamsSoundEvent, animController.mainRig.transform, animController.mainRig);
			Object.Destroy(GetComponent<FeedbackHandler>());
			if (base.name == "TABZOMBIEGIANT(Clone)")
			{
				GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				obj.transform.position = animController.mainRig.position;
				obj.AddComponent<UpdateMarkerPos>().toFollow = animController.mainRig.gameObject;
				obj.layer = LayerMask.NameToLayer("ZombeCollider");
				obj.transform.localScale *= 40f;
				obj.GetComponent<MeshRenderer>().material.color = Color.green;
				Object.Destroy(obj.GetComponent<SphereCollider>());
			}
		}
		mPhotonView = GetComponent<PhotonView>();
		if (!AI && m_modelRenderer != null && m_materials != null && m_materials.Length >= 2)
		{
			if (!mPhotonView.isMine)
			{
				GameObject obj2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				obj2.transform.position = animController.mainRig.position;
				obj2.AddComponent<UpdateMarkerPos>().toFollow = animController.mainRig.gameObject;
				obj2.layer = LayerMask.NameToLayer("ZombeCollider");
				obj2.transform.localScale *= 40f;
				obj2.GetComponent<MeshRenderer>().material.color = Color.magenta;
				Object.Destroy(obj2.GetComponent<SphereCollider>());
			}
			bool flag = (bool)mPhotonView.instantiationData[0];
			m_modelRenderer.material = m_materials[(!flag) ? 1u : 0u];
			m_modelRenderer.material.color = Color.grey;
			object[] instantiationData = mPhotonView.instantiationData;
			if (instantiationData.Length >= 5)
			{
				Color color = new Color((float)instantiationData[1], (float)instantiationData[2], (float)instantiationData[3], (float)instantiationData[4]);
				m_modelRenderer.material.color = color;
			}
			else if (flag)
			{
				m_modelRenderer.material.color = new Color(0.574f, 0.257f, 0.257f, 1f);
			}
			else
			{
				m_modelRenderer.material.color = new Color(0.257f, 0.364f, 0.574f, 1f);
			}
		}
		mPreviousMovementState = mMovementState;
		InitSounds();
	}

	private void InitSounds()
	{
		string walkingEvent = mSoundEventManager.WalkingEvent;
		if (!string.IsNullOrEmpty(walkingEvent))
		{
			mMovementSoundEvent = RuntimeManager.CreateInstance(walkingEvent);
			mMovementSoundEvent.start();
			RuntimeManager.AttachInstanceToGameObject(mMovementSoundEvent, base.transform, animController.mainRig);
		}
	}

	private void Update()
	{
		UpdateMovementSoundParameter();
		if (!mPhotonView.isMine)
		{
			ApplyClientMovementState();
			return;
		}
		jumpCD += Time.deltaTime;
		if (AI)
		{
			float num = DegreesToTarget();
			if (m_blackboard.BehaviorState == BehaviorState.ATTACK || m_blackboard.BehaviorState == BehaviorState.INSPECT)
			{
				target = m_blackboard.TargetTransform;
				if (target == null)
				{
					return;
				}
				playerDirection = (target.position + m_rotation.OffsetVector - animController.mainRig.position).normalized;
				if (!(target != null))
				{
					return;
				}
				if (m_blackboard.DistanceToTarget < 1f)
				{
					PlayAttackScreams();
					if (!VectorUtils.IsNaN(m_blackboard.CurrentWaypoint) && num < 70f)
					{
						movement.Walk();
						animController.Walk();
						mMovementState = 1;
					}
					else if (VectorUtils.IsNaN(m_blackboard.CurrentWaypoint))
					{
						movement.Walk();
						animController.Walk();
						mMovementState = 1;
					}
				}
				else if (!VectorUtils.IsNaN(m_blackboard.CurrentWaypoint) && num < 70f)
				{
					movement.Run();
					animController.Run();
					mMovementState = 1;
				}
				else if (VectorUtils.IsNaN(m_blackboard.CurrentWaypoint))
				{
					movement.Run();
					animController.Run();
					mMovementState = 1;
				}
			}
			else if (m_blackboard.BehaviorState == BehaviorState.IDLE)
			{
				mMovementState = 3;
				PlayIdleScreams();
			}
		}
		else
		{
			if (!(m_blackboard.Fallen < -0.5f))
			{
				return;
			}
			if (Input.GetKey(KeyCode.W) && !TABZChat.DisableInput)
			{
				if (Input.GetKey(KeyCode.LeftShift))
				{
					animController.Run();
					movement.Run();
					mMovementState = 1;
				}
				else
				{
					animController.Walk();
					movement.Walk();
					mMovementState = 0;
				}
				mIsStrafing = true;
			}
			else if (Input.GetKey(KeyCode.S) && !TABZChat.DisableInput)
			{
				animController.Back();
				movement.Back();
				mMovementState = 2;
				mIsStrafing = true;
			}
			else
			{
				mIsStrafing = false;
			}
			if (Input.GetKey(KeyCode.A) && !TABZChat.DisableInput)
			{
				movement.Right();
				if (!mIsStrafing)
				{
					animController.Walk();
					mMovementState = 3;
				}
			}
			else if (Input.GetKey(KeyCode.D) && !TABZChat.DisableInput)
			{
				movement.Left();
				if (!mIsStrafing)
				{
					animController.Walk();
					mMovementState = 3;
				}
			}
			else if (!mIsStrafing)
			{
				mMovementState = 4;
			}
			if (Input.GetKeyDown(KeyCode.Space) && !TABZChat.DisableInput && jumpCD > 0.5f)
			{
				jumpCD = 0f;
				movement.Jump();
				PlayJumpOneShot();
			}
		}
	}

	private void PlayJumpOneShot()
	{
		mPlayerSoundHandler.Jump();
	}

	private bool IsCurrentStandStateSneaking()
	{
		StandState standMultiplier = (StandState)mStandHandler.standMultiplier;
		if (standMultiplier != StandState.Crouch)
		{
			return standMultiplier == StandState.Prone;
		}
		return true;
	}

	private void PlayIdleScreams()
	{
		if (mIsAttacking)
		{
			Debug.Log("Playing Idle Screams!");
			PLAYBACK_STATE state;
			mScreamsSoundEvent.getPlaybackState(out state);
			if (state == PLAYBACK_STATE.PLAYING)
			{
				mScreamsSoundEvent.stop(STOP_MODE.ALLOWFADEOUT);
			}
			mScreamsSoundEvent.set3DAttributes(animController.mainRig.transform.To3DAttributes());
			mScreamsSoundEvent.setParameterValue(FMOD_ISATTACKING_PARAM, 0f);
			mScreamsSoundEvent.start();
			mIsAttacking = false;
		}
	}

	private void UpdateMovementSoundParameter()
	{
		if (IsCurrentStandStateSneaking())
		{
			mMovementSoundParameter = 2;
		}
		else if (mMovementState == 0)
		{
			mMovementSoundParameter = 0;
		}
		else if (mMovementState == 1)
		{
			mMovementSoundParameter = 1;
		}
		else if (mMovementState == 2)
		{
			mMovementSoundParameter = 0;
		}
		else if (mMovementState == 3)
		{
			mMovementSoundParameter = 0;
		}
		else if (mMovementState == 4)
		{
			mMovementSoundEvent.stop(STOP_MODE.ALLOWFADEOUT);
			mMovementSoundParameter = 3;
		}
		if (mMovementSoundParameter != 3)
		{
			mMovementSoundEvent.start();
			mMovementSoundEvent.setParameterValue(FMOD_MOVEMENT_TYPE_PARAM, (int)mMovementSoundParameter);
		}
	}

	private void PlayAttackScreams()
	{
		if (!mIsAttacking)
		{
			PLAYBACK_STATE state;
			mScreamsSoundEvent.getPlaybackState(out state);
			if (state == PLAYBACK_STATE.PLAYING)
			{
				mScreamsSoundEvent.stop(STOP_MODE.ALLOWFADEOUT);
			}
			mScreamsSoundEvent.set3DAttributes(animController.mainRig.transform.To3DAttributes());
			mScreamsSoundEvent.setParameterValue(FMOD_ISATTACKING_PARAM, 1f);
			mScreamsSoundEvent.start();
			mIsAttacking = true;
		}
	}

	public void OnDisable()
	{
		if (mScreamsSoundEvent != null)
		{
			mScreamsSoundEvent.stop(STOP_MODE.ALLOWFADEOUT);
			mScreamsSoundEvent.release();
		}
	}

	public void OnDestroy()
	{
		if (mScreamsSoundEvent != null)
		{
			mScreamsSoundEvent.stop(STOP_MODE.ALLOWFADEOUT);
			mScreamsSoundEvent.release();
		}
	}

	public void SetNewClientState(byte state, bool addForces)
	{
		mMovementState = state;
		addingForces = addForces;
	}

	private void ApplyClientMovementState()
	{
		switch (mMovementState)
		{
		case 0:
			animController.Walk();
			if (addingForces)
			{
				movement.Walk();
			}
			if (!AI)
			{
				movement.TriggerWalkNoise();
			}
			break;
		case 1:
			animController.Run();
			if (addingForces)
			{
				movement.Run();
			}
			if (!AI)
			{
				movement.TriggerRunNoise();
			}
			break;
		case 2:
			animController.Back();
			if (addingForces)
			{
				movement.Back();
			}
			if (!AI)
			{
				movement.TriggerWalkNoise();
			}
			break;
		case 3:
			animController.Walk();
			if (addingForces)
			{
				movement.Walk();
			}
			if (!AI)
			{
				movement.TriggerWalkNoise();
			}
			break;
		}
	}

	private float DegreesToTarget()
	{
		Vector3 forward = animController.forward.forward;
		forward.y = 0f;
		Vector3 to = m_blackboard.CurrentWaypoint - animController.forward.position;
		to.y = 0f;
		return Vector3.Angle(forward, to);
	}

	static PlayerHandler()
	{
		FMOD_MOVEMENT_TYPE_PARAM = "moveType";
		FMOD_ISATTACKING_PARAM = "isAttacking";
	}
}
