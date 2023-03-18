using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class AssetBundleLoader : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum LoadCategory
	{
		Immediate,
		Background,
		Cache
	}

	public enum BundleGroup
	{
		TutorialUIBugle,
		PrizeWheel,
		Characters,
		MissionsAndEnemies,
		NonBugleGameWorlds,
		HQ,
		CardGame,
		SpecialMission,
		VO,
		All,
		Any
	}

	protected enum ProcessResult
	{
		Skipped,
		CheckedVersion,
		StartedDownload
	}

	public enum BundleState
	{
		Unknown,
		ForegroundDownload,
		BackgroundDownload,
		SoftCache,
		HardCache,
		Error
	}

	public class BundleInfo
	{
		private GroupInfo group;

		private BundleState state;

		public string name;

		public ShsCacheManager.ManifestBundleInfo manifest;

		private bool signaledReady;

		public GroupInfo Group
		{
			get
			{
				return group;
			}
		}

		public BundleState State
		{
			get
			{
				return state;
			}
			set
			{
				if (state == value || state == BundleState.HardCache)
				{
					return;
				}
				state = value;
				if (state == BundleState.SoftCache || state == BundleState.HardCache)
				{
					if (!signaledReady)
					{
						signaledReady = true;
						group.BundleReady(this);
					}
				}
				else if (state == BundleState.BackgroundDownload)
				{
					group.BundleDownload(this);
				}
				else if (state == BundleState.Error && !signaledReady)
				{
					signaledReady = true;
					group.BundleReady(this);
				}
			}
		}

		public BundleInfo(string name, GroupInfo group, ShsCacheManager.ManifestBundleInfo bundleInfo)
		{
			this.name = name.Replace(".unity3d", string.Empty);
			state = BundleState.Unknown;
			this.group = group;
			manifest = bundleInfo;
		}
	}

	public class GroupInfo
	{
		protected float percentDone;

		protected bool done;

		public BundleGroup group;

		public int minPriority;

		public int maxPriority;

		protected AssetBundleLoader parent;

		protected int totalBundles;

		protected int readyBundles;

		protected bool requiredDownload;

		protected bool sentEvent;

		public int TotalBundles
		{
			get
			{
				return totalBundles;
			}
		}

		public int ReadyBundles
		{
			get
			{
				return readyBundles;
			}
		}

		public virtual float PercentDone
		{
			get
			{
				return percentDone;
			}
		}

		public virtual bool Done
		{
			get
			{
				return done;
			}
		}

		public GroupInfo(AssetBundleLoader parent, BundleGroup group, int min, int max)
		{
			this.parent = parent;
			this.group = group;
			minPriority = min;
			maxPriority = max;
			percentDone = 0f;
			done = false;
			requiredDownload = false;
			sentEvent = false;
		}

		public void PostInitialize()
		{
			if (totalBundles <= 0)
			{
				done = true;
				percentDone = 1f;
				CheckLoadedEvent();
			}
		}

		public void AddBundle(BundleInfo bundle)
		{
			totalBundles++;
		}

		public void BundleDownload(BundleInfo bundle)
		{
			requiredDownload = true;
		}

		public void BundleReady(BundleInfo bundle)
		{
			readyBundles++;
			if (readyBundles >= totalBundles)
			{
				done = true;
				percentDone = 1f;
				CheckLoadedEvent();
			}
			else
			{
				percentDone = Mathf.Clamp01((float)readyBundles / (float)totalBundles);
			}
		}

		protected void CheckLoadedEvent()
		{
			for (int i = 0; i < 9; i++)
			{
				GroupInfo groupInfo = parent.BundleGroups[i];
				if (groupInfo.sentEvent)
				{
					continue;
				}
				bool flag = true;
				bool flag2 = false;
				BundleGroup[] array = parent.BundleDependencies[(int)groupInfo.group];
				for (int j = 0; j < array.Length; j++)
				{
					GroupInfo groupInfo2 = parent.BundleGroups[(int)array[j]];
					if (!groupInfo2.Done)
					{
						flag = false;
						break;
					}
					if (groupInfo2.requiredDownload)
					{
						flag2 = true;
					}
				}
				if (flag)
				{
					groupInfo.sentEvent = true;
					AppShell.Instance.EventMgr.Fire(parent, new BundleGroupLoadedMessage(groupInfo.group, flag2));
				}
			}
		}
	}

	public class GroupInfoAll : GroupInfo
	{
		public override float PercentDone
		{
			get
			{
				float num = 0f;
				for (int i = 0; i < 9; i++)
				{
					num += AppShell.Instance.BundleLoader.bundleGroups[i].PercentDone;
				}
				return num / 9f;
			}
		}

		public override bool Done
		{
			get
			{
				for (int i = 0; i < 9; i++)
				{
					if (!AppShell.Instance.BundleLoader.bundleGroups[i].Done)
					{
						return false;
					}
				}
				return true;
			}
		}

		public GroupInfoAll(AssetBundleLoader parent)
			: base(parent, BundleGroup.All, 0, 9999)
		{
		}
	}

	public class GroupInfoAny : GroupInfo
	{
		public override float PercentDone
		{
			get
			{
				return 1f;
			}
		}

		public override bool Done
		{
			get
			{
				return true;
			}
		}

		public GroupInfoAny(AssetBundleLoader parent)
			: base(parent, BundleGroup.Any, 0, 0)
		{
		}
	}

	public delegate void AssetBundleLoaderCallback(AssetBundleLoadResponse response, object extraData);

	public delegate void AssetLoadedCallback(UnityEngine.Object obj, AssetBundle bundle, object extraData);

	public delegate void BackgroundDownloadCallback();

	public const bool ALWAYS_LOAD_FROM_WEB = false;

	public const string ASSET_CDN_URI_PREFIX = "content$";

	public const string ASSET_LOCAL_URI_PREFIX = "localfile$";

	public const string ASSET_FOLDER_NAME = "AssetBundles/";

	public const string ASSET_EXTENSION = ".unity3d";

	public const float ASSET_BUNDLE_DOWNLOAD_TIMEOUT = 600f;

	protected const string TEMP_BUNDLE_SESSION_ID = "345lkhjkjahg90u3456hjhdg9j456";

	protected const string CONTENT_AUTHORIZATION_ASSET_PATH = "cp/content/";

	protected const string CONTENT_AUTHORIZATION_NONCE_URI = "content$cp/shsAuthNonce";

	protected const string CONTENT_AUTHORIZATION_URI = "content$cp/shsAuthResponse/";

	protected const string CONTENT_AUTHORIZATION_SEPARATOR = "?";

	protected const string CONTENT_AUTHORIZATION_SESSION_STRING = "?jsessionid=";

	protected const string CONTENT_AUTHORIZATION_KEY = "abc123def";

	protected const float CONTENT_AUTHORIZATION_TIMEOUT = 30f;

	private const string hashFunction = "SHA-256";

	protected int[] DownloadLimit = new int[3]
	{
		2,
		1,
		1
	};

	protected string cdnUriPrefix = string.Empty;

	protected string localUriPrefix = string.Empty;

	protected List<CachedAssetBundle>[] pendingLoadCategories;

	protected LoadCategory loadingCategory;

	protected Dictionary<string, CachedAssetBundle> loadingBundles;

	protected List<string> loadedBundles;

	protected Queue<CachedAssetBundle> responsesReady;

	protected Queue<LoadAssetRequest> assetLoadRequests;

	protected Queue<AssetLoadResponse> assetsReady;

	protected Dictionary<string, CachedAssetBundle> cachedBundles;

	protected int downloadingPaused;

	protected List<CachedAssetBundle> bundlesToUnloadOnSceneTransition;

	protected bool bundleAuthorizationIsRequired;

	protected string bundleSessionId;

	protected bool haveManifest;

	protected bool haveServersConfig;

	protected bool isConfigured;

	protected Queue<CachedAssetBundle> preconfigRequests;

	protected bool downloadCompleteReported;

	protected bool cacheRequestsComplete;

	private byte[] secretKey = new byte[32]
	{
		224,
		192,
		43,
		209,
		214,
		3,
		250,
		46,
		84,
		160,
		90,
		22,
		109,
		123,
		235,
		78,
		203,
		19,
		144,
		130,
		185,
		173,
		163,
		248,
		98,
		1,
		24,
		23,
		50,
		201,
		244,
		86
	};

	protected BundleInfo[] sortedBundles;

	protected GroupInfo[] bundleGroups;

	protected BundleGroup[][] bundleDependencies;

	protected bool forcedReloadDone;

	protected bool isBackgroundDownloaderDone;

	protected bool didDownloadFile;

	protected bool didUpdateDownloadCounter;

	protected bool killUpdate = false;  // added by CSP for debugging only

	protected Dictionary<string, BundleInfo> infoFromBundle;

	public Dictionary<string, CachedAssetBundle> CachedBundles
	{
		get
		{
			return cachedBundles;
		}
	}

	public bool DownloadingPaused
	{
		get
		{
			return downloadingPaused > 0;
		}
		set
		{
			if (value)
			{
				downloadingPaused++;
				return;
			}
			downloadingPaused--;
			if (downloadingPaused < 0)
			{
				downloadingPaused = 0;
			}
		}
	}

	public BundleInfo[] Bundles
	{
		get
		{
			return sortedBundles;
		}
	}

	public GroupInfo[] BundleGroups
	{
		get
		{
			return bundleGroups;
		}
	}

	public BundleGroup[][] BundleDependencies
	{
		get
		{
			return bundleDependencies;
		}
	}

	private void Awake()
	{
		responsesReady = new Queue<CachedAssetBundle>();
		assetLoadRequests = new Queue<LoadAssetRequest>();
		assetsReady = new Queue<AssetLoadResponse>();
		cachedBundles = new Dictionary<string, CachedAssetBundle>();
		bundlesToUnloadOnSceneTransition = new List<CachedAssetBundle>();
		bundleAuthorizationIsRequired = false;
		bundleSessionId = "345lkhjkjahg90u3456hjhdg9j456";
		haveManifest = false;
		haveServersConfig = false;
		isConfigured = false;
		preconfigRequests = new Queue<CachedAssetBundle>();
		localUriPrefix = "localfile$AssetBundles/";
		cdnUriPrefix = "content$AssetBundles/";
		pendingLoadCategories = new List<CachedAssetBundle>[DownloadLimit.Length];
		for (int i = 0; i < DownloadLimit.Length; i++)
		{
			pendingLoadCategories[i] = new List<CachedAssetBundle>();
		}
		loadingBundles = new Dictionary<string, CachedAssetBundle>();
		loadedBundles = new List<string>();
	}

	private void OnEnable()
	{
		AppShell component = Utils.GetComponent<AppShell>(base.gameObject);
		component.OnNewControllerReady += OnNewControllerReady;
		Singleton<ShsCacheManager>.instance.ManifestLoadedEvent += OnManifestLoaded;
	}

	private void OnDisable()
	{
		AppShell.Instance.OnNewControllerReady -= OnNewControllerReady;
		Singleton<ShsCacheManager>.instance.ManifestLoadedEvent -= OnManifestLoaded;
	}

	private void OnApplicationQuit()
	{
		if (!Application.isPlaying)
		{
		}
	}

	public void OnServerConfigLoaded(DataWarehouse serverConfig)
	{
		if (serverConfig.TryGetString("build_timestamp", null) != null)
		{
			bundleAuthorizationIsRequired = false;
		}
		else
		{
			bundleAuthorizationIsRequired = serverConfig.TryGetBool("//content_auth", true);
		}
		if (bundleAuthorizationIsRequired)
		{
			AppShell.Instance.WebService.StartRequest("content$cp/shsAuthNonce", OnContentAuthorizationNonceRecieved, null, null, 30f, 0.5f, ShsWebService.ShsWebServiceType.Text);
		}
		else
		{
			haveServersConfig = true;
			CheckIfConfigured();
		}
		ShsCacheManager.InitializeLocaleManifestLoad(serverConfig.TryGetBool("load_locale_bundle_manifest", false));
	}

	protected void CheckIfConfigured()
	{
		if (haveServersConfig && haveManifest)
		{
			isConfigured = true;
			ProcessAllPreconfigRequests();
		}
	}

	protected void OnManifestLoaded(bool succeeded)
	{
		if (succeeded)
		{
			haveManifest = true;
			CheckIfConfigured();
		}
		else
		{
			CspUtils.DebugLog("Failed to load manifest, unable to configure bundle loader! No bundles will load.");
		}
	}

	protected void OnContentAuthorizationNonceRecieved(ShsWebResponse response)
	{
		bool flag = true;
		try
		{
			if (response.Status == 200)
			{
				string body = response.Body;
				string str = ClientResponse(body);
				AppShell.Instance.WebService.StartRequest("content$cp/shsAuthResponse/" + str, OnContentAuthorizationSessionIdRecieved, null, null, 30f, 0.5f, ShsWebService.ShsWebServiceType.Text);
			}
			else
			{
				flag = false;
			}
		}
		catch (Exception ex)
		{
			CspUtils.DebugLog("Exception occurred while handling content nonce: " + ex.Message);
			flag = false;
		}
		if (!flag)
		{
			CspUtils.DebugLog("Request for content nonce failed with status <" + response.Status + "> and message <" + response.Body + ">.");
			if (Application.isWebPlayer)
			{
				Application.ExternalCall("HEROUPNS.RedirectToLogin");
			}
			else
			{
				Application.Quit();
			}
		}
	}

	private string ClientResponse(string serverMessage)
	{
		HashAlgorithm hashAlgorithm = HashAlgorithm.Create("SHA-256");
		byte[] sourceArray = ByteArrayFromString(serverMessage);
		byte[] array = new byte[8];
		Array.Copy(sourceArray, array, array.Length);
		byte[] buffer = ArrayConcat(secretKey, array);
		byte[] a = hashAlgorithm.ComputeHash(buffer);
		byte[] input = ArrayConcat(array, a);
		return ByteArrayToString(input);
	}

	public static string ByteArrayToString(byte[] input)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (byte b in input)
		{
			stringBuilder.Append(b.ToString("X2"));
		}
		return stringBuilder.ToString();
	}

	public static byte[] ByteArrayFromString(string input)
	{
		List<byte> list = new List<byte>();
		for (int i = 0; i < input.Length; i += 2)
		{
			string s = input.Substring(i, 2);
			byte item = byte.Parse(s, NumberStyles.HexNumber);
			list.Add(item);
		}
		return list.ToArray();
	}

	private byte[] ArrayConcat(byte[] a1, byte[] a2)
	{
		byte[] array = new byte[a1.Length + a2.Length];
		Array.Copy(a1, array, a1.Length);
		Array.Copy(a2, 0, array, a1.Length, a2.Length);
		return array;
	}

	protected void OnContentAuthorizationSessionIdRecieved(ShsWebResponse response)
	{
		bool flag = true;
		try
		{
			if (response.Status == 200)
			{
				bundleSessionId = response.Body;
				cdnUriPrefix = "content$cp/content/AssetBundles/";
				haveServersConfig = true;
				CheckIfConfigured();
			}
			else
			{
				flag = false;
			}
		}
		catch (Exception ex)
		{
			CspUtils.DebugLog("Exception occurred while handling content session: " + ex.Message);
			flag = false;
		}
		if (!flag)
		{
			CspUtils.DebugLog("Request for content session failed with status <" + response.Status + "> and message <" + response.Body + ">.");
			if (Application.isWebPlayer)
			{
				Application.ExternalCall("HEROUPNS.RedirectToLogin");
			}
			else
			{
				Application.Quit();
			}
		}
	}

	protected void ProcessAllPreconfigRequests()
	{
		foreach (CachedAssetBundle preconfigRequest in preconfigRequests)
		{
			processRequest(preconfigRequest);
		}
		preconfigRequests.Clear();
	}

	private void Update()
	{
		if (killUpdate)		// added by CSP for debugging only
			return;			// added by CSP for debugging only

		if (responsesReady.Count > 0)
		{
			CachedAssetBundle cachedAssetBundle = responsesReady.Dequeue();
			while (cachedAssetBundle.OutstandingCallbacks.Count > 0)
			{
				AssetBundleLoadResponse assetBundleLoadResponse = new AssetBundleLoadResponse();
				assetBundleLoadResponse.Path = cachedAssetBundle.RequestPath;
				if (cachedAssetBundle.TimeLoaded >= 0f && cachedAssetBundle.Bundle != null)
				{
					assetBundleLoadResponse.Bundle = cachedAssetBundle.Bundle;
				}
				else
				{
					assetBundleLoadResponse.Error = "The requested asset bundle at <" + assetBundleLoadResponse.Path + "> could not be loaded.";
					CspUtils.DebugLog(assetBundleLoadResponse.Error);
					if (cachedAssetBundle.TimeLoaded >= 0f)
					{
						assetBundleLoadResponse.Error = "Unexpected error - the asset bundle <" + assetBundleLoadResponse.Path + "> was loaded at time <" + cachedAssetBundle.TimeLoaded + "> but is now somehow unavailable.";
					}
					cachedBundles.Remove(cachedAssetBundle.RequestPath);
				}
				AssetBundleCallbackWithData assetBundleCallbackWithData = cachedAssetBundle.OutstandingCallbacks.Dequeue();
				try
				{
					if (assetBundleCallbackWithData == null)
					{
						//CspUtils.DebugLog("AssetBundleLoader.Update() finished bundle " + cachedAssetBundle.RequestPath + " with no callback.");
					}
					else if (assetBundleCallbackWithData.Callback == null)
					{
						//CspUtils.DebugLog("AssetBundleLoader.Update() finished bundle " + cachedAssetBundle.RequestPath + " but had a NULL callback pointer.");
					}
					assetBundleCallbackWithData.Callback(assetBundleLoadResponse, assetBundleCallbackWithData.ExtraData);
				}
				catch (Exception ex)
				{
					CspUtils.DebugLog("Asset Bundle <" + cachedAssetBundle.RequestPath + "> Load callback failed with error <" + ex + ">.");
				}
			}
			//CspUtils.DebugLog("loadCategory=" + cachedAssetBundle.loadCategory + "  RequestPath" + cachedAssetBundle.RequestPath);  // CSP
			if (cachedAssetBundle.loadCategory > LoadCategory.Immediate)  
			{
				UnloadAssetBundle(cachedAssetBundle.RequestPath, false);
			}
		}
		if (assetLoadRequests.Count > 0)
		{
			//CspUtils.DebugLog("*assetLoadRequests Queue* size = " + assetLoadRequests.Count);
			//foreach (var e in assetLoadRequests)   // for loop added by CSP for debugging
			//{
			//	CspUtils.DebugLog("e: " + e.AssetName);
			//}

			LoadAssetRequest loadAssetRequest = assetLoadRequests.Dequeue();
			//CspUtils.DebugLog("REQ: AssetName=" + loadAssetRequest.AssetName + " in bundle " + loadAssetRequest.CachedBundle.RequestPath);
			
			if (loadAssetRequest.CachedBundle.Bundle != null)
			{				
				loadAssetRequest.CachedBundle.sceneRequested = AppShell.Instance.SceneCounter;
				StartCoroutine(StartAsyncAssetLoad(loadAssetRequest.CachedBundle, loadAssetRequest.AssetName, loadAssetRequest.ExtraData, loadAssetRequest.Callback, loadAssetRequest.IsPreload));
			}
			else
			{
				assetLoadRequests.Enqueue(loadAssetRequest);
			}
		}
		if (assetsReady.Count > 0)
		{
			AssetLoadResponse assetLoadResponse = assetsReady.Dequeue();
			if (assetLoadResponse.Callback != null)
			{
				assetLoadResponse.Callback(assetLoadResponse.Asset, assetLoadResponse.Bundle, assetLoadResponse.ExtraData);
			}
		}
		if (!isConfigured || pendingLoadCategories == null || DownloadingPaused)
		{
			return;
		}
		int num = 0;
		while (true)
		{
			if (num < pendingLoadCategories.Length)
			{
				if (pendingLoadCategories[num].Count > 0)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		if (loadingBundles.Count > 0)
		{
			if ((int)loadingCategory > num)
			{
				List<string> list = new List<string>();
				foreach (KeyValuePair<string, CachedAssetBundle> loadingBundle in loadingBundles)
				{
					CachedAssetBundle value = loadingBundle.Value;
					if ((int)value.loadCategory > num)
					{
						CspUtils.DebugLog("Replacing request <" + value.RequestPath + "> with higher priority request.");
						AppShell.Instance.WebService.CancelRequest(WebPathFromRequestPath(value.RequestPath));
						list.Add(loadingBundle.Key);
					}
					pendingLoadCategories[(int)value.loadCategory].Insert(0, value);
				}
				foreach (string item in list)
				{
					loadingBundles.Remove(item);
				}
			}
			else if (loadingBundles.Count >= DownloadLimit[num])
			{
				return;
			}
		}
		CachedAssetBundle cachedAssetBundle2 = pendingLoadCategories[num][0];
		pendingLoadCategories[num].RemoveAt(0);
		loadingCategory = (LoadCategory)num;
		if (!loadingBundles.ContainsKey(cachedAssetBundle2.RequestPath))
		{
			loadingBundles[cachedAssetBundle2.RequestPath] = cachedAssetBundle2;
			SubmitRequestToWebServices(cachedAssetBundle2);
		}
	}

	public bool IsAssetBundleCachedOnDisk(string bundlePath)
	{
		if (false || Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
		{
			return ShsCacheManager.IsCachedVersionCurrent("AssetBundles/" + bundlePath + ".unity3d");
		}
		return true;
	}

	public bool ListedBundlesCached(List<string> bundlesToCheck)
	{
		if (bundlesToCheck == null)
		{
			CspUtils.DebugLog("No list of bundles given to check if cached!");
			return false;
		}
		foreach (string item in bundlesToCheck)
		{
			string bundlePath = item.Replace(".unity3d", string.Empty);
			if (!IsAssetBundleCachedOnDisk(bundlePath))
			{
				return false;
			}
		}
		return true;
	}

	protected string WebPathFromRequestPath(string bundlePath)
	{
		string text = bundlePath;
		if (false || Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
		{
			text = cdnUriPrefix + bundlePath + ".unity3d";
			if (bundleAuthorizationIsRequired)
			{
				text = text + "?jsessionid=" + bundleSessionId;
			}
		}
		else
		{
			text = localUriPrefix + bundlePath + ".unity3d";
		}
		return text;
	}

	protected string RequestPathFromWebPath(string webPath)
	{
		if (string.IsNullOrEmpty(webPath))
		{
			return webPath;
		}
		try
		{
			if (false || Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
			{
				string text = webPath;
				if (bundleAuthorizationIsRequired)
				{
					text = text.Split("?"[0])[0];
				}
				return text.Substring(cdnUriPrefix.Length, text.Length - cdnUriPrefix.Length - ".unity3d".Length);
			}
			return webPath.Substring(localUriPrefix.Length, webPath.Length - localUriPrefix.Length - ".unity3d".Length);
		}
		catch (ArgumentOutOfRangeException ex)
		{
			CspUtils.DebugLog("Failed while converting web path to bundle path for <" + webPath + "> with exception:" + ex.Message);
			return string.Empty;
		}
	}

	public bool CacheAssetBundle(string bundleName)
	{
		bundleName = bundleName.Replace(".unity3d", string.Empty);
		if (!IsAssetBundleCachedOnDisk(bundleName))
		{
			FetchAssetBundle(bundleName, null, null, false, LoadCategory.Cache);
			return true;
		}
		return false;
	}

	public void FetchAssetBundle(string bundlePath, AssetBundleLoaderCallback callback)
	{
		FetchAssetBundle(bundlePath, callback, null);
	}

	public void FetchAssetBundle(string bundlePath, AssetBundleLoaderCallback callback, object extraData)
	{
		FetchAssetBundle(bundlePath, callback, extraData, true);
	}

	public void FetchAssetBundle(string bundlePath, AssetBundleLoaderCallback callback, object extraData, bool unloadOnSceneTransition)
	{
		FetchAssetBundle(bundlePath, callback, extraData, unloadOnSceneTransition, LoadCategory.Immediate);
	}

	public void FetchAssetBundle(string bundlePath, AssetBundleLoaderCallback callback, object extraData, bool unloadOnSceneTransition, LoadCategory category)
	{
		CachedAssetBundle value = null;
		//CspUtils.DebugLog("cachedBundles.TryGetValue(bp)  bundlePath= " + bundlePath);  // CSP
		if (cachedBundles.TryGetValue(bundlePath, out value))
		{
			value.sceneRequested = AppShell.Instance.SceneCounter;
			if (callback != null)
			{
				value.OutstandingCallbacks.Enqueue(new AssetBundleCallbackWithData(callback, extraData));
			}
			if (value.Bundle != null)
			{
				responsesReady.Enqueue(value);
			}
			else if (value.loadCategory > category)
			{
				if (!value.UnloadOnSceneTransition)
				{
					value.UnloadOnSceneTransition = unloadOnSceneTransition;
				}
				if (pendingLoadCategories[(int)value.loadCategory] != null && pendingLoadCategories[(int)value.loadCategory].Contains(value))
				{
					pendingLoadCategories[(int)value.loadCategory].Remove(value);
					pendingLoadCategories[(int)category].Add(value);
				}
				value.loadCategory = category;
			}
			if (!unloadOnSceneTransition && value.UnloadOnSceneTransition)
			{
				value.UnloadOnSceneTransition = false;
			}
		}
		else
		{
			RequestNewBundle(bundlePath, callback, extraData, unloadOnSceneTransition, category);
		}
	}

	protected void RequestNewBundle(string bundlePath, AssetBundleLoaderCallback callback, object extraData, bool unloadOnSceneTransition, LoadCategory category)
	{
		CachedAssetBundle cachedAssetBundle = new CachedAssetBundle();
		cachedAssetBundle.RequestPath = bundlePath;
		cachedAssetBundle.UnloadOnSceneTransition = unloadOnSceneTransition;
		cachedAssetBundle.sceneRequested = AppShell.Instance.SceneCounter;
		cachedAssetBundle.loadCategory = category;
		cachedAssetBundle.OutstandingCallbacks.Enqueue(new AssetBundleCallbackWithData(OnExternalDownloadCallback, null));
		if (callback != null)
		{
			cachedAssetBundle.OutstandingCallbacks.Enqueue(new AssetBundleCallbackWithData(callback, extraData));
		}
		//CspUtils.DebugLog("cachedAssetBundle.RequestPath= " + cachedAssetBundle.RequestPath);  // CSP
		cachedBundles[cachedAssetBundle.RequestPath] = cachedAssetBundle;
		if (unloadOnSceneTransition)
		{
			bundlesToUnloadOnSceneTransition.Add(cachedAssetBundle);
		}
		if (!isConfigured)
		{
			preconfigRequests.Enqueue(cachedAssetBundle);
		}
		else
		{
			processRequest(cachedAssetBundle);
		}
	}

	public float CheckAssetBundleRemaining(string bundlePath)
	{
		return AppShell.Instance.WebService.CheckRequestProgress(WebPathFromRequestPath(bundlePath));
	}

	public float CheckAssetBundleSize(string bundlePath)
	{
		return AppShell.Instance.WebService.CheckRequestSize(WebPathFromRequestPath(bundlePath));
	}

	protected void processRequest(CachedAssetBundle bundle)
	{
		pendingLoadCategories[(int)bundle.loadCategory].Add(bundle);
	}

	protected void SubmitRequestToWebServices(CachedAssetBundle cachedEntry)
	{			
		AppShell.Instance.WebService.StartRequest(WebPathFromRequestPath(cachedEntry.RequestPath), OnAssetBundleLoaded, null, null, 600f, 0.5f, ShsWebService.ShsWebServiceType.AssetBundle, true, cachedEntry.loadCategory == LoadCategory.Cache);
	}


	/////////////// CSP this method added for debugging only //////////////////////////
	IEnumerator testLoop(string bundleName) {
				
		foreach (KeyValuePair<string, CachedAssetBundle>  cb in cachedBundles) {
			//CspUtils.DebugLog("testloop: Unloading bundle " + cb.Key);
			//cb.Value.Unload(true);


			WWW www = null;

			//CspUtils.DebugLog(" testloop: downloading " + bundleName);

			www = WWW.LoadFromCacheOrDownload ("http://192.168.235.128/cab-stubs/" + bundleName, 5);
			yield return www;
			if (www.error != null)
			{
				CspUtils.DebugLog("testloop: " + www.error);
				//yield break;
			}
			else
			{
				AssetBundle myBundle = www.assetBundle;
				if (myBundle != null) {
					//CspUtils.DebugLog("testloop: BUNDLE " + bundleName + " LOADED!!!!!!!");  //AFTER " + cb.Key + " UNLOADED!!!!!!!");
					break;
				}
			}

			//CspUtils.DebugLog("testloop: Unloading bundle " + cb.Key);
			cb.Value.Unload(true);
			
		}

		//CspUtils.DebugLog("FINISHED DEBUGGING!!!!!!!!!!!!!");
	}
	////////////////////////////////////////////////////////////////////////////////////////////

	public void OnAssetBundleLoaded(ShsWebResponse response)
	{
		CachedAssetBundle value = null;
		string text = RequestPathFromWebPath(response.OriginalUri);
		if (loadingBundles.ContainsKey(text))
		{
			loadingBundles.Remove(text);
			loadedBundles.Add(text);
			if (!downloadCompleteReported && cacheRequestsComplete)
			{
				checkDownloadComplete();
			}
		}
		HttpStatusCode status = (HttpStatusCode)response.Status;
		AssetBundle assetBundle = response.Object as AssetBundle;
		try {
			assetBundle.name = text;		// CSP added for debugging purposes.
		}
		catch (Exception e) {
		}	
		//CspUtils.DebugLog("cachedBundles.TryGetValue(bp)  text= " + text);  // CSP
		if (!cachedBundles.TryGetValue(text, out value))
		{
			assetBundle.Unload(true);
			return;
		}		
		try {
			//CspUtils.DebugLog("value.TimeLoaded=" + value.TimeLoaded);		// CSP added for debugging purposes.
			//CspUtils.DebugLog("status=" + status);		// CSP added for debugging purposes.
			//if (assetBundle == null)
				//CspUtils.DebugLog("assetBundle is null");		// CSP added for debugging purposes.
			//else
				//CspUtils.DebugLog("assetBundle=" + assetBundle);   // CSP added for debugging purposes.
			//if (value.Bundle == null)
				//CspUtils.DebugLog("value.Bundle is null");		// CSP added for debugging purposes.
			//else
				//CspUtils.DebugLog("value.Bundle=" + value.Bundle);   // CSP added for debugging purposes.
		}
		catch (Exception e) {
		}	
		///////////////////// CSP temp debugging code to find out which bundle is duplicate problem  ///////////////////////////////////////
		// if (text == "SocialSpace/gameworld_common_objects") {
		// 	killUpdate = true;  // don't allow Update() method to do stuff while debugging...
		// 	StartCoroutine(testLoop("gameworld_common_objects.unity3d"));
		// }
		/////////////////////////////////////////////////////////////
		if (status == HttpStatusCode.OK && assetBundle != null)
		{
			value.Bundle = assetBundle;
			value.SizeInBytes = response.Size;
			value.TimeLoaded = Time.time;
		}
		else if (value.Bundle != null)    // && value.TimeLoaded >= 0f)    partly commented out by CSP
		{
			//value.TimeLoaded = Time.time;  // added by CSP for testing
			//CspUtils.DebugLog("Bundle load for <" + value.RequestPath + "> failed, but we already have that bundle in memory.");
		}
		else
		{
			CspUtils.DebugLog("The returned asset bundle was not an AssetBundle.  The request failed with status code <" + response.Status + ">. The request was for bundle <" + response.OriginalUri + ">.");
			value.TimeLoaded = -1f;
		}
		responsesReady.Enqueue(value);
	}

	public void PreLoadAsset(string bundlePath, string assetName)
	{
		LoadAsset(bundlePath, assetName, null, null, true);
	}

	public void LoadAsset(string bundlePath, string assetName, object extraData, AssetLoadedCallback callback)
	{
		LoadAsset(bundlePath, assetName, extraData, callback, false);
	}

	public void LoadAsset(string bundlePath, string assetName, object extraData, AssetLoadedCallback callback, bool isPreload)
	{
		if (assetName == "416d625f5377656574656e65725f4261787465725f526f6f6656656e7432") {
			CspUtils.DebugLog("CRAPPY ASSET FOUND!");
		}

		LoadAssetRequest loadAssetRequest = new LoadAssetRequest();
		loadAssetRequest.AssetName = assetName;
		loadAssetRequest.ExtraData = extraData;
		loadAssetRequest.Callback = callback;
		loadAssetRequest.IsPreload = isPreload;
		//CspUtils.DebugLog("cachedBundles.TryGetValue(bp)  bundlePath= " + bundlePath);  // CSP
		cachedBundles.TryGetValue(bundlePath, out loadAssetRequest.CachedBundle);
		if (loadAssetRequest.CachedBundle == null || loadAssetRequest.CachedBundle.loadCategory == LoadCategory.Cache)
		{
			//CspUtils.DebugLog("About to call FetchAssetBundle()");
			FetchAssetBundle(bundlePath, OnAssetBundleLoadedForAssetLoad, loadAssetRequest);
		}
		else
		{	
			//CspUtils.DebugLog("About to call assetLoadRequests.Enqueue()");
			assetLoadRequests.Enqueue(loadAssetRequest);
		}
	}

	protected void OnAssetBundleLoadedForAssetLoad(AssetBundleLoadResponse response, object extraData)
	{
		LoadAssetRequest loadAssetRequest = extraData as LoadAssetRequest;
		if (loadAssetRequest == null)
		{
			CspUtils.DebugLog("Expected asset load request not found when processing the asset bundle load for <" + response.Path + ">.");
		}
		else if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("Asset Bundle <" + response.Path + "> failed to load while trying to load asset <" + loadAssetRequest.AssetName + ">\nWith error <" + response.Error + ">.");
		}
		else if (!cachedBundles.TryGetValue(response.Path, out loadAssetRequest.CachedBundle))
		{
			CspUtils.DebugLog("Unable to find cached Asset Bundle <" + response.Path + "> after successfully loading it while attempting to load asset <" + loadAssetRequest.AssetName + ">.");
		}
		else
		{
			assetLoadRequests.Enqueue(loadAssetRequest);
		}
	}

	protected IEnumerator StartAsyncAssetLoad(CachedAssetBundle cachedBundle, string assetName, object extraData, AssetLoadedCallback callback, bool isPreload)
	{
		AssetLoadResponse response = new AssetLoadResponse();
		response.Callback = callback;
		response.ExtraData = extraData;
		response.Bundle = cachedBundle.Bundle;
		UnityEngine.Object asset2 = null;
		if (cachedBundle.PreloadedAssets.TryGetValue(assetName, out asset2))
		{
			response.Asset = asset2;
			//CspUtils.DebugLog("StartAsyncAssetLoad 1 asset found: " + assetName + " in bundle " + cachedBundle.RequestPath);
		}
		else if (cachedBundle.Bundle == null)
		{
			//CspUtils.DebugLog("cachedBundle.Bundle was null in " + cachedBundle.RequestPath);
		}
		else
		{			
			AssetBundleRequest request = cachedBundle.Bundle.LoadAsync(assetName, typeof(UnityEngine.Object));
			yield return request;
			asset2 = request.asset;

			if (asset2 == null) { // CSP - this if-block needed as a workaround to Load() problem.
				UnityEngine.Object[] objs = cachedBundle.Bundle.LoadAll();  
				request = cachedBundle.Bundle.LoadAsync(assetName, typeof(UnityEngine.Object));
				yield return request;

				asset2 = CspUtils.CspLoadObj(cachedBundle,  assetName,  request, objs);
			}
			else {
				//CspUtils.DebugLog("ASSET FOUND USING Load()! assetName=" + assetName);
			}


			if (cachedBundle.Bundle == null)
			{
				response.Asset = null;
				//CspUtils.DebugLog("StartAsyncAssetLoad 2 asset NOT found: " + assetName);
			}
			else
			{
				cachedBundle.PreloadedAssets[assetName] = asset2;  // Gaz used this to cache the asset.
				response.Asset = asset2;
				//CspUtils.DebugLog("StartAsyncAssetLoad 3 asset found: " + assetName);
				//CspUtils.DebugLog("asset2.GetType()=" + asset2.GetType().ToString());
			}
		}
		if (!isPreload)
		{
			assetsReady.Enqueue(response);
		}
	}

	public void OnNewControllerReady(AppShell.GameControllerTypeData currentGameTypeData, GameController controller)
	{
		for (int num = bundlesToUnloadOnSceneTransition.Count - 1; num >= 0; num--)
		{
			CachedAssetBundle cachedAssetBundle = bundlesToUnloadOnSceneTransition[num];
			if (cachedAssetBundle.sceneRequested < AppShell.Instance.SceneCounter)
			{
				bundlesToUnloadOnSceneTransition.RemoveAt(num);
				if (cachedAssetBundle.UnloadOnSceneTransition)
				{
					UnloadAssetBundle(cachedAssetBundle.RequestPath, true);
				}
			}
		}
	}

	public void UnloadAssetBundle(string bundlePath, bool unloadAllLoadedObjects)
	{
		CachedAssetBundle value = null;
		//CspUtils.DebugLog("cachedBundles.TryGetValue(ubp)  bundlePath= " + bundlePath);  // CSP
		if (cachedBundles.TryGetValue(bundlePath, out value))
		{
			if (value == null)
			{
				CspUtils.DebugLog(bundlePath + " has a NULL cached bundled");
				return;
			}
			AssetBundle bundle = value.Bundle;
			if (bundle == null)
			{
				CspUtils.DebugLog("Cancelling Request for " + value.RequestPath + " due to unloading");
				AppShell.Instance.WebService.CancelRequest(WebPathFromRequestPath(value.RequestPath));
			}
			else
			{
				foreach (CachedAssetBundle item in responsesReady)
				{
					if (item.Bundle == bundle)
					{
						item.Bundle = null;
					}
				}
				foreach (AssetLoadResponse item2 in assetsReady)
				{
					if (item2.Bundle == bundle)
					{
						item2.Asset = null;
					}
				}
			}
			value.Unload(unloadAllLoadedObjects);
			cachedBundles.Remove(value.RequestPath);
		}
		else
		{
			CspUtils.DebugLog("Asked to unload asset bundle <" + bundlePath + "> that we do not have cached.");
		}
	}

	public void GetDownloadProgress(out int filesCompleted, out int filesTotal, out bool cacheRequestsComplete)
	{
		cacheRequestsComplete = this.cacheRequestsComplete;
		if (loadedBundles == null || loadingBundles == null || pendingLoadCategories == null)
		{
			filesCompleted = 0;
			filesTotal = 0;
			return;
		}
		filesCompleted = loadedBundles.Count;
		filesTotal = filesCompleted + loadingBundles.Count;
		List<CachedAssetBundle>[] array = pendingLoadCategories;
		foreach (List<CachedAssetBundle> list in array)
		{
			filesTotal += list.Count;
		}
	}

	protected void checkDownloadComplete()
	{
		if (loadingBundles == null || pendingLoadCategories == null || loadingBundles.Count > 0)
		{
			return;
		}
		bool flag = true;
		List<CachedAssetBundle>[] array = pendingLoadCategories;
		foreach (List<CachedAssetBundle> list in array)
		{
			if (list.Count > 0)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			downloadCompleteReported = true;
			AppShell.Instance.EventMgr.Fire(base.gameObject, new GlobalContentLoadedMessage());
		}
	}

	public void CacheRequestsComplete()
	{
		cacheRequestsComplete = true;
		checkDownloadComplete();
	}

	public void BackgroundDownload()
	{
		InitializeBackgroundDownloader();
		StartCoroutine(BackgroundDownloader());
	}

	public float GetBundleGroupPercentageDone(BundleGroup group)
	{
		return bundleGroups[(int)group].PercentDone;
	}

	public bool GetBundleGroupDone(BundleGroup group)
	{
		return bundleGroups[(int)group].Done;
	}

	public float GetBundleGroupDependenciesPercentageDone(BundleGroup group)
	{
		BundleGroup[] array = bundleDependencies[(int)group];
		float num = 0f;
		for (int i = 0; i < array.Length; i++)
		{
			num += GetBundleGroupPercentageDone(array[i]);
		}
		return num / (float)array.Length;
	}

	public bool GetBundleGroupDependenciesDone(BundleGroup group)
	{
		BundleGroup[] array = bundleDependencies[(int)group];
		for (int i = 0; i < array.Length; i++)
		{
			if (!GetBundleGroupDone(array[i]))
			{
				return false;
			}
		}
		return true;
	}

	public void AggressiveBackgroundDownloading(bool doAggresiveDownloading)
	{
		if (doAggresiveDownloading)
		{
			DownloadLimit[2] = 2;
		}
		else
		{
			DownloadLimit[2] = 1;
		}
	}

	protected void InitializeBackgroundDownloader()
	{
		AppShell.Instance.EventMgr.AddListener<UserCounterDataLoadedMessage>(OnCountersLoaded);
		isBackgroundDownloaderDone = false;
		didDownloadFile = false;
		didUpdateDownloadCounter = false;
		bundleGroups = new GroupInfo[11];
		bundleGroups[0] = new GroupInfo(this, BundleGroup.TutorialUIBugle, 0, 20);
		bundleGroups[1] = new GroupInfo(this, BundleGroup.PrizeWheel, 21, 24);
		bundleGroups[4] = new GroupInfo(this, BundleGroup.NonBugleGameWorlds, 25, 29);
		bundleGroups[2] = new GroupInfo(this, BundleGroup.Characters, 30, 38);
		bundleGroups[7] = new GroupInfo(this, BundleGroup.SpecialMission, 39, 39);
		bundleGroups[3] = new GroupInfo(this, BundleGroup.MissionsAndEnemies, 40, 49);
		bundleGroups[5] = new GroupInfo(this, BundleGroup.HQ, 50, 59);
		bundleGroups[6] = new GroupInfo(this, BundleGroup.CardGame, 90, 97);
		bundleGroups[8] = new GroupInfo(this, BundleGroup.VO, 98, 98);
		bundleGroups[9] = new GroupInfoAll(this);
		bundleGroups[10] = new GroupInfoAny(this);
		bundleDependencies = new BundleGroup[11][];
		bundleDependencies[0] = new BundleGroup[1];
		bundleDependencies[1] = new BundleGroup[1]
		{
			BundleGroup.PrizeWheel
		};
		bundleDependencies[4] = new BundleGroup[3]
		{
			BundleGroup.TutorialUIBugle,
			BundleGroup.PrizeWheel,
			BundleGroup.NonBugleGameWorlds
		};
		bundleDependencies[2] = new BundleGroup[2]
		{
			BundleGroup.TutorialUIBugle,
			BundleGroup.Characters
		};
		bundleDependencies[7] = new BundleGroup[3]
		{
			BundleGroup.TutorialUIBugle,
			BundleGroup.Characters,
			BundleGroup.SpecialMission
		};
		bundleDependencies[3] = new BundleGroup[1]
		{
			BundleGroup.Any
		};
		bundleDependencies[5] = new BundleGroup[4]
		{
			BundleGroup.TutorialUIBugle,
			BundleGroup.PrizeWheel,
			BundleGroup.Characters,
			BundleGroup.HQ
		};
		bundleDependencies[6] = new BundleGroup[6]
		{
			BundleGroup.TutorialUIBugle,
			BundleGroup.PrizeWheel,
			BundleGroup.Characters,
			BundleGroup.SpecialMission,
			BundleGroup.MissionsAndEnemies,
			BundleGroup.CardGame
		};
		bundleDependencies[8] = new BundleGroup[7]
		{
			BundleGroup.TutorialUIBugle,
			BundleGroup.PrizeWheel,
			BundleGroup.Characters,
			BundleGroup.SpecialMission,
			BundleGroup.MissionsAndEnemies,
			BundleGroup.CardGame,
			BundleGroup.VO
		};
		bundleDependencies[9] = new BundleGroup[1]
		{
			BundleGroup.All
		};
		bundleDependencies[10] = new BundleGroup[1]
		{
			BundleGroup.Any
		};
		infoFromBundle = new Dictionary<string, BundleInfo>();
		List<BundleInfo> list = new List<BundleInfo>();
		foreach (ShsCacheManager.ManifestBundleInfo value in ShsCacheManager.Manifest.Values)
		{
			if (value.Active && AppShell.Instance.IsCurrentLocale(value.Locale))
			{
				GroupInfo groupInfoFromPriority = GetGroupInfoFromPriority(value.Priority);
				if (groupInfoFromPriority != null)
				{
					BundleInfo bundleInfo = new BundleInfo(value.BundlePath, groupInfoFromPriority, value);
					groupInfoFromPriority.AddBundle(bundleInfo);
					infoFromBundle.Add(bundleInfo.name.ToLower(), bundleInfo);
					list.Add(bundleInfo);
				}
			}
		}
		sortedBundles = list.ToArray();
		Array.Sort(sortedBundles, delegate(BundleInfo a, BundleInfo b)
		{
			return a.manifest.Priority.CompareTo(b.manifest.Priority);
		});
		GroupInfo[] array = BundleGroups;
		foreach (GroupInfo groupInfo in array)
		{
			groupInfo.PostInitialize();
		}
	}

	protected GroupInfo GetGroupInfoFromPriority(uint priority)
	{
		GroupInfo[] array = BundleGroups;
		foreach (GroupInfo groupInfo in array)
		{
			if (groupInfo.minPriority <= priority && priority <= groupInfo.maxPriority)
			{
				return groupInfo;
			}
		}
		CspUtils.DebugLog("Priority <" + priority + "> is not mapped to a group");
		return null;
	}

	protected void OnExternalDownloadCallback(AssetBundleLoadResponse response, object extraData)
	{
		if (!(response.Bundle == null))
		{
			BundleInfo value = null;
			if (infoFromBundle.TryGetValue(response.Path.ToLower(), out value))
			{
				value.State = BundleState.HardCache;
			}
		}
	}

	protected void OnBackgroundDownloadCallback(AssetBundleLoadResponse response, object extraData)
	{
		BundleInfo bundleInfo = (BundleInfo)extraData;
		if (response.Bundle != null)
		{
			bundleInfo.State = BundleState.HardCache;
			return;
		}
		CspUtils.DebugLog("Error <" + response.Error + "> downloading bundle <" + bundleInfo.name + ">");
		bundleInfo.State = BundleState.Error;
	}

	protected void OnForcedDownloadCallback(AssetBundleLoadResponse response, object extraData)
	{
		forcedReloadDone = true;
		BundleInfo bundleInfo = (BundleInfo)extraData;
		if (response.Bundle != null)
		{
			bundleInfo.State = BundleState.HardCache;
			return;
		}
		CspUtils.DebugLog("Error <" + response.Error + "> downloading bundle <" + bundleInfo.name + ">");
		bundleInfo.State = BundleState.Error;
	}

	protected ProcessResult ProcessBundle(BundleInfo bundle)
	{
		switch (bundle.State)
		{
		case BundleState.ForegroundDownload:
		case BundleState.BackgroundDownload:
		case BundleState.HardCache:
			return ProcessResult.Skipped;
		case BundleState.Unknown:
		case BundleState.Error:
			if (IsAssetBundleCachedOnDisk(bundle.name))
			{
				bundle.State = BundleState.SoftCache;
				return ProcessResult.CheckedVersion;
			}
			bundle.State = BundleState.BackgroundDownload;
			FetchAssetBundle(bundle.name, OnBackgroundDownloadCallback, bundle, false, LoadCategory.Cache);
			return ProcessResult.StartedDownload;
		case BundleState.SoftCache:
			if (IsAssetBundleCachedOnDisk(bundle.name))
			{
				return ProcessResult.CheckedVersion;
			}
			bundle.State = BundleState.ForegroundDownload;
			FetchAssetBundle(bundle.name, OnBackgroundDownloadCallback, bundle, true, LoadCategory.Immediate);
			return ProcessResult.StartedDownload;
		default:
			return ProcessResult.Skipped;
		}
	}

	protected IEnumerator BackgroundDownloader()
	{
		bool attemptedDownload = false;
		int bundleIdx = 0;
		while (bundleIdx < sortedBundles.Length)
		{
			yield return 0;
			int filesThisFrame = 0;
			while (filesThisFrame < 4 && bundleIdx < sortedBundles.Length)
			{
				BundleInfo currentBundle = sortedBundles[bundleIdx++];
				switch (ProcessBundle(currentBundle))
				{
				case ProcessResult.CheckedVersion:
					filesThisFrame++;
					break;
				case ProcessResult.StartedDownload:
					attemptedDownload = true;
					didDownloadFile = true;
					filesThisFrame++;
					break;
				}
			}
		}
		if (!attemptedDownload)
		{
			BackgroundDownloaderFinished();
		}
		else
		{
			StartCoroutine(BackgroundVerifyPass());
		}
	}

	protected IEnumerator BackgroundVerifyPass()
	{
		//CspUtils.DebugLog("===> Starting verify pass");
		bool attemptedDownload;
		do
		{
			attemptedDownload = false;
			bool restartPass = false;
			for (int i = 0; i < sortedBundles.Length; i++)
			{
				yield return 0;
				if (restartPass)
				{
					break;
				}
				BundleInfo currentBundle = sortedBundles[i];
				switch (currentBundle.State)
				{
				case BundleState.Unknown:
					CspUtils.DebugLog("BackgroundVerifyPass hit an unprocessed file <" + currentBundle.name + ">");
					break;
				case BundleState.BackgroundDownload:
					attemptedDownload = true;
					break;
				case BundleState.SoftCache:
					if (!IsAssetBundleCachedOnDisk(currentBundle.name))
					{
						yield return 0;
						CspUtils.DebugLog("BackgroundVerifyPass detected deleted file <" + currentBundle.name + ">");
						restartPass = true;
						attemptedDownload = true;
						forcedReloadDone = false;
						currentBundle.State = BundleState.ForegroundDownload;
						FetchAssetBundle(currentBundle.name, OnForcedDownloadCallback, currentBundle, true, LoadCategory.Immediate);
						while (!forcedReloadDone)
						{
							yield return 0;
						}
					}
					break;
				}
			}
		}
		while (attemptedDownload);
		BackgroundDownloaderFinished();
	}

	private void BackgroundDownloaderFinished()
	{
		isBackgroundDownloaderDone = true;
		CacheRequestsComplete();
		UpdateDownloadCounter();
	}

	protected void OnCountersLoaded(UserCounterDataLoadedMessage msg)
	{
		AppShell.Instance.EventMgr.RemoveListener<UserCounterDataLoadedMessage>(OnCountersLoaded);
		UpdateDownloadCounter();
	}

	protected void UpdateDownloadCounter()
	{
		if (!isBackgroundDownloaderDone || !AppShell.Instance.CounterManager.CountersLoaded || didUpdateDownloadCounter)
		{
			return;
		}
		didUpdateDownloadCounter = true;
		ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter("DownloadCount");
		if (counter == null)
		{
			return;
		}
		if (counter.GetCurrentValue() == 0L)
		{
			if (didDownloadFile)
			{
				counter.SetCounter(1L, SHSCounterType.ReportingMethodEnum.WebService);
			}
			else
			{
				counter.SetCounter(-1L, SHSCounterType.ReportingMethodEnum.WebService);
			}
		}
		else if (didDownloadFile)
		{
			long currentValue = counter.GetCurrentValue();
			currentValue = ((currentValue <= 0) ? (currentValue - 1) : (currentValue + 1));
			counter.SetCounter(currentValue, SHSCounterType.ReportingMethodEnum.WebService);
		}
	}
}
