public class VerifyPlayerName : AutomationCmd
{
	private string privateName;

	private string playerName;

	public VerifyPlayerName(string cmdline, string expPrivateName)
		: base(cmdline)
	{
		privateName = expPrivateName;
		AutomationManager.Instance.nGameWorld++;
	}

	public override bool precheckOk()
	{
		bool result = base.precheckOk();
		if (AutomationManager.Instance.LoginStatus != AutomationManager.LoginState.Succeeded)
		{
			result = false;
			base.ErrorCode = "P01";
			base.ErrorCode = "PreCheck Failed. You need to be logged in before you execute this command";
		}
		return result;
	}

	public override bool execute()
	{
		bool flag = base.execute();
		if (flag)
		{
			try
			{
				playerName = AppShell.Instance.Profile.PlayerName;
				return flag;
			}
			catch (AutomationExecuteException ex)
			{
				base.ErrorCode = "E001";
				base.ErrorMsg = "Unexpected Error" + ex.Message;
				return flag;
			}
		}
		base.ErrorCode = "E001";
		base.ErrorMsg = "Execution Timed Out. Unable to get Profile Data";
		return flag;
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		if (flag)
		{
			CspUtils.DebugLog("Acutal Name: " + playerName + "must not equal to Given Name: " + privateName);
			if (privateName == playerName)
			{
				base.ErrorCode = "C001";
				base.ErrorMsg = "Player Private name is same as Players Public Name!";
			}
		}
		return flag;
	}
}
