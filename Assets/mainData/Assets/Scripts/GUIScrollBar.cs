using UnityEngine;

public class GUIScrollBar : GUIControlWindow
{
	public enum SliderOrientationEnum
	{
		Vertical,
		Horizontal
	}

	public delegate void ChangedEventDelegate(GUIControl sender, GUIChangedEvent eventData);

	protected float min;

	protected float max;

	private SliderOrientationEnum orientation;

	protected float barWidth = 12f;

	private float dragBarSize = 50f;

	private Texture2D sliderCenterTexture;

	private Texture2D sliderStartTexture;

	private Texture2D sliderEndTexture;

	private Texture2D dragBarCenterTexture;

	private Texture2D dragBarStartTexture;

	private Texture2D dragBarEndTexture;

	private SHSButtonStyleInfo scrollButtonStyleInfo;

	private GUIDrawTexture sliderCenter;

	private GUIDrawTexture startCap;

	private GUIDrawTexture endCap;

	private GUIDrawTexture startDragBar;

	private GUIDrawTexture centerDragBar;

	private GUIDrawTexture endDragBar;

	private GUIControlWindow scrollSubWindow;

	private GUIButton topButton;

	private GUIButton bottomButton;

	private float percentage;

	private bool dragOn;

	private Vector2 offsetDrag = Vector2.zero;

	public float Value
	{
		get
		{
			return returnValueBasedOnPercentage(percentage);
		}
		set
		{
			float perc = percentage;
			setPercentageBasedOnValue(value);
			float num = returnValueBasedOnPercentage(perc);
			float num2 = returnValueBasedOnPercentage(percentage);
			if (this.Changed != null && num != num2)
			{
				this.Changed(this, new GUIChangedEvent(num, num2));
			}
		}
	}

	public float Min
	{
		get
		{
			return min;
		}
		set
		{
			float percentageBasedOnValue = returnValueBasedOnPercentage(percentage);
			min = value;
			setPercentageBasedOnValue(percentageBasedOnValue);
		}
	}

	public float Max
	{
		get
		{
			return max;
		}
		set
		{
			float percentageBasedOnValue = returnValueBasedOnPercentage(percentage);
			max = value;
			setPercentageBasedOnValue(percentageBasedOnValue);
		}
	}

	public SliderOrientationEnum Orientation
	{
		get
		{
			return orientation;
		}
		set
		{
			orientation = value;
			setupArt();
		}
	}

	public float BarWidth
	{
		get
		{
			return barWidth;
		}
		set
		{
			barWidth = value;
		}
	}

	public float DragBarSize
	{
		get
		{
			return dragBarSize;
		}
		set
		{
			dragBarSize = value;
		}
	}

	public float DragBarMaxSize
	{
		get
		{
			if (orientation == SliderOrientationEnum.Horizontal)
			{
				return scrollSubWindow.ScreenRect.width;
			}
			return scrollSubWindow.ScreenRect.height;
		}
	}

	public Texture2D SliderCenterTexture
	{
		get
		{
			return sliderCenterTexture;
		}
		set
		{
			sliderCenterTexture = value;
			setupArt();
		}
	}

	public Texture2D SliderStartTexture
	{
		get
		{
			return sliderStartTexture;
		}
		set
		{
			sliderStartTexture = value;
			setupArt();
		}
	}

	public Texture2D SliderEndTexture
	{
		get
		{
			return sliderEndTexture;
		}
		set
		{
			sliderEndTexture = value;
			setupArt();
		}
	}

	public Texture2D DragBarCenterTexture
	{
		get
		{
			return dragBarCenterTexture;
		}
		set
		{
			dragBarCenterTexture = value;
			setupArt();
		}
	}

	public Texture2D DragBarStartTexture
	{
		get
		{
			return dragBarStartTexture;
		}
		set
		{
			dragBarStartTexture = value;
			setupArt();
		}
	}

