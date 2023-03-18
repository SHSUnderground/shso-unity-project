using System.Collections.Generic;
using UnityEngine;

public class SHSStringTableViewerWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private Vector2 scrollPos;

	private SHSStyle headerStyle;

	private string searchString;

	public SHSStringTableViewerWindow(string name)
		: base(name, null)
	{
		GUILabel gUILabel = GUIControl.CreateControl<GUILabel>(new Vector2(194f, 25f), new Vector2(-50f, 30f), DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(239, 255, 119), TextAnchor.UpperLeft);
		gUILabel.Text = "Search:";
		Add(gUILabel);
		GUITextField search = GUIControl.CreateControl<GUITextField>(new Vector2(194f, 25f), new Vector2(-50f, 50f), DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		search.FontFace = GUIFontManager.SupportedFontEnum.Komica;
		search.FontSize = 15;
		search.TextColor = GUILabel.GenColor(81, 82, 81);
		search.Color = GUILabel.GenColor(0, 0, 255);
		search.BackgroundColor = GUILabel.GenColor(255, 0, 255);
		search.TextAlignment = TextAnchor.UpperLeft;
		search.Changed += delegate
		{
			searchString = search.Text;
		};
		Add(search);
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		GUILayout.BeginArea(new Rect(30f, 30f, 900f, base.rect.height - 50f));
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		GUILayout.Label("String Table", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		StringTable stringTable = AppShell.Instance.stringTable;
		if (stringTable == null)
		{
			GUILayout.Label("--- String Table not loaded yet ---");
			GUILayout.EndScrollView();
			GUILayout.EndArea();
			return;
		}
		GUILayout.BeginVertical();
		GUILayout.Space(20f);
		if (!string.IsNullOrEmpty(searchString))
		{
			foreach (KeyValuePair<string, string> entry in stringTable.Entries)
			{
				if (entry.Key.Contains(searchString) || entry.Value.Contains(searchString))
				{
					GUILayout.BeginHorizontal(GUILayout.Width(900f));
					GUILayout.Label(entry.Key, headerStyle.UnityStyle, GUILayout.Width(225f));
					GUILayout.Label(entry.Value, headerStyle.UnityStyle, GUILayout.Width(775f));
					GUILayout.EndHorizontal();
				}
			}
		}
		GUILayout.Space(30f);
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	public override void OnShow()
	{
		base.OnShow();
		headerStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleEventHeader");
		scrollPos = Vector2.zero;
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
	}
}
