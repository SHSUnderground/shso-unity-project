using UnityEngine;

public static class CoreAutoSelect
{
	public static string lastFocusedControl;

	public static void Pre(string name)
	{
		GUI.SetNextControlName(name);
	}

	public static void Post(string name)
	{
		string nameOfFocusedControl = GUI.GetNameOfFocusedControl();
		if (lastFocusedControl != nameOfFocusedControl)
		{
			if (nameOfFocusedControl == name)
			{
				TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
				textEditor.SelectAll();
				lastFocusedControl = nameOfFocusedControl;
			}
			else if (nameOfFocusedControl == string.Empty)
			{
				lastFocusedControl = nameOfFocusedControl;
			}
		}
	}
}
