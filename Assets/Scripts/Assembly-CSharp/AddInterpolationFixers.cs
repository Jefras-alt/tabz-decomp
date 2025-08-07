using UnityEngine;

public class AddInterpolationFixers : MonoBehaviour
{
	private void Start()
	{
		Rigidbody[] array = Object.FindObjectsOfType<Rigidbody>();
		foreach (Rigidbody rigidbody in array)
		{
			rigidbody.gameObject.AddComponent<InterpolationFixer>();
		}
	}
}
