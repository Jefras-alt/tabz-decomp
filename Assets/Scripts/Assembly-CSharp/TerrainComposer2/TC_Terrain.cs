using System;
using UnityEngine;

namespace TerrainComposer2
{
	[Serializable]
	public class TC_Terrain
	{
		public Transform t;

		public Vector3 newPosition;

		public int tasks;

		public TC_Node[] nodes;

		public Material rtpMat;

		public RenderTexture rtHeight;

		public Texture2D texHeight;

		public Texture2D texColormap;

		public Texture2D texNormalmap;

		public void DisposeTextures()
		{
			TC_Compute.DisposeRenderTexture(ref rtHeight);
			TC_Compute.DisposeTexture(ref texHeight);
			TC_Compute.DisposeTexture(ref texColormap);
			TC_Compute.DisposeTexture(ref texNormalmap);
		}

		public void SetNodesActive(bool active)
		{
			if (nodes != null)
			{
				for (int i = 0; i < nodes.Length; i++)
				{
					nodes[i].autoGenerate = false;
					nodes[i].t.position = t.position;
					nodes[i].gameObject.SetActive(active);
				}
			}
		}

		public void Init()
		{
			if (rtpMat == null)
			{
				MeshRenderer component = t.GetComponent<MeshRenderer>();
				if (component != null)
				{
					rtpMat = component.sharedMaterial;
				}
			}
		}
	}
}
