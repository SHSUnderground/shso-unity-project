using System.Collections.Generic;
using UnityEngine;

public class SHSDebugInputViewerWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private Vector2 scrollPos;

	private SHSStyle headerStyle;

	public SHSDebugInputViewerWindow(string name)
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
		GUILayout.Label("INPUT STACK...", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		List<IGUIControl> inputStack = GUIManager.Instance.InputStack;
		GUILayout.BeginVertical();
		GUILayout.Space(20f);
		foreach (IGUIControl item in inputStack)
		{
			GUIManager.InputState inputState = GUIManager.Instance.InputStateLookup[item];
			GUILayout.BeginHorizontal(GUILayout.Width(700f));
			GUILayout.Label(item.Id, headerStyle.UnityStyle, GUILayout.Width(250f));
			GUILayout.Label(inputState.frameAdded.ToString(), GUILayout.Width(50f));
			GUILayout.Label(inputState.frameUpdated.ToString(), GUILayout.Width(50f));
			GUILayout.Label(inputState.stackIndex.ToString(), GUILayout.Width(50f));
			GUILayout.Label(inputState.inputEventType.ToString(), GUILayout.Width(150f));
			GUILayout.EndHorizontal();
		}
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
