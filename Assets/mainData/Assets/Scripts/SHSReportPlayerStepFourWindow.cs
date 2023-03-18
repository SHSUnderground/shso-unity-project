using UnityEngine;

public class SHSReportPlayerStepFourWindow : GUIWindow
{
	private GUIImage backgroundImage;

	private GUIImage textInputImage;

	private GUITextField inputText;

	private GUIDropShadowTextLabel titleLabel;

	private GUIImage iconImage;

	private GUIButton closeButton;

	private GUIButton okButton;

	private GUILabel messageLabel;

	public SHSReportPlayerStepFourWindow()
	{
		backgroundImage = new GUIImage();
		backgroundImage.SetPosition(QuickSizingHint.Centered);
		backgroundImage.SetSize(new Vector2(494f, 432f));
		backgroundImage.TextureSource = "gameworld_bundle|report_notification_background_large";
		Add(backgroundImage);
		textInputImage = new GUIImage();
		textInputImage.SetPosition(90f, 195f);
		textInputImage.SetSize(new Vector2(368f, 160f));
		textInputImage.TextureSource = "gameworld_bundle|report_text_entry_field";
		Add(textInputImage);
		titleLabel = new GUIDropShadowTextLabel();
		titleLabel.SetPositionAndSize(130f, 74f, 305f, 168f);
		titleLabel.FontFace = GUIFontManager.SupportedFontEnum.Zooom;
		titleLabel.FontSize = 29;
		titleLabel.TextAlignment = TextAnchor.UpperCenter;
		titleLabel.FrontColor = new Color(226f / 255f, 92f / 255f, 16f / 255f);
		titleLabel.BackColor = new Color(0f, 4f / 51f, 14f / 51f, 0.1f);
		titleLabel.TextOffset = new Vector2(2f, 2f);
		titleLabel.Text = "#rap_step_four_title";
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
		messageLabel.Text = "#rap_summary";
		Add(messageLabel);
		inputText = new GUITextField();
		inputText.SetPosition(100f, 205f);
		inputText.SetSize(new Vector2(348f, 143f));
		inputText.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, new Color(26f / 85f, 32f / 85f, 109f / 255f), TextAnchor.UpperLeft);
		inputText.Text = string.Empty;
		Add(inputText);
		okButton = new GUIButton();
		okButton.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 3f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		okButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_ok");
		okButton.Text = string.Empty;
		okButton.Click += okButton_Click;
		Add(okButton);
	}

	private void okButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		SHSReportPlayerDialogWindow sHSReportPlayerDialogWindow = (SHSReportPlayerDialogWindow)parent;
		sHSReportPlayerDialogWindow.reportComments = inputText.Text;
		sHSReportPlayerDialogWindow.reportPlayerConfirmed = true;
		sHSReportPlayerDialogWindow.ReportStepCompleted(string.Empty);
	}
}
