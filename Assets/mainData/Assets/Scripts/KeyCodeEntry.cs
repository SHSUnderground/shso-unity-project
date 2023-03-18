using UnityEngine;

public struct KeyCodeEntry
{
	public readonly KeyCode KeyCode;

	public readonly bool Control;

	public readonly bool Alt;

	public readonly bool Shift;

	public bool Repeating;

	public KeyCodeEntry(KeyCode Code, bool Control, bool Alt, bool Shift)
	{
		this = new KeyCodeEntry(Code, Control, Alt, Shift, false);
	}

	public KeyCodeEntry(KeyCode Code, bool Control, bool Alt, bool Shift, bool Repeating)
	{
		KeyCode = Code;
		this.Control = Control;
		this.Alt = Alt;
		this.Shift = Shift;
		this.Repeating = Repeating;
	}

	public KeyCodeEntry(SHSKeyCode code)
	{
		KeyCode = code.code;
		Alt = code.alt;
		Control = code.control;
		Shift = code.shift;
		Repeating = false;
	}
}
