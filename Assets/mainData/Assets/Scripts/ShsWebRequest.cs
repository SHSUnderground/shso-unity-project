using System.Collections;
using UnityEngine;

public class ShsWebRequest
{
	public string OriginalUri;

	public byte[] PostData;

	public Hashtable Headers;

	public string Uri;

	public ShsWebService.ShsWebServiceCallback Callback;

	public float Timeout;

	public float PollingSeconds;

	public ShsWebService.ShsWebServiceType ServiceType;

	public string Source;

	public WWW WwwInstance;

	public bool Cached;

	public int Version;

	public bool Disposable;

	public bool CacheOnly;
}
