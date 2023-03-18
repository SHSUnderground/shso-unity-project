using UnityEngine;

public class SHSCharacterCombinationViewerWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private Vector2 scrollPos;

	private SHSStyle headerStyle;

	private bool enabled;

	public SHSCharacterCombinationViewerWindow(string name)
		: base(name, null)
	{
	}

	public override void Update()
	{
		base.Update();
		enabled = (!(BrawlerController.Instance == null) && BrawlerController.Instance.CharacterCombinationManager != null);
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		if (enabled)
		{
			DrawCombinations();
		}
	}

	private void DrawCombinations()
	{
		CharacterCombinationManager characterCombinationManager = BrawlerController.Instance.CharacterCombinationManager;
		GUILayout.BeginArea(new Rect(10f, 10f, base.rect.width - 10f, base.rect.height - 10f));
		scrollPos = GUILayout.BeginScrollView(scrollPos, true, true);
		GUILayout.Label("CHARACTER COMBINATIONS", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		GUILayout.BeginVertical();
		GUILayout.Label("Active Combinations", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleHeader").UnityStyle);
		GUILayout.Label("----------------------------------------", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleHeader").UnityStyle);
		Color color = GUI.color;
		GUILayout.BeginHorizontal(GUILayout.Width(1000f));
		GUILayout.Label("Combo Name", headerStyle.UnityStyle, GUILayout.Width(100f));
		GUILayout.Label("Combo Id", headerStyle.UnityStyle, GUILayout.Width(100f));
		GUILayout.Label("Active Characters", headerStyle.UnityStyle, GUILayout.Width(800f));
		GUILayout.EndHorizontal();
		if (characterCombinationManager.ActiveCombinations != null)
		{
			foreach (BaseCharacterCombination activeCombination in characterCombinationManager.ActiveCombinations)
			{
				GUILayout.BeginHorizontal(GUILayout.Width(1000f));
				GUI.color = Color.green;
				GUILayout.Label(activeCombination.DisplayName, headerStyle.UnityStyle, GUILayout.Width(100f));
				GUILayout.Label(activeCombination.Id, headerStyle.UnityStyle, GUILayout.Width(100f));
				string text = string.Empty;
				foreach (string activeCharacter in activeCombination.ActiveCharacters)
				{
					text = text + activeCharacter + "; ";
				}
				GUILayout.Label(text, headerStyle.UnityStyle);
				GUI.color = color;
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.Space(20f);
		GUILayout.Label("Preset Combination Definitions", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleHeader").UnityStyle);
		GUILayout.Label("----------------------------------------", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleHeader").UnityStyle);
		if (characterCombinationManager.PresetCombinations != null)
		{
			foreach (BaseCharacterCombination presetCombination in characterCombinationManager.PresetCombinations)
			{
				GUILayout.Label("Combo Name: " + presetCombination.DisplayName, headerStyle.UnityStyle);
				GUILayout.Label("Combo Id: " + presetCombination.Id, headerStyle.UnityStyle);
				GUI.color = Color.green;
				GUILayout.Label("Combat Effect: " + presetCombination.CombatEffect, headerStyle.UnityStyle);
				GUILayout.Label(presetCombination.ToString(), headerStyle.UnityStyle);
				GUI.color = color;
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
		scrollPos = Vector2.zero;
	}
}
