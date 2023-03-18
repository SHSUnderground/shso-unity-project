using System.Collections.Generic;
using UnityEngine;

public class GUIFocusManager : ICaptureManager, IInputHandler
{
	public enum FocusHolder
	{
		Object,
		Nothing,
		UnknownObject
	}

	public GUITextField objectWithFocus;

	private KeyBank keyBank;

	private static int registeredNumber;

	public static bool mouseClicked;

	public bool postProcessFlag;

	private bool tabHit;

	private bool focusGrabbedThisMouseClicked;

	private FocusHolder whatHoldsFocus;

	public SHSInput.InputRequestorType InputRequestorType
	{
		get
		{
			return SHSInput.InputRequestorType.Debug;
		}
	}

	public bool CanHandleInput
	{
		get
		{
			return true;
		}
	}

	public GUIFocusManager(GUIManager Instance)
	{
		keyBank = new KeyBank(this, GUIControl.KeyInputState.Visible, KeyBank.KeyBankTypeEnum.Additive, new Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate>());
		keyBank.AddKey(new KeyCodeEntry(KeyCode.Tab, false, false, false), tabPressed);
		keyBank.AddKey(new KeyCodeEntry(KeyCode.Mouse0, false, false, false), mouseIsClicked);
		SHSInput.RegisterListener(keyBank);
		whatHoldsFocus = FocusHolder.Nothing;
	}

	public void getFocus(GUITextField focusRequestingObject)
	{
		focusGrabbedThisMouseClicked = true;
		ensureControlName(focusRequestingObject);
		GUIManager.Instance.SetKeyboardFocus(focusRequestingObject.ControlName);
		if (!hasFocus(focusRequestingObject))
		{
			objectWithFocus = focusRequestingObject;
			whatHoldsFocus = FocusHolder.Object;
			SHSInput.FocusManagerInputBlocking = true;
			SHSInput.SetInputBlockingMode(this, SHSInput.InputBlockType.CaptureMode);
			SHSInput.AddCaptureHandler(this, objectWithFocus);
			objectWithFocus.FireFocusEvent(new GUIChangedEvent());
		}
	}

	public void ensureControlName(GUITextField toName)
	{
		if (toName.ControlName == null)
		{
			toName.ControlName = "DefaultControlName" + registeredNumber;
			registeredNumber++;
		}
	}

	public void loseFocus()
	{
		SHSInput.RevertInputBlockingMode(this);
		if (objectWithFocus != null)
		{
			objectWithFocus.FireLostFocusEvent(new GUIChangedEvent());
		}
		objectWithFocus = null;
		whatHoldsFocus = FocusHolder.Nothing;
		SHSInput.FocusManagerInputBlocking = false;
		GUIUtility.keyboardControl = 0;
	}

	public bool hasFocus(IGUIControl doesThisHaveFocus)
	{
		return doesThisHaveFocus == objectWithFocus;
	}

	public bool isFocusUnknown()
	{
		return whatHoldsFocus == FocusHolder.UnknownObject;
	}

	public void mouseIsClicked(SHSKeyCode code)
	{
		mouseClicked = true;
		postProcessFlag = true;
	}

	public void tabPressed(SHSKeyCode code)
	{
		postProcessFlag = true;
		tabHit = true;
	}

	public void postProcessInput()
	{
		postProcessFlag = false;
		if (tabHit)
		{
			tabHit = false;
			performTabHit();
		}
		if (mouseClicked)
		{
			mouseClicked = false;
			preformMouseClicked();
		}
	}

	public void performTabHit()
	{
		if (GUIUtility.keyboardControl != 0)
		{
			SHSInput.FocusManagerInputBlocking = true;
			whatHoldsFocus = FocusHolder.UnknownObject;
			objectWithFocus = null;
			SHSInput.RevertInputBlockingMode(this);
		}
	}

	public void preformMouseClicked()
	{
		if (focusGrabbedThisMouseClicked)
		{
			focusGrabbedThisMouseClicked = false;
		}
		else
		{
			loseFocus();
		}
	}

	public void preprocessCleanup()
	{
		focusGrabbedThisMouseClicked = false;
	}

	public bool PassthroughAllowed(SHSKeyCode keycode)
	{
		KeyCode code = keycode.code;
		if (code == KeyCode.Tab || code == KeyCode.Return || code == KeyCode.BackQuote || code == KeyCode.Mouse0 || code == KeyCode.Mouse1 || code == KeyCode.Mouse2 || code == KeyCode.Mouse3 || code == KeyCode.Mouse4 || code == KeyCode.Mouse5)
		{
			return true;
		}
		if (keycode.originOfRequest is GUIControl)
		{
			return true;
		}
		return false;
	}

	public Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> GetKeyList(GUIControl.KeyInputState inputState)
	{
		return new Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate>();
	}

	public void ConfigureKeyBanks()
	{
	}

	public bool IsDescendantHandler(IInputHandler handler)
	{
		return false;
	}

	public void CaptureHandlerGotInput(ICaptureHandler handler)
	{
	}
}
