using System;
using System.Collections.Generic;
using UnityEngine;

public class GUITabbedDialogWindow : GUIControlWindow
{
	public enum TabbedDialogOrientationEnum
	{
		Horizontal,
		Vertical
	}

	public class GUITabbedWindow : GUIControlWindow
	{
		private string windowName;

		protected GUISlider slider;

		public string WindowName
		{
			get
			{
				return windowName;
			}
			set
			{
				windowName = value;
			}
		}

		public GUISlider Slider
		{
			get
			{
				return slider;
			}
			set
			{
				slider = value;
			}
		}

		public GUITabbedWindow(string WindowName, GUISlider Slider)
		{
			windowName = WindowName;
			slider = Slider;
			SetPositionAndSize(QuickSizingHint.ParentSize);
		}

		public override void Draw(DrawModeSetting drawFlags)
		{
			base.Draw(drawFlags);
		}
	}

	public class GUITabbedButton : GUIButton
	{
		private string tabName;

		private GUILabel label;

		private SHSStyle labelStyle;

		private GUITabbedWindow window;

		private string context;

		private new string text;

		public string TabName
		{
			get
			{
				return tabName;
			}
			set
			{
				tabName = value;
			}
		}

		public GUITabbedWindow Window
		{
			get
			{
				return window;
			}
			set
			{
				window = value;
			}
		}

		public string Context
		{
			get
			{
				return context;
			}
			set
			{
				context = value;
			}
		}

		public new string Text
		{
			get
			{
				return text;
			}
		}

		public GUITabbedButton(string TabName)
			: this(TabName, string.Empty)
		{
		}

		public GUITabbedButton(string TabName, string Context)
		{
			tabName = TabName;
			context = Context;
			text = tabName;
			label = new GUILabel();
		}

		public override void Draw(DrawModeSetting drawFlags)
		{
			if (labelStyle == null)
			{
				labelStyle = new SHSStyle(Style);
				labelStyle.UnityStyle.normal.background = null;
				labelStyle.UnityStyle.active.background = null;
				label.Style = labelStyle;
			}
			base.Draw(drawFlags);
			if (((GUITabbedDialogWindow)Parent.Parent).Orientation == TabbedDialogOrientationEnum.Horizontal)
			{
				label.Rect = base.rect;
				label.Text = Text;
				label.Draw(drawFlags);
				return;
			}
			Matrix4x4 matrix = GUI.matrix;
			GUI.matrix = Matrix4x4.identity;
			GUIUtility.RotateAroundPivot(-90f, centerPoint);
			GUI.matrix = matrix * GUI.matrix;
			label.Rect = base.rect;
			label.Text = Text;
			label.Draw(drawFlags);
			GUI.matrix = matrix;
		}
	}

	public delegate void TabSelectedDelegate(GUIControl control, GUIClickEvent eventArgs);

	private GUIControlWindow contentWindow;

	private GUIControlWindow tabWindow;

	private List<GUITabbedButton> tabList;

	private List<KeyValuePair<string, GUITabbedWindow>> windowList;

	private TabbedDialogOrientationEnum orientation;

	private SHSStyle tabButtonNormalStyle;

	private SHSStyle tabButtonSelectedStyle;

	private int tabSecondarySizeNormal = 18;

	private int tabSecondarySizeSelected = 28;

	private int tabSizeNormal = 75;

	private int tabSizeSelected = 81;

	private int tabSpacing = 7;

	private int initialIndentSpacing = 10;

	private GUITabbedButton selectedTab;

	private GUIDrawTexture containerBgd;

	private string backgroundTextureSource = "debug_bundle|debug_tab_background_f01";

	private string currentContext;

	private int numberOfTabLines = 2;

	private GUIButton scrollRight;

	private GUIButton scrollLeft;

	private float tabOffset;

	private float maxTabOffset;

	private float totalTabWindowSize;

	private bool mouseOverLeft;

	private bool mouseOverRight;

	private float scrollSpeed = 1f;

	public List<KeyValuePair<string, GUITabbedWindow>> WindowList
	{
		get
		{
			return windowList;
		}
		set
		{
			windowList = value;
		}
	}

