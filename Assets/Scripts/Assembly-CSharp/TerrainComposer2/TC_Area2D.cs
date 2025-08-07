using System;
using UnityEngine;

namespace TerrainComposer2
{
	[ExecuteInEditMode]
	public class TC_Area2D : MonoBehaviour
	{
		public static TC_Area2D current;

		public TC_TerrainArea[] terrainAreas;

		public TC_TerrainMeshArea[] meshTerrainAreas;

		[NonSerialized]
		public Terrain currentTerrain;

		[NonSerialized]
		public TC_Terrain currentTCTerrain;

		[NonSerialized]
		public TCUnityTerrain currentTCUnityTerrain;

		[NonSerialized]
		public MeshTerrain currentMeshTerrain;

		public TC_TerrainArea currentTerrainArea;

		[NonSerialized]
		public TC_TerrainMeshArea currentMeshTerrainArea;

		[NonSerialized]
		public ComputeBuffer layerGroupBuffer;

		public TC_PreviewArea previewArea;

		public Rect area;

		public Rect totalArea;

		public Bounds totalTerrainBounds;

		public TC_TerrainLayer layerLevelC;

		public TC_TerrainLayer terrainLayer;

		public Vector2 resolution;

		public Vector2 resolutionPM;

		public Vector2 resToPreview;

		public Vector2 worldPos;

		public Vector2 localPos;

		public Vector2 pos;

		public Vector2 localNPos;

		public Vector2 previewPos;

		public Vector2 snapOffsetUV;

		public Vector2 outputOffsetV2;

		public Vector3 startPos;

		public Vector3 terrainSize;

		public Vector3 outputOffsetV3;

		public Bounds bounds;

		public Int2 intResolution;

		public float heightN;

		public float height;

		public float angle;

		public int splatLength;

		public int splatmapLength;

		public float frames;

		public float[] splatTotal;

		public float terrainsToDo;

		public float terrainsDone;

		public float progress;

		public bool showProgressBar;

		public int previewResolution;

		[NonSerialized]
		public float resExpandBorderPercentage = 0.0625f;

		[NonSerialized]
		public int resExpandBorder;

		[NonSerialized]
		public float resExpandBorderSize;

		public Vector2 terrainHeightRange;

		private void Awake()
		{
			current = this;
		}

		private void Start()
		{
			current = this;
		}

		private void OnDestroy()
		{
			current = null;
		}

		public void CalcProgress()
		{
			progress = (localPos.y / area.height + localPos.x / (area.width * area.height)) / terrainsToDo + terrainsDone / terrainsToDo;
		}

		private void OnEnable()
		{
			current = this;
			currentTerrainArea = terrainAreas[0];
		}

		private void OnDisable()
		{
			current = null;
		}

		public void DisposeTextures()
		{
			for (int i = 0; i < terrainAreas.Length; i++)
			{
				if (terrainAreas[i] != null)
				{
					terrainAreas[i].DisposeTextures();
				}
			}
		}

