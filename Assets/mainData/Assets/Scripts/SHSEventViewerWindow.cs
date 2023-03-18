using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSEventViewerWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private Vector2 scrollPos;

	private SHSStyle headerStyle;

	private SHSStyle entryStyle;

	private SHSStyle entryListenerStyle;

	public SHSEventViewerWindow(string name)
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
		GUILayout.BeginArea(new Rect(30f, 30f, 550f, base.rect.height - 50f));
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		GUILayout.Label("EVENT MAPPINGS...", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		Dictionary<object, Dictionary<Type, List<ShsEventMgr.ICallback>>> listeners = AppShell.Instance.EventMgr.Listeners;
		if (listeners.Count <= 0)
		{
			return;
		}
		GUILayout.BeginVertical();
		GUILayout.Space(20f);
		object[] array = new object[listeners.Count];
		listeners.Keys.CopyTo(array, 0);
		Array.Sort(array, new ShsEventMgr.ObjectNameComparer());
		object[] array2 = array;
		foreach (object obj in array2)
		{
			Dictionary<Type, List<ShsEventMgr.ICallback>> dictionary = listeners[obj];
			Type[] array3 = new Type[dictionary.Count];
			dictionary.Keys.CopyTo(array3, 0);
			Array.Sort(array3, new ShsEventMgr.ObjectNameComparer());
			GameObject gameObject = obj as GameObject;
			string text = (!(gameObject != null)) ? obj.ToString() : gameObject.name;
			GUILayout.BeginHorizontal(GUILayout.Width(700f));
			GUILayout.Label(text, headerStyle.UnityStyle, GUILayout.Width(250f));
			GUILayout.Label("Listeners: " + dictionary.Count.ToString(), entryStyle.UnityStyle, GUILayout.Width(125f));
			GUILayout.EndHorizontal();
			Type[] array4 = array3;
			foreach (Type type in array4)
			{
				GUILayout.BeginHorizontal(GUILayout.Width(700f));
				GUILayout.Space(20f);
				GUILayout.Label(type.ToString(), headerStyle.UnityStyle, GUILayout.Width(300f));
				GUILayout.EndHorizontal();
				foreach (ShsEventMgr.ICallback item in dictionary[type])
				{
					GUILayout.BeginHorizontal(GUILayout.Width(700f));
					GUILayout.Space(40f);
					GUILayout.Label(item.ReferencedFunction.ToString(), entryListenerStyle.UnityStyle, GUILayout.Width(600f));
					GUILayout.EndHorizontal();
				}
			}
		}
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	public override void OnShow()
	{
		base.OnShow();
		headerStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleEventHeader");
		entryStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleLogEntry");
		entryListenerStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleEventListener");
		scrollPos = Vector2.zero;
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
	}
}
