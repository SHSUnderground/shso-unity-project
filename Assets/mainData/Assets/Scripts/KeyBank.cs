using System.Collections.Generic;

public struct KeyBank
{
	public enum KeyBankTypeEnum
	{
		Additive,
		Blocking
	}

	public readonly KeyBankTypeEnum BankType;

	public readonly IInputHandler Source;

	public readonly GUIControl.KeyInputState InputState;

	public readonly Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> KeyEventDictionary;

	public KeyBank(IInputHandler Source, GUIControl.KeyInputState InputState)
	{
		this = new KeyBank(Source, InputState, new Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate>());
	}

	public KeyBank(IInputHandler Source, GUIControl.KeyInputState InputState, Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> EventDictionary)
	{
		this = new KeyBank(Source, InputState, KeyBankTypeEnum.Additive, EventDictionary);
	}

	public KeyBank(IInputHandler Source, GUIControl.KeyInputState InputState, KeyBankTypeEnum BankType, Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> EventDictionary)
	{
		this.Source = Source;
		this.InputState = InputState;
		this.BankType = BankType;
		KeyEventDictionary = EventDictionary;
	}

	public void AddKey(KeyCodeEntry CodeEntry, SHSInput.KeyEventDelegate keyDelegate)
	{
		if (KeyEventDictionary.ContainsKey(CodeEntry))
		{
			RemoveKey(CodeEntry);
		}
		KeyEventDictionary.Add(CodeEntry, keyDelegate);
	}

	public void RemoveKey(KeyCodeEntry CodeEntry)
	{
		if (!KeyEventDictionary.ContainsKey(CodeEntry))
		{
			CspUtils.DebugLog("Attempting to remove KeyCodeEntry " + CodeEntry.KeyCode + " when KeyCodeEntry not added to " + this);
		}
		else
		{
			KeyEventDictionary.Remove(CodeEntry);
		}
	}
}
