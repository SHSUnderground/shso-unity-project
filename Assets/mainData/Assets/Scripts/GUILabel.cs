using System;
using System.Collections.Generic;
using UnityEngine;

public class GUILabel : GUITextControlBase
{
	protected struct LabelLine
	{
		public Rect rect;

		public string text;

		public GUIContent content;

		public Rect TrueRect(Rect baseRect)
		{
			return new Rect(rect.x + baseRect.x, rect.y + baseRect.y, rect.width, rect.height);
		}
	}

	public enum AutoSizeTextEnum
	{
		None,
		ShrinkOnly,
		FitToRect
	}

	protected List<LabelLine> labelLines;

	private bool layoutCalculated;

	private int verticalKerning = -1;

	private int longestLine;

	private bool noLineLimit;

	protected AutoSizeTextEnum autoSizeText;

	private bool textOverflowError;

	public int VerticalKerning
	{
		get
		{
			return verticalKerning;
		}
		set
		{
			verticalKerning = value;
			layoutCalculated = false;
		}
	}

	public int LongestLine
	{
		get
		{
			int result;
			if (longestLine == 0)
			{
				Vector2 vector = Style.UnityStyle.CalcSize(new GUIContent(text));
				result = (int)vector.x;
			}
			else
			{
				result = longestLine;
			}
			return result;
		}
	}

	public bool NoLineLimit
	{
		get
		{
			return noLineLimit;
		}
		set
		{
			noLineLimit = value;
		}
	}

	public AutoSizeTextEnum AutoSizeText
	{
		get
		{
			return autoSizeText;
		}
		set
		{
			autoSizeText = value;
			ClearKerning();
		}
	}

	public int TotalTextHeight
	{
		get
		{
			if (verticalKerning != -1)
			{
				if (layoutCalculated)
				{
					return (int)labelLines[labelLines.Count - 1].rect.y + (int)labelLines[labelLines.Count - 1].rect.height - (int)labelLines[0].rect.y;
				}
				CalculateTextLayout();
				return (int)labelLines[labelLines.Count - 1].rect.y + (int)labelLines[labelLines.Count - 1].rect.height - (int)labelLines[0].rect.y;
			}
			Vector2 vector = Style.UnityStyle.CalcSize(new GUIContent(text));
			return (int)vector.y;
		}
	}

