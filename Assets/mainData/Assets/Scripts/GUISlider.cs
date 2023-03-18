using System;
using UnityEngine;

public class GUISlider : GUIControlWindow, IInputHandler
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

	private string sliderCenterTexture;

	private string sliderStartTexture;

	private string sliderEndTexture;

	private string arrowStartTexture;

	private string arrowEndTexture;

	private SHSButtonStyleInfo scrollButtonStyleInfo;

	private Vector2 scrollButtonSize;

	private bool isSizeSet;

	private bool arrowsEnabled;

	private bool arrowsAlwaysOn;

	public float TickValue = 1f;

	private bool useMouseWheelScroll;

	private float mouseScrollWheelAmount = 70f;

	private bool consrainToMaxAndMin = true;

	private bool updateSliderWhenNotVisible;

	public GUIImage sliderCenter;

	public GUIImage startCap;

	public GUIImage endCap;

	public GUIButton ThumbButton;

	public GUIButton StartArrow;

	public GUIButton EndArrow;

	public GUIHotSpotButton backgroundButton;

	public bool IsRefreshSuppressed;

	private float percentage;

	private bool dragOn;

	private float lastValue;

	public float barWidth = 28f;

	public float VerticalCapHeight = 18f;

	public float VerticalArrowOffset = 18f;

	public float VerticalCapOffset;

	public float VerticalButtonOffset = 20f;

	public float barHeight = 41f;

	public float HorizontalCapWidth = 146f;

	public float HorizontalArrowOffset = 30f;

	public float HorizontalCapOffset;

	public float HorizontalButtonOffset = 55f;

	SHSInput.InputRequestorType IInputHandler.InputRequestorType
	{
		get
		{
			return SHSInput.InputRequestorType.UI;
		}
	}

	bool IInputHandler.CanHandleInput
	{
		get
		{
			return (updateSliderWhenNotVisible || IsVisible) && IsEnabled && SHSInput.IsOverUI();
		}
	}

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
			if (num != num2)
			{
				UpdateArrowVisibility();
				if (this.Changed != null)
				{
					this.Changed(this, new GUIChangedEvent(num, num2));
				}
			}
		}
	}

	public float Percentage
	{
		get
		{
			return percentage;
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
			UpdateArrowVisibility();
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
			UpdateArrowVisibility();
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
			RefreshLayout();
		}
	}

	public string SliderCenterTexture
	{
		get
		{
			return sliderCenterTexture;
		}
		set
		{
			sliderCenterTexture = value;
			RefreshLayout();
		}
	}

	public string SliderStartTexture
	{
		get
		{
			return sliderStartTexture;
		}
		set
		{
			sliderStartTexture = value;
			RefreshLayout();
		}
	}

	public string SliderEndTexture
	{
		get
		{
			return sliderEndTexture;
		}
		set
		{
			sliderEndTexture = value;
			RefreshLayout();
		}
	}

	public string ArrowStartTexture
	{
		get
		{
			return arrowStartTexture;
		}
		set
		{
			arrowStartTexture = value;
			RefreshLayout();
		}
	}

	public string ArrowEndTexture
	{
		get
		{
			return arrowEndTexture;
		}
		set
		{
			arrowEndTexture = value;
			RefreshLayout();
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
			RefreshLayout();
		}
	}

	public Vector2 ScrollButtonSize
	{
		get
		{
			if (!isSizeSet)
			{
				return ThumbButton.Size;
			}
			return scrollButtonSize;
		}
		set
		{
			isSizeSet = true;
			scrollButtonSize = value;
			ThumbButton.SetSize(value);
		}
	}

	public Vector2 ScrollButtonHitTestSize
	{
		get
		{
			return ThumbButton.HitTestSize;
		}
		set
		{
			ThumbButton.HitTestSize = value;
		}
	}

	public HitTestTypeEnum ScrollButtonHitTestType
	{
		get
		{
			return ThumbButton.HitTestType;
		}
		set
		{
			ThumbButton.HitTestType = value;
		}
	}

	public float SliderThickness
	{
		get
		{
			if (orientation == SliderOrientationEnum.Vertical)
			{
				return barWidth;
			}
			return barHeight;
		}
		set
		{
			if (orientation == SliderOrientationEnum.Vertical)
			{
				barWidth = value;
			}
			else
			{
				barHeight = value;
			}
			RefreshLayout();
		}
	}

	public bool ArrowsEnabled
	{
		get
		{
			return arrowsEnabled;
		}
		set
		{
			arrowsEnabled = value;
			UpdateArrowVisibility();
		}
	}

	public bool ArrowsAlwaysOn
	{
		get
		{
			return arrowsAlwaysOn;
		}
		set
		{
			arrowsAlwaysOn = value;
			UpdateArrowVisibility();
		}
	}

	public override bool IsVisible
	{
		get
		{
			return base.IsVisible;
		}
		set
		{
			sliderCenter.IsVisible = value;
			startCap.IsVisible = value;
			endCap.IsVisible = value;
			ThumbButton.IsVisible = value;
			StartArrow.IsVisible = value;
			EndArrow.IsVisible = value;
			if (value)
			{
				UpdateArrowVisibility();
			}
			base.IsVisible = value;
		}
	}

	public bool UseMouseWheelScroll
	{
		get
		{
			return useMouseWheelScroll;
		}
		set
		{
			useMouseWheelScroll = value;
		}
	}

	public float MouseScrollWheelAmount
	{
		get
		{
			return mouseScrollWheelAmount;
		}
		set
		{
			mouseScrollWheelAmount = value;
		}
	}

	public bool ConsrainToMaxAndMin
	{
		get
		{
			return consrainToMaxAndMin;
		}
		set
		{
			consrainToMaxAndMin = value;
		}
	}

	public bool UpdateSliderWhenNotVisible
	{
		get
		{
			return updateSliderWhenNotVisible;
		}
		set
		{
			updateSliderWhenNotVisible = value;
		}
	}

	public bool IsDragging
	{
		get
		{
			return dragOn;
		}
	}

	public event ChangedEventDelegate Changed;

	public event Action<float> OnMouseWheelChanged;

	public GUISlider()
	{
		Traits = ControlTraits.ChildDefault;
		isVisible = false;
		cachedVisible = true;
		backgroundButton = new GUIHotSpotButton();
		backgroundButton.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
		Add(backgroundButton);
		min = 0f;
		max = 100f;
		sliderCenter = new GUIImage();
		sliderCenter.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
		Add(sliderCenter);
		startCap = new GUIImage();
		Add(startCap);
		endCap = new GUIImage();
		Add(endCap);
		StartArrow = new GUIButton();
		StartArrow.IsVisible = false;
		StartArrow.HitTestType = HitTestTypeEnum.Alpha;
		StartArrow.Click += delegate
		{
			Value -= TickValue;
			UpdateArrowVisibility();
		};
		Add(StartArrow);
		EndArrow = new GUIButton();
		EndArrow.IsVisible = false;
		EndArrow.HitTestType = HitTestTypeEnum.Alpha;
		EndArrow.Click += delegate
		{
			Value += TickValue;
			UpdateArrowVisibility();
		};
		Add(EndArrow);
		ThumbButton = new GUIButton();
		ThumbButton.HitTestType = HitTestTypeEnum.Alpha;
		ThumbButton.MouseDown += delegate
		{
			dragOn = true;
			lastValue = Value;
		};
		ThumbButton.MouseUp += delegate
		{
			dragOn = false;
			if (IsVisible)
			{
				float value = Value;
				if (lastValue != value && this.Changed != null)
				{
					this.Changed(this, new GUIChangedEvent(value, value));
				}
			}
		};
		Add(ThumbButton);
		backgroundButton.MouseDown += delegate
		{
			ThumbButton.FireMouseDown(new GUIMouseEvent());
		};
		backgroundButton.MouseUp += delegate
		{
			ThumbButton.FireMouseUp(new GUIMouseEvent());
		};
		setupArt();
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
	}

	private void setPercentageBasedOnValue(float value)
	{
		float num = value - min;
		if (max - min != 0f)
		{
			percentage = num / (max - min);
		}
		if (consrainToMaxAndMin)
		{
			fixUpPercentage();
		}
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

	public void RefreshLayout()
	{
		if (!IsRefreshSuppressed)
		{
			setupArt();
		}
	}

	protected virtual void setupArt()
	{
		if (orientation == SliderOrientationEnum.Vertical)
		{
			float num = (!arrowsEnabled) ? 0f : VerticalArrowOffset;
			float num2 = Rect.height - 2f * (VerticalCapHeight + VerticalCapOffset);
			num2 -= ((!arrowsEnabled) ? 0f : (2f * VerticalArrowOffset));
			num2 = Math.Max(num2, 0f);
			sliderCenter.TextureSource = ((sliderCenterTexture != null) ? sliderCenterTexture : "common_bundle|scrollbar_center");
			startCap.TextureSource = ((sliderStartTexture != null) ? sliderStartTexture : "common_bundle|scrollbar_top_cap");
			endCap.TextureSource = ((sliderEndTexture != null) ? sliderEndTexture : "common_bundle|scrollbar_bottom_cap");
			startCap.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, num));
			startCap.SetSize(barWidth, VerticalCapHeight);
			endCap.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 0f - num));
			endCap.SetSize(barWidth, VerticalCapHeight);
			sliderCenter.SetSize(barWidth, num2);
			sliderCenter.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
			StartArrow.StyleInfo = new SHSButtonStyleInfo((arrowStartTexture != null) ? arrowStartTexture : "common_bundle|arrow_up", SHSButtonStyleInfo.SizeCategoryEnum.Small);
			StartArrow.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, VerticalArrowOffset));
			if (arrowStartTexture == null)
			{
				StartArrow.SetSize(64f, 64f);
			}
			EndArrow.StyleInfo = new SHSButtonStyleInfo((arrowEndTexture != null) ? arrowEndTexture : "common_bundle|arrow_down", SHSButtonStyleInfo.SizeCategoryEnum.Small);
			EndArrow.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f - VerticalArrowOffset));
			if (arrowEndTexture == null)
			{
				EndArrow.SetSize(64f, 64f);
			}
			ThumbButton.StyleInfo = (scrollButtonStyleInfo ?? new SHSButtonStyleInfo("common_bundle|scrollbead", SHSButtonStyleInfo.SizeCategoryEnum.Small));
			ThumbButton.SetSize((!isSizeSet) ? new Vector2(64f, 64f) : scrollButtonSize);
			UpdateArrowVisibility();
			backgroundButton.SetSize(barWidth, VerticalCapHeight * 2f + num2);
		}
		else
		{
			float num3 = (!arrowsEnabled) ? 0f : HorizontalArrowOffset;
			float num4 = Rect.width - 2f * HorizontalCapWidth;
			num4 -= ((!arrowsEnabled) ? 0f : (2f * HorizontalArrowOffset));
			num4 = Math.Max(num4, 0f);
			sliderCenter.TextureSource = ((sliderCenterTexture != null) ? sliderCenterTexture : "common_bundle|scrollbar_horizontal_center");
			startCap.TextureSource = ((sliderStartTexture != null) ? sliderStartTexture : "common_bundle|scrollbar_left_cap");
			endCap.TextureSource = ((sliderEndTexture != null) ? sliderEndTexture : "common_bundle|scrollbar_right_cap");
			startCap.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(num3, 0f));
			startCap.SetSize(HorizontalCapWidth, barHeight);
			endCap.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(0f - num3, 0f));
			endCap.SetSize(HorizontalCapWidth, barHeight);
			sliderCenter.SetSize(num4, barHeight);
			sliderCenter.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
			StartArrow.StyleInfo = new SHSButtonStyleInfo((arrowStartTexture != null) ? arrowStartTexture : "common_bundle|arrow_left", SHSButtonStyleInfo.SizeCategoryEnum.Small);
			StartArrow.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(HorizontalArrowOffset, 0f));
			StartArrow.SetSize(128f, 128f);
			EndArrow.StyleInfo = new SHSButtonStyleInfo((arrowEndTexture != null) ? arrowEndTexture : "common_bundle|arrow_right", SHSButtonStyleInfo.SizeCategoryEnum.Small);
			EndArrow.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f - HorizontalArrowOffset, 0f));
			EndArrow.SetSize(128f, 128f);
			ThumbButton.StyleInfo = (scrollButtonStyleInfo ?? new SHSButtonStyleInfo("common_bundle|scrollbead_horizontal", SHSButtonStyleInfo.SizeCategoryEnum.Small));
			ThumbButton.SetSize((!isSizeSet) ? new Vector2(128f, 128f) : scrollButtonSize);
			UpdateArrowVisibility();
			backgroundButton.SetSize(HorizontalCapWidth * 2f + num4, barHeight);
		}
	}

	private void UpdateArrowVisibility()
	{
		float value = Value;
		if (!arrowsEnabled)
		{
			StartArrow.IsVisible = false;
			EndArrow.IsVisible = false;
			return;
		}
		if (arrowsAlwaysOn)
		{
			StartArrow.IsVisible = true;
			EndArrow.IsVisible = true;
			return;
		}
		if (value == min)
		{
			StartArrow.IsVisible = false;
		}
		else if (value > min)
		{
			StartArrow.IsVisible = true;
		}
		if (value == max)
		{
			EndArrow.IsVisible = false;
		}
		else if (value < max)
		{
			EndArrow.IsVisible = true;
		}
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		setupArt();
	}

	public void positionThumbButton()
	{
		if (orientation == SliderOrientationEnum.Horizontal)
		{
			float num = HorizontalButtonOffset + ((!arrowsEnabled) ? 0f : HorizontalArrowOffset);
			float x = num + Mathf.Clamp01(percentage) * (Rect.width - num * 2f);
			ThumbButton.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(x, 0f));
		}
		else
		{
			float num2 = VerticalCapOffset + VerticalButtonOffset + ((!arrowsEnabled) ? 0f : VerticalArrowOffset);
			float y = num2 + Mathf.Clamp01(percentage) * (Rect.height - num2 * 2f);
			ThumbButton.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, y));
		}
	}

	public void moveTheThumbButton()
	{
		if (!dragOn)
		{
			return;
		}
		float num = percentage;
		float num3;
		float num4;
		float num5;
		if (orientation == SliderOrientationEnum.Horizontal)
		{
			float num2 = (!arrowsEnabled) ? 0f : HorizontalArrowOffset;
			num3 = HorizontalButtonOffset + num2;
			num4 = ScreenRect.width - num3;
			Vector2 mouseScreenPosition = SHSInput.mouseScreenPosition;
			num5 = mouseScreenPosition.x - ScreenRect.x;
		}
		else
		{
			float num6 = (!arrowsEnabled) ? 0f : VerticalArrowOffset;
			num3 = VerticalCapOffset + VerticalButtonOffset + num6;
			num4 = ScreenRect.height - num3;
			Vector2 mouseScreenPosition2 = SHSInput.mouseScreenPosition;
			num5 = mouseScreenPosition2.y - ScreenRect.y;
		}
		if (num5 < num3)
		{
			percentage = 0f;
		}
		else if (num5 > num4)
		{
			percentage = 1f;
		}
		else
		{
			percentage = (num5 - num3) / (num4 - num3);
		}
		if (num != percentage)
		{
			UpdateArrowVisibility();
			if (this.Changed != null)
			{
				this.Changed(this, new GUIChangedEvent(returnValueBasedOnPercentage(num), returnValueBasedOnPercentage(percentage)));
			}
		}
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		moveTheThumbButton();
		positionThumbButton();
		base.Draw(drawFlags);
	}

	public void FireChanged()
	{
		if (this.Changed != null)
		{
			this.Changed(this, new GUIChangedEvent(Value, Value));
		}
	}

	public override void Update()
	{
		if (useMouseWheelScroll)
		{
			float mouseWheelDelta = SHSInput.GetMouseWheelDelta(this);
			if (mouseWheelDelta != 0f)
			{
				float value = Value;
				Value = Mathf.Clamp(Value - mouseScrollWheelAmount * 10f * mouseWheelDelta, Min, Max);
				if (this.OnMouseWheelChanged != null)
				{
					this.OnMouseWheelChanged(Value - value);
				}
			}
		}
		base.Update();
	}
}
