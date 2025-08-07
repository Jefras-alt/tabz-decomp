using System;
using UnityEngine;

namespace TerrainComposer2
{
	[ExecuteInEditMode]
	public class TC_CamCapture : MonoBehaviour
	{
		public Camera cam;

		public int collisionMask;

		[NonSerialized]
		public Terrain terrain;

		private Transform t;

		public CollisionDirection collisionDirection;

		private void Start()
		{
			t = base.transform;
			cam = GetComponent<Camera>();
			cam.aspect = 1f;
		}

		public void Capture(int collisionMask, CollisionDirection collisionDirection, int outputId)
		{
			if (!(TC_Area2D.current.currentTerrainArea == null))
			{
				this.collisionMask = collisionMask;
				terrain = TC_Area2D.current.currentTerrain;
				cam.cullingMask = collisionMask;
				SetCamera(collisionDirection, outputId);
				cam.Render();
			}
		}

		public void SetCamera(CollisionDirection collisionDirection, int outputId)
		{
			if (t == null)
			{
				Start();
			}
			if (collisionDirection == CollisionDirection.Up)
			{
				t.position = new Vector3(TC_Area2D.current.bounds.center.x, -1f, TC_Area2D.current.bounds.center.z);
				t.rotation = Quaternion.Euler(-90f, 0f, 0f);
			}
			else
			{
				t.position = new Vector3(TC_Area2D.current.bounds.center.x, TC_Area2D.current.bounds.center.y + 1f, TC_Area2D.current.bounds.center.z);
				t.rotation = Quaternion.Euler(90f, 0f, 0f);
			}
			float num = TC_Area2D.current.bounds.extents.x;
			if (outputId == 0)
			{
				num += TC_Area2D.current.resExpandBorderSize;
			}
			cam.orthographicSize = num;
			cam.nearClipPlane = 0f;
			cam.farClipPlane = TC_Area2D.current.currentTerrainArea.terrainSize.y + 1f;
		}
	}
}
