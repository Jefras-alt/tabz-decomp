using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainComposer2
{
	[ExecuteInEditMode]
	public class TC_Compute : MonoBehaviour
	{
		public class BytesArray
		{
			public byte[] bytes;
		}

		public static TC_Compute instance;

		public TC_CamCapture camCapture;

		public Transform target;

		public bool run;

		public ComputeShader shader;

		public string path;

		[NonSerialized]
		public float threads = 1024f;

		public int collisionMask;

		public PerlinNoise m_perlin;

		private int[] methodKernel;

		private int[] methodTexKernel;

		private int[] colorMethodTexKernel;

		private int[] multiMethodTexKernel;

		private int[] noisePerlinKernel;

		private int[] noiseBillowKernel;

		private int[] noiserRidgedKernel;

		private int[] noisePerlin2Kernel;

		private int[] noiseBillow2Kernel;

		private int[] noiseRidged2Kernel;

		private int[] noiseIQKernel;

		private int[] noiseSwissKernel;

		private int[] noiseJordanKernel;

		private int noiseRandomKernel;

		private int noiseCellNormalKernel;

		private int noiseCellFastKernel;

		private int colorMethodMultiplyBufferKernel;

		private int colorMethodTexLerpMaskKernel;

		private int multiMethodMultiplyBufferKernel;

		public int terrainHeightKernel;

		public int terrainAngleKernel;

		public int terrainSplatmap0Kernel;

		public int terrainSplatmap1Kernel;

		public int terrainConvexityKernel;

		public int terrainCollisionHeightKernel;

		public int terrainCollisionHeightIncludeKernel;

		public int terrainCollisionMaskKernel;

		private int methodLerpMaskKernel;

		private int methodTexLerpMaskKernel;

		private int multiMethodTexLerpMaskKernel;

		private int shapeGradientKernel;

		private int shapeCircleKernel;

		private int shapeSquareKernel;

		private int shapeConstantKernel;

		private int rawImageKernel;

		private int imageColorKernel;

		private int imageColorRangeKernel;

		private int currentBlurNormalKernel;

		private int currentBlurOutwardKernel;

		private int currentBlurInwardKernel;

		private int currentExpandKernel;

		private int currentShrinkKernel;

		private int currentEdgeDetectKernel;

		private int currentDistortionKernel;

		private int calcColorKernel;

		private int calcSplatKernel;

		private int normalizeSplatKernel;

		private int calcObjectKernel;

		private int methodItemTexMaskKernel;

		private int methodItemTex0MaskKernel;

		private int calcObjectPositionKernel;

		private int terrainTexKernel;

		private int resultBufferToTexKernel;

		private int copyBufferKernel;

		private int copyRenderTextureKernel;

		private int methodItemTexMaxKernel;

		private int methodItemTexMinKernel;

		private int methodItemTexLerpKernel;

		private int methodItemTexLerpMaskKernel;

		private Vector3 posOld;

		private Vector3 scaleOld;

		private Quaternion rotOld;

		private float bufferLength;

		public RenderTexture[] rtsColor;

		public RenderTexture[] rtsSplatmap;

		public RenderTexture[] rtsResult;

		public RenderTexture rtResult;

		public RenderTexture rtSplatPreview;

		public Texture2D[] texGrassmaps;

		public Vector4[] splatColors;

		public Vector4[] colors;

		[NonSerialized]
		public BytesArray[] bytesArray;

		private void OnEnable()
		{
			instance = this;
			methodKernel = new int[9];
			methodTexKernel = new int[9];
			colorMethodTexKernel = new int[9];
			multiMethodTexKernel = new int[9];
			noisePerlinKernel = new int[12];
			noiseBillowKernel = new int[12];
			noiserRidgedKernel = new int[12];
			noisePerlin2Kernel = new int[3];
			noiseBillow2Kernel = new int[3];
			noiseRidged2Kernel = new int[3];
			noiseIQKernel = new int[3];
			noiseSwissKernel = new int[3];
			noiseJordanKernel = new int[3];
			TC_Reporter.Log("Init compute");
			TC_Reporter.Log(methodKernel.Length + " - " + methodTexKernel.Length);
			for (int i = 0; i < 9; i++)
			{
				Method method = (Method)i;
				string text = method.ToString();
				methodKernel[i] = shader.FindKernel("Method" + text);
				methodTexKernel[i] = shader.FindKernel("MethodTex" + text);
				multiMethodTexKernel[i] = shader.FindKernel("MultiMethodTex" + text);
				colorMethodTexKernel[i] = shader.FindKernel("ColorMethodTex" + text);
			}
			for (int j = 1; j < noisePerlinKernel.Length + 1; j++)
			{
				noisePerlinKernel[j - 1] = shader.FindKernel("NoisePerlin" + j);
			}
			for (int k = 1; k < noiseBillowKernel.Length + 1; k++)
			{
				noiseBillowKernel[k - 1] = shader.FindKernel("NoiseBillow" + k);
			}
			for (int l = 1; l < noiserRidgedKernel.Length + 1; l++)
			{
				noiserRidgedKernel[l - 1] = shader.FindKernel("NoiseMultiFractal" + l);
			}
			for (int m = 0; m < 3; m++)
			{
				noisePerlin2Kernel[m] = shader.FindKernel("NoisePerlin" + Enum.GetName(typeof(NoiseMode), m + 1));
				noiseBillow2Kernel[m] = shader.FindKernel("NoiseBillow" + Enum.GetName(typeof(NoiseMode), m + 1));
				noiseRidged2Kernel[m] = shader.FindKernel("NoiseRidged" + Enum.GetName(typeof(NoiseMode), m + 1));
				noiseIQKernel[m] = shader.FindKernel("NoiseIQ" + Enum.GetName(typeof(NoiseMode), m + 1));
				noiseSwissKernel[m] = shader.FindKernel("NoiseSwiss" + Enum.GetName(typeof(NoiseMode), m + 1));
				noiseJordanKernel[m] = shader.FindKernel("NoiseJordan" + Enum.GetName(typeof(NoiseMode), m + 1));
			}
			noiseCellNormalKernel = shader.FindKernel("NoiseCellNormal");
			noiseCellFastKernel = shader.FindKernel("NoiseCellFast");
			colorMethodTexLerpMaskKernel = shader.FindKernel("ColorMethodTexLerpMask");
			colorMethodMultiplyBufferKernel = shader.FindKernel("ColorMethodMultiplyBuffer");
			multiMethodMultiplyBufferKernel = shader.FindKernel("MultiMethodMultiplyBuffer");
			terrainHeightKernel = shader.FindKernel("TerrainHeight");
			terrainAngleKernel = shader.FindKernel("TerrainAngle");
			terrainConvexityKernel = shader.FindKernel("TerrainConvexity");
			terrainSplatmap0Kernel = shader.FindKernel("TerrainSplatmap0");
			terrainSplatmap1Kernel = shader.FindKernel("TerrainSplatmap1");
			terrainCollisionHeightKernel = shader.FindKernel("TerrainCollisionHeightInverted");
			terrainCollisionHeightIncludeKernel = shader.FindKernel("TerrainCollisionHeightIncludeInverted");
			terrainCollisionMaskKernel = shader.FindKernel("TerrainCollisionMaskInverted");
			noiseRandomKernel = shader.FindKernel("NoiseRandom");
			rawImageKernel = shader.FindKernel("RawImage");
			imageColorKernel = shader.FindKernel("ImageColor");
			imageColorRangeKernel = shader.FindKernel("ImageColorRange");
			shapeGradientKernel = shader.FindKernel("ShapeGradient");
			shapeCircleKernel = shader.FindKernel("ShapeCircle");
			shapeSquareKernel = shader.FindKernel("ShapeSquare");
			shapeConstantKernel = shader.FindKernel("ShapeConstant");
			currentBlurNormalKernel = shader.FindKernel("CurrentBlurNormal");
			currentBlurOutwardKernel = shader.FindKernel("CurrentBlurOutward");
			currentBlurInwardKernel = shader.FindKernel("CurrentBlurInward");
			currentExpandKernel = shader.FindKernel("CurrentExpand");
			currentShrinkKernel = shader.FindKernel("CurrentShrink");
			currentEdgeDetectKernel = shader.FindKernel("CurrentEdgeDetect");
			currentDistortionKernel = shader.FindKernel("CurrentDistortion");
			methodLerpMaskKernel = shader.FindKernel("MethodLerpMask");
			methodTexLerpMaskKernel = shader.FindKernel("MethodTexLerpMask");
			multiMethodTexLerpMaskKernel = shader.FindKernel("MultiMethodTexLerpMask");
			calcColorKernel = shader.FindKernel("CalcColor");
			calcSplatKernel = shader.FindKernel("CalcSplat");
			calcObjectKernel = shader.FindKernel("CalcObject");
			calcObjectPositionKernel = shader.FindKernel("CalcObjectPosition");
			methodItemTexMaskKernel = shader.FindKernel("MethodItemTexMask");
			methodItemTex0MaskKernel = shader.FindKernel("MethodItemTex0Mask");
			normalizeSplatKernel = shader.FindKernel("NormalizeSplat");
			terrainTexKernel = shader.FindKernel("TerrainTex");
			resultBufferToTexKernel = shader.FindKernel("ResultBufferToTex");
			methodItemTexMaxKernel = shader.FindKernel("MethodItemTexMax");
			methodItemTexMinKernel = shader.FindKernel("MethodItemTexMin");
			methodItemTexLerpKernel = shader.FindKernel("MethodItemTexLerp");
			methodItemTexLerpMaskKernel = shader.FindKernel("MethodItemTexLerpMask");
			copyBufferKernel = shader.FindKernel("CopyBuffer");
			copyRenderTextureKernel = shader.FindKernel("CopyRenderTexture");
			if (TC_Settings.instance == null)
			{
				return;
			}
			if (TC_Settings.instance.global == null)
			{
				TC.GetInstallPath();
				if (!TC.LoadGlobalSettings())
				{
					return;
				}
			}
			splatColors = Mathw.ColorsToVector4(TC_Settings.instance.global.previewColors);
			TC_Reporter.Log("LerpKernel " + methodLerpMaskKernel + " - " + methodTexLerpMaskKernel);
			TC_Reporter.Log(string.Concat(rawImageKernel, " - ", noisePerlinKernel, " - ", shapeConstantKernel));
		}

		private void OnDestroy()
		{
			instance = null;
			DisposeTextures();
		}

		public void DisposeTextures()
		{
			DisposeRenderTextures(ref rtsColor);
			DisposeRenderTextures(ref rtsSplatmap);
			DisposeRenderTextures(ref rtsResult);
			DisposeRenderTexture(ref rtResult);
			DisposeRenderTexture(ref rtSplatPreview);
			DisposeTextures(ref texGrassmaps);
			DisposeTexture(ref m_perlin.m_permTable1D);
			DisposeTexture(ref m_perlin.m_permTable2D);
			DisposeTexture(ref m_perlin.m_gradient2D);
			DisposeTexture(ref m_perlin.m_gradient3D);
			DisposeTexture(ref m_perlin.m_gradient4D);
		}

		public void InitCurves(TC_ItemBehaviour item)
		{
			item.localCurve.ConvertCurve();
			item.worldCurve.ConvertCurve();
		}

		public void SetPreviewColors(Vector4[] colors)
		{
		}

		public void RunColorCompute(TC_NodeGroup nodeGroup, TC_SelectItemGroup itemGroup, ref RenderTexture rt, ref ComputeBuffer resultBuffer)
		{
			TC_Area2D current = TC_Area2D.current;
			ComputeBuffer buffer = new ComputeBuffer(itemGroup.colorMixBuffer.Length, 28);
			buffer.SetData(itemGroup.colorMixBuffer);
			int num = calcColorKernel;
			shader.SetInt("itemCount", itemGroup.colorMixBuffer.Length);
			shader.SetBuffer(num, "resultBuffer", resultBuffer);
			shader.SetTexture(num, "splatmap0", rt);
			shader.SetTexture(num, "splatPreviewTex", nodeGroup.rtColorPreview);
			shader.SetBuffer(num, "colorMixBuffer", buffer);
			shader.SetVector("resolutionPM", current.resolutionPM);
			shader.SetVector("resToPreview", current.resToPreview);
			shader.SetInt("resolutionX", current.intResolution.x);
			shader.SetInt("resolutionY", current.intResolution.y);
			Int2 intResolution = current.intResolution;
			bufferLength = intResolution.x * intResolution.y;
			if (num == -1)
			{
				Debug.Log("Kernel not found");
				return;
			}
			shader.Dispatch(num, Mathf.CeilToInt(bufferLength / threads), 1, 1);
			DisposeBuffer(ref resultBuffer);
			DisposeBuffer(ref buffer);
		}

		public void RunSplatCompute(TC_NodeGroup nodeGroup, TC_SelectItemGroup itemGroup, ref RenderTexture[] rts, ref ComputeBuffer resultBuffer)
		{
			TC_Area2D current = TC_Area2D.current;
			ComputeBuffer buffer = new ComputeBuffer(itemGroup.splatMixBuffer.Length, 48);
			buffer.SetData(itemGroup.splatMixBuffer);
			ComputeBuffer buffer2 = new ComputeBuffer(8, 16);
			buffer2.SetData(TC_Settings.instance.global.previewColors);
			int num = calcSplatKernel;
			shader.SetInt("itemCount", itemGroup.splatMixBuffer.Length);
			shader.SetBuffer(num, "resultBuffer", resultBuffer);
			shader.SetTexture(num, "splatmap0", rts[0]);
			if (rts.Length > 1)
			{
				shader.SetTexture(num, "splatmap1", rts[1]);
			}
			shader.SetTexture(num, "splatPreviewTex", nodeGroup.rtColorPreview);
			shader.SetBuffer(num, "splatMixBuffer", buffer);
			shader.SetBuffer(num, "itemColorBuffer", buffer2);
			shader.SetVector("resolutionPM", current.resolutionPM);
			shader.SetVector("resToPreview", current.resToPreview);
			shader.SetInt("resolutionX", current.intResolution.x);
			shader.SetInt("resolutionY", current.intResolution.y);
			Int2 intResolution = current.intResolution;
			bufferLength = intResolution.x * intResolution.y;
			if (num == -1)
			{
				Debug.Log("Kernel not found");
				return;
			}
			shader.Dispatch(num, Mathf.CeilToInt(bufferLength / threads), 1, 1);
			DisposeBuffer(ref resultBuffer);
			DisposeBuffer(ref buffer);
			DisposeBuffer(ref buffer2);
		}

		public void RunItemCompute(TC_Layer layer, ref ComputeBuffer itemMapBuffer, ref ComputeBuffer resultBuffer)
		{
			TC_GlobalSettings global = TC_Settings.instance.global;
			TC_Area2D current = TC_Area2D.current;
			TC_SelectItemGroup selectItemGroup = layer.selectItemGroup;
			TC_NodeGroup selectNodeGroup = layer.selectNodeGroup;
			int num = calcObjectKernel;
			if (selectItemGroup.indices == null)
			{
				selectItemGroup.CreateItemMixBuffer();
			}
			ComputeBuffer buffer = new ComputeBuffer(selectItemGroup.indices.Length, 20);
			buffer.SetData(selectItemGroup.indices);
			ComputeBuffer buffer2 = new ComputeBuffer(8, 16);
			buffer2.SetData(global.previewColors);
			int num2 = current.intResolution.x * current.intResolution.y;
			itemMapBuffer = new ComputeBuffer(num2, 24);
			shader.SetBuffer(num, "itemIndexBuffer", buffer);
			shader.SetBuffer(num, "itemColorBuffer", buffer2);
			shader.SetBuffer(num, "resultBuffer", resultBuffer);
			shader.SetBuffer(num, "itemMapBuffer", itemMapBuffer);
			shader.SetTexture(num, "splatPreviewTex", selectNodeGroup.rtColorPreview);
			shader.SetTexture(num, "previewTex", layer.rtPreview);
			shader.SetVector("colLayer", global.GetVisualizeColor(layer.listIndex));
			shader.SetInt("itemCount", selectItemGroup.indices.Length);
			shader.SetInt("resolutionX", current.intResolution.x);
			shader.SetInt("resolutionY", current.intResolution.y);
			shader.SetFloat("mixValue", selectItemGroup.mix);
			shader.SetVector("resolutionPM", current.resolutionPM);
			shader.SetVector("resToPreview", current.resToPreview);
			shader.SetVector("areaPos", current.area.position);
			shader.SetVector("outputOffsetV2", current.outputOffsetV2);
			shader.SetVector("totalAreaPos", current.totalArea.position);
			if (num == -1)
			{
				Debug.Log("Kernel not found");
				return;
			}
			shader.Dispatch(num, Mathf.CeilToInt((float)num2 / threads), 1, 1);
			DisposeBuffer(ref buffer);
			DisposeBuffer(ref buffer2);
		}

		public void RunItemPositionCompute(ComputeBuffer itemMapBuffer, int outputId)
		{
			TC_Area2D current = TC_Area2D.current;
			int num = calcObjectPositionKernel;
			int num2 = current.intResolution.x * current.intResolution.y;
			List<TC_SelectItem> list = ((outputId != 3) ? current.terrainLayer.objectSelectItems : current.terrainLayer.treeSelectItems);
			float[] array = new float[list.Count];
			if (outputId == 3)
			{
				for (int i = 0; i < list.Count; i++)
				{
					array[i] = list[i].tree.randomPosition / 2f;
				}
			}
			else
			{
				for (int j = 0; j < list.Count; j++)
				{
					array[j] = list[j].spawnObject.randomPosition / 2f;
				}
			}
			ComputeBuffer buffer = new ComputeBuffer(list.Count, 4);
			buffer.SetData(array);
			shader.SetBuffer(num, "resultBuffer", buffer);
			if (current.currentTCTerrain.texHeight != null)
			{
				shader.SetTexture(num, "terrainTexRead", current.currentTCTerrain.texHeight);
				shader.SetFloat("terrainTexReadResolution", current.currentTCTerrain.texHeight.width);
				shader.SetFloat("terrainTexReadNormalResolution", current.currentTCTerrain.texHeight.width - current.resExpandBorder * 2);
				shader.SetFloat("resExpandBorder", current.resExpandBorder);
			}
			shader.SetBuffer(num, "itemMapBuffer", itemMapBuffer);
			shader.SetInt("resolutionX", current.intResolution.x);
			shader.SetInt("resolutionY", current.intResolution.y);
			shader.SetVector("posOffset", Vector3.zero);
			shader.SetVector("texResolution", new Vector2(TC_Settings.instance.global.defaultTerrainSize.x, TC_Settings.instance.global.defaultTerrainSize.z));
			shader.SetVector("resolutionPM", current.resolutionPM);
			shader.SetVector("resToPreview", current.resToPreview);
			shader.SetVector("areaPos", current.area.position);
			shader.SetVector("outputOffsetV2", current.outputOffsetV2);
			shader.SetVector("totalAreaPos", current.totalArea.position);
			shader.SetVector("snapOffset", current.snapOffsetUV);
			shader.SetFloat("terrainHeight", current.terrainSize.y);
			if (num == -1)
			{
				Debug.Log("Kernel not found");
				return;
			}
			shader.Dispatch(num, Mathf.CeilToInt((float)num2 / threads), 1, 1);
			DisposeBuffer(ref buffer);
		}

		public void RunComputeCopyRenderTexture(RenderTexture rtSource, RenderTexture rtDest)
		{
			int num = copyRenderTextureKernel;
			Int2 @int = new Int2(rtSource.width, rtSource.height);
			shader.SetTexture(num, "splatmap1", rtDest);
			shader.SetTexture(num, "rightSplatmap1", rtSource);
			if (num == -1)
			{
				Debug.Log("Kernel not found");
			}
			else
			{
				shader.Dispatch(num, @int.x / 8, @int.y / 8, 1);
			}
		}

		public void RunItemComputeMask(TC_ItemBehaviour item, ref RenderTexture rtPreview, RenderTexture rtColorPreview, ref ComputeBuffer itemMapBuffer, ref ComputeBuffer maskBuffer)
		{
			TC_Area2D current = TC_Area2D.current;
			int num = ((item.level != 0) ? methodItemTexMaskKernel : methodItemTex0MaskKernel);
			int num2 = current.intResolution.x * current.intResolution.y;
			shader.SetVector("areaPos", current.area.position - current.outputOffsetV2);
			shader.SetInt("resolutionX", current.intResolution.x);
			shader.SetInt("resolutionY", current.intResolution.y);
			shader.SetVector("colLayer", TC_Settings.instance.global.GetVisualizeColor(item.listIndex));
			shader.SetVector("resolutionPM", current.resolutionPM);
			shader.SetVector("resToPreview", current.resToPreview);
			shader.SetVector("areaPos", current.area.position);
			shader.SetVector("outputOffsetV2", current.outputOffsetV2);
			shader.SetVector("totalAreaPos", current.totalArea.position);
			shader.SetTexture(num, "leftPreviewTex", rtColorPreview);
			shader.SetTexture(num, "splatPreviewTex", rtPreview);
			shader.SetBuffer(num, "itemMapBuffer", itemMapBuffer);
			shader.SetBuffer(num, "maskBuffer", maskBuffer);
			if (num == -1)
			{
				Debug.Log("Kernel not found RunItemComputeMask");
				return;
			}
			shader.Dispatch(num, Mathf.CeilToInt((float)num2 / threads), 1, 1);
			DisposeBuffer(ref maskBuffer);
		}

		public ComputeBuffer RunNodeCompute(TC_GroupBehaviour groupItem, TC_Node node, float seedParent, ComputeBuffer rightBuffer = null, bool disposeRightBuffer = false)
		{
			float num = threads;
			TC_Area2D current = TC_Area2D.current;
			TC_Settings tC_Settings = TC_Settings.instance;
			node.ct.CopySpecial(node);
			Vector2 resolution = current.resolution;
			InitCurves(node);
			if (groupItem != null)
			{
				groupItem.localCurve.ConvertCurve();
			}
			int num2 = 0;
			if (node.useConstant)
			{
				num2 = shapeConstantKernel;
			}
			else if (node.inputKind == InputKind.Terrain)
			{
				if (node.inputTerrain == InputTerrain.Height)
				{
					num2 = terrainHeightKernel;
				}
				else if (node.inputTerrain == InputTerrain.Angle)
				{
					num2 = terrainAngleKernel;
				}
				else if (node.inputTerrain == InputTerrain.Convexity && current.currentTCTerrain.texHeight != null)
				{
					num2 = terrainConvexityKernel;
					float num3 = Mathf.Log(current.currentTCTerrain.texHeight.width, 2f) - 6f;
					int val = Mathf.Clamp(node.mipmapLevel + (int)num3, 1, 8);
					shader.SetInt("itemCount", val);
					shader.SetFloat("overlay", (node.convexityMode != ConvexityMode.Convex) ? (0f - node.convexityStrength) : node.convexityStrength);
				}
				else if (node.inputTerrain == InputTerrain.Splatmap)
				{
					if (node.outputId == 1)
					{
						TC.AddMessage("Splat Input settings is currently not available in Splat Output. The node will be set inactive");
						node.enabled = false;
						return null;
					}
					Texture[] alphamapTextures = current.currentTerrain.terrainData.alphamapTextures;
					if (node.splatSelectIndex < 4)
					{
						num2 = terrainSplatmap0Kernel;
						shader.SetTexture(num2, "leftSplatmap0", alphamapTextures[0]);
						shader.SetInt("splatIndex", node.splatSelectIndex);
					}
					else
					{
						num2 = terrainSplatmap1Kernel;
						shader.SetTexture(num2, "leftSplatmap1", alphamapTextures[1]);
						shader.SetInt("splatIndex", node.splatSelectIndex - 4);
					}
				}
				else if (node.inputTerrain == InputTerrain.Collision)
				{
					if (camCapture.collisionMask != node.collisionMask || !camCapture.terrain != (bool)current.currentTerrain)
					{
						camCapture.Capture(node.collisionMask, node.collisionDirection, node.outputId);
					}
					num2 = ((node.collisionMode != CollisionMode.Height) ? terrainCollisionMaskKernel : ((!node.includeTerrainHeight || !node.heightDetectRange) ? terrainCollisionHeightKernel : terrainCollisionHeightIncludeKernel));
					shader.SetTexture(num2, "tex1", camCapture.cam.targetTexture);
					if (rightBuffer != null)
					{
						shader.SetBuffer(num2, "rightBuffer", rightBuffer);
					}
					if (node.heightDetectRange)
					{
						shader.SetVector("range", node.range / current.terrainSize.y);
					}
					else
					{
						shader.SetVector("range", new Vector2(0f, current.terrainSize.y));
					}
				}
				if (node.inputTerrain != InputTerrain.Splatmap && current.currentTCTerrain.texHeight != null)
				{
					shader.SetTexture(num2, "terrainTexRead", current.currentTCTerrain.texHeight);
					shader.SetFloat("terrainTexReadResolution", current.currentTCTerrain.texHeight.width);
					shader.SetFloat("terrainTexReadNormalResolution", current.currentTCTerrain.texHeight.width - current.resExpandBorder * 2);
					shader.SetFloat("resExpandBorder", current.resExpandBorder);
				}
			}
			else if (node.inputKind == InputKind.Noise || (node.inputKind == InputKind.Current && node.inputCurrent == InputCurrent.Distortion))
			{
				if (node.inputNoise == InputNoise.Perlin)
				{
					num2 = ((node.noise.mode != NoiseMode.TextureLookup) ? noisePerlin2Kernel[(int)(node.noise.mode - 1)] : noisePerlinKernel[node.noise.octaves - 1]);
				}
				else if (node.inputNoise == InputNoise.Billow)
				{
					num2 = ((node.noise.mode != NoiseMode.TextureLookup) ? noiseBillow2Kernel[(int)(node.noise.mode - 1)] : noiseBillowKernel[node.noise.octaves - 1]);
				}
				else if (node.inputNoise == InputNoise.Ridged)
				{
					num2 = ((node.noise.mode != NoiseMode.TextureLookup) ? noiseRidged2Kernel[(int)(node.noise.mode - 1)] : noiserRidgedKernel[node.noise.octaves - 1]);
				}
				else if (node.inputNoise == InputNoise.IQ)
				{
					num2 = noiseIQKernel[(int)(node.noise.mode - 1)];
					num = 512f;
				}
				else if (node.inputNoise == InputNoise.Swiss)
				{
					num2 = noiseSwissKernel[(int)(node.noise.mode - 1)];
					num = 512f;
				}
				else if (node.inputNoise == InputNoise.Jordan)
				{
					num2 = noiseJordanKernel[(int)(node.noise.mode - 1)];
					num = 512f;
				}
				else if (node.inputNoise == InputNoise.Cell)
				{
					num2 = ((node.noise.cellMode != CellNoiseMode.Normal) ? noiseCellFastKernel : noiseCellNormalKernel);
					shader.SetInt("_CellType", node.noise.cellType);
					shader.SetInt("_DistanceFunction", node.noise.distanceFunction);
				}
				else if (node.inputNoise == InputNoise.Random)
				{
					num2 = noiseRandomKernel;
				}
				if (m_perlin.GetPermutationTable2D() == null)
				{
					m_perlin = new PerlinNoise(0);
					m_perlin.LoadResourcesFor3DNoise();
				}
				shader.SetTexture(num2, "_PermTable2D", m_perlin.GetPermutationTable2D());
				shader.SetTexture(num2, "_Gradient3D", m_perlin.GetGradient3D());
				shader.SetFloat("_Frequency", node.noise.frequency / 10000f);
				shader.SetFloat("_Lacunarity", node.noise.lacunarity);
				shader.SetFloat("_Persistence", node.noise.persistence);
				shader.SetFloat("_Seed", node.noise.seed + seedParent + TC_Settings.instance.seed);
				if (node.noise.mode != NoiseMode.TextureLookup)
				{
					shader.SetFloat("_Amplitude", node.noise.amplitude);
					shader.SetInt("_Octaves", node.noise.octaves);
					shader.SetFloat("_Warp0", node.noise.warp0);
					shader.SetFloat("_Warp", node.noise.warp);
					shader.SetFloat("_Damp0", node.noise.damp0);
					shader.SetFloat("_Damp", node.noise.damp);
					shader.SetFloat("_DampScale", node.noise.dampScale);
				}
			}
			else if (node.inputKind == InputKind.Shape)
			{
				if (node.inputShape == InputShape.Gradient)
				{
					num2 = shapeGradientKernel;
				}
				else if (node.inputShape == InputShape.Circle)
				{
					num2 = shapeCircleKernel;
				}
				else if (node.inputShape == InputShape.Rectangle)
				{
					num2 = shapeSquareKernel;
					shader.SetVector("topResolution", node.shapes.topSize);
					shader.SetVector("bottomResolution", node.shapes.bottomSize);
				}
				else if (node.inputShape == InputShape.Constant)
				{
					num2 = shapeConstantKernel;
				}
				shader.SetFloat("shapeSize", node.shapes.size);
			}
			else if (node.inputKind == InputKind.File)
			{
				if (node.inputFile == InputFile.Image)
				{
					num2 = ((node.imageSettings.colSelectMode != ColorSelectMode.Color) ? imageColorRangeKernel : imageColorKernel);
					if (node.stampTex != null)
					{
						shader.SetTexture(num2, "leftSplatmap0", node.stampTex);
					}
					for (int i = 0; i < 4; i++)
					{
						ImageSettings.ColChannel colChannel = node.imageSettings.colChannels[i];
						shader.SetVector(TC.colChannelNamesLowerCase[i] + "Channel", new Vector3(colChannel.active ? 1 : 0, colChannel.range.x / 255f, colChannel.range.y / 255f));
					}
				}
				else if (node.inputFile == InputFile.RawImage)
				{
					if (node.rawImage == null)
					{
						return null;
					}
					if (node.rawImage.tex == null)
					{
						node.rawImage.LoadRawImage(node.rawImage.path);
						if (node.rawImage.tex == null)
						{
							return null;
						}
					}
					num2 = rawImageKernel;
					shader.SetInt("_Octaves", node.mipmapLevel);
					shader.SetTexture(num2, "tex1", node.rawImage.tex);
				}
			}
			else if (node.inputKind == InputKind.Portal)
			{
				num2 = copyBufferKernel;
				shader.SetBuffer(num2, "rightBuffer", current.layerGroupBuffer);
			}
			if (node.inputKind == InputKind.Current)
			{
				if (node.inputCurrent == InputCurrent.Blur)
				{
					if (node.blurMode == BlurMode.Normal)
					{
						num2 = currentBlurNormalKernel;
					}
					else if (node.blurMode == BlurMode.Outward)
					{
						num2 = currentBlurOutwardKernel;
					}
					else if (node.blurMode == BlurMode.Inward)
					{
						num2 = currentBlurInwardKernel;
					}
				}
				else if (node.inputCurrent == InputCurrent.Expand)
				{
					num2 = currentExpandKernel;
				}
				else if (node.inputCurrent == InputCurrent.Shrink)
				{
					num2 = currentShrinkKernel;
				}
				else if (node.inputCurrent == InputCurrent.EdgeDetect)
				{
					num2 = currentEdgeDetectKernel;
					shader.SetVector("range", node.detectRange);
				}
				else if (node.inputCurrent == InputCurrent.Distortion)
				{
					num2 = currentDistortionKernel;
					shader.SetFloat("shapeSize", node.radius);
				}
				shader.SetBuffer(num2, "rightBuffer", rightBuffer);
			}
			shader.SetVector("texResolution", new Vector2(node.size.x, node.size.z));
			shader.SetInt("isClamp", (node.wrapMode == ImageWrapMode.Clamp) ? 1 : 0);
			shader.SetInt("isMirror", (node.wrapMode == ImageWrapMode.Mirror) ? 1 : 0);
			shader.SetInt("preview", tC_Settings.preview ? 1 : 0);
			InitPreviewRenderTexture(ref node.rtPreview, node.name);
			shader.SetInt("previewResolution", node.rtPreview.width);
			shader.SetTexture(num2, "previewTex", node.rtPreview);
			InitPreviewRenderTexture(ref groupItem.rtPreview, "Preview");
			shader.SetTexture(num2, "previewTex2", groupItem.rtPreview);
			bufferLength = (int)resolution.x * (int)resolution.y;
			TC_Reporter.Log("Compute node buffer resolution " + resolution.x + " " + resolution.y);
			ComputeBuffer computeBuffer = new ComputeBuffer((int)bufferLength, 4);
			shader.SetBuffer(num2, "resultBuffer", computeBuffer);
			shader.SetInt("resolutionX", (int)resolution.x);
			shader.SetInt("resolutionY", (int)resolution.y);
			ComputeBuffer curveCalc = null;
			ComputeBuffer curveKeys = null;
			SetComputeCurve("local", num2, node.localCurve, ref curveCalc, ref curveKeys);
			ComputeBuffer curveCalc2 = null;
			ComputeBuffer curveKeys2 = null;
			if (groupItem != null)
			{
				SetComputeCurve("localGroup", num2, groupItem.localCurve, ref curveCalc2, ref curveKeys2);
			}
			ComputeBuffer curveCalc3 = null;
			ComputeBuffer curveKeys3 = null;
			SetComputeCurve("world", num2, node.worldCurve, ref curveCalc3, ref curveKeys3);
			if (node.nodeType == NodeGroupType.Mask)
			{
				shader.SetInt("mask", 1);
			}
			else
			{
				shader.SetInt("mask", 0);
			}
			shader.SetVector("resolutionPM", current.resolutionPM);
			shader.SetVector("resToPreview", current.resToPreview);
			shader.SetVector("terrainSize", new Vector2(current.terrainSize.x, current.terrainSize.z));
			shader.SetVector("offset", node.ct.position - current.startPos);
			shader.SetVector("posOffset", node.ct.posOffset);
			shader.SetVector("areaPos", current.area.position);
			shader.SetVector("totalAreaPos", current.totalArea.position);
			shader.SetVector("rot", new Vector4(node.ct.rotation.x, node.ct.rotation.y, node.ct.rotation.z, node.ct.rotation.w));
			if (node.outputId == 3 || node.outputId == 5)
			{
				shader.SetVector("uvOffset", new Vector2(0.5f / resolution.x, 0.5f / resolution.y));
			}
			else
			{
				shader.SetVector("uvOffset", Vector2.zero);
			}
			float num4 = node.ct.scale.y;
			if (((node.inputKind == InputKind.Noise && node.inputNoise == InputNoise.Swiss) || node.inputNoise == InputNoise.Jordan) && node.noise.amplitude > 1f)
			{
				num4 /= node.noise.amplitude;
			}
			shader.SetVector("scale", new Vector3(node.ct.scale.x, num4 * (node.size.y / 1000f), node.ct.scale.z));
			shader.SetFloat("terrainHeight", current.terrainSize.y);
			shader.SetInt("outputId", node.outputId);
			if (num2 == -1)
			{
				Debug.Log("Kernel not found");
				return null;
			}
			shader.Dispatch(num2, Mathf.CeilToInt(bufferLength / num), 1, 1);
			DisposeBuffers(ref curveKeys, ref curveCalc);
			DisposeBuffers(ref curveKeys2, ref curveCalc2);
			DisposeBuffers(ref curveKeys3, ref curveCalc3);
			if (disposeRightBuffer)
			{
				DisposeBuffer(ref rightBuffer);
			}
			return computeBuffer;
		}

		public void RunComputeMultiMethod(TC_ItemBehaviour item, Method method, bool normalize, ref RenderTexture[] rtsLeft, ref RenderTexture[] rtsRight, ComputeBuffer maskBuffer, RenderTexture rtPreview, ref RenderTexture rtPreviewClone, ref RenderTexture rtLeftPreview, RenderTexture rtRightPreview)
		{
			TC_Area2D current = TC_Area2D.current;
			int num = -1;
			if (method == Method.Lerp && maskBuffer != null)
			{
				num = multiMethodTexLerpMaskKernel;
				TC_Reporter.Log(num + " -> Lerp mask");
				shader.SetTexture(num, "previewTex2", item.rtPreview);
				shader.SetBuffer(num, "maskBuffer", maskBuffer);
			}
			else
			{
				num = multiMethodTexKernel[(int)method];
			}
			shader.SetFloat("overlay", item.opacity);
			shader.SetInt("doNormalize", normalize ? 1 : 0);
			Int2 intResolution = current.intResolution;
			shader.SetInt("resolutionX", intResolution.x);
			shader.SetInt("resolutionY", intResolution.y);
			shader.SetTexture(num, "leftSplatmap0", rtsLeft[0]);
			if (rtsLeft.Length > 1)
			{
				shader.SetTexture(num, "leftSplatmap1", rtsLeft[1]);
			}
			shader.SetTexture(num, "rightSplatmap0", rtsRight[0]);
			if (rtsRight.Length > 1)
			{
				shader.SetTexture(num, "rightSplatmap1", rtsRight[1]);
			}
			shader.SetTexture(num, "splatmap0", rtsResult[0]);
			shader.SetTexture(num, "splatmap1", rtsResult[1]);
			InitPreviewRenderTexture(ref rtPreviewClone, "Preview");
			shader.SetTexture(num, "leftPreviewTex", rtLeftPreview);
			shader.SetTexture(num, "rightPreviewTex", rtRightPreview);
			shader.SetTexture(num, "splatPreviewTex", rtPreview);
			shader.SetTexture(num, "splatPreviewTexClone", rtPreviewClone);
			if (num == -1)
			{
				Debug.Log("Kernel not found");
				return;
			}
			shader.Dispatch(num, intResolution.x / 8, intResolution.y / 8, 1);
			rtLeftPreview = rtPreviewClone;
			TC.Swap(ref rtsLeft, ref rtsResult);
			DisposeRenderTextures(ref rtsRight);
		}

		public void RunComputeMultiMethod(TC_ItemBehaviour item, bool doNormalize, ref RenderTexture[] rtsLeft, ComputeBuffer maskBuffer, RenderTexture rtLeftPreview = null)
		{
			TC_Area2D current = TC_Area2D.current;
			int num = -1;
			num = multiMethodMultiplyBufferKernel;
			shader.SetBuffer(num, "rightBuffer", maskBuffer);
			shader.SetInt("doNormalize", doNormalize ? 1 : 0);
			Int2 intResolution = current.intResolution;
			shader.SetInt("resolutionX", intResolution.x);
			shader.SetInt("resolutionY", intResolution.y);
			shader.SetTexture(num, "leftSplatmap0", rtsLeft[0]);
			if (rtsLeft.Length > 1)
			{
				shader.SetTexture(num, "leftSplatmap1", rtsLeft[1]);
			}
			shader.SetTexture(num, "splatmap0", rtsResult[0]);
			if (rtsResult.Length > 1)
			{
				shader.SetTexture(num, "splatmap1", rtsResult[1]);
			}
			TC_Layer tC_Layer = item as TC_Layer;
			if (tC_Layer != null)
			{
				shader.SetTexture(num, "leftPreviewTex", tC_Layer.selectNodeGroup.rtColorPreview);
			}
			else
			{
				TC_LayerGroup tC_LayerGroup = item as TC_LayerGroup;
				if (tC_LayerGroup != null)
				{
					shader.SetTexture(num, "leftPreviewTex", rtLeftPreview);
				}
			}
			shader.SetTexture(num, "splatPreviewTex", item.rtPreview);
			TC_Reporter.Log("maskbuffer " + intResolution.x + " , " + intResolution.y);
			if (num == -1)
			{
				Debug.Log("Kernel not found");
				return;
			}
			shader.Dispatch(num, intResolution.x / 8, intResolution.y / 8, 1);
			TC.Swap(ref rtsLeft, ref rtsResult);
		}

		public void RunComputeColorMethod(TC_ItemBehaviour item, Method method, ref RenderTexture rtLeft, ref RenderTexture rtRight, ComputeBuffer maskBuffer, RenderTexture rtPreview, ref RenderTexture rtPreviewClone, ref RenderTexture rtLeftPreview, RenderTexture rtRightPreview)
		{
			TC_Area2D current = TC_Area2D.current;
			int num = -1;
			if (method == Method.Lerp && maskBuffer != null)
			{
				num = colorMethodTexLerpMaskKernel;
				TC_Reporter.Log(num + " -> Lerp mask");
				shader.SetTexture(num, "previewTex2", item.rtPreview);
				shader.SetBuffer(num, "maskBuffer", maskBuffer);
			}
			else
			{
				num = colorMethodTexKernel[(int)method];
			}
			shader.SetFloat("overlay", item.opacity);
			Int2 intResolution = current.intResolution;
			shader.SetInt("resolutionX", intResolution.x);
			shader.SetInt("resolutionY", intResolution.y);
			shader.SetTexture(num, "leftSplatmap0", rtLeft);
			shader.SetTexture(num, "rightSplatmap0", rtRight);
			shader.SetTexture(num, "splatmap0", rtResult);
			InitPreviewRenderTexture(ref rtPreviewClone, "Preview");
			shader.SetTexture(num, "leftPreviewTex", rtLeftPreview);
			shader.SetTexture(num, "rightPreviewTex", rtRightPreview);
			shader.SetTexture(num, "splatPreviewTex", rtPreview);
			shader.SetTexture(num, "splatPreviewTexClone", rtPreviewClone);
			if (num == -1)
			{
				Debug.Log("Kernel not found");
				return;
			}
			shader.Dispatch(num, intResolution.x / 8, intResolution.y / 8, 1);
			TC.Swap(ref rtLeft, ref rtResult);
			rtLeftPreview = rtPreviewClone;
		}

		public void RunComputeColorMethod(TC_ItemBehaviour item, ref RenderTexture rtLeft, ComputeBuffer maskBuffer, RenderTexture rtLeftPreview = null)
		{
			TC_Area2D current = TC_Area2D.current;
			int num = -1;
			num = colorMethodMultiplyBufferKernel;
			shader.SetBuffer(num, "rightBuffer", maskBuffer);
			Int2 intResolution = current.intResolution;
			shader.SetInt("resolutionX", intResolution.x);
			shader.SetInt("resolutionY", intResolution.y);
			shader.SetTexture(num, "leftSplatmap0", rtLeft);
			shader.SetTexture(num, "splatmap0", rtResult);
			TC_Layer tC_Layer = item as TC_Layer;
			if (tC_Layer != null)
			{
				shader.SetTexture(num, "leftPreviewTex", tC_Layer.selectNodeGroup.rtColorPreview);
			}
			else
			{
				TC_LayerGroup tC_LayerGroup = item as TC_LayerGroup;
				if (tC_LayerGroup != null)
				{
					shader.SetTexture(num, "leftPreviewTex", rtLeftPreview);
				}
			}
			shader.SetTexture(num, "splatPreviewTex", item.rtPreview);
			TC_Reporter.Log("maskbuffer " + intResolution.x + " , " + intResolution.y);
			if (num == -1)
			{
				Debug.Log("Kernel not found");
				return;
			}
			shader.Dispatch(num, intResolution.x / 8, intResolution.y / 8, 1);
			TC.Swap(ref rtLeft, ref rtResult);
		}

		public void RunSplatNormalize(TC_LayerGroup layerGroup, ref RenderTexture[] rtsLeft, ref RenderTexture rtPreview)
		{
			if (TC_Settings.instance.preview || layerGroup.active)
			{
				int kernelIndex = normalizeSplatKernel;
				shader.SetTexture(kernelIndex, "leftSplatmap0", rtsLeft[0]);
				shader.SetTexture(kernelIndex, "leftSplatmap1", rtsLeft[1]);
				shader.SetTexture(kernelIndex, "splatmap0", rtsResult[0]);
				shader.SetTexture(kernelIndex, "splatmap1", rtsResult[1]);
				if (rtPreview != null)
				{
					shader.SetTexture(kernelIndex, "leftPreviewTex", rtPreview);
					shader.SetTexture(kernelIndex, "splatPreviewTex", rtSplatPreview);
				}
				TC.Swap(ref rtsLeft, ref rtsResult);
				TC.Swap(ref rtPreview, ref rtSplatPreview);
			}
		}

		public void RunComputeMethod(TC_GroupBehaviour groupItem, TC_ItemBehaviour item, ComputeBuffer resultBuffer, ref ComputeBuffer rightBuffer, int itemCount, RenderTexture rtPreview, ComputeBuffer maskBuffer = null)
		{
			if (!TC_Settings.instance.preview && !item.active)
			{
				return;
			}
			int num = -1;
			int num2;
			if (groupItem != null)
			{
				num2 = (int)item.method;
			}
			else
			{
				num2 = 3;
				shader.SetInt("localCurveKeysLength", 0);
				shader.SetInt("worldCurveKeysLength", 0);
			}
			if (rtPreview != null && TC_Settings.instance.preview)
			{
				if (maskBuffer == null)
				{
					num = methodTexKernel[num2];
				}
				else
				{
					num = methodTexLerpMaskKernel;
					if (item.rtPreview != null)
					{
						shader.SetTexture(num, "previewTex2", item.rtPreview);
					}
					shader.SetBuffer(num, "maskBuffer", maskBuffer);
				}
				shader.SetTexture(num, "previewTex", rtPreview);
			}
			else if (maskBuffer == null)
			{
				num = methodKernel[num2];
			}
			else
			{
				num = methodLerpMaskKernel;
				shader.SetBuffer(num, "maskBuffer", maskBuffer);
			}
			if (item != null)
			{
				shader.SetFloat("overlay", item.opacity);
			}
			shader.SetInt("itemCount", itemCount);
			shader.SetBuffer(num, "rightBuffer", rightBuffer);
			shader.SetBuffer(num, "resultBuffer", resultBuffer);
			if (num == -1)
			{
				Debug.Log("Kernel not found");
				return;
			}
			if (groupItem != null)
			{
				ComputeBuffer curveCalc = null;
				ComputeBuffer curveKeys = null;
				groupItem.worldCurve.ConvertCurve();
				SetComputeCurve("world", num, groupItem.worldCurve, ref curveCalc, ref curveKeys);
				shader.Dispatch(num, Mathf.CeilToInt(bufferLength / threads), 1, 1);
				DisposeBuffers(ref curveKeys, ref curveCalc);
			}
			else
			{
				shader.Dispatch(num, Mathf.CeilToInt(bufferLength / threads), 1, 1);
			}
			DisposeBuffer(ref rightBuffer);
		}

		public void RunComputeObjectMethod(TC_GroupBehaviour groupItem, TC_ItemBehaviour item, ComputeBuffer itemMapBuffer, ref ComputeBuffer rightItemMapBuffer, ComputeBuffer maskBuffer, RenderTexture rtPreview, ref RenderTexture rtPreviewClone, ref RenderTexture rtLeftPreview, RenderTexture rtRightPreview)
		{
			TC_Area2D current = TC_Area2D.current;
			TC_Settings tC_Settings = TC_Settings.instance;
			int num = -1;
			if (item.method == Method.Max)
			{
				num = methodItemTexMaxKernel;
			}
			else if (item.method == Method.Min)
			{
				num = methodItemTexMinKernel;
			}
			else if (item.method == Method.Lerp)
			{
				if (maskBuffer == null)
				{
					num = methodItemTexLerpKernel;
				}
				else
				{
					num = methodItemTexLerpMaskKernel;
					shader.SetBuffer(num, "maskBuffer", maskBuffer);
				}
			}
			if (rtPreview == null)
			{
				TC_Reporter.Log("rtPreview = null");
			}
			if (rtPreview != null && tC_Settings.preview)
			{
				InitPreviewRenderTexture(ref rtPreviewClone, "rtPreviewClone_" + TC.outputNames[groupItem.outputId]);
				if (maskBuffer != null)
				{
					shader.SetTexture(num, "previewTex2", item.rtPreview);
					shader.SetBuffer(num, "maskBuffer", maskBuffer);
					shader.SetVector("colLayer", tC_Settings.global.GetVisualizeColor(item.listIndex));
				}
				if (groupItem.level != 0)
				{
					InitPreviewRenderTexture(ref groupItem.parentItem.rtPreview, "rtPreview LayerGroup" + TC.outputNames[groupItem.outputId]);
					shader.SetTexture(num, "splatmap0", groupItem.parentItem.rtPreview);
					shader.SetVector("colLayer2", tC_Settings.global.GetVisualizeColor(groupItem.parentItem.listIndex));
				}
				if (rtLeftPreview != null)
				{
					shader.SetTexture(num, "leftPreviewTex", rtLeftPreview);
				}
				shader.SetTexture(num, "rightPreviewTex", rtRightPreview);
				shader.SetTexture(num, "splatPreviewTex", rtPreview);
				shader.SetTexture(num, "splatPreviewTexClone", rtPreviewClone);
			}
			else if (maskBuffer != null)
			{
				shader.SetBuffer(num, "maskBuffer", maskBuffer);
			}
			if (item != null)
			{
				shader.SetFloat("overlay", item.opacity);
			}
			shader.SetBuffer(num, "itemMapBuffer", itemMapBuffer);
			shader.SetBuffer(num, "rightItemMapBuffer", rightItemMapBuffer);
			shader.SetVector("resolutionPM", current.resolutionPM);
			Int2 intResolution = current.intResolution;
			shader.SetInt("resolutionX", intResolution.x);
			shader.SetInt("resolutionY", intResolution.y);
			shader.SetVector("areaPos", current.area.position);
			shader.SetVector("totalAreaPos", current.totalArea.position);
			shader.SetVector("resToPreview", current.resToPreview);
			if (groupItem != null)
			{
				if (num == -1)
				{
					Debug.Log("Kernel not found RunComputeObjectMethod");
					return;
				}
				shader.Dispatch(num, Mathf.CeilToInt(bufferLength / threads), 1, 1);
			}
			rtLeftPreview = rtPreviewClone;
			DisposeBuffer(ref rightItemMapBuffer);
		}

		public void RunTerrainTexFromTerrainData(TerrainData terrainData, ref RenderTexture rtHeight)
		{
			Debug.Log("Run terrain tex from TerrainData " + terrainData.name);
			int num = terrainData.heightmapResolution - 1;
			float[,] heights = terrainData.GetHeights(0, 0, num, num);
			float[] array = new float[num * num];
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num; j++)
				{
					array[j + i * num] = heights[i, j];
				}
			}
			ComputeBuffer buffer = new ComputeBuffer(array.Length, 4);
			buffer.SetData(array);
			RunTerrainTex(buffer, ref rtHeight, num);
			DisposeBuffer(ref buffer);
		}

		public void RunTerrainTex(ComputeBuffer resultBuffer, ref RenderTexture rtHeight, int resolution, bool useRTP = false)
		{
			TC_Area2D current = TC_Area2D.current;
			TC_Reporter.Log("Run terrain tex");
			InitRenderTexture(ref rtResult, "rtResult", resolution, RenderTextureFormat.RFloat);
			InitRenderTexture(ref rtHeight, "rtHeight " + current.currentTCUnityTerrain.terrain.name, new Int2(resolution, resolution), RenderTextureFormat.ARGB32, false, true);
			shader.SetBuffer(resultBufferToTexKernel, "resultBuffer", resultBuffer);
			shader.SetTexture(resultBufferToTexKernel, "resultTex", rtResult);
			TC_Reporter.Log("result Kernel " + resultBufferToTexKernel);
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (resultBufferToTexKernel == -1)
			{
				Debug.Log("Kernel not found");
				return;
			}
			TC_Reporter.Log("Area resolution " + current.resolution);
			int num = resultBufferToTexKernel;
			if (num == -1)
			{
				Debug.Log("Kernel not found");
				return;
			}
			shader.SetInt("resolutionX", resolution);
			shader.SetInt("resolutionY", resolution);
			shader.Dispatch(num, Mathf.CeilToInt((float)resolution / 8f), Mathf.CeilToInt((float)resolution / 8f), 1);
			float num2 = 1f / (Time.realtimeSinceStartup - realtimeSinceStartup);
			TC_Reporter.Log("Frames compute " + num2);
			num = terrainTexKernel;
			shader.SetTexture(terrainTexKernel, "terrainTex", rtHeight);
			shader.SetTexture(terrainTexKernel, "resultTexRead", rtResult);
			Vector3 size = current.currentTCUnityTerrain.terrain.terrainData.size;
			Vector2 vector = new Vector2(size.x / (float)resolution, size.z / (float)resolution);
			shader.SetVector("resolutionPM", vector);
			if (num == -1)
			{
				Debug.Log("Kernel not found");
			}
			else
			{
				shader.Dispatch(num, Mathf.CeilToInt((float)resolution / 8f), Mathf.CeilToInt((float)resolution / 8f), 1);
			}
		}

		private void SetComputeCurve(string name, int kernel, Curve curve, ref ComputeBuffer curveCalc, ref ComputeBuffer curveKeys)
		{
			if (curve.length > 0)
			{
				curveCalc = new ComputeBuffer(curve.c.Length, 16);
				curveKeys = new ComputeBuffer(curve.curveKeys.Length, 4);
				curveCalc.SetData(curve.c);
				curveKeys.SetData(curve.curveKeys);
				shader.SetBuffer(kernel, name + "CurveKeys", curveKeys);
				shader.SetBuffer(kernel, name + "CurveCalc", curveCalc);
			}
			shader.SetInt(name + "CurveKeysLength", curve.length);
			shader.SetVector(name + "CurveRange", new Vector3(curve.range.x, curve.range.y, curve.range.y - curve.range.x));
		}

		public static void InitTextures(ref Texture2D[] textures, string name, int length = 1)
		{
			TC_Area2D current = TC_Area2D.current;
			TC_Reporter.Log("InitTextures", 1);
			if (textures == null)
			{
				textures = new Texture2D[length];
			}
			else if (textures.Length != 2)
			{
				DisposeTextures(ref textures);
				textures = new Texture2D[length];
			}
			for (int i = 0; i < textures.Length; i++)
			{
				if (textures[i] != null)
				{
					TC_Reporter.Log(textures[i].name + " is assigned");
					if (textures[i].width != current.intResolution.x || textures[i].height != current.intResolution.y)
					{
						textures[i].Resize(current.intResolution.x, current.intResolution.y);
					}
				}
				else
				{
					textures[i] = new Texture2D(current.intResolution.x, current.intResolution.y, TextureFormat.ARGB32, false, true);
					textures[i].hideFlags = HideFlags.DontSave;
					textures[i].name = name;
				}
			}
		}

		public static void InitTexture(ref Texture2D tex, string name, int resolution = -1, bool mipmap = false, TextureFormat format = TextureFormat.ARGB32)
		{
			TC_Area2D current = TC_Area2D.current;
			TC_Reporter.Log("InitTextures", 1);
			Int2 @int = ((resolution != -1) ? new Int2(resolution, resolution) : current.intResolution);
			if (tex != null)
			{
				TC_Reporter.Log(tex.name + " is assigned");
				if (tex.format == format)
				{
					if (tex.mipmapCount != 1 || !mipmap)
					{
						if (tex.width != @int.x || tex.height != @int.y)
						{
							tex.Resize(@int.x, @int.y);
						}
						return;
					}
				}
				else
				{
					DisposeTexture(ref tex);
				}
			}
			TC_Reporter.Log("Create new Texture2D " + name);
			tex = new Texture2D(@int.x, @int.y, format, mipmap, true);
			tex.hideFlags = HideFlags.DontSave;
			tex.name = name;
		}

		public static void InitPreviewRenderTexture(ref RenderTexture rt, string name)
		{
			TC_Area2D current = TC_Area2D.current;
			TC_Reporter.Log("InitPreviewRenderTextures", 1);
			if (!(current == null))
			{
				int previewResolution = current.previewResolution;
				if (rt != null && rt.width != previewResolution)
				{
					TC_Reporter.Log("release " + rt.width + " " + previewResolution);
					DisposeRenderTexture(ref rt);
				}
				if (rt == null)
				{
					rt = new RenderTexture(previewResolution, previewResolution, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
					rt.name = name;
					rt.enableRandomWrite = true;
					rt.hideFlags = HideFlags.DontSave;
					rt.Create();
				}
			}
		}

		public static void InitRenderTextures(ref RenderTexture[] rts, string name, int length = 2)
		{
			TC_Area2D current = TC_Area2D.current;
			TC_Reporter.Log("InitRenderTextures");
			if (rts == null)
			{
				rts = new RenderTexture[length];
			}
			else if (rts.Length != length)
			{
				DisposeRenderTextures(ref rts);
				rts = new RenderTexture[length];
			}
			for (int i = 0; i < rts.Length; i++)
			{
				if (rts[i] != null)
				{
					TC_Reporter.Log(rts[i].name + " is assigned");
					if (rts[i].width == current.intResolution.x && rts[i].height == current.intResolution.y)
					{
						continue;
					}
					DisposeRenderTexture(ref rts[i]);
				}
				rts[i] = new RenderTexture(current.intResolution.x, current.intResolution.y, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
				rts[i].enableRandomWrite = true;
				rts[i].name = name;
				rts[i].hideFlags = HideFlags.DontSave;
				rts[i].Create();
			}
		}

		public static void InitRenderTexture(ref RenderTexture rt, string name)
		{
			TC_Area2D current = TC_Area2D.current;
			TC_Reporter.Log("InitRenderTextures");
			if (rt != null)
			{
				if (!rt.IsCreated())
				{
					Debug.Log("RenderTexture not Created!");
				}
				TC_Reporter.Log(rt.name + " is assigned");
				if (rt.width == current.intResolution.x && rt.height == current.intResolution.y)
				{
					return;
				}
				TC_Reporter.Log("release " + name + " from " + rt.width + " x " + rt.height + " to " + current.intResolution.x + " x " + current.intResolution.y);
				DisposeRenderTexture(ref rt);
				rt = null;
			}
			rt = new RenderTexture(current.intResolution.x, current.intResolution.y, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			rt.enableRandomWrite = true;
			rt.name = name;
			rt.hideFlags = HideFlags.DontSave;
			rt.Create();
		}

		public static void InitRenderTexture(ref RenderTexture rt, string name, int resolution, RenderTextureFormat format = RenderTextureFormat.ARGB32, bool forceCreate = false)
		{
			TC_Reporter.Log("InitRenderTextures");
			bool flag = forceCreate;
			if (rt == null)
			{
				flag = true;
			}
			else
			{
				if (!rt.IsCreated())
				{
					Debug.Log("RenderTexture not Created!");
				}
				if (rt.width == resolution)
				{
					return;
				}
				TC_Reporter.Log("release " + rt.width + " " + resolution);
				DisposeRenderTexture(ref rt);
				flag = true;
			}
			if (flag)
			{
				rt = new RenderTexture(resolution, resolution, 0, format, RenderTextureReadWrite.Linear);
				rt.name = name;
				rt.hideFlags = HideFlags.DontSave;
				rt.enableRandomWrite = true;
				rt.Create();
			}
		}

		public static void InitRenderTexture(ref RenderTexture rt, string name, Int2 resolution, RenderTextureFormat format = RenderTextureFormat.ARGB32, bool forceCreate = false, bool useMipmap = false)
		{
			TC_Reporter.Log("InitRenderTextures", 1);
			bool flag = forceCreate;
			if (rt == null)
			{
				flag = true;
			}
			else
			{
				if (!rt.IsCreated())
				{
					Debug.Log("RenderTexture not Created!");
				}
				if (rt.width != resolution.x || rt.height != resolution.y || rt.useMipMap != useMipmap)
				{
					TC_Reporter.Log("release " + rt.width + " " + resolution.x);
					DisposeRenderTexture(ref rt);
					flag = true;
				}
			}
			if (flag)
			{
				rt = new RenderTexture(resolution.x, resolution.y, 0, format, RenderTextureReadWrite.Linear);
				rt.name = name;
				rt.useMipMap = useMipmap;
				rt.autoGenerateMips = useMipmap;
				rt.hideFlags = HideFlags.DontSave;
				rt.enableRandomWrite = true;
				rt.Create();
			}
		}

		public void DisposeBuffer(ref ComputeBuffer buffer, bool warningEmpty = false)
		{
			if (buffer == null)
			{
				if (warningEmpty)
				{
					TC_Reporter.Log("Dispose buffer is empty");
				}
			}
			else
			{
				buffer.Dispose();
				buffer = null;
			}
		}

		public void DisposeBuffers(ref ComputeBuffer buffer1, ref ComputeBuffer buffer2)
		{
			if (buffer1 != null)
			{
				buffer1.Dispose();
				buffer1 = null;
			}
			if (buffer2 != null)
			{
				buffer2.Dispose();
				buffer2 = null;
			}
		}

		public static void DisposeRenderTexture(ref RenderTexture rt)
		{
			if (!(rt == null))
			{
				TC_Reporter.Log("DisposeRenderTextures", 1);
				rt.Release();
				rt = null;
			}
		}

		public static void DisposeRenderTextures(ref RenderTexture[] rts)
		{
			if (rts == null)
			{
				return;
			}
			TC_Reporter.Log("DisposeRenderTextures");
			for (int i = 0; i < rts.Length; i++)
			{
				if (!(rts[i] == null))
				{
					rts[i].Release();
					UnityEngine.Object.Destroy(rts[i]);
					rts[i] = null;
				}
			}
		}

		public static void DisposeTexture(ref Texture2D tex)
		{
			if (!(tex == null))
			{
				UnityEngine.Object.Destroy(tex);
			}
		}

		public static void DisposeTextures(ref Texture2D[] textures)
		{
			for (int i = 0; i < textures.Length; i++)
			{
				DisposeTexture(ref textures[i]);
			}
		}

		public void InitBytesArray(int length)
		{
			TC_Reporter.Log("InitByteArray");
			bool flag = false;
			if (bytesArray == null)
			{
				flag = true;
			}
			else if (bytesArray.Length != length)
			{
				flag = true;
			}
			else
			{
				for (int i = 0; i < bytesArray.Length; i++)
				{
					if (bytesArray[i] == null)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				bytesArray = new BytesArray[length];
				for (int j = 0; j < length; j++)
				{
					bytesArray[j] = new BytesArray();
				}
			}
		}
	}
}
