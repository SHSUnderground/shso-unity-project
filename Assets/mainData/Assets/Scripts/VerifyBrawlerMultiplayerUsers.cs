public class VerifyBrawlerMultiplayerUsers : AutomationCmd
{
	public VerifyBrawlerMultiplayerUsers(string cmdline)
		: base(cmdline)
	{
	}

	public override bool isReady()
	{
		bool flag = base.isReady();
		if (flag)
		{
			flag = (AutomationManager.Instance.activeController == GameController.ControllerType.Brawler);
		}
		return flag;
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		int count = AutomationManager.Instance.GetAllUsers.Count;
		if (flag && count < 4)
		{
			base.ErrorCode = "VBMU001";
			base.ErrorMsg = "There is only " + count + " players but should be 4. Current User Ids: ";
			for (int i = 0; i < count; i++)
			{
				base.ErrorMsg = base.ErrorMsg + AutomationManager.Instance.GetAllUsers[i].userName + " ";
			}
		}
		return flag;
	}
}
