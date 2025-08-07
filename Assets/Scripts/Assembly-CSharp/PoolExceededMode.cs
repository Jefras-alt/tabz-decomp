using System;

[Serializable]
public enum PoolExceededMode
{
	Ignore = 0,
	StopSpawning = 1,
	ReUse = 2
}
