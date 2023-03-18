public class AchievementReward
{
	public int achievementRewardID;

	public int gold;

	public int fractals;

	public int xp;

	public int ownableID1;

	public int ownableID1Quantity;

	public int ownableID2;

	public int ownableID2Quantity;

	public AchievementReward(AchievementRewardJsonData data)
	{
		achievementRewardID = data.achievement_reward_id;
		gold = data.gold;
		fractals = data.fractals;
		xp = data.xp;
		ownableID1 = data.ownable_id_1;
		ownableID1Quantity = data.ownable_id_1_quantity;
		ownableID2 = data.ownable_id_2;
		ownableID2Quantity = data.ownable_id_2_quantity;
	}
}
