using System.Collections.Generic;
using UnityEngine;

public class GUITBListBox : GUIControlWindow, GUITBInterface
{
	public class GUITextButton : GUIListItem
	{
		private GUITBListBox orgin;

		private GUIButton button;

		private GUILabel label;

		public string Text
		{
			get
			{
				return label.Text;
			}
		}

		public GUITextButton(string text, GUITBListBox orgin)
		{
			this.orgin = orgin;
			float width = orgin.ListWindow.ListItemWidth;
			float height = orgin.ListWindow.ListItemHeight;
			GUIImage gUIImage = new GUIImage();
			gUIImage.SetPositionAndSize(0f, 0f, width, height);
			gUIImage.TextureSource = "toolbox_bundle|contentField_218x585";
			button = new GUIButton();
			button.SetSize(width, height);
			button.SetPosition(0f, 0f);
			orgin.buttonList.Add(button);
			Add(button);
			label = new GUILabel();
			label.Rect = button.Rect;
			label.Text = text;
			label.SetSize(width, height);
			label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(0, 0, 0), TextAnchor.MiddleCenter);
			label.SetPosition(0f, 0f);
			label.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
			Add(label);
			Click += delegate
			{
				clicked(orgin, button, text);
			};
		}

		private void clicked(GUITBListBox orgin, GUIButton button, string text)
		{
			orgin.unselectAll();
			button.IsSelected = true;
			orgin.selectedItem = this;
			orgin.selectedItemText = text;
		}

		public void resize()
		{
			float width = orgin.ListWindow.ListItemWidth;
			float height = orgin.ListWindow.ListItemHeight;
			button.SetSize(width, height);
			label.SetSize(width, height);
		}
	}

	public GUIListViewWindow<GUITextButton> ListWindow;

	private GUISlider scroller;

	private GUIListItem selectedItem;

	private string selectedItemText = string.Empty;

	private List<GUIButton> buttonList = new List<GUIButton>();

	private List<GUITextButton> itemList = new List<GUITextButton>();

	public GUIListContainerWindow<GUITextButton>.SelectionChangedDelegate SelectionChanged
	{
		set
		{
			ListWindow.SelectionChanged += value;
		}
	}

	public GUIListItem SelectedItem
	{
		get
		{
			return selectedItem;
		}
	}

	public string SelectedItemText
	{
		get
		{
			return selectedItemText;
		}
	}

	public List<GUITextButton> ItemList
	{
		get
		{
			return itemList;
		}
	}

	public GUITBListBox()
	{
		scroller = new GUISlider();
		scroller.Orientation = GUISlider.SliderOrientationEnum.Vertical;
		scroller.SetPosition(30f, 0f);
		scroller.IsVisible = true;
		scroller.IsEnabled = true;
		scroller.Value = 0f;
		scroller.Min = 0f;
		scroller.Max = 100f;
		scroller.StyleInfo = SHSInheritedStyleInfo.Instance;
		scroller.UseMouseWheelScroll = true;
		Add(scroller);
		ListWindow = new GUIListViewWindow<GUITextButton>();
		ListWindow.SetPosition(100f, 0f);
		ListWindow.Slider = scroller;
		ListWindow.Orientation = GUIListViewWindow<GUITextButton>.ListViewOrientationEnum.Vertical;
		ListWindow.ListItemHeight = 30;
		ListWindow.Padding = new Rect(3f, 3f, 3f, 3f);
		ListWindow.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Inherit;
		Add(ListWindow);
	}

	public void unselectAll()
	{
		foreach (GUIButton button in buttonList)
		{
			button.IsSelected = false;
		}
	}

	public void AddItem(string itemName, object data)
	{
		GUITextButton gUITextButton = new GUITextButton(itemName, this);
		gUITextButton.Data = data;
		ListWindow.AddItem(gUITextButton);
		itemList.Add(gUITextButton);
	}

	public void AddItem(string itemName)
	{
		AddItem(itemName, null);
	}

	public override void Clear()
	{
		itemList.Clear();
		ListWindow.Clear();
	}

	public new void AutoSize(float x, float y)
	{
		SetSize(x, y);
		SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
		scroller.SetPositionAndSize(30f, 0f, 50f, y);
		ListWindow.SetSize(x - 100f, y);
		ListWindow.ListItemWidth = (int)x - 150;
		foreach (GUITextButton item in itemList)
		{
			item.resize();
		}
	}
}
