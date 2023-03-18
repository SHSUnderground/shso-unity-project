using System.Collections.Generic;
using UnityEngine;

public class SHSInputProxy : MonoBehaviour, IInputHandler
{
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
			return SHSInput.InputRequestorType.World;
		}
	}

	private void Awake()
	{
		SHSInput.SetInputProxy(this);
	}

	private void Start()
	{
	}

	private void Update()
	{
		SHSInput.Update();
	}

	public Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> GetKeyList(GUIControl.KeyInputState inputState)
	{
		Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> dictionary = new Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate>();
		dictionary.Add(new KeyCodeEntry(KeyCode.Return, true, false, false), OnTest);
		dictionary.Add(new KeyCodeEntry(KeyCode.Return, false, true, false), OnTest);
		dictionary.Add(new KeyCodeEntry(KeyCode.Return, false, false, true), OnTest);
		return dictionary;
	}

	public void ConfigureKeyBanks()
	{
	}

	public bool IsDescendantHandler(IInputHandler handler)
	{
		return false;
	}

	private void OnTest(SHSKeyCode code)
	{
		CspUtils.DebugLog("OnTest " + code);
	}
}
