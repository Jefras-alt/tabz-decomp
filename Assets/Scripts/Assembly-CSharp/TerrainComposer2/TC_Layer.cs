using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainComposer2
{
	public class TC_Layer : TC_ItemBehaviour
	{
		[NonSerialized]
		public TC_SelectItemGroup selectItemGroup;

		[NonSerialized]
		public TC_NodeGroup maskNodeGroup;

		[NonSerialized]
		public TC_NodeGroup selectNodeGroup;

		public List<TC_SelectItem.DistanceRule> distanceRules;

		public bool doNormalize;

		public float placeLimit = 0.5f;

		public float selectValue;

		public float maskValue;

		public float seed;

		public int placed;

		private float splatTotal;

		private float x;

		private float y;

		public void ComputeHeight(ref ComputeBuffer layerBuffer, ref ComputeBuffer maskBuffer, float seedParent, bool first = false)
		{
			TC_Compute instance = TC_Compute.instance;
			float seedParent2 = seed + seedParent;
			layerBuffer = selectNodeGroup.ComputeValue(seedParent2);
			if (layerBuffer != null)
			{
				if (maskNodeGroup.active)
				{
					maskBuffer = maskNodeGroup.ComputeValue(seedParent2);
				}
				if (maskBuffer != null)
				{
					if (method != Method.Lerp || first)
					{
						InitPreviewRenderTexture(true, "rtPreview_Layer_" + TC.outputNames[outputId]);
						instance.RunComputeMethod(null, null, layerBuffer, ref maskBuffer, 0, rtPreview);
					}
				}
				else
				{
					rtDisplay = selectNodeGroup.rtDisplay;
				}
			}
			else
			{
				TC_Reporter.Log("Layerbuffer " + listIndex + " = null, reporting from layer");
			}
		}

		public bool ComputeMulti(ref RenderTexture[] renderTextures, ref ComputeBuffer maskBuffer, float seedParent, bool first = false)
		{
			TC_Compute instance = TC_Compute.instance;
			bool result = false;
			float seedParent2 = seed + seedParent;
			ComputeBuffer resultBuffer = selectNodeGroup.ComputeValue(seedParent2);
			if (resultBuffer != null)
			{
				result = true;
				TC_Compute.InitPreviewRenderTexture(ref rtPreview, "rtPreview_Layer");
				if (maskNodeGroup.active)
				{
					maskBuffer = maskNodeGroup.ComputeValue(seedParent2);
				}
				TC_Compute.InitPreviewRenderTexture(ref selectNodeGroup.rtColorPreview, "rtNodeGroupPreview_" + TC.outputNames[outputId]);
				if (outputId == 2)
				{
					instance.RunColorCompute(selectNodeGroup, selectItemGroup, ref renderTextures[0], ref resultBuffer);
				}
				else
				{
					instance.RunSplatCompute(selectNodeGroup, selectItemGroup, ref renderTextures, ref resultBuffer);
				}
				instance.DisposeBuffer(ref resultBuffer);
				if (maskBuffer != null)
				{
					TC_Reporter.Log("Run layer select * mask");
					if (method != Method.Lerp || first)
					{
						if (outputId == 2)
						{
							instance.RunComputeColorMethod(this, ref renderTextures[0], maskBuffer, rtPreview);
						}
						else
						{
							instance.RunComputeMultiMethod(this, doNormalize, ref renderTextures, maskBuffer, rtPreview);
						}
					}
					rtDisplay = rtPreview;
				}
				else
				{
					TC_Reporter.Log("No mask buffer assign colorPreviewTex to layer");
					rtDisplay = selectNodeGroup.rtColorPreview;
				}
			}
			return result;
		}

		public bool ComputeItem(ref ComputeBuffer itemMapBuffer, ref ComputeBuffer maskBuffer, float seedParent, bool first = false)
		{
			TC_Compute instance = TC_Compute.instance;
			bool result = false;
			float seedParent2 = seed + seedParent;
			ComputeBuffer resultBuffer = selectNodeGroup.ComputeValue(seedParent2);
			if (resultBuffer != null)
			{
				result = true;
				TC_Compute.InitPreviewRenderTexture(ref rtPreview, "rtPreview_Layer_" + TC.outputNames[outputId]);
				rtDisplay = rtPreview;
				TC_Compute.InitPreviewRenderTexture(ref selectNodeGroup.rtColorPreview, "rtColorPreview");
				instance.RunItemCompute(this, ref itemMapBuffer, ref resultBuffer);
				instance.DisposeBuffer(ref resultBuffer);
				if (maskNodeGroup.active)
				{
					maskBuffer = maskNodeGroup.ComputeValue(seedParent2);
				}
				if (maskBuffer != null)
				{
					TC_Reporter.Log("Run layer select * mask");
					if (method != Method.Lerp || first)
					{
						instance.RunItemComputeMask(this, ref rtPreview, selectNodeGroup.rtColorPreview, ref itemMapBuffer, ref maskBuffer);
					}
				}
			}
			return result;
		}

		public void LinkClone(TC_Layer layerS)
		{
			preview = layerS.preview;
			maskNodeGroup.LinkClone(layerS.maskNodeGroup);
			selectNodeGroup.LinkClone(layerS.selectNodeGroup);
		}

		public void ResetPlaced()
		{
			selectItemGroup.ResetPlaced();
		}

		public int CalcPlaced()
		{
			placed = selectItemGroup.CalcPlaced();
			return placed;
		}

		public void ResetObjects()
		{
			selectItemGroup.ResetObjects();
		}

		public override void GetItems(bool refresh, bool rebuildGlobalLists, bool resetTextures)
		{
			if (resetTextures)
			{
				DisposeTextures();
			}
			active = visible;
			bool flag = true;
			maskNodeGroup = GetGroup<TC_NodeGroup>(0, refresh, resetTextures);
			if (maskNodeGroup != null)
			{
				maskNodeGroup.type = NodeGroupType.Mask;
				if (maskNodeGroup.totalActive > 0)
				{
					bounds = maskNodeGroup.bounds;
					flag = false;
				}
			}
			selectNodeGroup = GetGroup<TC_NodeGroup>(1, refresh, resetTextures);
			if (selectNodeGroup != null)
			{
				selectNodeGroup.type = NodeGroupType.Select;
				if (selectNodeGroup.totalActive == 0)
				{
					TC_Reporter.Log("SelectNodeGroup 0 active");
					active = false;
				}
				else if (flag)
				{
					bounds = selectNodeGroup.bounds;
				}
				else
				{
					bounds.Encapsulate(selectNodeGroup.bounds);
				}
			}
			else
			{
				active = false;
			}
			if (outputId == 0)
			{
				return;
			}
			selectItemGroup = GetGroup<TC_SelectItemGroup>(2, refresh, resetTextures);
			if (selectItemGroup != null)
			{
				if (selectItemGroup.totalActive == 0)
				{
					TC_Reporter.Log("itemGroup 0 active");
					active = false;
				}
				else if (selectItemGroup.itemList.Count <= 1)
				{
					selectNodeGroup.useConstant = true;
					if (selectNodeGroup.itemList.Count > 0)
					{
						selectNodeGroup.itemList[0].visible = true;
						active = visible;
						GetGroup<TC_NodeGroup>(1, true, resetTextures);
					}
				}
				else
				{
					selectNodeGroup.useConstant = false;
				}
			}
			else
			{
				active = false;
			}
		}

		public override void SetLockChildrenPosition(bool lockPos)
		{
			lockPosParent = lockPos;
			if (maskNodeGroup != null)
			{
				maskNodeGroup.SetLockChildrenPosition(lockPosParent || lockPosChildren);
			}
			if (selectNodeGroup != null)
			{
				selectNodeGroup.SetLockChildrenPosition(lockPosParent || lockPosChildren);
			}
		}

		public override void UpdateTransforms()
		{
			maskNodeGroup.UpdateTransforms();
			selectNodeGroup.UpdateTransforms();
		}

		public override void ChangeYPosition(float y)
		{
			selectNodeGroup.ChangeYPosition(y);
		}

		public override void SetFirstLoad(bool active)
		{
			base.SetFirstLoad(active);
			maskNodeGroup.SetFirstLoad(active);
			selectNodeGroup.SetFirstLoad(active);
			selectItemGroup.SetFirstLoad(active);
		}

		public override bool ContainsCollisionNode()
		{
			if (selectNodeGroup.ContainsCollisionNode())
			{
				return true;
			}
			if (maskNodeGroup.ContainsCollisionNode())
			{
				return true;
			}
			return false;
		}
	}
}
