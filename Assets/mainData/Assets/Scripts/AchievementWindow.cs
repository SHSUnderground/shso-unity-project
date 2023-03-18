using UnityEngine;

public class AchievementWindow : GUIDialogWindow
{
	private int targetPlayerID = -1;

	private int targetAchievementID = -1;

	private AchievementWindowNavPanel _navPanel;

	private AchievementWindowContentPanel _contentPanel;

	private AchievementWindowOverviewPanel _overviewPanel;

	private GUIStrokeTextLabel _points;

	private GUIButton upButton;

	private int _currentDisplayGroupID = -10;

	public AchievementWindow(int targetPlayerID, int targetAchievementID = -1)
	{
		this.targetPlayerID = targetPlayerID;
		this.targetAchievementID = targetAchievementID;
		if (!AchievementManager.allPlayerData.ContainsKey(targetPlayerID))
		{
			CspUtils.DebugLog("  player data not loaded yet, asking for it");
			AppShell.Instance.EventMgr.AddListener<AchievementDataLoadedMessage>(OnAchievementDataLoaded);
			AppShell.Instance.ServerConnection.LoadAchievementData(targetPlayerID);
		}
		else
		{
			init(AchievementManager.allPlayerData[targetPlayerID]);
		}
		AppShell.Instance.EventMgr.AddListener<AchievementGroupSelectedMessage>(OnAchievementGroupSelectedMessage);
		AppShell.Instance.EventMgr.AddListener<CloseAchievementWindowRequest>(OnCloseAchievementWindowRequest);
	}

	private void OnCloseAchievementWindowRequest(CloseAchievementWindowRequest msg)
	{
		Close();
	}

	private void OnAchievementGroupSelectedMessage(AchievementGroupSelectedMessage msg)
	{
		_currentDisplayGroupID = msg.groupID;
		if (msg.groupID <= 0 || AchievementManager.allGroups[msg.groupID].parentGroupID == 0)
		{
			AchievementWindowOverviewPanel overviewPanel = _overviewPanel;
			Vector2 position = _overviewPanel.Position;
			overviewPanel.SetPosition(position.x, 96f);
			_overviewPanel.IsVisible = true;
			_overviewPanel.displayGroup(msg.groupID);
			_contentPanel.IsVisible = false;
			if (msg.groupID <= 0)
			{
				upButton.IsVisible = false;
			}
			else
			{
				upButton.IsVisible = true;
			}
		}
		else
		{
			upButton.IsVisible = true;
			_overviewPanel.IsVisible = false;
			_contentPanel.IsVisible = true;
		}
	}

	private void OnAchievementDataLoaded(AchievementDataLoadedMessage msg)
	{
		CspUtils.DebugLog("OnAchievementDataLoaded for player " + msg.data.playerID);
		if (msg.data.playerID != targetPlayerID)
		{
			CspUtils.DebugLog("ignored because we want player ID " + targetPlayerID);
			return;
		}
		AppShell.Instance.EventMgr.RemoveListener<AchievementDataLoadedMessage>(OnAchievementDataLoaded);
		init(msg.data);
	}

