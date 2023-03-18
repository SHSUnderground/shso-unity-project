public class SHSBrawlerWaitWindow : SHSWaitWindow
{
	private GUIWindow alternateLoadingWindow;

	public SHSBrawlerWaitWindow()
	{
		Id = "BrawlerWaitWindow";
	}

	public override bool InitializeResources(bool reload)
	{
		if (!base.InitializeResources(reload))
		{
			return false;
		}
		if (alternateLoadingWindow != null)
		{
			alternateLoadingWindow.IsVisible = true;
			Add(alternateLoadingWindow, loadingBarBehindFrame);
			BackgroundLoading.IsVisible = false;
		}
		base.AnimationOnOpen = SHSAnimations.WindowOpenCloseDelegates.FadeIn(0f, this);
		base.AnimationOnClose = SHSAnimations.WindowOpenCloseDelegates.FadeOut(0.5f, this);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		Traits.LifeSpan = ControlTraits.LifeSpanTraitEnum.DestroyOnHide;
		return true;
	}

	public void ChangeLoadScreen(GUIWindow newLoadScreen)
	{
		GUIWindow gUIWindow = alternateLoadingWindow;
		alternateLoadingWindow = newLoadScreen;
		if (resourcesInitialized)
		{
			if (gUIWindow != null)
			{
				Remove(gUIWindow);
			}
			alternateLoadingWindow.IsVisible = true;
			Add(alternateLoadingWindow, loadingBarBehindFrame);
			BackgroundLoading.Hide();
			progressBar.Value = 0f;
		}
	}

	public void ResetLoadScreen()
	{
		if (alternateLoadingWindow != null && resourcesInitialized)
		{
			BackgroundLoading.IsVisible = alternateLoadingWindow.IsVisible;
			Remove(alternateLoadingWindow);
		}
		alternateLoadingWindow = null;
	}

	public void ResetProgressBar()
	{
		progressBar.Value = 0f;
	}

	public void SetUIVisibility()
	{
		leftTipBoxImg.IsVisible = false;
		centerTipBoxImg.IsVisible = false;
		rightTipBoxImg.IsVisible = false;
		titleText.IsVisible = false;
		tipText.IsVisible = false;
		whoseTipText.IsVisible = false;
		tipBoxIcon.IsVisible = false;
		progressBar.IsVisible = true;
		loadingBarFrame.IsVisible = true;
		loadingBarBehindFrame.IsVisible = true;
		loadingTextImg.IsVisible = true;
		loadingBarFillTip.IsVisible = true;
	}
}
