using UnityEngine;

public class AdvButtonApp : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GUIContent button1;

	private GUIContent[][] scrollImages;

	public GUISkin windowSkin;

	private string buttonNumber = "you have yet to click a button";

	private bool windowOn;

	private Vector2 scrollPosition = Vector2.zero;

	private string logcopy = string.Empty;

	public int imageWidth;

	public int imageHeight;

	public GUIContent[] ScrollImages1;

	public GUIContent[] ScrollImages2;

	public GUIContent[] ScrollImages3;

	private int highestOne;

	private int numberOfScrollImages = 3;

	private void OnGUI()
	{
		if (GUI.Button(new Rect(0f, Screen.height - 450, 50f, 50f), button1))
		{
			windowOn = true;
		}
		if (windowOn)
		{
			GUI.Window(1, new Rect(0f, Screen.height - 400, 500f, 400f), generateWindow, "Hellooooo Window");
		}
		createScrollBox();
		GUI.TextArea(new Rect(Screen.width - 400, Screen.height - 420, 400f, 20f), logcopy);
	}

	private void createScrollBox()
	{
		scrollPosition = GUI.BeginScrollView(new Rect(Screen.width - 400, Screen.height - 400, 400f, 200f), scrollPosition, new Rect(0f, 0f, imageWidth * highestOne, imageHeight * numberOfScrollImages));
		for (int i = 0; i < scrollImages.Length; i++)
		{
			for (int j = 0; j < scrollImages[i].Length; j++)
			{
				if (GUI.Button(new Rect(imageWidth * j, imageHeight * i, imageWidth, imageHeight), scrollImages[i][j]))
				{
					logcopy = "you have clicked on image id#: (" + i + ", " + j + ") have a nice day!";
					MonoBehaviour.print(logcopy);
				}
			}
		}
		GUI.EndScrollView();
	}

	private void generateWindow(int windowID)
	{
		GUI.skin = windowSkin;
		for (int i = 0; i < 10; i++)
		{
			if (GUI.Button(new Rect(0 + 50 * i, 80f, 40f, 40f), i + string.Empty))
			{
				buttonNumber = "you have clicked button number " + i;
			}
		}
		GUI.TextArea(new Rect(0f, 10f, 200f, 30f), buttonNumber);
		if (GUI.Button(new Rect(450f, 0f, 50f, 50f), string.Empty))
		{
			windowOn = false;
		}
	}

	private void Start()
	{
		scrollImages = new GUIContent[3][];
		scrollImages[0] = ScrollImages1;
		scrollImages[1] = ScrollImages2;
		scrollImages[2] = ScrollImages3;
		highestOne = Mathf.Max(ScrollImages1.Length, ScrollImages2.Length);
		highestOne = Mathf.Max(highestOne, ScrollImages3.Length);
	}

	private void Update()
	{
	}
}
