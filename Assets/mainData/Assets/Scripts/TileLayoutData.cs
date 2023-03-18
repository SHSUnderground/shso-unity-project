using System.Collections.Generic;

public class TileLayoutData : StaticDataDefinition, IStaticDataDefinition
{
	public List<TileData> tiles;

	public TileLayoutData()
	{
		tiles = new List<TileData>();
	}

	public void InitializeFromData(DataWarehouse data)
	{
		foreach (DataWarehouse item in data.GetIterator("//Tiles/Tile"))
		{
			TileData tileData = new TileData();
			tileData.InitializeFromData(item);
			tiles.Add(tileData);
		}
	}
}
