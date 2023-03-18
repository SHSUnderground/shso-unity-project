using System.Collections.Generic;
using UnityEngine;

public class HelpNotificationWindow : NotificationWindow
{
	public enum HelpInfo
	{
		DailyFractal,
		DailyGoldenFractal,
		DailyScavenger,
		DailyImpossibleMan,
		DailyToken,
		DailySeasonal_Halloween,
		DailyRareSeasonal_Halloween
	}

	private GUIImage backgroundLeft;

	private GUIImage backgroundMiddle;

	private GUIImage backgroundRight;

	private GUIImage inviteIcon;

	private GUILabel achievementStatusLabel;

	private GUILabel summaryLabel;

	private GUILabel achievementTitleLabel;

	private GUIButton acceptButton;

	private GUISimpleControlWindow _container;

	private GUIButton nextTextButton;

	private List<string> helpStrings = new List<string>();

	private int currentHelpIndex;

	public HelpNotificationWindow()
		: base(NotificationData.NotificationType.Help)
	{
		Traits.HitTestType = HitTestTypeEnum.Rect;
		Traits.BlockTestType = BlockTestTypeEnum.Rect;
		SetControlFlag(ControlFlagSetting.HitTestIgnore, false, true);
		Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		Traits.UpdateTrait = ControlTraits.UpdateTraitEnum.AlwaysUpdate;
	}

	public override void activate()
	{
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("earn_achievement"));
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
		HelpNotificationData helpNotificationData = getData() as HelpNotificationData;
		_container = new GUISimpleControlWindow();
		_container.SetPosition(Vector2.zero);
		Add(_container);
		float num = 230f;
		backgroundLeft = new GUIImage();
		backgroundLeft.SetSize(new Vector2(108f, 142f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		backgroundLeft.Position = Vector2.zero;
		backgroundLeft.TextureSource = "GUI/Notifications/invitation_frame_left_blue_icon";
		backgroundLeft.Id = "BackgroundLeftImage";
		_container.Add(backgroundLeft);
		backgroundMiddle = new GUIImage();
		backgroundMiddle.SetSize(new Vector2(num - 30f, 142f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage gUIImage = backgroundMiddle;
		Vector2 position = backgroundLeft.Position;
		float x = position.x;
		Vector2 size = backgroundLeft.Size;
		float x2 = x + size.x;
		Vector2 position2 = backgroundLeft.Position;
		gUIImage.Position = new Vector2(x2, position2.y);
		backgroundMiddle.TextureSource = "GUI/Notifications/invitation_frame_center_blue";
		backgroundMiddle.Id = "BackgroundMiddleImage";
		_container.Add(backgroundMiddle);
		backgroundRight = new GUIImage();
		backgroundRight.SetSize(new Vector2(61f, 142f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage gUIImage2 = backgroundRight;
		Vector2 position3 = backgroundMiddle.Position;
		float x3 = position3.x;
		Vector2 size2 = backgroundMiddle.Size;
		float x4 = x3 + size2.x;
		Vector2 position4 = backgroundMiddle.Position;
		gUIImage2.Position = new Vector2(x4, position4.y);
		backgroundRight.TextureSource = "GUI/Notifications/invitation_frame_right_blue";
		backgroundRight.Id = "BackgroundRightImage";
		_container.Add(backgroundRight);
		achievementTitleLabel = new GUILabel();
		achievementTitleLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
		achievementTitleLabel.SetPosition(new Vector2(80f, 25f));
		achievementTitleLabel.SetSize(new Vector2(num + 10f, 120f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		achievementTitleLabel.WordWrap = true;
		if (helpNotificationData.iconUnderlayPath != string.Empty)
		{
			GUIImage gUIImage3 = new GUIImage();
			gUIImage3.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(14f, 16f), new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIImage3.TextureSource = helpNotificationData.iconUnderlayPath;
			_container.Add(gUIImage3);
		}
		if (helpNotificationData.iconPath != string.Empty)
		{
			GUIImage gUIImage4 = new GUIImage();
			gUIImage4.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(14f, 16f), new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIImage4.TextureSource = helpNotificationData.iconPath;
			_container.Add(gUIImage4);
		}
		if (helpNotificationData.iconOverlayPath != string.Empty)
		{
			GUIImage gUIImage5 = new GUIImage();
			gUIImage5.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(14f, 16f), new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIImage5.TextureSource = helpNotificationData.iconOverlayPath;
			_container.Add(gUIImage5);
		}
		helpStrings = helpNotificationData.helpStrings;
		currentHelpIndex = 0;
		achievementTitleLabel.VerticalKerning = 12;
		achievementTitleLabel.Text = helpStrings[0];
		achievementTitleLabel.VerticalKerning = 12;
		_container.Add(achievementTitleLabel);
		acceptButton = new GUIButton();
		acceptButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		acceptButton.SetSize(new Vector2(36f, 36f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIButton gUIButton = acceptButton;
		Vector2 position5 = backgroundRight.Position;
		gUIButton.SetPosition(new Vector2(position5.x + 20f, 0f));
		acceptButton.StyleInfo = new SHSButtonStyleInfo("achievement_bundle|mshs_closebutton");
		acceptButton.Id = "AcceptButton";
		acceptButton.HitTestType = HitTestTypeEnum.Circular;
		acceptButton.HitTestSize = new Vector2(0.6f, 0.6f);
		acceptButton.Click += AcceptButtonClick;
		Add(acceptButton);
		if (helpStrings.Count > 1)
		{
			nextTextButton = new GUIButton();
			nextTextButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
			nextTextButton.SetSize(new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			GUIButton gUIButton2 = nextTextButton;
			Vector2 position6 = backgroundRight.Position;
			gUIButton2.SetPosition(new Vector2(position6.x + 8f, 20f));
			nextTextButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|arrow_right");
			nextTextButton.HitTestType = HitTestTypeEnum.Circular;
			nextTextButton.HitTestSize = new Vector2(0.6f, 0.6f);
			nextTextButton.Click += NextTextClick;
			_container.Add(nextTextButton);
		}
		Vector2 size3 = backgroundLeft.Size;
		float x5 = size3.x;
		Vector2 size4 = backgroundRight.Size;
		float num2 = x5 + size4.x;
		Vector2 size5 = backgroundMiddle.Size;
		SetSize(new Vector2(num2 + size5.x - 8f, 145f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_container.SetSize(Size);
	}

	private void NextTextClick(GUIControl sender, GUIClickEvent EventData)
	{
		currentHelpIndex++;
		if (currentHelpIndex >= helpStrings.Count)
		{
			currentHelpIndex = 0;
		}
		achievementTitleLabel.VerticalKerning = 12;
		achievementTitleLabel.Text = helpStrings[currentHelpIndex];
		achievementTitleLabel.VerticalKerning = 12;
	}

	private void AcceptButtonClick(GUIControl sender, GUIClickEvent EventData)
	{
		_parent.notificationComplete(this);
	}
}
