using System.Collections.Generic;

public class AchievementDisplayGroup
{
	public string name;

	public string desc;

	public int groupID;

	public int parentGroupID;

	public bool restricted;

	public List<NewAchievement> achievements = new List<NewAchievement>();

	public List<AchievementDisplayGroup> childGroups = new List<AchievementDisplayGroup>();

	public AchievementDisplayGroup(int groupID, int parentGroupID, string name, string desc)
	{
		this.groupID = groupID;
		this.parentGroupID = parentGroupID;
		this.name = name;
		this.desc = desc;
	}

	public void addChildGroup(AchievementDisplayGroup group)
	{
		childGroups.Add(group);
	}

	public virtual void addAchievement(NewAchievement achievement)
	{
		achievements.Add(achievement);
	}
}
