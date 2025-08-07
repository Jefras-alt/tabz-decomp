using UnityEngine;

[ExecuteInEditMode]
public class MeshBounds : MonoBehaviour
{
	private void Start()
	{
		MeshFilter component = GetComponent<MeshFilter>();
		if (!(component == null))
		{
			Mesh sharedMesh = component.sharedMesh;
			if (!(sharedMesh == null))
			{
				Vector3 vector = base.transform.lossyScale * 10f;
				vector.y = 4800f;
				sharedMesh.bounds = new Bounds(Vector3.zero, vector);
				Debug.Log(base.name + " new bounds " + vector);
			}
		}
	}
}
