using UnityEngine;

public class BrawlerScoringDefinition : StaticDataDefinition, IStaticDataDefinition
{
	public const int SURVIVAL_BONUS_COUNT = 4;

	public const int ENEMY_TYPE_COUNT = 6;

	public const int MEDAL_RATIO_COUNT = 3;

	public const int COMBO_LOSS_COUNT = 2;

	public static BrawlerScoringDefinition Instance;

	protected float[] survivalBonuses;

	protected int[] enemyKoValues;

	protected float[] medalRatios;

	protected float[] comboLoss;

	protected float gimmickBonus;

	public float GimmickBonus
	{
		get
		{
			return gimmickBonus;
		}
	}

	public BrawlerScoringDefinition()
	{
		survivalBonuses = new float[4];
		enemyKoValues = new int[6];
		medalRatios = new float[3];
		comboLoss = new float[2];
	}

	public int GetEnemyKOValue(int enemyRank)
	{
		if (enemyRank < 0 || enemyRank >= 6)
		{
			CspUtils.DebugLog("Invalid enemy rank passed in when asking for KO value!");
		}
		return enemyKoValues[enemyRank];
	}

	public float GetSurvivalBonus(int playerKOs)
	{
		playerKOs = Mathf.Min(playerKOs, 3);
		ActiveMission activeMission = (ActiveMission)AppShell.Instance.SharedHashTable["ActiveMission"];
		if (activeMission != null && activeMission.IsSurvivalMode)
		{
			playerKOs = 0;
		}
		return survivalBonuses[playerKOs];
	}

	public float GetMedalRatio(int medalIndex)
	{
		medalIndex = Mathf.Clamp(medalIndex, 0, 2);
		return medalRatios[medalIndex];
	}

	public float GetComboLoss(int comboIndex)
	{
		comboIndex = Mathf.Clamp(comboIndex, 0, 1);
		return comboLoss[comboIndex];
	}

	public void InitializeFromData(DataWarehouse data)
	{
		if (data.GetCount("brawler_scoring") != 1)
		{
			CspUtils.DebugLog("Invalid number of brawler scoring rule blocks!");
		}
		DataWarehouse data2 = data.GetData("brawler_scoring");
		survivalBonuses[0] = data2.TryGetFloat("defeated_bonus/deaths_0", 0f);
		survivalBonuses[1] = data2.TryGetFloat("defeated_bonus/deaths_1", 0f);
		survivalBonuses[2] = data2.TryGetFloat("defeated_bonus/deaths_2", 0f);
		survivalBonuses[3] = data2.TryGetFloat("defeated_bonus/deaths_3", 0f);
		enemyKoValues[0] = data2.TryGetInt("enemy_bonus/fodder", 0);
		enemyKoValues[1] = data2.TryGetInt("enemy_bonus/regular", 0);
		enemyKoValues[2] = data2.TryGetInt("enemy_bonus/lieutenant", 0);
		enemyKoValues[3] = data2.TryGetInt("enemy_bonus/miniboss", 0);
		enemyKoValues[4] = data2.TryGetInt("enemy_bonus/boss", 0);
		enemyKoValues[5] = data2.TryGetInt("enemy_bonus/megaboss", 0);
		medalRatios[0] = data2.TryGetFloat("score_calculator/adamantium_ratio", 1f);
		medalRatios[1] = data2.TryGetFloat("score_calculator/gold_ratio", 1f);
		medalRatios[2] = data2.TryGetFloat("score_calculator/silver_ratio", 1f);
		comboLoss[0] = data2.TryGetFloat("score_calculator/combo_wave_loss", 1f);
		comboLoss[1] = data2.TryGetFloat("score_calculator/combo_spawner_loss", 1f);
		gimmickBonus = data2.TryGetFloat("gimmick_bonus", 1f);
	}
}
