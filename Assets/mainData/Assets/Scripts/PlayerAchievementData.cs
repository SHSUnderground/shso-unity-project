using System.Collections.Generic;

public class PlayerAchievementData
{
	private Dictionary<int, AchievementData> allAchievementData = new Dictionary<int, AchievementData>();

	private Dictionary<int, AchievementStepData> allAchievementStepData = new Dictionary<int, AchievementStepData>();

	private Dictionary<int, AchievementStepDataEx> allAchievementStepDataEx = new Dictionary<int, AchievementStepDataEx>();

	public int playerID;

	public int achievementPoints;

	public int totalCompletedAchievements;

	public Dictionary<int, int> totalAchievementsCompletedByGroup = new Dictionary<int, int>();

	public PlayerAchievementData(int playerID)
	{
		this.playerID = playerID;
		totalAchievementsCompletedByGroup.Add(0, 0);
		foreach (AchievementDisplayGroup value in AchievementManager.allGroups.Values)
		{
			totalAchievementsCompletedByGroup.Add(value.groupID, 0);
		}
	}

	public bool achievementComplete(int achievementID)
	{
		if (!allAchievementData.ContainsKey(achievementID))
		{
			return false;
		}
		return allAchievementData[achievementID].complete == 1;
	}

	public void addData(AchievementData data)
	{
		bool flag = true;
		if (allAchievementData.ContainsKey(data.achievementID))
		{
			flag = (allAchievementData[data.achievementID].complete == 0);
			allAchievementData[data.achievementID].absorb(data);
		}
		else
		{
			allAchievementData.Add(data.achievementID, data);
		}
		if (!AchievementManager.allAchievements.ContainsKey(data.achievementID))
		{
			CspUtils.DebugLog("bad achievement data - invalid achievement id " + data.achievementID + string.Empty);
			return;
		}
		NewAchievement newAchievement = AchievementManager.allAchievements[data.achievementID];
		if (flag && data.complete == 1 && newAchievement.hidden != 1 && newAchievement.enabled == 1)
		{
			totalCompletedAchievements++;
			achievementPoints += newAchievement.achievementPoints;
			int rootGroupID = AchievementManager.getRootGroupID(newAchievement.displayGroupID);
			Dictionary<int, int> dictionary;
			Dictionary<int, int> dictionary2 = dictionary = totalAchievementsCompletedByGroup;
			int key;
			int key2 = key = rootGroupID;
			key = dictionary[key];
			dictionary2[key2] = key + 1;
			if (rootGroupID != newAchievement.displayGroupID)
			{
				Dictionary<int, int> dictionary3;
				Dictionary<int, int> dictionary4 = dictionary3 = totalAchievementsCompletedByGroup;
				int key3 = key = newAchievement.displayGroupID;
				key = dictionary3[key];
				dictionary4[key3] = key + 1;
			}
		}
	}

	public void addData(AchievementStepData data)
	{
		if (allAchievementStepData.ContainsKey(data.achievementStepID))
		{
			allAchievementStepData[data.achievementStepID].absorb(data);
		}
		else
		{
			allAchievementStepData.Add(data.achievementStepID, data);
		}
	}

	public void addData(AchievementStepDataEx data)
	{
		if (allAchievementStepDataEx.ContainsKey(data.achievementStepID))
		{
			allAchievementStepDataEx[data.achievementStepID].absorb(data);
		}
		else
		{
			allAchievementStepDataEx.Add(data.achievementStepID, data);
		}
	}

	public void reset()
	{
		allAchievementData = new Dictionary<int, AchievementData>();
		allAchievementStepData = new Dictionary<int, AchievementStepData>();
		allAchievementStepDataEx = new Dictionary<int, AchievementStepDataEx>();
	}

	public AchievementData getData(int achievementID)
	{
		if (allAchievementData.ContainsKey(achievementID))
		{
			return allAchievementData[achievementID];
		}
		return null;
	}

	public AchievementStepData getStepData(int stepID)
	{
		if (allAchievementStepData.ContainsKey(stepID))
		{
			return allAchievementStepData[stepID];
		}
		return null;
	}

	public AchievementStepDataEx getStepDataEx(int stepID)
	{
		if (allAchievementStepDataEx.ContainsKey(stepID))
		{
			return allAchievementStepDataEx[stepID];
		}
		return null;
	}

	public int getStepProgress(int stepID)
	{
		AchievementStepData stepData = getStepData(stepID);
		if (stepData == null)
		{
			return AchievementManager.INVALID_VALUE;
		}
		return stepData.progress;
	}
}
