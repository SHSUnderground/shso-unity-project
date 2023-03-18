public class AchievementCompleteHideMessage : ShsEventMessage
{
	public int achievementID;

	public AchievementCompleteHideMessage(int achievementID)
	{
		this.achievementID = achievementID;
	}
}
