using UnityEngine;

public class SHSLevelUpViewerWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private Vector2 scrollPos;

	private bool showDormant;

	public SHSLevelUpViewerWindow(string PanelName)
		: base(PanelName, null)
	{
		SetBackground(new Color(1f, 0.4f, 1f, 1f));
	}

	public override void OnShow()
	{
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
		GUILayout.Label("Level Up Information", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		GUILayout.BeginVertical();
		XpToLevelDefinition instance = XpToLevelDefinition.Instance;
		GUILayout.BeginHorizontal(GUILayout.Width(800f));
		GUILayout.Label("Level ", GUILayout.Width(50f));
		GUILayout.Label("XP", GUILayout.Width(50f));
		GUILayout.EndHorizontal();
		for (int i = 1; i <= XpToLevelDefinition.Instance.MaxLevel; i++)
		{
			GUILayout.BeginHorizontal(GUILayout.Width(800f));
			GUILayout.Label("Level " + i.ToString(), GUILayout.Width(75f));
			GUILayout.Label(instance.GetXpDescriptionForLevel(i).ToString(), GUILayout.Width(50f));
			GUILayout.EndHorizontal();
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
