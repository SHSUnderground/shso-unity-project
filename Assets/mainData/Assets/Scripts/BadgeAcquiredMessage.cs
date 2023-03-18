public class BadgeAcquiredMessage : ShsEventMessage
{
	public readonly int badgeOwnableTypeID;

	public readonly string heroName;

	public BadgeAcquiredMessage(int badgeOwnableTypeID, string heroName)
	{
		this.badgeOwnableTypeID = badgeOwnableTypeID;
		this.heroName = heroName;
	}
}
