public class AddTicketsCmd : AutomationCmd
{
	private int tickets;

	private int servertickets;

	public AddTicketsCmd(string cmdline, int tic)
		: base(cmdline)
	{
		tickets = tic;
		servertickets = 0;
		AutomationManager.Instance.nOther++;
	}

	public override bool execute()
	{
		try
		{
			AppShell.Instance.Profile.StartInventoryFetch();
			AppShell.Instance.Profile.StartCurrencyFetch();
			servertickets = AppShell.Instance.Profile.Tickets;
			AppShell.Instance.EventReporter.ReportAwardTokens(tickets);
		}
		catch
		{
			AutomationManager.Instance.errOther++;
			base.ErrorCode = "E001";
			base.ErrorMsg = "Failed to Add Tickets to the Profile";
		}
		return base.execute();
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		int num = tickets + servertickets;
		try
		{
			AppShell.Instance.Profile.StartInventoryFetch();
			AppShell.Instance.Profile.StartCurrencyFetch();
		}
		catch
		{
			AutomationManager.Instance.errOther++;
			base.ErrorCode = "C001";
			base.ErrorMsg = "Error: Fatching Inventory or Currency from Server has failed";
		}
		if (flag && num != AppShell.Instance.Profile.Tickets)
		{
			AutomationManager.Instance.errOther++;
			flag = false;
			base.ErrorCode = "C001";
			base.ErrorMsg = "Error: Tickets do not match the server ticket count";
		}
		return flag;
	}
}
