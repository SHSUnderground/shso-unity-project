using UnityEngine;

public class GUITextControlBase : GUIChildControl
{
	private GUIFontManager.SupportedFontEnum fontFace;

	private int fontSize;

	private bool bold;

	private bool italicized;

	private TextAnchor textAlignment;

	private bool wordWrap = true;

	private bool overflow;

	private Color textColor = Color.black;

	public GUIFontManager.SupportedFontEnum FontFace
	{
		get
		{
			return fontFace;
		}
		set
		{
			fontFace = value;
			style = null;
			ReflectToInspector();
		}
	}

	public int FontSize
	{
		get
		{
			return fontSize;
		}
		set
		{
			fontSize = value;
			if (style != null)
			{
				style.UnityStyle.fontSize = value;
			}
			ReflectToInspector();
		}
	}

	public bool Bold
	{
		get
		{
			return bold;
		}
		set
		{
			bold = value;
			if (style != null)
			{
				if (bold)
				{
					style.UnityStyle.fontStyle = ((!Italicized) ? FontStyle.Bold : FontStyle.BoldAndItalic);
				}
				else
				{
					style.UnityStyle.fontStyle = (Italicized ? FontStyle.Italic : FontStyle.Normal);
				}
			}
			ReflectToInspector();
		}
	}

	public bool Italicized
	{
		get
		{
			return italicized;
		}
		set
		{
			italicized = value;
			if (style != null)
			{
				if (italicized)
				{
					style.UnityStyle.fontStyle = ((!Bold) ? FontStyle.Italic : FontStyle.BoldAndItalic);
				}
				else
				{
					style.UnityStyle.fontStyle = (Bold ? FontStyle.Bold : FontStyle.Normal);
				}
			}
			ReflectToInspector();
		}
	}

	public TextAnchor TextAlignment
	{
		get
		{
			return textAlignment;
		}
		set
		{
			textAlignment = value;
			if (style != null)
			{
				style.UnityStyle.alignment = textAlignment;
			}
			ReflectToInspector();
		}
	}

	public bool WordWrap
	{
		get
		{
			return wordWrap;
		}
		set
		{
			wordWrap = value;
			if (style != null)
			{
				style.UnityStyle.wordWrap = value;
			}
			ReflectToInspector();
		}
	}

	public bool Overflow
	{
		get
		{
			return overflow;
		}
		set
		{
			overflow = value;
			if (style != null)
			{
				style.UnityStyle.clipping = ((!value) ? TextClipping.Clip : TextClipping.Overflow);
			}
			ReflectToInspector();
		}
	}

	public Color TextColor
	{
		get
		{
			return textColor;
		}
		set
		{
			textColor = value;
			if (style != null)
			{
				style.UnityStyle.normal.textColor = value;
				style.UnityStyle.hover.textColor = value;
				style.UnityStyle.active.textColor = value;
			}
			ReflectToInspector();
		}
	}

	public override SHSStyle Style
	{
		get
		{
			if (style != null)
			{
				return style;
			}
			style = GUIManager.Instance.StyleManager.GetStyle(this);
			return (style == null) ? SHSStyle.NoStyle : style;
		}
		set
		{
			base.Style = value;
		}
	}

	public void SetupText(GUIFontManager.SupportedFontEnum fontFace, int fontSize, Color color, TextAnchor anchor)
	{
		FontFace = fontFace;
		FontSize = fontSize;
		TextColor = color;
		TextAlignment = anchor;
		ReflectToInspector();
	}

	public void SetFontSizeForOneLine(int max)
	{
		GUIStyle unityStyle = Style.UnityStyle;
		GUIContent content = new GUIContent(Text);
		unityStyle.fontSize = max;
		float minWidth;
		float maxWidth;
		unityStyle.CalcMinMaxWidth(content, out minWidth, out maxWidth);
		while (true)
		{
			float num = maxWidth;
			Vector2 size = Size;
			if (!(num > size.x) || unityStyle.fontSize <= 0)
			{
				break;
			}
			unityStyle.fontSize--;
			unityStyle.CalcMinMaxWidth(content, out minWidth, out maxWidth);
		}
		fontSize = unityStyle.fontSize;
	}

	public float GetTextWidth()
	{
		float minWidth;
		float maxWidth;
		Style.UnityStyle.CalcMinMaxWidth(base.Content, out minWidth, out maxWidth);
		return maxWidth;
	}

	protected override void AttachInspector()
	{
		inspector = guiTreeNode.AddComponent(typeof(GUITextControlBaseInspector));
	}

	protected override void ReflectToInspector()
	{
		base.ReflectToInspector();
		if (inspector != null)
		{
			((GUITextControlBaseInspector)inspector).fontFace = fontFace;
			((GUITextControlBaseInspector)inspector).fontSize = fontSize;
			((GUITextControlBaseInspector)inspector).bold = bold;
			((GUITextControlBaseInspector)inspector).italicized = italicized;
			((GUITextControlBaseInspector)inspector).textAlignment = textAlignment;
			((GUITextControlBaseInspector)inspector).wordWrap = wordWrap;
			((GUITextControlBaseInspector)inspector).overflow = overflow;
			((GUITextControlBaseInspector)inspector).textColor = textColor;
		}
	}

	protected override void ReflectFromInspector()
	{
		base.ReflectFromInspector();
		if (inspector != null)
		{
			GUITextControlBaseInspector gUITextControlBaseInspector = (GUITextControlBaseInspector)inspector;
			if (FontFace != gUITextControlBaseInspector.fontFace)
			{
				FontFace = gUITextControlBaseInspector.fontFace;
			}
			if (FontSize != gUITextControlBaseInspector.fontSize)
			{
				FontSize = gUITextControlBaseInspector.fontSize;
			}
			if (Bold != gUITextControlBaseInspector.bold)
			{
				Bold = gUITextControlBaseInspector.bold;
			}
			if (Italicized != gUITextControlBaseInspector.italicized)
			{
				Italicized = gUITextControlBaseInspector.italicized;
			}
			if (TextAlignment != gUITextControlBaseInspector.textAlignment)
			{
				TextAlignment = gUITextControlBaseInspector.textAlignment;
			}
			if (WordWrap != gUITextControlBaseInspector.wordWrap)
			{
				WordWrap = gUITextControlBaseInspector.wordWrap;
			}
			if (Overflow != gUITextControlBaseInspector.overflow)
			{
				Overflow = gUITextControlBaseInspector.overflow;
			}
			if (TextColor != gUITextControlBaseInspector.textColor)
			{
				TextColor = gUITextControlBaseInspector.textColor;
			}
		}
	}
}
