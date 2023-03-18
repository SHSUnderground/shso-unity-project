public class SHSHqRailsMainWindow : GUITopLevelWindow
{
	public SHSHqRailsMainWindow()
		: base("SHSHqRailsMainWindow")
	{
	}

	protected override void InitializeBundleList()
	{
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("tutorial_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("persistent_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("gameworld_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("hq_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		base.InitializeBundleList();
	}
}
