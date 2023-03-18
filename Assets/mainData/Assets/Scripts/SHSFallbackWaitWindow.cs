using UnityEngine;

public class SHSFallbackWaitWindow : GUITopLevelWindow
{
	public GUIDrawTexture BackgroundLoading;

	protected GUIProgressBar progressBar;

	protected GUIImage backdrop;

	private GUILabel helperText;

	private GUILabel titleText;

	public SHSFallbackWaitWindow()
		: base("SHSFallbackWaitWindow")
	{
	}

	public override bool InitializeResources(bool reload)
	{
		backdrop = new GUIImage();
		backdrop.SetPosition(QuickSizingHint.Centered);
		backdrop.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		backdrop.TextureSource = "gui/white2x2";
		backdrop.Color = GUILabel.GenColor(56, 86, 172);
		backdrop.Traits.RespectDisabledAlphaTrait = ControlTraits.RespectDisabledAlphaTraitEnum.DisrespectDisabledAlpha;
		Add(backdrop);
		BackgroundLoading = new GUIDrawTexture();
		BackgroundLoading.SetPosition(QuickSizingHint.Centered);
		BackgroundLoading.SetSize(1020f, 644f);
		BackgroundLoading.Id = "loadingBackground";
		BackgroundLoading.Text = "BackgroundTexture";
		BackgroundLoading.Rotation = 0f;
		BackgroundLoading.Color = new Color(1f, 1f, 1f, 1f);
		BackgroundLoading.TextureSource = "gui/loading/loading";
		BackgroundLoading.IsVisible = true;
		Add(BackgroundLoading);
		progressBar = new GUIProgressBar();
		progressBar.Id = "percentBar";
		progressBar.UpdateSpeed = 3000f;
		progressBar.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -80f));
		progressBar.SetSize(800f, 30f);
		progressBar.IsVisible = true;
		Add(progressBar);
		titleText = new GUILabel();
		titleText.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 36, Color.white, TextAnchor.MiddleCenter);
		titleText.Id = "titleText";
		titleText.Text = "#loading";
		titleText.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, 20f));
		titleText.SetSize(500f, 100f);
		titleText.IsVisible = true;
		Add(titleText);
		helperText = new GUILabel();
		helperText.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 36, Color.white, TextAnchor.MiddleCenter);
		helperText.Id = "helperText";
		helperText.Text = "#loading";
		helperText.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -80f));
		helperText.SetSize(500f, 100f);
		helperText.IsVisible = true;
		Add(helperText);
		return base.InitializeResources(reload);
	}

	protected override void InitializeBundleList()
	{
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("brawler_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		base.InitializeBundleList();
	}
}
