using UnityEngine;

public class ShoppingTabButton : GUISimpleControlWindow
{
	private GUIStrokeTextLabel _enabledLabel;

	private GUIStrokeTextLabel _disabledLabel;

	private GUIImage _bgLeftEnabled;

	private GUIImage _bgMiddleEnabled;

	private GUIImage _bgRightEnabled;

	private GUIButton _bgLeftDisabled;

	private GUIButton _bgMiddleDisabled;

	private GUIButton _bgRightDisabled;

	public string tabName;

	public string command;

	public ShoppingTabButton(string tabName, string tabIcon, string command)
	{
		this.tabName = tabName;
		this.command = command;
		Traits.HitTestType = HitTestTypeEnum.Rect;
		Traits.BlockTestType = BlockTestTypeEnum.Rect;
		SetControlFlag(ControlFlagSetting.HitTestIgnore, false, true);
		Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		Traits.UpdateTrait = ControlTraits.UpdateTraitEnum.AlwaysUpdate;
		_enabledLabel = new GUIStrokeTextLabel();
		_enabledLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(20f, 18f), new Vector2(100f, 35f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_enabledLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(0f, 3f), TextAnchor.UpperLeft);
		_enabledLabel.BackColorAlpha = 1f;
		_enabledLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		_enabledLabel.Text = tabName;
		_enabledLabel.IsVisible = true;
		_disabledLabel = new GUIStrokeTextLabel();
		_disabledLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(20f, 18f), new Vector2(100f, 35f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_disabledLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(0, 121, 206), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(0f, 3f), TextAnchor.UpperLeft);
		_disabledLabel.BackColorAlpha = 0.5f;
		_disabledLabel.StrokeColorAlpha = 0f;
		_disabledLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		_disabledLabel.Text = tabName;
		_disabledLabel.IsVisible = false;
		float num = GUINotificationWindow.GetTextBlockSize(new GUILabel[2]
		{
			_enabledLabel,
			_disabledLabel
		}, GUINotificationWindow.BlockSizeType.Width) + 3f;
		_bgLeftEnabled = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(19f, 56f), new Vector2(0f, 0f));
		_bgLeftEnabled.TextureSource = "shopping_bundle|tab_left_normal";
		Add(_bgLeftEnabled);
		Vector2 size = _bgLeftEnabled.Size;
		Vector2 size2 = new Vector2(num, size.y);
		Vector2 size3 = _bgLeftEnabled.Size;
		_bgMiddleEnabled = GUIControl.CreateControlTopLeftFrame<GUIImage>(size2, new Vector2(size3.x, 0f));
		_bgMiddleEnabled.TextureSource = "shopping_bundle|tab_middle_normal";
		Add(_bgMiddleEnabled);
		Vector2 size4 = new Vector2(19f, 56f);
		Vector2 size5 = _bgLeftEnabled.Size;
		_bgRightEnabled = GUIControl.CreateControlTopLeftFrame<GUIImage>(size4, new Vector2(size5.x + num, 0f));
		_bgRightEnabled.TextureSource = "shopping_bundle|tab_right_normal";
		Add(_bgRightEnabled);
		_bgLeftDisabled = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(19f, 56f), new Vector2(0f, 0f));
		_bgLeftDisabled.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|tab_left_disabled", SHSButtonStyleInfo.SupportedStatesEnum.Normal);
		_bgLeftDisabled.Alpha = 1f;
		_bgLeftDisabled.Click += delegate
		{
			ShoppingWindow.instance.selectTab(this);
		};
		Add(_bgLeftDisabled);
		Vector2 size6 = _bgLeftDisabled.Size;
		Vector2 size7 = new Vector2(num, size6.y);
		Vector2 size8 = _bgLeftDisabled.Size;
		_bgMiddleDisabled = GUIControl.CreateControlTopLeftFrame<GUIButton>(size7, new Vector2(size8.x, 0f));
		_bgMiddleDisabled.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|tab_middle_disabled", SHSButtonStyleInfo.SupportedStatesEnum.Normal);
		_bgMiddleDisabled.Alpha = 1f;
		_bgMiddleDisabled.Click += delegate
		{
			ShoppingWindow.instance.selectTab(this);
		};
		Add(_bgMiddleDisabled);
		Vector2 size9 = new Vector2(19f, 56f);
		Vector2 size10 = _bgLeftDisabled.Size;
		_bgRightDisabled = GUIControl.CreateControlTopLeftFrame<GUIButton>(size9, new Vector2(size10.x + num, 0f));
		_bgRightDisabled.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|tab_right_disabled", SHSButtonStyleInfo.SupportedStatesEnum.Normal);
		_bgRightDisabled.Alpha = 1f;
		_bgRightDisabled.Click += delegate
		{
			ShoppingWindow.instance.selectTab(this);
		};
		Add(_bgRightDisabled);
		Add(_enabledLabel);
		Add(_disabledLabel);
		IsVisible = true;
		Vector2 size11 = _bgLeftEnabled.Size;
		float num2 = size11.x + num;
		Vector2 size12 = _bgRightEnabled.Size;
		float width = num2 + size12.x;
		Vector2 size13 = _bgLeftEnabled.Size;
		SetSize(width, size13.y);
	}

	public void deactivate()
	{
		_enabledLabel.IsVisible = false;
		_disabledLabel.IsVisible = true;
		_bgLeftEnabled.IsVisible = false;
		_bgMiddleEnabled.IsVisible = false;
		_bgRightEnabled.IsVisible = false;
		_bgLeftDisabled.IsVisible = true;
		_bgMiddleDisabled.IsVisible = true;
		_bgRightDisabled.IsVisible = true;
	}

	public void activate()
	{
		_enabledLabel.IsVisible = true;
		_disabledLabel.IsVisible = false;
		_bgLeftEnabled.IsVisible = true;
		_bgMiddleEnabled.IsVisible = true;
		_bgRightEnabled.IsVisible = true;
		_bgLeftDisabled.IsVisible = false;
		_bgMiddleDisabled.IsVisible = false;
		_bgRightDisabled.IsVisible = false;
	}
}
