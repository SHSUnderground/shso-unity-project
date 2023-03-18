public class SnapCmd : AutomationCmd
{
	private string snapTag;

	public SnapCmd(string cmdline, string tag)
		: base(cmdline)
	{
		snapTag = tag;
		AutomationManager.Instance.nBrawler++;
	}

	public override bool execute()
	{
		AutomationBrawler.instance.snap(snapTag);
		return base.execute();
	}
}
