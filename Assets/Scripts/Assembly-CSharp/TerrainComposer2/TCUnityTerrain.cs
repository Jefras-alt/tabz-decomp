using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

namespace TerrainComposer2
{
	[Serializable]
	public class TCUnityTerrain : TC_Terrain
	{
		public bool active = true;

		public int index;

		public int index_old;

		public bool on_row;

		public Color color_terrain = new Color(2f, 2f, 2f, 1f);

		public int copy_terrain;

		public bool copy_terrain_settings = true;

		public Transform objectsParent;

		public bool detailSettingsFoldout;

		public bool splatSettingsFoldout;

		public bool treeSettingsFoldout;

		public Terrain terrain;

		public Color[] splatColors;

		public List<TC_SplatPrototype> splatPrototypes = new List<TC_SplatPrototype>();

		public List<TC_TreePrototype> treePrototypes = new List<TC_TreePrototype>();

		public List<TC_DetailPrototype> detailPrototypes = new List<TC_DetailPrototype>();

		public int heightmapResolutionList = 5;

		public int splatmapResolutionList = 4;

		public int basemapResolutionList = 4;

		public int detailResolutionPerPatchList = 1;

		public Vector3 size = new Vector3(1000f, 500f, 1000f);

		public int tileX;

		public int tileZ;

		public int heightmapResolution = 512;

		public int splatmapResolution = 512;

		public int basemapResolution = 128;

		public int detailResolution = 512;

		public int detailResolutionPerPatch = 32;

		public int appliedResolutionPerPatch;

		public float grassScaleMulti = 1f;

		public float heightmapPixelError = 5f;

		public int heightmapMaximumLOD;

		public bool castShadows;

		public float basemapDistance = 20000f;

		public float treeDistance = 2000f;

		public float detailObjectDistance = 80f;

		public float detailObjectDensity = 1f;

		public int treeMaximumFullLODCount = 50;

		public float treeBillboardDistance = 50f;

		public float treeCrossFadeLength = 5f;

		public bool drawTreesAndFoliage = true;

		public ReflectionProbeUsage reflectionProbeUsage;

		public bool bakeLightProbesForTrees = true;

		public float thickness = 1f;

		public float legacyShininess = 0.7812f;

		public Color legacySpecular = new Color(0.5f, 0.5f, 0.5f, 1f);

		public TC_TerrainSettings terrainSettingsScript;

		public Terrain.MaterialType materialType;

		public Material materialTemplate;

		public bool drawHeightmap = true;

		public bool collectDetailPatches = true;

		public float wavingGrassSpeed = 0.5f;

		public float wavingGrassAmount = 0.5f;

		public float wavingGrassStrength = 0.5f;

		public Color wavingGrassTint = new Color(0.698f, 0.6f, 0.5f);

		public void AssignTextureRTP(string texName, Texture2D tex)
		{
			Type type = TC.FindRTP();
			if (type == null || terrain == null)
			{
				return;
			}
			Component component = terrain.GetComponent(type);
			if (!(component == null))
			{
				FieldInfo field = type.GetField(texName);
				if (field != null)
				{
					field.SetValue(component, tex);
				}
			}
		}

		public bool CheckValidUnityTerrain()
		{
			if (terrain == null)
			{
				return false;
			}
			if (terrain.terrainData == null)
			{
				return false;
			}
			return true;
		}

		public void AddSplatTexture(int index)
		{
			if (splatPrototypes.Count >= 8)
			{
				TC.AddMessage("TC2 supports generating maximum 8 splat textures.");
				Debug.Log("TC2 supports generating maximum 8 splat textures.");
			}
			else
			{
				splatPrototypes.Insert(index, new TC_SplatPrototype());
			}
		}

		public void EraseSplatTexture(int splat_number)
		{
			if (splatPrototypes.Count > 0)
			{
				splatPrototypes.RemoveAt(splat_number);
			}
		}

		public void ClearSplatTextures()
		{
			splatPrototypes.Clear();
		}

		public void clear_null_splatprototype()
		{
			for (int i = 0; i < splatPrototypes.Count; i++)
			{
				if (splatPrototypes[i].texture == null)
				{
					splatPrototypes.RemoveAt(i);
					i--;
				}
			}
		}

		public void add_treeprototype(int index)
		{
			treePrototypes.Insert(index, new TC_TreePrototype());
		}

