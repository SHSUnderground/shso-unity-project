public class CommentCmd : AutomationCmd
{
	private string msg;

	public CommentCmd(string cmdline)
		: base(cmdline)
	{
		AutomationManager.Instance.nOther++;
		msg = cmdline.Substring(6);
	}

	public override bool execute()
	{
		bool result = base.execute();
		base.ErrorMsg = msg;
		return result;
	}
}
