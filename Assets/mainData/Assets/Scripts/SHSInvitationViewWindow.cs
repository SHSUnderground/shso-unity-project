using System;
using System.Collections;
using UnityEngine;

public class SHSInvitationViewWindow : SHSInvitationWindow
{
	private struct MetricCacheInfo
	{
		public Vector2 cachedPosition;

		public Vector2 cachedSize;

		public int cachedFontSize;

		public Color cachedColor;

		public MetricCacheInfo(Vector2 cachedPosition, Vector2 cachedSize)
		{
			this.cachedPosition = cachedPosition;
			this.cachedSize = cachedSize;
			cachedFontSize = 0;
			cachedColor = Color.white;
		}
	}

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

	private Vector2 windowSize;

	private Hashtable uiCacheLookup = new Hashtable();

	private SHSInvitationShell shell;

	public SHSInvitationShell Shell
	{
		get
		{
			return shell;
		}
	}

	public string InviteeName
	{
		get
		{
			return playerNameLabel.Text;
		}
		set
		{
			playerNameLabel.Text = value;
		}
	}

	public string InviteDescription
	{
		get
		{
			return inviteLabel.Text;
		}
		set
		{
			inviteLabel.Text = value;
		}
	}

	public Vector2 WindowSize
	{
		get
		{
			return windowSize;
		}
	}

	public Color InviteDescriptionColor
	{
		get
		{
			return inviteLabel.Color;
		}
		set
		{
			inviteLabel.Color = value;
		}
	}

	public bool AcceptEnable
	{
		get
		{
			return acceptButton.IsEnabled;
		}
		set
		{
			acceptButton.IsEnabled = value;
		}
	}

	public bool DeclineEnable
	{
		get
		{
			return declineButton.IsEnabled;
		}
		set
		{
			declineButton.IsEnabled = value;
		}
	}

	public SHSInvitationViewWindow(SHSInvitationShell shell)
	{
		this.shell = shell;
		Traits.BlockTestType = BlockTestTypeEnum.Rect;
		Traits.HitTestType = HitTestTypeEnum.Rect;
	}

