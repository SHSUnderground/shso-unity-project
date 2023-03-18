using UnityEngine;

public class AchievementWindowNavButton : GUISimpleControlWindow
{
	public AchievementDisplayGroup group;

	private GUIImage _selectedUnderlay;

	public GUIHotSpotButton bg;

	private GUIStrokeTextLabel _label;

	private int baseFontSize = 16;

	private Vector2 shadowOffset = new Vector2(0f, 2f);

	private int navOffset;

	public AchievementWindowNavButton(string label, int navOffset, AchievementDisplayGroup group)
	{
		this.group = group;
		this.navOffset = navOffset;
		SetSize(200f, 60f);
		_selectedUnderlay = new GUIImage();
		_selectedUnderlay.SetSize(new Vector2(239f, 33f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_selectedUnderlay.Position = Vector2.zero;
		_selectedUnderlay.TextureSource = "achievement_bundle|leftnav_underlay";
		if (navOffset == 1)
		{
			_selectedUnderlay.IsVisible = true;
		}
		else
		{
			_selectedUnderlay.IsVisible = false;
		}
		Add(_selectedUnderlay);
		bg = GUIControl.CreateControlTopLeftFrame<GUIHotSpotButton>(Size, Vector2.zero);
		bg.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		bg.SetPositionAndSize(QuickSizingHint.ParentSize);
		Add(bg);
		_label = new GUIStrokeTextLabel();
		GUIStrokeTextLabel label2 = _label;
		Vector2 offset = new Vector2(navOffset * 10, 0f);
		Vector2 size = Size;
		float x = size.x - (float)(navOffset * 10);
		Vector2 size2 = Size;
		label2.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, offset, new Vector2(x, size2.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		if (navOffset == 0)
		{
			_label.SetupText(GUIFontManager.SupportedFontEnum.Grobold, baseFontSize + 1, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), shadowOffset, TextAnchor.MiddleLeft);
		}
		else
		{
			_label.SetupText(GUIFontManager.SupportedFontEnum.Grobold, baseFontSize, GUILabel.GenColor(167, 230, 243), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), shadowOffset, TextAnchor.MiddleLeft);
		}
		_label.BackColorAlpha = 1f;
		_label.StrokeColorAlpha = 0f;
		_label.WordWrap = true;
		_label.Text = label;
		_label.IsVisible = true;
		float textBlockSize = GUINotificationWindow.GetTextBlockSize(new GUILabel[1]
		{
			_label
		}, GUINotificationWindow.BlockSizeType.Height);
		GUIStrokeTextLabel label3 = _label;
		Vector2 size3 = _label.Size;
		label3.SetSize(size3.x, textBlockSize + 10f);
		Vector2 size4 = Size;
		SetSize(size4.x, textBlockSize + 10f);
		_selectedUnderlay.SetSize(Size);
		Add(_label);
		IsVisible = true;
	}

	public void select(bool selected)
	{
		if (selected)
		{
			if (navOffset == 0)
			{
				_selectedUnderlay.IsVisible = true;
				_label.SetupText(GUIFontManager.SupportedFontEnum.Grobold, baseFontSize + 1, GUILabel.GenColor(255, 200, 0), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), shadowOffset, TextAnchor.MiddleLeft);
			}
			else
			{
				_label.SetupText(GUIFontManager.SupportedFontEnum.Grobold, baseFontSize, GUILabel.GenColor(255, 200, 0), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), shadowOffset, TextAnchor.MiddleLeft);
			}
		}
		else if (navOffset == 0)
		{
			_selectedUnderlay.IsVisible = false;
			_label.SetupText(GUIFontManager.SupportedFontEnum.Grobold, baseFontSize + 1, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), shadowOffset, TextAnchor.MiddleLeft);
		}
		else
		{
			_label.SetupText(GUIFontManager.SupportedFontEnum.Grobold, baseFontSize, GUILabel.GenColor(167, 230, 243), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), shadowOffset, TextAnchor.MiddleLeft);
		}
	}
}
