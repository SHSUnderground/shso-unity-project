public class AvailableHQRoom : ShsCollectionItem
{
	protected string hqRoomId;

	public string MissionId
	{
		get
		{
			return hqRoomId;
		}
	}

	public AvailableHQRoom()
	{
	}

	public AvailableHQRoom(DataWarehouse xmlData)
	{
		InitializeFromData(xmlData);
	}

	public override bool InitializeFromData(DataWarehouse data)
	{
		base.InitializeFromData(data);
		hqRoomId = data.TryGetString("type", null);
		return true;
	}

	public override void UpdateFromData(DataWarehouse data)
	{
		CspUtils.DebugLog("Needs Implementation!");
	}

	public override string GetKey()
	{
		return hqRoomId;
	}
}
