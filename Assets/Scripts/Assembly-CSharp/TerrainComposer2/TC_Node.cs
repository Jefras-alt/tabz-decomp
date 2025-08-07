using System;
using UnityEngine;

namespace TerrainComposer2
{
	public class TC_Node : TC_ItemBehaviour
	{
		[Serializable]
		public class Shapes
		{
			public Vector2 topSize = new Vector2(500f, 500f);

			public Vector2 bottomSize = new Vector2(1000f, 1000f);

			public float size = 500f;
		}

		public InputKind inputKind;

		public InputTerrain inputTerrain;

		public InputNoise inputNoise;

		public InputShape inputShape;

		public InputFile inputFile;

		public InputCurrent inputCurrent;

		public InputPortal inputPortal;

		public NodeGroupType nodeType;

		public CollisionMode collisionMode;

		public CollisionDirection collisionDirection;

		public BlurMode blurMode;

		public int nodeGroupLevel;

		public ImageWrapMode wrapMode = ImageWrapMode.Repeat;

		public bool clamp;

		public float radius = 300f;

		public TC_RawImage rawImage;

		public TC_Image image;

		public ImageSettings imageSettings;

		public bool square;

		public int splatSelectIndex;

		public Noise noise;

		public Shapes shapes;

		public int iterations = 1;

		public Vector2 detectRange = new Vector2(0.003921569f, 0.003921569f);

		public int mipmapLevel = 1;

		public ConvexityMode convexityMode;

		public float convexityStrength = 5f;

		public Texture stampTex;

		public string pathTexStamp = string.Empty;

		public bool isStampInResourcesFolder;

		public string resourcesFolder;

		public float posYOld;

		public int collisionMask = -1;

		public bool heightDetectRange;

		public bool includeTerrainHeight = true;

		public Vector2 range;

		public bool useConstant;

		public Vector3 size = new Vector3(2048f, 0f, 2048f);

		public override void Awake()
		{
			base.Awake();
			if (rawImage != null)
			{
				rawImage.referenceCount++;
			}
			if (image != null)
			{
				image.referenceCount++;
			}
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			if (rawImage != null)
			{
				rawImage.UnregisterReference();
			}
			if (image != null)
			{
				image.UnregisterReference();
			}
		}

		public void SetDefaultSettings()
		{
			size = TC_Settings.instance.global.defaultTerrainSize;
			if (base.transform.parent.GetSiblingIndex() == 0)
			{
				inputKind = InputKind.Shape;
				inputShape = InputShape.Circle;
				wrapMode = ImageWrapMode.Clamp;
			}
			else if (outputId == 0)
			{
				inputKind = InputKind.File;
				inputFile = InputFile.RawImage;
				wrapMode = ImageWrapMode.Clamp;
			}
		}

		public void CheckTransformChange()
		{
			if (ctOld.hasChanged(this))
			{
				TC.AutoGenerate();
				ctOld.Copy(this);
			}
		}

		public override void ChangeYPosition(float y)
		{
			posY += y / t.lossyScale.y;
		}

		public void CalcBounds()
		{
			bounds.center = t.position;
			bounds.size = Vector3.Scale(new Vector3(1000f, 100f, 1000f), t.lossyScale);
		}

		public bool OutOfBounds()
		{
			if (bounds.Intersects(TC_Area2D.current.bounds))
			{
				return false;
			}
			TC_Reporter.Log(base.name + " Out of bounds!");
			return true;
		}

		public Enum GetInputPopup()
		{
			if (inputKind == InputKind.Terrain)
			{
				if (outputId == 0)
				{
					InputTerrainHeight inputTerrainHeight = InputTerrainHeight.Collision;
					return inputTerrainHeight;
				}
				return inputTerrain;
			}
			if (inputKind == InputKind.Noise)
			{
				return inputNoise;
			}
			if (inputKind == InputKind.Shape)
			{
				return inputShape;
			}
			if (inputKind == InputKind.File)
			{
				return inputFile;
			}
			if (inputKind == InputKind.Current)
			{
				return inputCurrent;
			}
			if (inputKind == InputKind.Portal)
			{
				return inputPortal;
			}
			return null;
		}

