using UnityEngine;

public class SHSFeatureNotImplementedDialogWindow : SHSCommonDialogWindow
{
	public SHSFeatureNotImplementedDialogWindow()
		: base("common_bundle|notification_icon_sorry", "common_bundle|L_mshs_button_ok", typeof(SHSDialogOkButton), false)
	{
		TitleText = "Feature Not Available";
		Text = string.Empty;
		type = NotificationType.Error;
		iconOffset.x = 65f;
		iconOffset.y = -0f;
	}

	public override void OnShow()
	{
		SetupBundleDependencies();
		Vector2 vector = BuildTextBlock();
		BuildBackground(new Vector2(vector.x, vector.y + 100f));
		FinalizeAllUIPositioning();
	}
}
