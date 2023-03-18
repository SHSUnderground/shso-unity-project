using UnityEngine;

public class SHSOpenChatUnavailableDialogWindow : SHSCommonDialogWindow
{
	private GUIImage cardImage;

	public SHSOpenChatUnavailableDialogWindow()
		: base("common_bundle|L_mshs_button_ok", typeof(SHSDialogOkButton))
	{
		TitleText = "#openchat_not_available_title";
		Text = string.Empty;
		type = NotificationType.Common;
		cardImage = new GUIImage();
		cardImage.SetPositionAndSize(20f, 20f, 256f, 256f);
		cardImage.TextureSource = "hud_bundle|chat08";
		Add(cardImage);
	}

	public override void OnShow()
	{
		SetupBundleDependencies();
		Vector2 vector = BuildTextBlock();
		BuildBackground(new Vector2(vector.x + 25f, vector.y + 100f));
		FinalizeAllUIPositioning();
	}
}
