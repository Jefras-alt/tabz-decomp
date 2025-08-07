using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainComposer2
{
	public class TC_SelectItemGroup : TC_GroupBehaviour
	{
		public List<TC_SelectItem> itemList = new List<TC_SelectItem>();

		[NonSerialized]
		public TC_SelectItem refreshRangeItem;

		public bool refreshRanges;

		public SplatCustom[] splatMixBuffer;

		public ColorItem[] colorMixBuffer;

		public ItemSettings[] indices;

		public Transform endT;

		public Vector2 scaleMinMaxMulti = Vector2.one;

		public float scaleMulti = 1f;

		public float mix;

		public float scale = 1f;

		public bool linkScaleToMask = true;

		public bool untouched = true;

		public int placed;

		public override void Awake()
		{
			if (!firstLoad)
			{
				t = base.transform;
				GetItems(true, true, false);
				if ((bool)TC_Settings.instance)
				{
					linkScaleToMask = TC_Settings.instance.global.linkScaleToMaskDefault;
				}
			}
			base.Awake();
			t.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
		}

		public override void OnEnable()
		{
			base.OnEnable();
			t.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
		}

		public override void OnDestroy()
		{
			if (preview.tex != null)
			{
				UnityEngine.Object.Destroy(preview.tex);
			}
			base.OnDestroy();
		}

		public override void CloneSetup()
		{
			base.CloneSetup();
			if (!(TC_Settings.instance == null))
			{
				TC_Settings.instance.HasMasterTerrain();
				preview.tex = null;
				GetItems(true, true, false);
			}
		}

		public override void OnTransformChildrenChanged()
		{
			refreshRanges = true;
			base.OnTransformChildrenChanged();
		}

		public void ResetPlaced()
		{
			for (int i = 0; i < itemList.Count; i++)
			{
				itemList[i].placed = 0;
			}
		}

		public int CalcPlaced()
		{
			placed = 0;
			for (int i = 0; i < itemList.Count; i++)
			{
				placed += itemList[i].placed;
			}
			return placed;
		}

		public void SetReadWriteTextureItems()
		{
			for (int i = 0; i < itemList.Count; i++)
			{
				TC.SetTextureReadWrite(itemList[i].preview.tex);
			}
		}

		public void ResetObjects()
		{
			for (int i = 0; i < itemList.Count; i++)
			{
				itemList[i].ResetObjects();
			}
		}

		public void CalcPreview(bool calcValues = true)
		{
			if (!TC_Settings.instance.hasMasterTerrain)
			{
				return;
			}
			if (itemList.Count > 1)
			{
				int num = TC_Settings.instance.masterTerrain.terrainData.splatPrototypes.Length;
				if (outputId == 1)
				{
					Texture2D[] array = new Texture2D[num];
					for (int i = 0; i < num; i++)
					{
						array[i] = TC_Settings.instance.masterTerrain.terrainData.splatPrototypes[i].texture;
						TC.SetTextureReadWrite(array[i]);
					}
					CalcPreview(array);
				}
				else
				{
					if (outputId == 4)
					{
						for (int j = 0; j < itemList.Count; j++)
						{
							TC.SetTextureReadWrite(itemList[j].preview.tex);
						}
					}
					CalcPreview(null);
				}
			}
			CreateMixBuffer();
			TC.AutoGenerate();
		}

		private void CalcPreview(Texture2D[] texArray)
		{
			preview.Init(128);
			float num = preview.tex.width;
			Color[] previewColors = TC_Settings.instance.global.previewColors;
			for (float num2 = 0f; num2 < num; num2 += 1f)
			{
				float num3 = num2 / num;
				for (float num4 = 0f; num4 < num; num4 += 1f)
				{
					Color color = Color.black;
					float num5 = 0f;
					float num6 = num4 / num;
					if (num6 > 0.9f)
					{
						color = Color.white * num3;
					}
					for (int i = 0; i < itemList.Count; i++)
					{
						TC_SelectItem tC_SelectItem = itemList[i];
						if (!tC_SelectItem.active)
						{
							continue;
						}
						float num7 = EvaluateItem(tC_SelectItem, num3);
						if (num3 < tC_SelectItem.range.y + 0.004f && num3 > tC_SelectItem.range.y - 0.004f)
						{
							color = Color.red;
							num5 = 1f;
							break;
						}
						if (num6 > 0.8f && num6 <= 0.9f && outputId != 2)
						{
							float num8 = (num6 - 0.8f) * 50f;
							if (tC_SelectItem.splatCustom)
							{
								for (int j = 0; j < texArray.Length; j++)
								{
									color += tC_SelectItem.splatCustomValues[j] / tC_SelectItem.splatCustomTotal * previewColors[j] * num7 * num8;
								}
							}
							else
							{
								color += tC_SelectItem.color * num7 * num8;
							}
							num5 += num7 * num8;
						}
						if ((!(num7 > 0f) || !(num6 <= 0.9f)) && outputId != 2)
						{
							continue;
						}
						if (tC_SelectItem.splatCustom)
						{
							for (int k = 0; k < texArray.Length; k++)
							{
								color += tC_SelectItem.splatCustomValues[k] / tC_SelectItem.splatCustomTotal * texArray[k].GetPixel(Mathf.RoundToInt(num6 * (float)tC_SelectItem.preview.tex.width), Mathf.RoundToInt(num3 * (float)tC_SelectItem.preview.tex.height)) * num7;
							}
						}
						else if (outputId != 2 && tC_SelectItem.preview.tex != null)
						{
							color += tC_SelectItem.preview.tex.GetPixel(Mathf.RoundToInt(num6 * (float)tC_SelectItem.preview.tex.width), Mathf.RoundToInt(num3 * (float)tC_SelectItem.preview.tex.height)) * num7 * Mathf.Lerp(1f, 0f, (num6 - 0.8f) * 10f);
						}
						else
						{
							color += tC_SelectItem.color * num7 * Mathf.Lerp(1f, 0f, (num6 - 0.8f) * 10f);
						}
						num5 += num7 * Mathf.Lerp(1f, 0f, (num6 - 0.8f) * 10f);
					}
					if (num6 <= 0.9f)
					{
						color /= num5;
					}
					preview.SetPixelColor(Mathf.RoundToInt(num6 * num), Mathf.RoundToInt(num3 * num), color);
				}
			}
			preview.UploadTexture();
		}

		public override void GetItems(bool refresh, bool rebuildGlobalLists, bool resetTextures)
		{
			if (resetTextures)
			{
				DisposeTextures();
			}
			itemList.Clear();
			totalActive = 0;
			int num = 0;
			for (int num2 = t.childCount - 1; num2 >= 0; num2--)
			{
				Transform child = t.GetChild(num2);
				TC_SelectItem component = child.GetComponent<TC_SelectItem>();
				if (component == null)
				{
					TC.MoveToDustbin(child);
				}
				else
				{
					component.SetParameters(this, num);
					component.parentItem = this;
					component.active = component.visible;
					if (outputId == 1 && component.splatCustom)
					{
						if (TC_Settings.instance.hasMasterTerrain && component.splatCustomValues.Length != TC_Settings.instance.masterTerrain.terrainData.splatPrototypes.Length)
						{
							component.splatCustomValues = Mathw.ResizeArray(component.splatCustomValues, TC_Settings.instance.masterTerrain.terrainData.splatPrototypes.Length);
						}
						component.CalcSplatCustomTotal();
					}
					component.SetPreviewItemTexture();
					if (component.active)
					{
						totalActive++;
					}
					if (outputId == 3)
					{
						if (component.tree == null)
						{
							component.tree = new TC_SelectItem.Tree();
						}
						if (component.active)
						{
							bool flag = true;
							List<TC_SelectItem> treeSelectItems = TC_Area2D.current.terrainLayer.treeSelectItems;
							if (!rebuildGlobalLists)
							{
								int num3 = treeSelectItems.IndexOf(component);
								if (num3 != -1)
								{
									flag = false;
									component.globalListIndex = num3;
								}
							}
							if (flag)
							{
								treeSelectItems.Add(component);
								component.globalListIndex = treeSelectItems.Count - 1;
							}
						}
					}
					else if (outputId == 5)
					{
						if (component.spawnObject == null)
						{
							component.spawnObject = new TC_SelectItem.SpawnObject();
						}
						if (component.spawnObject.go == null || (component.spawnObject.parentMode == TC_SelectItem.SpawnObject.ParentMode.Existing && component.spawnObject.parentT == null) || (component.spawnObject.parentMode == TC_SelectItem.SpawnObject.ParentMode.Create && component.spawnObject.parentName == string.Empty))
						{
							component.active = false;
							TC_Area2D.current.terrainLayer.objectSelectItems.Remove(component);
						}
						if (component.active)
						{
							bool flag2 = true;
							TC_Area2D tC_Area2D = ((!(TC_Area2D.current == null)) ? TC_Area2D.current : UnityEngine.Object.FindObjectOfType<TC_Area2D>());
							List<TC_SelectItem> objectSelectItems = tC_Area2D.terrainLayer.objectSelectItems;
							if (!rebuildGlobalLists)
							{
								int num4 = objectSelectItems.IndexOf(component);
								if (num4 != -1)
								{
									flag2 = false;
									component.globalListIndex = num4;
								}
							}
							if (flag2)
							{
								objectSelectItems.Add(component);
								component.globalListIndex = objectSelectItems.Count - 1;
							}
							component.selectIndex = num;
						}
					}
					component.SetPreviewItemTexture();
					component.SetPreviewColor();
					itemList.Add(component);
					num++;
				}
			}
			if (refreshRangeItem != null || refreshRanges)
			{
				refreshRanges = false;
				RefreshRanges();
				refreshRangeItem = null;
			}
			else if (refresh || TC.refreshPreviewImages)
			{
				CalcPreview();
			}
		}

		public void RefreshRanges()
		{
			if (itemList.Count <= 1)
			{
				untouched = true;
			}
			if (untouched)
			{
				ResetRanges();
			}
			else
			{
				SetRanges(refreshRangeItem);
			}
		}

		public void CreateMixBuffer()
		{
			if (outputId == 1 || outputId == 4)
			{
				CreateSplatMixBuffer();
			}
			else if (outputId == 2)
			{
				CreateColorMixBuffer();
			}
			else
			{
				CreateItemMixBuffer();
			}
		}

		public void CreateItemMixBuffer()
		{
			if (indices == null)
			{
				indices = new ItemSettings[totalActive];
			}
			else if (indices.Length != itemList.Count)
			{
				indices = new ItemSettings[totalActive];
			}
			int num = 0;
			for (int i = 0; i < itemList.Count; i++)
			{
				TC_SelectItem tC_SelectItem = itemList[i];
				if (tC_SelectItem.active)
				{
					indices[num++] = new ItemSettings(tC_SelectItem.globalListIndex, (outputId != 3) ? tC_SelectItem.spawnObject.randomPosition : tC_SelectItem.tree.randomPosition, tC_SelectItem.range, tC_SelectItem.opacity * opacity);
				}
			}
		}

		public void CreateColorMixBuffer()
		{
			if (colorMixBuffer == null)
			{
				colorMixBuffer = new ColorItem[totalActive];
			}
			if (colorMixBuffer.Length != totalActive)
			{
				colorMixBuffer = new ColorItem[totalActive];
			}
			float num = (mix + 0.001f) / (float)totalActive;
			float z = 1f / num;
			int num2 = 0;
			for (int i = 0; i < itemList.Count; i++)
			{
				TC_SelectItem tC_SelectItem = itemList[i];
				if (tC_SelectItem.active)
				{
					colorMixBuffer[num2++] = new ColorItem(new Vector3(tC_SelectItem.range.x - num, tC_SelectItem.range.y, z), tC_SelectItem.color);
				}
			}
		}

		public void CreateSplatMixBuffer()
		{
			if (splatMixBuffer == null)
			{
				splatMixBuffer = new SplatCustom[totalActive];
			}
			if (splatMixBuffer.Length != totalActive)
			{
				splatMixBuffer = new SplatCustom[totalActive];
			}
			float num = (mix + 0.001f) / (float)totalActive;
			float z = 1f / num;
			int num2 = 0;
			for (int i = 0; i < itemList.Count; i++)
			{
				TC_SelectItem tC_SelectItem = itemList[i];
				if (tC_SelectItem.active)
				{
					Vector4 map;
					Vector4 map2;
					if (tC_SelectItem.splatCustom)
					{
						float splatCustomTotal = tC_SelectItem.splatCustomTotal;
						float[] splatCustomValues = tC_SelectItem.splatCustomValues;
						map = new Vector4(splatCustomValues[0] / splatCustomTotal, splatCustomValues[1] / splatCustomTotal, splatCustomValues[2] / splatCustomTotal, splatCustomValues[3] / splatCustomTotal);
						map2 = new Vector4(splatCustomValues[4] / splatCustomTotal, splatCustomValues[5] / splatCustomTotal, splatCustomValues[6] / splatCustomTotal, splatCustomValues[7] / splatCustomTotal);
					}
					else
					{
						map = Vector4.zero;
						map2 = Vector4.zero;
					}
					splatMixBuffer[num2++] = new SplatCustom(new Vector4(tC_SelectItem.range.x - num, tC_SelectItem.range.y, z, (!tC_SelectItem.splatCustom) ? tC_SelectItem.selectIndex : (-tC_SelectItem.selectIndex)), map, map2);
				}
			}
		}

		public float EvaluateItem(TC_SelectItem selectItem, float time)
		{
			float num = (mix + 0.001f) / (float)totalActive;
			float x = selectItem.range.x;
			float y = selectItem.range.y;
			x += num / 2f;
			y -= num / 2f;
			float num2 = x - num;
			float num3 = 1f / num;
			if (time < y)
			{
				return Mathf.Lerp(0f, 1f, Mathf.Clamp01(time - num2) * num3);
			}
			return Mathf.Lerp(1f, 0f, Mathf.Clamp01(time - y) * num3);
		}

		public void SetRanges(TC_SelectItem changedSelectItem = null, bool resetInActive = false)
		{
			if (itemList.Count == 0)
			{
				untouched = true;
				return;
			}
			if (changedSelectItem == null)
			{
				changedSelectItem = itemList[0];
			}
			int num = (changedSelectItem.active ? changedSelectItem.listIndex : 0);
			float y = itemList[num].range.x;
			float x = itemList[num].range.y;
			if (!resetInActive)
			{
				untouched = false;
			}
			TC_SelectItem tC_SelectItem;
			for (int i = num + 1; i < itemList.Count; i++)
			{
				tC_SelectItem = itemList[i];
				if (tC_SelectItem.active)
				{
					tC_SelectItem.range.x = x;
					x = (tC_SelectItem.range.y = Mathf.Max(tC_SelectItem.range.x, tC_SelectItem.range.y));
				}
			}
			for (int num2 = num - 1; num2 >= 0; num2--)
			{
				tC_SelectItem = itemList[num2];
				if (tC_SelectItem.active)
				{
					tC_SelectItem.range.y = y;
					y = (tC_SelectItem.range.x = Mathf.Min(tC_SelectItem.range.x, tC_SelectItem.range.y));
				}
			}
			tC_SelectItem = GetActiveItemUp(0);
			if (tC_SelectItem != null)
			{
				tC_SelectItem.range.x = 0f;
			}
			tC_SelectItem = GetActiveItemDown(itemList.Count - 1);
			if (tC_SelectItem != null)
			{
				tC_SelectItem.range.y = 1f;
			}
			if (resetInActive)
			{
				for (int j = 0; j < itemList.Count; j++)
				{
					tC_SelectItem = itemList[j];
					if (!tC_SelectItem.active)
					{
						TC_SelectItem activeItemDown = GetActiveItemDown(j - 1);
						if (activeItemDown != null)
						{
							tC_SelectItem.range.y = (tC_SelectItem.range.x = activeItemDown.range.y);
						}
						else
						{
							tC_SelectItem.range.y = 0f;
						}
						activeItemDown = GetActiveItemUp(j + 1);
						if (activeItemDown != null)
						{
							tC_SelectItem.range.x = (tC_SelectItem.range.y = activeItemDown.range.x);
						}
						else
						{
							tC_SelectItem.range.x = 1f;
						}
					}
				}
			}
			CalcPreview();
		}

		private TC_SelectItem GetActiveItemUp(int index)
		{
			for (int i = index; i < itemList.Count; i++)
			{
				TC_SelectItem tC_SelectItem = itemList[i];
				if (tC_SelectItem.active)
				{
					return tC_SelectItem;
				}
			}
			return null;
		}

		private TC_SelectItem GetActiveItemDown(int index)
		{
			for (int num = index; num >= 0; num--)
			{
				TC_SelectItem tC_SelectItem = itemList[num];
				if (tC_SelectItem.active)
				{
					return tC_SelectItem;
				}
			}
			return null;
		}

		public Vector2 GetInbetweenRange(int index)
		{
			TC_SelectItem activeItemUp = GetActiveItemUp(index);
			Vector2 result = default(Vector2);
			if (activeItemUp != null)
			{
				result.x = activeItemUp.range.y;
			}
			else
			{
				result.x = 0f;
			}
			activeItemUp = GetActiveItemDown(index);
			if (activeItemUp != null)
			{
				result.y = activeItemUp.range.x;
			}
			else
			{
				result.y = 1f;
			}
			return result;
		}

		public void ResetRanges()
		{
			if (itemList.Count == 0)
			{
				return;
			}
			float num = 1f / (float)totalActive;
			float num2 = 0f;
			for (int i = 0; i < itemList.Count; i++)
			{
				TC_SelectItem tC_SelectItem = itemList[i];
				if (tC_SelectItem.active)
				{
					tC_SelectItem.range = new Vector2(num2, num2 + num);
					num2 += num;
				}
			}
			SetRanges(itemList[0], true);
		}

		public void CenterRange(TC_SelectItem changedSelectItem)
		{
			if (!changedSelectItem.active)
			{
				return;
			}
			float num = 1f / (float)totalActive;
			float num2 = 0f;
			int index = 0;
			for (int i = 0; i < itemList.Count; i++)
			{
				TC_SelectItem tC_SelectItem = itemList[i];
				if (tC_SelectItem == changedSelectItem)
				{
					tC_SelectItem.range = new Vector2(num2, num2 + num);
					index = i;
					break;
				}
				if (tC_SelectItem.active)
				{
					num2 += num;
				}
			}
			SetRanges(itemList[index], true);
		}

		public Vector2[] GetRanges()
		{
			Vector2[] array = new Vector2[itemList.Count];
			for (int i = 0; i < itemList.Count; i++)
			{
				array[i] = itemList[i].range;
			}
			return array;
		}

		public void SetRanges(Vector2[] ranges)
		{
			for (int i = 0; i < itemList.Count; i++)
			{
				if (i < ranges.Length)
				{
					itemList[i].range = ranges[i];
				}
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
	}
}
