using System;
using UnityEngine;

namespace TerrainComposer2
{
	[Serializable]
	public class TexturePreview
	{
		public bool edit;

		public Texture2D tex;

		[NonSerialized]
		public byte[] bytes;

		private float x;

		private float y;

		public void Init()
		{
			int previewResolution = TC_Area2D.current.previewResolution;
			if (bytes == null)
			{
				bytes = new byte[previewResolution * previewResolution * 4];
			}
			if (tex == null)
			{
				tex = new Texture2D(previewResolution, previewResolution, TextureFormat.RGBA32, false);
				tex.hideFlags = HideFlags.DontSave;
				tex.name = "texPreview";
			}
		}

		public void Init(int resolution)
		{
			bool flag = false;
			if (bytes == null)
			{
				bytes = new byte[resolution * resolution * 4];
			}
			if (tex == null)
			{
				flag = true;
			}
			else if (tex.width != resolution)
			{
				tex.Resize(resolution, resolution);
				return;
			}
			if (flag)
			{
				tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
				tex.hideFlags = HideFlags.DontSave;
				tex.name = "texPreview";
			}
		}

		public void ReCreate(int resolution)
		{
			bytes = new byte[resolution * resolution * 4];
			tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
			tex.hideFlags = HideFlags.DontSave;
			tex.name = "texPreview";
		}

		public void UploadTexture()
		{
			tex.LoadRawTextureData(bytes);
			tex.Apply(false);
		}

		public void SetPixel(float v)
		{
			x = TC_Area2D.current.previewPos.x;
			y = TC_Area2D.current.previewPos.y;
			int num = (int)x;
			int num2 = (int)y;
			if (num <= 127 && num >= 0 && num2 <= 127 && num2 >= 0)
			{
				num2 *= 512;
				num *= 4;
				Color32 color = Color.white * v;
				if (v > 1f)
				{
					color = Color.Lerp(Color.red, new Color(1f, 0f, 1f), Mathw.Clamp01(v - 1f));
				}
				else if (v < 0f)
				{
					color = Color.Lerp(Color.cyan, Color.blue, Mathw.Clamp01(v * -1f));
				}
				bytes[num + num2] = color.r;
				bytes[num + num2 + 1] = color.g;
				bytes[num + num2 + 2] = color.b;
				bytes[num + num2 + 3] = 1;
			}
		}

		public void SetPixelColor(Color color)
		{
			x = TC_Area2D.current.previewPos.x;
			y = TC_Area2D.current.previewPos.y;
			int num = (int)x;
			int num2 = (int)y;
			if (num <= 127 && num >= 0 && num2 <= 127 && num2 >= 0)
			{
				num2 *= 512;
				num *= 4;
				bytes[num + num2] = (byte)(color.r * 255f);
				bytes[num + num2 + 1] = (byte)(color.g * 255f);
				bytes[num + num2 + 2] = (byte)(color.b * 255f);
				bytes[num + num2 + 3] = 1;
			}
		}

		public void SetPixelColor(int px, int py, Color color)
		{
			py *= tex.width * 4;
			px *= 4;
			bytes[px + py] = (byte)(color.r * 255f);
			bytes[px + py + 1] = (byte)(color.g * 255f);
			bytes[px + py + 2] = (byte)(color.b * 255f);
			bytes[px + py + 3] = 1;
		}
	}
}
