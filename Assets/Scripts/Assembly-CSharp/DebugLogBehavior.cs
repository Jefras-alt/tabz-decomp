using UnityEngine;

public class DebugLogBehavior<T> : Behavior<T> where T : Blackboard
{
	public override void Initialize(T context)
	{
		Debug.Log("Initialized Debugger");
		base.Initialize(context);
	}

	public override BTStatus Update(T context, float dt)
	{
		Debug.Log("Updating Debugger");
		return BTStatus.SUCCESS;
	}

	public override void Terminate(T context)
	{
		Debug.Log("Terminating Debugger");
		base.Terminate(context);
	}
}
