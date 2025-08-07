using UnityEngine;

public class StandHandler : MonoBehaviour
{
	public bool ignoreAmountOfLegs;

	public Rigidbody[] rigsToLift;

	public float[] liftForces;

	public float sinceGrounded;

	private FootGroundChecker[] feet;

	private float multiplier;

	private float heightMultiplier = 1f;

	private float feetTouchingGroundMultiplier = 1f;

	public float reasonablyGroundedThreshold = 0.3f;

	public float groundedThreshold = 0.05f;

	public AnimationCurve heightMultiplierCurve;

	private PhysicsAmimationController animController;

	private PlayerSoundHandler mPlayerSoundHandler;

	private ZombieBlackboard m_blackboard;

	private PlayerHandler player;

	private HealthHandler health;

	private bool hasLanded;

	public byte standMultiplier = 1;

	private PhotonView mPhotonView;

	public bool ignoreGraviy;

	private Rigidbody[] m_rigidbodies;

	private float[] m_rigStartDrag;

	private float[] m_rigStartAngDrag;

	private PhysicMaterial[] myMaterials;

	private Collider[] colliders;

	private bool m_isStanding;

	private void Awake()
	{
		mPhotonView = GetComponent<PhotonView>();
		mPlayerSoundHandler = GetComponent<PlayerSoundHandler>();
	}

	private void Start()
	{
		health = GetComponent<HealthHandler>();
		m_blackboard = GetComponent<ZombieBlackboard>();
		player = GetComponent<PlayerHandler>();
		animController = base.transform.root.GetComponent<PhysicsAmimationController>();
		feet = GetComponentsInChildren<FootGroundChecker>();
		m_rigidbodies = GetComponentsInChildren<Rigidbody>();
		colliders = GetComponentsInChildren<Collider>();
		m_rigStartDrag = new float[m_rigidbodies.Length];
		m_rigStartAngDrag = new float[m_rigidbodies.Length];
		myMaterials = new PhysicMaterial[colliders.Length];
		for (int i = 0; i < m_rigidbodies.Length; i++)
		{
			m_rigStartDrag[i] = m_rigidbodies[i].drag;
			m_rigStartAngDrag[i] = m_rigidbodies[i].angularDrag;
		}
		for (int j = 0; j < colliders.Length; j++)
		{
			myMaterials[j] = colliders[j].material;
		}
	}

