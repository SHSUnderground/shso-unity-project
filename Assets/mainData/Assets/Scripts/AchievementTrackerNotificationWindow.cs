using System.Collections;
using UnityEngine;

public class AchievementTrackerNotificationWindow : NotificationWindow
{
	private GUIImage backgroundLeft;

	private GUIImage backgroundMiddle;

	private GUIImage backgroundRight;

	private GUIImage inviteIcon;

	private GUILabel achievementStatusLabel;

	private GUILabel summaryLabel;

	private GUILabel achievementTitleLabel;

	private GUIButton stopTrackButton;

	private GUIButton collapseButton;

	private GUIButton expandButton;

	private GUIButton detailsButton;

	private GUIButton helpButton;

	private NewAchievement _targetAchievement;

	private GUIHotSpotButton hotSpot;

	private GUIImage checkbox;

	private GUIImage checkboxCheck;

	private bool isCollapsed;

	private GUISimpleControlWindow _container;

	public AchievementTrackerNotificationWindow()
		: base(NotificationData.NotificationType.AchievementTracker)
	{
		Traits.HitTestType = HitTestTypeEnum.Rect;
		Traits.BlockTestType = BlockTestTypeEnum.Rect;
		SetControlFlag(ControlFlagSetting.HitTestIgnore, false, true);
		Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		Traits.UpdateTrait = ControlTraits.UpdateTraitEnum.AlwaysUpdate;
	}

