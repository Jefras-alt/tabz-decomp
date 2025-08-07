public class AlwaysSucceed<T> : Decorator<T> where T : Blackboard
{
	public AlwaysSucceed(Behavior<T> child)
		: base(child)
	{
	}

	public override BTStatus Update(T context, float dt)
	{
		return BTStatus.SUCCESS;
	}
}
