using System.IO;
using UnityEngine;

namespace TerrainComposer2
{
	[ExecuteInEditMode]
	public class TC_RawImage : MonoBehaviour
	{
		public enum ByteOrder
		{
			Windows = 0,
			Mac = 1
		}

		public bool isResourcesFolder;

		public string path;

		public string filename;

		public int referenceCount;

		public Int2 resolution;

		public bool squareResolution;

		public ByteOrder byteOrder;

		public Texture2D tex;

		public bool isDestroyed;

		public bool callDestroy;

		private void Awake()
		{
			if (isDestroyed)
			{
				LoadRawImage(path);
				TC_Settings.instance.rawFiles.Add(this);
			}
			if (!callDestroy)
			{
				TC.RefreshOutputReferences(6);
				referenceCount = 0;
			}
			else
			{
				callDestroy = false;
			}
		}

		private void OnDestroy()
		{
			TC_Compute.DisposeTexture(ref tex);
			if (!callDestroy)
			{
				TC.RefreshOutputReferences(6);
			}
		}

		private void DestroyMe()
		{
			TC_Settings instance = TC_Settings.instance;
			if (!(instance == null) && instance.rawFiles != null)
			{
				int num = instance.rawFiles.IndexOf(this);
				if (num != -1)
				{
					instance.rawFiles.RemoveAt(num);
				}
				Object.Destroy(base.gameObject);
			}
		}

		public void UnregisterReference()
		{
			referenceCount--;
			if (referenceCount <= 0)
			{
				isDestroyed = true;
				callDestroy = true;
				DestroyMe();
			}
		}

		public bool GetFileResolution()
		{
			if (!TC.FileExists(path))
			{
				return false;
			}
			long fileLength = TC.GetFileLength(path);
			GetResolutionFromLength(fileLength);
			return true;
		}

		public void GetResolutionFromLength(long length)
		{
			float num = Mathf.Sqrt(length / 2);
			if (num == Mathf.Floor(num))
			{
				squareResolution = true;
			}
			else
			{
				squareResolution = false;
			}
			resolution = new Int2(num, num);
		}

		public void LoadRawImage(string path)
		{
			this.path = path;
			string text = Application.dataPath.Replace("Assets", "/") + path;
			if (tex != null)
			{
				return;
			}
			TC_Reporter.Log("Load Raw file " + text);
			byte[] array = null;
			if (isResourcesFolder)
			{
				TextAsset textAsset = Resources.Load<TextAsset>(path);
				if (textAsset != null)
				{
					array = textAsset.bytes;
				}
				else
				{
					Debug.Log("Can't find file");
				}
			}
			else
			{
				array = File.ReadAllBytes(text);
			}
			if (array != null && array.Length != 0)
			{
				GetResolutionFromLength(array.Length);
				tex = new Texture2D(resolution.x, resolution.y, TextureFormat.R16, false);
				tex.hideFlags = HideFlags.DontSave;
				tex.LoadRawTextureData(array);
				tex.Apply();
			}
		}
	}
}