	public override List<IGUIControl> ControlList
	{
		get
		{
			return base.ControlList;
		}
	}

	public TabbedDialogOrientationEnum Orientation
	{
		get
		{
			return orientation;
		}
		set
		{
			orientation = value;
		}
	}

	public SHSStyle TabButtonNormalStyle
	{
		get
		{
			return tabButtonNormalStyle;
		}
		set
		{
			tabButtonNormalStyle = value;
		}
	}

	public SHSStyle TabButtonSelectedStyle
	{
		get
		{
			return tabButtonSelectedStyle;
		}
		set
		{
			tabButtonSelectedStyle = value;
		}
	}

	public int TabSecondarySizeNormal
	{
		get
		{
			return tabSecondarySizeNormal;
		}
		set
		{
			tabSecondarySizeNormal = value;
		}
	}

	public int TabSecondarySizeSelected
	{
		get
		{
			return tabSecondarySizeSelected;
		}
		set
		{
			tabSecondarySizeSelected = value;
		}
	}

	public int TabSizeNormal
	{
		get
		{
			return tabSizeNormal;
		}
		set
		{
			tabSizeNormal = value;
		}
	}

	public int TabSizeSelected
	{
		get
		{
			return tabSizeSelected;
		}
		set
		{
			tabSizeSelected = value;
		}
	}

	public int TabSpacing
	{
		get
		{
			return tabSpacing;
		}
		set
		{
			tabSpacing = value;
		}
	}

	public int InitialIndentSpacing
	{
		get
		{
			return initialIndentSpacing;
		}
		set
		{
			initialIndentSpacing = value;
		}
	}

	public GUITabbedButton SelectedTab
	{
		get
		{
			return selectedTab;
		}
		set
		{
			selectedTab = value;
		}
	}

	public string BackgroundTextureSource
	{
		get
		{
			return backgroundTextureSource;
		}
		set
		{
			backgroundTextureSource = value;
			if (containerBgd != null)
			{
				if (value != null && value != string.Empty)
				{
					containerBgd.TextureSource = backgroundTextureSource;
				}
				else
				{
					containerBgd.Texture = null;
				}
			}
		}
	}

	public string CurrentContext
	{
		get
		{
			return currentContext;
		}
	}

	public event TabSelectedDelegate TabSelected;

	public GUITabbedDialogWindow(TabbedDialogOrientationEnum Orientation)
	{
		orientation = Orientation;
		contentWindow = new GUIControlWindow();
		contentWindow.Id = "Tabbed Content Window";
		tabWindow = new GUIControlWindow();
		tabWindow.Id = "Tab Window";
		contentWindow.HitTestType = HitTestTypeEnum.Rect;
		tabWindow.HitTestType = HitTestTypeEnum.Rect;
		tabList = new List<GUITabbedButton>();
		windowList = new List<KeyValuePair<string, GUITabbedWindow>>();
		Add(tabWindow);
		Add(contentWindow);
		containerBgd = new GUIDrawTexture();
		containerBgd.SetPositionAndSize(QuickSizingHint.ParentSize);
		if (backgroundTextureSource != null && backgroundTextureSource != string.Empty)
		{
			containerBgd.TextureSource = backgroundTextureSource;
		}
		contentWindow.Add(containerBgd);
		scrollRight = new GUIButton();
		scrollRight.Text = ((Orientation != 0) ? "\\/" : ">>");
		scrollRight.MouseOver += delegate
		{
			mouseOverRight = true;
		};
		scrollRight.MouseOut += delegate
		{
			mouseOverRight = false;
		};
		Add(scrollRight);
		scrollLeft = new GUIButton();
		scrollLeft.Text = ((Orientation != 0) ? "/\\" : "<<");
		scrollLeft.MouseOver += delegate
		{
			mouseOverLeft = true;
		};
		scrollLeft.MouseOut += delegate
		{
			mouseOverLeft = false;
		};
		Add(scrollLeft);
		Configure();
	}

	public void setNumberOfTabLines(int tabLines)
	{
		numberOfTabLines = tabLines;
	}

