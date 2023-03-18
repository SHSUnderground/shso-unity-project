using UnityEngine;

public class KeyTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private int lastController = -1;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (SHSInput.GetKey("space"))
		{
			CspUtils.DebugLog(">> Keyboard script detected a key.");
			CspUtils.DebugLog(GUIUtility.keyboardControl);
		}
		if (GUIUtility.keyboardControl != lastController)
		{
			CspUtils.DebugLog("Changed from " + lastController + "to " + GUIUtility.keyboardControl);
			GUIUtility.keyboardControl = ((lastController != -1) ? GUIUtility.keyboardControl : 0);
			lastController = GUIUtility.keyboardControl;
		}
	}
}
