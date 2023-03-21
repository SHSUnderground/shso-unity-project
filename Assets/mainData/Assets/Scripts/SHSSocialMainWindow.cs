using System.Collections.Generic;
using UnityEngine;

public class SHSSocialMainWindow : GUITopLevelWindow
{
	private enum UpsellState
	{
		Subscribe,
		ByGold
	}

	private const float SUBSCRIBE_BUTTON_FLIP_TIME = 300f;

	private SHSSocialCharacterDisplayWindow playerPortrait;

	private GUISimpleControlWindow buttonWindow;

	private GUIButton SubscribeButton;

	private GUIButton BuyGoldButton;

	private GUIButton FullScreenButton;

	private GUIButton WindowedScreenButton;

	private long currentTimerId = -1L;

	private GUISimpleControlWindow _globalNavAnchor;

	private GlobalNav _globalNav;

	private SHSFeatherNotifyWindow featherWindow;

	private SHSFractalNotifyWindow fractalWindow;

	private SHSScavengerNotifyWindow scavengerWindow;

	private SHSScavengerItemNotifyWindow scavengerItemWindow;

	private SHSSocialWatcherFractalFindMe watcherFindMeWindow;

	private FirstCraftFoundWindow firstCraftWindow;

	private SHSWorldEventWinnerWindow worldEventWonWindow;

	private SHSHeroUnlockedWindow heroUnlockedWindow;

	private SocialSpecialAbilityHUD specialAbilitySelector;

	private NotificationHUD notificationHUD;

	private Dictionary<KeyCodeEntry, sbyte> emoteDict;

	private bool emotesOn;