	public void setScrollSpeed(float newScrollSpeed)
	{
		scrollSpeed = newScrollSpeed;
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		bool flag = false;
		if (mouseOverLeft && tabOffset > -10f)
		{
			tabOffset -= scrollSpeed;
			flag = true;
		}
		if (mouseOverRight && tabOffset < maxTabOffset)
		{
			tabOffset += scrollSpeed;
			flag = true;
		}
		if (flag)
		{
			Configure();
		}
		base.Draw(drawFlags);
	}

	public void setUpButtonStyleInfo(SHSStyleInfo styleInfo)
	{
		setUpLeftButtomStyleInfo(styleInfo);
	}

	public void setDownButtonStyleInfo(SHSStyleInfo styleInfo)
	{
		setDownRightButtomStyleInfo(styleInfo);
	}

	public void setLeftButtonStyleInfo(SHSStyleInfo styleInfo)
	{
		setUpLeftButtomStyleInfo(styleInfo);
	}

	public void setRightButtonStyleInfo(SHSStyleInfo styleInfo)
	{
		setDownRightButtomStyleInfo(styleInfo);
	}

	private void setUpLeftButtomStyleInfo(SHSStyleInfo styleInfo)
	{
		scrollLeft.StyleInfo = styleInfo;
	}

	private void setDownRightButtomStyleInfo(SHSStyleInfo styleInfo)
	{
		scrollRight.StyleInfo = styleInfo;
	}

