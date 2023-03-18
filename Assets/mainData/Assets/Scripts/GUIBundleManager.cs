using System;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class GUIBundleManager
{
	public enum BundleLoadStateEnum
	{
		NotLoaded,
		Loading,
		Loaded,
		Reloading
	}

	public enum BundleLocaleParamsEnum
	{
		NotLocalized,
		Localized,
		LocalizedOnly
	}

	public class BundleLoadInfo
	{
		public string BundleName;

		public AssetBundle Bundle;

		public AssetBundle EnglishBundle;

		public AssetBundle LanguageBundle;

		public BundleLoadStateEnum State;

		public List<KeyValuePair<AssetBundleLoader.AssetBundleLoaderCallback, object>> PendingRequests;

		public object ExtraData;

		public BundleLocaleParamsEnum LocaleDependency;

		public string CurrentLocale;

		public bool LocalizedFallbackRequest;

		public bool LoadingEnglishForLocalizedFallback;

		public BundleLoadInfo()
		{
			BundleName = string.Empty;
			Bundle = null;
			EnglishBundle = null;
			LanguageBundle = null;
			State = BundleLoadStateEnum.NotLoaded;
			PendingRequests = new List<KeyValuePair<AssetBundleLoader.AssetBundleLoaderCallback, object>>();
			ExtraData = null;
			LocaleDependency = BundleLocaleParamsEnum.NotLocalized;
			CurrentLocale = "non_locale";
		}
	}

	public delegate void BundleLoadedDelegate(string bundleName, AssetBundle bundle);

	public BundleLoadedDelegate OnBundleLoaded;

	public static readonly string BUNDLE_PATH_PREFIX = "GUI/";

	public static readonly string BUNDLE_PATH_LOCALE = "i18n/";

	public static readonly string BUNDLE_PATH_NON_LOCALE = "non_locale/";

	public static readonly string BUNDLE_ASSET_LOCALE_IDENTIFIER_PREFIX = "L_";

	public static readonly string BUNDLE_PATH_LOCALE_FALLBACK = "en_us";

	private GUIManager manager;

	private AssetBundleLoader loader;

	private Dictionary<string, BundleLoadInfo> assetBundleDictionary;

	public Dictionary<string, UnityEngine.Object> assetCache = new Dictionary<string, UnityEngine.Object>();  // CSP


	public List<BundleLoadInfo> AssetBundles
	{
		get
		{
			return new List<BundleLoadInfo>(assetBundleDictionary.Values);
		}
	}

	public GUIBundleManager(GUIManager manager, BundleLoadedDelegate loadDelegate)
	{
		this.manager = manager;
		OnBundleLoaded = (BundleLoadedDelegate)Delegate.Combine(OnBundleLoaded, loadDelegate);
		assetBundleDictionary = new Dictionary<string, BundleLoadInfo>();
		getLoader();
	}

	public BundleLoadStateEnum LoadBundle(string bundleName, AssetBundleLoader.AssetBundleLoaderCallback cb, object extraData)
	{
		if (assetBundleDictionary.ContainsKey(bundleName))
		{
			BundleLoadStateEnum state = assetBundleDictionary[bundleName].State;
			if (state == BundleLoadStateEnum.Loaded)
			{
				if (cb != null)
				{
					AssetBundleLoadResponse assetBundleLoadResponse = new AssetBundleLoadResponse();
					assetBundleLoadResponse.Bundle = assetBundleDictionary[bundleName].Bundle;
					cb(assetBundleLoadResponse, extraData);
				}
				return BundleLoadStateEnum.Loaded;
			}
			CspUtils.DebugLog("Bundle loading in progress: " + bundleName);
			assetBundleDictionary[bundleName].PendingRequests.Add(new KeyValuePair<AssetBundleLoader.AssetBundleLoaderCallback, object>(cb, extraData));
			return BundleLoadStateEnum.Loading;
		}
		BundleLoadInfo bundleLoadInfo = new BundleLoadInfo();
		bundleLoadInfo.BundleName = bundleName;
		bundleLoadInfo.State = BundleLoadStateEnum.NotLoaded;
		bundleLoadInfo.Bundle = null;
		bundleLoadInfo.ExtraData = extraData;
		if (cb != null)
		{
			bundleLoadInfo.PendingRequests.Add(new KeyValuePair<AssetBundleLoader.AssetBundleLoaderCallback, object>(cb, extraData));
		}
		assetBundleDictionary[bundleName] = bundleLoadInfo;
		AssetBundleLoader assetBundleLoader = getLoader();
		if (assetBundleLoader != null)
		{
			int version = -1;
			string bundlePath = GetBundlePath(bundleName, false);
			CspUtils.DebugLog("BLSE bundlePath (noloc)=" + bundlePath);  // CSP
			bool flag = ShsCacheManager.IsResourceCached("assetbundles/" + bundlePath + ".unity3d", out version);
			bundlePath = GetBundlePath(bundleName, true);
			CspUtils.DebugLog("BLSE bundlePath (loc)=" + bundlePath);  // CSP
			bool flag2 = ShsCacheManager.IsResourceCached("assetbundles/" + bundlePath + ".unity3d", out version);
			bundleLoadInfo.LocaleDependency = (flag2 ? (flag ? BundleLocaleParamsEnum.Localized : BundleLocaleParamsEnum.LocalizedOnly) : BundleLocaleParamsEnum.NotLocalized);
			
			CspUtils.DebugLog("BLSE bundleName=" + bundleName + " flag=" + flag + " flag2=" + flag2 + " bli.ld=" + bundleLoadInfo.LocaleDependency); // CSP
			
			if (bundleLoadInfo.LocaleDependency != BundleLocaleParamsEnum.LocalizedOnly)
			{
				assetBundleLoader.FetchAssetBundle(BUNDLE_PATH_PREFIX + BUNDLE_PATH_NON_LOCALE + bundleName, delegate(AssetBundleLoadResponse response, object ed)
				{
					bundleLoadedCallback(response, ed, bundleName, false);
				}, extraData, false);
			}
			if (bundleLoadInfo.LocaleDependency != 0)
			{
				bundleLoadInfo.CurrentLocale = AppShell.Instance.Locale;
				assetBundleLoader.FetchAssetBundle(bundlePath, delegate(AssetBundleLoadResponse response, object ed)
				{
					bundleLoadedCallback(response, ed, bundleName, true);
				}, extraData, false);
			}
		}
		else
		{
			CspUtils.DebugLog("Attempting to load bundle when AssetBundleLoader is non-existent");
		}
		return BundleLoadStateEnum.NotLoaded;
	}

	public string GetBundlePath(string bundleName, bool localized)
	{
		if (!localized)
		{
			return BUNDLE_PATH_PREFIX + BUNDLE_PATH_NON_LOCALE + bundleName;
		}
		string currentLocaleDirectory = LocaleMapper.GetCurrentLocaleDirectory();
		return BUNDLE_PATH_PREFIX + BUNDLE_PATH_LOCALE + currentLocaleDirectory + "/" + currentLocaleDirectory + "_" + bundleName;
	}

	public void LocaleChanged(TransactionMonitor localeTransaction)
	{
		if (localeTransaction == null)
		{
			localeTransaction = TransactionMonitor.CreateTransactionMonitor("GUIBundleManager_localeTransaction", onLocalChangeBundlesLoaded, 0f, null);
		}
		else
		{
			localeTransaction.onComplete += onLocalChangeBundlesLoaded;
		}
		foreach (KeyValuePair<string, BundleLoadInfo> item in assetBundleDictionary)
		{
			if (item.Value.LocaleDependency != 0)
			{
				localeTransaction.AddStep(item.Key);
				string bundlePath = GetBundlePath(item.Key, true);
				item.Value.LanguageBundle = null;
				item.Value.State = BundleLoadStateEnum.Loading;
				loader.FetchAssetBundle(bundlePath, delegate(AssetBundleLoadResponse response, object ed)
				{
					if (response.Error != null && response.Error != string.Empty)
					{
						CspUtils.DebugLog("Could not retrieve: " + ed.ToString() + " post locale change. Reason:" + response.Error);
						localeTransaction.FailStep(ed.ToString(), "Couldn't retrieve: " + ed.ToString());
					}
					bundleLoadedCallback(response, ed, ed.ToString(), true);
					localeTransaction.CompleteStep(ed.ToString());
				}, item.Key, false);
			}
		}
	}

	private void onLocalChangeBundlesLoaded(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		CspUtils.DebugLog("Change bundle load complete. going through GUI roots and updating.");
		foreach (IGUIContainer root in manager.Roots)
		{
			root.OnLocaleChange(LocaleMapper.GetCurrentLocale());
		}
	}

	public bool UnloadBundle(string bundleName)
	{
		BundleLoadInfo value = null;
		if (assetBundleDictionary.TryGetValue(bundleName, out value))
		{
			if (value.EnglishBundle != null)
			{
				value.EnglishBundle.Unload(true);
			}
			if (value.LanguageBundle != null)
			{
				value.LanguageBundle.Unload(true);
			}
			if (value.Bundle != null)
			{
				value.Bundle.Unload(true);
			}
			assetBundleDictionary.Remove(bundleName);
			return true;
		}
		CspUtils.DebugLog("No bundle named: " + bundleName + " to unload.");
		return false;
	}

	public bool Unload()
	{
		bool result = true;
		foreach (string key in assetBundleDictionary.Keys)
		{
			if (!UnloadBundle(key))
			{
				result = false;
			}
		}
		return result;
	}

	public void CspDebugBundle() {
			// CSP - LoadAll does work, though. ///////
			// string bundleName = "common_bundle";
			// CspUtils.DebugLog("****************** CspDebugBundle() CALLED!  Bundle name=" + bundleName);
			// BundleLoadInfo tempBundleInfo = assetBundleDictionary[bundleName];
			// UnityEngine.Object[] objs = tempBundleInfo.Bundle.LoadAll();
			// int i=0;
			// foreach (UnityEngine.Object obj in objs)
			// {	
			// 	CspUtils.DebugLog(i + " loaded asset name:" + obj.name);
			// 	i++;
			// 	if (i==5)
			// 	{
			// 		//EditorApplication.isPaused = true;   // going thru the first 5 objects is good enough...pause
			// 		break;
			// 	}
			// }
			///////////////////////////////////////
	}

	private void bundleLoadedCallback(AssetBundleLoadResponse response, object ed, string bundleName, bool localizedBundle)
	{
		BundleLoadInfo bundleLoadInfo = assetBundleDictionary[bundleName];
		if (localizedBundle)
		{
			if (!string.IsNullOrEmpty(response.Error))
			{
				if (bundleLoadInfo.LocalizedFallbackRequest)
				{
					CspUtils.DebugLog("Localized bundle for: " + bundleLoadInfo.BundleName + " Can't be retrieved, including the fallback english version.");
					bundleLoadInfo.LanguageBundle = response.Bundle;
					bundleLoadInfo.CurrentLocale = AppShell.Instance.Locale;
					CspDebugBundle();
				}
				else
				{
					CspUtils.DebugLog("ICK. Can't load localized asset, so trying English version");
					bundleLoadInfo.LocalizedFallbackRequest = true;
					string bundlePath = BUNDLE_PATH_PREFIX + BUNDLE_PATH_LOCALE + BUNDLE_PATH_LOCALE_FALLBACK + "/" + BUNDLE_PATH_LOCALE_FALLBACK + "_" + bundleLoadInfo.BundleName;
					loader.FetchAssetBundle(bundlePath, delegate(AssetBundleLoadResponse innerResponse, object innerEd)
					{
						bundleLoadedCallback(innerResponse, innerEd, bundleName, true);
					}, ed, false);
				}
			}
			else if (bundleLoadInfo.LoadingEnglishForLocalizedFallback)
			{
				bundleLoadInfo.EnglishBundle = response.Bundle;
				CspDebugBundle();
			}
			else
			{
				bundleLoadInfo.LanguageBundle = response.Bundle;
				bundleLoadInfo.CurrentLocale = AppShell.Instance.Locale;
				CspDebugBundle();
				if (bundleLoadInfo.CurrentLocale != BUNDLE_PATH_LOCALE_FALLBACK && bundleLoadInfo.CurrentLocale != BUNDLE_PATH_LOCALE_FALLBACK.Replace("_", "-"))
				{
					bundleLoadInfo.LoadingEnglishForLocalizedFallback = true;
					string bundlePath2 = BUNDLE_PATH_PREFIX + BUNDLE_PATH_LOCALE + BUNDLE_PATH_LOCALE_FALLBACK + "/" + BUNDLE_PATH_LOCALE_FALLBACK + "_" + bundleLoadInfo.BundleName;
					loader.FetchAssetBundle(bundlePath2, delegate(AssetBundleLoadResponse innerResponse, object innerEd)
					{
						bundleLoadedCallback(innerResponse, innerEd, bundleName, true);
					}, ed, false);
					bundleLoadInfo.State = BundleLoadStateEnum.Loading;
					return;
				}
				bundleLoadInfo.EnglishBundle = response.Bundle;
				CspDebugBundle();
			}
		}
		else
		{
			bundleLoadInfo.Bundle = response.Bundle;

			//CspUtils.DebugLog("MAINASSET= " + bundleLoadInfo.Bundle.mainAsset.ToString() + " Bundle name=" + bundleName);  // CSP
			CspDebugBundle();
		}
		bundleLoadInfo.State = BundleLoadStateEnum.Loading;
		if ((bundleLoadInfo.LocaleDependency == BundleLocaleParamsEnum.NotLocalized && bundleLoadInfo.Bundle != null) || (bundleLoadInfo.LocaleDependency == BundleLocaleParamsEnum.LocalizedOnly && bundleLoadInfo.LanguageBundle != null) || (bundleLoadInfo.LocaleDependency == BundleLocaleParamsEnum.Localized && bundleLoadInfo.Bundle != null && bundleLoadInfo.LanguageBundle != null && bundleLoadInfo.EnglishBundle != null))
		{
			bundleLoadInfo.State = BundleLoadStateEnum.Loaded;
		}
		if (bundleLoadInfo.State == BundleLoadStateEnum.Loaded)
		{
			foreach (KeyValuePair<AssetBundleLoader.AssetBundleLoaderCallback, object> pendingRequest in bundleLoadInfo.PendingRequests)
			{
				pendingRequest.Key(response, pendingRequest.Value);
			}
			bundleLoadInfo.PendingRequests.Clear();
			if (OnBundleLoaded != null)
			{
				OnBundleLoaded(bundleName, response.Bundle);
			}
		}
	}

	public bool IsBundleLoaded(string bundleName)
	{
		return assetBundleDictionary.ContainsKey(bundleName) && assetBundleDictionary[bundleName].State == BundleLoadStateEnum.Loaded;
	}

	public UnityEngine.Object LoadAsset(string bundleName, string bundleAsset)
	{
		UnityEngine.Object @object = null;

		//// CSP - original code did no asset caching....so I had to implement my own.//////////
		//// probably need to modify it to purge asset cache ASAP. ////////		
		if(assetCache.TryGetValue(bundleAsset, out @object))
		{
			CspUtils.DebugLog("GUIBM found in cache: " + bundleAsset);
			return @object;
		}
		///////////////////////////////////////////////////////////////////////////////////////

		bool flag = bundleAsset.StartsWith(BUNDLE_ASSET_LOCALE_IDENTIFIER_PREFIX);
		BundleLoadInfo value;
		if (assetBundleDictionary.TryGetValue(bundleName, out value))
		{
			if (value.LocaleDependency != 0)  // if Localized or LocalizedOnly
			{				
				//@object = assetBundleDictionary[bundleName].LanguageBundle.Load(bundleAsset);
				
				///////////////////////////////////////////////////////////////////////////
				// if (@object == null) { // CSP - this if-block needed as a workaround to Load() problem.
				// 	assetBundleDictionary[bundleName].LanguageBundle.LoadAll();  
				// 	@object = assetBundleDictionary[bundleName].LanguageBundle.Load(bundleAsset);

				// 	if (@object != null)
				// 	{
				// 		CspUtils.DebugLog("OBJECT FOUND USING LoadAll()! " + bundleAsset);
				// 		assetCache.Add(bundleAsset, @object);  // added by CSP
				// 		return @object;
				// 	}
				// }
				// else
				// {	
				// 	CspUtils.DebugLog("OBJECT FOUND USING LOAD()! " + bundleAsset);
				// 	assetCache.Add(bundleAsset, @object);  // added by CSP
				// 	return @object;
				// }
				////////////////////////////////////////////////////////////////////////////////

				////// THIS BLOCK ADDED BY CSP //////////////////////////////////////////////////
				CspUtils.DebugLog("attempting to find asset in bundle: " + bundleName);			
				@object = CspUtils.CspLoad(assetBundleDictionary[bundleName].LanguageBundle, bundleAsset);
				if (@object != null)
				{					
					assetCache.Add(bundleAsset, @object);  // added by CSP
					return @object;
				}
				/////////////////////////////////////////////////////////////////////////////////


				if (@object == null && flag)
				{
					CspUtils.DebugLog("Expecting: " + bundleAsset + " in localized bundle (" + bundleName + "), but asset not found.");
					
					UnityEngine.Object[] objs = assetBundleDictionary[bundleName].LanguageBundle.LoadAll();
					//CspUtils.DebugLog("looking for asset:" + bundleAsset + " in bundle " + bundleName + " AKA " + assetBundleDictionary[bundleName].BundleName);
					int i=0;
					foreach (UnityEngine.Object obj in objs)
					{	
						//CspUtils.DebugLog(i + " loaded asset name:" + obj.name);
						i++;
						if (obj.name == bundleAsset) {
							@object = obj;
							CspUtils.DebugLog(i + "FOUND asset:" + bundleAsset + " in bundle " + bundleName);
							assetCache.Add(bundleAsset, @object);  // added by CSP
							return @object;
						}					 
						//else
						//  Resources.UnloadAsset(obj);   // if it's not the object we're looking for, destroy it.
					}
				}
			}
			if (value.LocaleDependency != BundleLocaleParamsEnum.LocalizedOnly && @object == null)
			{
				assetBundleDictionary[bundleName].Bundle.LoadAll();  // CSP
				@object = assetBundleDictionary[bundleName].Bundle.Load(bundleAsset);
				if (@object == null) { // CSP - this if-block needed as a workaround to Load() problem.			 
					UnityEngine.Object[] objs = assetBundleDictionary[bundleName].Bundle.LoadAll();

					int i=0;
					foreach (UnityEngine.Object obj in objs)
					{	
						//CspUtils.DebugLog(i + " loaded asset name:" + obj.name);
						i++;
						if (obj.name == bundleAsset) {
							@object = obj;
							CspUtils.DebugLog(i + " FOUND asset:" + bundleAsset + " in bundle " + bundleName);
							assetCache.Add(bundleAsset, @object);  // added by CSP
							return @object;
						}					 
					}

					//@object = assetBundleDictionary[bundleName].Bundle.Load(bundleAsset);
				}
				else
				{
						CspUtils.DebugLog("OBJECT FOUND USING LOAD()! " + bundleAsset);
						assetCache.Add(bundleAsset, @object);  // added by CSP
						return @object;
				}
				if (@object == null)
				{
					if (assetBundleDictionary[bundleName] == null || assetBundleDictionary[bundleName].EnglishBundle == null)
					{
						CspUtils.DebugLog("Bundle problems with " + bundleAsset + ". Asset not found in " + bundleName + " bundle and it does not have an English fallback.");
						return null;
					}
					@object = assetBundleDictionary[bundleName].EnglishBundle.Load(bundleAsset);
					if (!(@object == null))
					{
						CspUtils.DebugLog("Expected localized asset, but found asset: " + bundleAsset + " in en_us bundle.");
					}
				}
				else if (flag)
				{
					CspUtils.DebugLog("Expected localized asset, but found asset: " + bundleAsset + " in non_locale bundle.");
				}
			}
			if (!(@object == null))
			{
			}
		}
		else
		{
			CspUtils.DebugLog("No bundle: " + bundleName + " exists to pull asset: " + bundleAsset + " from");
		}
		if (@object != null)
			assetCache.Add(bundleAsset, @object);  // added by CSP
		return @object;
	}

	public void LoadAsset(string bundleName, string bundleAsset, object extraData, AssetBundleLoader.AssetLoadedCallback callback)
	{
		bool expectsLocalizedAsset = bundleAsset.StartsWith(BUNDLE_ASSET_LOCALE_IDENTIFIER_PREFIX);
		BundleLoadInfo value;
		if (!assetBundleDictionary.TryGetValue(bundleName, out value))
		{
			CspUtils.DebugLog("No bundle: " + bundleName + " exists to pull asset: " + bundleAsset + " from");
		}
		else if (value.LocaleDependency != 0)
		{
			string bundlePath = GetBundlePath(bundleName, true);
			loader.LoadAsset(bundlePath, bundleAsset, extraData, delegate(UnityEngine.Object obj, AssetBundle bundle, object extraDataInner)
			{
				if (obj == null)
				{
					loader.LoadAsset(BUNDLE_PATH_PREFIX + BUNDLE_PATH_NON_LOCALE + bundleName, bundleAsset, extraData, delegate(UnityEngine.Object fallbackObject, AssetBundle bundle2, object extraDataNLInner)
					{
						if (fallbackObject == null)
						{
							CspUtils.DebugLog("Can't load: " + bundleAsset + " in " + bundleName);
						}
						else if (expectsLocalizedAsset)
						{
							CspUtils.DebugLog("Expecting localized asset for: " + bundleAsset + ", but found it in the non_locale bundle");
						}
						if (callback != null)
						{
							callback(fallbackObject, bundle2, extraDataNLInner);
						}
					});
				}
				else
				{
					if (!expectsLocalizedAsset)
					{
						CspUtils.DebugLog("Not expecting localized asset for: " + bundleAsset);
					}
					if (callback != null)
					{
						callback(obj, bundle, extraDataInner);
					}
				}
			});
		}
		else
		{
			loader.LoadAsset(BUNDLE_PATH_PREFIX + BUNDLE_PATH_NON_LOCALE + bundleName, bundleAsset, extraData, delegate(UnityEngine.Object nlObject, AssetBundle bundle2, object extraDataNLInner)
			{
				if (nlObject == null)
				{
					CspUtils.DebugLog("Can't load: " + bundleAsset + " in " + bundleName);
				}
				else if (expectsLocalizedAsset)
				{
					CspUtils.DebugLog("Expecting localized asset for: " + bundleAsset + ", but found it in the non_locale bundle");
				}
				if (callback != null)
				{
					callback(nlObject, bundle2, extraDataNLInner);
				}
			});
		}
	}

	public T Load<T>(string bundleName, string bundleAsset) where T : UnityEngine.Object
	{
		if (bundleName == "common_bundle")
			CspDebugBundle();
		return LoadAsset(bundleName, bundleAsset) as T;
	}

	private AssetBundleLoader getLoader()
	{
		if (AppShell.Instance.BundleLoader != null)
		{
			loader = AppShell.Instance.BundleLoader;
		}
		return loader;
	}
}
