public class InspectBehavior : Behavior<ZombieBlackboard>
{
	public override void Initialize(ZombieBlackboard context)
	{
		context.BehaviorState = BehaviorState.INSPECT;
		base.Initialize(context);
	}

	public override BTStatus Update(ZombieBlackboard context, float dt)
	{
		context.TargetTransform = context.EyeSensor.Target;
		return BTStatus.SUCCESS;
	}
}
