public class GameWorldMenuChatEmoteMessage : ShsEventMessage
{
	public string command;

	public GameWorldMenuChatEmoteMessage(string command)
	{
		this.command = command;
	}
}
