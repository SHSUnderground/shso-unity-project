public class CloseUICmd : AutomationCmd
{
	private string targetWindow;

	private bool abortFlow;

	public CloseUICmd(string cmdline)
		: base(cmdline)
	{
	}

	public CloseUICmd(string cmdline, string target)
		: base(cmdline)
	{
		targetWindow = target;
		abortFlow = true;
		AutomationManager.Instance.nOther++;
	}

	public CloseUICmd(string cmdline, string target, bool abort)
		: base(cmdline)
	{
		targetWindow = target;
		abortFlow = abort;
		AutomationManager.Instance.nOther++;
	}

	public override bool execute()
	{
		AppShell.Instance.EventMgr.Fire(this, new GUIAutoCloseWindowMessage(targetWindow, abortFlow));
		return base.execute();
	}
}
