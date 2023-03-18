using System.Collections.Generic;

public class ActiveMission
{
	public delegate void OnMissionDataLoadedCallback(ActiveMission mission);

	protected string id;

	protected MissionResults summaryResult;

	protected MissionDefinition definition;

	protected int currentStage = 1;

	protected int lastStage = 1;

	protected List<OnMissionDataLoadedCallback> notificationList;

	protected string selectedCostume;

	protected int selectedR2 = 1;

	public string Id
	{
		get
		{
			return id;
		}
	}

	public int CurrentStage
	{
		get
		{
			return currentStage;
		}
	}

	public int LastStage
	{
		get
		{
			return lastStage;
		}
	}

	public MissionResults SummaryResult
	{
		get
		{
			return summaryResult;
		}
	}

	public MissionDefinition MissionDefinition
	{
		get
		{
			return definition;
		}
	}

	public bool IsSurvivalMode
	{
		get
		{
			if (definition == null)
			{
				return false;
			}
			return definition.DisplayTimer;
		}
	}

	////// these methods added by CSP  //////////
	public bool IsMayhem
	{
		get
		{
			if (id.Contains("100M"))   // if '100M' exists in the mission id
			{
				return true;
			}
			return false;
		}
	}

	public bool IsCrisis
	{
		get
		{
			if (id.EndsWith("A"))  // mission id ends in capital 'A'
			{
				return true;
			}
			return false;
		}
	}
	/////////////////////////////////////////////

	public string SelectedCostume
	{
		get
		{
			return selectedCostume;
		}
		set
		{
			selectedCostume = value;
		}
	}

	public int SelectedR2
	{
		get
		{
			return selectedR2;
		}
		set
		{
			selectedR2 = value;
		}
	}

	public ActiveMission(string missionId)
	{
		id = missionId;
		notificationList = new List<OnMissionDataLoadedCallback>();
		AppShell.Instance.DataManager.LoadGameData("Missions/" + missionId, OnMissionDefinitionLoaded, new MissionDefinition());
	}

	public void NotifyWhenDefinitionLoaded(OnMissionDataLoadedCallback cb)
	{
		if (definition != null)
		{
			cb(this);
		}
		else
		{
			notificationList.Add(cb);
		}
	}

	public void BecomeActiveMission()
	{
		StartStage(1);
		AppShell.Instance.SharedHashTable["ActiveMission"] = this;
	}

	public void StartStage(int stageId)
	{
		PrepForNewStage();
		currentStage = stageId;
	}

	public int StartNextStage()
	{
		PrepForNewStage();
		return ++currentStage;
	}

	protected void PrepForNewStage()
	{
		summaryResult = new MissionResults();
	}

	public string GeometryBundle()
	{
		return definition.StageDefinition(currentStage).GeometryBundleName;
	}

	public string ScenarioBundle()
	{
		return definition.StageDefinition(currentStage).ScenarioBundleName;
	}

	public string ConcludingCinematicBundle()
	{
		return definition.CinematicBundleName;
	}

	public string StageCinematicBundle()
	{
		return definition.StageDefinition(currentStage).CinematicBundleName;
	}

	protected void OnMissionDefinitionLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			definition = null;
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		definition = (response.DataDefinition as MissionDefinition);
		lastStage = definition.LastStage;
		foreach (OnMissionDataLoadedCallback notification in notificationList)
		{
			notification(this);
		}
		notificationList.Clear();
	}
}
