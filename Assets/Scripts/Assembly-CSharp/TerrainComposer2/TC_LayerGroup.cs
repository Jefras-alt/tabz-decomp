using System;
using UnityEngine;

namespace TerrainComposer2
{
	public class TC_LayerGroup : TC_ItemBehaviour
	{
		[NonSerialized]
		public TC_NodeGroup maskNodeGroup;

		[NonSerialized]
		public TC_LayerGroupResult groupResult;

		public bool doNormalize;

		public float placeLimit = 0.5f;

		public Vector2 nodePos;

		public float seed;

		public int placed;

		public ComputeBuffer ComputeSingle(ref ComputeBuffer totalBuffer, float seedParent, bool first = false)
		{
			if (!groupResult.active)
			{
				return null;
			}
			TC_Compute instance = TC_Compute.instance;
			float seedParent2 = seed + seedParent;
			totalBuffer = groupResult.ComputeSingle(seedParent2, first);
			ComputeBuffer maskBuffer = null;
			if (maskNodeGroup.active)
			{
				maskBuffer = maskNodeGroup.ComputeValue(seedParent2);
			}
			if (maskBuffer != null)
			{
				TC_Compute.InitPreviewRenderTexture(ref rtPreview, "rtPreview_LayerGroup");
				if (method != Method.Lerp || first)
				{
					if (outputId == 0)
					{
						instance.RunComputeMethod(null, null, totalBuffer, ref maskBuffer, 0, rtPreview);
					}
					else
					{
						instance.RunItemComputeMask(this, ref rtPreview, groupResult.rtDisplay, ref totalBuffer, ref maskBuffer);
					}
				}
				rtDisplay = rtPreview;
			}
			else if (outputId == 0 || level == 0 || groupResult.totalActive == 1)
			{
				rtDisplay = groupResult.rtDisplay;
			}
			else
			{
				rtDisplay = rtPreview;
			}
			if (totalBuffer == null)
			{
				TC_Reporter.Log("Layer buffer null");
			}
			return maskBuffer;
		}

		public bool ComputeMulti(ref RenderTexture[] renderTextures, ref ComputeBuffer maskBuffer, float seedParent, bool first = false)
		{
			TC_Compute instance = TC_Compute.instance;
			float seedParent2 = seed + seedParent;
			bool result = groupResult.ComputeMulti(ref renderTextures, seedParent2, doNormalize, first);
			if (maskNodeGroup.active)
			{
				maskBuffer = maskNodeGroup.ComputeValue(seedParent2);
			}
			if (maskBuffer != null)
			{
				TC_Compute.InitPreviewRenderTexture(ref rtPreview, "rtPreview_LayerGroup_" + TC.outputNames[outputId]);
				if (method != Method.Lerp || first)
				{
					if (outputId == 2)
					{
						instance.RunComputeColorMethod(this, ref renderTextures[0], maskBuffer, groupResult.rtDisplay);
					}
					else
					{
						instance.RunComputeMultiMethod(this, doNormalize, ref renderTextures, maskBuffer, groupResult.rtDisplay);
					}
				}
				rtDisplay = rtPreview;
			}
			else
			{
				rtDisplay = groupResult.rtDisplay;
			}
			return result;
		}

		public void ResetPlaced()
		{
			groupResult.ResetPlaced();
		}

		public int CalcPlaced()
		{
			placed = groupResult.CalcPlaced();
			return placed;
		}

		public void LinkClone(TC_LayerGroup layerGroupS)
		{
			preview = layerGroupS.preview;
			maskNodeGroup.LinkClone(layerGroupS.maskNodeGroup);
			groupResult.LinkClone(layerGroupS.groupResult);
		}

		public override void SetLockChildrenPosition(bool lockPos)
		{
			lockPosParent = lockPos;
			groupResult.SetLockChildrenPosition(lockPosParent || lockPosChildren);
			maskNodeGroup.SetLockChildrenPosition(lockPosParent || lockPosChildren);
		}

		public override void UpdateTransforms()
		{
			ct.Copy(this);
			groupResult.UpdateTransforms();
		}

		public override void SetFirstLoad(bool active)
		{
			base.SetFirstLoad(active);
			maskNodeGroup.SetFirstLoad(active);
			groupResult.SetFirstLoad(active);
		}

		public void ResetObjects()
		{
			groupResult.ResetObjects();
		}

		public override void GetItems(bool refresh, bool rebuildGlobalLists, bool resetTextures)
		{
			bool flag = true;
			active = visible;
			if (resetTextures)
			{
				DisposeTextures();
			}
			maskNodeGroup = GetGroup<TC_NodeGroup>(0, refresh, resetTextures);
			if (maskNodeGroup == null)
			{
				active = false;
			}
			else
			{
				maskNodeGroup.type = NodeGroupType.Mask;
				if (maskNodeGroup.active)
				{
					if (flag)
					{
						bounds = maskNodeGroup.bounds;
					}
					else
					{
						bounds.Encapsulate(maskNodeGroup.bounds);
					}
				}
			}
			if (t.childCount <= 1)
			{
				active = false;
				return;
			}
			Transform child = t.GetChild(1);
			groupResult = child.GetComponent<TC_LayerGroupResult>();
			if (groupResult == null)
			{
				TC.MoveToDustbin(child);
				active = false;
				return;
			}
			groupResult.SetParameters(this, 1);
			groupResult.GetItems(refresh, rebuildGlobalLists, resetTextures);
			if (!groupResult.active)
			{
				active = false;
			}
		}

		public override void ChangeYPosition(float y)
		{
			if (groupResult != null)
			{
				groupResult.ChangeYPosition(y);
			}
		}

		public override bool ContainsCollisionNode()
		{
			if (groupResult != null)
			{
				return groupResult.ContainsCollisionNode();
			}
			return false;
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
			if (arg[0] == "LayerGroup" || arg[0] == "All")
			{
			}
			if (arg[0] != "LayerGroup")
			{
				if (arg.Length <= 1)
				{
					return -1;
				}
				if (groupResult != null)
				{
					result = groupResult.ExecuteCommand(arg);
				}
			}
			return result;
		}
	}
}
