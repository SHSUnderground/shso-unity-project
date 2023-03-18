public class SHSBrawlerRailsMainWindow : GUITopLevelWindow
{
	public SHSBrawlerRailsMainWindow()
		: base("SHSBrawlerRailsMainWindow")
	{
	}

	public override bool InitializeResources(bool reload)
	{
		return base.InitializeResources(reload);
	}

	public override void OnShow()
	{
		base.OnShow();
	}

	protected override void InitializeBundleList()
	{
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("brawler_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("tutorial_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		base.InitializeBundleList();
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
	}
}
