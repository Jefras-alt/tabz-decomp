public struct ChunkLoadPackage
{
	public int x;

	public int y;

	public int z;

	public bool primaryLoaded;

	public int loadValue;

	public ChunkLoadPackage(int _x, int _y, int _z, bool _primaryLoaded, int _loadValue)
	{
		x = _x;
		y = _y;
		z = _z;
		primaryLoaded = _primaryLoaded;
		loadValue = _loadValue;
	}
}
