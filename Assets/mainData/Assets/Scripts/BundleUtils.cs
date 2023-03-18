using System.IO;
using UnityEngine;

public class BundleUtils
{
	public static bool Exists(string bundlePath)
	{
		if (!Application.isPlaying || Application.platform.ToString().Contains("Editor"))
		{
			return OnDisk(bundlePath);
		}
		if (Application.isPlaying)
		{
			return InManifest(bundlePath);
		}
		return false;
	}

	public static bool InManifest(string bundlePath)
	{
		return ShsCacheManager.Manifest.ContainsKey(bundlePath);
	}

	public static bool OnDisk(string bundlePath)
	{
		string path = Application.dataPath + "/AssetBundles/" + bundlePath;
		return File.Exists(path);
	}
}
