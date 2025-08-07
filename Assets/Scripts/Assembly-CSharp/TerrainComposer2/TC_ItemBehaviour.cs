using System;
using UnityEngine;

namespace TerrainComposer2
{
	[ExecuteInEditMode]
	public class TC_ItemBehaviour : MonoBehaviour
	{
		public delegate void RepaintAction();

		[NonSerialized]
		public TC_ItemBehaviour parentItem;

		public float versionNumber;

		public bool defaultPreset;

		public bool isLocked;

		public bool autoGenerate = true;

		public bool visible = true;

		public bool active = true;

		public int foldout = 2;

		public bool nodeFoldout = true;

		public int outputId;

		public int terrainLevel;

		public int level;

		public string notes;

		public int listIndex;

		public bool firstLoad;

		public TexturePreview preview = new TexturePreview();

		public RenderTexture rtDisplay;

		public RenderTexture rtPreview;

		public Method method;

		public float opacity = 1f;

		public bool abs;

		public Curve localCurve = new Curve();

		public Curve worldCurve = new Curve();

		public Transform t;

		public Transform parentOld;

		public int siblingIndexOld = -1;

		public CachedTransform ct = new CachedTransform();

		public CachedTransform ctOld = new CachedTransform();

		public Bounds bounds;

		public bool lockTransform;

		public bool lockPosParent;

		public bool lockPosChildren;

		public bool lockPosX = true;

		public bool lockPosY = true;

		public bool lockPosZ = true;

		public bool lockRotY = true;

		public bool lockScaleX = true;

		public bool lockScaleY = true;

		public bool lockScaleZ = true;

		public PositionMode positionMode;

		public float posY;

		public Vector3 posOffset;

		[NonSerialized]
		public DropPosition dropPosition;

		public bool controlDown;

		[SerializeField]
		private int instanceID;

		public static event RepaintAction DoRepaint;

		public void SetVersionNumber()
		{
			versionNumber = TC.GetVersionNumber();
		}

		public void Repaint()
		{
			if (TC_ItemBehaviour.DoRepaint != null)
			{
				TC_ItemBehaviour.DoRepaint();
			}
		}

		public void InitPreviewRenderTexture(bool assignRtDisplay = true, string name = "Preview")
		{
			TC_Compute.InitPreviewRenderTexture(ref rtPreview, name);
			if (assignRtDisplay)
			{
				rtDisplay = rtPreview;
			}
		}

		public virtual void Awake()
		{
			if (!firstLoad)
			{
				firstLoad = true;
			}
			rtDisplay = null;
			rtPreview = null;
			t = base.transform;
			t.hasChanged = false;
			DetectClone();
		}

		public void Lock(bool active)
		{
			if (active)
			{
				t.hideFlags = HideFlags.NotEditable;
				base.hideFlags = HideFlags.NotEditable;
			}
			else
			{
				t.hideFlags = HideFlags.None;
				base.hideFlags = HideFlags.None;
			}
		}

		private void DetectClone()
		{
			if (instanceID == 0)
			{
				instanceID = GetInstanceID();
			}
			else if (instanceID != GetInstanceID() && GetInstanceID() < 0)
			{
				instanceID = GetInstanceID();
				CloneSetup();
			}
		}

		public virtual void CloneSetup()
		{
			RemoveCloneText();
		}

		public virtual void OnEnable()
		{
			if (TC_Settings.instance == null)
			{
				TC_Settings.GetInstance();
			}
		}

		public void OnDisable()
		{
		}

		public virtual void OnDestroy()
		{
			DisposeTextures();
		}

		public virtual void DisposeTextures()
		{
			rtDisplay = null;
			TC_Compute.DisposeRenderTexture(ref rtPreview);
		}

		private void RemoveCloneText()
		{
			string text = t.name;
			int num = text.IndexOf("(");
			if (num != -1)
			{
				t.name = text.Remove(num);
				if (TC_Settings.instance.selectionOld != null)
				{
					num = TC_Settings.instance.selectionOld.GetSiblingIndex();
					t.SetSiblingIndex(num);
				}
			}
		}

		public virtual void OnTransformChildrenChanged()
		{
			TC.RefreshOutputReferences(outputId, true);
		}

		public void SetLockPositionXZ(bool active)
		{
			if (active)
			{
				ctOld.Copy(this);
			}
		}

		public virtual void ChangeYPosition(float y)
		{
		}

		public virtual void SetLockChildrenPosition(bool lockPos)
		{
		}

		public virtual void UpdateTransforms()
		{
		}

		public void ResetPosition()
		{
			t.localPosition = Vector3.zero;
		}

		public void ResetPositionCompensateChildren()
		{
			GameObject gameObject = new GameObject();
			int childCount = t.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = t.GetChild(0);
				child.parent = gameObject.transform;
			}
			t.localPosition = Vector3.zero;
			for (int j = 0; j < childCount; j++)
			{
				Transform child = gameObject.transform.GetChild(0);
				child.parent = base.transform;
			}
			UnityEngine.Object.DestroyImmediate(gameObject);
		}

