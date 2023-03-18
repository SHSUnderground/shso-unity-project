using System;

public class LauncherMissionListsDefinition : StaticDataDefinitionDictionary<MissionListDefinition>
{
	public LauncherMissionListsDefinition(DataWarehouse data)
	{
		foreach (DataWarehouse item in data.GetIterator("launcherlists/launcher_list"))
		{
			if (!(item.GetString("type") != Enum.GetName(typeof(LauncherListTypeEnum), LauncherListTypeEnum.BrawlerMission)))
			{
				MissionListDefinition missionListDefinition = new MissionListDefinition();
				missionListDefinition.InitializeFromData(item);
				Add(missionListDefinition.ListKey, missionListDefinition);
			}
		}
	}
}
