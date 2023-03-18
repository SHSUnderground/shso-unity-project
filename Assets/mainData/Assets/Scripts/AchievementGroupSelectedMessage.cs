public class AchievementGroupSelectedMessage : ShsEventMessage
{
	public int groupID;

	public AchievementGroupSelectedMessage(int groupID)
	{
		this.groupID = groupID;
	}
}
