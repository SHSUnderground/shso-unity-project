using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Test/Kevin/Cache Inspector")]
public class CacheInspector : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public long inUse;

	public long available;

	public bool cacheEnabled;

	public bool cacheAuthorized;

	public bool clearCache;

	private void Update()
	{
		base.enabled = false;
		cacheAuthorized = ShsCacheManager.enabled;
		cacheEnabled = Caching.enabled;
		available = Caching.spaceFree;
		if (clearCache)
		{
			clearCache = false;
			Caching.CleanCache();
		}
		inUse = Caching.spaceOccupied;
	}
}
