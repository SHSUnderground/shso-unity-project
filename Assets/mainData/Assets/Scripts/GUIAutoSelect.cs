using UnityEngine;

public static class GUIAutoSelect
{
	public static string TextArea(string name, Rect pos, string text)
	{
		CoreAutoSelect.Pre(name);
		string result = GUI.TextArea(pos, text);
		CoreAutoSelect.Post(name);
		return result;
	}

	public static string TextArea(string name, Rect pos, string text, int maxLength)
	{
		CoreAutoSelect.Pre(name);
		string result = GUI.TextArea(pos, text, maxLength);
		CoreAutoSelect.Post(name);
		return result;
	}

	public static string TextArea(string name, Rect pos, string text, GUIStyle style)
	{
		CoreAutoSelect.Pre(name);
		string result = GUI.TextArea(pos, text, style);
		CoreAutoSelect.Post(name);
		return result;
	}

	public static string TextArea(string name, Rect pos, string text, int maxLength, GUIStyle style)
	{
		CoreAutoSelect.Pre(name);
		string result = GUI.TextArea(pos, text, maxLength, style);
		CoreAutoSelect.Post(name);
		return result;
	}
}
