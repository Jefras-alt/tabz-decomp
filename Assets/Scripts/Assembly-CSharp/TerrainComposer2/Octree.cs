using System.Collections.Generic;
using UnityEngine;

namespace TerrainComposer2
{
	public class Octree
	{
		public class SpawnedObject
		{
			public int objectIndex;

			public int listIndex;

			public Vector3 position;

			public MaxCell cell;

			public SpawnedObject(int objectIndex, Vector3 position)
			{
				this.objectIndex = objectIndex;
				this.position = position;
			}

			public void Remove()
			{
				cell.objects[listIndex] = cell.objects[cell.objects.Count - 1];
				cell.objects.RemoveAt(cell.objects.Count - 1);
				if (cell.objects.Count == 0)
				{
					cell.parent.RemoveCell(cell.cellIndex);
				}
			}
		}

		public class MaxCell : Cell
		{
			public List<SpawnedObject> objects = new List<SpawnedObject>();

			public MaxCell(Cell parent, int cellIndex, Bounds bounds)
				: base(parent, cellIndex, bounds)
			{
			}

			~MaxCell()
			{
				objects.Clear();
				objects = null;
			}
		}

		public class Cell
		{
			public Cell mainParent;

			public Cell parent;

			public Cell[] cells;

			public bool[] cellsUsed;

			public Bounds bounds;

			public int cellIndex;

			public int cellCount;

			public int level;

			public int maxLevels;

			public Cell(Cell parent, int cellIndex, Bounds bounds)
			{
				if (parent != null)
				{
					maxLevels = parent.maxLevels;
					mainParent = parent.mainParent;
					level = parent.level + 1;
				}
				this.parent = parent;
				this.cellIndex = cellIndex;
				this.bounds = bounds;
			}

			~Cell()
			{
				mainParent = (parent = null);
				cells = null;
			}

			private int AddCell(Vector3 position)
			{
				Vector3 vector = position - this.bounds.min;
				int num = (int)(vector.x / this.bounds.extents.x);
				int num2 = (int)(vector.y / this.bounds.extents.y);
				int num3 = (int)(vector.z / this.bounds.extents.z);
				int num4 = num + num2 * 4 + num3 * 2;
				if (cells == null)
				{
					cells = new Cell[8];
					cellsUsed = new bool[8];
				}
				if (!cellsUsed[num4])
				{
					Bounds bounds = new Bounds(new Vector3(this.bounds.min.x + this.bounds.extents.x * ((float)num + 0.5f), this.bounds.min.y + this.bounds.extents.y * ((float)num2 + 0.5f), this.bounds.min.z + this.bounds.extents.z * ((float)num3 + 0.5f)), this.bounds.extents);
					if (level == maxLevels - 1)
					{
						cells[num4] = new MaxCell(this, num4, bounds);
					}
					else
					{
						cells[num4] = new Cell(this, num4, bounds);
					}
					cellsUsed[num4] = true;
					cellCount++;
				}
				return num4;
			}

			public void RemoveCell(int index)
			{
				cells[index] = null;
				cellsUsed[index] = false;
				cellCount--;
				if (cellCount == 0 && parent != null)
				{
					parent.RemoveCell(cellIndex);
				}
			}

			public bool InsideBounds(Vector3 position)
			{
				position -= bounds.min;
				if (position.x >= bounds.size.x || position.y >= bounds.size.y || position.z >= bounds.size.z || position.x < 0f || position.y < 0f || position.z < 0f)
				{
					return false;
				}
				return true;
			}

			public void AddObject(SpawnedObject obj)
			{
				if (bounds.Contains(obj.position))
				{
					AddObjectInternal(obj);
				}
			}

			private void AddObjectInternal(SpawnedObject obj)
			{
				if (level == maxLevels)
				{
					obj.cell = (MaxCell)this;
					obj.cell.objects.Add(obj);
					obj.listIndex = obj.cell.objects.Count - 1;
				}
				else
				{
					int num = AddCell(obj.position);
					cells[num].AddObjectInternal(obj);
				}
			}

			public void Reset()
			{
				if (cells != null)
				{
					for (int i = 0; i < 8; i++)
					{
						cells[i] = null;
						cellsUsed[i] = false;
					}
				}
			}

			public void Draw(bool onlyMaxLevel)
			{
				if (!onlyMaxLevel || level == maxLevels)
				{
					Gizmos.DrawWireCube(bounds.center, bounds.size);
					if (level == maxLevels)
					{
						return;
					}
				}
				for (int i = 0; i < 8; i++)
				{
					if (cellsUsed[i])
					{
						cells[i].Draw(onlyMaxLevel);
					}
				}
			}
		}

		public Cell cell;
	}
}
