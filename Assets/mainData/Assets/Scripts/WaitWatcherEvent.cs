using ShsAudio;

public class WaitWatcherEvent
{
	public class SpawnWaitWatcher : ShsEventMessage
	{
		public TransactionMonitor transactionMonitor;

		public GUILoadingScreenContext.LoadingContext loadingContext;

		public GUILoadingScreenContext.LocationInfo locInfo;

		public string interfaceId;

		public SHSWaitWindow interfaceWindow;

		public string titleText;

		public string helperText;

		public bool isModal;

		public bool isFullscreen;

		public SABundle musicBundle;

		public string musicResourceName;

		public bool isVerbose;

		public float transitionInTime;

		public float transitionOutTime;

		public float minimumWaitTime;

		private bool useTransitionContext;

		private bool useTransitionLocInfo;

		public bool UseTransitionContext
		{
			get
			{
				return useTransitionContext;
			}
		}

		public bool UseTransitionLocInfo
		{
			get
			{
				return useTransitionLocInfo;
			}
		}

		public GUILoadingScreenContext.LoadingContext LoadingContext
		{
			get
			{
				return loadingContext;
			}
			set
			{
				loadingContext = value;
				useTransitionContext = false;
			}
		}

		public GUILoadingScreenContext.LocationInfo LocInfo
		{
			get
			{
				return locInfo;
			}
			set
			{
				locInfo = value;
				useTransitionLocInfo = false;
			}
		}

		public SpawnWaitWatcher(TransactionMonitor tm)
		{
			transactionMonitor = tm;
			isModal = true;
			isFullscreen = true;
			isVerbose = false;
			transitionInTime = 0.5f;
			transitionOutTime = 0.5f;
			minimumWaitTime = 1f;
			useTransitionContext = true;
			useTransitionLocInfo = true;
		}
	}

	public class DumpWaitWatcher : ShsEventMessage
	{
	}

	public class KillWaitWatcher : ShsEventMessage
	{
	}

	public class UpdateWaitWatcher : ShsEventMessage
	{
		public TransactionMonitor transactionMonitor;

		public string titleText;

		public string helperText;

		public UpdateWaitWatcher(TransactionMonitor tm)
		{
			transactionMonitor = tm;
		}
	}

	public enum NotificationType
	{
		TransitionInStarted,
		TransitionInFinished,
		TransitionOutStarted,
		TransitionOutFinished,
		TransactionFailed,
		TransactionComplete,
		TransactionUpdate
	}

	public class WaitNotification : ShsEventMessage
	{
		public TransactionMonitor transactionMonitor;

		public NotificationType notificationType;

		public int stepsCompleted;

		public int totalSteps;

		public WaitNotification(TransactionMonitor tm)
		{
			transactionMonitor = tm;
			notificationType = NotificationType.TransactionUpdate;
		}
	}

	public class StepComplete : ShsEventMessage
	{
		public TransactionMonitor transactionMonitor;

		public string stepName;

		public string updateNotice;

		public StepComplete(TransactionMonitor tm, string name, string notice)
		{
			transactionMonitor = tm;
			stepName = name;
			updateNotice = notice;
		}
	}

	public class TransactionComplete : ShsEventMessage
	{
		public TransactionMonitor transactionMonitor;

		public TransactionMonitor.ExitCondition exitCondition;

		public string exitString;

		public TransactionComplete(TransactionMonitor tm, TransactionMonitor.ExitCondition ec, string s)
		{
			transactionMonitor = tm;
			exitCondition = ec;
			exitString = s;
		}
	}
}
