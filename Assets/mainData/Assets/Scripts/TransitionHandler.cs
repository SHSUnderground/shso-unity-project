using ShsAudio;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class TransitionHandler
{
	public enum TransitionCompleteStatusEnum
	{
		Starting,
		Finished
	}

	public class TransitionCompleteStatus
	{
		private TransitionCompleteStatusEnum status;

		public TransitionCompleteStatusEnum Status
		{
			get
			{
				return status;
			}
			set
			{
				status = value;
			}
		}
	}

	public struct WWTransitionProperties
	{
		public GUILoadingScreenContext.LoadingContext loadingContext;

		public GUILoadingScreenContext.LocationInfo locationInfo;

		public TransactionLoadingContext.TransactionContext transactionContext;

		public TransitionCompleteStatus completeStatus;

		public bool isModal;

		public bool isFullScreen;

		public bool isVerbose;

		public string interfaceId;

		public string musicResourceName;

		public SABundle musicBundle;

		public OnTransitionComplete onTransitionComplete;

		public static WWTransitionProperties NoProperties
		{
			get
			{
				return new WWTransitionProperties(false, false, false, string.Empty, string.Empty);
			}
		}

		public WWTransitionProperties(bool isModal, bool isFullScreen, bool isVerbose, string interfaceId, string musicResourceName)
		{
			loadingContext = GUILoadingScreenContext.LoadingContext.EmptyContext;
			locationInfo = GUILoadingScreenContext.LocationInfo.NoInfo;
			transactionContext = TransactionLoadingContext.TransactionContext.EmptyContext;
			completeStatus = null;
			this.isModal = isModal;
			this.isFullScreen = isFullScreen;
			this.isVerbose = isVerbose;
			this.interfaceId = interfaceId;
			this.musicResourceName = musicResourceName;
			musicBundle = SABundle.None;
			onTransitionComplete = null;
		}
	}

	private struct ManualTransition
	{
		public WWTransitionProperties transitionProperties;

		public GUILoadingScreenContext guiLoadingScreenContext;

		public TransactionLoadingContext transactionLoadingContext;
	}

	public delegate void OnTransitionComplete();

	private const string defaultInterfaceId = "SHSWaitWindow";

	private const string defeaultMusicResourceName = "";

	private const bool defaultIsModal = false;

	private const bool defaultIsFullScreen = false;

	private const bool defaultIsVerbose = false;

	private const bool defaultIsComplete = false;

	private static Hashtable propertiesLookup = null;

	private static WWTransitionProperties currProperties = WWTransitionProperties.NoProperties;

	private TransactionLoadingContext transactionContext;

	private GUILoadingScreenContext loadingScreenContext;

	private SHSWaitWindow waitWindow;

	private WaitWatcherMgr waitWatcherMgr;

	private Hashtable manualTransitions = new Hashtable();

	private bool manualTransition;

	public WaitWatcherMgr WaitWatcherManager
	{
		set
		{
			waitWatcherMgr = value;
		}
	}

	public TransactionLoadingContext CurrentTransactionContext
	{
		get
		{
			return transactionContext;
		}
	}

	public GUILoadingScreenContext CurrentLoadingScreenContext
	{
		get
		{
			return loadingScreenContext;
		}
	}

	public SHSWaitWindow CurrentWaitWindow
	{
		get
		{
			return waitWindow;
		}
	}

	public TransitionHandler()
	{
		loadingScreenContext = new GUILoadingScreenContext();
		transactionContext = new TransactionLoadingContext();
	}

	private static void CreateLookup()
	{
		if (propertiesLookup == null)
		{
			propertiesLookup = new Hashtable();
			propertiesLookup[GameController.ControllerType.SocialSpace] = new Hashtable();
			propertiesLookup[GameController.ControllerType.Brawler] = new Hashtable();
			propertiesLookup[GameController.ControllerType.HeadQuarters] = new Hashtable();
			propertiesLookup[GameController.ControllerType.FrontEnd] = new Hashtable();
			propertiesLookup[GameController.ControllerType.CardGame] = new Hashtable();
			propertiesLookup[GameController.ControllerType.RailsGameWorld] = new Hashtable();
			WWTransitionProperties wWTransitionProperties = new WWTransitionProperties(false, false, false, "SHSWaitWindow", string.Empty);
			wWTransitionProperties.onTransitionComplete = WelcomeScreenRoutine;
			((Hashtable)propertiesLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.SocialSpace] = wWTransitionProperties;
			((Hashtable)propertiesLookup[GameController.ControllerType.RailsGameWorld])[GameController.ControllerType.SocialSpace] = wWTransitionProperties;
			wWTransitionProperties = new WWTransitionProperties(false, false, false, "SHSBrawlerWaitWindow", "Music_SHS_Mission_Loading_audio");
			wWTransitionProperties.musicBundle = SABundle.Brawler;
			((Hashtable)propertiesLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.Brawler] = wWTransitionProperties;
			((Hashtable)propertiesLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.Brawler] = wWTransitionProperties;
			((Hashtable)propertiesLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.Brawler] = wWTransitionProperties;
			((Hashtable)propertiesLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.Brawler] = wWTransitionProperties;
			((Hashtable)propertiesLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.Brawler] = wWTransitionProperties;
		}
	}

	public static void WelcomeScreenRoutine()
	{
		bool flag = false;
		string value;
		if (AppShell.Instance.WebService.UrlParameters.TryGetValue("auto", out value))
		{
			flag = value.Equals("true", StringComparison.OrdinalIgnoreCase);
		}
		if (!flag)
		{
			Hashtable sharedHashTable = AppShell.Instance.SharedHashTable;
			if (!sharedHashTable.ContainsKey("WelcomeScreenViewed") && NewTutorialManager.allowFullScreenWindow())
			{
				if (Screen.fullScreen)
				{
					currProperties.completeStatus.Status = TransitionCompleteStatusEnum.Finished;
					AppShell.Instance.SharedHashTable["WelcomeScreenViewed"] = true;
					AppShell.Instance.EventMgr.Fire(null, new WelcomeResponseMessage(false, null));
				}
				else
				{
					SHSWelcomeWindow sHSWelcomeWindow = new SHSWelcomeWindow();
					sHSWelcomeWindow.OnVisible += delegate
					{
						currProperties.completeStatus.Status = TransitionCompleteStatusEnum.Finished;
					};
					GUIManager.Instance.ShowDynamicWindow(sHSWelcomeWindow, GUIControl.ModalLevelEnum.Default);
				}
			}
			else
			{
				currProperties.completeStatus.Status = TransitionCompleteStatusEnum.Finished;
				AppShell.Instance.EventMgr.Fire(null, new WelcomeResponseMessage(false, null));
			}
		}
		else
		{
			currProperties.completeStatus.Status = TransitionCompleteStatusEnum.Finished;
			AppShell.Instance.EventMgr.Fire(null, new WelcomeResponseMessage(false, null));
		}
	}

	private static bool PropertiesDefinedForTransition(GameController.ControllerType from, GameController.ControllerType to)
	{
		CreateLookup();
		bool result = false;
		if (propertiesLookup.ContainsKey(from))
		{
			Hashtable hashtable = (Hashtable)propertiesLookup[from];
			if (hashtable.ContainsKey(to))
			{
				result = true;
			}
		}
		return result;
	}

	public void SetupTransition(GameController.ControllerType from, GameController.ControllerType to)
	{
		WWTransitionProperties noProperties = WWTransitionProperties.NoProperties;
		loadingScreenContext.SetLoadingContext(from, to);
		loadingScreenContext.SetLocationInfo(from, to);
		transactionContext.SetTransactionContext(from, to);
		bool flag = PropertiesDefinedForTransition(from, to);
		noProperties = (flag ? ((WWTransitionProperties)((Hashtable)propertiesLookup[from])[to]) : new WWTransitionProperties(false, false, false, "SHSWaitWindow", string.Empty));
		SetupTransitionStatus(ref noProperties);
		if (flag)
		{
			((Hashtable)propertiesLookup[from])[to] = noProperties;
		}
		currProperties = noProperties;
		CreateWaitWindow(noProperties, loadingScreenContext, transactionContext.Transaction);
	}

	private void SetupTransitionStatus(ref WWTransitionProperties propertiesForTransition)
	{
		if (propertiesForTransition.completeStatus == null)
		{
			propertiesForTransition.completeStatus = new TransitionCompleteStatus();
		}
		bool flag = propertiesForTransition.onTransitionComplete != null;
		propertiesForTransition.completeStatus.Status = ((!flag) ? TransitionCompleteStatusEnum.Finished : TransitionCompleteStatusEnum.Starting);
	}

	public void SetupTransition(WWTransitionProperties propertiesForTransition)
	{
		manualTransition = true;
		loadingScreenContext.SetLoadingContext(propertiesForTransition.loadingContext);
		loadingScreenContext.SetLocationInfo(propertiesForTransition.locationInfo);
		transactionContext.SetTransactionContext(propertiesForTransition.transactionContext);
		SetupTransitionStatus(ref propertiesForTransition);
		currProperties = propertiesForTransition;
		CreateWaitWindow(propertiesForTransition, loadingScreenContext, transactionContext.Transaction);
	}

	public void SetupTransitionContext(TransactionLoadingContext.TransactionContext transactionContext)
	{
		this.transactionContext.SetTransactionContext(transactionContext);
	}

	public void AddManualTransitionPoolObj(WWTransitionProperties propertiesForTransition, string manualTransactionKey)
	{
		if (!manualTransitions.ContainsKey(manualTransactionKey))
		{
			manualTransitions[manualTransactionKey] = default(ManualTransition);
			ManualTransition manualTransition = (ManualTransition)manualTransitions[manualTransactionKey];
			manualTransition.guiLoadingScreenContext = new GUILoadingScreenContext();
			manualTransition.guiLoadingScreenContext.SetLoadingContext(propertiesForTransition.loadingContext);
			manualTransition.transactionLoadingContext = new TransactionLoadingContext();
			manualTransition.transactionLoadingContext.SetTransactionContext(propertiesForTransition.transactionContext);
			manualTransition.transitionProperties = propertiesForTransition;
			manualTransitions[manualTransactionKey] = manualTransition;
		}
	}

	public TransactionLoadingContext GetManualTransactionContext(string manualTransactionKey)
	{
		if (manualTransitions.ContainsKey(manualTransactionKey))
		{
			ManualTransition manualTransition = (ManualTransition)manualTransitions[manualTransactionKey];
			return manualTransition.transactionLoadingContext;
		}
		return null;
	}

	public void MakeManualTransitionActive(string manualTransactionKey)
	{
		if (manualTransitions.ContainsKey(manualTransactionKey))
		{
			this.manualTransition = true;
			ManualTransition manualTransition = (ManualTransition)manualTransitions[manualTransactionKey];
			manualTransition.guiLoadingScreenContext.SetLoadingContext(manualTransition.transitionProperties.loadingContext);
			manualTransition.guiLoadingScreenContext.SetLocationInfo(manualTransition.transitionProperties.locationInfo);
			manualTransition.transactionLoadingContext.SetTransactionContext(manualTransition.transitionProperties.transactionContext);
			SetupTransitionStatus(ref manualTransition.transitionProperties);
			CreateWaitWindow(manualTransition.transitionProperties, manualTransition.guiLoadingScreenContext, manualTransition.transactionLoadingContext.Transaction);
		}
	}

	public void AbortTransition(string transactionName)
	{
		waitWatcherMgr.AbortWaiting(transactionName);
	}

	private void CreateWaitWindow(WWTransitionProperties propertiesForTransition, GUILoadingScreenContext guiLoadingScreenContext, TransactionMonitor transactionMonitor)
	{
		if (CurrentWaitWindow != null && !GameController.GetController().isTestScene)
		{
			CurrentWaitWindow.Hide();
		}
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		ConstructorInfo constructorInfo = null;
		Type type;
		if (!string.IsNullOrEmpty(propertiesForTransition.interfaceId))
		{
			type = executingAssembly.GetType(propertiesForTransition.interfaceId);
		}
		else
		{
			type = executingAssembly.GetType("SHSWaitWindow");
			if (type == null)
			{
				CspUtils.DebugLog("Can't find this given WaitWindow type: " + propertiesForTransition.interfaceId);
				return;
			}
		}
		constructorInfo = type.GetConstructor(new Type[0]);
		if (constructorInfo != null)
		{
			waitWindow = (SHSWaitWindow)constructorInfo.Invoke(new object[0]);
		}
		else
		{
			CspUtils.DebugLog("Invalid constructor information for WaitWindow type: " + type.ToString());
		}
		waitWindow.WindowContext = guiLoadingScreenContext;
		if (!manualTransition)
		{
			waitWindow.OnWindowBeginsWaiting = AppShell.Instance.CompleteTransition;
		}
		waitWatcherMgr.StartWaiting(propertiesForTransition, waitWindow, transactionMonitor);
		manualTransition = false;
	}

	public void ModifyTransitionProperties(string transactionName, WWTransitionProperties properties)
	{
		SetupTransitionStatus(ref properties);
		currProperties = properties;
		waitWatcherMgr.ModifyWaitWatcher(properties, transactionName);
	}
}
