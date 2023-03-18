using UnityEngine;

public class DailyRewardNotificationWindow : NotificationWindow
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

	public DailyRewardNotificationWindow()
		: base(NotificationData.NotificationType.DailyReward)
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
		mainLabel = new GUILabel();
		mainLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
		mainLabel.Text = "Here's your reward for logging into Super Hero City today!";
		mainLabel.SetPosition(new Vector2(115f, 30f));
		mainLabel.SetSize(new Vector2(200f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		mainLabel.WordWrap = true;
		mainLabel.Id = "InviteLabel";
		backgroundLeft = new GUIImage();
		backgroundLeft.SetSize(new Vector2(108f, 142f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		backgroundLeft.Position = Vector2.zero;
		backgroundLeft.TextureSource = "GUI/Notifications/invitation_frame_left";
		backgroundLeft.Id = "BackgroundLeftImage";
		Add(backgroundLeft);
		float textBlockSize = NotificationWindow.GetTextBlockSize(new GUILabel[1]
		{
			mainLabel
		}, BlockSizeType.Width);
		backgroundMiddle = new GUIImage();
		backgroundMiddle.SetSize(new Vector2(200f, 142f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage gUIImage = backgroundMiddle;
		Vector2 position = backgroundLeft.Position;
		float x = position.x;
		Vector2 size = backgroundLeft.Size;
		float x2 = x + size.x;
		Vector2 position2 = backgroundLeft.Position;
		gUIImage.Position = new Vector2(x2, position2.y);
		backgroundMiddle.TextureSource = "GUI/Notifications/invitation_frame_center";
		backgroundMiddle.Id = "BackgroundMiddleImage";
		Add(backgroundMiddle);
		backgroundRight = new GUIImage();
		backgroundRight.SetSize(new Vector2(49f, 142f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage gUIImage2 = backgroundRight;
		Vector2 position3 = backgroundMiddle.Position;
		float x3 = position3.x;
		Vector2 size2 = backgroundMiddle.Size;
		float x4 = x3 + size2.x;
		Vector2 position4 = backgroundMiddle.Position;
		gUIImage2.Position = new Vector2(x4, position4.y);
		backgroundRight.TextureSource = "GUI/Notifications/invitation_frame_right";
		backgroundRight.Id = "BackgroundRightImage";
		Add(backgroundRight);
		DailyRewardNotificationData dailyRewardNotificationData = (DailyRewardNotificationData)getData();
		string[] array = dailyRewardNotificationData.rewardData.Split(',');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split(':');
			int num = int.Parse(array3[1]);
			if (!(array3[0] == "gold"))
			{
				int id = int.Parse(array3[0]);
				_ownableDef = OwnableDefinition.getDef(id);
				rewardIcon = _ownableDef.getIcon(new Vector2(60f, 60f), true);
				rewardIcon.SetPosition(new Vector2(20f, 20f));
				rewardIcon.ToolTip = new NamedToolTipInfo(_ownableDef.shoppingName);
				Add(rewardIcon);
			}
			quantityLabel = new GUIStrokeTextLabel();
			GUIStrokeTextLabel gUIStrokeTextLabel = quantityLabel;
			Vector2 offset = rewardIcon.Position + new Vector2(0f, 34f);
			Vector2 size3 = rewardIcon.Size;
			gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, offset, new Vector2(size3.x, 33f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			quantityLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(1f, 2f), TextAnchor.UpperRight);
			quantityLabel.BackColorAlpha = 1f;
			quantityLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
			quantityLabel.Text = "x" + num;
			Add(quantityLabel);
		}
		Add(mainLabel);
		acceptButton = new GUIButton();
		acceptButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		acceptButton.SetSize(new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIButton gUIButton = acceptButton;
		Vector2 position5 = backgroundMiddle.Position;
		float x5 = position5.x;
		Vector2 size4 = backgroundMiddle.Size;
		float num2 = x5 + size4.x * 0.5f;
		Vector2 size5 = acceptButton.Size;
		gUIButton.SetPosition(new Vector2(num2 - size5.x * 0.5f, 50f));
		acceptButton.StyleInfo = new SHSButtonStyleInfo("notification_bundle|simple_ok_button");
		acceptButton.Id = "AcceptButton";
		acceptButton.HitTestType = HitTestTypeEnum.Circular;
		acceptButton.HitTestSize = new Vector2(0.6f, 0.6f);
		acceptButton.Click += AcceptButtonClick;
		Add(acceptButton);
		Id = "DailyRewardNotificationWindow";
		Vector2 size6 = backgroundLeft.Size;
		float x6 = size6.x;
		Vector2 size7 = backgroundRight.Size;
		float num3 = x6 + size7.x;
		Vector2 size8 = backgroundMiddle.Size;
		SetSize(new Vector2(num3 + size8.x, 145f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
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
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("popup_alert"));
	}

	private void AcceptButtonClick(GUIControl sender, GUIClickEvent EventData)
	{
		_parent.notificationComplete(this);
	}
}