	public SHSSocialMainWindow()
		: base("SHSSocialMainWindow")
	{
		HitTestType = HitTestTypeEnum.Transparent;
		SHSIndicatorArrow sHSIndicatorArrow = new SHSIndicatorArrow();
		sHSIndicatorArrow.ShowOnScreen = false;
		Add(sHSIndicatorArrow);
		specialAbilitySelector = GUIControl.CreateControlTopLeftFrame<SocialSpecialAbilityHUD>(new Vector2(450f, 60f), new Vector2(180f, 2f));
		specialAbilitySelector.IsVisible = true;
		Add(specialAbilitySelector);
		specialAbilitySelector.Init();
		notificationHUD = new NotificationHUD();
		notificationHUD.IsVisible = true;
		Add(notificationHUD);
		notificationHUD.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		notificationHUD.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
		notificationHUD.Init();
		AppShell.Instance.EventMgr.AddListener<RepositionSocialHUDMessage>(OnRepositionSocialHUDMessage);
		playerPortrait = new SHSSocialCharacterDisplayWindow();
		Add(playerPortrait);
		bool flag = Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.IsPayingSubscriber);
		_globalNav = new GlobalNav(false);
		_globalNav.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
		_globalNavAnchor = GUIControl.CreateControlTopRightFrame<GUISimpleControlWindow>(_globalNav.Size, new Vector2(0f, 0f));
		Add(_globalNavAnchor);
		_globalNavAnchor.IsVisible = true;
		_globalNavAnchor.Add(_globalNav);

	}

	private void OnRepositionSocialHUDMessage(RepositionSocialHUDMessage msg)
	{
		SocialSpecialAbilityHUD socialSpecialAbilityHUD = specialAbilitySelector;
		float x = msg.xoffset - 40f;
		Vector2 position = specialAbilitySelector.Position;
		socialSpecialAbilityHUD.SetPosition(x, position.y);
		AppShell.Instance.EventMgr.RemoveListener<RepositionSocialHUDMessage>(OnRepositionSocialHUDMessage);
	}

	protected void UpdateUpsellButton(long timerId, bool canceled)
	{
		if (!canceled)
		{
			if (SubscribeButton.IsVisible)
			{
				SubscribeButton.IsVisible = false;
				BuyGoldButton.IsVisible = true;
				AppShell.Instance.SharedHashTable["GameWorldUpsellButton"] = UpsellState.ByGold;
			}
			else
			{
				SubscribeButton.IsVisible = true;
				BuyGoldButton.IsVisible = false;
				AppShell.Instance.SharedHashTable["GameWorldUpsellButton"] = UpsellState.Subscribe;
			}
			currentTimerId = AppShell.Instance.TimerMgr.CreateTimer(300f, UpdateUpsellButton);
		}
	}

	public override void ConfigureKeyBanks()
	{
		base.ConfigureKeyBanks();
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.Return, false, false, false), OnChatToggle);
	}

	private void OnChatToggle(SHSKeyCode code)
	{
		if (Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.OpenChatPermitSet))
		{
			List<SHSEmoteChatBar> controlsOfType = ((IGUIContainer)GUIManager.Instance.Root["SHSMainWindow"]).GetControlsOfType<SHSEmoteChatBar>();
			if (controlsOfType.Count != 0)
			{
				SHSEmoteChatBar sHSEmoteChatBar = controlsOfType[0];
				sHSEmoteChatBar.RequestOpenOpenChat();
			}
		}
	}

	public override void OnActive()
	{
		AppShell.Instance.EventMgr.AddListener<PlayerTargetedMessage>(OnTargetPlayer);
		AppShell.Instance.EventMgr.AddListener<FriendListUpdatedMessage>(OnFriendListUpdateMessage);
		base.OnActive();
		UserProfile profile = AppShell.Instance.Profile;
		if (profile != null)
		{
			if (this["SHSSocialCharacterSelectionMainWindow"] != null)
			{
				this["SHSSocialCharacterSelectionMainWindow"].Hide();
			}
			AddEmoteKeys();
			ToggleFullscreen(Screen.fullScreen);
			Entitlements.ServerValueEntitlement serverValueEntitlement = Singleton<Entitlements>.instance.EntitlementsSet[Entitlements.EntitlementFlagEnum.DailyRewardStr] as Entitlements.ServerValueEntitlement;
			CspUtils.DebugLog("daily reward str is " + serverValueEntitlement.Value);
			if (AppShell.Instance.Profile != null && !AppShell.Instance.Profile.dailyRewardDisplayed && serverValueEntitlement != null && serverValueEntitlement.Value != null && serverValueEntitlement.Value.Length > 0)
			{
				CspUtils.DebugLog("notif!");
				DailyRewardNotificationData data = new DailyRewardNotificationData(serverValueEntitlement.Value);
				NotificationHUD.addNotification(data);
				AppShell.Instance.Profile.dailyRewardDisplayed = true;
			}
		}
	}

	public void ToggleFullscreen(bool isFullscreen)
	{
		if (WindowedScreenButton != null && FullScreenButton != null)
		{
			WindowedScreenButton.IsVisible = isFullscreen;
			FullScreenButton.IsVisible = !isFullscreen;
		}
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		if (WindowedScreenButton != null && FullScreenButton != null)
		{
			ToggleFullscreen(Screen.fullScreen);
		}
	}

	public override void OnInactive()
	{
		AppShell.Instance.EventMgr.RemoveListener<PlayerTargetedMessage>(OnTargetPlayer);
		AppShell.Instance.EventMgr.RemoveListener<FriendListUpdatedMessage>(OnFriendListUpdateMessage);
		RemoveEmoteKeys();
		if (currentTimerId != -1)
		{
			AppShell.Instance.TimerMgr.CancelTimer(currentTimerId, false);
		}
		base.OnInactive();
	}

	protected void DoEmote(sbyte emote)
	{
		if (SocialSpaceController.Instance == null)
		{
			CspUtils.DebugLog("No social space controller");
			return;
		}
		GameObject localPlayer = SocialSpaceController.Instance.LocalPlayer;
		AppShell.Instance.EventMgr.Fire(localPlayer, new EmoteMessage(localPlayer, emote));
	}

	public void OnTargetPlayer(PlayerTargetedMessage msg)
	{
		GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "Would you like to invite " + msg.targetUserName + " to a Squad Battle?", delegate(string Id, GUIDialogWindow.DialogState state)
		{
			if (state == GUIDialogWindow.DialogState.Ok)
			{
				AppShell.Instance.EventMgr.Fire(null, new BattleInviteMessage(msg.targetUserId));
			}
			else
			{
				AppShell.Instance.EventMgr.Fire(null, new BattleInviteMessage(-1));
			}
		}, ModalLevelEnum.Default);
	}

	public void OnFriendListUpdateMessage(FriendListUpdatedMessage msg)
	{
		if (msg.UpdateType != FriendListUpdatedMessage.Type.Update)
		{
			PlayerBillboard[] array = Utils.FindObjectsOfType<PlayerBillboard>();
			PlayerBillboard[] array2 = array;
			foreach (PlayerBillboard playerBillboard in array2)
			{
				playerBillboard.UpdateRenderers();
			}
		}
	}

	public void OnCollectFeather(int featherCount, int featherMax)
	{
		OnCollectFeather(featherCount, featherMax, false);
	}

	public void OnCollectFeather(int featherCount, int featherMax, bool startup)
	{
		if (featherCount >= 0)
		{
			NotificationHUD.addNotification(new DailyTokenNotificationData(AppShell.Instance.Profile.LastSelectedCostume, featherCount, featherMax));
		}
	}

	public void OnCollectScavengerObject(int objectIndex)
	{
		OnCollectScavengerObject(objectIndex, false);
	}

	public void OnCollectScavengerObject(int objectIndex, bool startup)
	{
		HeroPersisted value;
		if (!startup)
		{
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("oi", objectIndex);
			wWWForm.AddField("hero_name", AppShell.Instance.Profile.SelectedCostume);
			wWWForm.AddField("zone_name", SocialSpaceController.Instance.ShortZoneName);
			AppShell.Instance.WebService.StartRequest("resources$users/scavenger_collect.py", delegate(ShsWebResponse response)
			{
				if (response.Status == 200)
				{
					AppShell.Instance.Profile.StartCraftFetch();
				}
			}, wWWForm.data);
		}
		else if (AppShell.Instance.Profile.AvailableCostumes.TryGetValue(AppShell.Instance.Profile.LastSelectedCostume, out value))
		{
			NotificationHUD.addNotification(new DailyScavengerNotificationData(value.objectsCollected, value.maxScavengeObjects));
		}
	}

	public void OnDisplayScavengerBalance(int count, int collectedOwnableID = -1)
	{
		if (count >= 0)
		{
			HeroPersisted value;
			if (AppShell.Instance.Profile.AvailableCostumes.TryGetValue(AppShell.Instance.Profile.SelectedCostume, out value))
			{
				NotificationHUD.addNotification(new DailyScavengerNotificationData(value.objectsCollected, value.maxScavengeObjects));
			}
			if (collectedOwnableID > 0)
			{
				NotificationHUD.addNotification(new TotalScavengerNotificationData(collectedOwnableID, OwnableDefinition.numOwned(collectedOwnableID, AppShell.Instance.Profile) + 1));
			}
		}
	}

	public void OnCollectScavengerItem(int collectedOwnableID)
	{
		scavengerItemWindow = new SHSScavengerItemNotifyWindow();
		scavengerItemWindow.IsVisible = true;
		scavengerItemWindow.SetCount(OwnableDefinition.numOwned(collectedOwnableID, AppShell.Instance.Profile));
		scavengerItemWindow.showSpecificIcon(collectedOwnableID);
		Add(scavengerItemWindow);
	}

	public void OnCollectFractal(FractalActivitySpawnPoint.FractalType fractalType, int fractalCount)
	{
		OnCollectFractal(fractalType, fractalCount, false);
	}

	public void OnCollectFractal(FractalActivitySpawnPoint.FractalType fractalType, int fractalCount, bool startup)
	{
		if (fractalCount >= 0)
		{
			SHSFractalsActivity sHSFractalsActivity = AppShell.Instance.ActivityManager.GetActivity("fractalsactivity") as SHSFractalsActivity;
			NotificationHUD.addNotification(new DailyFractalNotificationData(sHSFractalsActivity.GetFractalCollectionCount(), sHSFractalsActivity.MaxFractals));
		}
	}

	public void OnDisplayFractalBalance(FractalActivitySpawnPoint.FractalType fractalType, int fractalCount)
	{
	}

	public void OnFirstFractalCollected(FractalActivitySpawnPoint.FractalType fractalType, int amount)
	{
		watcherFindMeWindow = new SHSSocialWatcherFractalFindMe(fractalType, amount);
		watcherFindMeWindow.IsVisible = true;
		Add(watcherFindMeWindow);
	}

	public void OnCollectGoldenFractal(int fractalCount)
	{
		OnCollectGoldenFractal(fractalCount, false);
	}

	public void OnCollectGoldenFractal(int fractalCount, bool startup)
	{
		if (fractalCount >= 0)
		{
			SHSGoldenFractalActivity sHSGoldenFractalActivity = AppShell.Instance.ActivityManager.GetActivity("goldenfractalactivity") as SHSGoldenFractalActivity;
			NotificationHUD.addNotification(new DailyGoldenFractalNotificationData(sHSGoldenFractalActivity.GetFractalCollectionCount(), 1));
		}
	}

	public void OnCollectSeasonalActivityObject(int fractalCount)
	{
		OnCollectSeasonalActivityObject(fractalCount, false);
	}

	public void OnCollectSeasonalActivityObject(int fractalCount, bool startup)
	{
		if (fractalCount >= 0)
		{
			SHSSeasonalActivity sHSSeasonalActivity = AppShell.Instance.ActivityManager.GetActivity("seasonalactivity") as SHSSeasonalActivity;
			NotificationHUD.addNotification(new DailySeasonalNotificationData(sHSSeasonalActivity.GetCollectionCount(), sHSSeasonalActivity.Max, sHSSeasonalActivity.Icon));
		}
	}

	public void OnDisplaySeasonalBalance(int count)
	{
	}

	public void OnFirstSeasonalCollected(int amount)
	{
	}

	public void OnCollectRareSeasonal(int count)
	{
		OnCollectRareSeasonal(count, false);
	}

	public void OnCollectRareSeasonal(int count, bool startup)
	{
		if (count >= 0)
		{
			SHSRareSeasonalActivity sHSRareSeasonalActivity = AppShell.Instance.ActivityManager.GetActivity("rareseasonalactivity") as SHSRareSeasonalActivity;
			NotificationHUD.addNotification(new DailyRareSeasonalNotificationData(sHSRareSeasonalActivity.GetCollectionCount(), 1, sHSRareSeasonalActivity.Icon));
		}
	}

	public void OnFirstCraftCollected()
	{
	}

	public void OnActivateWheresImpossibleMan(int count, int max)
	{
		OnActivateWheresImpossibleMan(count, max, false);
	}

	public void OnActivateWheresImpossibleMan(int count, int max, bool startup)
	{
		if (count >= 0)
		{
			NotificationHUD.addNotification(new DailyWheresImpossibleManNotificationData(count, max));
		}
	}

	public void OnWorldEventWon(string counterName)
	{
		string counterType = "Fractals." + counterName;
		ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter(counterType);
		int num = (int)counter.GetCurrentValue();
		if (num != 0)
		{
			OnWorldEventWon(counterName, num);
		}
	}

	public void OnWorldEventWon(string counterName, int rewardID)
	{
		string counterType = "Fractals." + counterName;
		ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter(counterType);
		worldEventWonWindow = new SHSWorldEventWinnerWindow(rewardID, counterName == "WinnerTopScore");
		worldEventWonWindow.IsVisible = true;
		Add(worldEventWonWindow);
		counter.SetCounter(0L);
	}

	public void ShowTutorialVideo(string url)
	{
		if (TutorialVideoWindow.alreadyPlaying)
		{
			CspUtils.DebugLog("there is already a video playing, cannot spawn another");
			return;
		}
		TutorialVideoWindow tutorialVideoWindow = new TutorialVideoWindow(url);
		tutorialVideoWindow.IsVisible = true;
		Add(tutorialVideoWindow);
	}

	public void ShowPromoImage()
	{
		if (PromoImageWindow.instance == null && AppShell.Instance != null && AppShell.Instance.promoImage != null)
		{
			PromoImageWindow promoImageWindow = new PromoImageWindow(AppShell.Instance.promoImage);
			promoImageWindow.IsVisible = true;
			Add(promoImageWindow);
		}
	}

	public void OnHeroUnlocked(int heroID)
	{
		heroUnlockedWindow = new SHSHeroUnlockedWindow(heroID);
		heroUnlockedWindow.IsVisible = true;
		Add(heroUnlockedWindow);
	}

	public void ToggleEmoteKeys()
	{
		if (!emotesOn)
		{
			AddEmoteKeys();
			emotesOn = true;
			CspUtils.DebugLog("Emote shortcut keys enabled.");
		}
		else
		{
			RemoveEmoteKeys();
			emotesOn = false;
			CspUtils.DebugLog("Emote shortcut keys disabled.");
		}
	}

	public void AddEmoteKeys()
	{
		if (EmotesDefinition.Instance == null)
		{
			CspUtils.DebugLog("Trying to configure emote keys when EmotesDefinition.Instance is null!");
			return;
		}
		emoteDict = new Dictionary<KeyCodeEntry, sbyte>();
		foreach (EmotesDefinition.EmoteDefinition item in EmotesDefinition.Instance)
		{
			KeyCodeEntry keyCodeEntry = item.keyCodeEntry;
			if (keyCodeEntry.KeyCode != 0)
			{
				if (!emoteDict.ContainsKey(item.keyCodeEntry))
				{
					emoteDict.Add(item.keyCodeEntry, item.id);
					KeyCodeEntry keyCodeEntry2 = item.keyCodeEntry;
					if (keyCodeEntry2.Shift)
					{
						keyBanks[KeyInputState.Active].AddKey(item.keyCodeEntry, ShiftEmoteKeyListener);
					}
					else
					{
						KeyCodeEntry keyCodeEntry3 = item.keyCodeEntry;
						if (keyCodeEntry3.Alt)
						{
							keyBanks[KeyInputState.Active].AddKey(item.keyCodeEntry, AltEmoteKeyListener);
						}
						else
						{
							keyBanks[KeyInputState.Active].AddKey(item.keyCodeEntry, EmoteKeyListener);
						}
					}
				}
				else
				{
					SHSDebug.DebugLogger log = CspUtils.DebugLog;
					object[] obj = new object[5]
					{
						"Trying to add duplicate emote key=<",
						null,
						null,
						null,
						null
					};
					KeyCodeEntry keyCodeEntry4 = item.keyCodeEntry;
					obj[1] = keyCodeEntry4.KeyCode;
					obj[2] = ">, command=<";
					obj[3] = item.command;
					obj[4] = ">";
					log(string.Concat(obj));
				}
			}
		}
	}

	public void RemoveEmoteKeys()
	{
		if (emotesOn)
		{
			foreach (KeyCodeEntry key in emoteDict.Keys)
			{
				keyBanks[KeyInputState.Active].RemoveKey(key);
			}
			emoteDict.Clear();
		}
	}

	protected void AltEmoteKeyListener(SHSKeyCode code)
	{
		foreach (KeyValuePair<KeyCodeEntry, sbyte> item in emoteDict)
		{
			KeyCodeEntry key = item.Key;
			if (key.KeyCode == code.code && !key.Shift && key.Alt)
			{
				DoEmote(item.Value);
				break;
			}
		}
	}

	protected void ShiftEmoteKeyListener(SHSKeyCode code)
	{
		foreach (KeyValuePair<KeyCodeEntry, sbyte> item in emoteDict)
		{
			KeyCodeEntry key = item.Key;
			if (key.KeyCode == code.code && key.Shift && !key.Alt)
			{
				DoEmote(item.Value);
				break;
			}
		}
	}

	protected void EmoteKeyListener(SHSKeyCode code)
	{
		foreach (KeyValuePair<KeyCodeEntry, sbyte> item in emoteDict)
		{
			KeyCodeEntry key = item.Key;
			if (key.KeyCode == code.code && !key.Shift && !key.Alt)
			{
				DoEmote(item.Value);
				break;
			}
		}
	}

	protected override void InitializeBundleList()
	{
		base.InitializeBundleList();
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("gameworld_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("items_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
	}
}
