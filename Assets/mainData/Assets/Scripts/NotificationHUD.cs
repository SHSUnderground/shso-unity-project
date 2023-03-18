using System;
using System.Collections.Generic;
using UnityEngine;

public class NotificationHUD : GUISimpleControlWindow
{
	private class QueueSettings
	{
		public static int ALIGN_LEFT = -1;

		public static int ALIGN_CENTER;

		public static int ALIGN_RIGHT = 1;

		public int baseYOffset;

		public int deltaY = 80;

		public int maxWindows = 4;

		public int baseX = 2;

		public int horizontalAlignment = ALIGN_LEFT;
	}

	private static Dictionary<NotificationData.NotificationOrientation, List<NotificationWindow>> _windows = new Dictionary<NotificationData.NotificationOrientation, List<NotificationWindow>>();

	private static Dictionary<NotificationData.NotificationOrientation, List<NotificationWindow>> _queues = new Dictionary<NotificationData.NotificationOrientation, List<NotificationWindow>>();

	private static Dictionary<NotificationData.NotificationOrientation, QueueSettings> _settings = new Dictionary<NotificationData.NotificationOrientation, QueueSettings>();

	private AnimClipManager _clipManager = new AnimClipManager();

	private static NotificationHUD _instance;

	public AnimClipManager animManager
	{
		get
		{
			return _clipManager;
		}
	}

	public NotificationHUD()
	{
		_instance = this;
		AppShell.Instance.EventMgr.AddListener<GUIResizeMessage>(HandleResize);
		AppShell.Instance.EventMgr.AddListener<StopTrackingAchievementMessage>(OnStopTrackingAchievementMessage);
		NotificationData.NotificationOrientation[] array = (NotificationData.NotificationOrientation[])Enum.GetValues(typeof(NotificationData.NotificationOrientation));
		foreach (NotificationData.NotificationOrientation key in array)
		{
			_windows[key] = new List<NotificationWindow>();
			_queues[key] = new List<NotificationWindow>();
		}
		QueueSettings queueSettings = new QueueSettings();
		queueSettings.baseX = 2;
		queueSettings.deltaY = 80;
		queueSettings.maxWindows = 5;
		queueSettings.baseYOffset = 210;
		_settings[NotificationData.NotificationOrientation.Left] = queueSettings;
		QueueSettings queueSettings2 = new QueueSettings();
		queueSettings2.baseX = Screen.width;
		queueSettings2.deltaY = 140;
		queueSettings2.baseYOffset = 90;
		queueSettings2.maxWindows = 3;
		queueSettings2.horizontalAlignment = QueueSettings.ALIGN_RIGHT;
		_settings[NotificationData.NotificationOrientation.Right] = queueSettings2;
		QueueSettings queueSettings3 = new QueueSettings();
		queueSettings3.baseX = Screen.width / 2;
		queueSettings3.deltaY = 80;
		queueSettings3.baseYOffset = 350;
		queueSettings3.maxWindows = 1;
		queueSettings3.horizontalAlignment = QueueSettings.ALIGN_CENTER;
		_settings[NotificationData.NotificationOrientation.Center] = queueSettings3;
		QueueSettings queueSettings4 = new QueueSettings();
		queueSettings4.baseX = Screen.width;
		queueSettings4.deltaY = 140;
		queueSettings4.baseYOffset = 195;
		queueSettings4.maxWindows = 3;
		queueSettings4.horizontalAlignment = QueueSettings.ALIGN_RIGHT;
		_settings[NotificationData.NotificationOrientation.AchievementTracker] = queueSettings4;
	}