		public void EraseTreeProtoType(int tree_number)
		{
			if (treePrototypes.Count > 0)
			{
				treePrototypes.RemoveAt(tree_number);
			}
		}

		public void clear_treeprototype()
		{
			treePrototypes.Clear();
		}

		public void clear_null_treeprototype()
		{
			for (int i = 0; i < treePrototypes.Count; i++)
			{
				if (treePrototypes[i].prefab == null)
				{
					treePrototypes.RemoveAt(i);
					i--;
				}
			}
		}

		public void AddDetailPrototype(int detail_number)
		{
			if (detailPrototypes.Count >= 8)
			{
				TC.AddMessage("TC2 supports generating maximum 8 grass textures.");
				Debug.Log("TC2 supports generating maximum 8 grass textures.");
			}
			else
			{
				detailPrototypes.Insert(detail_number, new TC_DetailPrototype());
			}
		}

		public void EraseDetailPrototype(int detail_number)
		{
			if (detailPrototypes.Count > 0)
			{
				detailPrototypes.RemoveAt(detail_number);
			}
		}

		public void clear_detailprototype()
		{
			detailPrototypes.Clear();
		}

		public void clear_null_detailprototype()
		{
			for (int i = 0; i < detailPrototypes.Count; i++)
			{
				if (detailPrototypes[i].prototype == null && detailPrototypes[i].prototypeTexture == null)
				{
					detailPrototypes.RemoveAt(i);
					i--;
				}
			}
		}

		public void SetTerrainResolutionsToList()
		{
			heightmapResolutionList = 12 - (int)Mathf.Log(heightmapResolution - 1, 2f);
			splatmapResolutionList = 11 - (int)Mathf.Log(splatmapResolution, 2f);
			basemapResolutionList = 11 - (int)Mathf.Log(basemapResolution, 2f);
			detailResolutionPerPatchList = (int)Mathf.Log(detailResolutionPerPatch, 2f) - 3;
		}

		public void SetTerrainResolutionFromList()
		{
			heightmapResolution = (int)(Mathf.Pow(2f, 12 - heightmapResolutionList) + 1f);
			splatmapResolution = (int)Mathf.Pow(2f, 11 - splatmapResolutionList);
			basemapResolution = (int)Mathf.Pow(2f, 11 - basemapResolutionList);
			detailResolutionPerPatch = (int)Mathf.Pow(2f, detailResolutionPerPatchList + 3);
			SetTerrainResolutionsToList();
		}

		public void ApplyAllSettings(TCUnityTerrain terrain, bool settingsEditor)
		{
			ApplyResolutionTerrain(terrain);
			ApplySettings(terrain, settingsEditor);
		}

		public void ApplyResolutionTerrain(TCUnityTerrain sTerrain)
		{
			SetTerrainResolutionFromList();
			if (CheckValidUnityTerrain())
			{
				if (terrain.terrainData.heightmapResolution != sTerrain.heightmapResolution)
				{
					Vector3 vector = terrain.terrainData.size;
					terrain.terrainData.heightmapResolution = sTerrain.heightmapResolution;
					terrain.terrainData.size = vector;
				}
				if (terrain.terrainData.alphamapResolution != sTerrain.splatmapResolution)
				{
					terrain.terrainData.alphamapResolution = sTerrain.splatmapResolution;
				}
				if (terrain.terrainData.baseMapResolution != sTerrain.basemapResolution)
				{
					terrain.terrainData.baseMapResolution = sTerrain.basemapResolution;
				}
				if (terrain.terrainData.detailResolution != sTerrain.detailResolution || sTerrain.detailResolutionPerPatch != sTerrain.appliedResolutionPerPatch)
				{
					terrain.terrainData.SetDetailResolution(sTerrain.detailResolution, sTerrain.detailResolutionPerPatch);
					sTerrain.appliedResolutionPerPatch = sTerrain.detailResolutionPerPatch;
				}
			}
		}

		public void GetResolutions()
		{
			if (!(terrain == null))
			{
				heightmapResolution = terrain.terrainData.heightmapResolution;
				basemapResolution = terrain.terrainData.baseMapResolution;
				splatmapResolution = terrain.terrainData.alphamapResolution;
				detailResolution = terrain.terrainData.detailResolution;
				SetTerrainResolutionsToList();
			}
		}

