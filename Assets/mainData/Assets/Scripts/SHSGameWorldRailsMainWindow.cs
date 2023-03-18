public class SHSGameWorldRailsMainWindow : GUITopLevelWindow
{
	public SHSGameWorldRailsMainWindow()
		: base("SHSGameWorldRailsMainWindow")
	{
	}

	protected override void InitializeBundleList()
	{
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("tutorial_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("persistent_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("gameworld_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("missions_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("missionflyers_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		base.InitializeBundleList();
	}
}
