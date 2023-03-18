using System;
using System.Collections;
using UnityEngine;

public class SHSTooltip : GUIWindow
{
	public enum ToolTipVerticalAlignment
	{
		Top,
		Bottom
	}

	public enum ToolTipHorizontalAlignment
	{
		Left,
		Right
	}

	public enum CommonToolTipText
	{
		Close,
		Accept,
		Decline
	}

	private struct ToolTipOrientationInfo
	{
		public bool horizontalFlip;

		public bool verticalFlip;

		public bool isThereAttachment;

		public bool useDefaultPadding;

		public bool useDefaultOffset;

		public int xPadding;

		public int yPadding;

		public int xOffset;

		public int yOffset;
	}

	private float maxWidth = 175f;

	private float minWidth = 85f;

	private float tooltipOffsetX = 60f;

	private float tooltipOffsetY = 5f;

	private SHSStyle tooltipSingleLineStyleRef;

	private SHSStyle tooltipMultiLineStyleRef;

	private GUILabel textLabel;

	private GUIImage bkgImage;

	private bool showOnNextFrame;

	private ToolTipHorizontalAlignment toolTipHorizontalAlign;

	private GUIToolTipManager.ToolTipResources toolTipResources;

	private GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle;

	private NineSliceTexture nineSliceTexture = new NineSliceTexture();

	private static Hashtable commonToolTipText;

	public float MaxWidth
	{
		get
		{
			return maxWidth;
		}
		set
		{
			maxWidth = value;
		}
	}

	public float MinWidth
	{
		get
		{
			return minWidth;
		}
		set
		{
			minWidth = value;
		}
	}

	public float TooltipOffsetX
	{
		get
		{
			return tooltipOffsetX;
		}
		set
		{
			tooltipOffsetX = value;
		}
	}

	public float TooltipOffsetY
	{
		get
		{
			return tooltipOffsetY;
		}
		set
		{
			tooltipOffsetY = value;
		}
	}

	public GUIToolTipManager.ToolTipResources ToolTipResource
	{
		set
		{
			toolTipResources = value;
		}
	}

