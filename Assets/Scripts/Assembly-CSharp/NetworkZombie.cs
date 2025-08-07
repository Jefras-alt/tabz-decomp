using Photon;
using UnityEngine;

public class NetworkZombie : Photon.MonoBehaviour, IPunObservable
{
	public static PhotonView LocalPlayerPhotonView;

	private PhotonView mPhotonView;

	private const float DISTANCE_THRESHOLD = 10f;

	[SerializeField]
	private Transform mHipTransform;

	private PhysicsAmimationController mPhysicsController;

	private PlayerHandler mPlayerHandler;

	private CameraMovement mCameraMovement;

	private byte mLastMovementState = 3;

	private Vector3 mLastPosition;

	private Vector3 mLastRotation;

	private Vector3 syncVelocity;

	public Transform ball;

	private void Awake()
	{
		mPhotonView = GetComponent<PhotonView>();
		mPlayerHandler = GetComponent<PlayerHandler>();
		mPhysicsController = GetComponent<PhysicsAmimationController>();
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
				vector = vector.normalized * 1f;
			}
			syncVelocity += vector;
			syncVelocity *= 0.7f;
			base.transform.position += syncVelocity * 0.05f;
			if (Vector3.Distance(mHipTransform.position, mLastPosition) > 10f)
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
			byte currentMovementState = mPlayerHandler.CurrentMovementState;
			stream.SendNext(position);
			stream.SendNext(currentMovementState);
		}
		else if (stream.isReading)
		{
			Vector3 vector = (Vector3)stream.ReceiveNext();
			byte newMovementState = (byte)stream.ReceiveNext();
			ApplyMovementState(newMovementState);
			mLastPosition = vector;
		}
	}

	private void ApplyMovementState(byte newMovementState)
	{
		mPlayerHandler.SetNewClientState(newMovementState, true);
	}

	[PunRPC]
	public void NetworkDestroy()
	{
		if (mPhotonView.isMine)
		{
			PhotonNetwork.Destroy(mPhotonView);
		}
	}

	private void OnDestroy()
	{
		NetworkZombieSpawner.OnZombieDied();
	}
}
