using System;

public class Invert<T> : Decorator<T> where T : Blackboard
{
	public Invert(Behavior<T> child)
		: base(child)
	{
	}

	public override BTStatus Update(T context, float dt)
	{
		if (m_child == null)
		{
			throw new ArgumentNullException("Inverter requires child to not be null!");
		}
		BTStatus bTStatus = m_child.Tick(context, dt);
		switch (bTStatus)
		{
		case BTStatus.SUCCESS:
			return BTStatus.FAILURE;
		case BTStatus.FAILURE:
			return BTStatus.SUCCESS;
		default:
			return bTStatus;
		}
	}
}
