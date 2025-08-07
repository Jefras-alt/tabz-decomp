using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshBehavior : Behavior<ZombieBlackboard>
{
	public override void Initialize(ZombieBlackboard context)
	{
		base.Initialize(context);
	}

	public override BTStatus Update(ZombieBlackboard context, float dt)
	{
		if (context.TargetTransform == null)
		{
			Debug.LogError("No target transform was found!");
			return BTStatus.FAILURE;
		}
		context.Path = new NavMeshPath();
		NavMesh.CalculatePath(context.GroundPosition, context.TargetTransform.position, -1, context.Path);
		context.Waypoints = new List<Vector3>(context.Path.corners);
		context.CurrentWaypoint = VectorUtils.NaN;
		return BTStatus.SUCCESS;
	}
}
