using UnityEngine;

public class SHSReportPlayerStepOneWindow : GUIWindow
{
	private GUIImage backgroundImage;

	private GUIDropShadowTextLabel titleLabel;

	private GUIImage iconImage;

	private GUIButton closeButton;

	private GUIButton yesButton;

	private GUIButton noButton;

	private GUILabel messageLabel;

	private GUILabel messageQuestionLabel;

	public SHSReportPlayerStepOneWindow()
	{
		backgroundImage = new GUIImage();
		backgroundImage.SetPosition(QuickSizingHint.Centered);
		backgroundImage.SetSize(new Vector2(494f, 352f));
		backgroundImage.TextureSource = "gameworld_bundle|report_notification_background_small";
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
		closeButton = GUIControl.CreateControlAbsolute<GUIButton>(new Vector2(45f, 45f), new Vector2(457f, 42f));
		closeButton.Click += delegate
		{
			parent.Hide();
		};
		closeButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|button_close");
		closeButton.ToolTip = new NamedToolTipInfo(SHSTooltip.GetCommonToolTipText(SHSTooltip.CommonToolTipText.Close));
		Add(closeButton);
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
		messageLabel = new GUILabel();
		messageLabel.SetPositionAndSize(92f, 148f, 350f, 350f);
		messageLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, new Color(26f / 85f, 32f / 85f, 109f / 255f), TextAnchor.UpperLeft);
		messageLabel.Text = "#rap_step_one_warning";
		Add(messageLabel);
		messageQuestionLabel = new GUILabel();
		messageQuestionLabel.SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft);
		messageQuestionLabel.Offset = new Vector2(92f, -127f);
		messageQuestionLabel.SetSize(new Vector2(400f, 30f));
		messageQuestionLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, new Color(26f / 85f, 32f / 85f, 109f / 255f), TextAnchor.UpperLeft);
		messageQuestionLabel.Bold = true;
		messageQuestionLabel.Text = "#rap_step_one_confirm";
		Add(messageQuestionLabel);
	}

	private void yesButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		((SHSReportPlayerDialogWindow)parent).ReportStepCompleted("StepTwoWindow");
	}

	private void noButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		parent.Hide();
	}
}
