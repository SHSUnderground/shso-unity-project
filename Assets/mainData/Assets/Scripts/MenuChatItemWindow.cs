using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MenuChatItemWindow : GUIChildWindow
{
	public const int ItemHeight = 36;

	public const int ItemWindowHeight = 50;

	public const int ItemOffset = 28;

	public const int ItemLeadingExtent = 20;

	public const int ItemTrailingExtent = 16;

	public const int ItemOverlapExtent = 10;

	public const int ItemHighlightWidthExtent = 7;

	public const float normalAlpha = 0.9f;

	public const float highlightAlpha = 1f;

	private bool highlighted;

	public readonly Vector2 TextOffset = new Vector2(20f, -5f);

	public readonly Vector2 LeftNormalImageDimensions = new Vector2(20f, 36f);

	public readonly Vector2 RightNormalImageDimensions = new Vector2(26f, 36f);

	public readonly Vector2 LeftHighlightImageDimensions = new Vector2(38f, 50f);

	public readonly Vector2 RightHighlightImageDimensions = new Vector2(33f, 50f);

	public readonly int TextSizeNormal = 17;

	public readonly int TextSizeHighlight = 20;

	private GUILabel chatLabel;

	private GUIImage leftImageNormal;

	private GUIImage middleImageNormal;

	private GUIImage rightImageNormal;

	private GUIImage leftImageHighlight;

	private GUIImage middleImageHighlight;

	private GUIImage rightImageHighlight;

	private bool configured;

	[CompilerGenerated]
	private MenuLevelInfo _003CMenuLevel_003Ek__BackingField;

	[CompilerGenerated]
	private MenuChatGroup _003CDataItem_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CNativeIndex_003Ek__BackingField;

	public MenuLevelInfo MenuLevel
	{
		[CompilerGenerated]
		get
		{
			return _003CMenuLevel_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CMenuLevel_003Ek__BackingField = value;
		}
	}

	public bool Highlighted
	{
		get
		{
			return highlighted;
		}
	}

	public MenuChatGroup DataItem
	{
		[CompilerGenerated]
		get
		{
			return _003CDataItem_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CDataItem_003Ek__BackingField = value;
		}
	}

	public bool IsLeaf
	{
		get
		{
			return DataItem != null && DataItem.MenuChatGroups.Count == 0;
		}
	}

	public float TextWidth
	{
		get
		{
			if (chatLabel != null)
			{
				SHSStyle sHSStyle = new SHSStyle(chatLabel.Style);
				sHSStyle.UnityStyle.fontSize = TextSizeHighlight;
				Vector2 vector = sHSStyle.UnityStyle.CalcSize(new GUIContent(AppShell.Instance.stringTable[DataItem.PhraseKey]));
				return vector.x;
			}
			return -1f;
		}
	}

	private int NativeIndex
	{
		[CompilerGenerated]
		get
		{
			return _003CNativeIndex_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CNativeIndex_003Ek__BackingField = value;
		}
	}

	private Rect menuRect
	{
		get
		{
			return new Rect(base.rect.x, base.rect.y, base.rect.width - 10f, base.rect.height);
		}
	}

	public MenuChatItemWindow(MenuChatGroup menuChatGroup)
	{
		bool flag = menuChatGroup.MenuChatGroups.Count > 0;
		string arg = (!flag) ? "non_cascade" : "cascade";
		DataItem = menuChatGroup;
		Traits.LifeSpan = ControlTraits.LifeSpanTraitEnum.DestroyOnHide;
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		Traits.BlockTestType = BlockTestTypeEnum.Rect;
		base.HitTestSize = new Vector2(0.99f, 0.8f);
		leftImageNormal = new GUIImage();
		leftImageNormal.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, LeftNormalImageDimensions);
		leftImageNormal.TextureSource = string.Format("{0}|{1}{2}", "communication_bundle", arg, "_left_normal");
		Add(leftImageNormal);
		middleImageNormal = new GUIImage();
		middleImageNormal.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, Vector2.zero);
		middleImageNormal.TextureSource = string.Format("{0}|{1}{2}", "communication_bundle", arg, "_middle_normal");
		Add(middleImageNormal);
		rightImageNormal = new GUIImage();
		rightImageNormal.SetPositionAndSize(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, RightNormalImageDimensions);
		if (!flag)
		{
			rightImageNormal.Offset = new Vector2(-17f, 0f);
		}
		else
		{
			rightImageNormal.Offset = new Vector2(-7f, 0f);
		}
		rightImageNormal.TextureSource = string.Format("{0}|{1}{2}", "communication_bundle", arg, "_right_normal");
		Add(rightImageNormal);
		leftImageHighlight = new GUIImage();
		leftImageHighlight.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, LeftHighlightImageDimensions);
		leftImageHighlight.TextureSource = string.Format("{0}|{1}{2}", "communication_bundle", arg, "_left_highlight");
		leftImageHighlight.IsVisible = false;
		Add(leftImageHighlight);
		middleImageHighlight = new GUIImage();
		middleImageHighlight.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, Vector2.zero);
		middleImageHighlight.TextureSource = string.Format("{0}|{1}{2}", "communication_bundle", arg, "_middle_highlight");
		middleImageHighlight.IsVisible = false;
		Add(middleImageHighlight);
		rightImageHighlight = new GUIImage();
		rightImageHighlight.SetPositionAndSize(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, RightHighlightImageDimensions);
		rightImageHighlight.TextureSource = string.Format("{0}|{1}{2}", "communication_bundle", arg, "_right_highlight");
		rightImageHighlight.IsVisible = false;
		if (!flag)
		{
			rightImageHighlight.Offset = new Vector2(-17f, 0f);
		}
		else
		{
			rightImageHighlight.Offset = new Vector2(0f, 0f);
		}
		Add(rightImageHighlight);
		chatLabel = new GUILabel();
		chatLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, TextSizeNormal, Color.black, TextAnchor.MiddleLeft);
		chatLabel.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, TextOffset, new Vector2(0f, 0f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		chatLabel.Text = menuChatGroup.PhraseKey;
		Add(chatLabel);
		Alpha = 0.9f;
		MouseOver += MenuChatItemWindow_MouseOver;
		Click += MenuChatItemWindow_Click;
	}

	private void MenuChatItemWindow_Click(GUIControl sender, GUIClickEvent EventData)
	{
		AppShell.Instance.EventMgr.Fire(this, new MenuChatSelectedMessage(DataItem));
	}

	private void MenuChatItemWindow_MouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
		MenuLevel.CurrentMenuChatItemWindow = this;
		AppShell.Instance.EventMgr.Fire(this, new MenuChatActivateMessage(DataItem, MenuLevel, menuRect));
	}

	public override void OnUpdate()
	{
		if (!configured)
		{
			Configure();
		}
		base.OnUpdate();
	}

	public void MouseOverConfigure(bool highlight)
	{
		leftImageNormal.IsVisible = !highlight;
		middleImageNormal.IsVisible = !highlight;
		rightImageNormal.IsVisible = !highlight;
		leftImageHighlight.IsVisible = highlight;
		middleImageHighlight.IsVisible = highlight;
		rightImageHighlight.IsVisible = highlight;
		chatLabel.TextColor = ((!highlight) ? Color.black : ColorUtil.FromRGB255(0, 100, 184));
		chatLabel.FontSize = ((!highlight) ? TextSizeNormal : TextSizeHighlight);
		chatLabel.Bold = highlight;
		Alpha = ((!highlight) ? 0.9f : 1f);
		highlighted = highlight;
	}

	private void Configure()
	{
		if (MenuLevel.Configured)
		{
			float menuWidth = MenuLevel.MenuWidth;
			SetSize(new Vector2(menuWidth + 7f, 50f));
			GUIImage gUIImage = middleImageNormal;
			float num = menuWidth + 7f;
			Vector2 leftNormalImageDimensions = LeftNormalImageDimensions;
			float num2 = num - leftNormalImageDimensions.x;
			Vector2 rightNormalImageDimensions = RightNormalImageDimensions;
			float num3 = num2 - rightNormalImageDimensions.x;
			Vector2 offset = rightImageNormal.Offset;
			gUIImage.SetSize(num3 - Math.Abs(offset.x), 36f);
			GUIImage gUIImage2 = middleImageNormal;
			Vector2 leftNormalImageDimensions2 = LeftNormalImageDimensions;
			gUIImage2.Offset = new Vector2(leftNormalImageDimensions2.x, 0f);
			GUIImage gUIImage3 = middleImageHighlight;
			Vector2 leftHighlightImageDimensions = LeftHighlightImageDimensions;
			float num4 = menuWidth - leftHighlightImageDimensions.x;
			Vector2 rightHighlightImageDimensions = RightHighlightImageDimensions;
			float num5 = num4 - rightHighlightImageDimensions.x;
			Vector2 offset2 = rightImageHighlight.Offset;
			gUIImage3.SetSize(num5 + offset2.x + 7f, 50f);
			GUIImage gUIImage4 = middleImageHighlight;
			Vector2 leftHighlightImageDimensions2 = LeftHighlightImageDimensions;
			gUIImage4.Offset = new Vector2(leftHighlightImageDimensions2.x, 0f);
			chatLabel.Size = new Vector2(menuWidth, 50f);
			configured = true;
		}
	}
}
