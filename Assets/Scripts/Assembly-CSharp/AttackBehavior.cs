public class AttackBehavior : Behavior<ZombieBlackboard>
{
	public override void Initialize(ZombieBlackboard context)
	{
		context.BehaviorState = BehaviorState.ATTACK;
		base.Initialize(context);
	}

	public override BTStatus Update(ZombieBlackboard context, float dt)
	{
		return BTStatus.SUCCESS;
	}
}
