using System;

public class BrawlerAcceptInviteCmd : AutomationCmd
{
	private const int INVITETIMEOUT = 200;

	public BrawlerAcceptInviteCmd(string cmdline)
		: base(cmdline)
	{
		AutomationManager.Instance.nBrawler++;
	}

	public override bool isReady()
	{
		bool flag = true;
		if (DateTime.Now - base.StartTime < TimeSpan.FromSeconds(200.0))
		{
			if (flag)
			{
				flag = AutomationBrawler.instance.AcceptInvite();
				isBrawlerMultiplayer = true;
			}
		}
		else
		{
			AutomationManager.Instance.errBrawler++;
			base.ErrorCode = "P001";
			base.ErrorMsg = "BrawlerAcceptInviteCmd has Timedout!";
		}
		return flag;
	}
}
