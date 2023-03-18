public class BundleGroupLoadedMessage : ShsEventMessage
{
	public AssetBundleLoader.BundleGroup groupUnlocked;

	public bool requiredDownload;

	public BundleGroupLoadedMessage(AssetBundleLoader.BundleGroup group, bool requiredDownload)
	{
		groupUnlocked = group;
		this.requiredDownload = requiredDownload;
	}
}
