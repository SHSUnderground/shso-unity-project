public class AndyLandWindow : GUITopLevelWindow
{
	public AndyLandWindow()
		: base("AndyLandWindow")
	{
	}

	public void LoadSampleMissionBriefing(string missionId)
	{
		AppShell.Instance.DataManager.LoadGameData("Missions/" + missionId, OnMissionDefinitionLoaded, new MissionDefinition());
	}

	private void OnMissionDefinitionLoaded(GameDataLoadResponse response, object extraData)
	{
		CspUtils.DebugLog("Attemping to Parse new front page info!");
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
		}
		else
		{
			CspUtils.DebugLog("Parsed front page info!");
		}
	}

	public override void ConfigureKeyBanks()
	{
		base.ConfigureKeyBanks();
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
	}
}
