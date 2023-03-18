public class JumpCmd : AutomationCmd
{
	public JumpCmd(string cmdline)
		: base(cmdline)
	{
		AutomationManager.Instance.nOther++;
	}

	public override bool precheckOk()
	{
		bool flag = base.precheckOk();
		if (flag)
		{
			if (AutomationManager.Instance.isBrawler || AutomationManager.Instance.isGameWorld)
			{
				flag = true;
			}
			else
			{
				flag = false;
				base.ErrorCode = "P01";
				base.ErrorMsg = "Precheck Failed: You need to be in GameWorld or Brawler in order to Jump";
			}
		}
		else
		{
			base.ErrorCode = "P01";
			base.ErrorMsg = "Precheck Failed: base.precheck failed";
		}
		return flag;
	}

	public override bool execute()
	{
		bool flag = base.execute();
		if (flag && !AutomationBehavior.Instance.jump())
		{
			flag = false;
			base.ErrorCode = "E001";
			base.ErrorMsg = "Execute Failed: Unable to jump";
		}
		return flag;
	}

	public override bool isCompleted()
	{
		if (base.isCompleted() && !AutomationBehavior.Instance.motionCtrl.IsOnGround())
		{
			bool flag = false;
			base.ErrorCode = "C001";
			base.ErrorMsg = "Player is not on the ground";
		}
		return true;
	}
}