		public void GetSettings(bool settingsEditor)
		{
			if (!CheckValidUnityTerrain())
			{
				return;
			}
			materialType = terrain.materialType;
			materialTemplate = terrain.materialTemplate;
			basemapDistance = terrain.basemapDistance;
			castShadows = terrain.castShadows;
			treeCrossFadeLength = terrain.treeCrossFadeLength;
			reflectionProbeUsage = terrain.reflectionProbeUsage;
			thickness = terrain.terrainData.thickness;
			collectDetailPatches = terrain.collectDetailPatches;
			legacyShininess = terrain.legacyShininess;
			legacySpecular = terrain.legacySpecular;
			wavingGrassSpeed = terrain.terrainData.wavingGrassSpeed;
			wavingGrassAmount = terrain.terrainData.wavingGrassAmount;
			wavingGrassStrength = terrain.terrainData.wavingGrassStrength;
			wavingGrassTint = terrain.terrainData.wavingGrassTint;
			if (settingsEditor)
			{
				heightmapPixelError = terrain.heightmapPixelError;
				heightmapMaximumLOD = terrain.heightmapMaximumLOD;
				drawTreesAndFoliage = terrain.drawTreesAndFoliage;
				treeDistance = terrain.treeDistance;
				detailObjectDistance = terrain.detailObjectDistance;
				detailObjectDensity = terrain.detailObjectDensity;
				treeBillboardDistance = terrain.treeBillboardDistance;
				treeMaximumFullLODCount = terrain.treeMaximumFullLODCount;
				return;
			}
			terrainSettingsScript = terrain.gameObject.GetComponent<TC_TerrainSettings>();
			if (terrainSettingsScript == null)
			{
				terrainSettingsScript = terrain.gameObject.AddComponent<TC_TerrainSettings>();
			}
			heightmapPixelError = terrainSettingsScript.heightmapPixelError;
			heightmapMaximumLOD = terrainSettingsScript.heightmapMaximumLOD;
			drawTreesAndFoliage = terrainSettingsScript.drawTreesAndFoliage;
			treeDistance = terrainSettingsScript.treeDistance;
			detailObjectDistance = terrainSettingsScript.detailObjectDistance;
			detailObjectDensity = terrainSettingsScript.detailObjectDensity;
			treeBillboardDistance = terrainSettingsScript.treeBillboardDistance;
			treeMaximumFullLODCount = terrainSettingsScript.treeMaximumFullLODCount;
		}

		public void ApplySettings(TCUnityTerrain sTerrain, bool settingsEditor)
		{
			if (!CheckValidUnityTerrain())
			{
				return;
			}
			terrain.drawHeightmap = sTerrain.drawHeightmap;
			terrain.collectDetailPatches = sTerrain.collectDetailPatches;
			terrain.legacyShininess = sTerrain.legacyShininess;
			terrain.legacySpecular = sTerrain.legacySpecular;
			terrain.reflectionProbeUsage = sTerrain.reflectionProbeUsage;
			terrain.materialType = sTerrain.materialType;
			terrain.materialTemplate = sTerrain.materialTemplate;
			terrain.terrainData.thickness = sTerrain.thickness;
			terrain.basemapDistance = sTerrain.basemapDistance;
			terrain.castShadows = sTerrain.castShadows;
			terrain.treeCrossFadeLength = sTerrain.treeCrossFadeLength;
			terrain.terrainData.wavingGrassSpeed = sTerrain.wavingGrassSpeed;
			terrain.terrainData.wavingGrassAmount = sTerrain.wavingGrassAmount;
			terrain.terrainData.wavingGrassStrength = sTerrain.wavingGrassStrength;
			terrain.terrainData.wavingGrassTint = sTerrain.wavingGrassTint;
			if (settingsEditor)
			{
				terrain.drawTreesAndFoliage = sTerrain.drawTreesAndFoliage;
				terrain.heightmapPixelError = sTerrain.heightmapPixelError;
				terrain.heightmapMaximumLOD = sTerrain.heightmapMaximumLOD;
				terrain.detailObjectDistance = sTerrain.detailObjectDistance;
				terrain.detailObjectDensity = sTerrain.detailObjectDensity;
				terrain.treeDistance = sTerrain.treeDistance;
				terrain.treeBillboardDistance = sTerrain.treeBillboardDistance;
				terrain.treeMaximumFullLODCount = sTerrain.treeMaximumFullLODCount;
				return;
			}
			if (terrainSettingsScript == null)
			{
				terrainSettingsScript = terrain.gameObject.GetComponent<TC_TerrainSettings>();
				if (terrainSettingsScript == null)
				{
					terrainSettingsScript = terrain.gameObject.AddComponent<TC_TerrainSettings>();
				}
			}
			terrainSettingsScript.heightmapPixelError = sTerrain.heightmapPixelError;
			terrainSettingsScript.heightmapMaximumLOD = sTerrain.heightmapMaximumLOD;
			terrainSettingsScript.drawTreesAndFoliage = sTerrain.drawTreesAndFoliage;
			terrainSettingsScript.treeDistance = sTerrain.treeDistance;
			terrainSettingsScript.detailObjectDistance = sTerrain.detailObjectDistance;
			terrainSettingsScript.detailObjectDensity = sTerrain.detailObjectDensity;
			terrainSettingsScript.treeBillboardDistance = sTerrain.treeBillboardDistance;
			terrainSettingsScript.treeMaximumFullLODCount = sTerrain.treeMaximumFullLODCount;
		}

