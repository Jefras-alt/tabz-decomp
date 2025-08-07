public class Behavior<T> : BehaviorBase where T : Blackboard
{
	public BTStatus Status { get; set; }

	public virtual BTStatus Update(T context, float dt)
	{
		return BTStatus.INVALID;
	}

	public virtual void Initialize(T context)
	{
	}

	public virtual void Terminate(T context)
	{
	}

	public BTStatus Tick(T context, float dt)
	{
		if (Status == BTStatus.INVALID)
		{
			Initialize(context);
		}
		Status = Update(context, dt);
		if (Status != BTStatus.RUNNING)
		{
			Terminate(context);
		}
		return Status;
	}
}
