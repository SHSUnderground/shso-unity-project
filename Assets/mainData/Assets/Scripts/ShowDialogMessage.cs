internal class ShowDialogMessage : ShsEventMessage
{
	public enum DialogTypeEnum
	{
		Common,
		Code,
		System,
		SystemMenu
	}

	public delegate void MessageResponseCallback(ShowDialogReplyMessage response);

	public readonly DialogTypeEnum DialogType;

	public readonly string Message;

	public readonly MessageResponseCallback Callback;

	public ShowDialogMessage(DialogTypeEnum Type)
		: this(Type, string.Empty, null)
	{
	}

	public ShowDialogMessage(string Message)
		: this(DialogTypeEnum.Common, Message, null)
	{
	}

	public ShowDialogMessage(DialogTypeEnum Type, string Message, MessageResponseCallback Callback)
	{
		DialogType = Type;
		this.Message = Message;
		this.Callback = Callback;
	}
}
