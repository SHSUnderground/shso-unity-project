using System;
using System.Collections.Generic;

public class FeatureImageLoader
{
	public enum FeatureImageEnum
	{
		Hero,
		Game,
		Mission,
		Upsell
	}

	private const int NEWS_ASSET_COUNT = 3;

	private static TransactionMonitor currentTransaction;

	private static int news_asset_current;

	public static Dictionary<FeatureImageEnum, FeatureImageInfo> FeatureImageInfoDictionary;

	static FeatureImageLoader()
	{
		FeatureImageInfoDictionary = new Dictionary<FeatureImageEnum, FeatureImageInfo>();
		Dictionary<FeatureImageEnum, FeatureImageInfo> featureImageInfoDictionary = FeatureImageInfoDictionary;
		FeatureImageInfo featureImageInfo = new FeatureImageInfo();
		featureImageInfo.ImagePrefix = "featured_hero";
		featureImageInfo.BackupImage = "featured_hero_00";
		featureImageInfo.ImageCount = 6;
		featureImageInfoDictionary[FeatureImageEnum.Hero] = featureImageInfo;
		Dictionary<FeatureImageEnum, FeatureImageInfo> featureImageInfoDictionary2 = FeatureImageInfoDictionary;
		featureImageInfo = new FeatureImageInfo();
		featureImageInfo.ImagePrefix = "featured_mission";
		featureImageInfo.BackupImage = "featured_mission_00";
		featureImageInfo.ImageCount = 4;
		featureImageInfoDictionary2[FeatureImageEnum.Mission] = featureImageInfo;
		Dictionary<FeatureImageEnum, FeatureImageInfo> featureImageInfoDictionary3 = FeatureImageInfoDictionary;
		featureImageInfo = new FeatureImageInfo();
		featureImageInfo.ImagePrefix = "upsell";
		featureImageInfo.BackupImage = "upsell_00";
		featureImageInfo.ImageCount = 3;
		featureImageInfoDictionary3[FeatureImageEnum.Upsell] = featureImageInfo;
	}

	public static void OnWebAssetCacheInitialized(TransactionMonitor transaction)
	{
		ShsWebAssetCache webAssetCache = AppShell.Instance.WebAssetCache;
		currentTransaction = transaction;
		int dayOfYear = DateTime.Now.DayOfYear;
		foreach (FeatureImageInfo value in FeatureImageInfoDictionary.Values)
		{
			value.CurrentImageIndex = dayOfYear % value.ImageCount + 1;
			webAssetCache.StartRequest(value.CachePath, value.OnWebContentItemLoaded);
		}
	}

	public static void OnFeatureImageLoaded()
	{
		news_asset_current++;
		if (news_asset_current >= 3 && currentTransaction != null)
		{
			currentTransaction.CompleteStep("newscontent");
		}
	}
}
