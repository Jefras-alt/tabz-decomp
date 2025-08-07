using System.Collections.Generic;
using UnityEngine;

namespace TerrainComposer2
{
	public class TC_TerrainLayerGroup : TC_GroupBehaviour
	{
		private List<TerrainLayerGroupItem> itemList = new List<TerrainLayerGroupItem>();

		public override void GetItems(bool refresh, bool rebuildGlobalLists, bool resetTextures)
		{
			int childCount = base.transform.childCount;
			itemList.Clear();
			lastActive = -1;
			totalActive = 0;
			bool flag = true;
			active = visible;
			int num = 0;
			for (int num2 = childCount - 1; num2 >= 0; num2--)
			{
				Transform child = base.transform.GetChild(num2);
				TC_TerrainLayer component = child.GetComponent<TC_TerrainLayer>();
				if (component != null)
				{
					component.SetParameters(this, num++);
					component.terrainLevel = terrainLevel + 1;
					component.GetItems(refresh, rebuildGlobalLists, resetTextures);
					component.GetItem(outputId, rebuildGlobalLists, resetTextures);
					totalActive++;
					itemList.Add(new TerrainLayerGroupItem(null, component));
					lastActive = itemList.Count - 1;
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
					TC_TerrainLayerGroup component2 = child.GetComponent<TC_TerrainLayerGroup>();
					if (component2 != null)
					{
						component2.SetParameters(this, num++);
						component2.terrainLevel = terrainLevel + 1;
						component2.GetItems(refresh, rebuildGlobalLists, resetTextures);
						totalActive++;
						itemList.Add(new TerrainLayerGroupItem(component2, null));
						lastActive = itemList.Count - 1;
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
			else
			{
				active = visible;
			}
		}
	}
}
