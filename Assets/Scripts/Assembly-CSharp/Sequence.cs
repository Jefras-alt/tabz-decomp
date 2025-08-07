public class Sequence<T> : Composite<T> where T : Blackboard
{
	public override void Initialize(T context)
	{
		base.Initialize(context);
		m_currentIndex = 0;
		m_currentChild = null;
	}

	public override BTStatus Update(T context, float dt)
	{
		BTStatus bTStatus = BTStatus.INVALID;
		for (int i = 0; i < m_children.Count; i++)
		{
			m_currentIndex = i;
			m_currentChild = m_children[i];
			switch (m_currentChild.Tick(context, dt))
			{
			case BTStatus.FAILURE:
				m_currentIndex = 0;
				return BTStatus.FAILURE;
			case BTStatus.RUNNING:
				return BTStatus.RUNNING;
			}
		}
		m_currentIndex = 0;
		return BTStatus.SUCCESS;
	}

	public override void Terminate(T context)
	{
		base.Terminate(context);
		foreach (Behavior<T> child in m_children)
		{
			child.Status = BTStatus.INVALID;
		}
	}
}
