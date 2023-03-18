public class LogCardGameResults : AutomationCmd
{
	public LogCardGameResults(string cmdline)
		: base(cmdline)
	{
	}

	public override bool isCompleted()
	{
		bool result = base.isCompleted();
		base.ErrorMsg = "W:" + AutomationManager.Instance.matchWon + " L:" + AutomationManager.Instance.matchLost + " I:" + AutomationManager.Instance.matchIncomplete;
		AutomationManager.Instance.LogCardInfoToFile("," + base.ErrorMsg);
		return result;
	}
}
