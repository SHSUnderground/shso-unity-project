public class StopTrackingAchievementMessage : ShsEventMessage
{
	public int achievementID;

	public StopTrackingAchievementMessage(int achievementID)
	{
		this.achievementID = achievementID;
	}
}