	public int LineCount
	{
		get
		{
			if (labelLines != null && verticalKerning != -1)
			{
				return labelLines.Count;
			}
			return 1;
		}
	}

	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			base.Text = value;
			ClearKerning();
		}
	}

	public bool TextOverflowError
	{
		get
		{
			return textOverflowError;
		}
	}

	public GUILabel()
	{
		Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Ignore;
	}

	protected override void AttachInspector()
	{
		inspector = guiTreeNode.AddComponent(typeof(GUILabelInspector));
	}

	protected override void ReflectToInspector()
	{
		base.ReflectToInspector();
		if (inspector != null)
		{
			((GUILabelInspector)inspector).verticalKerning = verticalKerning;
			((GUILabelInspector)inspector).autoSizeText = autoSizeText;
			((GUILabelInspector)inspector).textOverflowError = textOverflowError;
		}
	}

	protected override void ReflectFromInspector()
	{
		base.ReflectFromInspector();
		if (inspector != null)
		{
			VerticalKerning = ((GUILabelInspector)inspector).verticalKerning;
			AutoSizeText = ((GUILabelInspector)inspector).autoSizeText;
			textOverflowError = ((GUILabelInspector)inspector).textOverflowError;
		}
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		if (!layoutCalculated)
		{
			CalculateTextLayout();
		}
		Color backgroundColor = GUI.backgroundColor;
		if (textOverflowError)
		{
			GUI.backgroundColor = Color.red;
			GUI.Box(base.rect, string.Empty);
		}
		if (layoutCalculated)
		{
			foreach (LabelLine labelLine in labelLines)
			{
				DrawText(labelLine.TrueRect(base.rect), labelLine.content, Style);
			}
		}
		else
		{
			DrawText(base.rect, base.Content, Style);
		}
		GUI.backgroundColor = backgroundColor;
		base.Draw(drawFlags);
	}

	private void DrawText(Rect theRect, GUIContent theContent, SHSStyle theStyle)
	{
		if (theStyle == SHSStyle.NoStyle)
		{
			GUI.Label(theRect, theContent);
		}
		else
		{
			GUI.Label(theRect, theContent, theStyle.UnityStyle);
		}
	}

	public void CalculateTextLayout()
	{
		if (layoutCalculated)
		{
			return;
		}
		ClearKerning();
		LabelLine item = default(LabelLine);
		item.rect = default(Rect);
		float x = base.size.x;
		float num = 0f;
		bool flag = false;
		string text = string.Empty;
		Vector2 vector = Vector2.zero;
		GUIContent gUIContent = new GUIContent();
		longestLine = 0;
		labelLines = new List<LabelLine>();
		textOverflowError = false;
		if (AutoSizeText == AutoSizeTextEnum.None)
		{
			if (GUIManager.Instance.Diagnostics.EnableLabelBoundsChecking)
			{
				Vector2 vector2 = Style.UnityStyle.CalcSize(base.Content);
				if ((vector2.x > base.rect.width || vector2.y > base.rect.height) && (!Style.UnityStyle.wordWrap || Style.UnityStyle.CalcHeight(base.Content, base.rect.width) > base.rect.height))
				{
					textOverflowError = true;
				}
			}
			if (verticalKerning == -1)
			{
				return;
			}
			for (int i = 0; i < base.text.Length; i++)
			{
				gUIContent.text = text + base.text[i];
				vector = Style.UnityStyle.CalcSize(gUIContent);
				if (base.text[i] == '\n')
				{
					text = string.Empty;
					gUIContent.text = gUIContent.text.Remove(gUIContent.text.Length - 1);
					vector = Style.UnityStyle.CalcSize(gUIContent);
					flag = true;
				}
				else if (vector.x < x)
				{
					text = (gUIContent.text = text + base.text[i]);
				}
				else
				{
					flag = true;
					bool flag2 = false;
					int num2 = gUIContent.text.Length - 1;
					if (gUIContent.text[gUIContent.text.Length - 1] != ' ')
					{
						while (num2 >= 0)
						{
							if (gUIContent.text[num2] == ' ' && num2 != gUIContent.text.Length - 1)
							{
								flag2 = true;
								break;
							}
							num2--;
						}
					}
					if (flag2)
					{
						num2++;
						text = gUIContent.text.Substring(num2, gUIContent.text.Length - num2);
						gUIContent.text = gUIContent.text.Remove(num2);
					}
					else
					{
						text = new string(new char[1]
						{
							base.text[i]
						});
						gUIContent.text = gUIContent.text.Remove(gUIContent.text.Length - 1);
					}
					vector = Style.UnityStyle.CalcSize(gUIContent);
				}
				if (vector.x > (float)longestLine)
				{
					longestLine = (int)vector.x;
				}
				if (flag)
				{
					Rect rect = (labelLines.Count == 0) ? new Rect(0f, 0f, base.rect.width, vector.y) : new Rect(labelLines[labelLines.Count - 1].rect.x, labelLines[labelLines.Count - 1].rect.y + (float)verticalKerning, base.rect.width, vector.y);
					if (num + item.rect.height < base.size.y || noLineLimit)
					{
						item = default(LabelLine);
						item.rect = rect;
						item.text = gUIContent.text;
						item.content = new GUIContent();
						item.content.text = gUIContent.text;
						labelLines.Add(item);
						num += item.rect.height;
					}
					else if (noLineLimit)
					{
					}
				}
				flag = false;
			}
			if (labelLines.Count == 0)
			{
				item = default(LabelLine);
				item.rect = new Rect(0f, 0f, base.rect.width, vector.y);
				item.text = gUIContent.text;
				item.content = new GUIContent();
				item.content.text = text;
				labelLines.Add(item);
				num += item.rect.height;
			}
			else if (text != string.Empty && (num + item.rect.height < base.size.y || noLineLimit))
			{
				item = default(LabelLine);
				item.rect = new Rect(0f, labelLines[labelLines.Count - 1].rect.y + (float)verticalKerning, base.rect.width, vector.y);
				item.text = gUIContent.text;
				item.content = new GUIContent();
				item.content.text = text;
				labelLines.Add(item);
				num += item.rect.height;
			}
		}
		else if (AutoSizeText == AutoSizeTextEnum.ShrinkOnly)
		{
			gUIContent.text = base.text;
			bool flag3 = false;
			SHSStyle sHSStyle = new SHSStyle(Style);
			while (!flag3)
			{
				vector = sHSStyle.UnityStyle.CalcSize(gUIContent);
				if (vector.x < x)
				{
					flag3 = true;
					continue;
				}
				sHSStyle.UnityStyle.fontSize = sHSStyle.UnityStyle.fontSize - 1;
				if (sHSStyle.UnityStyle.fontSize > 0)
				{
					continue;
				}
				CspUtils.DebugLog("Can't fit text small enough. Consider making multiline or making text area larger.");
				break;
			}
			if (flag3)
			{
				Style = sHSStyle;
			}
			item = default(LabelLine);
			item.rect = new Rect(0f, 0f, base.rect.width, vector.y);
			item.text = gUIContent.text;
			item.content = new GUIContent();
			item.content.text = gUIContent.text;
			labelLines.Add(item);
			num += item.rect.height;
		}
		else if (AutoSizeText == AutoSizeTextEnum.FitToRect)
		{
			throw new NotImplementedException("Fit To Rect not yet implemented.");
		}
		layoutCalculated = true;
		bool flag4 = base.TextAlignment == TextAnchor.MiddleCenter || base.TextAlignment == TextAnchor.MiddleLeft || base.TextAlignment == TextAnchor.MiddleRight;
		bool flag5 = base.TextAlignment == TextAnchor.LowerCenter || base.TextAlignment == TextAnchor.LowerLeft || base.TextAlignment == TextAnchor.LowerRight;
		float num3 = TotalTextHeight;
		Vector2 size = Size;
		if (num3 < size.y && (flag4 || flag5))
		{
			float startpoint = 0f;
			if (flag4)
			{
				Vector2 size2 = Size;
				startpoint = size2.y * 0.5f - (float)TotalTextHeight * 0.5f;
			}
			else if (flag5)
			{
				Vector2 size3 = Size;
				startpoint = size3.y - (float)TotalTextHeight;
			}
			CalculateNewAnchorOffset(startpoint);
		}
	}

	private void CalculateNewAnchorOffset(float startpoint)
	{
		LabelLine value = labelLines[0];
		value.rect = new Rect(value.rect.x, value.rect.y + (startpoint - value.rect.y), value.rect.width, value.rect.height);
		labelLines[0] = value;
		for (int i = 1; i < labelLines.Count; i++)
		{
			value = labelLines[i];
			value.rect = new Rect(0f, labelLines[i - 1].rect.y + (float)verticalKerning, value.rect.width, value.rect.height);
			labelLines[i] = value;
		}
	}

	public void ClearKerning()
	{
		layoutCalculated = false;
		if (labelLines != null)
		{
			labelLines.Clear();
			labelLines = null;
		}
	}

	public void InvalidateKerning()
	{
		verticalKerning = -1;
		ClearKerning();
	}

	public static Color GenColor(int r, int g, int b)
	{
		return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
	}

	protected void OffsetTextPosition(Vector2 offset)
	{
		if (verticalKerning != -1)
		{
			for (int i = 0; i < labelLines.Count; i++)
			{
				LabelLine value = labelLines[i];
				value.rect = new Rect(value.rect.x + offset.x, value.rect.y + offset.y, value.rect.width, value.rect.height);
				labelLines[i] = value;
			}
		}
		else
		{
			base.rect = new Rect(base.rect.x + offset.x, base.rect.y + offset.y, base.rect.width, base.rect.height);
		}
	}

	public float GetLineWidth(int line)
	{
		//Discarded unreachable code: IL_0044, IL_0065
		if (labelLines == null)
		{
			return GetTextWidth();
		}
		try
		{
			GUIStyle unityStyle = Style.UnityStyle;
			LabelLine labelLine = labelLines[line];
			Vector2 vector = unityStyle.CalcSize(labelLine.content);
			return vector.x;
		}
		catch (Exception ex)
		{
			CspUtils.DebugLog(ex.Message);
			return 0f;
		}
	}
}
