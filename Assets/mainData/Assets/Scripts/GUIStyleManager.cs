using System.Collections.Generic;
using UnityEngine;

public class GUIStyleManager
{
	protected Dictionary<string, SHSSkin> skinList;

	protected Dictionary<string, SHSStyle> styleList;

	protected GUIManager uiManager;

	protected SHSSkin defaultSkin;

	protected SHSSkin currentSkin;

	public SHSSkin CurrentSkin
	{
		get
		{
			return currentSkin;
		}
	}

	public GUIStyleManager()
	{
		Init();
	}

	public GUIStyleManager(GUIManager mgr)
	{
		uiManager = mgr;
		Init();
	}

	protected void Init()
	{
		skinList = new Dictionary<string, SHSSkin>();
		styleList = new Dictionary<string, SHSStyle>();
		foreach (GUISkin skin in uiManager.skinList)
		{
			AddSkin(skin);
		}
	}

	public bool AddSkin(GUISkin skin)
	{
		if (skinList.ContainsKey(skin.name))
		{
			CspUtils.DebugLog("Skin " + skin.name + " already exists in this project. No need to add");
			return false;
		}
		skinList[skin.name] = new SHSSkin(skin);
		GUIStyle[] customStyles = skin.customStyles;
		for (int i = 0; i < customStyles.Length; i++)
		{
			AddStyle(customStyles[i]);
		}
		return true;
	}

	public bool AddStyle(GUIStyle style)
	{
		if (styleList.ContainsKey(style.name))
		{
			return false;
		}
		styleList[style.name] = new SHSStyle(style);
		return true;
	}

	public SHSSkin GetSkin(string name)
	{
		SHSSkin value;
		skinList.TryGetValue(name, out value);
		return value;
	}

	public SHSStyle GetStyle(string name, string fallbackName)
	{
		SHSStyle value;
		styleList.TryGetValue(fallbackName, out value);
		return buildStyle(name, value);
	}

	public SHSStyle GetStyle(string name)
	{
		return buildStyle(name, null);
	}

	private SHSStyle buildStyle(string name, SHSStyle fallbackStyle)
	{
		SHSStyle value;
		return (!styleList.TryGetValue(name, out value)) ? fallbackStyle : value;
	}

	public SHSStyle GetStyle(GUIControl control)
	{
		if (control is GUITextControlBase)
		{
			return buildTextControlStyle((GUITextControlBase)control);
		}
		SHSStyleInfo styleInfo = control.StyleInfo;
		if (styleInfo == null)
		{
			CspUtils.DebugLog("No styleInfo info assigned for control: " + control.Id + " can't build SHSStyle object");
			return null;
		}
		if (styleInfo is SHSInheritedStyleInfo)
		{
			return SHSStyle.NoStyle;
		}
		if (styleInfo is SHSNamedStyleInfo)
		{
			return GetStyle(((SHSNamedStyleInfo)styleInfo).StyleName, "fail");
		}
		if (styleInfo is SHSButtonStyleInfo)
		{
			return buildButtonStyle((SHSButtonStyleInfo)styleInfo, control);
		}
		if (styleInfo is SHSButtonNoHighlightStyleInfo)
		{
			return buildNoHighlightButtonStyle(styleInfo as SHSButtonNoHighlightStyleInfo, control);
		}
		CspUtils.DebugLog("Cant build styleInfo for " + control.Id + ". Style Info doesn't match any known styleInfo type.");
		return new SHSStyle();
	}

	public bool HasStyle(string name)
	{
		return styleList.ContainsKey(name);
	}

	public bool SetSkin(SHSSkin skin)
	{
		currentSkin = skin;
		return true;
	}

	public void Update()
	{
	}

	private SHSStyle buildTextControlStyle(GUITextControlBase control)
	{
		GUIFontManager.SupportedFontEnum fontType = control.FontFace;
		int fontSize = control.FontSize;
		if (control.FontFace == GUIFontManager.SupportedFontEnum.Undefined || control.FontSize == 0)
		{
			GUIFontInfo gUIFontInfo = findInheritedFontInfo(control);
			if (gUIFontInfo == null)
			{
				return SHSStyle.NoStyle;
			}
			fontType = ((control.FontFace != 0) ? control.FontFace : gUIFontInfo.Font);
			fontSize = ((control.FontSize != 0) ? control.FontSize : gUIFontInfo.FontSize);
		}
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.font = GUIManager.Instance.FontManager[fontType];
		gUIStyle.fontSize = fontSize;
		gUIStyle.fontStyle = ((control.Bold && control.Italicized) ? FontStyle.BoldAndItalic : (control.Bold ? FontStyle.Bold : (control.Italicized ? FontStyle.Italic : FontStyle.Normal)));
		gUIStyle.alignment = control.TextAlignment;
		gUIStyle.normal.textColor = control.TextColor;
		gUIStyle.hover.textColor = control.TextColor;
		gUIStyle.active.textColor = control.TextColor;
		gUIStyle.wordWrap = control.WordWrap;
		gUIStyle.clipping = ((!control.Overflow) ? TextClipping.Clip : TextClipping.Overflow);
		return new SHSStyle(gUIStyle);
	}

