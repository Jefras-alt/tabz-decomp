using UnityEngine;

public class PhysicsAmimationController : MonoBehaviour
{
	public Rigidbody mainRig;

	[HideInInspector]
	public Vector3 groundedForward;

	[HideInInspector]
	public Vector3 groundedRight;

	[HideInInspector]
	public Vector3 groundedCamForward;

	[HideInInspector]
	public Vector3 groundedCamRight;

	[HideInInspector]
	public Transform forward;

	[HideInInspector]
	public Transform right;

	[HideInInspector]
	public Transform up;

	[HideInInspector]
	public Transform center;

	public float maxAngular = 500f;

	public float drag = 5f;

	public float angularDrag = 1f;

	public float switchAngle;

	[HideInInspector]
	public float stepLength = 0.5f;

	public float stepLengthWalk = 0.5f;

	public float stepLengthRun = 0.25f;

	private JointAnimation[] jointAnimations;

	private JointAnimationExtended[] jointAnimationsExtended;

	private float counter;

	[HideInInspector]
	public bool leftLeg = true;

	[HideInInspector]
	private PlayerHandler player;

	private StandHandler standHandler;

	private BalanceHandler balanceHandler;

	public Vector3 groundNormal;

	private int currentState;

	private ZombieBlackboard m_blackboard;

	private Rigidbody[] mRigidbodiesAttached;

	private PlayerSoundHandler mPlayerSoundHandler;

	private TerrainMaterialChecker.GroundTypes mCurrentGroundType = TerrainMaterialChecker.GroundTypes.None;

	private static TerrainMaterialChecker mTerrainChecker;

	private MovementHandler move;

	private PhotonView mPhotonView;

	public int CurrentMovementState
	{
		get
		{
			return player.MovementSoundParameter;
		}
	}

	private void Awake()
	{
		mPlayerSoundHandler = GetComponent<PlayerSoundHandler>();
		move = GetComponent<MovementHandler>();
		mTerrainChecker = Object.FindObjectOfType<TerrainMaterialChecker>();
		mRigidbodiesAttached = GetComponentsInChildren<Rigidbody>();
		mPhotonView = GetComponent<PhotonView>();
		InitRigidbodies(mRigidbodiesAttached);
	}

	private void InitRigidbodies(Rigidbody[] RigidbodiesAttached)
	{
		for (byte b = 0; b < RigidbodiesAttached.Length; b++)
		{
			Rigidbody rigidbody = RigidbodiesAttached[b];
			rigidbody.gameObject.AddComponent<RigidBodyIndexHolder>().Init(b);
		}
	}

	[PunRPC]
	public void AddForce(byte index, Vector3 force, int forceMode = 1)
	{
		mRigidbodiesAttached[index].AddForce(force, (ForceMode)forceMode);
	}

	private void Start()
	{
		m_blackboard = GetComponent<ZombieBlackboard>();
		standHandler = GetComponent<StandHandler>();
		player = GetComponent<PlayerHandler>();
		balanceHandler = GetComponent<BalanceHandler>();
		Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>(true);
		foreach (Rigidbody rigidbody in componentsInChildren)
		{
			rigidbody.maxAngularVelocity = maxAngular;
			rigidbody.drag = drag;
			rigidbody.angularDrag = angularDrag;
		}
		forward = GetComponentInChildren<ForwardTag>().transform;
		right = GetComponentInChildren<RightTag>().transform;
		up = GetComponentInChildren<UpTag>().transform;
		center = GetComponentInChildren<CenterTag>().transform;
		if (Random.value > 0.5f)
		{
			leftLeg = false;
		}
		jointAnimations = GetComponentsInChildren<JointAnimation>();
		jointAnimationsExtended = GetComponentsInChildren<JointAnimationExtended>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.V) && !TABZChat.DisableInput && mPhotonView.isMine)
		{
			mPhotonView.RPC("Fall", PhotonTargets.All, 3f);
		}
		if (mainRig.velocity.magnitude > 1f || mainRig.angularVelocity.magnitude > 0.5f)
		{
			CheckSwitch();
		}
		SetRotations();
		if (m_blackboard.ReasonablyGrounded || !mPhotonView.isMine)
		{
			counter += Time.deltaTime;
		}
	}

	private void SwitchLegs()
	{
		for (int i = 0; i < jointAnimationsExtended.Length; i++)
		{
			if (jointAnimationsExtended[i].isLeft == leftLeg)
			{
				jointAnimationsExtended[i].Animate(currentState);
			}
		}
		counter = 0f;
		leftLeg = !leftLeg;
		mCurrentGroundType = mTerrainChecker.GetCurrentMaterialName(mainRig.position);
		mPlayerSoundHandler.FootStep(mCurrentGroundType, CurrentMovementState);
	}

	private void CheckSwitch()
	{
		if (!(counter < stepLength) && (switchAngle == 0f || !(balanceHandler.legAngle < switchAngle)))
		{
			SwitchLegs();
		}
	}

	public void Walk()
	{
		if (!(m_blackboard.Fallen > 0f) || !mPhotonView.isMine)
		{
			currentState = 0;
			stepLength = stepLengthWalk;
			if (m_blackboard.ReasonablyGrounded)
			{
				CheckSwitch();
			}
			for (int i = 0; i < jointAnimations.Length; i++)
			{
				jointAnimations[i].Animate(leftLeg, 0);
			}
		}
	}

	public void Run()
	{
		if (!(m_blackboard.Fallen > 0f) || !mPhotonView.isMine)
		{
			currentState = 1;
			stepLength = stepLengthRun;
			if (m_blackboard.ReasonablyGrounded)
			{
				CheckSwitch();
			}
			for (int i = 0; i < jointAnimations.Length; i++)
			{
				jointAnimations[i].Animate(leftLeg, 1);
			}
		}
	}

	public void Back()
	{
		if (!(m_blackboard.Fallen > 0f) || !mPhotonView.isMine)
		{
			currentState = 2;
			stepLength = stepLengthWalk;
			if (m_blackboard.ReasonablyGrounded)
			{
				CheckSwitch();
			}
			for (int i = 0; i < jointAnimations.Length; i++)
			{
				jointAnimations[i].Animate(leftLeg, 2);
			}
		}
	}

	private void SetRotations()
	{
		groundedForward = forward.forward;
		groundedForward.y = Vector3.Cross(groundNormal, right.forward).y;
		groundedForward = groundedForward.normalized;
		groundedRight = right.forward;
		groundedRight.y = 0f;
		groundedRight = groundedRight.normalized;
		if (player.AI)
		{
			groundedCamForward = forward.forward;
			groundedCamForward.y = Vector3.Cross(groundNormal, right.forward).y;
			groundedCamForward = groundedCamForward.normalized;
			groundedCamRight = groundedRight;
			groundedCamRight.y = 0f;
			groundedCamRight = groundedCamRight.normalized;
		}
		else
		{
			groundedCamForward = Camera.main.transform.forward;
			groundedCamForward.y = Vector3.Cross(Camera.main.transform.right, groundNormal).y;
			groundedCamForward = groundedCamForward.normalized;
			groundedCamRight = Camera.main.transform.right;
			groundedCamRight.y = 0f;
			groundedCamRight = groundedCamRight.normalized;
		}
	}
}