	private new void HandleResize(GUIResizeMessage message)
	{
		QueueSettings queueSettings = _settings[NotificationData.NotificationOrientation.Center];
		Vector2 newSize = message.NewSize;
		queueSettings.baseX = (int)newSize.x / 2;
		QueueSettings queueSettings2 = _settings[NotificationData.NotificationOrientation.Right];
		Vector2 newSize2 = message.NewSize;
		queueSettings2.baseX = (int)newSize2.x;
		QueueSettings queueSettings3 = _settings[NotificationData.NotificationOrientation.AchievementTracker];
		Vector2 newSize3 = message.NewSize;
		queueSettings3.baseX = (int)newSize3.x;
		foreach (NotificationWindow item in _windows[NotificationData.NotificationOrientation.Center])
		{
			Vector2 newSize4 = message.NewSize;
			float num = (int)newSize4.x / 2;
			Vector2 size = item.Size;
			float x = num - size.x / 2f;
			Vector2 position = item.Position;
			item.SetPosition(new Vector2(x, position.y));
		}
		foreach (NotificationWindow item2 in _windows[NotificationData.NotificationOrientation.Right])
		{
			Vector2 newSize5 = message.NewSize;
			float num2 = (int)newSize5.x;
			Vector2 size2 = item2.Size;
			float x2 = num2 - size2.x;
			Vector2 position2 = item2.Position;
			item2.SetPosition(new Vector2(x2, position2.y));
		}
		foreach (NotificationWindow item3 in _windows[NotificationData.NotificationOrientation.AchievementTracker])
		{
			Vector2 newSize6 = message.NewSize;
			float num3 = (int)newSize6.x;
			Vector2 size3 = item3.Size;
			float x3 = num3 - size3.x;
			Vector2 position3 = item3.Position;
			item3.SetPosition(new Vector2(x3, position3.y));
		}
	}

	private void OnStopTrackingAchievementMessage(StopTrackingAchievementMessage msg)
	{
		foreach (List<NotificationWindow> value in _queues.Values)
		{
			NotificationWindow notificationWindow = null;
			foreach (NotificationWindow item in value)
			{
				AchievementTrackerNotificationData achievementTrackerNotificationData = item.getData() as AchievementTrackerNotificationData;
				if (achievementTrackerNotificationData != null && achievementTrackerNotificationData.achievementID == msg.achievementID)
				{
					notificationWindow = item;
				}
			}
			if (notificationWindow != null)
			{
				value.Remove(notificationWindow);
				break;
			}
		}
	}

	public void Init()
	{
	}

