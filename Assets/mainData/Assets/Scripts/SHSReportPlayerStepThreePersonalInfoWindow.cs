using UnityEngine;

public class SHSReportPlayerStepThreePersonalInfoWindow : GUIWindow
{
	private GUIImage backgroundImage;

	private GUIDropShadowTextLabel titleLabel;

	private GUIImage iconImage;

	private GUIButton closeButton;

	private GUIButton yesButton;

	private GUIButton noButton;

	private GUILabel messageLabel;

	private GUIRadioButtonList reasonButtonList;

	public SHSReportPlayerStepThreePersonalInfoWindow()
	{
		backgroundImage = new GUIImage();
		backgroundImage.SetPosition(QuickSizingHint.Centered);
		backgroundImage.SetSize(new Vector2(494f, 432f));
		backgroundImage.TextureSource = "gameworld_bundle|report_notification_background_large";
		Add(backgroundImage);
		titleLabel = new GUIDropShadowTextLabel();
		titleLabel.SetPositionAndSize(130f, 74f, 305f, 168f);
		titleLabel.FontFace = GUIFontManager.SupportedFontEnum.Zooom;
		titleLabel.FontSize = 29;
		titleLabel.TextAlignment = TextAnchor.UpperCenter;
		titleLabel.FrontColor = new Color(226f / 255f, 92f / 255f, 16f / 255f);
		titleLabel.BackColor = new Color(0f, 4f / 51f, 14f / 51f, 0.1f);
		titleLabel.TextOffset = new Vector2(2f, 2f);
		titleLabel.Text = "#rap_step_one_title";
		Add(titleLabel);
		iconImage = new GUIImage();
		iconImage.SetPositionAndSize(-11f, 5f, 163f, 144f);
		iconImage.TextureSource = "common_bundle|notification_icon_block";
		Add(iconImage);
		closeButton = GUIControl.CreateControlAbsolute<GUIButton>(new Vector2(45f, 45f), new Vector2(457f, 50f));
		closeButton.Click += delegate
		{
			parent.Hide();
		};
		closeButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|button_close");
		closeButton.ToolTip = new NamedToolTipInfo(SHSTooltip.GetCommonToolTipText(SHSTooltip.CommonToolTipText.Close));
		Add(closeButton);
		messageLabel = new GUILabel();
		messageLabel.SetPositionAndSize(98f, 132f, 350f, 350f);
		messageLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, new Color(26f / 85f, 32f / 85f, 109f / 255f), TextAnchor.UpperLeft);
		messageLabel.Text = string.Empty;
		Add(messageLabel);
		reasonButtonList = new GUIRadioButtonList();
		reasonButtonList.SetPositionAndSize(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-43f, 361f), new Vector2(400f, 400f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(reasonButtonList);
		GUIToggleButton gUIToggleButton = new GUIToggleButton();
		gUIToggleButton.Label.Text = "#rap_step_three_phone";
		gUIToggleButton.Label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, new Color(26f / 85f, 32f / 85f, 109f / 255f), TextAnchor.UpperLeft);
		gUIToggleButton.ButtonStyleInfo = new SHSButtonStyleInfo("gameworld_bundle|options_radio_button");
		gUIToggleButton.SetButtonSize(new Vector2(64f, 64f));
		gUIToggleButton.Spacing = 60f;
		gUIToggleButton.VertSpacing = 5f;
		gUIToggleButton.SupportsHover = true;
		gUIToggleButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(56f, 10f), new Vector2(350f, 53f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		reasonButtonList.AddButton(gUIToggleButton);
		gUIToggleButton = new GUIToggleButton();
		gUIToggleButton.Label.Text = "#rap_step_three_name";
		gUIToggleButton.Label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, new Color(26f / 85f, 32f / 85f, 109f / 255f), TextAnchor.UpperLeft);
		gUIToggleButton.ButtonStyleInfo = new SHSButtonStyleInfo("gameworld_bundle|options_radio_button");
		gUIToggleButton.SetButtonSize(new Vector2(64f, 64f));
		gUIToggleButton.Spacing = 60f;
		gUIToggleButton.VertSpacing = 5f;
		gUIToggleButton.SupportsHover = true;
		gUIToggleButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(56f, 60f), new Vector2(350f, 53f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		reasonButtonList.AddButton(gUIToggleButton);
		gUIToggleButton = new GUIToggleButton();
		gUIToggleButton.Label.Text = "#rap_step_three_email";
		gUIToggleButton.Label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, new Color(26f / 85f, 32f / 85f, 109f / 255f), TextAnchor.UpperLeft);
		gUIToggleButton.ButtonStyleInfo = new SHSButtonStyleInfo("gameworld_bundle|options_radio_button");
		gUIToggleButton.SetButtonSize(new Vector2(64f, 64f));
		gUIToggleButton.Spacing = 60f;
		gUIToggleButton.VertSpacing = 5f;
		gUIToggleButton.SupportsHover = true;
		gUIToggleButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(56f, 110f), new Vector2(350f, 53f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		reasonButtonList.AddButton(gUIToggleButton);
		gUIToggleButton = new GUIToggleButton();
		gUIToggleButton.Label.Text = "#rap_step_na";
		gUIToggleButton.Label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, new Color(26f / 85f, 32f / 85f, 109f / 255f), TextAnchor.UpperLeft);
		gUIToggleButton.ButtonStyleInfo = new SHSButtonStyleInfo("gameworld_bundle|options_radio_button");
		gUIToggleButton.SetButtonSize(new Vector2(64f, 64f));
		gUIToggleButton.Spacing = 60f;
		gUIToggleButton.VertSpacing = 5f;
		gUIToggleButton.SupportsHover = true;
		gUIToggleButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(56f, 160f), new Vector2(350f, 53f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		reasonButtonList.AddButton(gUIToggleButton);
		yesButton = new GUIButton();
		yesButton.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, new Vector2(42f, 3f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		yesButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_yes");
		yesButton.Text = string.Empty;
		yesButton.Click += yesButton_Click;
		Add(yesButton);
		noButton = new GUIButton();
		noButton.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(17f, 2f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		noButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_no");
		noButton.Text = string.Empty;
		noButton.Click += noButton_Click;
		Add(noButton);
	}

	private void yesButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		SHSReportPlayerDialogWindow sHSReportPlayerDialogWindow = (SHSReportPlayerDialogWindow)parent;
		sHSReportPlayerDialogWindow.reportSubCause = reasonButtonList.SelectedButton.Text;
		sHSReportPlayerDialogWindow.ReportStepCompleted("StepFourWindow");
	}

	private void noButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		parent.Hide();
	}

	public override void OnShow()
	{
		messageLabel.Text = string.Format("#rap_step_three_pi_query");
		base.OnShow();
	}
}
