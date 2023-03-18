using UnityEngine;

public class GUITextControlBaseInspector : GUIChildControlInspector
{
	public GUIFontManager.SupportedFontEnum fontFace;

	public int fontSize;

	public bool bold;

	public bool italicized;

	public TextAnchor textAlignment;

	public bool wordWrap;

	public bool overflow;

	public Color textColor;
}
