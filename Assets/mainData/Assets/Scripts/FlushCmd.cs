using System;

public class FlushCmd : AutomationCmd
{
	public FlushCmd(string cmdline)
		: base(cmdline)
	{
	}

	public override bool execute()
	{
		try
		{
			AutomationManager.Instance.FlushLogging();
		}
		catch (Exception ex)
		{
			AutomationManager.Instance.errOther++;
			base.ErrorCode = "E001";
			base.ErrorMsg += ex.Message;
		}
		return base.execute();
	}
}
