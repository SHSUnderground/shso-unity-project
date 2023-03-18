public class LoadAssetRequest
{
	public CachedAssetBundle CachedBundle;

	public string AssetName;

	public AssetBundleLoader.AssetLoadedCallback Callback;

	public object ExtraData;

	public bool IsPreload;
}
