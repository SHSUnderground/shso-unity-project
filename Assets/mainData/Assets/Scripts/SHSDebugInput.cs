using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEngine;

public class SHSDebugInput : IInputHandler
{
	private enum KeyBankType
	{
		Permanent,
		Temp
	}

	private static SHSDebugInput inst;

	private Dictionary<KeyBankType, KeyBank> keyBanks;

	private KeyBankType activeKeyBank;

	private MovieMaker movieMaker;

	public static SHSDebugInput Inst
	{
		get
		{
			if (inst == null)
			{
				inst = new SHSDebugInput();
			}
			return inst;
		}
	}

	public bool CanHandleInput
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
			return SHSInput.InputRequestorType.Debug;
		}
	}

	private SHSDebugInput()
	{
		keyBanks = new Dictionary<KeyBankType, KeyBank>();
		keyBanks[KeyBankType.Permanent] = new KeyBank(this, GUIControl.KeyInputState.Active, KeyBank.KeyBankTypeEnum.Additive, GetKeyList(GUIControl.KeyInputState.Active));
		keyBanks[KeyBankType.Temp] = new KeyBank(this, GUIControl.KeyInputState.Active, KeyBank.KeyBankTypeEnum.Additive, GetKeyList(GUIControl.KeyInputState.Active));
		activeKeyBank = KeyBankType.Permanent;
	}

	public Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> GetKeyList(GUIControl.KeyInputState inputState)
	{
		return new Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate>();
	}

	public void AddKeyListener(KeyCodeEntry keyCodeEntry, SHSInput.KeyEventDelegate keyEventDelegate)
	{
		keyBanks[KeyBankType.Temp].AddKey(keyCodeEntry, keyEventDelegate);
	}

	public void RemoveKeyListener(KeyCodeEntry keyCodeEntry)
	{
		keyBanks[KeyBankType.Temp].RemoveKey(keyCodeEntry);
	}

	public void ActivateDebugKeys()
	{
		activeKeyBank = KeyBankType.Temp;
	}

	public void DeactivateDebugKeys()
	{
		activeKeyBank = KeyBankType.Permanent;
	}

	public void ActivateCurrentKeyBank()
	{
		SHSInput.RegisterListener(keyBanks[activeKeyBank]);
	}

	public void DeactivateCurrentKeyBank()
	{
		SHSInput.UnregisterListener(keyBanks[activeKeyBank]);
	}

	public void ConfigureKeyBanks()
	{
	}

	public bool IsDescendantHandler(IInputHandler handler)
	{
		return false;
	}

	public void OnConfigServersLoaded()
	{
		bool flag = AppShell.Instance != null && AppShell.Instance.ServerConfig != null && AppShell.Instance.ServerConfig.TryGetBool("//debug_keys_on", false);
		keyBanks[KeyBankType.Temp].AddKey(new KeyCodeEntry(KeyCode.F12, false, false, false), onScreenshot);
		keyBanks[KeyBankType.Temp].AddKey(new KeyCodeEntry(KeyCode.F11, false, false, false), onMovie);
		keyBanks[KeyBankType.Temp].AddKey(new KeyCodeEntry(KeyCode.F2, true, false, true), onFlyCam);
		keyBanks[KeyBankType.Temp].AddKey(new KeyCodeEntry(KeyCode.F6, false, false, false), onResetGlobalMinFps);
		SHSInput.RegisterListener(keyBanks[KeyBankType.Permanent], true);
		if (flag && activeKeyBank != KeyBankType.Temp)
		{
			DeactivateCurrentKeyBank();
			ActivateDebugKeys();
			ActivateCurrentKeyBank();
		}
		else if (activeKeyBank != 0)
		{
			DeactivateCurrentKeyBank();
			DeactivateDebugKeys();
			ActivateCurrentKeyBank();
		}
	}

	public string PrintDebugKeys()
	{
		StringBuilder stringBuilder = new StringBuilder();
		KeyBank keyBank = keyBanks[KeyBankType.Temp];
		foreach (KeyCodeEntry key in keyBank.KeyEventDictionary.Keys)
		{
			KeyBank keyBank2 = keyBanks[KeyBankType.Temp];
			SHSInput.KeyEventDelegate keyEventDelegate = keyBank2.KeyEventDictionary[key];
			if (key.Shift)
			{
				stringBuilder.Append("Shift ");
			}
			if (key.Control)
			{
				stringBuilder.Append("Control ");
			}
			if (key.Alt)
			{
				stringBuilder.Append("Alt ");
			}
			DescriptionAttribute[] array = (DescriptionAttribute[])keyEventDelegate.Method.GetCustomAttributes(typeof(DescriptionAttribute), false);
			string arg = (array.Length <= 0) ? keyEventDelegate.Method.Name : array[0].Description;
			stringBuilder.Append(key.KeyCode.ToString());
			stringBuilder.AppendFormat("\t{0}\n", arg);
		}
		return stringBuilder.ToString();
	}

	[Description("Take a screenshot")]
	private void onScreenshot(SHSKeyCode code)
	{
	}

	[Description("Creates a movie")]
	private void onMovie(SHSKeyCode code)
	{
		if (movieMaker == null)
		{
			GameObject gameObject = new GameObject();
			movieMaker = (MovieMaker)gameObject.AddComponent(typeof(MovieMaker));
			movieMaker.StartRecording();
			if (!movieMaker.Recording)
			{
				movieMaker = null;
			}
			else
			{
				CspUtils.DebugLog("Started Movie Recording");
			}
		}
		else if (movieMaker.Recording)
		{
			movieMaker.StopRecording();
			Object.Destroy(movieMaker.gameObject);
			movieMaker = null;
			CspUtils.DebugLog("Stopped Movie Recording");
		}
		CspUtils.DebugLog("OnMovie: " + code);
	}

	[Description("Switches to fly camera")]
	private void onFlyCam(SHSKeyCode code)
	{
		DebugFlyCam2.Toggle();
	}

	[Description("Resets the Global Min FPS Counter")]
	private void onResetGlobalMinFps(SHSKeyCode code)
	{
		SHSDebugWindow sHSDebugWindow = GUIManager.Instance.GetRoot(GUIManager.UILayer.Debug)["SHSDebugWindow"] as SHSDebugWindow;
		if (sHSDebugWindow != null)
		{
			sHSDebugWindow.OnGlobalMinFpsReset();
		}
		else
		{
			CspUtils.DebugLog("Unable to find the SHSDebugWindow to reset the global min FPS counter.");
		}
	}
}
