using System;
using UnityEngine;

namespace TerrainComposer2
{
	[Serializable]
	public class TC_DetailPrototype
	{
		public bool usePrototypeMesh;

		public float bendFactor = 0.1f;

		public Color dryColor = new Color(41f / 51f, 0.7372549f, 0.101960786f, 1f);

		public Color healthyColor = Color.white;

		public float maxHeight = 2f;

		public float maxWidth = 2f;

		public float minHeight = 1f;

		public float minWidth = 1f;

		public float noiseSpread = 0.1f;

		public GameObject prototype;

		public Texture2D prototypeTexture;

		public DetailRenderMode renderMode = DetailRenderMode.Grass;
	}
}
