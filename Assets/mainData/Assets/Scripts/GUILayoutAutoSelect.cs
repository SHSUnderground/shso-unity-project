using UnityEngine;

public static class GUILayoutAutoSelect
{
	public static string TextArea(string name, string text, params GUILayoutOption[] options)
	{
		CoreAutoSelect.Pre(name);
		string result = GUILayout.TextArea(text, options);
		CoreAutoSelect.Post(name);
		return result;
	}

	public static string TextArea(string name, string text, int maxLength, params GUILayoutOption[] options)
	{
		CoreAutoSelect.Pre(name);
		string result = GUILayout.TextArea(text, maxLength, options);
		CoreAutoSelect.Post(name);
		return result;
	}

	public static string TextArea(string name, string text, GUIStyle style, params GUILayoutOption[] options)
	{
		CoreAutoSelect.Pre(name);
		string result = GUILayout.TextArea(text, style, options);
		CoreAutoSelect.Post(name);
		return result;
	}

	public static string TextArea(string name, string text, int maxLength, GUIStyle style, params GUILayoutOption[] options)
	{
		CoreAutoSelect.Pre(name);
		string result = GUILayout.TextArea(text, maxLength, style, options);
		CoreAutoSelect.Post(name);
		return result;
	}
}
