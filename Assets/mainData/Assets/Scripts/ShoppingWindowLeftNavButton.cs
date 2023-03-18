using UnityEngine;

public class ShoppingWindowLeftNavButton : GUISimpleControlWindow
{
	public GUIButton bg;

	private GUIImage _icon;

	private GUIStrokeTextLabel _label;

	public NewShoppingManager.ShoppingCategory category;

	private int baseFontSize = 14;

	public ShoppingWindowLeftNavButton(string iconPath, string label, int fontSize, Color labelColor, Color labelShadowColor, bool stroke = false)
	{
		baseFontSize = fontSize;
		SetSize(174f, 40f);
		if (baseFontSize == 30)
		{
			SetSize(174f, 52f);
		}
		bg = GUIControl.CreateControlTopLeftFrame<GUIButton>(Size, new Vector2(0f, 0f));
		bg.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|itemBG", SHSButtonStyleInfo.SupportedStatesEnum.Normal);
		bg.Alpha = 0f;
		Add(bg);
		_icon = new GUIImage();
		_icon.SetSize(new Vector2(32f, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage icon = _icon;
		Vector2 size = Size;
		icon.Position = new Vector2(0f, size.y / 2f - 16f);
		_icon.TextureSource = iconPath;
		_icon.IsVisible = true;
		Add(_icon);
		Vector2 shadowOffset = new Vector2(0f, 4f);
		if (!stroke)
		{
			shadowOffset = new Vector2(0f, 2f);
		}
		_label = new GUIStrokeTextLabel();
		GUIStrokeTextLabel label2 = _label;
		Vector2 offset = new Vector2(35f, 0f);
		Vector2 size2 = Size;
		label2.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, offset, new Vector2(126f, size2.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_label.SetupText(GUIFontManager.SupportedFontEnum.Grobold, baseFontSize, labelColor, labelShadowColor, labelShadowColor, shadowOffset, TextAnchor.MiddleLeft);
		_label.BackColorAlpha = 1f;
		if (!stroke)
		{
			_label.StrokeColorAlpha = 0f;
		}
		_label.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		_label.WordWrap = false;
		_label.Text = label;
		_label.IsVisible = true;
		Add(_label);
		IsVisible = true;
	}

	public override bool FireMouseOver(GUIMouseEvent data)
	{
		CspUtils.DebugLog("over");
		_label.SetupText(GUIFontManager.SupportedFontEnum.Grobold, _label.FontSize + 2, _label.TextColor, _label.StrokeColor, _label.StrokeColor, _label.ShadowOffset, TextAnchor.MiddleLeft);
		return base.FireMouseOver(data);
	}

	public override bool FireMouseOut(GUIMouseEvent data)
	{
		CspUtils.DebugLog("out");
		_label.SetupText(GUIFontManager.SupportedFontEnum.Grobold, _label.FontSize - 2, _label.TextColor, _label.StrokeColor, _label.StrokeColor, _label.ShadowOffset, TextAnchor.MiddleLeft);
		return base.FireMouseOut(data);
	}
}
