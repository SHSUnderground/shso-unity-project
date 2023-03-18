using System.Collections.Generic;
using UnityEngine;

public class GUIDropDownListBox : GUIControlWindow, ICaptureHandler, ICaptureManager
{
	public class GUITextButton : GUIListItem
	{
		private GUIDropDownListBox main;

		public GUILabel label;

		private GUIImage background;

		public GUITextButton(GUIDropDownListBox main, string name)
		{
			this.main = main;
			background = new GUIImage();
			background.SetPosition(5f, 5f);
			background.TextureSource = "common_bundle|contentField_435x217";
			Add(background);
			label = new GUILabel();
			label.SetPosition(10f, 0f);
			label.Text = name;
			label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, Color.black, TextAnchor.MiddleLeft);
			Add(label);
			MouseUp += delegate
			{
				main.ItemSelected();
			};
			main.HandleResize(null);
		}

		public override void Draw(DrawModeSetting drawFlags)
		{
			if (Hover)
			{
				background.IsVisible = true;
				main.hoverItem = label.Text;
			}
			else
			{
				background.IsVisible = false;
			}
			base.Draw(drawFlags);
		}

		public override void HandleResize(GUIResizeMessage message)
		{
			float num = main.WindowForText.ListItemWidth;
			float num2 = main.WindowForText.ListItemHeight;
			label.SetSize(num - 10f, num2);
			background.SetSize(num - 10f, num2 - 10f);
		}
	}

	private GUIListViewWindow<GUITextButton> windowForText;

	private GUISlider scroller;

	private GUIControlWindow popupSelection;

	private GUIImage background;

	private GUIImage labelBackground;

	private GUILabel label;

	private GUIButton activateDropDown;

	private bool dropdownShown;

	private bool validInput;

	private bool validArea;

	private float itemHeight;

	private float scrollBarWidth;

	private float popupHeight;

	private bool hasBeenAdded;

	protected string hoverItem;

	private List<GUITextButton> buttons;

	public GUIListViewWindow<GUITextButton> WindowForText
	{
		get
		{
			return windowForText;
		}
	}

	public float ItemHeight
	{
		get
		{
			return itemHeight;
		}
		set
		{
			itemHeight = value;
			HandleResize(null);
		}
	}

	public float ScrollBarWidth
	{
		get
		{
			return scrollBarWidth;
		}
		set
		{
			scrollBarWidth = value;
			HandleResize(null);
		}
	}

	public float PopupHeight
	{
		get
		{
			return popupHeight;
		}
		set
		{
			popupHeight = value;
			HandleResize(null);
		}
	}

	public string SelectedText
	{
		get
		{
			return label.Text;
		}
		set
		{
			label.Text = value;
		}
	}

	public GUIDropDownListBox()
	{
		HitTestType = HitTestTypeEnum.Rect;
		controlTraits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		buttons = new List<GUITextButton>();
		labelBackground = new GUIImage();
		labelBackground.SetPosition(0f, 0f);
		labelBackground.TextureSource = "common_bundle|contentField_435x217";
		Add(labelBackground);
		label = new GUILabel();
		label.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(10f, 0f));
		label.Text = string.Empty;
		label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, Color.black, TextAnchor.MiddleLeft);
		Add(label);
		activateDropDown = new GUIButton();
		activateDropDown.StyleInfo = new SHSButtonStyleInfo("common_bundle|dropdown");
		activateDropDown.Rotation = 90f;
		activateDropDown.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		MouseClickDelegate value = delegate
		{
			dropDownToggle(!dropdownShown);
		};
		activateDropDown.Click += value;
		Click += value;
		Add(activateDropDown);
		popupSelection = new GUIControlWindow();
		background = new GUIImage();
		background.SetPosition(0f, 0f);
		background.TextureSource = "common_bundle|contentField_435x217";
		popupSelection.Add(background);
		scroller = new GUISlider();
		scroller.Orientation = GUISlider.SliderOrientationEnum.Vertical;
		scroller.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		scroller.IsVisible = true;
		scroller.IsEnabled = true;
		scroller.Value = 0f;
		scroller.Min = 0f;
		scroller.Max = 100f;
		popupSelection.Add(scroller);
		windowForText = new GUIListViewWindow<GUITextButton>();
		windowForText.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		windowForText.Slider = scroller;
		windowForText.Orientation = GUIListViewWindow<GUITextButton>.ListViewOrientationEnum.Vertical;
		windowForText.ListItemHeight = 30;
		windowForText.Padding = new Rect(3f, 3f, 3f, 3f);
		windowForText.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Inherit;
		popupSelection.Add(windowForText);
		PopupHeight = 400f;
		ScrollBarWidth = 40f;
		ItemHeight = 30f;
		dropDownToggle(false);
	}

	CaptureHandlerResponse ICaptureHandler.HandleCapture(SHSKeyCode code)
	{
		validInput = (code.code == KeyCode.Mouse0);
		validArea = (popupSelection.ScreenRect.Contains(SHSInput.mouseScreenPosition) || ScreenRect.Contains(SHSInput.mouseScreenPosition));
		if (validInput)
		{
			Manager.CaptureHandlerGotInput(this);
		}
		return (validInput || validArea) ? CaptureHandlerResponse.Passthrough : CaptureHandlerResponse.Block;
	}

	void ICaptureHandler.OnCaptureAcquired()
	{
	}

	void ICaptureHandler.OnCaptureUnacquired()
	{
	}

	private void dropDownToggle(bool toggle)
	{
		if (!hasBeenAdded && parent != null)
		{
			parent.Add(popupSelection, DrawOrder.DrawLast, DrawPhaseHintEnum.PostDraw);
			hasBeenAdded = true;
			HandleResize(null);
		}
		dropdownShown = toggle;
		if (toggle)
		{
			SHSInput.SetInputBlockingMode(this, SHSInput.InputBlockType.CaptureMode);
			SHSInput.AddCaptureHandler(this, this);
			popupSelection.HitTestType = HitTestTypeEnum.Rect;
		}
		else
		{
			SHSInput.RevertInputBlockingMode(this);
			popupSelection.HitTestType = HitTestTypeEnum.Transparent;
		}
	}

	public void AddItem(string name)
	{
		GUITextButton item = new GUITextButton(this, name);
		windowForText.AddItem(item);
		buttons.Add(item);
	}

	public void ClearAll()
	{
		windowForText.Clear();
		buttons.Clear();
	}

	public void SelectFirstItem()
	{
		hoverItem = buttons[0].label.Text;
		label.Text = hoverItem;
		AppShell.Instance.EventMgr.Fire(this, new DropDownItemSelectedMessage(this, SelectedText));
	}

	public void SelectItem(string text)
	{
		int index = 0;
		for (int i = 0; i < buttons.Count; i++)
		{
			if (buttons[i].label.Text == text)
			{
				index = i;
				break;
			}
		}
		hoverItem = buttons[index].label.Text;
		label.Text = hoverItem;
		AppShell.Instance.EventMgr.Fire(this, new DropDownItemSelectedMessage(this, SelectedText));
	}

	private void ItemSelected()
	{
		if (windowForText.ScreenRect.Contains(SHSInput.mouseScreenPosition))
		{
			label.Text = hoverItem;
			dropDownToggle(false);
			AppShell.Instance.EventMgr.Fire(this, new DropDownItemSelectedMessage(this, SelectedText));
		}
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		if (dropdownShown)
		{
			popupSelection.IsVisible = true;
		}
		else
		{
			popupSelection.IsVisible = false;
		}
		base.Draw(drawFlags);
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		GUIControlWindow gUIControlWindow = popupSelection;
		Vector2 size = Size;
		gUIControlWindow.SetSize(size.x, PopupHeight);
		popupSelection.SetPosition(position.x, position.y + ItemHeight);
		if (parent != null)
		{
			Rect screenRect = popupSelection.ScreenRect;
			if (isPopupIsOutOfScreen(screenRect.xMin, screenRect.xMax, screenRect.yMax))
			{
				GUIControlWindow gUIControlWindow2 = popupSelection;
				float x = position.x;
				float y = position.y;
				Vector2 size2 = popupSelection.Size;
				gUIControlWindow2.SetPosition(x, y - size2.y);
				screenRect = popupSelection.ScreenRect;
				if (isPopupIsOutOfScreen(screenRect.xMin, screenRect.xMax, screenRect.yMin))
				{
					if (parent.Rect.height - Rect.y > Rect.y)
					{
						GUIControlWindow gUIControlWindow3 = popupSelection;
						Vector2 size3 = popupSelection.Size;
						gUIControlWindow3.SetSize(size3.x, parent.Rect.height - Rect.y - ItemHeight);
						popupSelection.SetPosition(position.x, position.y + ItemHeight);
					}
					else
					{
						GUIControlWindow gUIControlWindow4 = popupSelection;
						Vector2 size4 = popupSelection.Size;
						gUIControlWindow4.SetSize(size4.x, Rect.y);
						GUIControlWindow gUIControlWindow5 = popupSelection;
						float x2 = position.x;
						float y2 = position.y;
						Vector2 size5 = popupSelection.Size;
						gUIControlWindow5.SetPosition(x2, y2 - size5.y);
					}
				}
			}
		}
		activateDropDown.SetSize(scrollBarWidth, ItemHeight);
		GUILabel gUILabel = label;
		Vector2 size6 = Size;
		float width = size6.x - scrollBarWidth - 10f;
		Vector2 size7 = Size;
		gUILabel.SetSize(width, size7.y);
		GUIImage gUIImage = labelBackground;
		Vector2 size8 = Size;
		float width2 = size8.x - scrollBarWidth;
		Vector2 size9 = Size;
		gUIImage.SetSize(width2, size9.y);
		GUIImage gUIImage2 = background;
		Vector2 size10 = popupSelection.Size;
		float x3 = size10.x;
		Vector2 size11 = popupSelection.Size;
		gUIImage2.SetSize(x3, size11.y);
		GUISlider gUISlider = scroller;
		float width3 = ScrollBarWidth;
		Vector2 size12 = popupSelection.Size;
		gUISlider.SetSize(width3, size12.y);
		GUIListViewWindow<GUITextButton> gUIListViewWindow = windowForText;
		Vector2 size13 = popupSelection.Size;
		float width4 = size13.x - ScrollBarWidth;
		Vector2 size14 = popupSelection.Size;
		gUIListViewWindow.SetSize(width4, size14.y);
		windowForText.ListItemHeight = (int)ItemHeight;
		GUIListViewWindow<GUITextButton> gUIListViewWindow2 = windowForText;
		Vector2 size15 = windowForText.Size;
		gUIListViewWindow2.ListItemWidth = (int)size15.x;
		foreach (GUITextButton button in buttons)
		{
			button.HandleResize(message);
		}
	}

	private bool isPopupIsOutOfScreen(float xPoint1, float xPoint2, float yPoint)
	{
		return !parent.ScreenRect.Contains(new Vector2(xPoint1, yPoint)) && !parent.ScreenRect.Contains(new Vector2(xPoint2, yPoint));
	}

	public void CaptureHandlerGotInput(ICaptureHandler handler)
	{
		if (handler == this && validInput && !validArea)
		{
			CspUtils.DebugLog("List box got its own valid input. Toggle?");
			dropDownToggle(false);
		}
	}
}
