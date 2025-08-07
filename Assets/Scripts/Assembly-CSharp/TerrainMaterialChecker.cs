using System;
using UnityEngine;

public class TerrainMaterialChecker : MonoBehaviour
{
	public enum GroundTypes
	{
		Grass = 0,
		Road = 1,
		Inside = 2,
		Snow = 3,
		None = 4
	}

	private int surfaceIndex;

	private static Terrain terrain;

	private static TerrainData terrainData;

	private static Vector3 terrainPos;

	private static TerrainMaterialChecker mInstance;

	[SerializeField]
	private Texture[] m_GrassMaterials;

	[SerializeField]
	private Texture[] m_RoadMaterials;

	[SerializeField]
	private Texture[] m_InsideMaterials;

	[SerializeField]
	private Texture[] m_SnowMaterials;

	private GroundTypes mDefault;

	public static TerrainMaterialChecker Instance
	{
		get
		{
			return mInstance;
		}
	}

	private void Awake()
	{
		if (mInstance != null)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			mInstance = this;
		}
	}

	private void Start()
	{
		terrain = Terrain.activeTerrain;
		terrainData = terrain.terrainData;
		terrainPos = terrain.transform.position;
	}

	public GroundTypes GetGroundType(string mat)
	{
		bool flag = false;
		if (IsMaterial(GroundTypes.Grass, mat))
		{
			return GroundTypes.Grass;
		}
		if (IsMaterial(GroundTypes.Road, mat))
		{
			return GroundTypes.Road;
		}
		if (IsMaterial(GroundTypes.Inside, mat))
		{
			return GroundTypes.Inside;
		}
		return mDefault;
	}

	private bool IsMaterial(GroundTypes type, string mat)
	{
		Texture[] array;
		switch (type)
		{
		case GroundTypes.Grass:
			array = m_GrassMaterials;
			break;
		case GroundTypes.Road:
			array = m_RoadMaterials;
			break;
		case GroundTypes.Inside:
			array = m_InsideMaterials;
			break;
		default:
			throw new Exception("Invalid GroundType: " + type);
		}
		if (array == null)
		{
			return false;
		}
		for (ushort num = 0; num < array.Length; num++)
		{
			Texture texture = array[num];
			if (texture.name.Equals(mat.Split(' ')[0]))
			{
				return true;
			}
		}
		return false;
	}

	public GroundTypes GetCurrentMaterialName(Vector3 position)
	{
		int mainTexture = GetMainTexture(position);
		string mat = terrainData.splatPrototypes[mainTexture].texture.name;
		return GetGroundType(mat);
	}

	private static float[] GetTextureMix(Vector3 WorldPos)
	{
		int x = (int)((WorldPos.x - terrainPos.x) / terrainData.size.x * (float)terrainData.alphamapWidth);
		int y = (int)((WorldPos.z - terrainPos.z) / terrainData.size.z * (float)terrainData.alphamapHeight);
		float[,,] alphamaps = terrainData.GetAlphamaps(x, y, 1, 1);
		float[] array = new float[alphamaps.GetUpperBound(2) + 1];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = alphamaps[0, 0, i];
		}
		return array;
	}

	private static int GetMainTexture(Vector3 WorldPos)
	{
		float[] textureMix = GetTextureMix(WorldPos);
		float num = 0f;
		int result = 0;
		for (int i = 0; i < textureMix.Length; i++)
		{
			if (textureMix[i] > num)
			{
				result = i;
				num = textureMix[i];
			}
		}
		return result;
	}
}
