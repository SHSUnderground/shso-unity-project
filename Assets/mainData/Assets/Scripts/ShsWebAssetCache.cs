using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[RequireComponent(typeof(ShsWebService))]
public class ShsWebAssetCache : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public delegate void AssetCacheConfiguredDelegate();

	public const string WEB_SOURCE_PATH = "/";

	private string serverUrl;

	private bool configured;

	private ShsWebService webService;

	private Dictionary<string, ShsWebResponse> cachedWebAssets;

	public string ServerUrl
	{
		get
		{
			return serverUrl;
		}
		set
		{
			serverUrl = value;
			Configured = true;
		}
	}

	public bool Configured
	{
		get
		{
			return configured;
		}
		set
		{
			if (!configured)
			{
				configured = value;
				if (this.AssetCacheConfigured != null)
				{
					this.AssetCacheConfigured();
				}
			}
		}
	}

	public Dictionary<string, ShsWebResponse> CachedWebAssets
	{
		get
		{
			return cachedWebAssets;
		}
	}

	public event AssetCacheConfiguredDelegate AssetCacheConfigured;

	public void StartRequest(string uri, ShsWebService.ShsWebServiceCallback callback)
	{
		string text = serverUrl;
		if (Regex.IsMatch(uri, "^http[s]*\\:") || Regex.IsMatch(uri, "^content\\$"))
		{
			text = uri;
		}
		else
		{
			if (!text.EndsWith("/"))
			{
				text += "/";
			}
			if (uri.StartsWith("/"))
			{
				text = text.Substring(0, text.Length - 1);
			}
			text += uri;
		}
		webService.StartRequest(text, delegate(ShsWebResponse response)
		{
			webAssetLoaded(response, callback, uri);
		}, ShsWebService.ShsWebServiceType.Texture);
	}

	private void webAssetLoaded(ShsWebResponse response, ShsWebService.ShsWebServiceCallback callback, string uriKey)
	{
		if (response.Status == 200)
		{
			cachedWebAssets[uriKey] = response;
		}
		if (callback != null)
		{
			callback(response);
		}
	}

	private void Awake()
	{
		webService = (ShsWebService)GetComponent(typeof(ShsWebService));
	}

	private void Start()
	{
		cachedWebAssets = new Dictionary<string, ShsWebResponse>();
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	public void OnServerConfigLoaded(DataWarehouse serverConfig)
	{
		if (!configured)
		{
			string str = serverConfig.TryGetString("//environment", "INT");
			string text = AppShell.Instance.ServerConfig.TryGetString("//" + str + "/webcontent/serverpath", string.Empty);
			if (!string.IsNullOrEmpty(text))
			{
				ServerUrl = string.Format("http://{0}", text);
				return;
			}
			ServerUrl = string.Empty;
			CspUtils.DebugLog("No web content specified for this server. In-game dynamic web content won't be viewable");
		}
	}
}
