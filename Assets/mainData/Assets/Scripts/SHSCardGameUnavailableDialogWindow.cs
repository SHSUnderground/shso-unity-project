using UnityEngine;

public class SHSCardGameUnavailableDialogWindow : SHSCommonDialogWindow
{
	private GUIImage cardImage;

	public SHSCardGameUnavailableDialogWindow()
		: base("common_bundle|L_mshs_button_ok", typeof(SHSDialogOkButton))
	{
		TitleText = "#cardgame_not_available_title";
		Text = string.Empty;
		type = NotificationType.Common;
		cardImage = new GUIImage();
		cardImage.SetPositionAndSize(20f, 20f, 159f, 198f);
		cardImage.TextureSource = "common_bundle|notification_icon_cardgamenogo";
		Add(cardImage);
	}

	public override void OnShow()
	{
		SetupBundleDependencies();
		Vector2 vector = BuildTextBlock();
		BuildBackground(new Vector2(vector.x, vector.y + 100f));
		FinalizeAllUIPositioning();
	}
}