		public void SwapSplatTexture(int index1, int index2)
		{
			if (index2 > -1 && index2 < splatPrototypes.Count)
			{
				TC_SplatPrototype value = splatPrototypes[index1];
				splatPrototypes[index1] = splatPrototypes[index2];
				splatPrototypes[index2] = value;
			}
		}

		public void ApplySplatTextures(TCUnityTerrain sTerrain = null)
		{
			if (!CheckValidUnityTerrain())
			{
				return;
			}
			if (sTerrain == null)
			{
				sTerrain = this;
			}
			List<SplatPrototype> list = new List<SplatPrototype>();
			bool flag = false;
			for (int i = 0; i < sTerrain.splatPrototypes.Count; i++)
			{
				if (list.Count >= 8)
				{
					flag = true;
					break;
				}
				TC_SplatPrototype tC_SplatPrototype = sTerrain.splatPrototypes[i];
				if (tC_SplatPrototype.texture != null)
				{
					SplatPrototype splatPrototype = new SplatPrototype();
					splatPrototype.texture = tC_SplatPrototype.texture;
					splatPrototype.normalMap = tC_SplatPrototype.normalMap;
					splatPrototype.metallic = tC_SplatPrototype.metallic;
					splatPrototype.smoothness = tC_SplatPrototype.smoothness;
					splatPrototype.tileOffset = tC_SplatPrototype.tileOffset;
					float num = sTerrain.terrain.terrainData.size.x / Mathf.Round(sTerrain.terrain.terrainData.size.x / tC_SplatPrototype.tileSize.x);
					splatPrototype.tileSize = new Vector2(num, num);
					list.Add(splatPrototype);
					TC.SetTextureReadWrite(tC_SplatPrototype.texture);
				}
			}
			if (flag)
			{
				TC.AddMessage("TC2 supports generating maximum 8 splat textures.");
				Debug.Log("TC2 supports generating maximum 8 splat textures.");
			}
			terrain.terrainData.splatPrototypes = list.ToArray();
		}

		public void CleanSplatTextures(TCUnityTerrain sTerrain = null)
		{
			if (sTerrain == null)
			{
				sTerrain = this;
			}
			for (int i = 0; i < sTerrain.splatPrototypes.Count; i++)
			{
				if (sTerrain.splatPrototypes[i].texture == null)
				{
					sTerrain.EraseSplatTexture(i);
					i--;
				}
			}
		}

		public void GetSize()
		{
			if (!(terrain == null))
			{
				size = terrain.terrainData.size;
			}
		}

		public void GetSplatTextures()
		{
			if (CheckValidUnityTerrain())
			{
				splatPrototypes.Clear();
				for (int i = 0; i < terrain.terrainData.splatPrototypes.Length; i++)
				{
					SplatPrototype splatPrototype = terrain.terrainData.splatPrototypes[i];
					TC_SplatPrototype tC_SplatPrototype = new TC_SplatPrototype();
					tC_SplatPrototype.texture = splatPrototype.texture;
					tC_SplatPrototype.normalMap = splatPrototype.normalMap;
					tC_SplatPrototype.metallic = splatPrototype.metallic;
					tC_SplatPrototype.smoothness = splatPrototype.smoothness;
					tC_SplatPrototype.tileOffset = splatPrototype.tileOffset;
					tC_SplatPrototype.tileSize = splatPrototype.tileSize;
					splatPrototypes.Add(tC_SplatPrototype);
				}
			}
		}

