using UnityEngine;

public class SHSSubmitClientLogWindow : SHSCommonDialogWindow
{
	private GUIImage iconImage;

	private bool pendingClose;

	private float timeTillClose;

	public SHSSubmitClientLogWindow()
		: base("common_bundle|notification_icon_sendbug", new Vector2(137f, 234f), string.Empty, "common_bundle|L_mshs_button_send_it", "common_bundle|L_mshs_button_cancel", typeof(SHSDialogOkButton), typeof(SHSDialogCancelButton), false)
	{
		type = NotificationType.Common;
		pendingClose = false;
		timeTillClose = -1f;
		iconImage = new GUIImage();
		iconImage.SetPositionAndSize(20f, 20f, 163f, 144f);
		iconImage.TextureSource = "common_bundle|notification_icon_block";
		Add(iconImage);
	}

	public override void OnShow()
	{
		TitleText = "#submit_report_title";
		Text = "#submit_report_blurb";
		SetupBundleDependencies();
		Vector2 vector = BuildTextBlock();
		BuildBackground(new Vector2(vector.x, vector.y + 100f));
		FinalizeAllUIPositioning();
	}

	public override void OnOk()
	{
		PostLogFile.PostToServer();
		okButton.IsEnabled = false;
		pendingClose = true;
		timeTillClose = 4f;
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (pendingClose)
		{
			timeTillClose -= Time.deltaTime;
			if (timeTillClose <= 0f || PostLogFile.IsDone)
			{
				base.OnOk();
			}
		}
	}
}
