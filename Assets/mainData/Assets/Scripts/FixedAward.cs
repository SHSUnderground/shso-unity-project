public class FixedAward
{
	public enum AwardType
	{
		XP,
		Coins,
		Tickets,
		LevelUp,
		Unknown
	}

	public readonly AwardType Type;

	public readonly string Name;

	public readonly string IconName;

	public readonly int Value;

	public FixedAward(AwardType type, string name, string iconName)
		: this(type, name, iconName, 0)
	{
	}

	public FixedAward(AwardType type, string name, string iconName, int value)
	{
		Type = type;
		Name = name;
		IconName = iconName;
		Value = value;
	}
}
