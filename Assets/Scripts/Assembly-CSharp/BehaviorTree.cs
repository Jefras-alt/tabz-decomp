public class BehaviorTree<T> where T : Blackboard
{
	private Behavior<T> m_root;

	private float m_counter;

	public Behavior<T> Root
	{
		get
		{
			return m_root;
		}
		set
		{
			m_root = value;
		}
	}

	public void Update(float dt, T blackboard)
	{
		if (Root != null)
		{
			m_root.Tick(blackboard, dt);
		}
	}
}
