using UnityEngine;

public class SHSTutorialViewerWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private delegate void StateChangedDelegate();

	private GUILayoutControlWindow mainWindow;

	private SHSStyle headerStyle;

	private Vector2 scrollPos;

	public SHSTutorialViewerWindow(string PanelName)
		: base(PanelName, null)
	{
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		GUILayout.BeginArea(new Rect(30f, 30f, 900f, base.rect.height - 50f));
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		GUILayout.Label("Tutorials", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		GUILayout.BeginVertical();
		GUILayout.Space(20f);
		GUILayout.BeginHorizontal(GUILayout.Width(700f));
		GUILayout.Label("Tutorial", headerStyle.UnityStyle, GUILayout.Width(150f));
		GUILayout.Label("Stage", headerStyle.UnityStyle, GUILayout.Width(40f));
		GUILayout.Label("Normal", headerStyle.UnityStyle, GUILayout.Width(75f));
		GUILayout.Label("Force On", headerStyle.UnityStyle, GUILayout.Width(75f));
		GUILayout.Label("Force Off", headerStyle.UnityStyle, GUILayout.Width(75f));
		GUILayout.Label("Counter", headerStyle.UnityStyle, GUILayout.Width(140f));
		GUILayout.Label("GUI", headerStyle.UnityStyle, GUILayout.Width(300f));
		GUILayout.EndHorizontal();
		GUILayout.Space(15f);
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void setTutorialState(string state, string buttonState, string buttonText, string prefKey, StateChangedDelegate onChanged)
	{
		if (state == buttonState)
		{
			GUILayout.Label(buttonText, headerStyle.UnityStyle, GUILayout.Width(75f));
		}
		else if (GUILayout.Button(new GUIContent(buttonText), GUILayout.Width(75f)))
		{
			PlayerPrefs.SetString(prefKey, buttonState);
			if (onChanged != null)
			{
				onChanged();
			}
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		headerStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleEventHeader");
		scrollPos = Vector2.zero;
	}
}
