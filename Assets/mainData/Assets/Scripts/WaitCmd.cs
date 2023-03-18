public class WaitCmd : AutomationCmd
{
	public int delay;

	private bool addstep;

	public WaitCmd(string cmdline, int waitTime)
		: base(cmdline)
	{
		delay = waitTime;
		addstep = true;
		AutomationManager.Instance.nOther++;
	}

	public override bool isCompleted()
	{
		if (addstep)
		{
			addstep = false;
			base.isCompleted();
			return false;
		}
		return true;
	}
}
