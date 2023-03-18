public class KillAll : AutomationCmd
{
	public KillAll(string cmdline)
		: base(cmdline)
	{
	}

	public override bool execute()
	{
		try
		{
			AutomationBrawler.instance.DefeatActiveEnemies();
		}
		catch
		{
			CspUtils.DebugLog("No Enemies to Kill!");
		}
		return base.execute();
	}
}
