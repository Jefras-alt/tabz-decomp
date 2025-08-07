using UnityEngine;

public class ZombieFighting : MonoBehaviour
{
	private ZombieBlackboard m_blackboard;

	private PhysicsAmimationController anim;

	private Rigidbody leftHand;

	private Rigidbody rightHand;

	private FixedJoint leftJoint;

	private FixedJoint rightJoint;

	private float dmgCD;

	public float range = 1.5f;

	public float reachRange = 10f;

	public float reachForce = 3000f;

	public float dmg = 6f;

	public float knockbackAwalys = -2f;

	public float knockback = -2f;

	public float knockbackUp;

	private PhotonView mPhotonView;

	private void Awake()
	{
		mPhotonView = GetComponent<PhotonView>();
	}

	private void Start()
	{
		m_blackboard = GetComponent<ZombieBlackboard>();
		anim = GetComponent<PhysicsAmimationController>();
		rightHand = GetComponentInChildren<RightArmTag>().GetComponent<Rigidbody>();
		leftHand = GetComponentInChildren<LeftArmTag>().GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if ((bool)m_blackboard.TargetTransform && m_blackboard.DistanceToTarget < 5f && m_blackboard.TargetTransform.name != "POI")
		{
			ReachForTarget();
		}
	}

	private void Update()
	{
		if (m_blackboard.Fallen > 0f)
		{
			dmgCD = 0f;
		}
		if ((bool)m_blackboard.TargetTransform && m_blackboard.DistanceToTarget < 5f && m_blackboard.TargetTransform.name != "POI")
		{
			ReachForTarget();
			dmgCD += Time.deltaTime;
		}
		m_blackboard.AttackCooldownLeft -= Time.deltaTime;
		m_blackboard.AttackCooldownRight -= Time.deltaTime;
		if (Random.Range(0f, 1f) > 0.5f)
		{
			TryLeftAttack();
		}
		else
		{
			TryRightAttack();
		}
	}

	private void ReachForTarget()
	{
		if (!(m_blackboard.DistanceToTarget < reachRange) || !(m_blackboard.Fallen < -1f))
		{
			return;
		}
		leftHand.AddForce((m_blackboard.TargetTransform.position - leftHand.position).normalized * reachForce * Time.deltaTime, ForceMode.Acceleration);
		rightHand.AddForce((m_blackboard.TargetTransform.position - rightHand.position).normalized * reachForce * Time.deltaTime, ForceMode.Acceleration);
		if (!(m_blackboard.DistanceToTarget < range))
		{
			return;
		}
		Rigidbody componentInParent = m_blackboard.TargetTransform.GetComponentInParent<Rigidbody>();
		if (knockbackAwalys != 0f)
		{
			if (componentInParent.drag > 1f)
			{
				componentInParent.velocity += (m_blackboard.SelfTransform.position - m_blackboard.TargetTransform.position) * (0f - knockbackAwalys);
			}
			else
			{
				componentInParent.velocity += (m_blackboard.SelfTransform.position - m_blackboard.TargetTransform.position) * (0f - knockbackAwalys) * 0.03f;
			}
		}
		PhotonView componentInParent2 = componentInParent.GetComponentInParent<PhotonView>();
		if (dmgCD > 1f)
		{
			if (knockback != 0f)
			{
				if (componentInParent.drag > 1f)
				{
					componentInParent.velocity += (m_blackboard.SelfTransform.position - m_blackboard.TargetTransform.position).normalized * (0f - knockback);
					if (knockbackUp != 0f)
					{
						componentInParent.velocity += Vector3.up * knockbackUp;
					}
				}
				else
				{
					componentInParent.velocity += (m_blackboard.SelfTransform.position - m_blackboard.TargetTransform.position).normalized * (0f - knockback) * 0.05f;
					if (knockbackUp != 0f)
					{
						componentInParent.velocity += Vector3.up * knockbackUp * 0.05f;
					}
				}
			}
			componentInParent2.RPC("TakeDamage", PhotonTargets.All, dmg, null, componentInParent2.GetComponent<HealthHandler>().currentHealth <= dmg);
			dmgCD = 0f;
		}
		if (componentInParent2.isMine)
		{
			screenShake.AddShake(1f, 0.2f);
		}
	}

	private void ExecuteLeftAttack()
	{
	}

	private void ExecuteRightAttack()
	{
	}

	public void TryLeftAttack()
	{
		if (m_blackboard.AttackCooldownLeft <= 0f && m_blackboard.BehaviorState == BehaviorState.ATTACK)
		{
			ExecuteLeftAttack();
			m_blackboard.AttackCooldownLeft = m_blackboard.AttackRateLeft;
		}
	}

	public void TryRightAttack()
	{
		if (m_blackboard.AttackCooldownRight <= 0f && m_blackboard.BehaviorState == BehaviorState.ATTACK)
		{
			ExecuteRightAttack();
			m_blackboard.AttackCooldownRight = m_blackboard.AttackRateRight;
		}
	}
}
