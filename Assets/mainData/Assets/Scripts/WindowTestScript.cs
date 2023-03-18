using UnityEngine;

public class WindowTestScript : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public MovieTexture movieTexture;

	private string[] windowList;

	private void Start()
	{
		windowList = new string[10];
		for (int i = 0; i < windowList.Length; i++)
		{
			windowList[i] = "window_" + i;
		}
		movieTexture.Play();
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		GUI.Box(new Rect(400f, 25f, 100f, 100f), "ASDADSDA");
		for (int i = 0; i < windowList.Length; i++)
		{
		}
		GUI.DrawTexture(new Rect(200f, 400f, 230f, 230f), movieTexture);
	}

	private void winFunc(int i)
	{
		GUI.BringWindowToFront(i);
	}
}
