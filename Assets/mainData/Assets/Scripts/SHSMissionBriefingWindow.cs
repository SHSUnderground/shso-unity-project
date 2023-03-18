using UnityEngine;

public class SHSMissionBriefingWindow : GUIControlWindow
{
	private const float newspaperSizeX = 612f;

	private const float newspaperSizeY = 792f;

	private GUIImage background;

	private GUIImage newspaper;

	private bool useScreensize;

	public SHSMissionBriefingWindow()
	{
		newspaper = new GUIImage();
		newspaper.IsVisible = true;
		background = new GUIImage();
		background.SetPositionAndSize(QuickSizingHint.ParentSize);
		background.TextureSource = "persistent_bundle|loading_bg_bluecircles";
		background.Color = new Color(1f, 1f, 1f, 1f);
		background.IsVisible = true;
		Add(background);
		Add(newspaper);
		SetPositionAndSize(QuickSizingHint.ScreenSize);
	}

	public void SetSplashImage(string TextureSource, bool UseScreensize)
	{
		newspaper.TextureSource = TextureSource;
		useScreensize = UseScreensize;
	}

	public override void OnShow()
	{
		base.OnShow();
		if (!useScreensize)
		{
			newspaper.SetPosition(QuickSizingHint.Centered);
			newspaper.SetSize(612f * (GUIManager.ScreenRect.height / 792f), 792f * (GUIManager.ScreenRect.height / 792f));
		}
		else
		{
			newspaper.SetSize(GUIManager.ScreenRect.width, GUIManager.ScreenRect.height);
		}
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		if (!useScreensize)
		{
			newspaper.SetPosition(QuickSizingHint.Centered);
			newspaper.SetSize(612f * (GUIManager.ScreenRect.height / 792f), 792f * (GUIManager.ScreenRect.height / 792f));
		}
		else
		{
			newspaper.SetSize(GUIManager.ScreenRect.width, GUIManager.ScreenRect.height);
		}
	}
}
