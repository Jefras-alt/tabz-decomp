using System.Collections;
using UnityEngine;

public class JointAnimationExtended : MonoBehaviour
{
	public enum Direction
	{
		right = 0,
		up = 1,
		forward = 2,
		velocity = 3
	}

	public enum MovementType
	{
		Torque = 0,
		Force = 1
	}

	public bool selfDirecions;

	public Direction direction;

	public MovementType movementType;

	public bool isLeft;

	public AnimationCurve curve;

	public float multiplier = 1f;

	public float offset;

	private Rigidbody rig;

	private PhysicsAmimationController controller;

	private Vector3 dir = Vector3.zero;

	public float runMultiplier = 1f;

	private float currentStateMultiplier = 1f;

	private void Start()
	{
		rig = GetComponent<Rigidbody>();
		controller = base.transform.root.GetComponent<PhysicsAmimationController>();
	}

	private void Update()
	{
	}

	private void SetDirection()
	{
		if (selfDirecions)
		{
			if (direction == Direction.right)
			{
				dir = base.transform.right;
			}
			if (direction == Direction.up)
			{
				dir = base.transform.up;
			}
			if (direction == Direction.forward)
			{
				dir = base.transform.forward;
			}
			if (direction == Direction.velocity)
			{
				dir = controller.mainRig.velocity.normalized;
				dir.y = 0f;
			}
		}
		else
		{
			if (direction == Direction.right)
			{
				dir = controller.right.forward;
			}
			if (direction == Direction.up)
			{
				dir = controller.up.forward;
			}
			if (direction == Direction.forward)
			{
				dir = controller.forward.forward;
			}
			if (direction == Direction.velocity)
			{
				dir = controller.mainRig.velocity.normalized;
				dir.y = 0f;
			}
		}
	}

	public void Animate(int state)
	{
		SetDirection();
		if (state == 0)
		{
			currentStateMultiplier = 1f;
		}
		if (state == 1)
		{
			currentStateMultiplier = runMultiplier;
		}
		StartCoroutine(Play());
	}

	private IEnumerator Play()
	{
		yield return new WaitForSeconds(offset);
		float counter = 0f;
		while (counter < controller.stepLength)
		{
			counter += Time.deltaTime;
			float curveValue = curve.Evaluate(counter / controller.stepLength);
			if (movementType == MovementType.Force)
			{
				rig.AddForce(dir * currentStateMultiplier * curveValue * multiplier * Time.deltaTime, ForceMode.Acceleration);
			}
			if (movementType == MovementType.Torque)
			{
				rig.AddTorque(dir * currentStateMultiplier * curveValue * multiplier * Time.deltaTime, ForceMode.Acceleration);
			}
			yield return null;
		}
	}
}
