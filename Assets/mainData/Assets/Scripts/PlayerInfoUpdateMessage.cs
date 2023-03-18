public class PlayerInfoUpdateMessage : ShsEventMessage
{
	public int userId;

	public PlayerDictionary.Player player;

	public PlayerInfoUpdateMessage(int userId, PlayerDictionary.Player player)
	{
		this.userId = userId;
		this.player = player;
	}
}
