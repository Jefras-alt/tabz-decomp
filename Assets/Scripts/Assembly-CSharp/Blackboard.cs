using UnityEngine;

public class Blackboard : MonoBehaviour
{
	[SerializeField]
	private BTType m_behaviour;

	public BTType Behaviour
	{
		get
		{
			return m_behaviour;
		}
		set
		{
			m_behaviour = value;
		}
	}

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
	}
}
