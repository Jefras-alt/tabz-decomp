using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlayerSoundHandler : MonoBehaviour
{
	private const string GROUND_TYPE_PARAM = "groundType";

	private const string MOVEMENT_STATE_PARAM = "moveType";

	private EventInstance mFootStepEvent;

	private EventInstance mJumpEvent;

	private EventInstance mProneEvent;

	private string mWalkingSound;

	private Transform mChachedTransform;

	private static SoundEventsManager mSoundEventHandler;

	private void Awake()
	{
		if (mSoundEventHandler == null)
		{
			mSoundEventHandler = Object.FindObjectOfType<SoundEventsManager>();
		}
		mWalkingSound = mSoundEventHandler.WalkingEvent;
		string jumpEvent = mSoundEventHandler.JumpEvent;
		string proneEvent = mSoundEventHandler.ProneEvent;
		if (!string.IsNullOrEmpty(mWalkingSound))
		{
			mFootStepEvent = RuntimeManager.CreateInstance(mWalkingSound);
		}
		if (!string.IsNullOrEmpty(jumpEvent))
		{
			mJumpEvent = RuntimeManager.CreateInstance(jumpEvent);
		}
		if (!string.IsNullOrEmpty(proneEvent))
		{
			mProneEvent = RuntimeManager.CreateInstance(proneEvent);
			mProneEvent.setParameterValue("moveType", 1f);
		}
		mChachedTransform = GetComponent<PhysicsAmimationController>().mainRig.transform;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Prone()
	{
		mProneEvent.set3DAttributes(mChachedTransform.To3DAttributes());
		mProneEvent.start();
	}

	public void Jump()
	{
		mJumpEvent.set3DAttributes(mChachedTransform.To3DAttributes());
		mJumpEvent.start();
	}

	public void FootStep(TerrainMaterialChecker.GroundTypes type, int movementState)
	{
		if (!string.IsNullOrEmpty(mWalkingSound))
		{
			mFootStepEvent.set3DAttributes(mChachedTransform.To3DAttributes());
			mFootStepEvent.setParameterValue("groundType", (float)type);
			mFootStepEvent.setParameterValue("moveType", movementState);
			mFootStepEvent.start();
		}
	}
}
