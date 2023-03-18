using System.Collections.Generic;
using System.Xml.XPath;

public class ShsWebSource
{
	protected List<string> uriList;

	protected int uriIndex;

	public string SourceUri
	{
		get
		{
			uriIndex = (uriIndex + 1) % uriList.Count;
			return uriList[uriIndex];
		}
	}

	public ShsWebSource()
		: this(new string[0])
	{
	}

	public ShsWebSource(string[] uriArray)
	{
		uriList = new List<string>(uriArray);
		uriIndex = 0;
	}

	public ShsWebSource(DataWarehouse serverData, string sourcePath)
	{
		XPathNodeIterator values = serverData.GetValues(sourcePath);
		if (values == null)
		{
			CspUtils.DebugLog("No <" + sourcePath + "> web services specified in configuration!");
		}
		else
		{
			foreach (XPathNavigator item in Utils.Enumerate(values))
			{
				uriList.Add(item.Value);
			}
		}
	}

	public void AddSource(string newSource)
	{
		uriList.Add(newSource);
	}

	public virtual void GetCacheInfo(string resource, out bool isCached, out int version)
	{
		isCached = false;
		version = 0;
	}
}
