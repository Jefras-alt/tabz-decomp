public class Parallel<T> : Composite<T> where T : Blackboard
{
	public override void Initialize(T context)
	{
		base.Initialize(context);
	}

	public override BTStatus Update(T context, float dt)
	{
		BTStatus result = BTStatus.SUCCESS;
		for (int i = 0; i < m_children.Count; i++)
		{
			m_currentIndex = i;
			m_currentChild = m_children[i];
			BTStatus bTStatus = m_currentChild.Tick(context, dt);
			if (bTStatus == BTStatus.FAILURE)
			{
				result = BTStatus.FAILURE;
			}
		}
		m_currentIndex = 0;
		return result;
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
