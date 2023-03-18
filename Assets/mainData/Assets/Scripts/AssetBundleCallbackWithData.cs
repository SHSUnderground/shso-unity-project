public class AssetBundleCallbackWithData
{
	public AssetBundleLoader.AssetBundleLoaderCallback Callback;

	public object ExtraData;

	public AssetBundleCallbackWithData(AssetBundleLoader.AssetBundleLoaderCallback cb, object ed)
	{
		Callback = cb;
		ExtraData = ed;
	}
}
