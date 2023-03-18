using ShsAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VOBundleLoader : Singleton<VOBundleLoader>
{
	private Dictionary<string, IEnumerable<string>> cachedBundleLists = new Dictionary<string, IEnumerable<string>>();

	public void LoadCharacter(string characterID)
	{
		LoadCharacter(characterID, null);
	}

	public void LoadCharacter(string characterID, TransactionMonitor transaction)
	{
		IEnumerable<string> bundlesUsedByCharacter = GetBundlesUsedByCharacter(characterID);
		if (bundlesUsedByCharacter != null)
		{
			LoadBundles(bundlesUsedByCharacter, transaction);
		}
	}

	protected IEnumerable<string> GetBundlesUsedByCharacter(string characterID)
	{
		IEnumerable<string> value = null;
		if (!cachedBundleLists.TryGetValue(characterID, out value))
		{
			value = GetBundlesFromManifest(characterID);
			cachedBundleLists[characterID] = value;
		}
		return value;
	}

	protected IEnumerable<string> GetBundlesFromManifest(string characterID)
	{
		List<string> list = new List<string>();
		string text = "vo/" + characterID + "_vo";
		CspUtils.DebugLog("Searching for character VO bundles... " + text);
		foreach (DictionaryEntry vOAsset in Singleton<VOAssetManifest>.instance.VOAssets)
		{
			if ((vOAsset.Key as string).StartsWith(text))
			{
				string bundle = (vOAsset.Value as BundledAsset).Bundle;
				if (!list.Contains(bundle) && bundle != null)
				{
					list.Add(bundle);
				}
			}
		}
		return list;
	}

	protected void LoadBundles(IEnumerable<string> bundles, TransactionMonitor transaction)
	{
		if (!Helpers.VOBundlesDownloaded)
		{
			if (transaction != null)
			{
				transaction.Fail("VO Bundles are not yet downloaded.");
			}
		}
		else
		{
			foreach (string bundle in bundles)
			{
				string bundlePath = GetBundlePath(bundle);
				if (transaction != null)
				{
					transaction.AddStep(bundlePath, TransactionMonitor.DumpTransactionStatus);
				}
				AppShell.Instance.BundleLoader.FetchAssetBundle(bundlePath, OnAssetBundleLoaded, transaction);
			}
		}
	}

	protected string GetBundlePath(string bundleName)
	{
		return Helpers.GetAudioBundleName(bundleName, Helpers.GetLocalizationStrategy(AudioAssetType.FlavorVO));
	}

	protected void OnAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		TransactionMonitor transactionMonitor = extraData as TransactionMonitor;
		if (transactionMonitor != null)
		{
			transactionMonitor.CompleteStep(response.Path);
		}
	}
}
