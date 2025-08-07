public class ReactToNoiseBehavior : Behavior<ZombieBlackboard>
{
	public override void Initialize(ZombieBlackboard context)
	{
		base.Initialize(context);
	}

	public override BTStatus Update(ZombieBlackboard context, float dt)
	{
		if (context.EarSensor.NoiseLoudness >= context.InvestigateLoudnessThreshold)
		{
			context.TargetTransform = context.PointOfInterest;
			context.BehaviorState = BehaviorState.INSPECT;
		}
		else
		{
			context.TargetTransform = context.PointOfInterest;
		}
		return BTStatus.SUCCESS;
	}
}
