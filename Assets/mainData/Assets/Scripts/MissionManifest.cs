public class MissionManifest : StaticDataDefinitionDictionary<MissionManifestEntry>
{
	public static int WIPMissionEntry = -1;

	public void Add(MissionManifestEntryJson json)
	{
		MissionManifestEntry missionManifestEntry = new MissionManifestEntry(json);
		string text = missionManifestEntry.TypeId;
		if (int.Parse(text) <= 0)
		{
			int num = --WIPMissionEntry;
			text = num.ToString();
		}
		Add(text, missionManifestEntry);
	}

	public string GetMissionIdFromKey(string key)
	{
		foreach (MissionManifestEntry value in base.Values)
		{
			if (value.MissionKey == key)
			{
				return value.TypeId;
			}
		}
		return null;
	}

	public string GetMissionKeyFromId(string missionId)
	{
		foreach (MissionManifestEntry value in base.Values)
		{
			if (value.TypeId == missionId)
			{
				return value.MissionKey;
			}
		}
		return null;
	}
}