	public SHSTooltip()
	{
		textLabel = new GUILabel();
		textLabel.Id = "SHSTooltipLabel";
		textLabel.SetControlFlag(ControlFlagSetting.HitTestIgnore, true, false);
		textLabel.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Ignore;
		textLabel.SetPositionAndSize(QuickSizingHint.ParentSize);
		textLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(58, 71, 94), TextAnchor.UpperLeft);
		tooltipSingleLineStyleRef = textLabel.Style;
		bkgImage = new GUIImage();
		SetControlFlag(ControlFlagSetting.HitTestIgnore, true, false);
		Traits.HitTestType = HitTestTypeEnum.Transparent;
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		tooltipMultiLineStyleRef = new SHSStyle(tooltipSingleLineStyleRef);
		tooltipMultiLineStyleRef.UnityStyle.wordWrap = true;
		nineSliceTexture.AttachmentHOffset = 0;
		nineSliceTexture.AttachmentVOffset = -15;
		showOnNextFrame = false;
		Add(bkgImage);
		Add(textLabel);
		Rotation = 0f;
	}

	private static void DefineCommonToolTipText()
	{
		commonToolTipText = new Hashtable();
		commonToolTipText.Add(CommonToolTipText.Close, "#TT_COMMON_1");
		commonToolTipText.Add(CommonToolTipText.Accept, "#TT_COMMON_2");
		commonToolTipText.Add(CommonToolTipText.Decline, "#TT_COMMON_3");
	}

	public static string GetCommonToolTipText(CommonToolTipText ttText)
	{
		if (commonToolTipText == null)
		{
			DefineCommonToolTipText();
		}
		if (commonToolTipText.ContainsKey(ttText))
		{
			return (string)commonToolTipText[ttText];
		}
		return string.Empty;
	}

	public void Configure(ToolTipInfo toolTipInfo, IGUIControl Control)
	{
		Configure(toolTipInfo, Control, true, Vector2.zero);
	}

	public void Configure(ToolTipInfo toolTipInfo, IGUIControl Control, bool tilt)
	{
		Configure(toolTipInfo, Control, tilt, Vector2.zero);
	}

	public void Configure(ToolTipInfo toolTipInfo, IGUIControl Control, Vector2 positionOffset)
	{
		Configure(toolTipInfo, Control, true, positionOffset);
	}

	public void Configure(ToolTipInfo toolTipInfo, IGUIControl Control, bool tilt, Vector2 positionOffset)
	{
		ToolTipOrientationInfo orientationInfo = default(ToolTipOrientationInfo);
		bool flag = false;
		textLabel.InvalidateKerning();
		textLabel.Text = toolTipInfo.GetToolTipText();
		Vector2 vector = tooltipSingleLineStyleRef.UnityStyle.CalcSize(new GUIContent(textLabel.Text));
		float num = (!(toolTipInfo.OverrideMaxWidth > 0f)) ? maxWidth : toolTipInfo.OverrideMaxWidth;
		if (vector.x > num)
		{
			vector.x = num;
			textLabel.VerticalKerning = toolTipInfo.VerticalKerning;
			textLabel.NoLineLimit = true;
			textLabel.Style = tooltipMultiLineStyleRef;
			flag = true;
		}
		else
		{
			textLabel.NoLineLimit = false;
			textLabel.Style = tooltipSingleLineStyleRef;
		}
		Rect screenRect = Control.ScreenRect;
		if (screenRect.x > GUIManager.ScreenRect.width / 2f)
		{
			toolTipHorizontalAlign = ToolTipHorizontalAlignment.Right;
		}
		else
		{
			toolTipHorizontalAlign = ToolTipHorizontalAlignment.Left;
		}
		resourceBundle = toolTipResources.GetBundle("default");
		ToolTipVerticalAlignment toolTipVerticalAlignment = (!(screenRect.y < (float)(toolTipInfo.VerticalKerning * textLabel.LineCount + resourceBundle.attachmentTexture.height))) ? ToolTipVerticalAlignment.Bottom : ToolTipVerticalAlignment.Top;
		if (toolTipInfo.Padding == Vector2.zero)
		{
			orientationInfo.useDefaultPadding = true;
			orientationInfo.isThereAttachment = true;
		}
		else
		{
			Vector2 padding = toolTipInfo.Padding;
			orientationInfo.xPadding = (int)padding.x;
			Vector2 padding2 = toolTipInfo.Padding;
			orientationInfo.yPadding = (int)padding2.y;
		}
		if (toolTipInfo.TextOffset == Vector2.zero)
		{
			orientationInfo.useDefaultOffset = true;
		}
		else
		{
			Vector2 offset = toolTipInfo.Offset;
			orientationInfo.xOffset = (int)offset.x;
			Vector2 offset2 = toolTipInfo.Offset;
			orientationInfo.yOffset = (int)offset2.y;
		}
		if (toolTipInfo.OverrideTooltipAlignment)
		{
			toolTipVerticalAlignment = toolTipInfo.VerticalAlignmentOverride;
			toolTipHorizontalAlign = toolTipInfo.HorizontalAlignmentOverride;
		}
		GatherToolTipOrientationInfo(ref orientationInfo, toolTipVerticalAlignment, toolTipHorizontalAlign, NineSliceTexture.Side.Bottom, NineSliceTexture.SideAlignment.Left);
		textLabel.SetPositionAndSize(orientationInfo.xOffset, orientationInfo.yOffset, vector.x, 100f);
		if (flag)
		{
			textLabel.CalculateTextLayout();
		}
		nineSliceTexture.HorizontalFlip = orientationInfo.horizontalFlip;
		nineSliceTexture.VerticalFlip = orientationInfo.verticalFlip;
		nineSliceTexture.CreateNineSliceTexture(new Vector2(((!flag) ? vector.x : ((float)textLabel.LongestLine)) + (float)orientationInfo.xPadding, toolTipInfo.VerticalKerning * textLabel.LineCount + orientationInfo.yPadding), resourceBundle.bodyTexture, resourceBundle.cornerTextures, resourceBundle.borderTextures, resourceBundle.attachmentTexture, NineSliceTexture.Side.Bottom, NineSliceTexture.SideAlignment.Left);
		bkgImage.SetPositionAndSize(0f, 0f, nineSliceTexture.Texture.width, nineSliceTexture.Texture.height);
		bkgImage.Texture = nineSliceTexture.Texture;
		Size = new Vector2(nineSliceTexture.Texture.width, (float)nineSliceTexture.Texture.height - ((!orientationInfo.verticalFlip) ? 1f : 0.1f));
		Rect screenRect2 = Control.ScreenRect;
		Vector2 vector2 = GUICommon.CenterPoint(screenRect2);
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		if (((GUIControl)Control).HitTestType == HitTestTypeEnum.Alpha)
		{
			Rect rect = new Rect(screenRect2.x + ((GUIControl)Control).AlphaHitTestRect.x, screenRect2.y + ((GUIControl)Control).AlphaHitTestRect.y, ((GUIControl)Control).AlphaHitTestRect.width, ((GUIControl)Control).AlphaHitTestRect.height);
			num4 = ((toolTipHorizontalAlign != 0) ? (rect.x - (float)(nineSliceTexture.Texture.width / 2)) : (rect.x + rect.width));
			num5 = ((toolTipVerticalAlignment != ToolTipVerticalAlignment.Bottom) ? (rect.y + rect.height) : (rect.y - (float)(nineSliceTexture.Texture.height / 2)));
		}
		else
		{
			float height = screenRect2.height;
			Vector2 hitTestSize = ((GUIControl)Control).HitTestSize;
			num2 = height * hitTestSize.y / 2f;
			float width = screenRect2.width;
			Vector2 hitTestSize2 = ((GUIControl)Control).HitTestSize;
			num3 = width * hitTestSize2.x / 2f;
			num4 = ((toolTipHorizontalAlign != 0) ? (vector2.x - num3 - (float)(nineSliceTexture.Texture.width / 2)) : (vector2.x + num3 - (float)(nineSliceTexture.Texture.width / 2)));
			num5 = ((toolTipVerticalAlignment != ToolTipVerticalAlignment.Bottom) ? (vector2.y + num2 - (float)(nineSliceTexture.Texture.height / 2)) : (vector2.y - num2 - (float)(nineSliceTexture.Texture.height / 2)));
		}
		float num6 = num4 + (float)nineSliceTexture.Texture.width - GUIManager.ScreenRect.width;
		bool flag2 = num4 < 0f;
		bool flag3 = num6 > 0f;
		if (flag2)
		{
			num4 = 0f;
		}
		if (flag3)
		{
			num4 -= num6;
		}
		if (toolTipVerticalAlignment == ToolTipVerticalAlignment.Top && !toolTipInfo.IgnoreCursor)
		{
			num4 = GenerateOffsetFromCursor();
		}
		float num7 = num4;
		Vector2 offset3 = toolTipInfo.Offset;
		float x = num7 + offset3.x;
		float num8 = num5;
		Vector2 offset4 = toolTipInfo.Offset;
		Position = new Vector2(x, num8 + offset4.y);
		Vector2 position = Position;
		float x2 = position.x + (float)(nineSliceTexture.Texture.width / 2);
		Vector2 position2 = Position;
		base.RotationPoint = new Vector2(x2, position2.y + (float)(nineSliceTexture.Texture.height / 2));
		if (tilt)
		{
			Rotation = UnityEngine.Random.Range(-3.5f, 3.5f);
			Rotation += 1.5f * (float)Math.Sign(Rotation);
		}
		showOnNextFrame = true;
	}

	private float GenerateOffsetFromCursor()
	{
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		float num = 0f;
		zero = GUIManager.Instance.CursorManager.CursorSize;
		zero2 = GUIManager.Instance.CursorManager.CursorScale;
		Vector2 mouseScreenPosition = SHSInput.mouseScreenPosition;
		float x = mouseScreenPosition.x;
		Vector2 mouseScreenPosition2 = SHSInput.mouseScreenPosition;
		Rect rect = new Rect(x, mouseScreenPosition2.y, zero.x * zero2.x, zero.y * zero2.y);
		if (toolTipHorizontalAlign == ToolTipHorizontalAlignment.Left)
		{
			float x2 = rect.x;
			Vector2 vector = GUIManager.Instance.CursorManager.CustomCursorMetrics(GUIManager.Instance.CursorManager.CursorCurrentType);
			return x2 + vector.x;
		}
		Vector2 position = Position;
		float x3 = position.x;
		Vector2 position2 = Position;
		return x3 + (0f - (position2.x + (float)nineSliceTexture.Texture.width - rect.x));
	}

	private void GatherToolTipOrientationInfo(ref ToolTipOrientationInfo orientationInfo, ToolTipVerticalAlignment verticalAlignment, ToolTipHorizontalAlignment horizontalAlignment, NineSliceTexture.Side side, NineSliceTexture.SideAlignment sideAlignment)
	{
		if (verticalAlignment == ToolTipVerticalAlignment.Top)
		{
			if (horizontalAlignment == ToolTipHorizontalAlignment.Left)
			{
				switch (side)
				{
				case NineSliceTexture.Side.Bottom:
					if (sideAlignment == NineSliceTexture.SideAlignment.Left)
					{
						orientationInfo.horizontalFlip = true;
					}
					orientationInfo.verticalFlip = true;
					break;
				case NineSliceTexture.Side.Left:
					if (sideAlignment == NineSliceTexture.SideAlignment.Bottom)
					{
						orientationInfo.verticalFlip = true;
					}
					break;
				case NineSliceTexture.Side.Right:
					if (sideAlignment == NineSliceTexture.SideAlignment.Bottom)
					{
						orientationInfo.verticalFlip = true;
					}
					orientationInfo.horizontalFlip = true;
					break;
				case NineSliceTexture.Side.Top:
					if (sideAlignment == NineSliceTexture.SideAlignment.Left)
					{
						orientationInfo.horizontalFlip = true;
					}
					break;
				}
			}
			else
			{
				switch (side)
				{
				case NineSliceTexture.Side.Bottom:
					if (sideAlignment == NineSliceTexture.SideAlignment.Right)
					{
						orientationInfo.horizontalFlip = true;
					}
					orientationInfo.verticalFlip = true;
					break;
				case NineSliceTexture.Side.Left:
					if (sideAlignment == NineSliceTexture.SideAlignment.Bottom)
					{
						orientationInfo.verticalFlip = true;
					}
					orientationInfo.horizontalFlip = true;
					break;
				case NineSliceTexture.Side.Right:
					if (sideAlignment == NineSliceTexture.SideAlignment.Bottom)
					{
						orientationInfo.verticalFlip = true;
					}
					break;
				case NineSliceTexture.Side.Top:
					if (sideAlignment == NineSliceTexture.SideAlignment.Right)
					{
						orientationInfo.horizontalFlip = true;
					}
					break;
				}
			}
		}
		else if (horizontalAlignment == ToolTipHorizontalAlignment.Left)
		{
			switch (side)
			{
			case NineSliceTexture.Side.Bottom:
				if (sideAlignment == NineSliceTexture.SideAlignment.Left)
				{
					orientationInfo.horizontalFlip = true;
				}
				break;
			case NineSliceTexture.Side.Left:
				if (sideAlignment == NineSliceTexture.SideAlignment.Top)
				{
					orientationInfo.verticalFlip = true;
				}
				orientationInfo.horizontalFlip = true;
				break;
			case NineSliceTexture.Side.Right:
				if (sideAlignment == NineSliceTexture.SideAlignment.Top)
				{
					orientationInfo.verticalFlip = true;
				}
				break;
			case NineSliceTexture.Side.Top:
				if (sideAlignment == NineSliceTexture.SideAlignment.Right)
				{
					orientationInfo.horizontalFlip = true;
				}
				orientationInfo.verticalFlip = true;
				break;
			}
		}
		else
		{
			switch (side)
			{
			case NineSliceTexture.Side.Bottom:
				if (sideAlignment == NineSliceTexture.SideAlignment.Right)
				{
					orientationInfo.horizontalFlip = true;
				}
				break;
			case NineSliceTexture.Side.Left:
				if (sideAlignment == NineSliceTexture.SideAlignment.Top)
				{
					orientationInfo.verticalFlip = true;
				}
				break;
			case NineSliceTexture.Side.Right:
				if (sideAlignment == NineSliceTexture.SideAlignment.Top)
				{
					orientationInfo.verticalFlip = true;
				}
				break;
			case NineSliceTexture.Side.Top:
				if (sideAlignment == NineSliceTexture.SideAlignment.Right)
				{
					orientationInfo.horizontalFlip = true;
				}
				orientationInfo.verticalFlip = true;
				break;
			}
		}
		if (orientationInfo.useDefaultOffset)
		{
			orientationInfo.xOffset = resourceBundle.cornerTextures[0].width;
			orientationInfo.yOffset = ((!orientationInfo.verticalFlip || side != NineSliceTexture.Side.Bottom || !orientationInfo.isThereAttachment) ? resourceBundle.borderTextures[0].height : (resourceBundle.attachmentTexture.height + resourceBundle.cornerTextures[0].height));
		}
		if (orientationInfo.useDefaultPadding)
		{
			orientationInfo.xPadding = resourceBundle.cornerTextures[0].width;
			orientationInfo.yPadding = resourceBundle.borderTextures[0].height;
		}
	}

	public override void Update()
	{
		if (showOnNextFrame)
		{
			showOnNextFrame = false;
			Show();
		}
		base.OnUpdate();
	}

	public override void OnHide()
	{
		base.OnHide();
	}

	public override void DrawPreprocess()
	{
		base.DrawPreprocess();
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
	}

	protected override void handleMouseHitTestOverState()
	{
	}
}
