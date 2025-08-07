using System.Collections.Generic;

public class Composite<T> : Behavior<T> where T : Blackboard
{
	protected List<Behavior<T>> m_children;

	protected Behavior<T> m_currentChild;

	protected int m_currentIndex;

	public List<Behavior<T>> Children
	{
		get
		{
			return m_children;
		}
	}

	public Composite()
	{
		m_children = new List<Behavior<T>>();
	}

	public void Add(Behavior<T> behavior)
	{
		m_children.Add(behavior);
	}

	public void Remove(Behavior<T> behavior)
	{
		m_children.Remove(behavior);
	}
}
