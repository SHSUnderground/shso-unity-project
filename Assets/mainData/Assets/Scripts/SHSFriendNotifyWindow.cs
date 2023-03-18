using UnityEngine;

public class SHSFriendNotifyWindow : GUINotificationWindow
{
	private const float buttonShrinkPercentage = 0.95f;

	private const float okButtonOffset = 55f;

	private float fadeStartTime = 3.75f;

	private float fadeDurationTime = 2f;

	private NotificationBackground notificationBackground;

	private GUIImage backgroundLeft;

	private GUIImage backgroundMiddle;

	private GUIImage backgroundRight;

	private GUIImage friendIcon;

	private GUILabel titleLabel;

	private GUIButton okButton;

	public SHSFriendNotifyWindow(string title)
	{
		HitTestType = HitTestTypeEnum.Rect;
		BlockTestType = BlockTestTypeEnum.Rect;
		titleLabel = new GUILabel();
		titleLabel.Text = title;
		titleLabel.Size = new Vector2(300f, 200f);
		titleLabel.IsVisible = true;
		titleLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 17, Color.white, TextAnchor.UpperCenter);
		titleLabel.Bold = true;
		titleLabel.Italicized = true;
		friendIcon = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(97f, 91f), Vector2.zero);
		friendIcon.Position = Vector2.zero;
		friendIcon.TextureSource = "notification_bundle|friend_confirmation_icon";
		notificationBackground = new NotificationBackground();
		notificationBackground.Build(GUINotificationWindow.GetTextBlockSize(new GUILabel[1]
		{
			titleLabel
		}, BlockSizeType.Width) + 50f);
		backgroundLeft = notificationBackground.GetBackgroundPiece(NotificationBackground.PieceType.LeftPiece);
		backgroundMiddle = notificationBackground.GetBackgroundPiece(NotificationBackground.PieceType.MiddlePiece);
		backgroundRight = notificationBackground.GetBackgroundPiece(NotificationBackground.PieceType.RightPiece);
		okButton = new GUIButton();
		okButton.SetSize(new Vector2(121.6f, 121.6f));
		GUIButton gUIButton = okButton;
		Vector2 position = backgroundMiddle.Position;
		float x = position.x;
		Vector2 size = backgroundMiddle.Size;
		float num = x + size.x * 0.5f;
		Vector2 size2 = okButton.Size;
		gUIButton.SetPosition(new Vector2(num - size2.x * 0.5f, 55f));
		okButton.StyleInfo = new SHSButtonStyleInfo("notification_bundle|simple_ok_button");
		okButton.Traits.BlockTestType = BlockTestTypeEnum.Rect;
		okButton.Click += delegate
		{
			Hide();
		};
		GUILabel gUILabel = titleLabel;
		Vector2 position2 = backgroundLeft.Position;
		float x2 = position2.x;
		Vector2 size3 = backgroundLeft.Size;
		float num2 = x2 + size3.x;
		Vector2 size4 = backgroundMiddle.Size;
		float num3 = num2 + size4.x * 0.5f;
		Vector2 size5 = titleLabel.Size;
		gUILabel.Position = new Vector2(num3 - size5.x * 0.5f, 30f);
		Add(backgroundLeft);
		Add(backgroundMiddle);
		Add(backgroundRight);
		Add(titleLabel);
		Add(friendIcon);
		Add(okButton);
		Vector2 position3 = okButton.Position;
		float y = position3.y;
		Vector2 size6 = okButton.Size;
		float num4 = y + size6.y;
		Vector2 size7 = backgroundLeft.Size;
		float num5 = num4 - size7.y;
		Vector2 offset = new Vector2(0f, 20f);
		Vector2 size8 = backgroundLeft.Size;
		float x3 = size8.x;
		Vector2 size9 = backgroundMiddle.Size;
		float num6 = x3 + size9.x;
		Vector2 size10 = backgroundRight.Size;
		float x4 = num6 + size10.x;
		Vector2 size11 = backgroundLeft.Size;
		SetPositionAndSize(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, offset, new Vector2(x4, size11.y + num5), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		float num = Time.time - timeStarted;
		if (num > fadeStartTime)
		{
			Alpha = 1f - (num - fadeStartTime) / fadeDurationTime;
		}
		if (Alpha <= 0f)
		{
			Hide();
		}
	}

	public override void OnShow()
	{
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("popup_alert"));
	}

	protected override void dispose(bool disposing)
	{
		base.dispose(disposing);
		backgroundLeft = null;
		backgroundMiddle = null;
		backgroundRight = null;
		titleLabel = null;
		notificationBackground.Dispose();
		notificationBackground = null;
	}
}
