using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HqControllerTutorial : HqController2
{
	public TextAsset EditorUserProfile;

	public TextAsset EditorItemDefinitions;

	public TextAsset EditorInventory;

	[CompilerGenerated]
	private HqRoom2 _003CBridge_003Ek__BackingField;

	[CompilerGenerated]
	private HqRoom2 _003CDorm1_003Ek__BackingField;

	public HqRoom2 Bridge
	{
		[CompilerGenerated]
		get
		{
			return _003CBridge_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CBridge_003Ek__BackingField = value;
		}
	}

	public HqRoom2 Dorm1
	{
		[CompilerGenerated]
		get
		{
			return _003CDorm1_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CDorm1_003Ek__BackingField = value;
		}
	}

	protected override GUITopLevelWindow MainWindow
	{
		get
		{
			return (GUITopLevelWindow)GUIManager.Instance["SHSMainWindow/SHSHqRailsMainWindow"];
		}
	}

	public override void Start()
	{
		StartTransaction.AddStep("secondaryInitializeStep");
		AppShell.Instance.EventMgr.Fire(this, new NewControllerLoadingMessage(this));
		if (bCallControllerReadyFromStart)
		{
			ControllerReady();
		}
		AppShell.Instance.OnOldControllerUnloading += OnOldControllerUnloading;
		rooms = new Dictionary<string, HqRoom2>();
		roomIds = new List<string>();
		bundles = new Dictionary<string, AssetBundle>();
		characterPrefabs = new Dictionary<string, GameObject>();
		AIControllers = new List<AIControllerHQ>();
		fsm = new ShsFSM();
		fsm.AddState(new HqTutorialLoad(this));
		fsm.AddState(new HqTutorialPause());
		fsm.AddState(new HqTutorialPlay());
		hqInput = Utils.GetComponent<HqInput>(base.gameObject);
		if (hqInput == null)
		{
			CspUtils.DebugLog("HqController2 can not find HqInput");
		}
		if (isTestScene)
		{
			DataWarehouse dataWarehouse = new DataWarehouse(EditorItemDefinitions.text);
			dataWarehouse.Parse();
			AppShell.Instance.ItemDictionary = new ItemDefinitionDictionary(dataWarehouse);
			profile = UserProfile.CreateOfflineUserProfile(EditorUserProfile, EditorInventory);
		}
		else
		{
			profile = AppShell.Instance.Profile;
		}
		if (AppShell.Instance.Profile != null)
		{
			WatchMe("#transition_headquarters");
		}
		fsm.GotoState<HqTutorialLoad>();
	}

	public void GoToNextRoom()
	{
		if (base.ActiveRoom == Bridge)
		{
			SetActiveRoom(Dorm1);
		}
		else
		{
			SetActiveRoom(Bridge);
		}
	}

	protected override void OnNewControllerReady(NewControllerReadyMessage readyMessage)
	{
		CspUtils.DebugLog("On New Controller ready in tutorial.");
	}

	public void GoToPauseMode()
	{
		fsm.GotoState<HqTutorialPause>();
	}

	public void GoToPlayMode()
	{
		fsm.GotoState<HqTutorialPlay>();
	}

	public override bool IsInPlayMode()
	{
		return base.State == typeof(HqTutorialPlay);
	}
}
