using UnityEngine;

public class HelpWindow : GUIDynamicWindow
{
	private GUIImage backgroundLeft;

	private GUIImage backgroundMiddle;

	private GUIImage backgroundRight;

	private GUIImage inviteIcon;

	private GUILabel achievementStatusLabel;

	private GUILabel summaryLabel;

	private GUILabel achievementTitleLabel;

	private GUIButton acceptButton;

	private GUISimpleControlWindow _container;

	public HelpWindow(string helpText, string iconPath)
	{
		_container = new GUISimpleControlWindow();
		_container.SetPosition(Vector2.zero);
		Add(_container);
		float num = 170f;
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
		achievementTitleLabel.VerticalKerning = 12;
		achievementTitleLabel.Text = helpText;
		achievementTitleLabel.VerticalKerning = 12;
		if (iconPath != string.Empty)
		{
			inviteIcon = new GUIImage();
			inviteIcon.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(14f, 16f), new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			inviteIcon.TextureSource = iconPath;
			_container.Add(inviteIcon);
		}
		_container.Add(achievementTitleLabel);
		acceptButton = new GUIButton();
		acceptButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		acceptButton.SetSize(new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIButton gUIButton = acceptButton;
		Vector2 position5 = backgroundMiddle.Position;
		float x5 = position5.x;
		Vector2 size3 = backgroundMiddle.Size;
		float num2 = x5 + size3.x * 0.5f;
		Vector2 size4 = acceptButton.Size;
		gUIButton.SetPosition(new Vector2(num2 - size4.x * 0.5f, 50f));
		acceptButton.StyleInfo = new SHSButtonStyleInfo("notification_bundle|simple_ok_button");
		acceptButton.Id = "AcceptButton";
		acceptButton.HitTestType = HitTestTypeEnum.Circular;
		acceptButton.HitTestSize = new Vector2(0.6f, 0.6f);
		acceptButton.Click += AcceptButtonClick;
		Add(acceptButton);
		Vector2 size5 = backgroundLeft.Size;
		float x6 = size5.x;
		Vector2 size6 = backgroundRight.Size;
		float num3 = x6 + size6.x;
		Vector2 size7 = backgroundMiddle.Size;
		SetSize(new Vector2(num3 + size7.x - 8f, 145f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_container.SetSize(Size);
	}

	private void AcceptButtonClick(GUIControl sender, GUIClickEvent EventData)
	{
		Dispose();
	}
}
