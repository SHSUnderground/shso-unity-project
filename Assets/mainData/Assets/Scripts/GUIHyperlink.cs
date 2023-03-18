using UnityEngine;

public class GUIHyperlink : GUIControlWindow
{
	private GUILabel label;

	private GUIImage underline;

	private SHSStyle cashedStyle;

	public string Text
	{
		get
		{
			return label.Text;
		}
		set
		{
			label.Text = value;
			modifySize();
		}
	}

	public float LinePercentOffset
	{
		get
		{
			Vector2 offset = underline.Offset;
			return offset.y;
		}
		set
		{
			underline.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Percentage, new Vector2(0f, 0f - value));
		}
	}

	public GUIHyperlink()
	{
		label = new GUILabel();
		label.SetPosition(0f, 0f);
		label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(14, 1, 226), TextAnchor.MiddleCenter);
		Add(label);
		underline = new GUIImage();
		underline.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Percentage, new Vector2(0f, -0.2f));
		underline.Texture = GUIManager.Instance.LoadTexture("common_bundle|white2x2");
		Add(underline);
		LinePercentOffset = 0.2f;
		Text = string.Empty;
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		label.SetSize(Size);
		GUIImage gUIImage = underline;
		Vector2 size = Size;
		gUIImage.SetSize(size.x, 1f);
		base.HandleResize(message);
	}

	private void modifySize()
	{
		SetSize(label.Style.UnityStyle.CalcSize(label.Content));
		HandleResize(null);
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		if (cashedStyle != Style)
		{
			modifySize();
			cashedStyle = Style;
		}
		if (Hover)
		{
			label.TextColor = GUILabel.GenColor(0, 213, 215);
			underline.Color = GUILabel.GenColor(0, 213, 215);
		}
		else
		{
			label.TextColor = GUILabel.GenColor(14, 1, 226);
			underline.Color = GUILabel.GenColor(14, 1, 226);
		}
		base.Draw(drawFlags);
	}
}
