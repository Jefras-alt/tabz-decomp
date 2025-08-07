using System;
using UnityEngine;

namespace TerrainComposer2
{
	[Serializable]
	public class ImageSettings
	{
		[Serializable]
		public class ColChannel
		{
			public bool active = true;

			public Vector2 range = new Vector2(0f, 255f);
		}

		private const int red = 0;

		private const int green = 1;

		private const int blue = 2;

		private const int alpha = 3;

		public ColorSelectMode colSelectMode = ColorSelectMode.ColorRange;

		public ColChannel[] colChannels;

		public Int2 tiles = new Int2(1, 1);

		public ImageSettings()
		{
			colChannels = new ColChannel[4];
			for (int i = 0; i < 4; i++)
			{
				colChannels[i] = new ColChannel();
			}
			colChannels[3].active = false;
		}
	}
}