	private GUIFontInfo findInheritedFontInfo(GUITextControlBase control)
	{
		if (control.Parent == null)
		{
			return null;
		}
		IGUIContainer parent = control.Parent;
		while (parent != null && parent.FontInfo == null)
		{
			parent = parent.Parent;
		}
		if (parent == null)
		{
			return null;
		}
		return parent.FontInfo;
	}

	private SHSStyle buildButtonStyle(SHSButtonStyleInfo styleInfo, IGUIControl control)
	{
		GUIStyle gUIStyle = new GUIStyle();
		if (!styleInfo.isStateless)
		{
			if ((styleInfo.SupportedStates & SHSButtonStyleInfo.SupportedStatesEnum.Normal) != 0)
			{
				gUIStyle.normal.background = uiManager.LoadTexture(styleInfo.buttonSourceRoot + "_normal");
			}
			if ((styleInfo.SupportedStates & SHSButtonStyleInfo.SupportedStatesEnum.Highlight) != 0)
			{
				gUIStyle.hover.background = uiManager.LoadTexture(styleInfo.buttonSourceRoot + "_highlight");
			}
			if ((styleInfo.SupportedStates & SHSButtonStyleInfo.SupportedStatesEnum.Pressed) != 0)
			{
				gUIStyle.active.background = uiManager.LoadTexture(styleInfo.buttonSourceRoot + "_pressed");
			}
		}
		else
		{
			gUIStyle.normal.background = uiManager.LoadTexture(styleInfo.buttonSourceRoot);
		}
		gUIStyle.alignment = TextAnchor.MiddleCenter;
		if (control.Traits.HitTestType == GUIControl.HitTestTypeEnum.Alpha)
		{
			control.SetMask(uiManager.LoadTexture(styleInfo.buttonSourceRoot + "_normal"));
		}
		SHSStyle sHSStyle = new SHSStyle(gUIStyle);
		bool flag = styleInfo.IsLarge(control);
		styleInfo.downAudio = (styleInfo.downAudio ?? ((!flag) ? "click_down" : "large_click_down"));
		styleInfo.upAudio = (styleInfo.upAudio ?? ((!flag) ? "click_up" : "large_click_up"));
		styleInfo.overAudio = (styleInfo.overAudio ?? ((!flag) ? "hover_over" : "large_hover_over"));
		sHSStyle.AudioDown = GetUISound(styleInfo.downAudio);
		sHSStyle.AudioUp = GetUISound(styleInfo.upAudio);
		if (styleInfo.useHighlightAudio)
		{
			sHSStyle.AudioOver = GetUISound(styleInfo.overAudio);
		}
		return sHSStyle;
	}

	private SHSStyle buildNoHighlightButtonStyle(SHSButtonNoHighlightStyleInfo styleInfo, IGUIControl control)
	{
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.normal.background = uiManager.LoadTexture(styleInfo.buttonSourceRoot + "_normal");
		gUIStyle.active.background = uiManager.LoadTexture(styleInfo.buttonSourceRoot + "_pressed");
		gUIStyle.hover.background = uiManager.LoadTexture(styleInfo.buttonSourceRoot + "_normal");
		gUIStyle.alignment = TextAnchor.MiddleCenter;
		if (control.Traits.HitTestType == GUIControl.HitTestTypeEnum.Alpha)
		{
			control.SetMask(uiManager.LoadTexture(styleInfo.buttonSourceRoot + "_normal"));
		}
		SHSStyle sHSStyle = new SHSStyle(gUIStyle);
		bool flag = styleInfo.IsLarge(control);
		sHSStyle.AudioDown = ((!flag) ? GetUISound("click_down") : GetUISound("large_click_down"));
		sHSStyle.AudioUp = ((!flag) ? GetUISound("click_up") : GetUISound("large_click_up"));
		return sHSStyle;
	}

	public GameObject GetUISound(string name)
	{
		GUIManager.UISFX[] sounds = uiManager.sounds;
		foreach (GUIManager.UISFX uISFX in sounds)
		{
			if (uISFX.name == name)
			{
				return uISFX.sfx;
			}
		}
		return null;
	}
}
