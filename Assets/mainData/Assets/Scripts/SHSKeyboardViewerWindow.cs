using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class SHSKeyboardViewerWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private SHSStyle headerStyle;

	private Vector2 scrollPos;

	private bool showDormant;

	public SHSKeyboardViewerWindow(string PanelName)
		: base(PanelName, null)
	{
		SetBackground(new Color(1f, 0.4f, 1f, 1f));
	}

	public override void OnShow()
	{
		headerStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleHeader");
		scrollPos = Vector2.zero;
		base.OnShow();
	}

	public override void DrawPreprocess()
	{
		base.DrawPreprocess();
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		GUILayout.BeginArea(new Rect(30f, 30f, 850f, base.rect.height - 50f));
		GUILayout.Label("KEYBOARD MAPPINGS...", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		showDormant = GUILayout.Toggle(showDormant, "Show Inactive Controls");
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		GUILayout.BeginVertical();
		for (int num = SHSInput.EventListeners.Count - 1; num >= 0; num--)
		{
			KeyBank keyBank = SHSInput.EventListeners[num];
			string str = keyBank.Source.ToString();
			str = "(" + keyBank.InputState.ToString() + ") " + str;
			if (keyBank.KeyEventDictionary.Count > 0)
			{
				GUILayout.Label(str, headerStyle.UnityStyle);
			}
			else
			{
				if (!showDormant)
				{
					continue;
				}
				GUILayout.Label(str);
			}
			foreach (KeyValuePair<KeyCodeEntry, SHSInput.KeyEventDelegate> item in keyBank.KeyEventDictionary)
			{
				KeyCodeEntry key = item.Key;
				int num2 = Convert.ToInt32(key.KeyCode);
				num2 = ((num2 << 4) | (key.Alt ? 1 : (key.Control ? 2 : (key.Shift ? 4 : 0))));
				if (item.Value != null)
				{
					GUILayout.BeginHorizontal(GUILayout.Width(800f));
					GUILayout.Space(30f);
					KeyCodeEntry key2 = item.Key;
					GUILayout.Label(key2.KeyCode.ToString(), GUILayout.Width(70f));
					GUILayout.BeginHorizontal();
					KeyCodeEntry key3 = item.Key;
					GUILayout.Label((!key3.Control) ? "-----" : "Ctrl ", GUILayout.Width(30f));
					KeyCodeEntry key4 = item.Key;
					GUILayout.Label((!key4.Alt) ? "-----" : "Alt  ", GUILayout.Width(30f));
					KeyCodeEntry key5 = item.Key;
					GUILayout.Label((!key5.Shift) ? "-----" : "Shift", GUILayout.Width(30f));
					string text = item.Value.Method.ToString();
					string text2 = string.Empty;
					GUILayout.Label(text.Substring(0, text.IndexOf("(")), GUILayout.Width(260f));
					DescriptionAttribute[] array = (DescriptionAttribute[])item.Value.Method.GetCustomAttributes(typeof(DescriptionAttribute), false);
					if (array.Length > 0)
					{
						text2 += array[0].Description;
					}
					GUILayout.Label(text2, GUILayout.Width(330f));
					GUILayout.EndHorizontal();
					GUILayout.EndHorizontal();
				}
			}
			if (keyBank.BankType == KeyBank.KeyBankTypeEnum.Blocking)
			{
				CspUtils.DebugLog("Bank " + keyBank + " is blocking all lower input. Returning...");
				break;
			}
		}
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
	}

	public override void DrawFinalize()
	{
		base.DrawFinalize();
	}
}
