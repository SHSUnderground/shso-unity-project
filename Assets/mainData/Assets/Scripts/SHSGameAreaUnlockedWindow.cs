using System;
using UnityEngine;

public class SHSGameAreaUnlockedWindow : GUINotificationWindow
{
	private const float fadeStartTime = 3f;

	private const float fadeDurationTime = 0.5f;

	private const int logoOffset = 20;

	private const int textBlockOffset = 20;

	private const int textLineOffset = 10;

	private GUIImage backgroundLeft;

	private GUIImage backgroundMiddle;

	private GUIImage backgroundRight;

	private GUIImage inviteIcon;

	private GUILabel playerNameLabel;

	private GUILabel inviteLabel;

	private GUIButton acceptButton;

	private GUINotificationManager.GUINotificationStyleEnum windowStyle;

	private AssetBundleLoader.BundleGroup group;

	public SHSGameAreaUnlockedWindow(AssetBundleLoader.BundleGroup group)
	{
		windowStyle = GUINotificationManager.GUINotificationStyleEnum.GameAreaUnlocked;
		this.group = group;
		Traits.BlockTestType = BlockTestTypeEnum.Rect;
		Traits.HitTestType = HitTestTypeEnum.Rect;
		Traits.LifeSpan = ControlTraits.LifeSpanTraitEnum.DestroyOnHide;
		Traits.UpdateTrait = ControlTraits.UpdateTraitEnum.AlwaysUpdate;
	}

	public override bool InitializeResources(bool reload)
	{
		if (reload)
		{
			return base.InitializeResources(reload);
		}
		inviteLabel = new GUILabel();
		inviteLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
		SHSStagedDownloadWindow.StageState stageState = SHSStagedDownloadWindow.StageStatusList.Find(delegate(SHSStagedDownloadWindow.StageState testState)
		{
			return testState.assetBundleGroup == group;
		});
		if (stageState != null)
		{
			inviteLabel.Text = AppShell.Instance.stringTable[stageState.stage] + Environment.NewLine + AppShell.Instance.stringTable["#GAME_AREA_NOW_AVAILABLE_TOAST"];
		}
		else
		{
			inviteLabel.Text = "-";
		}
		inviteLabel.SetPosition(new Vector2(115f, 30f));
		inviteLabel.SetSize(new Vector2(142f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		inviteLabel.Id = "InviteLabel";
		backgroundLeft = new GUIImage();
		backgroundLeft.SetSize(new Vector2(108f, 142f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		backgroundLeft.Position = Vector2.zero;
		backgroundLeft.TextureSource = "GUI/Notifications/invitation_frame_left";
		backgroundLeft.Id = "BackgroundLeftImage";
		Add(backgroundLeft);
		float textBlockSize = GUINotificationWindow.GetTextBlockSize(new GUILabel[1]
		{
			inviteLabel
		}, BlockSizeType.Width);
		backgroundMiddle = new GUIImage();
		backgroundMiddle.SetSize(new Vector2(textBlockSize + 1f, 142f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
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
		inviteIcon = new GUIImage();
		inviteIcon.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(-60f, -60f), new Vector2(256f, 256f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		if (stageState != null && stageState.imagePath != null)
		{
			inviteIcon.TextureSource = stageState.imagePath;
			inviteIcon.Id = "InviteIcon";
			Add(inviteIcon);
		}
		Add(inviteLabel);
		acceptButton = new GUIButton();
		acceptButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		acceptButton.SetSize(new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIButton gUIButton = acceptButton;
		Vector2 position5 = backgroundMiddle.Position;
		float x5 = position5.x;
		Vector2 size3 = backgroundMiddle.Size;
		float num = x5 + size3.x * 0.75f;
		Vector2 size4 = acceptButton.Size;
		gUIButton.SetPosition(new Vector2(num - size4.x * 0.5f, 55f));
		acceptButton.StyleInfo = new SHSButtonStyleInfo("notification_bundle|simple_ok_button");
		acceptButton.Id = "AcceptButton";
		acceptButton.ToolTip = new NamedToolTipInfo("#InvitationWindowAccept");
		acceptButton.HitTestType = HitTestTypeEnum.Circular;
		acceptButton.HitTestSize = new Vector2(0.6f, 0.6f);
		acceptButton.Click += AcceptButtonClick;
		Add(acceptButton);
		base.OccupiedSlot = GUINotificationWindow.SlotsManager.AssignSlot(windowStyle, this);
		GUINotificationWindow.SlotsManager.AddOffset(base.OccupiedSlot, 142f);
		Vector2 offset = new Vector2(0f, 0f - GUINotificationWindow.SlotsManager.GetCurrentOffset(base.OccupiedSlot));
		Vector2 size5 = backgroundLeft.Size;
		float x6 = size5.x;
		Vector2 size6 = backgroundMiddle.Size;
		float num2 = x6 + size6.x;
		Vector2 size7 = backgroundRight.Size;
		float x7 = num2 + size7.x;
		Vector2 size8 = backgroundLeft.Size;
		float y = size8.y;
		Vector2 size9 = backgroundMiddle.Size;
		float a = Mathf.Max(y, size9.y);
		Vector2 size10 = backgroundRight.Size;
		SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, offset, new Vector2(x7, Mathf.Max(a, size10.y) + 15f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		return base.InitializeResources(reload);
	}

	private void AcceptButtonClick(GUIControl sender, GUIClickEvent EventData)
	{
		Hide();
	}

	public override void OnUpdate()
	{
		float num = Time.time - timeStarted;
		float num2 = num - 3f;
		if (num > 3f)
		{
			Alpha = 1f - num2 / 0.5f;
		}
		if (Alpha <= 0f)
		{
			Hide();
		}
	}

	public override void Hide()
	{
		if (!base.WindowOverwrite)
		{
			GUINotificationWindow.WindowHandler.ClearWindowOfType(windowStyle);
			GUINotificationWindow.SlotsManager.UnassignSlot(base.OccupiedSlot);
		}
		base.Hide();
	}
}
