public interface IAchievement
{
	string Id
	{
		get;
	}

	string Name
	{
		get;
	}

	Achievement.AchievementTypeEnum Type
	{
		get;
	}

	Achievement.AchievementSourceEnum Source
	{
		get;
	}

	AchievementsManager Manager
	{
		get;
		set;
	}

	int[] Thresholds
	{
		get;
	}

	void Reset();

	void Initialize(object initData);

	long GetLevelThreshold(Achievement.AchievementLevelEnum level);

	void ProcessSourceUpdate(object sourceData);

	void OnSourceUpdated(object sourceData);

	void DebugSetSourceValue(string heroKey, long value);

	Achievement.AchievementLevelEnum GetLevel(string heroKey, object extraData);

	Achievement.AchievementLevelEnum GetLevel(string heroKey);

	long GetCharacterLevelValue(string heroKey, object extraData);

	long GetCharacterLevelValue(string heroKey);
}
