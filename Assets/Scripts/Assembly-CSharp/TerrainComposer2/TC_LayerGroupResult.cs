using System.Collections.Generic;
using UnityEngine;

namespace TerrainComposer2
{
	public class TC_LayerGroupResult : TC_GroupBehaviour
	{
		public List<TC_ItemBehaviour> itemList = new List<TC_ItemBehaviour>();

		public float seed;

		public ComputeBuffer ComputeSingle(float seedParent, bool first = false)
		{
			TC_Compute instance = TC_Compute.instance;
			ComputeBuffer totalBuffer = null;
			ComputeBuffer totalBuffer2 = null;
			ComputeBuffer buffer = null;
			RenderTexture[] rts = null;
			RenderTexture renderTexture = null;
			RenderTexture rtLeftPreview = null;
			if (outputId != 0)
			{
				rts = new RenderTexture[2];
			}
			SetPreviewTextureBefore();
			int num = 0;
			float seedParent2 = seed + seedParent;
			for (int i = 0; i < itemList.Count; i++)
			{
				TC_Layer tC_Layer = itemList[i] as TC_Layer;
				if (tC_Layer != null)
				{
					if (!tC_Layer.active)
					{
						TC_Reporter.Log("Inactive layer " + i);
						continue;
					}
					if (totalBuffer == null)
					{
						if (outputId == 0)
						{
							tC_Layer.ComputeHeight(ref totalBuffer, ref buffer, seedParent2, i == firstActive);
						}
						else
						{
							tC_Layer.ComputeItem(ref totalBuffer, ref buffer, seedParent2, i == firstActive);
							if (totalBuffer != null)
							{
								rtLeftPreview = tC_Layer.rtDisplay;
							}
						}
						TC_Area2D.current.layerGroupBuffer = totalBuffer;
						instance.DisposeBuffer(ref buffer);
						continue;
					}
					if (outputId == 0)
					{
						tC_Layer.ComputeHeight(ref totalBuffer2, ref buffer, seedParent2);
					}
					else
					{
						tC_Layer.ComputeItem(ref totalBuffer2, ref buffer, seedParent2);
					}
					if (totalBuffer2 != null)
					{
						if (outputId == 0)
						{
							instance.RunComputeMethod(this, tC_Layer, totalBuffer, ref totalBuffer2, totalActive, (i != lastActive) ? null : rtPreview, buffer);
						}
						else
						{
							renderTexture = tC_Layer.rtDisplay;
							instance.RunComputeObjectMethod(this, tC_Layer, totalBuffer, ref totalBuffer2, buffer, rtPreview, ref rts[num++ % 2], ref rtLeftPreview, renderTexture);
						}
						TC_Area2D.current.layerGroupBuffer = totalBuffer;
					}
					instance.DisposeBuffer(ref buffer);
					continue;
				}
				TC_LayerGroup tC_LayerGroup = itemList[i] as TC_LayerGroup;
				if (tC_LayerGroup == null || !tC_LayerGroup.active)
				{
					continue;
				}
				if (totalBuffer == null)
				{
					buffer = tC_LayerGroup.ComputeSingle(ref totalBuffer, seedParent2, i == firstActive);
					if (totalBuffer != null)
					{
						rtLeftPreview = tC_LayerGroup.rtDisplay;
					}
					TC_Area2D.current.layerGroupBuffer = totalBuffer;
					instance.DisposeBuffer(ref buffer);
					continue;
				}
				buffer = tC_LayerGroup.ComputeSingle(ref totalBuffer2, seedParent2);
				if (totalBuffer2 != null)
				{
					if (outputId == 0)
					{
						instance.RunComputeMethod(this, tC_LayerGroup, totalBuffer, ref totalBuffer2, totalActive, (i != lastActive) ? null : rtPreview, buffer);
					}
					else
					{
						renderTexture = tC_LayerGroup.rtDisplay;
						instance.RunComputeObjectMethod(this, tC_LayerGroup, totalBuffer, ref totalBuffer2, buffer, rtPreview, ref rts[num++ % 2], ref rtLeftPreview, renderTexture);
					}
					TC_Area2D.current.layerGroupBuffer = totalBuffer;
				}
				instance.DisposeBuffer(ref buffer);
			}
			SetPreviewTextureAfter();
			if (outputId != 0)
			{
				TC_Compute.DisposeRenderTextures(ref rts);
			}
			instance.DisposeBuffer(ref buffer);
			if (totalBuffer == null)
			{
				TC_Reporter.Log("Layer buffer null");
			}
			return totalBuffer;
		}

