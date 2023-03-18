using UnityEngine;

public class SHSBlockOrReportPlayerDialogWindow : SHSCommonDialogWindow
{
	public GUIButton blockButton;

	public GUIButton reportButton;

	public SHSBlockOrReportPlayerDialogWindow()
		: base("common_bundle|notification_icon_blocking", new Vector2(153f, 238f), string.Empty, "common_bundle|L_mshs_button_yes", "common_bundle|L_mshs_button_cancel", typeof(SHSDialogOkButton), typeof(SHSDialogCancelButton), false)
	{
		Text = string.Empty;
		type = NotificationType.Common;
		iconOffset.x = 27f;
		iconOffset.y = 3f;
		blockButton = new GUIButton();
		blockButton.Id = "Block Button";
		blockButton.SetPositionAndSize(163f, 120f, 259f, 231f);
		blockButton.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|L_block_button");
		blockButton.Click += blockButton_Click;
		Add(blockButton);
		reportButton = new GUIButton();
		reportButton.Id = "Report Button";
		reportButton.SetPositionAndSize(420f, 120f, 259f, 231f);
		reportButton.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|L_report_button");
		reportButton.Click += reportButton_Click;
		Add(reportButton);
	}

	private void reportButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		((GUIButton)sender).IsSelected = true;
		blockButton.IsSelected = false;
		okButton.IsEnabled = true;
	}

	private void blockButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		((GUIButton)sender).IsSelected = true;
		reportButton.IsSelected = false;
		okButton.IsEnabled = true;
	}

	public override void OnShow()
	{
		okButton.IsEnabled = false;
		TitleText = "#blockorreport_title";
		SetupBundleDependencies();
		BuildTextBlock();
		BuildBackground(new Vector2(390f, 270f));
		FinalizeAllUIPositioning();
		SetPosition(QuickSizingHint.Centered);
		SetSize(new Vector2(743f, 461f));
	}
}
