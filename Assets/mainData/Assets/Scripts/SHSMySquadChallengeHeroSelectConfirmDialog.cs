using UnityEngine;

public class SHSMySquadChallengeHeroSelectConfirmDialog : SHSCommonDialogWindow
{
	private GUIImage heroSelectImage;

	public SHSMySquadChallengeHeroSelectConfirmDialog()
		: base("common_bundle|L_mshs_button_ok", "common_bundle|L_mshs_button_cancel", typeof(SHSDialogOkButton), typeof(SHSDialogCancelButton))
	{
		type = NotificationType.Common;
		heroSelectImage = new GUIImage();
		heroSelectImage.SetPositionAndSize(7f, 20f, 184f, 154f);
		heroSelectImage.TextureSource = "common_bundle|notification_pickthishero";
		Add(heroSelectImage);
	}

	public override void OnShow()
	{
		TitleText = "#challenge_heroselect_confirm_title";
		Text = string.Empty;
		SetupBundleDependencies();
		BuildTextBlock();
		BuildBackground(new Vector2(340f, 110f));
		SetPosition(QuickSizingHint.Centered);
		FinalizeAllUIPositioning();
	}
}
