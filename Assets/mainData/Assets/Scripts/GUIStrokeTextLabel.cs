using System;
using UnityEngine;

public class GUIStrokeTextLabel : GUILabel
{
	private Color frontColor;

	private Color strokeColor;

	private float strokeColorAlpha = 1f;

	private Color backColor;

	private float backColorAlpha = 1f;

	private Vector2 shadowOffset = new Vector2(-1f, 1f);

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

	public Color StrokeColor
	{
		get
		{
			return strokeColor;
		}
		set
		{
			strokeColor = value;
		}
	}

	public float StrokeColorAlpha
	{
		get
		{
			return strokeColorAlpha;
		}
		set
		{
			strokeColorAlpha = value;
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

	public Vector2 ShadowOffset
	{
		get
		{
			return shadowOffset;
		}
		set
		{
			shadowOffset = value;
		}
	}

	public GUIStrokeTextLabel()
	{
		frontColor = Color.white;
		strokeColor = Color.grey;
		backColor = Color.black;
	}

	public GUIStrokeTextLabel(Color frontColor, Color strokeColor, Color backColor)
	{
		this.frontColor = frontColor;
		this.strokeColor = strokeColor;
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
				OffsetTextPosition(shadowOffset);
			}
			base.Draw(drawFlags);
			GUI.color = new Color(strokeColor.r, strokeColor.g, strokeColor.b, alpha * animationAlpha * strokeColorAlpha);
			if (flag)
			{
				OffsetTextPosition(-shadowOffset + new Vector2(1f, 1f));
				base.Draw(drawFlags);
				OffsetTextPosition(new Vector2(0f, -2f));
				base.Draw(drawFlags);
				OffsetTextPosition(new Vector2(-2f, 0f));
				base.Draw(drawFlags);
				OffsetTextPosition(new Vector2(0f, 2f));
				base.Draw(drawFlags);
			}
			GUI.color = new Color(frontColor.r, frontColor.g, frontColor.b, alpha * animationAlpha);
			if (flag)
			{
				OffsetTextPosition(new Vector2(1f, -1f));
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
			OffsetTextPosition(shadowOffset);
		}
		base.Draw(drawFlags);
		base.TextColor = StrokeColor;
		GUI.color = new Color(strokeColor.r, strokeColor.g, strokeColor.b, alpha * animationAlpha * strokeColorAlpha);
		if (flag)
		{
			OffsetTextPosition(-shadowOffset + new Vector2(1f, 1f));
			base.Draw(drawFlags);
			OffsetTextPosition(new Vector2(0f, -2f));
			base.Draw(drawFlags);
			OffsetTextPosition(new Vector2(-2f, 0f));
			base.Draw(drawFlags);
			OffsetTextPosition(new Vector2(0f, 2f));
			base.Draw(drawFlags);
		}
		GUI.color = color2;
		base.TextColor = FrontColor;
		if (flag)
		{
			OffsetTextPosition(new Vector2(1f, -1f));
		}
		base.Draw(drawFlags);
	}

	public void SetupText(GUIFontManager.SupportedFontEnum fontFace, int fontSize, Color frontColor, Color strokeColor, Color backColor, Vector2 shadowOffset, TextAnchor anchor)
	{
		FrontColor = frontColor;
		StrokeColor = strokeColor;
		BackColor = backColor;
		ShadowOffset = shadowOffset;
		SetupText(fontFace, fontSize, frontColor, anchor);
		base.VerticalKerning = Convert.ToInt16((float)fontSize * 0.8f);
	}
}
