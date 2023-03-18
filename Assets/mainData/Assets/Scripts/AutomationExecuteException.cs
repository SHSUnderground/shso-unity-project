using System;

[Serializable]
public class AutomationExecuteException : Exception
{
	public AutomationExecuteException()
	{
	}

	public AutomationExecuteException(string s)
		: base(s)
	{
	}

	public AutomationExecuteException(string s, Exception e)
		: base(s, e)
	{
	}
}
