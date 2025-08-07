public class AlwaysFail<T> : Decorator<T> where T : Blackboard
{
	public AlwaysFail(Behavior<T> child)
		: base(child)
	{
	}

	public override BTStatus Update(T context, float dt)
	{
		return BTStatus.FAILURE;
	}
}
