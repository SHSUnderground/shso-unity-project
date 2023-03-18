using System;
using System.Collections.Generic;

public class CardScenarioListDefinition : StaticDataDefinition, IStaticDataDefinition
{
	private string listKey;

	private List<string> scenarioList;

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
			return scenarioList;
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
		scenarioList = new List<string>();
		listKey = data.GetString("id");
		if (data.GetString("type") != Enum.GetName(typeof(LauncherListTypeEnum), LauncherListTypeEnum.CardScenario))
		{
			throw new Exception("Invalid data type passed to this method. CardScenario List Definition requires scenario XML");
		}
		foreach (DataWarehouse item in data.GetData("cardscenarios").GetIterator("scenario_entry"))
		{
			scenarioList.Add(item.GetString("scenario_id"));
			if (item.TryGetString("desc_override", null) != null)
			{
				overrideDesc = item.GetString("desc_override");
			}
		}
		CspUtils.DebugLog("Card scenario list: " + listKey + " has: " + scenarioList.Count + " Scenarios.");
	}
}
