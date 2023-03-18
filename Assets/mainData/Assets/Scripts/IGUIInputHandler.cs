using System.Collections.Generic;

public interface IGUIInputHandler
{
	Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> GetKeyList(GUIControl.KeyInputState inputState);

	void ConfigureKeyBanks();
}