		public void CopyTree(TC_TreePrototype treePrototype1, TC_TreePrototype treePrototype2)
		{
			treePrototype2.prefab = treePrototype1.prefab;
			treePrototype2.bendFactor = treePrototype1.bendFactor;
		}

		public void ApplyTrees(TCUnityTerrain sTerrain = null)
		{
			if (!CheckValidUnityTerrain())
			{
				return;
			}
			if (sTerrain == null)
			{
				sTerrain = this;
			}
			if (sTerrain.treePrototypes.Count == 0)
			{
				ResetTrees();
			}
			List<TreePrototype> list = new List<TreePrototype>();
			for (int i = 0; i < sTerrain.treePrototypes.Count; i++)
			{
				TC_TreePrototype tC_TreePrototype = sTerrain.treePrototypes[i];
				if (!(tC_TreePrototype.prefab == null))
				{
					TreePrototype treePrototype = new TreePrototype();
					treePrototype.bendFactor = tC_TreePrototype.bendFactor;
					treePrototype.prefab = tC_TreePrototype.prefab;
					list.Add(treePrototype);
				}
			}
			terrain.terrainData.treePrototypes = list.ToArray();
		}

		public void GetTrees()
		{
			if (CheckValidUnityTerrain())
			{
				treePrototypes.Clear();
				for (int i = 0; i < terrain.terrainData.treePrototypes.Length; i++)
				{
					TreePrototype treePrototype = terrain.terrainData.treePrototypes[i];
					TC_TreePrototype tC_TreePrototype = new TC_TreePrototype();
					tC_TreePrototype.bendFactor = treePrototype.bendFactor;
					tC_TreePrototype.prefab = treePrototype.prefab;
					treePrototypes.Add(tC_TreePrototype);
				}
			}
		}

		public void SwapTree(int index1, int index2)
		{
			if (index2 >= 0 && index2 < treePrototypes.Count)
			{
				TC_TreePrototype value = treePrototypes[index1];
				treePrototypes[index1] = treePrototypes[index2];
				treePrototypes[index2] = value;
			}
		}

		public void ApplyGrass(TCUnityTerrain sTerrain = null)
		{
			if (!CheckValidUnityTerrain())
			{
				return;
			}
			if (sTerrain == null)
			{
				sTerrain = this;
			}
			CleanGrassPrototypes(sTerrain);
			List<DetailPrototype> list = new List<DetailPrototype>();
			float num = sTerrain.grassScaleMulti;
			bool flag = false;
			for (int i = 0; i < sTerrain.detailPrototypes.Count; i++)
			{
				if (list.Count >= 8)
				{
					flag = true;
					break;
				}
				TC_DetailPrototype tC_DetailPrototype = sTerrain.detailPrototypes[i];
				DetailPrototype detailPrototype = new DetailPrototype();
				detailPrototype.bendFactor = tC_DetailPrototype.bendFactor;
				detailPrototype.dryColor = tC_DetailPrototype.dryColor;
				detailPrototype.healthyColor = tC_DetailPrototype.healthyColor;
				detailPrototype.minHeight = tC_DetailPrototype.minHeight * num;
				detailPrototype.maxHeight = tC_DetailPrototype.maxHeight * num;
				detailPrototype.minWidth = tC_DetailPrototype.minWidth * num;
				detailPrototype.maxWidth = tC_DetailPrototype.maxWidth * num;
				detailPrototype.noiseSpread = tC_DetailPrototype.noiseSpread;
				detailPrototype.usePrototypeMesh = tC_DetailPrototype.usePrototypeMesh;
				detailPrototype.prototype = tC_DetailPrototype.prototype;
				detailPrototype.prototypeTexture = tC_DetailPrototype.prototypeTexture;
				detailPrototype.renderMode = tC_DetailPrototype.renderMode;
				TC.SetTextureReadWrite(detailPrototype.prototypeTexture);
				list.Add(detailPrototype);
			}
			if (flag)
			{
				TC.AddMessage("TC2 supports generating maximum 8 grass textures.");
				Debug.Log("TC2 supports generating maximum 8 grass textures.");
			}
			terrain.terrainData.detailPrototypes = list.ToArray();
		}

