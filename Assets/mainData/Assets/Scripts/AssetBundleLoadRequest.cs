using UnityEngine;

public class AssetBundleLoadRequest
{
	public string Path = string.Empty;

	public string FullPath = string.Empty;

	public WWW LoaderWww;

	public AssetBundleLoader.AssetBundleLoaderCallback Callback;

	public object ExtraData;

	public CachedAssetBundle CacheHit;
}
