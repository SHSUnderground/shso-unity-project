using System.Collections.Generic;
using UnityEngine;

public class SHSSmartTipsViewerWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private Vector2 scrollPos;

	private SHSStyle headerStyle;

	private GUIToggleButton CategoryViewButton;

	private GUIToggleButton ShowTitlesButton;

	private GUIButton SimulateButton;

	private GUIDropDownListBox categoryList;

	public SHSSmartTipsViewerWindow(string name)
		: base(name, null)
	{
		CategoryViewButton = new GUIToggleButton();
		CategoryViewButton.Text = "Category View";
		CategoryViewButton.Spacing = 30f;
		CategoryViewButton.SetButtonSize(new Vector2(30f, 30f));
		CategoryViewButton.SetPositionAndSize(20f, 20f, 200f, 30f);
		CategoryViewButton.StyleInfo = SHSInheritedStyleInfo.Instance;
		CategoryViewButton.Value = false;
		Add(CategoryViewButton);
		ShowTitlesButton = new GUIToggleButton();
		ShowTitlesButton.Text = "Show Titles";
		ShowTitlesButton.Spacing = 30f;
		ShowTitlesButton.SetButtonSize(new Vector2(30f, 30f));
		ShowTitlesButton.SetPositionAndSize(140f, 20f, 200f, 30f);
		ShowTitlesButton.StyleInfo = SHSInheritedStyleInfo.Instance;
		ShowTitlesButton.Value = true;
		Add(ShowTitlesButton);
		categoryList = new GUIDropDownListBox();
		categoryList.SetPositionAndSize(260f, 20f, 180f, 30f);
		Add(categoryList);
		foreach (string key in AppShell.Instance.SmartTipsManager.CategorySmartTipDictionary.Keys)
		{
			categoryList.AddItem(key);
		}
		SimulateButton = new GUIButton();
		SimulateButton.Text = "Simulate";
		SimulateButton.SetPositionAndSize(470f, 20f, 60f, 30f);
		SimulateButton.StyleInfo = SHSInheritedStyleInfo.Instance;
		SimulateButton.Click += SimulateButton_Click;
		Add(SimulateButton);
	}

	private void SimulateButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		for (int i = 0; i < 1000; i++)
		{
			AppShell.Instance.SmartTipsManager.GetSmartTip(categoryList.SelectedText);
		}
	}

	public override void Show()
	{
		base.Show();
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		GUILayout.BeginArea(new Rect(30f, 60f, 900f, base.rect.height - 50f));
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		GUILayout.Label("SMART TIPS", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		SmartTipsManager smartTipsManager = AppShell.Instance.SmartTipsManager;
		if (smartTipsManager == null)
		{
			GUILayout.Label("--- No Smart Tips Manager to display. ---");
			GUILayout.EndScrollView();
			GUILayout.EndArea();
			return;
		}
		GUILayout.BeginVertical();
		GUILayout.Space(20f);
		if (ShowTitlesButton.Value)
		{
			GUILayout.BeginHorizontal(GUILayout.Width(700f));
			GUILayout.Label("TIP TITLES", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle, GUILayout.Width(350f));
			GUILayout.EndHorizontal();
			foreach (SmartTipTitle value in smartTipsManager.SmartTipTitles.Values)
			{
				GUILayout.Space(2f);
				GUILayout.BeginHorizontal(GUILayout.Width(900f));
				GUILayout.Label(value.Key.ToString(), headerStyle.UnityStyle, GUILayout.Width(120f));
				GUILayout.Label(AppShell.Instance.stringTable.GetString(value.Key), headerStyle.UnityStyle, GUILayout.Width(480f));
				GUILayout.Label(value.Icon.ToString(), headerStyle.UnityStyle, GUILayout.Width(300f));
				GUILayout.EndHorizontal();
				GUILayout.Space(1f);
			}
		}
		if (CategoryViewButton.Value)
		{
			foreach (string key in smartTipsManager.CategorySmartTipDictionary.Keys)
			{
				GUILayout.Space(12f);
				Color color = GUI.color;
				GUI.color = Color.cyan;
				GUILayout.BeginHorizontal(GUILayout.Width(700f));
				GUILayout.Label(key, GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle, GUILayout.Width(350f));
				GUILayout.EndHorizontal();
				GUI.color = color;
				foreach (SmartTip item in smartTipsManager.CategorySmartTipDictionary[key])
				{
					GUILayout.Space(5f);
					GUILayout.BeginHorizontal(GUILayout.Width(900f));
					GUILayout.Label(item.tipKey, headerStyle.UnityStyle, GUILayout.Width(150f));
					GUILayout.Label(item.Target.ToString(), headerStyle.UnityStyle, GUILayout.Width(100f));
					GUILayout.Label(item.Erosion.ToString(), headerStyle.UnityStyle, GUILayout.Width(50f));
					GUILayout.Label(AppShell.Instance.stringTable.GetString(item.tipKey), headerStyle.UnityStyle, GUILayout.Width(500f));
					SmartTipCategory[] smartTipCategories = item.SmartTipCategories;
					foreach (SmartTipCategory smartTipCategory in smartTipCategories)
					{
						if (smartTipCategory.Name == key)
						{
							GUILayout.Label(smartTipCategory.Weight.ToString(), headerStyle.UnityStyle, GUILayout.Width(100f));
							break;
						}
					}
					GUILayout.Label(item.titleKey.ToString(), headerStyle.UnityStyle, GUILayout.Width(150f));
					GUILayout.EndHorizontal();
					GUILayout.Space(1f);
				}
				GUILayout.Space(5f);
			}
		}
		else
		{
			List<SmartTip> smartTips = smartTipsManager.SmartTips;
			smartTips.Sort(delegate(SmartTip a, SmartTip b)
			{
				return a.tipKey.CompareTo(b.tipKey);
			});
			foreach (SmartTip item2 in smartTips)
			{
				GUILayout.Space(5f);
				GUILayout.BeginHorizontal(GUILayout.Width(900f));
				GUILayout.Label(item2.tipKey, headerStyle.UnityStyle, GUILayout.Width(150f));
				GUILayout.Label(AppShell.Instance.stringTable.GetString(item2.tipKey), headerStyle.UnityStyle, GUILayout.Width(400f));
				GUILayout.Label(item2.Target.ToString(), headerStyle.UnityStyle, GUILayout.Width(100f));
				GUILayout.Label(item2.UseCount.ToString(), headerStyle.UnityStyle, GUILayout.Width(50f));
				GUILayout.Label(item2.Erosion.ToString(), headerStyle.UnityStyle, GUILayout.Width(50f));
				GUILayout.Label(item2.titleKey.ToString(), headerStyle.UnityStyle, GUILayout.Width(150f));
				GUILayout.EndHorizontal();
				SmartTipCategory[] smartTipCategories2 = item2.SmartTipCategories;
				foreach (SmartTipCategory smartTipCategory2 in smartTipCategories2)
				{
					if (smartTipCategory2.Weight != 0f)
					{
						GUILayout.BeginHorizontal(GUILayout.Width(900f));
						GUILayout.Space(50f);
						GUILayout.Label(smartTipCategory2.Name.ToString(), headerStyle.UnityStyle, GUILayout.Width(100f));
						GUILayout.Label(smartTipCategory2.Weight.ToString(), headerStyle.UnityStyle, GUILayout.Width(100f));
						GUILayout.EndHorizontal();
						GUILayout.Space(1f);
					}
				}
			}
			GUILayout.Space(5f);
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
