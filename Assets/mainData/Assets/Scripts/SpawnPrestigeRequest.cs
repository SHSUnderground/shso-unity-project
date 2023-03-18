public class SpawnPrestigeRequest : ShsEventMessage
{
	public readonly int UserID;

	public SpawnPrestigeRequest(int userID)
	{
		UserID = userID;
	}
}
