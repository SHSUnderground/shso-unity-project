using System.Collections.Generic;
using UnityEngine;

public class SHSHintLabel : GUISimpleControlWindow
{
	private GUILabel label;

	private string symbolMarkupPrefix;

	private string originalText;

	private List<GUIImage> symbolsAdded;

	private string _text;

	public string Text
	{
		get
		{
			return label.Text;
		}
		set
		{
			label.Text = value;
			originalText = value;
			AddSymbols();
		}
	}

	public Vector2 TextSize
	{
		get
		{
			return label.Style.UnityStyle.CalcSize(new GUIContent(label.Text));
		}
	}

	public override SHSStyle Style
	{
		get
		{
			return label.Style;
		}
		set
		{
			label.Style = new SHSStyle(value);
		}
	}

	public override bool IsVisible
	{
		get
		{
			return base.IsVisible;
		}
		set
		{
			label.IsVisible = value;
			foreach (GUIImage item in symbolsAdded)
			{
				item.IsVisible = value;
			}
			base.IsVisible = value;
		}
	}

	public SHSHintLabel()
	{
		symbolsAdded = new List<GUIImage>();
		symbolMarkupPrefix = "[symbol";
		label = GUIControl.CreateControlTopLeftFrame<GUILabel>(size, default(Vector2));
		label.Id = "textLabel";
		label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, ColorUtil.FromRGB255(78, 96, 109), TextAnchor.UpperLeft);
		label.WordWrap = true;
		Add(label);
	}

	public override void SetSize(Vector2 Size)
	{
		base.SetSize(Size);
		label.SetSize(Size);
		AddSymbols();
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		label.SetSize(Size);
		if (symbolsAdded.Count > 0)
		{
			label.Text = originalText;
			foreach (GUIImage item in symbolsAdded)
			{
				Remove(item);
			}
			AddSymbols();
		}
		base.HandleResize(message);
	}

	public void SetupText(GUIFontManager.SupportedFontEnum fontFace, int fontSize, Color color, TextAnchor anchor)
	{
		label.SetupText(fontFace, fontSize, color, anchor);
	}

	protected void AddSymbols()
	{
		originalText = label.Text;
		if (!label.Text.Contains(symbolMarkupPrefix))
		{
			return;
		}
		GUIStyle unityStyle = label.Style.UnityStyle;
		GUIContent content = new GUIContent(" ");
		Vector2 size = label.Size;
		float num = unityStyle.CalcHeight(content, size.x);
		GUILabel gUILabel = new GUILabel();
		gUILabel.Style = new SHSStyle(label.Style);
		gUILabel.Style.UnityStyle.wordWrap = false;
		Vector2 size2 = label.Size;
		gUILabel.SetSize(new Vector2(size2.x, num));
		Vector2 vector = gUILabel.Style.UnityStyle.CalcSize(new GUIContent("A A"));
		float x = vector.x;
		Vector2 vector2 = gUILabel.Style.UnityStyle.CalcSize(new GUIContent("AA"));
		float num2 = x - vector2.x;
		int num3 = label.Text.IndexOf(symbolMarkupPrefix);
		if (num3 == -1)
		{
			return;
		}
		int num4 = 0;
		Vector2 symbolPos = default(Vector2);
		int num5 = (int)Mathf.Ceil(num / num2);
		float offset = ((float)num5 * num2 - num) / 2f;
		float num6 = (float)num5 * num2;
		List<string> list = new List<string>();
		while (num3 != -1)
		{
			string text = label.Text.Substring(num3, label.Text.IndexOf(']', num3 + 1) - (num3 - 1));
			if (!list.Contains(text))
			{
				list.Add(text);
			}
			string text2 = label.Text.Substring(num4, num3 - num4);
			char[] array = text2.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				Vector2 vector3 = gUILabel.Style.UnityStyle.CalcSize(new GUIContent(array[i].ToString()));
				float num7 = vector3.x;
				if (array[i] == ' ')
				{
					num7 = num2;
				}
				symbolPos.x += num7;
				float x2 = symbolPos.x;
				Vector2 size3 = label.Size;
				if (x2 > size3.x)
				{
					symbolPos.x = 0f;
					symbolPos.y += num;
				}
			}
			AddSymbol(num, num2, offset, symbolPos, text);
			num4 = num3 + text.Length;
			symbolPos.x += num6;
			num3 = label.Text.IndexOf(symbolMarkupPrefix, num4);
		}
		string text3 = string.Empty;
		for (int j = 0; j < num5; j++)
		{
			text3 += " ";
		}
		foreach (string item in list)
		{
			label.Text = label.Text.Replace(item, text3);
		}
	}

	private void AddSymbol(float lineHeight, float spaceWidth, float offset, Vector2 symbolPos, string symbolMarkup)
	{
		Vector2 offset2 = new Vector2(symbolPos.x + offset, symbolPos.y);
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, offset2, new Vector2(lineHeight, lineHeight), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage.TextureSource = "GUI/Symbols/" + ConvertSymbolMarkupToTextureAssetName(symbolMarkup);
		gUIImage.Id = "Hint Symbol";
		Add(gUIImage);
		GUIManager.Instance.SetModal(gUIImage, ModalLevelEnum.Default);
		symbolsAdded.Add(gUIImage);
	}

	private string ConvertSymbolMarkupToTextureAssetName(string symbolMarkup)
	{
		string[] array = symbolMarkup.Split(' ');
		string text = array[1];
		return text.Remove(text.Length - 1, 1);
	}
}
