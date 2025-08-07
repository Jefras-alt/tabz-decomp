using UnityEngine;

public class RaycastProjectile : MonoBehaviour
{
	public float rayLength = 2f;

	private ProjectileHit projectileHit;

	private RaycastHit hit;

	private Ray ray;

	public Transform shooterRoot;

	private PhotonPlayer mPhotonPlayer;

	public LayerMask mask;

	private bool done;

	[SerializeField]
	private float m_BulletLifeTime = 2f;

	private float m_CurrentTimer;

	private void Awake()
	{
		Initialize();
		CheckHit();
		m_CurrentTimer = m_BulletLifeTime;
	}

	private void Initialize()
	{
		projectileHit = GetComponent<ProjectileHit>();
	}

	private void Update()
	{
		CheckHit();
		Tick();
	}

	private void Tick()
	{
		m_CurrentTimer -= Time.deltaTime;
		if (m_CurrentTimer < 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void CheckHit()
	{
		if (!done)
		{
			ray = new Ray(base.transform.position, base.transform.forward);
			Physics.Raycast(ray, out hit, rayLength, mask);
			if ((bool)hit.transform && hit.transform.root != shooterRoot)
			{
				projectileHit.Hit(hit.transform.GetComponent<Rigidbody>(), hit.point, hit.normal, base.transform.forward, mPhotonPlayer);
				done = true;
			}
		}
	}

	internal void InitSender(PhotonPlayer sender)
	{
		mPhotonPlayer = sender;
	}
}
