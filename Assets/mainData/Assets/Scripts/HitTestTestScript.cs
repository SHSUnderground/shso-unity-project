using UnityEngine;

public class HitTestTestScript : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(400f, 0f, 133f, 100f), "Button A", GUIManager.Instance.StyleManager.GetStyle("HitTestStyle").UnityStyle))
		{
			CspUtils.DebugLog("Button A");
		}
		if (GUI.Button(new Rect(300f, 0f, 133f, 100f), "Button B", GUIManager.Instance.StyleManager.GetStyle("HitTestStyle").UnityStyle))
		{
			CspUtils.DebugLog("Button B");
		}
	}
}
