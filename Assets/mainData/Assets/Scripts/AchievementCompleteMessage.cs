public class AchievementCompleteMessage : ShsEventMessage
{
	public int achievementID;

	public AchievementCompleteMessage(int achievementID)
	{
		this.achievementID = achievementID;
	}
}
