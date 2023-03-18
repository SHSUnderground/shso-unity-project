public class LoginCompleteMessage : ShsEventMessage
{
	public enum LoginStatus
	{
		LoginSucceeded,
		LoginFailed
	}

	public readonly LoginStatus status;

	public readonly string message;

	public readonly string profile;

	public LoginCompleteMessage(LoginStatus Status, string Message, string Profile)
	{
		status = Status;
		message = Message;
		profile = Profile;
	}
}
