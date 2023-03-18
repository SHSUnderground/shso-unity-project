using UnityEngine;

[RequireComponent(typeof(IEmotor))]
[AddComponentMenu("Lab/Emotor/Subtitle")]
public class EmotorSubtitle : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Font Font;

	private string _subtitle = string.Empty;

	private GUIStyle _bigFont;

	private void Start()
	{
		Emotor component = Utils.GetComponent<Emotor>(this);
		component.OnEmoteStart += delegate(EmotesDefinition.EmoteDefinition emoteDef)
		{
			if (emoteDef != null)
			{
				_subtitle = emoteDef.command;
			}
		};
		component.OnEmoteStop += delegate
		{
			_subtitle = string.Empty;
		};
	}

	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (_bigFont == null || _bigFont.font != Font)
		{
			_bigFont = new GUIStyle(GUI.skin.label);
			_bigFont.font = Font;
		}
		GUILayout.Label(_subtitle, _bigFont);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
