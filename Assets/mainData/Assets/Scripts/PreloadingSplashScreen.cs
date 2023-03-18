using System;
using UnityEngine;

public class PreloadingSplashScreen : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
        
	public GUIStyle presentsStyle;

	private Texture2D splashTexture;

	private Texture2D TASGAZLogo;

	private int x;

	private int y;

	private int tx;

	private int ty;

	private int width;

	private int height;

	private int twidth;

	private int theight;

	private int px;

	private int py;

	private float pwidth;

	private string presents = "Presents...";

	private float dotTime;

	private float dotDelay = 0.15f;

	private float dotCount = 3f;

	private bool loaded;

	private void Start()
	{
		// ABundleTester.DoCoroutine();  // CSP - testing bundle load()
		// CabToU3d.DoCoroutine();   // CSP - test assetbundle cache
  

		splashTexture = (Texture2D)Resources.Load("GUI/loading/preloading_blue_backdrop");
		TASGAZLogo = (Texture2D)Resources.Load("GUI/loading/loader_gaz_tas_logos");
		if (splashTexture != null)
		{
			width = Screen.width;
			height = Screen.height;
			if (splashTexture != null)
			{
				twidth = 849;
				theight = 195;
				loaded = true;
				dotTime = Time.time;
			}
		}
	}


	private void Update()
	{
		x = Screen.width / 2 - width / 2;
		y = Screen.height / 2 - height / 2;
		twidth = 849 * Screen.width / 1020;
		theight = 195 * Screen.width / 1020;
		tx = x + width / 2 - twidth / 2;
		ty = y + height / 2 - Convert.ToInt32((double)theight * 0.62);
		px = x + width / 2;
		py = y + height / 2 + 69;
		Vector2 vector = presentsStyle.CalcSize(new GUIContent("Presents..."));
		pwidth = vector.x;
		float time = Time.time;
		if (time - dotTime > dotDelay)
		{
			dotTime = Time.time;
			dotCount += 1f;
			if (dotCount > 3f)
			{
				dotCount = 0f;
			}
			presents = "Presents";
			for (int i = 0; (float)i < dotCount; i++)
			{
				presents += ".";
			}
		}
	}

	private void OnGUI()
	{
		GUI.depth = 100;
		if (loaded)
		{
			GUI.DrawTexture(new Rect(x, y, width, height), splashTexture);
			GUI.DrawTexture(new Rect(tx, ty, twidth, theight), TASGAZLogo);
			Color color = GUI.color;
			GUI.color = ColorUtil.FromRGB255(232, 230, 208);
			Color color2 = GUI.color;
			float r = color2.r;
			Color color3 = GUI.color;
			float g = color3.g;
			Color color4 = GUI.color;
			GUI.color = new Color(r, g, color4.b, 0.4f);
			GUI.Label(new Rect((float)px - pwidth / 2f + 1f, py + 1, pwidth, 35f), presents, presentsStyle);
			GUI.color = ColorUtil.FromRGB255(16, 18, 64);
			GUI.Label(new Rect((float)px - pwidth / 2f, py, pwidth, 35f), presents, presentsStyle);
			GUI.color = color;
		}
	}
}
