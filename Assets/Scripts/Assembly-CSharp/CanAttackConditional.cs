public class CanAttackConditional : Behavior<ZombieBlackboard>
{
	public override BTStatus Update(ZombieBlackboard context, float dt)
	{
		float magnitude = (context.TargetTransform.position - context.SelfTransform.position).magnitude;
		context.DistanceToTarget = magnitude;
		if (context.DistanceToTarget <= context.AttackRange)
		{
			return BTStatus.SUCCESS;
		}
		return BTStatus.FAILURE;
	}
}
