using UnityEngine;

public class DailyTokenNotificationWindow : GeneralNotificationWindow
{
	protected GUIImage _faceIcon;

	public DailyTokenNotificationWindow()
		: base(NotificationData.NotificationType.DailyTokens, "GUI/Notifications/gameworld_pickup_toast_herotokens", "GUI/Notifications/herobg", new Vector2(93f, 69f), new Vector2(-10f, 2f), "Daily Tokens", "Collected")
	{
		_helpInfo = HelpNotificationWindow.HelpInfo.DailyToken;
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
		DailyTokenNotificationData dailyTokenNotificationData = (DailyTokenNotificationData)data;
		if (dailyTokenNotificationData == null)
		{
			CspUtils.DebugLog("ERROR: DailyTokenNotificationWindow called with an invalid data record " + data);
			return;
		}
		_mainLabel.Text = dailyTokenNotificationData.getTokensFound().ToString() + "/" + dailyTokenNotificationData.getMaxTokens().ToString();
		recenterLabel(_mainLabel);
		if (_faceIcon == null)
		{
			_faceIcon = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 64f), Vector2.zero);
			_faceIcon.TextureSource = "characters_bundle|token_" + dailyTokenNotificationData.getHero();
			_faceIcon.Position = new Vector2(16f, 16f);
			Add(_faceIcon);
		}
	}
}
