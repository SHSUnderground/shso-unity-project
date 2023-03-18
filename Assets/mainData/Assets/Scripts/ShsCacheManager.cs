using System.Collections.Generic;
using System.Net;
using System.Xml.XPath;
using UnityEngine;

public class ShsCacheManager : Singleton<ShsCacheManager>
{
	protected class ShsCacheKey
	{
		public string CacheName = "SHS";

		public string Signature;

		public string Url;

		public string UrlMatch;

		public long CacheSize = 10737418240L;

		public ShsCacheKey(string cacheName, string signature, string url, long cacheSize, string urlMatch)
		{
			CacheName = cacheName;
			Signature = signature;
			Url = url;
			CacheSize = cacheSize;
			UrlMatch = urlMatch;
		}
	}

	public class ManifestBundleInfo
	{
		public string BundlePath;

		public int Version;

		public string Locale;

		public uint Priority;

		public bool Active;

		public ManifestBundleInfo(string bundlePath, int version, string locale, uint priority, bool active)
		{
			BundlePath = bundlePath;
			Version = version;
			Locale = locale;
			Priority = priority;
			Active = active;
		}
	}

	private enum LocaleManifestLoadStatus
	{
		Unknown,
		Load,
		Complete
	}

	public delegate void ManifestLoadedDelegate(bool success);

	public const string MANIFEST_FILE = "AssetBundles/Configuration/AssetBundleManifest.xml";

	public const string CACHE_PREFIX = "assetbundles/";

	public const string LOCALE_MANIFEST_FILE = "AssetBundles/Configuration/LocaleBundleManifest.xml";

	public readonly string MANIFEST_FILE_WEB_PATH = "localfile$AssetBundles/Configuration/AssetBundleManifest.xml";

	public readonly string LOCALE_MANIFEST_FILE_WEB_PATH = "localfile$AssetBundles/Configuration/LocaleBundleManifest.xml";

	protected ShsCacheKey[] _cacheKeys = new ShsCacheKey[11];

	private readonly bool _enabled;

	private Dictionary<string, ManifestBundleInfo> _manifest;

	private Dictionary<string, ManifestBundleInfo> _localeManifest;

	private LocaleManifestLoadStatus _loadLocaleStatus;

	private bool _refreshPending;

	private int _refreshRetriesRemaining = -1;

	private bool _refreshLocalePending;

	private int _refreshLocaleRetriesRemaining = -1;

	public static bool enabled
	{
		get
		{
			return Singleton<ShsCacheManager>.instance._enabled;
		}
	}

	public static Dictionary<string, ManifestBundleInfo> Manifest
	{
		get
		{
			return Singleton<ShsCacheManager>.instance._manifest;
		}
	}

	public event ManifestLoadedDelegate ManifestLoadedEvent;

