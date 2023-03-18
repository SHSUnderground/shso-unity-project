public class UserAdminMessage : ShsEventMessage
{
	public string Message;

	public UserAdminMessage(string message)
	{
		Message = message;
	}
}
