using System;

public class AchievementStepDataEx
{
	public int playerID;

	public int achievementStepID;

	public DateTime date1;

	public int data1;

	public int data2;

	public string str1;

	public string str2;

	public AchievementStepDataEx(AchievementStepDataExJsonData data)
	{
		playerID = data.pid;
		achievementStepID = data.asid;
		date1 = Convert.ToDateTime(data.date1);
		data1 = data.data1;
		data2 = data.data2;
		str1 = data.str1;
		str2 = data.str2;
	}

	public void absorb(AchievementStepDataEx data)
	{
		date1 = data.date1;
		data1 = data.data1;
		data2 = data.data2;
		str1 = data.str1;
		str2 = data.str2;
	}
}
