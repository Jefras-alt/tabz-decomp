using System.Collections.Generic;
using UnityEngine;

namespace TerrainComposer2
{
	[ExecuteInEditMode]
	public class TC_Settings : MonoBehaviour
	{
		public enum ImageExportFormat
		{
			JPG = 0,
			PNG = 1
		}

		public static TC_Settings instance;

		public float version;

		public Vector2 scrollOffset = Vector2.zero;

		public Vector2 scrollAdd = Vector2.zero;

		public float scale = 1f;

		public bool drawDefaultInspector;

		public bool debugMode = true;

		public bool hideTerrainGroup = true;

		public bool useTCRuntime;

		public bool showFps = true;

		public bool hideMenuBar;

		public Transform dustbinT;

		public Transform selectionOld;

		public Terrain masterTerrain;

		public bool hasMasterTerrain;

		public PresetMode presetMode;

		public float seed;

		public string lastPath = string.Empty;

		public bool preview;

		public int previewResolution = 128;

		public ImageExportFormat imageExportFormat;

		public bool combineHeightmapImage = true;

		public string heightmapFilename = "Heightmap";

		public string normalmapFilename = "Normalmap";

		public string splatmapFilename = "Splatmap";

		public string colormapFilename = "Colormap";

		public float normalmapStrength = 0.5f;

		public List<TC_RawImage> rawFiles = new List<TC_RawImage>();

		public List<TC_Image> imageList = new List<TC_Image>();

		public bool isRTPDetected;

		public bool autoNormalmapRTP = true;

		public bool autoColormapRTP = true;

		public TC_GlobalSettings global;

		public string exportPath;

		private void Awake()
		{
			instance = this;
			if (exportPath == string.Empty)
			{
				exportPath = Application.dataPath;
			}
		}

		private void OnEnable()
		{
			instance = this;
			if (base.transform.parent != null)
			{
				base.transform.parent.hideFlags = HideFlags.NotEditable;
			}
		}

		private void OnDestroy()
		{
			TC_Reporter.Log("OnDestroy");
			instance = null;
		}

		public static void GetInstance()
		{
			GameObject gameObject = GameObject.Find("Settings");
			if (gameObject != null)
			{
				instance = gameObject.GetComponent<TC_Settings>();
			}
		}

		public void DisposeTextures()
		{
			for (int i = 0; i < rawFiles.Count; i++)
			{
				if (rawFiles[i] != null)
				{
					Object.Destroy(rawFiles[i].gameObject);
				}
			}
		}

		public void HasMasterTerrain()
		{
			if (masterTerrain == null)
			{
				TC_Area2D current = TC_Area2D.current;
				if (current.currentTerrainArea != null && current.currentTerrainArea.terrains.Count > 0)
				{
					masterTerrain = current.currentTerrainArea.terrains[0].terrain;
				}
				if (masterTerrain == null)
				{
					hasMasterTerrain = false;
					return;
				}
			}
			if (masterTerrain.terrainData == null)
			{
				hasMasterTerrain = false;
			}
			else
			{
				hasMasterTerrain = true;
			}
		}

		public TC_RawImage AddRawFile(string fullPath, bool isResourcesFolder)
		{
			for (int i = 0; i < rawFiles.Count; i++)
			{
				if (rawFiles[i] == null)
				{
					rawFiles.RemoveAt(i);
					i--;
				}
				else if (rawFiles[i].path == fullPath)
				{
					rawFiles[i].referenceCount++;
					if (rawFiles[i].tex == null)
					{
						rawFiles[i].LoadRawImage(fullPath);
					}
					return rawFiles[i];
				}
			}
			string fileName = TC.GetFileName(fullPath);
			GameObject gameObject = new GameObject(fileName);
			gameObject.transform.parent = base.transform;
			TC_RawImage tC_RawImage = gameObject.AddComponent<TC_RawImage>();
			tC_RawImage.isResourcesFolder = isResourcesFolder;
			tC_RawImage.LoadRawImage(fullPath);
			tC_RawImage.referenceCount = 1;
			rawFiles.Add(tC_RawImage);
			return tC_RawImage;
		}

		public TC_Image AddImage(Texture2D tex)
		{
			TC_Image tC_Image;
			for (int i = 0; i < imageList.Count; i++)
			{
				tC_Image = imageList[i];
			}
			tC_Image = new TC_Image();
			imageList.Add(tC_Image);
			return tC_Image;
		}

		public void CreateDustbin()
		{
			GameObject gameObject = new GameObject("Dustbin");
			dustbinT = gameObject.transform;
			dustbinT.parent = base.transform.parent;
		}

		public static Transform GetTransformFromLevel(int index, Transform t)
		{
			Transform root = t.root;
			List<Transform> list = new List<Transform>();
			Transform transform = t;
			do
			{
				transform = transform.parent;
				list.Insert(0, transform);
			}
			while (transform != root);
			return list[index];
		}
	}
}
