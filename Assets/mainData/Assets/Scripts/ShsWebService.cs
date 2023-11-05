using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using UnityEngine;
//using UnityEditor;

public class ShsWebService : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum ShsWebServiceType
	{
		Unspecified,
		RASP,
		Mono,
		Text,
		Texture,
		Binary,
		AssetBundle,
		AudioClip,
		Movie,
		LocalResource
	}

	public class StubServerDelegate
	{
		public ShsWebService parent;

		public string otherUrl;

		public void OnServerConfigLoaded(ShsWebResponse response)
		{
			if (response.Status == 200)
			{
				parent.OnServerConfigLoaded(response);
				return;
			}
			CspUtils.DebugLog("Second Try: " + otherUrl);
			parent.LoadServerConfig(otherUrl);
		}
	}

	public delegate void ShsWebServiceCallback(ShsWebResponse response);

	public const float DEFAULT_TIMEOUT = 30f;

	public const float DEFAULT_POLLING_PERIOD = 0.5f;

	public const string SHS_WEBSERVICE_URI_USERS = "users/";

	public const string SHS_WEBSERVICE_URI_CSR = "csr/";

	public const string SHS_WEBSERVICE_URI_GETTOKEN = "platform/gettoken/";

	public const string SHS_WEBSERVICE_URI_COUNTERS = "counters/";

	public const string SHS_WEBSERVICE_URI_OBJECTS = "objects/";

	public const string SHS_WEBSERVICE_URI_CARDS = "cards/";

	public const string SHS_WEBSERVICE_URI_CODE_REDEMPTION = "redemption/";

	public const string SHS_WEBSERVICE_URI_INVENTORY = "inventory";

	public const string SHS_WEBSERVICE_URI_FRIENDS = "friends";

	public const string SHS_WEBSERVICE_URI_FRIENDS_REMOVE = "friends/remove";

	public const string SHS_WEBSERVICE_URI_FRIENDS_ADD_FRIEND = "friends/add/friend";

	public const string SHS_WEBSERVICE_URI_FRIENDS_ADD_IGNORE = "friends/add/ignore";

	public const string SHS_WEBSERVICE_URI_FRIENDS_DECLINE = "friends/decline";

	public const string SHS_WEBSERVICE_URI_HQ = "hq/room/";

	public const string SHS_WEBSERVICE_URI_WHEEL_SPIN = "wheel/spin/";

	public const string SHS_WEBSERVICE_URI_LOBBY_ARRIVE = "mm/lobby/arrive/";

	public const string SHS_WEBSERVICE_URI_LOBBY_SELECTION = "mm/lobby/selection/";

	public const string SHS_WEBSERVICE_URI_LEVEL_UP_REWARDS = "data/json/hero-level-rewards/";

	public const string SHS_WEBSERVICE_URI_LEVEL_UP_THRESHOLDS = "data/json/level-chart/";

	public const string SHS_WEBSERVICE_URI_MATCHMAKING_JOIN = "mm/join/";

	public const string SHS_WEBSERVICE_URI_MATCHMAKING_CARD_SOLO = "mm/card/solo/";

	public const string SHS_WEBSERVICE_URI_MATCHMAKING_CARD_INVITE = "mm/card/create/";

	public const string SHS_WEBSERVICE_URI_MATCHMAKING_CARD_ELO = "mm/card/anyone/";

	public const string SHS_WEBSERVICE_URI_MATCHMAKING_CARD_CANCEL = "mm/card/leave/";

	public const string SHS_WEBSERVICE_URI_MATCHMAKING_CARD_ACCEPT = "mm/card/accept/";

	public const string SHS_WEBSERVICE_URI_MATCHMAKING_BRAWLER_SOLO = "mm/brawler/solo/"; //was "mm/brawler/solo/"

	public const string SHS_WEBSERVICE_URI_MATCHMAKING_BRAWLER_ANYONE = "mm/brawler/anyone/";

	public const string SHS_WEBSERVICE_URI_MATCHMAKING_BRAWLER_FRIENDS = "mm/brawler/friends/";

	public const string SHS_WEBSERVICE_URI_MATCHMAKING_BRAWLER_ACCEPT = "mm/brawler/accept/";

	public const string SHS_WEBSERVICE_URI_MATCHMAKING_BRAWLER_LOCK = "mm/brawler/lock/";

	public const string SHS_WEBSERVICE_URI_MATCHMAKING_HQ_KNOCK = "mm/hq/knock/";

	public const string SHS_WEBSERVICE_URI_MATCHMAKING_ARCADE_ENTER = "mm/arcade/enter/";

	public const string SHS_WEBSERVICE_URI_ARCADE_DETAILS = "details/";

	public const string SHS_WEBSERVICE_URI_ARCADE_HIGHSCORE = "highscore/";

	public const string SHS_WEBSERVICE_URI_ARCADE_TIME = "system/time";

	public const string SHS_WEBSERVICE_URI_ARCADE_DETAILS_RELATIVE = "?relative=true";

	public const string SHS_WEBSOURCE_URI_CHALLENGE = "challenge";

	protected const string SHS_WEBSERVICE_URI_RASP_SUFFIX = ".rasp";

	protected const string SHS_WEBSERVICE_URI_RASP_PREFIX = "rasp/";

	protected const string SHS_WEBSERVICE_URI_TEXT_SUFFIX = "";

	protected const string SHS_URI_SEPARATOR = "$";

	protected const string SHS_WEBSOURCE_RESOURCES = "resources";

	protected const string SHS_WEBSOURCE_CONTENT = "content";

	protected const string SHS_WEBSOURCE_LOCALFILE = "localfile";

	protected const string SHS_WEBSOURCE_LOCALRESOURCE = "localresource";

	protected const string SHS_WEBSOURCE_ARCADE = "arcade";

	public const string SHS_WEBSOURCE_RESOURCES_URI = "resources$";

	public const string SHS_WEBSOURCE_CONTENT_URI = "content$";

	public const string SHS_WEBSOURCE_LOCALFILE_URI = "localfile$";

	public const string SHS_WEBSOURCE_LOCALRESOURCE_URI = "localresource$";

	public const string SHS_WEBSOURCE_ARCADE_URI = "arcade$";

	protected const string SHS_WEBSOURCE_SECURE = "secure";

	public const string SHS_WEBSOURCE_SECURE_URI = "secure$";

	protected const string SHS_WEBSERVICE_CONFIGFILE_URI = "AssetBundles/Configuration/servers.xml";

	public const int SHS_UNKNOWN_SIZE = 1;

	protected bool isConfigured;

	protected bool isConfigRequested;

	protected bool areParametersRequested;

	public Dictionary<string, string> UrlParameters;

	protected Queue<ShsWebRequest> preconfigRequests;

	public ShsWebServiceType CurrentType = ShsWebServiceType.RASP;

	protected Dictionary<string, ShsWebSource> webSources;

	protected Queue<ShsWebRequest> newRequests;

	protected Dictionary<string, ShsWebRequest> activeRequests;

	protected Dictionary<WWW, bool> activeWWW;

	private string sessionKey;

	private bool urlParametersLoaded;

	private bool serverConfigLoaded;

	public string SessionKey
	{
		get
		{
			return sessionKey;
		}
		set
		{
			sessionKey = value;
		}
	}

	public bool UrlParametersLoaded
	{
		get
		{
			return urlParametersLoaded;
		}
		set
		{
			urlParametersLoaded = value;
		}
	}

	public bool ServerConfigurationLoaded
	{
		get
		{
			return serverConfigLoaded;
		}
		set
		{
			serverConfigLoaded = value;
		}
	}

	public ShsWebService()
	{
		isConfigured = false;
		isConfigRequested = false;
		preconfigRequests = new Queue<ShsWebRequest>();
		areParametersRequested = false;
		UrlParameters = new Dictionary<string, string>();
		newRequests = new Queue<ShsWebRequest>();
		activeRequests = new Dictionary<string, ShsWebRequest>();
		webSources = new Dictionary<string, ShsWebSource>();
		activeWWW = new Dictionary<WWW, bool>();
		UrlParameters = new Dictionary<string, string>();
	}

	public static void SafeJavaScriptCall(string functionStr)
	{
		string script = string.Format("try {{ {0} }} catch(ex) {{}}", functionStr);
		Application.ExternalEval(script);
	}

	private void OnApplicationQuit()
	{
		StopAllCoroutines();
		foreach (WWW key in activeWWW.Keys)
		{
			key.Dispose();
		}
		activeWWW.Clear();
	}

	private void OnDestory()
	{
		if (activeWWW != null && activeWWW.Count > 0)
		{
			CspUtils.DebugLog("activeWWW is not empty on exit!");
		}
	}

	private void Start()
	{
		if (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
		{
			SafeJavaScriptCall("HEROUPNS.SendServersToUnity();");
			Application.ExternalCall("HEROUPNS.SendURLToUnity");
			areParametersRequested = true;
			try
			{
				Application.ExternalCall("HEROUPNS.SendUrlParametersToUnity");
			}
			catch (Exception arg)
			{
				CspUtils.DebugLog("Failed requesting URL parameters with error <" + arg + ">.");
			}
		}
		else
		{
			string configUrl = "file://" + Application.dataPath + "/AssetBundles/Configuration/servers.xml";
			LoadServerConfig(configUrl);
		}
	}

	public void OnRecieveServersFromWebPage(string serversUrl)
	{
		CspUtils.DebugLog("OnRecieveServersFromWebPage <" + serversUrl + "> && isConfigRequested = " + isConfigRequested + ", isConfigured = " + isConfigured);
		if (!isConfigRequested && !isConfigured)
		{
			isConfigRequested = true;
			LoadServerConfig(serversUrl + "?selector=" + UnityEngine.Random.Range(100000, 1000000000).ToString());
		}
	}

	public void OnRecieveUrlFromWebPage(string pageUrl)
	{
		CspUtils.DebugLog("OnRecieveUrlFromWebPage <" + pageUrl + "> && isConfigRequested = " + isConfigRequested + ", isConfigured = " + isConfigured);
		if (!isConfigRequested && !isConfigured)
		{
			string text = pageUrl.Trim();
			int num = text.LastIndexOf('/');
			if (num + 1 < text.Length)
			{
				text = text.Remove(num + 1);
			}
			string text2 = text + "AssetBundles/Configuration/servers.xml";
			string text3 = null;
			if (text.Contains(".dev.") || text.Contains(".int.") || text.Contains(".abc.") || text.Contains(".pdev."))
			{
				text3 = text + "servers.xml";
			}
			CspUtils.DebugLog("================Requesting the servers config file from <" + text3 + " , " + text2 + ">.");
			isConfigRequested = true;
			LoadServerConfigWithFallback(text3, text2);
		}
	}

	public void OnRecieveParametersFromWebPage(string parametersString)
	{
		CspUtils.DebugLog("Recieving URL parameters from hosting web page <" + parametersString + ">.");
		if (areParametersRequested)
		{
			areParametersRequested = false;
			string[] array = parametersString.Split('&');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (!string.IsNullOrEmpty(text))
				{
					string[] array3 = text.Split('=');
					if (array3.Length == 2)
					{
						UrlParameters[array3[0]] = array3[1];
					}
					else
					{
						CspUtils.DebugLog("Recieved malformed URL parameter <" + text + ">, unable to parse it.");
					}
				}
			}
			UrlParametersLoaded = true;
			AppShell.Instance.EventMgr.Fire(null, new UrlParametersReceivedMessage());
		}
		else
		{
			CspUtils.DebugLog("Received the URL from our web page when we didn't request it.  Ignoring it.");
		}
	}

	protected void LoadServerConfigWithFallback(string alternativeUrl, string configUrl)
	{
		if (!string.IsNullOrEmpty(alternativeUrl))
		{
			CspUtils.DebugLog("First Try: " + alternativeUrl);
			StubServerDelegate stubServerDelegate = new StubServerDelegate();
			stubServerDelegate.parent = this;
			stubServerDelegate.otherUrl = configUrl;
			ShsWebRequest shsWebRequest = new ShsWebRequest();
			shsWebRequest.OriginalUri = alternativeUrl;
			shsWebRequest.Callback = stubServerDelegate.OnServerConfigLoaded;
			shsWebRequest.PollingSeconds = 0.5f;
			shsWebRequest.Timeout = 30f;
			shsWebRequest.ServiceType = ShsWebServiceType.Text;
			StartRequest(shsWebRequest);
		}
		else
		{
			LoadServerConfig(configUrl);
		}
	}

	protected void LoadServerConfig(string configUrl)
	{
		ShsWebRequest shsWebRequest = new ShsWebRequest();
		shsWebRequest.OriginalUri = configUrl;
		shsWebRequest.Callback = OnServerConfigLoaded;
		shsWebRequest.PollingSeconds = 0.5f;
		shsWebRequest.Timeout = 30f;
		shsWebRequest.ServiceType = ShsWebServiceType.Text;
		StartRequest(shsWebRequest);
	}

	private void LoadServerConfigData(DataWarehouse serverConfigData)
	{
		AppShell.Instance.OnConfigServersLoaded(serverConfigData);
		DataWarehouse serverConfig = AppShell.Instance.ServerConfig;
		if (serverConfig.Navigator == null)
		{
			CspUtils.DebugLog("Unable to configure web sources because the server config is invalid.");
			return;
		}
		webSources["resources"] = new ShsWebSource();
		foreach (XPathNavigator item in Utils.Enumerate(serverConfig.GetValues("resources/server")))
		{
			webSources["resources"].AddSource("http://" + item.Value + "/");
		}
		webSources["arcade"] = new ShsWebSource();
		foreach (XPathNavigator item2 in Utils.Enumerate(serverConfig.GetValues("arcade/server")))
		{
			webSources["arcade"].AddSource("http://" + item2.Value + "/");
		}
		string text = serverConfig.TryGetString("build_timestamp", null);
		text = ((text == null) ? string.Empty : (text + "/"));
		webSources["content"] = ((!ShsCacheManager.enabled) ? new ShsWebSource() : new ShsCachedWebSource());
		foreach (XPathNavigator item3 in Utils.Enumerate(serverConfig.GetValues("content/server")))
		{
			string text2 = "http://" + item3.Value;
			string text3 = (!item3.Value.Contains(".shs.tas")) ? (text2 + "/shs/" + text) : (text2 + "/shs/");
			CspUtils.DebugLog("Adding content web source <" + text3 + ">.");
			webSources["content"].AddSource(text3);
			AppShell.Instance.SharedHashTable["ContentUri"] = text2;
		}
		string[] uriArray = new string[1]
		{
			"file://" + Application.dataPath + "/"
		};
		webSources["localfile"] = ((!serverConfig.TryGetBool("forcecaching", false)) ? new ShsWebSource(uriArray) : new ShsCachedWebSource(uriArray));
		webSources["localresource"] = new ShsWebSource(new string[1]
		{
			string.Empty
		});
		if (ShsCacheManager.enabled && serverConfig.TryGetBool("clearcache", false))
		{
			Caching.CleanCache();
		}
		isConfigured = true;
		foreach (ShsWebRequest preconfigRequest in preconfigRequests)
		{
			StartRequest(preconfigRequest);
		}
		preconfigRequests.Clear();
	}

	protected void OnServerConfigLoaded(ShsWebResponse response)
	{
		if (response.Status == 200)
		{
			DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
			dataWarehouse.Parse();
			LoadServerConfigData(dataWarehouse);
			ServerConfigurationLoaded = true;
			AppShell.Instance.EventMgr.Fire(this, new ServerConfigLoadedMessage());
		}
		else
		{
			CspUtils.DebugLog("Unable to load the server config data from <" + response.RequestUri + ">!  The game cannot continue.");
		}
	}

	private void Update()
	{
		if (newRequests.Count <= 0)
		{
			return;
		}
		ShsWebRequest shsWebRequest = newRequests.Dequeue();
		string originalUri = shsWebRequest.OriginalUri;
		if (!activeRequests.ContainsKey(originalUri))
		{
			if (shsWebRequest.Disposable)
			{
				activeRequests.Add(originalUri, shsWebRequest);
			}
			StartCoroutine(ProcessWebRequest(shsWebRequest));
		}
	}

	protected string BuildFullUri(string partialUri, ShsWebRequest request)
	{
		return BuildFullUri(partialUri, CurrentType, request);
	}

	public string BuildFullUri(string partialUri, ShsWebServiceType serviceType, ShsWebRequest request)
	{
		string text = partialUri;
		bool flag = false;
		if (request == null)
		{
			request = new ShsWebRequest();
		}
		request.Source = string.Empty;
		if (serviceType == ShsWebServiceType.Unspecified)
		{
			serviceType = CurrentType;
		}
		int num = partialUri.IndexOf("$");
		int startIndex = 0;
		if (num >= 0)
		{
			string text2 = partialUri.Substring(startIndex, num);
			if (text2 == "secure")
			{
				flag = true;
				startIndex = num + 1;
				num = partialUri.IndexOf("$", startIndex);
				text2 = partialUri.Substring(startIndex, num - startIndex);
			}
			ShsWebSource value;
			webSources.TryGetValue(text2, out value);
			if (value != null)
			{
				string text3 = partialUri.Substring(num + 1);
				value.GetCacheInfo(text3, out request.Cached, out request.Version);
				request.Source = text2;
				text = value.SourceUri;
				if (serviceType == ShsWebServiceType.RASP)
				{
					text += "rasp/";
				}
				text += text3;
			}
			else
			{
				CspUtils.DebugLog("A Source name <" + text2 + "> was given that was not recognized. Treating it as a full URI.");
			}
		}
		if (flag)
		{
			text = text.Replace("http://", "https://");
		}
		if (serviceType == ShsWebServiceType.Text)
		{
			text += string.Empty;
		}
		return text;
	}

	public void StartRequest(string uri, ShsWebServiceCallback callback)
	{
		StartRequest(uri, callback, null, null, 30f, 0.5f, ShsWebServiceType.Unspecified);
	}

	public void StartRequest(string uri, ShsWebServiceCallback callback, ShsWebServiceType serviceType)
	{
		StartRequest(uri, callback, null, null, 30f, 0.5f, serviceType);
	}

	public void StartRequest(string uri, ShsWebServiceCallback callback, byte[] postData)
	{
		StartRequest(uri, callback, postData, null, 30f, 0.5f, ShsWebServiceType.Unspecified);
	}

	public void StartRequest(string uri, ShsWebServiceCallback callback, byte[] postData, ShsWebServiceType serviceType)
	{
		StartRequest(uri, callback, postData, null, 30f, 0.5f, serviceType);
	}

	public void StartRequest(string uri, ShsWebServiceCallback callback, byte[] postData, Hashtable headers)
	{
		StartRequest(uri, callback, postData, headers, 30f, 0.5f, ShsWebServiceType.Unspecified);
	}

	public void StartRequest(string uri, ShsWebServiceCallback callback, byte[] postData, Hashtable headers, ShsWebServiceType serviceType)
	{
		StartRequest(uri, callback, postData, headers, 30f, 0.5f, serviceType);
	}

	public void StartRequest(string uri, ShsWebServiceCallback callback, byte[] postData, Hashtable headers, float timeout, float pollingSeconds)
	{
		StartRequest(uri, callback, postData, headers, timeout, pollingSeconds, ShsWebServiceType.Unspecified);
	}

	public void StartRequest(string uri, ShsWebServiceCallback callback, byte[] postData, Hashtable headers, float timeout, float pollingSeconds, ShsWebServiceType serviceType)
	{
		StartRequest(uri, callback, postData, headers, timeout, pollingSeconds, serviceType, false, false);
	}

	public void StartRequest(string uri, ShsWebServiceCallback callback, byte[] postData, Hashtable headers, float timeout, float pollingSeconds, ShsWebServiceType serviceType, bool disposable, bool cacheOnly)
	{
		CspUtils.DebugLog(uri);
		ShsWebRequest shsWebRequest = new ShsWebRequest();
		shsWebRequest.OriginalUri = uri;
		shsWebRequest.PostData = postData;
		shsWebRequest.Headers = headers;
		shsWebRequest.Timeout = timeout;
		shsWebRequest.PollingSeconds = pollingSeconds;
		shsWebRequest.Callback = callback;
		shsWebRequest.ServiceType = serviceType;
		shsWebRequest.Disposable = disposable;
		shsWebRequest.CacheOnly = cacheOnly;
		if (!isConfigured)
		{
			preconfigRequests.Enqueue(shsWebRequest);
		}
		else
		{
			StartRequest(shsWebRequest);
		}
	}

	public void CancelRequest(string uri)
	{
		if (activeRequests.ContainsKey(uri))
		{
			activeRequests.Remove(uri);
		}
	}

	public float CheckRequestProgress(string uri)
	{
		if (activeRequests.ContainsKey(uri))
		{
			if (activeRequests[uri].Cached)
			{
				return 0f;
			}
			WWW wwwInstance = activeRequests[uri].WwwInstance;
			return wwwInstance.size - wwwInstance.bytes.Length;
		}
		return -1f;
	}

	public float CheckRequestSize(string uri)
	{
		if (activeRequests.ContainsKey(uri))
		{
			if (activeRequests[uri].Cached)
			{
				return 0f;
			}
			WWW wwwInstance = activeRequests[uri].WwwInstance;
			return wwwInstance.size;
		}
		return -1f;
	}

	protected void StartRequest(ShsWebRequest request)
	{
		ShsWebServiceType serviceType = request.ServiceType;
		if (request.ServiceType == ShsWebServiceType.Unspecified)
		{
			serviceType = CurrentType;
		}
		request.ServiceType = serviceType;
		string text = BuildFullUri(request.OriginalUri, serviceType, request);
		CspUtils.DebugLog(text);
		WWW wWW = null;
		CspUtils.DebugLog("request.Source = " + request.Source);
		if (!(request.Source == "localresource"))
		{
			if (request.ServiceType == ShsWebServiceType.RASP && sessionKey != null)
			{
				if (request.PostData == null)
				{
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("AS_SESSION_KEY", sessionKey);
					request.PostData = wWWForm.data;
				}
				else
				{
					string @string = Encoding.ASCII.GetString(request.PostData);
					@string = @string + "&AS_SESSION_KEY=" + sessionKey;
					request.PostData = Encoding.ASCII.GetBytes(@string);
				}
			}
			if (request.PostData != null)
			{
				CspUtils.DebugLog("www1");
				wWW = ((request.Headers == null) ? new WWW(text, request.PostData) : new WWW(text, request.PostData, request.Headers));
			}
			else if (request.Cached)
			{
				try
				{
					CspUtils.DebugLog("www2");
					wWW = WWW.LoadFromCacheOrDownload(text, request.Version);
				}
				catch (Exception ex)
				{
					CspUtils.DebugLog("www3");
					CspUtils.DebugLog(string.Format("Failed loading <{0}> from cache.  Error message: {1}", text, ex.Message));
					wWW = new WWW(text);
				}
			}
			else
			{
				CspUtils.DebugLog("www4");   // this is where all the loading of assetbundles happens...
				CspUtils.DebugLog("text = " + text);

				///////////// block added by CSP /////////////////////////////////////////////
				// int pos = text.LastIndexOf(".") + 1;  // CSP
				// string ext = text.Substring(pos, text.Length - pos); // CSP
				// if (ext == "unity3d") { // CSP
				// 	if (!(text.Contains("/GUI/"))) {   // CSP only do unity3d bundles that arent GUI-related..
				// 			pos = text.LastIndexOf("/") + 1; // CSP
				// 			string bundleName = text.Substring(pos, text.Length - pos); // CSP
				// 			// need to do the following only for .unity3d files....also need to trim leading file path...					
				// 			CspUtils.DebugLog("bundleName = " + bundleName);
				// 			CspUtils.DebugLog("SourceUri = " + webSources["resources"].SourceUri);   
				// 			wWW = WWW.LoadFromCacheOrDownload(webSources["resources"].SourceUri + "cab-stubs/" + bundleName, request.Version);  // CSP
				// 			CspUtils.DebugLog("wWW = " + wWW);
				// 	}
				// 	else {
				// 		wWW = new WWW(text);  
				// 	}
				// }
				////////////////////////////////////////////////////////////////////////////////////
				//else {
					wWW = new WWW(text);  
				//}
				
			}
		}

		/////////////////// block added by CSP //////////////////////////////////
		// try
		// 		{
		// 			wWW = WWW.LoadFromCacheOrDownload(text, request.Version);
		// 		}
		// 		catch (Exception ex)
		// 		{
		// 			CspUtils.DebugLog(string.Format("Failed loading <{0}> from cache.  Error message: {1}", text, ex.Message));
		// 			wWW = new WWW(text);
		// 		}
		////////////////////////////////////////////////////


		request.Uri = text;
		request.WwwInstance = wWW;
		activeWWW.Add(wWW, true);
		
		newRequests.Enqueue(request);
	}

	protected IEnumerator ProcessWebRequest(ShsWebRequest request)
	{
		WWW www = request.WwwInstance;
		string key = request.OriginalUri;
		CspUtils.DebugLog("request.Source=" + request.Source);  
		CspUtils.DebugLog("request.OriginalUri=" + request.OriginalUri);  
		if (request.Source != "localresource")
		{
			float stopTime = Time.realtimeSinceStartup + request.Timeout;
			while (!www.isDone && stopTime > Time.realtimeSinceStartup)
			{
				yield return new WaitForSeconds(request.PollingSeconds);
				CspUtils.DebugLog("WAIT request.OriginalUri=" + request.OriginalUri); 
				if (!www.isDone && request.Disposable && !activeRequests.ContainsKey(key))
				{
					activeWWW.Remove(www);
					www.Dispose();
					CspUtils.DebugLog("BREAK request.OriginalUri=" + request.OriginalUri); 
					yield break;
				}
			}
		}

		CspUtils.DebugLog("DONE request.Source=" + request.Source);  
		CspUtils.DebugLog("DONE request.OriginalUri=" + request.OriginalUri); 
		CspUtils.DebugLog("DONE request.ServiceType=" +  request.ServiceType); 

		activeRequests.Remove(key);
		ShsWebResponse response = new ShsWebResponse();
		response.OriginalUri = request.OriginalUri;
		response.RequestUri = request.Uri;
		response.ServiceType = request.ServiceType;
		if (request.Source != "localresource" && !www.isDone)
		{
			response.TimedOut = true;
			CspUtils.DebugLog("timeout!");
		}
		else
		{
			switch (request.ServiceType)
			{
			case ShsWebServiceType.RASP:
				ProcessResponseRasp(www, response);
				break;
			case ShsWebServiceType.Mono:
				throw new NotImplementedException();
			case ShsWebServiceType.Text:
			case ShsWebServiceType.Binary:
			case ShsWebServiceType.AssetBundle:
			case ShsWebServiceType.AudioClip:
			case ShsWebServiceType.Movie:
			case ShsWebServiceType.LocalResource:
				ProcessResponseSimple(www, request, response);
				break;
			case ShsWebServiceType.Texture:
				yield return new WaitForSeconds(UnityEngine.Random.value * 0.9f);
				ProcessResponseSimple(www, request, response);
				break;
			default:
				throw new NotImplementedException();
			}
		}
		request.Callback(response);
		if (www != null)
		{
			activeWWW.Remove(www);
			www.Dispose();
		}
	}

	protected void ProcessHeadersXml(XmlReader r, Dictionary<string, string> headers)
	{
		string key = string.Empty;
		while (r.Read())
		{
			switch (r.NodeType)
			{
			case XmlNodeType.Element:
				key = r.Name;
				break;
			case XmlNodeType.Text:
				headers[key] = r.ReadString();
				break;
			}
		}
	}

	protected void ProcessResponseRasp(WWW www, ShsWebResponse response)
	{
		if (www != null && !string.IsNullOrEmpty(www.error))
		{
			response.Status = 404;
			CspUtils.DebugLog("ShsWebService requested URI: " + response.RequestUri);
			CspUtils.DebugLog("An error occured during the WWW request: " + www.error);
			//EditorApplication.isPaused = true;  // CSP - temporary, to see if UNity won't crash when quitting.
			return;
		}
		try
		{
			using (StringReader reader = new StringReader(www.text))
			{
				using (XmlReader xmlReader = XmlReader.Create(reader))
				{
					while (xmlReader.Read())
					{
						XmlNodeType nodeType = xmlReader.NodeType;
						if (nodeType == XmlNodeType.Element)
						{
							if (xmlReader.Name == "status")
							{
								int result;
								if (int.TryParse(xmlReader.ReadString(), out result))
								{
									response.Status = result;
								}
							}
							else if (xmlReader.Name == "headers")
							{
								ProcessHeadersXml(xmlReader.ReadSubtree(), response.Headers);
							}
							else if (xmlReader.Name == "body")
							{
								xmlReader.Read();
								response.Body = xmlReader.Value;
							}
						}
					}
				}
			}
			response.Size = www.bytes.Length;
		}
		catch (OutOfMemoryException)
		{
			AppShell.Instance.CriticalError(SHSErrorCodes.Code.OutOfMemory);
		}
		catch (Exception ex2)
		{
			CspUtils.DebugLog("Web Requested URI: " + response.RequestUri);
			CspUtils.DebugLog("An " + ex2 + " (" + ex2.Message + ") occurred while parsing: \n" + www.text);
			response.Status = 404;
		}
		if (response.Status >= 500 && response.Status <= 599)
		{
			CspUtils.DebugLog("Web request return server error <status=" + response.Status + ">! Request URL was <" + response.RequestUri + "> and the response body was <" + response.Body + ">.");
		}
	}

	protected void ProcessResponseSimple(WWW www, ShsWebRequest request, ShsWebResponse response)
	{
		try
		{
			if (www != null && !string.IsNullOrEmpty(www.error))
			{
				response.Status = 500;
				CspUtils.DebugLog("ShsWebService requested URI: " + response.RequestUri);
				CspUtils.DebugLog("An error occured during the WWW request: " + www.error);

				//// block added by CSP /////////////////////////////////
				CachedAssetBundle value = null;
				string text = response.RequestUri;  
				int pos = text.LastIndexOf("AssetBundles") + 13; // CSP
				string bundlePath = text.Substring(pos, text.Length - pos); // CSP
				bundlePath = bundlePath.Remove(bundlePath.Length - 8); // remove '.unity3d' from end

				if (AppShell.Instance.BundleLoader.CachedBundles.TryGetValue(bundlePath, out value)) {
					CspUtils.DebugLog("value=" + value);
				}
				else {
					CspUtils.DebugLog("bundle not found in cache for: " + bundlePath);
				}
				///////////////////////////////////////////////////////////

				//EditorApplication.isPaused = true;  // CSP - temporary, to see if UNity won't crash when quitting.
			}
			else
			{
				response.Status = 200;

				CspUtils.DebugLog("request.Source = " + response.RequestUri + "    " + request.Source);

				if (request.Source == "localresource")
				{
					switch (response.ServiceType)
					{
					case ShsWebServiceType.AssetBundle:
						response.Object = (Resources.Load(response.RequestUri) as AssetBundle);
						response.Size = 1;
						break;
					case ShsWebServiceType.AudioClip:
						response.Object = (Resources.Load(response.RequestUri) as AudioClip);
						response.Size = 1;
						break;
					case ShsWebServiceType.Binary:
						CspUtils.DebugLog("Service Type 'Binary' unsupported for Source 'localresource'.  Request <" + response.RequestUri + "> failed.");
						break;
					case ShsWebServiceType.Movie:
						response.Object = (Resources.Load(response.RequestUri) as MovieTexture);
						response.Size = 1;
						break;
					case ShsWebServiceType.LocalResource:
						response.Object = Resources.Load(response.RequestUri);
						response.Size = 1;
						break;
					case ShsWebServiceType.Text:
					{
						TextAsset textAsset = Resources.Load(response.RequestUri) as TextAsset;
						if (textAsset != null && textAsset.bytes != null)
						{
							response.Body = textAsset.text;
							response.Size = textAsset.bytes.Length;
						}
						else
						{
							response.Status = 404;
						}
						break;
					}
					case ShsWebServiceType.Texture:
						response.Texture = (Resources.Load(response.RequestUri) as Texture2D);
						response.Size = 1;
						break;
					default:
						throw new NotImplementedException();
					}
				}
				else
				{
					switch (response.ServiceType)
					{
					case ShsWebServiceType.AssetBundle:
						response.Object = www.assetBundle;
						CspUtils.DebugLog("PRS assetbundle:" + www.assetBundle.ToString());  // CSP
						//response.Size = (request.Cached ? 1 : www.bytes.Length);  //commented out by CSP
						///////// black added by CSP : ////////////////////
						if (request.Cached)
							response.Size = 1;
						else {
							if ((response.RequestUri.Contains("/GUI/"))) {
								response.Size = www.bytes.Length;
							}
							else {
								response.Size = 1;
							}
						}
						//////////////////////////////////////////////////////////
						
						break;
					case ShsWebServiceType.AudioClip:
						response.Object = www.audioClip;
						response.Size = www.bytes.Length;
						break;
					case ShsWebServiceType.Binary:
						response.Bytes = www.bytes;
						response.Size = www.bytes.Length;
						break;
					case ShsWebServiceType.Movie:
						response.Object = www.movie;
						response.Size = www.bytes.Length;
						break;
					case ShsWebServiceType.LocalResource:
						CspUtils.DebugLog("Service Type 'LocalResource' must be used with Source 'localresource'.  Request for <" + response.RequestUri + "> failed.");
						break;
					case ShsWebServiceType.Text:
						response.Body = www.text;
						response.Size = www.bytes.Length;
						break;
					case ShsWebServiceType.Texture:
						if (www.responseHeaders.ContainsKey("CONTENT-TYPE"))
						{
							string a = www.responseHeaders["CONTENT-TYPE"];
							if (a != "image/jpeg" && a != "image/png")
							{
								response.Status = 401;
							}
						}
						try
						{
							response.Texture = www.texture;
							response.Size = www.bytes.Length;
						}
						catch (SecurityException ex)
						{
							CspUtils.DebugLog("Permission Error :" + request.OriginalUri);
							CspUtils.DebugLog(ex.Message);
							response.Status = 402;
						}
						break;
					default:
						throw new NotImplementedException();
					}
				}
			}
		}
		catch (OutOfMemoryException)
		{
			AppShell.Instance.CriticalError(SHSErrorCodes.Code.OutOfMemory);
		}
	}

	public void Logout()
	{
		SessionKey = null;
	}
}