	public ShsCacheManager()
	{
		CspUtils.DebugLog("Using 3.2 cache branch");
		_cacheKeys[0] = new ShsCacheKey("SHS", "19b10a3b041b6a1b01c9ceb450afb17682e383ed529b37c069a17f343926e310b2485b0a4f52fee59f0d78621b97bf2555bc92fda27fde3aa9b3e57ea03115d1e2e9f6d88c92d27643c414d510ccffe4717164a0f8d62d5853b5857230110e27abd51fdd95fade3d0ac8be5cc7f0c16feb3e200461e47422ee2d2c5e6ad042dd", "http://heroup.com/", 10737418240L, "heroup.com");
		_cacheKeys[1] = new ShsCacheKey("SHS", "23145a7af815949d522bf7a4f585a08d11c7d0e90e760a1e8f5a84dda7964c63ad9f79f9d2db1c62e3777021635b782f9d6a6c0c90faa668bbc0e292c68f22713613501c1373c679e63cdbcff60b595fda3968f0954cef63e1777d44567a6b54dc243769420e0b88731c964dcf680d14f7b62725ad144111a501328b297b9300", "http://hq-california.com/", 10737418240L, "hq-california.com");
		_cacheKeys[2] = new ShsCacheKey("SHS", "309f59cdf5822752100f4b0dc8ab1dee54840c5d96be9dbe2daa90770ddc7813ef9fbc1837211d0a4fc32ad9ee40a03ad07c84d67bb46a74ef74906c7378795f233d43db01f0c95b598197fcf5b9c303ab617139476a9c8a1a8ca29d0ebb89c8c010905e76547a063bdec387a20304452cbff21d534cb3e5f2f2ce1b70571b72", "http://shs.tas/", 10737418240L, "shs.tas");
		_cacheKeys[3] = new ShsCacheKey("SHS", "096ebaa204ff193d415e3c330cef525a873c59cfa21c6120000d47fbb91b0b9a68d5e261f07defa3d13484c53407115130268e59de4fd08645a5b62eb22e319f2c43fb655f523ea23671c6f52afbc05fb617168d9dc7fd15672d0946077a126508d9db82e03cca382c7d9832b0b9a5351f4586bed49728fa70b04983252e6927", "http://d1hs5x6xvqpt2.cloudfront.net/", 10737418240L, "d1hs5x6xvqpt2.cloudfront.net");
		_cacheKeys[4] = new ShsCacheKey("SHS", "191c94994f174477a6c16acdbb4c0d5c2d6f8cb7b23d0844982fa999aa09a8a342ce9f52a45222ccf3ddf6aebd7a5c0b621893c731a93dc7de96ba0d9be55bbe3c69c2b97d0d011ad719c4abefba3f1e8b36116db792467e7829b64ded4093524daca86b7f5c17ca5f3483b7877e52a7882ec2e63607cdce63985a03321b5c2f", "http://d1cl77k00408sp.cloudfront.net/", 10737418240L, "d1cl77k00408sp.cloudfront.net");
		_cacheKeys[5] = new ShsCacheKey("SHS", "1739af9370a2cb678ec58c068434403edb4bc9da8dd7c6b0e23f6d835e8574937054a84ee19c059cb03e8832d49de3fcf28de46d4afdea2a349692402f15827496fe8c46257383609cd3f4ce18542217210db56a80f66a4221cd00980382a206b2a8c0a51ca4b3c828923eb4e1d0322b48452f1bbceaeefe03a6f7a61ab3fedc", "http://shs-dev.s3.amazonaws.com/", 10737418240L, "shs-dev.s3.amazonaws.com");
		_cacheKeys[6] = new ShsCacheKey("SHS", "1213cf62a608bf9398c66f2fe976b414270fa321f521fbb8be60d625cdac70d99c128ab63de43c11a6482c4d215cc6e64d822a625ee93fb01d3a7f4acab24ae4969a6e0876318e137c646a11fff4b70cafcdad1f043df4425ee9deccc3180f517c5f3acba8947986565e731deaf52f15362386cc41a5678455e02242a7fab5eb", "http://heroup.net/", 10737418240L, "heroup.net");
		_cacheKeys[7] = new ShsCacheKey("SHS", "81d324ae48b59ae1c66e680edc54411692a9c030c0f2153f15c7f866d5284409abbaa03f02f5783516ec6b4ce9045fa5dd265ca5e9ea4fc4b2d722fa71efc478bd86c4f470ff18273182ed6e65e6767638c2adf5245658bfcc716ba536d9ec72ae2f1903487544dd263b73b0170dbd8ef982cb06ced533bfd71f8729b401be38", "http://super-hero-game.eu/", 10737418240L, "super-hero-game.eu");
		_cacheKeys[8] = new ShsCacheKey("SHS", "5404f37cdcbd6e8c31938c903fbba2c61005983f5970bdc3892b503a1be495cb4653ab8e63e3dcf33d6f8fe46d1f094c7990c4cea4b29bd9fab4a3f3fd18294cb7418b55671b31ec9dd48aec1a1dab73934a273a347d19e2a62061c8e35de30de4324753bb32a557e3b38754c7e2a7ff41d7a0fe1d928665461aa46b2fe48421", "http://d8n8brx8s9byk.cloudfront.net/", 10737418240L, "d8n8brx8s9byk.cloudfront.net");
		_cacheKeys[9] = new ShsCacheKey("SHS", "0346280f70c53efeab86e78d8ec810c82cd9207c53d0a02308439202c8ed451ba88af34245ce894e76d0841951acfcde8f4bb7ea49d0ba51f4744237fa9b2579150b406f2bc562b67b7583b53410fce4e6c02f123d514133c745156dad7f8c9b53e3a06bb497eb555bb11ef9bdb01f667cb33be8db79aba7743aca16a8d59660", "http://nivalnetwork.com/", 10737418240L, "nivalnetwork.com");
		_cacheKeys[10] = new ShsCacheKey("SHS", "59b095a58209170f89f5a773b58284cffbc7dd71b51c1abd10ccaa5a8d39046ee7138edcfc4a4bf3f57a5c5a12c444b32f472368870c14c2eb85ce210c69adc9afe4aba7a91f3ea01c16559174d43dba5c7d103446b24402a91aa729574622798d56ca20214f2e8da9801ff9910d4946733e9e9e9fa4941ac31b3263d8b4c65b", "http://playheroes.mail.ru/", 10737418240L, "playheroes.mail.ru");
		string text;
		if (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
		{
			MANIFEST_FILE_WEB_PATH = "content$AssetBundles/Configuration/AssetBundleManifest.xml";
			LOCALE_MANIFEST_FILE_WEB_PATH = "content$AssetBundles/Configuration/LocaleBundleManifest.xml";
			text = Application.absoluteURL;
		}
		else
		{
			text = "shs.tas";
		}
		ShsCacheKey[] cacheKeys = _cacheKeys;
		foreach (ShsCacheKey shsCacheKey in cacheKeys)
		{
			if (text.Contains(shsCacheKey.UrlMatch))
			{
				_enabled = Caching.Authorize(shsCacheKey.CacheName, shsCacheKey.Url, shsCacheKey.CacheSize, shsCacheKey.Signature);
				if (_enabled)
				{
					_RefreshManifest();
					break;
				}
			}
		}
		if (!_enabled)
		{
			CspUtils.DebugLog("Caching disabled - authorization failed for <" + text + ">");
		}
	}

	public static bool IsResourceCached(string resource, out int version)
	{
		// int pos = resource.LastIndexOf("/") + 1; // CSP
		// string tmp = resource.Substring(pos, resource.Length - pos); // CSP
		// resource = tmp; // CSP
		return Singleton<ShsCacheManager>.instance._IsResourceCached(resource, out version);
	}

	public static bool IsCachedVersionCurrent(string resource)
	{
		int version;
		if (IsResourceCached(resource, out version))
		{
			bool flag = Caching.IsVersionCached(resource, version);
			if (!flag)
			{
				CspUtils.DebugLog("CACHE: Downloading cached 3.2 resource <" + resource + "> has manifest version <" + version + ">.");
			}
			return flag;
		}
		return false;
	}

	public static void RefreshManifest()
	{
		Singleton<ShsCacheManager>.instance._RefreshManifest();
	}

	public static void InitializeLocaleManifestLoad(bool load)
	{
		CspUtils.DebugLog("received initialization for locale manifest load " + load);
		if (load)
		{
			Singleton<ShsCacheManager>.instance._loadLocaleStatus = LocaleManifestLoadStatus.Load;
			Singleton<ShsCacheManager>.instance._RefreshLocaleManifest();
		}
		else
		{
			Singleton<ShsCacheManager>.instance._loadLocaleStatus = LocaleManifestLoadStatus.Complete;
			Singleton<ShsCacheManager>.instance._TryToSendManifestLoadedEvent();
		}
	}

	public static Dictionary<string, int> GetManifestFromXml(string xml)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		DataWarehouse dataWarehouse = new DataWarehouse(xml);
		dataWarehouse.Parse();
		foreach (XPathNavigator item in Utils.Enumerate(dataWarehouse.GetValues("//bundle")))
		{
			string key = item.GetAttribute("path", string.Empty).ToLower();
			int num2 = dictionary[key] = int.Parse(item.GetAttribute("version", string.Empty));
			//CspUtils.DebugLog("manifest: key=" + key + "  value=" + int.Parse(item.GetAttribute("version", string.Empty)));
		}
		return dictionary;
	}

