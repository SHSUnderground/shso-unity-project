using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
//using UnityEditor;

public class BrawlerController : GameController, ICharacterCache
{

	public static bool cspReady = false;   // CSP - added for testing
	internal class StartExtraData
	{
		internal string bundleName;

		internal UnityEngine.Object geometryBundle;

		internal UnityEngine.Object scenarioBundle;

		internal StartExtraData()
		{
			bundleName = string.Empty;
			geometryBundle = null;
			scenarioBundle = null;
		}
	}

	public class PickupData
	{
		public string id;

		public string pickupName;

		public string effectName;

		public float collisionRadius;

		public float despawnTime;

		public float healthChange;

		public float powerChange;

		public string combatEffectName;

		public int scoreChange;
	}

	public class DropTable
	{
		public string name;

		public string[] pickupPrefab;

		public float[] chance;
	}

	protected class CachedPrespawn
	{
		public string characterName;

		public CharacterSpawnData extraData;

		public CharacterSpawn requestee;

		public CachedPrespawn(string name, CharacterSpawnData initData, CharacterSpawn source)
		{
			characterName = name;
			extraData = initData;
			requestee = source;
		}
	}

	public enum BrawlerUIMode
	{
		Loading,
		Airlock,
		Level,
		Diorama,
		StageComplete,
		MissionComplete,
		LevelContinue,
		CutScene,
		Uninitialized
	}

	private const string OBJECTIVE_COMPLETED = "ObjectiveCompleted";

	private const string RECEIVED_MISSION_EVENT_RESPONSE = "MissionEventResponseReceived";

	private const string LAST_ENEMY_DESPAWNED = "LastEnemyDespawned";

	public const string BRAWLER_BUNDLE_DIRECTORY = "Brawler/";

	protected const float INITIAL_OBJECTIVE_TIME = 6f;

	protected const float OBJECTIVE_CHECK_TIME = 45f;

	protected const float OBJECTIVE_REMINDER_TIME = 4f;

	public bool isTestNetwork;

	private ObjectiveBase currentObjective;

	private bool objectiveMet;

	private TransactionMonitor gameplayDoneTransaction;

	protected TransactionMonitor stageCompleteTransaction;

	protected EventResultSet missionResultSet;

	private CharacterSelectionBlock characterSelected;

	protected ActiveMission mission;

	protected GameObject brawlerScenario;

	protected CharacterCombinationManager characterCombinationManager;

	protected static BrawlerController instance;

	protected Dictionary<string, PickupData> pickupDataDictionary;

	protected Dictionary<string, DropTable> dropTableDictionary;

	protected TransactionMonitor brawlerStartTransaction;

	protected TransactionMonitor selectCharacterTransaction;

	protected TransactionMonitor gettingReadyToSpawnTransaction;

	protected TransactionMonitor playersSpawnedTransaction;

	protected TransactionMonitor restartTransaction;

	protected TransactionMonitor gettingReadyToReactivateTransaction;

	protected List<CharacterSpawn> playerSpawners;

	public AssetBundle brawlerBundle;

	public GameObject throwableTargetPrefab;

	protected int forcedMedal = -1;

	private bool waitingForNotification;

	private float notificationTimeOut = 120f;

	private float notificationWait;

	protected Matchmaker2.Ticket ticket;

	protected string READY_KEY = "ready";  // CSP changed to lowercase 'r'   // "Ready"; //

	protected bool waitingForAirlock;

	protected bool autoSelectOn;

	protected bool showAirlockOnLoad;

	private bool isCutScenePlaying;

	private bool endStage;

	private int pendingPlaceholderObjects;

	public bool allowPickupSpawn;

	public bool prespawnStarted;

	protected string heroUpCharacterEffect = string.Empty;

	protected string persistentHeroUpCharacterEffect = string.Empty;

	protected HashSet<string> validPolymorphs;

	protected BrawlerOrthographicHud brawlerHud;

	public bool showAttackColliders;

	public bool showAttackChainNumbers;

	public bool showDioramaObjects = true;

	public bool showClickBoxes;

	public int debugPlayerCount;

	public bool dumpAIChoices;

	private GameObject clickBoxVisualizerPrefab;

	private bool DioramaMode;

	private GameObject rewardInstance;

	protected SHSBrawlerIndicatorArrow UiArrow;

	protected List<GameObject> activeEnemies;

	protected List<GameObject> wayPoints;

	protected int currentIndicator;

	protected float objectDistance = 9000f;

	protected GameObject closestObject;

	protected Dictionary<GameObject, GameObject> wayPointEffects;

	protected int prespawners;

	[HideInInspector]
	public Dictionary<string, GameObject> characterPrefabs;

	protected List<CachedPrespawn> requestedSpawns;

	private TransactionMonitor loadStep;

	protected List<string> heroCharacters;

	private bool _prespawnAIPlayers;

	protected bool isLoading;

	protected bool playedBriefingVO;

	protected BrawlerUIMode currentUIMode = BrawlerUIMode.Uninitialized;

	protected List<GameObject> bossToShow;

	protected SHSBrawlerWaitWindow loadWindow;

	protected SHSBrawlerScoreWindow scoreWindow;

	private string objectiveText;

	private string objectiveIcon;

	[CompilerGenerated]
	private IMissionEndHandler _003CMissionEndHandler_003Ek__BackingField;

	public CharacterCombinationManager CharacterCombinationManager
	{
		get
		{
			return characterCombinationManager;
		}
	}

	public Matchmaker2.Ticket GetTicket
	{
		get
		{
			return ticket;
		}
	}

	public string HeroUpCharacterEffect
	{
		get
		{
			return heroUpCharacterEffect;
		}
	}

	public string PersistentHeroUpCharacterEffect
	{
		get
		{
			return persistentHeroUpCharacterEffect;
		}
	}

	public HashSet<string> GetValidPolymorphs
	{
		get
		{
			return validPolymorphs;
		}
	}

	public BrawlerOrthographicHud BrawlerHud
	{
		get
		{
			return brawlerHud;
		}
	}

	public static BrawlerController Instance
	{
		get
		{
			return instance;
		}
	}

	public bool IsCutScenePlaying
	{
		get
		{
			return isCutScenePlaying;
		}
	}

	public bool PrespawnAIPlayers
	{
		get
		{
			return _prespawnAIPlayers;
		}
		set
		{
			_prespawnAIPlayers = value;
		}
	}

	public IMissionEndHandler MissionEndHandler
	{
		[CompilerGenerated]
		get
		{
			return _003CMissionEndHandler_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CMissionEndHandler_003Ek__BackingField = value;
		}
	}

	public override void Awake()
	{
		base.Awake();

		CspUtils.DebugLog("BrawlerController Awake() called!");

		if (AppShell.Instance.EventMgr != null)
		{
			AppShell.Instance.EventMgr.Fire(this, new BrawlerStartedMessage());
		}
		if (isTestScene)
		{
			List<string> steps = new List<string>(new string[9]
			{
				"pickupDataLoaded",
				"dropTableDataLoaded",
				"characterCombinationDataLoaded",
				"enemyBarDataLoaded",
				"brawlerCharacterDataLoaded",
				"brawlerAssetBundleLoaded",
				"brawlerStartTransaction",
				"selectCharacterTransaction",
				"brawlerOrthographicHudDataLoaded"
			});
			TransactionLoadingContext.TransactionContext transactionContext = new TransactionLoadingContext.TransactionContext(steps, "brawler_start_transaction");
			TransitionHandler.WWTransitionProperties propertiesForTransition = new TransitionHandler.WWTransitionProperties(false, false, false, "SHSBrawlerWaitWindow", string.Empty);
			propertiesForTransition.transactionContext = transactionContext;
			propertiesForTransition.locationInfo = GUILoadingScreenContext.LocationInfo.NoInfo;
			propertiesForTransition.loadingContext = GUILoadingScreenContext.LoadingContext.EmptyContext;
			TransactionLoadingContext currentTransactionContext = AppShell.Instance.TransitionHandler.CurrentTransactionContext;
			currentTransactionContext.RemoveChildTransaction("LaunchLogin");
			currentTransactionContext.RemoveChildTransaction("LaunchTransition");
			AppShell.Instance.TransitionHandler.AddManualTransitionPoolObj(propertiesForTransition, "brawler_test_wait_window");
			AppShell.Instance.TransitionHandler.MakeManualTransitionActive("brawler_test_wait_window");
			StartTransaction = AppShell.Instance.TransitionHandler.GetManualTransactionContext("brawler_test_wait_window").Transaction;
		}
		else
		{
			StartTransaction = AppShell.Instance.TransitionHandler.CurrentTransactionContext.Transaction;
		}
		StartTransaction.onComplete += OnStartBrawlerComplete;
		if (instance != null)
		{
			CspUtils.DebugLog("A second BrawlerController is being created.  This may lead to instabilities!");
		}
		else
		{
			instance = this;
		}
		bCallControllerReadyFromStart = false;
		AppShell.Instance.DataManager.LoadGameData("Brawler/pickup_data", OnPickupDataLoaded);
		StartTransaction.AddStepBundle("pickupDataLoaded", "Brawler/pickup_data");
		AppShell.Instance.DataManager.LoadGameData("Brawler/drop_table_data", OnDropTableDataLoaded);
		StartTransaction.AddStepBundle("dropTableDataLoaded", "Brawler/drop_table_data");
		AppShell.Instance.DataManager.LoadGameData("Brawler/character_combination_data", OnCharacterCombinationDataLoaded);
		StartTransaction.AddStepBundle("characterCombinationDataLoaded", "Brawler/character_combination_data");
		AppShell.Instance.DataManager.LoadGameData("Brawler/enemy_health_bar_data", OnEnemyHealthBarDataLoaded);
		StartTransaction.AddStepBundle("enemyBarDataLoaded", "Brawler/enemy_health_bar_data");
		AppShell.Instance.DataManager.LoadGameData("Brawler/brawler_character_data", OnBrawlerCharacterDataLoaded);
		StartTransaction.AddStepBundle("brawlerCharacterDataLoaded", "Brawler/brawler_character_data");
		AppShell.Instance.DataManager.LoadGameData("Brawler/brawler_orthographic_hud_data", OnBrawlerOrthographicHudDataLoaded);
		StartTransaction.AddStepBundle("brawlerOrthographicHudDataLoaded", "Brawler/brawler_orthographic_hud_data");
		AppShell.Instance.BundleLoader.FetchAssetBundle("Brawler/Brawler_globals", OnBrawlerAssetBundleLoaded, null);
		StartTransaction.AddStepBundle("brawlerAssetBundleLoaded", "Brawler/Brawler_globals");
		playerSpawners = new List<CharacterSpawn>();
		characterCache = this;
		ResetPrespawnData();
		AppShell.Instance.EventMgr.AddListener<CharacterReadyMessage>(OnPlayerCharacterReady);
		AppShell.Instance.EventMgr.AddListener<CharacterRequestedMessage>(OnPlayerCharacterRequested);
		AppShell.Instance.EventMgr.AddListener<CharacterSelectedMessage>(OnPlayerCharacterSelected);
		AppShell.Instance.EventMgr.AddListener<AirlockTimerMessage>(OnCharacterSelectTimer);
		AppShell.Instance.EventMgr.AddListener<BrawlerHideSelectMessage>(OnGameplayReady);
		AppShell.Instance.EventMgr.AddListener<BrawlerForceMedalMessage>(OnForceMedal);
		AppShell.Instance.EventMgr.AddListener<PresetCombinationsRequestMessage>(OnPresetCharacterCombination);
		RegisterDebugEvents();
		RegisterUIEvents();
		RegisterIndicatorEvents();
		UiEnable();
		if (isTestNetwork)
		{
			RoomNetTest component = GetComponent<RoomNetTest>();
			if (component == null)
			{
				isTestNetwork = false;
			}
		}
		if ((AppShell.Instance.ServerConnection.State & NetworkManager.ConnectionState.Authenticated) == 0)
		{
			if (!isTestNetwork)
			{
				CspUtils.DebugLog("Switching to standalone mode");
				AppShell.Instance.StubServerConnection();
			}
			else
			{
				CspUtils.DebugLog("Network Test Mode");
			}
		}
		brawlerHud = GetComponentInChildren<BrawlerOrthographicHud>();
		VOManager.Instance.StopAll();
	}

