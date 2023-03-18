using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class SHSInput
{
	System.Security.Cryptography.MD5CryptoServiceProvider md = null;
        
	[Flags]
	public enum InputBlockType
	{
		BlockUI = 0x2,
		BlockWorld = 0x4,
		BlockDebug = 0x8,
		CaptureMode = 0x10,
		BlockExternal = 0x20
	}

	public enum InputRequestorType
	{
		World,
		UI,
		Script,
		Debug
	}

	public enum MouseButtonType
	{
		Left,
		Right,
		Middle
	}

	public delegate void KeyEventDelegate(SHSKeyCode keyEventInfo);

	private delegate bool ButtonInputDelegate(string buttonName);

	private delegate bool KeyInputDelegate(string buttonName);

	private delegate bool KeyInputCodeDelegate(KeyCode code);

	private delegate bool MouseInputDelegate(int button);

	private static InputBlockType blockType;

	private static object blockRequester;

	private static Stack<CaptureManager> captureManagerStack;

	private static Dictionary<ICaptureManager, CaptureManager> captureManagerLookup;

	private static CaptureManager currentCaptureManager;

	private static IInputHandler proxy;

	private static Vector2 CachedScreenSize;

	private static List<KeyBank> eventListeners;

	private static Vector3 lastMousePosition;

	private static float lastInputTime;

	private static List<SHSKeyCode> blockTestOverrideKeys;

	private static Dictionary<KeyValuePair<IInputHandler, GUIControl.KeyInputState>, KeyBank> KeyBankDictionary;

	private static bool eventListenersInitialized;

	public static int MouseWheelValue;

	public static bool FocusManagerInputBlocking;

	private static int counter;
	
	public static float maxAFK = 300f; //Doggo
	
	private static bool windowFocus = true; //Doggo
	
	public static InputBlockType BlockType
	{
		get
		{
			return blockType;
		}
	}

	public static List<KeyBank> EventListeners
	{
		get
		{
			return eventListeners;
		}
	}

	public static float IdleTime
	{
		get
		{
			return Time.time - lastInputTime;
		}
	}

	public static Vector2 mouseScreenPosition
	{
		get
		{
			Vector3 mousePosition = SHSInput.mousePosition;
			float x = mousePosition.x / GUIManager.CurrentScale;
			float num = Screen.height;
			Vector3 mousePosition2 = SHSInput.mousePosition;
			return new Vector2(x, (num - mousePosition2.y) / GUIManager.CurrentScale);
		}
	}

	public static Vector3 mousePosition
	{
		get
		{
			return Input.mousePosition;
		}
	}

	public static string inputString
	{
		get
		{
			return Input.inputString;
		}
	}

	public static bool anyKey
	{
		get
		{
			return Input.anyKey;
		}
	}

	public static bool anyKeyDown
	{
		get
		{
			return Input.anyKeyDown;
		}
	}

	public static IInputHandler Proxy
	{
		get
		{
			return proxy;
		}
	}
	

	static SHSInput()
	{
		blockRequester = null;
		captureManagerStack = new Stack<CaptureManager>();
		captureManagerLookup = new Dictionary<ICaptureManager, CaptureManager>();
		currentCaptureManager = null;
		CachedScreenSize = new Vector2(-1f, -1f);
		lastMousePosition = Vector3.zero;
		lastInputTime = 0f;
		blockTestOverrideKeys = new List<SHSKeyCode>();
		eventListenersInitialized = false;
		MouseWheelValue = -10;
		FocusManagerInputBlocking = false;
		counter = 0;
		InitializeListeners();
	}

	public static void SetInputBlockingMode(object requester, InputBlockType inputBlockType)
	{
		CspUtils.DebugLog("SetInputBlockingMode inputBlockType=" + inputBlockType);

		if ((inputBlockType & InputBlockType.CaptureMode) != 0)
		{
			if (requester is ICaptureManager)
			{
				if (!captureManagerLookup.ContainsKey((ICaptureManager)requester))
				{
					captureManagerStack.Push(new CaptureManager());
					currentCaptureManager = captureManagerStack.Peek();
					captureManagerLookup.Add((ICaptureManager)requester, captureManagerStack.Peek());
					if ((blockType & InputBlockType.CaptureMode) == 0)
					{
						blockType |= InputBlockType.CaptureMode;
					}
				}
			}
			else
			{
				CspUtils.DebugLog("Invalid Capture request. Requester doesn't implement ICaptureManager interface.");
			}
		}
		else if (blockRequester == null)
		{
			blockRequester = requester;
			blockType |= inputBlockType;
		}
	}

	public static void RevertInputBlockingMode(object requester)
	{
		CspUtils.DebugLog("RevertInputBlockingMode");

		if ((blockType & InputBlockType.CaptureMode) != 0)
		{
			if (requester is ICaptureManager)
			{
				if (captureManagerLookup.ContainsKey((ICaptureManager)requester))
				{
					if (object.ReferenceEquals(captureManagerLookup[(ICaptureManager)requester], currentCaptureManager))
					{
						CaptureManager captureManager = captureManagerStack.Pop();
						captureManager.RemoveAllCaptureHandlers();
						captureManagerLookup.Remove((ICaptureManager)requester);
						if (captureManagerStack.Count > 0)
						{
							currentCaptureManager = captureManagerStack.Peek();
						}
						else
						{
							CspUtils.DebugLog("RevertInputBlockingMode blockType before=" + blockType);
							CspUtils.DebugLog("RevertInputBlockingMode InputBlockType.CaptureMode=" + InputBlockType.CaptureMode);							
							blockType ^= InputBlockType.CaptureMode;
							CspUtils.DebugLog("RevertInputBlockingMode blockType after=" + blockType);
						}
					}
					else
					{
						CspUtils.DebugLog("Can't revert blocking, this requestor is not at top of request stack");
					}
				}
			}
			else
			{
				CspUtils.DebugLog("Revert request denied. The requester doesn't implement the ICaptureManager interface");
				CspUtils.DebugLog(requester.ToString());
				CspUtils.DebugLog(requester.GetType());
			}
		}
		if (object.ReferenceEquals(blockRequester, requester))
		{
			blockRequester = null;
			if ((blockType & InputBlockType.BlockWorld) != 0)
			{
				blockType ^= InputBlockType.BlockWorld;
			}
			if ((blockType & InputBlockType.BlockUI) != 0)
			{
				blockType ^= InputBlockType.BlockUI;
			}
			if ((blockType & InputBlockType.BlockDebug) != 0)
			{
				blockType ^= InputBlockType.BlockDebug;
			}
			if ((blockType & InputBlockType.BlockExternal) != 0)
			{
				blockType ^= InputBlockType.BlockExternal;
			}
		}
	}

	public static bool IsCaptureAllowingInput(SHSKeyCode code)
	{
		return currentCaptureManager.InputAllowed(code);
	}

	public static void AddCaptureHandler(ICaptureManager captureManager, ICaptureHandler handler)
	{
		if ((blockType & InputBlockType.CaptureMode) != 0 && captureManagerLookup.ContainsKey(captureManager))
		{
			handler.Manager = captureManager;
			captureManagerLookup[captureManager].AddCaptureHandler(handler);
		}
	}

	public static void AddCaptureHandlers(ICaptureManager captureManager, ICaptureHandler[] handlers)
	{
		if ((blockType & InputBlockType.CaptureMode) != 0 && captureManagerLookup.ContainsKey(captureManager))
		{
			foreach (ICaptureHandler captureHandler in handlers)
			{
				captureHandler.Manager = captureManager;
			}
			captureManagerLookup[captureManager].AddCaptureHandlers(handlers);
		}
	}

	public static void RemoveCaptureHandler(ICaptureManager captureManager, ICaptureHandler handler)
	{
		if ((blockType & InputBlockType.CaptureMode) != 0 && captureManagerLookup.ContainsKey(captureManager))
		{
			captureManagerLookup[captureManager].RemoveCaptureHandler(handler);
		}
	}

	public static void RemoveAllCaptureHandlers(ICaptureManager captureManager)
	{
		if ((blockType & InputBlockType.CaptureMode) != 0 && captureManagerLookup.ContainsKey(captureManager))
		{
			captureManagerLookup[captureManager].RemoveAllCaptureHandlers();
		}
	}

	public static bool AllowInput(ICaptureHandler Source)
	{
		return WorldInputPassthroughAllowed(null);
	}

	public static void SetInputProxy(IInputHandler Proxy)
	{
		proxy = Proxy;
		RegisterListener(new KeyBank(proxy, GUIControl.KeyInputState.Active, KeyBank.KeyBankTypeEnum.Additive, proxy.GetKeyList(GUIControl.KeyInputState.Active)), true);
		SHSDebugInput inst = SHSDebugInput.Inst;
		inst.ToString();
	}

	private static void InitializeListeners()
	{
		eventListeners = new List<KeyBank>();
		KeyBankDictionary = new Dictionary<KeyValuePair<IInputHandler, GUIControl.KeyInputState>, KeyBank>();
		eventListenersInitialized = true;
	}

	private static void OnEnter(KeyCode code)
	{
		CspUtils.DebugLog("Root Level Enter key");
	}

	private static void OnEscape(KeyCode code)
	{
		CspUtils.DebugLog("Root Level Escape key");
	}

	public static void RegisterListener(KeyBank KeyBank)
	{
		RegisterListener(KeyBank, false);
	}

	public static void RegisterListener(KeyBank KeyBank, bool isDefaultListener)
	{
		if (!eventListenersInitialized)
		{
			InitializeListeners();
		}
		if (!KeyBankDictionary.ContainsKey(new KeyValuePair<IInputHandler, GUIControl.KeyInputState>(KeyBank.Source, KeyBank.InputState)))
		{
			if (!isDefaultListener)
			{
				eventListeners.Add(KeyBank);
			}
			else
			{
				List<KeyBank> list = new List<KeyBank>();
				list.Add(KeyBank);
				list.AddRange(eventListeners);
				eventListeners = list;
			}
			KeyBankDictionary.Add(new KeyValuePair<IInputHandler, GUIControl.KeyInputState>(KeyBank.Source, KeyBank.InputState), KeyBank);
		}
	}

	public static void RegisterKey(SHSKeyCode code, KeyEventDelegate callback, IInputHandler handler)
	{
		KeyBank keyBank;
		if (!KeyBankDictionary.ContainsKey(new KeyValuePair<IInputHandler, GUIControl.KeyInputState>(handler, GUIControl.KeyInputState.Transitory)))
		{
			keyBank = new KeyBank(handler, GUIControl.KeyInputState.Transitory, KeyBank.KeyBankTypeEnum.Additive, new Dictionary<KeyCodeEntry, KeyEventDelegate>());
			RegisterListener(keyBank);
		}
		else
		{
			keyBank = KeyBankDictionary[new KeyValuePair<IInputHandler, GUIControl.KeyInputState>(handler, GUIControl.KeyInputState.Transitory)];
			CspUtils.DebugLog("Transitory keybank exists for: " + handler + ". Adding to the keybank that exists...?");
		}
		keyBank.AddKey(new KeyCodeEntry(code), callback);
	}

	public static void UnregisterKey(SHSKeyCode code, IInputHandler handler)
	{
		if (!KeyBankDictionary.ContainsKey(new KeyValuePair<IInputHandler, GUIControl.KeyInputState>(handler, GUIControl.KeyInputState.Transitory)))
		{
			CspUtils.DebugLog("Attempt to unregister a key for: " + handler + " when no transitory keybank for this handler exists.");
			return;
		}
		KeyBank keyBank = KeyBankDictionary[new KeyValuePair<IInputHandler, GUIControl.KeyInputState>(handler, GUIControl.KeyInputState.Transitory)];
		if (keyBank.KeyEventDictionary.ContainsKey(new KeyCodeEntry(code)))
		{
			keyBank.RemoveKey(new KeyCodeEntry(code));
			if (keyBank.KeyEventDictionary.Count == 0)
			{
				UnregisterListener(keyBank);
			}
		}
		else
		{
			CspUtils.DebugLog("Bank: " + keyBank + " does not have the key: " + code.code + " registered in it.");
		}
	}

	public static void UnregisterListener(IInputHandler Source, GUIControl.KeyInputState inputState)
	{
		if (KeyBankDictionary.ContainsKey(new KeyValuePair<IInputHandler, GUIControl.KeyInputState>(Source, inputState)))
		{
			UnregisterListener(KeyBankDictionary[new KeyValuePair<IInputHandler, GUIControl.KeyInputState>(Source, inputState)]);
		}
	}

	public static void UnregisterListener(KeyBank KeyBank)
	{
		if (!KeyBankDictionary.ContainsKey(new KeyValuePair<IInputHandler, GUIControl.KeyInputState>(KeyBank.Source, KeyBank.InputState)))
		{
			CspUtils.DebugLog("No object " + KeyBank.Source + " is listening for key events.");
			return;
		}
		KeyBankDictionary.Remove(new KeyValuePair<IInputHandler, GUIControl.KeyInputState>(KeyBank.Source, KeyBank.InputState));
		eventListeners.Remove(KeyBank);
	}

	public static void Update()
	{
		ScreenResizeCheck();
		IdleCheck();
		InputCheck();		
	}

	public static void InputCheck()
	{
		bool flag = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		bool flag2 = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		bool flag3 = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		int num = eventListeners.Count - 1;
		KeyBank keyBank;
		while (true)
		{
			if (num < 0)
			{
				return;
			}
			keyBank = eventListeners[num];
			if (keyBank.Source.CanHandleInput)
			{
				foreach (KeyValuePair<KeyCodeEntry, KeyEventDelegate> item in keyBank.KeyEventDictionary)
				{
					KeyCodeEntry key = item.Key;
					int num2 = Convert.ToInt32(key.KeyCode);
					num2 = ((num2 << 4) | (key.Alt ? 1 : (key.Control ? 2 : (key.Shift ? 4 : 0))));
					if (!dictionary.ContainsKey(num2) && ((Input.GetKeyDown(key.KeyCode) && key.Alt == flag && key.Control == flag2 && key.Shift == flag3) || (key.Repeating && Input.GetKey(key.KeyCode) && key.Alt == flag && key.Control == flag2 && key.Shift == flag3)))
					{
						dictionary[num2] = 1;
						if (item.Value != null)
						{
							SHSKeyCode sHSKeyCode = new SHSKeyCode();
							sHSKeyCode.alt = flag;
							sHSKeyCode.shift = flag3;
							sHSKeyCode.control = flag2;
							sHSKeyCode.code = key.KeyCode;
							sHSKeyCode.source = keyBank.Source;
							if ((blockType & InputBlockType.CaptureMode) != 0)
							{
								if (currentCaptureManager.InputAllowed(sHSKeyCode) || keyBank.Source.InputRequestorType == InputRequestorType.Debug)
								{
									item.Value(sHSKeyCode);
								}
								else
								{
									dictionary.Remove(num2);
								}
							}
							else if (((blockType & InputBlockType.BlockWorld) == 0 || keyBank.Source.InputRequestorType != 0) && ((blockType & InputBlockType.BlockUI) == 0 || keyBank.Source.InputRequestorType != InputRequestorType.UI) && ((blockType & InputBlockType.BlockDebug) == 0 || keyBank.Source.InputRequestorType != InputRequestorType.Debug))
							{
								item.Value(sHSKeyCode);
							}
						}
						else
						{
							dictionary.Remove(num2);
						}
					}
				}
				if (keyBank.BankType == KeyBank.KeyBankTypeEnum.Blocking)
				{
					break;
				}
			}
			num--;
		}
		CspUtils.DebugLog("Bank " + keyBank + " is blocking all lower input. Returning...");
	}

	private static void KeyFocusCheck()
	{
		if (GetMouseButton(MouseButtonType.Left, AppShell.Instance) && WorldInputPassthroughAllowed(null) && !UIKeyboardCheck())
		{
			GUIUtility.keyboardControl = 0;
		}
	}

	private static void ScreenResizeCheck()
	{
		if (CachedScreenSize[0] == -1f)
		{
			CachedScreenSize = new Vector2(Screen.width, Screen.height);
		}
		else if (CachedScreenSize[0] != (float)Screen.width || CachedScreenSize[1] != (float)Screen.height)
		{
			Vector2 cachedScreenSize = CachedScreenSize;
			CachedScreenSize = new Vector2(Screen.width, Screen.height);
			AppShell.Instance.EventMgr.Fire(null, new GUIResizeMessage(GUIControl.HandleResizeSource.Screen, new Rect(0f, 0f, cachedScreenSize.x, cachedScreenSize.y), new Rect(0f, 0f, CachedScreenSize.x, CachedScreenSize.y)));
		}
	}

	private static void IdleCheck()
	{
		if (Input.anyKey)
		{
			lastInputTime = Time.time;
			lastMousePosition = mousePosition;
		}
	}
	
	
	
	
	private static bool verifyCommonInput(SHSKeyCode code, IInputHandler requestor)
	{
		if ((blockType & InputBlockType.CaptureMode) != 0)
		{
			SHSKeyCode sHSKeyCode = new SHSKeyCode();
			sHSKeyCode.source = requestor;
			if (!currentCaptureManager.InputAllowed(sHSKeyCode) && requestor.InputRequestorType != InputRequestorType.Debug)
			{
				return false;
			}
		}
		
		//CspUtils.DebugLog("blockType = " + blockType);
		//CspUtils.DebugLog("InputBlockType.BlockWorld = " + InputBlockType.BlockWorld);
		//CspUtils.DebugLog("InputBlockType.BlockWorld = " + InputBlockType.BlockUI);
		//CspUtils.DebugLog("InputBlockType.BlockWorld = " + InputBlockType.BlockDebug);
		//CspUtils.DebugLog("requestor.InputRequestorType = " + requestor.InputRequestorType);
		//CspUtils.DebugLog("InputRequestorType.World = " + InputRequestorType.World);	
		//CspUtils.DebugLog("InputRequestorType.World = " + InputRequestorType.UI);
		//CspUtils.DebugLog("InputRequestorType.World = " + InputRequestorType.Debug);
	
		if (((blockType & InputBlockType.BlockWorld) != 0 && requestor.InputRequestorType == InputRequestorType.World) 
			|| ((blockType & InputBlockType.BlockUI) != 0 && requestor.InputRequestorType == InputRequestorType.UI) 
			|| ((blockType & InputBlockType.BlockDebug) != 0 && requestor.InputRequestorType == InputRequestorType.Debug))
		{
			return false;
		}
		if (requestor.InputRequestorType == InputRequestorType.World)
		{
			if (GUIManager.Instance.CurrentState == GUIManager.ModalStateEnum.Modal)
			{
				return false;
			}
			return WorldInputPassthroughAllowed(code) && requestor.CanHandleInput;
		}
		return requestor.CanHandleInput;
	}

	public static bool IsObjectAllowedInput(GameObject gameObject)
	{
		Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeof(Component));
		IInputHandler inputHandler = null;
		SHSKeyCode code = new SHSKeyCode();
		bool result = true;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i] is IInputHandler)
			{
				inputHandler = (IInputHandler)componentsInChildren[i];
				i = componentsInChildren.Length;
			}
		}
		if (inputHandler != null)
		{
			result = verifyCommonInput(code, inputHandler);
		}
		else if ((blockType & InputBlockType.CaptureMode) != 0 || (blockType & InputBlockType.BlockWorld) != 0)
		{
			result = false;
		}
		return result;
	}

	private static bool verifyNoneInputHandler()
	{
		if ((blockType & InputBlockType.CaptureMode) != 0)
		{
			return false;
		}
		if ((blockType & InputBlockType.BlockWorld) != 0)
		{
			return false;
		}
		return true;
	}

	public static float GetAxis(string axis)
	{
		SHSKeyCode sHSKeyCode = new SHSKeyCode();
		sHSKeyCode.source = AppShell.Instance;
		sHSKeyCode.keyName = "Axis";
		RegisterBlockTestOverrideCode(sHSKeyCode);
		if (!verifyCommonInput(sHSKeyCode, AppShell.Instance))
		{
			DestroyBlockTestOverrideCode(sHSKeyCode);
			return 0f;
		}
		DestroyBlockTestOverrideCode(sHSKeyCode);
		return Input.GetAxis(axis);
	}

	public static float GetAxisRaw(string axis)
	{
		SHSKeyCode sHSKeyCode = new SHSKeyCode();
		sHSKeyCode.source = AppShell.Instance;
		sHSKeyCode.keyName = "Axis";
		RegisterBlockTestOverrideCode(sHSKeyCode);
		if (!verifyCommonInput(sHSKeyCode, AppShell.Instance))
		{
			DestroyBlockTestOverrideCode(sHSKeyCode);
			return 0f;
		}
		DestroyBlockTestOverrideCode(sHSKeyCode);
		return Input.GetAxisRaw(axis);
	}

	public static bool GetKey(KeyCode key)
	{
		SHSKeyCode sHSKeyCode = new SHSKeyCode();
		sHSKeyCode.source = AppShell.Instance;
		sHSKeyCode.code = key;
		if (!verifyCommonInput(sHSKeyCode, AppShell.Instance))
		{
			DestroyBlockTestOverrideCode(sHSKeyCode);
			return false;
		}
		DestroyBlockTestOverrideCode(sHSKeyCode);
		return Input.GetKey(key);
	}

	private static bool verifyInput(string buttonName, IInputHandler requestor, ButtonInputDelegate func)
	{
		SHSKeyCode sHSKeyCode = new SHSKeyCode();
		sHSKeyCode.keyName = buttonName;
		return verifyCommonInput(sHSKeyCode, requestor) && func(buttonName);
	}

	public static bool GetButton(string buttonName, IInputHandler requestor)
	{
		return verifyInput(buttonName, requestor, Input.GetButton);
	}

	public static bool GetButtonDown(string buttonName, IInputHandler requestor)
	{
		return verifyInput(buttonName, requestor, Input.GetButtonDown);
	}

	public static bool GetButtonUp(string buttonName, IInputHandler requestor)
	{
		return verifyInput(buttonName, requestor, Input.GetButtonUp);
	}

	public static bool GetButton(string buttonName)
	{
		return GetButton(buttonName, AppShell.Instance);
	}

	public static bool GetButtonDown(string buttonName)
	{
		return GetButtonDown(buttonName, AppShell.Instance);
	}

	public static bool GetButtonUp(string buttonName)
	{
		return GetButtonUp(buttonName, AppShell.Instance);
	}

	private static bool verifyKey(KeyCode code, IInputHandler requestor, KeyInputCodeDelegate func)
	{
		SHSKeyCode sHSKeyCode = new SHSKeyCode();
		sHSKeyCode.code = code;
		return verifyCommonInput(sHSKeyCode, requestor) && func(code);
	}

	private static bool verifyKey(string buttonName, IInputHandler requestor, KeyInputDelegate func)
	{
		SHSKeyCode sHSKeyCode = new SHSKeyCode();
		sHSKeyCode.keyName = buttonName;
		return verifyCommonInput(sHSKeyCode, requestor) && func(buttonName);
	}

	public static bool GetKey(string name, IInputHandler requestor)
	{
		return verifyKey(name, requestor, Input.GetKey);
	}

	[Obsolete("Use GetKeyDown(KeyCode code, IInputHandler requestor) instead")]
	public static bool GetKeyDown(string name, IInputHandler requestor)
	{
		return verifyKey(name, requestor, Input.GetKeyDown);
	}

	[Obsolete("Use GetKeyUp(KeyCode code, IInputHandler requestor) instead")]
	public static bool GetKeyUp(string name, IInputHandler requestor)
	{
		return verifyKey(name, requestor, Input.GetKeyUp);
	}

	public static bool GetKeyDown(KeyCode key, IInputHandler requestor)
	{
		return verifyKey(key, requestor, Input.GetKeyDown);
	}

	public static bool GetKeyUp(KeyCode key, IInputHandler requestor)
	{
		return verifyKey(key, requestor, Input.GetKeyUp);
	}

	public static bool GetKey(string name)
	{
		return GetKey(name, AppShell.Instance);
	}

	[Obsolete("Use GetKeyDown(KeyCode code) instead")]
	public static bool GetKeyDown(string name)
	{
		return GetKeyDown(name, AppShell.Instance);
	}

	[Obsolete("Use GetKeyUp(KeyCode code) instead")]
	public static bool GetKeyUp(string name)
	{
		return GetKeyUp(name, AppShell.Instance);
	}

	public static bool GetKeyDown(KeyCode key)
	{
		return GetKeyDown(key, AppShell.Instance);
	}

	public static bool GetKeyUp(KeyCode key)
	{
		return GetKeyUp(key, AppShell.Instance);
	}

	public static float GetMouseWheelDelta(IInputHandler requestor)
	{
		SHSKeyCode sHSKeyCode = new SHSKeyCode();
		sHSKeyCode.keyName = "Mouse ScrollWheel";
		if (!verifyCommonInput(sHSKeyCode, requestor))
		{
			return 0f;
		}
		if (requestor.InputRequestorType == InputRequestorType.World && !IsMousePositionInView(mousePosition))
		{
			return 0f;
		}
		return Input.GetAxis("Mouse ScrollWheel");
	}

	public static float GetMouseWheelDelta()
	{
		return GetMouseWheelDelta(AppShell.Instance);
	}

	private static bool verifyMouseButton(MouseButtonType button, IInputHandler requestor, MouseInputDelegate func)
	{
		SHSKeyCode sHSKeyCode = new SHSKeyCode();
		switch (button)
		{
		case MouseButtonType.Left:
			sHSKeyCode.code = KeyCode.Mouse0;
			break;
		case MouseButtonType.Right:
			sHSKeyCode.code = KeyCode.Mouse1;
			break;
		case MouseButtonType.Middle:
			sHSKeyCode.code = KeyCode.Mouse2;
			break;
		}
		if (!verifyCommonInput(sHSKeyCode, requestor))
		{
			return false;   //
		}
		if (requestor.InputRequestorType == InputRequestorType.World)
		{
			if (IsMousePositionInView(mousePosition))
			{
				return func((int)button);
			}
			return false;
		}
		if (!(requestor is IGUIControl))
		{
			CspUtils.DebugLog("Requestor is NOT an IGUIControl");
			return false;
		}
		if (GUIManager.Instance.CurrentOverControl == null || requestor.IsDescendantHandler((IInputHandler)GUIManager.Instance.CurrentOverControl))
		{
			return func((int)button);
		}
		return false;
	}

	public static bool GetMouseButton(MouseButtonType button, IInputHandler requestor)
	{
		return verifyMouseButton(button, requestor, Input.GetMouseButton);
	}

	public static bool GetMouseButtonDown(MouseButtonType button, IInputHandler requestor)
	{
		return verifyMouseButton(button, requestor, Input.GetMouseButtonDown);
	}

	public static bool GetMouseButtonUp(MouseButtonType button, IInputHandler requestor)
	{
		return verifyMouseButton(button, requestor, Input.GetMouseButtonUp);
	}

	public static bool GetMouseButton(MouseButtonType button)
	{
		return GetMouseButton(button, AppShell.Instance);
	}

	public static bool GetMouseButtonDown(MouseButtonType button)
	{
		return GetMouseButtonDown(button, AppShell.Instance);
	}

	public static bool GetMouseButtonUp(MouseButtonType button)
	{
		return GetMouseButtonUp(button, AppShell.Instance);
	}

	public static bool GetMouseButton(int button)
	{
		throw new NotImplementedException();
	}

	public static bool GetMouseButtonDown(int button)
	{
		throw new NotImplementedException();
	}

	public static bool GetMouseButtonUp(int button)
	{
		throw new NotImplementedException();
	}

	public static void ResetInputAxes()
	{
		Input.ResetInputAxes();
	}

	public static bool IsOverUI()
	{
		return !WorldInputPassthroughAllowed(null);
	}

	private static bool WorldInputPassthroughAllowed(SHSKeyCode keyCode)
	{
		if (keyCode != null)
		{
			for (int i = 0; i < blockTestOverrideKeys.Count; i++)
			{
				if (keyCode.keyName == blockTestOverrideKeys[i].keyName || keyCode.code == blockTestOverrideKeys[i].code)
				{
					return true;
				}
			}
		}
		return (GUIUtility.hotControl == 0 && GUIManager.Instance.CursorBlockTestState != GUIManager.CursorBlockTestStateEnum.UI) ? true : false;
	}

	private static bool UIKeyboardCheck()
	{
		return GUIUtility.keyboardControl == 0;
	}

	public static void RegisterBlockTestOverrideCode(SHSKeyCode code)
	{
		if (!blockTestOverrideKeys.Contains(code))
		{
			blockTestOverrideKeys.Add(code);
		}
	}

	public static void DestroyBlockTestOverrideCode(SHSKeyCode code)
	{
		blockTestOverrideKeys.Remove(code);
	}

	public static bool IsMousePositionInView(Vector3 position)
	{
		if (position.x >= 0f && position.x <= (float)Screen.width && position.y >= 0f && position.y <= (float)Screen.height)
		{
			return true;
		}
		return false;
	}
}
