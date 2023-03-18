using System.Collections.Generic;

public class HeroDefinition
{
	public string name = "BAD CHARACTER";

	public int ownableTypeID;

	public OwnableDefinition ownableDef;

	private int baseHealth;

	private int maxHealth;

	private List<int> healthIncreases = new List<int>();

	public CharacterDefinition charDef;

	public string familyName
	{
		get
		{
			return AppShell.Instance.CharacterDescriptionManager[name].CharacterFamily;
		}
	}

	public HeroDefinition(DataWarehouse data)
	{
		if (data == null)
		{
			CspUtils.DebugLog("got a null DataWarehouse when parsing a hero def");
			return;
		}
		name = data.TryGetString("//name", "WEEE");
		charDef = new CharacterDefinition();
		charDef.InitializeFromData(data);
		charDef.isVillain = AppShell.Instance.CharacterDescriptionManager.VillainList.Contains(name);
		DataWarehouse data2 = data.GetData("//character_stats");
		foreach (DataWarehouse item in data2.GetIterator("stat"))
		{
			switch (item.TryGetString("stat_type", "*missing*"))
			{
			case "Health":
				baseHealth = (int)item.TryGetFloat("initial_maximum", 100f);
				maxHealth = baseHealth;
				break;
			}
		}
		healthIncreases.Add(0);
		DataWarehouse data3 = data.GetData("//player_combat_controller");
		for (int i = 1; i <= StatLevelReqsDefinition.HEALTH_RANK_COUNT; i++)
		{
			int num = (int)data3.TryGetFloat("health_increase_" + i, 0f);
			healthIncreases.Add(num);
			maxHealth += num;
		}
	}

	public OwnableDefinition getBadgeDef(int tier)
	{
		if (!OwnableDefinition.HeroIDToBadgeID.ContainsKey(ownableTypeID))
		{
			CspUtils.DebugLog("getBadgeDef - OwnableDefinition.HeroIDToBadgeID does not contain an entry for " + name + " (ownable type ID " + ownableTypeID + ")");
			return null;
		}
		return OwnableDefinition.getDef(OwnableDefinition.HeroIDToBadgeID[ownableTypeID][tier - 1]);
	}

	public int getMaxHealth()
	{
		return maxHealth;
	}

	public int getBaseHealth()
	{
		return baseHealth;
	}

	public int getHealthAtLevel(int level)
	{
		int num = baseHealth;
		int numberOfHealthRanksForLevel = StatLevelReqsDefinition.Instance.GetNumberOfHealthRanksForLevel(level);
		int num2 = 1;
		while (num2 <= numberOfHealthRanksForLevel)
		{
			num += healthIncreases[num2++];
		}
		if (level > 20)
		{
			num += (level - 20) * 10;
		}
		return num;
	}
}
