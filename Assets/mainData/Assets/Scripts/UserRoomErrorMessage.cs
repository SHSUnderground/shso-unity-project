using System.Collections;

public class UserRoomErrorMessage : ShsEventMessage
{
	public string errorMessage;

	public string errorCode;

	public Hashtable arguments;

	public UserRoomErrorMessage(string message, string code, Hashtable arguments)
	{
		errorMessage = message;
		errorCode = code;
		this.arguments = arguments;
	}
}