	protected void OnBrawlerAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("Failed to load the brawler asset bundle <" + response.Path + ">.");
			if (StartTransaction != null)
			{
				StartTransaction.FailStep("brawlerAssetBundleLoaded", response.Error);
			}
		}
		brawlerBundle = response.Bundle;
		throwableTargetPrefab = (brawlerBundle.Load("Temporary_Throwable_Target") as GameObject);
		if (BrawlerHud != null)
		{
			BrawlerHud.InitializeHudBundle(brawlerBundle);
		}
		if (StartTransaction != null)
		{
			StartTransaction.CompleteStep("brawlerAssetBundleLoaded");
		}
	}

	public void OnDropTableDataLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			if (StartTransaction != null)
			{
				StartTransaction.FailStep("dropTableDataLoaded", response.Error);
			}
			return;
		}
		DataWarehouse data = response.Data;
		dropTableDictionary = new Dictionary<string, DropTable>();
		foreach (DataWarehouse item in data.GetIterator("//drop_table"))
		{
			DropTable dropTable = new DropTable();
			dropTable.name = item.GetString("name");
			int count = item.GetCount("drop");
			dropTable.pickupPrefab = new string[count];
			dropTable.chance = new float[count];
			int num = 0;
			float num2 = 0f;
			foreach (DataWarehouse item2 in item.GetIterator("drop"))
			{
				dropTable.pickupPrefab[num] = item2.GetString("prefab");
				num2 += item2.GetFloat("chance");
				if (num2 > 100f)
				{
					CspUtils.DebugLog("Total chance of a drop for drop table " + dropTable.name + " exceeds 100, some drops may not be possible");
				}
				dropTable.chance[num] = num2;
				num++;
			}
			dropTableDictionary.Add(dropTable.name, dropTable);
		}
		if (StartTransaction != null)
		{
			StartTransaction.CompleteStep("dropTableDataLoaded");
		}
	}

	public void OnPickupDataLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			if (StartTransaction != null)
			{
				StartTransaction.FailStep("pickupDataLoaded", response.Error);
			}
			return;
		}
		DataWarehouse data = response.Data;
		pickupDataDictionary = new Dictionary<string, PickupData>();
		foreach (DataWarehouse item in data.GetIterator("//pickup"))
		{
			PickupData pickupData = new PickupData();
			pickupData.id = item.GetString("id");
			pickupData.pickupName = item.GetString("display_name");
			pickupData.effectName = item.TryGetString("effect", null);
			pickupData.despawnTime = item.TryGetFloat("time_to_despawn", 30f);
			pickupData.healthChange = item.TryGetFloat("health", 0f);
			pickupData.powerChange = item.TryGetFloat("power", 0f);
			pickupData.combatEffectName = item.TryGetString("combat_effect", null);
			pickupData.scoreChange = item.TryGetInt("scoring/picked_up", 0);
			pickupDataDictionary.Add(pickupData.id, pickupData);
		}
		if (StartTransaction != null)
		{
			StartTransaction.CompleteStep("pickupDataLoaded");
		}
	}

	public void OnCharacterCombinationDataLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			if (StartTransaction != null)
			{
				StartTransaction.FailStep("characterCombinationDataLoaded", response.Error);
			}
			return;
		}
		DataWarehouse data = response.Data;
		characterCombinationManager = new CharacterCombinationManager();
		characterCombinationManager.InitializeFromData(data);
		characterCombinationManager.AddEventListeners();
		if (StartTransaction != null)
		{
			StartTransaction.CompleteStep("characterCombinationDataLoaded");
		}
		base.gameObject.AddComponent<CharacterCombinationBridge>();
	}

	public void OnEnemyHealthBarDataLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			if (StartTransaction != null)
			{
				StartTransaction.FailStep("enemyBarDataLoaded", response.Error);
			}
			return;
		}
		DataWarehouse data = response.Data;
		SHSBrawlerEnemyBar.InitializeHealthBar(data);
		if (StartTransaction != null)
		{
			StartTransaction.CompleteStep("enemyBarDataLoaded");
		}
	}

	public void OnBrawlerCharacterDataLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			if (StartTransaction != null)
			{
				StartTransaction.FailStep("brawlerCharacterDataLoaded", response.Error);
			}
			return;
		}
		DataWarehouse data = response.Data;
		persistentHeroUpCharacterEffect = data.TryGetString("brawler_character_data/persistent_hero_up_character_effect", string.Empty);
		heroUpCharacterEffect = data.TryGetString("brawler_character_data/hero_up_character_effect", string.Empty);
		validPolymorphs = new HashSet<string>();
		DataWarehouse data2 = data.GetData("brawler_character_data/polymorph_data");
		if (data2 != null)
		{
			int count = data2.GetCount("valid_choice");
			for (int i = 0; i < count; i++)
			{
				validPolymorphs.Add(data2.GetString("valid_choice", i));
			}
		}
		if (StartTransaction != null)
		{
			StartTransaction.CompleteStep("brawlerCharacterDataLoaded");
		}
	}

	public void OnBrawlerOrthographicHudDataLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			if (StartTransaction != null)
			{
				StartTransaction.FailStep("brawlerOrthographicHudDataLoaded", response.Error);
			}
			return;
		}
		if (BrawlerHud != null)
		{
			BrawlerHud.InitializeHudData(response.Data);
		}
		if (StartTransaction != null)
		{
			StartTransaction.CompleteStep("brawlerOrthographicHudDataLoaded");
		}
	}

	public override void Start()
	{
		base.Start();
		PlayerCombatController.PlayerListClear();
		if (ScenarioEventManager.Instance != null)
		{
			ScenarioEventManager.Instance.ClearEventRecord();
			ScenarioEventManager.Instance.RecordEvents(true);
		}
		base.gameObject.AddComponent(typeof(ScenarioEventObjective));
		base.gameObject.AddComponent(typeof(AttackDataManager));
		characterSelected = null;
		StartExtraData userData = new StartExtraData();
		brawlerStartTransaction = TransactionMonitor.CreateTransactionMonitor("Brawler Start Transaction", OnStartTransactionComplete, 120f, userData);
		brawlerStartTransaction.AddStep("ticket", TransactionMonitor.DumpTransactionStatus);
		brawlerStartTransaction.AddStep("seat", TransactionMonitor.DumpTransactionStatus);
		brawlerStartTransaction.AddStep("load", TransactionMonitor.DumpTransactionStatus);
		brawlerStartTransaction.AddStep("refresh_heroes", TransactionMonitor.DumpTransactionStatus);
		selectCharacterTransaction = TransactionMonitor.CreateTransactionMonitor("Select Character Transaction", OnSelectedCharacter, float.MaxValue, 0);
		selectCharacterTransaction.AddStep("load_complete", TransactionMonitor.DumpTransactionStatus);
		selectCharacterTransaction.AddStep("select_character", TransactionMonitor.DumpTransactionStatus);
		gettingReadyToSpawnTransaction = TransactionMonitor.CreateTransactionMonitor("Spawn Transaction", OnReadyToSpawnPlayerCharacter, float.MaxValue, 0);
		gettingReadyToSpawnTransaction.AddStep("load_level_spawners", TransactionMonitor.DumpTransactionStatus, "#load_screen_brawler_loaded_characters");
		gettingReadyToSpawnTransaction.AddStep("airlock_timer_complete", TransactionMonitor.DumpTransactionStatus);
		if (AppShell.Instance.SharedHashTable["BrawlerAirlockPlaySolo"] != null && (bool)AppShell.Instance.SharedHashTable["BrawlerAirlockPlaySolo"])
		{
			StartTransaction.AddStep("gameplay_ready", TransactionMonitor.DumpTransactionStatus, "#load_screen_brawler_ready");
			StartTransaction.AddChild(gettingReadyToSpawnTransaction);
		}
		waitingForAirlock = true;
		autoSelectOn = false;
		if ((AppShell.Instance.ServerConnection.State & NetworkManager.ConnectionState.ConnectedToGame) != 0)
		{
			AppShell.Instance.ServerConnection.SetUserVariable(READY_KEY, false);
		}
		objectiveMet = false;
		currentObjective = (ObjectiveBase)base.gameObject.GetComponent(typeof(ObjectiveBase));
		gameplayDoneTransaction = TransactionMonitor.CreateTransactionMonitor("Gameplay Done", OnGameplayComplete, 0f, null);
		gameplayDoneTransaction.AddStep("ObjectiveCompleted");
		GameObject gameObject = GameObject.Find("/Tiles");
		if (gameObject != null)
		{
			Utils.ActivateTree(gameObject, false);
		}
		if (!isTestScene)
		{
			UserProfile profile = AppShell.Instance.Profile;
			if (profile == null)
			{
				AppShell.Instance.Transition(ControllerType.FrontEnd);
				return;
			}
			AppShell.Instance.WebService.StartRequest("resources$users/" + profile.UserId + "/heroes", OnHeroXpLevelResponse);
		}
		else
		{
			brawlerStartTransaction.CompleteStep("load");
			brawlerStartTransaction.CompleteStep("refresh_heroes");
		}
		IndicatorUiSetup();
		if (!isTestScene)
		{
			ChangeLoadingScreen();
		}
		PrepRelationshipVO();
	}

	public override void OnEnable()
	{
		base.OnEnable();
		IndicatorSetup();
		if (brawlerHud == null)
		{
			brawlerHud = GetComponentInChildren<BrawlerOrthographicHud>();
		}
	}

	public override void OnDisable()
	{
		base.OnDisable();
		AppShell.Instance.EventMgr.RemoveListener<BrawlerSummaryCompleteMessage>(OnSummaryComplete);
		AppShell.Instance.EventMgr.RemoveListener<CharacterRequestedMessage>(OnPlayerCharacterRequested);
		AppShell.Instance.EventMgr.RemoveListener<CharacterSelectedMessage>(OnPlayerCharacterSelected);
		AppShell.Instance.EventMgr.RemoveListener<AirlockTimerMessage>(OnCharacterSelectTimer);
		AppShell.Instance.EventMgr.RemoveListener<BrawlerHideSelectMessage>(OnGameplayReady);
		AppShell.Instance.EventMgr.RemoveListener<BrawlerForceMedalMessage>(OnForceMedal);
		AppShell.Instance.EventMgr.RemoveListener<PresetCombinationsRequestMessage>(OnPresetCharacterCombination);
		UnregisterDebugEvents();
		UnregisterUIEvents();
		UnregisterIndicatorEvents();
		UserProfile profile = AppShell.Instance.Profile;
		if (profile != null)
		{
			profile.SelectedCostume = null;
		}
		if (characterCombinationManager != null)
		{
			characterCombinationManager.RemoveEventListeners();
			characterCombinationManager = null;
		}
		if (ScenarioEventManager.Instance != null)
		{
			ScenarioEventManager.Instance.ClearEventRecord();
			ScenarioEventManager.Instance.RecordEvents(false);
			ScenarioEventManager.Instance.DebugRecordEvents(false);
		}
		instance = null;
	}

	private void OnGameplayReady(BrawlerHideSelectMessage e)
	{
		if (StartTransaction != null)
		{
			StartTransaction.CompleteStep("gameplay_ready");
		}
		SetUIMode(BrawlerUIMode.Level);
	}

	private void OnForceMedal(BrawlerForceMedalMessage e)
	{
		if (e.medalValue < -1 || e.medalValue > 3)
		{
			CspUtils.DebugLog("Invalid medal value passed!  Valid range is from -1 to 3");
		}
		else
		{
			forcedMedal = e.medalValue;
		}
	}

	protected void OnStepComplete(string step, bool success, TransactionMonitor transaction)
	{
		CspUtils.DebugLog("Step <" + step + "> completed with <" + success + ">");
	}

	protected void OnSeatStep(string step, bool success, TransactionMonitor transaction)
	{
		CspUtils.DebugLog("Seated step <" + success + ">");
	}

	protected void OnSeated(bool success, string error)
	{
		if (brawlerStartTransaction == null)
		{
			return;
		}
		if (success)
		{
			brawlerStartTransaction.CompleteStep("seat");
			if (brawlerStartTransaction != null && !brawlerStartTransaction.IsStepCompleted("load"))
			{
				StartLevelLoading();
			}
			if (!isTestScene)
			{
				ActiveMission activeMission = AppShell.Instance.SharedHashTable["ActiveMission"] as ActiveMission;
				if (activeMission.CurrentStage <= 1)
				{
					showAirlockOnLoad = true;
				}
				if (activeMission.CurrentStage == activeMission.LastStage)
				{
					ShsAudioSourceList.GetList("MissionResultsScreen").PreloadAll(brawlerStartTransaction);
				}
				ShsAudioSourceList.GetList("MissionGlobal").PreloadAll(brawlerStartTransaction);
			}
		}
		else
		{
			CspUtils.DebugLog("Seating failed with error: " + error);
			brawlerStartTransaction.FailStep("seat", error);
		}
	}

	protected void StartLevelLoading()
	{
		mission = (AppShell.Instance.SharedHashTable["ActiveMission"] as ActiveMission);
		Resources.UnloadUnusedAssets();
		string text = AppShell.Instance.ServerConnection.GetRoomVariable("StageID") as string;
		CspUtils.DebugLog("Room Stage <" + text + ">");
		if (text != null && text != "*" && text != "1")
		{
			int stageId = 0;
			int num = text.IndexOf(":");
			if (num != -1)
			{
				stageId = int.Parse(text.Substring(num + 1));
				text = text.Substring(0, num);
			}
			if (mission == null)
			{
				mission = new ActiveMission(text);
				mission.BecomeActiveMission();
			}
			else if (text != mission.Id)
			{
				CspUtils.DebugLog("Room stage <" + text + "> does not match mission <" + mission.Id + ">");
			}
			mission.StartStage(stageId);
		}
		if (mission == null)
		{
			mission = new ActiveMission("m_0003_1_DefeatDoctorDoom");
			mission.BecomeActiveMission();
		}
		mission.NotifyWhenDefinitionLoaded(OnMissionDataLoaded);
		SetupResultsTransaction();
	}

	private void FetchMissionAssets(StartExtraData extraData)
	{
		extraData.bundleName = mission.ScenarioBundle();
		if (mission.GeometryBundle() != null)
		{
			CspUtils.DebugLog("Brawler mission references geometry bundle!  This is deprecated.  Report the misbehaving mission to one of the brawler level designers");
			return;
		}
		CspUtils.DebugLog("Attempting to load " + mission.ScenarioBundle());
		AppShell.Instance.BundleLoader.FetchAssetBundle("Brawler/" + mission.ScenarioBundle(), OnBrawlerScenarioBundleLoaded, extraData);
	}

	protected void OnMissionDataLoaded(ActiveMission mission)
	{
		if (object.ReferenceEquals(mission, this.mission))
		{
			StartExtraData extraData = brawlerStartTransaction.UserData as StartExtraData;
			FetchMissionAssets(extraData);
		}
		else
		{
			CspUtils.DebugLog("Failing in BrawlerController initialization.  The mission data loaded <" + mission.Id + "> was not the active mission >" + this.mission.Id + ">!");
		}
	}

	protected void OnHeroXpLevelResponse(ShsWebResponse response)
	{
		if (brawlerStartTransaction != null)
		{
			UserProfile profile = AppShell.Instance.Profile;
			if (response.Status == 200)
			{
				try
				{
					profile.AvailableCostumes.UpdateItemsFromData(response.Body);
				}
				catch (Exception arg)
				{
					CspUtils.DebugLog("Exception occurred while processing the HeroXPLevelUpResponse: <" + arg + ">.");
				}
			}
			else
			{
				CspUtils.DebugLog("Unable to retrieve updated Hero XP/Level info.  Proceeding with existing info.");
			}
			brawlerStartTransaction.CompleteStep("refresh_heroes");
		}
		else
		{
			CspUtils.DebugLog("The start transaction is gone.  Cannot proceed.");
		}
	}

	protected new void OnStartTransactionComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		brawlerStartTransaction = null;
		switch (exit)
		{
		case TransactionMonitor.ExitCondition.Success:
			selectCharacterTransaction.CompleteStep("load_complete");
			break;
		case TransactionMonitor.ExitCondition.TimedOut:
			AppShell.Instance.CriticalError(SHSErrorCodes.Code.BrawlerTransactionTimeout, error);
			break;
		default:
			AppShell.Instance.CriticalError(SHSErrorCodes.Code.BrawlerTransactionError, error);
			break;
		}
		StartTransaction.CompleteStep("brawlerStartTransaction");
	}

	protected void OnSelectedCharacter(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		selectCharacterTransaction = null;
		switch (exit)
		{
		case TransactionMonitor.ExitCondition.Success:
			AppShell.Instance.ServerConnection.Game.ClientReady();
			ActivatePrespawns(gettingReadyToSpawnTransaction);
			prespawnStarted = true;
			if (!isTestScene)
			{
				ControllerReady();
			}
			break;
		case TransactionMonitor.ExitCondition.TimedOut:
			AppShell.Instance.CriticalError(SHSErrorCodes.Code.BrawlerTransactionTimeout, error);
			break;
		default:
			AppShell.Instance.CriticalError(SHSErrorCodes.Code.BrawlerTransactionError, error);
			break;
		}
		StartTransaction.CompleteStep("selectCharacterTransaction");
	}

	protected void OnStartBrawlerComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		CspUtils.DebugLog("BrawlerController.OnStartBrawlerComplete()");
		StartTransaction = null;
	}

	private void OnBrawlerScenarioBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if (response.Error != null)
		{
			CspUtils.DebugLog("Unable to load requested asset bundle <" + response.Path + "> with error: " + response.Error);
			if (brawlerStartTransaction != null)
			{
				brawlerStartTransaction.FailStep("load", response.Error);
			}
			if (restartTransaction != null)
			{
				restartTransaction.FailStep("load", response.Error);
			}
			return;
		}
		CspUtils.DebugLog("Loaded scenario");
		if (showAirlockOnLoad)
		{
			showAirlockOnLoad = false;
			SetUIMode(BrawlerUIMode.Airlock);
			if (mission.SelectedCostume != null && mission.SelectedCostume != string.Empty)
			{
				characterSelected = new CharacterSelectionBlock(mission.SelectedCostume, mission.SelectedR2);
				AppShell.Instance.EventMgr.Fire(this, new CharacterRequestedMessage(characterSelected));
			}
		}
		StartExtraData startExtraData = extraData as StartExtraData;
		startExtraData.scenarioBundle = response.Bundle.mainAsset;
		if (startExtraData.scenarioBundle != null)
		{
			brawlerScenario = (UnityEngine.Object.Instantiate(startExtraData.scenarioBundle) as GameObject);
			if (brawlerScenario != null)
				CspUtils.DebugLog("Scenario instantiated!");  // added by CSP for testing
		}
