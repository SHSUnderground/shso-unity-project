public class MissionResults
{
	public int tickets;

	public int earnedXp;

	public int bonusXp;

	public int coins;

	public string ownable;

	public bool levelUp;

	public string heroName;

	public int currentXp;

	public int rewardTier;

	public int enemyKoScore;

	public int survivalScore;

	public int gimmickScore;

	public int comboScore;

	public MissionResults()
	{
		tickets = 0;
		earnedXp = 0;
		bonusXp = 0;
		coins = 0;
		ownable = string.Empty;
		enemyKoScore = 0;
		survivalScore = 0;
		gimmickScore = 0;
		comboScore = 0;
		levelUp = false;
		heroName = "Unretrieved!";
	}
}
