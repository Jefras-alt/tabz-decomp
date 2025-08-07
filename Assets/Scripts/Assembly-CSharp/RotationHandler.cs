using UnityEngine;

public class RotationHandler : MonoBehaviour
{
	private PhysicsAmimationController animController;

	[SerializeField]
	private Transform m_rotationTarget;

	public Rigidbody turnRig;

	public Vector3 OffsetVector;

	private ZombieBlackboard m_blackboard;

	private Vector3 offsetPreference;

	private bool done;

	private PhotonView mPhotonView;

	public Transform RotationTarget
	{
		get
		{
			return m_rotationTarget;
		}
		set
		{
			m_rotationTarget = value;
		}
	}

	private void Awake()
	{
		mPhotonView = GetComponent<PhotonView>();
		Vector3 insideUnitSphere = Random.insideUnitSphere;
		if (mPhotonView.isMine)
		{
			offsetPreference = insideUnitSphere;
			offsetPreference.y = 0f;
			mPhotonView.RPC("RecieveOffset", PhotonTargets.OthersBuffered, offsetPreference);
		}
	}

	private void Start()
	{
		m_blackboard = GetComponent<ZombieBlackboard>();
		animController = GetComponent<PhysicsAmimationController>();
	}

	[PunRPC]
	public void RecieveOffset(Vector3 offset)
	{
		offsetPreference = offset;
	}

	private void Update()
	{
		if (m_blackboard.Fallen > 0f)
		{
			return;
		}
		FetchNextWaypoint();
		if (m_blackboard.Behaviour != BTType.DEFAULT)
		{
			m_rotationTarget = m_blackboard.PickTargetTransform();
			if (!VectorUtils.IsNaN(m_blackboard.CurrentWaypoint))
			{
				OffsetVector = offsetPreference * Vector3.Distance(turnRig.transform.position, m_blackboard.CurrentWaypoint) * 0.9f;
				if (m_blackboard.DistanceToTarget < 3f)
				{
					OffsetVector = Vector3.zero;
				}
				turnRig.AddTorque(animController.forward.InverseTransformPoint(m_blackboard.CurrentWaypoint).x * Vector3.up * m_blackboard.TurnForce);
			}
			else if (m_rotationTarget != null)
			{
				OffsetVector = offsetPreference * Vector3.Distance(turnRig.transform.position, m_rotationTarget.position) * 0.9f;
				if (m_blackboard.DistanceToTarget < 3f)
				{
					OffsetVector = Vector3.zero;
				}
				turnRig.AddTorque(animController.forward.InverseTransformPoint(m_rotationTarget.position + OffsetVector).x * Vector3.up * m_blackboard.TurnForce * 2f);
			}
		}
		else
		{
			turnRig.AddTorque(Mathf.Clamp(animController.forward.InverseTransformPoint(m_rotationTarget.position).x, -5f, 5f) * Vector3.up * m_blackboard.TurnForce);
		}
	}

	private void FetchNextWaypoint()
	{
		if (!VectorUtils.IsNaN(m_blackboard.CurrentWaypoint) && Vector3.Distance(m_blackboard.GroundPosition, m_blackboard.CurrentWaypoint) < 0.5f)
		{
			m_blackboard.CurrentWaypoint = VectorUtils.NaN;
		}
		if (VectorUtils.IsNaN(m_blackboard.CurrentWaypoint) && m_blackboard.Waypoints.Count > 0)
		{
			m_blackboard.CurrentWaypoint = m_blackboard.Waypoints[0];
			m_blackboard.Waypoints.RemoveAt(0);
		}
	}
}
