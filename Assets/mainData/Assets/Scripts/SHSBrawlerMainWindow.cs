using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SHSBrawlerMainWindow : GUITopLevelWindow
{
	System.Security.Cryptography.MD5CryptoServiceProvider md = null;
        
	private class DangerRedFlare : GUISimpleControlWindow
	{
		private enum AnimationState
		{
			FirstIncrease,
			Oscillating,
			GoingToZero,
			Rest
		}

		private const float GLOW_WIDTH = 69f;

		private SHSBrawlerPlayerBar healthLoc;

		private AnimationState currentAnimationState;

		private float currentAlpha;

		private float currentScale;

		private List<GUIImage> redFlares;

		private float lastAlpha;

		private float targetAlpha;

		private AnimClip animationPiece;

		private GUIImage dummyAnimator;

		public DangerRedFlare(SHSBrawlerPlayerBar healthLoc)
		{
			Traits.ResourceLoadingPhaseTrait = ControlTraits.ResourceLoadingPhaseTraitEnum.Active;
			this.healthLoc = healthLoc;
		}

		public override bool InitializeResources(bool reload)
		{
			SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
			SetPosition(0f, 0f);
			currentAnimationState = AnimationState.Rest;
			SetControlFlag(ControlFlagSetting.HitTestIgnore, true, false);
			redFlares = new List<GUIImage>();
			GUIImage gUIImage = new GUIImage();
			gUIImage.SetPositionAndSize(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, Vector2.zero, new Vector2(1f, 69f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Absolute);
			gUIImage.TextureSource = "brawler_bundle|mshs_brawler_hud_damage_glow_top";
			redFlares.Add(gUIImage);
			GUIImage gUIImage2 = new GUIImage();
			gUIImage2.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, Vector2.zero, new Vector2(1f, 69f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Absolute);
			gUIImage2.TextureSource = "brawler_bundle|mshs_brawler_hud_damage_glow_bottom";
			redFlares.Add(gUIImage2);
			GUIImage gUIImage3 = new GUIImage();
			gUIImage3.SetPositionAndSize(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, Vector2.zero, new Vector2(69f, 1f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Percentage);
			gUIImage3.TextureSource = "brawler_bundle|mshs_brawler_hud_damage_glow_right";
			redFlares.Add(gUIImage3);
			GUIImage gUIImage4 = new GUIImage();
			gUIImage4.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, Vector2.zero, new Vector2(69f, 1f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Percentage);
			gUIImage4.TextureSource = "brawler_bundle|mshs_brawler_hud_damage_glow_left";
			redFlares.Add(gUIImage4);
			foreach (GUIImage redFlare in redFlares)
			{
				Add(redFlare);
			}
			dummyAnimator = new GUIImage();
			dummyAnimator.Alpha = 0f;
			return base.InitializeResources(reload);
		}

		public override void OnUpdate()
		{
			updateState();
			updateCurrentAlphaAndScale();
			setTheAlphaAndScaleOfTheDangerRedFlare();
			base.OnUpdate();
		}

		public void Deactivate()
		{
			currentAlpha = 0f;
			currentAnimationState = AnimationState.Rest;
		}

		private void setTheAlphaAndScaleOfTheDangerRedFlare()
		{
			foreach (GUIImage redFlare in redFlares)
			{
				redFlare.Alpha = currentAlpha;
				if (redFlare.HorizontalSizeHint == AutoSizeTypeEnum.Percentage)
				{
					redFlare.SetSize(new Vector2(1f, 69f * currentScale), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Absolute);
				}
				else
				{
					redFlare.SetSize(new Vector2(69f * currentScale, 1f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Percentage);
				}
			}
		}

		private void updateCurrentAlphaAndScale()
		{
			switch (currentAnimationState)
			{
			case AnimationState.Rest:
				currentAlpha = 0f;
				currentScale = 1f;
				break;
			case AnimationState.FirstIncrease:
				currentAlpha = targetAlpha;
				currentScale = 1f + targetAlpha * 0.2f;
				break;
			case AnimationState.Oscillating:
				currentAlpha = 0.45f + 0.4f * targetAlpha;
				currentScale = 1f + targetAlpha * 0.2f;
				break;
			case AnimationState.GoingToZero:
				currentAlpha = Mathf.Max(currentAlpha - 1f * Time.deltaTime, 0f);
				currentScale = 1f;
				break;
			}
		}

		private void updateState()
		{
			if (healthLoc.healthBar.GetPercentage() < 0.2f)
			{
				if (currentAnimationState != 0 && currentAnimationState != AnimationState.Oscillating)
				{
					currentAnimationState = AnimationState.FirstIncrease;
				}
				if (animationPiece == null || animationPiece.Done)
				{
					animationPiece = AnimClipBuilder.Absolute.Alpha(0.5f - AnimClipBuilder.Path.Cos(0f, 1f, 1f) * 0.5f, dummyAnimator);
					base.AnimationPieceManager.Add(animationPiece);
				}
			}
			else if (currentAnimationState != AnimationState.Rest && currentAnimationState != AnimationState.GoingToZero)
			{
				currentAnimationState = AnimationState.GoingToZero;
			}
			targetAlpha = dummyAnimator.Alpha;
			if (currentAnimationState == AnimationState.FirstIncrease && targetAlpha < lastAlpha)
			{
				currentAnimationState = AnimationState.Oscillating;
			}
			if (currentAnimationState == AnimationState.GoingToZero && currentAlpha == 0f)
			{
				currentAnimationState = AnimationState.Rest;
			}
			lastAlpha = targetAlpha;
		}
	}

	public class PlayerInfoGroup
	{
		public SHSBrawlerPlayerBar healthBar;

		public SHSBrawlerBuffPanel buffPanel;

		public bool reserved;

		public PlayerInfoGroup()
		{
		}

		public PlayerInfoGroup(SHSBrawlerPlayerBar HealthBar, SHSBrawlerBuffPanel BuffPanel)
		{
			healthBar = HealthBar;
			buffPanel = BuffPanel;
		}
	}

	private const float LOCAL_HEALTHBAR_SIZE = 1f;

	private const float MULTIPLAYER_HEALTHBAR_SIZE = 0.63f;

	private const int BRAWLER_FRIEND_MAX = 3;

	private PlayerInfoGroup[] playerInfoBoxes;

	private SHSBrawlerBossBarWindow bossControl;

	private GUISimpleControlWindow _globalNavAnchor;

	private GlobalNav _globalNav;

	private DangerRedFlare playerWarning;

	protected bool PowerMoveEnabled;

	private SHSBrawlerSuperButton PowerMoveButton;

	public SHSBrawlerCompleteWindow brawlerCompletePanel;

	private SHSBrawlerStageCompleteWindow brawlerStageCompletePanel;

	private SHSBrawlerObjectiveWindow ordersControl;

	private SHSBrawlerIndicatorArrow indicatorArrow;

	private SHSBrawlerEnemyBar healthBar;

	private SHSBrawlerScoreWindow scoreWindow;

	private SHSBrawlerComboWindow comboWindow;

	private SHSBrawlerStatusAlertWindow statusAlert;

	private bool suppressPowerStateChange;

	private float lastPowerPercent = -1f;

	private bool ordersCanShow;

	private BrawlerAttackSelector attackSelector;

	private BrawlerSpecialAttackHUD specialAttackSelector;

	private NotificationHUD notificationHUD;

	private Dictionary<KeyCodeEntry, sbyte> emoteDict;

	[CompilerGenerated]
	private bool _003CShowScoreWindow_003Ek__BackingField;

	public bool ShowScoreWindow
	{
		[CompilerGenerated]
		get
		{
			return _003CShowScoreWindow_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CShowScoreWindow_003Ek__BackingField = value;
		}
	}

	public SHSBrawlerMainWindow()
		: base("SHSBrawlerMainWindow")
	{
		BrawlerStatManager.Instanciate(this);
	}

	public override void OnShow()
	{
		playerWarning.Deactivate();
		PlayerInfoGroup[] array = playerInfoBoxes;
		foreach (PlayerInfoGroup playerInfoGroup in array)
		{
			playerInfoGroup.healthBar.RestartControl();
		}
		bossControl.RestartControl();
		base.OnShow();
	}

	public override bool InitializeResources(bool reload)
	{
		HitTestType = HitTestTypeEnum.Transparent;
		playerInfoBoxes = new PlayerInfoGroup[4];
		bossControl = new SHSBrawlerBossBarWindow();
		bossControl.SetPosition(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, Vector2.zero);
		bossControl.SetSize(1000f, 292f);
		bossControl.HitTestType = HitTestTypeEnum.Transparent;
		Add(bossControl);
		for (int num = 2; num >= 0; num--)
		{
			SHSBrawlerPlayerBar sHSBrawlerPlayerBar = new SHSBrawlerPlayerBar(0.63f, false);
			sHSBrawlerPlayerBar.SetPositionAndSize(340 + num * 200, 8f, 277f, 101f);
			sHSBrawlerPlayerBar.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			sHSBrawlerPlayerBar.IsVisible = false;
			SHSBrawlerBuffPanel sHSBrawlerBuffPanel = new SHSBrawlerBuffPanel(0.63f);
			sHSBrawlerBuffPanel.SetPositionAndSize(340 + num * 200, 8f, 416f, 218f);
			sHSBrawlerBuffPanel.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			sHSBrawlerBuffPanel.IsVisible = false;
			sHSBrawlerBuffPanel.IsEnabled = true;
			Add(sHSBrawlerPlayerBar);
			Add(sHSBrawlerBuffPanel);
			playerInfoBoxes[num + 1] = new PlayerInfoGroup(sHSBrawlerPlayerBar, sHSBrawlerBuffPanel);
		}
		SHSBrawlerPlayerBar sHSBrawlerPlayerBar2 = new SHSBrawlerPlayerBar(1f, true);
		sHSBrawlerPlayerBar2.SetPositionAndSize(0f, 0f, 416f, 218f);
		sHSBrawlerPlayerBar2.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		sHSBrawlerPlayerBar2.IsVisible = false;
		playerWarning = new DangerRedFlare(sHSBrawlerPlayerBar2);
		Add(playerWarning, DrawOrder.DrawFirst, DrawPhaseHintEnum.PreDraw);
		Add(sHSBrawlerPlayerBar2);
		SHSBrawlerBuffPanel sHSBrawlerBuffPanel2 = new SHSBrawlerBuffPanel(1f);
		sHSBrawlerBuffPanel2.SetPositionAndSize(0f, 0f, 416f, 218f);
		sHSBrawlerBuffPanel2.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		sHSBrawlerBuffPanel2.IsVisible = false;
		sHSBrawlerBuffPanel2.IsEnabled = true;
		Add(sHSBrawlerBuffPanel2);
		playerInfoBoxes[0] = new PlayerInfoGroup(sHSBrawlerPlayerBar2, sHSBrawlerBuffPanel2);
		scoreWindow = new SHSBrawlerScoreWindow();
		comboWindow = new SHSBrawlerComboWindow();
		ShowScoreWindow = true;
		UpdateScorePosition();
		scoreWindow.IsVisible = false;
		comboWindow.IsVisible = false;
		Add(scoreWindow);
		Add(comboWindow);
		ordersControl = new SHSBrawlerObjectiveWindow();
		ordersControl.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		ordersControl.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 164f));
		ordersControl.IsEnabled = true;
		ordersControl.IsVisible = false;
		Add(ordersControl);
		brawlerCompletePanel = new SHSBrawlerCompleteWindow();
		brawlerCompletePanel.Traits.ActivationTrait = ControlTraits.ActivationTraitEnum.Auto;
		brawlerCompletePanel.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		brawlerCompletePanel.Hide();
		brawlerCompletePanel.SetPositionAndSize(QuickSizingHint.ParentSize);
		Add(brawlerCompletePanel);
		Add(brawlerCompletePanel.levelUpPlate);
		brawlerStageCompletePanel = GUIControl.CreateControlFrameCentered<SHSBrawlerStageCompleteWindow>(new Vector2(512f, 512f), new Vector2(0f, -148f));
		brawlerStageCompletePanel.Traits.ActivationTrait = ControlTraits.ActivationTraitEnum.Auto;
		Add(brawlerStageCompletePanel);
		indicatorArrow = new SHSBrawlerIndicatorArrow();
		Add(indicatorArrow);
		healthBar = new SHSBrawlerEnemyBar();
		healthBar.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		healthBar.IsVisible = false;
		healthBar.IsEnabled = false;
		Add(healthBar, DrawOrder.DrawLast);
		PowerMoveButton = GUIControl.CreateControlBottomLeftFrameCentered<SHSBrawlerSuperButton>(new Vector2(256f, 256f), new Vector2(135f, -156f));
		PowerMoveButton.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		PowerMoveButton.Traits.HitTestType = HitTestTypeEnum.Transparent;
		PowerMoveButton.IsVisible = false;
		Add(PowerMoveButton);
		statusAlert = new SHSBrawlerStatusAlertWindow();
		statusAlert.SetPositionAndSize(QuickSizingHint.ParentSize);
		statusAlert.IsVisible = true;
		statusAlert.IsEnabled = true;
		Add(statusAlert);
		attackSelector = GUIControl.CreateControlTopLeftFrame<BrawlerAttackSelector>(new Vector2(211f, 60f), new Vector2(170f, 106f));
		attackSelector.IsVisible = true;
		Add(attackSelector);
		specialAttackSelector = GUIControl.CreateControlTopLeftFrame<BrawlerSpecialAttackHUD>(new Vector2(60f, 311f), new Vector2(5f, 136f));
		specialAttackSelector.IsVisible = true;
		Add(specialAttackSelector);
		notificationHUD = GUIControl.CreateControlTopLeftFrame<NotificationHUD>(new Vector2(1500f, 1000f), new Vector2(2f, 2f));
		notificationHUD.IsVisible = true;
		Add(notificationHUD);
		notificationHUD.Init();
		_globalNav = new GlobalNav(true);
		_globalNav.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
		_globalNav.hideCurrency();
		_globalNavAnchor = GUIControl.CreateControlTopRightFrame<GUISimpleControlWindow>(_globalNav.Size, new Vector2(0f, 0f));
		Add(_globalNavAnchor);
		_globalNavAnchor.IsVisible = true;
		_globalNavAnchor.Add(_globalNav);
		AppShell.Instance.EventMgr.Fire(this, new BrawlerMainWindowInitializedMessage(this));
		return base.InitializeResources(reload);
	}

	protected override void InitializeBundleList()
	{
		base.InitializeBundleList();
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("brawler_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
	}

	public void SetDisplayOrders(string newOrders, string newObjectiveIcon, float duration)
	{
		if (ordersCanShow)
		{
			ordersControl.DisplayOrders(newOrders, newObjectiveIcon, duration);
		}
	}

	public void HideOrders()
	{
		ordersControl.IsVisible = false;
	}

	public void OrdersCanShow(bool canShow)
	{
		ordersCanShow = canShow;
	}

	public override void OnActive()
	{
		base.OnActive();
		UserProfile profile = AppShell.Instance.Profile;
		BrawlerStatManager.instance.Ready();
		AppShell.Instance.EventMgr.AddListener<GUIResizeMessage>(OnResize);
		AppShell.Instance.EventMgr.AddListener<AirlockTimerMessage>(OnLevelStart);
		if (profile == null)
		{
			CspUtils.DebugLog("No profile. Offline?Controller is:" + GameController.GetController());
		}
		else
		{
			AddEmoteKeys();
		}
	}

	public override void OnInactive()
	{
		base.OnInactive();
		BrawlerStatManager.instance.Unready();
		AppShell.Instance.EventMgr.RemoveListener<GUIResizeMessage>(OnResize);
		AppShell.Instance.EventMgr.RemoveListener<AirlockTimerMessage>(OnLevelStart);
		for (int i = 0; i < 4; i++)
		{
			ReturnInfoPanel(i);
		}
		RemoveEmoteKeys();
	}

	public int GetInfoPanel(int multiplayerID, string characterName, string squadName, float health, float power, GameObject owner)
	{
		int num = -1;
		if (multiplayerID >= -1)
		{
			if (multiplayerID == -1)
			{
				num = 0;
			}
			else
			{
				for (int i = 0; i < 3; i++)
				{
					if (!playerInfoBoxes[i + 1].reserved)
					{
						num = i + 1;
						break;
					}
				}
			}
		}
		if (num >= 0)
		{
			playerInfoBoxes[num].healthBar.SetCharacterName(characterName, squadName);
			playerInfoBoxes[num].healthBar.IsVisible = PowerMoveEnabled;
			playerInfoBoxes[num].healthBar.UpdateHealth(health);
			playerInfoBoxes[num].healthBar.UpdatePower(power);
			playerInfoBoxes[num].buffPanel.IsVisible = PowerMoveEnabled;
			playerInfoBoxes[num].buffPanel.ClearBuffIcons();
			playerInfoBoxes[num].reserved = true;
			scoreWindow.GetScoreTargets(BrawlerStatManager.instance.GetScoredPlayerCount());
		}
		UpdateScorePosition();
		return num;
	}

	public bool ValidPanelIndex(int panelIndex)
	{
		return panelIndex >= 0 && panelIndex < 4;
	}

	public void ReturnInfoPanel(int panelIndex)
	{
		if (ValidPanelIndex(panelIndex))
		{
			playerInfoBoxes[panelIndex].healthBar.IsVisible = false;
			playerInfoBoxes[panelIndex].buffPanel.IsVisible = false;
			playerInfoBoxes[panelIndex].reserved = false;
			UpdateScorePosition();
		}
	}

	public void UpdatePlayerEmotion(int panelIndex, int happiness)
	{
		if (ValidPanelIndex(panelIndex))
		{
			playerInfoBoxes[panelIndex].healthBar.ChangeHappiness(happiness);
		}
	}

	public int ReportBuffAdded(int panelIndex, string iconPath, string buffName, string alertTexture)
	{
		int num = -1;
		if (ValidPanelIndex(panelIndex))
		{
			num = playerInfoBoxes[panelIndex].buffPanel.AddBuff(iconPath, buffName);
			playerInfoBoxes[panelIndex].buffPanel.UpdateBuffIcons();
			if (panelIndex == 0 && !string.IsNullOrEmpty(alertTexture))
			{
				statusAlert.PushStatusAlert(num, alertTexture);
			}
		}
		return num;
	}

	public void ReportBuffRemoved(int panelIndex, int buffID)
	{
		if (ValidPanelIndex(panelIndex))
		{
			playerInfoBoxes[panelIndex].buffPanel.RemoveBuff(buffID);
			playerInfoBoxes[panelIndex].buffPanel.UpdateBuffIcons();
			if (panelIndex == 0)
			{
				statusAlert.RemoveStatusAlert(buffID);
			}
		}
	}

	public void ReportBuffRemovedAll()
	{
		statusAlert.ClearStatusAlerts();
	}

	public void ReportBuffCountdown(int buffID)
	{
		statusAlert.CountdownStatusAlert(buffID);
	}

	public void ReportHealthChange(int panelIndex, float percent)
	{
		if (ValidPanelIndex(panelIndex))
		{
			playerInfoBoxes[panelIndex].healthBar.UpdateHealth(percent);
		}
	}

	public void ReportPowerChange(int panelIndex, float percent)
	{
		if (ValidPanelIndex(panelIndex))
		{
			playerInfoBoxes[panelIndex].healthBar.UpdatePower(percent);
			if (panelIndex == 0)
			{
				ReportPowerStateChange(percent);
			}
		}
	}

	public void ReportPowerRefund()
	{
		statusAlert.SetPowerState(true);
	}

	public void ReportPowerMoveLevel(int panelIndex, float powerCost)
	{
		if (ValidPanelIndex(panelIndex))
		{
			playerInfoBoxes[panelIndex].healthBar.UpdatePowerMoveLevel(powerCost);
		}
	}

	public void SetNetworkDisconnect(int panelIndex, bool connected)
	{
		if ((panelIndex != 0 && playerInfoBoxes[0].healthBar.GetNetworkDisconnect()) || !ValidPanelIndex(panelIndex))
		{
			return;
		}
		playerInfoBoxes[panelIndex].healthBar.UpdateNetworkDisconnect(connected);
		if (panelIndex == 0 && !connected)
		{
			for (int i = 1; i < playerInfoBoxes.Length; i++)
			{
				playerInfoBoxes[i].healthBar.UpdateNetworkDisconnect(true);
			}
		}
	}

	public void ReportPowerStateChange(float percent)
	{
		if (suppressPowerStateChange)
		{
			lastPowerPercent = percent;
			return;
		}
		bool flag = percent == 1f;
		PowerMoveButton.ChangePowerState(flag);
		statusAlert.ChangePowerState(flag);
		if (BrawlerController.Instance != null && BrawlerController.Instance.BrawlerHud != null)
		{
			BrawlerController.Instance.BrawlerHud.ShowHeroUpEffect(flag);
		}
	}

	public void SetPowerBarVisibility(bool visible)
	{
		if (!resourcesInitialized)
		{
			InitializeResources(false);
		}
		if (!visible)
		{
			playerWarning.Deactivate();
		}
		playerWarning.IsVisible = visible;
		scoreWindow.IsVisible = (visible && ShowScoreWindow);
		comboWindow.IsVisible = visible;
		bossControl.IsVisible = (visible && bossControl.BossActive);
		PowerMoveEnabled = visible;
		PowerMoveButton.IsVisible = visible;
		statusAlert.IsVisible = visible;
		if (attackSelector != null)
		{
			attackSelector.IsVisible = visible;
		}
		if (specialAttackSelector != null)
		{
			specialAttackSelector.IsVisible = visible;
		}
		PlayerInfoGroup[] array = playerInfoBoxes;
		foreach (PlayerInfoGroup playerInfoGroup in array)
		{
			playerInfoGroup.healthBar.IsVisible = (visible && playerInfoGroup.reserved);
			playerInfoGroup.buffPanel.IsVisible = (visible && playerInfoGroup.reserved);
		}
	}

	public void EnablePowerButton(bool enable)
	{
		PowerMoveButton.EnableButton(enable);
	}

	public void SuppressPowerStateChange(bool suppress)
	{
		suppressPowerStateChange = suppress;
		if (!suppress && lastPowerPercent >= 0f)
		{
			ReportPowerStateChange(lastPowerPercent);
			lastPowerPercent = -1f;
		}
		if (statusAlert != null)
		{
			statusAlert.SuppressPowerState(suppress);
		}
	}

	public void BossInactive()
	{
		bossControl.BossActive = false;
	}

	public void UpdateScorePosition()
	{
		bool flag = true;
		if (playerInfoBoxes[3].reserved && Screen.width < 1400)
		{
			flag = false;
		}
		if (flag)
		{
			scoreWindow.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, new Vector2(0f, 56f));
			comboWindow.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, new Vector2(0f, 214f));
		}
		else
		{
			scoreWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 148f));
			comboWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 318f));
		}
	}

	private void OnResize(GUIResizeMessage msg)
	{
		UpdateScorePosition();
	}

	private void OnLevelStart(AirlockTimerMessage message)
	{
		CspUtils.DebugLog("OnLevelStart");
		attackSelector.AddAttacks();
		specialAttackSelector.Init();
	}

	public void AttachHealthBar(GameObject obj)
	{
		healthBar.AttachHealthBar(obj);
	}

	public void DetachHealthBar()
	{
		healthBar.DetachHealthBar();
	}

	public void TransferHealthBar(GameObject owner, GameObject obj)
	{
		healthBar.TransferHealthBar(owner, obj);
	}

	public void TeamScoreChanged(int newScore)
	{
		scoreWindow.UpdateScore(newScore, false);
	}

	public void GetComboUpdate(float newComboHeat)
	{
		comboWindow.UpdateCombo(newComboHeat);
	}

	public void AddEmoteKeys()
	{
		if (EmotesDefinition.Instance == null)
		{
			CspUtils.DebugLog("Missions: Trying to configure emote keys when EmotesDefinition.Instance is null!");
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
						"Missions: Trying to add duplicate emote key=<",
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
		foreach (KeyCodeEntry key in emoteDict.Keys)
		{
			keyBanks[KeyInputState.Active].RemoveKey(key);
		}
		emoteDict.Clear();
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

	protected void DoEmote(sbyte emote)
	{
		if (BrawlerController.Instance == null)
		{
			CspUtils.DebugLog("No Missions controller");
			return;
		}
		GameObject localPlayer = BrawlerController.Instance.LocalPlayer;
		AppShell.Instance.EventMgr.Fire(localPlayer, new EmoteMessage(localPlayer, emote));
	}
}
