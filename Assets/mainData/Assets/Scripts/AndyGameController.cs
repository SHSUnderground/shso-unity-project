using UnityEngine;

public class AndyGameController : GameController
{
	private bool createdMissionBriefing;

	public TextAsset missionFile;

	public override void Start()
	{
		base.Start();
		CspUtils.DebugLog("Start AndyController");
	}

	private void Update()
	{
		if (missionFile == null)
		{
			missionFile = new TextAsset();
			missionFile.name = "m_0001_1_SampleMission";
		}
		if (!createdMissionBriefing && GUIManager.Instance["SHSMainWindow/AndyLandWindow"] != null)
		{
			((AndyLandWindow)GUIManager.Instance["SHSMainWindow/AndyLandWindow"]).LoadSampleMissionBriefing(missionFile.name);
			createdMissionBriefing = true;
		}
	}

	public override void OnOldControllerUnloading(AppShell.GameControllerTypeData currentGameData, AppShell.GameControllerTypeData newGameData)
	{
		base.OnOldControllerUnloading(currentGameData, newGameData);
		UnregisterDebugKeys();
	}

	protected override void RegisterDebugKeys()
	{
		KeyCodeEntry keyCodeEntry = new KeyCodeEntry(KeyCode.R, false, false, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, ReloadWindow);
		base.RegisterDebugKeys();
	}

	private void ReloadWindow(SHSKeyCode code)
	{
		CspUtils.DebugLog("Reloading mission briefing from key press: " + code.ToString());
		if (GUIManager.Instance["SHSMainWindow/AndyLandWindow"] != null)
		{
			AppShell.Instance.DataManager.ClearGameDataCache();
			((AndyLandWindow)GUIManager.Instance["SHSMainWindow/AndyLandWindow"]).LoadSampleMissionBriefing(missionFile.name);
			createdMissionBriefing = true;
		}
	}
}
