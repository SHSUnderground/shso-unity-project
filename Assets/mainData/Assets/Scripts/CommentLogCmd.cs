public class CommentLogCmd : AutomationCmd
{
	private string msg;

	public CommentLogCmd(string cmdline)
		: base(cmdline)
	{
		AutomationManager.Instance.nOther++;
		msg = cmdline.Substring(6);
	}

	public override bool execute()
	{
		base.ErrorMsg = msg;
		return base.execute();
	}
}
