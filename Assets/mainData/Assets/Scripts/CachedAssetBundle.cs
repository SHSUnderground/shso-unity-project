using System.Collections.Generic;
using UnityEngine;

public class CachedAssetBundle
{
	public AssetBundle Bundle;

	public string RequestPath = string.Empty;

	public float TimeLoaded;

	public int SizeInBytes;

	public Queue<AssetBundleCallbackWithData> OutstandingCallbacks;

	public bool UnloadOnSceneTransition = true;

	public int sceneRequested = -1;

	public AssetBundleLoader.LoadCategory loadCategory = AssetBundleLoader.LoadCategory.Cache;

	public Dictionary<string, Object> PreloadedAssets;

	public CachedAssetBundle()
	{
		OutstandingCallbacks = new Queue<AssetBundleCallbackWithData>();
		PreloadedAssets = new Dictionary<string, Object>();
	}

	public void Unload(bool unloadAllLoadedObjects)
	{
		if (PreloadedAssets != null)
		{
			PreloadedAssets.Clear();
			if (Bundle != null)
			{
				Bundle.Unload(unloadAllLoadedObjects);
				Bundle = null;
			}
			if (OutstandingCallbacks != null)
			{
				if (OutstandingCallbacks.Count <= 0)
				{
					return;
				}
				CspUtils.DebugLog("Unloading bundle <" + RequestPath + "> with outstanding callbacks");
				AssetBundleLoadResponse assetBundleLoadResponse = new AssetBundleLoadResponse();
				assetBundleLoadResponse.Path = RequestPath;
				assetBundleLoadResponse.Error = "loading canceled";
				assetBundleLoadResponse.Bundle = null;
				while (OutstandingCallbacks.Count > 0)
				{
					AssetBundleCallbackWithData assetBundleCallbackWithData = OutstandingCallbacks.Dequeue();
					if (assetBundleCallbackWithData != null && assetBundleCallbackWithData.Callback != null)
					{
						assetBundleCallbackWithData.Callback(assetBundleLoadResponse, assetBundleCallbackWithData.ExtraData);
					}
				}
			}
			else
			{
				CspUtils.DebugLog("OutstandingCallbacks is NULL");
			}
		}
		else
		{
			CspUtils.DebugLog("PreloadedAssets is null");
		}
	}

	public void MarkAssetsAsUsed()
	{
		sceneRequested = AppShell.Instance.SceneCounter;
	}
}
