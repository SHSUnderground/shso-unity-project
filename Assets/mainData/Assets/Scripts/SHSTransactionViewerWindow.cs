using UnityEngine;

public class SHSTransactionViewerWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private Vector2 scrollPos;

	private SHSStyle headerStyle;

	private SHSStyle entryStyle;

	private string currentExpandedTransaction;

	private bool expandAll;

	public SHSTransactionViewerWindow(string name)
		: base(name, null)
	{
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		GUILayout.BeginArea(new Rect(30f, 30f, 1000f, base.rect.height - 50f));
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		GUILayout.BeginVertical();
		GUILayout.Space(10f);
		GUILayout.Space(10f);
		GUILayout.Label("Game Transactions", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		if (GUILayout.Button((!expandAll) ? "Expand All" : "Compact All"))
		{
			expandAll = !expandAll;
		}
		GUILayout.Space(12f);
		GUILayout.BeginHorizontal(GUILayout.Width(1000f));
		GUILayout.Label("Transaction Name", headerStyle.UnityStyle, GUILayout.Width(300f));
		GUILayout.Label("Start", headerStyle.UnityStyle, GUILayout.Width(100f));
		GUILayout.Label("End", headerStyle.UnityStyle, GUILayout.Width(100f));
		GUILayout.Label("Result", headerStyle.UnityStyle, GUILayout.Width(50f));
		GUILayout.Label("Steps", headerStyle.UnityStyle, GUILayout.Width(50f));
		GUILayout.EndHorizontal();
		GUILayout.Space(20f);
		foreach (TransactionMonitor item in TransactionMonitor.TransactionHistory)
		{
			if (item.parent == null)
			{
				DisplayTransaction(item, 0);
			}
			GUILayout.Space(5f);
		}
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void DisplayTransaction(TransactionMonitor monitor, int nestingLevel)
	{
		Color color = GUI.color;
		GUI.color = ((monitor.result == TransactionMonitor.ExitCondition.NotExited) ? Color.cyan : ((monitor.result != 0) ? Color.red : Color.white));
		GUILayout.BeginHorizontal(GUILayout.Width(1000f));
		if (monitor.Id != currentExpandedTransaction && monitor.parent == null && GUILayout.Button("+"))
		{
			currentExpandedTransaction = monitor.Id;
		}
		GUILayout.Space(40 * nestingLevel);
		SHSStyle sHSStyle = (monitor.parent == null) ? headerStyle : entryStyle;
		GUILayout.Label(monitor.Id, sHSStyle.UnityStyle, GUILayout.Width(300f));
		GUILayout.Label(monitor.startTime.ToString(), sHSStyle.UnityStyle, GUILayout.Width(100f));
		GUILayout.Label(monitor.endTime.ToString(), sHSStyle.UnityStyle, GUILayout.Width(100f));
		GUILayout.Label(monitor.result.ToString(), sHSStyle.UnityStyle, GUILayout.Width(50f));
		GUILayout.Label(monitor.StepCount.ToString(), sHSStyle.UnityStyle, GUILayout.Width(50f));
		GUILayout.EndHorizontal();
		GUI.color = color;
		if (monitor.Id == currentExpandedTransaction || expandAll || monitor.parent != null)
		{
			foreach (TransactionMonitor.Step step in monitor.Steps)
			{
				color = GUI.color;
				GUI.color = ((step.result != TransactionMonitor.ExitCondition.NotExited) ? ((step.result != 0) ? Color.red : Color.white) : Color.blue);
				GUILayout.BeginHorizontal(GUILayout.Width(600f));
				GUILayout.Space(40 + 40 * nestingLevel);
				GUILayout.Label("Step: " + step.name, entryStyle.UnityStyle, GUILayout.Width(200f));
				GUILayout.Label(step.startTime.ToString(), entryStyle.UnityStyle, GUILayout.Width(100f));
				GUILayout.Label(step.endTime.ToString(), entryStyle.UnityStyle, GUILayout.Width(100f));
				GUILayout.Label(step.result.ToString(), entryStyle.UnityStyle, GUILayout.Width(50f));
				GUILayout.EndHorizontal();
				GUI.color = color;
			}
			foreach (TransactionMonitor transaction in monitor.Transactions)
			{
				DisplayTransaction(transaction, nestingLevel + 1);
			}
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		headerStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleEventHeader");
		entryStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleLogEntry");
		scrollPos = Vector2.zero;
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
	}
}
