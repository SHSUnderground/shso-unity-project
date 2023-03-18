using UnityEngine;

public class SHSFeatherViewerWindow : GUINotificationWindow
{
	private const float fadeStartTime = 1f;

	private const float fadeDurationTime = 0.5f;

	private SHSCurrencyWindow featherWindow;

	private int collectedCount;

	private int totalCount;

	public SHSFeatherViewerWindow(int collectedCount, int totalCount)
	{
		this.collectedCount = collectedCount;
		this.totalCount = totalCount;
		featherWindow = new SHSCurrencyWindow();
		featherWindow.SetPositionAndSize(0f, 0f, 227f, 119f);
		featherWindow.BgdTextureSource = "notification_bundle|inventory_currency_bg_all_f01";
		featherWindow.IconTextureSource = "common_bundle|wip_attractive";
		featherWindow.LabelTextureSource = "common_bundle|wip_attractive";
		featherWindow.CurrencyLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, Color.black, TextAnchor.MiddleCenter);
		Add(featherWindow);
	}

	public override void OnShow()
	{
		base.OnShow();
		SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(5f, -275f), new Vector2(227f, 119f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		foreach (IGUIControl control in featherWindow.ControlList)
		{
			control.Alpha = 1f;
		}
	}

	public override void OnUpdate()
	{
		float num = Time.time - timeStarted;
		float num2 = num - 1f;
		if (collectedCount > 0)
		{
			featherWindow.CurrencyLabel.Text = string.Format(AppShell.Instance.stringTable["#feather_collection"], collectedCount.ToString(), totalCount.ToString());
		}
		else
		{
			featherWindow.CurrencyLabel.Text = "#feather_error";
		}
		if (num > 1f)
		{
			Alpha = 1f - num2 / 0.5f;
			foreach (IGUIControl control in featherWindow.ControlList)
			{
				control.Alpha = 1f - num2 / 0.5f;
			}
		}
		if (Alpha <= 0f)
		{
			Hide();
		}
	}
}
