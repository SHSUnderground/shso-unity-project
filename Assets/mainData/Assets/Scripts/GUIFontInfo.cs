public class GUIFontInfo
{
	private GUIFontManager.SupportedFontEnum font;

	private int fontSize;

	private bool bold;

	private bool italic;

	public GUIFontManager.SupportedFontEnum Font
	{
		get
		{
			return font;
		}
		set
		{
			font = value;
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
		}
	}

	public bool Italic
	{
		get
		{
			return italic;
		}
		set
		{
			italic = value;
		}
	}

	public GUIFontInfo(GUIFontManager.SupportedFontEnum font, int fontSize, bool bold, bool italic)
	{
		this.font = font;
		this.fontSize = fontSize;
		this.bold = bold;
		this.italic = italic;
	}

	public GUIFontInfo(GUIFontManager.SupportedFontEnum font, int fontSize)
		: this(font, fontSize, false, false)
	{
	}

	public static GUIFontInfo Create(GUIFontManager.SupportedFontEnum font, int fontSize)
	{
		return new GUIFontInfo(font, fontSize);
	}

	public static GUIFontInfo Create(GUIFontManager.SupportedFontEnum font, int fontSize, bool bold, bool italic)
	{
		return new GUIFontInfo(font, fontSize, bold, italic);
	}
}