		public virtual void DestroyMe(bool undo)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		public virtual MonoBehaviour Add<T>(string label, bool addSameLevel, bool addBefore = false, bool makeSelection = false, int startIndex = 1) where T : TC_ItemBehaviour
		{
			GameObject gameObject = new GameObject();
			Transform transform = gameObject.transform;
			Type typeFromHandle = typeof(T);
			if (label == string.Empty)
			{
				if (typeFromHandle == typeof(TC_LayerGroup))
				{
					label = "Layer Group";
				}
				else if (typeFromHandle == typeof(TC_Layer))
				{
					label = "Layer";
				}
				else if (typeFromHandle == typeof(TC_Node))
				{
					label = "Node";
				}
				else if (typeFromHandle == typeof(TC_SelectItem))
				{
					label = "Item";
				}
				else if (typeFromHandle == typeof(TC_SelectItemGroup))
				{
					label = "Item Group";
				}
			}
			transform.name = label;
			TC_ItemBehaviour tC_ItemBehaviour = gameObject.AddComponent<T>();
			tC_ItemBehaviour.SetVersionNumber();
			tC_ItemBehaviour.outputId = outputId;
			if (addSameLevel)
			{
				transform.parent = t.parent;
				int siblingIndex = t.GetSiblingIndex() + (addBefore ? 1 : 0);
				transform.SetSiblingIndex(siblingIndex);
			}
			else
			{
				if (typeFromHandle == typeof(TC_SelectItemGroup))
				{
					startIndex = 2;
				}
				transform.parent = t;
				int siblingIndex;
				transform.SetSiblingIndex(siblingIndex = startIndex);
			}
			if (transform.parent != null)
			{
				transform.localPosition = Vector3.zero;
			}
			if (typeFromHandle == typeof(TC_Node))
			{
				((TC_Node)tC_ItemBehaviour).SetDefaultSettings();
			}
			else if (typeFromHandle == typeof(TC_LayerGroup))
			{
				if (outputId != 0 && outputId != 4)
				{
					tC_ItemBehaviour.method = Method.Lerp;
				}
				tC_ItemBehaviour.Add<TC_NodeGroup>("Mask Group", false);
				tC_ItemBehaviour.Add<TC_LayerGroupResult>("Result", false);
			}
			else if (typeFromHandle == typeof(TC_Layer))
			{
				if (outputId != 0 && outputId != 4)
				{
					tC_ItemBehaviour.method = Method.Lerp;
				}
				AddLayerNodeGroups((TC_Layer)tC_ItemBehaviour);
			}
			return tC_ItemBehaviour;
		}

		public void AddLayerNodeGroups(TC_Layer layer)
		{
			layer.Add<TC_NodeGroup>("Mask Group", false);
			TC_NodeGroup tC_NodeGroup = (TC_NodeGroup)layer.Add<TC_NodeGroup>("Select Group", false);
			tC_NodeGroup.Add<TC_Node>(string.Empty, false);
			layer.Add<TC_SelectItemGroup>(string.Empty, false);
		}

		public T GetGroup<T>(int index, bool refresh, bool resetTextures) where T : TC_GroupBehaviour
		{
			if (resetTextures)
			{
				DisposeTextures();
			}
			if (index >= t.childCount)
			{
				TC.MoveToDustbin(t);
				return (T)null;
			}
			Transform child = t.GetChild(index);
			T component = child.GetComponent<T>();
			if (component == null)
			{
				TC.MoveToDustbin(t);
			}
			else
			{
				component.SetParameters(this, index);
				component.GetItems(refresh, resetTextures, true);
			}
			return component;
		}

		public void SetParameters(TC_ItemBehaviour parentItem, int index)
		{
			this.parentItem = parentItem;
			level = parentItem.level + 1;
			outputId = parentItem.outputId;
			listIndex = index;
		}

		public TC_ItemBehaviour Duplicate(Transform parent)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(base.gameObject, t.position, t.rotation);
			Transform transform = gameObject.transform;
			transform.parent = parent;
			if (dropPosition == DropPosition.Left)
			{
				transform.SetSiblingIndex(t.GetSiblingIndex() - 1);
			}
			else
			{
				transform.SetSiblingIndex(t.GetSiblingIndex());
			}
			transform.localScale = t.localScale;
			return gameObject.GetComponent<TC_ItemBehaviour>();
		}

		public virtual void GetItems(bool refresh, bool rebuildGlobalLists, bool resetTextures)
		{
		}

		public virtual void SetFirstLoad(bool active)
		{
			firstLoad = active;
		}

		public virtual bool ContainsCollisionNode()
		{
			return false;
		}
	}
}
