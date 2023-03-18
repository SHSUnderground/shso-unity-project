using System;

public class GameAreaTransitionCmd : AutomationCmd
{
	private string spacename;

	private GameController.ControllerType ctrlspace;

	public GameAreaTransitionCmd(string cmdline, GameController.ControllerType cspace, string sname)
		: base(cmdline)
	{
		ctrlspace = cspace;
		spacename = sname;
		AutomationManager.Instance.nGameWorld++;
	}

	public override bool execute()
	{
		base.execute();
		try
		{
			AutomationManager.Instance.LogAttribute("spacename", spacename);
			CspUtils.DebugLog("GameAreaTransitionCmd ctrlspace " + ctrlspace + " spacename " + spacename);
			switch (ctrlspace)
			{
			case GameController.ControllerType.SocialSpace:
				CspUtils.DebugLog("SHT_KEY_SOCIAL_LEVEL  SocialSpaceLevel SHT_KEY_SOCIAL_SPAWNPOINT SocialSpaceSpawnPoint SHT_KEY_SOCIAL_CHARACTER   SocialSpaceCharacter SHT_KEY_SOCIAL_TICKET SocialSpaceTicket");
				AppShell.Instance.SharedHashTable["SocialSpaceLevel"] = spacename;
				break;
			case GameController.ControllerType.Brawler:
			{
				ActiveMission activeMission = new ActiveMission(spacename);
				activeMission.BecomeActiveMission();
				AppShell.Instance.Matchmaker2.SoloBrawler(spacename, null);
				AppShell.Instance.Transition(GameController.ControllerType.Brawler);
				break;
			}
			default:
				CspUtils.DebugLog("Got default in GameAreaTransitionCmd with controller:" + ctrlspace);
				break;
			}
			AppShell.Instance.Transition(ctrlspace);
		}
		catch (Exception ex)
		{
			base.ErrorCode = "E001";
			base.ErrorMsg += ex.Message;
		}
		return base.execute();
	}

	public override bool precheckOk()
	{
		bool flag = AutomationManager.Instance.activeController != ctrlspace;
		if (!flag)
		{
			base.ErrorCode = "P01";
			base.ErrorMsg = "Precheck failed : GameAreaTransition Command Error";
		}
		else
		{
			flag = base.precheckOk();
		}
		return flag;
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		if (flag)
		{
			flag = (AutomationManager.Instance.activeController == ctrlspace);
		}
		if (!flag)
		{
			base.ErrorCode = "C001";
			base.ErrorMsg = "GameAreaTransitionCmd Error.";
		}
		try
		{
			if (AutomationManager.Instance.GetAllUsers.Count > 0)
			{
				return flag;
			}
			base.ErrorMsg += " Connection to Server has been lost";
			return flag;
		}
		catch
		{
			base.ErrorMsg += " No Active Server Connections Found";
			return flag;
		}
	}
}
