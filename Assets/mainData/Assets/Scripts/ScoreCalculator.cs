using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private static int charactersToLoad = 0;

	private static Dictionary<string, DataWarehouse> characterInfo = new Dictionary<string, DataWarehouse>();

	protected static string CalculateScoreForPlayers(int playerCount)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float medalRatio = BrawlerScoringDefinition.Instance.GetMedalRatio(0);
		float medalRatio2 = BrawlerScoringDefinition.Instance.GetMedalRatio(1);
		float medalRatio3 = BrawlerScoringDefinition.Instance.GetMedalRatio(2);
		float comboLoss = BrawlerScoringDefinition.Instance.GetComboLoss(0);
		float comboLoss2 = BrawlerScoringDefinition.Instance.GetComboLoss(1);
		ScenarioEventScore[] array = Utils.FindObjectsOfType<ScenarioEventScore>();
		if (array != null)
		{
			ScenarioEventScore[] array2 = array;
			foreach (ScenarioEventScore scenarioEventScore in array2)
			{
				num += (float)scenarioEventScore.scoreForEvent;
			}
		}
		float num5 = 0f;
		SpawnController[] array3 = Utils.FindObjectsOfType<SpawnController>();
		if (array3 != null)
		{
			SpawnController[] array4 = array3;
			foreach (SpawnController spawnController in array4)
			{
				CharacterSpawn[] componentsInChildren = spawnController.GetComponentsInChildren<CharacterSpawn>();
				if (componentsInChildren == null)
				{
					continue;
				}
				int num6 = 0;
				float num7 = ((float)playerCount - 1f) / 3f;
				float num8 = (float)spawnController.totalSpawnMultiplier4Player * num7 + 1f * (1f - num7);
				int num9 = (int)((float)spawnController.totalSpawns * num8);
				if (num9 > 300)
				{
					num9 = 12;
				}
				float num10 = spawnController.spawnRateMultiplier4Player * num7 + 1f * (1f - num7);
				int a = (int)((float)spawnController.maximumActiveSpawns * num10);
				while (num6 < num9)
				{
					int k = 0;
					int num11;
					for (num11 = Mathf.Min(a, num9 - num6); k < num11; k++)
					{
						CharacterSpawn characterSpawn = componentsInChildren[num6 % componentsInChildren.Length];
						DataWarehouse dataWarehouse = characterInfo[characterSpawn.CharacterName];
						int num12 = dataWarehouse.TryGetInt("//character_stats/stat/initial_value", 1);
						num5 = Mathf.Min(num5 + (float)num12 / (40f * (float)playerCount), 2.99f);
						int enemyRank = dataWarehouse.TryGetInt("//scoring/defeat", 1);
						int enemyKOValue = BrawlerScoringDefinition.Instance.GetEnemyKOValue(enemyRank);
						if (characterSpawn.rewardsPoints)
						{
							num2 += (float)enemyKOValue;
							int num13 = (int)Mathf.Floor(num5);
							num3 += (float)(enemyKOValue * num13);
						}
						num6++;
					}
					if (num11 <= 0)
					{
						num6++;
					}
					if (num6 < spawnController.totalSpawns)
					{
						num5 = Mathf.Max(0f, num5 - comboLoss);
					}
				}
				num5 = Mathf.Max(0f, num5 - comboLoss2);
			}
		}
		num4 = (num + num2 + num3) * (BrawlerScoringDefinition.Instance.GetSurvivalBonus(0) - 1f);
		float num14 = num3 + num + num4;
		int num15 = (int)(num2 + num14 * medalRatio);
		int num16 = (int)(num2 + num14 * medalRatio2);
		int num17 = (int)(num2 + num14 * medalRatio3);
		return playerCount.ToString() + "\t" + num17 + "\t" + num16 + "\t" + num15;
	}

	protected static void InternalCalculateScore()
	{
		string text = "\ts\tg\ta\r\n";
		for (int i = 1; i <= 4; i++)
		{
			text = text + CalculateScoreForPlayers(i) + "\r\n";
		}
		CspUtils.DebugLog(text);
	}

	protected static void OnCharacterDataLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		string text = extraData as string;
		if (text == null || text == string.Empty)
		{
			CspUtils.DebugLog("Invalid character name returned for scored character");
			return;
		}
		characterInfo[text] = response.Data;
		charactersToLoad--;
		if (charactersToLoad <= 0)
		{
			InternalCalculateScore();
		}
	}

	public static void CalculateLevelScore()
	{
		CharacterSpawn[] array = Utils.FindObjectsOfType<CharacterSpawn>();
		charactersToLoad = 0;
		if (array != null)
		{
			CharacterSpawn[] array2 = array;
			foreach (CharacterSpawn characterSpawn in array2)
			{
				if (!characterInfo.ContainsKey(characterSpawn.CharacterName))
				{
					charactersToLoad++;
					characterInfo.Add(characterSpawn.CharacterName, new DataWarehouse());
					AppShell.Instance.DataManager.LoadGameData("Characters/" + characterSpawn.CharacterName, OnCharacterDataLoaded, characterSpawn.CharacterName);
				}
			}
		}
		if (charactersToLoad == 0)
		{
			InternalCalculateScore();
		}
	}

	private void Start()
	{
	}
}