		public void SetInputPopup(Enum popup)
		{
			if (inputKind == InputKind.Terrain)
			{
				inputTerrain = (InputTerrain)(object)popup;
			}
			else if (inputKind == InputKind.Noise)
			{
				inputNoise = (InputNoise)(object)popup;
			}
			else if (inputKind == InputKind.Shape)
			{
				inputShape = (InputShape)(object)popup;
			}
			else if (inputKind == InputKind.File)
			{
				inputFile = (InputFile)(object)popup;
			}
			else if (inputKind == InputKind.Current)
			{
				inputCurrent = (InputCurrent)(object)popup;
			}
			else if (inputKind == InputKind.Portal)
			{
				inputPortal = (InputPortal)(object)popup;
			}
		}

		public void Init()
		{
			if (inputKind == InputKind.Terrain && inputTerrain == InputTerrain.Normal)
			{
				active = false;
			}
			if (inputKind == InputKind.Noise)
			{
				if (noise == null)
				{
					noise = new Noise();
				}
				if ((inputNoise == InputNoise.IQ || inputNoise == InputNoise.Swiss || inputNoise == InputNoise.Jordan) && noise.mode == NoiseMode.TextureLookup)
				{
					noise.mode = NoiseMode.Normal;
				}
			}
			else if (inputKind == InputKind.Shape)
			{
				if (shapes == null)
				{
					shapes = new Shapes();
				}
			}
			else if (inputKind == InputKind.File)
			{
				if (inputFile == InputFile.RawImage)
				{
					if (rawImage != null && rawImage.tex != null && stampTex != null)
					{
						active = true;
						return;
					}
					if (rawImage != null)
					{
						if (rawImage.tex == null)
						{
							rawImage.LoadRawImage(rawImage.path);
						}
						if (stampTex != null && rawImage.tex != null)
						{
							active = true;
							return;
						}
					}
					if (stampTex == null && pathTexStamp == string.Empty)
					{
						active = false;
						return;
					}
					if (rawImage == null)
					{
						DropTextureEditor(stampTex);
					}
					if (rawImage == null)
					{
						active = false;
						stampTex = null;
					}
					else
					{
						active = true;
					}
				}
				else if (inputFile == InputFile.Image && stampTex == null)
				{
					active = false;
				}
			}
			else if (inputKind == InputKind.Portal)
			{
				active = false;
			}
		}

		public void UpdateVersion()
		{
			if (versionNumber != 0f)
			{
				return;
			}
			wrapMode = ((!clamp) ? ImageWrapMode.Repeat : ImageWrapMode.Clamp);
			size.y = 1024f;
			if (inputKind == InputKind.Terrain)
			{
				if (inputTerrain == InputTerrain.Collision)
				{
					inputTerrain = InputTerrain.Convexity;
				}
				else if (inputTerrain == InputTerrain.Splatmap)
				{
					inputTerrain = InputTerrain.Collision;
				}
				else if (inputTerrain == InputTerrain.Convexity)
				{
					inputTerrain = InputTerrain.Splatmap;
				}
			}
			if (inputKind == InputKind.File && inputFile == InputFile.RawImage)
			{
				t.localScale = new Vector3(t.localScale.x, t.localScale.y, 0f - t.localScale.z);
			}
			if (inputKind == InputKind.Noise)
			{
				if (inputNoise == InputNoise.Billow)
				{
					inputNoise = InputNoise.Ridged;
				}
				else if (inputNoise == InputNoise.Ridged)
				{
					inputNoise = InputNoise.Billow;
				}
				else if (inputNoise == InputNoise.IQ)
				{
					inputNoise = InputNoise.Random;
				}
			}
			SetVersionNumber();
		}

		public bool DropTextureEditor(Texture tex)
		{
			if (isStampInResourcesFolder)
			{
				rawImage = TC_Settings.instance.AddRawFile(resourcesFolder, isStampInResourcesFolder);
			}
			if (rawImage != null)
			{
				stampTex = tex;
				TC.RefreshOutputReferences(outputId, true);
				TC_Reporter.Log("Node index " + rawImage.name);
				return true;
			}
			TC.AddMessage("This is not a stamp preview image.\n\nThe raw heightmap file needs to be placed in a 'RawFiles' folder, then TC2 will automatically make a preview image one folder before it.\nThis image needs to be used for dropping on the node.", 0f, 4f);
			return false;
		}
	}
}
