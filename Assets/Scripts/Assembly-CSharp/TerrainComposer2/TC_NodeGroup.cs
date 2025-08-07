using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainComposer2
{
	public class TC_NodeGroup : TC_GroupBehaviour
	{
		[NonSerialized]
		public List<TC_ItemBehaviour> itemList = new List<TC_ItemBehaviour>();

		public int nodeGroupLevel;

		public NodeGroupType type;

		public RenderTexture rtColorPreview;

		public bool useConstant;

		public float seed;

		public override void Awake()
		{
			rtColorPreview = null;
			base.Awake();
		}

		public override void OnDestroy()
		{
			DisposeTextures();
		}

		public override void DisposeTextures()
		{
			base.DisposeTextures();
			TC_Compute.DisposeRenderTexture(ref rtColorPreview);
		}

		public void LinkClone(TC_NodeGroup nodeGroupS)
		{
			preview = nodeGroupS.preview;
			for (int i = 0; i < itemList.Count; i++)
			{
				TC_Node tC_Node = itemList[i] as TC_Node;
				if (tC_Node != null)
				{
					TC_Node tC_Node2 = nodeGroupS.itemList[i] as TC_Node;
					tC_Node.preview = tC_Node2.preview;
					tC_Node.Init();
				}
			}
		}

		public override void SetLockChildrenPosition(bool lockPos)
		{
			lockPosParent = lockPos;
			for (int i = 0; i < itemList.Count; i++)
			{
				TC_Node tC_Node = itemList[i] as TC_Node;
				if (tC_Node != null)
				{
					tC_Node.lockPosParent = lockPosParent || lockPosChildren;
				}
			}
		}

		public override void UpdateTransforms()
		{
			for (int i = 0; i < itemList.Count; i++)
			{
				TC_Node tC_Node = itemList[i] as TC_Node;
				if (tC_Node != null)
				{
					tC_Node.ct.CopySpecial(tC_Node);
				}
			}
		}

		public ComputeBuffer ComputeValue(float seedParent)
		{
			TC_Compute instance = TC_Compute.instance;
			if (instance == null)
			{
				Debug.Log("Compute is null");
			}
			ComputeBuffer rightBuffer = null;
			ComputeBuffer computeBuffer = null;
			if (totalActive > 1)
			{
				InitPreviewRenderTexture(true, base.name);
			}
			int num = (useConstant ? 1 : itemList.Count);
			float seedParent2 = seed + seedParent;
			for (int i = 0; i < num; i++)
			{
				TC_Node tC_Node = itemList[i] as TC_Node;
				if (tC_Node != null)
				{
					tC_Node.Init();
					if (!tC_Node.active)
					{
						continue;
					}
					if (tC_Node.clamp)
					{
					}
					bool flag = tC_Node.inputKind == InputKind.Current;
					tC_Node.InitPreviewRenderTexture(true, tC_Node.name);
					if (computeBuffer == null && !flag)
					{
						computeBuffer = instance.RunNodeCompute(this, tC_Node, seedParent2);
					}
					else if (!flag)
					{
						rightBuffer = instance.RunNodeCompute(this, tC_Node, seedParent2, computeBuffer);
					}
					else
					{
						for (int j = 0; j < tC_Node.iterations; j++)
						{
							computeBuffer = instance.RunNodeCompute(this, tC_Node, seedParent2, computeBuffer, true);
						}
					}
					if (computeBuffer != null && rightBuffer != null && !flag)
					{
						instance.RunComputeMethod(this, tC_Node, computeBuffer, ref rightBuffer, itemList.Count, (i != lastActive) ? null : rtPreview);
					}
					continue;
				}
				TC_NodeGroup tC_NodeGroup = itemList[i] as TC_NodeGroup;
				if (tC_NodeGroup != null && tC_NodeGroup.active)
				{
					if (computeBuffer == null)
					{
						computeBuffer = tC_NodeGroup.ComputeValue(seedParent2);
					}
					else
					{
						rightBuffer = tC_NodeGroup.ComputeValue(seedParent2);
					}
					if (computeBuffer != null && rightBuffer != null)
					{
						instance.RunComputeMethod(this, tC_NodeGroup, computeBuffer, ref rightBuffer, itemList.Count, (i != lastActive) ? null : rtPreview);
					}
				}
			}
			if (totalActive == 1)
			{
				TC_Compute.DisposeRenderTexture(ref rtPreview);
				rtDisplay = itemList[firstActive].rtDisplay;
			}
			return computeBuffer;
		}

		public override void ChangeYPosition(float y)
		{
			for (int i = 0; i < itemList.Count; i++)
			{
				itemList[i].ChangeYPosition(y);
			}
		}

		public override void SetFirstLoad(bool active)
		{
			base.SetFirstLoad(active);
			for (int i = 0; i < itemList.Count; i++)
			{
				itemList[i].SetFirstLoad(active);
			}
		}

		public override bool ContainsCollisionNode()
		{
			for (int i = 0; i < itemList.Count; i++)
			{
				TC_Node tC_Node = itemList[i] as TC_Node;
				if (tC_Node != null && tC_Node.active && tC_Node.visible && tC_Node.inputKind == InputKind.Terrain && tC_Node.inputTerrain == InputTerrain.Collision)
				{
					return true;
				}
			}
			return false;
		}

		public override void GetItems(bool refresh, bool rebuildGlobalLists, bool resetTextures)
		{
			if (resetTextures)
			{
				DisposeTextures();
			}
			int childCount = base.transform.childCount;
			itemList.Clear();
			active = visible;
			firstActive = (lastActive = -1);
			totalActive = 0;
			bool flag = true;
			int num = 0;
			for (int num2 = childCount - 1; num2 >= 0; num2--)
			{
				Transform child = t.GetChild(num2);
				TC_Node component = child.GetComponent<TC_Node>();
				if (component != null)
				{
					if (resetTextures)
					{
						component.DisposeTextures();
					}
					component.active = component.visible;
					component.SetParameters(this, num);
					component.nodeGroupLevel = nodeGroupLevel + 1;
					component.nodeType = type;
					component.Init();
					component.UpdateVersion();
					if (component.active)
					{
						if (component.clamp)
						{
							component.CalcBounds();
						}
						if (flag)
						{
							bounds = component.bounds;
							flag = false;
						}
						else
						{
							bounds.Encapsulate(component.bounds);
						}
						lastActive = num;
						if (firstActive == -1)
						{
							firstActive = lastActive;
						}
						totalActive++;
					}
					if (num2 == childCount - 1 && component.method != Method.Add && component.method != Method.Subtract)
					{
						component.method = Method.Add;
					}
					itemList.Add(component);
					num++;
				}
				else
				{
					TC_NodeGroup component2 = child.GetComponent<TC_NodeGroup>();
					if (component2 != null)
					{
						component2.SetParameters(this, num);
						component2.nodeGroupLevel = nodeGroupLevel + 1;
						itemList.Add(component2);
						num++;
						component2.GetItems(refresh, rebuildGlobalLists, resetTextures);
						if (component2.active)
						{
							lastActive = num;
							if (firstActive == -1)
							{
								firstActive = lastActive;
							}
							totalActive++;
						}
					}
				}
			}
			if (itemList.Count == 1 && itemList[0].active)
			{
				active = (visible = true);
			}
			if (!active)
			{
				totalActive = 0;
			}
			if (totalActive == 0)
			{
				active = false;
			}
		}
	}
}
