using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieBlackboard : Blackboard
{
	[Header("Movement")]
	[SerializeField]
	private float m_runMultiplier = 1f;

	[SerializeField]
	private float m_walkMultiplier = 0.3f;

	[SerializeField]
	private float m_crouchMultiplier = 0.3f;

	[SerializeField]
	private float m_backMultiplier = -0.5f;

	[SerializeField]
	private float m_sideMultiplier = 0.5f;

	[SerializeField]
	private float m_jumpForce = 300f;

	[SerializeField]
	private float m_gravityForce = 4000f;

	[Space(10f)]
	[Header("Standing")]
	[SerializeField]
	private bool m_grounded;

	[SerializeField]
	private bool m_reasonablyGrounded;

	[SerializeField]
	private float m_fallen;

	[Space(10f)]
	[Header("Fighting")]
	[SerializeField]
	private float m_attackRange = 3f;

	private float m_distanceToTarget;

	[SerializeField]
	private float m_attackRateLeft = 1f;

	private float m_attackCooldownLeft = 1f;

	[SerializeField]
	private float m_attackRateRight = 1f;

	private float m_attackCooldownRight = 1f;

	[Space(10f)]
	[Header("Rotation")]
	[SerializeField]
	private float m_turnForce = 200f;

	[Space(10f)]
	[Header("Behaviour")]
	private EyeSensor m_eyes;

	private HearSensor m_ears;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_investigateLoudnessThreshold = 11f;

	[SerializeField]
	private Transform m_targetTransform;

	[SerializeField]
	private MovementState m_movementState;

	[SerializeField]
	private BehaviorState m_behaviorState;

	[SerializeField]
	private Transform m_poi;

	private Transform m_selfTransform;

	[SerializeField]
	private Transform m_HipTransform;

	private NavMeshPath m_path;

	private List<Vector3> m_waypoints = new List<Vector3>();

	private Vector3 m_currentWaypoint = VectorUtils.NaN;

	private Vector3 m_groundPos;

	[SerializeField]
	private bool m_debug;

	private PhotonView mPhotonView;

	public float RunMultiplier
	{
		get
		{
			return m_runMultiplier;
		}
		set
		{
			m_runMultiplier = value;
		}
	}

	public float WalkMultiplier
	{
		get
		{
			return m_walkMultiplier;
		}
		set
		{
			m_walkMultiplier = value;
		}
	}

	public float CrouchMultiplier
	{
		get
		{
			return m_crouchMultiplier;
		}
		set
		{
			m_crouchMultiplier = value;
		}
	}

	public float BackMultiplier
	{
		get
		{
			return m_backMultiplier;
		}
		set
		{
			m_backMultiplier = value;
		}
	}

	public float SideMultiplier
	{
		get
		{
			return m_sideMultiplier;
		}
		set
		{
			m_sideMultiplier = value;
		}
	}

	public float JumpForce
	{
		get
		{
			return m_jumpForce;
		}
		set
		{
			m_jumpForce = value;
		}
	}

	public float GravityForce
	{
		get
		{
			return m_gravityForce;
		}
		set
		{
			m_gravityForce = value;
		}
	}

	public bool Grounded
	{
		get
		{
			return m_grounded;
		}
		set
		{
			m_grounded = value;
		}
	}

	public bool ReasonablyGrounded
	{
		get
		{
			return m_reasonablyGrounded;
		}
		set
		{
			m_reasonablyGrounded = value;
		}
	}

	public float Fallen
	{
		get
		{
			return m_fallen;
		}
		set
		{
			m_fallen = value;
		}
	}

	public float AttackRange
	{
		get
		{
			return m_attackRange;
		}
		set
		{
			m_attackRange = value;
		}
	}

	public float DistanceToTarget
	{
		get
		{
			return m_distanceToTarget;
		}
		set
		{
			m_distanceToTarget = value;
		}
	}

	public float AttackRateLeft
	{
		get
		{
			return m_attackRateLeft;
		}
		set
		{
			m_attackRateLeft = value;
		}
	}

	public float AttackCooldownLeft
	{
		get
		{
			return m_attackCooldownLeft;
		}
		set
		{
			m_attackCooldownLeft = value;
		}
	}

	public float AttackRateRight
	{
		get
		{
			return m_attackRateRight;
		}
		set
		{
			m_attackRateRight = value;
		}
	}

	public float AttackCooldownRight
	{
		get
		{
			return m_attackCooldownRight;
		}
		set
		{
			m_attackCooldownRight = value;
		}
	}

	public float TurnForce
	{
		get
		{
			return m_turnForce;
		}
		set
		{
			m_turnForce = value;
		}
	}

	public EyeSensor EyeSensor
	{
		get
		{
			return m_eyes;
		}
	}

	public HearSensor EarSensor
	{
		get
		{
			return m_ears;
		}
	}

	public float InvestigateLoudnessThreshold
	{
		get
		{
			return m_investigateLoudnessThreshold;
		}
	}

	public Transform TargetTransform
	{
		get
		{
			return m_targetTransform;
		}
		set
		{
			if (mPhotonView.isMine && m_targetTransform != value)
			{
				if (value == m_poi)
				{
					mPhotonView.RPC("TargetChangedPOI", PhotonTargets.Others, m_poi.position);
				}
				else
				{
					PhotonView componentInParent = value.GetComponentInParent<PhotonView>();
					mPhotonView.RPC("TargetChangedPlayer", PhotonTargets.Others, componentInParent.viewID);
				}
			}
			m_targetTransform = value;
		}
	}

	public MovementState MovementState
	{
		get
		{
			return m_movementState;
		}
		set
		{
			m_movementState = value;
		}
	}

	public BehaviorState BehaviorState
	{
		get
		{
			return m_behaviorState;
		}
		set
		{
			m_behaviorState = value;
		}
	}

	public Transform PointOfInterest
	{
		get
		{
			return m_poi;
		}
	}

	public Transform SelfTransform
	{
		get
		{
			return m_selfTransform;
		}
		set
		{
			m_selfTransform = value;
		}
	}

	public NavMeshPath Path
	{
		get
		{
			return m_path;
		}
		set
		{
			m_path = value;
		}
	}

	public List<Vector3> Waypoints
	{
		get
		{
			return m_waypoints;
		}
		set
		{
			m_waypoints = value;
		}
	}

	public Vector3 CurrentWaypoint
	{
		get
		{
			return m_currentWaypoint;
		}
		set
		{
			m_currentWaypoint = value;
		}
	}

	public Vector3 GroundPosition
	{
		get
		{
			return m_groundPos;
		}
		set
		{
			m_groundPos = value;
		}
	}

	private void Awake()
	{
		mPhotonView = GetComponent<PhotonView>();
		m_eyes = GetComponentInChildren<EyeSensor>();
		if (m_eyes == null)
		{
			Debug.LogWarning("Could not find EyeSensor component!", base.gameObject);
		}
		m_ears = GetComponentInChildren<HearSensor>();
		if (m_ears == null)
		{
			Debug.LogWarning("Could not find HearSensor component!", base.gameObject);
		}
		m_poi = base.transform.Find("POI");
		if (m_poi == null)
		{
			Debug.LogWarning("Could not find Point of Interest transform");
		}
		m_selfTransform = m_HipTransform;
		if (base.Behaviour == BTType.ZOMBIE)
		{
			if (mPhotonView.instantiationData == null)
			{
				return;
			}
			if ((bool)mPhotonView.instantiationData[0])
			{
				Object.Destroy(GetComponentInChildren<ChunkDynamicObject>());
			}
		}
		Init();
	}

	public void Init()
	{
		if (mPhotonView.isMine)
		{
			ServiceLocator.GetService<BTService>().AddZombieBlackboard(this);
		}
	}

	protected override void Start()
	{
	}

	private void OnDestroy()
	{
		ServiceLocator.GetService<BTService>().RemoveZombieBlackboard(this);
	}

	protected override void Update()
	{
		if (!mPhotonView.isMine && !(TargetTransform == null))
		{
			m_distanceToTarget = (TargetTransform.position - SelfTransform.position).magnitude;
		}
	}

	public Transform PickTargetTransform()
	{
		if (m_targetTransform != null)
		{
			return m_targetTransform;
		}
		return null;
	}

	[PunRPC]
	public void TargetChangedPOI(Vector3 pos)
	{
		m_poi.position = pos;
		m_targetTransform = m_poi;
	}

	[PunRPC]
	public void TargetChangedPlayer(int viewID)
	{
		PhotonView photonView = PhotonView.Find(viewID);
		Transform targetCollider = photonView.GetComponent<NetworkPlayer>().TargetCollider;
		m_targetTransform = targetCollider;
	}

	public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		if (mPhotonView.isMine && !(m_targetTransform == null))
		{
			if (m_targetTransform == m_poi)
			{
				mPhotonView.RPC("TargetChangedPOI", newPlayer, m_poi.position);
			}
			else
			{
				PhotonView componentInParent = m_targetTransform.GetComponentInParent<PhotonView>();
				mPhotonView.RPC("TargetChangedPlayer", newPlayer, componentInParent.viewID);
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (!m_debug || m_waypoints == null)
		{
			return;
		}
		Gizmos.color = Color.magenta;
		Vector3 vector = new Vector3(0f, 0.5f, 0f);
		int num = 0;
		foreach (Vector3 waypoint in m_waypoints)
		{
			Gizmos.DrawSphere(waypoint, 0.3f);
			num++;
		}
		for (int i = 0; i < m_waypoints.Count - 1; i++)
		{
			Vector3 vector2 = m_waypoints[i];
			Vector3 to = m_waypoints[i + 1];
			if (i == 0)
			{
				Gizmos.DrawLine(GroundPosition, vector2);
			}
			Gizmos.DrawLine(vector2, to);
		}
		if (m_waypoints.Count <= 0 && TargetTransform != null)
		{
			Gizmos.DrawLine(GroundPosition, TargetTransform.position);
		}
	}
}
