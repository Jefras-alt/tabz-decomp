using UnityEngine;

public class NoiseSpawner : MonoBehaviour
{
	[SerializeField]
	private float m_interval = 0.1f;

	private float m_counter;

	[SerializeField]
	private LayerMask m_hearMask;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_noiseLoudness = 10f;

	[SerializeField]
	private float m_distance = 10f;

	[SerializeField]
	private bool m_debug = true;

	private void Start()
	{
		m_counter = m_interval;
	}

	private void Update()
	{
		m_counter -= Time.deltaTime;
	}

	public void Trigger()
	{
		if (!(m_counter > 0f))
		{
			m_counter = m_interval;
			Collider[] colliders = Physics.OverlapSphere(base.transform.position, m_distance, m_hearMask);
			NotifyHeard(colliders);
		}
	}

	private void NotifyHeard(Collider[] colliders)
	{
		foreach (Collider collider in colliders)
		{
			HearSensor component = collider.GetComponent<HearSensor>();
			if (!(component == null))
			{
				component.HeardNoise(base.transform.position, m_noiseLoudness);
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (m_debug)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(base.transform.position, m_distance);
		}
	}
}