	public Texture2D DragBarEndTexture
	{
		get
		{
			return dragBarEndTexture;
		}
		set
		{
			dragBarEndTexture = value;
			setupArt();
		}
	}

	public SHSButtonStyleInfo ScrollButtonStyleInfo
	{
		get
		{
			return scrollButtonStyleInfo;
		}
		set
		{
			scrollButtonStyleInfo = value;
			setupArt();
		}
	}

	private float growth
	{
		get
		{
			return barWidth / 12f;
		}
	}

	public event ChangedEventDelegate Changed;

	public GUIScrollBar()
	{
		Traits = ControlTraits.ChildDefault;
		IsVisible = true;
		scrollSubWindow = new GUIControlWindow();
		scrollSubWindow.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
		Add(scrollSubWindow);
		min = 0f;
		max = 100f;
		sliderCenter = new GUIDrawTexture();
		sliderCenter.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
		scrollSubWindow.Add(sliderCenter);
		startCap = new GUIDrawTexture();
		scrollSubWindow.Add(startCap);
		endCap = new GUIDrawTexture();
		scrollSubWindow.Add(endCap);
		startDragBar = new GUIDrawTexture();
		startDragBar.Color = Color.black;
		scrollSubWindow.Add(startDragBar);
		centerDragBar = new GUIDrawTexture();
		centerDragBar.Color = Color.black;
		scrollSubWindow.Add(centerDragBar);
		endDragBar = new GUIDrawTexture();
		endDragBar.Color = Color.black;
		scrollSubWindow.Add(endDragBar);
		topButton = new GUIButton();
		Add(topButton);
		bottomButton = new GUIButton();
		Add(bottomButton);
		topButton.MouseDown += quickJump;
		bottomButton.MouseDown += quickJump;
		startDragBar.MouseDown += activateDrag;
		startDragBar.MouseUp += deactivateDrag;
		centerDragBar.MouseDown += activateDrag;
		centerDragBar.MouseUp += deactivateDrag;
		endDragBar.MouseDown += activateDrag;
		endDragBar.MouseUp += deactivateDrag;
		sliderCenter.MouseDown += quickJump;
		startCap.MouseDown += quickJump;
		endCap.MouseDown += quickJump;
		setupArt();
	}

	private void setPercentageBasedOnValue(float value)
	{
		float num = value - min;
		if (max - min != 0f)
		{
			percentage = num / (max - min);
		}
		fixUpPercentage();
		float num2 = returnValueBasedOnPercentage(percentage);
		if (this.Changed != null && value != num2)
		{
			this.Changed(this, new GUIChangedEvent(value, num2));
		}
	}

	private float returnValueBasedOnPercentage(float perc)
	{
		float num = max - min;
		return num * perc + min;
	}

	private void fixUpPercentage()
	{
		if (percentage > 1f)
		{
			percentage = 1f;
		}
		else if (percentage < 0f)
		{
			percentage = 0f;
		}
	}

	public void AutoDragBarResize(float screenSize)
	{
		if (max == 0f)
		{
			dragBarSize = DragBarMaxSize;
		}
		else if (screenSize / max < 1f)
		{
			dragBarSize = DragBarMaxSize * (screenSize / (max * 2f));
		}
		else
		{
			dragBarSize = DragBarMaxSize - DragBarMaxSize * (max / screenSize / 2f);
		}
		if (dragBarSize < 30f)
		{
			dragBarSize = 30f;
		}
		if (dragBarSize > DragBarMaxSize)
		{
			dragBarSize = DragBarMaxSize;
		}
	}

