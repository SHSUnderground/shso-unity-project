using UnityEngine;

public class SHSKeyCode
{
	public KeyCode code;

	public bool alt;

	public bool shift;

	public bool control;

	public IInputHandler source;

	public object target;

	public object originOfRequest;

	public string keyName = string.Empty;

	public SHSKeyCode(KeyCode code)
	{
		this.code = code;
	}

	public SHSKeyCode()
	{
	}

	public override string ToString()
	{
		return "[Code: " + code + " A:" + alt + " C:" + control + " S:" + shift + " Src: " + source + " Tgt:" + target + "]";
	}
}
