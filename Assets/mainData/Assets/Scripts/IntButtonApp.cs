using UnityEngine;

public class IntButtonApp : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GUIContent labelContent;

	public GUIStyle style1;

	public GUIStyle style2;

	public GUIStyle style3;

	public GUIContent[] toolbarContent;

	public GUIContent toolbarSelected;

	public GUIContent[] toolbarDisplayImage;

	private int selected = -1;

	private string typeing = "type here";

	private string password = string.Empty;

	private void OnGUI()
	{
		GUI.Label(new Rect(0f, 400f, 60f, 60f), labelContent, style1);
		typeing = GUI.TextField(new Rect(500f, 200f, 300f, 200f), typeing, style2);
		for (int i = 0; i < toolbarContent.Length; i++)
		{
			if (GUI.Button(new Rect(Screen.width - 50 * (i + 1), Screen.height - 50, 50f, 50f), toolbarContent[i]))
			{
				selected = i;
			}
		}
		if (selected != -1)
		{
			GUI.Label(new Rect(Screen.width - 50 * (selected + 1), Screen.height - 50, 50f, 50f), toolbarSelected);
			GUI.Label(new Rect(Screen.width - 300, Screen.height - 350, 300f, 300f), toolbarDisplayImage[selected]);
		}
		password = GUI.PasswordField(new Rect(600f, 500f, 300f, 40f), password, '*');
		GUI.TextArea(new Rect(600f, 540f, 300f, 60f), "password hacker:\n" + password);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