	public override bool absorb(NotificationData data)
	{
		if (!(data is AchievementTrackerNotificationData))
		{
			return false;
		}
		AchievementTrackerNotificationData achievementTrackerNotificationData = data as AchievementTrackerNotificationData;
		if (achievementTrackerNotificationData.achievementID != _targetAchievement.achievementID)
		{
			return false;
		}
		updateStatus();
		return true;
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
		int achievementID = (data as AchievementTrackerNotificationData).achievementID;
		_targetAchievement = AchievementManager.allAchievements[achievementID];
		isCollapsed = (data as AchievementTrackerNotificationData).isCollapsed;
		_container = new GUISimpleControlWindow();
		_container.SetPosition(Vector2.zero);
		Add(_container);
		achievementTitleLabel = new GUILabel();
		achievementTitleLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
		achievementTitleLabel.SetPosition(new Vector2(80f, 25f));
		achievementTitleLabel.Id = "achievementTitleLabel";
		achievementTitleLabel.VerticalKerning = 12;
		achievementTitleLabel.Text = AchievementManager.formatAchievementString(_targetAchievement.displayName, 0, 0, string.Empty);
		achievementTitleLabel.VerticalKerning = 12;
		int num = 20;
		float textBlockSize = NotificationWindow.GetTextBlockSize(new GUILabel[1]
		{
			achievementTitleLabel
		}, BlockSizeType.Width);
		if (textBlockSize > 175f)
		{
			achievementTitleLabel.WordWrap = true;
			achievementTitleLabel.SetSize(new Vector2(175f, 40f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			num = 30;
		}
		else
		{
			achievementTitleLabel.SetSize(new Vector2(175f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		}
		textBlockSize = 175f;
		achievementStatusLabel = new GUILabel();
		achievementStatusLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
		GUILabel gUILabel = achievementStatusLabel;
		Vector2 position = achievementTitleLabel.Position;
		gUILabel.SetPosition(new Vector2(80f, position.y + (float)num));
		achievementStatusLabel.SetSize(new Vector2(textBlockSize, 70f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		achievementStatusLabel.Id = "achievementStatusLabel";
		achievementStatusLabel.WordWrap = true;
		backgroundLeft = new GUIImage();
		backgroundLeft.SetSize(new Vector2(108f, 142f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		backgroundLeft.Position = Vector2.zero;
		backgroundLeft.TextureSource = "GUI/Notifications/invitation_frame_left_blue_icon";
		backgroundLeft.Id = "BackgroundLeftImage";
		_container.Add(backgroundLeft);
		backgroundMiddle = new GUIImage();
		backgroundMiddle.SetSize(new Vector2(textBlockSize - 30f, 142f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage gUIImage = backgroundMiddle;
		Vector2 position2 = backgroundLeft.Position;
		float x = position2.x;
		Vector2 size = backgroundLeft.Size;
		float x2 = x + size.x;
		Vector2 position3 = backgroundLeft.Position;
		gUIImage.Position = new Vector2(x2, position3.y);
		backgroundMiddle.TextureSource = "GUI/Notifications/invitation_frame_center_blue";
		backgroundMiddle.Id = "BackgroundMiddleImage";
		_container.Add(backgroundMiddle);
		backgroundRight = new GUIImage();
		backgroundRight.SetSize(new Vector2(61f, 142f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage gUIImage2 = backgroundRight;
		Vector2 position4 = backgroundMiddle.Position;
		float x3 = position4.x;
		Vector2 size2 = backgroundMiddle.Size;
		float x4 = x3 + size2.x;
		Vector2 position5 = backgroundMiddle.Position;
		gUIImage2.Position = new Vector2(x4, position5.y);
		backgroundRight.TextureSource = "GUI/Notifications/invitation_frame_right_blue";
		backgroundRight.Id = "BackgroundRightImage";
		_container.Add(backgroundRight);
		string[] array = _targetAchievement.hintIcon.Split(',');
		inviteIcon = new GUIImage();
		inviteIcon.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(14f, 16f), new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		if (array.Length > 0)
		{
			inviteIcon.TextureSource = array[0];
			_container.Add(inviteIcon);
			if (array.Length > 1)
			{
				inviteIcon = new GUIImage();
				inviteIcon.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(14f, 64f), new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				inviteIcon.TextureSource = array[1];
				_container.Add(inviteIcon);
			}
		}
		_container.Add(achievementTitleLabel);
		_container.Add(achievementStatusLabel);
		checkbox = new GUIImage();
		checkbox.SetSize(new Vector2(20f, 18f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		checkbox.Position = achievementStatusLabel.Position;
		checkbox.TextureSource = "achievement_bundle|objective_incomplete";
		checkbox.IsVisible = true;
		_container.Add(checkbox);
		checkboxCheck = new GUIImage();
		checkboxCheck.SetSize(new Vector2(18f, 20f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		checkboxCheck.Position = checkbox.Position + new Vector2(0f, 0f);
		checkboxCheck.TextureSource = "achievement_bundle|objective_check";
		checkboxCheck.IsVisible = false;
		_container.Add(checkboxCheck);
		achievementStatusLabel.Position += new Vector2(20f, 0f);
		achievementStatusLabel.Size -= new Vector2(20f, 0f);
		stopTrackButton = new GUIButton();
		stopTrackButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		stopTrackButton.SetSize(new Vector2(36f, 36f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIButton gUIButton = stopTrackButton;
		Vector2 position6 = backgroundRight.Position;
		gUIButton.SetPosition(new Vector2(position6.x + 20f, 0f));
		stopTrackButton.StyleInfo = new SHSButtonStyleInfo("achievement_bundle|mshs_closebutton");
		stopTrackButton.ToolTip = new NamedToolTipInfo("#ACHIEVEMENT_STOP_TRACKING");
		stopTrackButton.HitTestType = HitTestTypeEnum.Circular;
		stopTrackButton.HitTestSize = new Vector2(0.6f, 0.6f);
		stopTrackButton.Click += StopTrackClick;
		_container.Add(stopTrackButton);
		if (_targetAchievement.track == AchievementManager.DestinyTracks.Valor && _targetAchievement.achievementID <= 200050)
		{
			stopTrackButton.IsVisible = false;
		}
		collapseButton = new GUIButton();
		collapseButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		collapseButton.SetSize(new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIButton gUIButton2 = collapseButton;
		Vector2 position7 = backgroundRight.Position;
		gUIButton2.SetPosition(new Vector2(position7.x + 8f, 20f));
		collapseButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|arrow_right");
		collapseButton.ToolTip = new NamedToolTipInfo("#ACHIEVEMENT_MINIMIZE");
		collapseButton.HitTestType = HitTestTypeEnum.Circular;
		collapseButton.HitTestSize = new Vector2(0.6f, 0.6f);
		collapseButton.Click += CollapseClick;
		_container.Add(collapseButton);
		expandButton = new GUIButton();
		expandButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		expandButton.SetSize(new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIButton gUIButton3 = expandButton;
		Vector2 size3 = backgroundLeft.Size;
		float y = size3.y;
		Vector2 size4 = collapseButton.Size;
		gUIButton3.SetPosition(new Vector2(11f, y - size4.y + 10f));
		expandButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|arrow_left");
		expandButton.ToolTip = new NamedToolTipInfo("#ACHIEVEMENT_MINIMIZE");
		expandButton.HitTestType = HitTestTypeEnum.Circular;
		expandButton.HitTestSize = new Vector2(0.6f, 0.6f);
		expandButton.Click += OpenClick;
		_container.Add(expandButton);
		expandButton.IsVisible = false;
		detailsButton = new GUIButton();
		detailsButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		detailsButton.SetSize(new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIButton gUIButton4 = detailsButton;
		Vector2 position8 = backgroundRight.Position;
		gUIButton4.SetPosition(new Vector2(position8.x + 8f, 50f));
		detailsButton.StyleInfo = new SHSButtonStyleInfo("achievement_bundle|search_field_icon");
		detailsButton.ToolTip = new NamedToolTipInfo("#ACHIEVEMENT_DETAILS");
		detailsButton.HitTestType = HitTestTypeEnum.Circular;
		detailsButton.HitTestSize = new Vector2(0.6f, 0.6f);
		detailsButton.Click += DetailsClick;
		_container.Add(detailsButton);
		helpButton = new GUIButton();
		helpButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		helpButton.SetSize(new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIButton gUIButton5 = helpButton;
		Vector2 position9 = backgroundRight.Position;
		gUIButton5.SetPosition(new Vector2(position9.x + 3f, 82f));
		helpButton.StyleInfo = new SHSButtonStyleInfo("achievement_bundle|mshs_button_help");
		helpButton.ToolTip = new NamedToolTipInfo("#ACHIEVEMENT_HELP");
		helpButton.HitTestType = HitTestTypeEnum.Circular;
		helpButton.HitTestSize = new Vector2(0.6f, 0.6f);
		helpButton.Click += HelpClick;
		_container.Add(helpButton);
		if (_targetAchievement.helpURL == null || _targetAchievement.helpURL.Length < 2)
		{
			helpButton.IsVisible = false;
		}
		hotSpot = GUIControl.CreateControlTopLeftFrame<GUIHotSpotButton>(backgroundLeft.Size, backgroundLeft.Position);
		hotSpot.HitTestType = HitTestTypeEnum.Rect;
		hotSpot.HitTestSize = new Vector2(1f, 1f);
		hotSpot.Click += OpenClick;
		_container.Add(hotSpot);
		hotSpot.IsVisible = false;
		Id = "AchievementTrackerNotificationWindow";
		Vector2 size5 = backgroundLeft.Size;
		float x5 = size5.x;
		Vector2 size6 = backgroundRight.Size;
		float num2 = x5 + size6.x;
		Vector2 size7 = backgroundMiddle.Size;
		SetSize(new Vector2(num2 + size7.x - 8f, 145f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_container.SetSize(Size);
		summaryLabel = new GUILabel();
		summaryLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(0, 0, 0), TextAnchor.MiddleCenter);
		GUILabel gUILabel2 = summaryLabel;
		Vector2 position10 = backgroundLeft.Position;
		Vector2 size8 = backgroundLeft.Size;
		gUILabel2.SetPosition(position10 + new Vector2(0f, size8.y - 35f));
		GUILabel gUILabel3 = summaryLabel;
		Vector2 size9 = backgroundLeft.Size;
		gUILabel3.SetSize(new Vector2(size9.x - 20f, 20f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		summaryLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		_container.Add(summaryLabel);
		summaryLabel.IsVisible = false;
		AppShell.Instance.EventMgr.AddListener<AchievementCompleteMessage>(OnAchievementCompleteMessage);
		AppShell.Instance.EventMgr.AddListener<AchievementProgressUpdateMessage>(OnAchievementProgressUpdateMessage);
		AppShell.Instance.EventMgr.AddListener<StopTrackingAchievementMessage>(OnStopTrackingAchievementMessage);
		updateStatus();
		if (GameController.GetController() is SocialSpaceController && _targetAchievement.jumpAction != string.Empty)
		{
			AchievementJumpActionContainer achievementJumpActionContainer = new AchievementJumpActionContainer(_targetAchievement);
			Vector2 position11 = backgroundLeft.Position;
			Vector2 size10 = backgroundLeft.Size;
			achievementJumpActionContainer.SetPosition(position11 + new Vector2(120f, size10.y - 50f));
			Add(achievementJumpActionContainer);
		}
	}

	private void OnAchievementCompleteMessage(AchievementCompleteMessage msg)
	{
		if (msg.achievementID == _targetAchievement.achievementID)
		{
			_parent.notificationComplete(this);
		}
	}

	private void OnAchievementProgressUpdateMessage(AchievementProgressUpdateMessage msg)
	{
		NewAchievement newAchievement = AchievementManager.allAchievements[msg.achievementID];
		if (msg.achievementID == _targetAchievement.achievementID)
		{
			updateStatus();
		}
	}

	private void updateStatus()
	{
		PlayerAchievementData playerAchievementData = AchievementManager.allPlayerData[(int)AppShell.Instance.Profile.UserId];
		int num = 0;
		NewAchievementStep newAchievementStep = _targetAchievement.steps[0];
		int num2 = 0;
		while (true)
		{
			num = playerAchievementData.getStepProgress(newAchievementStep.achievementStepID);
			if (num == AchievementManager.INVALID_VALUE)
			{
				num = 0;
			}
			if (num >= _targetAchievement.steps[num2].threshold && _targetAchievement.steps.Count > num2 + 1)
			{
				num2++;
				newAchievementStep = _targetAchievement.steps[num2];
				continue;
			}
			break;
		}
		achievementStatusLabel.VerticalKerning = 12;
		achievementStatusLabel.Text = AchievementManager.formatAchievementString(newAchievementStep.displayDesc, num, newAchievementStep.threshold, newAchievementStep.heroName, newAchievementStep.int1);
		achievementStatusLabel.VerticalKerning = 12;
		if (num >= newAchievementStep.threshold)
		{
			checkboxCheck.IsVisible = true;
		}
		if (_targetAchievement.steps.Count == 1)
		{
			num = playerAchievementData.getStepProgress(newAchievementStep.achievementStepID);
			if (num == AchievementManager.INVALID_VALUE)
			{
				num = 0;
			}
			summaryLabel.Text = num + "/" + newAchievementStep.threshold;
			return;
		}
		int num3 = 0;
		newAchievementStep = _targetAchievement.steps[0];
		for (int i = 0; i < _targetAchievement.steps.Count; i++)
		{
			num = playerAchievementData.getStepProgress(_targetAchievement.steps[i].achievementStepID);
			if (num == AchievementManager.INVALID_VALUE)
			{
				num = 0;
			}
			if (num >= _targetAchievement.steps[num2].threshold)
			{
				num3++;
			}
		}
		summaryLabel.Text = num3 + "/" + _targetAchievement.steps.Count;
	}

	private IEnumerator disableHelp()
	{
		helpButton.IsEnabled = false;
		yield return new WaitForSeconds(5f);
		helpButton.IsEnabled = true;
	}

	private void HelpClick(GUIControl sender, GUIClickEvent EventData)
	{
		SHSSocialMainWindow sHSSocialMainWindow = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
		if (sHSSocialMainWindow != null)
		{
			sHSSocialMainWindow.ShowTutorialVideo(_targetAchievement.helpURL);
		}
		AppShell.Instance.StartCoroutine(disableHelp());
	}

	private void DetailsClick(GUIControl sender, GUIClickEvent EventData)
	{
		AchievementWindow dialogWindow = new AchievementWindow((int)AppShell.Instance.Profile.UserId, _targetAchievement.achievementID);
		GUIManager.Instance.ShowDynamicWindow(dialogWindow, ModalLevelEnum.Default);
	}

	private void StopTrackClick(GUIControl sender, GUIClickEvent EventData)
	{
		AchievementManager.stopTrackingAchievement(_targetAchievement.achievementID);
	}

	private void OnStopTrackingAchievementMessage(StopTrackingAchievementMessage msg)
	{
		if (msg.achievementID == _targetAchievement.achievementID)
		{
			_parent.notificationComplete(this);
		}
	}

	private void CollapseClick(GUIControl sender, GUIClickEvent EventData)
	{
		AchievementManager.trackedAchievementCollapse(_targetAchievement.achievementID);
		Vector2 position = Position;
		Vector2 size = Size;
		float x = size.x;
		Vector2 size2 = backgroundLeft.Size;
		Position = position + new Vector2(x - size2.x + 35f, 0f);
		hotSpot.IsVisible = true;
		collapseButton.IsVisible = false;
		summaryLabel.IsVisible = true;
		isCollapsed = true;
	}

	private void OpenClick(GUIControl sender, GUIClickEvent EventData)
	{
		AchievementManager.trackedAchievementExpand(_targetAchievement.achievementID);
		Vector2 position = Position;
		Vector2 size = Size;
		float x = size.x;
		Vector2 size2 = backgroundLeft.Size;
		Position = position - new Vector2(x - size2.x + 35f, 0f);
		hotSpot.IsVisible = false;
		expandButton.IsVisible = false;
		collapseButton.IsVisible = true;
		summaryLabel.IsVisible = false;
		isCollapsed = false;
	}

	public override void activate()
	{
		if (isCollapsed)
		{
			Vector2 position = Position;
			Vector2 size = Size;
			float x = size.x;
			Vector2 size2 = backgroundLeft.Size;
			Position = position + new Vector2(x - size2.x + 35f, 0f);
			hotSpot.IsVisible = true;
			collapseButton.IsVisible = false;
			summaryLabel.IsVisible = true;
		}
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		Alpha = 0f;
		AnimClip animClip = base.animClip = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 1f), this);
		_parent.animManager.Add(base.animClip);
	}
}
