public class SendGameMessageMessage : ShsEventMessage
{
	private string message;

	public string Message
	{
		get
		{
			return message;
		}
		set
		{
			message = value;
		}
	}

	public SendGameMessageMessage(string message)
	{
		Message = message;
	}
}
