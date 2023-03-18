public class ShsCachedWebSource : ShsWebSource
{
	public ShsCachedWebSource()
	{
	}

	public ShsCachedWebSource(string[] uriArray)
		: base(uriArray)
	{
	}

	public override void GetCacheInfo(string resource, out bool isCached, out int version)
	{
		isCached = ShsCacheManager.IsResourceCached(resource, out version);
	}
}
