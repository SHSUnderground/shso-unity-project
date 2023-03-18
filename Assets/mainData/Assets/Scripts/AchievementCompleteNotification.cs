using UnityEngine;

public class AchievementCompleteNotification : NotificationWindow
{
	private GUIImage backgroundLeft;

	private GUIImage backgroundMiddle;

	private GUIImage backgroundRight;

	private GUISimpleControlWindow rewardIcon;

	private GUILabel playerNameLabel;

	private GUILabel mainLabel;

	private GUIStrokeTextLabel quantityLabel;

	private GUIButton acceptButton;

	private OwnableDefinition _ownableDef;

	public AchievementCompleteNotification()
		: base(NotificationData.NotificationType.AchievementComplete)
	{
		Traits.HitTestType = HitTestTypeEnum.Rect;
		Traits.BlockTestType = BlockTestTypeEnum.Rect;
		SetControlFlag(ControlFlagSetting.HitTestIgnore, false, true);
		Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		Traits.UpdateTrait = ControlTraits.UpdateTraitEnum.AlwaysUpdate;
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
		AchievementCompleteNotificationData achievementCompleteNotificationData = data as AchievementCompleteNotificationData;
		NewAchievement newAchievement = AchievementManager.allAchievements[achievementCompleteNotificationData.achievementID];
		backgroundLeft = new GUIImage();
		backgroundLeft.SetSize(new Vector2(81f, 211f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		backgroundLeft.Position = Vector2.zero;
		backgroundLeft.TextureSource = "GUI/Notifications/invitation_frame_left_R2";
		backgroundLeft.Id = "BackgroundLeftImage";
		Add(backgroundLeft);
		backgroundMiddle = new GUIImage();
		backgroundMiddle.SetSize(new Vector2(250f, 211f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage gUIImage = backgroundMiddle;
		Vector2 position = backgroundLeft.Position;
		float x = position.x;
		Vector2 size = backgroundLeft.Size;
		float x2 = x + size.x;
		Vector2 position2 = backgroundLeft.Position;
		gUIImage.Position = new Vector2(x2, position2.y);
		backgroundMiddle.TextureSource = "GUI/Notifications/invitation_frame_center_R2";
		backgroundMiddle.Id = "BackgroundMiddleImage";
		Add(backgroundMiddle);
		backgroundRight = new GUIImage();
		backgroundRight.SetSize(new Vector2(81f, 211f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage gUIImage2 = backgroundRight;
		Vector2 position3 = backgroundMiddle.Position;
		float x3 = position3.x;
		Vector2 size2 = backgroundMiddle.Size;
		float x4 = x3 + size2.x;
		Vector2 position4 = backgroundMiddle.Position;
		gUIImage2.Position = new Vector2(x4, position4.y);
		backgroundRight.TextureSource = "GUI/Notifications/invitation_frame_right_R2";
		backgroundRight.Id = "BackgroundRightImage";
		Add(backgroundRight);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(1f, 2f), TextAnchor.UpperCenter);
		gUIStrokeTextLabel.Text = "Achievement Complete!";
		gUIStrokeTextLabel.SetPosition(new Vector2(20f, 25f));
		Vector2 size3 = backgroundLeft.Size;
		float x5 = size3.x;
		Vector2 size4 = backgroundMiddle.Size;
		float num = x5 + size4.x;
		Vector2 size5 = backgroundRight.Size;
		float num2 = num + size5.x;
		Vector2 position5 = gUIStrokeTextLabel.Position;
		gUIStrokeTextLabel.SetSize(new Vector2(num2 - position5.x * 2f, 90f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.WordWrap = true;
		GUILabel gUILabel = new GUILabel();
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperCenter);
		gUILabel.Text = AchievementManager.formatAchievementString(newAchievement.displayName, 0, 0, string.Empty);
		gUILabel.SetPosition(gUIStrokeTextLabel.Position + new Vector2(0f, 30f));
		gUILabel.SetSize(gUIStrokeTextLabel.Size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUILabel.WordWrap = true;
		if (achievementCompleteNotificationData.rewards.Count > 0)
		{
			GUIStrokeTextLabel gUIStrokeTextLabel2 = new GUIStrokeTextLabel();
			gUIStrokeTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(1f, 2f), TextAnchor.UpperCenter);
			gUIStrokeTextLabel2.Text = "Rewards:";
			gUIStrokeTextLabel2.SetPosition(backgroundMiddle.Position + new Vector2(0f, 100f));
			gUIStrokeTextLabel2.SetSize(backgroundMiddle.Size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIStrokeTextLabel2.WordWrap = true;
			Add(gUIStrokeTextLabel2);
			int num3 = 0;
			int num4 = 64;
			CspUtils.DebugLog("total rewards " + achievementCompleteNotificationData.rewards.Count);
			foreach (OwnableSet reward in achievementCompleteNotificationData.rewards)
			{
				_ownableDef = OwnableDefinition.getDef(reward.ownableTypeID);
				rewardIcon = _ownableDef.getIcon(new Vector2(60f, 60f));
				GUISimpleControlWindow gUISimpleControlWindow = rewardIcon;
				Vector2 position6 = backgroundMiddle.Position;
				Vector2 size6 = backgroundMiddle.Size;
				gUISimpleControlWindow.SetPosition(position6 + new Vector2(size6.x / 2f - (float)(num4 / 2 * achievementCompleteNotificationData.rewards.Count) + (float)(num4 * num3), 120f));
				Add(rewardIcon);
				if (reward.quantity > 1)
				{
					quantityLabel = new GUIStrokeTextLabel();
					GUIStrokeTextLabel gUIStrokeTextLabel3 = quantityLabel;
					Vector2 offset = rewardIcon.Position + new Vector2(0f, 34f);
					Vector2 size7 = rewardIcon.Size;
					gUIStrokeTextLabel3.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, offset, new Vector2(size7.x, 33f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
					quantityLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(1f, 2f), TextAnchor.UpperRight);
					quantityLabel.BackColorAlpha = 1f;
					quantityLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
					quantityLabel.Text = "x" + reward.quantity;
					Add(quantityLabel);
				}
				num3++;
			}
		}
		Add(gUIStrokeTextLabel);
		Add(gUILabel);
		acceptButton = new GUIButton();
		acceptButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		acceptButton.SetSize(new Vector2(96f, 96f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIButton gUIButton = acceptButton;
		Vector2 position7 = backgroundRight.Position;
		float x6 = position7.x;
		Vector2 size8 = backgroundRight.Size;
		float num5 = x6 + size8.x;
		Vector2 size9 = acceptButton.Size;
		float x7 = num5 - size9.x + 5f;
		Vector2 size10 = backgroundRight.Size;
		gUIButton.SetPosition(new Vector2(x7, size10.y - 80f));
		acceptButton.StyleInfo = new SHSButtonStyleInfo("notification_bundle|simple_ok_button");
		acceptButton.Id = "AcceptButton";
		acceptButton.HitTestType = HitTestTypeEnum.Circular;
		acceptButton.HitTestSize = new Vector2(0.6f, 0.6f);
		acceptButton.Click += AcceptButtonClick;
		Add(acceptButton);
		GUIButton gUIButton2 = new GUIButton();
		gUIButton2.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		gUIButton2.SetSize(new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Vector2 position8 = backgroundRight.Position;
		gUIButton2.SetPosition(new Vector2(position8.x + 28f, 37f));
		gUIButton2.StyleInfo = new SHSButtonStyleInfo("achievement_bundle|search_field_icon");
		gUIButton2.ToolTip = new NamedToolTipInfo("#ACH_COMPLETE_DETAILS");
		gUIButton2.HitTestType = HitTestTypeEnum.Circular;
		gUIButton2.HitTestSize = new Vector2(0.6f, 0.6f);
		gUIButton2.Click += DetailsClick;
		Add(gUIButton2);
		Id = "AchievementCompleteNotification";
		Vector2 size11 = backgroundLeft.Size;
		float x8 = size11.x;
		Vector2 size12 = backgroundRight.Size;
		float num6 = x8 + size12.x;
		Vector2 size13 = backgroundMiddle.Size;
		float x9 = num6 + size13.x;
		Vector2 size14 = backgroundMiddle.Size;
		SetSize(new Vector2(x9, size14.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
	}

	private void DetailsClick(GUIControl sender, GUIClickEvent EventData)
	{
		AchievementCompleteNotificationData achievementCompleteNotificationData = _data as AchievementCompleteNotificationData;
		NewAchievement newAchievement = AchievementManager.allAchievements[achievementCompleteNotificationData.achievementID];
		AchievementWindow dialogWindow = new AchievementWindow((int)AppShell.Instance.Profile.UserId, newAchievement.achievementID);
		GUIManager.Instance.ShowDynamicWindow(dialogWindow, ModalLevelEnum.Default);
		AppShell.Instance.EventMgr.Fire(this, new OpenAchievementsWindowMessage());
	}

	public override void activate()
	{
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		Alpha = 0f;
		AnimClip animClip = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 1f), this);
		AnimClip animClip2 = SHSAnimations.Generic.Wait(55f);
		AnimClip animClip3 = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(1f, 0f, 1f), this);
		base.animClip = animClip;
		_parent.animManager.Add(base.animClip);
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("earn_achievement"));
	}

	private void AcceptButtonClick(GUIControl sender, GUIClickEvent EventData)
	{
		AchievementCompleteNotificationData achievementCompleteNotificationData = _data as AchievementCompleteNotificationData;
		AppShell.Instance.EventMgr.Fire(this, new AchievementCompleteHideMessage(achievementCompleteNotificationData.achievementID));
		_parent.notificationComplete(this);
	}
}
