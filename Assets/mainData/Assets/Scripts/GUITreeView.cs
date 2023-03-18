using System.Collections.Generic;
using UnityEngine;

public class GUITreeView : GUIControlWindow
{
	private class TreeNode
	{
		public GUILabel label;

		public GUIButton button;

		public GUIHotSpotButton clickToHighlight;

		public List<TreeNode> childrenNodes;

		public TreeNode parentNode;

		public GUITreeView window;

		public int generation;

		public bool open;

		public TreeNode(GUITreeView window, string name, TreeNode parentNode, int gen)
		{
			generation = gen;
			this.window = window;
			childrenNodes = new List<TreeNode>();
			this.parentNode = parentNode;
			label = new GUILabel();
			label.Text = name;
			window.sWindow.Add(label);
			label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.MiddleLeft);
			button = new GUIButton();
			button.StyleInfo = new SHSButtonStyleInfo("toolbox_bundle|TreeViewButton");
			window.sWindow.Add(button);
			button.Click += toggleOpen;
			clickToHighlight = new GUIHotSpotButton();
			window.sWindow.Add(clickToHighlight);
			clickToHighlight.Click += highlightMe;
			clickToHighlight.MouseOver += clickToHighlight_MouseOver;
			clickToHighlight.MouseOut += clickToHighlight_MouseOut;
			button.MouseOver += clickToHighlight_MouseOver;
			button.MouseOut += clickToHighlight_MouseOut;
			open = false;
		}

		private void clickToHighlight_MouseOut(GUIControl sender, GUIMouseEvent EventData)
		{
			label.Color = window.style.UnityStyle.normal.textColor;
		}

		private void clickToHighlight_MouseOver(GUIControl sender, GUIMouseEvent EventData)
		{
			label.Color = window.style.UnityStyle.hover.textColor;
		}

		private void highlightMe(GUIControl sender, GUIClickEvent EventData)
		{
			if (window.itemSelected != this)
			{
				if (window.itemSelected != null && window.OnUnselected != null)
				{
					window.OnUnselected(label, new GUITreeChangedEvent(window.itemSelected.label.Text, window.itemSelected.GetTreePath()));
				}
				window.itemSelected = this;
				if (window.OnSelected != null)
				{
					window.OnSelected(label, new GUITreeChangedEvent(label.Text, GetTreePath()));
				}
				ShowBackground();
			}
		}

		public void ShowBackground()
		{
			window.highlightBackground.SetSize(label.Size);
			window.highlightBackground.SetPosition(label.Position);
		}

		public void HideBackground()
		{
			window.highlightBackground.SetSize(Vector2.zero);
		}

		private void toggleOpen(GUIControl sender, GUIClickEvent EventData)
		{
			open = !open;
			button.IsSelected = open;
			if (open)
			{
				if (window.OnTabOpened != null)
				{
					window.OnTabOpened(label, new GUITreeChangedEvent(label.Text, GetTreePath()));
				}
				int location = window.displayList.LastIndexOf(this);
				foreach (TreeNode childrenNode in childrenNodes)
				{
					location = childrenNode.recursiveShow(location);
				}
			}
			else
			{
				if (window.OnTabClosed != null)
				{
					window.OnTabClosed(label, new GUITreeChangedEvent(label.Text, GetTreePath()));
				}
				int num = window.displayList.LastIndexOf(this);
				foreach (TreeNode childrenNode2 in childrenNodes)
				{
					childrenNode2.recursiveHide(num + 1);
				}
			}
			window.UpdateDisplay();
		}

		public int recursiveShow(int location)
		{
			ShowNode();
			window.displayList.Insert(location + 1, this);
			location++;
			if (open)
			{
				foreach (TreeNode childrenNode in childrenNodes)
				{
					location = childrenNode.recursiveShow(location);
				}
				return location;
			}
			return location;
		}

		public void recursiveHide(int location)
		{
			HideNode();
			window.displayList.RemoveAt(location);
			if (window.itemSelected == this)
			{
				HideBackground();
			}
			if (open)
			{
				foreach (TreeNode childrenNode in childrenNodes)
				{
					childrenNode.recursiveHide(location);
				}
			}
		}

		public void recursiveResize()
		{
			ShowNode();
			if (open)
			{
				foreach (TreeNode childrenNode in childrenNodes)
				{
					childrenNode.recursiveResize();
				}
			}
		}

		public string GetTreePath()
		{
			if (parentNode == null)
			{
				return label.Text;
			}
			return parentNode.GetTreePath() + "/" + label.Text;
		}

		public void positionNode(float i, float j)
		{
			button.SetPosition(0f + i, 0f + j);
			label.SetPosition(window.itemHeight + i, 0f + j);
			clickToHighlight.SetPosition(window.itemHeight + i, 0f + j);
		}

