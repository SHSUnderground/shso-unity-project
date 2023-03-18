using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIControl : IDisposable, IContentDependency, IGUIControl, IGUIDragDrop, IGUIDrawable, IGUIHitTestable, IGUINamed, IGUIResizable, ICaptureHandler, IInputHandler
{
	public enum HandleResizeSource
	{
		Screen,
		Control
	}

	public enum DockingAlignmentEnum
	{
		TopLeft,
		TopMiddle,
		TopRight,
		MiddleLeft,
		Middle,
		MiddleRight,
		BottomLeft,
		BottomMiddle,
		BottomRight,
		None
	}

	public enum AnchorAlignmentEnum
	{
		TopLeft,
		TopMiddle,
		TopRight,
		MiddleLeft,
		Middle,
		MiddleRight,
		BottomLeft,
		BottomMiddle,
		BottomRight,
		None
	}

	public enum QuickSizingHint
	{
		ParentSize,
		Centered,
		NoSize,
		ScreenSize
	}

	public enum AutoSizeTypeEnum
	{
		Absolute,
		Percentage,
		Proportional
	}

	public enum OffsetType
	{
		Absolute,
		Percentage
	}

	public enum HitTestTypeEnum
	{
		Rect,
		Region,
		Circular,
		Alpha,
		Transparent
	}

	public enum KeyInputState
	{
		Active,
		Visible,
		Transitory
	}

	public enum BlockTestTypeEnum
	{
		Rect,
		Region,
		Circular,
		Alpha,
		Transparent
	}

	private class PrivateRoundedRect
	{
		private Rect rect;

		public Rect Rect
		{
			get
			{
				return rect;
			}
			set
			{
				rect = new Rect(Mathf.Round(value.x), Mathf.Round(value.y), Mathf.Round(value.width), Mathf.Round(value.height));
			}
		}
	}

	public enum ControlFlagSetting
	{
		IsModal,
		DisabledByModal,
		DrawOnTop,
		DragDropMode,
		HitTestIgnore,
		ToolTipTestIgnore,
		AlphaCascade,
		EnableInherited,
		Persistent,
		ForceDisable,
		ForceInvisible
	}

	public enum DebugDrawSetting
	{
		ShowDrawRegions,
		ShowHitTestRegions
	}

	public enum DrawModeSetting
	{
		NormalMode,
		AlwaysOnTopMode
	}

	public enum ModalLevelEnum
	{
		Full,
		Default,
		LayerBound,
		None
	}

	public enum DrawOrder
	{
		DrawFirst,
		DrawLast
	}

	public enum DrawPhaseHintEnum
	{
		PreDraw,
		PostDraw
	}

	public enum CenteringContext
	{
		Parent,
		Screen
	}

	public enum SetVisibleReason
	{
		Normal,
		Transition
	}

	public class ControlTraits
	{
		public enum ActivationTraitEnum
		{
			Auto,
			Manual
		}

		public enum VisibilityTraitEnum
		{
			Inherit,
			Manual,
			Cached
		}

		public enum DeactivationTraitEnum
		{
			DeactivateOnHide,
			Manual
		}

		public enum LifeSpanTraitEnum
		{
			KeepAlive,
			DestroyOnHide
		}

		public enum VisibilityAncestryTraitEnum
		{
			EnsureAncestorsVisible,
			DetachedVisibility
		}

		public enum UpdateTraitEnum
		{
			ActiveAndEnabled,
			VisibleAndEnabled,
			ActiveIgnoreEnabled,
			VisibleIgnoreEnabled,
			AlwaysUpdate
		}

		public enum EventHandlingEnum
		{
			Block,
			Bubble,
			Ignore
		}

		public enum ResourceLoadingTraitEnum
		{
			Async,
			Sync
		}

		public enum ResourceLoadingPhaseTraitEnum
		{
			Show,
			Active,
			Manual,
			Draw
		}

		public enum EventListenerRegistrationTraitEnum
		{
			Register,
			Ignore
		}

		public enum DeactiveAlphaBlendTraitEnum
		{
			Opaque,
			FadeSyncInternal
		}

		public enum FullScreenOpaqueBackgroundTraitEnum
		{
			HasFullScreenOpaqueBackground,
			DoesNotHaveFullScreenOpaqueBackground
		}

		public enum RespectDisabledAlphaTraitEnum
		{
			RespectDisabledAlpha,
			DisrespectDisabledAlpha
		}

		public enum ContentDependentDisableTraitEnum
		{
			DisableOnContentDependency,
			IgnoreDisableOnContentDependency
		}

		public ActivationTraitEnum ActivationTrait;

		public VisibilityTraitEnum VisibilityTrait;

		public DeactivationTraitEnum DeactivationTrait;

		public VisibilityAncestryTraitEnum VisibleAncestryTrait;

		public UpdateTraitEnum UpdateTrait;

		public LifeSpanTraitEnum LifeSpan;

		public HitTestTypeEnum HitTestType;

		public BlockTestTypeEnum BlockTestType;

		public EventHandlingEnum EventHandlingTrait;

		public ResourceLoadingTraitEnum ResourceLoadingTrait;

		public ResourceLoadingPhaseTraitEnum ResourceLoadingPhaseTrait;

		public EventListenerRegistrationTraitEnum EventListenerRegistrationTrait;

		public DeactiveAlphaBlendTraitEnum DeactiveAlphaBlendTrait;

		public FullScreenOpaqueBackgroundTraitEnum FullScreenOpaqueBackgroundTrait;

		public RespectDisabledAlphaTraitEnum RespectDisabledAlphaTrait;

		public ContentDependentDisableTraitEnum ContentDependentDisableTrait;

		public static ControlTraits Default
		{
			get
			{
				return new ControlTraits(ActivationTraitEnum.Manual, VisibilityTraitEnum.Manual, DeactivationTraitEnum.DeactivateOnHide, VisibilityAncestryTraitEnum.DetachedVisibility, UpdateTraitEnum.ActiveAndEnabled, LifeSpanTraitEnum.KeepAlive, HitTestTypeEnum.Transparent, BlockTestTypeEnum.Rect, EventHandlingEnum.Block, ResourceLoadingTraitEnum.Sync, ResourceLoadingPhaseTraitEnum.Show, EventListenerRegistrationTraitEnum.Register, DeactiveAlphaBlendTraitEnum.FadeSyncInternal, FullScreenOpaqueBackgroundTraitEnum.DoesNotHaveFullScreenOpaqueBackground, RespectDisabledAlphaTraitEnum.RespectDisabledAlpha, ContentDependentDisableTraitEnum.IgnoreDisableOnContentDependency);
			}
		}

		public static ControlTraits ControlDefault
		{
			get
			{
				return new ControlTraits(ActivationTraitEnum.Auto, VisibilityTraitEnum.Inherit, DeactivationTraitEnum.DeactivateOnHide, VisibilityAncestryTraitEnum.DetachedVisibility, UpdateTraitEnum.ActiveAndEnabled, LifeSpanTraitEnum.KeepAlive, HitTestTypeEnum.Rect, BlockTestTypeEnum.Rect, EventHandlingEnum.Block, ResourceLoadingTraitEnum.Sync, ResourceLoadingPhaseTraitEnum.Show, EventListenerRegistrationTraitEnum.Ignore, DeactiveAlphaBlendTraitEnum.FadeSyncInternal, FullScreenOpaqueBackgroundTraitEnum.DoesNotHaveFullScreenOpaqueBackground, RespectDisabledAlphaTraitEnum.RespectDisabledAlpha, ContentDependentDisableTraitEnum.IgnoreDisableOnContentDependency);
			}
		}

		public static ControlTraits ChildDefault
		{
			get
			{
				return new ControlTraits(ActivationTraitEnum.Auto, VisibilityTraitEnum.Cached, DeactivationTraitEnum.DeactivateOnHide, VisibilityAncestryTraitEnum.DetachedVisibility, UpdateTraitEnum.ActiveAndEnabled, LifeSpanTraitEnum.KeepAlive, HitTestTypeEnum.Rect, BlockTestTypeEnum.Transparent, EventHandlingEnum.Block, ResourceLoadingTraitEnum.Sync, ResourceLoadingPhaseTraitEnum.Show, EventListenerRegistrationTraitEnum.Ignore, DeactiveAlphaBlendTraitEnum.FadeSyncInternal, FullScreenOpaqueBackgroundTraitEnum.DoesNotHaveFullScreenOpaqueBackground, RespectDisabledAlphaTraitEnum.RespectDisabledAlpha, ContentDependentDisableTraitEnum.IgnoreDisableOnContentDependency);
			}
		}

		public ControlTraits(ActivationTraitEnum ActivationTrait, VisibilityTraitEnum VisibilityTrait, DeactivationTraitEnum DeactivationTrait, VisibilityAncestryTraitEnum VisibleAncestryTrait, UpdateTraitEnum UpdateTrait, LifeSpanTraitEnum LifeSpan, HitTestTypeEnum HitTestType, BlockTestTypeEnum BlockTestType, EventHandlingEnum EventHandlingTrait, ResourceLoadingTraitEnum ResourceLoadingTrait, ResourceLoadingPhaseTraitEnum ResourceLoadingPhaseTrait, EventListenerRegistrationTraitEnum EventListenerRegistrationTrait, DeactiveAlphaBlendTraitEnum DeactiveAlphaBlendTrait, FullScreenOpaqueBackgroundTraitEnum FullScreenOpaqueBackgroundTrait, RespectDisabledAlphaTraitEnum RespectDisabledAlphaTrait, ContentDependentDisableTraitEnum ContentDependentDisableTrait)
		{
			this.ActivationTrait = ActivationTrait;
			this.VisibilityTrait = VisibilityTrait;
			this.VisibleAncestryTrait = VisibleAncestryTrait;
			this.DeactivationTrait = DeactivationTrait;
			this.UpdateTrait = UpdateTrait;
			this.LifeSpan = LifeSpan;
			this.BlockTestType = BlockTestType;
			this.EventHandlingTrait = EventHandlingTrait;
			this.ResourceLoadingTrait = ResourceLoadingTrait;
			this.ResourceLoadingPhaseTrait = ResourceLoadingPhaseTrait;
			this.EventListenerRegistrationTrait = EventListenerRegistrationTrait;
			this.DeactiveAlphaBlendTrait = DeactiveAlphaBlendTrait;
			this.FullScreenOpaqueBackgroundTrait = FullScreenOpaqueBackgroundTrait;
			this.RespectDisabledAlphaTrait = RespectDisabledAlphaTrait;
			this.ContentDependentDisableTrait = ContentDependentDisableTrait;
		}

		public override string ToString()
		{
			return "ActivationTrait: " + ActivationTrait + "  VisibilityTrait: " + VisibilityTrait + "  DeactivationTrait: " + DeactivationTrait + "  VisibleAncestryTrait: " + VisibleAncestryTrait + "  UpdateTrait: " + UpdateTrait + "  LifeSpan: " + LifeSpan + "  HitTestType: " + HitTestType + "  BlockTestType: " + BlockTestType + "  EventHandlingTrait: " + EventHandlingTrait + "  ResourceLoadingTrait: " + ResourceLoadingTrait + "  ResourceLoadingPhaseTrait: " + ResourceLoadingPhaseTrait + "  EventListenerRegistrationTrait: " + EventListenerRegistrationTrait + "  FullScreenOpaqueBackgroundTrait: " + FullScreenOpaqueBackgroundTrait + "  RespectDisabledAlphaTrait: " + RespectDisabledAlphaTrait + "  ContentDependentDisableTrait: " + ContentDependentDisableTrait;
		}

		public ControlTraits Copy()
		{
			return (ControlTraits)MemberwiseClone();
		}
	}

	public class GUIContext
	{
		public enum ContextType
		{
			NoContext
		}

		public enum Status
		{
			Default,
			Disabled
		}

		private GUIContextManager contextManager;

		private ContextType contextType;

		private Status contextStatus;

		public ContextType Type
		{
			get
			{
				return contextType;
			}
			set
			{
				contextType = value;
			}
		}

		public Status ContextStatus
		{
			get
			{
				contextManager.ContextSwitch(this);
				return contextStatus;
			}
			set
			{
				contextStatus = value;
			}
		}

		public GUIContext()
		{
			contextManager = GUIManager.Instance.ContextManager;
		}
	}

	public class ToolTipInfo
	{
		public enum ToolTipTypeEnum
		{
			None,
			Named,
			Keyed,
			HoverHelp,
			Inherit
		}

		private bool overrideTooltipAlignment;

		private SHSTooltip.ToolTipHorizontalAlignment horizontalAlignmentOverride;

		private SHSTooltip.ToolTipVerticalAlignment verticalAlignmentOverride;

		protected float overrideMaxWidth;

		protected GUIContext context;

		protected Hashtable contextStringLookup;

		protected int verticalKerning = 12;

		protected Vector2 offset = Vector2.zero;

		protected Vector2 textOffset = Vector2.zero;

		protected Vector2 padding = Vector2.zero;

		private ToolTipTypeEnum tooltipType;

		private bool ignoreCursor;

		public bool OverrideTooltipAlignment
		{
			get
			{
				return overrideTooltipAlignment;
			}
			set
			{
				overrideTooltipAlignment = value;
			}
		}

		public SHSTooltip.ToolTipHorizontalAlignment HorizontalAlignmentOverride
		{
			get
			{
				return horizontalAlignmentOverride;
			}
			set
			{
				horizontalAlignmentOverride = value;
				OverrideTooltipAlignment = true;
			}
		}

		public SHSTooltip.ToolTipVerticalAlignment VerticalAlignmentOverride
		{
			get
			{
				return verticalAlignmentOverride;
			}
			set
			{
				verticalAlignmentOverride = value;
				OverrideTooltipAlignment = true;
			}
		}

		public float OverrideMaxWidth
		{
			get
			{
				return overrideMaxWidth;
			}
			set
			{
				overrideMaxWidth = value;
			}
		}

		public GUIContext Context
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

		public Hashtable ContextStringLookup
		{
			get
			{
				return contextStringLookup;
			}
		}

		public int VerticalKerning
		{
			get
			{
				return verticalKerning;
			}
			set
			{
				verticalKerning = value;
			}
		}

		public Vector2 Offset
		{
			get
			{
				return offset;
			}
			set
			{
				offset = value;
			}
		}

		public Vector2 TextOffset
		{
			get
			{
				return textOffset;
			}
			set
			{
				textOffset = value;
			}
		}

		public Vector2 Padding
		{
			get
			{
				return padding;
			}
			set
			{
				padding = value;
			}
		}

		public ToolTipTypeEnum TooltipType
		{
			get
			{
				return tooltipType;
			}
		}

		public bool IgnoreCursor
		{
			get
			{
				return ignoreCursor;
			}
			set
			{
				ignoreCursor = value;
			}
		}

		public ToolTipInfo(ToolTipTypeEnum type)
		{
			context = new GUIContext();
			contextStringLookup = new Hashtable();
			tooltipType = type;
		}

		public virtual string GetToolTipText()
		{
			return string.Empty;
		}
	}

	public class NoToolTipInfo : ToolTipInfo
	{
		private static NoToolTipInfo instance;

		public static NoToolTipInfo Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new NoToolTipInfo();
				}
				return instance;
			}
		}

		private NoToolTipInfo()
			: base(ToolTipTypeEnum.None)
		{
		}

		public override string GetToolTipText()
		{
			throw new Exception("No tool tip text supported in the NoToolTip option.");
		}
	}

	public class InheritedToolTipInfo : ToolTipInfo
	{
		protected IGUIControl ownerControl;

		public InheritedToolTipInfo(IGUIControl ownerControl)
			: base(ToolTipTypeEnum.Inherit)
		{
			this.ownerControl = ownerControl;
		}

		public override string GetToolTipText()
		{
			if (ownerControl != null && ownerControl.Parent != null)
			{
				return ownerControl.ToolTip.GetToolTipText();
			}
			CspUtils.DebugLog("Tried to retrieve text from a tooltip that inherits its text, BUT has no parent control.");
			return string.Empty;
		}
	}

	public class NamedToolTipInfo : ToolTipInfo
	{
		public NamedToolTipInfo(string Name)
			: base(ToolTipTypeEnum.Named)
		{
			contextStringLookup[GUIContext.Status.Default] = Name;
			contextStringLookup[GUIContext.Status.Disabled] = AppShell.Instance.stringTable[Name];
		}

		public NamedToolTipInfo(string Name, Vector2 offset)
			: base(ToolTipTypeEnum.Named)
		{
			contextStringLookup[GUIContext.Status.Default] = Name;
			contextStringLookup[GUIContext.Status.Disabled] = AppShell.Instance.stringTable[Name];
			base.offset = offset;
		}

		public override string GetToolTipText()
		{
			if (contextStringLookup.ContainsKey(base.Context.ContextStatus))
			{
				return (string)contextStringLookup[base.Context.ContextStatus];
			}
			CspUtils.DebugLog("Tried to retrieve text from a tooltip that isn't aware of its current context status.");
			return string.Empty;
		}
	}

	public class KeyedToolTipInfo : ToolTipInfo
	{
		private string key;

		public KeyedToolTipInfo(string Key)
			: base(ToolTipTypeEnum.Keyed)
		{
			key = Key;
		}

		public override string GetToolTipText()
		{
			return key;
		}
	}

	public abstract class HoverHelpInfo : ToolTipInfo
	{
		public enum FlipOverride
		{
			Auto,
			On,
			Off
		}

		public FlipOverride verticalFlipOverride;

		public FlipOverride horizontalFlipOverride;

		public bool extendLeft;

		public HoverHelpInfo()
			: base(ToolTipTypeEnum.HoverHelp)
		{
			verticalFlipOverride = FlipOverride.Auto;
			horizontalFlipOverride = FlipOverride.Auto;
			extendLeft = false;
		}
	}

	public class GenericHoverHelpInfo : HoverHelpInfo
	{
		public string name;

		public string description;

		public string IconLocation;

		public Vector2 IconSize;

		public GenericHoverHelpInfo(string name, string description, string IconLocation, Vector2 IconSize)
		{
			this.name = name;
			this.description = description;
			this.IconLocation = IconLocation;
			this.IconSize = IconSize;
		}
	}

	public class InventoryHoverHelpInfo : HoverHelpInfo
	{
		public object item;

		public string text;

		public InventoryHoverHelpInfo(object item)
		{
			this.item = item;
			text = string.Empty;
		}

		public InventoryHoverHelpInfo(object item, string text)
		{
			this.item = item;
			this.text = text;
		}
	}

	public class HeroHoverHelpInfo : HoverHelpInfo
	{
		public HeroPersisted item;

		public HeroHoverHelpInfo(HeroPersisted item)
		{
			this.item = item;
		}
	}

	public class AchievementHoverHelpInfo : HoverHelpInfo
	{
		public SHSCounterAchievement achievement;

		public SHSCounterBank bank;

		public string heroName;

		public Achievement.AchievementLevelEnum level;

		public AchievementHoverHelpInfo(SHSCounterAchievement achievement, SHSCounterBank bank, string heroName, Achievement.AchievementLevelEnum level)
		{
			this.bank = bank;
			this.achievement = achievement;
			this.heroName = heroName;
			this.level = level;
		}
	}

	public delegate void MouseOverDelegate(GUIControl sender, GUIMouseEvent EventData);

	public delegate void MouseOutDelegate(GUIControl sender, GUIMouseEvent EventData);

	public delegate void MouseClickDelegate(GUIControl sender, GUIClickEvent EventData);

	public delegate void MouseDownDelegate(GUIControl sender, GUIMouseEvent EventData);

	public delegate void MouseUpDelegate(GUIControl sender, GUIMouseEvent EventData);

	public delegate void MouseWheelDelegate(GUIControl sender, GUIMouseWheelEvent EventData);

	public delegate void ResizeDelegate(GUIControl sender, GUIResizeMessage message);

	public delegate void ShowDelegate(GUIControl sender);

	public delegate void HideDelegate(GUIControl sender);

	public delegate void DrawContentLoadingDelegate(IGUIControl control, DrawModeSetting drawFlags);

	private const float CONTENT_LOADING_SIZE_MOD = 1.1f;

	public GameObject guiTreeNode;

	protected Component inspector;

	protected int uniqueId;

	public readonly float DEFAULT_MASK_SCALE = 0.5f;

	public readonly float DEFAULT_OPACITY_THRESHOLD = 0.25f;

	public static DrawContentLoadingDelegate DrawContentLoadingDefault;

	public static ContentLoadingActivateDelegate OnLoadingActivateDefault;

	private ICaptureManager manager;

	private SHSInput.InputRequestorType requestorType;

	private bool mouseWheelListener;

	private bool disposed;

	private BitTexture mask;

	private Color debugColor;

	private PrivateRoundedRect privateRect = new PrivateRoundedRect();

	protected bool guiEnabledState;

	protected Dictionary<KeyInputState, KeyBank> keyBanks;

	protected BitArray controlFlagMask;

	protected BitArray controlFlags;

	protected Rect alphaHitTestRect;

	protected ToolTipInfo toolTip;

	protected Vector2 toolTipOffset;

	protected bool resourcesInitialized;

	private bool InitailizeResourcesInProgress;

	private Texture savedTexture;

	protected List<ContentReference> contentReferenceList;

	private bool isContentConfigured;

	private bool isContentLoaded;

	private bool isContentDependent;

	private float lastContentCheck;

	private float contentCheckFrequency = 2f;

	private ContentLoadingActivateDelegate onLoadingActivate;

	private bool usesCustomContentLoadingDrawRect;

	private Rect contentLoadingCustomDrawRect;

	protected string id;

	private GUIManager.UILayer layer;

	protected string tooltipKey;

	private Matrix4x4 cachedRotation;

	protected float rotation;

	private Vector2 rotationPoint = new Vector2(2.14748365E+09f, 2.14748365E+09f);

	protected SHSStyleInfo styleInfo = SHSInheritedStyleInfo.Instance;

	protected SHSStyle style;

	protected string styleName;

	private DrawPhaseHintEnum drawPhaseHint;

	protected IGUIContainer parent;

	protected ControlTraits controlTraits;

	protected Vector2 hitTestSize;

	protected Vector2 blockTestSize;

	protected bool cachedVisible;

	protected bool isVisible;

	private bool isActive;

	private bool isEnabled = true;

	protected bool clickEnableOverride = true;

	private bool hover;

	protected bool isDownState;

	protected bool isRightDownState;

	protected Color cachedColor;

	protected Color color = new Color(1f, 1f, 1f, 1f);

	protected float animationAlpha = 1f;

	protected float alpha = 1f;

	protected Rect padding;

	protected Rect margin;

	protected bool canDrag;

	protected DockingAlignmentEnum docking = DockingAlignmentEnum.None;

	protected AnchorAlignmentEnum anchor = AnchorAlignmentEnum.None;

	protected OffsetType offsetType;

	protected Vector2 offset = Vector2.zero;

	protected bool directRectMode;

	protected bool autoPosition;

	protected Vector2 centerPoint;

	public readonly Vector2 POSITION_UNDEFINED = new Vector2(1.07374182E+09f, 1.07374182E+09f);

	protected Vector2 position = new Vector2(1.07374182E+09f, 1.07374182E+09f);

	protected Vector2 size = Vector2.zero;

	protected AutoSizeTypeEnum verticalSizeHint;

	protected AutoSizeTypeEnum horizontalSizeHint;

	protected bool autoSize;

	protected float scale = 1f;

	public virtual bool MouseWheelListener
	{
		get
		{
			return mouseWheelListener;
		}
		set
		{
			mouseWheelListener = value;
		}
	}

	public BitTexture Mask
	{
		get
		{
			return mask;
		}
		set
		{
			mask = value;
			int num = mask.Width - 1;
			int num2 = mask.Height - 1;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < mask.Width; i++)
			{
				for (int j = 0; j < mask.Height; j++)
				{
					if (mask.Bits[j * mask.Width + i])
					{
						if (num > i)
						{
							num = i;
						}
						if (num3 < i)
						{
							num3 = i;
						}
						if (num2 > j)
						{
							num2 = j;
						}
						if (num4 < j)
						{
							num4 = j;
						}
					}
				}
			}
			alphaHitTestRect = new Rect(num, num2, num3 - num, num4 - num2);
		}
	}

	protected Rect rect
	{
		get
		{
			return privateRect.Rect;
		}
		set
		{
			privateRect.Rect = value;
		}
	}

	public BitArray ControlFlags
	{
		get
		{
			return controlFlags;
		}
	}

	public Rect AlphaHitTestRect
	{
		get
		{
			return alphaHitTestRect;
		}
		set
		{
			alphaHitTestRect = value;
		}
	}

	public ToolTipInfo ToolTip
	{
		get
		{
			return toolTip;
		}
		set
		{
			toolTip = value;
		}
	}

	public Vector2 ToolTipOffset
	{
		get
		{
			return toolTipOffset;
		}
		set
		{
			toolTipOffset = value;
			if (inspector != null)
			{
				((GUIControlInspector)inspector).toolTipOffset = toolTipOffset;
			}
		}
	}

	public bool ResourcesInitialized
	{
		get
		{
			return resourcesInitialized;
		}
	}

	public virtual HitTestTypeEnum HitTestType
	{
		get
		{
			return controlTraits.HitTestType;
		}
		set
		{
			controlTraits.HitTestType = value;
		}
	}

	public virtual BlockTestTypeEnum BlockTestType
	{
		get
		{
			return controlTraits.BlockTestType;
		}
		set
		{
			controlTraits.BlockTestType = value;
		}
	}

	public virtual bool CanHandleInput
	{
		get
		{
			return !GetControlFlag(ControlFlagSetting.DisabledByModal);
		}
	}

	public virtual SHSInput.InputRequestorType InputRequestorType
	{
		get
		{
			return requestorType;
		}
		set
		{
			requestorType = value;
		}
	}

	public virtual List<ContentReference> ContentReferenceList
	{
		get
		{
			if (!isContentConfigured)
			{
				ConfigureRequiredContent();
			}
			if (isContentConfigured)
			{
				return contentReferenceList;
			}
			return null;
		}
	}

	public virtual bool IsContentLoaded
	{
		get
		{
			if (isContentLoaded)
			{
				return true;
			}
			if (!IsContentDependent)
			{
				CspUtils.DebugLog("Asking for content loading state, but this object is set as not content dependent.");
				return true;
			}
			if (Time.time - lastContentCheck < contentCheckFrequency)
			{
				return false;
			}
			lastContentCheck = Time.time;
			AssetBundleLoader bundleLoader = AppShell.Instance.BundleLoader;
			if (bundleLoader == null)
			{
				CspUtils.DebugLog("Attepting to check content load requirements, but no asset bundle loader class exists.");
				return false;
			}
			int version = -1;
			foreach (ContentReference contentReference in contentReferenceList)
			{
				switch (contentReference.ContentType)
				{
				case ContentTypeEnum.Bundle:
					if (!ShsCacheManager.IsResourceCached("assetbundles/" + (string)contentReference.ReferenceKey + ".unity3d", out version))
					{
						return false;
					}
					if (!bundleLoader.IsAssetBundleCachedOnDisk((string)contentReference.ReferenceKey))
					{
						return false;
					}
					break;
				case ContentTypeEnum.GlobalContentFlag:
					if (!LauncherSequences.DependencyCheck(AssetBundleLoader.BundleGroup.All, false))
					{
						return false;
					}
					break;
				case ContentTypeEnum.PriorityGroup:
					if (!LauncherSequences.DependencyCheck((AssetBundleLoader.BundleGroup)(int)contentReference.ReferenceKey, false))
					{
						return false;
					}
					break;
				case ContentTypeEnum.Other:
					return false;
				default:
					throw new NotImplementedException("Only bundle content types currently supported.");
				}
			}
			isContentLoaded = true;
			return true;
		}
	}

	public virtual bool IsContentDependent
	{
		get
		{
			return isContentDependent;
		}
	}

	public virtual ContentLoadingActivateDelegate OnLoadingActivate
	{
		get
		{
			if (onLoadingActivate != null)
			{
				return onLoadingActivate;
			}
			return OnLoadingActivateDefault;
		}
		set
		{
			onLoadingActivate = value;
		}
	}

	public Rect ContentLoadingCustomDrawRect
	{
		get
		{
			return (!usesCustomContentLoadingDrawRect) ? GetDefaultContentLoadingRect() : contentLoadingCustomDrawRect;
		}
		set
		{
			contentLoadingCustomDrawRect = value;
			usesCustomContentLoadingDrawRect = true;
		}
	}

	public virtual string Id
	{
		get
		{
			return (id != null) ? id : GetType().ToString();
		}
		set
		{
			id = value;
			if (guiTreeNode != null)
			{
				guiTreeNode.name = id;
				((GUIControlInspector)inspector).id = id;
			}
		}
	}

	public GUIManager.UILayer Layer
	{
		get
		{
			return layer;
		}
		set
		{
			layer = value;
		}
	}

	public virtual Rect Rect
	{
		get
		{
			return rect;
		}
		set
		{
			Rect rect = this.rect;
			this.rect = value;
			directRectMode = true;
			HandleResize(new GUIResizeMessage(HandleResizeSource.Control, rect, this.rect));
		}
	}

	public string TooltipKey
	{
		get
		{
			return tooltipKey;
		}
		set
		{
			tooltipKey = value;
		}
	}

	public virtual Rect ScreenRect
	{
		get
		{
			Vector2 vector = new Vector2(rect.x, rect.y);
			if (Parent != null)
			{
				vector.x += Parent.ScreenRect.x;
				vector.y += Parent.ScreenRect.y;
			}
			return new Rect(vector.x, vector.y, rect.width, rect.height);
		}
	}

	public virtual float Rotation
	{
		get
		{
			return rotation;
		}
		set
		{
			rotation = value;
		}
	}

	public Vector2 RotationPoint
	{
		get
		{
			if (rotationPoint[0] == 2.14748365E+09f && rotationPoint[1] == 2.14748365E+09f)
			{
				return new Vector2(rect.x + rect.width / 2f, rect.y + rect.height / 2f);
			}
			return rotationPoint;
		}
		set
		{
			rotationPoint = value;
		}
	}

	public virtual SHSStyleInfo StyleInfo
	{
		get
		{
			return styleInfo;
		}
		set
		{
			styleInfo = value;
			style = null;
		}
	}

	public virtual SHSStyle Style
	{
		get
		{
			if (style != null)
			{
				return style;
			}
			if (styleInfo != null)
			{
				style = GUIManager.Instance.StyleManager.GetStyle(this);
				return style;
			}
			if (styleName != null)
			{
				style = GUIManager.Instance.StyleManager.GetStyle(styleName);
				return (style != null) ? style : GUIManager.Instance.StyleManager.GetStyle("fail");
			}
			return SHSStyle.NoStyle;
		}
		set
		{
			if (value != null)
			{
				style = value;
				styleName = style.UnityStyle.name;
			}
		}
	}

	public string StyleName
	{
		get
		{
			return (style == null) ? styleName : style.UnityStyle.name;
		}
		set
		{
			if (GUIManager.Instance == null || GUIManager.Instance.StyleManager == null)
			{
				throw new Exception("No GUIManager or StyleManager to handle Style Name property set");
			}
			if (GUIManager.Instance.StyleManager.HasStyle(value))
			{
				styleName = value;
				style = GUIManager.Instance.StyleManager.GetStyle(styleName);
			}
			else
			{
				CspUtils.DebugLog("Style : " + styleName + " does not exist as an entry in the StyleManager.");
			}
		}
	}

	public DrawPhaseHintEnum DrawPhaseHint
	{
		get
		{
			return drawPhaseHint;
		}
		set
		{
			drawPhaseHint = value;
		}
	}

	public ControlTraits Traits
	{
		get
		{
			return controlTraits;
		}
		set
		{
			controlTraits = value;
		}
	}

	public Vector2 HitTestSize
	{
		get
		{
			return hitTestSize;
		}
		set
		{
			hitTestSize = value;
		}
	}

	public Vector2 BlockTestSize
	{
		get
		{
			return blockTestSize;
		}
		set
		{
			blockTestSize = value;
		}
	}

	public bool CachedVisible
	{
		get
		{
			return cachedVisible;
		}
		set
		{
			cachedVisible = value;
		}
	}

	public virtual bool IsVisible
	{
		get
		{
			return GetVisible();
		}
		set
		{
			SetVisible(value);
		}
	}

	public bool IsActive
	{
		get
		{
			return isActive;
		}
	}

	public virtual bool IsEnabled
	{
		get
		{
			if (GetControlFlag(ControlFlagSetting.EnableInherited) && parent != null)
			{
				return parent.IsEnabled;
			}
			return isEnabled && (Traits.ContentDependentDisableTrait == ControlTraits.ContentDependentDisableTraitEnum.IgnoreDisableOnContentDependency || !IsContentDependent || IsContentLoaded) && !GetControlFlag(ControlFlagSetting.DisabledByModal) && !GetControlFlag(ControlFlagSetting.ForceDisable);
		}
		set
		{
			isEnabled = value;
		}
	}

	public bool ClickEnabled
	{
		get
		{
			return clickEnableOverride && this.Click != null;
		}
	}

	public virtual bool Hover
	{
		get
		{
			return hover;
		}
	}

	public Color Color
	{
		get
		{
			return color;
		}
		set
		{
			color = value;
		}
	}

	public virtual float AnimationAlpha
	{
		get
		{
			return animationAlpha;
		}
		set
		{
			animationAlpha = value;
			ReflectToInspector();
		}
	}

	public virtual float Alpha
	{
		get
		{
			return alpha;
		}
		set
		{
			alpha = value;
			ReflectToInspector();
		}
	}

	public float DisabledAlpha
	{
		get
		{
			return Alpha * AnimationAlpha * 0.25f;
		}
	}

	public virtual IGUIContainer Parent
	{
		get
		{
			return parent;
		}
		set
		{
			if (parent != null)
			{
			}
			if (value != null)
			{
				parent = value;
				parent.Add(this, DrawOrder.DrawFirst);
				CalculateRect();
			}
		}
	}

	public Rect ClientRect
	{
		get
		{
			return new Rect(PaddingLeft, PaddingTop, rect.width - PaddingRight - PaddingLeft, rect.height - PaddingBottom - PaddingTop);
		}
	}

	public virtual Rect Padding
	{
		get
		{
			return padding;
		}
		set
		{
			padding = value;
		}
	}

	public float PaddingLeft
	{
		get
		{
			return padding.xMin;
		}
		set
		{
			padding = new Rect(value, padding.yMin, padding.width, padding.height);
		}
	}

	public float PaddingTop
	{
		get
		{
			return padding.yMin;
		}
		set
		{
			padding = new Rect(padding.xMin, value, padding.width, padding.height);
		}
	}

	public float PaddingRight
	{
		get
		{
			return padding.width;
		}
		set
		{
			padding = new Rect(padding.xMin, padding.yMin, value, padding.height);
		}
	}

	public float PaddingBottom
	{
		get
		{
			return padding.height;
		}
		set
		{
			padding = new Rect(padding.xMin, padding.yMin, padding.width, value);
		}
	}

	public virtual Rect Margin
	{
		get
		{
			return margin;
		}
		set
		{
			margin = value;
		}
	}

	public float MarginLeft
	{
		get
		{
			return margin.xMin;
		}
		set
		{
			margin = new Rect(value, margin.yMin, margin.width, margin.height);
		}
	}

	public float MarginTop
	{
		get
		{
			return margin.yMin;
		}
		set
		{
			margin = new Rect(margin.xMin, value, margin.width, margin.height);
		}
	}

	public float MarginRight
	{
		get
		{
			return margin.width;
		}
		set
		{
			margin = new Rect(margin.xMin, margin.yMin, value, margin.height);
		}
	}

	public float MarginBottom
	{
		get
		{
			return margin.height;
		}
		set
		{
			margin = new Rect(margin.xMin, margin.yMin, margin.width, value);
		}
	}

	public ICaptureManager Manager
	{
		get
		{
			return manager;
		}
		set
		{
			manager = value;
		}
	}

	public virtual bool CanDrag
	{
		get
		{
			return canDrag;
		}
	}

	public DockingAlignmentEnum Docking
	{
		get
		{
			return docking;
		}
		set
		{
			docking = value;
			autoPosition = (Docking != DockingAlignmentEnum.None || Anchor != AnchorAlignmentEnum.None);
			HandleResize(new GUIResizeMessage(HandleResizeSource.Control, rect, rect));
		}
	}

	public AnchorAlignmentEnum Anchor
	{
		get
		{
			return anchor;
		}
		set
		{
			anchor = value;
			autoPosition = (Docking != DockingAlignmentEnum.None || Anchor != AnchorAlignmentEnum.None);
			HandleResize(new GUIResizeMessage(HandleResizeSource.Control, rect, rect));
		}
	}

	public OffsetType OffsetStyle
	{
		get
		{
			return offsetType;
		}
		set
		{
			offsetType = value;
			HandleResize(new GUIResizeMessage(HandleResizeSource.Control, rect, rect));
		}
	}

	public virtual Vector2 Offset
	{
		get
		{
			return offset;
		}
		set
		{
			offset = value;
			HandleResize(new GUIResizeMessage(HandleResizeSource.Control, rect, rect));
		}
	}

	public bool DirectRectMode
	{
		get
		{
			return directRectMode;
		}
	}

	public bool AutoPosition
	{
		get
		{
			return autoPosition;
		}
	}

	public Vector2 CenterPoint
	{
		get
		{
			return centerPoint;
		}
		set
		{
			centerPoint = value;
		}
	}

	public virtual Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			SetPosition(value);
		}
	}

	public virtual Vector2 Size
	{
		get
		{
			return size;
		}
		set
		{
			SetSize(value);
		}
	}

	public Vector2 RectSize
	{
		get
		{
			return new Vector2(rect.width, rect.height);
		}
	}

	public AutoSizeTypeEnum VerticalSizeHint
	{
		get
		{
			return verticalSizeHint;
		}
		set
		{
			verticalSizeHint = value;
			autoSize = (value != 0 || horizontalSizeHint != AutoSizeTypeEnum.Absolute);
			CalculateRect();
		}
	}

	public AutoSizeTypeEnum HorizontalSizeHint
	{
		get
		{
			return horizontalSizeHint;
		}
		set
		{
			horizontalSizeHint = value;
			autoSize = (value != 0 || verticalSizeHint != AutoSizeTypeEnum.Absolute);
			CalculateRect();
		}
	}

	public bool AutoSize
	{
		get
		{
			return autoSize;
		}
	}

	public float Scale
	{
		get
		{
			return scale;
		}
		set
		{
			scale = value;
			CalculateRect();
		}
	}

	public virtual string Path
	{
		get
		{
			string text = Id;
			for (IGUIContainer iGUIContainer = parent; iGUIContainer != null; iGUIContainer = iGUIContainer.Parent)
			{
				text = iGUIContainer.Id + "/" + text;
			}
			return text;
		}
	}

	public virtual event MouseOverDelegate MouseOver;

	public virtual event MouseOutDelegate MouseOut;

	public virtual event MouseClickDelegate Click;

	public virtual event MouseDownDelegate MouseDown;

	public virtual event MouseDownDelegate RightMouseDown;

	public virtual event MouseUpDelegate MouseUp;

	public virtual event MouseUpDelegate RightMouseUp;

	public virtual event MouseWheelDelegate MouseWheel;

	public virtual event ResizeDelegate Resize;

	public virtual event ShowDelegate OnVisible;

	public virtual event HideDelegate OnHidden;

	public GUIControl(ControlTraits Traits)
	{
		uniqueId = GUIManager.Instance.NextUniqueID;
		AddInspector();
		padding = new Rect(0f, 0f, 0f, 0f);
		rect = default(Rect);
		isVisible = false;
		this.Traits = Traits;
		hitTestSize = new Vector2(1f, 1f);
		blockTestSize = new Vector2(1f, 1f);
		controlFlags = new BitArray(32);
		controlFlagMask = new BitArray(controlFlags.Length, true);
		keyBanks = new Dictionary<KeyInputState, KeyBank>();
		keyBanks[KeyInputState.Active] = new KeyBank(this, KeyInputState.Active, KeyBank.KeyBankTypeEnum.Additive, new Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate>());
		keyBanks[KeyInputState.Visible] = new KeyBank(this, KeyInputState.Visible, KeyBank.KeyBankTypeEnum.Additive, new Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate>());
		ConfigureKeyBanks();
		debugColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 0.3f);
		toolTip = NoToolTipInfo.Instance;
		toolTipOffset = Vector2.zero;
		requestorType = SHSInput.InputRequestorType.UI;
	}

	public virtual void AddInspector()
	{
		if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != 0)
		{
			return;
		}
		if (guiTreeNode != null)
		{
			CspUtils.DebugLog("Attempting to add an inspector to this control twice: " + Id);
			return;
		}
		string name = Id + "_" + uniqueId;
		guiTreeNode = new GameObject(name);
		GameObject x = GameObject.Find("/GUIOrphanage");
		if (x != null)
		{
			Utils.AttachGameObject(x, guiTreeNode);
		}
		AttachInspector();
		((GUIControlInspector)inspector).GUIControl = this;
	}

	public virtual void RemoveInspector()
	{
		if (!(guiTreeNode == null))
		{
			GameObject x = GameObject.Find("/GUIOrphanage");
			if (x != null)
			{
				Utils.AttachGameObject(x, guiTreeNode);
			}
		}
	}

	public void FromInspectorToObject()
	{
		ReflectFromInspector();
	}

	public void FromObjectToInspector()
	{
		ReflectToInspector();
	}

	protected virtual void AttachInspector()
	{
		inspector = guiTreeNode.AddComponent(typeof(GUIControlInspector));
	}

	protected virtual void ReflectToInspector()
	{
		if (inspector != null)
		{
			if (!((GUIControlInspector)inspector).ReflectButton)
			{
				((GUIControlInspector)inspector).id = id;
				((GUIControlInspector)inspector).UniqueId = uniqueId;
				((GUIControlInspector)inspector).type = GetType().ToString();
			}
			((GUIControlInspector)inspector).dockingAlignment = docking;
			((GUIControlInspector)inspector).anchorAlignment = anchor;
			((GUIControlInspector)inspector).offsetType = offsetType;
			((GUIControlInspector)inspector).offset = offset;
			((GUIControlInspector)inspector).position = position;
			((GUIControlInspector)inspector).padding = Padding;
			((GUIControlInspector)inspector).margin = margin;
			((GUIControlInspector)inspector).size = Size;
			((GUIControlInspector)inspector).toolTipOffset = toolTipOffset;
			((GUIControlInspector)inspector).hover = Hover;
			((GUIControlInspector)inspector).IsVisible = IsVisible;
			((GUIControlInspector)inspector).LastIsVisible = IsVisible;
			((GUIControlInspector)inspector).Alpha = alpha;
			((GUIControlInspector)inspector).AnimationAlpha = animationAlpha;
			((GUIControlInspector)inspector).VisibilityTrait = Traits.VisibilityTrait;
			((GUIControlInspector)inspector).EventHandling = Traits.EventHandlingTrait;
			((GUIControlInspector)inspector).RespectDisabledAlphaTrait = Traits.RespectDisabledAlphaTrait;
			((GUIControlInspector)inspector).HitTestType = Traits.HitTestType;
			((GUIControlInspector)inspector).BlockTestType = Traits.BlockTestType;
			((GUIControlInspector)inspector).contentLoadingRect = GetDefaultContentLoadingRect();
		}
	}

	protected virtual void ReflectFromInspector()
	{
		if (inspector != null)
		{
			if (!string.IsNullOrEmpty(((GUIControlInspector)inspector).id))
			{
				Id = ((GUIControlInspector)inspector).id;
			}
			docking = ((GUIControlInspector)inspector).dockingAlignment;
			anchor = ((GUIControlInspector)inspector).anchorAlignment;
			autoPosition = (Docking != DockingAlignmentEnum.None || Anchor != AnchorAlignmentEnum.None);
			offsetType = ((GUIControlInspector)inspector).offsetType;
			offset = ((GUIControlInspector)inspector).offset;
			position = ((GUIControlInspector)inspector).position;
			padding = ((GUIControlInspector)inspector).padding;
			margin = ((GUIControlInspector)inspector).margin;
			size = ((GUIControlInspector)inspector).size;
			toolTipOffset = ((GUIControlInspector)inspector).toolTipOffset;
			hover = ((GUIControlInspector)inspector).hover;
			if (((GUIControlInspector)inspector).LastIsVisible != ((GUIControlInspector)inspector).IsVisible)
			{
				IsVisible = ((GUIControlInspector)inspector).IsVisible;
				((GUIControlInspector)inspector).LastIsVisible = ((GUIControlInspector)inspector).IsVisible;
			}
			alpha = ((GUIControlInspector)inspector).Alpha;
			animationAlpha = ((GUIControlInspector)inspector).AnimationAlpha;
			Traits.VisibilityTrait = ((GUIControlInspector)inspector).VisibilityTrait;
			Traits.EventHandlingTrait = ((GUIControlInspector)inspector).EventHandling;
			Traits.RespectDisabledAlphaTrait = ((GUIControlInspector)inspector).RespectDisabledAlphaTrait;
			Traits.HitTestType = ((GUIControlInspector)inspector).HitTestType;
			Traits.BlockTestType = ((GUIControlInspector)inspector).BlockTestType;
			CalculateRect();
		}
	}

	~GUIControl()
	{
		dispose(false);
	}

	public void Dispose()
	{
		dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing && guiTreeNode != null)
			{
				UnityEngine.Object.Destroy(guiTreeNode);
				guiTreeNode = null;
			}
			disposed = true;
		}
	}

	public virtual bool FireMouseOver(GUIMouseEvent data)
	{
		if (this.MouseOver != null)
		{
			this.MouseOver(this, data);
		}
		return this.MouseOver != null;
	}

	public virtual bool FireMouseOut(GUIMouseEvent data)
	{
		if (this.MouseOut != null)
		{
			this.MouseOut(this, data);
		}
		return this.MouseOut != null;
	}

	public virtual bool FireMouseClick(GUIClickEvent data)
	{
		if (IsContentDependent && !IsContentLoaded)
		{
			if (OnLoadingActivate != null)
			{
				OnLoadingActivate(this);
				return true;
			}
			return false;
		}
		if (this.Click != null)
		{
			this.Click(this, data);
		}
		return this.Click != null;
	}

	public virtual bool FireMouseDown(GUIMouseEvent data)
	{
		if (this.MouseDown != null)
		{
			this.MouseDown(this, data);
		}
		return this.MouseDown != null || this.Click != null;
	}

	public virtual bool FireMouseUp(GUIMouseEvent data)
	{
		if (this.MouseUp != null)
		{
			this.MouseUp(this, data);
		}
		return this.MouseUp != null;
	}

	public virtual bool FireRightMouseUp(GUIMouseEvent data)
	{
		if (this.RightMouseUp != null)
		{
			this.RightMouseUp(this, data);
		}
		return this.RightMouseUp != null;
	}

	public virtual bool FireMouseWheel(GUIMouseWheelEvent data)
	{
		if (this.MouseWheel != null)
		{
			this.MouseWheel(this, data);
		}
		return this.MouseWheel != null;
	}

	public virtual bool FireRightClick(GUIMouseEvent data)
	{
		if (this.RightMouseDown != null)
		{
			this.RightMouseDown(this, data);
		}
		return this.RightMouseDown != null;
	}

	public bool HasAClickOrMouseDownEvent()
	{
		return this.MouseDown != null || this.Click != null;
	}

	public void SetMask(BitTexture Mask, int Width, int Height)
	{
		mask = Mask;
	}

	public void SetMask(Texture2D texture)
	{
		SetMask(texture, DEFAULT_MASK_SCALE);
	}

	public void SetMask(Texture2D texture, float scale)
	{
		mask = GUICommon.MaskFromTexture(texture, scale, DEFAULT_OPACITY_THRESHOLD);
	}

	public void BringToFront()
	{
		if (Parent != null)
		{
			GUIWindow gUIWindow = Parent as GUIWindow;
			if (gUIWindow != null)
			{
				gUIWindow.ControlToFront(this);
			}
		}
	}

	public void SendToBack()
	{
		if (Parent != null)
		{
			GUIWindow gUIWindow = Parent as GUIWindow;
			if (gUIWindow != null)
			{
				gUIWindow.ControlToBack(this);
			}
		}
	}

	public void CheckAndInitializeResourcesIfTime(ControlTraits.ResourceLoadingPhaseTraitEnum trait)
	{
		if (!resourcesInitialized && trait == Traits.ResourceLoadingPhaseTrait)
		{
			if (!InitailizeResourcesInProgress)
			{
				InitailizeResourcesInProgress = true;
				resourcesInitialized = InitializeResources(false);
				InitailizeResourcesInProgress = false;
			}
			else
			{
				CspUtils.DebugLog("Initialize Resource prevented from being called during an Initialize Resource");
			}
		}
	}

	public virtual bool InitializeResources(bool reload)
	{
		return !reload || (reload && resourcesInitialized);
	}

	public void EnsureResourcesInitialized()
	{
		if (!resourcesInitialized)
		{
			resourcesInitialized = InitializeResources(false);
		}
	}

	public static T CreateControlAbsolute<T>(Vector2 size, Vector2 offset) where T : GUIControl, new()
	{
		T result = new T();
		result.SetSize(size);
		result.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, offset);
		return result;
	}

	public static T CreateControlCenter<T>(Vector2 size, Vector2 offset) where T : GUIControl, new()
	{
		T result = new T();
		result.SetSize(size);
		result.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, offset);
		return result;
	}

	public static T CreateControlFrameCentered<T>(Vector2 size, Vector2 offset) where T : GUIControl, new()
	{
		T result = new T();
		result.SetSize(size);
		result.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, offset);
		return result;
	}

	public static T CreateControlTopFrame<T>(Vector2 size, Vector2 offset) where T : GUIControl, new()
	{
		T result = new T();
		result.SetSize(size);
		result.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, offset);
		return result;
	}

	public static T CreateControlTopFrameCentered<T>(Vector2 size, Vector2 offset) where T : GUIControl, new()
	{
		T result = new T();
		result.SetSize(size);
		result.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, offset);
		return result;
	}

	public static T CreateControlBottomFrame<T>(Vector2 size, Vector2 offset) where T : GUIControl, new()
	{
		T result = new T();
		result.SetSize(size);
		result.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, offset);
		return result;
	}

	public static T CreateControlBottomFrameCentered<T>(Vector2 size, Vector2 offset) where T : GUIControl, new()
	{
		T result = new T();
		result.SetSize(size);
		result.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, offset);
		return result;
	}

	public static T CreateControlBottomLeftFrameCentered<T>(Vector2 size, Vector2 offset) where T : GUIControl, new()
	{
		T result = new T();
		result.SetSize(size);
		result.SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, offset);
		return result;
	}

	public static T CreateControlBottomRightFrameCentered<T>(Vector2 size, Vector2 offset) where T : GUIControl, new()
	{
		T result = new T();
		result.SetSize(size);
		result.SetPosition(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, offset);
		return result;
	}

	public static T CreateControlTopLeftFrame<T>(Vector2 size, Vector2 offset) where T : GUIControl, new()
	{
		T result = new T();
		result.SetSize(size);
		result.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, offset);
		return result;
	}

	public static T CreateControlTopRightFrame<T>(Vector2 size, Vector2 offset) where T : GUIControl, new()
	{
		T result = new T();
		result.SetSize(size);
		result.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, offset);
		return result;
	}

	public static T CreateControlBottomRightFrame<T>(Vector2 size, Vector2 offset) where T : GUIControl, new()
	{
		T result = new T();
		result.SetSize(size);
		result.SetPosition(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, offset);
		return result;
	}

	public static T CreateControl<T>(Vector2 size, Vector2 offset, DockingAlignmentEnum Docking, AnchorAlignmentEnum Anchor) where T : GUIControl, new()
	{
		T result = new T();
		result.SetSize(size);
		result.SetPosition(Docking, Anchor, OffsetType.Absolute, offset);
		return result;
	}

	public virtual void DrawPreprocess()
	{
		CheckAndInitializeResourcesIfTime(ControlTraits.ResourceLoadingPhaseTraitEnum.Draw);
		guiEnabledState = GUI.enabled;
		ApplyRotation();
		cachedColor = GUI.color;
		GUI.color = new Color(color.r, color.g, color.b, alpha * animationAlpha);
		GUI.enabled = IsEnabled;
		if (Parent != null && Parent.LayoutType != 0)
		{
			Vector2 vector = Size;
			float width = vector.x + MarginLeft + MarginRight;
			Vector2 vector2 = Size;
			Rect rect = GUILayoutUtility.GetRect(width, vector2.y + MarginTop + MarginBottom);
			if (Event.current.type == EventType.Repaint)
			{
				SetLayoutRect(new Rect(rect.x + MarginLeft, rect.y + MarginTop, rect.width - MarginRight - MarginLeft, rect.height - MarginTop - MarginBottom));
			}
		}
		if (Event.current.type != EventType.Layout && Traits.EventHandlingTrait != ControlTraits.EventHandlingEnum.Ignore)
		{
			handleMouseInput();
			handleMouseHitTestOverState();
		}
	}

	public virtual void InitiateDraw(DrawModeSetting drawFlags)
	{
		if (IsContentDependent && !IsContentLoaded)
		{
			DrawContentLoading(drawFlags);
		}
		else
		{
			Draw(drawFlags);
		}
	}

	public virtual void DrawContentLoading(DrawModeSetting drawFlags)
	{
		if (DrawContentLoadingDefault != null)
		{
			DrawContentLoadingDefault(this, drawFlags);
		}
	}

	public virtual void Draw(DrawModeSetting drawFlags)
	{
	}

	public virtual void DrawFinalize()
	{
		GUI.color = cachedColor;
		UndoRotation();
		GUI.enabled = guiEnabledState;
	}

	public virtual void DebugDraw(BitArray DrawFlags)
	{
		if (!QueryHierarchyVisibleState() || !GUIManager.canDrawControl(this))
		{
			return;
		}
		if (DrawFlags[0])
		{
			cachedColor = GUI.color;
			GUI.color = debugColor;
			ApplyRotation();
			GUI.Box(ScreenRect, Id, GUIManager.Instance.StyleManager.GetStyle("DebugBoxBase").UnityStyle);
			UndoRotation();
			GUI.color = cachedColor;
		}
		if (DrawFlags[1])
		{
			cachedColor = GUI.color;
			GUI.color = debugColor;
			ApplyRotation();
			switch (HitTestType)
			{
			case HitTestTypeEnum.Alpha:
				printTheAlpha();
				break;
			case HitTestTypeEnum.Circular:
				printTheCircle();
				break;
			case HitTestTypeEnum.Rect:
				printTheRect();
				break;
			}
			UndoRotation();
			GUI.color = cachedColor;
		}
	}

	public void DebugDrawTheBlockTestControl()
	{
		switch (BlockTestType)
		{
		case BlockTestTypeEnum.Region:
			break;
		case BlockTestTypeEnum.Rect:
			printTheBlockTestRect();
			break;
		case BlockTestTypeEnum.Circular:
			printTheBlockTestCircle();
			break;
		}
	}

	private void printTheRect()
	{
		Vector2 vector = new Vector2(ScreenRect.x + ScreenRect.width * 0.5f, ScreenRect.y + ScreenRect.height * 0.5f);
		GUI.Box(new Rect(vector.x - ScreenRect.width * hitTestSize.x * 0.5f, vector.y - ScreenRect.height * hitTestSize.y * 0.5f, ScreenRect.width * hitTestSize.x, ScreenRect.height * hitTestSize.y), Id, GUIManager.Instance.StyleManager.GetStyle("DebugBoxBase").UnityStyle);
	}

	private void printTheCircle()
	{
		if (savedTexture == null)
		{
			savedTexture = GUIManager.Instance.LoadTexture("common_bundle|blankCircle");
		}
		Vector2 vector = new Vector2(ScreenRect.x + ScreenRect.width * 0.5f, ScreenRect.y + ScreenRect.height * 0.5f);
		GUI.DrawTexture(new Rect(vector.x - ScreenRect.width * hitTestSize.x * 0.5f, vector.y - ScreenRect.width * hitTestSize.x * 0.5f, ScreenRect.width * hitTestSize.x, ScreenRect.width * hitTestSize.x), savedTexture);
	}

	private void printTheBlockTestRect()
	{
		cachedColor = GUI.color;
		GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.6f);
		GUI.Box(ScreenRect, Id);
		GUI.color = cachedColor;
	}

	private void printTheBlockTestCircle()
	{
		cachedColor = GUI.color;
		GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.6f);
		if (savedTexture == null)
		{
			savedTexture = GUIManager.Instance.LoadTexture("common_bundle|blankCircle");
		}
		Vector2 vector = new Vector2(ScreenRect.x + ScreenRect.width * 0.5f, ScreenRect.y + ScreenRect.height * 0.5f);
		GUI.DrawTexture(new Rect(vector.x - ScreenRect.width * blockTestSize.x * 0.5f, vector.y - ScreenRect.width * blockTestSize.x * 0.5f, ScreenRect.width * blockTestSize.x, ScreenRect.width * blockTestSize.x), savedTexture);
		GUI.color = cachedColor;
	}

	private void printTheAlpha()
	{
		if (savedTexture == null)
		{
			savedTexture = getMask(this, 1f);
		}
		GUI.DrawTexture(ScreenRect, savedTexture);
	}

	private Texture getMask(GUIControl sourceButton, float scale)
	{
		if (Mask != null)
		{
			return GUICommon.TextureFromMask(Mask);
		}
		return GUICommon.TextureFromMask(GUICommon.MaskFromTexture(sourceButton.Style.UnityStyle.normal.background, scale, DEFAULT_OPACITY_THRESHOLD));
	}

	public virtual string getDebugTooltip()
	{
		string text = "GUI Control: ";
		string text2 = text;
		text = text2 + "x: " + position.x + " y: " + position.y + "   ";
		text2 = text;
		text = text2 + "Anchor: " + anchor.ToString() + " Docking: " + docking.ToString() + "   ";
		text2 = text;
		text = text2 + "id: " + id + " Style Name: " + styleName + "   ";
		return text + "tooltip type: " + toolTip.TooltipType;
	}

	public virtual void SetControlFlag(ControlFlagSetting Setting, bool On, bool SetChildren)
	{
		controlFlags.Set((int)Setting, On);
	}

	public virtual void SetControlMask(ControlFlagSetting Setting, bool On, bool SetChildren)
	{
		controlFlagMask.Set((int)Setting, On);
	}

	public virtual void RemoveControlFlag(ControlFlagSetting Setting, bool SetChildren)
	{
		BitArray bitArray = new BitArray(32);
		bitArray.Set((int)Setting, true);
		controlFlags.Xor(bitArray);
		bitArray = null;
	}

	public virtual bool GetControlFlag(ControlFlagSetting Setting)
	{
		return controlFlags[(int)Setting] && controlFlagMask[(int)Setting];
	}

	public virtual void SetActive()
	{
		if (!isActive)
		{
			if (Traits.EventListenerRegistrationTrait == ControlTraits.EventListenerRegistrationTraitEnum.Register)
			{
				SHSInput.RegisterListener(keyBanks[KeyInputState.Active]);
			}
			isActive = true;
			CheckAndInitializeResourcesIfTime(ControlTraits.ResourceLoadingPhaseTraitEnum.Active);
			OnActive();
			AppShell.Instance.EventMgr.Fire(this, new GUIDisplayChangeMessage(this, GUIDisplayChangeMessage.DisplayTypeEnum.Active));
		}
	}

	public virtual void SetInactive()
	{
		if (!isActive)
		{
			return;
		}
		if (Traits.EventListenerRegistrationTrait == ControlTraits.EventListenerRegistrationTraitEnum.Register)
		{
			SHSInput.UnregisterListener(this, KeyInputState.Active);
		}
		bool flag = cachedVisible;
		if (isVisible)
		{
			Hide();
			if (Traits.VisibilityTrait == ControlTraits.VisibilityTraitEnum.Cached)
			{
				cachedVisible = flag;
			}
		}
		isActive = false;
		if (isDownState)
		{
			GUIUtility.hotControl = 0;
			isDownState = false;
		}
		OnInactive();
		AppShell.Instance.EventMgr.Fire(this, new GUIDisplayChangeMessage(this, GUIDisplayChangeMessage.DisplayTypeEnum.Inactive));
	}

	public virtual void Show()
	{
		Show(ModalLevelEnum.None);
	}

	public virtual void Show(ModalLevelEnum modalLevel)
	{
		if (modalLevel != ModalLevelEnum.None)
		{
			GUIManager.Instance.SetModal(this, modalLevel);
		}
		IsVisible = true;
	}

	public virtual void Hide()
	{
		IsVisible = false;
	}

	public virtual void OnActive()
	{
	}

	public virtual void OnInactive()
	{
	}

	public virtual void OnShow()
	{
	}

	public virtual void OnHide()
	{
	}

	public virtual void OnUpdate()
	{
	}

	public virtual void OnAdded(IGUIContainer addedTo)
	{
		if (addedTo.ControlFlags[6])
		{
			SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		}
		SetControlFlag(ControlFlagSetting.IsModal, addedTo.ControlFlags[0], true);
		SetControlFlag(ControlFlagSetting.DisabledByModal, addedTo.ControlFlags[1], true);
		SetControlFlag(ControlFlagSetting.HitTestIgnore, addedTo.ControlFlags[4], false);
		SetControlFlag(ControlFlagSetting.ToolTipTestIgnore, addedTo.ControlFlags[5], false);
		if (addedTo.ControlFlags[6])
		{
			Alpha = addedTo.Alpha;
		}
		AnimationAlpha = addedTo.AnimationAlpha;
		if (addedTo.IsActive && Traits.ActivationTrait == ControlTraits.ActivationTraitEnum.Auto)
		{
			SetActive();
		}
		if (Traits.VisibilityTrait == ControlTraits.VisibilityTraitEnum.Inherit)
		{
			IsVisible = addedTo.IsVisible;
		}
		HandleResize(new GUIResizeMessage(HandleResizeSource.Control, rect, rect));
	}

	public virtual void OnRemoved(IGUIContainer removedFrom)
	{
	}

	public virtual void Update()
	{
		if (Traits.UpdateTrait == ControlTraits.UpdateTraitEnum.AlwaysUpdate || (Traits.UpdateTrait == ControlTraits.UpdateTraitEnum.ActiveAndEnabled && isActive && IsEnabled) || (Traits.UpdateTrait == ControlTraits.UpdateTraitEnum.ActiveIgnoreEnabled && isActive) || (Traits.UpdateTrait == ControlTraits.UpdateTraitEnum.VisibleAndEnabled && isVisible && IsEnabled) || (Traits.UpdateTrait == ControlTraits.UpdateTraitEnum.VisibleIgnoreEnabled && isVisible))
		{
			OnUpdate();
		}
	}

	public virtual void Clear()
	{
	}

	protected virtual void handleMouseHitTestOverState()
	{
		if (!GetControlFlag(ControlFlagSetting.HitTestIgnore) && HitTest(Event.current.mousePosition) && (parent == null || parent.HitTestBoundsCheck()))
		{
			GUIManager.Instance.RegisterGUIOver(this);
		}
	}

	protected virtual void handleMouseInput()
	{
		if ((SHSInput.BlockType & SHSInput.InputBlockType.CaptureMode) != 0)
		{
			SHSKeyCode sHSKeyCode = new SHSKeyCode();
			sHSKeyCode.source = this;
			sHSKeyCode.originOfRequest = this;
			if (!SHSInput.IsCaptureAllowingInput(sHSKeyCode))
			{
				return;
			}
		}
		if (((SHSInput.BlockType & SHSInput.InputBlockType.BlockUI) == 0 || InputRequestorType != SHSInput.InputRequestorType.UI) && !GetControlFlag(ControlFlagSetting.DragDropMode))
		{
			bool flag = HitTest(Event.current.mousePosition);
			if (IsEnabled && flag)
			{
				GUIManager.Instance.RegisterGUIElementForEvents(this, new GUIManager.GUIEventRegistrationInfo(this, delegate
				{
					hover = true;
					FireMouseOver(new GUIMouseEvent());
				}, delegate
				{
					hover = false;
					FireMouseOut(new GUIMouseEvent());
				}, delegate
				{
					isDownState = FireMouseDown(new GUIMouseEvent());
					GUIManager.Instance.RegisterGUIElementLeftClickEventResult(isDownState, Traits.EventHandlingTrait);
				}, delegate
				{
					isDownState = false;
					if (CanDrag)
					{
						AppShell.Instance.EventMgr.Fire(this, new GUIDragBeginMessage(new DragDropInfo(this)));
					}
					FireMouseClick(new GUIClickEvent());
					FireMouseUp(new GUIMouseEvent());
				}, delegate
				{
					isRightDownState = FireRightClick(new GUIMouseEvent());
					GUIManager.Instance.RegisterGUIElementRightClickEventResult(isRightDownState, Traits.EventHandlingTrait);
				}, delegate
				{
					isRightDownState = false;
					FireRightMouseUp(new GUIMouseEvent());
				}, delegate(IGUIControl control, float delta, int direction)
				{
					FireMouseWheel(new GUIMouseWheelEvent(delta, direction));
				}));
			}
		}
	}

	public virtual bool HitTest(Vector2 point)
	{
		if (!Rect.Contains(point) || (parent != null && !parent.HitTestBoundsCheck()))
		{
			return false;
		}
		if (Traits.HitTestType == HitTestTypeEnum.Rect)
		{
			Vector2 vector = new Vector2(Rect.xMin + Rect.width * 0.5f, Rect.yMin + Rect.height * 0.5f);
			Rect rect = new Rect(vector.x - Rect.width * hitTestSize.x * 0.5f, vector.y - Rect.height * hitTestSize.y * 0.5f, Rect.width * hitTestSize.x, Rect.height * hitTestSize.y);
			if (rect.Contains(point))
			{
				return true;
			}
		}
		else if (Traits.HitTestType == HitTestTypeEnum.Circular)
		{
			Circle circle = new Circle(new Vector2(Rect.xMin + Rect.width * 0.5f, Rect.yMin + Rect.height * 0.5f), Rect.width * hitTestSize.x * 0.5f);
			if (circle.Contains(point))
			{
				return true;
			}
		}
		else if (Traits.HitTestType == HitTestTypeEnum.Region)
		{
			Polygon polygon = new Polygon(new Vector2[4]
			{
				new Vector2(Rect.x, Rect.y),
				new Vector2(Rect.x + Rect.width, Rect.y),
				new Vector2(Rect.x + Rect.width, Rect.y + Rect.height),
				new Vector2(Rect.x, Rect.y + Rect.height)
			});
			if (polygon.Contains(point))
			{
				return true;
			}
		}
		else if (Traits.HitTestType == HitTestTypeEnum.Alpha)
		{
			if (mask == null)
			{
				CspUtils.DebugLog("Control: " + Id + " specifies alpha testing, but hasn't provided a mask...");
				return false;
			}
			float num = Mathf.Clamp01((point.x - this.rect.x) / Rect.width);
			float num2 = Mathf.Clamp01((point.y - this.rect.y) / Rect.height);
			int num3 = Convert.ToInt32((float)mask.Width * num);
			int num4 = Convert.ToInt32((float)mask.Height * num2);
			if (mask.Bits.Length <= num3 + num4 * mask.Width)
			{
				return false;
			}
			if (mask.Bits[num3 + num4 * mask.Width])
			{
				return true;
			}
		}
		return false;
	}

	private void drawLogMask(int x, int y)
	{
		string text = string.Empty;
		for (int i = 0; i < mask.Height; i++)
		{
			for (int j = 0; j < mask.Width; j++)
			{
				text = ((x != j || y != i) ? (text + ((!mask.Bits[Convert.ToInt32(j) + Convert.ToInt32(i * mask.Width)]) ? "0" : "1")) : (text + "X"));
			}
			text += Environment.NewLine;
		}
		CspUtils.DebugLog(text);
	}

	public virtual bool BlockTest(Vector2 point)
	{
		return BlockTest(point, ScreenRect, BlockTestType);
	}

	public virtual bool BlockTest(Vector2 point, Rect Rect, BlockTestTypeEnum blockTestType)
	{
		switch (blockTestType)
		{
		case BlockTestTypeEnum.Rect:
		{
			Vector2 vector = new Vector2(Rect.xMin + Rect.width * 0.5f, Rect.yMin + Rect.height * 0.5f);
			if (new Rect(vector.x - Rect.width * blockTestSize.x * 0.5f, vector.y - Rect.height * blockTestSize.y * 0.5f, Rect.width * blockTestSize.x, Rect.height * blockTestSize.y).Contains(point))
			{
				return true;
			}
			break;
		}
		case BlockTestTypeEnum.Circular:
			if (new Circle(new Vector2(Rect.xMin + Rect.width * 0.5f, Rect.yMin + Rect.height * 0.5f), Rect.width * blockTestSize.x * 0.5f).Contains(point))
			{
				return true;
			}
			break;
		case BlockTestTypeEnum.Region:
		{
			Polygon polygon = new Polygon(new Vector2[4]
			{
				new Vector2(Rect.x, Rect.y),
				new Vector2(Rect.x + Rect.width, Rect.y),
				new Vector2(Rect.x + Rect.width, Rect.y + Rect.height),
				new Vector2(Rect.x, Rect.y + Rect.height)
			});
			if (polygon.Contains(point))
			{
				return true;
			}
			break;
		}
		}
		return false;
	}

	public virtual Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> GetKeyList(KeyInputState inputState)
	{
		KeyBank keyBank = keyBanks[inputState];
		return keyBank.KeyEventDictionary;
	}

	public virtual void ConfigureKeyBanks()
	{
	}

	public virtual void OnCaptureAcquired()
	{
	}

	public virtual void OnCaptureUnacquired()
	{
	}

	public virtual bool IsDescendantHandler(IInputHandler handler)
	{
		return this == handler;
	}

	public virtual void ConfigureRequiredContent(List<ContentReference> ContentReferenceList)
	{
		contentReferenceList = ContentReferenceList;
		isContentConfigured = true;
		isContentDependent = true;
	}

	public virtual void ConfigureRequiredContent(ContentReference ContentReference)
	{
		List<ContentReference> list = new List<ContentReference>();
		list.Add(ContentReference);
		ConfigureRequiredContent(list);
	}

	public virtual void ConfigureRequiredContent()
	{
		isContentConfigured = true;
	}

	private Rect GetDefaultContentLoadingRect()
	{
		float num = Mathf.Min(rect.width * hitTestSize.x, rect.height * hitTestSize.y) * 1.1f;
		Vector2 vector = new Vector2(rect.width - num, rect.height - num);
		return new Rect(rect.x + vector.x * 0.5f, rect.y + vector.y * 0.5f, num, num);
	}

	public virtual void SetLayoutRect(Rect r)
	{
		rect = r;
	}

	private void ApplyRotation()
	{
		if (!Mathf.Approximately(Rotation, 0f))
		{
			cachedRotation = GUI.matrix;
			GUI.matrix = Matrix4x4.identity;
			GUIUtility.RotateAroundPivot(Rotation, RotationPoint);
			GUI.matrix = cachedRotation * GUI.matrix;
		}
	}

	private void UndoRotation()
	{
		if (!Mathf.Approximately(Rotation, 0f))
		{
			GUI.matrix = cachedRotation;
		}
	}

	public virtual void EnsureHierarchyVisibleState()
	{
		if (Parent != null && !Parent.IsVisible)
		{
			Parent.Show();
		}
	}

	public virtual bool QueryHierarchyVisibleState()
	{
		return (Parent == null) ? isVisible : (isVisible && Parent.QueryHierarchyVisibleState());
	}

	public virtual void SetVisible(bool visible)
	{
		if (GUIManager.Instance.CurrentState == GUIManager.ModalStateEnum.Transition)
		{
			SetVisible(visible, SetVisibleReason.Transition);
		}
		else
		{
			SetVisible(visible, SetVisibleReason.Normal);
		}
	}

	public virtual bool GetVisible()
	{
		return isVisible && !GetControlFlag(ControlFlagSetting.ForceInvisible);
	}

	public virtual void SetVisible(bool visible, SetVisibleReason reason)
	{
		if (visible == isVisible)
		{
			return;
		}
		if (visible)
		{
			if (!IsActive)
			{
				SetActive();
			}
			bool flag = isVisible;
			if (Traits.VisibleAncestryTrait == ControlTraits.VisibilityAncestryTraitEnum.EnsureAncestorsVisible)
			{
				EnsureHierarchyVisibleState();
			}
			if (isVisible == flag)
			{
				isVisible = true;
				if (BlockTestType != BlockTestTypeEnum.Transparent)
				{
					GUIManager.Instance.AddBlockTestControl(this);
				}
				if (Traits.EventListenerRegistrationTrait == ControlTraits.EventListenerRegistrationTraitEnum.Register)
				{
					SHSInput.RegisterListener(keyBanks[KeyInputState.Visible]);
				}
				CheckAndInitializeResourcesIfTime(ControlTraits.ResourceLoadingPhaseTraitEnum.Show);
				OnShow();
				if (this.OnVisible != null)
				{
					this.OnVisible(this);
				}
				AppShell.Instance.EventMgr.Fire(this, new GUIDisplayChangeMessage(this, GUIDisplayChangeMessage.DisplayTypeEnum.Visible));
			}
		}
		else
		{
			if (GetControlFlag(ControlFlagSetting.IsModal))
			{
				if (GUIManager.Instance.IsInTheModalList(this))
				{
					GUIManager.Instance.SetModal(this, ModalLevelEnum.None);
				}
				else if (!GUIManager.Instance.IsParentInTheModalList(this))
				{
					CspUtils.DebugLog("Control: " + Id + " has its modal flag set, but it, or its parents are not in the modal list.  THIS IS BAD!");
				}
			}
			isVisible = false;
			if (BlockTestType != BlockTestTypeEnum.Transparent)
			{
				GUIManager.Instance.RemoveBlockTestControl(this);
			}
			if (Traits.EventListenerRegistrationTrait == ControlTraits.EventListenerRegistrationTraitEnum.Register)
			{
				SHSInput.UnregisterListener(this, KeyInputState.Visible);
			}
			if (Traits.DeactivationTrait == ControlTraits.DeactivationTraitEnum.DeactivateOnHide)
			{
				SetInactive();
			}
			OnHide();
			if (this.OnHidden != null)
			{
				this.OnHidden(this);
			}
			AppShell.Instance.EventMgr.Fire(this, new GUIDisplayChangeMessage(this, GUIDisplayChangeMessage.DisplayTypeEnum.Invisible));
			if (Traits.LifeSpan == ControlTraits.LifeSpanTraitEnum.DestroyOnHide)
			{
				Parent.InternalDeferredRemove(this);
			}
		}
		cachedVisible = isVisible;
		if (GUIManager.Instance.TooltipManager != null)
		{
			GUIManager.Instance.TooltipManager.Update();
		}
		ReflectToInspector();
	}

	protected void AlphaOutIfDisabled()
	{
		if (Traits.RespectDisabledAlphaTrait == ControlTraits.RespectDisabledAlphaTraitEnum.RespectDisabledAlpha && !IsEnabled && color.a > 0f)
		{
			GUI.color = new Color(color.r, color.g, color.b, DisabledAlpha);
		}
	}

	public string GetPath()
	{
		string text = GetType().ToString();
		IGUIContainer iGUIContainer = Parent;
		while (iGUIContainer != null && !iGUIContainer.IsRoot)
		{
			text = iGUIContainer.Id + '/' + text;
			iGUIContainer = iGUIContainer.Parent;
		}
		return text;
	}

	public virtual CaptureHandlerResponse HandleCapture(SHSKeyCode code)
	{
		return (code.source == this) ? CaptureHandlerResponse.Passthrough : CaptureHandlerResponse.Block;
	}

	public virtual void HandleResize(GUIResizeMessage message)
	{
		if (position != POSITION_UNDEFINED)
		{
			CalculateRect();
			ReflectToInspector();
		}
		if (this.Resize != null)
		{
			this.Resize(this, message);
		}
	}

	public virtual bool CanDrop(DragDropInfo DragDropInfo)
	{
		return Parent != null && Parent.CanDrop(DragDropInfo);
	}

	public virtual void OnDragBegin(DragDropInfo DragBeginInfo)
	{
		if (Parent != null)
		{
			Parent.OnDragBegin(DragBeginInfo);
		}
	}

	public virtual void OnDragOver(DragDropInfo DragOverInfo)
	{
		if (Parent != null)
		{
			Parent.OnDragOver(DragOverInfo);
		}
	}

	public virtual void OnDragEnd(DragDropInfo DragEndInfo)
	{
		if (Parent != null)
		{
			Parent.OnDragEnd(DragEndInfo);
		}
	}

	public virtual void SetDragInfo(DragDropInfo DragDropInfo)
	{
	}

	public void SetPosition(QuickSizingHint hint)
	{
		if (hint == QuickSizingHint.Centered)
		{
			SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
		}
		else
		{
			CspUtils.DebugLog("Requested to set position to: " + hint.ToString() + ", which is not supported for this quick sizing hint");
		}
	}

	public void SetPositionAndSize(QuickSizingHint hint)
	{
		switch (hint)
		{
		case QuickSizingHint.ParentSize:
			SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, Vector2.zero, new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
			break;
		case QuickSizingHint.NoSize:
			SetPositionAndSize(0f, 0f, 0f, 0f);
			break;
		case QuickSizingHint.ScreenSize:
			SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, Vector2.zero, new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
			break;
		default:
			CspUtils.DebugLog("Requested to set position and size to: " + hint.ToString() + ", which is not supported.");
			break;
		}
	}

	public void SetPositionAndSize(float x, float y, float width, float height)
	{
		SetPositionAndSize(new Vector2(x, y), new Vector2(width, height));
	}

	public void SetPositionAndSize(Vector2 Position, Vector2 Size)
	{
		SetPositionAndSize(Position, DockingAlignmentEnum.None, AnchorAlignmentEnum.None, OffsetType.Absolute, Vector2.zero, Size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
	}

	public virtual void SetPositionAndSize(DockingAlignmentEnum Docking, AnchorAlignmentEnum Anchor, OffsetType OffsetType, Vector2 Offset, Vector2 Size, AutoSizeTypeEnum HorizontalSizeHint, AutoSizeTypeEnum VerticalSizeHint)
	{
		setSize(Size, HorizontalSizeHint, VerticalSizeHint);
		setPosition(Vector2.zero, Docking, Anchor, OffsetType, Offset);
	}

	public void SetPositionAndSize(DockingAlignmentEnum Docking, AnchorAlignmentEnum Anchor, Vector2 Size)
	{
		setSize(Size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		setPosition(Vector2.zero, Docking, Anchor, OffsetType.Absolute, Vector2.zero);
	}

	public void SetPositionAndSize(AnchorAlignmentEnum Anchor, Vector2 Position, Vector2 Size)
	{
		setSize(Size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		setPosition(Position, DockingAlignmentEnum.None, Anchor, OffsetType.Absolute, Vector2.zero);
	}

	public void SetPositionAndSize(Vector2 Position, DockingAlignmentEnum Docking, AnchorAlignmentEnum Anchor, OffsetType OffsetType, Vector2 Offset, Vector2 Size, AutoSizeTypeEnum HorizontalSizeHint, AutoSizeTypeEnum VerticalSizeHint)
	{
		setSize(Size, HorizontalSizeHint, VerticalSizeHint);
		setPosition(Position, Docking, Anchor, OffsetType, Offset);
	}

	public void SetPosition(DockingAlignmentEnum Docking, AnchorAlignmentEnum Anchor)
	{
		setPosition(Vector2.zero, Docking, Anchor, OffsetType.Absolute, Vector2.zero);
	}

	public void SetPosition(DockingAlignmentEnum Docking, AnchorAlignmentEnum Anchor, OffsetType OffsetType, Vector2 Offset)
	{
		setPosition(Vector2.zero, Docking, Anchor, OffsetType, Offset);
	}

	public void SetPosition(DockingAlignmentEnum Docking, AnchorAlignmentEnum Anchor, Vector2 Offset)
	{
		setPosition(Vector2.zero, Docking, Anchor, OffsetType.Absolute, Offset);
	}

	public void SetPosition(Vector2 Position, DockingAlignmentEnum Docking, AnchorAlignmentEnum Anchor, OffsetType OffsetType, Vector2 Offset)
	{
		setPosition(Position, Docking, Anchor, OffsetType, Offset);
	}

	public void SetPosition(Vector2 Position)
	{
		setPosition(Position, DockingAlignmentEnum.None, AnchorAlignmentEnum.None, OffsetType.Absolute, Vector2.zero);
	}

	public void SetPosition(float x, float y)
	{
		setPosition(new Vector2(x, y), DockingAlignmentEnum.None, AnchorAlignmentEnum.None, OffsetType.Absolute, Vector2.zero);
	}

	public void SetPosition(float x, float y, AnchorAlignmentEnum anchor)
	{
		setPosition(new Vector2(x, y), DockingAlignmentEnum.None, anchor, OffsetType.Absolute, Vector2.zero);
	}

	private void setPosition(Vector2 Position, DockingAlignmentEnum Docking, AnchorAlignmentEnum Anchor, OffsetType OffsetType, Vector2 Offset)
	{
		position = Position;
		docking = Docking;
		anchor = Anchor;
		offsetType = OffsetType;
		offset = Offset;
		directRectMode = false;
		autoPosition = (Docking != DockingAlignmentEnum.None || Anchor != AnchorAlignmentEnum.None);
		HandleResize(new GUIResizeMessage(HandleResizeSource.Control, rect, rect));
	}

	protected void CalculateRect()
	{
		if (directRectMode)
		{
			return;
		}
		if (!autoPosition && !autoSize)
		{
			rect = new Rect(position.x, position.y, size.x, size.y);
		}
		else if (parent != null)
		{
			setAutoSize();
			setAutoPosition();
			if (Docking != DockingAlignmentEnum.None)
			{
				position = new Vector2(rect.x, rect.y);
			}
			centerPoint = GUICommon.CenterPoint(rect);
		}
	}

	private void setAutoPosition()
	{
		float num = 0f;
		float num2 = 0f;
		num = ((Docking != 0 && Docking != DockingAlignmentEnum.MiddleLeft && Docking != DockingAlignmentEnum.BottomLeft) ? ((Docking != DockingAlignmentEnum.Middle && Docking != DockingAlignmentEnum.TopMiddle && Docking != DockingAlignmentEnum.BottomMiddle) ? ((Docking != DockingAlignmentEnum.TopRight && Docking != DockingAlignmentEnum.MiddleRight && Docking != DockingAlignmentEnum.BottomRight) ? position.x : Parent.ClientRect.width) : (Parent.ClientRect.x + Parent.ClientRect.width / 2f)) : Parent.ClientRect.x);
		num2 = ((Docking != 0 && Docking != DockingAlignmentEnum.TopMiddle && Docking != DockingAlignmentEnum.TopRight) ? ((Docking != DockingAlignmentEnum.Middle && Docking != DockingAlignmentEnum.MiddleLeft && Docking != DockingAlignmentEnum.MiddleRight) ? ((Docking != DockingAlignmentEnum.BottomRight && Docking != DockingAlignmentEnum.BottomMiddle && Docking != DockingAlignmentEnum.BottomLeft) ? position.y : Parent.ClientRect.height) : (Parent.ClientRect.y + Parent.ClientRect.height / 2f)) : Parent.ClientRect.y);
		float num3 = num;
		float num4;
		if (Anchor == AnchorAlignmentEnum.TopLeft || Anchor == AnchorAlignmentEnum.MiddleLeft || Anchor == AnchorAlignmentEnum.BottomLeft)
		{
			num4 = 0f;
		}
		else if (Anchor == AnchorAlignmentEnum.Middle || Anchor == AnchorAlignmentEnum.TopMiddle || Anchor == AnchorAlignmentEnum.BottomMiddle)
		{
			Vector2 rectSize = RectSize;
			num4 = rectSize.x / 2f;
		}
		else if (Anchor == AnchorAlignmentEnum.TopRight || Anchor == AnchorAlignmentEnum.MiddleRight || Anchor == AnchorAlignmentEnum.BottomRight)
		{
			Vector2 rectSize2 = RectSize;
			num4 = rectSize2.x;
		}
		else
		{
			num4 = 0f;
		}
		num = num3 - num4;
		float num5 = num2;
		float num6;
		if (Anchor == AnchorAlignmentEnum.TopLeft || Anchor == AnchorAlignmentEnum.TopMiddle || Anchor == AnchorAlignmentEnum.TopRight)
		{
			num6 = 0f;
		}
		else if (Anchor == AnchorAlignmentEnum.Middle || Anchor == AnchorAlignmentEnum.MiddleLeft || Anchor == AnchorAlignmentEnum.MiddleRight)
		{
			Vector2 rectSize3 = RectSize;
			num6 = rectSize3.y / 2f;
		}
		else if (Anchor == AnchorAlignmentEnum.BottomRight || Anchor == AnchorAlignmentEnum.BottomMiddle || Anchor == AnchorAlignmentEnum.BottomLeft)
		{
			Vector2 rectSize4 = RectSize;
			num6 = rectSize4.y;
		}
		else
		{
			num6 = 0f;
		}
		num2 = num5 - num6;
		if (OffsetStyle == OffsetType.Percentage)
		{
			num += Parent.ClientRect.width * Offset[0];
			num2 += Parent.ClientRect.height * Offset[1];
		}
		else if (OffsetStyle == OffsetType.Absolute)
		{
			num += Offset[0];
			num2 += Offset[1];
		}
		rect = new Rect(num, num2, rect.width, rect.height);
	}

	public void SetSize(Vector2 Size, AutoSizeTypeEnum HorizontalSizeHint, AutoSizeTypeEnum VerticalSizeHint)
	{
		setSize(Size, HorizontalSizeHint, VerticalSizeHint);
		CalculateRect();
	}

	public virtual void SetSize(float width, float height)
	{
		setSize(new Vector2(width, height), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		CalculateRect();
	}

	public virtual void SetSize(Vector2 Size)
	{
		setSize(Size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		CalculateRect();
	}

	private void setSize(Vector2 Size, AutoSizeTypeEnum HorizontalSizeHint, AutoSizeTypeEnum VerticalSizeHint)
	{
		directRectMode = false;
		size = Size;
		horizontalSizeHint = HorizontalSizeHint;
		verticalSizeHint = VerticalSizeHint;
		autoSize = (verticalSizeHint != 0 || horizontalSizeHint != AutoSizeTypeEnum.Absolute);
		HandleResize(new GUIResizeMessage(HandleResizeSource.Control, rect, rect));
	}

	private void setAutoSize()
	{
		float num = size[0];
		float num2 = size[1];
		switch (horizontalSizeHint)
		{
		case AutoSizeTypeEnum.Percentage:
			num = parent.ClientRect.width * num;
			break;
		case AutoSizeTypeEnum.Proportional:
			CspUtils.DebugLog("Proportional sizing NYI.");
			break;
		default:
			CspUtils.DebugLog(horizontalSizeHint + " is unknown.");
			break;
		case AutoSizeTypeEnum.Absolute:
			break;
		}
		switch (verticalSizeHint)
		{
		case AutoSizeTypeEnum.Percentage:
			num2 = parent.ClientRect.height * num2;
			break;
		case AutoSizeTypeEnum.Proportional:
			CspUtils.DebugLog("Proportional sizing NYI.");
			break;
		default:
			CspUtils.DebugLog(verticalSizeHint + " is unknown.");
			break;
		case AutoSizeTypeEnum.Absolute:
			break;
		}
		rect = new Rect(rect.x, rect.y, num * scale, num2 * scale);
	}

	public virtual void OnSceneEnter(AppShell.GameControllerTypeData currentGameData)
	{
	}

	public virtual void OnSceneLoaded(AppShell.GameControllerTypeData currentGameData)
	{
	}

	public virtual void OnSceneLeave(AppShell.GameControllerTypeData lastGameData, AppShell.GameControllerTypeData currentGameData)
	{
	}

	public virtual void OnLocaleChange(string newLocale)
	{
		resourcesInitialized = InitializeResources(true);
	}
}
