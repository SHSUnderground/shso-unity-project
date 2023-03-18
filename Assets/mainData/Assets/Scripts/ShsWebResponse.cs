using System.Collections.Generic;
using UnityEngine;

public class ShsWebResponse
{
	protected ShsWebService.ShsWebServiceType serviceType;

	protected int status;

	protected string originalUri = string.Empty;

	protected string uri = string.Empty;

	protected Dictionary<string, string> headers;

	protected string body = string.Empty;

	protected byte[] bytes;

	protected Texture2D texture;

	protected Object theObject;

	protected bool timedOut;

	protected int size;

	public ShsWebService.ShsWebServiceType ServiceType
	{
		get
		{
			return serviceType;
		}
		set
		{
			serviceType = value;
		}
	}

	public int Status
	{
		get
		{
			return status;
		}
		set
		{
			status = value;
		}
	}

	public string OriginalUri
	{
		get
		{
			return originalUri;
		}
		set
		{
			originalUri = value;
		}
	}

	public string RequestUri
	{
		get
		{
			return uri;
		}
		set
		{
			uri = value;
		}
	}

	public Dictionary<string, string> Headers
	{
		get
		{
			return headers;
		}
	}

	public string Body
	{
		get
		{
			return body;
		}
		set
		{
			body = value;
		}
	}

	public Texture2D Texture
	{
		get
		{
			return texture;
		}
		set
		{
			texture = value;
		}
	}

	public byte[] Bytes
	{
		get
		{
			return bytes;
		}
		set
		{
			bytes = value;
		}
	}

	public Object Object
	{
		get
		{
			return theObject;
		}
		set
		{
			theObject = value;
		}
	}

	public bool TimedOut
	{
		get
		{
			return timedOut;
		}
		set
		{
			timedOut = value;
		}
	}

	public int Size
	{
		get
		{
			return size;
		}
		set
		{
			size = value;
		}
	}

	public ShsWebResponse()
	{
		headers = new Dictionary<string, string>();
	}

	public ShsWebResponse Copy()
	{
		return (ShsWebResponse)MemberwiseClone();
	}
}
