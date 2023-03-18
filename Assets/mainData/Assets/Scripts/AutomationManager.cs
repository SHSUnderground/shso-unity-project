using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class AutomationManager : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum LoginState
	{
		None,
		inProgress,
		Succeeded,
		Failed
	}

	protected const string SHS_WEBSERVICE_URL = "content$";

	protected const string AUTOMATION_STANDALONE_CONFIG_URI = "/Automation/StandaloneConfig/automation.xml";

	protected const string AUTOMATION_WEB_CONFIG_URL = "Automation/WebConfig/";

	protected const string AUTOMATION_WEB_SCRIPT_URL = "Automation/";

	protected const string AUTOMATION_WSC_LOG_URL = "http://taswk7-1709/qa/shs/logging.php";

	protected const string AUTOMATION_WSC_STATS_URL = "http://taswk7-1709/qa/shs/logbvtstats.php";

	protected const string AUTOMATION_CONFIG_FILE = "automation.xml";

	protected const int DEFAULT_WAIT_TIME = 5;

	private FileStream fstrm;

	private MemoryStream memstrm;

	private string logFilePath;

	private string scriptFilePath;

	private string subScriptFilePath;

	private string testCaseName;

	private string commentString;

	private int loopIterations;

	private int numberOfSteps;

	private int numberOfFailures;

	private string testResult;

	private XmlTextWriter xmlLogWriter;

	public string HQRoomId;

	private Vector3 objlocation;

	private bool moveprogress;

	private bool fightmode;

	private int spawnobjcount;

	private List<GameObject> goPickUpList;

	public bool isPrizeWheelOpen;

	public bool isPrizeWheelRunning;

	public int npcCount;

	public int userCount;

	public int matchPlayed;

	public int matchWon;

	public int matchLost;

	public int matchIncomplete;

	public string cardGameLog;

	public static bool isRunning;

	public string chatMessageReceived;

	private SHSDebug.DebugLogger LogError;

	public static string message;

	public int errGameWorld;

	public int errHeadQuareter;

	public int errBrawler;

	public int errCardGame;

	public int errOther;

	public int nGameWorld;

	public int nBrawler;

	public int nHeadQuarters;

	public int nCardGame;

	public int nOther;

	private Dictionary<string, string> fallenObjects;

	protected static AutomationManager instance;

	private bool automationmode;

	private int currentstage;

	private string missionname;

	public bool GUI_LoginComplete;

	public GameController.ControllerType activeController;

	public LoadingProgressMessage.LoadingState loadingState;

	public bool zoneLoaded;

	public string playerCharacterSelected;

	private Queue<AutomationCmd> automationCmdsQ;

	private LoginState loginState;

	protected GameObject localPlayer;

	protected GameObject localEnemy;

	public static AutomationManager Instance
	{
		get
		{
			return instance;
		}
	}

	public int NumberOfFailures
	{
		get
		{
			return numberOfFailures;
		}
		set
		{
			numberOfFailures = value;
		}
	}

	public Queue<AutomationCmd> commandsQ
	{
		get
		{
			return automationCmdsQ;
		}
		set
		{
			automationCmdsQ = value;
		}
	}

	public virtual GameObject LocalPlayer
	{
		get
		{
			return localPlayer;
		}
		set
		{
			localPlayer = value;
		}
	}

	public virtual GameObject LocalEnemy
	{
		get
		{
			return localEnemy;
		}
		set
		{
			localEnemy = value;
		}
	}

	public bool automateClient
	{
		get
		{
			return automationmode;
		}
		set
		{
			automationmode = value;
		}
	}

	public int currentStage
	{
		get
		{
			return currentstage;
		}
		set
		{
			currentstage = value;
		}
	}

	public string missionName
	{
		get
		{
			return missionname;
		}
		set
		{
			missionname = value;
		}
	}

	public Vector3 objLocation
	{
		get
		{
			return objlocation;
		}
		set
		{
			objlocation = value;
		}
	}

	public bool moveInProgress
	{
		get
		{
			return moveprogress;
		}
		set
		{
			moveprogress = value;
		}
	}

	public bool inFightMode
	{
		get
		{
			return fightmode;
		}
		set
		{
			fightmode = value;
		}
	}

	public int LoopIterations
	{
		get
		{
			return loopIterations;
		}
		set
		{
			loopIterations = value;
		}
	}

	public string ID
	{
		get
		{
			return Guid.NewGuid().ToString("N");
		}
	}

	public int GetPrizeWheelGoldStops
	{
		get
		{
			int num = 0;
			for (int i = 0; i < AppShell.Instance.Profile.PrizeWheelState.Count; i++)
			{
				if (Convert.ToInt16(AppShell.Instance.Profile.PrizeWheelState[i]) == 1)
				{
					num++;
				}
			}
			return num;
		}
	}

	public string subScriptPath
	{
		get
		{
			return subScriptFilePath;
		}
	}

	public int spawnObjCount
	{
		get
		{
			return spawnobjcount;
		}
		set
		{
			spawnobjcount = value;
		}
	}

	public List<GameObject> pickUpList
	{
		get
		{
			return goPickUpList;
		}
		set
		{
			goPickUpList = value;
		}
	}

	public bool isWebPlayer
	{
		get
		{
			return Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer;
		}
	}

	public bool isBrawler
	{
		get
		{
			return activeController == GameController.ControllerType.Brawler;
		}
	}

	public bool isGameWorld
	{
		get
		{
			return activeController == GameController.ControllerType.SocialSpace;
		}
	}

	public bool isHeadQuarters
	{
		get
		{
			return activeController == GameController.ControllerType.HeadQuarters;
		}
	}

	public LoginState LoginStatus
	{
		get
		{
			return loginState;
		}
		set
		{
			loginState = value;
		}
	}

	public Dictionary<string, string> FallenObjects
	{
		get
		{
			return fallenObjects;
		}
		set
		{
			fallenObjects = value;
		}
	}

	public List<NetworkManager.UserInfo> GetAllUsers
	{
		get
		{
			IServerConnection serverConnection = AppShell.Instance.ServerConnection;
			return serverConnection.GetGameAllUsers();
		}
	}

	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
			testResult = "TEST SUCCEEDED";
			automationmode = false;
			GUI_LoginComplete = false;
			moveprogress = false;
			fightmode = false;
			loopIterations = 0;
			goPickUpList = new List<GameObject>();
			spawnobjcount = 0;
			activeController = GameController.ControllerType.None;
			loadingState = LoadingProgressMessage.LoadingState.Started;
			zoneLoaded = false;
			loginState = LoginState.None;
			localPlayer = null;
			playerCharacterSelected = string.Empty;
			isPrizeWheelOpen = false;
			isPrizeWheelRunning = false;
			matchPlayed = 0;
			matchWon = 0;
			matchLost = 0;
			matchIncomplete = 0;
			errBrawler = 0;
			errCardGame = 0;
			errHeadQuareter = 0;
			errGameWorld = 0;
			errOther = 0;
			nGameWorld = 0;
			nBrawler = 0;
			nHeadQuarters = 0;
			nCardGame = 0;
			nOther = 0;
			isRunning = false;
			LogError = (SHSDebug.DebugLogger)Delegate.Combine(LogError, new SHSDebug.DebugLogger(ManagedLogError));
			fallenObjects = new Dictionary<string, string>();
			HQRoomId = string.Empty;
			chatMessageReceived = string.Empty;
			AppShell.Instance.EventMgr.AddListener<NewControllerReadyMessage>(OnNewControllerReadyMessage);
			AppShell.Instance.EventMgr.AddListener<GameControllerExitedMessage>(OnGameControllerExited);
			AppShell.Instance.EventMgr.AddListener<LoadingProgressMessage>(OnLoadingProgressMessage);
			AppShell.Instance.EventMgr.AddListener<EntitySpawnMessage>(OnEntitySpawned);
			AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(OnEntityDespawned);
			AppShell.Instance.EventMgr.AddListener<LoginCompleteMessage>(OnLoginCompleted);
			AppShell.Instance.EventMgr.AddListener<CharacterSelectedMessage>(OnPlayerCharacterSelected);
			AppShell.Instance.EventMgr.AddListener<ZoneLoadedMessage>(OnZoneLoaded);
			AppShell.Instance.EventMgr.AddListener<CombatCharacterHitMessage>(OnCombatCharacterHit);
			AppShell.Instance.EventMgr.AddListener<CombatCharacterAggroMessage>(OnCharacterAggroEvent);
			AppShell.Instance.EventMgr.AddListener<HQRoomChangedMessage>(OnRoomChanged);
			AppShell.Instance.EventMgr.AddListener<GameWorldOpenChatMessage>(OnChatMsgReceived);
			AppShell.Instance.EventMgr.AddListener<WelcomeResponseMessage>(OnWelcomeResponse);
		}
		else
		{
			CspUtils.DebugLog("AutomationManager already created, attempt to create a second one will lead to instability.");
		}
	}

	private static void ManagedLogError(object message)
	{
		CspUtils.DebugLog("Got the message: " + message);
	}

	public bool Start()
	{
		if (isWebPlayer)
		{
			StartWebAutomation();
		}
		else
		{
			StartStandaloneAutomation();
		}
		return true;
	}

	protected void StartStandaloneAutomation()
	{
		string text = Environment.GetEnvironmentVariable("AUTOMATION_CONFIGFILE_URI");
		if (text == null)
		{
			text = Application.dataPath + "/Automation/StandaloneConfig/automation.xml";
		}
		if (!File.Exists(text))
		{
			CspUtils.DebugLog("Automation failed - configuration file not found " + text);
		}
		else if (EvaluateConfigFile(ReadConfigFile(text)))
		{
			if (automateClient)
			{
				StartAutomation();
			}
		}
		else
		{
			CspUtils.DebugLogError("Could not load configuration file " + text);
		}
	}

	protected void StartWebAutomation()
	{
		string uri = "content$Automation/WebConfig/automation.xml";
		if (!AppShell.Instance.configFile.Equals(string.Empty))
		{
			uri = "content$Automation/WebConfig/" + AppShell.Instance.configFile;
		}
		AppShell.Instance.WebService.StartRequest(uri, OnWebConfigLoaded, ShsWebService.ShsWebServiceType.Text);
	}

	public void OnWebConfigLoaded(ShsWebResponse response)
	{
		EvaluateConfigFile(response.Body);
		CspUtils.DebugLog(response.Body);
		if (automateClient && SetupLogging())
		{
			string uri = "content$Automation/" + scriptFilePath;
			if (!AppShell.Instance.scriptFile.Equals(string.Empty))
			{
				scriptFilePath = "Scripts/" + AppShell.Instance.scriptFile;
				uri = "content$Automation/" + scriptFilePath;
			}
			AppShell.Instance.WebService.StartRequest(uri, OnWebScriptLoaded, ShsWebService.ShsWebServiceType.Text);
		}
	}

	public void OnWebScriptLoaded(ShsWebResponse response)
	{
		string[] cscript = response.Body.Split('\n');
		if (GenerateCommandList(cscript))
		{
			try
			{
				StartCoroutine(RunScripting());
			}
			catch (AutomationTimeOutException ex)
			{
				CspUtils.DebugLog(ex.ToString());
			}
			catch (AutomationExecuteException ex2)
			{
				CspUtils.DebugLog(ex2.ToString());
			}
		}
	}

	private void OnWelcomeResponse(WelcomeResponseMessage msg)
	{
	}

	private void OnChatMsgReceived(GameWorldOpenChatMessage message)
	{
		chatMessageReceived = message.sendingPlayerId + ":" + message.chatMessage;
	}

	protected bool StartAutomation()
	{
		if (SetupLogging())
		{
			string[] cscript = LoadCommandScriptFile(scriptFilePath);
			if (GenerateCommandList(cscript))
			{
				try
				{
					StartCoroutine(RunScripting());
				}
				catch (AutomationTimeOutException ex)
				{
					CspUtils.DebugLog(ex.ToString());
				}
				catch (AutomationExecuteException ex2)
				{
					CspUtils.DebugLog(ex2.ToString());
				}
			}
		}
		return true;
	}

	protected IEnumerator RunScripting()
	{
		yield return 0;
		numberOfSteps = 1;
		numberOfFailures = 0;
		Type waitCmdType = Type.GetType("WaitCmd");
		Type commentCmdType = Type.GetType("CommentLogCmd");
		List<string> str = new List<string>();
		foreach (AutomationCmd s in automationCmdsQ)
		{
			str.Add(s.GetType().ToString());
		}
		while (automationCmdsQ.Count > 0)
		{
			AutomationCmd cmd = automationCmdsQ.Dequeue();
			Type cmdType = cmd.GetType();
			CspUtils.DebugLog(cmd.ToString());
			xmlLogWriter.WriteStartElement("step");
			LogElementString("step_number", numberOfSteps++.ToString());
			if (cmdType == commentCmdType)
			{
				cmd.execute();
			}
			else if (cmd.ErrorCode == "OK")
			{
				LogElementString("timestamp", DateTime.Now.TimeOfDay.ToString());
				LogElementString("command", cmd.ToString());
				if (cmd.precheckOk())
				{
					bool onetime2 = true;
					while (!cmd.isReady())
					{
						if (cmd.ErrorCode != "OK")
						{
							CspUtils.DebugLog("isReady : error occurred , break out of the loop");
							break;
						}
						if (onetime2)
						{
							CspUtils.DebugLog("Waiting for Ready condition for :" + cmd.ToString());
							onetime2 = false;
						}
						yield return 0;
					}
					LogStartElement(cmd.GetType().ToString());
					if (cmd.ErrorCode == "OK")
					{
						cmd.execute();
						onetime2 = true;
						while (!cmd.isCompleted())
						{
							if (cmdType == waitCmdType)
							{
								WaitCmd w = (WaitCmd)cmd;
								CspUtils.DebugLog("Calling wait cmd for secs:" + w.delay);
								yield return new WaitForSeconds(w.delay);
								continue;
							}
							if (onetime2)
							{
								CspUtils.DebugLog("Waiting for completion of :" + cmd.ToString());
								onetime2 = false;
							}
							if (cmd.ErrorCode != "OK")
							{
								CspUtils.DebugLog("isCompleted : error occurred , break out of the loop");
								break;
							}
							yield return 0;
						}
						CspUtils.DebugLog("Completed :" + cmd.ToString());
					}
				}
				cmd.results();
				LogEndElement();
			}
			LogEndElement();
			if (cmd.ErrorCode != "OK")
			{
				testResult = "TEST FAILED";
				if (cmd.isCriticalCmd)
				{
					InsertCriticalCmdFailureMsg(cmd);
					break;
				}
			}
		}
		CloseLogging();
		CspUtils.DebugLog("---- Done with RunScripting ---- ");
		yield return 0;
	}

	private void InsertCriticalCmdFailureMsg(AutomationCmd cmd)
	{
		testResult = "Test Failed : Incomplete due to critical command failure.";
		xmlLogWriter.WriteElementString("CRITICAL FAILURE", "Automation aborted due to failure of critical command: " + cmd);
	}

	public void OnNewControllerReadyMessage(NewControllerReadyMessage message)
	{
		activeController = GameController.GetController().controllerType;
	}

	private void OnLoadingProgressMessage(LoadingProgressMessage Msg)
	{
		loadingState = Msg.State;
		if (loadingState == LoadingProgressMessage.LoadingState.Started)
		{
			LocalPlayer = null;
		}
	}

	protected void OnPlayerCharacterSelected(CharacterSelectedMessage message)
	{
		playerCharacterSelected = message.CharacterName;
	}

	public void OnEntitySpawned(EntitySpawnMessage msg)
	{
		if (msg.spawnType == CharacterSpawn.Type.LocalPlayer)
		{
			LocalPlayer = msg.go;
		}
		if (msg.spawnType == CharacterSpawn.Type.LocalAI)
		{
			spawnobjcount++;
			LocalEnemy = msg.go;
		}
		if (msg.spawnType == CharacterSpawn.Type.NPC)
		{
			npcCount++;
		}
		if (msg.spawnType == CharacterSpawn.Type.Local)
		{
			userCount++;
		}
	}

	protected void OnEntityDespawned(EntityDespawnMessage msg)
	{
		if (spawnobjcount > 1)
		{
			spawnobjcount--;
		}
		if (msg.type == CharacterSpawn.Type.NPC)
		{
			npcCount--;
		}
		if (msg.type == CharacterSpawn.Type.Remote)
		{
			userCount--;
		}
	}

	private void OnLoginCompleted(LoginCompleteMessage message)
	{
		if (message.status == LoginCompleteMessage.LoginStatus.LoginSucceeded)
		{
			LoginStatus = LoginState.Succeeded;
			GUI_LoginComplete = true;
		}
		else
		{
			LoginStatus = LoginState.Failed;
			GUI_LoginComplete = false;
		}
		AppShell.Instance.EventMgr.RemoveListener<LoginCompleteMessage>(OnLoginCompleted);
	}

	public void OnZoneLoaded(ZoneLoadedMessage msg)
	{
		zoneLoaded = true;
	}

	private void OnRoomChanged(HQRoomChangedMessage message)
	{
		HQRoomId = message.roomId;
	}

	public string[] LoadCommandScriptFile(string scriptpath)
	{
		string[] result = null;
		try
		{
			result = File.ReadAllLines(scriptpath);
			return result;
		}
		catch (Exception ex)
		{
			CspUtils.DebugLog("The file could not be read: " + ex.Message);
			return result;
		}
	}

	public bool GenerateCommandList(string[] cscript)
	{
		automationCmdsQ = new Queue<AutomationCmd>();
		foreach (string text in cscript)
		{
			string text2 = text.Trim();
			if (text2.Length != 0 && text2[0] != '#')
			{
				AutomationCmd automationCmd = null;
				automationCmd = CmdFactory(text2);
				if (automationCmd != null)
				{
					CspUtils.DebugLog("adding " + text2);
					automationCmdsQ.Enqueue(automationCmd);
				}
				else
				{
					CspUtils.DebugLog("Error with " + text2);
				}
			}
		}
		return true;
	}

	public AutomationCmd CmdFactory(string cmdline)
	{
		AutomationCmd automationCmd = null;
		bool isCriticalCmd = false;
		cmdline = cmdline.Trim();
		if (cmdline[0] != '#' || cmdline.Length > 0)
		{
			if (cmdline.StartsWith("crit ", StringComparison.OrdinalIgnoreCase))
			{
				isCriticalCmd = true;
				cmdline = cmdline.Substring(5);
				CspUtils.DebugLog("cmdCritical for " + cmdline);
			}
			string[] array = cmdline.Split(' ');
			switch (array[0].ToLower())
			{
			case "login":
				automationCmd = ((!(AppShell.Instance.username == string.Empty) && !(AppShell.Instance.password == string.Empty)) ? new LoginCmd(cmdline, AppShell.Instance.username, AppShell.Instance.password) : new LoginCmd(cmdline, array[1], array[2]));
				break;
			case "logout":
				automationCmd = new LogoutCmd(cmdline);
				break;
			case "flush":
				automationCmd = new FlushCmd(cmdline);
				break;
			case "wait":
				automationCmd = new WaitCmd(cmdline, Convert.ToInt16(array[1]));
				break;
			case "socialspacelaunch":
				automationCmd = new SocialSpaceLaunchCmd(cmdline, array[1]);
				break;
			case "emote":
				automationCmd = new EmoteCmd(cmdline, array[1].ToLower());
				break;
			case "socialspacechar":
				automationCmd = new SocialSpaceCharacterSelectCmd(cmdline, array[1]);
				break;
			case "brawlerlaunch":
				automationCmd = new BrawlerLaunchCmd(cmdline, array[1], array[2], array[3]);
				break;
			case "acceptbrawlerinvite":
				automationCmd = new BrawlerAcceptInviteCmd(cmdline);
				break;
			case "brawlerchar":
				automationCmd = new BrawlerCharacterSelectCmd(cmdline, array[1]);
				break;
			case "brawlerstage_next":
				automationCmd = new BrawlerStageNext(cmdline);
				break;
			case "cardgame":
				automationCmd = ((array.Length != 6) ? ((array.Length != 4) ? new CardGameTransitionCmd(cmdline) : new CardGameTransitionCmd(cmdline, array[1], array[2], array[3])) : new CardGameTransitionCmd(cmdline, array[1], array[2], array[3], array[4], array[5]));
				break;
			case "playcards":
				automationCmd = ((array.Length != 2) ? new PlayCardsCmd(cmdline) : new PlayCardsCmd(cmdline, array[1]));
				break;
			case "hqlaunch":
				automationCmd = new HQLaunchCmd(cmdline);
				break;
			case "hqchangeroom":
				automationCmd = new ChangeRoomCmd(cmdline, array[1]);
				break;
			case "togglephysics":
				automationCmd = new TogglePhysicsCmd(cmdline);
				break;
			case "addtokens":
				automationCmd = new AddTokensCmd(cmdline, Convert.ToInt32(array[1]));
				break;
			case "addtickets":
				automationCmd = new AddTicketsCmd(cmdline, Convert.ToInt32(array[1]));
				break;
			case "throwException":
				automationCmd = new ThrowExceptnCmd(cmdline, array[1]);
				break;
			case "move":
				automationCmd = new MoveCmd(cmdline, array[1]);
				break;
			case "teleport":
				automationCmd = new TeleportCmd(cmdline, array[1]);
				break;
			case "fight":
				automationCmd = new FightCmd(cmdline, true);
				break;
			case "stopfight":
				automationCmd = new FightCmd(cmdline, false);
				break;
			case "killall":
				automationCmd = new KillAll(cmdline);
				break;
			case "setenemycount":
				automationCmd = new SetBrawlerEnemiesCount(cmdline, array[1]);
				break;
			case "import":
				automationCmd = new ImportCmd(cmdline, array[1]);
				break;
			case "loop":
				automationCmd = new LoopCmd(cmdline, Convert.ToInt32(array[1]));
				break;
			case "endloop":
				automationCmd = new EndLoopCmd(cmdline);
				break;
			case "snap":
				automationCmd = new SnapCmd(cmdline, array[1]);
				break;
			case "jump":
				automationCmd = new JumpCmd(cmdline);
				break;
			case "comment":
				automationCmd = new CommentCmd(cmdline);
				break;
			case "verifyfps":
				automationCmd = ((array.Length <= 1) ? new CountFrames(cmdline) : new CountFrames(cmdline, Convert.ToInt32(array[1])));
				break;
			case "gotozone":
				automationCmd = new GoToZone(cmdline, array[1]);
				break;
			case "gofullscreen":
				automationCmd = new GoFullScreen(cmdline);
				break;
			case "verifyplayernpcratio":
				automationCmd = new VerifyPlayerNPCRatio(cmdline);
				break;
			case "verifyplayername":
				automationCmd = new VerifyPlayerName(cmdline, array[1]);
				break;
			case "logcardgameresults":
				automationCmd = new LogCardGameResults(cmdline);
				break;
			case "loggamearearesults":
				automationCmd = new LogGameAreaResults(cmdline);
				break;
			case "verifybrawlerusers":
				automationCmd = new VerifyBrawlerMultiplayerUsers(cmdline);
				break;
			case "verifybrawlerenemies":
				automationCmd = new VerifyBrawlerSpawnEnemies(cmdline);
				break;
			case "verifyopenchat":
				automationCmd = new VerifyOpenChat(cmdline);
				break;
			case "triggerfly":
				automationCmd = new TriggerFly(cmdline);
				break;
			case "throw":
				automationCmd = new DoPickUp(cmdline);
				break;
			case "gototarget":
				automationCmd = new GoToTarget(cmdline);
				break;
			case "closeui":
				automationCmd = ((array.Length != 3) ? ((array.Length != 2) ? new CloseUICmd(cmdline) : new CloseUICmd(cmdline, array[1])) : new CloseUICmd(cmdline, array[1], Convert.ToBoolean(array[2])));
				break;
			}
			automationCmd.isCriticalCmd = isCriticalCmd;
		}
		return automationCmd;
	}

	private void OnGameControllerExited(GameControllerExitedMessage msg)
	{
		loadingState = LoadingProgressMessage.LoadingState.InProgress;
	}

	private void OnCombatCharacterHit(CombatCharacterHitMessage msg)
	{
		if (inFightMode)
		{
			if (!LocalEnemy && (bool)msg.SourceCharacterCombat.gameObject)
			{
				LocalEnemy = msg.SourceCharacterCombat.gameObject;
			}
			moveInProgress = !AutomationBrawler.instance.fight();
		}
	}

	private void OnCharacterAggroEvent(CombatCharacterAggroMessage msg)
	{
		if (inFightMode)
		{
			if ((bool)msg.CharacterCombat.gameObject && !LocalEnemy)
			{
				LocalEnemy = msg.CharacterCombat.gameObject;
			}
			moveInProgress = !AutomationBrawler.instance.fight();
		}
	}

	public bool SetupLogging()
	{
		fstrm = null;
		memstrm = null;
		xmlLogWriter = null;
		try
		{
			memstrm = new MemoryStream();
			xmlLogWriter = new XmlTextWriter(memstrm, null);
			xmlLogWriter.Formatting = Formatting.Indented;
		}
		catch (IOException ex)
		{
			CspUtils.DebugLog(ex.Message);
		}
		if (!isWebPlayer)
		{
			try
			{
				fstrm = new FileStream(logFilePath, FileMode.Create);
			}
			catch (IOException ex2)
			{
				CspUtils.DebugLog(ex2.Message);
			}
		}
		return true;
	}

	public void OnLogSaved(ShsWebResponse response)
	{
		if (response.Body == "OK" || response.Body == string.Empty)
		{
			CspUtils.DebugLog("OnLogSaved : Log Saved Successfuly");
		}
		else
		{
			CspUtils.DebugLog("OnLogSaved : Logging Failed - Body: " + response.Body);
		}
	}

	public bool CloseLogging()
	{
		CspUtils.DebugLog("CloseLogging");
		if (xmlLogWriter != null)
		{
			MemoryStream memoryStream = new MemoryStream();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, null);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.WriteStartDocument();
			xmlTextWriter.WriteStartElement("automation_results");
			xmlTextWriter.WriteStartElement("script");
			xmlTextWriter.WriteElementString("name", testCaseName);
			xmlTextWriter.WriteElementString("date", DateTime.Now.ToString());
			xmlTextWriter.WriteElementString("NetworkEnvironment", AppShell.Instance.NetworkEnvironment);
			xmlTextWriter.WriteElementString("comment", commentString);
			xmlTextWriter.WriteElementString("result", testResult);
			xmlTextWriter.WriteElementString("numberOfSteps", numberOfSteps.ToString());
			xmlTextWriter.WriteElementString("numberOfFailures", numberOfFailures.ToString());
			xmlTextWriter.WriteStartElement("steps");
			xmlLogWriter.Flush();
			memstrm.Position = 0L;
			StreamReader streamReader = new StreamReader(memstrm);
			xmlTextWriter.WriteRaw(streamReader.ReadToEnd());
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.WriteEndDocument();
			xmlTextWriter.Flush();
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("req", "log");
			wWWForm.AddField("mn", commentString);
			wWWForm.AddField("bid", testCaseName);
			DateTime now = DateTime.Now;
			string value = string.Format("{0:D}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
			wWWForm.AddField("td", value);
			memoryStream.Position = 0L;
			StreamReader streamReader2 = new StreamReader(memoryStream);
			wWWForm.AddField("res", streamReader2.ReadToEnd());
			AppShell.Instance.WebService.StartRequest("http://taswk7-1709/qa/shs/logging.php", OnLogSaved, wWWForm.data);
			if (fstrm != null)
			{
				try
				{
					memoryStream.WriteTo(fstrm);
					fstrm.Flush();
					CspUtils.DebugLog("fstrm.Length :" + fstrm.Length.ToString());
					fstrm.Close();
				}
				catch (IOException ex)
				{
					CspUtils.DebugLog(ex.Message);
				}
			}
			xmlLogWriter.Close();
			xmlTextWriter.Close();
			CspUtils.DebugLog("logging file closed");
			return true;
		}
		return false;
	}

	public void FlushLogging()
	{
		xmlLogWriter.Flush();
	}

	public void LogComment(string comment)
	{
		xmlLogWriter.WriteComment(comment);
	}

	public void LogElement(string name, string data)
	{
		xmlLogWriter.WriteStartElement(name);
		xmlLogWriter.WriteString(data);
		xmlLogWriter.WriteEndElement();
	}

	public void LogStartElement(string name)
	{
		xmlLogWriter.WriteStartElement(name);
	}

	public void LogElementString(string name, string value)
	{
		try
		{
			xmlLogWriter.WriteElementString(name, value);
		}
		catch
		{
			CspUtils.DebugLog("Failed to write - Name: " + name + "Value: " + value);
		}
	}

	public void LogString(string data)
	{
		xmlLogWriter.WriteString(data);
	}

	public void LogEndElement()
	{
		try
		{
			xmlLogWriter.WriteEndElement();
		}
		catch
		{
			CspUtils.DebugLog("No Elements to Close");
		}
		xmlLogWriter.Flush();
	}

	public void LogAttribute(string localName, string value)
	{
		try
		{
			xmlLogWriter.WriteAttributeString(localName, value);
		}
		catch
		{
			CspUtils.DebugLog("Failed to Log Attribute");
		}
	}

	public string ReadConfigFile(string path)
	{
		XmlDocument xmlDocument = new XmlDocument();
		StringWriter stringWriter = new StringWriter();
		try
		{
			xmlDocument.Load(path);
			XmlTextWriter w = new XmlTextWriter(stringWriter);
			xmlDocument.WriteTo(w);
		}
		catch (XmlException ex)
		{
			CspUtils.DebugLog("problem with XML  :" + ex.ToString());
		}
		catch (IOException ex2)
		{
			CspUtils.DebugLog("problem with config file :" + ex2.ToString());
		}
		return stringWriter.ToString();
	}

	public bool EvaluateConfigFile(string xmlDataString)
	{
		if (xmlDataString.Length != 0)
		{
			DataWarehouse dataWarehouse = new DataWarehouse(xmlDataString);
			dataWarehouse.Parse();
			automateClient = dataWarehouse.TryGetBool("//activate", false);
			testCaseName = dataWarehouse.TryGetString("//testname", string.Empty);
			commentString = dataWarehouse.TryGetString("//comment", string.Empty);
			logFilePath = dataWarehouse.TryGetString("//logpath", string.Empty);
			scriptFilePath = dataWarehouse.TryGetString("//scriptpath ", string.Empty);
			subScriptFilePath = dataWarehouse.TryGetString("//subscriptpath ", string.Empty);
			return true;
		}
		return false;
	}

	public float GetDistance(float x1, float y1, float z1, float x2, float y2, float z2)
	{
		float num = 0f;
		float num2 = (float)Math.Pow(x2 - x1, 2.0);
		float num3 = (float)Math.Pow(y2 - y1, 2.0);
		float num4 = (float)Math.Pow(z2 - z1, 2.0);
		float num5 = num2 + num3 + num4;
		return (float)Math.Sqrt(num5);
	}

	public GameObject Throwable()
	{
		float num = 1f;
		GameObject result = null;
		AutomationBrawler.instance.pickup(null);
		if (pickUpList.Count > 0)
		{
			foreach (GameObject pickUp in pickUpList)
			{
				Vector3 position = pickUp.transform.position;
				Vector3 position2 = LocalPlayer.transform.position;
				float distance = GetDistance(position2.x, position2.y, position2.z, position.x, position.y, position.z);
				if (distance <= num)
				{
					result = pickUp;
				}
			}
			return result;
		}
		return result;
	}

	public int GetEnemiesList()
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(DynamicShadowTarget));
		return array.Length;
	}

	public int GetNPCCount()
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(AIControllerNPC));
		return array.Length;
	}

	public int GetPCount()
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(AIControllerPigeon));
		return array.Length;
	}

	public void LogCardInfoToFile(string info)
	{
		string text = "C:/SHSCardGame/Log/";
		string path = text + AppShell.Instance.Profile.PlayerName + ".csv";
		if (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
		{
			CspUtils.DebugLog("You can't log to file in Web Environment");
			return;
		}
		if (!isRunning && File.Exists(path))
		{
			isRunning = true;
			File.Delete(path);
		}
		if (!File.Exists(path))
		{
			Directory.CreateDirectory(text);
		}
		StreamWriter streamWriter = new StreamWriter(path, true);
		streamWriter.WriteLine(info);
		streamWriter.Flush();
		streamWriter.Close();
	}

	public void LogStatistics(int gw, int hq, int br, int cd, int ot)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("req", "log");
		wWWForm.AddField("gw", Convert.ToString(gw));
		wWWForm.AddField("hq", Convert.ToString(hq));
		wWWForm.AddField("br", Convert.ToString(br));
		wWWForm.AddField("cd", Convert.ToString(cd));
		wWWForm.AddField("ot", Convert.ToString(ot));
		AppShell.Instance.WebService.StartRequest("http://taswk7-1709/qa/shs/logbvtstats.php", OnLogSaved, wWWForm.data);
	}

	public void SetPrizeWheel()
	{
		string uri = "http://172.21.2.230:8091/topic.SHS";
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("message_type", "wheel_test");
		wWWForm.AddField("main_wheel_stop", 1);
		wWWForm.AddField("sub_wheel_stop", 1);
		wWWForm.AddField("card_wheel_stop", 1);
		AppShell.Instance.WebService.StartRequest(uri, OnLogSaved, wWWForm.data);
	}
}
