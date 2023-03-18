using UnityEngine;

public class TestSkin : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public SHSSkin assignedSkin;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (assignedSkin != null)
		{
			GUI.skin = assignedSkin.skin;
		}
		SHSStyle sHSStyle = new SHSStyle(GUI.skin.button);
		sHSStyle.UnityStyle.alignment = TextAnchor.LowerCenter;
		RectOffset rectOffset = new RectOffset();
		rectOffset.top = 30;
		rectOffset.bottom = 30;
		sHSStyle.UnityStyle.padding = rectOffset;
		sHSStyle.UnityStyle.font = new Font();
		GUILayout.Label("Testing content", sHSStyle.UnityStyle);
		GUILayout.Button("Click Me Not", sHSStyle.UnityStyle);
	}
}
