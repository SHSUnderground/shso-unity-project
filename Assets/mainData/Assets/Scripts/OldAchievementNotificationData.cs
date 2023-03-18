public class OldAchievementNotificationData : NotificationData
{
	public Achievement achievement;

	public Achievement.AchievementLevelEnum prevLevel;

	public Achievement.AchievementLevelEnum newLevel;

	public string heroKey;

	public OldAchievementNotificationData(Achievement achievement, Achievement.AchievementLevelEnum prevLevel, Achievement.AchievementLevelEnum newLevel, string heroKey)
		: base(NotificationType.OldAchievement, NotificationOrientation.Right)
	{
		this.achievement = achievement;
		this.prevLevel = prevLevel;
		this.newLevel = newLevel;
		this.heroKey = heroKey;
	}
}
