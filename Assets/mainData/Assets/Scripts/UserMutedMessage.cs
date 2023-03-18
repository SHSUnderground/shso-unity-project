public class UserMutedMessage : ShsEventMessage
{
	public int Duration;

	public string Message;

	public UserMutedMessage(string message, string duration)
	{
		Message = message;
		if (!int.TryParse(duration, out Duration))
		{
			CspUtils.DebugLog("User Muted message received: " + duration + " when expecting an integer value.");
		}
	}
}
