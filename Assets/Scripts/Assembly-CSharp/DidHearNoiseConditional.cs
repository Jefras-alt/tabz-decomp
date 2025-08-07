public class DidHearNoiseConditional : Behavior<ZombieBlackboard>
{
	public override BTStatus Update(ZombieBlackboard context, float dt)
	{
		if (context.EarSensor.DidHearNoise)
		{
			context.PointOfInterest.position = context.EarSensor.LastNoisePosition;
			return BTStatus.SUCCESS;
		}
		return BTStatus.FAILURE;
	}
}