	private void init(PlayerAchievementData data)
	{
		Traits.FullScreenOpaqueBackgroundTrait = ControlTraits.FullScreenOpaqueBackgroundTraitEnum.HasFullScreenOpaqueBackground;
		Traits.EventListenerRegistrationTrait = ControlTraits.EventListenerRegistrationTraitEnum.Ignore;
		SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		SetPosition(QuickSizingHint.Centered);
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetSize(new Vector2(1020f, 644f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage.Position = Vector2.zero;
		gUIImage.TextureSource = "achievement_bundle|achievement_bg";
		Add(gUIImage);
		NewAchievement initialAchievement = null;
		if (targetAchievementID != -1)
		{
			initialAchievement = AchievementManager.allAchievements[targetAchievementID];
		}
		_navPanel = new AchievementWindowNavPanel(targetPlayerID, new Vector2(240f, 397f), initialAchievement);
		Add(_navPanel);
		_navPanel.SetPosition(65f, 159f);
		_navPanel.IsVisible = true;
		_contentPanel = new AchievementWindowContentPanel(data, initialAchievement);
		Add(_contentPanel);
		_contentPanel.SetPosition(341f, 96f);
		_contentPanel.IsVisible = false;
		_overviewPanel = new AchievementWindowOverviewPanel(data);
		Add(_overviewPanel);
		_overviewPanel.SetPosition(341f, 96f);
		_overviewPanel.displayGroup(0);
		if (targetAchievementID == -1)
		{
			_overviewPanel.IsVisible = true;
		}
		else
		{
			AchievementWindowOverviewPanel overviewPanel = _overviewPanel;
			Vector2 position = _overviewPanel.Position;
			overviewPanel.SetPosition(position.x, 1000f);
			_overviewPanel.IsVisible = false;
		}
		GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(48f, 48f), new Vector2(945f, 16f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|button_mysquad_close");
		gUIButton.Click += delegate
		{
			Close();
		};
		gUIButton.ToolTip = new NamedToolTipInfo("#TT_COMMON_1");
		Add(gUIButton);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		Vector2 size = gUIImage.Size;
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(size.x / 2f - 300f, 33f), new Vector2(600f, 100f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 48, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 43, 68), GUILabel.GenColor(0, 43, 68), new Vector2(3f, 4f), TextAnchor.UpperCenter);
		gUIStrokeTextLabel.BackColorAlpha = 1f;
		gUIStrokeTextLabel.StrokeColorAlpha = 1f;
		gUIStrokeTextLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel.Text = "#ACH_WINDOW_TITLE";
		gUIStrokeTextLabel.IsVisible = true;
		Add(gUIStrokeTextLabel);
		_points = new GUIStrokeTextLabel();
		_points.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(743f, 50f), new Vector2(100f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_points.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 24, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 43, 68), GUILabel.GenColor(0, 43, 68), new Vector2(2f, 2f), TextAnchor.UpperRight);
		_points.BackColorAlpha = 1f;
		_points.StrokeColorAlpha = 1f;
		_points.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		_points.Text = string.Format("{0:#,0}", data.achievementPoints);
		_points.IsVisible = true;
		Add(_points);
		GUIImage gUIImage2 = new GUIImage();
		gUIImage2.SetSize(new Vector2(29f, 31f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Vector2 position2 = _points.Position;
		Vector2 size2 = _points.Size;
		gUIImage2.Position = position2 + new Vector2(size2.x + 5f, 0f);
		gUIImage2.TextureSource = "achievement_bundle|achievement_coin_small";
		Add(gUIImage2);
		GUIButton gUIButton2 = new GUIButton();
		gUIButton2.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -1f), new Vector2(325f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton2.StyleInfo = new SHSButtonStyleInfo("achievement_bundle|close");
		gUIButton2.Click += delegate
		{
			Close();
		};
		gUIButton2.ToolTip = new NamedToolTipInfo("#TT_COMMON_1");
		Add(gUIButton2);
		AppShell.Instance.EventMgr.Fire(this, new CloseHudMessage());
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("global_window_in"));
		upButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(36f, 36f), new Vector2(929f, 66f));
		upButton.StyleInfo = new SHSButtonStyleInfo("achievement_bundle|mshs_button_reset_locked");
		upButton.Click += delegate
		{
			AchievementDisplayGroup achievementDisplayGroup = AchievementManager.allGroups[_currentDisplayGroupID];
			if (achievementDisplayGroup.parentGroupID <= 0)
			{
				AppShell.Instance.EventMgr.Fire(this, new AchievementGroupSelectedMessage(-10));
				upButton.ToolTip = new NamedToolTipInfo("#ACH_BACK_TO_OVERVIEW");
			}
			else
			{
				AppShell.Instance.EventMgr.Fire(this, new AchievementGroupSelectedMessage(achievementDisplayGroup.parentGroupID));
				upButton.ToolTip = new NamedToolTipInfo(achievementDisplayGroup.name);
			}
		};
		upButton.ToolTip = new NamedToolTipInfo("#ACH_BACK_TO_OVERVIEW");
		Add(upButton);
		upButton.IsVisible = false;
		AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "open_achievements", 1, string.Empty);
	}

	public void Close()
	{
		IsVisible = false;
		if (PlayerStatus.GetLocalStatus() == PlayerStatusDefinition.Instance.GetStatus("Achievements"))
		{
			PlayerStatus.ClearLocalStatus();
		}
		AppShell.Instance.AudioManager.RequestCrossfade(null);
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("global_window_out"));
		_contentPanel.Close();
		_overviewPanel.Close();
		AppShell.Instance.EventMgr.RemoveListener<CloseAchievementWindowRequest>(OnCloseAchievementWindowRequest);
		OnHide();
		Dispose();
	}
}
