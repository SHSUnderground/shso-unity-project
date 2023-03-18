using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SHSExpendablesWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private Vector2 scrollPos;

	private SHSStyle headerStyle;

	private ExpendablesManager mgr;

	private GUIToggleButton ShowDefinitionsButton;

	private GUIToggleButton ShowExpendablesStateButton;

	public SHSExpendablesWindow(string name)
		: base(name, null)
	{
		ShowExpendablesStateButton = new GUIToggleButton();
		ShowExpendablesStateButton.Text = "Expendable State";
		ShowExpendablesStateButton.Spacing = 30f;
		ShowExpendablesStateButton.SetButtonSize(new Vector2(30f, 30f));
		ShowExpendablesStateButton.SetPositionAndSize(220f, 20f, 150f, 30f);
		ShowExpendablesStateButton.StyleInfo = SHSInheritedStyleInfo.Instance;
		ShowExpendablesStateButton.Value = true;
		ShowExpendablesStateButton.Changed += delegate
		{
			ShowDefinitionsButton.Value = !ShowExpendablesStateButton.Value;
		};
		Add(ShowExpendablesStateButton);
		ShowDefinitionsButton = new GUIToggleButton();
		ShowDefinitionsButton.Text = "Expendable Types";
		ShowDefinitionsButton.Spacing = 30f;
		ShowDefinitionsButton.SetButtonSize(new Vector2(30f, 30f));
		ShowDefinitionsButton.SetPositionAndSize(20f, 20f, 150f, 30f);
		ShowDefinitionsButton.StyleInfo = SHSInheritedStyleInfo.Instance;
		ShowDefinitionsButton.Changed += delegate
		{
			ShowExpendablesStateButton.Value = !ShowDefinitionsButton.Value;
		};
		Add(ShowDefinitionsButton);
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		if (ShowDefinitionsButton.Value)
		{
			drawDefinitions();
		}
		else if (ShowExpendablesStateButton.Value)
		{
			drawState();
		}
	}

	private void drawDefinitions()
	{
		GUILayout.BeginArea(new Rect(30f, 70f, base.rect.width - 40f, base.rect.height - 70f));
		scrollPos = GUILayout.BeginScrollView(scrollPos, true, true);
		GUILayout.Label("EXPENDABLE DEFINITIONS", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		if (AppShell.Instance.ExpendablesManager.ExpendableTypes.Count > 0)
		{
			mgr = AppShell.Instance.ExpendablesManager;
			GUILayout.BeginVertical();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(GUILayout.Width(1000f));
			GUILayout.Label("ID", headerStyle.UnityStyle, GUILayout.Width(50f));
			GUILayout.Label("NAME", headerStyle.UnityStyle, GUILayout.Width(100f));
			GUILayout.Label("DUR", headerStyle.UnityStyle, GUILayout.Width(30f));
			GUILayout.Label("COOL", headerStyle.UnityStyle, GUILayout.Width(30f));
			GUILayout.Label("HANDLER", headerStyle.UnityStyle, GUILayout.Width(300f));
			GUILayout.Label("CATEGORIES", headerStyle.UnityStyle, GUILayout.Width(120f));
			GUILayout.Label("ICON", headerStyle.UnityStyle, GUILayout.Width(200f));
			GUILayout.Label("IMAGE", headerStyle.UnityStyle, GUILayout.Width(200f));
			GUILayout.Label("DESC", headerStyle.UnityStyle, GUILayout.Width(300f));
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			foreach (ExpendableDefinition value in mgr.ExpendableTypes.Values)
			{
				GUILayout.BeginHorizontal(GUILayout.Width(1000f));
				GUILayout.Label(value.OwnableTypeId, headerStyle.UnityStyle, GUILayout.Width(50f));
				GUILayout.Label(value.Name, headerStyle.UnityStyle, GUILayout.Width(100f));
				GUILayout.Label(value.Duration.ToString(), headerStyle.UnityStyle, GUILayout.Width(30f));
				GUILayout.Label(value.Cooldown.ToString(), headerStyle.UnityStyle, GUILayout.Width(50f));
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(value.ExpendEffectHandler);
				foreach (ExpendHandlerParameters parameter in value.Parameters)
				{
					stringBuilder.AppendLine(" -- " + parameter.Key + " : " + parameter.Value);
				}
				GUILayout.Label(stringBuilder.ToString(), headerStyle.UnityStyle, GUILayout.Width(300f));
				GUILayout.Label(string.Join(Environment.NewLine, value.CategoryNames), headerStyle.UnityStyle, GUILayout.Width(120f));
				GUILayout.Label(value.InventoryIcon, headerStyle.UnityStyle, GUILayout.Width(200f));
				GUILayout.Label(value.HoverHelpIcon, headerStyle.UnityStyle, GUILayout.Width(200f));
				GUILayout.Label(value.Description, headerStyle.UnityStyle, GUILayout.Width(300f));
				GUILayout.EndHorizontal();
				GUILayout.Space(3f);
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void drawState()
	{
		GUILayout.BeginArea(new Rect(30f, 70f, base.rect.width - 40f, base.rect.height - 70f));
		scrollPos = GUILayout.BeginScrollView(scrollPos, true, true);
		GUILayout.Label("EXPENDABLE STATE", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		mgr = AppShell.Instance.ExpendablesManager;
		GUILayout.BeginVertical();
		if (mgr.activeExpendQueue.Count > 0)
		{
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(GUILayout.Width(1000f));
			GUILayout.Label(string.Empty, headerStyle.UnityStyle, GUILayout.Width(55f));
			GUILayout.Label("RID", headerStyle.UnityStyle, GUILayout.Width(35f));
			GUILayout.Label("STATUS", headerStyle.UnityStyle, GUILayout.Width(150f));
			GUILayout.Label("TYPE", headerStyle.UnityStyle, GUILayout.Width(75f));
			GUILayout.Label("TIME", headerStyle.UnityStyle, GUILayout.Width(50f));
			GUILayout.Label("HISTORY", headerStyle.UnityStyle, GUILayout.Width(500f));
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			Color color = GUI.color;
			GUI.color = Color.cyan;
			foreach (IExpendHandler value in mgr.activeExpendQueue.Values)
			{
				GUILayout.BeginHorizontal(GUILayout.Width(1000f));
				GUILayout.Label(value.ExpendRequestId.ToString(), headerStyle.UnityStyle, GUILayout.Width(75f));
				GUILayout.Label(value.State.ToString(), headerStyle.UnityStyle, GUILayout.Width(150f));
				GUILayout.Label(value.OwnableTypeId, headerStyle.UnityStyle, GUILayout.Width(75f));
				GUILayout.Label((Time.time - value.StartTime).ToString(), headerStyle.UnityStyle, GUILayout.Width(50f));
				StringBuilder stringBuilder = new StringBuilder();
				foreach (KeyValuePair<float, string> item in value.ExpendHistoryLog)
				{
					stringBuilder.AppendLine(item.Key + ": " + item.Value);
				}
				GUILayout.Label(stringBuilder.ToString(), headerStyle.UnityStyle, GUILayout.Width(500f));
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
			}
			GUI.color = color;
		}
		if (mgr.expendHistoryQueue.Count > 0)
		{
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(GUILayout.Width(1000f));
			GUILayout.Label("RID", headerStyle.UnityStyle, GUILayout.Width(35f));
			GUILayout.Label("STATUS", headerStyle.UnityStyle, GUILayout.Width(150f));
			GUILayout.Label("TYPE", headerStyle.UnityStyle, GUILayout.Width(75f));
			GUILayout.Label("TIMEOUT", headerStyle.UnityStyle, GUILayout.Width(50f));
			GUILayout.Label("HISTORY", headerStyle.UnityStyle, GUILayout.Width(500f));
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			Color color2 = GUI.color;
			foreach (IExpendHandler value2 in mgr.expendHistoryQueue.Values)
			{
				GUI.color = ((value2.State == ExpendHandlerState.Failed) ? Color.red : ((value2.State != ExpendHandlerState.Completed) ? Color.yellow : Color.white));
				GUILayout.BeginHorizontal(GUILayout.Width(1000f));
				GUILayout.Label(value2.ExpendRequestId.ToString(), headerStyle.UnityStyle, GUILayout.Width(35f));
				GUILayout.Label(value2.State.ToString(), headerStyle.UnityStyle, GUILayout.Width(150f));
				GUILayout.Label(value2.OwnableTypeId, headerStyle.UnityStyle, GUILayout.Width(75f));
				GUILayout.Label(value2.Timeout.ToString(), headerStyle.UnityStyle, GUILayout.Width(50f));
				StringBuilder stringBuilder2 = new StringBuilder();
				foreach (KeyValuePair<float, string> item2 in value2.ExpendHistoryLog)
				{
					stringBuilder2.AppendLine(item2.Key + ": " + item2.Value);
				}
				GUILayout.Label(stringBuilder2.ToString(), headerStyle.UnityStyle, GUILayout.Width(500f));
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
			}
			GUI.color = color2;
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
