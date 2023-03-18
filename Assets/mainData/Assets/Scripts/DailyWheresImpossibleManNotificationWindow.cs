using UnityEngine;

public class DailyWheresImpossibleManNotificationWindow : GeneralNotificationWindow
{
	protected GUIImage _icon2;

	public DailyWheresImpossibleManNotificationWindow()
		: base(NotificationData.NotificationType.DailyWheresImpossibleMan, "GUI/Notifications/baseWindow", "GUI/Notifications/globe", new Vector2(60f, 61f), new Vector2(2f, 1f), "#CIN_IMPOSSIBLE_MAN_PLAYABLE_EXNM", "Found")
	{
		_icon2 = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 64f), Vector2.zero);
		_icon2.TextureSource = "characters_bundle|token_impossible_man_playable";
		_icon2.Position = new Vector2(16f, 16f);
		Add(_icon2);
		_helpInfo = HelpNotificationWindow.HelpInfo.DailyImpossibleMan;
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
		DailyWheresImpossibleManNotificationData dailyWheresImpossibleManNotificationData = (DailyWheresImpossibleManNotificationData)data;
		if (dailyWheresImpossibleManNotificationData == null)
		{
			CspUtils.DebugLog("ERROR: DailyWheresImpossibleManNotificationData called with an invalid data record " + data);
			return;
		}
		_mainLabel.Text = dailyWheresImpossibleManNotificationData.getItemsFound().ToString() + "/" + dailyWheresImpossibleManNotificationData.getMaxItems().ToString();
		recenterLabel(_mainLabel);
	}
}
