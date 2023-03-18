using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUINotificationManager
{
	private class SHSNotificationInvitationHandler : GUIWindow
	{
		private List<SHSInvitationWindow> invitationWindowList = new List<SHSInvitationWindow>();

		private List<SHSInvitationWindow> addQueueList = new List<SHSInvitationWindow>();

		private List<SHSInvitationWindow> removeQueueList = new List<SHSInvitationWindow>();

		private IGUIContainer notificationWindow;

		private SHSInvitationWindow focusedWindow;

		public List<SHSInvitationWindow> InvitationWindowList
		{
			get
			{
				return invitationWindowList;
			}
		}

		public SHSNotificationInvitationHandler(IGUIContainer notificationWindow)
		{
			this.notificationWindow = notificationWindow;
		}

		protected override void dispose(bool disposing)
		{
			if (disposing)
			{
				invitationWindowList = null;
				addQueueList = null;
				removeQueueList = null;
			}
			base.dispose(disposing);
		}

		public override void Update()
		{
			base.Update();
			ProcessAllAddAndRemove();
		}

		private void InternalAddInvitation(SHSInvitationWindow invitation)
		{
			invitationWindowList.Add(invitation);
			AdjustIndices();
			notificationWindow.Add(invitation);
			invitation.OnClosedCallback += delegate
			{
				CloseInvitation(invitation);
			};
			invitation.Show();
		}

		private void InternalCloseInvitation(SHSInvitationWindow invitation)
		{
			if (object.ReferenceEquals(invitation, focusedWindow))
			{
				focusedWindow = null;
			}
			invitationWindowList.Remove(invitation);
			AdjustIndices();
		}

		public void AddInvitation(SHSInvitationWindow invitation)
		{
			addQueueList.Add(invitation);
		}

		public void CloseInvitation(SHSInvitationWindow invitation)
		{
			removeQueueList.Add(invitation);
		}

		private void ProcessAllAddAndRemove()
		{
			foreach (SHSInvitationWindow removeQueue in removeQueueList)
			{
				InternalCloseInvitation(removeQueue);
			}
			removeQueueList.Clear();
			foreach (SHSInvitationWindow addQueue in addQueueList)
			{
				InternalAddInvitation(addQueue);
			}
			addQueueList.Clear();
		}

		public void SendNotificationToFront(SHSInvitationWindow invitation)
		{
			if (focusedWindow == null)
			{
				focusedWindow = invitation;
				ProcessAllAddAndRemove();
				InternalSendNotificationToFront(invitation);
			}
		}

		public void SendNotificationToPriorPosition(SHSInvitationWindow invitation)
		{
			if (addQueueList.Count == 0 && removeQueueList.Count == 0 && object.ReferenceEquals(focusedWindow, invitation))
			{
				InternalSendNotificationToPriorPosition(invitation);
				focusedWindow = null;
			}
		}

		private void InternalSendNotificationToFront(SHSInvitationWindow invitation)
		{
			foreach (SHSInvitationWindow invitationWindow in invitationWindowList)
			{
				notificationWindow.Remove(invitationWindow);
			}
			invitationWindowList.Remove(invitation);
			if (invitationWindowList.Count > SHSInvitationWindow.ViewableInviteCount)
			{
				invitationWindowList.Insert(SHSInvitationWindow.ViewableInviteCount - 1, invitation);
			}
			else
			{
				invitationWindowList.Add(invitation);
			}
			foreach (SHSInvitationWindow invitationWindow2 in invitationWindowList)
			{
				notificationWindow.Add(invitationWindow2);
			}
		}

		private void InternalSendNotificationToPriorPosition(SHSInvitationWindow invitation)
		{
			foreach (SHSInvitationWindow invitationWindow in invitationWindowList)
			{
				notificationWindow.Remove(invitationWindow);
			}
			for (int i = 0; i < invitationWindowList.Count; i++)
			{
				for (int j = i + 1; j < invitationWindowList.Count; j++)
				{
					if (invitationWindowList[i].Index > invitationWindowList[j].Index)
					{
						SHSInvitationWindow value = invitationWindowList[i];
						invitationWindowList[i] = invitationWindowList[j];
						invitationWindowList[j] = value;
					}
				}
			}
			foreach (SHSInvitationWindow invitationWindow2 in invitationWindowList)
			{
				notificationWindow.Add(invitationWindow2);
			}
		}

		private void AdjustIndices()
		{
			int num = 0;
			int viewableInviteCount = SHSInvitationWindow.ViewableInviteCount;
			foreach (SHSInvitationWindow invitationWindow in invitationWindowList)
			{
				invitationWindow.Index = ((num < viewableInviteCount) ? num++ : viewableInviteCount);
			}
		}

		public float GetMaxOffsetFromActiveInvites()
		{
			int num = (invitationWindowList.Count <= SHSInvitationWindow.ViewableInviteCount) ? invitationWindowList.Count : SHSInvitationWindow.ViewableInviteCount;
			SHSInvitationViewWindow sHSInvitationViewWindow = null;
			float num2 = 0f;
			for (int i = 0; i < num; i++)
			{
				if (invitationWindowList[i] is SHSInvitationViewWindow)
				{
					sHSInvitationViewWindow = (SHSInvitationViewWindow)invitationWindowList[i];
					Vector2 windowSize = sHSInvitationViewWindow.WindowSize;
					float x = windowSize.x;
					Vector2 offset = sHSInvitationViewWindow.Offset;
					float num3 = x + (0f - offset.x);
					if (num2 < num3)
					{
						num2 = num3;
					}
				}
			}
			return num2;
		}
	}

	public enum GUINotificationStyleEnum
	{
		AchievementNotify,
		CoinUpNotify,
		TicketNotify,
		FeatherNotify,
		BrawlerInvite,
		CardGameInvite,
		HQInvite,
		FriendRequest,
		FriendNotify,
		ErrorNotify,
		LevelUpNotify,
		GameAreaUnlocked,
		FriendOfflineNotify,
		FractalNotify
	}

	public struct NotificationKeyEntry
	{
		public KeyCodeEntry keyEntry;

		public SHSInput.KeyEventDelegate keyCallback;

		public NotificationKeyEntry(KeyCodeEntry keyEntry, SHSInput.KeyEventDelegate keyCallback)
		{
			this.keyEntry = keyEntry;
			this.keyCallback = keyCallback;
		}
	}

	private const int MaxNotificationKeyCodes = 5;

	private IGUIContainer notificationWindow;

	private SHSNotificationInvitationHandler invitationHandler;

	private Dictionary<GUINotificationStyleEnum, Type> stockWindows;

	public List<SHSInvitationWindow> invitationWindowList;

	private Hashtable additionalWindowArguments = new Hashtable();

	private static NotificationKeyEntry[] debugKeyCodes;

	private static int gameAreaLookupIndex;

	private static int gameAreaLookupMax;

	public static NotificationKeyEntry[] DebugKeyCodes
	{
		get
		{
			return debugKeyCodes;
		}
	}

	public GUINotificationManager(GUIManager Manager, IGUIContainer notificationWindow)
	{
		invitationHandler = new SHSNotificationInvitationHandler(notificationWindow);
		this.notificationWindow = notificationWindow;
		this.notificationWindow.Add(invitationHandler);
		stockWindows = new Dictionary<GUINotificationStyleEnum, Type>();
		invitationWindowList = new List<SHSInvitationWindow>();
		RegisterStockWindow(GUINotificationStyleEnum.AchievementNotify, typeof(SHSAchievementUnlockedWindow));
		RegisterStockWindow(GUINotificationStyleEnum.CoinUpNotify, typeof(SHSItemViewerWindow));
		RegisterStockWindow(GUINotificationStyleEnum.TicketNotify, typeof(SHSItemViewerWindow));
		RegisterStockWindow(GUINotificationStyleEnum.GameAreaUnlocked, typeof(SHSGameAreaUnlockedWindow));
		RegisterStockWindow(GUINotificationStyleEnum.BrawlerInvite, typeof(SHSBrawlerInvitationWindow));
		RegisterStockWindow(GUINotificationStyleEnum.CardGameInvite, typeof(SHSCardGameInvitationWindow));
		RegisterStockWindow(GUINotificationStyleEnum.HQInvite, typeof(SHSInvitationWindow));
		RegisterStockWindow(GUINotificationStyleEnum.FriendRequest, typeof(SHSFriendInvitationWindow));
		RegisterStockWindow(GUINotificationStyleEnum.FriendNotify, typeof(SHSFriendNotifyWindow));
		RegisterStockWindow(GUINotificationStyleEnum.FriendOfflineNotify, typeof(SHSFriendOfflineNotifyWindow));
		RegisterStockWindow(GUINotificationStyleEnum.ErrorNotify, typeof(SHSInvitationWindow));
		RegisterStockWindow(GUINotificationStyleEnum.LevelUpNotify, typeof(SHSLeveledUpNotifyWindow));
		RegisterStockWindow(GUINotificationStyleEnum.FractalNotify, typeof(SHSItemViewerWindow));
		additionalWindowArguments.Add(GUINotificationStyleEnum.TicketNotify, new object[2]
		{
			GUINotificationStyleEnum.TicketNotify,
			this.notificationWindow
		});
		additionalWindowArguments.Add(GUINotificationStyleEnum.CoinUpNotify, new object[2]
		{
			GUINotificationStyleEnum.CoinUpNotify,
			this.notificationWindow
		});
		additionalWindowArguments.Add(GUINotificationStyleEnum.FractalNotify, new object[2]
		{
			GUINotificationStyleEnum.FractalNotify,
			this.notificationWindow
		});
		AppShell.Instance.EventMgr.AddListener<InvitationBrawlerMessage>(OnBrawlerInviteMessage);
		AppShell.Instance.EventMgr.AddListener<InvitationBrawlerCanceledMessage>(OnBrawlerInviteCanceledMessage);
		AppShell.Instance.EventMgr.AddListener<InvitationCardGameMessage>(OnCardGameInviteMessage);
		AppShell.Instance.EventMgr.AddListener<InvitationCardGameCanceledMessage>(OnCardGameInviteCanceledMessage);
		AppShell.Instance.EventMgr.AddListener<FriendRequestMessage>(OnFriendRequestMessage);
		AppShell.Instance.EventMgr.AddListener<FriendAcceptedMessage>(OnFriendAcceptedMessage);
		AppShell.Instance.EventMgr.AddListener<FriendDeclinedMessage>(OnFriendDeclinedMessage);
	}

	public static void AddDebugKeys()
	{
		if (debugKeyCodes == null)
		{
			debugKeyCodes = new NotificationKeyEntry[5];
			debugKeyCodes[0] = new NotificationKeyEntry(new KeyCodeEntry(KeyCode.Z, true, true, false, false), OnAchievement);
			debugKeyCodes[1] = new NotificationKeyEntry(new KeyCodeEntry(KeyCode.X, true, true, false, false), OnCoinup);
			debugKeyCodes[2] = new NotificationKeyEntry(new KeyCodeEntry(KeyCode.C, true, true, false, false), OnTicket);
			debugKeyCodes[3] = new NotificationKeyEntry(new KeyCodeEntry(KeyCode.V, true, true, false, false), OnInvitation);
			debugKeyCodes[4] = new NotificationKeyEntry(new KeyCodeEntry(KeyCode.B, true, true, false, false), OnGameAreaLoaded);
		}
		for (int i = 0; i < debugKeyCodes.Length; i++)
		{
			SHSDebugInput.Inst.AddKeyListener(debugKeyCodes[i].keyEntry, debugKeyCodes[i].keyCallback);
		}
	}

	public static void RemoveDebugKeys()
	{
		for (int i = 0; i < debugKeyCodes.Length; i++)
		{
			SHSDebugInput.Inst.RemoveKeyListener(debugKeyCodes[i].keyEntry);
		}
	}

	public void AddInvitation(SHSInvitationWindow invitation)
	{
		invitationHandler.AddInvitation(invitation);
	}

	public void CloseInvitation(SHSInvitationWindow invitation)
	{
		invitationHandler.CloseInvitation(invitation);
	}

	public void SendNotificationToFront(SHSInvitationWindow win)
	{
		invitationHandler.SendNotificationToFront(win);
	}

	public void SendNotificationToPriorPosition(SHSInvitationWindow win)
	{
		invitationHandler.SendNotificationToPriorPosition(win);
	}

	public float GetMaxOffsetFromActiveInvites()
	{
		return invitationHandler.GetMaxOffsetFromActiveInvites();
	}

	private void OnBrawlerInviteMessage(InvitationBrawlerMessage message)
	{
		CspUtils.DebugLog("incoming invitation." + message.invitation.invitationId);
		if (message.invitation != null)
		{
			BrawlerInviteNotificationData data = new BrawlerInviteNotificationData(message.invitation);
			NotificationHUD.addNotification(data);
		}
	}

	private void OnBrawlerInviteCanceledMessage(InvitationBrawlerCanceledMessage message)
	{
		CspUtils.DebugLog("Canceled invitation." + message.invitation.invitationId + " reason: " + message.reason);
		Matchmaker2.BrawlerInvitation invitation = message.invitation;
		foreach (SHSInvitationViewWindow invitationWindow in invitationHandler.InvitationWindowList)
		{
			if (invitationWindow.Shell.Invitation.invitationId == invitation.invitationId)
			{
				invitationWindow.Shell.OnCancelled(message.reason);
			}
		}
	}

	private void OnCardGameInviteMessage(InvitationCardGameMessage message)
	{
		CspUtils.DebugLog("incoming invitation." + message.invitation.invitationId);
		if (message.invitation != null)
		{
			CardGameInviteNotificationData data = new CardGameInviteNotificationData(message.invitation);
			NotificationHUD.addNotification(data);
		}
	}

	private void OnCardGameInviteCanceledMessage(InvitationCardGameCanceledMessage message)
	{
		CspUtils.DebugLog("Canceled invitation." + message.invitation.invitationId + " reason: " + message.reason);
		Matchmaker2.CardGameInvitation invitation = message.invitation;
		foreach (SHSInvitationViewWindow invitationWindow in invitationHandler.InvitationWindowList)
		{
			if (invitationWindow.Shell.Invitation.invitationId == invitation.invitationId)
			{
				invitationWindow.OnCancelled(message.reason);
			}
		}
	}

	private void OnFriendRequestMessage(FriendRequestMessage message)
	{
		CspUtils.DebugLog("Incoming friend request." + message.FriendID);
		FriendInviteNotificationData data = new FriendInviteNotificationData(message);
		NotificationHUD.addNotification(data);
	}

	private void OnFriendAcceptedMessage(FriendAcceptedMessage message)
	{
		AppShell.Instance.Profile.AvailableFriends.ReloadFriendList();
		message.FetchSquadName(delegate(string squadName)
		{
			if (!string.IsNullOrEmpty(squadName))
			{
				StringTable stringTable = AppShell.Instance.stringTable;
				Display(GUINotificationStyleEnum.FriendNotify, stringTable["#friendnotify_title_1"] + " " + squadName + " " + stringTable["#friendnotify_title_2"]);
			}
		});
		foreach (SHSInvitationViewWindow invitationWindow in invitationHandler.InvitationWindowList)
		{
			if (invitationWindow.Shell is SHSFriendInvitationWindow)
			{
				SHSFriendInvitationWindow sHSFriendInvitationWindow = (SHSFriendInvitationWindow)invitationWindow.Shell;
				if (sHSFriendInvitationWindow.RequestorID == message.FriendID)
				{
					invitationWindow.PerformCloseProcess();
				}
			}
		}
	}

	private void OnFriendDeclinedMessage(FriendDeclinedMessage message)
	{
		Friend value;
		if (AppShell.Instance.Profile.AvailableFriends.TryGetValue(message.FriendID.ToString(), out value))
		{
			AppShell.Instance.Profile.AvailableFriends.RemoveFriend(message.FriendID);
		}
		else
		{
			AppShell.Instance.Profile.AvailableFriends.IsPlayerInSentInvites(message.FriendID, delegate(bool isInSentInvitations)
			{
				if (isInSentInvitations)
				{
					AppShell.Instance.Profile.AvailableFriends.DeclineFriend(message.FriendID);
				}
			});
		}
		foreach (SHSInvitationViewWindow invitationWindow in invitationHandler.InvitationWindowList)
		{
			if (invitationWindow.Shell is SHSFriendInvitationWindow)
			{
				SHSFriendInvitationWindow sHSFriendInvitationWindow = (SHSFriendInvitationWindow)invitationWindow.Shell;
				if (sHSFriendInvitationWindow.RequestorID == message.FriendID)
				{
					invitationWindow.PerformCloseProcess();
				}
			}
		}
	}

	public void Display(GUINotificationStyleEnum style, params object[] values)
	{
		GUINotificationWindow gUINotificationWindow = null;
		if (additionalWindowArguments.ContainsKey(style))
		{
			int num = (values != null) ? values.Length : 0;
			int num2 = ((object[])additionalWindowArguments[style]).Length;
			object[] array = new object[num + num2];
			for (int i = 0; i < array.Length; i++)
			{
				if (i <= num2 - 1)
				{
					array[i] = ((object[])additionalWindowArguments[style])[i];
				}
				else
				{
					array[i] = values[i - num2];
				}
			}
			gUINotificationWindow = (GUINotificationWindow)Activator.CreateInstance(stockWindows[style], new object[1]
			{
				array
			});
		}
		else
		{
			gUINotificationWindow = (GUINotificationWindow)Activator.CreateInstance(stockWindows[style], values);
		}
		notificationWindow.Add(gUINotificationWindow);
		gUINotificationWindow.Show();
	}

	public void Display(GUIVisualCueWindow window)
	{
		notificationWindow.Add(window);
		window.Show();
	}

	public void RegisterStockWindow(GUINotificationStyleEnum style, Type windowType)
	{
		stockWindows[style] = windowType;
	}

	public static void OnInvitation(SHSKeyCode keyEventInfo)
	{
		int num = UnityEngine.Random.Range(0, 2);
		SHSInvitationShell sHSInvitationShell = null;
		switch (num)
		{
		case 0:
		{
			Matchmaker2.BrawlerInvitation brawlerInvitation = new Matchmaker2.BrawlerInvitation();
			brawlerInvitation.inviterName = "Brawler Houdini";
			sHSInvitationShell = (SHSInvitationShell)Activator.CreateInstance(typeof(SHSBrawlerInvitationWindow), brawlerInvitation);
			break;
		}
		case 1:
		{
			Matchmaker2.CardGameInvitation cardGameInvitation = new Matchmaker2.CardGameInvitation();
			cardGameInvitation.inviterName = "Card Houdini";
			sHSInvitationShell = (SHSInvitationShell)Activator.CreateInstance(typeof(SHSCardGameInvitationWindow), cardGameInvitation);
			break;
		}
		case 2:
			sHSInvitationShell = (SHSFriendInvitationWindow)Activator.CreateInstance(typeof(SHSFriendInvitationWindow));
			((SHSFriendInvitationWindow)sHSInvitationShell).requestorSquadName = "Friend Houdini";
			break;
		}
		sHSInvitationShell.BuildViewWindow();
		GUIManager.Instance.NotificationManager.AddInvitation(sHSInvitationShell.ViewWindow);
	}

	public static void OnGameAreaLoaded(SHSKeyCode keyEventInfo)
	{
		Array values = Enum.GetValues(typeof(AssetBundleLoader.BundleGroup));
		if (gameAreaLookupMax == 0)
		{
			gameAreaLookupMax = values.Length;
		}
		gameAreaLookupIndex = ((gameAreaLookupIndex + 1 != gameAreaLookupMax) ? (gameAreaLookupIndex + 1) : 0);
		AssetBundleLoader.BundleGroup group = (AssetBundleLoader.BundleGroup)(int)values.GetValue(gameAreaLookupIndex);
		BundleGroupLoadedMessage msg = new BundleGroupLoadedMessage(group, true);
		AppShell.Instance.EventMgr.Fire(GUIManager.Instance, msg);
	}

	public static void OnAchievement(SHSKeyCode keyEventInfo)
	{
		Achievement achievement = new Achievement();
		achievement.Description = "You are a true playa. I worship you.";
		achievement.Name = "True PLAYA";
		achievement.Id = "best_there_is";
		achievement.Thresholds = new int[2]
		{
			1,
			2
		};
		GUIManager.Instance.NotificationManager.Display(GUINotificationStyleEnum.AchievementNotify, achievement, Achievement.AchievementLevelEnum.Bronze, Achievement.AchievementLevelEnum.Adamantium, AppShell.Instance.Profile.PlayerName);
		achievement = null;
	}

	public static void OnTicket(SHSKeyCode keyEventInfo)
	{
		NotificationHUD.addNotification(new TotalTicketsNotificationData(AppShell.Instance.Profile.Tickets));
	}

	public static void OnCoinup(SHSKeyCode keyEventInfo)
	{
		NotificationHUD.addNotification(new TotalSilverNotificationData(AppShell.Instance.Profile.Silver));
	}

	public static void OnFractal(SHSKeyCode keyEventInfo)
	{
		GUIManager.Instance.NotificationManager.Display(GUINotificationStyleEnum.FractalNotify, 1, true);
	}

	~GUINotificationManager()
	{
		AppShell.Instance.EventMgr.RemoveListener<InvitationBrawlerMessage>(OnBrawlerInviteMessage);
		AppShell.Instance.EventMgr.RemoveListener<InvitationBrawlerCanceledMessage>(OnBrawlerInviteCanceledMessage);
		AppShell.Instance.EventMgr.RemoveListener<InvitationCardGameMessage>(OnCardGameInviteMessage);
		AppShell.Instance.EventMgr.RemoveListener<InvitationCardGameCanceledMessage>(OnCardGameInviteCanceledMessage);
		AppShell.Instance.EventMgr.RemoveListener<FriendRequestMessage>(OnFriendRequestMessage);
		AppShell.Instance.EventMgr.RemoveListener<FriendAcceptedMessage>(OnFriendAcceptedMessage);
		AppShell.Instance.EventMgr.RemoveListener<FriendDeclinedMessage>(OnFriendDeclinedMessage);
	}
}
