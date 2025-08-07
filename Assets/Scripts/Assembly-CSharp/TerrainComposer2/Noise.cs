using System;

namespace TerrainComposer2
{
	[Serializable]
	public class Noise
	{
		public NoiseMode mode;

		public CellNoiseMode cellMode;

		public float frequency = 100f;

		public float lacunarity = 2f;

		public int octaves = 6;

		public float persistence = 0.5f;

		public float seed;

		public float amplitude = 7f;

		public float warp0 = 0.5f;

		public float warp = 0.25f;

		public float damp0 = 0.8f;

		public float damp = 1f;

		public float dampScale = 1f;

		public int cellType = 1;

		public int distanceFunction = 1;
	}
}
