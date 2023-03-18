using System;

public class LauncherCardScenariosListsDefinition : StaticDataDefinitionDictionary<CardScenarioListDefinition>
{
	public LauncherCardScenariosListsDefinition(DataWarehouse data)
	{
		CspUtils.DebugLog(data);
		foreach (DataWarehouse item in data.GetIterator("launcherlists/launcher_list"))
		{
			if (!(item.GetString("type") != Enum.GetName(typeof(LauncherListTypeEnum), LauncherListTypeEnum.CardScenario)))
			{
				CardScenarioListDefinition cardScenarioListDefinition = new CardScenarioListDefinition();
				cardScenarioListDefinition.InitializeFromData(item);
				Add(cardScenarioListDefinition.ListKey, cardScenarioListDefinition);
			}
		}
	}
}
