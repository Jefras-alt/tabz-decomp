using UnityEngine;

[ExecuteInEditMode]
public class TC_LevelWithTerrain : MonoBehaviour
{
	public bool levelChildren;

	private void Update()
	{
		if (levelChildren)
		{
			levelChildren = false;
			LevelChildren();
		}
	}

	private void LevelChildren()
	{
		Ray ray = new Ray
		{
			direction = new Vector3(0f, -1f, 0f)
		};
		int num = LayerMask.NameToLayer("Terrain");
		num = ~num;
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			ray.origin = child.position;
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				child.position = new Vector3(child.position.x, hitInfo.point.y, child.position.z);
			}
		}
	}
}
