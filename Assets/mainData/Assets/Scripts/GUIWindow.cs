using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIWindow : GUIControl, IGUIContainer, IGUIControl, IGUIDragDrop, IGUIDrawable, IGUIResizable, ICaptureHandler
{
	public enum LayoutTypeEnum
	{
		Fixed,
		Flow
	}

	public enum SupportingAssetBundleInfoTypeEnum
	{
		BlockUntilLoaded,
		LoadBeforeShow,
		OptionalLoad
	}

	public class SupportingAssetBundleInfo
	{
		public readonly string BundleName;

		public readonly SupportingAssetBundleInfoTypeEnum LoadType;

		public SupportingAssetBundleInfo(string BundleName, SupportingAssetBundleInfoTypeEnum LoadType)
		{
			this.BundleName = BundleName;
			this.LoadType = LoadType;
		}
	}

	protected class BundleList : List<SupportingAssetBundleInfo>
	{
		public new void Add(SupportingAssetBundleInfo bundle)
		{
			if (bundle.LoadType != 0)
			{
				throw new NotImplementedException();
			}
			base.Add(bundle);
		}
	}

	private struct controlAddDeferredStruct
	{
		public IGUIControl Control;

		public DrawPhaseHintEnum DrawPhaseHint;

		public int Index;

		public controlAddDeferredStruct(IGUIControl control, int index, DrawPhaseHintEnum drawPhaseHint)
		{
			Control = control;
			Index = index;
			DrawPhaseHint = drawPhaseHint;
		}
	}

	public enum VisibilityState
	{
		Visible,
		Invisible,
		Showing,
		Hiding
	}

	private enum QuitOrContinueTheIsVisibleCall
	{
		QuitOutOf,
		ContinueOn
	}

	private enum AnimationProgress
	{
		AnimationOpenInProgress,
		AnimationCloseInProgress,
		Idle,
		QueuedOpenReady,
		QueuedCloseReady
	}

	private enum QueuedOpenClose
	{
		QueuedOpen,
		QueuedClose,
		None
	}

	private class FullScreenFadeoutImage : GUISimpleControlWindow
	{
		private class FadeoutAnimations : SHSAnimations
		{
			public static AnimationPieceGeneratorDelegate OpenFadeIn(GUIControl ctrl, float fadeTime)
			{
				return delegate
				{
					return Absolute.Alpha(Path.Linear(0f, 1f, fadeTime), ctrl);
				};
			}

			public static AnimationPieceGeneratorDelegate CloseFadeOut(GUIControl ctrl, float fadeTime)
			{
				return delegate
				{
					return Absolute.Alpha(Path.Linear(1f, 0f, fadeTime), ctrl);
				};
			}
		}

		private GUIImage backdrop;

		private string textureSource;

		private float fadeTime;

		public FullScreenFadeoutImage(string textureSource)
			: this((!(textureSource == string.Empty)) ? textureSource : "persistent_bundle|loading_bg_bluecircles", 0.3f)
		{
		}

		public FullScreenFadeoutImage()
			: this("persistent_bundle|loading_bg_bluecircles", 0.3f)
		{
		}

		public FullScreenFadeoutImage(string textureSource, float fadeTime)
		{
			this.textureSource = textureSource;
			this.fadeTime = fadeTime;
		}

		public override bool InitializeResources(bool reload)
		{
			if (reload)
			{
				return base.InitializeResources(reload);
			}
			SetPosition(QuickSizingHint.Centered);
			SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
			Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			Traits.HitTestType = HitTestTypeEnum.Rect;
			Traits.BlockTestType = BlockTestTypeEnum.Rect;
			Traits.LifeSpan = ControlTraits.LifeSpanTraitEnum.DestroyOnHide;
			backdrop = new GUIImage();
			backdrop.SetPosition(QuickSizingHint.Centered);
			backdrop.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
			backdrop.TextureSource = textureSource;
			backdrop.Traits.RespectDisabledAlphaTrait = ControlTraits.RespectDisabledAlphaTraitEnum.DisrespectDisabledAlpha;
			Add(backdrop);
			base.AnimationOnOpen = FadeoutAnimations.OpenFadeIn(backdrop, fadeTime);
			base.AnimationOnClose = FadeoutAnimations.CloseFadeOut(backdrop, fadeTime);
			return base.InitializeResources(reload);
		}
	}

	public delegate AnimClip AnimationPieceGeneratorDelegate();

	public delegate void AnimationFinishDelegate();

	private List<IGUIControl> delayedRemoveList = new List<IGUIControl>();

	private bool deferredRemoveValid;

	protected BundleList supportingAssetBundles = new BundleList();

	private Queue<IGUIControl> deferredRemoveQueue = new Queue<IGUIControl>();

	private Queue<controlAddDeferredStruct> deferredAddQueue = new Queue<controlAddDeferredStruct>();

	protected LayoutTypeEnum layoutType;

	private AnimClipManager animationPieceManager;

	private AnimClipManager windowAnimationPieceManager;

	protected List<IGUIControl> controlList = new List<IGUIControl>();

	private bool skinInitialized;

	private SHSSkin skin;

	private SHSSkin cachedSkin;

	private GUIFontInfo fontInfo;

	private AudioManager.MuteContext currentMutingContext;

	private bool fullScreenBackgroundUp;

	private AnimationPieceGeneratorDelegate animationOnOpen;

	private AnimationPieceGeneratorDelegate animationOnClose;

	private bool disableAutoOpenAndCloseAnimations;

	private AnimationProgress animationProgressState = AnimationProgress.Idle;

	private QueuedOpenClose QuededOpenCloseState = QueuedOpenClose.None;

	private bool SkipAnimation;

	protected bool clippingEnabled = true;

	private Color cachedcolor;

	protected Color backgroundcolor;

	protected bool backgroundEnabled;

	private int fullScreenFadeoutVisibilityCounter;

	private FullScreenFadeoutImage fadeoutImage;

	protected string fullScreenBackgroundTextureSource = string.Empty;

	private Branch graphNode;

	public LayoutTypeEnum LayoutType
	{
		get
		{
			return layoutType;
		}
	}

	public AnimClipManager AnimationPieceManager
	{
		get
		{
			if (animationPieceManager == null)
			{
				animationPieceManager = new AnimClipManager();
				animationPieceManager.Name = GetType().ToString();
			}
			return animationPieceManager;
		}
	}

	private AnimClipManager WindowAnimationPieceManager
	{
		get
		{
			if (windowAnimationPieceManager == null)
			{
				windowAnimationPieceManager = new AnimClipManager();
				windowAnimationPieceManager.Name = GetType().ToString() + "anim open/close";
			}
			return windowAnimationPieceManager;
		}
	}

	public SHSSkin Skin
	{
		get
		{
			if (skin != null)
			{
				return skin;
			}
			if (parent != null)
			{
				return parent.Skin;
			}
			return null;
		}
		set
		{
			skin = value;
		}
	}

	public GUIFontInfo FontInfo
	{
		get
		{
			return fontInfo;
		}
		set
		{
			fontInfo = value;
		}
	}

	public override float Alpha
	{
		get
		{
			return base.Alpha;
		}
		set
		{
			base.Alpha = value;
			if (GetControlFlag(ControlFlagSetting.AlphaCascade))
			{
				foreach (IGUIControl control in controlList)
				{
					control.Alpha = value;
				}
			}
		}
	}

	public override float AnimationAlpha
	{
		get
		{
			return base.AnimationAlpha;
		}
		set
		{
			base.AnimationAlpha = value;
			foreach (IGUIControl control in controlList)
			{
				control.AnimationAlpha = value;
			}
		}
	}

	public VisibilityState CurrentVisibilityState
	{
		get
		{
			if (animationProgressState == AnimationProgress.AnimationCloseInProgress)
			{
				return VisibilityState.Hiding;
			}
			if (animationProgressState == AnimationProgress.AnimationOpenInProgress)
			{
				return VisibilityState.Showing;
			}
			if (IsVisible)
			{
				return VisibilityState.Visible;
			}
			return VisibilityState.Invisible;
		}
	}

	public AnimationPieceGeneratorDelegate AnimationOnOpen
	{
		get
		{
			return animationOnOpen;
		}
		set
		{
			animationOnOpen = value;
		}
	}

	public AnimationPieceGeneratorDelegate AnimationOnClose
	{
		get
		{
			return animationOnClose;
		}
		set
		{
			animationOnClose = value;
		}
	}

	public bool AnimationInProgress
	{
		get
		{
			return animationProgressState != AnimationProgress.Idle;
		}
	}

	public bool DisableAutoOpenAndCloseAnimations
	{
		get
		{
			return disableAutoOpenAndCloseAnimations;
		}
		set
		{
			disableAutoOpenAndCloseAnimations = value;
		}
	}

	public bool ClippingEnabled
	{
		get
		{
			return clippingEnabled;
		}
		set
		{
			clippingEnabled = value;
		}
	}

	public Color BackgroundColor
	{
		get
		{
			return backgroundcolor;
		}
	}

	public bool BackgroundEnabled
	{
		get
		{
			return backgroundEnabled;
		}
		set
		{
			backgroundEnabled = value;
		}
	}

	public string FullScreenBackgroundTextureSource
	{
		get
		{
			return fullScreenBackgroundTextureSource;
		}
		set
		{
			fullScreenBackgroundTextureSource = value;
		}
	}

	public virtual bool IsRoot
	{
		get
		{
			return false;
		}
	}

	public List<IGUIContainer> ContainerList
	{
		get
		{
			List<IGUIContainer> list = new List<IGUIContainer>();
			foreach (IGUIControl control in controlList)
			{
				if (control is IGUIContainer)
				{
					list.Add((IGUIContainer)control);
				}
			}
			return list;
		}
	}

	public virtual List<IGUIControl> ControlList
	{
		get
		{
			return controlList;
		}
	}

	public int Level
	{
		get
		{
			int num = 0;
			for (IGUIContainer parent = Parent; parent != null; parent = parent.Parent)
			{
				num++;
			}
			return num;
		}
	}

	public Branch GraphNode
	{
		get
		{
			return graphNode;
		}
		set
		{
			graphNode = value;
		}
	}

	public override IGUIContainer Parent
	{
		get
		{
			return parent;
		}
		set
		{
			base.Parent = value;
		}
	}

	public IGUIControl this[string key]
	{
		get
		{
			IGUIContainer iGUIContainer = this;
			IGUIControl iGUIControl = null;
			string[] tokens = GetTokenizedPath(key);
			if (tokens == null)
			{
				CspUtils.DebugLog("Can't find: " + key + " when searching.");
				return null;
			}
			for (int i = 0; i < tokens.Length; i++)
			{
				iGUIControl = iGUIContainer.ControlList.Find(delegate(IGUIControl c)
				{
					return c.Id == tokens[i];
				});
				if (!(iGUIControl is IGUIControl))
				{
					return null;
				}
				if (iGUIControl is IGUIContainer)
				{
					iGUIContainer = (IGUIContainer)iGUIControl;
					continue;
				}
				if (i != tokens.Length - 1)
				{
					CspUtils.DebugLog("Found: " + iGUIControl.ToString() + " in search of " + key + ", but a control doesnt have children. Aborting...");
					return null;
				}
				return iGUIControl;
			}
			return iGUIContainer;
		}
	}

	public event AnimationFinishDelegate AnimationOpenFinished;

	public event AnimationFinishDelegate AnimationCloseFinished;

	public GUIWindow()
		: base(ControlTraits.Default)
	{
		InitializeBundleList();
		cachedSkin = new SHSSkin();
	}

	private void skinCheck()
	{
		if (skin == null)
		{
			if (IsRoot)
			{
				skin = GUIManager.Instance.StyleManager.GetSkin("rootSkin");
			}
			else
			{
				skin = GUIManager.Instance.StyleManager.GetSkin(Id + "Skin");
			}
		}
		skinInitialized = true;
	}

	public override void DrawPreprocess()
	{
		base.DrawPreprocess();
		if (!skinInitialized)
		{
			skinCheck();
		}
		if (skin != null)
		{
			cachedSkin.skin = GUI.skin;
			if (GUI.skin != skin.skin)
			{
				GUI.skin = skin.skin;
			}
		}
		if (backgroundEnabled)
		{
			cachedcolor = GUI.color;
			GUI.color = new Color(backgroundcolor.r, backgroundcolor.g, backgroundcolor.b, alpha);
			GUI.Box(base.rect, string.Empty);
			GUI.color = cachedcolor;
		}
		if (LayoutType == LayoutTypeEnum.Fixed)
		{
			if (clippingEnabled)
			{
				GUI.BeginGroup(Rect);
			}
		}
		else if (clippingEnabled)
		{
			GUILayout.BeginArea(Rect);
		}
	}

	protected bool checkDrawRequirements(IGUIControl control, DrawPhaseHintEnum drawHint, DrawModeSetting drawMode)
	{
		return control.IsVisible && control.DrawPhaseHint == drawHint && (drawMode == DrawModeSetting.AlwaysOnTopMode || (!control.GetControlFlag(ControlFlagSetting.IsModal) && !control.GetControlFlag(ControlFlagSetting.DrawOnTop)));
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		foreach (IGUIControl control in controlList)
		{
			if (checkDrawRequirements(control, DrawPhaseHintEnum.PreDraw, drawFlags))
			{
				control.DrawPreprocess();
				control.InitiateDraw(drawFlags);
				control.DrawFinalize();
			}
		}
		foreach (IGUIControl control2 in controlList)
		{
			if (checkDrawRequirements(control2, DrawPhaseHintEnum.PostDraw, drawFlags))
			{
				control2.DrawPreprocess();
				control2.InitiateDraw(drawFlags);
				control2.DrawFinalize();
			}
		}
	}

	public override void DrawFinalize()
	{
		base.DrawFinalize();
		if (LayoutType == LayoutTypeEnum.Fixed)
		{
			if (clippingEnabled)
			{
				GUI.EndGroup();
			}
		}
		else if (LayoutType == LayoutTypeEnum.Flow && clippingEnabled)
		{
			GUILayout.EndArea();
		}
		if (skin != null && GUI.skin != cachedSkin.skin)
		{
			GUI.skin = cachedSkin.skin;
		}
	}

	public override void DebugDraw(BitArray DrawFlags)
	{
		if (!(this is GUITopLevelWindow) && !(this is GUIRootWindow))
		{
			base.DebugDraw(DrawFlags);
		}
		foreach (IGUIControl control in controlList)
		{
			control.DebugDraw(DrawFlags);
		}
	}

	public override string getDebugTooltip()
	{
		string text = "GUI Window: ";
		string text2 = text;
		text = text2 + "x: " + position.x + " y: " + position.y + "   ";
		text2 = text;
		text = text2 + "sizeX: " + size.x + " sizeY: " + size.y + "   ";
		text2 = text;
		text = text2 + "Anchor: " + anchor.ToString() + " Docking: " + docking.ToString() + "   ";
		text2 = text;
		text = text2 + "id: " + id + " number of children: " + controlList.Count + "   ";
		return text + "skin: " + ((skin != null) ? skin.ToString() : string.Empty);
	}

	public override void Update()
	{
		GUIManager.Instance.Diagnostics.GUIControlBeginUpdate(this);
		if (animationPieceManager != null)
		{
			animationPieceManager.Update(Time.deltaTime);
			if (!IsVisible)
			{
				animationPieceManager.ForceCompleteAllAndClear();
			}
		}
		if (windowAnimationPieceManager != null)
		{
			windowAnimationPieceManager.Update(Time.deltaTime);
			if (!IsVisible)
			{
				windowAnimationPieceManager.ForceCompleteAllAndClear();
			}
		}
		while (deferredRemoveQueue.Count > 0)
		{
			IGUIControl control = deferredRemoveQueue.Dequeue();
			removeControlDeferred(control);
		}
		while (deferredAddQueue.Count > 0)
		{
			controlAddDeferredStruct controlAddDeferredStruct = deferredAddQueue.Dequeue();
			addControlDeferred(controlAddDeferredStruct.Control, controlAddDeferredStruct.Index, controlAddDeferredStruct.DrawPhaseHint);
		}
		base.Update();
		if (deferredRemoveValid)
		{
			foreach (IGUIControl delayedRemove in delayedRemoveList)
			{
				Remove(delayedRemove);
			}
			delayedRemoveList.Clear();
			deferredRemoveValid = false;
		}
		IGUIControl iGUIControl = null;
		try
		{
			IGUIControl[] array = new IGUIControl[controlList.Count];
			controlList.CopyTo(array);
			IGUIControl[] array2 = array;
			foreach (IGUIControl iGUIControl2 in array2)
			{
				iGUIControl = iGUIControl2;
				iGUIControl2.Update();
			}
		}
		catch (InvalidOperationException ex)
		{
			CspUtils.DebugLog("InvalidOperationException in GUIWindow.Update call from <" + ((Parent == null) ? "No Parent" : Parent.Id) + "/" + Id + "> with exception <" + ex + ">");
			CspUtils.DebugLog("Last processed control was: " + ((iGUIControl == null) ? string.Empty : iGUIControl.Path));
		}
		GUIManager.Instance.Diagnostics.GUIControlEndUpdate(this);
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		foreach (IGUIControl control in controlList)
		{
			control.HandleResize(message);
		}
	}

	private void SetupFullScreenAndMute()
	{
		if (!fullScreenBackgroundUp)
		{
			fullScreenBackgroundUp = true;
			GUIManager.Instance.SetScreenBackImage(fullScreenBackgroundTextureSource);
			GameController.GetController().DisableSceneRendering();
			currentMutingContext = AppShell.Instance.AudioManager.MuteAllExcept(new string[5]
			{
				"HUD_UI",
				"VO_UI",
				"VO_UI_Boss",
				"Music",
				"Level_Music"
			});
			AppShell.Instance.EventMgr.Fire(null, new GUIFullScreenOpenEvent());
		}
	}

	private void CleanupFullScreenAndMute()
	{
		if (fullScreenBackgroundUp)
		{
			fullScreenBackgroundUp = false;
			GUIManager.Instance.SetScreenBackImage(null);
			GameController.GetController().EnableSceneRendering();
			AppShell.Instance.AudioManager.UnMute(currentMutingContext);
			AppShell.Instance.EventMgr.Fire(null, new GUIFullScreenClosedEvent());
		}
	}

	public override void SetVisible(bool visible, SetVisibleReason reason)
	{
		CheckAndInitializeResourcesIfTime(ControlTraits.ResourceLoadingPhaseTraitEnum.Show);
		bool flag = Traits.FullScreenOpaqueBackgroundTrait == ControlTraits.FullScreenOpaqueBackgroundTraitEnum.HasFullScreenOpaqueBackground;
		if (OpenAndCloseAnimations(visible) == QuitOrContinueTheIsVisibleCall.QuitOutOf)
		{
			if (flag)
			{
				CleanupFullScreenAndMute();
			}
			if (reason == SetVisibleReason.Transition)
			{
				windowAnimationPieceManager.ForceCompleteAllAndClear();
			}
			return;
		}
		if (flag)
		{
			if (visible)
			{
				SetupFullScreenAndMute();
			}
			else
			{
				CleanupFullScreenAndMute();
			}
		}
		if (visible != isVisible)
		{
			base.SetVisible(visible, reason);
			foreach (IGUIControl control in controlList)
			{
				if (control.Traits.VisibilityTrait == ControlTraits.VisibilityTraitEnum.Inherit)
				{
					control.SetVisible(visible, reason);
				}
				else if (visible && control.Traits.VisibilityTrait == ControlTraits.VisibilityTraitEnum.Cached)
				{
					control.IsVisible = control.CachedVisible;
				}
			}
			if (animationPieceManager != null && !IsVisible)
			{
				animationPieceManager.ForceCompleteAllAndClear();
			}
			if (windowAnimationPieceManager != null && !IsVisible)
			{
				windowAnimationPieceManager.ForceCompleteAllAndClear();
			}
		}
	}

	private QuitOrContinueTheIsVisibleCall OpenAndCloseAnimations(bool value)
	{
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Expected O, but got Unknown
		if (!SkipAnimation)
		{
			if ((value && isVisible && animationProgressState == AnimationProgress.Idle) || (!value && !isVisible && animationProgressState == AnimationProgress.Idle))
			{
				return QuitOrContinueTheIsVisibleCall.ContinueOn;
			}
			if ((value && animationProgressState == AnimationProgress.AnimationOpenInProgress) || (!value && animationProgressState == AnimationProgress.AnimationCloseInProgress))
			{
				return QuitOrContinueTheIsVisibleCall.QuitOutOf;
			}
			if (animationProgressState == AnimationProgress.QueuedCloseReady || animationProgressState == AnimationProgress.QueuedOpenReady)
			{
				animationProgressState = AnimationProgress.Idle;
			}
			if (value && AnimationOnOpen != null && !disableAutoOpenAndCloseAnimations)
			{
				if (animationProgressState == AnimationProgress.AnimationCloseInProgress)
				{
					QuededOpenCloseState = QueuedOpenClose.QueuedOpen;
				}
				else if (animationProgressState == AnimationProgress.Idle)
				{
					AnimClip animClip = AnimationOnOpen();
					animClip.OnFinished += (Action)(object)(Action)delegate
					{
						animationProgressState = AnimationProgress.Idle;
						if (QuededOpenCloseState == QueuedOpenClose.QueuedClose)
						{
							animationProgressState = AnimationProgress.QueuedCloseReady;
							QuededOpenCloseState = QueuedOpenClose.None;
							IsVisible = false;
						}
						FireAnimationOpenFinished();
					};
					WindowAnimationPieceManager.Add(animClip);
					animationProgressState = AnimationProgress.AnimationOpenInProgress;
					return QuitOrContinueTheIsVisibleCall.ContinueOn;
				}
			}
			if (!value && AnimationOnClose != null && !disableAutoOpenAndCloseAnimations)
			{
				if (animationProgressState == AnimationProgress.AnimationOpenInProgress)
				{
					QuededOpenCloseState = QueuedOpenClose.QueuedClose;
					return QuitOrContinueTheIsVisibleCall.QuitOutOf;
				}
				if (animationProgressState == AnimationProgress.Idle)
				{
					AnimClip animClip2 = AnimationOnClose();
					animClip2.OnFinished += (Action)(object)(Action)delegate
					{
						animationProgressState = AnimationProgress.Idle;
						if (QuededOpenCloseState == QueuedOpenClose.QueuedOpen)
						{
							animationProgressState = AnimationProgress.QueuedOpenReady;
							QuededOpenCloseState = QueuedOpenClose.None;
							IsVisible = true;
						}
						else
						{
							SkipAnimation = true;
							IsVisible = false;
						}
						FireAnimationCloseFinished();
					};
					WindowAnimationPieceManager.Add(animClip2);
					animationProgressState = AnimationProgress.AnimationCloseInProgress;
					return QuitOrContinueTheIsVisibleCall.QuitOutOf;
				}
			}
		}
		SkipAnimation = false;
		return QuitOrContinueTheIsVisibleCall.ContinueOn;
	}

	public void FireAnimationOpenFinished()
	{
		if (this.AnimationOpenFinished != null)
		{
			this.AnimationOpenFinished();
		}
	}

	public void FireAnimationCloseFinished()
	{
		if (this.AnimationCloseFinished != null)
		{
			this.AnimationCloseFinished();
		}
	}

	public void AnimationOpenFinishedClear()
	{
		AnimationFinishDelegate[] array = this.AnimationOpenFinished.GetInvocationList() as AnimationFinishDelegate[];
		if (array != null)
		{
			AnimationFinishDelegate[] array2 = array;
			foreach (AnimationFinishDelegate value in array2)
			{
				this.AnimationOpenFinished = (AnimationFinishDelegate)Delegate.Remove(this.AnimationOpenFinished, value);
			}
		}
	}

	public void AnimationCloseFinishedClear()
	{
		if (this.AnimationCloseFinished == null)
		{
			return;
		}
		Delegate[] invocationList = this.AnimationCloseFinished.GetInvocationList();
		if (invocationList != null)
		{
			Delegate[] array = invocationList;
			foreach (Delegate @delegate in array)
			{
				this.AnimationCloseFinished = (AnimationFinishDelegate)Delegate.Remove(this.AnimationCloseFinished, (AnimationFinishDelegate)@delegate);
			}
		}
	}

	public void SetVisibilityAndSkipAnimation(bool visibility)
	{
		SkipAnimation = true;
		IsVisible = visibility;
		SkipAnimation = false;
	}

	public void SetSkipAnimation(bool skip)
	{
		SkipAnimation = skip;
	}

	public void ControlToFront(IGUIControl ctrl)
	{
		controlList.Remove(ctrl);
		controlList.Add(ctrl);
	}

	public void ControlToBack(IGUIControl ctrl)
	{
		controlList.Remove(ctrl);
		controlList.Insert(0, ctrl);
	}

	public virtual void Add(IGUIControl Control, IGUIControl TargetControl)
	{
		int num = controlList.IndexOf(TargetControl);
		if (num == -1)
		{
			CspUtils.DebugLog("Can't find control " + TargetControl + " to insert " + Control + " into list.");
		}
		else
		{
			internalAdd(Control, num, TargetControl.DrawPhaseHint);
		}
	}

	public virtual void Add(IGUIControl Control)
	{
		Add(Control, DrawOrder.DrawFirst, DrawPhaseHintEnum.PreDraw);
	}

	public virtual void Add(IGUIControl Control, DrawOrder DrawOrder)
	{
		Add(Control, DrawOrder, DrawPhaseHintEnum.PreDraw);
	}

	public virtual void Add(IGUIControl Control, DrawOrder DrawOrder, DrawPhaseHintEnum DrawPhaseHint)
	{
		switch (DrawOrder)
		{
		case DrawOrder.DrawFirst:
			internalAdd(Control, controlList.Count, DrawPhaseHint);
			break;
		case DrawOrder.DrawLast:
			internalAdd(Control, 0, DrawPhaseHint);
			break;
		}
	}

	protected virtual void internalAdd(IGUIControl Control, int Index, DrawPhaseHintEnum DrawPhaseHint)
	{
		if (!controlList.Contains(Control))
		{
			controlList.Insert(Index, Control);
			Control.Parent = this;
			Control.Layer = Layer;
			Control.DrawPhaseHint = DrawPhaseHint;
			Control.OnAdded(this);
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
			{
				reparentGameObjects(Control);
			}
		}
		else if (DrawPhaseHint != this.DrawPhaseHint)
		{
			this.DrawPhaseHint = DrawPhaseHint;
		}
	}

	public virtual void AddDeferred(IGUIControl Control, int Index, DrawPhaseHintEnum DrawPhaseHint)
	{
		deferredAddQueue.Enqueue(new controlAddDeferredStruct(Control, Index, DrawPhaseHint));
	}

	private void addControlDeferred(IGUIControl Control, int Index, DrawPhaseHintEnum DrawPhaseHint)
	{
		controlList.Insert(Index, Control);
		Control.Parent = this;
		Control.DrawPhaseHint = DrawPhaseHint;
		Control.OnAdded(this);
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			reparentGameObjects(Control);
		}
	}

	protected void reparentGameObjects(IGUIControl Control)
	{
		if (((GUIControl)Control).guiTreeNode == null)
		{
			(Control as GUIControl).AddInspector();
		}
		if (guiTreeNode == null)
		{
			CspUtils.DebugLog("GUIWindow.InternalAdd: Window " + Id + " does not have a guiTreeNode!");
		}
		else if (((GUIControl)Control).guiTreeNode == null)
		{
			CspUtils.DebugLog("GUIWIndow.InternalAdd: Child control " + Control.Id + " does not have a guiTreeNode!");
		}
		else
		{
			Utils.AttachGameObject(guiTreeNode, ((GUIControl)Control).guiTreeNode);
		}
	}

	private void removeControlDeferred(IGUIControl Control)
	{
		if (Control.IsVisible)
		{
			Control.Hide();
		}
		controlList.Remove(Control);
		Control.OnRemoved(this);
		(Control as GUIControl).RemoveInspector();
		Control = null;
	}

	public virtual void RemoveAllControls()
	{
		foreach (IGUIControl control in controlList)
		{
			(control as GUIControl).RemoveInspector();
		}
		controlList = new List<IGUIControl>();
	}

	public virtual void Remove(IGUIControl Control)
	{
		if (Control.IsVisible)
		{
			Control.SetVisible(false, SetVisibleReason.Transition);
		}
		if (controlList.Contains(Control))
		{
			(Control as GUIControl).RemoveInspector();
			controlList.Remove(Control);
			Control = null;
		}
	}

	public IGUIControl RemoveControlFromList(IGUIControl Control)
	{
		if (!controlList.Contains(Control))
		{
			CspUtils.DebugLog("Removing Control " + this + ", but it's already gone or has never been added.");
			return null;
		}
		controlList.Remove(Control);
		Control.OnRemoved(this);
		return Control;
	}

	public override void Clear()
	{
		base.Clear();
		foreach (IGUIControl control in controlList)
		{
			if (control.IsVisible)
			{
				control.Hide();
			}
			if (controlList.Contains(control))
			{
				control.Parent = null;
			}
		}
		controlList.Clear();
	}

	public override void SetControlFlag(ControlFlagSetting Setting, bool On, bool SetChildren)
	{
		base.SetControlFlag(Setting, On, SetChildren);
		if (SetChildren)
		{
			foreach (IGUIControl control in controlList)
			{
				control.SetControlFlag(Setting, On, SetChildren);
			}
		}
	}

	public override void RemoveControlFlag(ControlFlagSetting Setting, bool SetChildren)
	{
		base.RemoveControlFlag(Setting, SetChildren);
		if (SetChildren)
		{
			foreach (IGUIControl control in controlList)
			{
				control.RemoveControlFlag(Setting, SetChildren);
			}
		}
	}

	protected override void handleMouseHitTestOverState()
	{
		if (HitTestType != HitTestTypeEnum.Transparent)
		{
			base.handleMouseHitTestOverState();
		}
	}

	public override bool IsDescendantHandler(IInputHandler handler)
	{
		if (this == handler)
		{
			return true;
		}
		foreach (IGUIControl control in controlList)
		{
			if (((IInputHandler)control).IsDescendantHandler(handler))
			{
				return true;
			}
		}
		return false;
	}

	public void SetBackground(Color color)
	{
		backgroundEnabled = true;
		backgroundcolor = color;
		alpha = color.a;
	}

	public void SetBackground(bool off)
	{
		if (off)
		{
			SetBackground(backgroundcolor);
		}
		else
		{
			backgroundEnabled = false;
		}
	}

	public void SetBackground(string textureSource)
	{
		if (textureSource != null)
		{
			fullScreenFadeoutVisibilityCounter++;
			if (fullScreenFadeoutVisibilityCounter == 1)
			{
				DisplayFadeoutWindow(textureSource);
			}
		}
		else
		{
			fullScreenFadeoutVisibilityCounter--;
			if (fullScreenFadeoutVisibilityCounter == 0)
			{
				HideFadeoutWindow();
			}
		}
	}

	private void DisplayFadeoutWindow(string textureSource)
	{
		if (fadeoutImage != null)
		{
			HideFadeoutWindow();
		}
		fadeoutImage = new FullScreenFadeoutImage(textureSource);
		Add(fadeoutImage, DrawOrder.DrawFirst, DrawPhaseHintEnum.PreDraw);
		fadeoutImage.IsVisible = true;
	}

	private void HideFadeoutWindow()
	{
		if (fadeoutImage != null)
		{
			fadeoutImage.IsVisible = false;
		}
	}

	protected void EnsureHierarchyActive()
	{
		if (Parent != null && !Parent.IsActive)
		{
			Parent.SetActive();
		}
	}

	public bool HitTestBoundsCheck()
	{
		if (!ScreenRect.Contains(SHSInput.mouseScreenPosition))
		{
			return false;
		}
		if (parent != null)
		{
			return parent.HitTestBoundsCheck();
		}
		return true;
	}

	public override void SetActive()
	{
		EnsureHierarchyActive();
		if (!IsActive)
		{
			base.SetActive();
			foreach (IGUIControl control in controlList)
			{
				if (control != null && control.Traits != null && control.Traits.ActivationTrait == ControlTraits.ActivationTraitEnum.Auto)
				{
					control.SetActive();
				}
			}
		}
	}

	public override void SetInactive()
	{
		if (IsActive)
		{
			base.SetInactive();
		}
		foreach (IGUIControl control in controlList)
		{
			control.SetInactive();
		}
	}

	public void InternalDeferredRemove(IGUIControl Control)
	{
		deferredRemoveValid = true;
		delayedRemoveList.Add(Control);
	}

	public override void OnActive()
	{
		base.OnActive();
	}

	public virtual List<T> GetControlsOfType<T>()
	{
		return GetControlsOfType<T>(false);
	}

	public virtual List<T> GetControlsOfType<T>(bool recursive)
	{
		List<T> list = new List<T>();
		foreach (IGUIControl control in controlList)
		{
			if (control is T)
			{
				list.Add((T)control);
			}
			if (control is IGUIContainer && recursive)
			{
				List<T> list2 = new List<T>();
				list2 = ((IGUIContainer)control).GetControlsOfType<T>(recursive);
				list.AddRange(list2);
			}
		}
		return list;
	}

	public IGUIControl GetControl(string name)
	{
		foreach (IGUIControl control in controlList)
		{
			if (control.Id == name)
			{
				return control;
			}
		}
		return null;
	}

	public T GetControl<T>(string name) where T : GUIControl
	{
		return GetControl(name) as T;
	}

	private string[] GetTokenizedPath(string path)
	{
		string[] array = path.Split('/');
		if (array.Length == 0)
		{
			return null;
		}
		if (array[0].Length == 0)
		{
			string[] array2 = new string[array.Length - 1];
			Array.Copy(array, 1, array2, 0, array2.Length);
			array = array2;
		}
		return array;
	}

	public bool Transition(string WindowName)
	{
		return Transition(WindowName, null);
	}

	public bool Transition(string WindowName, TransactionMonitor transaction)
	{
		SetInactive();
		string[] tokenizedPath = GetTokenizedPath(WindowName);
		if (tokenizedPath == null || tokenizedPath.Length < 1)
		{
			CspUtils.DebugLog("Can't parse child window path: " + WindowName);
			return false;
		}
		IGUIControl iGUIControl = this;
		for (int i = 0; i < tokenizedPath.Length; i++)
		{
			iGUIControl = ((IGUIContainer)iGUIControl)[tokenizedPath[i]];
			if (iGUIControl == null)
			{
				CspUtils.DebugLog("No window named: " + tokenizedPath[i]);
				return false;
			}
			if (!(iGUIControl is IGUIContainer))
			{
				CspUtils.DebugLog("Cant transition a non IGUIContainer object: " + this[tokenizedPath[i]]);
				return false;
			}
		}
		List<SupportingAssetBundleInfo> bundleList = new List<SupportingAssetBundleInfo>();
		iGUIControl = this;
		for (int j = 0; j < tokenizedPath.Length; j++)
		{
			iGUIControl = ((IGUIContainer)iGUIControl)[tokenizedPath[j]];
			if (iGUIControl is IGUIContainer)
			{
				((IGUIContainer)iGUIControl).GetBundleList(ref bundleList, j == tokenizedPath.Length - 1);
			}
		}
		bool flag = true;
		foreach (SupportingAssetBundleInfo item in bundleList)
		{
			if (!GUIManager.Instance.BundleManager.IsBundleLoaded(item.BundleName))
			{
				flag = false;
				if (transaction != null)
				{
					string text = item.BundleName + "_step";
					if (!transaction.HasStep(text))
					{
						transaction.AddStep(text, TransactionMonitor.DumpTransactionStatus);
						GUIManager.Instance.BundleManager.LoadBundle(item.BundleName, delegate(AssetBundleLoadResponse response, object extraData)
						{
							if (response.Error != null && response.Error != string.Empty)
							{
								transaction.FailStep((string)extraData, "Couldn't load " + (string)extraData);
							}
							else
							{
								transaction.CompleteStep((string)extraData);
							}
						}, text);
					}
				}
				else
				{
					CspUtils.DebugLog("Required bundle: " + item.BundleName + " not loaded for:" + WindowName);
				}
			}
		}
		if (flag)
		{
			((IGUIContainer)iGUIControl).SetActive();
			((IGUIContainer)iGUIControl).Show();
		}
		else if (transaction != null)
		{
			transaction.onComplete += OnBundleLoadCompleteDelegate(iGUIControl);
		}
		return true;
	}

	private TransactionMonitor.OnCompleteDelegate OnBundleLoadCompleteDelegate(IGUIControl controlToShow)
	{
		return delegate
		{
			((IGUIContainer)controlToShow).SetActive();
			((IGUIContainer)controlToShow).Show();
		};
	}

	public override void OnSceneEnter(AppShell.GameControllerTypeData currentGameData)
	{
		foreach (IGUIControl control in controlList)
		{
			if (control.IsActive)
			{
				control.OnSceneEnter(currentGameData);
			}
		}
	}

	public override void OnSceneLoaded(AppShell.GameControllerTypeData currentGameData)
	{
		foreach (IGUIControl control in controlList)
		{
			if (control.IsActive)
			{
				control.OnSceneLoaded(currentGameData);
			}
		}
	}

	public override void OnSceneLeave(AppShell.GameControllerTypeData lastGameData, AppShell.GameControllerTypeData currentGameData)
	{
		foreach (IGUIControl control in controlList)
		{
			if (control.IsActive)
			{
				control.OnSceneLeave(lastGameData, currentGameData);
			}
		}
	}

	public override void OnLocaleChange(string newLocale)
	{
		foreach (IGUIControl control in controlList)
		{
			control.OnLocaleChange(newLocale);
		}
	}

	protected virtual void InitializeBundleList()
	{
	}

	public virtual void GetBundleList(ref List<SupportingAssetBundleInfo> bundleList, bool recurse)
	{
		bundleList.AddRange(supportingAssetBundles);
		if (recurse)
		{
			foreach (IGUIContainer item in GetControlsOfType<IGUIContainer>())
			{
				item.GetBundleList(ref bundleList, recurse);
			}
		}
	}

	protected override void AttachInspector()
	{
		inspector = guiTreeNode.AddComponent(typeof(GUIWindowInspector));
	}

	public override void AddInspector()
	{
		if (guiTreeNode == null)
		{
			base.AddInspector();
		}
		foreach (IGUIControl control in controlList)
		{
			GUIControl gUIControl = control as GUIControl;
			if (gUIControl != null && gUIControl.guiTreeNode == null)
			{
				gUIControl.AddInspector();
			}
		}
	}

	public override void RemoveInspector()
	{
		base.RemoveInspector();
	}

	protected override void ReflectToInspector()
	{
		base.ReflectToInspector();
		if (inspector != null)
		{
			((GUIWindowInspector)inspector).layoutType = layoutType;
		}
	}

	protected override void ReflectFromInspector()
	{
		base.ReflectFromInspector();
		if (inspector != null)
		{
			layoutType = ((GUIWindowInspector)inspector).layoutType;
		}
	}
}