		public void CleanGrassPrototypes(TCUnityTerrain sTerrain)
		{
			if (sTerrain == null)
			{
				sTerrain = this;
			}
			for (int i = 0; i < sTerrain.detailPrototypes.Count; i++)
			{
				if (sTerrain.detailPrototypes[i].prototypeTexture == null && sTerrain.detailPrototypes[i].prototype == null)
				{
					sTerrain.EraseDetailPrototype(i);
					i--;
				}
			}
		}

		public void GetGrass()
		{
			if (CheckValidUnityTerrain())
			{
				detailPrototypes.Clear();
				for (int i = 0; i < terrain.terrainData.detailPrototypes.Length; i++)
				{
					DetailPrototype detailPrototype = terrain.terrainData.detailPrototypes[i];
					TC_DetailPrototype tC_DetailPrototype = new TC_DetailPrototype();
					tC_DetailPrototype.minHeight = detailPrototype.minHeight / grassScaleMulti;
					tC_DetailPrototype.minWidth = detailPrototype.minWidth / grassScaleMulti;
					tC_DetailPrototype.maxHeight = detailPrototype.maxHeight / grassScaleMulti;
					tC_DetailPrototype.maxWidth = detailPrototype.maxWidth / grassScaleMulti;
					tC_DetailPrototype.bendFactor = detailPrototype.bendFactor;
					tC_DetailPrototype.dryColor = detailPrototype.dryColor;
					tC_DetailPrototype.healthyColor = detailPrototype.healthyColor;
					tC_DetailPrototype.noiseSpread = detailPrototype.noiseSpread;
					tC_DetailPrototype.usePrototypeMesh = detailPrototype.usePrototypeMesh;
					tC_DetailPrototype.prototype = detailPrototype.prototype;
					tC_DetailPrototype.prototypeTexture = detailPrototype.prototypeTexture;
					tC_DetailPrototype.renderMode = detailPrototype.renderMode;
					detailPrototypes.Add(tC_DetailPrototype);
				}
			}
		}

		public void ResetHeightmap(float[,] heights = null)
		{
			if (!(terrain == null))
			{
				int num = terrain.terrainData.heightmapResolution;
				if (heights == null)
				{
					heights = new float[num, num];
				}
				terrain.terrainData.SetHeights(0, 0, heights);
			}
		}

		public void ResetSplatmap(float[,,] splat = null)
		{
			if (terrain == null)
			{
				return;
			}
			int alphamapResolution = terrain.terrainData.alphamapResolution;
			if (splat == null)
			{
				splat = new float[alphamapResolution, alphamapResolution, terrain.terrainData.alphamapLayers];
				for (int i = 0; i < alphamapResolution; i++)
				{
					for (int j = 0; j < alphamapResolution; j++)
					{
						splat[j, i, 0] = 1f;
					}
				}
			}
			terrain.terrainData.SetAlphamaps(0, 0, splat);
		}

		public void ResetTrees()
		{
			if (!(terrain == null))
			{
				TreeInstance[] treeInstances = new TreeInstance[0];
				terrain.terrainData.treeInstances = treeInstances;
			}
		}

		public void ResetGrass()
		{
			if (!(terrain == null))
			{
				int num = terrain.terrainData.detailResolution;
				int[,] details = new int[num, num];
				int num2 = terrain.terrainData.detailPrototypes.Length;
				for (int i = 0; i < num2; i++)
				{
					terrain.terrainData.SetDetailLayer(0, 0, i, details);
				}
			}
		}

		public void ResetObjects()
		{
			if (!(objectsParent == null))
			{
				TC.DestroyChildrenTransform(objectsParent);
			}
		}

		public static Color GetTextureColor(Texture2D tex, int scanPixelCount)
		{
			TC.SetTextureReadWrite(tex);
			Color32[] pixels = tex.GetPixels32();
			double num2;
			double num3;
			double num = (num2 = (num3 = 0.0));
			scanPixelCount *= scanPixelCount;
			int num4 = pixels.Length / scanPixelCount;
			int num5 = 0;
			for (int i = 0; i < pixels.Length; i += num4)
			{
				num += (double)((float)(int)pixels[i].r / 255f);
				num3 += (double)((float)(int)pixels[i].g / 255f);
				num2 += (double)((float)(int)pixels[i].b / 255f);
				num5++;
			}
			return new Color((float)(num / (double)num5), (float)(num3 / (double)num5), (float)(num2 / (double)num5), 1f);
		}
	}
}
