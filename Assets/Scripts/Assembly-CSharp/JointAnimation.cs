using UnityEngine;

public class JointAnimation : MonoBehaviour
{
	public enum Direction
	{
		right = 0,
		up = 1,
		forward = 2
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

	[Header("Run")]
	public float torqueOut;

	public float TorqueIn;

	[Header("Walk")]
	public float torqueOutWalk;

	public float TorqueInWalk;

	[Header("Back")]
	public float torqueOutBack;

	public float TorqueInBack;

	private Rigidbody rig;

	private PhysicsAmimationController controller;

	private Vector3 dir = Vector3.zero;

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
		}
	}

	public void Animate(bool left, int state)
	{
		SetDirection();
		if (isLeft == left)
		{
			if (state == 0)
			{
				AnimateOut(torqueOutWalk);
			}
			if (state == 1)
			{
				AnimateOut(torqueOut);
			}
			if (state == 2)
			{
				AnimateOut(torqueOutBack);
			}
		}
		else
		{
			if (state == 0)
			{
				AnimateIn(TorqueInWalk);
			}
			if (state == 1)
			{
				AnimateIn(TorqueIn);
			}
			if (state == 2)
			{
				AnimateIn(TorqueInBack);
			}
		}
	}

	private void AnimateOut(float outToeque)
	{
		if (movementType == MovementType.Torque)
		{
			rig.AddTorque(dir * outToeque * Time.deltaTime, ForceMode.Acceleration);
		}
		if (movementType == MovementType.Force)
		{
			rig.AddForce(dir * outToeque * Time.deltaTime, ForceMode.Acceleration);
		}
	}

	private void AnimateIn(float inTorque)
	{
		if (movementType == MovementType.Torque)
		{
			rig.AddTorque(dir * (0f - inTorque) * Time.deltaTime, ForceMode.Acceleration);
		}
		if (movementType == MovementType.Force)
		{
			rig.AddForce(dir * (0f - inTorque) * Time.deltaTime, ForceMode.Acceleration);
		}
	}
}
