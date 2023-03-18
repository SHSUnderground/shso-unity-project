public class AchievementProgressUpdateMessage : ShsEventMessage
{
	public AchievementData data;

	public AchievementStepData stepData;

	public AchievementStepDataEx stepDataEx;

	public int achievementID;

	public AchievementProgressUpdateMessage(AchievementData data, AchievementStepData stepData, AchievementStepDataEx stepDataEx)
	{
		this.data = data;
		this.stepData = stepData;
		this.stepDataEx = stepDataEx;
		if (data != null)
		{
			achievementID = data.achievementID;
		}
		if (stepData != null)
		{
			achievementID = AchievementManager.getAchievementIDFromStepID(stepData.achievementStepID);
		}
		if (stepDataEx != null)
		{
			achievementID = AchievementManager.getAchievementIDFromStepID(stepDataEx.achievementStepID);
		}
	}
}
