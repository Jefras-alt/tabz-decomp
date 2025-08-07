using UnityEngine;

namespace TerrainComposer2
{
	public struct ItemSettings
	{
		public int index;

		public float randomPosition;

		public Vector2 range;

		public float opacity;

		public ItemSettings(int index, float randomPosition, Vector2 range, float opacity)
		{
			this.index = index;
			this.randomPosition = randomPosition;
			this.range = range;
			this.opacity = opacity;
		}
	}
}
