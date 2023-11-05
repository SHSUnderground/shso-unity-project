using LitJson;
using ShsAudio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif

public class AppShell : MonoBehaviour, IInputHandler
{
	public class GameControllerTypeData
	{
		public string sceneName;

		public string windowName;

		public bool ignoreWindowLoad;

		public List<GUIManager.WindowBinding> windowsToLoad;

		public List<GUIManager.WindowBinding> windowsToUnload;

		public GameControllerTypeData(string sceneNameIn, string windowNameIn)
		{
			sceneName = sceneNameIn;
			windowName = windowNameIn;
			ignoreWindowLoad = false;
		}

		public GameControllerTypeData(string sceneNameIn, string windowNameIn, GUIManager.WindowBinding boundWindow)
			: this(sceneNameIn, windowNameIn)
		{
			windowsToLoad = new List<GUIManager.WindowBinding>();
			windowsToLoad.Add(boundWindow);
			windowsToUnload = new List<GUIManager.WindowBinding>();
			windowsToUnload.Add(boundWindow);
			if (boundWindow.registrationPeriod == GUIManager.WindowBinding.RegistrationPeriod.Initialization)
			{
				GUIManager.AddWindowToInitRegistry(boundWindow);
			}
			else
			{
				GUIManager.Instance.RegisterWindowForDynamicLoading(boundWindow);
			}
		}

		public GameControllerTypeData(string sceneNameIn, string windowNameIn, List<GUIManager.WindowBinding> boundWindows)
			: this(sceneNameIn, windowNameIn)
		{
			windowsToLoad = new List<GUIManager.WindowBinding>(boundWindows.ToArray());
			windowsToUnload = new List<GUIManager.WindowBinding>(boundWindows.ToArray());
			foreach (GUIManager.WindowBinding boundWindow in boundWindows)
			{
				if (boundWindow.registrationPeriod == GUIManager.WindowBinding.RegistrationPeriod.Initialization)
				{
					GUIManager.AddWindowToInitRegistry(boundWindow);
				}
				else
				{
					GUIManager.Instance.RegisterWindowForDynamicLoading(boundWindow);
				}
			}
		}

		public void AddWindowToJustLoad(GUIManager.WindowBinding windowBinding)
		{
			if (windowsToLoad != null)
			{
				windowsToLoad.Add(windowBinding);
			}
		}

		public void AddWindowsToJustLoad(List<GUIManager.WindowBinding> windowsToLoad)
		{
			if (this.windowsToLoad != null)
			{
				foreach (GUIManager.WindowBinding item in this.windowsToLoad)
				{
					this.windowsToLoad.Add(item);
				}
			}
		}

		public void LoadAllBoundWindows()
		{
			if (windowsToLoad == null)
			{
				return;
			}
			for (int i = 0; i < windowsToLoad.Count; i++)
			{
				GUIManager.WindowBinding windowBinding = windowsToLoad[i];
				GUIManager.WindowBinding windowBinding2 = windowsToLoad[i];
				if (windowBinding2.uiLayerName.Contains("/"))
				{
					GUIManager.Instance.MarkSubWindowForLoading(windowBinding.windowName);
				}
				else
				{
					GUIManager.Instance.MarkWindowForLoading(windowBinding.windowName);
				}
			}
		}

		public void AddWindowToJustUnload(GUIManager.WindowBinding windowBinding)
		{
			if (windowsToUnload != null)
			{
				windowsToUnload.Add(windowBinding);
			}
		}

		public void AddWindowsToJustUnload(List<GUIManager.WindowBinding> windowsToUnload)
		{
			if (this.windowsToUnload != null)
			{
				foreach (GUIManager.WindowBinding item in windowsToUnload)
				{
					this.windowsToUnload.Add(item);
				}
			}
		}

		public void UnloadAllBoundWindows()
		{
			if (windowsToUnload == null)
			{
				return;
			}
			for (int i = 0; i < windowsToUnload.Count; i++)
			{
				GUIManager.WindowBinding windowBinding = windowsToUnload[i];
				if (windowBinding.uiLayerName.Contains("/"))
				{
					GUIManager instance = GUIManager.Instance;
					GUIManager.WindowBinding windowBinding2 = windowsToUnload[i];
					instance.MarkSubWindowForUnloading(windowBinding2.windowName);
				}
				else
				{
					GUIManager instance2 = GUIManager.Instance;
					GUIManager.WindowBinding windowBinding3 = windowsToUnload[i];
					instance2.MarkWindowForUnloading(windowBinding3.windowName);
				}
			}
		}
	}

	public delegate void OldControllerUnloadingDelegate(GameControllerTypeData currentGameTypeData, GameControllerTypeData newGameTypeData);

	public delegate void NewControllerLoadingDelegate(GameControllerTypeData newGameTypeData, GameController controller);

	public delegate void NewControllerReadyDelegate(GameControllerTypeData newGameTypeData, GameController controller);

	public delegate void AboutToChangeFullscreenStateDelegate(bool newState);

	public delegate void ChangedFullscreenStateDelegate(bool newState);

	private const float tickerTime = 0.1f;

	public const string SHT_KEY_BRAWLER_LEVEL = "BrawlerLevel";

	public const string SHT_KEY_BRAWLER_TICKET = "BrawlerTicket";

	public const string SHT_KEY_CARD_LEVEL = "CardGameLevel";

	public const string SHT_KEY_CARD_TICKET = "CardGameTicket";

	public const string SHT_KEY_DEBUG_PROFILE = "DebugProfile";

	public const string SHT_KEY_ACTIVE_MISSION = "ActiveMission";

	public const string SHT_KEY_EVENT_RESULTS = "EventResults";

	public const string SHT_KEY_SOCIAL_LEVEL = "SocialSpaceLevel";

	public const string SHT_KEY_SOCIAL_TICKET = "SocialSpaceTicket";

	public const string SHT_KEY_SOCIAL_CHARACTER = "SocialSpaceCharacter";

	public const string SHT_KEY_SOCIAL_SPAWNPOINT = "SocialSpaceSpawnPoint";

	public const string SHT_KEY_SOCIAL_SPAWNPOINT_POSITION_OVERRIDE = "SocialSpaceSpawnPointPositionOverride";

	public const string SHT_KEY_SOCIAL_LEVEL_CURRENT = "SocialSpaceLevelCurrent";

	public const string SHT_KEY_SOCIAL_TICKET_CURRENT = "SocialSpaceTicketCurrent";

	public const string SHT_KEY_SOCIAL_CHARACTER_CURRENT = "SocialSpaceCharacterCurrent";

	public const string SHT_KEY_BRAWLER_AIRLOCK_PLAY_SOLO = "BrawlerAirlockPlaySolo";

	public const string SHT_KEY_BRAWLER_AIRLOCK_RETURNS_TO_GADGET = "BrawlerAirlockReturnsToGadget";

	public const string SHT_KEY_GUI_HUD_SINGLE_PLAYER_GAME = "GUIHudPlaySolo";

	public const string SHT_KEY_GUI_GAME_WORLD_FORCE_CHARACTER_SELECT = "GUIGameWorldForceCharacterSelect";

	public const string SHT_KEY_GUI_INVENTORY_DEFAULT_TAB_POSITION = "GUIDefaultTabPosition";

	public const string SHT_KEY_FRIEND_LIST_LAST_INVITE_TIME = "FriendListLastInviteTime";

	public const string SHT_KEY_PRIZE_WHEEL_USED = "PrizeWheelUsed";

	public const string SHT_KEY_WELCOME_VIEWED = "WelcomeScreenViewed";

	public const string SHT_KEY_CONTENT_URI = "ContentUri";

	public const string SHT_KEY_ARCADE_GAME = "AracdeGame";

	public const string SHT_KEY_GAME_WORLD_LASTSHOWN_UPSELL = "GameWorldUpsellButton";

	public const string SHT_KEY_NEWSPAPER_HAS_BEEN_SHOWN = "NewsPaperHasBeenShown";

	private const int UPSELL_ASSET_COUNT = 4;

	protected string[] validURLs = new string[11]
	{
		"&j^r,q]n",
		"&_g,a^hd`hje_`,`kh",
		"-fbnjoi&Zel",
		"%Wl_weialgZ_drv*^if",
		"ocm&!!l-q0*!gZrfd`up*^if",
		"!(^r3u2spjhk(-aikp^_jfds,kao",
		"['bj43f*),'.rn+_gin!]hnlq*i_m",
		"-fbnjoi&e[s",
		"%itnbn(b^jf#f_ja)_n",
		"!/d7`ot3m2Zpa-aikp^_jfds,kao",
		"'f`l`jkaoqhjb$bmj"
	};

	public StringTable stringTable;

	public ServerTime serverTime;

	public GameObject UIManagerPrefab;

	public Hashtable SharedHashTable;

	public ShsEventMgr EventMgr;

	public ShsTimerMgr TimerMgr;

	public AssetBundleLoader BundleLoader;

	public ShsWebService WebService;

	public ShsWebAssetCache WebAssetCache;

	public GameDataManager DataManager;

	public IServerConnection ServerConnection;

	public DataWarehouse ServerConfig;

	public Matchmaker2 Matchmaker2;

	public EventReporter EventReporter;

	public UserProfile Profile;

	public SHSCountersManager CounterManager;

	public SHSActivityManager ActivityManager;

	public AchievementsManager AchievementsManager;

	public AchievementManager AchievementManager;

	public PrizeWheelManager PrizeWheelManager;

	public NewShoppingManager NewShoppingManager;

	public ArcadeManager ArcadeManager;

	public ChallengeManager ChallengeManager;

	public MenuChatManager MenuChatManager;

	public WaitWatcherMgr WaitWatcher;

	public CharacterDescriptionsManager CharacterDescriptionManager;

	public SmartTipsManager SmartTipsManager;

	public CombatEffectDataWarehouse CombatEffectDataWarehouse;

	public TransitionHandler TransitionHandler;

	public GameObject VOManagerPrefab;

	public HeroLoreManager HeroLoreManager;

	public CelebrationManager CelebrationManager;

	public InventorySessionRecorder InventoryRecorder;

	public ItemDefinitionDictionary ItemDictionary;

	public ExpendablesManager ExpendablesManager;

	public MissionManifest MissionManifest;

	public LevelUpRewardItemsManifest LevelUpRewardItemsManifest;

	public HQRoomManifest HQRoomManifest;

	public CommunicationManager CommunicationManager;

	public CardQuestManager CardQuestManager;

