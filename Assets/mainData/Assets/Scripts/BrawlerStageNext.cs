public class BrawlerStageNext : AutomationCmd
{
	private int previousMission;

	private ActiveMission mission;

	private GUIManager gui;

	public BrawlerStageNext(string cmdline)
		: base(cmdline)
	{
		mission = new ActiveMission(AutomationManager.Instance.missionName);
		gui = new GUIManager();
		AutomationManager.Instance.nBrawler++;
	}

	public override bool precheckOk()
	{
		previousMission = AutomationManager.Instance.currentStage;
		return base.precheckOk();
	}

	public override bool isReady()
	{
		bool flag = base.isReady();
		flag = (gui.CurrentState == GUIManager.ModalStateEnum.Normal);
		if (flag && (mission.CurrentStage == 0 || AutomationManager.Instance.currentStage == mission.LastStage))
		{
			flag = false;
		}
		if (!flag)
		{
			base.ErrorMsg = "Error: Stage has not loaded or has no more stages";
		}
		return flag;
	}

	public override bool execute()
	{
		try
		{
			AppShell.Instance.EventMgr.Fire(null, new CompleteMissionMessage());
			AutomationManager.Instance.currentStage++;
		}
		catch
		{
			AutomationManager.Instance.errBrawler++;
			base.ErrorCode = "E001";
			base.ErrorMsg = "Mission Complete Error";
		}
		return base.execute();
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		if (flag)
		{
			flag = (previousMission != AutomationManager.Instance.currentStage);
		}
		if (!flag)
		{
			AutomationManager.Instance.errBrawler++;
			base.ErrorCode = "C001";
			base.ErrorMsg = "Error: Brawler has not trasitioned to the next Stage";
		}
		return flag;
	}
}