		public bool SetCurrentArea(TCUnityTerrain tcTerrain, int outputId)
		{
			Terrain terrain = (currentTerrain = tcTerrain.terrain);
			currentTCUnityTerrain = tcTerrain;
			currentTCTerrain = tcTerrain;
			currentTerrainArea = terrainAreas[0];
			if (!currentTCUnityTerrain.active)
			{
				return false;
			}
			intResolution = default(Int2);
			Int2 @int = default(Int2);
			if (terrain.terrainData.heightmapResolution > 2049)
			{
				resExpandBorder = 0;
				resExpandBorderSize = 0f;
			}
			else
			{
				resExpandBorder = Mathf.RoundToInt((float)(terrain.terrainData.heightmapResolution - 1) * resExpandBorderPercentage);
				resExpandBorderSize = terrain.terrainData.size.x * resExpandBorderPercentage;
			}
			switch (outputId)
			{
			case 0:
				intResolution.x = (intResolution.y = terrain.terrainData.heightmapResolution + resExpandBorder * 2);
				@int = new Int2(terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
				break;
			case 1:
				intResolution.x = (intResolution.y = terrain.terrainData.alphamapResolution);
				@int = intResolution;
				splatLength = currentTerrain.terrainData.splatPrototypes.Length;
				splatmapLength = currentTerrain.terrainData.alphamapTextures.Length;
				break;
			case 3:
				intResolution.x = (intResolution.y = (int)(terrain.terrainData.size.x / terrainLayer.treeResolutionPM));
				@int = intResolution;
				break;
			case 4:
				intResolution.x = (intResolution.y = terrain.terrainData.detailResolution);
				@int = intResolution;
				break;
			case 5:
				intResolution.x = (intResolution.y = (int)(terrain.terrainData.size.x / terrainLayer.objectResolutionPM));
				@int = intResolution;
				break;
			case 2:
				intResolution.x = (intResolution.y = terrainLayer.colormapResolution);
				@int = intResolution;
				break;
			}
			outputOffsetV2 = new Vector2(terrainLayer.layerGroups[outputId].t.position.x, terrainLayer.layerGroups[outputId].t.position.z);
			outputOffsetV3 = new Vector3(outputOffsetV2.x, 0f, outputOffsetV2.y);
			resolution = intResolution.ToVector2();
			if (intResolution.x < TC_Settings.instance.previewResolution)
			{
				previewResolution = intResolution.x;
				TC_Reporter.Log("From " + TC_Settings.instance.previewResolution + " To " + previewResolution);
			}
			else
			{
				previewResolution = TC_Settings.instance.previewResolution;
			}
			resToPreview = new Vector2((float)previewResolution / totalArea.width, (float)previewResolution / totalArea.height);
			if (outputId == 0 || outputId == 1)
			{
				resolutionPM = new Vector2(terrain.terrainData.size.x / (float)(@int.x - 1), terrain.terrainData.size.z / (float)(@int.y - 1));
			}
			else
			{
				resolutionPM = new Vector2(terrain.terrainData.size.x / (float)@int.x, terrain.terrainData.size.z / (float)@int.y);
			}
			if (outputId == 0)
			{
				area = new Rect(terrain.transform.position.x - resolutionPM.x * (float)resExpandBorder, terrain.transform.position.z - resolutionPM.y * (float)resExpandBorder, intResolution.x, intResolution.y);
			}
			else
			{
				Vector2 vector = default(Vector2);
				vector.x = Mathw.Snap(terrain.transform.position.x, resolutionPM.x);
				vector.y = Mathw.Snap(terrain.transform.position.z, resolutionPM.y);
				switch (outputId)
				{
				}
				area = new Rect(vector.x, vector.y, intResolution.x, intResolution.y);
				snapOffsetUV = new Vector2(terrain.transform.position.x, terrain.transform.position.z) - vector;
				snapOffsetUV.x /= terrain.terrainData.size.x;
				snapOffsetUV.y /= terrain.terrainData.size.z;
			}
			bounds = new Bounds(terrain.transform.position + terrain.terrainData.size / 2f, terrain.terrainData.size);
			startPos = new Vector3(area.xMin, terrain.transform.position.y, area.yMin);
			terrainSize = terrain.terrainData.size;
			return true;
		}

		public void SetManualTotalArea()
		{
			totalArea = previewArea.area;
			resToPreview = new Vector2((float)current.previewResolution / totalArea.width, (float)previewResolution / totalArea.height);
		}

		public bool CalcTotalArea()
		{
			Vector2 vector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
			Vector2 vector2 = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
			Vector2 vector3 = new Vector2(float.PositiveInfinity, float.NegativeInfinity);
			for (int i = 0; i < terrainAreas.Length; i++)
			{
				for (int j = 0; j < terrainAreas[i].terrains.Count; j++)
				{
					Terrain terrain = terrainAreas[i].terrains[j].terrain;
					if (terrain == null)
					{
						return false;
					}
					if (terrain.gameObject.activeSelf)
					{
						Vector3 position = terrain.transform.position;
						Vector3 size = terrain.terrainData.size;
						if (position.x < vector.x)
						{
							vector.x = position.x;
						}
						if (position.z < vector.y)
						{
							vector.y = position.z;
						}
						Vector2 vector4 = new Vector2(position.x + size.x, position.z + size.z);
						Vector2 vector5 = new Vector2(position.y, position.y + size.y);
						if (vector4.x > vector2.x)
						{
							vector2.x = vector4.x;
						}
						if (vector4.y > vector2.y)
						{
							vector2.y = vector4.y;
						}
						if (vector5.x < vector3.x)
						{
							vector3.x = vector5.x;
						}
						if (vector5.y > vector3.y)
						{
							vector3.y = vector5.y;
						}
					}
				}
			}
			totalArea = default(Rect);
			totalArea.xMin = vector.x;
			totalArea.yMin = vector.y;
			totalArea.xMax = vector2.x;
			totalArea.yMax = vector2.y;
			totalTerrainBounds = default(Bounds);
			totalTerrainBounds.min = new Vector3(vector.x, vector3.x, vector.y);
			totalTerrainBounds.max = new Vector3(vector2.x, vector3.y, vector2.y);
			return true;
		}

		public void SetTerrainAreasSize(Vector3 size)
		{
			for (int i = 0; i < terrainAreas.Length; i++)
			{
				if (terrainAreas[i].active)
				{
					terrainAreas[i].terrainSize = size;
				}
			}
		}

		public void ApplyTerrainAreasSize()
		{
			for (int i = 0; i < terrainAreas.Length; i++)
			{
				if (terrainAreas[i].active)
				{
					terrainAreas[i].ApplySizeTerrainArea();
				}
			}
		}

		public void AddTerrainArea(TC_TerrainArea terrainArea)
		{
		}

		public void RemoveTerrainArea(TC_TerrainArea terrainArea)
		{
		}

		public void ApplySizeTerrainAreas()
		{
			for (int i = 0; i < terrainAreas.Length; i++)
			{
				terrainAreas[i].ApplySizeTerrainArea();
			}
		}

		public void ApplyResolutionTerrainAreas(TCUnityTerrain sTerrain)
		{
			for (int i = 0; i < terrainAreas.Length; i++)
			{
				terrainAreas[i].ApplyResolutionTerrainArea(sTerrain);
			}
		}

		public void ApplySplatTexturesTerrainAreas(TCUnityTerrain sTerrain)
		{
			for (int i = 0; i < terrainAreas.Length; i++)
			{
				terrainAreas[i].ApplySplatTextures(sTerrain);
			}
		}
	}
}
