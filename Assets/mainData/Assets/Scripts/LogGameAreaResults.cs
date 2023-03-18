public class LogGameAreaResults : AutomationCmd
{
	public LogGameAreaResults(string cmdline)
		: base(cmdline)
	{
	}

	public override bool isCompleted()
	{
		bool result = base.isCompleted();
		base.ErrorMsg = "B:" + AutomationManager.Instance.errBrawler + " C:" + AutomationManager.Instance.errCardGame + " G:" + AutomationManager.Instance.errGameWorld + " H:" + AutomationManager.Instance.errHeadQuareter + " O:" + AutomationManager.Instance.errOther;
		AutomationManager.Instance.LogStatistics(AutomationManager.Instance.errGameWorld, AutomationManager.Instance.errHeadQuareter, AutomationManager.Instance.errBrawler, AutomationManager.Instance.errCardGame, AutomationManager.Instance.errOther);
		return result;
	}
}
