public class VerifyGameFallenOjects : AutomationCmd
{
	public VerifyGameFallenOjects(string cmdline)
		: base(cmdline)
	{
	}

	public override bool execute()
	{
		base.execute();
		bool flag = (AutomationManager.Instance.FallenObjects.Count == 0) ? true : false;
		if (!flag)
		{
			base.ErrorCode = "F001";
			{
				foreach (string key in AutomationManager.Instance.FallenObjects.Keys)
				{
					base.ErrorMsg = base.ErrorMsg + key + " ";
				}
				return flag;
			}
		}
		return flag;
	}
}
