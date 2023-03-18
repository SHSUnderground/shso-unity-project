using UnityEngine;

public class FriendInviteNotificationWindow : NotificationWindow
{
	private const int textLineOffset = 5;

	private const int textBlockOffset = 30;

	private const int buttonOffset = 50;

	private const int declineDeficit = 5;

	private const float minWindowWidth = 200f;

	private GUIImage backgroundLeft;

	private GUIImage backgroundMiddle;

	private GUIImage backgroundRight;

	private GUIImage inviteIcon;

	private GUILabel playerNameLabel;

	private GUILabel inviteLabel;

	private GUIButton acceptButton;

	private GUIButton declineButton;

	private Vector2 iconSize;

	private Vector2 iconOffset;

	private string _iconPath = string.Empty;

	private Vector2 _iconSize = Vector2.zero;

	private Vector2 _iconOffset = Vector2.zero;

	protected bool inviteValid = true;

	protected bool acceptPending;

	protected string cancelledMessage = string.Empty;

	protected string _inviteMessage = string.Empty;

	public FriendInviteNotificationWindow()
		: base(NotificationData.NotificationType.FriendInvite)
	{
		_iconPath = "hud_bundle|friends01";
		_iconSize = new Vector2(256f, 256f);
		_iconOffset = new Vector2(-70f, -85f);
		_inviteMessage = AppShell.Instance.stringTable["#friendinvite_title"];
		Traits.HitTestType = HitTestTypeEnum.Rect;
		Traits.BlockTestType = BlockTestTypeEnum.Rect;
		SetControlFlag(ControlFlagSetting.HitTestIgnore, false, true);
		Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		Traits.UpdateTrait = ControlTraits.UpdateTraitEnum.AlwaysUpdate;
	}

