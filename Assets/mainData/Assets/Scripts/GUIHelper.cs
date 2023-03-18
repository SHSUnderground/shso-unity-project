using UnityEngine;

public static class GUIHelper
{
	public static GUILayoutOption ExpandWidth = GUILayout.ExpandWidth(true);

	public static GUILayoutOption NoExpandWidth = GUILayout.ExpandWidth(false);

	public static GUILayoutOption ExpandHeight = GUILayout.ExpandHeight(true);

	public static GUILayoutOption NoExpandHeight = GUILayout.ExpandHeight(false);

	public static GUIStyle NoStyle = GUIStyle.none;

	public static GUIContent NoContent = GUIContent.none;

	public static GUILayoutOption Width(float w)
	{
		return GUILayout.Width(w);
	}
}
