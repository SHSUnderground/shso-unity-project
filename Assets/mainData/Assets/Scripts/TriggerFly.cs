public class TriggerFly : AutomationCmd
{
	private string iObject;

	public TriggerFly(string cmdline)
		: base(cmdline)
	{
		iObject = cmdline.Substring("triggerfly".Length).Trim();
	}

	public override bool isReady()
	{
		return base.isReady();
	}

	public override bool execute()
	{
		AutomationBehavior.Instance.fly(iObject);
		return base.execute();
	}

	public override bool isCompleted()
	{
		return base.isCompleted();
	}
}
