using Photon;
using UnityEngine;

public class NetworkPlayer : Photon.MonoBehaviour, IPunObservable
{
	public static PhotonView LocalPlayerPhotonView;

	private PhotonView mPhotonView;

	private const float DISTANCE_THRESHOLD = 5f;

	[SerializeField]
	private Transform mTargetCollider;

	[SerializeField]
	private Transform mHipTransform;

	private PhysicsAmimationController mPhysicsController;

	private PlayerHandler mPlayerHandler;

	private CameraMovement mCameraMovement;

	private StandHandler mStandHandler;

	private byte mLastMovementState = 3;

	private Vector3 mLastPosition;

	private Vector3 mLastRotation;

	private Vector3 syncVelocity;

	public Transform TargetCollider
	{
		get
		{
			return mTargetCollider;
		}
	}

	private void Awake()
	{
		mPhotonView = GetComponent<PhotonView>();
		mPlayerHandler = GetComponent<PlayerHandler>();
		mPhysicsController = GetComponent<PhysicsAmimationController>();
		mCameraMovement = GetComponentInChildren<CameraMovement>();
		mStandHandler = GetComponent<StandHandler>();
		mLastPosition = mHipTransform.position;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		if (!mPhotonView.isMine)
		{
			Vector3 vector = mLastPosition - mHipTransform.position;
			if (vector.magnitude > 1f)
			{
				vector = vector.normalized;
			}
			syncVelocity += vector;
			syncVelocity *= 0.5f;
			base.transform.position += syncVelocity * 0.2f;
			mCameraMovement.ApplyClientCameraRotation(mLastRotation);
			if (Vector3.Distance(mHipTransform.position, mLastPosition) > 5f)
			{
				mHipTransform.position = mLastPosition;
			}
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			Vector3 position = mHipTransform.position;
			Vector3 cameraRotation = mCameraMovement.CameraRotation;
			byte currentMovementState = mPlayerHandler.CurrentMovementState;
			byte standMultiplier = mStandHandler.standMultiplier;
			stream.SendNext(position);
			stream.SendNext(cameraRotation);
			stream.SendNext(currentMovementState);
			stream.SendNext(standMultiplier);
		}
		else if (stream.isReading)
		{
			Vector3 vector = (Vector3)stream.ReceiveNext();
			Vector3 vector2 = (Vector3)stream.ReceiveNext();
			byte newMovementState = (byte)stream.ReceiveNext();
			byte newStandState = (byte)stream.ReceiveNext();
			ApplyMovementState(newMovementState);
			ApplyStandState(newStandState);
			mLastPosition = vector;
			mLastRotation = vector2;
		}
	}

	private void ApplyMovementState(byte newMovementState)
	{
		mPlayerHandler.SetNewClientState(newMovementState, false);
	}

	private void ApplyStandState(byte newStandState)
	{
		mStandHandler.SetNewClientState(newStandState);
	}
}