	private void quickJump(GUIControl sender, GUIMouseEvent EventData)
	{
		float oldPerc = percentage;
		Rect screenRect = centerDragBar.ScreenRect;
		float num2;
		if (orientation == SliderOrientationEnum.Horizontal)
		{
			float num = screenRect.x + screenRect.width * 0.5f;
			Vector2 mouseScreenPosition = SHSInput.mouseScreenPosition;
			num2 = num - mouseScreenPosition.x;
		}
		else
		{
			float num3 = screenRect.y + screenRect.height * 0.5f;
			Vector2 mouseScreenPosition2 = SHSInput.mouseScreenPosition;
			num2 = num3 - mouseScreenPosition2.y;
		}
		float num4 = 1f;
		if (num2 > 0f)
		{
			num4 = -1f;
		}
		percentage += num4 * 0.1f;
		if (percentage > 1f)
		{
			percentage = 1f;
		}
		if (percentage < 0f)
		{
			percentage = 0f;
		}
		sendChangedEvent(oldPerc, percentage);
	}

	private void activateDrag(GUIControl sender, GUIMouseEvent EventData)
	{
		dragOn = true;
		Rect screenRect = centerDragBar.ScreenRect;
		float num = screenRect.x + screenRect.width * 0.5f;
		Vector2 mouseScreenPosition = SHSInput.mouseScreenPosition;
		float x = num - mouseScreenPosition.x;
		float num2 = screenRect.y + screenRect.height * 0.5f;
		Vector2 mouseScreenPosition2 = SHSInput.mouseScreenPosition;
		offsetDrag = new Vector2(x, num2 - mouseScreenPosition2.y);
	}

	private void deactivateDrag(GUIControl sender, GUIMouseEvent EventData)
	{
		dragOn = false;
	}

