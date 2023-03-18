public class DoPickUp : AutomationCmd
{
	private string iObject;

	private bool iObjectAvailable;

	private int xp;

	private string coins;

	private string tickets;

	public HeroPersisted heroData;

	public DoPickUp(string cmdline)
		: base(cmdline)
	{
		iObject = cmdline.Substring("triggerfly".Length).Trim();
	}

	public override bool isReady()
	{
		bool flag = base.isReady();
		iObjectAvailable = true;
		if (flag)
		{
			if (AppShell.Instance.Profile.AvailableCostumes.TryGetValue(AppShell.Instance.Profile.SelectedCostume, out heroData))
			{
				xp = heroData.Xp;
			}
			else
			{
				flag = false;
			}
		}
		else
		{
			base.ErrorMsg = "Failed to obtain Hero XP";
			base.ErrorCode = "TP001";
			AutomationManager.Instance.errGameWorld++;
		}
		return base.isReady();
	}

	public override bool execute()
	{
		bool flag = base.execute();
		if (flag && !AutomationBehavior.Instance.pickup(iObject))
		{
			CspUtils.DebugLog("There is no " + iObject + " available to pick up");
			iObjectAvailable = false;
		}
		return flag;
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		int num = 0;
		if (flag)
		{
			if (iObjectAvailable)
			{
				if (AppShell.Instance.Profile.AvailableCostumes.TryGetValue(AppShell.Instance.Profile.SelectedCostume, out heroData))
				{
					num = heroData.Xp;
					if (xp == num)
					{
						flag = false;
					}
					else if (heroData.Level != GetLevelFromXP(num))
					{
						base.ErrorCode = "LC001";
						base.ErrorMsg = "Error: Hero Level should be: " + GetLevelFromXP(num) + " but found level:" + heroData.Level;
						AutomationManager.Instance.errGameWorld++;
					}
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = true;
				base.ErrorMsg = "Nothing to throw";
			}
		}
		else
		{
			base.ErrorCode = "TC001";
			base.ErrorMsg = "Error: Hero XP failed to update!";
			AutomationManager.Instance.errGameWorld++;
		}
		return flag;
	}

	public int GetLevelFromXP(int xp)
	{
		if (0 <= xp && xp < 500)
		{
			return 1;
		}
		if (500 <= xp && xp < 1250)
		{
			return 2;
		}
		if (1250 <= xp && xp < 2000)
		{
			return 3;
		}
		if (2000 <= xp && xp < 3000)
		{
			return 4;
		}
		if (3000 <= xp && xp < 4000)
		{
			return 5;
		}
		if (4000 <= xp && xp < 5000)
		{
			return 6;
		}
		if (5000 <= xp && xp < 6000)
		{
			return 7;
		}
		if (6000 <= xp && xp < 7000)
		{
			return 8;
		}
		if (7000 <= xp && xp < 8500)
		{
			return 9;
		}
		if (8500 <= xp && xp < 10000)
		{
			return 10;
		}
		return 11;
	}
}
