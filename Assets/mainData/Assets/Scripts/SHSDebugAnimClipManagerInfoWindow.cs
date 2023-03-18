using System.Collections.Generic;
using UnityEngine;

public class SHSDebugAnimClipManagerInfoWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	public class DataNode
	{
		public const string Unknown = "---";

		public bool manager;

		public string name;

		public string info1;

		public string info2;

		public DataNode(bool manager, string name, string info1)
			: this(manager, name, info1, string.Empty)
		{
		}

		public DataNode(bool manager, string name, string info1, string info2)
		{
			this.manager = manager;
			this.name = name;
			this.info1 = info1;
			this.info2 = info2;
			if (name == null)
			{
				name = "---";
			}
			if (info1 == null)
			{
				info1 = string.Empty;
			}
			if (info2 == null)
			{
				info2 = string.Empty;
			}
		}
	}

	private SHSStyle headerStyle;

	private Vector2 scrollPos;

	public static bool RecordingAnim = false;

	private bool pauseState;

	public static List<DataNode> dataNodes = new List<DataNode>();

	public static List<DataNode> dataNodesToDisp = new List<DataNode>();

	public SHSDebugAnimClipManagerInfoWindow(string name)
		: base(name, null)
	{
	}

	public static void Publish(bool manager, string name, string info)
	{
		if (RecordingAnim)
		{
			dataNodes.Add(new DataNode(manager, name, info));
		}
	}

	public static void Publish(bool manager, string name, string info1, string info2)
	{
		if (RecordingAnim)
		{
			dataNodes.Add(new DataNode(manager, name, info1, info2));
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		headerStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleEventHeader");
		scrollPos = Vector2.zero;
		RecordingAnim = true;
	}

	public override void OnHide()
	{
		base.OnHide();
		RecordingAnim = false;
	}

	public override void Update()
	{
		base.Update();
		if (!pauseState)
		{
			List<DataNode> list = dataNodesToDisp;
			dataNodesToDisp = dataNodes;
			dataNodes = list;
		}
		dataNodes.Clear();
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		GUILayout.BeginArea(new Rect(30f, 30f, 700f, base.rect.height - 50f));
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		GUILayout.Label("Animations Running", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		GUILayout.BeginVertical();
		GUILayout.Space(20f);
		if (GUILayout.Button((!pauseState) ? "Pause State" : "Unpause State", GUILayout.Width(150f)))
		{
			pauseState = !pauseState;
		}
		GUILayout.Space(20f);
		GUILayout.BeginHorizontal(GUILayout.Width(700f));
		GUILayout.Space(40f);
		GUILayout.Label("ElapsedTime", headerStyle.UnityStyle, GUILayout.Width(75f));
		GUILayout.Label("Value", headerStyle.UnityStyle, GUILayout.Width(75f));
		GUILayout.Label("Name", headerStyle.UnityStyle, GUILayout.Width(300f));
		GUILayout.EndHorizontal();
		foreach (DataNode item in dataNodesToDisp)
		{
			GUILayout.BeginHorizontal(GUILayout.Width(700f));
			if (!item.manager)
			{
				GUILayout.Space(40f);
				GUILayout.Label(item.info1, headerStyle.UnityStyle, GUILayout.Width(75f));
				GUILayout.Label(item.info2, headerStyle.UnityStyle, GUILayout.Width(75f));
				GUILayout.Label(item.name, headerStyle.UnityStyle, GUILayout.Width(300f));
			}
			else
			{
				GUILayout.Label(item.name + "------------------------------", headerStyle.UnityStyle, GUILayout.Width(300f));
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}
}
