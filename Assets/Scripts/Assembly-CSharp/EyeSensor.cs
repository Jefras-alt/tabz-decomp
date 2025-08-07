using UnityEngine;

public class EyeSensor : Sensor
{
	[SerializeField]
	private bool m_isDisabled;

	[SerializeField]
	private LayerMask m_sightMask;

	[SerializeField]
	private LayerMask ignoreMask;

	[SerializeField]
	private Vector3 m_size;

	[SerializeField]
	private Vector3 m_offset;

	[SerializeField]
	private bool m_debug = true;

	private Transform m_target;

	private bool m_isTriggered;

	private ZombieBlackboard m_blackboard;

	public Vector3 Size
	{
		get
		{
			return m_size;
		}
		set
		{
			m_size = value;
		}
	}

	public Vector3 Offset
	{
		get
		{
			return m_offset;
		}
		set
		{
			m_offset = value;
		}
	}

	public Transform Target
	{
		get
		{
			return m_target;
		}
		set
		{
			m_target = value;
		}
	}

	private void Start()
	{
		m_blackboard = GetComponentInParent<ZombieBlackboard>();
	}

	private void Update()
	{
	}

	public override bool RunSensor()
	{
		if (m_isDisabled)
		{
			return false;
		}
		Collider[] array = Physics.OverlapBox(base.transform.position + base.transform.forward * m_offset.z + base.transform.right * m_offset.x + base.transform.up * m_offset.y, m_size / 2f, base.transform.rotation, m_sightMask);
		if (array.Length > 0 && CanSeeTarget(array))
		{
			m_isTriggered = true;
			return true;
		}
		m_isTriggered = false;
		m_target = null;
		return false;
	}

	private bool CanSeeTarget(Collider[] colliders)
	{
		foreach (Collider collider in colliders)
		{
			Vector3 direction = collider.transform.position - base.transform.position;
			RaycastHit hitInfo;
			if (Physics.Raycast(base.transform.position, direction, out hitInfo, 500f, ignoreMask) && hitInfo.collider.transform == collider.transform)
			{
				m_target = hitInfo.collider.transform;
				return true;
			}
		}
		return false;
	}

	private void OnDrawGizmos()
	{
		if (m_debug)
		{
			if (!m_isTriggered)
			{
				Gizmos.color = Color.green;
			}
			else
			{
				Gizmos.color = Color.red;
			}
			ExtDebug.DrawBox(base.transform.position + base.transform.forward * m_offset.z + base.transform.right * m_offset.x + base.transform.up * m_offset.y, m_size / 2f, base.transform.rotation, Gizmos.color);
		}
	}
}
