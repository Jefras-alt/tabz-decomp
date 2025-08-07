public class Decorator<T> : Behavior<T> where T : Blackboard
{
	protected Behavior<T> m_child;

	public Decorator(Behavior<T> child = null)
	{
		m_child = child;
	}

	public void SetChild(Behavior<T> child)
	{
		m_child = child;
	}
}