	public static Dictionary<string, ManifestBundleInfo> GetFullManifestFromXml(string xml)
	{
		Dictionary<string, ManifestBundleInfo> dictionary = new Dictionary<string, ManifestBundleInfo>();
		DataWarehouse dataWarehouse = new DataWarehouse(xml);
		dataWarehouse.Parse();
		foreach (XPathNavigator item in Utils.Enumerate(dataWarehouse.GetValues("//bundle")))
		{
			string attribute = item.GetAttribute("path", string.Empty);
			int version = int.Parse(item.GetAttribute("version", string.Empty));
			string attribute2 = item.GetAttribute("locale", string.Empty);
			attribute2 = ((!string.IsNullOrEmpty(attribute2)) ? attribute2 : "common");
			string attribute3 = item.GetAttribute("priority", string.Empty);
			uint priority = (!string.IsNullOrEmpty(attribute3)) ? uint.Parse(attribute3) : 99u;
			bool active = item.GetAttribute("active", string.Empty) == "1";
			ManifestBundleInfo value = new ManifestBundleInfo(attribute, version, attribute2, priority, active);
			dictionary[attribute.ToLower()] = value;

			// int pos = attribute.ToLower().LastIndexOf("/") + 1; // CSP
			// string bundleName = attribute.Substring(pos, attribute.Length - pos); // CSP
			// dictionary[bundleName.ToLower()] = value; // CSP

			// CspUtils.DebugLog("manifest: key=" + bundleName.ToLower() + "  value=" + value);  // CSP

		}
		return dictionary;
	}

