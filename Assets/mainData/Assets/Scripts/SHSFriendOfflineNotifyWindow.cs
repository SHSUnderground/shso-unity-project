using UnityEngine;

public class SHSFriendOfflineNotifyWindow : GUINotificationWindow
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

	public SHSFriendOfflineNotifyWindow(string title)
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
		friendIcon = new GUIImage();
		friendIcon.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(156f, 190f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		friendIcon.TextureSource = "notification_bundle|notification_icon_noinvite";
		notificationBackground = new NotificationBackground();
		NotificationBackground obj = notificationBackground;
		Vector2 size = titleLabel.Size;
		obj.Build(size.x);
		backgroundLeft = notificationBackground.GetBackgroundPiece(NotificationBackground.PieceType.LeftPiece);
		backgroundMiddle = notificationBackground.GetBackgroundPiece(NotificationBackground.PieceType.MiddlePiece);
		backgroundRight = notificationBackground.GetBackgroundPiece(NotificationBackground.PieceType.RightPiece);
		okButton = new GUIButton();
		okButton.SetSize(new Vector2(121.6f, 121.6f));
		GUIButton gUIButton = okButton;
		Vector2 position = backgroundMiddle.Position;
		float x = position.x;
		Vector2 size2 = backgroundMiddle.Size;
		float num = x + size2.x * 0.5f;
		Vector2 size3 = okButton.Size;
		gUIButton.SetPosition(new Vector2(num - size3.x * 0.5f, 55f));
		okButton.StyleInfo = new SHSButtonStyleInfo("notification_bundle|simple_ok_button");
		okButton.Traits.BlockTestType = BlockTestTypeEnum.Rect;
		okButton.Click += delegate
		{
			Hide();
		};
		GUILabel gUILabel = titleLabel;
		Vector2 position2 = backgroundLeft.Position;
		float x2 = position2.x;
		Vector2 size4 = backgroundLeft.Size;
		float num2 = x2 + size4.x;
		Vector2 size5 = backgroundMiddle.Size;
		float num3 = num2 + size5.x * 0.5f;
		Vector2 size6 = titleLabel.Size;
		gUILabel.Position = new Vector2(num3 - size6.x * 0.5f, 30f);
		Add(backgroundLeft);
		Add(backgroundMiddle);
		Add(backgroundRight);
		Add(titleLabel);
		Add(friendIcon);
		Add(okButton);
		Vector2 offset = new Vector2(0f, 20f);
		Vector2 size7 = backgroundLeft.Size;
		float x3 = size7.x;
		Vector2 size8 = backgroundMiddle.Size;
		float num4 = x3 + size8.x;
		Vector2 size9 = backgroundRight.Size;
		float x4 = num4 + size9.x;
		Vector2 size10 = friendIcon.Size;
		SetPositionAndSize(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, offset, new Vector2(x4, size10.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
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
