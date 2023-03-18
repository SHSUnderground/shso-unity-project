using System;
using System.Runtime.CompilerServices;

public class XpToLevelDefinition
{
	public const int MAX_XP = int.MaxValue;

	private const int MAX_POSSIBLE_LEVEL = 100;

	public static XpToLevelDefinition Instance = new XpToLevelDefinition();

	private readonly int[] xpNeededForLevel;

	[CompilerGenerated]
	private int _003CMaxLevel_003Ek__BackingField;

	public int MaxLevel
	{
		[CompilerGenerated]
		get
		{
			return _003CMaxLevel_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CMaxLevel_003Ek__BackingField = value;
		}
	}

	public XpToLevelDefinition()
	{
		xpNeededForLevel = new int[101];
	}

	public void AddLevel(int level, int neededXp)
	{
		if (level > 0 && neededXp >= 0)
		{
			xpNeededForLevel[level] = neededXp;
			MaxLevel = Math.Max(MaxLevel, level);
		}
		else
		{
			CspUtils.DebugLog("Missing data in XP to Level table! (level=" + level + ", xp needed=" + neededXp + ").");
		}
	}

	public int GetXpForLevel(int level)
	{
		if (level > 0 && level <= MaxLevel)
		{
			return xpNeededForLevel[level];
		}
		if (level > MaxLevel)
		{
			return int.MaxValue;
		}
		CspUtils.DebugLog("Asked for XP Needed for invalid level <" + level + ">, failing.");
		return -1;
	}

	public string GetXpDescriptionForLevel(int level)
	{
		if (level > 0 && level <= MaxLevel)
		{
			return xpNeededForLevel[level].ToString();
		}
		if (level > MaxLevel)
		{
			return "#Max";
		}
		CspUtils.DebugLog("Asked for XP Needed for invalid level <" + level + ">, failing.");
		return "-1";
	}

	public int GetLevelForXp(int xp, int bonus_level_tier = 0)
	{
		int num = -1;
		for (int num2 = MaxLevel; num2 > 0; num2--)
		{
			if (xp >= xpNeededForLevel[num2])
			{
				num = num2;
				break;
			}
		}
		if (num == -1)
		{
			CspUtils.DebugLog("Asked to determine level for XP <" + xp + "> that did not land in any level bracket!  Failing.");
			return -1;
		}
		if (bonus_level_tier == 0 && num >= 11)
		{
			return 11;
		}
		if (bonus_level_tier == 1 && num >= 21)
		{
			return 20;
		}
		if (bonus_level_tier == 2 && num >= 40)
		{
			return 40;
		}
		return num;
	}

	public string GetFormattedLevel(int level)
	{
		return (level != MaxLevel) ? level.ToString() : "#Max";
	}

	public string GetLevelDescriptionForXp(int xp, int bonus_level_tier = 0)
	{
		int levelForXp = GetLevelForXp(xp, bonus_level_tier);
		if (levelForXp == -1)
		{
			CspUtils.DebugLog("Asked to determine level for XP <" + xp + "> that did not land in any level bracket!  Failing.");
			return "-1";
		}
		if (levelForXp >= MaxLevel)
		{
			return "#Max";
		}
		return string.Empty + levelForXp;
	}

	public int GetXpNeededForNextLevel(int level, int currentXp)
	{
		if (level > MaxLevel)
		{
			return int.MaxValue;
		}
		if (level > 0)
		{
			return xpNeededForLevel[level + 1] - currentXp;
		}
		CspUtils.DebugLog("Given an invalid level <" + level + "> when asked to compute the XP to the next level.  Failing.");
		return -1;
	}

	public int GetXpNeededForNextLevel(int currentXp)
	{
		int levelForXp = GetLevelForXp(currentXp);
		return GetXpNeededForNextLevel(levelForXp, currentXp);
	}

	public static void GetExp(UserProfile profile, out int expCur, out int expNext, out bool max, string heroName)
	{
		expCur = 0;
		expNext = 0;
		max = false;
		if (!profile.AvailableCostumes.ContainsKey(heroName))
		{
			return;
		}
		HeroPersisted heroPersisted = profile.AvailableCostumes[heroName];
		if (heroPersisted.Level >= heroPersisted.MaxLevel)
		{
			expNext = int.MaxValue;
			max = true;
			return;
		}
		int level = heroPersisted.Level;
		int xp = heroPersisted.Xp;
		int xpForLevel = Instance.GetXpForLevel(level + 1);
		int xpForLevel2 = Instance.GetXpForLevel(level);
		expCur = xp - xpForLevel2;
		if (xpForLevel == int.MaxValue)
		{
			expNext = xpForLevel;
			max = true;
		}
		else
		{
			expNext = xpForLevel - xpForLevel2;
		}
	}

	public static void GetExp(out int expCur, out int expNext, out bool max, string heroName)
	{
		GetExp(AppShell.Instance.Profile, out expCur, out expNext, out max, heroName);
	}

	public static string GetExpText(UserProfile profile, string heroName)
	{
		int expCur;
		int expNext;
		bool max;
		GetExp(profile, out expCur, out expNext, out max, heroName);
		if (expNext == int.MaxValue)
		{
			return "#Max_Exp";
		}
		return string.Format(AppShell.Instance.stringTable["#airlock_hero_experience"], expCur, expNext);
	}

	public static string GetExpText(string heroName)
	{
		return GetExpText(AppShell.Instance.Profile, heroName);
	}

	public static string GetLevelText(UserProfile profile, string heroName)
	{
		if (profile.AvailableCostumes.ContainsKey(heroName))
		{
			HeroPersisted heroPersisted = profile.AvailableCostumes[heroName];
			if (heroPersisted.Level == heroPersisted.MaxLevel)
			{
				return string.Format(AppShell.Instance.stringTable["#airlock_hero_level"], AppShell.Instance.stringTable["#Max"]);
			}
			return string.Format(AppShell.Instance.stringTable["#airlock_hero_level"], heroPersisted.Level);
		}
		return string.Empty;
	}

	public static string GetLevelText(string heroName)
	{
		return GetLevelText(AppShell.Instance.Profile, heroName);
	}
}
