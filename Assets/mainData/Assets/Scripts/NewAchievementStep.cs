public class NewAchievementStep
{
	public int achievementID;

	public int achievementStepID;

	public string readableName;

	public string type;

	public string displayName;

	public string displayDesc;

	public string eventType;

	public string subEventType;

	public string heroName;

	public int enabled;

	public int hidden;

	public int threshold;

	public int int1;

	public int int2;

	public int int3;

	public int int4;

	public string str1;

	public string str2;

	public string str3;

	public string str4;

	public string helpURL;

	public NewAchievementStep(AchievementStepJsonData data)
	{
		achievementID = data.aid;
		achievementStepID = data.id;
		type = data.t;
		displayName = data.dn;
		displayDesc = data.dd;
		eventType = data.et;
		subEventType = data.set;
		heroName = data.hn;
		enabled = data.e;
		hidden = data.h;
		threshold = data.th;
		int1 = data.i1;
		int2 = data.i2;
		int3 = data.i3;
		int4 = data.i4;
		str1 = data.s1;
		str2 = data.s2;
		str3 = data.s3;
		str4 = data.s4;
		helpURL = data.help;
	}
}
