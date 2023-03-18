public class GoToZone : AutomationCmd
{
	public string zone;

	public GoToZone(string cmdline, string zn)
		: base(cmdline)
	{
		zone = zn;
		AutomationManager.Instance.nGameWorld++;
	}

	public override bool execute()
	{
		bool flag = base.execute();
		if (flag)
		{
			string value = null;
			if (GameController.GetController() != null && GameController.GetController().LocalPlayer != null)
			{
				value = GameController.GetController().LocalPlayer.name;
			}
			AppShell.Instance.SharedHashTable["SocialSpaceLevel"] = zone;
			AppShell.Instance.SharedHashTable["SocialSpaceSpawnPoint"] = null;
			AppShell.Instance.SharedHashTable["SocialSpaceCharacter"] = value;
			AppShell.Instance.Transition(GameController.ControllerType.SocialSpace);
		}
		return flag;
	}
}
