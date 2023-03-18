using System;

[Serializable]
public class AutomationTimeOutException : Exception
{
	public AutomationTimeOutException()
	{
	}

	public AutomationTimeOutException(string s)
		: base(s)
	{
	}

	public AutomationTimeOutException(string s, Exception e)
		: base(s, e)
	{
	}
}
