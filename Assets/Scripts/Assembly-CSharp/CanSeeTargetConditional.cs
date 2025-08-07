public class CanSeeTargetConditional : Behavior<ZombieBlackboard>
{
	public override BTStatus Update(ZombieBlackboard context, float dt)
	{
		if (context.EyeSensor.RunSensor())
		{
			context.TargetTransform = context.EyeSensor.Target;
			return BTStatus.SUCCESS;
		}
		return BTStatus.FAILURE;
	}
}