	private GameController.ControllerType currentControllerType;

	private GameController.ControllerType previousControllerType;

	protected Dictionary<string, CardScenarioDefinition> cardScenarios;

	private bool transitionInProgress;

	private float transitionTicker;

	protected bool globalContentLoaded;

	protected int sceneCounter;

	protected float fps = 60f;

	protected bool hasFocus = true;

	public float cachedXPMultiplier = 1f;

	private TransitionHandler.WWTransitionProperties launchTransitionProperties;

	public TransactionMonitor LaunchTransaction;

	public TransactionMonitor LaunchAppShellTransaction;

	public TransactionMonitor LaunchLoginTransaction;

	public TransactionMonitor LaunchTransitionTransaction;

	protected TransactionMonitor appShellStartTransaction;

	protected TransactionMonitor guiBundleTransaction;

	protected TransactionMonitor uiSfxPreloadTransaction;

	protected TransactionMonitor postGlobalContentGuiBundleTransaction;

	protected TransactionMonitor transitionTransaction;

	protected TransactionMonitor localeInitTransaction;

	protected bool isReady;

	public AudioManager AudioManager;

	public string NetworkEnvironment;

	protected bool isHeroAuth;

	private TransactionMonitor localeChangeTransaction;

	protected string locale;

	private int targetFrameRate;

	private GameControllerTypeData currentControllerTypeData;

	protected int windowedWidth = -1;

	protected int windowedHeight = -1;

	[HideInInspector]
	public Dictionary<GameController.ControllerType, GameControllerTypeData> GameControllerInfo;

	public DebugOptions DebugOptions;

	public bool initialized;

	public bool automateclient;

	public string configFile;

	public string scriptFile;

	public string username;

	public string password;

	public ShsAudioSource startupMusic;

	public ShsAudioSource loadingMusic;

	protected static AppShell instance;

	private readonly PlayerDictionary players = new PlayerDictionary();

	private int upsell_asset_current;

	public static bool PromoSpawnAllowed = true;

	public Texture2D promoImage;

	public GameController.ControllerType CurrentControllerType
	{
		get
		{
			return currentControllerType;
		}
		set
		{
			currentControllerType = value;
		}
	}

	public GameController.ControllerType PreviousControllerType
	{
		get
		{
			return previousControllerType;
		}
		set
		{
			previousControllerType = value;
		}
	}

	public Dictionary<string, CardScenarioDefinition> CardScenarios
	{
		get
		{
			return cardScenarios;
		}
	}

	public bool GlobalContentLoaded
	{
		get
		{
			return globalContentLoaded;
		}
		set
		{
			globalContentLoaded = value;
		}
	}

	public int SceneCounter
	{
		get
		{
			return sceneCounter;
		}
	}

	public float FPS
	{
		get
		{
			return fps;
		}
	}

	public bool HasFocus
	{
		get
		{
			return hasFocus;
		}
	}

	public bool IsReady
	{
		get
		{
			return isReady;
		}
	}

	public bool IsHeroAuth
	{
		get
		{
			return isHeroAuth;
		}
	}

	public string Locale
	{
		get
		{
			return locale;
		}
		set
		{
			string text = locale;
			locale = value;
			if (locale != text)
			{
				if (localeChangeTransaction != null)
				{
					throw new TransactionInProgressException();
				}
				ShsWebService.SafeJavaScriptCall(string.Format("HEROUPNS.SetLocaleFromGame(\"{0}\");", locale));
				localeChangeTransaction = TransactionMonitor.CreateTransactionMonitor("localeChangeTransaction", localeChangeComplete, 360f, null);
				if (EventMgr != null)
				{
					localeChangeTransaction.AddStep("init");
					EventMgr.Fire(this, new LocaleChangedMessage(text, locale, localeChangeTransaction));
					localeChangeTransaction.CompleteStep("init");
				}
			}
		}
	}

	public int TargetFrameRate
	{
		get
		{
			return targetFrameRate;
		}
		set
		{
			targetFrameRate = value;
			Application.targetFrameRate = targetFrameRate;
		}
	}

	public GameControllerTypeData CurrentControllerTypeData
	{
		get
		{
			return currentControllerTypeData;
		}
	}

	public static AppShell Instance
	{
		get
		{
			return instance;
		}
	}

	public bool CanHandleInput
	{
		get
		{
			return true;
		}
	}

	public SHSInput.InputRequestorType InputRequestorType
	{
		get
		{
			return SHSInput.InputRequestorType.World;
		}
	}

	public PlayerDictionary PlayerDictionary
	{
		get
		{
			return players;
		}
	}

	public event OldControllerUnloadingDelegate OnOldControllerUnloading;

	public event NewControllerLoadingDelegate OnNewControllerLoading;

	public event NewControllerReadyDelegate OnNewControllerReady;

	public event AboutToChangeFullscreenStateDelegate OnAboutToChangeFullscreenState;

	public event ChangedFullscreenStateDelegate OnChangedFullscreenState;

	public bool IsCurrentLocale(string compareLocale)
	{
		if (string.IsNullOrEmpty(compareLocale))
		{
			return true;
		}
		if (compareLocale == "common")
		{
			return true;
		}
		if (compareLocale.Replace("-", "_") == locale.Replace("-", "_"))
		{
			return true;
		}
		return false;
	}

