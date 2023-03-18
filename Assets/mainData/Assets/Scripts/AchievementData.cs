using System;

public class AchievementData
{
	public int playerID;

	public int achievementID;

	public int progress;

	public int complete;

	public int ack;

	public DateTime completeDate;

	public AchievementData(AchievementDataJsonData data)
	{
		playerID = data.pid;
		achievementID = data.aid;
		progress = data.p;
		complete = data.c;
		ack = data.a;
		completeDate = Convert.ToDateTime(data.cd);
	}

	public void absorb(AchievementData data)
	{
		progress = data.progress;
		complete = data.complete;
		ack = data.ack;
		completeDate = data.completeDate;
	}
}
