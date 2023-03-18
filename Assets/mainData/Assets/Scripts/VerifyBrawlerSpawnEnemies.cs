public class VerifyBrawlerSpawnEnemies : AutomationCmd
{
	public VerifyBrawlerSpawnEnemies(string cmdline)
		: base(cmdline)
	{
	}

	public override bool isCompleted()
	{
		bool result = base.execute();
		try
		{
			string str = (AutomationManager.Instance.spawnObjCount != 1) ? AutomationManager.Instance.spawnObjCount.ToString() : "none";
			base.ErrorMsg = "Enemies Spawned: " + str;
			return result;
		}
		catch
		{
			CspUtils.DebugLog("No Enemies Detected!");
			return result;
		}
	}
}
