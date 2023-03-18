using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSItemLoadingWindow : GUISimpleControlWindow
{
	private float startTime = Time.time;

	private static int NUM_LOADING_IMAGES = 20;

	private static int FPS = 15;

	private static bool clInitialized;

	private static List<Texture2D> loadingTextures;

	public override bool IsContentLoaded
	{
		get
		{
			return base.IsContentLoaded;
		}
	}

	public override void DrawContentLoading(DrawModeSetting drawFlags)
	{
		if (!clInitialized)
		{
			initializeContentLoadingResources();
		}
		bool enabled = GUI.enabled;
		Color color = GUI.color;
		GUI.enabled = false;
		Draw(drawFlags);
		GUI.enabled = true;
		GUI.DrawTexture(new Rect(5f, 5f, 90f, 90f), loadingTextures[Convert.ToInt32((Time.time - startTime) * (float)FPS % (float)(NUM_LOADING_IMAGES - 1))], ScaleMode.StretchToFill);
		GUI.enabled = enabled;
		GUI.color = color;
	}

	private static void initializeContentLoadingResources()
	{
		loadingTextures = new List<Texture2D>();
		for (int i = 0; i < NUM_LOADING_IMAGES; i++)
		{
			loadingTextures.Add(GUIManager.Instance.LoadTexture(string.Format("common_bundle|loading_stars_{0}", i)));
		}
		clInitialized = true;
	}
}
