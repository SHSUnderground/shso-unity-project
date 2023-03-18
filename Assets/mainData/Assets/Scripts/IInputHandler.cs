using System.Collections.Generic;

public interface IInputHandler
{
	SHSInput.InputRequestorType InputRequestorType
	{
		get;
	}

	bool CanHandleInput
	{
		get;
	}

	Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> GetKeyList(GUIControl.KeyInputState inputState);

	void ConfigureKeyBanks();

	bool IsDescendantHandler(IInputHandler handler);
}
