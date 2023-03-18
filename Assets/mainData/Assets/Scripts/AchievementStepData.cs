using System;

public class AchievementStepData
{
	public int playerID;

	public int achievementStepID;

	public int progress;

	public int complete;

	public DateTime completeDate;

	public AchievementStepData(AchievementStepDataJsonData data)
	{
		playerID = data.pid;
		achievementStepID = data.asid;
		progress = data.p;
		complete = data.c;
		completeDate = Convert.ToDateTime(data.cd);
	}

	public void absorb(AchievementStepData data)
	{
		progress = data.progress;
		complete = data.complete;
		completeDate = data.completeDate;
	}
}
