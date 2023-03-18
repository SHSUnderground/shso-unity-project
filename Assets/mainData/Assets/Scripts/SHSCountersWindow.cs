using System.Collections.Generic;
using UnityEngine;

public class SHSCountersWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private Vector2 scrollPos;

	private SHSStyle headerStyle;

	private SHSStyle entryStyle;

	public SHSCountersWindow(string name)
		: base(name, null)
	{
	}

	public override void ConfigureKeyBanks()
	{
		base.ConfigureKeyBanks();
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		GUILayout.BeginArea(new Rect(30f, 30f, 850f, base.rect.height - 50f));
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		if (GUILayout.Button("Clear Counters", GUILayout.Width(90f)))
		{
			AppShell.Instance.CounterManager.Reset();
		}
		GUILayout.Label("COUNTER TYPES...", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		if (AppShell.Instance.CounterManager.Counters.Count > 0)
		{
			GUILayout.BeginVertical();
			GUILayout.Space(10f);
			foreach (ISHSCounterType value in AppShell.Instance.CounterManager.Counters.Values)
			{
				drawCounterInfo(value, 1);
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void drawCounterInfo(ISHSCounterType counter, int level)
	{
		foreach (SHSCounterBank key2 in counter.QualifierValues.Keys)
		{
			foreach (KeyValuePair<string, long> item in counter.QualifierValues[key2])
			{
				string key = item.Key;
				GUILayout.BeginHorizontal(GUILayout.Width(700f));
				GUILayout.Space((level - 1) * 20);
				GUILayout.Label(key2.Id + ((!key2.ReadOnly) ? string.Empty : " (RO)"), headerStyle.UnityStyle, GUILayout.Width(75f));
				GUILayout.Label(counter.Name + "(" + key + ")", headerStyle.UnityStyle, GUILayout.Width(225f));
				GUILayout.Label(counter.QualifierValues[key2][key].ToString(), entryStyle.UnityStyle, GUILayout.Width(300f));
				GUILayout.EndHorizontal();
			}
		}
		foreach (ISHSCounterType value in counter.SubCounters.Values)
		{
			drawCounterInfo(value, level + 1);
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		headerStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleEventHeader");
		entryStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleLogEntry");
		scrollPos = Vector2.zero;
	}
}
