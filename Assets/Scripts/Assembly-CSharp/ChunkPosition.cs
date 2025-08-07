public struct ChunkPosition
{
	public int x;

	public int y;

	public int z;

	public ChunkPosition(int _x, int _y, int _z)
	{
		x = _x;
		y = _y;
		z = _z;
	}

	public static bool IsSame(ChunkPosition a, ChunkPosition b)
	{
		if (a.x == b.x && a.y == b.y && a.z == b.z)
		{
			return true;
		}
		return false;
	}

	public override string ToString()
	{
		return "[" + x + ":" + y + ":" + z + "]";
	}
}
