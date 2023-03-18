using System;
using UnityEngine;

public class GeneralNotificationWindow : NotificationWindow
{
	protected GUILabel _mainLabel;

	protected GUILabel _topLabel;

	protected GUILabel _bottomLabel;

	protected GUIImage _icon;

	protected GUIButton _helpButton;

	protected GUILabel _helpLabel;

	protected HelpNotificationWindow.HelpInfo _helpInfo;

	protected static readonly int LabelOffset = 23;

	public new static Vector2 size
	{
		get
		{
			return new Vector2(231f, 100f);
		}
	}

	public GeneralNotificationWindow(NotificationData.NotificationType targetDataType, string bgPath, string iconPath, Vector2 iconSize, Vector2 iconOffset, string topLabelTxt = "", string bottomLabelTxt = "")
		: base(targetDataType)
	{
		Traits.HitTestType = HitTestTypeEnum.Rect;
		Traits.BlockTestType = BlockTestTypeEnum.Rect;
		SetControlFlag(ControlFlagSetting.HitTestIgnore, false, true);
		Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		Traits.UpdateTrait = ControlTraits.UpdateTraitEnum.AlwaysUpdate;
		_background = GUIControl.CreateControlAbsolute<GUIImageWithEvents>(new Vector2(225f, 94f), new Vector2(3f, 3f));
		_background.TextureSource = bgPath;
		_background.MouseOver += delegate
		{
			resetFade(3f);
		};
		Add(_background);
		if (topLabelTxt != string.Empty)
		{
			_topLabel = new GUILabel();
			_topLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(255, 249, 157), TextAnchor.UpperLeft);
			_topLabel.Text = topLabelTxt;
			_topLabel.Position = new Vector2(0f, 20f);
			recenterLabel(_topLabel);
			Add(_topLabel);
		}
		if (bottomLabelTxt != string.Empty)
		{
			_bottomLabel = new GUILabel();
			_bottomLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(255, 249, 157), TextAnchor.UpperLeft);
			_bottomLabel.Text = bottomLabelTxt;
			_bottomLabel.Position = new Vector2(0f, 61f);
			recenterLabel(_bottomLabel);
			Add(_bottomLabel);
		}
		_mainLabel = new GUILabel();
		_mainLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 22, GUILabel.GenColor(255, 249, 157), TextAnchor.UpperLeft);
		_mainLabel.Position = new Vector2(0f, 36f);
		recenterLabel(_mainLabel);
		Add(_mainLabel);
		if (iconPath != string.Empty)
		{
			_icon = GUIControl.CreateControlAbsolute<GUIImage>(iconSize, Vector2.zero);
			_icon.TextureSource = iconPath;
			_icon.Position = new Vector2(16f, 16f) + iconOffset;
			Add(_icon);
		}
		_helpButton = new GUIButton();
		_helpButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		_helpButton.SetSize(new Vector2(48f, 48f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIButton helpButton = _helpButton;
		Vector2 size = _background.Size;
		helpButton.SetPosition(new Vector2(size.x - 45f, 0f));
		_helpButton.StyleInfo = new SHSButtonStyleInfo("achievement_bundle|mshs_button_help");
		_helpButton.ToolTip = new NamedToolTipInfo("#ACHIEVEMENT_HELP");
		_helpButton.HitTestType = HitTestTypeEnum.Circular;
		_helpButton.HitTestSize = new Vector2(1f, 1f);
		_helpButton.BlockTestType = BlockTestTypeEnum.Rect;
		_helpButton.BlockTestSize = new Vector2(1f, 1f);
		_helpButton.Click += HelpClick;
		Add(_helpButton);
		_helpButton.IsVisible = true;
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
	}

	private void HelpClick(GUIControl sender, GUIClickEvent EventData)
	{
		NotificationHUD.addNotification(new HelpNotificationData(_helpInfo));
	}

	protected void recenterLabel(GUILabel label, bool updateSize = true)
	{
		if (_background == null)
		{
			throw new Exception("ERROR - recenterLabel called and _background is null, did you forget to set it?");
		}
		if (updateSize)
		{
			updateLabelSize(label);
		}
		Vector2 size = _background.Size;
		float num = size.x * 0.5f;
		Vector2 size2 = label.Size;
		float x = num - size2.x * 0.5f + (float)LabelOffset;
		Vector2 position = label.Position;
		label.Position = new Vector2(x, position.y);
	}

	public override bool absorb(NotificationData data)
	{
		if (data.notificationType == _targetDataType)
		{
			init(_parent, data);
			resetFade(3f);
			return true;
		}
		return false;
	}
}
