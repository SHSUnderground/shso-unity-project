public class VerifyPlayerNPCRatio : AutomationCmd
{
	public int users;

	public int npcs;

	public int pg;

	private IServerConnection conn;

	public VerifyPlayerNPCRatio(string cmdline)
		: base(cmdline)
	{
		AutomationManager.Instance.nGameWorld++;
	}

	public override bool isReady()
	{
		bool flag = base.isReady();
		if (flag)
		{
			conn = AppShell.Instance.ServerConnection;
			users = conn.GetGameUserCount();
			npcs = AutomationManager.Instance.GetNPCCount();
			pg = AutomationManager.Instance.GetPCount();
		}
		return flag;
	}

	public override bool execute()
	{
		return base.execute();
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		string text = string.Empty;
		if (flag)
		{
			if (users < 10)
			{
				flag = (npcs == 4 && pg == 21);
				text = "npc=4,pg=21";
			}
			else if (users >= 10 && users < 15)
			{
				flag = (npcs == 3 && pg == 14);
				text = "npc=3,pg=14";
			}
			else if (users >= 15 && users < 20)
			{
				flag = (npcs == 2 && pg == 10);
				text = "npc=2,pg=10";
			}
			else if (users >= 20)
			{
				flag = (npcs == 1 && pg == 5);
				text = "npc=1,pg=5";
			}
			else
			{
				base.ErrorMsg = "It should be impossible to get this message. If you got it, something is eriously wrong with this code";
				base.ErrorCode = "CXX1";
			}
		}
		if (!flag)
		{
			base.ErrorMsg = "Actual: Users[" + users + "] NPC[" + npcs + "] Pg[" + pg + "] Expected: " + text;
			base.ErrorCode = "C001";
		}
		else
		{
			base.ErrorMsg = "Actual: Users[" + users + "] NPC[" + npcs + "] Pg[" + pg + "] Expected: " + text;
		}
		CspUtils.DebugLog("users: " + users + " : " + text);
		return flag;
	}
}
