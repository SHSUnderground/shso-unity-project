using System;
using UnityEngine;

public class SHSBrawlerRailsIntroGadget : SHSGadget
{
	public class SHSBrawlerRailsTopWindow : GadgetTopWindow
	{
		public GUILabel title;

		public SHSBrawlerRailsTopWindow()
		{
			GUIImage gUIImage = new GUIImage();
			gUIImage.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 10f));
			gUIImage.TextureSource = "persistent_bundle|gadget_topmodule";
			gUIImage.SetSize(592f, 141f);
			Add(gUIImage);
			title = new GUILabel();
			title.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f));
			title.SetSize(379f, 60f);
			title.Text = "Tutorial (Missions)";
			title.TextAlignment = TextAnchor.MiddleCenter;
			title.TextColor = Color.white;
			title.FontSize = 34;
			Add(title);
			base.AnimationOnOpen = SHSAnimations.WindowOpenCloseDelegates.FadeIn(0.3f, gUIImage, title);
			base.AnimationOnClose = SHSAnimations.WindowOpenCloseDelegates.FadeOut(0.3f, gUIImage, title);
		}
	}

	public SHSBrawlerRailsTopWindow topWindow;

	public SHSBrawlerRailsIntroGadget()
	{
		topWindow = new SHSBrawlerRailsTopWindow();
		topWindow.Id = "SHSBrawlerRailsTopWindow";
		SetupOpeningTopWindow(topWindow);
	}

	public void SetContentWindow(Type windowType)
	{
		if (typeof(GadgetWindow).IsAssignableFrom(windowType))
		{
			GadgetWindow gadgetWindow = (GadgetWindow)Activator.CreateInstance(windowType);
			SetupOpeningWindow(BackgroundType.OnePanel, gadgetWindow);
		}
		else
		{
			CspUtils.DebugLog("Window is NOT a gadget window for setting content");
		}
	}
}
