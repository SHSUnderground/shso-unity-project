using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class GUIManager : MonoBehaviour, IInputHandler
{
	public enum InputEventStateEnum
	{
		MouseDown,
		MouseOver,
		MouseOut,
		MouseUp,
		MouseWheel,
		Idle
	}

	public enum UILayer
	{
		Debug,
		System,
		Main,
		Tooltip,
		Notification
	}

	public enum CursorBlockTestStateEnum
	{
		World,
		UI,
		External
	}

	public class InputState
	{
		public GUIControl.ControlTraits.EventHandlingEnum eventHandlingType;

		public InputEventStateEnum inputEventType;

		public int frameAdded;

		public int frameUpdated;

		public int stackIndex;

		public GUIEventRegistrationInfo registrationInfo;

		public bool triggered;

		public bool handledMouseOver;

		public bool hadLeftDownEvent;

		public bool hadRightDownEvent;

		public InputState(int frameAdd, int stackIdx, GUIEventRegistrationInfo regInfo)
		{
			eventHandlingType = GUIControl.ControlTraits.EventHandlingEnum.Ignore;
			inputEventType = InputEventStateEnum.Idle;
			frameAdded = (frameUpdated = frameAdd);
			stackIndex = stackIdx;
			registrationInfo = regInfo;
			triggered = false;
			hadLeftDownEvent = false;
			hadRightDownEvent = false;
		}
	}

	private class ModalGUIControlInfo
	{
		public readonly IGUIControl modalControl;

		public readonly GUIControl.ModalLevelEnum modalLevel;

		public ModalGUIControlInfo(IGUIControl ModalControl, GUIControl.ModalLevelEnum ModalLevel)
		{
			modalControl = ModalControl;
			modalLevel = ModalLevel;
		}
	}

	private class ModalGUIWindowInfoList<T> : List<T> where T : ModalGUIControlInfo
	{
		public new bool Contains(T item)
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					if (current == item)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool Contains(IGUIControl ctrl)
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					if (current.modalControl == ctrl)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool Remove(IGUIControl ctrl)
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					if (current.modalControl == ctrl)
					{
						Remove(current);
						return true;
					}
				}
			}
			return false;
		}
	}

	public enum ModalStateEnum
	{
		Normal,
		Modal,
		Transition
	}

	public enum InputModeEnum
	{
		Normal,
		Capture,
		DragDrop
	}

	public enum UILogLevelEnum
	{
		Normal,
		Verbose,
		FrameRateNightmare
	}

	[Serializable]
	public class UISFX
	{
		public string name;

		public GameObject sfx;
	}

	public enum VisulizerLayering
	{
		Top,
		Stacked,
		All
	}

	[Flags]
	public enum InitializationState
	{
		StringTableLoaded = 0x1,
		AssetManifestLoaded = 0x2,
		FontBankLoaded = 0x4,
		CursorsLoaded = 0x8
	}

	public enum DialogTypeEnum
	{
		YesNoDialog,
		OkCancelDialog,
		OkDialog,
		ProgressDialog,
		AwardDialog,
		ErrorDialog
	}

	public enum PersistentGUIToggleOption
	{
		Visibility,
		Enable
	}

	public class GUIEventRegistrationInfo
	{
		public IGUIControl control;

		public GUIEventCallbackDelegate mouseOverCallback;

		public GUIEventCallbackDelegate mouseOutCallback;

		public GUIEventCallbackDelegate mouseDownCallback;

		public GUIEventCallbackDelegate rightMouseDownCallback;

		public GUIEventCallbackDelegate mouseUpCallback;

		public GUIEventCallbackDelegate rightMouseUpCallback;

		public GUIEventMouseWheelCallbackDelegate mouseWheelCallback;

		public GUIEventRegistrationInfo(IGUIControl control, GUIEventCallbackDelegate mouseOverCallback, GUIEventCallbackDelegate mouseOutCallback, GUIEventCallbackDelegate mouseDownCallback, GUIEventCallbackDelegate mouseUpCallback, GUIEventCallbackDelegate rightMouseDownCallback, GUIEventCallbackDelegate rightMouseUpCallback, GUIEventMouseWheelCallbackDelegate mouseWheelCallback)
		{
			this.control = control;
			this.mouseOverCallback = mouseOverCallback;
			this.mouseOutCallback = mouseOutCallback;
			this.mouseDownCallback = mouseDownCallback;
			this.mouseUpCallback = mouseUpCallback;
			this.rightMouseDownCallback = rightMouseDownCallback;
			this.rightMouseUpCallback = rightMouseUpCallback;
			this.mouseWheelCallback = mouseWheelCallback;
		}
	}

	public enum LoadState
	{
		Awake,
		Start,
		Restart
	}

	public struct WindowBinding
	{
		public enum RegistrationPeriod
		{
			Initialization,
			Runtime
		}

		public RegistrationPeriod registrationPeriod;

		public Type windowType;

		public IGUIControl windowRef;

		public string uiLayerName;

		public string windowName;

		public WindowBinding(string uiLayerName, string windowName)
		{
			this.uiLayerName = uiLayerName;
			this.windowName = windowName;
			windowRef = null;
			windowType = null;
			registrationPeriod = RegistrationPeriod.Initialization;
		}

		public WindowBinding(Type windowType, string uiLayerName, string windowName)
		{
			this = new WindowBinding(uiLayerName, windowName);
			this.windowType = windowType;
		}
	}

	public delegate void ModalNotificationDelegate(IGUIControl control, bool on);

	public delegate bool GUIEventInputCheckDelegate(EventType eventType);

	public delegate void GUIEventCallbackDelegate(IGUIControl control, InputEventStateEnum type);

	public delegate void GUIEventMouseWheelCallbackDelegate(IGUIControl control, float delta, int direction);

	protected GUIDiagnosticManager diagnosticsManager;

	protected GUIStyleManager styleManager;

	protected GUINotificationManager notificationManager;

	protected GUIFontManager fontManager;

	protected GUICursorManager cursorManager;

	protected GUIContextManager contextManager;

	protected GameObject uiTreeGameObject;

	protected GameObject uiTreeOrphanage;

	private int uiDrawframeCount;

	private int uiInputStackCount;

	protected IGUIControl currentOverControl;

	private EventType currentEventType;

	private bool isLeftClickEventHandled;

	private bool isRightClickEventHandled;

	private GUIControl.ControlTraits.EventHandlingEnum currentEventState;

	private bool leftMouseDownFlag;

	private bool rightMouseDownFlag;

	private int leftMouseDownLastUpdatedFrame;

	private int rightMouseDownLastUpdatedFrame;

	private List<IGUIControl> inputStack;

	private Dictionary<IGUIControl, InputState> inputStateLookup;

	private List<IGUIControl> mouseDownList;

	protected KeyBank keyBank;

	protected GUIFocusManager focusManager;

	protected GUIBundleManager bundleManager;

	protected GUIToolTipManager tooltipManager;

	protected Dictionary<UILayer, IGUIContainer> uiRoots;

	protected static int nextUniqueID;

	protected List<IGUIControl> alwaysTopContainers;

	private ModalGUIWindowInfoList<ModalGUIControlInfo> modalWindowList;

	private ModalStateEnum currentState;

	private InputModeEnum currentMode;

	private ICaptureHandler captureHandler;

	private DragDropInfo currentDragDropInfo;

	public List<GUISkin> skinList;

	public GUISkin fontSkin;

	public Font fallbackFont;

	public UILogLevelEnum logLevel;

	public UISFX[] sounds;

	protected bool inTransition;

	protected bool inDrawingMode;

	protected GameObject mouseoverEnemyIndicator;

	protected GameObject attackingEnemyIndicator;

	protected HostileTargetSelection attackingEnemyIndicatorScript;

	protected GameObject targetedPlayerIndicatorPrefab;

	protected GameObject targetedPlayerIndicator;

	protected ParticleEmitter targetedPlayerIndicatorEmitter;

	protected GameObject mouseoverThrowableIndicator;

	protected List<KeyValuePair<IGUIHitTestable, Color>> blocktestControls;

	private bool drawingEnabled = true;

	protected BitArray debugDrawFlags = new BitArray(32);

	public static VisulizerLayering drawTypeLayer;

	protected bool clearKeyboardFocusOnNextFrame;

	protected string setKeyboardFocusOnNextFrameTo;

	public InitializationState initializationState;

	private bool firstTimeInitialization = true;

	public static readonly Vector2 MinResolution = new Vector2(1020f, 644f);

	protected static GUIManager instance;

	private readonly Hashtable dynWindowTable = new Hashtable();

	private readonly Hashtable dynWindowLoadingTable = new Hashtable();

	private readonly Hashtable dynSubwindowLoadingTable = new Hashtable();

	private readonly Hashtable dynWindowUnloadingTable = new Hashtable();

	private readonly Hashtable dynSubwindowUnloadingTable = new Hashtable();

	private static readonly List<WindowBinding> registeredWindowInitList = new List<WindowBinding>();

	public GUIDiagnosticManager Diagnostics
	{
		get
		{
			return diagnosticsManager;
		}
	}

	public GUICursorManager CursorManager
	{
		get
		{
			return cursorManager;
		}
	}

	public GUIContextManager ContextManager
	{
		get
		{
			return contextManager;
		}
	}

	public IGUIControl CurrentOverControl
	{
		get
		{
			return currentOverControl;
		}
	}

	public EventType CurrentEventType
	{
		get
		{
			return currentEventType;
		}
	}

	public bool IsLeftClickEventHandled
	{
		get
		{
			return isLeftClickEventHandled;
		}
	}

	public bool IsRightClickEventHandled
	{
		get
		{
			return isRightClickEventHandled;
		}
	}

	public GUIControl.ControlTraits.EventHandlingEnum CurrentEventState
	{
		get
		{
			return currentEventState;
		}
	}

	public List<IGUIControl> InputStack
	{
		get
		{
			return inputStack;
		}
	}

	public Dictionary<IGUIControl, InputState> InputStateLookup
	{
		get
		{
			return inputStateLookup;
		}
	}

	public GUIBundleManager BundleManager
	{
		get
		{
			return bundleManager;
		}
	}

	public GUIToolTipManager TooltipManager
	{
		get
		{
			return tooltipManager;
		}
	}

	public IGUIControl this[UILayer layer, string key]
	{
		get
		{
			IGUIContainer root = GetRoot(layer);
			if (root != null)
			{
				return GetRoot(layer)[key];
			}
			return null;
		}
	}

	public Dictionary<UILayer, IGUIContainer> UIRoots
	{
		get
		{
			return uiRoots;
		}
	}

	public List<IGUIContainer> Roots
	{
		get
		{
			IGUIContainer[] array = new IGUIContainer[uiRoots.Count];
			uiRoots.Values.CopyTo(array, 0);
			return new List<IGUIContainer>(array);
		}
	}

	public int NextUniqueID
	{
		get
		{
			return nextUniqueID++;
		}
	}

	public ModalStateEnum CurrentState
	{
		get
		{
			return currentState;
		}
	}

	public InputModeEnum CurrentMode
	{
		get
		{
			return currentMode;
		}
	}

	public DragDropInfo CurrentDragDropInfo
	{
		get
		{
			return currentDragDropInfo;
		}
	}

	public List<KeyValuePair<IGUIHitTestable, Color>> BlockTestControls
	{
		get
		{
			return blocktestControls;
		}
		set
		{
			blocktestControls = value;
		}
	}

	public bool DrawingEnabled
	{
		get
		{
			return drawingEnabled;
		}
		set
		{
			drawingEnabled = value;
			if (AppShell.Instance != null && AppShell.Instance.EventMgr != null)
			{
				AppShell.Instance.EventMgr.Fire(null, new GUIDrawingEnabledMessage(value));
			}
		}
	}

	public BitArray DebugDrawFlags
	{
		get
		{
			return debugDrawFlags;
		}
	}

	public CursorBlockTestStateEnum CursorBlockTestState
	{
		get
		{
			Vector2 mouseScreenPosition = SHSInput.mouseScreenPosition;
			if (!ScreenRect.Contains(mouseScreenPosition))
			{
				return CursorBlockTestStateEnum.External;
			}
			if (BlockTestControls != null)
			{
				for (int num = BlockTestControls.Count - 1; num >= 0; num--)
				{
					IGUIHitTestable key = BlockTestControls[num].Key;
					if (key.BlockTest(mouseScreenPosition))
					{
						return CursorBlockTestStateEnum.UI;
					}
				}
			}
			return CursorBlockTestStateEnum.World;
		}
	}

	public GUINotificationManager NotificationManager
	{
		get
		{
			return notificationManager;
		}
	}

	public GUIStyleManager StyleManager
	{
		get
		{
			return styleManager;
		}
	}

	public GUIFocusManager FocusManager
	{
		get
		{
			return focusManager;
		}
	}

	public GUIFontManager FontManager
	{
		get
		{
			return fontManager;
		}
	}

	public IGUIContainer Root
	{
		get
		{
			if (!uiRoots.ContainsKey(UILayer.Main))
			{
				return null;
			}
			return uiRoots[UILayer.Main];
		}
	}

	public IGUIControl this[string key]
	{
		get
		{
			return Root[key];
		}
	}

	public static float CurrentScale
	{
		get
		{
			float num = Screen.width;
			Vector2 minResolution = MinResolution;
			float a = num / minResolution.x;
			float num2 = Screen.height;
			Vector2 minResolution2 = MinResolution;
			return Mathf.Min(1f, Mathf.Min(a, num2 / minResolution2.y));
		}
	}

	public static Rect ScreenRect
	{
		get
		{
			float num = Screen.width;
			Vector2 minResolution = MinResolution;
			if (num > minResolution.x)
			{
				float num2 = Screen.height;
				Vector2 minResolution2 = MinResolution;
				if (num2 > minResolution2.y)
				{
					return new Rect(0f, 0f, Screen.width, Screen.height);
				}
			}
			return new Rect(0f, 0f, (float)Screen.width / CurrentScale, (float)Screen.height / CurrentScale);
		}
	}

	public static GUIManager Instance
	{
		get
		{
			return instance;
		}
	}

	public virtual bool CanHandleInput
	{
		get
		{
			return true;
		}
	}

	public SHSInput.InputRequestorType InputRequestorType
	{
		get
		{
			return SHSInput.InputRequestorType.Script;
		}
	}

	public bool IsParentInTheModalList(IGUIControl item)
	{
		for (item = item.Parent; item != null; item = item.Parent)
		{
			if (modalWindowList.Contains(item))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsInTheModalList(IGUIControl item)
	{
		return modalWindowList.Contains(item);
	}

	private void setInputMode(InputModeEnum Mode, ICaptureHandler handler)
	{
		if (Mode != currentMode)
		{
			if (captureHandler != null)
			{
				captureHandler.OnCaptureUnacquired();
				captureHandler = null;
			}
			if (currentMode == InputModeEnum.DragDrop)
			{
				Root.SetControlFlag(GUIControl.ControlFlagSetting.DragDropMode, false, true);
				CursorManager.SetCursorType(GUICursorManager.CursorType.Normal);
				SHSInput.UnregisterListener(keyBank);
				if (currentDragDropInfo.Result == DragDropResult.Interrupted)
				{
					onDragDropCancelled(new SHSKeyCode(KeyCode.JoystickButton19));
					return;
				}
				AppShell.Instance.EventMgr.RemoveListener<GUIDragEndMessage>(onDragDropEnd);
			}
		}
		switch (Mode)
		{
		case InputModeEnum.Capture:
			captureHandler = handler;
			currentMode = ((handler != null) ? InputModeEnum.Capture : InputModeEnum.Normal);
			break;
		case InputModeEnum.DragDrop:
			currentMode = InputModeEnum.DragDrop;
			Root.SetControlFlag(GUIControl.ControlFlagSetting.DragDropMode, true, true);
			keyBank = new KeyBank(this, GUIControl.KeyInputState.Visible, KeyBank.KeyBankTypeEnum.Additive, new Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate>());
			keyBank.AddKey(new KeyCodeEntry(KeyCode.Escape, false, false, false), onDragDropCancelled);
			SHSInput.RegisterListener(keyBank);
			AppShell.Instance.EventMgr.AddListener<GUIDragEndMessage>(onDragDropEnd);
			break;
		default:
			currentMode = InputModeEnum.Normal;
			break;
		}
	}

	public static bool canDrawControl(GUIControl testControl)
	{
		if (drawTypeLayer == VisulizerLayering.All)
		{
			return true;
		}
		if (drawTypeLayer == VisulizerLayering.Stacked)
		{
			return isOver(testControl);
		}
		if (drawTypeLayer == VisulizerLayering.Top)
		{
			return isTop(testControl);
		}
		return false;
	}

	public static bool isOver(GUIControl testControl)
	{
		return testControl.ScreenRect.Contains(SHSInput.mouseScreenPosition);
	}

	public static bool isTop(GUIControl testControl)
	{
		return Instance.CurrentOverControl == testControl;
	}

	private void Awake()
	{
		if (instance != null)
		{
			CspUtils.DebugLog("GUIManager already created, attempt to create a second one will only lead to instability and hurt feelings.");
			return;
		}
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
		uiTreeGameObject = new GameObject("GUITree");
		UnityEngine.Object.DontDestroyOnLoad(uiTreeGameObject);
		uiTreeOrphanage = new GameObject("GUIOrphanage");
		UnityEngine.Object.DontDestroyOnLoad(uiTreeOrphanage);
		modalWindowList = new ModalGUIWindowInfoList<ModalGUIControlInfo>();
		alwaysTopContainers = new List<IGUIControl>();
		Configure(LoadState.Awake);
		AppShell.Instance.EventMgr.AddListener<StringTableLoadedMessage>(onStringTableLoaded);
		AppShell.Instance.EventMgr.AddListener<FontBankLoadedMessage>(onFontBankLoaded);
	}

	public void AddBlockTestControl(IGUIHitTestable control)
	{
		if (control.HitTestType != GUIControl.HitTestTypeEnum.Transparent)
		{
			foreach (KeyValuePair<IGUIHitTestable, Color> blockTestControl in BlockTestControls)
			{
				if (blockTestControl.Key == control)
				{
					return;
				}
			}
			BlockTestControls.Add(new KeyValuePair<IGUIHitTestable, Color>(control, new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 0.3f)));
		}
	}

	public void RemoveBlockTestControl(IGUIHitTestable control)
	{
		tooltipManager.Update();
		int num = 0;
		while (true)
		{
			if (num < BlockTestControls.Count)
			{
				if (BlockTestControls[num].Key == control)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		BlockTestControls.RemoveAt(num);
	}

	private void OnBundleLoaded(string bundleName, AssetBundle bundle)
	{
		if (bundleName.Contains("cursor_bundle"))
		{
			OnInitializationStep(InitializationState.CursorsLoaded);
		}
	}

	public void OnInitializationStep(InitializationState initSource)
	{
		initializationState |= initSource;
		if (initializationState != (InitializationState.StringTableLoaded | InitializationState.AssetManifestLoaded | InitializationState.FontBankLoaded | InitializationState.CursorsLoaded))
		{
			return;
		}
		if (!firstTimeInitialization)
		{
			Configure(LoadState.Restart);
			return;
		}
		firstTimeInitialization = false;
		styleManager = new GUIStyleManager(this);
		focusManager = new GUIFocusManager(this);
		fontManager = new GUIFontManager();
		cursorManager = new GUICursorManager(this);
		cursorManager.InitializeCursor();
		tooltipManager = new GUIToolTipManager(this);
		notificationManager = new GUINotificationManager(this, GetRoot(UILayer.Notification));
		contextManager = new GUIContextManager();
		blocktestControls = new List<KeyValuePair<IGUIHitTestable, Color>>();
		inputStack = new List<IGUIControl>();
		inputStateLookup = new Dictionary<IGUIControl, InputState>();
		mouseDownList = new List<IGUIControl>();
		Configure(LoadState.Start);
		GameController controller = GameController.GetController();
		if (controller != null)
		{
			AppShell appShell = AppShell.Instance;
			if (controller.controllerType != GameController.ControllerType.FrontEnd && controller.isTestScene)
			{
				ForceLoadOnlyTransition(controller.controllerType);
			}
			OnNewControllerLoading(new AppShell.GameControllerTypeData(appShell.GameControllerInfo[controller.controllerType].sceneName, appShell.GameControllerInfo[controller.controllerType].windowName), controller);
			OnNewControllerReady(new AppShell.GameControllerTypeData(appShell.GameControllerInfo[controller.controllerType].sceneName, appShell.GameControllerInfo[controller.controllerType].windowName), controller);
		}
		else
		{
			CspUtils.DebugLog("No game controller found, so no UI can be shown");
		}
		AppShell.Instance.EventMgr.Fire(null, new GUIResizeMessage(GUIControl.HandleResizeSource.Screen, new Rect(0f, 0f, -1f, -1f), new Rect(0f, 0f, ScreenRect.x, ScreenRect.y)));
	}

	private void OnEnable()
	{
		AppShell.Instance.EventMgr.AddListener<GUICaptureMessage>(onCaptureMessage);
		AppShell.Instance.EventMgr.AddListener<GUIDragBeginMessage>(onDragDropMode);
		AppShell.Instance.EventMgr.AddListener<LocaleChangedMessage>(onLocaleChanged);
		AppShell.Instance.EventMgr.AddListener<WindowStatusMessage>(onWindowStatus);
		AppShell.Instance.OnOldControllerUnloading += OnOldControllerUnloading;
		AppShell.Instance.OnNewControllerLoading += OnNewControllerLoading;
		AppShell.Instance.OnNewControllerReady += OnNewControllerReady;
	}

	private void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<LocaleChangedMessage>(onLocaleChanged);
		AppShell.Instance.EventMgr.RemoveListener<GUICaptureMessage>(onCaptureMessage);
		AppShell.Instance.EventMgr.RemoveListener<GUIDragBeginMessage>(onDragDropMode);
		AppShell.Instance.EventMgr.RemoveListener<WindowStatusMessage>(onWindowStatus);
		AppShell.Instance.OnOldControllerUnloading -= OnOldControllerUnloading;
		AppShell.Instance.OnNewControllerLoading -= OnNewControllerLoading;
		AppShell.Instance.OnNewControllerReady -= OnNewControllerReady;
	}

	private static void onStringTableLoaded(StringTableLoadedMessage message)
	{
		Instance.OnInitializationStep(InitializationState.StringTableLoaded);
	}

	private static void onFontBankLoaded(FontBankLoadedMessage message)
	{
		Instance.OnInitializationStep(InitializationState.FontBankLoaded);
	}

	private void onLocaleChanged(LocaleChangedMessage message)
	{
		initializationState = InitializationState.AssetManifestLoaded;
	}

	private void OnOldControllerUnloading(AppShell.GameControllerTypeData currentGameData, AppShell.GameControllerTypeData newGameData)
	{
		if (newGameData.windowName == null)
		{
			CspUtils.DebugLog("Transition to new area, but no GUI specified. is this really your intent?");
			return;
		}
		if (currentState == ModalStateEnum.Transition)
		{
			CspUtils.DebugLog("Asking to transition UI from: " + currentGameData.sceneName + " to " + newGameData.sceneName + " when transition is already in progress.");
			return;
		}
		currentState = ModalStateEnum.Transition;
		if (Instance.Root != null)
		{
			Instance.Root.OnSceneLeave(currentGameData, newGameData);
		}
		if (currentMode != 0)
		{
			if (currentDragDropInfo != null)
			{
				currentDragDropInfo.Result = DragDropResult.Interrupted;
			}
			setInputMode(InputModeEnum.Normal, null);
		}
	}

	private static void OnNewControllerLoading(AppShell.GameControllerTypeData newGameData, GameController controller)
	{
		Instance.Root.OnSceneEnter(newGameData);
		Instance.Transition(newGameData.windowName, controller.StartTransaction);
	}

	private void OnNewControllerReady(AppShell.GameControllerTypeData newGameData, GameController controller)
	{
		currentState = ModalStateEnum.Normal;
		Instance.Root.OnSceneLoaded(newGameData);
	}

	private void onDragDropMode(GUIDragBeginMessage message)
	{
		if (currentState != 0)
		{
			CspUtils.DebugLog("Asked to enter drag drop mode, but not in a standard UI state");
			return;
		}
		if (currentMode == InputModeEnum.DragDrop)
		{
			CspUtils.DebugLog("Asked to enter drag drop mode when already in drag drop mode...");
		}
		currentDragDropInfo = message.DragDropInfo;
		currentDragDropInfo.Result = DragDropResult.Pending;
		if (currentDragDropInfo.SourceType == DragDropSourceType.UI)
		{
			if (currentDragDropInfo.UISource == null)
			{
				CspUtils.DebugLog("for drag drop ops that begin via UI, must provide source GUIControl.");
				return;
			}
			currentDragDropInfo.UISource.SetDragInfo(currentDragDropInfo);
			currentDragDropInfo.UISource.OnDragBegin(currentDragDropInfo);
		}
		CursorManager.SetCursorForDragDropMode(new GUICursor(currentDragDropInfo.IconSource, Vector2.zero));
		CursorManager.DragDropCursorSize = currentDragDropInfo.IconSize;
		setInputMode(InputModeEnum.DragDrop, null);
	}

	private void onDragDropCancelled(SHSKeyCode code)
	{
		currentDragDropInfo.Result = DragDropResult.Cancelled;
		AppShell.Instance.EventMgr.Fire(this, new GUIDragEndMessage(currentDragDropInfo));
	}

	private void onDragDropEnd(GUIDragEndMessage message)
	{
		currentDragDropInfo = message.DragDropInfo;
		setInputMode(InputModeEnum.Normal, null);
		if (currentDragDropInfo.SourceType == DragDropSourceType.UI)
		{
			currentDragDropInfo.UISource.OnDragEnd(currentDragDropInfo);
		}
		if (currentDragDropInfo.Result != DragDropResult.Cancelled && currentDragDropInfo.TargetType == DragDropSourceType.UI)
		{
			currentDragDropInfo.UITarget.OnDragEnd(currentDragDropInfo);
		}
	}

	private void onCaptureMessage(GUICaptureMessage message)
	{
		if (currentState != 0)
		{
		}
		setInputMode(InputModeEnum.Capture, message.Handler);
	}

	private void onWindowStatus(WindowStatusMessage wsEvent)
	{
		if (!wsEvent.isActive && CursorManager != null)
		{
			CursorManager.SetCursorType(GUICursorManager.CursorType.Native);
			SHSInput.SetInputBlockingMode(this, SHSInput.InputBlockType.BlockExternal);
		}
		else if (wsEvent.isActive && CursorManager != null)
		{
			CursorManager.SetCursorType(GUICursorManager.CursorType.Normal);
			SHSInput.RevertInputBlockingMode(this);
		}
	}

	private void Update()
	{
		if (styleManager != null)
		{
			styleManager.Update();
			Root.Update();
			uiRoots[UILayer.Debug].Update();
			uiRoots[UILayer.System].Update();
			uiRoots[UILayer.Tooltip].Update();
			uiRoots[UILayer.Notification].Update();
			if (tooltipManager != null)
			{
				tooltipManager.Update();
			}
			if (currentMode == InputModeEnum.DragDrop)
			{
				updateDragDropState();
			}
			if (mouseoverEnemyIndicator != null)
			{
				mouseoverEnemyIndicator.transform.rotation = Quaternion.identity;
			}
			diagnosticsManager.Update();
		}
	}

	private void updateDragDropState()
	{
		if (SHSInput.IsOverUI())
		{
			CursorManager.SetCursorType(GUICursorManager.CursorType.DragDrop);
			if (currentOverControl == null || !currentOverControl.CanDrop(currentDragDropInfo))
			{
				return;
			}
			currentOverControl.OnDragOver(currentDragDropInfo);
			if (SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Left, (IInputHandler)currentOverControl))
			{
				currentDragDropInfo.TargetType = DragDropSourceType.UI;
				currentDragDropInfo.UITarget = currentOverControl;
				currentDragDropInfo.Result = DragDropResult.UIDropped;
				if (currentDragDropInfo.SourceType == DragDropSourceType.UI && ((IInputHandler)currentOverControl).IsDescendantHandler((IInputHandler)currentDragDropInfo.UISource))
				{
					currentDragDropInfo.Result = DragDropResult.Cancelled;
				}
				AppShell.Instance.EventMgr.Fire(this, new GUIDragEndMessage(currentDragDropInfo));
			}
		}
		else
		{
			CursorManager.SetCursorType(GUICursorManager.CursorType.None);
		}
	}

	private void OnGUI()
	{
		//CspUtils.DebugLog(UnityEngine.StackTraceUtility.ExtractStackTrace ());

		GUI.depth = 0;
		if (Instance != null && Instance.StyleManager != null)
		{
			//CspUtils.DebugLog("Instance.StyleManager=" + Instance.StyleManager);
			//CspUtils.DebugLog("Instance.StyleManager.GetSkin(rootSkin)=" + Instance.StyleManager.GetSkin("rootSkin"));
			GUI.skin = Instance.StyleManager.GetSkin("rootSkin").skin;
		}
		if (focusManager == null)
		{
			return;
		}
		GUIUtility.ScaleAroundPivot(new Vector2(CurrentScale, CurrentScale), default(Vector2));
		focusManager.preprocessCleanup();
		currentEventType = Event.current.type;
		bool flag = currentEventType == EventType.Layout || currentEventType == EventType.Layout;
		if (!flag)
		{
			uiDrawframeCount++;
			uiInputStackCount = 0;
			currentOverControl = null;
		}
		GUI.BeginGroup(ScreenRect);
		if (DrawingEnabled)
		{
			uiRoots[UILayer.Main].InitiateDraw(GUIControl.DrawModeSetting.NormalMode);
			uiRoots[UILayer.System].InitiateDraw(GUIControl.DrawModeSetting.NormalMode);
			for (int i = 0; i < alwaysTopContainers.Count; i++)
			{
				IGUIControl iGUIControl = alwaysTopContainers[i];
				iGUIControl.DrawPreprocess();
				iGUIControl.InitiateDraw(GUIControl.DrawModeSetting.AlwaysOnTopMode);
				iGUIControl.DrawFinalize();
			}
			uiRoots[UILayer.Notification].InitiateDraw(GUIControl.DrawModeSetting.NormalMode);
			uiRoots[UILayer.Tooltip].InitiateDraw(GUIControl.DrawModeSetting.NormalMode);
			if (!flag && AppShell.Instance.DebugOptions.GetSettingAsBool("UI_DEBUG_DRAW"))
			{
				uiRoots[UILayer.Main].DebugDraw(debugDrawFlags);
				uiRoots[UILayer.System].DebugDraw(debugDrawFlags);
				uiRoots[UILayer.Notification].DebugDraw(debugDrawFlags);
				uiRoots[UILayer.Tooltip].DebugDraw(debugDrawFlags);
				GUI.Label(new Rect(0f, 0f, 150f, 40f), "Debug Drawing Mode");
				if (AppShell.Instance.DebugOptions.GetSettingAsBool("UI_SHOW_HITTEST_REGIONS"))
				{
					foreach (KeyValuePair<IGUIHitTestable, Color> blockTestControl in BlockTestControls)
					{
						if (canDrawControl((GUIControl)blockTestControl.Key))
						{
							((GUIControl)blockTestControl.Key).DebugDrawTheBlockTestControl();
						}
					}
				}
			}
		}
		uiRoots[UILayer.Debug].InitiateDraw(GUIControl.DrawModeSetting.NormalMode);
		CursorManager.UpdateCursor();
		GUI.EndGroup();
		if (!flag)
		{
			postProcessMouseInput();
			postProcessKeyInput();
		}
	}

	private void postProcessMouseInput()
	{
		if (focusManager.postProcessFlag)
		{
			focusManager.postProcessInput();
		}
		tooltipManager.CurrentOverControl = currentOverControl;
		List<IGUIControl> list = new List<IGUIControl>();
		SHSKeyCode sHSKeyCode = new SHSKeyCode();
		for (int num = inputStack.Count - 1; num >= 0; num--)
		{
			InputState inputState = inputStateLookup[inputStack[num]];
			if (inputState.frameUpdated != uiDrawframeCount)
			{
				list.Add(inputStack[num]);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			inputStateLookup[list[i]].inputEventType = InputEventStateEnum.MouseOut;
			inputStateLookup[list[i]].registrationInfo.mouseOutCallback(list[i], InputEventStateEnum.MouseOut);
			inputStack.Remove(list[i]);
		}
		if (!AppShell.Instance.HasFocus)
		{
			return;
		}
		for (int num2 = inputStack.Count - 1; num2 >= 0; num2--)
		{
			InputState inputState = inputStateLookup[inputStack[num2]];
			if (inputState.eventHandlingType == GUIControl.ControlTraits.EventHandlingEnum.Block && !inputState.handledMouseOver)
			{
				inputState.frameAdded = inputState.frameUpdated;
				inputState.frameAdded = uiDrawframeCount;
				inputStateLookup[inputStack[num2]] = inputState;
				for (int num3 = num2 - 1; num3 >= 0; num3--)
				{
					inputState = inputStateLookup[inputStack[num3]];
					inputState.handledMouseOver = false;
					inputStateLookup[inputStack[num3]] = inputState;
				}
				num2 = 0;
			}
		}
		for (int num4 = inputStack.Count - 1; num4 >= 0; num4--)
		{
			InputState inputState = inputStateLookup[inputStack[num4]];
			if (inputState.frameAdded == inputState.frameUpdated && uiDrawframeCount == inputState.frameAdded)
			{
				inputState.inputEventType = InputEventStateEnum.MouseOver;
				inputState.handledMouseOver = true;
				inputState.registrationInfo.mouseOverCallback(inputStack[num4], InputEventStateEnum.MouseOver);
				inputStateLookup[inputStack[num4]] = inputState;
			}
			if (inputState.eventHandlingType == GUIControl.ControlTraits.EventHandlingEnum.Block)
			{
				num4 = 0;
			}
		}
		if (currentEventType == EventType.ScrollWheel || currentEventType == EventType.ScrollWheel)
		{
			float mouseWheelDelta = SHSInput.GetMouseWheelDelta(this);
			int direction = (!(mouseWheelDelta < 0f)) ? 1 : (-1);
			mouseWheelDelta = Mathf.Abs(mouseWheelDelta);
			for (int num5 = inputStack.Count - 1; num5 >= 0; num5--)
			{
				bool flag = true;
				IGUIControl iGUIControl = inputStack[num5];
				GUIControl source = (GUIControl)iGUIControl;
				isLeftClickEventHandled = false;
				sHSKeyCode.source = source;
				sHSKeyCode.code = (KeyCode)SHSInput.MouseWheelValue;
				inputStateLookup[iGUIControl].inputEventType = InputEventStateEnum.MouseWheel;
				if ((SHSInput.BlockType & SHSInput.InputBlockType.CaptureMode) != 0 && !SHSInput.IsCaptureAllowingInput(sHSKeyCode))
				{
					flag = false;
				}
				if (flag)
				{
					inputStateLookup[iGUIControl].registrationInfo.mouseWheelCallback(iGUIControl, mouseWheelDelta, direction);
				}
				if (inputStateLookup[iGUIControl].eventHandlingType == GUIControl.ControlTraits.EventHandlingEnum.Block)
				{
					num5 = 0;
				}
			}
		}
		if (currentEventType == EventType.MouseDown || currentEventType == EventType.MouseDown)
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(0);
			bool mouseButtonDown2 = Input.GetMouseButtonDown(1);
			for (int num6 = inputStack.Count - 1; num6 >= 0; num6--)
			{
				bool flag = true;
				IGUIControl iGUIControl = inputStack[num6];
				GUIControl source = (GUIControl)iGUIControl;
				isLeftClickEventHandled = false;
				isRightClickEventHandled = false;
				inputStateLookup[iGUIControl].inputEventType = InputEventStateEnum.MouseDown;
				if ((SHSInput.BlockType & SHSInput.InputBlockType.CaptureMode) != 0)
				{
					sHSKeyCode.source = source;
					sHSKeyCode.originOfRequest = this;
					if (mouseButtonDown)
					{
						sHSKeyCode.code = KeyCode.Mouse0;
						if (!SHSInput.IsCaptureAllowingInput(sHSKeyCode))
						{
							flag = false;
						}
					}
					if (mouseButtonDown2)
					{
						sHSKeyCode.code = KeyCode.Mouse1;
						if (!SHSInput.IsCaptureAllowingInput(sHSKeyCode))
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					if (mouseButtonDown)
					{
						inputStateLookup[iGUIControl].registrationInfo.mouseDownCallback(iGUIControl, InputEventStateEnum.MouseDown);
					}
					if (mouseButtonDown2)
					{
						inputStateLookup[iGUIControl].registrationInfo.rightMouseDownCallback(iGUIControl, InputEventStateEnum.MouseDown);
					}
				}
				if (isLeftClickEventHandled || isRightClickEventHandled)
				{
					inputStateLookup[iGUIControl].hadLeftDownEvent = isLeftClickEventHandled;
					inputStateLookup[iGUIControl].hadRightDownEvent = isRightClickEventHandled;
					mouseDownList.Add(iGUIControl);
				}
				if (inputStateLookup[iGUIControl].eventHandlingType == GUIControl.ControlTraits.EventHandlingEnum.Block)
				{
					num6 = 0;
				}
			}
		}
		if (currentEventType != EventType.MouseUp && currentEventType != EventType.MouseUp)
		{
			return;
		}
		bool mouseButtonUp = Input.GetMouseButtonUp(0);
		bool mouseButtonUp2 = Input.GetMouseButtonUp(1);
		List<IGUIControl> list2 = new List<IGUIControl>();
		for (int j = 0; j < mouseDownList.Count; j++)
		{
			inputStateLookup[mouseDownList[j]].inputEventType = InputEventStateEnum.MouseUp;
			if (mouseButtonUp)
			{
				inputStateLookup[mouseDownList[j]].registrationInfo.mouseUpCallback(mouseDownList[j], InputEventStateEnum.MouseUp);
			}
			if (mouseButtonUp2)
			{
				inputStateLookup[mouseDownList[j]].registrationInfo.rightMouseUpCallback(mouseDownList[j], InputEventStateEnum.MouseUp);
			}
			bool flag2 = inputStateLookup[mouseDownList[j]].hadLeftDownEvent && !mouseButtonUp;
			bool flag3 = inputStateLookup[mouseDownList[j]].hadRightDownEvent && !mouseButtonUp2;
			if (flag2 || flag3)
			{
				list2.Add(mouseDownList[j]);
			}
		}
		mouseDownList.Clear();
		foreach (IGUIControl item in list2)
		{
			mouseDownList.Add(item);
		}
		list2.Clear();
		if (mouseButtonUp && leftMouseDownFlag)
		{
			leftMouseDownFlag = false;
		}
		if (mouseButtonUp2 && rightMouseDownFlag)
		{
			rightMouseDownFlag = false;
		}
	}

	private void postProcessKeyInput()
	{
		if (clearKeyboardFocusOnNextFrame)
		{
			GUIUtility.keyboardControl = 0;
			clearKeyboardFocusOnNextFrame = false;
			setKeyboardFocusOnNextFrameTo = null;
		}
		else if (setKeyboardFocusOnNextFrameTo != null)
		{
			GUI.FocusControl(setKeyboardFocusOnNextFrameTo);
			setKeyboardFocusOnNextFrameTo = null;
		}
	}

	public void SetModal(IGUIControl control, GUIControl.ModalLevelEnum modalLevel)
	{
		SetModal(control, modalLevel, null);
	}

	public void SetModal(IGUIControl control, GUIControl.ModalLevelEnum modalLevel, ModalNotificationDelegate callback)
	{
		if (modalLevel != GUIControl.ModalLevelEnum.None)
		{
			if (modalWindowList.Contains(control))
			{
				CspUtils.DebugLog("Control: " + control.Id + " attempting to make itself modal when its already modal. That's wicked weird.");
				return;
			}
			setLayersToDisabledModalState(modalLevel, true, true);
			control.SetControlFlag(GUIControl.ControlFlagSetting.DisabledByModal, false, true);
			modalWindowList.Add(new ModalGUIControlInfo(control, modalLevel));
			if (!alwaysTopContainers.Contains(control))
			{
				alwaysTopContainers.Add(control);
				control.SetControlFlag(GUIControl.ControlFlagSetting.DrawOnTop, true, false);
			}
			control.SetControlFlag(GUIControl.ControlFlagSetting.IsModal, true, true);
			currentState = ModalStateEnum.Modal;
			return;
		}
		if (!modalWindowList.Contains(control))
		{
			CspUtils.DebugLog("Control: " + control.Id + " attempting to remove itself from modality. but its not modal. That's crazy talk.");
			return;
		}
		if (alwaysTopContainers.Contains(control))
		{
			alwaysTopContainers.Remove(control);
			control.SetControlFlag(GUIControl.ControlFlagSetting.DrawOnTop, false, false);
		}
		modalWindowList.Remove(control);
		if (modalWindowList.Count == 0)
		{
			setLayersToDisabledModalState(GUIControl.ModalLevelEnum.Full, false, true);
			currentState = ModalStateEnum.Normal;
		}
		else
		{
			setLayersToDisabledModalState(GUIControl.ModalLevelEnum.Full, false, true);
			ModalGUIControlInfo modalGUIControlInfo = modalWindowList[modalWindowList.Count - 1];
			IGUIControl modalControl = modalGUIControlInfo.modalControl;
			setLayersToDisabledModalState(modalGUIControlInfo.modalLevel, true, true);
			modalControl.SetControlFlag(GUIControl.ControlFlagSetting.DisabledByModal, false, true);
			modalControl.SetControlFlag(GUIControl.ControlFlagSetting.IsModal, true, true);
		}
		control.SetControlFlag(GUIControl.ControlFlagSetting.IsModal, false, true);
	}

	private void ClearModalHistory()
	{
		List<ModalGUIControlInfo> list = new List<ModalGUIControlInfo>(modalWindowList.ToArray());
		foreach (ModalGUIControlInfo item in list)
		{
			SetModal(item.modalControl, GUIControl.ModalLevelEnum.None);
		}
		list.Clear();
		modalWindowList.Clear();
		alwaysTopContainers.Clear();
		setLayersToDisabledModalState(GUIControl.ModalLevelEnum.Full, false, true);
	}

	public List<string> GetModalNameList()
	{
		List<string> list = new List<string>();
		foreach (ModalGUIControlInfo modalWindow in modalWindowList)
		{
			list.Add(modalWindow.modalControl.Id);
		}
		return list;
	}

	private void setLayersToDisabledModalState(GUIControl.ModalLevelEnum modalLevel, bool modal, bool applyToChildren)
	{
		if (modalLevel == GUIControl.ModalLevelEnum.None)
		{
			CspUtils.DebugLog("Request to disable layers in a given modal state of none.");
		}
		else
		{
			foreach (IGUIContainer value in UIRoots.Values)
			{
				if (value.Layer != 0 && value.Layer != UILayer.Tooltip && (!modal || modalLevel != GUIControl.ModalLevelEnum.Default || value.Layer != UILayer.Notification))
				{
					value.SetControlFlag(GUIControl.ControlFlagSetting.DisabledByModal, modal, applyToChildren);
				}
			}
		}
	}

	private void OnDrawGizmos()
	{
	}

	public void Transition(string screenName)
	{
		Root.Transition(screenName, null);
	}

	public void Transition(string screenName, TransactionMonitor transaction)
	{
		Root.Transition(screenName, transaction);
	}

	public IGUIContainer GetRoot(UILayer layer)
	{
		return uiRoots[layer];
	}

	public void ClearKeyboardFocus()
	{
		clearKeyboardFocusOnNextFrame = true;
	}

	public void SetKeyboardFocus(string controlName)
	{
		setKeyboardFocusOnNextFrameTo = controlName;
	}

	protected void createMouseoverEnemyIndicator()
	{
		UnityEngine.Object original = Resources.Load("GUI/Targeting/HostileTargetMouseover");
		mouseoverEnemyIndicator = (UnityEngine.Object.Instantiate(original) as GameObject);
		mouseoverEnemyIndicator.active = false;
	}

	protected void createAttackingEnemyIndicator()
	{
		UnityEngine.Object original = Resources.Load("GUI/Targeting/HostileTargetSelection");
		attackingEnemyIndicator = (UnityEngine.Object.Instantiate(original) as GameObject);
		Utils.ActivateTree(attackingEnemyIndicator, false);
		attackingEnemyIndicatorScript = attackingEnemyIndicator.GetComponent<HostileTargetSelection>();
	}

	private void onTargetedPlayerEffectLoaded(UnityEngine.Object obj, AssetBundle bundle, object extraData)
	{
		targetedPlayerIndicatorPrefab = (obj as GameObject);
		if (obj != null)
		{
			createTargetedPlayerIndicator(extraData as GameObject);
		}
	}

	protected void createTargetedPlayerIndicator(GameObject target)
	{
		if (targetedPlayerIndicatorPrefab == null)
		{
			AppShell.Instance.BundleLoader.LoadAsset("FX/shared_general_fx", "selection greenring", target, onTargetedPlayerEffectLoaded);
			return;
		}
		targetedPlayerIndicator = (UnityEngine.Object.Instantiate(targetedPlayerIndicatorPrefab) as GameObject);
		targetedPlayerIndicatorEmitter = (targetedPlayerIndicator.GetComponent(typeof(ParticleEmitter)) as ParticleEmitter);
		AttachTargetedPlayerIndicator(target);
	}

	protected void createMouseoverThrowableIndicator()
	{
		UnityEngine.Object original = Resources.Load("GUI/Throwable/BrawlerThrowableEffect");
		mouseoverThrowableIndicator = (UnityEngine.Object.Instantiate(original) as GameObject);
		mouseoverThrowableIndicator.active = true;
	}

	public void AttachMouseoverEnemyIndicator(GameObject target)
	{
		if (mouseoverEnemyIndicator == null)
		{
			createMouseoverEnemyIndicator();
		}
		if (!(attackingEnemyIndicator != null) || !(attackingEnemyIndicatorScript.TargetObject == target))
		{
			Utils.ActivateTree(mouseoverEnemyIndicator, true);
			Utils.AttachGameObject(target, mouseoverEnemyIndicator);
		}
	}

	public void AttachAttackingEnemyIndicator(GameObject target)
	{
		if (attackingEnemyIndicator == null)
		{
			createAttackingEnemyIndicator();
		}
		if (attackingEnemyIndicatorScript.TargetObject != target)
		{
			Utils.ActivateTree(attackingEnemyIndicator, true);
			attackingEnemyIndicatorScript.TargetEntity(target);
		}
	}

	public void AttachTargetedPlayerIndicator(GameObject target)
	{
		if (targetedPlayerIndicator == null)
		{
			createTargetedPlayerIndicator(target);
			return;
		}
		Utils.ActivateTree(targetedPlayerIndicator, true);
		Utils.AttachGameObject(target, targetedPlayerIndicator);
		targetedPlayerIndicator.transform.rotation = Quaternion.identity;
		if (targetedPlayerIndicatorEmitter != null)
		{
			targetedPlayerIndicatorEmitter.Emit();
		}
	}

	public void AttachMouseoverThrowableIndicator(GameObject target)
	{
		if (mouseoverThrowableIndicator == null)
		{
			createMouseoverThrowableIndicator();
		}
		mouseoverThrowableIndicator.active = true;
		Utils.AttachGameObject(target, mouseoverThrowableIndicator);
		BrawlerThrowableEffect component = mouseoverThrowableIndicator.GetComponent<BrawlerThrowableEffect>();
		component.Attach(target);
	}

	public void AttachHealthBarIndicator(GameObject target)
	{
		SHSBrawlerMainWindow sHSBrawlerMainWindow = (SHSBrawlerMainWindow)Instance["/SHSMainWindow/SHSBrawlerMainWindow"];
		if (sHSBrawlerMainWindow != null && sHSBrawlerMainWindow.IsVisible)
		{
			sHSBrawlerMainWindow.AttachHealthBar(target);
		}
	}

	public void DetachMouseoverIndicator()
	{
		if (mouseoverEnemyIndicator != null && mouseoverEnemyIndicator.active)
		{
			Utils.DetachGameObject(mouseoverEnemyIndicator);
			Utils.ActivateTree(mouseoverEnemyIndicator, false);
		}
		if (mouseoverThrowableIndicator != null && mouseoverThrowableIndicator.active)
		{
			Utils.DetachGameObject(mouseoverThrowableIndicator);
			Utils.ActivateTree(mouseoverThrowableIndicator, false);
		}
	}

	public void DetachAttackingIndicator()
	{
		if (attackingEnemyIndicator != null && attackingEnemyIndicator.active)
		{
			attackingEnemyIndicatorScript.TargetEntity(null);
			Utils.ActivateTree(attackingEnemyIndicator, false);
		}
	}

	public GameObject GetTargetedEnemy()
	{
		return (!(attackingEnemyIndicatorScript != null)) ? null : attackingEnemyIndicatorScript.TargetObject;
	}

	public void DetachTargetedPlayerIndicator()
	{
		if (targetedPlayerIndicator != null && targetedPlayerIndicator.active)
		{
			Utils.DetachGameObject(targetedPlayerIndicator);
			Utils.ActivateTree(targetedPlayerIndicator, false);
			if (targetedPlayerIndicatorEmitter != null)
			{
				targetedPlayerIndicatorEmitter.ClearParticles();
			}
		}
	}

	public void DetachHealthBarIndicator()
	{
		SHSBrawlerMainWindow sHSBrawlerMainWindow = (SHSBrawlerMainWindow)Instance["/SHSMainWindow/SHSBrawlerMainWindow"];
		if (sHSBrawlerMainWindow != null && sHSBrawlerMainWindow.IsVisible)
		{
			sHSBrawlerMainWindow.DetachHealthBar();
		}
	}

	public void TransferHealthBarIndicator(GameObject owner, GameObject target)
	{
		SHSBrawlerMainWindow sHSBrawlerMainWindow = (SHSBrawlerMainWindow)Instance["/SHSMainWindow/SHSBrawlerMainWindow"];
		if (sHSBrawlerMainWindow != null && sHSBrawlerMainWindow.IsVisible)
		{
			sHSBrawlerMainWindow.TransferHealthBar(owner, target);
		}
	}

	public void LoadTexture(string texturePath, AssetBundleLoader.AssetLoadedCallback callback, object extraData)
	{
		if (texturePath.StartsWith("WEBCACHE$"))
		{
			string text = texturePath.Replace("WEBCACHE$", string.Empty);
			string[] array = text.Split('#');
			text = array[0];
			ShsWebResponse value;
			if (AppShell.Instance.WebAssetCache.CachedWebAssets.TryGetValue(text, out value))
			{
				if (callback != null)
				{
					callback(value.Texture, null, null);
				}
			}
			else if (array.Length > 1)
			{
				LoadTexture(array[1], callback, extraData);
			}
			else if (callback != null)
			{
				callback((Texture2D)Resources.Load(getMissingTextureImage()), null, null);
			}
		}
		else if (Regex.IsMatch(texturePath, ".*\\|{1}"))
		{
			string[] array2 = texturePath.Split('|');
			string text2 = array2[0];
			string bundleAsset = array2[1];
			if (bundleManager.IsBundleLoaded(text2))
			{
				bundleManager.LoadAsset(text2, bundleAsset, extraData, delegate(UnityEngine.Object obj, AssetBundle bundle, object innerExtraData)
				{
					diagnosticsManager.TextureLoaded(texturePath, (Texture2D)obj);
					if (callback != null)
					{
						callback(obj, bundle, innerExtraData);
					}
				});
			}
			else
			{
				CspUtils.DebugLog("Bundle: " + text2 + " is not loaded, so textures can not be obtained.");
			}
		}
		else if (Regex.IsMatch(texturePath, "http[s]*\\:\\/\\/.+"))
		{
			AppShell.Instance.WebService.StartRequest(texturePath, delegate(ShsWebResponse response)
			{
				onWebFetchComplete(response, callback);
			}, ShsWebService.ShsWebServiceType.Texture);
		}
	}

	private void onWebFetchComplete(ShsWebResponse response, AssetBundleLoader.AssetLoadedCallback callback)
	{
		if (response.Status != 200)
		{
			CspUtils.DebugLog("image not retrievable:" + response.OriginalUri);
			callback((Texture2D)Resources.Load(getMissingTextureImage()), null, null);
		}
		else
		{
			callback(response.Texture, null, null);
		}
	}

	public Texture2D LoadTexture(string texturePath)
	{
		Texture2D texture;
		LoadTexture(texturePath, out texture);
		return texture;
	}

	public bool LoadTexture(string texturePath, out Texture2D texture)
	{
		//CspUtils.DebugLog("texturePath=" + texturePath);

		if (texturePath.StartsWith("WEBCACHE$"))
		{
			string text = texturePath.Replace("WEBCACHE$", string.Empty);
			string[] array = text.Split('#');
			text = array[0];
			ShsWebResponse value;
			if (AppShell.Instance.WebAssetCache.CachedWebAssets.TryGetValue(text, out value))
			{
				texture = value.Texture;
				return true;
			}
			if (array.Length > 1)
			{
				return LoadTexture(array[1], out texture);
			}
			texture = (Texture2D)Resources.Load(getMissingTextureImage());
			return false;
		}
		if (Regex.IsMatch(texturePath, ".*\\|{1}"))
		{
			string[] array2 = texturePath.Split('|');
			string text2 = array2[0];
			string bundleAsset = array2[1];
			if (bundleManager.IsBundleLoaded(text2))
			{
				//CspUtils.DebugLog("text2=" + text2);
				texture = bundleManager.Load<Texture2D>(text2, bundleAsset);
				if (texture == null)
				{
					texture = (Texture2D)Resources.Load(getMissingTextureImage());
					return false;
				}
				diagnosticsManager.TextureLoaded(texturePath, texture);
				return true;
			}
			CspUtils.DebugLog("Attempting to load: " + texturePath + ", but the bundle (" + text2 + ") is not loaded.");
			texture = (Texture2D)Resources.Load(getMissingTextureImage());
			return false;
		}
		if (Regex.IsMatch(texturePath, ".*[\\\\|\\/]+.*"))
		{
			texture = (Texture2D)Resources.Load(texturePath);
			if (texture == null)
			{
				texture = (Texture2D)Resources.Load(getMissingTextureImage());
				diagnosticsManager.TextureLoaded(texturePath, texture);
				return false;
			}
			return true;
		}
		CspUtils.DebugLog("Unrecognized format for texture loading: " + texturePath);
		texture = (Texture2D)Resources.Load(getMissingTextureImage());
		return false;
	}

	public EffectSequence LoadEffectSequence(string sequencePath)
	{
		EffectSequence sequence;
		LoadEffectSequence(sequencePath, out sequence);
		return sequence;
	}

	public bool LoadEffectSequence(string sequencePath, out EffectSequence sequence)
	{
		if (sequencePath.Contains("|"))
		{
			string[] array = sequencePath.Split('|');
			string bundleName = array[0];
			string bundleAsset = array[1];
			if (bundleManager.IsBundleLoaded(bundleName))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(bundleManager.Load<GameObject>(bundleName, bundleAsset)) as GameObject;
				sequence = (gameObject.GetComponent(typeof(EffectSequence)) as EffectSequence);
				if (sequence == null)
				{
					sequence = null;
					return false;
				}
				return true;
			}
			sequence = null;
			return false;
		}
		if (Regex.IsMatch(sequencePath, ".*[\\\\|\\/]+.*"))
		{
			sequence = (EffectSequence)Resources.Load(sequencePath);
			if (sequence == null)
			{
				sequence = null;
				return false;
			}
			return true;
		}
		sequence = null;
		return false;
	}

	public MovieTexture LoadMovieTexture(string texturePath)
	{
		MovieTexture texture;
		LoadMovieTexture(texturePath, out texture);
		return texture;
	}

	public bool LoadMovieTexture(string texturePath, out MovieTexture texture)
	{
		if (Regex.IsMatch(texturePath, ".*\\|{1}"))
		{
			string[] array = texturePath.Split('|');
			string text = array[0];
			string bundleAsset = array[1];
			if (bundleManager.IsBundleLoaded(text))
			{
				texture = bundleManager.Load<MovieTexture>(text, bundleAsset);
				if (texture == null)
				{
					return Resources.Load(getMissingTextureImage()) != null;
				}
				return true;
			}
			CspUtils.DebugLog("Attempting to load: " + texturePath + ", but the bundle (" + text + ") is not loaded.");
			texture = (MovieTexture)Resources.Load(getMissingTextureImage());
			return false;
		}
		if (Regex.IsMatch(texturePath, ".*[\\\\|\\/]+.*"))
		{
			texture = (MovieTexture)Resources.Load(texturePath);
			if (texture == null)
			{
				texture = (MovieTexture)Resources.Load(getMissingTextureImage());
				return false;
			}
			return true;
		}
		CspUtils.DebugLog("Unrecognized format for texture loading: " + texturePath);
		texture = (MovieTexture)Resources.Load(getMissingTextureImage());
		return false;
	}

	public void LoadMovieTexture(string texturePath, AssetBundleLoader.AssetLoadedCallback callback, object extraData)
	{
		if (Regex.IsMatch(texturePath, ".*\\|{1}"))
		{
			string[] array = texturePath.Split('|');
			string text = array[0];
			string bundleAsset = array[1];
			if (bundleManager.IsBundleLoaded(text))
			{
				bundleManager.LoadAsset(text, bundleAsset, extraData, delegate(UnityEngine.Object obj, AssetBundle bundle, object innerExtraData)
				{
					if (callback != null)
					{
						callback(obj, bundle, innerExtraData);
					}
				});
			}
			else
			{
				CspUtils.DebugLog("Bundle: " + text + " is not loaded, so movie can not be obtained.");
			}
		}
	}

	public void SetScreenBackColor(UILayer layer, Color color)
	{
		uiRoots[layer].SetBackground(color);
	}

	public void SetScreenBackColor(UILayer layer, bool off)
	{
		uiRoots[layer].SetBackground(off);
	}

	public void SetScreenBackColor(Color color)
	{
		Root.SetBackground(color);
	}

	public void SetScreenBackColor(bool off)
	{
		Root.SetBackground(off);
	}

	public void SetScreenBackImage(string textureSource)
	{
		Root.SetBackground(textureSource);
	}

	public void RegisterGUIElementLeftClickEventResult(bool isHandled, GUIControl.ControlTraits.EventHandlingEnum eventState)
	{
		isLeftClickEventHandled = isHandled;
		currentEventState = eventState;
	}

	public void RegisterGUIElementRightClickEventResult(bool isHandled, GUIControl.ControlTraits.EventHandlingEnum eventState)
	{
		isRightClickEventHandled = isHandled;
		currentEventState = eventState;
	}

	public void RegisterGUIElementForEvents(IGUIControl control, GUIEventRegistrationInfo registrationInfo)
	{
		if (!inputStack.Contains(control))
		{
			inputStack.Insert(uiInputStackCount, control);
			InputState inputState = new InputState(uiDrawframeCount, uiInputStackCount, registrationInfo);
			inputState.eventHandlingType = control.Traits.EventHandlingTrait;
			inputStateLookup[control] = inputState;
			for (int i = uiInputStackCount + 1; i < inputStack.Count; i++)
			{
				inputStateLookup[inputStack[i]].stackIndex = i;
			}
		}
		else
		{
			InputState inputState;
			if (!inputStateLookup.TryGetValue(control, out inputState))
			{
				CspUtils.DebugLog("Control: " + control.Id + " Is in the input stack, but has no input state object. Bad.");
				return;
			}
			if (inputState.frameUpdated == uiDrawframeCount)
			{
				return;
			}
			if (inputState.stackIndex != uiInputStackCount)
			{
				inputState.stackIndex = uiInputStackCount;
				inputStack.Remove(control);
				inputStack.Insert(uiInputStackCount, control);
				for (int j = uiInputStackCount + 1; j < inputStack.Count; j++)
				{
					inputStateLookup[inputStack[j]].stackIndex = j;
				}
			}
			inputState.frameUpdated = uiDrawframeCount;
		}
		uiInputStackCount++;
	}

	public void RegisterGUIOver(IGUIControl control)
	{
		currentOverControl = control;
	}

	public virtual Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> GetKeyList(GUIControl.KeyInputState inputState)
	{
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

	public void ShowDialog(DialogTypeEnum dialogType, string text, GUIDialogNotificationSink.DialogEventNotificationDelegate closedDelegate, GUIControl.ModalLevelEnum Modal)
	{
		ShowDialog(dialogType, text, new GUIDialogNotificationSink(closedDelegate), Modal);
	}

	public void ShowDialog(DialogTypeEnum dialogType, string text, IGUIDialogNotification notifySink, GUIControl.ModalLevelEnum Modal)
	{
		switch (dialogType)
		{
		case DialogTypeEnum.OkCancelDialog:
			ShowDialog(typeof(SHSOkCancelDialogWindow), text, notifySink, Modal);
			break;
		case DialogTypeEnum.OkDialog:
			ShowDialog(typeof(SHSOkDialogWindow), text, notifySink, Modal);
			break;
		case DialogTypeEnum.ProgressDialog:
			ShowDialog(typeof(SHSProgressDialogWindow), text, notifySink, Modal);
			break;
		case DialogTypeEnum.YesNoDialog:
			ShowDialog(typeof(SHSYesNoDialogWindow), text, notifySink, Modal);
			break;
		case DialogTypeEnum.ErrorDialog:
			ShowDialog(typeof(SHSErrorNotificationWindow), text, notifySink, Modal);
			break;
		case DialogTypeEnum.AwardDialog:
			throw new NotImplementedException();
		}
	}

	public void ShowDialog(DialogTypeEnum dialogType, IGUIDialogNotification notifySink, GUIControl.ModalLevelEnum Modal)
	{
		ShowDialog(dialogType, string.Empty, notifySink, Modal);
	}

	public void ShowDialog(DialogTypeEnum dialogType, GUIDialogNotificationSink.DialogEventNotificationDelegate closedDelegate, GUIControl.ModalLevelEnum Modal)
	{
		ShowDialog(dialogType, string.Empty, new GUIDialogNotificationSink(closedDelegate), Modal);
	}

	public void ShowDialog(Type dialogType, GUIDialogNotificationSink.DialogEventNotificationDelegate closedDelegate, GUIControl.ModalLevelEnum Modal)
	{
		ShowDialog(dialogType, string.Empty, new GUIDialogNotificationSink(closedDelegate), Modal);
	}

	public void ShowDialog(Type dialogType, string text, GUIDialogNotificationSink.DialogEventNotificationDelegate closedDelegate, GUIControl.ModalLevelEnum Modal)
	{
		ShowDialog(dialogType, text, new GUIDialogNotificationSink(closedDelegate), Modal);
	}

	public void ShowDialog(Type dialogType, string text, IGUIDialogNotification notifySink, GUIControl.ModalLevelEnum Modal)
	{
		ShowDialog(dialogType, text, string.Empty, notifySink, Modal);
	}

	public void ShowDialog(Type dialogType, string text, string windowLocation, IGUIDialogNotification notifySink, GUIControl.ModalLevelEnum Modal)
	{
		ShowDynamicWindow(dialogType, text, windowLocation, notifySink, Modal);
	}

	public void ShowDialogWithTitle(DialogTypeEnum dialogType, string titleText, string text, GUIDialogNotificationSink notifySink, GUIControl.ModalLevelEnum Modal)
	{
		ShowDialogWithTitle(dialogType, titleText, text, string.Empty, notifySink, Modal);
	}

	public void ShowDialogWithTitle(DialogTypeEnum dialogType, string titleText, string text, string windowLocation, GUIDialogNotificationSink notifySink, GUIControl.ModalLevelEnum Modal)
	{
		Type dialogType2 = null;
		switch (dialogType)
		{
		case DialogTypeEnum.OkCancelDialog:
			dialogType2 = typeof(SHSOkCancelDialogWindow);
			break;
		case DialogTypeEnum.OkDialog:
			dialogType2 = typeof(SHSOkDialogWindow);
			break;
		case DialogTypeEnum.ProgressDialog:
			dialogType2 = typeof(SHSProgressDialogWindow);
			break;
		case DialogTypeEnum.YesNoDialog:
			dialogType2 = typeof(SHSYesNoDialogWindow);
			break;
		case DialogTypeEnum.ErrorDialog:
			dialogType2 = typeof(SHSErrorNotificationWindow);
			break;
		case DialogTypeEnum.AwardDialog:
			throw new NotImplementedException();
		}
		ShowDynamicWindow(dialogType2, text, titleText, windowLocation, notifySink, Modal);
	}

	public void ShowDynamicWindow(Type dialogType, string text, string windowLocation, IGUIDialogNotification notifySink, GUIControl.ModalLevelEnum Modal)
	{
		ShowDynamicWindow(dialogType, text, string.Empty, windowLocation, notifySink, Modal);
	}

	public void ShowDynamicWindow(Type dialogType, string text, string titleText, string windowLocation, IGUIDialogNotification notifySink, GUIControl.ModalLevelEnum Modal)
	{
		if (!typeof(GUIDynamicWindow).IsAssignableFrom(dialogType))
		{
			CspUtils.DebugLog("Type: " + dialogType + " is not a GUIDynamicWindow subclass. Gandalf is displeased.");
		}
		else if (typeof(IGUIDialogWindow).IsAssignableFrom(dialogType))
		{
			IGUIDialogWindow iGUIDialogWindow = (IGUIDialogWindow)Activator.CreateInstance(dialogType);
			iGUIDialogWindow.Text = text;
			iGUIDialogWindow.TitleText = titleText;
			iGUIDialogWindow.NotificationSink = notifySink;
			GUIDynamicWindow dialogWindow = iGUIDialogWindow as GUIDynamicWindow;
			ShowDynamicWindow(dialogWindow, windowLocation, Modal);
		}
		else
		{
			GUIDynamicWindow dialogWindow2 = (GUIDynamicWindow)Activator.CreateInstance(dialogType);
			ShowDynamicWindow(dialogWindow2, windowLocation, Modal);
		}
	}

	public void ShowDynamicWindow(GUIDynamicWindow dialogWindow, GUIControl.ModalLevelEnum Modal)
	{
		ShowDynamicWindow(dialogWindow, string.Empty, Modal);
	}

	public void ShowDynamicWindow(GUIDynamicWindow dialogWindow, string windowLocation, GUIControl.ModalLevelEnum Modal)
	{
		ShowDynamicWindow(dialogWindow, string.Empty, GUIControl.DrawOrder.DrawFirst, GUIControl.DrawPhaseHintEnum.PreDraw, Modal);
	}

	public void ShowDynamicWindow(GUIDynamicWindow dialogWindow, string windowLocation, GUIControl.DrawOrder DrawOrder, GUIControl.DrawPhaseHintEnum DrawPhase, GUIControl.ModalLevelEnum Modal)
	{
		if (windowLocation == null || (windowLocation == string.Empty && AppShell.Instance.CurrentControllerTypeData != null))
		{
			if (dialogWindow == null)
			{
				CspUtils.DebugLog("Passed in null window for showing... null is unviewable");
			}
			((IGUIContainer)Root[AppShell.Instance.CurrentControllerTypeData.windowName]).Add(dialogWindow, DrawOrder, DrawPhase);
		}
		else
		{
			((IGUIContainer)Root[windowLocation]).Add(dialogWindow, DrawOrder, DrawPhase);
		}
		dialogWindow.Show(Modal);
	}

	public string GetDefaultDialogWindowLocation()
	{
		return AppShell.Instance.CurrentControllerTypeData.windowName;
	}

	public void TogglePersistentGUI(PersistentGUIToggleOption option, bool setting)
	{
		togglePersistentGUINested(Root, option, setting);
	}

	private void togglePersistentGUINested(IGUIControl control, PersistentGUIToggleOption option, bool setting)
	{
		if (control.GetControlFlag(GUIControl.ControlFlagSetting.Persistent))
		{
			switch (option)
			{
			case PersistentGUIToggleOption.Visibility:
				control.SetControlFlag(GUIControl.ControlFlagSetting.ForceInvisible, setting, false);
				break;
			case PersistentGUIToggleOption.Enable:
				control.SetControlFlag(GUIControl.ControlFlagSetting.ForceDisable, setting, false);
				break;
			}
		}
		if (control is IGUIContainer)
		{
			foreach (IGUIControl control2 in ((IGUIContainer)control).ControlList)
			{
				togglePersistentGUINested(control2, option, setting);
			}
		}
	}

	public void ShowErrorDialog(SHSErrorCodes.Response response, string error)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("ShowErrorDialog");
		stringBuilder.AppendLine("  code: " + response.Code.ToString());
		stringBuilder.AppendLine("  number: " + response.Number.ToString());
		stringBuilder.AppendLine("  error: " + (error ?? "none"));
		stringBuilder.AppendLine("  message: " + response.getMessage());
		CspUtils.DebugLog(stringBuilder.ToString());
		if (response.BypassNotification)
		{
			response.ResponseFunction(response.Code);
			return;
		}
		SHSErrorNotificationWindow windowToMake = new SHSErrorNotificationWindow(response.Code);
		CreateErrorDialogWindow(windowToMake, response.getMessage() + " " + error, response);
	}

	public void ShowErrorDialog(SHSErrorCodes.Code code, string serverMessage, string error)
	{
		SHSErrorCodes.Response response = SHSErrorCodes.GetResponse(code);
		SHSErrorNotificationWindow windowToMake = new SHSErrorNotificationWindow(response.Code);
		CreateErrorDialogWindow(windowToMake, serverMessage, response);
	}

	public void ShowErrorDialog(SHSErrorCodes.Code code, string error)
	{
		SHSErrorCodes.Response response = SHSErrorCodes.GetResponse(code);
		SHSErrorNotificationWindow windowToMake = new SHSErrorNotificationWindow(response.Code);
		CreateErrorDialogWindow(windowToMake, SHSErrorCodes.GetResponseMessage(response), response);
	}

	private void CreateErrorDialogWindow(SHSErrorNotificationWindow windowToMake, string errorMessage, SHSErrorCodes.Response response)
	{
		windowToMake.Text = errorMessage;
		windowToMake.TitleText = SHSErrorCodes.GetResponseMessageTitle(response);
		windowToMake.NotificationSink = new GUIDialogNotificationSink(delegate
		{
		}, delegate
		{
		}, delegate
		{
		}, delegate
		{
		}, delegate
		{
			if (response != null)
			{
				response.ResponseFunction(response.Code);
			}
		});
		uiRoots[UILayer.Notification].Add(windowToMake);
		windowToMake.HandleResize(new GUIResizeMessage(GUIControl.HandleResizeSource.Control, new Rect(0f, 0f, 0f, 0f), windowToMake.Rect));
		windowToMake.Show(GUIControl.ModalLevelEnum.Full);
	}

	public GameObject GetUISound(string name)
	{
		UISFX[] array = sounds;
		foreach (UISFX uISFX in array)
		{
			if (uISFX.name == name)
			{
				return uISFX.sfx;
			}
		}
		return null;
	}

	private string getMissingTextureImage()
	{
		return "GUI/fallback/missing_release";
	}

	protected void CreateRootWindow(UILayer layer, string windowName)
	{
		GUIRootWindow gUIRootWindow = new GUIRootWindow();
		gUIRootWindow.Id = windowName;
		gUIRootWindow.Layer = layer;
		uiRoots.Add(layer, gUIRootWindow);
		if (gUIRootWindow.guiTreeNode != null)
		{
			Utils.AttachGameObject(uiTreeGameObject, gUIRootWindow.guiTreeNode);
		}
	}

	protected void Configure(LoadState state)
	{
		Configure(state, null);
	}

	protected void Configure(LoadState state, TransactionMonitor transaction)
	{
		switch (state)
		{
		case LoadState.Awake:
			uiRoots = new Dictionary<UILayer, IGUIContainer>();
			CreateRootWindow(UILayer.Main, "Main Root");
			CreateRootWindow(UILayer.System, "System Root");
			CreateRootWindow(UILayer.Tooltip, "Tooltip Root");
			CreateRootWindow(UILayer.Notification, "Notification Root");
			CreateRootWindow(UILayer.Debug, "Debug Root");
			diagnosticsManager = new GUIDiagnosticManager(this);
			bundleManager = new GUIBundleManager(this, OnBundleLoaded);
			foreach (WindowBinding registeredWindowInit in registeredWindowInitList)
			{
				RegisterWindowForDynamicLoading(registeredWindowInit);
			}
			break;
		case LoadState.Start:
			uiRoots[UILayer.Debug].Add(new SHSDebugWindow());
			uiRoots[UILayer.Tooltip].Add(new SHSTooltip());
			tooltipManager.ToolTipControl = uiRoots[UILayer.Tooltip]["SHSTooltip"];
			((SHSTooltip)uiRoots[UILayer.Tooltip]["SHSTooltip"]).ToolTipResource = tooltipManager.ToolTipResource;
			uiRoots[UILayer.Tooltip].Add(new SHSHoverHelp());
			tooltipManager.HoverHelpLayer = (SHSHoverHelp)uiRoots[UILayer.Tooltip]["SHSHoverHelp"];
			((SHSHoverHelp)uiRoots[UILayer.Tooltip]["SHSHoverHelp"]).ToolTipResource = tooltipManager.ToolTipResource;
			Root.Add(new SHSMainWindow("SHSMainWindow"));
			Root.Add(new SHSFrontEndWindow("SHSFrontendWindow"));
			Root.Add(new SHSBlankWindow("SHSBlankWindow"));
			((IGUIContainer)Root["SHSFrontendWindow"]).Add(new SHSLoginWindow());
			((IGUIContainer)Root["SHSFrontendWindow"]).Add(new SHSSystemMainWindow());
			((IGUIContainer)Root["SHSMainWindow"]).Add(new SHSSystemMainWindow());
			break;
		case LoadState.Restart:
			bundleManager.LocaleChanged(transaction);
			fontManager.InitializeFontBundle();
			break;
		}
	}

	public static void AddWindowToInitRegistry(WindowBinding binding)
	{
		registeredWindowInitList.Add(binding);
	}

	public void RegisterWindowForDynamicLoading(string uiLayerName, string windowName, Type windowType)
	{
		if (!dynWindowTable.ContainsKey(windowName))
		{
			dynWindowTable.Add(windowName, new WindowBinding(windowType, uiLayerName, windowName));
		}
	}

	public void RegisterWindowForDynamicLoading(WindowBinding windowBinding)
	{
		if (!dynWindowTable.ContainsKey(windowBinding.windowName))
		{
			dynWindowTable.Add(windowBinding.windowName, windowBinding);
		}
	}

	public void UnregisterWindowForDynamicLoading(string windowName)
	{
		if (dynWindowTable.ContainsKey(windowName))
		{
			dynWindowTable.Remove(windowName);
		}
	}

	public void HookPreloadedWindowToDynamicTable(string uiLayerName, string windowName, ref bool alreadyDynamic)
	{
		WindowBinding windowBinding = default(WindowBinding);
		if (!dynWindowTable.ContainsKey(windowName))
		{
			windowBinding.uiLayerName = uiLayerName;
			windowBinding.windowName = windowName;
			dynWindowTable.Add(windowBinding.windowName, windowBinding);
			alreadyDynamic = false;
		}
		else
		{
			windowBinding = (WindowBinding)dynWindowTable[windowName];
			alreadyDynamic = true;
		}
		windowBinding.windowRef = ((IGUIContainer)Root[windowBinding.uiLayerName])[windowName];
		dynWindowTable[windowBinding.windowName] = windowBinding;
	}

	public bool IsWindowDynamic(string windowName)
	{
		List<string> list = new List<string>();
		foreach (string key in dynWindowTable.Keys)
		{
			list.Add(key);
		}
		for (int i = 0; i < list.Count; i++)
		{
			WindowBinding windowBinding = (WindowBinding)dynWindowTable[list[i]];
			if (windowBinding.windowName == windowName)
			{
				return true;
			}
		}
		return false;
	}

	public void ForceLoadOnlyTransition(GameController.ControllerType controllerType)
	{
		if (AppShell.Instance.GameControllerInfo.ContainsKey(controllerType))
		{
			AppShell.GameControllerTypeData gameControllerTypeData = AppShell.Instance.GameControllerInfo[controllerType];
			gameControllerTypeData.LoadAllBoundWindows();
			List<string> list = new List<string>();
			foreach (string key in dynWindowLoadingTable.Keys)
			{
				list.Add(key);
			}
			foreach (string key2 in dynSubwindowLoadingTable.Keys)
			{
				list.Add(key2);
			}
			for (int i = 0; i < list.Count; i++)
			{
				LoadWindowOnTransition(list[i]);
			}
			list.Clear();
			dynWindowLoadingTable.Clear();
			dynSubwindowLoadingTable.Clear();
		}
		else
		{
			CspUtils.DebugLog("Invalid Controller Type while trying to force a LoadOnly Transition.");
		}
	}

	private void LoadWindowOnTransition(string windowName)
	{
		if (dynWindowTable.ContainsKey(windowName))
		{
			WindowBinding windowBinding = (WindowBinding)dynWindowTable[windowName];
			ConstructorInfo constructor = windowBinding.windowType.GetConstructor(new Type[0]);
			IGUIControl iGUIControl = windowBinding.windowRef = (IGUIControl)constructor.Invoke(new object[0]);
			dynWindowTable[windowName] = windowBinding;
			if (!((IGUIContainer)Root[windowBinding.uiLayerName]).ControlList.Contains(iGUIControl))
			{
				((IGUIContainer)Root[windowBinding.uiLayerName]).Add(iGUIControl);
			}
		}
	}

	private void UnloadWindowOnTransition(string windowName)
	{
		if (dynWindowTable.ContainsKey(windowName))
		{
			WindowBinding windowBinding = (WindowBinding)dynWindowTable[windowName];
			((IGUIContainer)Root[windowBinding.uiLayerName]).Remove(windowBinding.windowRef);
			if (windowBinding.windowRef is GUIControl)
			{
				(windowBinding.windowRef as GUIControl).Dispose();
			}
		}
	}

	public void MarkWindowForLoading(string windowName)
	{
		if (!dynWindowLoadingTable.ContainsKey(windowName))
		{
			dynWindowLoadingTable.Add(windowName, false);
		}
		dynWindowLoadingTable[windowName] = true;
	}

	public void MarkSubWindowForLoading(string windowName)
	{
		if (!dynSubwindowLoadingTable.ContainsKey(windowName))
		{
			dynSubwindowLoadingTable.Add(windowName, false);
		}
		dynSubwindowLoadingTable[windowName] = true;
	}

	public void MarkWindowForUnloading(string windowName)
	{
		if (!dynWindowUnloadingTable.ContainsKey(windowName))
		{
			dynWindowUnloadingTable.Add(windowName, false);
		}
		dynWindowUnloadingTable[windowName] = true;
	}

	public void MarkSubWindowForUnloading(string windowName)
	{
		if (!dynSubwindowUnloadingTable.ContainsKey(windowName))
		{
			dynSubwindowUnloadingTable.Add(windowName, false);
		}
		dynSubwindowUnloadingTable[windowName] = true;
	}

	public void HandleTransitionLeave()
	{
		List<string> list = new List<string>();
		ClearModalHistory();
		currentState = ModalStateEnum.Transition;
		foreach (string key in dynSubwindowUnloadingTable.Keys)
		{
			list.Add(key);
		}
		foreach (string key2 in dynWindowUnloadingTable.Keys)
		{
			list.Add(key2);
		}
		for (int i = 0; i < list.Count; i++)
		{
			UnloadWindowOnTransition(list[i]);
		}
		list.Clear();
		dynWindowUnloadingTable.Clear();
		dynSubwindowUnloadingTable.Clear();
	}

	public void HandleTransitionEnter()
	{
		List<string> list = new List<string>();
		foreach (string key in dynWindowLoadingTable.Keys)
		{
			list.Add(key);
		}
		foreach (string key2 in dynSubwindowLoadingTable.Keys)
		{
			list.Add(key2);
		}
		for (int i = 0; i < list.Count; i++)
		{
			LoadWindowOnTransition(list[i]);
		}
		list.Clear();
		dynWindowLoadingTable.Clear();
		dynSubwindowLoadingTable.Clear();
	}
}
