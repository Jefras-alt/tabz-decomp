using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainComposer2
{
	public class TC_TerrainLayer : TC_ItemBehaviour
	{
		[NonSerialized]
		public TC_LayerGroup[] layerGroups = new TC_LayerGroup[6];

		public List<TC_SelectItem> objectSelectItems;

		public List<TC_SelectItem> treeSelectItems;

		public float treeResolutionPM = 128f;

		public float objectResolutionPM = 128f;

		public Vector2 objectAreaSize;

		public Transform objectTransform;

		public int colormapResolution = 128;

		public int meshResolution = 2048;

		public float seedChild;

		public TC_LayerGroup Clone()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(base.gameObject, t.position, t.rotation);
			gameObject.transform.parent = TC_Area2D.current.transform;
			return gameObject.GetComponent<TC_LayerGroup>();
		}

		public void LinkClone(TC_TerrainLayer terrainLayerS)
		{
			for (int i = 0; i < layerGroups.Length; i++)
			{
				TC_LayerGroup tC_LayerGroup = layerGroups[i];
				if (tC_LayerGroup != null)
				{
					tC_LayerGroup.LinkClone(terrainLayerS.layerGroups[i]);
				}
			}
		}

		public void New(bool undo)
		{
			bool flag = TC_Generate.instance.autoGenerate;
			TC_Generate.instance.autoGenerate = false;
			Reset();
			CreateLayerGroups();
			TC_Generate.instance.autoGenerate = flag;
		}

		public void CreateLayerGroups()
		{
			for (int i = 0; i < 6; i++)
			{
				TC_ItemBehaviour tC_ItemBehaviour = (TC_ItemBehaviour)Add<TC_LayerGroup>(TC.outputNames[i] + " Output", false, false, false, i);
				tC_ItemBehaviour.visible = false;
			}
		}

		public int ExecuteCommand(string[] arg)
		{
			if (arg == null)
			{
				return -1;
			}
			if (arg.Length == 0)
			{
				return -1;
			}
			int result = -1;
			if (!(arg[0] == "TerrainLayer") && !(arg[0] == "All"))
			{
				if (arg.Length <= 1)
				{
					return -1;
				}
				int num = TC.OutputNameToOutputID(arg[1]);
				if (num == -1)
				{
					for (int i = 0; i < 6; i++)
					{
						result = layerGroups[i].ExecuteCommand(arg);
					}
				}
				else
				{
					result = layerGroups[num].ExecuteCommand(arg);
				}
			}
			return result;
		}

		public void ResetPlaced()
		{
			layerGroups[3].ResetPlaced();
			layerGroups[5].ResetPlaced();
		}

		public void CalcTreePlaced()
		{
			layerGroups[3].CalcPlaced();
		}

		public void CalcObjectPlaced()
		{
			layerGroups[5].CalcPlaced();
		}

		public void ResetObjects()
		{
			layerGroups[5].ResetObjects();
		}

		public void Reset()
		{
			TC.DestroyChildrenTransform(t);
		}

		public void GetItems(bool refresh)
		{
			GetItems(refresh, true, false);
		}

		public override void GetItems(bool refresh, bool rebuildGlobalLists, bool resetTextures)
		{
			if (!(TC_Settings.instance == null))
			{
				TC_Settings.instance.HasMasterTerrain();
				if (resetTextures)
				{
					TC_Compute.instance.DisposeTextures();
					TC_Area2D.current.DisposeTextures();
					TC_Settings.instance.DisposeTextures();
				}
				for (int i = 0; i < layerGroups.Length; i++)
				{
					GetItem(i, rebuildGlobalLists, resetTextures);
				}
				TC.MoveToDustbinChildren(t, 6);
			}
		}

		public void GetItem(int outputId, bool rebuildGlobalLists, bool resetTextures)
		{
			active = visible;
			if (t.childCount < 6)
			{
				active = false;
				return;
			}
			switch (outputId)
			{
			case 5:
				if (objectSelectItems == null)
				{
					objectSelectItems = new List<TC_SelectItem>();
				}
				else
				{
					objectSelectItems.Clear();
				}
				break;
			case 3:
				if (treeSelectItems == null)
				{
					treeSelectItems = new List<TC_SelectItem>();
				}
				else
				{
					treeSelectItems.Clear();
				}
				break;
			}
			Transform child = t.GetChild(outputId);
			TC_LayerGroup component = child.GetComponent<TC_LayerGroup>();
			if (component != null)
			{
				component.level = 0;
				component.outputId = outputId;
				component.listIndex = outputId;
				component.parentItem = this;
				component.GetItems(true, rebuildGlobalLists, resetTextures);
				layerGroups[outputId] = component;
			}
		}
	}
}
