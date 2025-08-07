public class Include<T> : Decorator<T> where T : Blackboard
{
	private BehaviorTree<T> m_childTree;

	public Include(BehaviorTree<T> childTree)
		: base((Behavior<T>)null)
	{
		m_childTree = childTree;
	}

	public override BTStatus Update(T context, float dt)
	{
		m_childTree.Update(dt, context);
		return BTStatus.SUCCESS;
	}
}
