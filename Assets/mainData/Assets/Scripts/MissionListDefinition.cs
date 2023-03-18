using System;
using System.Collections.Generic;

public class MissionListDefinition : StaticDataDefinition, IStaticDataDefinition
{
	private string listKey;

	private List<string> missionList;

	private string overrideDesc;

	public string ListKey
	{
		get
		{
			return listKey;
		}
	}

	public List<string> MissionList
	{
		get
		{
			return missionList;
		}
	}

	public string OverrideDesc
	{
		get
		{
			return overrideDesc;
		}
	}

	public void InitializeFromData(DataWarehouse data)
	{
		missionList = new List<string>();
		listKey = data.GetString("id");
		if (data.GetString("type") != Enum.GetName(typeof(LauncherListTypeEnum), LauncherListTypeEnum.BrawlerMission))
		{
			throw new Exception("Invalid data type passed to this method. Mission List Definition requires mission XML");
		}
		foreach (DataWarehouse item in data.GetData("missions").GetIterator("mission_entry"))
		{
			missionList.Add(item.GetString("mission_id"));
			if (item.TryGetString("desc_override", null) != null)
			{
				overrideDesc = item.GetString("desc_override");
			}
		}
	}
}