		public bool ComputeMulti(ref RenderTexture[] renderTextures, float seedParent, bool doNormalize, bool first = false)
		{
			TC_Compute instance = TC_Compute.instance;
			RenderTexture[] rts = null;
			RenderTexture renderTexture = null;
			RenderTexture rtLeftPreview = null;
			RenderTexture[] array = null;
			ComputeBuffer maskBuffer = null;
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			array = new RenderTexture[2];
			SetPreviewTextureBefore();
			float seedParent2 = seed + seedParent;
			for (int i = 0; i < itemList.Count; i++)
			{
				TC_Layer tC_Layer = itemList[i] as TC_Layer;
				if (tC_Layer != null)
				{
					if (!tC_Layer.active)
					{
						continue;
					}
					if (!flag)
					{
						flag = tC_Layer.ComputeMulti(ref renderTextures, ref maskBuffer, seedParent2, i == firstActive);
						if (flag)
						{
							rtLeftPreview = tC_Layer.rtDisplay;
							TC_Reporter.Log("firt compute " + tC_Layer.maskNodeGroup.totalActive);
							instance.DisposeBuffer(ref maskBuffer);
						}
						continue;
					}
					TC_Compute.InitRenderTextures(ref rts, "rtsLayer", renderTextures.Length);
					if (tC_Layer.ComputeMulti(ref rts, ref maskBuffer, seedParent2))
					{
						TC_Reporter.Log("Run layer method multi");
						renderTexture = ((tC_Layer.method != Method.Lerp) ? tC_Layer.rtDisplay : tC_Layer.selectNodeGroup.rtColorPreview);
						if (outputId == 2)
						{
							instance.RunComputeColorMethod(tC_Layer, tC_Layer.method, ref renderTextures[0], ref rts[0], maskBuffer, rtPreview, ref array[num++ % 2], ref rtLeftPreview, renderTexture);
						}
						else
						{
							instance.RunComputeMultiMethod(tC_Layer, tC_Layer.method, i == lastActive && doNormalize, ref renderTextures, ref rts, maskBuffer, rtPreview, ref array[num++ % 2], ref rtLeftPreview, renderTexture);
						}
						instance.DisposeBuffer(ref maskBuffer);
					}
					continue;
				}
				TC_LayerGroup tC_LayerGroup = itemList[i] as TC_LayerGroup;
				if (tC_LayerGroup == null || !tC_LayerGroup.active)
				{
					continue;
				}
				if (!flag)
				{
					flag = tC_LayerGroup.ComputeMulti(ref renderTextures, ref maskBuffer, seedParent2, i == firstActive);
					if (flag)
					{
						rtLeftPreview = tC_LayerGroup.rtDisplay;
						instance.DisposeBuffer(ref maskBuffer);
						TC_Reporter.Log("LayerGroup did first compute");
					}
					continue;
				}
				TC_Compute.InitRenderTextures(ref rts, "rtsLayer", renderTextures.Length);
				if (tC_LayerGroup.ComputeMulti(ref rts, ref maskBuffer, seedParent2))
				{
					renderTexture = ((tC_LayerGroup.method != Method.Lerp) ? tC_LayerGroup.rtDisplay : tC_LayerGroup.groupResult.rtDisplay);
					if (outputId == 2)
					{
						instance.RunComputeColorMethod(tC_LayerGroup, tC_LayerGroup.method, ref renderTextures[0], ref rts[0], maskBuffer, rtPreview, ref array[num++ % 2], ref rtLeftPreview, renderTexture);
					}
					else
					{
						instance.RunComputeMultiMethod(tC_LayerGroup, tC_LayerGroup.method, i == lastActive && doNormalize, ref renderTextures, ref rts, maskBuffer, rtPreview, ref array[num++ % 2], ref rtLeftPreview, renderTexture);
					}
					instance.DisposeBuffer(ref maskBuffer);
				}
			}
			SetPreviewTextureAfter();
			if (maskBuffer != null)
			{
				instance.DisposeBuffer(ref maskBuffer);
				TC_Reporter.Log("Dispose layerMaskBuffer");
			}
			TC_Compute.DisposeRenderTextures(ref array);
			TC_Compute.DisposeRenderTextures(ref rts);
			return flag;
		}

		public void SetPreviewTextureBefore()
		{
			if (totalActive == 0)
			{
				active = false;
				rtDisplay = null;
				TC_Compute.DisposeRenderTexture(ref rtPreview);
			}
			else if (totalActive != 1)
			{
				TC_Compute.InitPreviewRenderTexture(ref rtPreview, "rtGroupResult");
				rtDisplay = rtPreview;
			}
		}

		public void SetPreviewTextureAfter()
		{
			if (totalActive == 1)
			{
				TC_Compute.DisposeRenderTexture(ref rtPreview);
				rtDisplay = itemList[firstActive].rtDisplay;
			}
		}

