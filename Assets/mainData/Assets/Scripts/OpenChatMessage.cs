public class OpenChatMessage : ShsEventMessage
{
	public enum MessageStyle
	{
		Self,
		NonSub,
		Sub,
		Friend,
		System
	}

	public MessageStyle messageStyle;

	public string message;

	public string sendingPlayerName;

	public long sendingPlayerId;

	public OpenChatMessage(MessageStyle messageStyle, string message, string sendingPlayerName, long sendingPlayerId)
	{
		this.messageStyle = messageStyle;
		this.message = message;
		this.sendingPlayerName = sendingPlayerName;
		this.sendingPlayerId = sendingPlayerId;
	}
}
