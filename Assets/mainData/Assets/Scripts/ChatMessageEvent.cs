public class ChatMessageEvent : ShsEventMessage
{
	public enum ChatMessageTypeEnum
	{
		Bubble,
		FullText,
		System
	}

	public readonly ChatMessageTypeEnum ChatMessageType;

	public readonly string MessageText;

	public readonly sbyte MessageId = -1;

	public ChatMessageEvent(ChatMessageTypeEnum messageType, string message, sbyte id)
	{
		ChatMessageType = messageType;
		MessageText = message;
		MessageId = id;
	}

	public ChatMessageEvent(sbyte id)
	{
		ChatMessageType = ChatMessageTypeEnum.Bubble;
		MessageId = id;
	}

	public ChatMessageEvent(string message)
	{
		ChatMessageType = ChatMessageTypeEnum.FullText;
		MessageText = message;
	}
}