	public FriendInviteNotificationData getActualData()
	{
		return (FriendInviteNotificationData)_data;
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
		GUIContent gUIContent = new GUIContent();
		float num = 0f;
		playerNameLabel = new GUILabel();
		playerNameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 249, 157), TextAnchor.UpperLeft);
		playerNameLabel.Text = getActualData().message.FriendName;
		gUIContent.text = playerNameLabel.Text;
		Vector2 vector = playerNameLabel.Style.UnityStyle.CalcSize(gUIContent);
		playerNameLabel.SetSize(new Vector2(vector.x, vector.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		playerNameLabel.Id = "PlayerNameLabel";
		num += vector.y;
		inviteLabel = new GUILabel();
		inviteLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
		inviteLabel.Text = _inviteMessage;
		gUIContent.text = inviteLabel.Text;
		vector = inviteLabel.Style.UnityStyle.CalcSize(gUIContent);
		inviteLabel.SetSize(new Vector2(vector.x, vector.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		inviteLabel.Id = "InviteLabel";
		backgroundLeft = new GUIImage();
		backgroundLeft.SetSize(new Vector2(108f, 142f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		backgroundLeft.Position = Vector2.zero;
		backgroundLeft.TextureSource = "GUI/Notifications/invitation_frame_left";
		backgroundLeft.Id = "BackgroundLeftImage";
		float num2 = GUINotificationWindow.GetTextBlockSize(new GUILabel[2]
		{
			playerNameLabel,
			inviteLabel
		}, GUINotificationWindow.BlockSizeType.Width);
		if (num2 < 200f)
		{
			num2 = 200f;
		}
		backgroundMiddle = new GUIImage();
		backgroundMiddle.SetSize(new Vector2(num2 + 1f, 142f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage gUIImage = backgroundMiddle;
		Vector2 position = backgroundLeft.Position;
		float x = position.x;
		Vector2 size = backgroundLeft.Size;
		float x2 = x + size.x;
		Vector2 position2 = backgroundLeft.Position;
		gUIImage.Position = new Vector2(x2, position2.y);
		backgroundMiddle.TextureSource = "GUI/Notifications/invitation_frame_center";
		backgroundMiddle.Id = "BackgroundMiddleImage";
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
		if (_iconPath != string.Empty)
		{
			inviteIcon = new GUIImage();
			inviteIcon.TextureSource = _iconPath;
			inviteIcon.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, _iconOffset);
			inviteIcon.SetSize(_iconSize);
		}
		GUILabel gUILabel = playerNameLabel;
		Vector2 position5 = backgroundLeft.Position;
		float x5 = position5.x;
		Vector2 size3 = backgroundLeft.Size;
		gUILabel.Position = new Vector2(x5 + size3.x, 30f);
		GUILabel gUILabel2 = inviteLabel;
		Vector2 position6 = playerNameLabel.Position;
		float x6 = position6.x;
		Vector2 position7 = playerNameLabel.Position;
		gUILabel2.Position = new Vector2(x6, position7.y + num - 5f);
		acceptButton = new GUIButton();
		acceptButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		acceptButton.SetSize(new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIButton gUIButton = acceptButton;
		Vector2 position8 = backgroundMiddle.Position;
		float x7 = position8.x;
		Vector2 size4 = backgroundMiddle.Size;
		float num3 = x7 + size4.x * 0.25f;
		Vector2 size5 = acceptButton.Size;
		gUIButton.SetPosition(new Vector2(num3 - size5.x * 0.5f, 50f));
		acceptButton.StyleInfo = new SHSButtonStyleInfo("notification_bundle|simple_ok_button");
		acceptButton.Id = "AcceptButton";
		acceptButton.ToolTip = new NamedToolTipInfo("#InvitationWindowAccept");
		acceptButton.HitTestType = HitTestTypeEnum.Circular;
		acceptButton.HitTestSize = new Vector2(0.6f, 0.6f);
		acceptButton.Click += acceptButton_Click;
		declineButton = new GUIButton();
		declineButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		declineButton.SetSize(new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIButton gUIButton2 = declineButton;
		Vector2 position9 = backgroundMiddle.Position;
		float x8 = position9.x;
		Vector2 size6 = backgroundMiddle.Size;
		float num4 = x8 + size6.x * 0.75f;
		Vector2 size7 = declineButton.Size;
		gUIButton2.SetPosition(new Vector2(num4 - size7.x * 0.5f, 50f));
		declineButton.StyleInfo = new SHSButtonStyleInfo("notification_bundle|simple_cancel_button");
		declineButton.ToolTip = new NamedToolTipInfo("#InvitationWindowDecline");
		declineButton.HitTestType = HitTestTypeEnum.Circular;
		declineButton.HitTestSize = new Vector2(0.6f, 0.6f);
		declineButton.Id = "DeclineButton";
		declineButton.Click += declineButton_Click;
		Add(backgroundLeft);
		Add(backgroundMiddle);
		Add(backgroundRight);
		Add(inviteIcon);
		Add(playerNameLabel);
		Add(inviteLabel);
		Add(acceptButton);
		Add(declineButton);
		Id = "SHSInvitationViewWindow";
		Vector2 size8 = backgroundLeft.Size;
		float x9 = size8.x;
		Vector2 size9 = backgroundRight.Size;
		float num5 = x9 + size9.x;
		Vector2 size10 = backgroundMiddle.Size;
		SetSize(new Vector2(num5 + size10.x, 145f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
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

	protected void HandleInvitationResponse(bool accept)
	{
		if (accept)
		{
			AppShell.Instance.Profile.AvailableFriends.AddFriend(getActualData().message.FriendID);
		}
		else
		{
			AppShell.Instance.Profile.AvailableFriends.DeclineFriend(getActualData().message.FriendID);
		}
		_parent.notificationComplete(this);
	}

	protected void declineButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		declineButton.IsEnabled = false;
		HandleInvitationResponse(false);
	}

	protected void acceptButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		acceptButton.IsEnabled = false;
		HandleInvitationResponse(true);
	}
}
