public class HQRoomManifest : StaticDataDefinitionDictionary<HQRoomManifestEntry>
{
	public HQRoomManifest(DataWarehouse data)
	{
		foreach (DataWarehouse item in data.GetIterator("Rooms/Room"))
		{
			HQRoomManifestEntry hQRoomManifestEntry = new HQRoomManifestEntry();
			hQRoomManifestEntry.InitializeFromData(item);
			Add(hQRoomManifestEntry.TypeId, hQRoomManifestEntry);
		}
	}

	public string GetRoomIdFromKey(string key)
	{
		foreach (HQRoomManifestEntry value in base.Values)
		{
			if (value.RoomKey == key)
			{
				return value.TypeId;
			}
		}
		return null;
	}
}