	private void Configure()
	{
		GUIControlWindow gUIControlWindow = contentWindow;
		Vector2 zero = Vector2.zero;
		float x;
		if (Orientation == TabbedDialogOrientationEnum.Horizontal)
		{
			Vector2 rectSize = base.RectSize;
			x = rectSize.x;
		}
		else
		{
			Vector2 rectSize2 = base.RectSize;
			x = rectSize2.x - (float)(tabSecondarySizeSelected * numberOfTabLines);
		}
		float y;
		if (Orientation == TabbedDialogOrientationEnum.Horizontal)
		{
			Vector2 rectSize3 = base.RectSize;
			y = rectSize3.y - (float)(tabSecondarySizeSelected * numberOfTabLines);
		}
		else
		{
			Vector2 rectSize4 = base.RectSize;
			y = rectSize4.y;
		}
		gUIControlWindow.SetPositionAndSize(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, zero, new Vector2(x, y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		if (Orientation == TabbedDialogOrientationEnum.Horizontal)
		{
			tabWindow.SetPosition(0f - tabOffset, 0f);
			tabWindow.SetSize(totalTabWindowSize, tabSecondarySizeSelected * numberOfTabLines);
		}
		else
		{
			tabWindow.SetPosition(0f, 0f - tabOffset);
			tabWindow.SetSize(tabSecondarySizeSelected * numberOfTabLines, totalTabWindowSize);
		}
		int num = initialIndentSpacing + tabSizeSelected / 2;
		int num2;
		try
		{
			num2 = Convert.ToInt32(totalTabWindowSize / (float)(initialIndentSpacing + tabSizeSelected));
		}
		catch (Exception)
		{
			num2 = 0;
		}
		if (num2 == 0)
		{
			num2 = 1;
		}
		int num3 = 0;
		int num4 = 0;
		foreach (GUITabbedButton tab in tabList)
		{
			num4++;
			if (num4 > num2)
			{
				num3++;
				num4 = 1;
				num = initialIndentSpacing + tabSizeSelected / 2;
			}
			tab.SetPositionAndSize((Orientation != 0) ? DockingAlignmentEnum.TopRight : DockingAlignmentEnum.BottomLeft, (Orientation != 0) ? AnchorAlignmentEnum.MiddleRight : AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2((Orientation != 0) ? (-tabSecondarySizeNormal * num3) : num, (Orientation != 0) ? num : (-tabSecondarySizeNormal * num3)), (!tab.IsSelected) ? new Vector2((Orientation != 0) ? tabSecondarySizeNormal : tabSizeNormal, (Orientation != 0) ? tabSizeNormal : tabSecondarySizeNormal) : new Vector2((Orientation != 0) ? tabSecondarySizeSelected : tabSizeSelected, (Orientation != 0) ? tabSizeSelected : tabSecondarySizeSelected), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			tab.Style = ((!tab.IsSelected) ? TabButtonNormalStyle : TabButtonSelectedStyle);
			num += tabSizeNormal + tabSpacing;
		}
		maxTabOffset = (float)num - ((Orientation != 0) ? Rect.yMax : Rect.xMax);
		totalTabWindowSize = Rect.xMax;
		if ((float)num > ((Orientation != 0) ? Rect.yMax : Rect.xMax))
		{
			if (Orientation == TabbedDialogOrientationEnum.Horizontal)
			{
				scrollRight.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
			}
			else
			{
				scrollRight.SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft);
			}
			scrollRight.SetSize(30f, 30f);
			scrollRight.Show();
			scrollLeft.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
			scrollLeft.SetSize(30f, 30f);
			scrollLeft.Show();
		}
		else
		{
			scrollRight.Hide();
			scrollLeft.Hide();
		}
	}

	public void Add(GUITabbedWindow Window, string WindowName)
	{
		Add(Window, WindowName, null);
	}

	public void Add(GUITabbedWindow Window, string WindowName, string tabName)
	{
		GUITabbedButton gUITabbedButton = null;
		if (windowList.Contains(new KeyValuePair<string, GUITabbedWindow>(WindowName, Window)))
		{
			CspUtils.DebugLog("Tabbed Dialog already contains a window " + Window + " with a name of " + WindowName);
			return;
		}
		if (tabName != null)
		{
			gUITabbedButton = tabList.Find(delegate(GUITabbedButton b)
			{
				return b.TabName == tabName;
			});
			if (gUITabbedButton == null)
			{
				CspUtils.DebugLog("Can't find button " + tabName);
				return;
			}
		}
		windowList.Add(new KeyValuePair<string, GUITabbedWindow>(WindowName, Window));
		contentWindow.Add(Window);
		if (tabName != null && gUITabbedButton != null)
		{
			gUITabbedButton.Window = Window;
		}
	}

	public void AddTab(string TabName)
	{
		AddTab(TabName, string.Empty, null);
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		Configure();
	}

	public void AddTab(string TabName, string Context, GUITabbedWindow Window)
	{
		if (tabList.Find(delegate(GUITabbedButton findbutton)
		{
			return findbutton.TabName == TabName;
		}) != null)
		{
			CspUtils.DebugLog("Tab " + TabName + " already exists.");
			return;
		}
		GUITabbedButton gUITabbedButton = new GUITabbedButton(TabName, Context);
		gUITabbedButton.Click += OnTabSelected;
		gUITabbedButton.Window = Window;
		tabList.Add(gUITabbedButton);
		tabWindow.Add(gUITabbedButton);
		if (Window != null)
		{
			Window.SetPositionAndSize(QuickSizingHint.ParentSize);
		}
		Configure();
	}

	public void SelectTab(int TabIndex)
	{
		if (TabIndex >= 0 && TabIndex < tabList.Count)
		{
			OnTabSelected(tabList[TabIndex], null);
		}
	}

	private void OnTabSelected(GUIControl sender, GUIClickEvent data)
	{
		if (SelectedTab == (GUITabbedButton)sender)
		{
			return;
		}
		if (SelectedTab != null)
		{
			SelectedTab.IsSelected = false;
			SelectedTab.Style = TabButtonNormalStyle;
			if (SelectedTab.Window != null)
			{
				contentWindow.Remove(SelectedTab.Window);
			}
		}
		SelectedTab = (GUITabbedButton)sender;
		SelectedTab.IsSelected = true;
		SelectedTab.Style = TabButtonSelectedStyle;
		currentContext = selectedTab.Context;
		if (SelectedTab != null && SelectedTab.Window != null)
		{
			SelectedTab.Window.IsVisible = true;
			contentWindow.Add(SelectedTab.Window);
		}
		Configure();
		if (this.TabSelected != null)
		{
			this.TabSelected(sender, data);
		}
	}
}
