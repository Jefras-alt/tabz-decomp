using UnityEngine;

public class JointInvincibility : MonoBehaviour
{
	private FixedJoint[] joints;

	private float counter;

	private float breakForce;

	private float breakTorque;

	private bool done;

	private void Start()
	{
		joints = GetComponents<FixedJoint>();
		FixedJoint[] array = joints;
		foreach (FixedJoint fixedJoint in array)
		{
			breakForce = fixedJoint.breakForce;
			breakTorque = fixedJoint.breakTorque;
			fixedJoint.breakForce = float.PositiveInfinity;
			fixedJoint.breakTorque = float.PositiveInfinity;
		}
	}

	private void Update()
	{
		counter += Time.deltaTime;
		if (counter > 2f && !done)
		{
			done = true;
			FixedJoint[] array = joints;
			foreach (FixedJoint fixedJoint in array)
			{
				fixedJoint.breakForce = breakForce;
				fixedJoint.breakTorque = breakTorque;
			}
		}
		for (int j = 0; j < joints.Length; j++)
		{
			if (joints[j] == null)
			{
			}
		}
	}
}
