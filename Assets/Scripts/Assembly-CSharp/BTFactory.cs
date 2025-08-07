public class BTFactory
{
	private BTFactory()
	{
	}

	public static BehaviorTree<ZombieBlackboard> CreateDefaultBT()
	{
		return new BehaviorTree<ZombieBlackboard>();
	}

	public static BehaviorTree<ZombieBlackboard> CreateZombieBT()
	{
		BehaviorTree<ZombieBlackboard> behaviorTree = new BehaviorTree<ZombieBlackboard>();
		Selector<ZombieBlackboard> selector = (Selector<ZombieBlackboard>)(behaviorTree.Root = new Selector<ZombieBlackboard>());
		Sequence<ZombieBlackboard> sequence = new Sequence<ZombieBlackboard>();
		sequence.Add(new CanSeeTargetConditional());
		sequence.Add(new CanAttackConditional());
		sequence.Add(new AttackBehavior());
		sequence.Add(new NavmeshBehavior());
		Sequence<ZombieBlackboard> sequence2 = new Sequence<ZombieBlackboard>();
		sequence2.Add(new CanSeeTargetConditional());
		sequence2.Add(new InspectBehavior());
		sequence2.Add(new NavmeshBehavior());
		Sequence<ZombieBlackboard> sequence3 = new Sequence<ZombieBlackboard>();
		sequence3.Add(new IdleBehavior());
		Sequence<ZombieBlackboard> sequence4 = new Sequence<ZombieBlackboard>();
		sequence4.Add(new DidHearNoiseConditional());
		sequence4.Add(new ReactToNoiseBehavior());
		sequence4.Add(new NavmeshBehavior());
		selector.Add(sequence);
		selector.Add(sequence2);
		selector.Add(sequence4);
		return behaviorTree;
	}
}
