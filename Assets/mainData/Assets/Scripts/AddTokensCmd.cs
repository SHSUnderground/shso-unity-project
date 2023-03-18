public class AddTokensCmd : AutomationCmd
{
	private int tokens;

	private int servertokens;

	public AddTokensCmd(string cmdline, int toc)
		: base(cmdline)
	{
		tokens = toc;
		AutomationManager.Instance.nOther++;
	}

	public override bool execute()
	{
		try
		{
			AppShell.Instance.Profile.StartInventoryFetch();
			AppShell.Instance.Profile.StartCurrencyFetch();
			servertokens = AppShell.Instance.Profile.Gold;
			AppShell.Instance.EventReporter.ReportAwardTokens(tokens);
		}
		catch
		{
			AutomationManager.Instance.errOther++;
			base.ErrorCode = "E001";
			base.ErrorMsg = "Failed to Add Tokens to the Profile";
		}
		return base.execute();
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		int num = tokens + servertokens;
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
		if (flag && num != AppShell.Instance.Profile.Gold)
		{
			AutomationManager.Instance.errOther++;
			flag = false;
			base.ErrorCode = "C001";
			base.ErrorMsg = "Error: Tickets do not match the server ticket count";
		}
		return flag;
	}
}