	private void localeChangeComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		localeChangeTransaction = null;
		CspUtils.DebugLog("locale change completed.");
	}

	public void OnApplicationQuit()
	{
		Utils.BroadcastUnloadEvent();
		if (ServerConnection != null)
		{
			IDisposable disposable = ServerConnection as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			ServerConnection = null;
		}
	}

	public void Awake()
	{
		Application.runInBackground = true;  // added by CSP
		System.Security.Cryptography.MD5CryptoServiceProvider md = null;
        

		isReady = false;
		if (instance == null)
		{
			instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (Application.isWebPlayer)
			{
				CspUtils.DebugLog("Application.srcValue = " + Application.srcValue);
				CspUtils.DebugLog("Application.absoluteURL = " + Application.absoluteURL);
				string absoluteURL = Application.absoluteURL;
				bool flag = false;
				string[] array = validURLs;
				foreach (string literal in array)
				{
					if (absoluteURL.Contains(Utils.LiteralScrambler(literal, true) + "/"))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					CspUtils.DebugLog("INVALID URL!");
					Application.ExternalEval("document.location='http://www.heroup.com';");
					return;
				}
				CspUtils.DebugLog("VALID URL!");
			}
			stringTable = new StringTable();
			serverTime = new ServerTime();
			SharedHashTable = new Hashtable();
			EventMgr = new ShsEventMgr();
			TimerMgr = new ShsTimerMgr();
			ServerConnection = new NetworkManager();
			Matchmaker2 = new Matchmaker2();
			EventReporter = new EventReporter();
			AudioManager = new AudioManager();
			CounterManager = new SHSCountersManager();
			ActivityManager = new SHSActivityManager(CounterManager);
			ArcadeManager = new ArcadeManager();
			CommunicationManager = new CommunicationManager();
			CardQuestManager = new CardQuestManager();
			MenuChatManager = new MenuChatManager();
			ChallengeManager = new ChallengeManager();
			HeroLoreManager = new HeroLoreManager();
			ExpendablesManager = new ExpendablesManager();
			CelebrationManager = new CelebrationManager();
			InventoryRecorder = new InventorySessionRecorder();
			AchievementManager = new AchievementManager();
			ServerConfig = null;
			Profile = null;
			BundleLoader = (GetComponent(typeof(AssetBundleLoader)) as AssetBundleLoader);
			WebService = (GetComponent(typeof(ShsWebService)) as ShsWebService);
			WebAssetCache = (GetComponent(typeof(ShsWebAssetCache)) as ShsWebAssetCache);
			DataManager = (GetComponent(typeof(GameDataManager)) as GameDataManager);
			CspUtils.DebugLog("DataManager" + DataManager);
			TransitionHandler = new TransitionHandler();
			TransitionHandler.WaitWatcherManager = (GetComponent(typeof(WaitWatcherMgr)) as WaitWatcherMgr);
			GUIControl.DrawContentLoadingDefault = SHSControlCustomDrawMethods.DrawContentLoadingDefault;
			GUIControl.OnLoadingActivateDefault = SHSControlCustomDrawMethods.OnLoadingActivateDefault;
			SharedHashTable["DebugProfile"] = new DebugOptions();
			DebugOptions = (DebugOptions)SharedHashTable["DebugProfile"];
			DebugOptions.AddSetting("UI_DEBUG_DRAW", false);
			DebugOptions.AddSetting("UI_SHOW_HITTEST_REGIONS", false);
			DebugOptions.AddSetting("UI_SHOW_NEW_MAP", true);
			DebugOptions.AddSetting("UI_SHOW_DRAW_REGIONS", false);
			DebugOptions.AddSetting("UI_SHOW_HIT_TEST_DRAW_REGIONS", false);
			DebugOptions.AddSetting("UI_WHEEL_SIMULATE", false);
			SetupGameControllers();
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(UIManagerPrefab);
			gameObject.transform.parent = base.transform;
			if (VOManagerPrefab != null)
			{
				GameObject child = UnityEngine.Object.Instantiate(VOManagerPrefab) as GameObject;
				Utils.AttachGameObject(base.gameObject, child);
			}
			launchTransitionProperties = new TransitionHandler.WWTransitionProperties(false, false, false, "SHSStartupWaitWindow", string.Empty);
			launchTransitionProperties.transactionContext = new TransactionLoadingContext.TransactionContext(new List<string>(new string[1]
			{
				"launch_init"
			}), "AppShell_LaunchTransaction");
			launchTransitionProperties.transactionContext.timeOut = 0f;
			launchTransitionProperties.transactionContext.onComplete = OnLaunchTransactionDone;
			TransitionHandler.SetupTransitionContext(launchTransitionProperties.transactionContext);
			LaunchTransaction = TransitionHandler.CurrentTransactionContext.Transaction;
			LaunchAppShellTransaction = TransactionMonitor.CreateTransactionMonitor("LaunchAppShell", null, 0f, null);
			LaunchTransaction.AddChild(LaunchAppShellTransaction);
			LaunchLoginTransaction = TransactionMonitor.CreateTransactionMonitor("LaunchLogin", null, 120f, null);
			LaunchLoginTransaction.AddStep("initialize");
			LaunchTransaction.AddChild(LaunchLoginTransaction);
			LaunchTransitionTransaction = TransactionMonitor.CreateTransactionMonitor("LaunchTransition", null, 0f, null);
			LaunchTransitionTransaction.AddStep("initialize");
			LaunchTransaction.AddChild(LaunchTransitionTransaction);
			localeInitTransaction = TransactionMonitor.CreateTransactionMonitor("AppShell_LocaleInitTransaction", OnLocaleInitDone, 0f, null);
			localeInitTransaction.AddStep("locale");
			localeInitTransaction.AddStep("manifest");
			localeInitTransaction.AddStep("start");
			LaunchTransaction.AddChild(localeInitTransaction);
			EventMgr.AddListener<GlobalContentLoadedMessage>(OnGlobalContentLoaded);
			EventMgr.AddListener<BundleGroupLoadedMessage>(OnBundleGroupLoaded);
			if (Application.isWebPlayer)
			{
				Application.ExternalEval("try { HEROUPNS.SendLocaleToUnity(); } \r\n                  catch (ex) { GetUnity().SendMessage(\"_AppShell\", \"OnNoLocaleFromWebPage\", \"\"); }");
			}
			else
			{
				OnNoLocaleFromWebPage();
			}
			SHSInput.ResetInputAxes();
		}
		else
		{
			CspUtils.DebugLog("AppShell already created, attempt to create a second one will lead to instability.");
		}
	}

	public void OnLocaleFromWebPage(string webLocale)
	{
		CspUtils.DebugLog("Locale <" + webLocale + "> was received from the web page");
		locale = LocaleMapper.CultureInfoToLocale(webLocale);
		localeInitTransaction.CompleteStep("locale");
	}

	public void OnNoLocaleFromWebPage()
	{
		locale = LocaleMapper.GetCurrentLocale();
		localeInitTransaction.CompleteStep("locale");
	}

	protected void OnLocaleInitDone(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		if (exit == TransactionMonitor.ExitCondition.Success)
		{
			StartLocalization(appShellStartTransaction);
			if (ShsCacheManager.Manifest != null)
			{
				GUIManager.Instance.OnInitializationStep(GUIManager.InitializationState.AssetManifestLoaded);
				preloadGUIBundles();
				BundleLoader.BackgroundDownload();
			}
			launchTransitionProperties.locationInfo = GUILoadingScreenContext.LocationInfo.NoInfo;
			launchTransitionProperties.loadingContext = GUILoadingScreenContext.LoadingContext.EmptyContext;
			TransitionHandler.SetupTransition(launchTransitionProperties);
		}
	}

	private void OnBundleGroupLoaded(BundleGroupLoadedMessage message)
	{
		AssetBundleLoader.BundleGroup groupUnlocked = message.groupUnlocked;
		List<string> list = new List<string>();
		switch (groupUnlocked)
		{
		case AssetBundleLoader.BundleGroup.PrizeWheel:
			list.Add("hq_bundle");
			list.Add("items_bundle");
			list.Add("goodiescommon_bundle");
			list.Add("goodieseffects_bundle");
			list.Add("goodiesstats_bundle");
			list.Add("potioneffects_bundle");
			list.Add("missions_bundle");
			list.Add("missionflyers_bundle");
			break;
		case AssetBundleLoader.BundleGroup.CardGame:
			list.Add("cardgame_tutorial_bundle");
			break;
		}
		foreach (string item in list)
		{
			GUIManager.Instance.BundleManager.LoadBundle(item, delegate(AssetBundleLoadResponse response, object extraData)
			{
				if (!string.IsNullOrEmpty(response.Error))
				{
					postGlobalContentGuiBundleTransaction.FailStep((string)extraData, "Unable to load");
				}
			}, null);
		}
		if (message.requiredDownload && !Instance.transitionInProgress && Instance.SharedHashTable.ContainsKey("NewsPaperHasBeenShown") && groupUnlocked != 0 && groupUnlocked != AssetBundleLoader.BundleGroup.VO && groupUnlocked != AssetBundleLoader.BundleGroup.All && groupUnlocked != AssetBundleLoader.BundleGroup.Any)
		{
			
			GameAreaAvailableNotificationData data = new GameAreaAvailableNotificationData(groupUnlocked);
			NotificationHUD.addNotification(data);
			CspUtils.DebugLog("HUD Notfied!");
		}
		else {
			CspUtils.DebugLog("HUD NOT Notfied! groupUnlocked = " + groupUnlocked);
			CspUtils.DebugLog("message.requiredDownload=" + message.requiredDownload);
			//CspUtils.DebugLog("!Instance.transitionInProgress=" + !Instance.transitionInProgress);
			CspUtils.DebugLog("Instance.SharedHashTable.ContainsKey(NewsPaperHasBeenShown)=" + Instance.SharedHashTable.ContainsKey("NewsPaperHasBeenShown"));
			//CspUtils.DebugLog("groupUnlocked != 0=" + (groupUnlocked != 0));
			//CspUtils.DebugLog("groupUnlocked != AssetBundleLoader.BundleGroup.All=" + (groupUnlocked != AssetBundleLoader.BundleGroup.All));
			//CspUtils.DebugLog("groupUnlocked != AssetBundleLoader.BundleGroup.VO=" + (groupUnlocked != AssetBundleLoader.BundleGroup.VO));
			//CspUtils.DebugLog("groupUnlocked != AssetBundleLoader.BundleGroup.Any=" + (groupUnlocked != AssetBundleLoader.BundleGroup.Any));
			
		}
	}

	private void OnGlobalContentLoaded(GlobalContentLoadedMessage message)
	{
		if (!globalContentLoaded)
		{
			postGlobalContentGuiBundleTransaction = TransactionMonitor.CreateTransactionMonitor(delegate(TransactionMonitor.ExitCondition exit, string error, object userData)
			{
				if (exit == TransactionMonitor.ExitCondition.Success)
				{
					globalContentLoaded = (message.ForceComplete || PlayerPrefs.GetInt("ContentNeverLoad", 0) == 0);
				}
				else
				{
					CspUtils.DebugLog("GUI bundle assets can not be loaded after global content complete flag set.");
				}
			}, 30f, null);
			List<string> list = new List<string>();
			list.Add("hq_bundle");
			list.Add("items_bundle");
			list.Add("missions_bundle");
			foreach (string item in list)
			{
				string text = item + "_step";
				if (postGlobalContentGuiBundleTransaction != null)
				{
					postGlobalContentGuiBundleTransaction.AddStep(text, TransactionMonitor.DumpTransactionStatus);
				}
				GUIManager.Instance.BundleManager.LoadBundle(item, delegate(AssetBundleLoadResponse response, object extraData)
				{
					if (postGlobalContentGuiBundleTransaction != null)
					{
						if (!string.IsNullOrEmpty(response.Error))
						{
							postGlobalContentGuiBundleTransaction.FailStep((string)extraData, "Unable to load");
						}
						else
						{
							postGlobalContentGuiBundleTransaction.CompleteStep((string)extraData);
						}
					}
				}, text);
			}
		}
	}

	private void OnUserCounterDataLoaded(UserCounterDataLoadedMessage message)
	{
		EventMgr.RemoveListener<UserCounterDataLoadedMessage>(OnUserCounterDataLoaded);
		DetermineWelcomeScreenPath();
	}

	public void Start()
	{
		appShellStartTransaction = TransactionMonitor.CreateTransactionMonitor("AppShell_startTransaction", OnStartTransactionDone, 60f, null);
		appShellStartTransaction.AddStep("start", TransactionMonitor.DumpTransactionStatus, "App Shell Started");
		appShellStartTransaction.AddStep("serverconfig", TransactionMonitor.DumpTransactionStatus, "Server Config Completed");
		LaunchAppShellTransaction.AddChild(appShellStartTransaction);
		guiBundleTransaction = TransactionMonitor.CreateTransactionMonitor("AppShell_bundleTransaction", OnBundleTransactionDone, float.MaxValue, null);
		guiBundleTransaction.AddStep("start", TransactionMonitor.DumpTransactionStatus, "App Shell Bundle Ready");
		appShellStartTransaction.AddChild(guiBundleTransaction);
		StartCoroutine(PollFPS());
		StartCoroutine(PollCurrency());
		CommunicationManager.Initialize();
		EventMgr.AddListener<UserCounterDataLoadedMessage>(OnUserCounterDataLoaded);
		appShellStartTransaction.AddStep("char_desc", TransactionMonitor.DumpTransactionStatus);
		CspUtils.DebugLog("CommunicationManager=" + JsonMapper.ToJson(CommunicationManager));
		//CspUtils.DebugLog("DataManager=" + JsonMapper.ToJson(DataManager));
		CspUtils.DebugLog("DataManager=" + DataManager);
		//if (OnCharDescriptionLoaded == null)
		//	CspUtils.DebugLog("OnCharDescriptionLoaded=null");
		CspUtils.DebugLog("CharacterDescriptionsManager=" + new CharacterDescriptionsManager());
		DataManager.LoadGameData("characterdescriptions/CharacterDescriptions", OnCharDescriptionLoaded, new CharacterDescriptionsManager());
		appShellStartTransaction.AddStep("xp_to_level", TransactionMonitor.DumpTransactionStatus);
		Instance.WebService.StartRequest("resources$data/json/level_chart.py", OnXpToLevelDataLoaded, null, ShsWebService.ShsWebServiceType.RASP);
		appShellStartTransaction.AddStep("emotes", TransactionMonitor.DumpTransactionStatus);
		DataManager.LoadGameData("emotes", OnEmotesDataLoaded, new EmotesDefinition());
		appShellStartTransaction.AddStep("motd", TransactionMonitor.DumpTransactionStatus);
		Instance.WebService.StartRequest("resources$data/json/daily_missions.py", OnMotdWebResponse, null, ShsWebService.ShsWebServiceType.RASP);
		appShellStartTransaction.AddStep("botd", TransactionMonitor.DumpTransactionStatus);
		Instance.WebService.StartRequest("resources$data/json/daily_quests.py", OnBotdWebResponse, null, ShsWebService.ShsWebServiceType.RASP);
		appShellStartTransaction.AddStep("brawler_leveling_stats", TransactionMonitor.DumpTransactionStatus);
		DataManager.LoadGameData("brawler_leveling_stats", OnStatRequirementsLoaded, new StatLevelReqsDefinition());
		appShellStartTransaction.AddStep("brawler_scoring", TransactionMonitor.DumpTransactionStatus);
		DataManager.LoadGameData("brawler_scoring", OnBrawlerScoringLoaded, new BrawlerScoringDefinition());
		appShellStartTransaction.AddStep("brawler_action_times", TransactionMonitor.DumpTransactionStatus);
		DataManager.LoadGameData("brawler_action_times", OnActionTimesLoaded, new ActionTimesDefinition());
		appShellStartTransaction.AddStep("item_defs", TransactionMonitor.DumpTransactionStatus);
		appShellStartTransaction.AddStep("expendable_defs", TransactionMonitor.DumpTransactionStatus);
		appShellStartTransaction.AddStep("feature_defs", TransactionMonitor.DumpTransactionStatus);
		appShellStartTransaction.AddStep("pet_defs", TransactionMonitor.DumpTransactionStatus);
		appShellStartTransaction.AddStep("scavenger_defs", TransactionMonitor.DumpTransactionStatus);
		appShellStartTransaction.AddStep("mission_defs", TransactionMonitor.DumpTransactionStatus);
		Instance.WebService.StartRequest("resources$data/json/brawler_missions.py", OnMissionDefsWebResponse, null, ShsWebService.ShsWebServiceType.RASP);
		appShellStartTransaction.AddStep("hqrooms_defs", TransactionMonitor.DumpTransactionStatus);
		DataManager.LoadGameData("HQ/Rooms", OnHQRoomsManifestLoaded);
		appShellStartTransaction.AddStep("counters", TransactionMonitor.DumpTransactionStatus);
		DataManager.LoadGameData("Counters/shs_counters", OnCounterLoaded);
		appShellStartTransaction.AddStep("activities", TransactionMonitor.DumpTransactionStatus);
		DataManager.LoadGameData("Activities/shs_activities", OnActivitiesLoaded);
		appShellStartTransaction.AddStep("arcade", TransactionMonitor.DumpTransactionStatus);
		DataManager.LoadGameData("Arcade/arcade_games", OnArcadeGamesLoaded);
		appShellStartTransaction.AddStep("challenge", TransactionMonitor.DumpTransactionStatus);
		DataManager.LoadGameData("Challenges/challenges", OnChallengesLoaded);
		appShellStartTransaction.AddStep("achievements", TransactionMonitor.DumpTransactionStatus);
		DataManager.LoadGameData("Achievements/shs_achievements", OnAchievementsLoaded, new AchievementsManager());
		appShellStartTransaction.AddStep("smarttips", TransactionMonitor.DumpTransactionStatus);
		DataManager.LoadGameData("SmartTips/SmartTips", OnSmartTipsLoaded, new SmartTipsManager());
		appShellStartTransaction.AddStep("deckbuilder_config", TransactionMonitor.DumpTransactionStatus);
		DataManager.LoadGameData("Cards/deckbuilder_config", OnDeckBuilderConfigLoaded, new DeckBuilderConfigDefinition());
		CardManager.LoadCardData();
		DataManager.LoadGameData("player_statuses", PlayerStatusDefinition.OnDataLoaded, new PlayerStatusDefinition(appShellStartTransaction));
		CombatEffectDataWarehouse = new CombatEffectDataWarehouse();
		appShellStartTransaction.AddStep("menuchat", TransactionMonitor.DumpTransactionStatus);
		DataManager.LoadGameData("MenuChat/SHSMenuChat", OnMenuChatLoaded);
		Instance.WebService.StartRequest("resources$data/json/card_quests.py", OnCardQuestDataWebResponse, null, ShsWebService.ShsWebServiceType.RASP);
		appShellStartTransaction.AddStep("level_up_items_defs", TransactionMonitor.DumpTransactionStatus);
		Instance.WebService.StartRequest("resources$data/json/hero_level_rewards.py", OnLevelUpRewardWebResponse, null, ShsWebService.ShsWebServiceType.RASP);
		Instance.WebService.StartRequest("resources$data/json/dutilJson", OnDutilLoaded,null,ShsWebService.ShsWebServiceType.RASP); //DOGGO
		Instance.WebService.StartRequest("resources$data/json/dutilAfk", OnDutilAfk,null,ShsWebService.ShsWebServiceType.RASP); //DOGGO
		if (ActivityManager != null)
		{
			ActivityManager.Start();
		}
		appShellStartTransaction.AddStep("assetbundlemanifest", TransactionMonitor.DumpTransactionStatus);
		appShellStartTransaction.AddStep("bundles_loaded", TransactionMonitor.DumpTransactionStatus);
		Singleton<ShsCacheManager>.instance.ManifestLoadedEvent += onManifestLoaded;
		appShellStartTransaction.AddStep("newscontent");
		WebAssetCache.AssetCacheConfigured += OnWebAssetCacheConfigured;
		SHSNewsRewardJson.Instance.RequestDailyReward(appShellStartTransaction);
		Singleton<VOActionDataManager>.instance.LoadVOActionData(appShellStartTransaction, "vo_action_data");
		Singleton<VOAssetManifest>.instance.LoadVOAssetManifestData(appShellStartTransaction, "vo_asset_manifest");
		CspUtils.DebugLog("CSP1");
		HeroLoreManager.RequestHeroLoreValues(appShellStartTransaction);
		CspUtils.DebugLog("CSP2");
		appShellStartTransaction.CompleteStep("start");
		CspUtils.DebugLog("CSP3");
		EventMgr.AddListener<LocaleChangedMessage>(this, onLocaleChanged);
		CspUtils.DebugLog("CSP4");
		if (CelebrationManager != null)
		{
			CelebrationManager.Start();
			CspUtils.DebugLog("CSP5");
		}
		InventoryRecorder.Initialize();
		CspUtils.DebugLog("CSP6");
		localeInitTransaction.CompleteStep("start");
		CspUtils.DebugLog("CSP7");
	}

	private void OnWebAssetCacheConfigured()
	{
		FeatureImageLoader.OnWebAssetCacheInitialized(appShellStartTransaction);
	}

	private void onWebContentItemLoaded(ShsWebResponse response)
	{
		upsell_asset_current++;
		if (upsell_asset_current >= 4 && appShellStartTransaction != null)
		{
			appShellStartTransaction.CompleteStep("upsellcontent");
		}
	}

	private void onWebPromoImageLoaded(ShsWebResponse response)
	{
		CspUtils.DebugLog(response.RequestUri + " status = " + response.Status);
	}

	private void onManifestLoaded(bool success)
	{
		if (success)
		{
			AudioManager.Initialize(appShellStartTransaction, onAudioManagerInitialized);
			if (localeInitTransaction != null)
			{
				localeInitTransaction.CompleteStep("manifest");
			}
			if (appShellStartTransaction != null)
			{
				appShellStartTransaction.CompleteStep("assetbundlemanifest");
			}
		}
		else
		{
			if (localeInitTransaction != null)
			{
				localeInitTransaction.FailStep("manifest", "Unable to retrieve manifest file.");
			}
			if (appShellStartTransaction != null)
			{
				appShellStartTransaction.FailStep("assetbundlemanifest", "Unable to retrieve manifest file.");
			}
		}
	}
		

	private void onAudioManagerInitialized()
	{
		// block temporarily commented out by CSP
		// GameObject gameObject = new GameObject("_sfx_preload");
		// uiSfxPreloadTransaction = TransactionMonitor.CreateTransactionMonitor("AppShell_sfxTransaction", OnUISFXPreloadTransactionDone, float.MaxValue, gameObject);
		// uiSfxPreloadTransaction.AddStep("bundle", TransactionMonitor.DumpTransactionStatus);
		// appShellStartTransaction.AddChild(uiSfxPreloadTransaction);
		// appShellStartTransaction.AddStep("sfx_preload");
		// preloadUISounds(gameObject);
	}

	private void onLocaleChanged(LocaleChangedMessage message)
	{
		StartLocalization(message.Transaction);
	}

	private void StartLocalization(TransactionMonitor transaction)
	{
		if (transaction == null)
		{
			throw new TransactionRequiredException();
		}
		transaction.AddStep("strings");
		stringTable.Load(locale, delegate(bool success, string error)
		{
			OnStringsLoaded(success, error, transaction);
		});
		transaction.AddStep("fontbank");
		GUIFontManager.LoadFontBank(locale, delegate(bool success, string error, AssetBundle asset)
		{
			OnFontBankLoaded(success, error, asset, transaction);
		});
	}

	private void OnFontBankLoaded(bool success, string error, AssetBundle bundle, TransactionMonitor transaction)
	{
		if (!success)
		{
			transaction.FailStep("fontbank", "Can't load font bank... which begs the question how you'd be able to read this.");
			return;
		}
		EventMgr.Fire(this, new FontBankLoadedMessage(locale, transaction));
		transaction.CompleteStep("fontbank");
	}

	private void OnStringsLoaded(bool success, string error, TransactionMonitor transaction)
	{
		if (transaction != null)
		{
			if (success)
			{
				EventMgr.Fire(this, new StringTableLoadedMessage(locale, transaction));
				transaction.CompleteStep("strings");
				DataManager.LoadGameData("Items/definitions", OnItemDefinitionsLoaded);
				DataManager.LoadGameData("Items/expendables", OnExpendableDefinitionsLoaded);
				DataManager.LoadGameData("Items/features", OnFeatureDefinitionsLoaded);
				DataManager.LoadGameData("Items/pets", OnPetsDefinitionsLoaded);
				DataManager.LoadGameData("Items/scavenger", OnScavengerDefinitionsLoaded);
			}
			else
			{
				transaction.FailStep("strings", error);
			}
		}
	}

	protected void OnStartTransactionDone(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		CspUtils.DebugLog("!!!!!!!!!!!!!!! OnStartTransactionDone called !!!!!!!!!!!!");

		LaunchTransaction.CompleteStep("launch_init");
		LaunchTransaction = null;
		appShellStartTransaction = null;
		if (exit == TransactionMonitor.ExitCondition.Success)
		{
			isReady = true;
			Instance.EventMgr.Fire(this, new ApplicationInitializedMessage());
		}
		else
		{
			CriticalError(SHSErrorCodes.Code.CoreLoadFail, "Start Fail: " + error);
		}
	}

	protected void OnLaunchTransactionDone(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
	}

	public void OnPrizeWheelWebResponse(ShsWebResponse response)
	{
		if (response.Status != 200)
		{
			appShellStartTransaction.FailStep("prizewheel", "Prize wheel data unavailable. Status:" + response.Status);
			return;
		}
		PrizeWheelManager prizeWheelManager = new PrizeWheelManager();
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		prizeWheelManager.InitializeFromData(dataWarehouse);
		PrizeWheelManager = prizeWheelManager;
		appShellStartTransaction.CompleteStep("prizewheel");
		Instance.EventMgr.Fire(this, new PrizeWheelLoadedMessage(prizeWheelManager));
	}

	public void PromptQuit()
	{
		GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate(string Id, GUIDialogWindow.DialogState state)
		{
			if (state == GUIDialogWindow.DialogState.Ok)
			{
				Quit();
			}
		});
		SHSCommonDialogWindow sHSCommonDialogWindow = new SHSCommonDialogWindow("common_bundle|notification_icon_exit", new Vector2(146f, 190f), new Vector2(18f, 0f), string.Empty, "common_bundle|L_mshs_button_yes", "common_bundle|L_mshs_button_no", typeof(SHSDialogYesButton), typeof(SHSDialogNoButton), false);
		sHSCommonDialogWindow.TitleText = "#exit_dialog_title";
		sHSCommonDialogWindow.Text = "#exit_dialog_message";
		sHSCommonDialogWindow.NotificationSink = notificationSink;
		GUIManager.Instance.ShowDynamicWindow(sHSCommonDialogWindow, GUIControl.ModalLevelEnum.Full);
	}

	// this method added by CSP
	public void PrompServerPlayerMaxReached()
	{
		GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate(string Id, GUIDialogWindow.DialogState state)
		{
			#if UNITY_EDITOR
         		// Application.Quit() does not work in the editor so
         		// UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
         		UnityEditor.EditorApplication.isPlaying = false;
    		#else
         		Application.Quit();
     		#endif	
		});
		SHSCommonDialogWindow sHSCommonDialogWindow = new SHSCommonDialogWindow("common_bundle|notification_icon_exit", new Vector2(146f, 190f), new Vector2(18f, 0f), string.Empty, "common_bundle|L_mshs_button_yes", "common_bundle|L_mshs_button_no", typeof(SHSDialogYesButton), typeof(SHSDialogNoButton), false);
		sHSCommonDialogWindow.TitleText = "ALERT!";
		sHSCommonDialogWindow.Text = "Too many players on server...please try again later.";
		sHSCommonDialogWindow.NotificationSink = notificationSink;
		GUIManager.Instance.ShowDynamicWindow(sHSCommonDialogWindow, GUIControl.ModalLevelEnum.Full);
	}

	// this method added by CSP
	public void PrompInvalidClient()
	{
		GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate(string Id, GUIDialogWindow.DialogState state)
		{
			#if UNITY_EDITOR
         		// Application.Quit() does not work in the editor so
         		// UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
         		UnityEditor.EditorApplication.isPlaying = false;
    		#else
         		Application.Quit();
     		#endif	
		});
		SHSCommonDialogWindow sHSCommonDialogWindow = new SHSCommonDialogWindow("common_bundle|notification_icon_exit", new Vector2(146f, 190f), new Vector2(18f, 0f), string.Empty, "common_bundle|L_mshs_button_yes", "common_bundle|L_mshs_button_no", typeof(SHSDialogYesButton), typeof(SHSDialogNoButton), false);
		sHSCommonDialogWindow.TitleText = "ALERT!";
		sHSCommonDialogWindow.Text = "Invalid client version...please run latest client only.";
		sHSCommonDialogWindow.NotificationSink = notificationSink;
		GUIManager.Instance.ShowDynamicWindow(sHSCommonDialogWindow, GUIControl.ModalLevelEnum.Full);
	}

	public void Quit()
	{
		CspUtils.DebugLog("Shutting down...");
		Application.ExternalCall("HEROUPNS.RedirectToLogin");
		Application.Quit();
	}

	public static void callAnalytics(string category, string action, string label = "", string val = "")
	{
		Application.ExternalCall("HEROUPNS.trackAnalyticsEvent", category, action, label, val);
		CspUtils.DebugLog("callAnalytics " + category + " " + action + " " + label + " " + val);
	}

	public void CriticalError(SHSErrorCodes.Code code)
	{
		CriticalError(code, null);
	}

	public void CriticalError(SHSErrorCodes.Code code, string logMessage)
	{
		SHSErrorCodes.Response response = SHSErrorCodes.GetResponse(code);
		GUIManager.Instance.ShowErrorDialog(response, logMessage);
		PostErrorDialog(logMessage);
	}

	private void PostErrorDialog(string logMessage)
	{
		Instance.EventMgr.Fire(this, new LoadingProgressMessage(LoadingProgressMessage.LoadingState.Complete, 0f, "Critical Error Loading Screen Shutdown"));
		if (!string.IsNullOrEmpty(logMessage))
		{
			CspUtils.DebugLog(logMessage);
		}
	}

	private void preloadGUIBundles()
	{
		if (appShellStartTransaction != null)
		{
			List<string> list = new List<string>();
			list.Add("common_bundle");
			list.Add("debug_bundle");
			list.Add("cursor_bundle");
			list.Add("characters_bundle");
			list.Add("notification_bundle");
			list.Add("communication_bundle");
			list.Add("options_bundle");
			list.Add("prizewheel_bundle");
			list.Add("shopping_bundle");
			list.Add("zonechooser_bundle");
			list.Add("missions_bundle");
			list.Add("missionflyers_bundle");
			list.Add("brawlergadget_bundle");
			list.Add("cardgamegadget_bundle");
			list.Add("mysquadgadget_bundle");
			list.Add("challengerewards_bundle");
			list.Add("achievement_bundle");
			list.Add("persistent_bundle");
			list.Add("smarttip_bundle");
			foreach (string item in list)
			{
				string text = item + "_step";
				if (guiBundleTransaction != null)
				{
					guiBundleTransaction.AddStep(text, TransactionMonitor.DumpTransactionStatus);
				}
				GUIManager.Instance.BundleManager.LoadBundle(item, delegate(AssetBundleLoadResponse response, object extraData)
				{
					if (guiBundleTransaction != null)
					{
						if (!string.IsNullOrEmpty(response.Error))
						{
							guiBundleTransaction.FailStep((string)extraData, "Unable to load");
						}
						else
						{
							guiBundleTransaction.CompleteStep((string)extraData);
						}
					}
				}, text);
			}
			if (guiBundleTransaction != null)
			{
				guiBundleTransaction.CompleteStep("start");
			}
		}
	}

	private void preloadUISounds(GameObject sfxContainer)
	{
		if (appShellStartTransaction != null && uiSfxPreloadTransaction != null)
		{
			GUIManager.UISFX[] sounds = GUIManager.Instance.sounds;
			foreach (GUIManager.UISFX uISFX in sounds)
			{
				uiSfxPreloadTransaction.AddStep(uISFX.name, TransactionMonitor.DumpTransactionStatus);
			}
			BundleLoader.FetchAssetBundle(Helpers.GetAudioBundleName(SABundle.UI), OnUIAudioBundleLoaded, sfxContainer, false);
		}
	}

	private void OnUIAudioBundleLoaded(AssetBundleLoadResponse response, object userData)
	{
		GameObject parent = userData as GameObject;
		if (response.Error != null)
		{
			uiSfxPreloadTransaction.FailStep("bundle", response.Error);
			return;
		}
		uiSfxPreloadTransaction.CompleteStep("bundle");
		GUIManager.UISFX[] sounds = GUIManager.Instance.sounds;
		foreach (GUIManager.UISFX uISFX in sounds)
		{
			ShsAudioSource.PreloadSound(uISFX.sfx, parent, delegate(ShsAudioSource audioSrcInstance, object extraData)
			{
				UnityEngine.Object.Destroy(audioSrcInstance.gameObject);
				GUIManager.UISFX uISFX2 = extraData as GUIManager.UISFX;
				uiSfxPreloadTransaction.CompleteStep(uISFX2.name);
			}, uISFX);
		}
	}

	private void OnBundleTransactionDone(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		guiBundleTransaction = null;
		if (exit == TransactionMonitor.ExitCondition.Success)
		{
			if (appShellStartTransaction != null)
			{
				appShellStartTransaction.CompleteStep("bundles_loaded");
			}
		}
		else if (appShellStartTransaction != null)
		{
			appShellStartTransaction.FailStep("bundles_loaded", error);
		}
	}

	private void OnUISFXPreloadTransactionDone(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		GameObject obj = userData as GameObject;
		uiSfxPreloadTransaction = null;
		if (exit == TransactionMonitor.ExitCondition.Success)
		{
			if (appShellStartTransaction != null)
			{
				appShellStartTransaction.CompleteStep("sfx_preload");
			}
		}
		else if (appShellStartTransaction != null)
		{
			appShellStartTransaction.FailStep("sfx_preload", error);
		}
		UnityEngine.Object.Destroy(obj);
	}

	private void Update()
	{
		//////// block added by CSP ////////////////////////
		if (Input.GetKey ("f9"))
		{
			string text = "m_102S_1_Skydome003";
			AppShell.Instance.Matchmaker2.AnyoneBrawler(CspUtils.OnAcceptBrawler, text);
			CspUtils.DebugLog("Fn9 key being held down!");
		}	
		if (Input.GetKey ("f10"))
        	CspUtils.DebugLog("Fn10 key being held down!");
		/////////////////////////////////////////////////////

		if (ServerConnection != null)
		{
			ServerConnection.Update();
		}
		if (ActivityManager != null)
		{
			ActivityManager.Update();
		}
		if (CounterManager != null)
		{
			CounterManager.Update();
		}
		if (TimerMgr != null)
		{
			TimerMgr.Update();
		}
		if (ChallengeManager != null)
		{
			ChallengeManager.Update();
		}
		if (CelebrationManager != null)
		{
			CelebrationManager.Update();
		}
		if (ExpendablesManager != null)
		{
			ExpendablesManager.Update();
		}
		if (AchievementManager != null)
		{
			AchievementManager.Update();
		}
		if (transitionInProgress)
		{
			transitionTicker += Time.deltaTime;
			if (transitionTicker >= 0.1f)
			{
				CompleteTransition();
			}
		}
	}

	public void Transition(GameController.ControllerType nextGameType, bool noContextOverride)
	{
		ExecuteTransition(nextGameType, noContextOverride);
	}

	public void Transition(GameController.ControllerType nextGameType)
	{
		ExecuteTransition(nextGameType, false);
	}

	private void ExecuteTransition(GameController.ControllerType nextGameType, bool noContextOverride)
	{
		if (transitionTransaction != null)
		{
			cancelTransition();
		}
		previousControllerType = currentControllerType;
		currentControllerType = nextGameType;
		if (this.OnOldControllerUnloading != null)
		{
			this.OnOldControllerUnloading(currentControllerTypeData, GameControllerInfo[currentControllerType]);
		}
		if (!noContextOverride)
		{
			TransitionHandler.SetupTransition(previousControllerType, currentControllerType);
		}
		transitionInProgress = true;
		transitionTicker = 0f;
	}

	public void CompleteTransition()
	{
		transitionTransaction = TransactionMonitor.CreateTransactionMonitor(OnTransitionComplete, 30f, null);
		transitionTransaction.AddStep("oldControllerUnload");
		transitionTransaction.AddStep("newControllerLoad");
		transitionTransaction.AddStep("newControllerReady");
		transitionTransaction.AddStep("uiUnloadComplete");
		GameControllerTypeData gameControllerTypeData = GameControllerInfo[currentControllerType];
		if (gameControllerTypeData == null || gameControllerTypeData.sceneName == null)
		{
			CspUtils.DebugLog("Unable to retrieve gametypeData for given game type: " + currentControllerType);
			return;
		}
		sceneCounter++;
		currentControllerTypeData = gameControllerTypeData;
		GameControllerTypeData gameControllerTypeData2 = GameControllerInfo[previousControllerType];
		if (gameControllerTypeData2 != null)
		{
			gameControllerTypeData2.UnloadAllBoundWindows();
		}
		GUIManager.Instance.HandleTransitionLeave();
		AudioManager.OnSceneTransition();
		Utils.BroadcastUnloadEvent();
		CspUtils.DebugLog("$$$$$$$$$$$$$$$$ Loading Scene: " + gameControllerTypeData.sceneName);
		Application.LoadLevel(gameControllerTypeData.sceneName);
		gameControllerTypeData2 = GameControllerInfo[currentControllerType];
		if (gameControllerTypeData2 != null)
		{
			if (!gameControllerTypeData2.ignoreWindowLoad)
			{
				gameControllerTypeData2.LoadAllBoundWindows();
			}
			gameControllerTypeData2.ignoreWindowLoad = false;
		}
		GUIManager.Instance.HandleTransitionEnter();
		transitionTransaction.CompleteStep("uiUnloadComplete");
		Instance.EventMgr.AddListener<NewControllerLoadingMessage>(OnNewControllerLoadingMessage);
		transitionTransaction.CompleteStep("oldControllerUnload");
		transitionInProgress = false;
	}

	public void OnNewControllerLoadingMessage(NewControllerLoadingMessage message)
	{
		Instance.EventMgr.RemoveListener<NewControllerLoadingMessage>(OnNewControllerLoadingMessage);
		if (this.OnNewControllerLoading != null)
		{
			this.OnNewControllerLoading(currentControllerTypeData, message.controller);
		}
		EventMgr.DetectLeakedObjects();
		Instance.EventMgr.AddListener<NewControllerReadyMessage>(OnNewControllerReadyMessage);
		transitionTransaction.CompleteStep("newControllerLoad");
	}

	public void OnNewControllerReadyMessage(NewControllerReadyMessage message)
	{
		Instance.EventMgr.RemoveListener<NewControllerReadyMessage>(OnNewControllerReadyMessage);
		if (this.OnNewControllerReady != null)
		{
			this.OnNewControllerReady(GameControllerInfo[currentControllerType], message.controller);
		}
		transitionTransaction.CompleteStep("newControllerReady");
	}

	private void OnTransitionComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		switch (exit)
		{
		case TransactionMonitor.ExitCondition.Fail:
			CspUtils.DebugLog("AppShell: Transition failed");
			break;
		case TransactionMonitor.ExitCondition.TimedOut:
			CspUtils.DebugLog("AppShell: Transition timed out");
			break;
		}
		transitionTransaction = null;
	}

	private void cancelTransition()
	{
		if (transitionTransaction == null)
		{
			return;
		}
		if (transitionTransaction.IsStepCompleted("oldControllerUnload"))
		{
			if (transitionTransaction.IsStepCompleted("newControllerLoad"))
			{
				if (transitionTransaction.IsStepCompleted("newControllerReady"))
				{
				}
				Instance.EventMgr.RemoveListener<NewControllerReadyMessage>(OnNewControllerReadyMessage);
			}
			Instance.EventMgr.RemoveListener<NewControllerLoadingMessage>(OnNewControllerLoadingMessage);
		}
		transitionTransaction = null;
	}

	private void SetupGameControllers()
	{
		GameControllerInfo = new Dictionary<GameController.ControllerType, GameControllerTypeData>();
		List<GUIManager.WindowBinding> list = new List<GUIManager.WindowBinding>();
		GameControllerInfo[GameController.ControllerType.CardGame] = new GameControllerTypeData("CardGame", "SHSMainWindow/SHSCardGameMainWindow", new GUIManager.WindowBinding(typeof(SHSCardGameMainWindow), "SHSMainWindow", "SHSCardGameMainWindow"));
		list.Add(new GUIManager.WindowBinding(typeof(SHSSocialMainWindow), "SHSMainWindow", "SHSSocialMainWindow"));
		list.Add(new GUIManager.WindowBinding(typeof(SHSSocialCharacterSelectionMainWindow), "SHSMainWindow/SHSSocialMainWindow", "SHSSocialCharacterSelectionMainWindow"));
		GameControllerInfo[GameController.ControllerType.SocialSpace] = new GameControllerTypeData("SocialSpace", "SHSMainWindow/SHSSocialMainWindow", list);
		list.Clear();
		list.Add(new GUIManager.WindowBinding(typeof(SHSLoginWindow), "SHSFrontendWindow", "SHSLoginWindow"));
		list.Add(new GUIManager.WindowBinding(typeof(SHSSystemMainWindow), "SHSFrontendWindow", "SHSSystemMainWindow"));
		GameControllerInfo[GameController.ControllerType.FrontEnd] = new GameControllerTypeData("__FrontEnd", "SHSFrontendWindow", list);
		list.Clear();
		list.Add(new GUIManager.WindowBinding(typeof(SHSBrawlerMainWindow), "SHSMainWindow", "SHSBrawlerMainWindow"));
		list.Add(new GUIManager.WindowBinding(typeof(SHSBrawlerCharacterSelectionMainWindow), "SHSMainWindow/SHSBrawlerMainWindow", "SHSBrawlerCharacterSelectionMainWindow"));
		GameControllerInfo[GameController.ControllerType.Brawler] = new GameControllerTypeData("Brawler", "SHSMainWindow/SHSBrawlerMainWindow", list);
		GameControllerInfo[GameController.ControllerType.HeadQuarters] = new GameControllerTypeData("HeadQuarters2", "SHSMainWindow/SHSHQWindow", new GUIManager.WindowBinding(typeof(SHSHQWindow), "SHSMainWindow", "SHSHQWindow"));
		list.Clear();
		list.Add(new GUIManager.WindowBinding(typeof(SHSAutoDeckBuilderWindow), "SHSMainWindow", "SHSAutoDeckBuilderWindow"));
		list.Add(new GUIManager.WindowBinding(typeof(SHSDeckBuilderWindow), "SHSMainWindow/SHSAutoDeckBuilderWindow", "SHSDeckBuilderWindow"));
		GameControllerInfo[GameController.ControllerType.DeckBuilder] = new GameControllerTypeData("DeckBuilder", "SHSMainWindow/SHSAutoDeckBuilderWindow", list);
		GameControllerInfo[GameController.ControllerType.Debug_TimLand] = new GameControllerTypeData("TimLand", "SHSMainWindow/SHSTimLandWindow", new GUIManager.WindowBinding(typeof(SHSTimLandWindow), "SHSMainWindow", "SHSTimLandWindow"));
		GameControllerInfo[GameController.ControllerType.Debug_AndyLand] = new GameControllerTypeData("Andy Mission Briefing Test", "SHSMainWindow/AndyLandWindow", new GUIManager.WindowBinding(typeof(AndyLandWindow), "SHSMainWindow", "AndyLandWindow"));
		list.Clear();
		list.Add(new GUIManager.WindowBinding(typeof(SHSBrawlerRailsMainWindow), "SHSMainWindow", "SHSBrawlerRailsMainWindow"));
		list.Add(new GUIManager.WindowBinding(typeof(SHSBrawlerMainWindow), "SHSMainWindow", "SHSBrawlerMainWindow"));
		list.Add(new GUIManager.WindowBinding(typeof(SHSBrawlerCharacterSelectionMainWindow), "SHSMainWindow/SHSBrawlerMainWindow", "SHSBrawlerCharacterSelectionMainWindow"));
		GameControllerInfo[GameController.ControllerType.RailsBrawler] = new GameControllerTypeData("RailsBrawler", "SHSMainWindow/SHSBrawlerRailsMainWindow", list);
		GameControllerInfo[GameController.ControllerType.RailsGameWorld] = new GameControllerTypeData("RailsGameWorld", "SHSMainWindow/SHSGameWorldRailsMainWindow", new GUIManager.WindowBinding(typeof(SHSGameWorldRailsMainWindow), "SHSMainWindow", "SHSGameWorldRailsMainWindow"));
		GameControllerInfo[GameController.ControllerType.RailsHq] = new GameControllerTypeData("HQTutorial", "SHSMainWindow/SHSHqRailsMainWindow", new GUIManager.WindowBinding(typeof(SHSHqRailsMainWindow), "SHSMainWindow", "SHSHqRailsMainWindow"));
		GameControllerInfo[GameController.ControllerType.GameShell] = new GameControllerTypeData(null, "SHSMainWindow");
		GameControllerInfo[GameController.ControllerType.CardHub] = new GameControllerTypeData("CardHub", "SHSMainWindow/SHSCardHubWindow");
		GameControllerInfo[GameController.ControllerType.Test] = new GameControllerTypeData("Chad Test Scene", null);
		GameControllerInfo[GameController.ControllerType.None] = new GameControllerTypeData("Blank", "SHSBlankWindow");
		GameControllerInfo[GameController.ControllerType.Fallback] = new GameControllerTypeData("Fallback", "SHSMainWindow");
		GameControllerInfo[GameController.ControllerType.CardGameDeckTest] = new GameControllerTypeData("CardGameDeckTest", "SHSMainWindow/SHSCardGameMainWindow");
		GameControllerInfo[GameController.ControllerType.ArcadeShell] = new GameControllerTypeData("ArcadeShell", "SHSBlankWindow");
	}

	public void OnConfigServersLoaded(DataWarehouse serverConfigData)
	{
		NetworkEnvironment = serverConfigData.TryGetString("//environment", "INT");   // CSP - server name
		ServerConfig = serverConfigData.GetData("//" + NetworkEnvironment);
		isHeroAuth = serverConfigData.TryGetBool("//hero_auth", true);
		if (ServerConfig.Navigator == null && appShellStartTransaction != null)
		{
			appShellStartTransaction.FailStep("serverconfig", "Invalid server config specified");
			return;
		}
		if (SHSDebugInput.Inst != null)
		{
			SHSDebugInput.Inst.OnConfigServersLoaded();
		}
		GraphicsOptions.Load(serverConfigData.TryGetInt("//graphics_profile", 1));
		BundleLoader.OnServerConfigLoaded(ServerConfig);
		WebAssetCache.OnServerConfigLoaded(ServerConfig);
		TargetFrameRate = serverConfigData.TryGetInt("//target_fps", -1);
		SHSDebug.PassThrough = serverConfigData.TryGetBool("//file_logging", Application.isEditor);
		try
		{
			string text = Instance.ServerConfig.TryGetString("build_timestamp", null);
			if (text != null)
			{
				string b = Instance.ServerConfig.TryGetString("//build_timestamp", null);
				if (text != b)
				{
					CspUtils.DebugLogError("The build_timestamps do not match, using one from environment block");
				}
			}
			else
			{
				text = Instance.ServerConfig.TryGetString("//build_timestamp", null);
			}
			if (!string.IsNullOrEmpty(text))
			{
				CspUtils.DebugLog("Build Timestamp <" + text + ">.");
				if (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
				{
					try
					{
						Application.ExternalCall("HEROUPNS.SetUnityBuildVersion", text);
					}
					catch (Exception arg)
					{
						CspUtils.DebugLog("Unable to set build version with error <" + arg + ">.");
					}
				}
			}
		}
		catch (Exception arg2)
		{
			CspUtils.DebugLog("Failed to find the build time stamp with message: " + arg2);
		}
		if (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
		{
			string value;
			if (Instance.WebService.UrlParameters.TryGetValue("auto", out value))
			{
				automateclient = value.Equals("true", StringComparison.OrdinalIgnoreCase);
			}
			string value2;
			if (Instance.WebService.UrlParameters.TryGetValue("conf", out value2))
			{
				configFile = value2;
			}
			string value3;
			if (Instance.WebService.UrlParameters.TryGetValue("script", out value3))
			{
				scriptFile = value3;
			}
			string value4;
			if (Instance.WebService.UrlParameters.TryGetValue("user", out value4))
			{
				username = value4;
			}
			string value5;
			if (Instance.WebService.UrlParameters.TryGetValue("pass", out value5))
			{
				password = value5;
			}
			if (automateclient)
			{
				instance.gameObject.AddComponent<AutomationManager>();
			}
		}
		else
		{
			//CSP instance.gameObject.AddComponent<AutomationManager>();
		}
		if (appShellStartTransaction != null)
		{
			appShellStartTransaction.CompleteStep("serverconfig");
		}
	}

	public void StubServerConnection()
	{
		if (ServerConnection != null)
		{
			IDisposable disposable = ServerConnection as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			ServerConnection = null;
		}
		ServerConnection = new NetworkManagerStub();
	}

	protected void OnCharDescriptionLoaded(GameDataLoadResponse response, object extraData)
	{
		CspUtils.DebugLog("response=" + response);
		CspUtils.DebugLog("extraData=" + extraData);

		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("char_desc", response.Error);
			CharacterDescriptionManager = null;
			CriticalError(SHSErrorCodes.Code.ApplicationLoadDataFail, response.Error);
		}
		else
		{
			CharacterDescriptionManager = (response.DataDefinition as CharacterDescriptionsManager);
			appShellStartTransaction.CompleteStep("char_desc");
		}
	}

	protected void OnXpToLevelDataLoaded(ShsWebResponse response)
	{
		if (response.Status != 200)
		{
			appShellStartTransaction.FailStep("xp_to_level", response.Status.ToString());
			XpToLevelDefinition.Instance = null;
			CriticalError(SHSErrorCodes.Code.ApplicationLoadDataFail, response.Status.ToString());
		}
		else
		{
			Dictionary<string, List<XpToLevelDefinitionJson>> dictionary = JsonMapper.ToObject<Dictionary<string, List<XpToLevelDefinitionJson>>>(response.Body);
			foreach (XpToLevelDefinitionJson item in dictionary["level-chart"])
			{
				XpToLevelDefinition.Instance.AddLevel(item.level, item.start);
			}
			appShellStartTransaction.CompleteStep("xp_to_level");
		}
	}

	public void OnCardQuestDataWebResponse(ShsWebResponse response)
	{
		CardQuestManager.InitializeFromData(response);
	}

	protected void OnEmotesDataLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("emotes", response.Error);
			EmotesDefinition.Instance = null;
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
		}
		else
		{
			EmotesDefinition.Instance = (response.DataDefinition as EmotesDefinition);
			appShellStartTransaction.CompleteStep("emotes");
		}
	}

	protected void OnMotdWebResponse(ShsWebResponse response)
	{
		Dictionary<string, MotdDefinitionJson> dictionary = JsonMapper.ToObject<Dictionary<string, MotdDefinitionJson>>(response.Body);
		MotdDefinitionJson value;
		if (dictionary.TryGetValue("daily_mission", out value))
		{
			MotdDefinition.Instance.MissionOwnableTypeId = value.ownable_type_id.ToString();
		}
		appShellStartTransaction.CompleteStep("motd");
	}

	protected void OnBotdWebResponse(ShsWebResponse response)
	{
		Dictionary<string, BotdDefinitionJson> dictionary = JsonMapper.ToObject<Dictionary<string, BotdDefinitionJson>>(response.Body);
		BotdDefinitionJson value;
		if (dictionary.TryGetValue("daily_quest", out value))
		{
			BotdDefinition.Instance.BattleOwnableTypeId = value.ownable_type_id.ToString();
		}
		appShellStartTransaction.CompleteStep("botd");
	}

	protected void OnStatRequirementsLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("brawler_leveling_stats", response.Error);
			StatLevelReqsDefinition.Instance = null;
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
		}
		else
		{
			StatLevelReqsDefinition.Instance = (response.DataDefinition as StatLevelReqsDefinition);
			appShellStartTransaction.CompleteStep("brawler_leveling_stats");
		}
	}

	protected void OnBrawlerScoringLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("brawler_scoring", response.Error);
			BrawlerScoringDefinition.Instance = null;
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
		}
		else
		{
			BrawlerScoringDefinition.Instance = (response.DataDefinition as BrawlerScoringDefinition);
			appShellStartTransaction.CompleteStep("brawler_scoring");
		}
	}

	protected void OnDeckBuilderConfigLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("deckbuilder_config", response.Error);
			DeckBuilderConfigDefinition.Instance = null;
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
		}
		else
		{
			DeckBuilderConfigDefinition.Instance = (response.DataDefinition as DeckBuilderConfigDefinition);
			appShellStartTransaction.CompleteStep("deckbuilder_config");
		}
	}

	protected void OnActionTimesLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("brawler_action_times", response.Error);
			ActionTimesDefinition.Instance = null;
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
		}
		else
		{
			ActionTimesDefinition.Instance = (response.DataDefinition as ActionTimesDefinition);
			appShellStartTransaction.CompleteStep("brawler_action_times");
		}
	}

	protected void OnItemDefinitionsLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("item_defs", response.Error);
			ItemDictionary = null;
			CspUtils.DebugLog("The Item Dictionary failed to load!  The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
		}
		else
		{
			ItemDictionary = new ItemDefinitionDictionary(response.Data);
			appShellStartTransaction.CompleteStep("item_defs");
		}
	}

	protected void OnFeatureDefinitionsLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("feature_defs", response.Error);
			ItemDictionary = null;
			CspUtils.DebugLog("The Feature data failed to load!  The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
		}
		else
		{
			FeaturesManager.InitializeFromData(response.Data);
			appShellStartTransaction.CompleteStep("feature_defs");
		}
	}

	protected void OnPetsDefinitionsLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("pet_defs", response.Error);
			ItemDictionary = null;
			CspUtils.DebugLog("The Pet data failed to load!  The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
		}
		else
		{
			PetDataManager.InitializeFromData(response.Data);
			appShellStartTransaction.CompleteStep("pet_defs");
		}
	}

	protected void OnScavengerDefinitionsLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("scavenger_defs", response.Error);
			ItemDictionary = null;
			CspUtils.DebugLog("The Scavenger data failed to load!  The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
		}
		else
		{
			OwnableDefinition.loadScavengerInfo(response.Data);
			appShellStartTransaction.CompleteStep("scavenger_defs");
		}
	}

	protected void OnExpendableDefinitionsLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("expendable_defs", response.Error);
			ItemDictionary = null;
			CspUtils.DebugLog("The Expendable data failed to load!  The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
		}
		else
		{
			ExpendablesManager.InitializeFromData(response.Data);
			appShellStartTransaction.CompleteStep("expendable_defs");
		}
	}

	protected void OnMissionDefsWebResponse(ShsWebResponse response)
	{
		Dictionary<string, List<MissionManifestEntryJson>> dictionary = JsonMapper.ToObject<Dictionary<string, List<MissionManifestEntryJson>>>(response.Body);
		MissionManifest = new MissionManifest();
		foreach (MissionManifestEntryJson item in dictionary["brawler_missions"])
		{
			MissionManifest.Add(item);
		}
		appShellStartTransaction.CompleteStep("mission_defs");
	}

	protected void OnLevelUpRewardWebResponse(ShsWebResponse response)
	{
		Dictionary<string, Dictionary<string, List<LevelUpRewardItemJson>>> dictionary = JsonMapper.ToObject<Dictionary<string, Dictionary<string, List<LevelUpRewardItemJson>>>>(response.Body);
		LevelUpRewardItemsManifest = new LevelUpRewardItemsManifest();
		foreach (KeyValuePair<string, List<LevelUpRewardItemJson>> item in dictionary["hero_level_rewards"])
		{
			string key = item.Key;
			LevelUpRewardItems levelUpRewardItems = new LevelUpRewardItems(key);
			LevelUpRewardItemsManifest.Add(key, levelUpRewardItems);
			foreach (LevelUpRewardItemJson item2 in item.Value)
			{
				levelUpRewardItems.AddItem(item2.level, item2.ownable_type_id);
			}
		}
		appShellStartTransaction.CompleteStep("level_up_items_defs");
	}
	
	protected void OnDutilLoaded(ShsWebResponse response){
		
		var d = JsonMapper.ToObject<List<string>>(response.Body);
		DUtils.single = d;
		
	}
	
	protected void OnDutilAfk(ShsWebResponse response){
		
		CspUtils.DebugLog("SETTING MAX AFK");
		float d = float.Parse(response.Body);
		CspUtils.DebugLog(d);
		SHSInput.maxAFK = d;
		
	}
	
	protected void OnHQRoomsManifestLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("mission_defs", response.Error);
			HQRoomManifest = null;
			CspUtils.DebugLog("The HQ Room list failed to load!  The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
		}
		else
		{
			HQRoomManifest = new HQRoomManifest(response.Data);
			appShellStartTransaction.CompleteStep("hqrooms_defs");
		}
	}

	protected void OnCounterLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("counters", response.Error);
			CspUtils.DebugLog("The Counter definition file failed to load!  The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		if (CounterManager != null)
		{
			CounterManager.LoadConfiguration(response.Data);
		}
		appShellStartTransaction.CompleteStep("counters");
	}

	private void DetermineWelcomeScreenPath()
	{
		if (Application.isWebPlayer)
		{
			CspUtils.DebugLog("transition complete being assigned!!!!");
			launchTransitionProperties.onTransitionComplete = TransitionHandler.WelcomeScreenRoutine;
			Instance.TransitionHandler.ModifyTransitionProperties("AppShell_LaunchTransaction", launchTransitionProperties);
		}
	}

	protected void OnAchievementsLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("achievements", response.Error);
			AchievementsManager = null;
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
		}
		else
		{
			AchievementsManager = (response.DataDefinition as AchievementsManager);
			appShellStartTransaction.CompleteStep("achievements");
		}
	}

	protected void OnSmartTipsLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("smarttips", response.Error);
			AchievementsManager = null;
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
		}
		else
		{
			SmartTipsManager = (response.DataDefinition as SmartTipsManager);
			appShellStartTransaction.CompleteStep("smarttips");
		}
	}

	protected void OnActivitiesLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("activities", response.Error);
			CspUtils.DebugLog("The Activities definition file failed to load!  The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		if (ActivityManager != null)
		{
			ActivityManager.LoadConfiguration(response.Data);
		}
		appShellStartTransaction.CompleteStep("activities");
	}

	protected void OnArcadeGamesLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("arcade", response.Error);
			CspUtils.DebugLog("The arcade games list is not loadable. The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		if (ArcadeManager != null)
		{
			ArcadeManager.InitializeFromData(response.Data);
		}
		appShellStartTransaction.CompleteStep("arcade");
	}

	protected void OnChallengesLoaded(GameDataLoadResponse response, object extraData)
	{
		if (!string.IsNullOrEmpty(response.Error))
		{
			appShellStartTransaction.FailStep("challenges", response.Error);
			CspUtils.DebugLog("The challenges xml is not loadable. The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		if (ChallengeManager != null)
		{
			ChallengeManager.InitializeFromData(response.Data);
		}
		appShellStartTransaction.CompleteStep("challenge");
	}

	protected void OnMenuChatLoaded(GameDataLoadResponse response, object extraData)
	{
		if (MenuChatManager != null)
		{
			MenuChatManager.LoadConfiguration(response.Data);
		}
		appShellStartTransaction.CompleteStep("menuchat");
	}

	protected void OnCharacterDescriptionsLoaded(TextAsset source)
	{
		if (CharacterDescriptionManager != null)
		{
			DataWarehouse dataWarehouse = new DataWarehouse(source.text);
			dataWarehouse.Parse();
			CharacterDescriptionManager.InitializeFromData(dataWarehouse);
		}
	}

	public void OnRecieveWindowStatusFromWebPage(string status)
	{
		if (Screen.fullScreen)
		{
			CspUtils.DebugLog(string.Format("In full screen ignoring status - received Window Status from web page. {0}", status));
			return;
		}
		if (status.Equals("active", StringComparison.OrdinalIgnoreCase))
		{
			hasFocus = true;
		}
		else if (status.Equals("inactive", StringComparison.OrdinalIgnoreCase))
		{
			hasFocus = false;
		}
		else
		{
			CspUtils.DebugLog("Unable to decode window status from web page message <" + status + ">.");
			hasFocus = true;
		}
		WindowStatusMessage msg = new WindowStatusMessage(hasFocus);
		EventMgr.Fire(this, msg);
	}

	private void OpenAvailableBoosterPacks()
	{
		if (Instance.Profile != null && Instance.Profile.AvailableBoosterPacks != null && Instance.Profile.AvailableBoosterPacks.Count > 0)
		{
			foreach (AvailableBoosterPack value in Instance.Profile.AvailableBoosterPacks.Values)
			{
				int num = int.Parse(value.BoosterPackId);
				OwnableDefinition def = OwnableDefinition.getDef(num);
				if (def != null)
				{
					SHSBoosterPackOpeningWindow dialogWindow = new SHSBoosterPackOpeningWindow(num, def.shoppingName, delegate
					{
						Instance.Profile.StartBoosterPacksFetch(delegate
						{
							OpenAvailableBoosterPacks();
						});
					});
					GUIManager.Instance.ShowDynamicWindow(dialogWindow, GUIControl.ModalLevelEnum.Full);
					break;
				}
			}
		}
		else
		{
			ShsWebService.SafeJavaScriptCall("HEROUPNS.ReturnToWebStore();");
		}
	}

	public void StoreLocationInfo()
	{
		SharedHashTable["SocialSpaceLevelCurrent"] = SharedHashTable["SocialSpaceLevel"];
		SharedHashTable["SocialSpaceCharacterCurrent"] = SharedHashTable["SocialSpaceCharacter"];
	}

	public void QueueLocationInfo()
	{
		SharedHashTable["SocialSpaceLevel"] = SharedHashTable["SocialSpaceLevelCurrent"];
		SharedHashTable["SocialSpaceCharacter"] = SharedHashTable["SocialSpaceCharacterCurrent"];
	}

	public IEnumerator PollFPS()
	{
		fps = 60f;
		while (true)
		{
			int startSceneCounter = Time.frameCount;
			float startTime = Time.realtimeSinceStartup;
			yield return new WaitForSeconds(1f);
			fps = (float)(Time.frameCount - startSceneCounter) / (Time.realtimeSinceStartup - startTime);
		}
	}

	public void disableControl(GUIControl control, float duration = 2f)
	{
		StartCoroutine(disableControlBlink(control, duration));
	}

	private IEnumerator disableControlBlink(GUIControl control, float duration)
	{
		control.IsEnabled = false;
		yield return new WaitForSeconds(duration);
		control.IsEnabled = true;
	}

	public void delayedAchievementEvent(string heroStr, string type, string subtype, string str1, float delay = 3f)
	{
		AchievementManager.queueAchievementEvent(heroStr, type, subtype, str1, delay);
	}

	public IEnumerator SendAchievementMessage(string heroStr, string type, string subtype, string str1, float delay = 3f)
	{
		yield return new WaitForSeconds(delay);
		Instance.EventReporter.ReportAchievementEvent(heroStr, type, subtype, 1, str1);
	}

	public IEnumerator PollForShards()
	{
		CspUtils.DebugLog("PollForShards begin");
		while (Instance == null || Instance.Profile == null || SocialSpaceController.Instance == null)
		{
			yield return new WaitForSeconds(2f);
		}
		CspUtils.DebugLog("profile and social space loaded, do initial ");
		Instance.Profile.CollectShard();
		yield return new WaitForSeconds(10f);
		while (true)
		{
			yield return new WaitForSeconds(1f);
			Instance.Profile.nextShardTime -= 1f;
			if (Instance.Profile.nextShardTime < 0f)
			{
				Instance.Profile.CollectShard();
				yield return new WaitForSeconds(30f);
			}
		}
	}

	public IEnumerator PollCurrency()
	{
		for (int i = 0; i < 9; i++)
		{
			yield return new WaitForSeconds(20f);
			if (instance.Profile == null)
			{
				i++;
				continue;
			}
			instance.Profile.StartCurrencyFetch();
			CspUtils.DebugLog("Starting auto-poll of currency");
		}
	}

	public Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> GetKeyList(GUIControl.KeyInputState inputState)
	{
		return new Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate>();
	}

	public void ConfigureKeyBanks()
	{
	}

	public bool IsDescendantHandler(IInputHandler handler)
	{
		return false;
	}

	public void OnRecieveFullScreenFromWebPage(string parametersString)
	{
		AutoFullScreenToggle();
	}

	public void AutoFullScreenToggle()
	{
		if (!Screen.fullScreen)
		{
			windowedWidth = Screen.width;
			windowedHeight = Screen.height;
			Resolution resolution = GraphicsOptions.FindBestResolution();
			CspUtils.DebugLog("Switching to " + resolution.width + "x" + resolution.height);
			AboutToChangeFullscreenState(true);
			Screen.SetResolution(resolution.width, resolution.height, true);
			ChangedFullscreenState(true);
		}
		else
		{
			AboutToChangeFullscreenState(false);
			if (windowedWidth > 0)
			{
				Screen.SetResolution(windowedWidth, windowedHeight, false);
			}
			else
			{
				Screen.fullScreen = false;
			}
			ChangedFullscreenState(false);
		}
	}

	public bool ForceWindowedMode()
	{
		if (!Screen.fullScreen)
		{
			return false;
		}
		AboutToChangeFullscreenState(false);
		if (windowedWidth > 0)
		{
			Screen.SetResolution(windowedWidth, windowedHeight, false);
		}
		else
		{
			Screen.fullScreen = false;
		}
		StartCoroutine(ChangedFullscreenState(false));
		return true;
	}

	protected void AboutToChangeFullscreenState(bool newState)
	{
		if (this.OnAboutToChangeFullscreenState != null)
		{
			this.OnAboutToChangeFullscreenState(newState);
		}
	}

	protected IEnumerator ChangedFullscreenState(bool newState)
	{
		yield return 0;
		if (this.OnChangedFullscreenState != null)
		{
			this.OnChangedFullscreenState(newState);
		}
	}
}