		public void ShowNode()
		{
			button.IsVisible = true;
			label.IsVisible = true;
			clickToHighlight.IsVisible = true;
			button.SetSize(window.itemHeight, window.itemHeight);
			label.SetSize(window.sWindow.Rect.width, window.itemHeight);
			clickToHighlight.SetSize(window.sWindow.Rect.width, window.itemHeight);
		}

		public void HideNode()
		{
			button.IsVisible = false;
			label.IsVisible = false;
			clickToHighlight.IsVisible = false;
		}
	}

	public delegate void OnSelectedEventDelegate(GUIControl sender, GUITreeChangedEvent eventData);

	public delegate void OnUnselectedEventDelegate(GUIControl sender, GUITreeChangedEvent eventData);

	public delegate void OnTabOpenedEventDelegate(GUIControl sender, GUITreeChangedEvent eventData);

	public delegate void OnTabClosedEventDelegate(GUIControl sender, GUITreeChangedEvent eventData);

	private GUIControlWindow sWindow;

	private GUIScrollBar scroller;

	private float itemHeight;

	private List<TreeNode> root;

	private TreeNode itemSelected;

	private GUIImage highlightBackground;

	private List<TreeNode> displayList;

	public float ItemHeight
	{
		get
		{
			return itemHeight;
		}
		set
		{
			itemHeight = value;
		}
	}

	public string ItemSelected
	{
		get
		{
			return itemSelected.label.Text;
		}
	}

	public event OnSelectedEventDelegate OnSelected;

	public event OnUnselectedEventDelegate OnUnselected;

	public event OnTabOpenedEventDelegate OnTabOpened;

	public event OnTabClosedEventDelegate OnTabClosed;

	public GUITreeView()
	{
		displayList = new List<TreeNode>();
		base.StyleName = "TreeViewStyle";
		itemHeight = 18f;
		sWindow = new GUIControlWindow();
		sWindow.SetPosition(0f, 0f);
		scroller = new GUIScrollBar();
		scroller.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
		scroller.Changed += scroller_Changed;
		highlightBackground = new GUIImage();
		highlightBackground.SetPosition(0f, 0f);
		highlightBackground.SetSize(Vector2.zero);
		highlightBackground.TextureSource = "common_bundle|white2x2";
		highlightBackground.Color = Color.black;
		sWindow.Add(highlightBackground);
		root = new List<TreeNode>();
		Add(sWindow);
		Add(scroller);
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		sWindow.SetSize(Rect.width - 50f, sWindow.Rect.height);
		scroller.SetSize(50f, Rect.height);
		scroller.Max = sWindow.Rect.height - Rect.height;
		if (scroller.Max <= 0f)
		{
			scroller.IsVisible = false;
		}
		else
		{
			scroller.IsVisible = true;
			scroller.AutoDragBarResize(Rect.height);
		}
		foreach (TreeNode item in root)
		{
			item.recursiveResize();
		}
	}

	public void AddItem(string path)
	{
		string[] array = path.Split('/');
		TreeNode treeNode = null;
		TreeNode treeNode2 = null;
		for (int i = 0; i < array.Length; i++)
		{
			List<TreeNode> list = (treeNode2 != null) ? treeNode2.childrenNodes : root;
			treeNode = getTreeNodeFromName(list, array[i]);
			if (treeNode == null)
			{
				treeNode = new TreeNode(this, array[i], treeNode2, i);
				list.Add(treeNode);
				if (treeNode2 == null)
				{
					displayList.Add(treeNode);
					treeNode.ShowNode();
					UpdateDisplay();
				}
			}
			treeNode2 = treeNode;
		}
	}

	private TreeNode getTreeNodeFromName(List<TreeNode> place, string name)
	{
		foreach (TreeNode item in place)
		{
			if (item.label.Text == name)
			{
				return item;
			}
		}
		return null;
	}

	private void scroller_Changed(GUIControl sender, GUIChangedEvent eventData)
	{
		sWindow.SetPosition(0f, 0f - scroller.Value);
	}

	public void UpdateDisplay()
	{
		for (int i = 0; i < displayList.Count; i++)
		{
			displayList[i].positionNode(displayList[i].generation * 10, (float)i * itemHeight);
			displayList[i].ShowNode();
		}
		sWindow.SetSize(sWindow.Rect.width, (float)displayList.Count * itemHeight);
		scroller.Max = sWindow.Rect.height - Rect.height;
		if (scroller.Max <= 0f)
		{
			scroller.IsVisible = false;
		}
		else
		{
			scroller.IsVisible = true;
			scroller.AutoDragBarResize(Rect.height);
		}
		if (itemSelected != null)
		{
			itemSelected.ShowBackground();
		}
	}
}
