public class StatLevelReqsDefinition : StaticDataDefinition, IStaticDataDefinition
{
	public static StatLevelReqsDefinition Instance;

	public static int HEALTH_RANK_COUNT = 8;

	public static int POWER_ATTACKS_COUNT = 3;

	public static int POWER_ATTACKS_RANK_COUNT = 3;

	public static int HEROUP_RANK_COUNT = 3;

	protected int[] healthBonuses;

	protected int[][] powerAttacks;

	protected int[] heroupRanks;

	protected int comboStart;

	protected int powerStart;

	public static int MAX_HERO_LEVEL_NORMAL = 11;

	public static int MAX_HERO_LEVEL_BADGE1 = 20;

	public static int MAX_HERO_LEVEL_BADGE2 = 40;

	public StatLevelReqsDefinition()
	{
		comboStart = 1;
		powerStart = 1;
	}

	protected void ReadTableOfElements(DataWarehouse data, string elementName, int[] dest, int maxElements)
	{
		int num = data.GetCount(elementName);
		if (num != maxElements)
		{
			CspUtils.DebugLog("Invalid number of " + elementName + " elements!  Please correct your brawler_leveling_stats file.");
			CspUtils.DebugLog("Count is: " + num.ToString() + ",  Should be: " + maxElements.ToString());
			num = 0;
		}
		for (int i = 0; i < num; i++)
		{
			dest[i] = data.GetInt(elementName, i);
		}
	}

	public void InitializeFromData(DataWarehouse data)
	{
		if (data.GetCount("brawler_leveling_stats") != 1)
		{
			CspUtils.DebugLog("Invalid number of stat-level blocks!");
		}
		DataWarehouse data2 = data.GetData("brawler_leveling_stats");
		HEALTH_RANK_COUNT = data2.GetData("health_bonuses").GetCount("level");
		healthBonuses = new int[HEALTH_RANK_COUNT];
		ReadTableOfElements(data2.GetData("health_bonuses"), "level", healthBonuses, HEALTH_RANK_COUNT);
		POWER_ATTACKS_COUNT = data2.GetData("power_attacks").GetCount("power_attack");
		powerAttacks = new int[POWER_ATTACKS_COUNT][];
		for (int i = 0; i < POWER_ATTACKS_COUNT; i++)
		{
			powerAttacks[i] = new int[POWER_ATTACKS_RANK_COUNT];
			DataWarehouse data3 = data2.GetData("power_attacks").GetData("power_attack", i);
			for (int j = 0; j < data3.GetCount("level"); j++)
			{
				powerAttacks[i][j] = data3.GetInt("level", j);
			}
		}
		HEROUP_RANK_COUNT = data2.GetData("heroup_attack").GetCount("level");
		heroupRanks = new int[HEROUP_RANK_COUNT];
		ReadTableOfElements(data2.GetData("heroup_attack"), "level", heroupRanks, HEROUP_RANK_COUNT);
		comboStart = data2.TryGetInt("combo_start", 1);
		powerStart = data2.TryGetInt("power_start", 1);
	}

	public int GetLevelHealthRankIsUnlockedAt(int whatRank)
	{
		return healthBonuses[whatRank - 1];
	}

	public int GetNumberOfHealthRanksForLevel(int level)
	{
		return UnlocksForLevel(level, healthBonuses);
	}

	public int GetLevelHeroupRankIsUnlockedAt(int whatRank)
	{
		return heroupRanks[whatRank - 1];
	}

	public int GetNumberOfHeroupRanksForLevel(int level)
	{
		return UnlocksForLevel(level, heroupRanks);
	}

	public int GetMaxPowerAttackUnlockedAt(int level)
	{
		int num = 0;
		for (num = 0; num < POWER_ATTACKS_COUNT && powerAttacks[num][0] <= level; num++)
		{
		}
		if (num > POWER_ATTACKS_COUNT)
		{
			return POWER_ATTACKS_COUNT;
		}
		return num;
	}

	public int GetLChainIndexAt(int level)
	{
		return 0;
	}

	public int GetMaxPowerAttackRankUnlockedAt(int attackIndex, int level)
	{
		attackIndex--;
		int num = 0;
		for (num = 0; num < POWER_ATTACKS_RANK_COUNT && powerAttacks[attackIndex][num] <= level; num++)
		{
		}
		return num;
	}

	public int GetLevelPowerAttackRankIsUnlockedAt(int attackIndex, int rank)
	{
		return powerAttacks[attackIndex - 1][rank - 1];
	}

	protected int UnlocksForLevel(int level, int[] data)
	{
		int num = 0;
		foreach (int num2 in data)
		{
			if (level >= num2)
			{
				num++;
			}
		}
		return num;
	}

	public int GetMaxCombo(int level)
	{
		return 5;
	}
}
