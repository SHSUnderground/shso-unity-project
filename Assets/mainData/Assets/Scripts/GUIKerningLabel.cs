using UnityEngine;

public class GUIKerningLabel : GUITextControlBase
{
	protected string[] textLines;

	protected Rect[] textRects;

	protected float kerning = -1f;

	public float Kerning
	{
		get
		{
			return kerning;
		}
		set
		{
			kerning = value;
			HandleResize(new GUIResizeMessage(HandleResizeSource.Control, base.rect, base.rect));
		}
	}

	public override string Text
	{
		get
		{
			return text;
		}
		set
		{
			base.Text = value;
			HandleResize(new GUIResizeMessage(HandleResizeSource.Control, base.rect, base.rect));
		}
	}

	public GUIKerningLabel()
	{
		Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		if (textLines == null)
		{
			if (Style == SHSStyle.NoStyle)
			{
				GUI.Label(base.rect, base.Content);
			}
			else
			{
				GUI.Label(base.rect, base.Content, Style.UnityStyle);
			}
		}
		else
		{
			for (int i = 0; i < textLines.Length; i++)
			{
				if (Style == SHSStyle.NoStyle)
				{
					GUI.Label(textRects[i], textLines[i]);
				}
				else
				{
					GUI.Label(textRects[i], textLines[i], Style.UnityStyle);
				}
			}
		}
		base.Draw(drawFlags);
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		textLines = null;
		base.HandleResize(message);
		GUIStyle gUIStyle = (Style != SHSStyle.NoStyle) ? Style.UnityStyle : new GUIStyle();
		if (gUIStyle == null)
		{
			CspUtils.DebugLog("No available font style!");
			return;
		}
		GUIContent content = new GUIContent(text);
		float lineHeight = gUIStyle.lineHeight;
		float num = kerning;
		if (num == -1f)
		{
			num = lineHeight;
		}
		float height = num * 1.1f;
		float num2 = gUIStyle.CalcHeight(content, base.rect.width);
		int num3 = (int)Mathf.Ceil(num2 / lineHeight);
		Vector2 cursorPixelPosition = new Vector2(0f, 0f);
		Rect position = new Rect(0f, 0f, base.rect.width, num2);
		textLines = new string[num3];
		textRects = new Rect[num3];
		int i = 1;
		int num4 = 0;
		for (; i <= num3; i++)
		{
			if (num4 == text.Length)
			{
				break;
			}
			cursorPixelPosition.y = lineHeight * (float)i + 1f;
			int cursorStringIndex = gUIStyle.GetCursorStringIndex(position, content, cursorPixelPosition);
			textLines[i - 1] = text.Substring(num4, cursorStringIndex - num4);
			textRects[i - 1] = new Rect(base.rect);
			textRects[i - 1].y += num * (float)(i - 1);
			textRects[i - 1].height = height;
			num4 = cursorStringIndex;
		}
	}
}
