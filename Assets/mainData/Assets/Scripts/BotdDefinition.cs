public class BotdDefinition
{
	public static readonly BotdDefinition Instance = new BotdDefinition();

	private string ownableTypeId;

	public string BattleOwnableTypeId
	{
		get
		{
			if (string.IsNullOrEmpty(ownableTypeId))
			{
				CspUtils.DebugLog("No battle of the day found!");
			}
			return ownableTypeId;
		}
		set
		{
			ownableTypeId = value;
		}
	}
}
