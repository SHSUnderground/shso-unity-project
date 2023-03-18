public class ContributingFactor
{
	public enum FactorType
	{
		PlayerScore,
		PlayTime,
		BattleBonus,
		Unknown
	}

	public readonly FactorType Type;

	public readonly string Name;

	public readonly string Value;

	public readonly int ScoreAdjustment;

	public readonly string IconName;

	public ContributingFactor(FactorType type, string name, string value, int scoreAdjustment, string iconName)
	{
		Type = type;
		Name = name;
		Value = value;
		ScoreAdjustment = scoreAdjustment;
		IconName = iconName;
	}
}
