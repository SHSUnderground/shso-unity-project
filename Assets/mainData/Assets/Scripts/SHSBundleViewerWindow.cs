using System.Collections.Generic;
using UnityEngine;

public class SHSBundleViewerWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private Vector2 scrollPos;

	private SHSStyle headerStyle;

	private GUIToggleButton guiOnlyButton;

	private GUIToggleButton bundleStageOnlyButton;

	private GUIToggleButton detailedStageButton;

	public SHSBundleViewerWindow(string name)
		: base(name, null)
	{
		guiOnlyButton = new GUIToggleButton();
		guiOnlyButton.Text = "GUI View";
		guiOnlyButton.Spacing = 30f;
		guiOnlyButton.SetButtonSize(new Vector2(30f, 30f));
		guiOnlyButton.SetPositionAndSize(20f, 20f, 200f, 30f);
		guiOnlyButton.StyleInfo = SHSInheritedStyleInfo.Instance;
		Add(guiOnlyButton);
		bundleStageOnlyButton = new GUIToggleButton();
		bundleStageOnlyButton.Text = "Load Stages";
		bundleStageOnlyButton.Spacing = 30f;
		bundleStageOnlyButton.SetButtonSize(new Vector2(30f, 30f));
		bundleStageOnlyButton.SetPositionAndSize(220f, 20f, 200f, 30f);
		bundleStageOnlyButton.StyleInfo = SHSInheritedStyleInfo.Instance;
		Add(bundleStageOnlyButton);
		detailedStageButton = new GUIToggleButton();
		detailedStageButton.Text = "Background Status";
		detailedStageButton.Spacing = 30f;
		detailedStageButton.SetButtonSize(new Vector2(30f, 30f));
		detailedStageButton.SetPositionAndSize(420f, 20f, 200f, 30f);
		detailedStageButton.StyleInfo = SHSInheritedStyleInfo.Instance;
		Add(detailedStageButton);
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		if (guiOnlyButton.Value)
		{
			GUILayout.BeginArea(new Rect(30f, 30f, 550f, base.rect.height - 50f));
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			GUILayout.BeginVertical();
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Label("GUI Bundles", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
			Color color = GUI.color;
			GUILayout.BeginHorizontal(GUILayout.Width(700f));
			GUILayout.Label("Bundle", headerStyle.UnityStyle, GUILayout.Width(150f));
			GUILayout.Label("State", headerStyle.UnityStyle, GUILayout.Width(60f));
			GUILayout.Label("Main Loaded", headerStyle.UnityStyle, GUILayout.Width(100f));
			GUILayout.Label("Locale", headerStyle.UnityStyle, GUILayout.Width(100f));
			GUILayout.Label("Locale Loaded?", headerStyle.UnityStyle, GUILayout.Width(100f));
			GUILayout.Label("Failover?", headerStyle.UnityStyle, GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			foreach (GUIBundleManager.BundleLoadInfo assetBundle in GUIManager.Instance.BundleManager.AssetBundles)
			{
				GUILayout.BeginHorizontal(GUILayout.Width(700f));
				GUILayout.Label(assetBundle.BundleName, headerStyle.UnityStyle, GUILayout.Width(150f));
				GUILayout.Label(assetBundle.State.ToString(), headerStyle.UnityStyle, GUILayout.Width(60f));
				if ((assetBundle.LocaleDependency == GUIBundleManager.BundleLocaleParamsEnum.NotLocalized && assetBundle.Bundle == null) || (assetBundle.LocaleDependency == GUIBundleManager.BundleLocaleParamsEnum.LocalizedOnly && assetBundle.LanguageBundle == null) || (assetBundle.LocaleDependency == GUIBundleManager.BundleLocaleParamsEnum.Localized && assetBundle.Bundle == null && assetBundle.LanguageBundle == null && assetBundle.EnglishBundle == null))
				{
					GUI.color = Color.red;
				}
				GUILayout.Label((!(assetBundle.Bundle != null)) ? "-" : "Loaded", headerStyle.UnityStyle, GUILayout.Width(100f));
				GUILayout.Label(assetBundle.CurrentLocale, headerStyle.UnityStyle, GUILayout.Width(100f));
				GUILayout.Label((!(assetBundle.LanguageBundle != null)) ? "-" : "Loaded", headerStyle.UnityStyle, GUILayout.Width(100f));
				GUILayout.Label((!(assetBundle.EnglishBundle != null)) ? "-" : "Loaded", headerStyle.UnityStyle, GUILayout.Width(100f));
				GUI.color = color;
				GUILayout.Label((!assetBundle.LocalizedFallbackRequest) ? "-" : "Fail", headerStyle.UnityStyle, GUILayout.Width(50f));
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}
		else if (bundleStageOnlyButton.Value)
		{
			GUILayout.BeginArea(new Rect(30f, 30f, 900f, base.rect.height - 50f));
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			GUILayout.BeginVertical();
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Label("Bundle Stages", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
			GUILayout.BeginHorizontal(GUILayout.Width(900f));
			GUILayout.Label("Bundle Group", headerStyle.UnityStyle, GUILayout.Width(150f));
			GUILayout.Label("Min", headerStyle.UnityStyle, GUILayout.Width(60f));
			GUILayout.Label("Max", headerStyle.UnityStyle, GUILayout.Width(60f));
			GUILayout.Label("Pct", headerStyle.UnityStyle, GUILayout.Width(100f));
			GUILayout.Label("Loaded?", headerStyle.UnityStyle, GUILayout.Width(100f));
			GUILayout.EndHorizontal();
			GUILayout.Space(30f);
			AssetBundleLoader.GroupInfo[] bundleGroups = AppShell.Instance.BundleLoader.BundleGroups;
			foreach (AssetBundleLoader.GroupInfo groupInfo in bundleGroups)
			{
				string text = "N";
				if (groupInfo.PercentDone >= 1f)
				{
					text = "Y";
				}
				GUILayout.BeginHorizontal(GUILayout.Width(700f));
				GUILayout.Label(groupInfo.group.ToString(), headerStyle.UnityStyle, GUILayout.Width(150f));
				GUILayout.Label(groupInfo.minPriority.ToString(), headerStyle.UnityStyle, GUILayout.Width(60f));
				GUILayout.Label(groupInfo.maxPriority.ToString(), headerStyle.UnityStyle, GUILayout.Width(60f));
				GUILayout.Label(groupInfo.PercentDone.ToString(), headerStyle.UnityStyle, GUILayout.Width(100f));
				GUILayout.Label(text, headerStyle.UnityStyle, GUILayout.Width(100f));
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}
		else if (detailedStageButton.Value)
		{
			GUILayout.BeginArea(new Rect(30f, 30f, 900f, base.rect.height - 50f));
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			GUILayout.BeginVertical();
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Label("Bundle Stages", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
			GUILayout.BeginHorizontal(GUILayout.Width(900f));
			GUILayout.Label("Bundle Group", headerStyle.UnityStyle, GUILayout.Width(150f));
			GUILayout.Label("File", headerStyle.UnityStyle, GUILayout.Width(250f));
			GUILayout.Label("State", headerStyle.UnityStyle, GUILayout.Width(100f));
			GUILayout.EndHorizontal();
			GUILayout.Space(30f);
			AssetBundleLoader.BundleInfo[] bundles = AppShell.Instance.BundleLoader.Bundles;
			foreach (AssetBundleLoader.BundleInfo bundleInfo in bundles)
			{
				GUILayout.BeginHorizontal(GUILayout.Width(700f));
				GUILayout.Label(bundleInfo.Group.group.ToString(), headerStyle.UnityStyle, GUILayout.Width(150f));
				GUILayout.Label(bundleInfo.name, headerStyle.UnityStyle, GUILayout.Width(250f));
				GUILayout.Label(bundleInfo.State.ToString(), headerStyle.UnityStyle, GUILayout.Width(100f));
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}
		else
		{
			GUILayout.BeginArea(new Rect(30f, 30f, 1000f, base.rect.height - 50f));
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			GUILayout.BeginVertical();
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Label("Game Bundles", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
			GUILayout.BeginHorizontal(GUILayout.Width(1000f));
			GUILayout.Label("Bundle Name", headerStyle.UnityStyle, GUILayout.Width(200f));
			GUILayout.Label("Size", headerStyle.UnityStyle, GUILayout.Width(100f));
			GUILayout.Label("Time", headerStyle.UnityStyle, GUILayout.Width(100f));
			GUILayout.Label("CB", headerStyle.UnityStyle, GUILayout.Width(50f));
			GUILayout.Label("PRE ", headerStyle.UnityStyle, GUILayout.Width(50f));
			GUILayout.Label("BRef", headerStyle.UnityStyle, GUILayout.Width(50f));
			GUILayout.Label("Scene #", headerStyle.UnityStyle, GUILayout.Width(50f));
			GUILayout.Label("Unload?", headerStyle.UnityStyle, GUILayout.Width(50f));
			GUILayout.Label("Path", headerStyle.UnityStyle, GUILayout.Width(350f));
			GUILayout.EndHorizontal();
			AssetBundleLoader bundleLoader = AppShell.Instance.BundleLoader;
			if (bundleLoader != null)
			{
				foreach (KeyValuePair<string, CachedAssetBundle> cachedBundle in bundleLoader.CachedBundles)
				{
					GUILayout.BeginHorizontal(GUILayout.Width(1000f));
					GUILayout.Label(cachedBundle.Key.Substring(cachedBundle.Key.LastIndexOf("/") + 1), headerStyle.UnityStyle, GUILayout.Width(200f));
					GUILayout.Label(cachedBundle.Value.SizeInBytes.ToString(), headerStyle.UnityStyle, GUILayout.Width(100f));
					GUILayout.Label(cachedBundle.Value.TimeLoaded.ToString(), headerStyle.UnityStyle, GUILayout.Width(100f));
					GUILayout.Label(cachedBundle.Value.OutstandingCallbacks.Count.ToString(), headerStyle.UnityStyle, GUILayout.Width(50f));
					GUILayout.Label(cachedBundle.Value.PreloadedAssets.Count.ToString(), headerStyle.UnityStyle, GUILayout.Width(50f));
					GUILayout.Label((!(cachedBundle.Value.Bundle == null)) ? "obj " : "null", headerStyle.UnityStyle, GUILayout.Width(50f));
					GUILayout.Label(cachedBundle.Value.sceneRequested.ToString(), headerStyle.UnityStyle, GUILayout.Width(50f));
					GUILayout.Label(cachedBundle.Value.UnloadOnSceneTransition.ToString(), headerStyle.UnityStyle, GUILayout.Width(50f));
					GUILayout.Label(cachedBundle.Value.RequestPath, headerStyle.UnityStyle, GUILayout.Width(350f));
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		headerStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleEventHeader");
		scrollPos = Vector2.zero;
	}
}
