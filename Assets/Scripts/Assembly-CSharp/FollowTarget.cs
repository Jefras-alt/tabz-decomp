using UnityEngine;

public class FollowTarget : MonoBehaviour
{
	public Transform target;

	private void Update()
	{
		base.transform.position = target.position;
	}
}
