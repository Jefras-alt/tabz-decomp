using UnityEngine;

public class AddShake : MonoBehaviour
{
	public float strenght = 0.3f;

	public float lenght = 0.3f;

	public float distance;

	private void Start()
	{
		if (distance != 0f)
		{
			screenShake.AddShake(strenght, lenght, distance, base.transform);
		}
		else
		{
			screenShake.AddShake(strenght, lenght);
		}
	}
}
