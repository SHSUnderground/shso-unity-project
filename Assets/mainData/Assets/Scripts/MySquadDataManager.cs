using System;
using System.Collections.Generic;
using System.Linq;

public class MySquadDataManager
{
	public enum Tabs
	{
		Upgrades,
		Medals,
		Items
	}

	public UserProfile Profile;

	public Tabs CurrentlySelectedTab;

	protected string squadName;

	protected int squadLevel;

	protected int playerId;

	public bool isLocalPlayer
	{
		get
		{
			if (Profile != null)
			{
				return Profile.ProfileType == UserProfile.ProfileTypeEnum.LocalPlayer;
			}
			if (AppShell.Instance != null && AppShell.Instance.Profile != null)
			{
				return playerId == AppShell.Instance.Profile.UserId;
			}
			return false;
		}
	}

	public string SquadName
	{
		get
		{
			if (Profile == null || Profile.ProfileType == UserProfile.ProfileTypeEnum.RemotePlayer)
			{
				return squadName;
			}
			return Profile.PlayerName;
		}
	}

	public int SquadLevel
	{
		get
		{
			if (Profile == null || Profile.ProfileType == UserProfile.ProfileTypeEnum.RemotePlayer)
			{
				return squadLevel;
			}
			return Profile.SquadLevel;
		}
	}

	public MySquadDataManager(long playerId, string squadName, int squadLevel)
	{
		this.squadName = squadName;
		this.squadLevel = squadLevel;
		if (playerId != AppShell.Instance.Profile.UserId)
		{
			RemotePlayerProfile.FetchProfile(playerId, delegate(UserProfile profile)
			{
				Profile = profile;
			});
		}
		else
		{
			Profile = AppShell.Instance.Profile;
		}
	}

	public MySquadDataManager(UserProfile profile)
	{
		Profile = profile;
		if (Profile == null)
		{
			CspUtils.DebugLog("If your user profile does not exist, this gadget will not work");
		}
		CurrentlySelectedTab = Tabs.Upgrades;
	}

	public void ClearProfile()
	{
		if (Profile != null && Profile is RemotePlayerProfile)
		{
			((RemotePlayerProfile)Profile).Dispose();
			Profile = null;
		}
	}

	public static List<ItemDefinition> GetTheFiveItemUnlockables(string heroName)
	{
		LevelUpRewardItemsManifest levelUpRewardItemsManifest = AppShell.Instance.LevelUpRewardItemsManifest;
		if (levelUpRewardItemsManifest.ContainsKey(heroName))
		{
			LevelUpRewardItems levelUpRewardItems = AppShell.Instance.LevelUpRewardItemsManifest[heroName];
			List<int> list = new List<int>(5);
			list.AddRange(levelUpRewardItems.GetRewardsForLevel(2));
			list.AddRange(levelUpRewardItems.GetRewardsForLevel(4));
			list.AddRange(levelUpRewardItems.GetRewardsForLevel(6));
			list.AddRange(levelUpRewardItems.GetRewardsForLevel(8));
			list.AddRange(levelUpRewardItems.GetRewardsForLevel(10));
			return Enumerable.ToList(Enumerable.Select<int, ItemDefinition>((IEnumerable<int>)list, (Func<int, ItemDefinition>)delegate(int o)
			{
				return AppShell.Instance.ItemDictionary[o.ToString()];
			}));
		}
		CspUtils.DebugLog(string.Format("Hero {0} does not have an entry in the level up reward items manifest.", heroName));
		return new List<ItemDefinition>();
	}

	public static string GetLevelText(UserProfile profile, string heroName)
	{
		if (profile.AvailableCostumes.ContainsKey(heroName))
		{
			int level = profile.AvailableCostumes[heroName].Level;
			if (level == profile.AvailableCostumes[heroName].MaxLevel)
			{
				return string.Format(AppShell.Instance.stringTable["#airlock_hero_level"], GetMax());
			}
			return string.Format(AppShell.Instance.stringTable["#airlock_hero_level"], level);
		}
		return string.Empty;
	}

	public static float GetPercToNextLevel(string heroName)
	{
		return GetPercToNextLevel(AppShell.Instance.Profile, heroName);
	}

	public static float GetPercToNextLevel(UserProfile profile, string heroName)
	{
		int expCur;
		int expNext;
		bool max;
		XpToLevelDefinition.GetExp(profile, out expCur, out expNext, out max, heroName);
		float num = (float)expCur * 1f / ((float)expNext * 1f);
		if (expCur == expNext || max || num > 1f || num < 0f || float.IsInfinity(num) || float.IsNaN(num))
		{
			num = 1f;
		}
		return num;
	}

	public static string GetExpTextRaw(UserProfile profile, string heroName)
	{
		int expCur;
		int expNext;
		bool max;
		XpToLevelDefinition.GetExp(profile, out expCur, out expNext, out max, heroName);
		if (expNext == int.MaxValue)
		{
			return "#Max_Exp";
		}
		return expCur.ToString() + "/" + expNext.ToString();
	}

	public static string GetUnlockAtLevelText(int unlockAtLevel)
	{
		return string.Format(AppShell.Instance.stringTable["#unlocked_at_level"], unlockAtLevel.ToString());
	}

	public static string GetHealthLevelText(int levelAt)
	{
		if (levelAt >= StatLevelReqsDefinition.HEALTH_RANK_COUNT)
		{
			return GetMax();
		}
		return string.Format(AppShell.Instance.stringTable["#Rank"], levelAt);
	}

	public static string GetPowerAttackName(string heroName, int rank)
	{
		return AppShell.Instance.stringTable[string.Format("#RIGHTCLICK{0}_{1}", rank + 1, heroName.ToUpper())];
	}

	public static string GetPowerAttackName(string heroName, int whatPowerAttack, int rank)
	{
		if (whatPowerAttack == 0)
		{
			switch (rank)
			{
			case 1:
				return "#HEROUP";
			case 2:
				return "#UPGRADEDHEROUP";
			case 3:
				return "#UPGRADEDHEROUP2";
			}
		}
		string text = AppShell.Instance.stringTable[string.Format("#RIGHTCLICK{0}_{1}", whatPowerAttack, heroName.ToUpper())];
		switch (rank)
		{
		case 2:
			text = string.Format(AppShell.Instance.stringTable["#UpgradePowerAttack"], text);
			break;
		case 3:
			text = string.Format(AppShell.Instance.stringTable["#UpgradePowerAttack2"], text);
			break;
		}
		return text;
	}

	public static string GetUpgradedPowerAttackName(string heroName, int rank)
	{
		string arg = AppShell.Instance.stringTable[string.Format("#RIGHTCLICK{0}_{1}", rank + 1, heroName.ToUpper())];
		return string.Format(AppShell.Instance.stringTable["#UpgradePowerAttack"], arg);
	}

	public static string GetMax()
	{
		return AppShell.Instance.stringTable["#Max"];
	}
}