		public void LinkClone(TC_LayerGroupResult resultLayerGroupS)
		{
			preview = resultLayerGroupS.preview;
			for (int i = 0; i < itemList.Count; i++)
			{
				TC_Layer tC_Layer = itemList[i] as TC_Layer;
				if (tC_Layer != null)
				{
					TC_Layer layerS = resultLayerGroupS.itemList[i] as TC_Layer;
					tC_Layer.LinkClone(layerS);
					continue;
				}
				TC_LayerGroup tC_LayerGroup = itemList[i] as TC_LayerGroup;
				if (tC_LayerGroup != null)
				{
					TC_LayerGroup layerGroupS = resultLayerGroupS.itemList[i] as TC_LayerGroup;
					tC_LayerGroup.LinkClone(layerGroupS);
				}
			}
		}

		public void ResetPlaced()
		{
			for (int i = 0; i < itemList.Count; i++)
			{
				TC_Layer tC_Layer = itemList[i] as TC_Layer;
				if (tC_Layer != null)
				{
					tC_Layer.ResetPlaced();
					continue;
				}
				TC_LayerGroup tC_LayerGroup = itemList[i] as TC_LayerGroup;
				if (tC_LayerGroup != null)
				{
					tC_LayerGroup.ResetPlaced();
				}
			}
		}

		public int CalcPlaced()
		{
			int num = 0;
			for (int i = 0; i < itemList.Count; i++)
			{
				TC_Layer tC_Layer = itemList[i] as TC_Layer;
				if (tC_Layer != null)
				{
					num += tC_Layer.CalcPlaced();
					continue;
				}
				TC_LayerGroup tC_LayerGroup = itemList[i] as TC_LayerGroup;
				if (tC_LayerGroup != null)
				{
					num += tC_LayerGroup.CalcPlaced();
				}
			}
			return num;
		}

		public override void SetLockChildrenPosition(bool lockPos)
		{
			lockPosParent = lockPos;
			for (int i = 0; i < itemList.Count; i++)
			{
				itemList[i].SetLockChildrenPosition(lockPosParent || lockPosChildren);
			}
		}

		public override void UpdateTransforms()
		{
			for (int i = 0; i < itemList.Count; i++)
			{
				itemList[i].UpdateTransforms();
			}
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
			bool flag = false;
			for (int i = 0; i < itemList.Count; i++)
			{
				if (itemList[i].ContainsCollisionNode())
				{
					return true;
				}
			}
			return false;
		}

		public void ResetObjects()
		{
			for (int i = 0; i < itemList.Count; i++)
			{
				TC_Layer tC_Layer = itemList[i] as TC_Layer;
				if (tC_Layer != null)
				{
					tC_Layer.ResetObjects();
					continue;
				}
				TC_LayerGroup tC_LayerGroup = itemList[i] as TC_LayerGroup;
				if (tC_LayerGroup != null)
				{
					tC_LayerGroup.ResetObjects();
				}
			}
		}

		public override void GetItems(bool refresh, bool rebuildGlobalLists, bool resetTextures)
		{
			if (resetTextures)
			{
				DisposeTextures();
			}
			active = visible;
			itemList.Clear();
			firstActive = (lastActive = -1);
			totalActive = 0;
			bool flag = true;
			int num = 0;
			for (int num2 = t.childCount - 1; num2 >= 0; num2--)
			{
				Transform child = base.transform.GetChild(num2);
				TC_Layer component = child.GetComponent<TC_Layer>();
				if (component != null)
				{
					component.SetParameters(this, num);
					component.GetItems(refresh, rebuildGlobalLists, resetTextures);
					if (component.active)
					{
						totalActive++;
						lastActive = num;
						if (firstActive == -1)
						{
							firstActive = lastActive;
						}
					}
					itemList.Add(component);
					num++;
					if (flag)
					{
						bounds = component.bounds;
						flag = false;
					}
					else
					{
						bounds.Encapsulate(component.bounds);
					}
				}
				else
				{
					TC_LayerGroup component2 = child.GetComponent<TC_LayerGroup>();
					if (component2 == null)
					{
						TC.MoveToDustbin(child);
					}
					else
					{
						component2.SetParameters(this, num);
						component2.GetItems(refresh, rebuildGlobalLists, resetTextures);
						if (component2.active)
						{
							totalActive++;
							lastActive = num;
							if (firstActive == -1)
							{
								firstActive = lastActive;
							}
						}
						if (component2.groupResult == null)
						{
							TC.MoveToDustbin(child);
						}
						else
						{
							itemList.Add(component2);
							num++;
						}
						if (flag)
						{
							bounds = component2.bounds;
							flag = false;
						}
						else
						{
							bounds.Encapsulate(component2.bounds);
						}
					}
				}
			}
			TC_Reporter.Log(TC.outputNames[outputId] + " Level " + level + " activeTotal " + totalActive);
			if (!active)
			{
				totalActive = 0;
			}
			else if (totalActive == 0)
			{
				active = false;
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
			if (arg[0] == "ResultGroup" || arg[0] == "All")
			{
			}
			if (arg[0] != "ResultGroup")
			{
				if (arg.Length <= 1)
				{
					return -1;
				}
				for (int i = 0; i < itemList.Count; i++)
				{
				}
			}
			return result;
		}
	}
}
