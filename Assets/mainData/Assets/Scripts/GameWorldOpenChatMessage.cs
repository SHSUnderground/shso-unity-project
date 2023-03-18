public class GameWorldOpenChatMessage : ShsEventMessage
{
	public enum ChatMessageTypeEnum
	{
		PlayerChat,
		Moderator,
		Filter
	}

	public string chatMessage;

	public long sendingPlayerId;

	public ChatMessageTypeEnum messageType;

	public GameWorldOpenChatMessage(string message, long playerId)
		: this(message, playerId, ChatMessageTypeEnum.PlayerChat)
	{
	}

	public GameWorldOpenChatMessage(string message, long playerId, ChatMessageTypeEnum messageType)
	{
		chatMessage = message;
		sendingPlayerId = playerId;
		this.messageType = messageType;
	}
}
