using System.Collections.Generic;

public class NewAchievement
{
	public int achievementID;

	public string readableName;

	public string type;

	public string eventType;

	public string displayName;

	public string displayDesc;

	public string displayShortDesc;

	public int enabled;

	public int hidden;

	public int displayGroupID;

	public int rewardID;

	public int achievementPoints;

	public int prereqAchievementID;

	public int difficulty;

	public string helpURL;

	public string metadata;

	public string hintIcon = string.Empty;

	public string jumpAction = string.Empty;

	public AchievementManager.DestinyTracks track = AchievementManager.DestinyTracks.None;

	public List<NewAchievementStep> steps = new List<NewAchievementStep>();

	public NewAchievement(AchievementJsonData data)
	{
		achievementID = data.id;
		type = data.t;
		displayName = data.dn;
		displayDesc = data.dd;
		displayShortDesc = data.dsd;
		enabled = data.e;
		hidden = data.h;
		displayGroupID = data.dg;
		rewardID = data.r;
		achievementPoints = data.ap;
		prereqAchievementID = data.req;
		difficulty = data.dif;
		helpURL = data.help;
		metadata = data.met;
		if (data.hi != null)
		{
			hintIcon = data.hi;
		}
		if (data.ja != null)
		{
			jumpAction = data.ja;
		}
	}

	public void addStep(NewAchievementStep step)
	{
		if (step.hidden != 1)
		{
			steps.Add(step);
		}
	}
}
