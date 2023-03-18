public class CallMethodOnBehavior : ShsEventMessage
{
	private string methodName;

	public string MethodName
	{
		get
		{
			return methodName;
		}
		set
		{
			methodName = value;
		}
	}

	public CallMethodOnBehavior(string methodName)
	{
		MethodName = methodName;
	}
}
