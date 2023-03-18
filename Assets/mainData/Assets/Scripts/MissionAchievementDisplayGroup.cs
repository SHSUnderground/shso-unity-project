using System.Collections.Generic;

public class MissionAchievementDisplayGroup : AchievementDisplayGroup
{
	private static int nextGroupID = 300000;

	private static Dictionary<string, AchievementDisplayGroup> _missionNameToGroupMap = new Dictionary<string, AchievementDisplayGroup>();

	public MissionAchievementDisplayGroup(int groupID, int parentGroupID, string name, string desc)
		: base(groupID, parentGroupID, name, desc)
	{
	}

	public AchievementDisplayGroup addGroupForMission(string bossName)
	{
		AchievementDisplayGroup achievementDisplayGroup = new AchievementDisplayGroup(nextGroupID++, groupID, "#CIN_" + bossName + "_EXNM", bossName + " desc");
		_missionNameToGroupMap.Add(bossName, achievementDisplayGroup);
		AchievementManager.allGroups.Add(achievementDisplayGroup.groupID, achievementDisplayGroup);
		addChildGroup(achievementDisplayGroup);
		return achievementDisplayGroup;
	}

	public override void addAchievement(NewAchievement achievement)
	{
		if (achievement.displayGroupID != 3)
		{
			CspUtils.DebugLog("MissionAchievementDisplayGroup got an achievement with a groupID != 3 " + achievement.achievementID + " " + achievement.displayGroupID);
			base.addAchievement(achievement);
			return;
		}
		OwnableDefinition missionDef = OwnableDefinition.getMissionDef(achievement.metadata);
		if (_missionNameToGroupMap.ContainsKey(missionDef.metadata))
		{
			AchievementDisplayGroup achievementDisplayGroup = _missionNameToGroupMap[missionDef.metadata];
			achievement.displayGroupID = achievementDisplayGroup.groupID;
			achievementDisplayGroup.addAchievement(achievement);
		}
	}
}
