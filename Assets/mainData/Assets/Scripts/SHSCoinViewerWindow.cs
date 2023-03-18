using UnityEngine;

public class SHSCoinViewerWindow : GUINotificationWindow
{
	private float fadeStartTime = 1f;

	private float fadeDurationTime = 0.5f;

	private SHSCurrencyWindow coinWindow;

	private UserProfile profile;

	private static int offlineProfileCoinUpdateFakeage;

	private bool firstUpdate;

	public SHSCoinViewerWindow()
	{
		coinWindow = new SHSCurrencyWindow();
		coinWindow.SetPositionAndSize(0f, 0f, 227f, 119f);
		coinWindow.BgdTextureSource = "notification_bundle|inventory_currency_bg_all_f01";
		coinWindow.IconTextureSource = "notification_bundle|inventory_currency_coins_icon_f01";
		coinWindow.LabelTextureSource = "notification_bundle|inventory_currency_coins_title_f01";
		coinWindow.CurrencyLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, Color.black, TextAnchor.MiddleCenter);
		Add(coinWindow);
	}

	public override void OnShow()
	{
		profile = AppShell.Instance.Profile;
		base.OnShow();
		SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(5f, -150f), new Vector2(227f, 119f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		foreach (IGUIControl control in coinWindow.ControlList)
		{
			control.Alpha = 1f;
		}
	}

	public override void OnUpdate()
	{
		float num = Time.time - timeStarted;
		float num2 = num - fadeStartTime;
		if (profile != null)
		{
			coinWindow.CurrencyLabel.Text = profile.Silver.ToString();
		}
		else if (!firstUpdate)
		{
			GUILabel currencyLabel = coinWindow.CurrencyLabel;
			int num3 = ++offlineProfileCoinUpdateFakeage;
			currencyLabel.Text = num3.ToString();
			firstUpdate = true;
		}
		if (num > fadeStartTime)
		{
			Alpha = 1f - num2 / fadeDurationTime;
			foreach (IGUIControl control in coinWindow.ControlList)
			{
				control.Alpha = 1f - num2 / fadeDurationTime;
			}
		}
		if (Alpha <= 0f)
		{
			firstUpdate = false;
			Hide();
		}
	}
}
