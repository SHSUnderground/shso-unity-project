using UnityEngine;

public class SHSMembershipUpsellBumperWindow : SHSCommonDialogWindow
{
	private GUIImage shieldImage;

	public SHSMembershipUpsellBumperWindow()
		: base("common_bundle|L_mshs_button_ok", "common_bundle|L_mshs_button_cancel", typeof(SHSDialogOkButton), typeof(SHSDialogCancelButton))
	{
		TitleText = "#openchat_not_available_title";
		Text = string.Empty;
		type = NotificationType.Common;
		shieldImage = new GUIImage();
		shieldImage.SetPositionAndSize(20f, 70f, 140f, 140f);
		shieldImage.TextureSource = "mysquadgadget_bundle|L_shield_agent_logo_01";
		Add(shieldImage);
	}

	public override void OnShow()
	{
		SetupBundleDependencies();
		Vector2 vector = BuildTextBlock();
		BuildBackground(new Vector2(vector.x + 35f, vector.y + 20f));
		FinalizeAllUIPositioning();
	}
}
