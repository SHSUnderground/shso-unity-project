using UnityEngine;

public class DailyScavengerNotificationWindow : GeneralNotificationWindow
{
	protected GUIImage _icon2;

	public DailyScavengerNotificationWindow()
		: base(NotificationData.NotificationType.DailyScavenger, "GUI/Notifications/baseWindow", "GUI/Notifications/globe", new Vector2(60f, 61f), new Vector2(2f, 1f), "Crafting Parts", "Collected")
	{
		_icon2 = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 64f), Vector2.zero);
		_icon2.TextureSource = "shopping_bundle|craft_generic";
		_icon2.Position = new Vector2(16f, 16f);
		Add(_icon2);
		_helpInfo = HelpNotificationWindow.HelpInfo.DailyScavenger;
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
		DailyScavengerNotificationData dailyScavengerNotificationData = (DailyScavengerNotificationData)data;
		if (dailyScavengerNotificationData == null)
		{
			CspUtils.DebugLog("ERROR: DailyTokenNotificationWindow called with an invalid data record " + data);
			return;
		}
		_mainLabel.Text = dailyScavengerNotificationData.getItemsFound().ToString() + "/" + dailyScavengerNotificationData.getMaxItems().ToString();
		recenterLabel(_mainLabel);
	}
}