	private void Update()
	{
		if (!player.AI && mPhotonView.isMine && !TABZChat.DisableInput && m_blackboard.Fallen < -0.5f)
		{
			if (Input.GetKeyDown(KeyCode.LeftControl))
			{
				if (standMultiplier == 2)
				{
					standMultiplier = 1;
				}
				else
				{
					standMultiplier = 2;
				}
			}
			if (Input.GetKeyDown(KeyCode.Z))
			{
				if (standMultiplier == 10)
				{
					standMultiplier = 1;
				}
				else
				{
					standMultiplier = 10;
				}
				mPlayerSoundHandler.Prone();
			}
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Space))
			{
				standMultiplier = 1;
			}
		}
		m_blackboard.Fallen -= Time.deltaTime;
		sinceGrounded += Time.deltaTime;
		float num = 0f;
		int num2 = 0;
		for (int i = 0; i < feet.Length; i++)
		{
			if (feet[i].secondsSinceGrounded < 0.05f)
			{
				if (sinceGrounded > 1.5f && hasLanded && mPhotonView.isMine)
				{
					mPhotonView.RPC("TakeDamage", PhotonTargets.All, sinceGrounded * 50f, PhotonNetwork.player, health.currentHealth <= sinceGrounded * 50f);
					mPhotonView.RPC("Fall", PhotonTargets.All, sinceGrounded, true);
				}
				hasLanded = true;
				sinceGrounded = 0f;
				num += Mathf.Abs(feet[i].position.y - animController.center.position.y);
				num2++;
			}
		}
		if (num2 != 0)
		{
			num /= (float)num2;
		}
		feetTouchingGroundMultiplier = (float)num2 / (float)feet.Length;
		if (feet.Length == 2 || ignoreAmountOfLegs)
		{
			feetTouchingGroundMultiplier = 1f;
		}
		if (num != 0f)
		{
			heightMultiplier = heightMultiplierCurve.Evaluate(num * (float)(int)standMultiplier) * feetTouchingGroundMultiplier;
		}
		if (sinceGrounded < groundedThreshold)
		{
			m_blackboard.Grounded = true;
		}
		else
		{
			m_blackboard.Grounded = false;
		}
		if (sinceGrounded < reasonablyGroundedThreshold)
		{
			m_blackboard.ReasonablyGrounded = true;
		}
		else
		{
			m_blackboard.ReasonablyGrounded = false;
		}
		if (!mPhotonView.isMine && ignoreGraviy)
		{
			sinceGrounded = 0f;
			multiplier = 1f;
			heightMultiplier = 1f;
		}
		if (standMultiplier != 10)
		{
			Stand();
		}
		if (!m_blackboard.Grounded && multiplier > 0f)
		{
			multiplier -= Time.deltaTime * 5f;
		}
		if (m_blackboard.Grounded)
		{
			multiplier = 1f;
		}
	}

	private void Stand()
	{
		if (m_blackboard.Fallen > 0f)
		{
			return;
		}
		if (!m_isStanding)
		{
			m_isStanding = true;
			for (int i = 0; i < m_rigidbodies.Length; i++)
			{
				m_rigidbodies[i].drag = m_rigStartDrag[i];
				m_rigidbodies[i].angularDrag = m_rigStartAngDrag[i];
			}
			for (int j = 0; j < colliders.Length; j++)
			{
				colliders[j].material = myMaterials[j];
			}
		}
		for (int k = 0; k < rigsToLift.Length; k++)
		{
			if (heightMultiplier != 0f && multiplier * liftForces[k] * heightMultiplier != 0f)
			{
				rigsToLift[k].AddForce(Vector3.up * liftForces[k] * multiplier * (heightMultiplier + Mathf.Clamp(animController.mainRig.velocity.magnitude * 0.02f, 0f, 1f)) * Time.deltaTime, ForceMode.Acceleration);
			}
		}
	}

	[PunRPC]
	public void Fall(float time)
	{
		if (m_blackboard == null)
		{
			return;
		}
		if (m_blackboard.Fallen < -1f)
		{
			m_blackboard.Fallen = time;
		}
		if (m_isStanding)
		{
			m_isStanding = false;
			for (int i = 0; i < m_rigidbodies.Length; i++)
			{
				m_rigidbodies[i].drag = 0.5f;
				m_rigidbodies[i].angularDrag = 0.5f;
			}
			for (int j = 0; j < colliders.Length; j++)
			{
				colliders[j].material = null;
			}
		}
		if (rigsToLift[0].velocity.magnitude > 10f && rigsToLift[0].velocity.y > -5f)
		{
			Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				componentsInChildren[k].velocity *= 0.01f;
			}
		}
	}

	[PunRPC]
	public void Fall(float time, bool hardFall)
	{
		if (m_blackboard == null)
		{
			return;
		}
		m_blackboard.Fallen = time;
		if (m_isStanding)
		{
			m_isStanding = false;
			for (int i = 0; i < m_rigidbodies.Length; i++)
			{
				m_rigidbodies[i].drag = 0.5f;
				m_rigidbodies[i].angularDrag = 0.5f;
			}
			for (int j = 0; j < colliders.Length; j++)
			{
				colliders[j].material = null;
			}
		}
		if (rigsToLift[0].velocity.magnitude > 10f && rigsToLift[0].velocity.y > -5f)
		{
			Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				componentsInChildren[k].velocity *= 0.01f;
			}
		}
	}

	public void SetNewClientState(byte newStandState)
	{
		standMultiplier = newStandState;
	}
}
