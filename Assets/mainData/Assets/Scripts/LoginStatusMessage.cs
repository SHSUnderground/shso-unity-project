public class LoginStatusMessage : ShsEventMessage
{
	private readonly LoginStatusNotifier.LoginStep step;

	private readonly string message;

	public LoginStatusNotifier.LoginStep LastStep
	{
		get
		{
			return step;
		}
	}

	public string Message
	{
		get
		{
			return message;
		}
	}

	public LoginStatusMessage(LoginStatusNotifier.LoginStep step, string message)
	{
		this.step = step;
		this.message = message;
	}
}
