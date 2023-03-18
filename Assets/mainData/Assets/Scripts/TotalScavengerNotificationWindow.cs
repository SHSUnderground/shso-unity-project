using UnityEngine;

public class TotalScavengerNotificationWindow : GeneralNotificationWindow
{
	protected GUIImage _icon2;

	public TotalScavengerNotificationWindow()
		: base(NotificationData.NotificationType.TotalScavenger, "GUI/Notifications/baseWindow", "GUI/Notifications/globe", new Vector2(60f, 61f), new Vector2(2f, 1f), string.Empty, "Collected")
	{
		_mainLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, GUILabel.GenColor(255, 249, 157), TextAnchor.MiddleCenter);
		_mainLabel.VerticalKerning = -1;
		_mainLabel.WordWrap = true;
		_mainLabel.Size = new Vector2(110f, 80f);
		_mainLabel.Position = new Vector2(83f, 0f);
		_bottomLabel.FontSize += 5;
		_bottomLabel.Position += new Vector2(0f, -5f);
		_helpInfo = HelpNotificationWindow.HelpInfo.DailyScavenger;
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
		TotalScavengerNotificationData totalScavengerNotificationData = (TotalScavengerNotificationData)data;
		if (totalScavengerNotificationData == null)
		{
			CspUtils.DebugLog("ERROR: TotalScavengerNotificationWindow called with an invalid data record " + data);
			return;
		}
		OwnableDefinition def = OwnableDefinition.getDef(totalScavengerNotificationData.getOwnableTypeID());
		_mainLabel.Text = def.name;
		_bottomLabel.Text = "x" + totalScavengerNotificationData.getTotalItems();
		if (_icon2 == null)
		{
			_icon2 = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 64f), Vector2.zero);
			_icon2.Position = new Vector2(16f, 16f);
			Add(_icon2);
		}
		_icon2.TextureSource = def.iconFullPath;
	}

	public override bool absorb(NotificationData data)
	{
		if (data.notificationType == _targetDataType && data is TotalScavengerNotificationData)
		{
			TotalScavengerNotificationData totalScavengerNotificationData = (TotalScavengerNotificationData)data;
			if (totalScavengerNotificationData.getOwnableTypeID() == ((TotalScavengerNotificationData)_data).getOwnableTypeID())
			{
				init(_parent, data);
				resetFade(3f);
				return true;
			}
		}
		return false;
	}
}
