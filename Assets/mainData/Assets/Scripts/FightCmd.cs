public class FightCmd : AutomationCmd
{
	public FightCmd(string cmdline, bool OnOff)
		: base(cmdline)
	{
		AutomationManager.Instance.inFightMode = OnOff;
		AutomationManager.Instance.nBrawler++;
	}
}
