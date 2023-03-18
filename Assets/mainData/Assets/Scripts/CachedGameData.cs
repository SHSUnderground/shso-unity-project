using System.Collections.Generic;

public class CachedGameData
{
	public string Path = string.Empty;

	public string DataText;

	public StaticDataDefinition DataDefinition;

	public float TimeLoaded;

	public bool needsDataWarehouse;

	public Queue<GameDataCallbackWithData> OutstandingCallbacks;

	public CachedGameData()
	{
		OutstandingCallbacks = new Queue<GameDataCallbackWithData>();
	}
}
