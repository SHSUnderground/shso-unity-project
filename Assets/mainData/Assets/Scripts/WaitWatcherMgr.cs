using ShsAudio;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class WaitWatcherMgr : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public class WaitWatcher
	{
		public TransactionMonitor transactionMonitor;

		public SHSWaitWindow waitWindow;

		public GUILabel helperTextLabel;

		public GUILabel helperTextLabelShadow;

		public GUILabel titleTextLabel;

		public GUILabel tipTextLabel;

		public GUILabel whoseTextLabel;

		public GUIDrawTexture background;

		public GUIProgressBar progressBar;

		public GUIImage tipBoxIcon;

		public GUIImage additionalLoadingImage;

		public SABundle musicBundle;

		public string musicResourceName;

		public TransitionHandler.OnTransitionComplete onTransitionComplete;

		private bool completeInvoked;

		public TransitionHandler.TransitionCompleteStatus completeStatus;

		public bool isModal;

		public bool isFullScreen;

		public bool isVerbose;

		public bool isComplete;

		public string interfaceId;

		public string helperText;

		public string titleText;

		public float transitionInTime;

		public float totalTransitionInTime;

		public float transitionOutTime;

		public float totalTransitionOutTime;

		public float minimumWaitTime;

		public float percentComplete;

		public int stepsComplete;

		public int totalSteps;

		public bool windowInSync;

		private GUILoadingScreenContext loadingContext = new GUILoadingScreenContext();

		public TransactionMonitor.ExitCondition exitCondition;

		private bool transitionOutFired;

		private bool transitionInFired;

		public WaitWatcher(WaitWatcherEvent.SpawnWaitWatcher e)
		{
			transactionMonitor = e.transactionMonitor;
			isFullScreen = e.isFullscreen;
			isModal = e.isModal;
			waitWindow = e.interfaceWindow;
			interfaceId = e.interfaceId;
			helperText = e.helperText;
			titleText = e.titleText;
			musicBundle = e.musicBundle;
			musicResourceName = e.musicResourceName;
			transitionInTime = (totalTransitionInTime = e.transitionInTime);
			transitionOutTime = (totalTransitionOutTime = e.transitionOutTime);
			minimumWaitTime = e.minimumWaitTime;
			isVerbose = e.isVerbose;
			if (!e.UseTransitionContext)
			{
				loadingContext.SetLoadingContext(e.LoadingContext);
			}
			else
			{
				loadingContext.SetLoadingContext(AppShell.Instance.PreviousControllerType, AppShell.Instance.CurrentControllerType);
			}
			if (!e.UseTransitionLocInfo)
			{
				CspUtils.DebugLog("************************************** e.LocInfo=" + e.LocInfo);
				//Application.Quit();
				loadingContext.SetLocationInfo(e.LocInfo);
			}
			else
			{
				CspUtils.DebugLog("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&& AppShell.Instance.CurrentControllerType=" + AppShell.Instance.CurrentControllerType);
				//Application.Quit();
				loadingContext.SetLocationInfo(AppShell.Instance.PreviousControllerType, AppShell.Instance.CurrentControllerType);
			}
		}

		public WaitWatcher(TransitionHandler.WWTransitionProperties properties)
		{
			isFullScreen = properties.isFullScreen;
			isModal = properties.isModal;
			musicResourceName = properties.musicResourceName;
			isVerbose = properties.isVerbose;
		}

		public void InvokeCompleteCallback()
		{
			if (!completeInvoked && onTransitionComplete != null)
			{
				completeInvoked = true;
				onTransitionComplete();
			}
		}

		public void Start()
		{
			if (waitWindow == null)
			{
				waitWindow = GetWaitInterface(interfaceId);
			}
			if (waitWindow != null)
			{
				GUIManager.Instance.UIRoots[GUIManager.UILayer.System].Add(waitWindow);
				waitWindow.Show(GUIControl.ModalLevelEnum.Default);
				FormatWaitWatcherScreen();
				if (isVerbose)
				{
					WaitWatcherEvent.WaitNotification waitNotification = new WaitWatcherEvent.WaitNotification(transactionMonitor);
					waitNotification.notificationType = WaitWatcherEvent.NotificationType.TransitionInStarted;
					AppShell.Instance.EventMgr.Fire(null, waitNotification);
				}
				PlayLoadingMusic();
			}
			else
			{
				CspUtils.DebugLog("WaitWatcher: Could not find wait window '" + interfaceId + "'.");
			}
		}

		private void FormatWaitWatcherScreen()
		{
			titleTextLabel = waitWindow.GetControl<GUILabel>("titleText");
			helperTextLabel = waitWindow.GetControl<GUILabel>("helperText");
			tipTextLabel = waitWindow.GetControl<GUILabel>("tipText");
			helperTextLabelShadow = waitWindow.GetControl<GUILabel>("helperTextShadow");
			background = waitWindow.GetControl<GUIDrawTexture>("loadingBackground");
			whoseTextLabel = waitWindow.GetControl<GUILabel>("whoseTipText");
			tipBoxIcon = waitWindow.GetControl<GUIImage>("tipBoxIcon");
			additionalLoadingImage = waitWindow.GetControl<GUIImage>("additionalLoadingImage");
			progressBar = waitWindow.GetControl<GUIProgressBar>("percentBar");
			if (titleTextLabel != null && !string.IsNullOrEmpty(titleText))
			{
				titleTextLabel.Text = loadingContext.LocationName;
			}
			if (helperTextLabel != null && !string.IsNullOrEmpty(helperText))
			{
				helperTextLabel.Text = helperText;
				if (helperTextLabelShadow != null)
				{
					helperTextLabelShadow.Text = helperText;
				}
			}
			GUIStyleManager styleManager = GUIManager.Instance.StyleManager;
			GUIContent gUIContent = null;
			Vector2 textSize = Vector2.zero;
			bool flag = styleManager != null;
			if (tipTextLabel != null)
			{
				tipTextLabel.Text = loadingContext.TipText;
			}
			if (whoseTextLabel != null)
			{
				bool flag2 = flag && loadingContext.WhoseTipText != string.Empty;
				whoseTextLabel.Text = ((!flag2) ? string.Empty : loadingContext.WhoseTipText);
			}
			if (flag && tipTextLabel != null)
			{
				gUIContent = new GUIContent(tipTextLabel.Text);
				textSize = tipTextLabel.Style.UnityStyle.CalcSize(gUIContent);
			}
			bool flag3 = loadingContext.TipText != string.Empty && loadingContext.TipText != null;
			if (flag && flag3)
			{
				waitWindow.BuildTipBox(textSize, loadingContext.TipTextureSource);
			}
			if (background != null)
			{
				background.TextureSource = loadingContext.BackgroundTextureSource;
			}
			if (additionalLoadingImage != null)
			{
				additionalLoadingImage.TextureSource = loadingContext.AdditionalTextureSource;
				additionalLoadingImage.Offset = loadingContext.AdditionalTextureOffset;
				additionalLoadingImage.Size = loadingContext.AdditionalTextureSize;
			}
			if (loadingContext.CustomSetup != null)
			{
				loadingContext.CustomSetup(waitWindow);
			}
		}

		private SHSWaitWindow GetWaitInterface(string interfaceName)
		{
			GUIControl control = GUIManager.Instance.UIRoots[GUIManager.UILayer.System].GetControl<GUIControl>(interfaceName);
			if (control != null && control is SHSWaitWindow)
			{
				return (SHSWaitWindow)control;
			}
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Type type;
			if (!string.IsNullOrEmpty(interfaceName))
			{
				type = executingAssembly.GetType(interfaceName);
			}
			else
			{
				type = executingAssembly.GetType("SHSWaitWindow");
				if (type == null)
				{
					CspUtils.DebugLog("!!! Can't get window type: " + type + " from executing assembly.");
					return null;
				}
			}
			SHSWaitWindow sHSWaitWindow = (SHSWaitWindow)Activator.CreateInstance(type);
			sHSWaitWindow.AnimationOnOpen = SHSAnimations.WindowOpenCloseDelegates.FadeIn(0f, sHSWaitWindow);
			sHSWaitWindow.AnimationOnClose = SHSAnimations.WindowOpenCloseDelegates.FadeOut(0.5f, sHSWaitWindow);
			if (sHSWaitWindow == null)
			{
				CspUtils.DebugLog(string.Format("!!! Can't instantiate instance of type {0}.", type));
				return null;
			}
			return sHSWaitWindow;
		}

		public void Update()
		{
			if (transitionInTime > 0f)
			{
				transitionInTime -= Time.deltaTime;
				if (transitionInTime > 0f)
				{
					CspUtils.DebugLog("transition time reached for the wait watcher");
					return;
				}
			}
			if (isVerbose && !transitionInFired)
			{
				WaitWatcherEvent.WaitNotification waitNotification = new WaitWatcherEvent.WaitNotification(transactionMonitor);
				waitNotification.notificationType = WaitWatcherEvent.NotificationType.TransitionInFinished;
				AppShell.Instance.EventMgr.Fire(null, waitNotification);
				transitionInFired = true;
			}
			if (minimumWaitTime > 0f)
			{
				minimumWaitTime -= Time.deltaTime;
				if (minimumWaitTime > 0f)
				{
					return;
				}
			}
			transactionMonitor.UpdateMonitoredTransaction();
			float progressValue = 100f * transactionMonitor.PercentComplete;
			waitWindow.ProgressValue = progressValue;
			windowInSync = waitWindow.PercentInSync;
			if (!transactionMonitor.IsCompleted)
			{
				return;
			}
			if (isVerbose && !transitionOutFired)
			{
				WaitWatcherEvent.WaitNotification waitNotification2 = new WaitWatcherEvent.WaitNotification(transactionMonitor);
				waitNotification2.notificationType = WaitWatcherEvent.NotificationType.TransitionOutStarted;
				AppShell.Instance.EventMgr.Fire(null, waitNotification2);
				transitionOutFired = true;
			}
			if (transitionOutTime > 0f)
			{
				transitionOutTime -= Time.deltaTime;
				if (transitionOutTime > 0f)
				{
					return;
				}
			}
			if (isVerbose)
			{
				WaitWatcherEvent.WaitNotification waitNotification3 = new WaitWatcherEvent.WaitNotification(transactionMonitor);
				waitNotification3.notificationType = WaitWatcherEvent.NotificationType.TransitionOutFinished;
				AppShell.Instance.EventMgr.Fire(null, waitNotification3);
			}
			WaitWatcherEvent.WaitNotification waitNotification4 = new WaitWatcherEvent.WaitNotification(transactionMonitor);
			waitNotification4.notificationType = ((exitCondition != 0) ? WaitWatcherEvent.NotificationType.TransactionFailed : WaitWatcherEvent.NotificationType.TransactionComplete);
			AppShell.Instance.EventMgr.Fire(null, waitNotification4);
			isComplete = true;
		}

		public void PlayLoadingMusic()
		{
			if (!GameController.GetController().isTestScene && AppShell.Instance.AudioManager.PlayedStartupMusic)
			{
				if (musicBundle != 0 && musicResourceName != null)
				{
					Helpers.LoadPrefabFromBundle(musicBundle, musicResourceName, OnLoadingMusicLoaded, null);
				}
				else if (AppShell.Instance.loadingMusic != null)
				{
					OnLoadingMusicLoaded(AppShell.Instance.loadingMusic.gameObject, null);
				}
			}
		}

		private void OnLoadingMusicLoaded(GameObject audioPrefab, object extraData)
		{
			if (!isComplete)
			{
				ShsAudioSource.PlayAutoSound(audioPrefab);
			}
		}
	}

	private List<WaitWatcher> waitList;

	private List<WaitWatcher> toRemove;

	private bool eventsRegistered;

	public WaitWatcherMgr()
	{
		waitList = new List<WaitWatcher>();
		toRemove = new List<WaitWatcher>();
	}

	private void Awake()
	{
	}

	private void Start()
	{
		if (!eventsRegistered)
		{
			AppShell.Instance.EventMgr.AddListener<WaitWatcherEvent.SpawnWaitWatcher>(OnSpawnWaitWatcher);
			AppShell.Instance.EventMgr.AddListener<WaitWatcherEvent.DumpWaitWatcher>(OnDumpWaitWatcher);
			AppShell.Instance.EventMgr.AddListener<WaitWatcherEvent.KillWaitWatcher>(OnKillWaitWatcher);
			AppShell.Instance.EventMgr.AddListener<WaitWatcherEvent.StepComplete>(OnStepComplete);
			AppShell.Instance.EventMgr.AddListener<WaitWatcherEvent.TransactionComplete>(OnTransactionComplete);
			eventsRegistered = true;
		}
	}

	private void OnEnable()
	{
		if (!eventsRegistered)
		{
			AppShell.Instance.EventMgr.AddListener<WaitWatcherEvent.SpawnWaitWatcher>(OnSpawnWaitWatcher);
			AppShell.Instance.EventMgr.AddListener<WaitWatcherEvent.DumpWaitWatcher>(OnDumpWaitWatcher);
			AppShell.Instance.EventMgr.AddListener<WaitWatcherEvent.KillWaitWatcher>(OnKillWaitWatcher);
			AppShell.Instance.EventMgr.AddListener<WaitWatcherEvent.StepComplete>(OnStepComplete);
			AppShell.Instance.EventMgr.AddListener<WaitWatcherEvent.TransactionComplete>(OnTransactionComplete);
			eventsRegistered = true;
		}
	}

	private void OnDisable()
	{
	}

	public void Update()
	{
		foreach (WaitWatcher wait in waitList)
		{
			wait.Update();
			bool flag = wait.isComplete && wait.windowInSync;
			bool flag2 = flag && wait.completeStatus.Status == TransitionHandler.TransitionCompleteStatusEnum.Finished;
			if (flag)
			{
				wait.InvokeCompleteCallback();
			}
			if (flag2)
			{
				wait.transactionMonitor.CloseTransaction();
				wait.waitWindow.Hide();
				toRemove.Add(wait);
			}
		}
		if (toRemove.Count > 0)
		{
			foreach (WaitWatcher item in toRemove)
			{
				waitList.Remove(item);
			}
			toRemove.Clear();
		}
	}

	protected void UpdateVisibility()
	{
		WaitWatcher waitWatcher = null;
		foreach (WaitWatcher wait in waitList)
		{
			if (wait.isFullScreen)
			{
				waitWatcher = wait;
			}
		}
		if (waitWatcher != null)
		{
			foreach (WaitWatcher wait2 in waitList)
			{
				if (wait2.isFullScreen)
				{
					if (wait2 == waitWatcher)
					{
						if (!wait2.waitWindow.IsVisible)
						{
							wait2.waitWindow.Show(GUIControl.ModalLevelEnum.Default);
						}
					}
					else if (wait2.waitWindow.IsVisible)
					{
						wait2.waitWindow.Hide();
					}
				}
			}
		}
	}

	public void StartWaiting(TransitionHandler.WWTransitionProperties properties, SHSWaitWindow waitWindow, TransactionMonitor transactionToWatch)
	{
		WaitWatcher waitWatcher = new WaitWatcher(properties);
		waitWatcher.waitWindow = waitWindow;
		waitWatcher.transactionMonitor = transactionToWatch;
		waitWatcher.transactionMonitor.WatcherRef = waitWatcher;
		waitWatcher.onTransitionComplete = properties.onTransitionComplete;
		waitWatcher.completeStatus = properties.completeStatus;
		waitWatcher.musicBundle = properties.musicBundle;
		waitWatcher.musicResourceName = properties.musicResourceName;
		GUIManager.Instance.UIRoots[GUIManager.UILayer.System].Add(waitWatcher.waitWindow);
		waitWatcher.waitWindow.Show(GUIControl.ModalLevelEnum.Default);
		waitWatcher.PlayLoadingMusic();
		waitList.Add(waitWatcher);
		UpdateVisibility();
	}

	public void ModifyWaitWatcher(TransitionHandler.WWTransitionProperties properties, string transactionName)
	{
		WaitWatcher waitWatcher = null;
		foreach (WaitWatcher wait in waitList)
		{
			if (wait.transactionMonitor.Id == transactionName)
			{
				waitWatcher = wait;
				break;
			}
		}
		if (waitWatcher != null)
		{
			CspUtils.DebugLog("modifying wait watcher!");
			waitWatcher.onTransitionComplete = properties.onTransitionComplete;
			waitWatcher.completeStatus = properties.completeStatus;
			waitWatcher.musicBundle = properties.musicBundle;
			waitWatcher.musicResourceName = properties.musicResourceName;
		}
	}

	public void AbortWaiting(string transactionName)
	{
		foreach (WaitWatcher wait in waitList)
		{
			if (wait.transactionMonitor.Id == transactionName)
			{
				wait.transactionMonitor.CloseTransaction();
				wait.waitWindow.Hide();
				toRemove.Add(wait);
			}
		}
		if (toRemove.Count > 0)
		{
			foreach (WaitWatcher item in toRemove)
			{
				waitList.Remove(item);
			}
			toRemove.Clear();
		}
		toRemove.Clear();
	}

	public void OnSpawnWaitWatcher(WaitWatcherEvent.SpawnWaitWatcher e)
	{
		WaitWatcher waitWatcher = new WaitWatcher(e);
		waitList.Add(waitWatcher);
		waitWatcher.Start();
		UpdateVisibility();
	}

	public void OnDumpWaitWatcher(WaitWatcherEvent.DumpWaitWatcher e)
	{
		foreach (WaitWatcher wait in waitList)
		{
			wait.transactionMonitor.DumpStatus();
		}
	}

	public void OnKillWaitWatcher(WaitWatcherEvent.KillWaitWatcher e)
	{
		foreach (WaitWatcher wait in waitList)
		{
			if (wait.waitWindow == null)
			{
			}
		}
		waitList.Clear();
	}

	public void OnStepComplete(WaitWatcherEvent.StepComplete e)
	{
		WaitWatcher waitElement = GetWaitElement(e.transactionMonitor);
		if (waitElement == null)
		{
			return;
		}
		float value = 100f * waitElement.transactionMonitor.PercentComplete;
		if (waitElement.progressBar != null)
		{
			waitElement.progressBar.Value = value;
		}
		if (!string.IsNullOrEmpty(e.updateNotice) && waitElement.helperTextLabel != null)
		{
			waitElement.helperTextLabel.Text = e.updateNotice;
			if (waitElement.helperTextLabelShadow != null)
			{
				waitElement.helperTextLabelShadow.Text = e.updateNotice;
			}
		}
	}

	public void OnTransactionComplete(WaitWatcherEvent.TransactionComplete e)
	{
		WaitWatcher waitWatcher = null;
		foreach (WaitWatcher wait in waitList)
		{
			if (wait.transactionMonitor == e.transactionMonitor)
			{
				waitWatcher = wait;
				break;
			}
		}
		if (waitWatcher != null)
		{
			waitWatcher.exitCondition = e.exitCondition;
		}
	}

	private WaitWatcher GetWaitElement(TransactionMonitor tm)
	{
		while (tm != null)
		{
			foreach (WaitWatcher wait in waitList)
			{
				if (wait.transactionMonitor == tm)
				{
					return wait;
				}
			}
			tm = tm.parent;
		}
		return null;
	}
}
