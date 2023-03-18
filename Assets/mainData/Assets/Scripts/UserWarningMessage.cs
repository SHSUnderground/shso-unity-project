public class UserWarningMessage : ShsEventMessage
{
	public string warningMessage;

	public SHSErrorCodes.Code warningType;

	public UserWarningMessage(string message)
		: this(message, SHSErrorCodes.Code.ModeratorWarning)
	{
	}

	public UserWarningMessage(string message, SHSErrorCodes.Code warningType)
	{
		warningMessage = message;
		this.warningType = warningType;
	}
}
