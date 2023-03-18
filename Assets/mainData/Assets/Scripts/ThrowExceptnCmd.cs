public class ThrowExceptnCmd : AutomationCmd
{
	public string excptype;

	public ThrowExceptnCmd(string cmdline, string excptn)
		: base(cmdline)
	{
		excptype = excptn;
	}

	public override bool execute()
	{
		if (excptype == "timeout")
		{
			throw new AutomationTimeOutException();
		}
		throw new AutomationExecuteException();
	}
}
