using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ButtonApp2 : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public AudioClip beep;

	public AudioClip beep2;

	public Texture2D img;

	private void OnGUI()
	{
		if (GUI.Button(new Rect(0f, 0f, 250f, 120f), "I am in the upper left corner\nand play a sound whne clicked.\n\nLook!\n\nWhen you put your mouse over me,\nI change...slightly"))
		{
			base.audio.PlayOneShot(beep);
		}
		if (GUI.Button(new Rect(0f, 120f, 240f, 60f), "I send a log message when clicked"))
		{
			MonoBehaviour.print("grats");
		}
		Rect rect = new Rect(200f, 180f, 200f, 30f);
		if (GUI.Button(rect, "click me for sound or \n mouse over me for logchat"))
		{
			base.audio.PlayOneShot(beep2);
		}
		if (rollover(rect, Event.current.mousePosition))
		{
			MonoBehaviour.print("you are rolling over a button!!! Good for you.");
		}
		for (int i = 0; i < 20; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				GUI.DrawTexture(new Rect(400 + 16 * i, 10 + 16 * j, 16f, 16f), img);
			}
		}
	}

	private bool rollover(Rect button, Vector2 mousePos)
	{
		return mousePos.x > button.xMin && mousePos.y > button.yMin && mousePos.x < button.xMax && mousePos.y < button.yMax;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
