using System;
using UnityEngine;

//[Extension]
public static class GUIStyleExtensions
{
	//[Extension]
	public static GUIStyle NoBackgroundImages(GUIStyle style)
	{
		style.normal.background = null;
		style.active.background = null;
		style.hover.background = null;
		style.focused.background = null;
		style.onNormal.background = null;
		style.onActive.background = null;
		style.onHover.background = null;
		style.onFocused.background = null;
		return style;
	}

	//[Extension]
	public static GUIStyle BaseTextColor(GUIStyle style, Color c)
	{
		GUIStyleState normal = style.normal;
		Color color = c;
		style.onFocused.textColor = color;
		color = color;
		style.onHover.textColor = color;
		color = color;
		style.onActive.textColor = color;
		color = color;
		style.onNormal.textColor = color;
		color = color;
		style.focused.textColor = color;
		color = color;
		style.hover.textColor = color;
		color = color;
		style.active.textColor = color;
		normal.textColor = color;
		return style;
	}

	//[Extension]
	public static GUIStyle ResetBoxModel(GUIStyle style)
	{
		style.border = new RectOffset();
		style.margin = new RectOffset();
		style.padding = new RectOffset();
		style.overflow = new RectOffset();
		style.contentOffset = Vector2.zero;
		return style;
	}

	//[Extension]
	public static GUIStyle Padding(GUIStyle style, int left, int right, int top, int bottom)
	{
		style.padding = new RectOffset(left, right, top, bottom);
		return style;
	}

	//[Extension]
	public static GUIStyle Margin(GUIStyle style, int left, int right, int top, int bottom)
	{
		style.margin = new RectOffset(left, right, top, bottom);
		return style;
	}

	//[Extension]
	public static GUIStyle Border(GUIStyle style, int left, int right, int top, int bottom)
	{
		style.border = new RectOffset(left, right, top, bottom);
		return style;
	}

	//[Extension]
	public static GUIStyle Named(GUIStyle style, string name)
	{
		style.name = name;
		return style;
	}

	//[Extension]
	public static GUIStyle ClipText(GUIStyle style)
	{
		style.clipping = TextClipping.Clip;
		return style;
	}

	//[Extension]
	public static GUIStyle Size(GUIStyle style, int width, int height, bool stretchWidth, bool stretchHeight)
	{
		style.fixedWidth = width;
		style.fixedHeight = height;
		style.stretchWidth = stretchWidth;
		style.stretchHeight = stretchHeight;
		return style;
	}
}
