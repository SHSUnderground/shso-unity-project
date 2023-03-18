using UnityEngine;

public class GUIDropShadowTextLabel : GUILabel
{
	private Color frontColor;

	private Color backColor;

	private float backColorAlpha = 1f;

	private Vector2 textOffset = new Vector2(-1f, 1f);

	public Color FrontColor
	{
		get
		{
			return frontColor;
		}
		set
		{
			frontColor = value;
		}
	}

	public Color BackColor
	{
		get
		{
			return backColor;
		}
		set
		{
			backColor = value;
		}
	}

	public float BackColorAlpha
	{
		get
		{
			return backColorAlpha;
		}
		set
		{
			backColorAlpha = value;
		}
	}

	public Vector2 TextOffset
	{
		get
		{
			return textOffset;
		}
		set
		{
			textOffset = value;
		}
	}

	public GUIDropShadowTextLabel()
	{
		frontColor = Color.white;
		backColor = Color.black;
	}

	public GUIDropShadowTextLabel(Color frontColor, Color backColor)
	{
		this.frontColor = frontColor;
		this.backColor = backColor;
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		bool flag = (base.VerticalKerning != -1 && labelLines != null) || base.VerticalKerning == -1;
		if (Style == SHSStyle.NoStyle)
		{
			Color color = GUI.color;
			GUI.color = new Color(backColor.r, backColor.g, backColor.b, alpha * animationAlpha * backColorAlpha);
			if (flag)
			{
				OffsetTextPosition(textOffset);
			}
			base.Draw(drawFlags);
			GUI.color = new Color(frontColor.r, frontColor.g, frontColor.b, alpha * animationAlpha);
			if (flag)
			{
				OffsetTextPosition(new Vector2(0f - textOffset.x, 0f - textOffset.y));
			}
			base.Draw(drawFlags);
			GUI.color = color;
			return;
		}
		base.TextColor = BackColor;
		Color color2 = GUI.color;
		GUI.color = new Color(base.color.r, base.color.g, base.color.b, alpha * animationAlpha * backColorAlpha);
		if (flag)
		{
			OffsetTextPosition(textOffset);
		}
		base.Draw(drawFlags);
		GUI.color = color2;
		base.TextColor = FrontColor;
		if (flag)
		{
			OffsetTextPosition(new Vector2(0f - textOffset.x, 0f - textOffset.y));
		}
		base.Draw(drawFlags);
	}

	public void SetupText(GUIFontManager.SupportedFontEnum fontFace, int fontSize, Color frontColor, Color backColor, Vector2 textOffset, TextAnchor anchor)
	{
		FrontColor = frontColor;
		BackColor = backColor;
		TextOffset = textOffset;
		SetupText(fontFace, fontSize, frontColor, anchor);
	}
}
