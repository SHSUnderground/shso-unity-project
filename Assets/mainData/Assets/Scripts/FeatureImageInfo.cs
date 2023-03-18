public class FeatureImageInfo
{
	public string ImagePrefix;

	public int ImageCount;

	public string BackupImage;

	public int CurrentImageIndex;

	public GUIButton BuyButton;

	public bool WebContentLoaded;

	public string CachePath
	{
		get
		{
			return string.Format("{0}{1}_{2}.png", "/", ImagePrefix, string.Format("{0:0#}", CurrentImageIndex));
		}
	}

	public string TexturePath
	{
		get
		{
			return string.Format("{0}{1}{2}_{3}.png#{4}", "WEBCACHE$", "/", ImagePrefix, string.Format("{0:0#}", CurrentImageIndex), string.Format("{0}|{1}", "gameworld_bundle", BackupImage));
		}
	}

	public void OnWebContentItemLoaded(ShsWebResponse response)
	{
		WebContentLoaded = (response.Status == 200);
		if (BuyButton != null)
		{
			BuyButton.IsVisible = (WebContentLoaded && LauncherSequences.DependencyCheck(AssetBundleLoader.BundleGroup.Characters, false));
		}
		FeatureImageLoader.OnFeatureImageLoaded();
	}

	public void SetBuyButton(GUIButton button)
	{
		BuyButton = button;
		if (BuyButton != null)
		{
			BuyButton.IsVisible = (WebContentLoaded && LauncherSequences.DependencyCheck(AssetBundleLoader.BundleGroup.Characters, false));
		}
	}
}