//		EditorApplication.isPaused = true;  // added by CSP for testing
		ObjectPlaceholder[] array = UnityEngine.Object.FindObjectsOfType(typeof(ObjectPlaceholder)) as ObjectPlaceholder[];
		if (array != null && array.Length > 0)
		{
			ObjectPlaceholder[] array2 = array;
			foreach (ObjectPlaceholder objectPlaceholder in array2)
			{
				objectPlaceholder.createObject(OnBrawlerPlaceholderObjectLoaded);
				pendingPlaceholderObjects++;
			}
		}
		else
		{
			if (brawlerStartTransaction != null)
			{
				brawlerStartTransaction.CompleteStep("load");
			}
			if (restartTransaction != null)
			{
				restartTransaction.CompleteStep("load");
			}
		}
	}

	private void OnBrawlerPlaceholderObjectLoaded(bool success)
	{
		if (!success)
		{
			if (brawlerStartTransaction != null)
			{
				brawlerStartTransaction.FailStep("load", "Placeholder object failed to spawn");
			}
			if (restartTransaction != null)
			{
				restartTransaction.FailStep("load", "Placeholder object failed to spawn");
			}
		}
		else if (--pendingPlaceholderObjects == 0)
		{
			if (brawlerStartTransaction != null)
			{
				brawlerStartTransaction.CompleteStep("load");
			}
			if (restartTransaction != null)
			{
				restartTransaction.CompleteStep("load");
			}
		}
	}

	private void OnScenarioPrefetchedIgnore(AssetBundleLoadResponse response, object extraData)
	{
	}

	private void ClearCurrentAssets()
	{
		CspUtils.DebugLog("ClearCurrentAssets : " + ((mission == null) ? "<null>" : mission.Id));
		AppShell.Instance.ServerConnection.SetUserVariable(READY_KEY, false);
		if (mission != null)
		{
			CspUtils.DebugLog("Unloading existing brawler assets " + mission.GeometryBundle() + " and " + mission.ScenarioBundle());
			UnityEngine.Object.Destroy(brawlerScenario);
			ClearObjects();
			ObjectFromPlaceholder[] array = UnityEngine.Object.FindObjectsOfType(typeof(ObjectFromPlaceholder)) as ObjectFromPlaceholder[];
			ObjectFromPlaceholder[] array2 = array;
			foreach (ObjectFromPlaceholder objectFromPlaceholder in array2)
			{
				UnityEngine.Object.Destroy(objectFromPlaceholder.gameObject);
			}
			playerSpawners = new List<CharacterSpawn>();
			ResetPrespawnData();
			currentObjective.Reset();
			objectiveMet = false;
			waitingForNotification = false;
			PlayerCombatController.PlayerListClear();
			CombatController.ClearFactionLists();
			if (AppShell.Instance.ServerConnection.IsGameHost())
			{
				AppShell.Instance.ServerConnection.ResetAllOwnership();
			}
			AppShell.Instance.ServerConnection.Game.ClientUnready();
			SetWaypointAnchor(null);
		}
	}

	protected void OnCharacterSelectTimer(AirlockTimerMessage message)
	{
		CspUtils.DebugLog("Air lock time captured.");
		if (message.Type != AirlockTimerMessage.AirlockTimerType.Complete)
		{
			return;
		}
		if (gettingReadyToSpawnTransaction == null && gettingReadyToReactivateTransaction == null)
		{
			CspUtils.DebugLog("Neither spawning transaction available.  Can't set step for airlock timer complete");
			return;
		}
		if (gettingReadyToSpawnTransaction != null)
		{
			gettingReadyToSpawnTransaction.CompleteStep("airlock_timer_complete");
		}
		if (gettingReadyToReactivateTransaction != null)
		{
			gettingReadyToReactivateTransaction.CompleteStep("airlock_timer_complete");
		}
	}

	protected void OnPlayerCharacterRequested(CharacterRequestedMessage message)
	{
		CspUtils.DebugLog("Requesting: " + message.CharacterName);
		AppShell.Instance.ServerConnection.TakeOwnership(message.CharacterName, false);
		if ((AppShell.Instance.ServerConnection.State & NetworkManager.ConnectionState.ConnectedToGame) == 0)
		{
			CspUtils.DebugLog("Player selected when not in a room");
		}
	}

	protected void OnPlayerCharacterSelected(CharacterSelectedMessage message)
	{
		if (message.sender == null)
		{
			UserProfile profile = AppShell.Instance.Profile;
			ActiveMission activeMission = AppShell.Instance.SharedHashTable["ActiveMission"] as ActiveMission;
			if (activeMission != null)
			{
				activeMission.SelectedCostume = message.CharacterName;
				activeMission.SelectedR2 = message.R2Attack;
			}
			if (profile != null)
			{
				profile.SelectedCostume = message.CharacterName;
				profile.LastSelectedCostume = message.CharacterName;
				profile.PersistExtendedData();
				AppShell.Instance.SharedHashTable["SocialSpaceCharacter"] = message.CharacterName;
			}
			characterSelected = message.data;
			CspUtils.DebugLog("got char selection <" + characterSelected.name + ", " + characterSelected.r2Attack + "> - " + selectCharacterTransaction);
			if (selectCharacterTransaction != null)
			{
				AppShell.Instance.EventMgr.RemoveListener<CharacterSelectedMessage>(OnPlayerCharacterSelected);
				selectCharacterTransaction.CompleteStep("select_character");
			}
			else
			{
				CspUtils.DebugLog("selectCharacterTransaction transaction has disappeared. Can't set step for character select");
			}
		}
	}

	protected void OnPlayerCharacterReady(CharacterReadyMessage readyMessage)
	{
		UserProfile profile = AppShell.Instance.Profile;
		ActiveMission activeMission = AppShell.Instance.SharedHashTable["ActiveMission"] as ActiveMission;
		if (activeMission != null)
		{
			activeMission.SelectedCostume = readyMessage.SelectionBlock.name;
			activeMission.SelectedR2 = readyMessage.SelectionBlock.r2Attack;
		}
		if (profile != null)
		{
			profile.SelectedCostume = readyMessage.SelectionBlock.name;
			profile.LastSelectedCostume = readyMessage.SelectionBlock.name;
			profile.PersistExtendedData();
			AppShell.Instance.SharedHashTable["SocialSpaceCharacter"] = readyMessage.SelectionBlock.name;
		}
		characterSelected = readyMessage.SelectionBlock;
		CspUtils.DebugLog("got char selection <" + readyMessage.SelectionBlock.name + ", " + readyMessage.SelectionBlock.r2Attack + "> - " + selectCharacterTransaction);
		if (selectCharacterTransaction != null)
		{
			AppShell.Instance.EventMgr.RemoveListener<CharacterReadyMessage>(OnPlayerCharacterReady);
			selectCharacterTransaction.CompleteStep("select_character");
		}
		else
		{
			CspUtils.DebugLog("selectCharacterTransaction transaction has disappeared. Can't set step for character select");
		}
	}

	protected int CompareSpawners(CharacterSpawn x, CharacterSpawn y)
	{
		return string.Compare(x.name, y.name, true);
	}

	protected int GetSpawnerIndex()
	{
		playerSpawners.Sort(CompareSpawners);
		int[] gameAllUserIds = AppShell.Instance.ServerConnection.GetGameAllUserIds();
		Array.Sort(gameAllUserIds);
		int num = Array.IndexOf(gameAllUserIds, AppShell.Instance.ServerConnection.GetGameUserId());
		return num % playerSpawners.Count;
	}

	protected void OnReadyToSpawnPlayerCharacter(TransactionMonitor.ExitCondition exitCondition, string error, object userData)
	{
		CspUtils.DebugLog("ready to spawn player character <" + exitCondition.ToString() + ">");
		gettingReadyToSpawnTransaction = null;
		int num = (int)userData;
		if (exitCondition == TransactionMonitor.ExitCondition.Success || exitCondition == TransactionMonitor.ExitCondition.TimedOut)
		{
			if (playerSpawners.Count > 0)
			{
				StartCoroutine(WaitForPlayersToSpawn());
				int spawnerIndex = GetSpawnerIndex();
				if (isTestScene)
				{
					playerSpawners[spawnerIndex].SpawnPlayerCharacter(characterSelected);
				}
				else
				{
					CspUtils.DebugLog("spawning player character <" + mission.SelectedCostume + ">.");
					characterSelected.name = mission.SelectedCostume;
					playerSpawners[spawnerIndex].SpawnPlayerCharacter(characterSelected);
				}
				ReadyStageParameters();
				AppShell.Instance.EventMgr.RemoveListener<AirlockTimerMessage>(OnCharacterSelectTimer);
			}
			else
			{
				gettingReadyToSpawnTransaction = TransactionMonitor.CreateTransactionMonitor("Ready to Spawn Transaction", OnReadyToSpawnPlayerCharacter, 1f, num + 1);
				gettingReadyToSpawnTransaction.AddStep("never_used");
				CspUtils.DebugLog("Trying (" + num + ") to spawn the player character, but no spawners are available!  Waiting for 1 second to try again.");
			}
		}
		else
		{
			CspUtils.DebugLog("gettingReadyToSpawnTransaction failed <" + exitCondition + "> with error <" + error + ">.");
			AppShell.Instance.CriticalError(SHSErrorCodes.Code.BrawlerCantSpawnPlayer, error);
		}
	}

	protected IEnumerator WaitForPlayersToSpawn()
	{
		float timeout = Time.time + 10f;
		PlayerCombatController[] players;
		while (true)
		{
			if (Time.time < timeout)
			{
				players = Utils.FindObjectsOfType<PlayerCombatController>();
				if (players.Length >= AppShell.Instance.ServerConnection.GetGameUserCount())
				{
					break;
				}
				yield return new WaitForSeconds(1f);
				continue;
			}
			yield break;
		}
		OnAllPlayersSpawned(players);
	}

	protected void OnAllPlayersSpawned(PlayerCombatController[] players)
	{
		PlayPostSpawnVO(players);
	}

	protected void PlayPostSpawnVO(PlayerCombatController[] players)
	{
		if (AppShell.Instance.ServerConnection.IsGameHost())
		{
			ResolvedVOAction[] array = GatherRelationships(players);
			if (array.Length > 0)
			{
				VOManager.Instance.PlayResolvedVO(array[UnityEngine.Random.Range(0, array.Length)]);
			}
		}
	}

	protected ResolvedVOAction[] GatherRelationships(PlayerCombatController[] players)
	{
		List<ResolvedVOAction> list = new List<ResolvedVOAction>();
		foreach (PlayerCombatController playerCombatController in players)
		{
			foreach (PlayerCombatController playerCombatController2 in players)
			{
				if (playerCombatController != playerCombatController2)
				{
					ResolvedVOAction vO = VOManager.Instance.GetVO("relationships", playerCombatController.gameObject, new VOInputString(playerCombatController2.name));
					if (vO != null && vO.IsResolved)
					{
						CspUtils.DebugLog("Found VO relationship: " + playerCombatController.name + " -> " + playerCombatController2.name);
						vO.CustomSubmixerName = "mission_start_queue";
						list.Add(vO);
					}
				}
			}
		}
		return list.ToArray();
	}

	protected void PrepRelationshipVO()
	{
		VOHooks hooks = VOManager.Instance.Hooks;
		hooks.OnResolvedVOPlayed = (VOHooks.ResolvedVOPlayedDelegate)Delegate.Combine(hooks.OnResolvedVOPlayed, new VOHooks.ResolvedVOPlayedDelegate(OnResolvedVOPlayed));
	}

	protected void OnResolvedVOPlayed(ResolvedVOAction action)
	{
		if (action.VOAction.Name == "relationships")
		{
			action.CustomHandler = new RelationshipsVOHandler();
		}
	}

	public override bool AddPlayerCharacterSpawner(CharacterSpawn possibleSpawner)
	{
		CspUtils.DebugLog("spawner reported: " + possibleSpawner.gameObject.name);
		playerSpawners.Add(possibleSpawner);
		return isTestScene;
	}

	private void Update()
	{
		if (isTestScene && selectCharacterTransaction != null && !selectCharacterTransaction.IsStepCompleted("select_character") && playerSpawners.Count >= 1 && (SHSBrawlerMainWindow)GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow"] != null)
		{
			AppShell.Instance.EventMgr.Fire(null, new CharacterReadyMessage(new CharacterSelectionBlock(playerSpawners[0].CharacterName, 1)));
			AppShell.Instance.EventMgr.Fire(null, new AirlockTimerMessage(AirlockTimerMessage.AirlockTimerType.Complete));
		}
		if (brawlerStartTransaction != null)
		{
			if (!isTestNetwork)
			{
				if (!brawlerStartTransaction.IsStepCompleted("ticket"))
				{
					ticket = (AppShell.Instance.SharedHashTable["BrawlerTicket"] as Matchmaker2.Ticket);
					if (ticket != null)
					{
						AppShell.Instance.SharedHashTable["BrawlerTicket"] = null;
						if (ticket.status == Matchmaker2.Ticket.Status.SUCCESS)
						{
							brawlerStartTransaction.CompleteStep("ticket");
							CspUtils.DebugLog("Brawler ConnectToGame() called!");
							AppShell.Instance.ServerConnection.ConnectToGame("shs.all", ticket, OnSeated);
						}
						else
						{
							brawlerStartTransaction.FailStep("ticket", ticket.ticket);
						}
						return;
					}
					if (isTestScene && !isTestNetwork)
					{
						brawlerStartTransaction.CompleteStep("ticket");
						AppShell.Instance.ServerConnection.ConnectToGame("shs.all", null, OnSeated);
					}
				}
			}
			else
			{
				if (!AppShell.Instance.IsReady)
				{
					CspUtils.DebugLog("AppShell not ready");
					return;
				}
				if (!brawlerStartTransaction.IsStepCompleted("ticket"))
				{
					brawlerStartTransaction.CompleteStep("ticket");
					RoomNetTest component = GetComponent<RoomNetTest>();
					component.Connect(OnSeated);
				}
			}
			if (brawlerStartTransaction != null)
			{
				brawlerStartTransaction.Update();
			}
		}
		if (waitingForAirlock)
		{
			IServerConnection serverConnection = AppShell.Instance.ServerConnection;
			if ((serverConnection.State & NetworkManager.ConnectionState.ConnectedToGame) != 0 && serverConnection.GetRoomIsActive())
			{
				int[] gameAllUserIds = serverConnection.GetGameAllUserIds();
				if (gameAllUserIds.Length > 0)
				{
					bool result = true;
					int[] array = gameAllUserIds;
					foreach (int userId in array)
					{
						bool flag = false;
						object userVariable = serverConnection.GetUserVariable(userId, READY_KEY);
						if (userVariable != null)
						{
							flag = bool.TryParse(userVariable.ToString(), out result);
							/////// this block added by CSP for testing  ////////////////
							// uservariable 'Ready' does not seem to be working...check this soon.
							CspUtils.DebugLog("GetUserVariable userId=" + userId + " result=" + result);
							// if (cspReady)
							// {
							// 	flag = true;
							// 	result = true;
							// 	AppShell.Instance.EventMgr.Fire(null, new CharacterReadyMessage(new CharacterSelectionBlock(heroCharacters[0], 1))); // kludge
							// }
							///////////////////////////////////////////////////////////////
						}
						if (!result || !flag)
						{
							result = false;
							//result = true;   // CSP - init to true instead of false for testing only.
							break;
						}
					}
					if (result)
					{
						waitingForAirlock = false;
						AppShell.Instance.EventMgr.Fire(this, new AirlockTimerMessage(AirlockTimerMessage.AirlockTimerType.Complete));
					}
					else if (!autoSelectOn)
					{
						bool flag2 = false;
						bool result2 = false;
						int gameHostId = serverConnection.GetGameHostId();
						object userVariable2 = serverConnection.GetUserVariable(gameHostId, READY_KEY);
						if (userVariable2 != null)
						{
							flag2 = bool.TryParse(userVariable2.ToString(), out result2);
						}
						if (flag2 && result2)
						{
							autoSelectOn = true;
							AppShell.Instance.EventMgr.Fire(this, new BrawlerAutoChooseStartMessage());
						}
					}
				}
			}
		}
		if (gettingReadyToSpawnTransaction != null)
		{
			gettingReadyToSpawnTransaction.Update();
		}
		if (restartTransaction != null)
		{
			restartTransaction.Update();
		}
		if (gettingReadyToReactivateTransaction != null)
		{
			gettingReadyToReactivateTransaction.Update();
		}
		if (waitingForNotification)
		{
			notificationWait += Time.deltaTime;
			if (notificationWait > notificationTimeOut)
			{
				AppShell.Instance.EventMgr.Fire(this, new BrawlerSummaryCompleteMessage());
			}
		}
		BrawlerUIUpdate();
		SpawnerUpdate();
		IndicatorUpdate();
		if (BrawlerStatManager.Active)
		{
			BrawlerStatManager.instance.UpdateComboLoss();
		}
		if (!objectiveMet && currentObjective != null && currentObjective.IsMet() && AppShell.Instance.ServerConnection.IsGameHost())
		{
			HandleStageEnd();
		}
	}

	public void ProcessStageComplete(int currentStage)
	{
		if (mission != null && mission.CurrentStage == currentStage && !objectiveMet)
		{
			HandleStageEnd();
		}
	}

	public void HandleStageEnd(int delay)
	{
		StartCoroutine(DelayHandleStageEnd(delay));
	}

	private IEnumerator DelayHandleStageEnd(int delay)
	{
		yield return new WaitForSeconds(delay);
		HandleStageEnd();
	}

	public void HandleStageEnd()
	{
		if (IsCutScenePlaying)
		{
			endStage = true;
			return;
		}
		objectiveMet = true;
		gameplayDoneTransaction.CompleteStep("ObjectiveCompleted");
		CspUtils.DebugLog("Adding BrawlerSummaryCompleteMessage listener");
		AppShell.Instance.EventMgr.AddListener<BrawlerSummaryCompleteMessage>(OnSummaryComplete);
		if (mission != null)
		{
			AppShell.Instance.EventReporter.ReportEnemyDefeatedAllUpdate();
			if (AppShell.Instance.ServerConnection.IsGameHost())
			{
				BrawlerStageCompleteMessage msg = new BrawlerStageCompleteMessage(mission.CurrentStage);
				AppShell.Instance.ServerConnection.SendGameMsg(msg);
			}
			if (mission.CurrentStage != mission.LastStage)
			{
				SetUIMode(BrawlerUIMode.StageComplete);
				return;
			}
			waitingForNotification = true;
			AppShell.Instance.EventReporter.ReportStageStatus(mission, mission.LastStage, false, characterSelected.name);
		}
		else
		{
			SetUIMode(BrawlerUIMode.MissionComplete);
		}
	}

	public override void OnNotificationResult(Hashtable msg)
	{
		CspUtils.DebugLog("Received brawler notification...");
		EventResultSet eventResultSet = AppShell.Instance.SharedHashTable["EventResults"] as EventResultSet;
		if (eventResultSet == null)
		{
			eventResultSet = new EventResultSet();
			AppShell.Instance.SharedHashTable["EventResults"] = eventResultSet;
		}
		if (eventResultSet.Enqueue(msg) && stageCompleteTransaction != null)
		{
			missionResultSet = eventResultSet;
			stageCompleteTransaction.CompleteStep("results");
		}
	}

	protected void ProcessResultSet(EventResultSet eventResultSet)
	{
		if (!object.ReferenceEquals(instance, this))
		{
			return;
		}
		EventResultMissionEvent eventResultMissionEvent = null;
		while (eventResultSet.Count > 0)
		{
			EventResultBase eventResultBase = eventResultSet.Dequeue();
			EventResultBase.EventResultType type = eventResultBase.type;
			if (type == EventResultBase.EventResultType.MissionEvent)
			{
				eventResultMissionEvent = (eventResultBase as EventResultMissionEvent);
			}
			else
			{
				CspUtils.DebugLog("Unhandled type of EventResult <" + eventResultBase.type + "> while processing event results set.");
			}
		}
		if (eventResultMissionEvent == null)
		{
			return;
		}
		waitingForNotification = false;
		if (forcedMedal != -1 && mission != null)
		{
			int a = mission.MissionDefinition.ScoreNeededForRating((MissionDefinition.Ratings)forcedMedal, eventResultMissionEvent.PlayerResults.Count);
			eventResultMissionEvent.TotalScore = Mathf.Max(a, eventResultMissionEvent.TotalScore);
		}
		SetUIMode(BrawlerUIMode.MissionComplete);
		OutputMissionResults(eventResultMissionEvent);
		ReportMissionResults(eventResultMissionEvent);
		StoreMissionResults(eventResultMissionEvent);
		UserProfile profile = AppShell.Instance.Profile;
		PlayerDictionary.Player value;
		AppShell.Instance.PlayerDictionary.TryGetValue(AppShell.Instance.ServerConnection.GetGameUserId(), out value);
		long key = -1L;
		if (value != null)
		{
			key = value.PlayerId;
		}
		MissionResults missionResults = eventResultMissionEvent.PlayerResults[key];
		if (missionResults != null)
		{
			HeroPersisted heroPersisted = profile.AvailableCostumes[missionResults.heroName];
			if (heroPersisted != null)
			{
				heroPersisted.UpdateFromResult(missionResults);
			}
		}
		profile.StartCurrencyFetch();
	}

	protected void OnSummaryComplete(BrawlerSummaryCompleteMessage e)
	{
		if (ScenarioEventManager.Instance != null)
		{
			ScenarioEventManager.Instance.ClearEventRecord();
		}
		AppShell.Instance.EventMgr.RemoveListener<BrawlerSummaryCompleteMessage>(OnSummaryComplete);
		if (mission == null)
		{
			CspUtils.DebugLog("Missionless brawler stage recieved OnSummaryComplete.  Good job");
			return;
		}
		CspUtils.DebugLog("OnSummaryComplete: " + mission.CurrentStage);
		if (mission.CurrentStage == mission.LastStage)
		{
			ResetLoadingScreen();
			if (mission != null)
			{
				mission.SelectedCostume = null;
			}
			if (AppShell.Instance.SharedHashTable["SocialSpaceLevel"] != null)
			{
				AppShell.Instance.Transition(ControllerType.SocialSpace);
			}
			else
			{
				AppShell.Instance.Transition(ControllerType.SocialSpace);
			}
			return;
		}
		StartExtraData startExtraData = new StartExtraData();
		startExtraData.bundleName = mission.ScenarioBundle();
		CspUtils.DebugLog("Attempting to load " + mission.GeometryBundle() + " and " + mission.ScenarioBundle());
		restartTransaction = TransactionMonitor.CreateTransactionMonitor("Restart Transaction", OnRestartTransactionComplete, 120f, startExtraData);
		restartTransaction.AddStep("load", TransactionMonitor.DumpTransactionStatus, "#load_screen_brawler_loaded_level");
		if (mission.CurrentStage == mission.LastStage - 1)
		{
			//ShsAudioSourceList.GetList("MissionResultsScreen").PreloadAll(restartTransaction);  // CSP temporary commented out
		}
		gettingReadyToReactivateTransaction = TransactionMonitor.CreateTransactionMonitor("Reactivate Transaction", OnReadyToReactivatePlayerCharacter, float.MaxValue, 0);
		gettingReadyToReactivateTransaction.AddStep("load_level_spawners", TransactionMonitor.DumpTransactionStatus, "#load_screen_brawler_loaded_characters");
		gettingReadyToReactivateTransaction.AddStep("airlock_timer_complete", TransactionMonitor.DumpTransactionStatus, "#load_screen_brawler_selected_character");
		TransitionHandler.WWTransitionProperties propertiesForTransition = new TransitionHandler.WWTransitionProperties(false, false, false, "SHSBrawlerWaitWindow", string.Empty);
		propertiesForTransition.locationInfo = new GUILoadingScreenContext.LocationInfo("#load_screen_brawler_title");
		propertiesForTransition.loadingContext = new GUILoadingScreenContext.LoadingContext(string.Empty, string.Empty);
		propertiesForTransition.transactionContext = new TransactionLoadingContext.TransactionContext(new List<string>(new string[1]
		{
			"gameplay_ready"
		}), "brawler_start_transaction");
		propertiesForTransition.loadingContext.customSetup = delegate(SHSWaitWindow win)
		{
			if (win is SHSBrawlerWaitWindow)
			{
				((SHSBrawlerWaitWindow)win).SetUIVisibility();
			}
		};
		TransitionHandler transitionHandler = AppShell.Instance.TransitionHandler;
		transitionHandler.SetupTransition(propertiesForTransition);
		SHSBrawlerWaitWindow sHSBrawlerWaitWindow = (SHSBrawlerWaitWindow)transitionHandler.CurrentWaitWindow;
		sHSBrawlerWaitWindow.ChangeLoadScreen(GetLoadingScreen(mission));
		StartTransaction = transitionHandler.CurrentTransactionContext.Transaction;
		StartTransaction.AddChild(gettingReadyToReactivateTransaction);
		StartTransaction.AddChild(restartTransaction);
		SetUIMode(BrawlerUIMode.Loading);
		ClearCurrentAssets();
		int num = mission.StartNextStage();
		CspUtils.DebugLog("The next stage is ID: " + num);
		waitingForAirlock = true;
		autoSelectOn = false;
		AppShell.Instance.EventMgr.AddListener<AirlockTimerMessage>(OnCharacterSelectTimer);
		FetchMissionAssets(startExtraData);
		gameplayDoneTransaction = TransactionMonitor.CreateTransactionMonitor("Gameplay Done Transaction", OnGameplayComplete, 0f, null);
		gameplayDoneTransaction.AddStep("ObjectiveCompleted");
		SetupResultsTransaction();
	}

	protected void OnRestartTransactionComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		restartTransaction = null;
		AppShell.Instance.ServerConnection.Game.ClientReady();
		if (exit == TransactionMonitor.ExitCondition.Success)
		{
			ActivatePrespawns(gettingReadyToReactivateTransaction);
			if (isTestScene)
			{
			}
		}
		else
		{
			gettingReadyToReactivateTransaction.FailStep("load_level_spawners", error);
		}
	}

	protected void OnReadyToReactivatePlayerCharacter(TransactionMonitor.ExitCondition exitCondition, string error, object userData)
	{
		CspUtils.DebugLog("ready to reactivate player character <" + exitCondition.ToString() + ">");
		gettingReadyToReactivateTransaction = null;
		int num = (int)userData;
		if (exitCondition == TransactionMonitor.ExitCondition.Success || exitCondition == TransactionMonitor.ExitCondition.TimedOut)
		{
			if (playerSpawners.Count > 0)
			{
				int spawnerIndex = GetSpawnerIndex();
				playerSpawners[spawnerIndex].SpawnPlayerCharacter(characterSelected);
				ReadyStageParameters();
				AppShell.Instance.EventMgr.RemoveListener<AirlockTimerMessage>(OnCharacterSelectTimer);
			}
			else
			{
				gettingReadyToReactivateTransaction = TransactionMonitor.CreateTransactionMonitor("Ready to Reactivate Transaction", OnReadyToReactivatePlayerCharacter, 1f, num + 1);
				gettingReadyToReactivateTransaction.AddStep("never_used");
				CspUtils.DebugLog("Trying (" + num + ") to reactivate the player character, but no spawners are available!  Waiting for 1 second to try again.");
			}
		}
		else
		{
			CspUtils.DebugLog("gettingReadyToReactivateTransaction failed <" + exitCondition + "> with error <" + error + ">.");
		}
	}

	protected void ReadyStageParameters()
	{
		if (mission != null)
		{
			AppShell.Instance.EventReporter.ReportStageStatus(mission, mission.CurrentStage, true, mission.SelectedCostume);
			if (mission.MissionDefinition != null && mission.CurrentStage >= 1 && mission.CurrentStage <= mission.LastStage)
			{
				StageDefinition stageDefinition = mission.MissionDefinition.StageDefinition(mission.CurrentStage);
				objectiveText = stageDefinition.Orders;
			}
		}
		UIStart();
		if (AppShell.Instance != null && AppShell.Instance.EventMgr != null && mission != null)
		{
			AppShell.Instance.EventMgr.Fire(this, new BrawlerStageBegin(mission.CurrentStage));
		}
		allowPickupSpawn = true;
	}

	public override void OnOldControllerUnloading(AppShell.GameControllerTypeData currentGameData, AppShell.GameControllerTypeData newGameData)
	{
		AppShell.Instance.EventReporter.ReportEnemyDefeatedAllUpdate();
		UiDisable();
		base.OnOldControllerUnloading(currentGameData, newGameData);
		AppShell.Instance.ServerConnection.DisconnectFromGame();
		brawlerStartTransaction = null;
		gettingReadyToSpawnTransaction = null;
		UserProfile profile = AppShell.Instance.Profile;
		if (profile != null)
		{
			profile.SelectedCostume = null;
		}
	}

	public void ClearObjects()
	{
		ClearObjects(true);
	}

	public void ClearObjects(bool includingPlayer)
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			GameObject gameObject = (GameObject)array2[i];
			if (!(gameObject != null))
			{
				continue;
			}
			if (Utils.GetComponent<CharacterController>(gameObject) != null)
			{
				if (Utils.GetComponent<PlayerInputController>(gameObject) != null)
				{
					CharacterStats component = Utils.GetComponent<CharacterStats>(gameObject);
					CharacterStat stat = component.GetStat(CharacterStats.StatType.Power);
					stat.Value = 0f;
					if (!includingPlayer)
					{
						continue;
					}
				}
				SpawnData component2 = Utils.GetComponent<SpawnData>(gameObject);
				component2.Despawn(EntityDespawnMessage.despawnType.defeated);
				UnityEngine.Object.Destroy(gameObject);
			}
			else if (Utils.GetComponent<CharacterSpawn>(gameObject) != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			else if (Utils.GetComponent<SpawnController>(gameObject) != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			else if (Utils.GetComponent<ProjectileColliderController>(gameObject) != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			else if (Utils.GetComponent<BrawlerTrapBase>(gameObject) != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			else if (Utils.GetComponent<ThrowableCarry>(gameObject) != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			else if (Utils.GetComponent<ThrowableGround>(gameObject) != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			else if (Utils.GetComponent<Pickup>(gameObject) != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
		IndicatorClear();
	}

	protected void OnGameplayComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		gameplayDoneTransaction = null;
		switch (exit)
		{
		case TransactionMonitor.ExitCondition.Fail:
		case TransactionMonitor.ExitCondition.TimedOut:
			CspUtils.DebugLog("OnGameplayComplete error <" + exit.ToString() + ">, <" + (error ?? "unknown") + ">");
			break;
		case TransactionMonitor.ExitCondition.Success:
			if (MissionEndHandler == null || MissionEndHandler.OnMissionEndDiorama())
			{
				AppShell.Instance.EventMgr.Fire(null, new BrawlerMissionCompleteMessage());
				CspUtils.DebugLog("Attempting to switch to diorama");
				ClearObjects();
				SwitchToDiorama();
			}
			allowPickupSpawn = false;
			if (stageCompleteTransaction != null)
			{
				stageCompleteTransaction.CompleteStep("diorama");
			}
			break;
		}
	}

	// this method added by CSP to send mission results to server.
	protected void ReportMissionResults(EventResultMissionEvent results)
	{
		// send mission results (for this game clien't player only) to server.
		int playerID = AppShell.Instance.ServerConnection.getNotificationServer().PlayerId;
		PlayerDictionary.Player value;
		AppShell.Instance.PlayerDictionary.TryGetValue(AppShell.Instance.ServerConnection.GetGameUserId(), out value);
		long key = -1L;
		if (value != null)
		{
			key = value.PlayerId;
		}
		//int key = AppShell.Instance.ServerConnection.getNotificationServer().UserId;

		Hashtable playerResHT = new Hashtable();


		MissionResults missionResults = results.PlayerResults[key];
		  
		playerResHT.Add("userID", playerID);
		playerResHT.Add("hero", AppShell.Instance.Profile.SelectedCostume);
		playerResHT.Add("xp", missionResults.earnedXp + missionResults.bonusXp);
		playerResHT.Add("coins", missionResults.coins);
		playerResHT.Add("tickets", missionResults.tickets);			
		
		AppShell.Instance.EventReporter.ReportMissionResults(playerResHT);
	}
	protected void OutputMissionResults(EventResultMissionEvent results)
	{
		string empty = string.Empty;
		empty += "<-@@@@@@@@@@ Begin Mission Result Data @@@@@@@@@@->\n";
		string text = empty;
		empty = text + "Total Player KOS: " + results.TotalKOs + "\n";
		text = empty;
		empty = text + "Total Score: " + results.TotalScore + "\n";
		empty += "Begin Per-Player Result Data\n";
		foreach (long key in results.PlayerResults.Keys)
		{
			MissionResults missionResults = results.PlayerResults[key];
			text = empty;
			empty = text + "Results for Player with ID: " + key + "\n";
			empty = empty + "\tHero Name: " + missionResults.heroName + "\n";
			text = empty;
			empty = text + "\tLeveled Up: " + missionResults.levelUp + "\n";
			text = empty;
			empty = text + "\tCoins: " + missionResults.coins + "\n";
			text = empty;
			empty = text + "\tTickets: " + missionResults.tickets + "\n";
			text = empty;
			empty = text + "\tEarned XP: " + missionResults.earnedXp + "\n";
			text = empty;
			empty = text + "\tBonus XP: " + missionResults.bonusXp + "\n";
			text = empty;
			empty = text + "\tSurvival Bonus: " + missionResults.survivalScore + "\n";
			text = empty;
			empty = text + "\tEnemy Ko Score: " + missionResults.enemyKoScore + "\n";
			text = empty;
			empty = text + "\tGimmick Score: " + missionResults.gimmickScore + "\n";
			text = empty;
			empty = text + "\tCombo Bonus: " + missionResults.comboScore + "\n";
			text = empty;
			empty = text + "\tXp Total: " + missionResults.currentXp + "\n";
			empty = empty + "\tOwnables: " + missionResults.ownable + "\n";
		}
		CspUtils.DebugLog(empty);
	}

	protected void SetupResultsTransaction()
	{

		if (mission.CurrentStage == mission.LastStage)
		{
			stageCompleteTransaction = TransactionMonitor.CreateTransactionMonitor("Stage Complete Transaction", OnReadyForMissionComplete, 120f, null);
			stageCompleteTransaction.AddStep("results", TransactionMonitor.DumpTransactionStatus);
			stageCompleteTransaction.AddStep("diorama", TransactionMonitor.DumpTransactionStatus);
		}
	}

	protected void OnReadyForMissionComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		stageCompleteTransaction = null;
		switch (exit)
		{
		case TransactionMonitor.ExitCondition.Fail:
		case TransactionMonitor.ExitCondition.TimedOut:
			CspUtils.DebugLog("OnReadyForMissionComplete error <" + exit.ToString() + ">, <" + (error ?? "unknown") + ">");
			break;
		case TransactionMonitor.ExitCondition.Success:
			ProcessResultSet(missionResultSet);
			break;
		}
	}

	public bool pvpEnabled()
	{
		return false;
	}

	public void spawnPickup(string newPickupName, Vector3 newPosition, GoNetId newID)
	{
		//UnityEngine.Object @object = brawlerBundle.Load(newPickupName);
		UnityEngine.Object @object = CspUtils.CspLoad(brawlerBundle, newPickupName); // CSP   

		if (@object == null)
		{
			CspUtils.DebugLog("pickup " + newPickupName + " does not exist");
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(@object, newPosition, Quaternion.identity) as GameObject;
		if (newID.IsValid())
		{
			NetworkComponent networkComponent = gameObject.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
			if (networkComponent != null)
			{
				networkComponent.goNetId = newID;
			}
		}
		else
		{
			NetworkComponent networkComponent2 = gameObject.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
			GoNetId invalid = GoNetId.Invalid;
			if (networkComponent2 == null)
			{
				CspUtils.DebugLog("Pickup " + newPickupName + " does not have a network component and will not be synchronized");
				return;
			}
			networkComponent2.goNetId = AppShell.Instance.ServerConnection.Game.GetNewDynamicId();
			invalid = networkComponent2.goNetId;
			PickupSpawnMessage msg = new PickupSpawnMessage(GoNetId.Invalid, newPickupName, gameObject.transform.position, invalid);
			AppShell.Instance.ServerConnection.SendGameMsg(msg);
		}
		AppShell.Instance.EventMgr.Fire(base.gameObject, new EntitySpawnMessage(gameObject, CharacterSpawn.Type.Unknown));
	}

	public PickupData getPickupData(string pickupID)
	{
		if (pickupDataDictionary == null)
		{
			return null;
		}
		PickupData value = null;
		if (!pickupDataDictionary.TryGetValue(pickupID, out value))
		{
			CspUtils.DebugLog("BrawlerController: Request for unknown pickup ID " + pickupID);
		}
		return value;
	}

	public string pickDrop(string dropTableName)
	{
		DropTable value;
		dropTableDictionary.TryGetValue(dropTableName, out value);
		if (value == null)
		{
			CspUtils.DebugLog("Drop table " + dropTableName + " not found in pickDrop");
			return null;
		}
		float num = UnityEngine.Random.Range(0, 100);
		for (int i = 0; i < value.chance.Length; i++)
		{
			if (num <= value.chance[i])
			{
				return value.pickupPrefab[i];
			}
		}
		return null;
	}

	public override void OnCutSceneStart()
	{
		base.OnCutSceneStart();
		isCutScenePlaying = true;
		SetUIMode(BrawlerUIMode.CutScene);
		if (BrawlerHud != null)
		{
			BrawlerHud.Hide();
		}
	}

	public override void OnCutSceneEnd()
	{
		base.OnCutSceneEnd();
		isCutScenePlaying = false;
		SetUIMode(BrawlerUIMode.LevelContinue);
		if (BrawlerHud != null)
		{
			BrawlerHud.Show();
		}
		else
		{
			CspUtils.DebugLog("Tried to show the Brawler HUD in OnCutSceneEnd, but it was null.");
		}
		if (endStage)
		{
			endStage = false;
			HandleStageEnd();
		}
	}

	public bool UseDebugPlayerCount()
	{
		return Instance.debugPlayerCount > 0;
	}

	protected override void RegisterDebugKeys()
	{
		KeyCodeEntry keyCodeEntry = new KeyCodeEntry(KeyCode.W, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, CompleteMission);
		keyCodeEntry = new KeyCodeEntry(KeyCode.B, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, ShowClickBoxes);
		keyCodeEntry = new KeyCodeEntry(KeyCode.N, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, ShowAttackChainNumbers);
		keyCodeEntry = new KeyCodeEntry(KeyCode.C, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, ShowAttackColliders);
		keyCodeEntry = new KeyCodeEntry(KeyCode.U, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, CycleSuperActivationMode);
		keyCodeEntry = new KeyCodeEntry(KeyCode.Insert, false, false, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, ShowDioramaObjects);
		keyCodeEntry = new KeyCodeEntry(KeyCode.Q, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, DefeatActiveEnemies);
		keyCodeEntry = new KeyCodeEntry(KeyCode.Quote, false, false, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, ToggleRegen);
		keyCodeEntry = new KeyCodeEntry(KeyCode.F8, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, ReloadActiveCharacters);
		keyCodeEntry = new KeyCodeEntry(KeyCode.Backslash, false, false, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, ToggleEverything);
		keyCodeEntry = new KeyCodeEntry(KeyCode.Backslash, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, ToggleHarmless);
		keyCodeEntry = new KeyCodeEntry(KeyCode.F7, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, ToggleDumpAIChoices);
		keyCodeEntry = new KeyCodeEntry(KeyCode.H, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, ShowActiveCharacterCombinations);
		keyCodeEntry = new KeyCodeEntry(KeyCode.R, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, CycleRAttack);
		if (clickBoxVisualizerPrefab == null)
		{
			clickBoxVisualizerPrefab = (Resources.Load("Character/ClickBoxCapsuleVisualizer") as GameObject);
		}
		base.RegisterDebugKeys();
	}

	private void RegisterDebugEvents()
	{
		AppShell.Instance.EventMgr.AddListener<CompleteMissionMessage>(OnCompleteMissionMessage);
		AppShell.Instance.EventMgr.AddListener<ShowAttackChainNumbersMessage>(OnShowAttackChainNumbersMessage);
		AppShell.Instance.EventMgr.AddListener<ShowClickBoxesMessage>(OnShowClickBoxesMessage);
		AppShell.Instance.EventMgr.AddListener<ShowAttackCollidersMessage>(OnShowAttackCollidersMessage);
	}

	private void UnregisterDebugEvents()
	{
		AppShell.Instance.EventMgr.RemoveListener<CompleteMissionMessage>(OnCompleteMissionMessage);
		AppShell.Instance.EventMgr.RemoveListener<ShowAttackChainNumbersMessage>(OnShowAttackChainNumbersMessage);
		AppShell.Instance.EventMgr.RemoveListener<ShowClickBoxesMessage>(OnShowClickBoxesMessage);
		AppShell.Instance.EventMgr.RemoveListener<ShowAttackCollidersMessage>(OnShowAttackCollidersMessage);
	}

	private void OnCompleteMissionMessage(CompleteMissionMessage msg)
	{
		CompleteMission(new SHSKeyCode(KeyCode.W));
	}

	[Description("Complete Mission")]
	private void CompleteMission(SHSKeyCode code)
	{
		if (waitingForNotification)
		{
			SetUIMode(BrawlerUIMode.MissionComplete);
		}
		if (gameplayDoneTransaction != null)
		{
			HandleStageEnd();
		}
	}

	private void OnShowClickBoxesMessage(ShowClickBoxesMessage msg)
	{
		ShowClickBoxes(null);
	}

	[Description("Show click boxes")]
	private void ShowClickBoxes(SHSKeyCode code)
	{
		if (clickBoxVisualizerPrefab == null)
		{
			CspUtils.DebugLog("Click box visualizer prefab not available");
			return;
		}
		showClickBoxes = !showClickBoxes;
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		UnityEngine.Object[] array2 = array;
		Vector3 localScale = default(Vector3);
		foreach (UnityEngine.Object @object in array2)
		{
			GameObject gameObject = @object as GameObject;
			if (showClickBoxes && gameObject.name == "ClickBox")
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(clickBoxVisualizerPrefab) as GameObject;
				gameObject2.name = "ClickBoxVisualizer";
				Utils.AttachGameObject(gameObject, gameObject2);
				CapsuleCollider capsuleCollider = gameObject.GetComponent(typeof(CapsuleCollider)) as CapsuleCollider;
				gameObject2.transform.localPosition = capsuleCollider.center;
				localScale.x = capsuleCollider.radius / 0.5f;
				localScale.z = localScale.x;
				localScale.y = capsuleCollider.height / 2f;
				gameObject2.transform.localScale = localScale;
			}
			else if (!showClickBoxes && gameObject.name == "ClickBoxVisualizer")
			{
				UnityEngine.Object.Destroy(@object);
			}
		}
	}

	private void OnShowAttackChainNumbersMessage(ShowAttackChainNumbersMessage msg)
	{
		ShowAttackChainNumbers(new SHSKeyCode(KeyCode.Alpha2));
	}

	[Description("Show attack chain numbers")]
	private void ShowAttackChainNumbers(SHSKeyCode code)
	{
		CspUtils.DebugLog("Show attack chain numbers called");
		showAttackChainNumbers = !showAttackChainNumbers;
	}

	private void OnShowAttackCollidersMessage(ShowAttackCollidersMessage msg)
	{
		ShowAttackColliders(new SHSKeyCode(KeyCode.Alpha1));
	}

	[Description("Show attack colliders")]
	private void ShowAttackColliders(SHSKeyCode code)
	{
		CspUtils.DebugLog("Show attack colliders called");
		showAttackColliders = !showAttackColliders;
	}

	[Description("Cycle Super Activation Mode")]
	private void CycleSuperActivationMode(SHSKeyCode code)
	{
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(PlayerInputController));
		if (@object != null)
		{
			PlayerInputController playerInputController = @object as PlayerInputController;
			if (playerInputController.CurrentSuperActivationMode == PlayerInputController.SuperActivationMode.Hold)
			{
				playerInputController.CurrentSuperActivationMode = PlayerInputController.SuperActivationMode.BothButton;
			}
			else
			{
				playerInputController.CurrentSuperActivationMode++;
			}
			CspUtils.DebugLog("Current Super Activation Mode is now: " + playerInputController.CurrentSuperActivationMode);
		}
	}

	[Description("Show diorama objects")]
	private void ShowDioramaObjects(SHSKeyCode code)
	{
		showDioramaObjects = !showDioramaObjects;
		GameObject gameObject = GameObject.Find("Diorama");
		if (gameObject == null)
		{
			return;
		}
		UnityEngine.Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeof(DioramaPositioner));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			DioramaPositioner dioramaPositioner = componentsInChildren[i] as DioramaPositioner;
			MeshRenderer meshRenderer = dioramaPositioner.GetComponentInChildren(typeof(MeshRenderer)) as MeshRenderer;
			if (meshRenderer != null)
			{
				meshRenderer.enabled = showDioramaObjects;
			}
		}
	}

	[Description("Defeat Active enemies")]
	private void DefeatActiveEnemies(SHSKeyCode code)
	{
		CombatController.AttackData attackData = AttackDataManager.Instance.getAttackData("DebugCheatKillAttack");
		if (attackData == null)
		{
			return;
		}
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (localPlayer == null)
		{
			return;
		}
		CombatController combatController = localPlayer.GetComponent(typeof(CombatController)) as CombatController;
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(CharacterGlobals));
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			CharacterGlobals characterGlobals = (CharacterGlobals)array2[i];
			if (characterGlobals != null)
			{
				SpawnData spawnData = characterGlobals.spawnData;
				if (spawnData != null && ((spawnData.spawnType & CharacterSpawn.Type.AI) != 0 || (spawnData.spawnType & CharacterSpawn.Type.Boss) != 0))
				{
					combatController.attackHit(characterGlobals.transform.position, characterGlobals.combatController, attackData, attackData.impacts[0]);
				}
			}
		}
	}

	[Description("Reload active character!")]
	private void ReloadActiveCharacters(SHSKeyCode code)
	{
		AppShell.Instance.DataManager.ClearGameDataCache();
		AttackDataManager.Instance.ClearDataCache();
		ClearCharacterCache();
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			GameObject gameObject = (GameObject)array2[i];
			if (!(gameObject != null) || !(Utils.GetComponent<CharacterController>(gameObject) != null))
			{
				continue;
			}
			SpawnData spawnData = gameObject.GetComponent(typeof(SpawnData)) as SpawnData;
			if (!(spawnData != null))
			{
				continue;
			}
			Transform transform = gameObject.transform;
			NetworkComponent x = gameObject.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
			if (x != null)
			{
			}
			spawnData.Despawn(EntityDespawnMessage.despawnType.reload);
			UnityEngine.Object.Destroy(gameObject);
			GameObject gameObject2 = null;
			CharacterSpawn characterSpawn = null;
			if (spawnData.spawner != null)
			{
				CharacterSpawn spawner = spawnData.spawner;
				CspUtils.DebugLog("ReloadActiveCharacters " + spawnData.modelName);
				if (spawner.IsNetworked)
				{
					CspUtils.DebugLog("ReloadActiveCharacters IsNetworked " + spawnData.modelName);
				}
				if (spawner.IsPlayer)
				{
					spawner.SpawnPlayerCharacter(spawnData.modelName);
				}
				else
				{
					spawner.spawnFXPrefab = null;
					spawner.SpawnOnTime(0f);
				}
				spawner.transform.position = transform.position;
				spawner.transform.localScale = transform.localScale;
				spawner.transform.rotation = transform.rotation;
			}
			else
			{
				gameObject2 = (UnityEngine.Object.Instantiate(Resources.Load("Spawners/RemoteSpawner"), transform.position, transform.rotation) as GameObject);
				gameObject2.transform.localScale = transform.localScale;
				characterSpawn = (gameObject2.GetComponent(typeof(CharacterSpawn)) as CharacterSpawn);
				characterSpawn.SetCharacterName(spawnData.modelName);
				characterSpawn.IsLocal = ((spawnData.spawnType & CharacterSpawn.Type.Local) != 0);
				characterSpawn.IsPlayer = ((spawnData.spawnType & CharacterSpawn.Type.Player) != 0);
				characterSpawn.IsAI = ((spawnData.spawnType & CharacterSpawn.Type.AI) != 0);
				characterSpawn.IsBoss = ((spawnData.spawnType & CharacterSpawn.Type.Boss) != 0);
				characterSpawn.SpawnOnStart = true;
				characterSpawn.DestroyOnSpawn = true;
				characterSpawn.IsNetworked = false;
				CspUtils.DebugLog("ReloadActiveCharacters not networked " + spawnData.modelName);
				characterSpawn.goNetId = GoNetId.Invalid;
				characterSpawn.RecordHistory = false;
			}
			
		}
	}

	private void ToggleCombatEffectOnPlayer(string combatEffectName)
	{
		PlayerCombatController playerCombatController = null;
		foreach (PlayerCombatController player in PlayerCombatController.PlayerList)
		{
			if (player.CharGlobals != null && player.CharGlobals.spawnData != null && (player.CharGlobals.spawnData.spawnType & CharacterSpawn.Type.LocalPlayer) != 0)
			{
				playerCombatController = player;
				break;
			}
		}
		if (playerCombatController != null)
		{
			if (playerCombatController.currentActiveEffects.ContainsKey(combatEffectName))
			{
				playerCombatController.removeCombatEffect(combatEffectName);
			}
			else
			{
				playerCombatController.createCombatEffect(combatEffectName, playerCombatController, false);
			}
		}
	}

	[Description("Puts the 'Super' in Superhero!!")]
	private void ToggleEverything(SHSKeyCode code)
	{
		ToggleCombatEffectOnPlayer("CheatSuperStamina");
		ToggleCombatEffectOnPlayer("CheatSuperStrength");
		ToggleCombatEffectOnPlayer("CheatSuperCharged");
		ToggleCombatEffectOnPlayer("CheatSuperSpeed");
	}

	[Description("Toggles harmless mode, deal/receive no damage")]
	private void ToggleHarmless(SHSKeyCode code)
	{
		ToggleCombatEffectOnPlayer("CheatHarmless");
		ToggleCombatEffectOnPlayer("CheatSuperSpeed");
		ToggleCombatEffectOnPlayer("CheatSuperCharged");
	}

	[Description("Toggles regen mode")]
	private void ToggleRegen(SHSKeyCode code)
	{
		ToggleCombatEffectOnPlayer("CheatHealthRegen");
	}

	[Description("Toggles debug output of choices made by the AI combat controller")]
	private void ToggleDumpAIChoices(SHSKeyCode code)
	{
		dumpAIChoices = !dumpAIChoices;
		CspUtils.DebugLog("Turning " + ((!dumpAIChoices) ? "OFF" : "ON") + " AI choice logging");
	}

	[Description("Reshows active character combinations in stage one")]
	private void ShowActiveCharacterCombinations(SHSKeyCode code)
	{
		if (CharacterCombinationManager != null)
		{
			CharacterCombinationBridge characterCombinationBridge = UnityEngine.Object.FindObjectOfType(typeof(CharacterCombinationBridge)) as CharacterCombinationBridge;
			if (!(characterCombinationBridge == null))
			{
				characterCombinationBridge.ClearDisplayedCombinations();
				AppShell.Instance.EventMgr.Fire(null, new PresetCombinationsApplyMessage(CharacterCombinationManager.ActiveCombinations));
			}
		}
	}

	[Description("Cycle active character's secondary (R) attack.")]
	private void CycleRAttack(SHSKeyCode code)
	{
		PlayerCombatController playerCombatController = null;
		foreach (PlayerCombatController player in PlayerCombatController.PlayerList)
		{
			if (player.CharGlobals != null && player.CharGlobals.spawnData != null && (player.CharGlobals.spawnData.spawnType & CharacterSpawn.Type.LocalPlayer) != 0)
			{
				playerCombatController = player;
				break;
			}
		}
		if (playerCombatController != null)
		{
			playerCombatController.selectedSecondaryAttack++;
			if (playerCombatController.selectedSecondaryAttack > 3)
			{
				playerCombatController.selectedSecondaryAttack = 1;
			}
			CspUtils.DebugLog("Current secondary attack: " + playerCombatController.selectedSecondaryAttack);
		}
	}

	protected Transform[] GetDioramaPositionsBySize(out float[] offsets)
	{
		offsets = null;
		GameObject gameObject = GameObject.Find("Diorama");
		if (gameObject == null)
		{
			return null;
		}
		UnityEngine.Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeof(DioramaPositioner));
		if (componentsInChildren.Length < 4)
		{
			return null;
		}
		Transform[] array = new Transform[4];
		offsets = new float[4];
		DioramaPositioner.DioramaSizePosition[] array2 = new DioramaPositioner.DioramaSizePosition[4]
		{
			DioramaPositioner.DioramaSizePosition.Small,
			DioramaPositioner.DioramaSizePosition.Medium,
			DioramaPositioner.DioramaSizePosition.Large,
			DioramaPositioner.DioramaSizePosition.Huge
		};
		for (int i = 0; i < array.Length; i++)
		{
			DioramaPositioner.DioramaSizePosition dioramaSizePosition = array2[i];
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				DioramaPositioner dioramaPositioner = componentsInChildren[j] as DioramaPositioner;
				offsets[j] = dioramaPositioner.positionSnapOffset;
				if (dioramaPositioner.positionSlot == dioramaSizePosition)
				{
					array[i] = componentsInChildren[j].transform;
				}
			}
			if (array[i] == null)
			{
				CspUtils.DebugLog("No diorama position found for: " + dioramaSizePosition.ToString());
				return null;
			}
		}
		return array;
	}

	public int SmallerPlayer(BrawlerStatManager.CharacterScoreData player1, BrawlerStatManager.CharacterScoreData player2)
	{
		if (player1.characterSize > player2.characterSize)
		{
			return 1;
		}
		if (player1.characterSize < player2.characterSize)
		{
			return -1;
		}
		if (player1.multiplayerID < player2.multiplayerID)
		{
			return 1;
		}
		if (player1.multiplayerID > player2.multiplayerID)
		{
			return -1;
		}
		return 0;
	}

	protected void DisplayDioramaPlayers()
	{
		float[] offsets;
		Transform[] dioramaPositionsBySize = GetDioramaPositionsBySize(out offsets);
		if (dioramaPositionsBySize != null)
		{
			List<BrawlerStatManager.CharacterScoreData> allStatBlocks = BrawlerStatManager.instance.GetAllStatBlocks();
			allStatBlocks.Sort(SmallerPlayer);   
			while (allStatBlocks.Count > 4)
			{
				allStatBlocks.RemoveAt(4);
			}
			for (int i = 0; i < allStatBlocks.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Spawners/DioramaSpawner"), dioramaPositionsBySize[i].transform.position, dioramaPositionsBySize[i].transform.rotation) as GameObject;
				//DioramaCharacterSpawn component = gameObject.GetComponent<DioramaCharacterSpawn>();
				DioramaCharacterSpawn component = (DioramaCharacterSpawn)CspUtils.findComponentByType(gameObject, "DioramaCharacterSpawn");  // CSP
				component.SetCharacterName(allStatBlocks[i].modelName);
				//component.SetCharacterName(allStatBlocks[(allStatBlocks.Count - 1) - i].modelName);  // CSP test
				component.SetSnapOffset(offsets[i]);
				component.SetSpawnedCharacterCount(allStatBlocks.Count);
				allStatBlocks[i].dioramaPosition = dioramaPositionsBySize[i].transform.position;
			}
		}
	}

	protected void SwitchToDiorama()
	{
		if (!(CameraLiteManager.Instance != null))
		{
			return;
		}
		GameObject gameObject = GameObject.Find("DioramaCam");
		if (gameObject == null)
		{
			return;
		}
		CameraLite cameraLite = gameObject.GetComponent(typeof(CameraLite)) as CameraLite;
		if (cameraLite == null || CameraLiteManager.Instance == null)
		{
			return;
		}
		CameraLiteManager.Instance.PushCamera(cameraLite, 0f);
		GameObject gameObject2 = GameObject.FindGameObjectWithTag("MainCamera");
		if (gameObject2 != null)
		{
			AOSceneCamera aOSceneCamera = gameObject2.GetComponentInChildren(typeof(AOSceneCamera)) as AOSceneCamera;
			if (aOSceneCamera != null)
			{
				aOSceneCamera.DOFRange = 20f;
			}
		}
		DisplayDioramaPlayers();
		SetUIMode(BrawlerUIMode.Diorama);
		GameObject gameObject3 = GameObject.Find("Diorama");
		if (gameObject3 != null && gameObject3.layer == 23)
		{
			RenderSettings.ambientLight = ColorUtil.FromRGB255(120, 111, 156);
		}
		DioramaMode = true;
		
	}

	protected void LeaveDiorama()
	{
		if (DioramaMode)
		{
			CameraLiteManager.Instance.PopCamera(0f);
			DioramaMode = false;
			VOManager.Instance.StopAll();
		}
	}

	public void ShowRewardEffect()
	{
		GameObject gameObject = GameObject.Find("DioramaCam");
		if (!(gameObject == null))
		{
			UnityEngine.Object @object = brawlerBundle.Load("cascading_prize_sequence");
			if (@object == null)
			{
				CspUtils.DebugLog("Reward prefab does not exist or is not loaded!!!!");
				return;
			}
			rewardInstance = (UnityEngine.Object.Instantiate(@object) as GameObject);
			rewardInstance.transform.parent = gameObject.transform;
			rewardInstance.transform.localPosition = new Vector3(0f, -2.5f, 5f);
			rewardInstance.transform.localRotation = Quaternion.Euler(0f, -180f, 0f);
		}
	}

	protected void IndicatorSetup()
	{
		activeEnemies = new List<GameObject>();
		wayPoints = new List<GameObject>();
		wayPointEffects = new Dictionary<GameObject, GameObject>();
	}

	protected void IndicatorUiSetup()
	{
		UiArrow = (GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow/SHSBrawlerIndicatorArrow"] as SHSBrawlerIndicatorArrow);
	}

	protected void IndicatorClear()
	{
		if (activeEnemies != null)
		{
			activeEnemies.Clear();
		}
		if (wayPoints != null)
		{
			wayPoints.Clear();
		}
		currentIndicator = 0;
		objectDistance = 9000f;
		closestObject = null;
	}

	protected void RegisterIndicatorEvents()
	{
		AppShell.Instance.EventMgr.AddListener<EntitySpawnMessage>(OnEnemySpawned);
		AppShell.Instance.EventMgr.AddListener<EntitySpawnAnimationCompleteMessage>(OnEnemyReady);
		AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(OnEnemyDespawned);
		AppShell.Instance.EventMgr.AddListener<EntityPolymorphMessage>(OnEnemyPolymorph);
		AppShell.Instance.EventMgr.AddListener<CombatCharacterKilledMessage>(OnEnemyKilled);
	}

	protected void UnregisterIndicatorEvents()
	{
		AppShell.Instance.EventMgr.RemoveListener<EntitySpawnMessage>(OnEnemySpawned);
		AppShell.Instance.EventMgr.RemoveListener<EntitySpawnAnimationCompleteMessage>(OnEnemyReady);
		AppShell.Instance.EventMgr.RemoveListener<EntityDespawnMessage>(OnEnemyDespawned);
		AppShell.Instance.EventMgr.RemoveListener<EntityPolymorphMessage>(OnEnemyPolymorph);
		AppShell.Instance.EventMgr.RemoveListener<CombatCharacterKilledMessage>(OnEnemyKilled);
	}

	protected void SetWaypointAnchor(GameObject newAnchor)
	{
		UiArrow.SetIndicatorAnchor(newAnchor);
	}

	protected void IndicatorUpdate()
	{
		if (Camera.main == null)
		{
			return;
		}
		bool flag = false;
		List<GameObject> list = activeEnemies;
		if (wayPoints.Count > 0)
		{
			list = wayPoints;
			flag = true;
		}
		if (currentIndicator >= list.Count)
		{
			UpdateIndicatorTarget(closestObject);
			closestObject = null;
			currentIndicator = 0;
			objectDistance = 9000f;
			return;
		}
		Vector3 vector = Camera.main.WorldToViewportPoint(list[currentIndicator].transform.position);
		vector.z = 0f;
		if (vector.x < 0f || vector.y < 0f || vector.x > 1f || vector.y > 1f || flag)
		{
			if (vector.sqrMagnitude < objectDistance)
			{
				closestObject = list[currentIndicator];
				objectDistance = vector.sqrMagnitude;
			}
			currentIndicator++;
		}
		else
		{
			UpdateIndicatorTarget(null);
			currentIndicator = 0;
		}
	}

	protected void UpdateIndicatorTarget(GameObject toTrack)
	{
		if (UiArrow == null || UiArrow.arrowTarget == toTrack)
		{
			return;
		}
		GameObject arrowTarget = UiArrow.arrowTarget;
		UiArrow.arrowTarget = toTrack;
		UiArrow.SetIndicatorIcon(GetIndicatorIcon(toTrack));
		GameObject indicatorEffect = GetIndicatorEffect(arrowTarget);
		GameObject indicatorEffect2 = GetIndicatorEffect(toTrack);
		if (indicatorEffect == indicatorEffect2)
		{
			if (indicatorEffect != null)
			{
				Utils.AttachGameObject(toTrack, indicatorEffect);
				Utils.ActivateTree(indicatorEffect, true);
			}
			return;
		}
		if (indicatorEffect != null)
		{
			Utils.ActivateTree(indicatorEffect, false);
			Utils.DetachGameObject(indicatorEffect);
		}
		if (indicatorEffect2 != null)
		{
			Utils.AttachGameObject(toTrack, indicatorEffect2);
			Utils.ActivateTree(indicatorEffect2, true);
		}
	}

	public void RemoveIndicatorEnemy(GameObject enemy)
	{
		activeEnemies.Remove(enemy);
		if (UiArrow != null && UiArrow.arrowTarget == enemy)
		{
			UpdateIndicatorTarget(null);
		}
		if (closestObject == enemy)
		{
			closestObject = null;
		}
	}

	public void AddIndicatorEnemy(GameObject enemy)
	{
		if (!(enemy == null))
		{
			ScenarioEventIndicatorSuppressor component = enemy.GetComponent<ScenarioEventIndicatorSuppressor>();
			if ((!(component != null) || !component.indicatorSuppressed) && !activeEnemies.Contains(enemy))
			{
				activeEnemies.Add(enemy);
			}
		}
	}

	protected void OnEnemySpawned(EntitySpawnMessage e)
	{
		if ((e.spawnType & CharacterSpawn.Type.AI) == 0 || (e.spawnType & CharacterSpawn.Type.Boss) != 0)
		{
			return;
		}
		BehaviorManager component = Utils.GetComponent<BehaviorManager>(e.go);
		if (component != null)
		{
			BehaviorBase behavior = component.getBehavior();
			if (behavior is BehaviorSpawnAnimate || behavior is BehaviorGliderSpawn)
			{
				return;
			}
		}
		CombatController component2 = e.go.GetComponent<CombatController>();
		if (!component2.isHidden)
		{
			AddIndicatorEnemy(e.go);
		}
	}

	protected void OnEnemyReady(EntitySpawnAnimationCompleteMessage e)
	{
		AddIndicatorEnemy(e.go);
	}

	protected void OnEnemyKilled(CombatCharacterKilledMessage e)
	{
		RemoveIndicatorEnemy(e.Character);
	}

	protected void OnEnemyDespawned(EntityDespawnMessage e)
	{
		if ((e.type & CharacterSpawn.Type.AI) != 0 && (e.type & CharacterSpawn.Type.Boss) == 0)
		{
			RemoveIndicatorEnemy(e.go);
		}
		if (characterSelected != null && e.cause == EntityDespawnMessage.despawnType.defeated)
		{
			if ((e.type & CharacterSpawn.Type.AI) != 0)
			{
				AppShell.Instance.CounterManager.AddCounter("BestThereIsCounter", characterSelected.name, false);
				AchievementManager.recordEnemyKill(AchievementManager.EnemyType.Mob, characterSelected.name, 1);
			}
			if ((e.type & CharacterSpawn.Type.Boss) != 0)
			{
				AppShell.Instance.CounterManager.AddCounter("EvilExtinguisherCounter", characterSelected.name);
			}
		}
	}

	protected void OnEnemyPolymorph(EntityPolymorphMessage e)
	{
		if ((e.originalType & CharacterSpawn.Type.AI) != 0 && (e.originalType & CharacterSpawn.Type.Boss) == 0)
		{
			RemoveIndicatorEnemy(e.original);
		}
		if ((e.polymorphType & CharacterSpawn.Type.AI) != 0 && (e.polymorphType & CharacterSpawn.Type.Boss) == 0)
		{
			AddIndicatorEnemy(e.polymorph);
		}
	}

	public void AddWaypoint(GameObject newWaypoint, GameObject indicatorPrefab)
	{
		wayPoints.Add(newWaypoint);
		if (indicatorPrefab != null && !wayPointEffects.ContainsKey(indicatorPrefab))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(indicatorPrefab) as GameObject;
			if (gameObject == null)
			{
				CspUtils.DebugLog("Failed to create waypoint effect for waypoint <" + newWaypoint.name + ">");
			}
			else
			{
				wayPointEffects.Add(indicatorPrefab, gameObject);
			}
		}
	}

	public void RemoveWaypoint(GameObject toRemove)
	{
		wayPoints.Remove(toRemove);
		if (UiArrow != null && UiArrow.arrowTarget == toRemove)
		{
			UpdateIndicatorTarget(null);
		}
		if (closestObject == toRemove)
		{
			closestObject = null;
		}
	}

	public GameObject GetIndicatorEffect(GameObject toTrack)
	{
		if (toTrack == null)
		{
			return null;
		}
		IndicatorWaypoint component = toTrack.GetComponent<IndicatorWaypoint>();
		if (component != null && component.indicatorPrefab != null)
		{
			if (wayPointEffects.ContainsKey(component.indicatorPrefab))
			{
				return wayPointEffects[component.indicatorPrefab];
			}
			return null;
		}
		return null;
	}

	public string GetIndicatorIcon(GameObject toTrack)
	{
		if (toTrack == null)
		{
			return UiArrow.defaultIndicatorIcon;
		}
		IndicatorWaypoint component = toTrack.GetComponent<IndicatorWaypoint>();
		if (component != null && component.indicatorIcon != string.Empty)
		{
			return component.indicatorIcon;
		}
		return UiArrow.defaultIndicatorIcon;
	}

	protected void ResetPrespawnData()
	{
		if (characterPrefabs == null)
		{
			characterPrefabs = new Dictionary<string, GameObject>();
		}
		requestedSpawns = new List<CachedPrespawn>();
		prespawners = 0;
	}

	protected void ActivatePrespawns(TransactionMonitor currentStep)
	{
		CspUtils.DebugLog("Prespawning");
		loadStep = currentStep;
		int num = 0;
		int num2 = -1;
		if (heroCharacters != null)
		{
			num2 = heroCharacters.Count;
		}
		CharacterSpawn[] array = Utils.FindObjectsOfType<CharacterSpawn>();
		CharacterSpawn[] array2 = array;
		foreach (CharacterSpawn characterSpawn in array2)
		{
			if ((characterSpawn.GetSpawnType() & CharacterSpawn.Type.AI) > CharacterSpawn.Type.Unknown)
			{
				characterSpawn.Prespawn();
			}
			else if (_prespawnAIPlayers && (characterSpawn.GetSpawnType() & CharacterSpawn.Type.Player) > CharacterSpawn.Type.Unknown)
			{
				if (num < num2)
				{
					int num3 = 0;
					foreach (string heroCharacter in heroCharacters)
					{
						if (num3++ == num)
						{
							characterSpawn.SetCharacterName(heroCharacter);
							break;
						}
					}
					num++;
				}
				characterSpawn.ToggleAIPlayerPrespawn(true);
				characterSpawn.Prespawn();
			}
		}
		CspUtils.DebugLog("prespawners=" + prespawners); // CSP
		prespawners = 0; // CSP test - this allows Asgardian Gladiators to load, not sure what bad effect this change might have.
		if (prespawners == 0)
		{
			loadStep.CompleteStep("load_level_spawners");
			AppShell.Instance.ServerConnection.SetUserVariable(READY_KEY, true);
		}
	}

	protected void SpawnerUpdate()
	{
		int num = 0;
		while (num < requestedSpawns.Count)
		{
			CachedPrespawn cachedPrespawn = requestedSpawns[num];
			GameObject gameObject = characterPrefabs[cachedPrespawn.characterName];
			if (gameObject != null)
			{
				cachedPrespawn.requestee.SpawnFromPrefab(gameObject, cachedPrespawn.extraData);
				requestedSpawns.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
	}

	public bool IsCharacterCached(string characterName)
	{
		return characterPrefabs.ContainsKey(characterName);
	}

	public void StartCharacterCache(string characterName)
	{
		if (characterPrefabs.ContainsKey(characterName))
		{
			CspUtils.DebugLog("Preparing for precache after cached entry already exists!");
		}
		characterPrefabs[characterName] = null;
		prespawners++;
	}

	public void CacheCharacter(string characterName, GameObject prefab)
	{
		characterPrefabs[characterName] = prefab;
		prespawners--;
		if (prespawners == 0 && loadStep != null)
		{
			loadStep.CompleteStep("load_level_spawners");
			AppShell.Instance.ServerConnection.SetUserVariable(READY_KEY, true);
			loadStep = null;
		}
	}

	public bool SpawnCachedCharacter(string characterName, CharacterSpawnData initData, CharacterSpawn targetSpawnPoint)
	{
		if (!characterPrefabs.ContainsKey(characterName))
		{
			return false;
		}
		CachedPrespawn item = new CachedPrespawn(characterName, initData, targetSpawnPoint);
		requestedSpawns.Add(item);
		return true;
	}

	public void ClearCharacterCache()
	{
		characterPrefabs.Clear();
		requestedSpawns.Clear();
	}

	public bool IsCharacterBeingCached(string characterName)
	{
		return characterPrefabs.ContainsKey(characterName) && characterPrefabs[characterName] == null;
	}

	public void FailCacheCharacter(string characterName, string error)
	{
		characterPrefabs.Remove(characterName);
		CharacterSpawn[] array = Utils.FindObjectsOfType<CharacterSpawn>();
		CharacterSpawn[] array2 = array;
		foreach (CharacterSpawn characterSpawn in array2)
		{
			if ((characterSpawn.GetSpawnType() & CharacterSpawn.Type.AI) > CharacterSpawn.Type.Unknown || (_prespawnAIPlayers && (characterSpawn.GetSpawnType() & CharacterSpawn.Type.Player) > CharacterSpawn.Type.Unknown))
			{
				characterSpawn.HaltSpawn = true;
			}
		}
		loadStep.FailStep("load_level_spawners", error);
	}

	private void OnPresetCharacterCombination(PresetCombinationsRequestMessage msg)
	{
		heroCharacters = msg.characterNames;
	}

	protected void UIStart()
	{
		MissionEndHandler = null;
	}

	protected void BrawlerUIUpdate()
	{
		HandleObjectiveRefresh();
	}

	public void SetUIMode(BrawlerUIMode mode)
	{
		CspUtils.DebugLog("Changing Brawler UI State to: " + mode.ToString());
		SHSBrawlerMainWindow sHSBrawlerMainWindow = (SHSBrawlerMainWindow)GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow"];
		currentUIMode = mode;
		if (mode == BrawlerUIMode.Loading)
		{
			if (!isLoading)
			{
				isLoading = true;
			}
		}
		else if (isLoading)
		{
			isLoading = false;
		}
		if (mode == BrawlerUIMode.Airlock)
		{
			GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow/SHSBrawlerCharacterSelectionMainWindow"].Show();
		}
		else
		{
			GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow/SHSBrawlerCharacterSelectionMainWindow"].Hide();
		}
		switch (mode)
		{
		case BrawlerUIMode.Level:
			sHSBrawlerMainWindow.SetPowerBarVisibility(true);
			sHSBrawlerMainWindow.OrdersCanShow(true);
			SetObjectiveText(6f);
			GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow/SHSBrawlerIndicatorArrow"].Show();
			IndicatorUiSetup();
			if (bossToShow != null)
			{
				AppShell.Instance.EventMgr.Fire(null, new BossAIControllerBrawler.BossBattleBeginEvent(bossToShow));
				bossToShow = null;
			}
			break;
		default:
			sHSBrawlerMainWindow.SetPowerBarVisibility(false);
			sHSBrawlerMainWindow.OrdersCanShow(false);
			sHSBrawlerMainWindow.HideOrders();
			sHSBrawlerMainWindow.DetachHealthBar();
			GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow/SHSBrawlerIndicatorArrow"].Hide();
			break;
		case BrawlerUIMode.LevelContinue:
			break;
		}
		if (mode == BrawlerUIMode.Diorama)
		{
			//////////// this block added by CSP for testing //////////////
			ActiveMission activeMission = AppShell.Instance.SharedHashTable["ActiveMission"] as ActiveMission;
			CspUtils.DebugLog("activeMission.CurrentStage=" + activeMission.CurrentStage);
			CspUtils.DebugLog("activeMission.LastStage=" + activeMission.LastStage);
			if (activeMission.CurrentStage == activeMission.LastStage)  
			{
				sHSBrawlerMainWindow.brawlerCompletePanel.IsEnabled = true; 
				sHSBrawlerMainWindow.brawlerCompletePanel.IsVisible = true; 
				sHSBrawlerMainWindow.brawlerCompletePanel.Show(); 
			}
			///////////////////////////////////////////////////////////////
		}
		if (mode == BrawlerUIMode.LevelContinue)
		{
			sHSBrawlerMainWindow.SetPowerBarVisibility(true);
			sHSBrawlerMainWindow.OrdersCanShow(true);
			GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow/SHSBrawlerIndicatorArrow"].Show();
			if (bossToShow != null)
			{
				AppShell.Instance.EventMgr.Fire(null, new BossAIControllerBrawler.BossBattleBeginEvent(bossToShow));
				bossToShow = null;
			}
		}
		if (mode == BrawlerUIMode.CutScene)
		{
		}
		if (mode == BrawlerUIMode.StageComplete)
		{
			sHSBrawlerMainWindow.BossInactive();
			GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow/SHSBrawlerStageCompleteWindow"].Show();
			AchievementManager.reportTotalKills(AppShell.Instance.Profile.SelectedCostume);
		}
		else
		{
			GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow/SHSBrawlerStageCompleteWindow"].Hide();
		}
		if (mode == BrawlerUIMode.MissionComplete)
		{
			sHSBrawlerMainWindow.BossInactive();
			
			if (MissionEndHandler == null || MissionEndHandler.OnMissionEndUI())
			{			
				GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow/SHSBrawlerCompleteWindow"].Show();
				playedBriefingVO = false;
				AchievementManager.reportTotalKills(AppShell.Instance.Profile.SelectedCostume);
			}
		}
		else
		{			
			GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow/SHSBrawlerCompleteWindow"].Hide();
		}
	}

	protected void RegisterUIEvents()
	{
		AppShell.Instance.EventMgr.AddListener<BossAIControllerBrawler.BossCreatedEvent>(OnBossCreated);
	}

	protected void UnregisterUIEvents()
	{
		AppShell.Instance.EventMgr.RemoveListener<BossAIControllerBrawler.BossCreatedEvent>(OnBossCreated);
	}

	protected void UiEnable()
	{
		if (loadWindow == null)
		{
			loadWindow = (SHSBrawlerWaitWindow)AppShell.Instance.TransitionHandler.CurrentWaitWindow;
		}
	}

	protected void UiDisable()
	{
		ResetLoadingScreen();
		loadWindow = null;
	}

	public string GetCurrentMedalName()
	{
		if (scoreWindow == null)
		{
			scoreWindow = (GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow/SHSBrawlerScoreWindow"] as SHSBrawlerScoreWindow);
		}
		if (scoreWindow != null)
		{
			return scoreWindow.GetCurrentMedalName();
		}
		CspUtils.DebugLog("unable to retrieve medal name because score window is not set in brawler controller");
		return string.Empty;
	}

	protected void HandleObjectiveRefresh()
	{
	}

	public void DisplayNewObjective(string newObjective, string newObjectiveIcon, bool disableEvent)
	{
		objectiveText = newObjective;
		objectiveIcon = newObjectiveIcon;
		if (disableEvent)
		{
			SetObjectiveText(60f);
		}
		else
		{
			SetObjectiveText(6f);
		}
	}

	protected void SetObjectiveText(float displayTime)
	{
		StartCoroutine(ShowObjective(2f, displayTime));
	}

	protected IEnumerator ShowObjective(float delayTime, float displayTime)
	{
		yield return new WaitForSeconds(delayTime);
		ActiveMission mission = AppShell.Instance.SharedHashTable["ActiveMission"] as ActiveMission;
		if (mission != null && mission.CurrentStage <= 1)
		{
			ResolvedVOAction briefingVO = VOManager.Instance.GetVO("mission_briefing", VOInputString.FromStrings(mission.Id.ToString()));
			if (briefingVO != null && briefingVO.IsResolved && !playedBriefingVO)
			{
				briefingVO.CustomHandler = new PopupSpeakerVOActionHandler(1, SHSVOBrawlerObjectiveWindow.CreateWindow);
				briefingVO.CustomSubmixerName = "mission_start_queue";
				VOManager.Instance.PlayResolvedVO(briefingVO);
				playedBriefingVO = true;
				yield break;
			}
			(VOManager.Instance.CustomSubmixes["mission_start_queue"] as VOMissionBriefingSubmixer).ReleasePendingRelationship();
		}
		if (!string.IsNullOrEmpty(objectiveText))
		{
			SHSBrawlerMainWindow brawlScreen = (SHSBrawlerMainWindow)GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow"];
			brawlScreen.SetDisplayOrders(objectiveText, objectiveIcon, displayTime);
		}
		else
		{
			AppShell.Instance.EventMgr.Fire(null, new BrawlerMissionBriefCompleteMessage());
		}
	}

	protected SHSMissionBriefingWindow GetLoadingScreen(ActiveMission mission)
	{
		if (mission == null)
		{
			CspUtils.DebugLog("BrawlerController::GetLoadingScreen() - no active mission found");
			return null;
		}
		string textureSource = "missions_bundle|L_mshs_gameworld_" + mission.Id.ToString();
		bool useScreensize = false;
		if (mission.MissionDefinition == null)
		{
			textureSource = "persistent_bundle|loading_bg_bluecircles";
			mission.NotifyWhenDefinitionLoaded(OnMissionDataLoadedUI);
			useScreensize = true;
		}
		else if (mission.MissionDefinition.MissionSplashScreenOverride != null && mission.MissionDefinition.MissionSplashScreenOverride != string.Empty)
		{
			textureSource = "missions_bundle|" + mission.MissionDefinition.MissionSplashScreenOverride;
			useScreensize = true;
		}
		SHSMissionBriefingWindow sHSMissionBriefingWindow = new SHSMissionBriefingWindow();
		sHSMissionBriefingWindow.SetSplashImage(textureSource, useScreensize);
		return sHSMissionBriefingWindow;
	}

	protected void OnMissionDataLoadedUI(ActiveMission mission)
	{
		ChangeLoadingScreen();
	}

	protected void ChangeLoadingScreen()
	{
		ActiveMission activeMission = AppShell.Instance.SharedHashTable["ActiveMission"] as ActiveMission;
		loadWindow.ChangeLoadScreen(GetLoadingScreen(activeMission));
	}

	protected void ResetProgressBar()
	{
	}

	protected void ResetLoadingScreen()
	{
	}

	protected void StoreMissionResults(EventResultMissionEvent results)
	{
		SHSBrawlerCompleteWindow sHSBrawlerCompleteWindow = GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow/SHSBrawlerCompleteWindow"] as SHSBrawlerCompleteWindow;
		if (sHSBrawlerCompleteWindow != null)
		{
			sHSBrawlerCompleteWindow.ProcessMissionResultList(results);
		}
		AppShell.Instance.EventMgr.Fire(null, new BrawlerResultsMessage(results));
	}

	protected void OnBossCreated(BossAIControllerBrawler.BossCreatedEvent e)
	{
		if (currentUIMode == BrawlerUIMode.Level || currentUIMode == BrawlerUIMode.LevelContinue)
		{
			AppShell.Instance.EventMgr.Fire(null, new BossAIControllerBrawler.BossBattleBeginEvent(e.Bosses));
		}
		else
		{
			bossToShow = e.Bosses;
		}
	}
}
