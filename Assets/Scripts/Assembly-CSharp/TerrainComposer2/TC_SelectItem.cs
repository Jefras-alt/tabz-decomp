using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainComposer2
{
	public class TC_SelectItem : TC_ItemBehaviour
	{
		[Serializable]
		public class SpawnObject
		{
			public enum ParentMode
			{
				Terrain = 0,
				Existing = 1,
				Create = 2
			}

			public GameObject go;

			public bool linkToPrefab;

			public ParentMode parentMode;

			public string parentName;

			public Transform parentT;

			public Transform newParentT;

			public bool parentToTerrain;

			public float randomPosition = 1f;

			public Vector2 heightRange = Vector2.zero;

			public bool includeScale;

			public float heightOffset;

			public bool includeTerrainHeight = true;

			public Vector2 rotRangeX = Vector2.zero;

			public Vector2 rotRangeY = Vector2.zero;

			public Vector2 rotRangeZ = Vector2.zero;

			public bool isSnapRot;

			public bool isSnapRotX = true;

			public bool isSnapRotY = true;

			public bool isSnapRotZ = true;

			public float snapRotX = 45f;

			public float snapRotY = 45f;

			public float snapRotZ = 45f;

			public bool customScaleRange;

			public Vector2 scaleRangeX = Vector2.one;

			public Vector2 scaleRangeY = Vector2.one;

			public Vector2 scaleRangeZ = Vector2.one;

			public Vector2 scaleRange = Vector2.one;

			public float scaleMulti = 1f;

			public float nonUniformScale;

			public AnimationCurve scaleCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

			public Transform lookAtTarget;

			public bool lookAtX;
		}

		[Serializable]
		public class Tree
		{
			public float randomPosition = 1f;

			public float heightOffset;

			public Vector2 scaleRange = new Vector2(0.5f, 2f);

			public float nonUniformScale = 0.2f;

			public float scaleMulti = 1f;

			public AnimationCurve scaleCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
		}

		[Serializable]
		public class DistanceRule
		{
			public List<TC_ItemBehaviour> items;

			public Vector2 range;
		}

		[NonSerialized]
		public new TC_SelectItemGroup parentItem;

		public Vector2 range;

		public GameObject oldSpawnObject;

		public Tree tree;

		public SpawnObject spawnObject;

		public Color color = Color.white;

		public int selectIndex;

		public float[] splatCustomValues;

		public bool splatCustom;

		public float splatCustomTotal;

		public int globalListIndex = -1;

		public int placed;

		[NonSerialized]
		private TC_Settings settings;

		public override void Awake()
		{
			base.Awake();
			t.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
		}

		public override void OnEnable()
		{
			base.OnEnable();
			t.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
		}

		public void CalcSplatCustomTotal()
		{
			splatCustomTotal = 0f;
			for (int i = 0; i < splatCustomValues.Length; i++)
			{
				splatCustomTotal += splatCustomValues[i];
			}
		}

		public void ResetObjects()
		{
			if (spawnObject == null)
			{
				return;
			}
			if (spawnObject.parentMode == SpawnObject.ParentMode.Create)
			{
				if (spawnObject.newParentT != null)
				{
					UnityEngine.Object.DestroyImmediate(spawnObject.newParentT.gameObject);
				}
			}
			else if (spawnObject.parentMode == SpawnObject.ParentMode.Existing && spawnObject.parentT != null)
			{
				TC.DestroyChildrenTransform(spawnObject.parentT);
			}
		}

		public void SetPreviewColor()
		{
			if (outputId == 2)
			{
				return;
			}
			TC_GlobalSettings global = TC_Settings.instance.global;
			if (selectIndex == -1)
			{
				selectIndex = 0;
			}
			if (outputId == 1)
			{
				if (splatCustom)
				{
					color = Color.black;
					for (int i = 0; i < splatCustomValues.Length; i++)
					{
						color += splatCustomValues[i] * global.GetVisualizeColor(i);
					}
					color /= splatCustomTotal;
				}
				else
				{
					color = global.GetVisualizeColor(selectIndex);
				}
			}
			else if (outputId == 3 || outputId == 5)
			{
				color = global.GetVisualizeColor(listIndex);
			}
			else
			{
				color = global.GetVisualizeColor(selectIndex);
			}
		}

		public int GetItemTotalFromTerrain()
		{
			TC_Settings instance = TC_Settings.instance;
			if (!instance.hasMasterTerrain)
			{
				return 0;
			}
			if (outputId == 1)
			{
				return instance.masterTerrain.terrainData.splatPrototypes.Length;
			}
			if (outputId == 4)
			{
				return instance.masterTerrain.terrainData.detailPrototypes.Length;
			}
			if (outputId == 3)
			{
				return instance.masterTerrain.terrainData.treePrototypes.Length;
			}
			return 0;
		}

		public void Refresh()
		{
			parentItem.GetItems(true, false, false);
		}

		public void SetPreviewItemTexture()
		{
			if (outputId == 1)
			{
				SetPreviewSplatTexture();
			}
			else if (outputId == 2)
			{
				SetPreviewColorTexture();
			}
			else if (outputId == 3)
			{
				SetPreviewTreeTexture();
			}
			else if (outputId == 4)
			{
				SetPreviewGrassTexture();
			}
			else if (outputId == 5)
			{
				SetPreviewObjectTexture();
			}
		}

		public void SetPreviewSplatTexture()
		{
			TC_Settings instance = TC_Settings.instance;
			if (instance.hasMasterTerrain)
			{
				if (selectIndex < instance.masterTerrain.terrainData.splatPrototypes.Length && selectIndex >= 0)
				{
					preview.tex = instance.masterTerrain.terrainData.splatPrototypes[selectIndex].texture;
					if (preview.tex != null)
					{
						base.name = Mathw.CutString(preview.tex.name, 19);
					}
				}
				else
				{
					active = false;
				}
			}
			else
			{
				preview.tex = null;
			}
		}

		public void SetPreviewTreeTexture()
		{
		}

		public void SetPreviewObjectTexture()
		{
		}

		public void SetPreviewGrassTexture()
		{
			TC_Settings instance = TC_Settings.instance;
			if (instance.hasMasterTerrain)
			{
				if (selectIndex < instance.masterTerrain.terrainData.detailPrototypes.Length && selectIndex >= 0)
				{
					DetailPrototype detailPrototype = instance.masterTerrain.terrainData.detailPrototypes[selectIndex];
					if (!detailPrototype.usePrototypeMesh)
					{
						preview.tex = detailPrototype.prototypeTexture;
					}
					if (preview.tex != null)
					{
						base.name = Mathw.CutString(preview.tex.name, 19);
					}
				}
				else
				{
					active = false;
				}
			}
			else
			{
				preview.tex = null;
			}
		}

		public void SetPreviewColorTexture()
		{
		}
	}
}
