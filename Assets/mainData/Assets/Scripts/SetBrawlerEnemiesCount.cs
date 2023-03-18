using System;

public class SetBrawlerEnemiesCount : AutomationCmd
{
	private string enemycount;

	public SetBrawlerEnemiesCount(string cmdline, string enemycount)
		: base(cmdline)
	{
		this.enemycount = enemycount;
	}

	public override bool execute()
	{
		BrawlerController.Instance.debugPlayerCount = Convert.ToInt32(enemycount);
		base.ErrorMsg = "Enemies Count set to: " + enemycount;
		return base.execute();
	}
}
