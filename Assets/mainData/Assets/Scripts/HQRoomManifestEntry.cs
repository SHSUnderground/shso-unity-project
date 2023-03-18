public class HQRoomManifestEntry : StaticDataDefinition, IStaticDataDefinition
{
	public string TypeId;

	public string RoomKey;

	public void InitializeFromData(DataWarehouse data)
	{
		TypeId = data.GetString("typeid");
		RoomKey = data.GetString("id");
	}
}
