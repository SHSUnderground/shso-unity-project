public class AvailableQuest : ShsCollectionItem
{
	protected int questId = -1;

	protected string strQuestId;

	public int QuestId
	{
		get
		{
			return questId;
		}
	}

	public AvailableQuest()
	{
	}

	public AvailableQuest(DataWarehouse xmlData)
	{
		InitializeFromData(xmlData);
	}

	public override bool InitializeFromData(DataWarehouse data)
	{
		base.InitializeFromData(data);
		questId = data.TryGetInt("type", -1);
		strQuestId = questId.ToString();
		return true;
	}

	public override void UpdateFromData(DataWarehouse data)
	{
		CspUtils.DebugLog("Needs Implementation!");
	}

	public override string GetKey()
	{
		return strQuestId;
	}
}
