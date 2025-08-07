using UnityEngine;

public class InterpolationFixer : MonoBehaviour
{
	private Rigidbody rig;

	private void Start()
	{
		rig = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (Time.timeScale < 0.9f)
		{
			rig.interpolation = RigidbodyInterpolation.Interpolate;
		}
		else
		{
			rig.interpolation = RigidbodyInterpolation.None;
		}
	}
}
