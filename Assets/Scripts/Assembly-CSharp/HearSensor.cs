using UnityEngine;

public class HearSensor : Sensor
{
	[SerializeField]
	private bool m_isDisabled;

	private Vector3 m_lastNoisePosition = new Vector3(float.NaN, float.NaN, float.NaN);

	private float m_timer;

	private float m_maxTimer = 1f;

	private bool m_heardNoise;

	private float m_noiseLoudness;

	[SerializeField]
	private bool m_debug;

	public Vector3 LastNoisePosition
	{
		get
		{
			return m_lastNoisePosition;
		}
		set
		{
			m_lastNoisePosition = value;
		}
	}

	public bool DidHearNoise
	{
		get
		{
			return m_heardNoise;
		}
	}

	public float NoiseLoudness
	{
		get
		{
			return m_noiseLoudness;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		m_timer -= Time.deltaTime;
		if (m_timer <= 0f && m_heardNoise)
		{
			m_heardNoise = false;
			m_lastNoisePosition = VectorUtils.NaN;
			m_noiseLoudness = 0f;
		}
	}

	public void HeardNoise(Vector3 position, float loudness)
	{
		if (!m_isDisabled)
		{
			float num = loudness - m_noiseLoudness;
			num = 1f - num;
			float num2 = Random.Range(0f, 1f);
			if (m_noiseLoudness <= 0f || num2 >= num)
			{
				m_noiseLoudness = loudness;
				m_lastNoisePosition = position;
				m_heardNoise = true;
				m_timer = m_maxTimer;
			}
		}
	}

	public override bool RunSensor()
	{
		if (m_isDisabled)
		{
			return false;
		}
		return false;
	}

	private void OnDrawGizmos()
	{
		if (m_debug)
		{
			if (m_heardNoise)
			{
				Gizmos.color = Color.red;
			}
			else
			{
				Gizmos.color = Color.green;
			}
			float num = 0.3f;
			Gizmos.DrawWireCube(base.transform.position, new Vector3(num, num, num));
		}
	}
}
