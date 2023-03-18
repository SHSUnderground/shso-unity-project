public class AvailableMission : ShsCollectionItem
{
	protected string missionId;

	public string MissionId
	{
		get
		{
			return missionId;
		}
	}

	public AvailableMission()
	{
	}

	public AvailableMission(DataWarehouse xmlData)
	{
		InitializeFromData(xmlData);
	}

	public AvailableMission(string missionId)
	{
		this.missionId = missionId;
	}

	public override bool InitializeFromData(DataWarehouse data)
	{
		base.InitializeFromData(data);
		missionId = data.TryGetString("type", null);
		return true;
	}

	public override void UpdateFromData(DataWarehouse data)
	{
		CspUtils.DebugLog("Needs Implementation!");
	}

	public override string GetKey()
	{
		return missionId;
	}
}
