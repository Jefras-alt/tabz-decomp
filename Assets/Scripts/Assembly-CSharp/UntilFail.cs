public class UntilFail<T> : Decorator<T> where T : Blackboard
{
	public UntilFail(Behavior<T> child)
		: base(child)
	{
	}

	public override BTStatus Update(T context, float dt)
	{
		BTStatus bTStatus = m_child.Tick(context, dt);
		if (bTStatus != BTStatus.FAILURE)
		{
			return BTStatus.RUNNING;
		}
		return BTStatus.SUCCESS;
	}
}
