using UnityEngine;

public class FootGroundChecker : MonoBehaviour
{
	public float secondsSinceGrounded = 1f;

	public Vector3 position;

	public float shakeMultiplier = 1f;

	private PhysicsAmimationController physicsAnim;

	private ZombieBlackboard m_blackboard;

	private PhotonView mPhotonView;

	public string spawnObject = string.Empty;

	private float cd;

	private void Awake()
	{
		secondsSinceGrounded = 1f;
		physicsAnim = base.transform.GetComponentInParent<PhysicsAmimationController>();
		m_blackboard = base.transform.GetComponentInParent<ZombieBlackboard>();
		mPhotonView = GetComponentInParent<PhotonView>();
	}

	private void Update()
	{
		secondsSinceGrounded += Time.deltaTime;
	}

	private void OnCollisionStay(Collision collision)
	{
		if (!(collision.transform.root == base.transform.root) && !(collision.transform.root.tag == "Player") && !(Vector3.Angle(collision.contacts[0].normal, Vector3.up) > 60f))
		{
			secondsSinceGrounded = 0f;
			position = collision.contacts[0].point;
			physicsAnim.groundNormal = collision.contacts[0].normal;
			m_blackboard.GroundPosition = collision.contacts[0].point;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!(collision.transform.root == base.transform.root) && !(collision.transform.root.tag == "Player") && !(Vector3.Angle(collision.contacts[0].normal, Vector3.up) > 60f))
		{
			if (collision.relativeVelocity.magnitude > 5f && spawnObject != string.Empty && secondsSinceGrounded > 0.5f)
			{
				Object.Instantiate(Resources.Load(spawnObject), collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
			}
			secondsSinceGrounded = 0f;
			position = collision.contacts[0].point;
			if (mPhotonView.isMine)
			{
				screenShake.AddShake(collision.relativeVelocity.magnitude * 0.1f * shakeMultiplier, 0.1f);
			}
			physicsAnim.groundNormal = collision.contacts[0].normal;
			m_blackboard.GroundPosition = collision.contacts[0].point;
		}
	}
}
