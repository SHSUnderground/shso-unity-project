using UnityEngine;

public class GUITextListItem : GUIListItem
{
	private GUILabel label;

	private object key;

	public object Key
	{
		get
		{
			return key;
		}
		set
		{
			key = value;
		}
	}

	public string Text
	{
		get
		{
			return label.Text;
		}
		set
		{
			label.Text = value;
		}
	}

	public GUITextListItem(string text)
		: this(text, null)
	{
	}

	public GUITextListItem(string text, object key)
	{
		label = new GUILabel();
		label.SetPositionAndSize(QuickSizingHint.ParentSize);
		label.Text = text;
		label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, Color.white, TextAnchor.UpperLeft);
		Add(label);
		this.key = key;
	}

	public override void DrawPreprocess()
	{
		if (isSelected)
		{
			cachedColor = base.Color;
			color = new Color(1f, 0.5f, 1f, 1f);
			label.Color = color;
		}
		base.DrawPreprocess();
	}

	public override void DrawFinalize()
	{
		base.DrawFinalize();
		if (isSelected)
		{
			color = cachedColor;
			label.Color = color;
		}
	}
}