	public static void addNotification(NotificationData data)
	{
		NotificationWindow notificationWindow = null;
		if (_windows.ContainsKey(data.orientation))
		{
			foreach (NotificationWindow item in _windows[data.orientation])
			{
				if (item.absorb(data))
				{
					return;
				}
			}
		}
		if (_queues.ContainsKey(data.orientation))
		{
			foreach (NotificationWindow item2 in _queues[data.orientation])
			{
				if (item2.absorb(data))
				{
					return;
				}
			}
		}
		switch (data.notificationType)
		{
		case NotificationData.NotificationType.TotalFractals:
		{
			TotalFractalNotificationWindow totalFractalNotificationWindow = GUIControl.CreateControlTopLeftFrame<TotalFractalNotificationWindow>(GeneralNotificationWindow.size, new Vector2(0f, 0f));
			totalFractalNotificationWindow.init(_instance, data);
			notificationWindow = totalFractalNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.DailyFractals:
		{
			DailyFractalNotificationWindow dailyFractalNotificationWindow = GUIControl.CreateControlTopLeftFrame<DailyFractalNotificationWindow>(GeneralNotificationWindow.size, new Vector2(0f, 0f));
			dailyFractalNotificationWindow.init(_instance, data);
			notificationWindow = dailyFractalNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.DailyGoldenFractal:
		{
			DailyGoldenFractalNotificationWindow dailyGoldenFractalNotificationWindow = GUIControl.CreateControlTopLeftFrame<DailyGoldenFractalNotificationWindow>(GeneralNotificationWindow.size, new Vector2(0f, 0f));
			dailyGoldenFractalNotificationWindow.init(_instance, data);
			notificationWindow = dailyGoldenFractalNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.DailyScavenger:
		{
			DailyScavengerNotificationWindow dailyScavengerNotificationWindow = GUIControl.CreateControlTopLeftFrame<DailyScavengerNotificationWindow>(GeneralNotificationWindow.size, new Vector2(0f, 0f));
			dailyScavengerNotificationWindow.init(_instance, data);
			notificationWindow = dailyScavengerNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.DailyTokens:
		{
			DailyTokenNotificationWindow dailyTokenNotificationWindow = GUIControl.CreateControlTopLeftFrame<DailyTokenNotificationWindow>(GeneralNotificationWindow.size, new Vector2(0f, 0f));
			dailyTokenNotificationWindow.init(_instance, data);
			notificationWindow = dailyTokenNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.DailyWheresImpossibleMan:
		{
			DailyWheresImpossibleManNotificationWindow dailyWheresImpossibleManNotificationWindow = GUIControl.CreateControlTopLeftFrame<DailyWheresImpossibleManNotificationWindow>(GeneralNotificationWindow.size, Vector2.zero);
			dailyWheresImpossibleManNotificationWindow.init(_instance, data);
			notificationWindow = dailyWheresImpossibleManNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.DailySeasonals:
		{
			DailySeasonalNotificationWindow dailySeasonalNotificationWindow = GUIControl.CreateControlTopLeftFrame<DailySeasonalNotificationWindow>(GeneralNotificationWindow.size, new Vector2(0f, 0f));
			dailySeasonalNotificationWindow.init(_instance, data);
			notificationWindow = dailySeasonalNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.DailyRareSeasonal:
		{
			DailyRareSeasonalNotificationWindow dailyRareSeasonalNotificationWindow = GUIControl.CreateControlTopLeftFrame<DailyRareSeasonalNotificationWindow>(GeneralNotificationWindow.size, new Vector2(0f, 0f));
			dailyRareSeasonalNotificationWindow.init(_instance, data);
			notificationWindow = dailyRareSeasonalNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.TotalScavenger:
		{
			TotalScavengerNotificationWindow totalScavengerNotificationWindow = GUIControl.CreateControlTopLeftFrame<TotalScavengerNotificationWindow>(GeneralNotificationWindow.size, new Vector2(0f, 0f));
			totalScavengerNotificationWindow.init(_instance, data);
			notificationWindow = totalScavengerNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.TotalSilver:
		{
			TotalSilverNotificationWindow totalSilverNotificationWindow = GUIControl.CreateControlTopLeftFrame<TotalSilverNotificationWindow>(GeneralNotificationWindow.size, new Vector2(0f, 0f));
			totalSilverNotificationWindow.init(_instance, data);
			notificationWindow = totalSilverNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.TotalTickets:
		{
			TotalTicketsNotificationWindow totalTicketsNotificationWindow = GUIControl.CreateControlTopLeftFrame<TotalTicketsNotificationWindow>(GeneralNotificationWindow.size, new Vector2(0f, 0f));
			totalTicketsNotificationWindow.init(_instance, data);
			notificationWindow = totalTicketsNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.BrawlerInvite:
		{
			BrawlerInviteNotificationWindow brawlerInviteNotificationWindow = new BrawlerInviteNotificationWindow();
			brawlerInviteNotificationWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
			brawlerInviteNotificationWindow.init(_instance, data);
			notificationWindow = brawlerInviteNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.CardGameInvite:
		{
			CardGameInviteNotificationWindow cardGameInviteNotificationWindow = new CardGameInviteNotificationWindow();
			cardGameInviteNotificationWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
			cardGameInviteNotificationWindow.init(_instance, data);
			notificationWindow = cardGameInviteNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.OldAchievement:
		{
			OldAchievementNotificationWindow oldAchievementNotificationWindow = new OldAchievementNotificationWindow();
			oldAchievementNotificationWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
			oldAchievementNotificationWindow.init(_instance, data);
			notificationWindow = oldAchievementNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.FriendInvite:
		{
			FriendInviteNotificationWindow friendInviteNotificationWindow = new FriendInviteNotificationWindow();
			friendInviteNotificationWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
			friendInviteNotificationWindow.init(_instance, data);
			notificationWindow = friendInviteNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.GameAreaAvailable:
		{
			GameAreaAvailableNotificationWindow gameAreaAvailableNotificationWindow = new GameAreaAvailableNotificationWindow();
			gameAreaAvailableNotificationWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
			gameAreaAvailableNotificationWindow.init(_instance, data);
			notificationWindow = gameAreaAvailableNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.DailyReward:
		{
			DailyRewardNotificationWindow dailyRewardNotificationWindow = new DailyRewardNotificationWindow();
			dailyRewardNotificationWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
			dailyRewardNotificationWindow.init(_instance, data);
			notificationWindow = dailyRewardNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.AchievementTracker:
		{
			AchievementTrackerNotificationWindow achievementTrackerNotificationWindow = new AchievementTrackerNotificationWindow();
			achievementTrackerNotificationWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
			achievementTrackerNotificationWindow.init(_instance, data);
			notificationWindow = achievementTrackerNotificationWindow;
			break;
		}
		case NotificationData.NotificationType.AchievementComplete:
		{
			AchievementCompleteNotification achievementCompleteNotification = new AchievementCompleteNotification();
			achievementCompleteNotification.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
			achievementCompleteNotification.init(_instance, data);
			notificationWindow = achievementCompleteNotification;
			break;
		}
		case NotificationData.NotificationType.Help:
		{
			HelpNotificationWindow helpNotificationWindow = new HelpNotificationWindow();
			helpNotificationWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
			helpNotificationWindow.init(_instance, data);
			notificationWindow = helpNotificationWindow;
			break;
		}
		default:
			CspUtils.DebugLog("NotificationHUD addNotification got bad NotificationType " + data.notificationType);
			return;
		}
		if (_instance != null)
		{
			_instance.queueWindow(notificationWindow);
		}
	}

	public void queueWindow(NotificationWindow window)
	{
		NotificationData.NotificationOrientation orientation = window.getData().orientation;
		_queues[orientation].Add(window);
		while (_windows[orientation].Count < _settings[orientation].maxWindows && _queues[orientation].Count > 0)
		{
			showNextWindow(orientation);
		}
	}

	public override void Update()
	{
		base.Update();
		animManager.Update(Time.deltaTime);
		foreach (List<NotificationWindow> value in _windows.Values)
		{
			foreach (NotificationWindow item in value)
			{
				item.update();
			}
		}
	}

	public void notificationComplete(NotificationWindow window)
	{
		NotificationData.NotificationOrientation orientation = window.getData().orientation;
		_windows[orientation].Remove(window);
		Remove(window);
		window.Dispose();
		showNextWindow(orientation);
	}

	public void showNextWindow(NotificationData.NotificationOrientation whichQueue)
	{
		int num = _settings[whichQueue].baseX;
		int deltaY = _settings[whichQueue].deltaY;
		if (_queues[whichQueue].Count > 0 && _windows[whichQueue].Count < _settings[whichQueue].maxWindows)
		{
			NotificationWindow notificationWindow = _queues[whichQueue][0];
			_queues[whichQueue].RemoveAt(0);
			if (_settings[whichQueue].horizontalAlignment == QueueSettings.ALIGN_RIGHT)
			{
				int num2 = num;
				Vector2 size = notificationWindow.Size;
				num = num2 - (int)size.x;
			}
			else if (_settings[whichQueue].horizontalAlignment == QueueSettings.ALIGN_CENTER)
			{
				int num3 = num;
				Vector2 size2 = notificationWindow.Size;
				num = num3 - (int)size2.x / 2;
			}
			int maxWindows = _settings[whichQueue].maxWindows;
			notificationWindow.SetPosition(num, (_settings[whichQueue].maxWindows + 2) * deltaY + 5);
			_windows[whichQueue].Add(notificationWindow);
			Add(notificationWindow);
			notificationWindow.activate();
		}
		int num4 = _settings[whichQueue].baseYOffset + (_settings[whichQueue].maxWindows - _windows[whichQueue].Count) * deltaY;
		foreach (NotificationWindow item in _windows[whichQueue])
		{
			Vector2 position = item.Position;
			AnimClip toAdd = AnimClipBuilder.Absolute.PositionY(AnimClipBuilder.Path.Quadratic(position.y, num4, 0.2f, 0.3f), item);
			animManager.Add(toAdd);
			num4 += deltaY;
		}
	}
}