	public static string CleanResourcePath(string resource)
	{
		string text = resource.ToLower().Replace('\\', '/');
		int num = text.IndexOf("assetbundles/");
		if (num >= 0)
		{
			text = text.Substring(num);
		}
		if (!text.StartsWith("assetbundles/"))
		{
			return null;
		}
		text = text.Split('?')[0];
		return text.Substring("assetbundles/".Length);
	}

	public static List<string> GetBundlesByPriority()
	{
		return GetBundlesToPriority(-1, 999);
	}

	public static List<string> GetBundlesToPriority(int minPriority, int maxPriority)
	{
		if (Singleton<ShsCacheManager>.instance._manifest == null)
		{
			CspUtils.DebugLog("GetBundlesByPriority called before manifest loaded");
			return null;
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, ManifestBundleInfo> item in Singleton<ShsCacheManager>.instance._manifest)
		{
			ManifestBundleInfo value = item.Value;
			if (value.Active && value.Priority >= minPriority && value.Priority <= maxPriority && AppShell.Instance.IsCurrentLocale(value.Locale))
			{
				list.Add(value.BundlePath);
			}
		}
		list.Sort(CompareBundlePriority);
		return list;
	}

	protected static int CompareBundlePriority(string x, string y)
	{
		uint priority = Singleton<ShsCacheManager>.instance._manifest[x.ToLower()].Priority;
		uint priority2 = Singleton<ShsCacheManager>.instance._manifest[y.ToLower()].Priority;
		if (priority > priority2)
		{
			return 1;
		}
		if (priority2 > priority)
		{
			return -1;
		}
		return 0;
	}

