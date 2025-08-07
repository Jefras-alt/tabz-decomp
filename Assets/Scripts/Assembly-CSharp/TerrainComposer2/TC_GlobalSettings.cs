using UnityEngine;

namespace TerrainComposer2
{
	public class TC_GlobalSettings : ScriptableObject
	{
		public bool tooltip;

		public Vector3 defaultTerrainSize = new Vector3(2048f, 1000f, 2048f);

		public bool SavePreviewTextures = true;

		public Color[] previewColors = new Color[8]
		{
			Color.red,
			Color.blue,
			Color.green,
			Color.yellow,
			Color.magenta,
			Color.cyan,
			Color.white,
			Color.grey
		};

		public Color colLayerGroup;

		public Color colLayer;

		public Color colMaskNodeGroup;

		public Color colMaskNode;

		public Color colSelectNodeGroup;

		public Color colSelectNode;

		public Color colSelectItemGroup;

		public Color colSelectItem;

		public float shelveHeight = 428f;

		public float shelveRightWidth = 18f;

		public float outputVSpace;

		public float groupVSpace = 25f;

		public float layerVSpace = 50f;

		public float layerHSpace = 180f;

		public float nodeHSpace = 5f;

		public float bracketHSpace = 10f;

		public bool showResolutionWarnings = true;

		public bool linkScaleToMaskDefault = true;

		public bool documentationClicked;

		public Rect rect;

		public Rect rect2;

		public Rect rect3;

		public Rect rect4;

		public Rect rect5;

		public Rect rect6;

		public Rect rect7;

		public Rect rect8;

		public KeyCode keyZoomIn = KeyCode.Plus;

		public KeyCode keyZoomOut = KeyCode.Minus;

		public Color GetVisualizeColor(int index)
		{
			return previewColors[(int)Mathf.Repeat(index, previewColors.Length)];
		}
	}
}
