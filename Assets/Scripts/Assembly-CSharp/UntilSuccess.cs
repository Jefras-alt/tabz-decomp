public class UntilSuccess<T> : Decorator<T> where T : Blackboard
{
	public UntilSuccess(Behavior<T> child)
		: base(child)
	{
	}

	public override BTStatus Update(T context, float dt)
	{
		BTStatus bTStatus = m_child.Tick(context, dt);
		if (bTStatus != BTStatus.SUCCESS)
		{
			return BTStatus.RUNNING;
		}
		return BTStatus.SUCCESS;
	}
}
