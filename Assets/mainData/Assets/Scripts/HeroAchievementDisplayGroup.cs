using System.Collections.Generic;

public class HeroAchievementDisplayGroup : AchievementDisplayGroup
{
	private static int nextGroupID = 400000;

	private static Dictionary<string, AchievementDisplayGroup> _heroNameToGroupMap = new Dictionary<string, AchievementDisplayGroup>();

	public HeroAchievementDisplayGroup(int groupID, int parentGroupID, string name, string desc)
		: base(groupID, parentGroupID, name, desc)
	{
	}

	public AchievementDisplayGroup addGroupForHero(string hero)
	{
		AchievementDisplayGroup achievementDisplayGroup = new AchievementDisplayGroup(nextGroupID++, groupID, AppShell.Instance.CharacterDescriptionManager[hero].CharacterName, hero + " desc");
		_heroNameToGroupMap.Add(hero, achievementDisplayGroup);
		AchievementManager.allGroups.Add(achievementDisplayGroup.groupID, achievementDisplayGroup);
		addChildGroup(achievementDisplayGroup);
		return achievementDisplayGroup;
	}

	public override void addAchievement(NewAchievement achievement)
	{
		if (!_heroNameToGroupMap.ContainsKey(achievement.metadata))
		{
			CspUtils.DebugLog("HeroAchievementDisplayGroup.addAchievement got bad heroName " + achievement.metadata + " for achievement ID " + achievement.achievementID);
			return;
		}
		AchievementDisplayGroup achievementDisplayGroup = _heroNameToGroupMap[achievement.metadata];
		achievement.displayGroupID = achievementDisplayGroup.groupID;
		achievementDisplayGroup.addAchievement(achievement);
	}
}
