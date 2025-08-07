using UnityEngine;

public class VectorUtils
{
	public static Vector3 NaN = new Vector3(float.NaN, float.NaN, float.NaN);

	public static bool IsNaN(Vector3 vec)
	{
		if (float.IsNaN(vec.x) && float.IsNaN(vec.y) && float.IsNaN(vec.z))
		{
			return true;
		}
		return false;
	}
}
