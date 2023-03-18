public class MotdDefinition
{
	public static readonly MotdDefinition Instance = new MotdDefinition();

	private string ownableTypeId;

	public string MissionOwnableTypeId
	{
		get
		{
			if (string.IsNullOrEmpty(ownableTypeId))
			{
				CspUtils.DebugLog("No mission of the day found!");
			}
			return ownableTypeId;
		}
		set
		{
			ownableTypeId = value;
		}
	}
}
