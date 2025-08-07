using System;
using UnityEngine;

namespace TerrainComposer2
{
	[Serializable]
	public class CachedTransform
	{
		public Vector3 position;

		public Vector3 posOffset;

		public Quaternion rotation;

		public Vector3 scale;

		public float positionYOld;

		public void CopySpecial(TC_Node node)
		{
			posOffset = node.posOffset;
			bool lockPosParent = node.lockPosParent;
			if (node.lockTransform || lockPosParent)
			{
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				if (!node.lockPosX && !lockPosParent)
				{
					zero.x = node.t.position.x;
				}
				else
				{
					zero.x = position.x;
				}
				if (!node.lockPosY && !lockPosParent)
				{
					zero.y = node.posY * scale.y;
				}
				else
				{
					zero.y = position.y;
				}
				if (!node.lockPosZ && !lockPosParent)
				{
					zero.z = node.t.position.z;
				}
				else
				{
					zero.z = position.z;
				}
				Quaternion quaternion = ((node.lockRotY && node.lockTransform) ? rotation : Quaternion.Euler(0f, node.t.eulerAngles.y, 0f));
				if (!node.lockScaleX || !node.lockTransform)
				{
					zero2.x = node.t.lossyScale.x;
				}
				else
				{
					zero2.x = scale.x;
				}
				if (!node.lockScaleY || !node.lockTransform)
				{
					if (node.nodeType == NodeGroupType.Mask)
					{
						zero2.y = node.t.localScale.y;
					}
					else
					{
						zero2.y = node.t.lossyScale.y * node.opacity;
					}
				}
				else
				{
					zero2.y = scale.y;
				}
				if (!node.lockScaleZ || !node.lockTransform)
				{
					zero2.z = node.t.lossyScale.z;
				}
				else
				{
					zero2.z = scale.z;
				}
				position = zero;
				rotation = quaternion;
				scale = zero2;
				if (node.t.position != position)
				{
					node.t.position = position;
				}
				if (node.t.rotation != rotation)
				{
					node.t.rotation = rotation;
				}
				node.t.hasChanged = false;
			}
			else
			{
				rotation = Quaternion.Euler(0f, node.t.eulerAngles.y, 0f);
				scale.x = node.t.lossyScale.x;
				scale.z = node.t.lossyScale.z;
				if (node.nodeType == NodeGroupType.Mask)
				{
					scale.y = node.t.localScale.y;
				}
				else
				{
					scale.y = node.t.lossyScale.y;
				}
				scale.y *= node.opacity;
				position = node.t.position;
				position.y = node.posY * scale.y;
			}
		}

		public void Copy(TC_ItemBehaviour item)
		{
			position.x = item.t.position.x;
			position.z = item.t.position.z;
			posOffset = item.posOffset;
			rotation = item.t.rotation;
			scale = item.t.lossyScale;
			positionYOld = item.posY;
		}

		public void Copy(Transform t)
		{
			position = t.position;
			rotation = t.rotation;
			scale = t.lossyScale;
		}

		public bool hasChanged(Transform t)
		{
			if (t == null)
			{
				return false;
			}
			if (position != t.position || rotation != t.rotation || scale != t.lossyScale)
			{
				return true;
			}
			return false;
		}

		public bool hasChanged(TC_ItemBehaviour item)
		{
			if (position.x != item.t.position.x || position.z != item.t.position.z || item.posY != positionYOld)
			{
				return true;
			}
			if (rotation != item.t.rotation)
			{
				return true;
			}
			if (scale != item.t.lossyScale)
			{
				return true;
			}
			return false;
		}
	}
}