	public override void SetCustomSize(float scale)
	{
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Expected O, but got Unknown
		base.SetCustomSize(scale);
		foreach (IGUIControl control in ControlList)
		{
			if (uiCacheLookup.ContainsKey(control.Id))
			{
				MetricCacheInfo metricCacheInfo = (MetricCacheInfo)uiCacheLookup[control.Id];
				((GUIControl)control).Position = new Vector2(metricCacheInfo.cachedPosition.x * scale, metricCacheInfo.cachedPosition.y * scale);
				((GUIControl)control).Size = new Vector2(metricCacheInfo.cachedSize.x * scale, metricCacheInfo.cachedSize.y * scale);
				if (control is GUILabel)
				{
					((GUILabel)control).SetupText(GUIFontManager.SupportedFontEnum.Komica, (int)((float)metricCacheInfo.cachedFontSize * scale), metricCacheInfo.cachedColor, TextAnchor.UpperLeft);
				}
			}
		}
		inviteIcon.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(iconOffset.x * scale, iconOffset.y * scale), new Vector2(iconSize.x * scale, iconSize.y * scale), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Size = new Vector2(windowSize.x * scale, windowSize.y * scale);
		bool makeButtonsActive = scale >= 0.999f;
		AnimClip animClip = AnimClipBuilder.Absolute.Nothing(AnimClipBuilder.Path.Linear(0f, 0f, 0.2f), this);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			acceptButton.Traits.EventHandlingTrait = ((!makeButtonsActive) ? ControlTraits.EventHandlingEnum.Ignore : ControlTraits.EventHandlingEnum.Block);
			declineButton.Traits.EventHandlingTrait = ((!makeButtonsActive) ? ControlTraits.EventHandlingEnum.Ignore : ControlTraits.EventHandlingEnum.Block);
		};
		base.AnimationPieceManager.Add(animClip);
	}

	public void BuildWindow(string inviterName, string invitationMessage)
	{
		BuildWindow(inviterName, invitationMessage, AssetBundleLoader.BundleGroup.Any);
	}

	public void BuildWindow(string inviterName, string invitationMessage, AssetBundleLoader.BundleGroup bundleDependency)
	{
		GUIContent gUIContent = new GUIContent();
		float num = 0f;
		playerNameLabel = new GUILabel();
		playerNameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 249, 157), TextAnchor.UpperLeft);
		playerNameLabel.Text = inviterName;
		gUIContent.text = playerNameLabel.Text;
		Vector2 vector = playerNameLabel.Style.UnityStyle.CalcSize(gUIContent);
		playerNameLabel.SetSize(new Vector2(vector.x, vector.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		playerNameLabel.Id = "PlayerNameLabel";
		num += vector.y;
		inviteLabel = new GUILabel();
		inviteLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
		inviteLabel.Text = invitationMessage;
		gUIContent.text = inviteLabel.Text;
		vector = inviteLabel.Style.UnityStyle.CalcSize(gUIContent);
		inviteLabel.SetSize(new Vector2(vector.x, vector.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		inviteLabel.Id = "InviteLabel";
		backgroundLeft = new GUIImage();
		backgroundLeft.SetSize(new Vector2(108f, 142f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		backgroundLeft.Position = Vector2.zero;
		backgroundLeft.TextureSource = "GUI/Notifications/invitation_frame_left";
		backgroundLeft.Id = "BackgroundLeftImage";
		uiCacheLookup.Add(backgroundLeft.Id, new MetricCacheInfo(backgroundLeft.Position, backgroundLeft.Size));
		float num2 = GUINotificationWindow.GetTextBlockSize(new GUILabel[2]
		{
			playerNameLabel,
			inviteLabel
		}, BlockSizeType.Width);
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
		uiCacheLookup.Add(backgroundMiddle.Id, new MetricCacheInfo(backgroundMiddle.Position, backgroundMiddle.Size));
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
		uiCacheLookup.Add(backgroundRight.Id, new MetricCacheInfo(backgroundRight.Position, backgroundRight.Size));
		if (inviteIcon == null)
		{
			inviteIcon = new GUIImage();
			inviteIcon.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
		}
		GUILabel gUILabel = playerNameLabel;
		Vector2 position5 = backgroundLeft.Position;
		float x5 = position5.x;
		Vector2 size3 = backgroundLeft.Size;
		gUILabel.Position = new Vector2(x5 + size3.x, 30f);
		uiCacheLookup.Add(playerNameLabel.Id, new MetricCacheInfo(playerNameLabel.Position, playerNameLabel.Size));
		MetricCacheInfo metricCacheInfo = (MetricCacheInfo)uiCacheLookup[playerNameLabel.Id];
		metricCacheInfo.cachedColor = GUILabel.GenColor(255, 249, 157);
		metricCacheInfo.cachedFontSize = 18;
		uiCacheLookup[playerNameLabel.Id] = metricCacheInfo;
		GUILabel gUILabel2 = inviteLabel;
		Vector2 position6 = playerNameLabel.Position;
		float x6 = position6.x;
		Vector2 position7 = playerNameLabel.Position;
		gUILabel2.Position = new Vector2(x6, position7.y + num - 5f);
		uiCacheLookup.Add(inviteLabel.Id, new MetricCacheInfo(inviteLabel.Position, inviteLabel.Size));
		metricCacheInfo = (MetricCacheInfo)uiCacheLookup[inviteLabel.Id];
		metricCacheInfo.cachedColor = GUILabel.GenColor(255, 255, 255);
		metricCacheInfo.cachedFontSize = 14;
		uiCacheLookup[inviteLabel.Id] = metricCacheInfo;
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
		if (bundleDependency != AssetBundleLoader.BundleGroup.Any)
		{
			acceptButton.ConfigureRequiredContent(new ContentReference(ContentTypeEnum.PriorityGroup, bundleDependency));
		}
		uiCacheLookup.Add(acceptButton.Id, new MetricCacheInfo(acceptButton.Position, acceptButton.Size));
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
		uiCacheLookup.Add(declineButton.Id, new MetricCacheInfo(declineButton.Position, declineButton.Size));
		Add(backgroundLeft);
		Add(backgroundMiddle);
		Add(backgroundRight);
		Add(inviteIcon);
		Add(playerNameLabel);
		Add(inviteLabel);
		Add(acceptButton);
		Add(declineButton);
		Id = "SHSInvitationViewWindow";
		Vector2 zero = Vector2.zero;
		Vector2 size8 = backgroundLeft.Size;
		float x9 = size8.x;
		Vector2 size9 = backgroundRight.Size;
		float num5 = x9 + size9.x;
		Vector2 size10 = backgroundMiddle.Size;
		SetPositionAndSize(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, zero, new Vector2(num5 + size10.x, 142f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		windowSize = Size;
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		SetInvitationInitialState();
	}

	public void IconSetup(string textureSource, Vector2 offset, Vector2 size)
	{
		inviteIcon = new GUIImage();
		inviteIcon.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, offset, size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		iconOffset = inviteIcon.Offset;
		iconSize = inviteIcon.Size;
		inviteIcon.TextureSource = textureSource;
		inviteIcon.Id = "InviteIcon";
	}

	public void AddAcceptEvent(MouseClickDelegate clickDelegate)
	{
		acceptButton.Click += clickDelegate;
	}

	public void AddDeclineEvent(MouseClickDelegate clickDelegate)
	{
		declineButton.Click += clickDelegate;
	}
}
