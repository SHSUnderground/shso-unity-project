using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

internal class SHSControlCustomDrawMethods
{
	private static int NUM_LOADING_IMAGES = 20;

	private static int FPS = 15;

	private static bool clInitialized;

	private static Dictionary<IGUIControl, float> controlsLoadingState;

	private static List<Texture2D> loadingTextures;

	public static void OnLoadingActivateDefault(object extraData)
	{
		if (!SHSStagedDownloadWindow.DownloadStatusCurrentlyShowing)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Type type = executingAssembly.GetType("SHSStagedDownloadWindow");
			GUIWindow gUIWindow = (GUIWindow)Activator.CreateInstance(type);
			GUIManager.Instance.UIRoots[GUIManager.UILayer.System].Add(gUIWindow);
			gUIWindow.Show(GUIControl.ModalLevelEnum.Default);
		}
	}

	public static void DrawContentLoadingDefault(IGUIControl control, GUIControl.DrawModeSetting drawFlags)
	{
		if (!clInitialized)
		{
			initializeContentLoadingResources();
		}
		control.Draw(drawFlags);
		if (!controlsLoadingState.ContainsKey(control))
		{
			controlsLoadingState[control] = Time.time;
		}
		bool enabled = GUI.enabled;
		Color color = GUI.color;
		GUI.enabled = true;
		GUI.color = new Color(color.r, color.g, color.b, 1f);
		Rect contentLoadingCustomDrawRect = control.ContentLoadingCustomDrawRect;
		GUI.DrawTexture(contentLoadingCustomDrawRect, loadingTextures[Convert.ToInt32((Time.time - controlsLoadingState[control]) * (float)FPS % (float)(NUM_LOADING_IMAGES - 1))]);
		GUI.color = color;
		GUI.enabled = enabled;
	}

	private static void initializeContentLoadingResources()
	{
		controlsLoadingState = new Dictionary<IGUIControl, float>();
		loadingTextures = new List<Texture2D>();
		for (int i = 0; i < NUM_LOADING_IMAGES; i++)
		{
			loadingTextures.Add(GUIManager.Instance.LoadTexture(string.Format("common_bundle|loading_stars_{0}", (i + 1).ToString("00"))));
		}
		clInitialized = true;
	}
}
