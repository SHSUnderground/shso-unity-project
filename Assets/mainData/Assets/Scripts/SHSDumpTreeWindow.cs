using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SHSDumpTreeWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private GUITreeView treeView;

	private GUIControlWindow DumpInfo;

	private GUIControlWindow DumpTextWindow;

	private GUILabel DumpText;

	private GUIButton refreshButton;

	private GUIScrollBar scroller;

	private GUIScrollBar horzScroller;

	private GUIControlWindow TextLimitingWindow;

	private float treeToTextRatio = 0.35f;

	private float refreshButtonSize = 26f;

	public SHSDumpTreeWindow(string WindowName)
		: base(WindowName, null)
	{
		SetBackground(new Color(1f, 0.4f, 1f, 0.2f));
		DumpInfo = new GUIControlWindow();
		DumpInfo.SetSize(new Vector2(1f - treeToTextRatio, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		DumpInfo.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
		Add(DumpInfo);
		DumpTextWindow = new GUIControlWindow();
		DumpTextWindow.SetSize(new Vector2(1f, 0.9f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		DumpTextWindow.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle);
		DumpInfo.Add(DumpTextWindow);
		TextLimitingWindow = new GUIControlWindow();
		TextLimitingWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		DumpTextWindow.Add(TextLimitingWindow);
		DumpText = new GUILabel();
		DumpText.SetSize(0f, 0f);
		DumpText.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		DumpText.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, GUILabel.GenColor(255, 255, 255), TextAnchor.MiddleLeft);
		TextLimitingWindow.Add(DumpText);
		scroller = new GUIScrollBar();
		scroller.SetSize(new Vector2(50f, 0.8f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Percentage);
		scroller.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		DumpTextWindow.Add(scroller);
		scroller.Changed += delegate
		{
			GUILabel dumpText2 = DumpText;
			Vector2 position2 = DumpText.Position;
			dumpText2.SetPosition(position2.x, 0f - scroller.Value);
		};
		horzScroller = new GUIScrollBar();
		horzScroller.Orientation = GUIScrollBar.SliderOrientationEnum.Horizontal;
		horzScroller.SetSize(new Vector2(0.8f, 50f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Absolute);
		horzScroller.SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft);
		DumpTextWindow.Add(horzScroller);
		horzScroller.Changed += delegate
		{
			GUILabel dumpText = DumpText;
			float x = 0f - horzScroller.Value;
			Vector2 position = DumpText.Position;
			dumpText.SetPosition(x, position.y);
		};
		refreshButton = new GUIButton();
		refreshButton.SetSize(refreshButtonSize, refreshButtonSize);
		refreshButton.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		refreshButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|mshs_costume_select_scrubber");
		DumpInfo.Add(refreshButton);
		refreshButton.Click += delegate
		{
			if (treeView != null)
			{
				Remove(treeView);
			}
			setupDumpTreeView();
			DumpText.Text = string.Empty;
			treeView.UpdateDisplay();
			treeView.IsVisible = true;
			treeView.SetSize(new Vector2(treeToTextRatio, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
			modifyTextSize();
			treeView.HandleResize(null);
		};
		GUILabel gUILabel = new GUILabel();
		gUILabel.SetSize(100f, refreshButtonSize);
		gUILabel.SetPosition(refreshButtonSize, 0f);
		gUILabel.Text = "<-- Refresh";
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.MiddleLeft);
		DumpInfo.Add(gUILabel);
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		if (TextLimitingWindow != null && DumpTextWindow != null)
		{
			DumpTextWindow.SetSize(DumpInfo.Rect.width, DumpInfo.Rect.height - refreshButtonSize);
			TextLimitingWindow.SetSize(DumpTextWindow.Rect.width - 50f, DumpTextWindow.Rect.height - 50f);
		}
		modifyTextSize();
	}

	private void modifyTextSize()
	{
		if (DumpText != null && DumpText.Style != null)
		{
			DumpText.SetSize(DumpText.Style.UnityStyle.CalcSize(DumpText.Content));
			scroller.Max = DumpText.Rect.height - TextLimitingWindow.Rect.height;
			if (scroller.Max <= 0f)
			{
				scroller.IsVisible = false;
			}
			else
			{
				scroller.IsVisible = true;
				scroller.AutoDragBarResize(TextLimitingWindow.Rect.height);
			}
			horzScroller.Max = DumpText.Rect.width - TextLimitingWindow.Rect.width;
			if (horzScroller.Max <= 0f)
			{
				horzScroller.IsVisible = false;
				return;
			}
			horzScroller.IsVisible = true;
			horzScroller.AutoDragBarResize(TextLimitingWindow.Rect.width);
		}
	}

	public override void OnShow()
	{
		if (treeView != null)
		{
			treeView.UpdateDisplay();
		}
		base.OnShow();
		modifyTextSize();
	}

	public override void OnUpdate()
	{
		Vector2 size = TextLimitingWindow.Size;
		if (size.x < 0f || treeView == null)
		{
			HandleResize(null);
			if (treeView != null)
			{
				Remove(treeView);
			}
			setupDumpTreeView();
			treeView.UpdateDisplay();
			modifyTextSize();
			treeView.HandleResize(null);
		}
		base.OnUpdate();
	}

	private string getFullPath(GUITreeChangedEvent eventData)
	{
		if (eventData.Path == string.Empty)
		{
			return eventData.ChangedName;
		}
		return eventData.Path + "/" + eventData.ChangedName;
	}

	private void setupDumpTreeView()
	{
		treeView = new GUITreeView();
		treeView.SetSize(Rect.width * treeToTextRatio, Rect.height);
		treeView.SetSize(new Vector2(treeToTextRatio, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		treeView.SetPosition(0f, 0f);
		Add(treeView);
		List<string> list = new List<string>();
		Object[] topLevelObjects = DebugUtil.GetTopLevelObjects();
		Object[] array = topLevelObjects;
		foreach (Object @object in array)
		{
			list.Add(@object.name);
		}
		processAndAddStrings(list, string.Empty);
		treeView.OnTabOpened += delegate(GUIControl sender, GUITreeChangedEvent eventData)
		{
			string path = eventData.Path;
			object ResultingValue = null;
			string Error;
			DebugUtil.GetObjectFromObjPathString(null, path, out Error, out ResultingValue);
			if (ResultingValue is GameObject)
			{
				GameObject gameObject = (GameObject)ResultingValue;
				List<string> list2 = new List<string>();
				for (int j = 0; j < gameObject.transform.GetChildCount(); j++)
				{
					list2.Add(gameObject.transform.GetChild(j).name);
				}
				processAndAddStrings(list2, path);
				list2.Clear();
				Component[] components = gameObject.GetComponents(typeof(Component));
				Component[] array2 = components;
				foreach (Component component in array2)
				{
					if (component != null)
					{
						list2.Add(component.GetType().ToString());
					}
				}
				processAndAddStrings(list2, path);
			}
		};
		treeView.OnSelected += delegate(GUIControl sender, GUITreeChangedEvent eventData)
		{
			DumpText.Text = processDumpText(DebugUtil.Dump(eventData.Path, true));
			scroller.Value = scroller.Min;
			horzScroller.Value = horzScroller.Min;
			modifyTextSize();
		};
		treeView.IsVisible = true;
	}

	private string processDumpText(string tp)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string[] array = tp.Split('\n');
		string[] array2 = array;
		foreach (string text in array2)
		{
			stringBuilder.Append(text.Trim() + "\n");
		}
		return stringBuilder.ToString();
	}

	private void processAndAddStrings(List<string> tp, string path)
	{
		for (int i = 0; i < tp.Count; i++)
		{
			List<string> list = tp.FindAll(delegate(string toTest)
			{
				return toTest.CompareTo(tp[i]) == 0;
			});
			if (list.Count > 1)
			{
				for (int j = 0; j < list.Count; j++)
				{
					int index = tp.IndexOf(list[0]);
					tp[index] = list[j] + "[" + j + "]";
				}
			}
		}
		foreach (string item in tp)
		{
			if (path == string.Empty)
			{
				treeView.AddItem(item);
			}
			else
			{
				treeView.AddItem(path + "/" + item);
			}
		}
	}
}