	private void setupArt()
	{
		if (orientation == SliderOrientationEnum.Vertical)
		{
			scrollSubWindow.SetSize(base.rect.width, base.rect.height - 80f);
		}
		else
		{
			scrollSubWindow.SetSize(base.rect.width - 80f, base.rect.height);
		}
		if (orientation == SliderOrientationEnum.Vertical)
		{
			if (sliderCenterTexture == null)
			{
				sliderCenter.TextureSource = "toolbox_bundle|scrollBar_centerTileVertical";
			}
			if (sliderStartTexture == null)
			{
				startCap.TextureSource = "toolbox_bundle|scrollBar_top";
			}
			if (sliderEndTexture == null)
			{
				endCap.TextureSource = "toolbox_bundle|scrollBar_bottom";
			}
			startCap.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, 0f));
			startCap.SetSize(12f * growth, 7f * growth);
			endCap.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 0f));
			endCap.SetSize(12f * growth, 7f * growth);
			sliderCenter.SetSize(12f * growth, scrollSubWindow.Rect.height - 14f * growth);
			sliderCenter.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, 7f * growth));
			if (dragBarCenterTexture == null)
			{
				centerDragBar.TextureSource = "toolbox_bundle|scrollBar_centerTileVertical";
			}
			if (dragBarStartTexture == null)
			{
				startDragBar.TextureSource = "toolbox_bundle|scrollBar_top";
			}
			if (dragBarEndTexture == null)
			{
				endDragBar.TextureSource = "toolbox_bundle|scrollBar_bottom";
			}
			startDragBar.SetSize(12f * growth, 7f * growth);
			endDragBar.SetSize(12f * growth, 7f * growth);
			topButton.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle);
			topButton.Rotation = 270f;
			bottomButton.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle);
			bottomButton.Rotation = 90f;
		}
		else
		{
			if (sliderCenterTexture == null)
			{
				sliderCenter.TextureSource = "toolbox_bundle|scrollBar_centerTileVertical";
			}
			if (sliderStartTexture == null)
			{
				startCap.TextureSource = "toolbox_bundle|scrollBar_top";
			}
			if (sliderEndTexture == null)
			{
				endCap.TextureSource = "toolbox_bundle|scrollBar_bottom";
			}
			startCap.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(0f, 0f));
			startCap.SetSize(7f * growth, 12f * growth);
			endCap.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(0f, 0f));
			endCap.SetSize(7f * growth, 12f * growth);
			sliderCenter.SetSize(scrollSubWindow.Rect.width - 14f * growth, 12f * growth);
			sliderCenter.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(7f * growth, 0f));
			if (dragBarCenterTexture == null)
			{
				centerDragBar.TextureSource = "toolbox_bundle|scrollBar_centerTileHorizontal";
			}
			if (dragBarStartTexture == null)
			{
				startDragBar.TextureSource = "toolbox_bundle|scrollBar_left";
			}
			if (dragBarEndTexture == null)
			{
				endDragBar.TextureSource = "toolbox_bundle|scrollBar_right";
			}
			startDragBar.SetSize(7f * growth, 12f * growth);
			endDragBar.SetSize(7f * growth, 12f * growth);
			topButton.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft);
			topButton.Rotation = 180f;
			bottomButton.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
			bottomButton.Rotation = 0f;
		}
		topButton.StyleInfo = ((scrollButtonStyleInfo == null) ? new SHSButtonStyleInfo("toolbox_bundle|scroll_button_minimap_characters") : scrollButtonStyleInfo);
		bottomButton.StyleInfo = ((scrollButtonStyleInfo == null) ? new SHSButtonStyleInfo("toolbox_bundle|scroll_button_minimap_characters") : scrollButtonStyleInfo);
		topButton.SetSize(43f, 43f);
		bottomButton.SetSize(43f, 43f);
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		setupArt();
	}

	public void positionDragBar()
	{
		if (orientation == SliderOrientationEnum.Horizontal)
		{
			centerDragBar.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(percentage * (scrollSubWindow.Rect.width - dragBarSize) + 7f * growth, 0f));
			centerDragBar.SetSize(dragBarSize - 14f * growth, 12f * growth);
			Rect rect = centerDragBar.Rect;
			startDragBar.SetPosition(rect.x, rect.y, AnchorAlignmentEnum.TopRight);
			endDragBar.SetPosition(rect.x + rect.width, rect.y, AnchorAlignmentEnum.TopLeft);
		}
		else
		{
			centerDragBar.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, percentage * (scrollSubWindow.Rect.height - dragBarSize) + 7f * growth));
			centerDragBar.SetSize(12f * growth, dragBarSize - 14f * growth);
			Rect rect2 = centerDragBar.Rect;
			startDragBar.SetPosition(rect2.x, rect2.y, AnchorAlignmentEnum.BottomLeft);
			endDragBar.SetPosition(rect2.x, rect2.y + rect2.height, AnchorAlignmentEnum.TopLeft);
		}
	}

	public void moveTheDragBar()
	{
		if (dragOn)
		{
			float num = percentage;
			float num3;
			float num4;
			if (orientation == SliderOrientationEnum.Horizontal)
			{
				float num2 = dragBarSize;
				num3 = scrollSubWindow.ScreenRect.width - num2;
				Vector2 mouseScreenPosition = SHSInput.mouseScreenPosition;
				num4 = mouseScreenPosition.x - scrollSubWindow.ScreenRect.x - num2 * 0.5f + offsetDrag.x;
			}
			else
			{
				float num2 = dragBarSize;
				num3 = scrollSubWindow.ScreenRect.height - num2;
				Vector2 mouseScreenPosition2 = SHSInput.mouseScreenPosition;
				num4 = mouseScreenPosition2.y - scrollSubWindow.ScreenRect.y - num2 * 0.5f + offsetDrag.y;
			}
			if (num4 < 0f)
			{
				percentage = 0f;
			}
			else if (num4 > num3)
			{
				percentage = 1f;
			}
			else
			{
				percentage = num4 / num3;
			}
			if (num != percentage)
			{
				sendChangedEvent(num, percentage);
			}
		}
	}

	private void sendChangedEvent(float oldPerc, float newPerc)
	{
		if (this.Changed != null)
		{
			this.Changed(this, new GUIChangedEvent(returnValueBasedOnPercentage(oldPerc), returnValueBasedOnPercentage(newPerc)));
		}
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		moveTheDragBar();
		positionDragBar();
		base.Draw(drawFlags);
	}
}