	private bool _IsResourceCached(string resource, out int version)
	{
		

		version = 0;
		if (!_enabled || _manifest == null)
		{
			if (_enabled && _refreshRetriesRemaining > 0 && !_refreshPending)
			{
				_RefreshManifest();
			}
			CspUtils.DebugLog("IRC resource=" + resource);   // CSP
			return false;
		}
		if (_loadLocaleStatus == LocaleManifestLoadStatus.Load)
		{
			if (_refreshLocaleRetriesRemaining > 0 && !_refreshLocalePending)
			{
				_RefreshLocaleManifest();
			}
			CspUtils.DebugLog("IRC resource=" + resource);   // CSP
			return false;
		}
		CspUtils.DebugLog("IRC resource(before)=" + resource);   // CSP
		resource = CleanResourcePath(resource);
		if (resource == null)
		{
			CspUtils.DebugLog("IRC resource=null");   // CSP
			return false;
		}
		ManifestBundleInfo value;

		// int pos = resource.LastIndexOf("/") + 1; // CSP
		// string tmp = resource.Substring(pos, resource.Length - pos); // CSP
		// resource = tmp; // CSP
		// CspUtils.DebugLog("IRC resource(substring)=" + resource + "   tmp=" + tmp);   // CSP

		if (_manifest.TryGetValue(resource, out value))
		{
			version = value.Version;
			return true;
		}
		CspUtils.DebugLog("CACHE: Looking in manifest: " + resource + ":" + _manifest.TryGetValue(resource, out value).ToString());
		return false;
	}

	private void _RefreshManifest()
	{
		if (!_refreshPending)
		{
			_refreshPending = true;
			AppShell.Instance.WebService.StartRequest(MANIFEST_FILE_WEB_PATH, OnManifestLoaded, ShsWebService.ShsWebServiceType.Text);
		}
	}

	private void _RefreshLocaleManifest()
	{
		if (!_refreshLocalePending)
		{
			_refreshLocalePending = true;
			AppShell.Instance.WebService.StartRequest(LOCALE_MANIFEST_FILE_WEB_PATH, OnLocaleManifestLoaded, ShsWebService.ShsWebServiceType.Text);
		}
	}

	private void OnManifestLoaded(ShsWebResponse response)
	{
		switch (response.Status)
		{
		case 200:
			_manifest = GetFullManifestFromXml(response.Body);
			_MergeManifest(ref _manifest, _localeManifest);
			_refreshRetriesRemaining = 0;
			_TryToSendManifestLoadedEvent();
			break;
		case 408:
			if (_refreshRetriesRemaining == -1)
			{
				_refreshRetriesRemaining = 5;
			}
			else
			{
				_refreshRetriesRemaining--;
			}
			break;
		}
		_refreshPending = false;
	}

	private void OnLocaleManifestLoaded(ShsWebResponse response)
	{
		HttpStatusCode status = (HttpStatusCode)response.Status;
		CspUtils.DebugLog("receipt for locale manifest load with http response status: " + status);
		switch (status)
		{
		case HttpStatusCode.OK:
			_localeManifest = GetFullManifestFromXml(response.Body);
			_MergeManifest(ref _manifest, _localeManifest);
			_refreshLocaleRetriesRemaining = 0;
			_loadLocaleStatus = LocaleManifestLoadStatus.Complete;
			_TryToSendManifestLoadedEvent();
			break;
		case HttpStatusCode.RequestTimeout:
			if (_refreshLocaleRetriesRemaining == -1)
			{
				_refreshLocaleRetriesRemaining = 5;
			}
			else
			{
				_refreshLocaleRetriesRemaining--;
			}
			break;
		}
		_refreshLocalePending = false;
	}

	private void _MergeManifest(ref Dictionary<string, ManifestBundleInfo> to, Dictionary<string, ManifestBundleInfo> from)
	{
		if (to != null && from != null)
		{
			foreach (KeyValuePair<string, ManifestBundleInfo> item in from)
			{
				if (to.ContainsKey(item.Key))
				{
					CspUtils.DebugLog("LocaleBundleManifest contains duplicate key <" + item.Key + "> found in AssetBundleManifest - keeping key's original value");
				}
				else
				{
					to.Add(item.Key, item.Value);
				}
			}
		}
	}

	private bool _IsManifestLoaded()
	{
		return _manifest != null && _loadLocaleStatus == LocaleManifestLoadStatus.Complete;
	}

	private void _TryToSendManifestLoadedEvent()
	{
		if (_IsManifestLoaded() && this.ManifestLoadedEvent != null)
		{
			this.ManifestLoadedEvent(true);
		}
	}
}
