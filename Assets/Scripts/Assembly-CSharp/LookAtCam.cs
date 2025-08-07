using UnityEngine;

public class LookAtCam : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		Vector3 vector = Camera.main.transform.forward * -1f;
		Debug.Log("Vec: " + vector);
		base.transform.rotation.eulerAngles.Set(vector.x, vector.y, vector.z);
		Debug.Log("Rotation: ");
	}
}
