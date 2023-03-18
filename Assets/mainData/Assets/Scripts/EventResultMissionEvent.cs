using System;
using System.Collections;
using System.Collections.Generic;

public class EventResultMissionEvent : EventResultBase
{
	protected long gazillionUserID;

	protected int totalScore;

	protected int totalKOs;

	protected int totalPickups;

	protected Dictionary<long, MissionResults> playerResults;

	public Dictionary<long, MissionResults> PlayerResults
	{
		get
		{
			return playerResults;
		}
	}

	public int TotalScore
	{
		get
		{
			return totalScore;
		}
		set
		{
			totalScore = value;
		}
	}

	public int TotalKOs
	{
		get
		{
			return totalKOs;
		}
	}

	public int TotalPickups
	{
		get
		{
			return totalPickups;
		}
	}

	public EventResultMissionEvent()
		: base(EventResultType.MissionEvent)
	{
	}

	public void ProcessPassthrough(string passThroughChunk)
	{
		CspUtils.DebugLog("Process passthrough: " + passThroughChunk);
		if (passThroughChunk == null)
		{
			CspUtils.DebugLog("passThroughChunk was empty!");
			return;
		}
		DataWarehouse dataWarehouse = new DataWarehouse(passThroughChunk);
		dataWarehouse.Parse();
		DataWarehouse dataWarehouse2 = dataWarehouse.TryGetData("passthrough", null);
		if (dataWarehouse2 == null)
		{
			CspUtils.DebugLog("passThrough was empty!");
			return;
		}
		int count = dataWarehouse2.GetCount("//player");
		for (int i = 0; i < count; i++)
		{
			DataWarehouse data = dataWarehouse2.GetData("//player", i);
			long key = data.TryGetLong("id", -1L);
			if (playerResults.ContainsKey(key))
			{
				MissionResults missionResults = playerResults[key];
				missionResults.survivalScore = data.TryGetInt("player_kos", 0);
				missionResults.enemyKoScore = data.TryGetInt("enemy_kos", 0);
				missionResults.gimmickScore = data.TryGetInt("gimmick_bonus", 0);
				missionResults.comboScore = data.TryGetInt("combo_bonus", 0);
			}
		}
	}

	public override void InitializeFromData(DataWarehouse data)
	{
		throw new Exception("STUPID DEAD CODE");
	}

	public override void InitializeFromData(Hashtable data)
	{
		totalScore = int.Parse((string)data["total_score"]);
		totalKOs = int.Parse((string)data["kos"]);
		totalPickups = int.Parse((string)data["pickups"]);
		playerResults = new Dictionary<long, MissionResults>();
		string[] entryList = GetEntryList(data, "leveled_up");
		string[] entryList2 = GetEntryList(data, "hero_name");
		string[] entryList3 = GetEntryList(data, "player_ids");
		int num = entryList3.Length;
		int i = 0;
		PlayerDictionary.Player value;
		AppShell.Instance.PlayerDictionary.TryGetValue(AppShell.Instance.ServerConnection.GetGameUserId(), out value);
		long num2 = -1L;
		if (value != null)
		{
			num2 = value.PlayerId;
		}
		for (; i < num; i++)
		{
			MissionResults missionResults = new MissionResults();
			missionResults.heroName = entryList2[i];
			long num3 = long.Parse(entryList3[i]);
			if (num3 == num2)
			{
				missionResults.currentXp = int.Parse((string)data["hero_current_xp"]);
				missionResults.bonusXp = int.Parse((string)data["bonus_xp"]);
				CspUtils.DebugLog(" bonus XP was " + missionResults.bonusXp);
				missionResults.coins = int.Parse((string)data["silver"]);
				missionResults.tickets = int.Parse((string)data["tickets"]);
				missionResults.earnedXp = int.Parse((string)data["xp"]);
				missionResults.ownable = (string)data["ownable_type_id"];
				missionResults.rewardTier = int.Parse((string)data["reward_tier"]);
				CspUtils.DebugLog(" reward tier was " + missionResults.rewardTier);
				CspUtils.DebugLog(" other rewards were " + missionResults.coins + " | " + missionResults.tickets + " | " + missionResults.earnedXp + " | " + missionResults.ownable);
			}
			int j = 0;
			missionResults.levelUp = false;
			for (; j < entryList.Length; j++)
			{
				if (num3 == long.Parse(entryList[j]))
				{
					missionResults.levelUp = true;
					break;
				}
			}
			playerResults.Add(num3, missionResults);
		}
		string passThroughChunk = (string)data["display_data"];
		ProcessPassthrough(passThroughChunk);
	}

	protected string[] GetEntryList(Hashtable data, string entryKey)
	{
		char[] separator = new char[1]
		{
			','
		};
		if (!data.ContainsKey(entryKey))
		{
			return new string[0];
		}
		return ((string)data[entryKey]).Split(separator, StringSplitOptions.RemoveEmptyEntries);
	}
}
