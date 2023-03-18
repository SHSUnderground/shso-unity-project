using UnityEngine;

namespace ShsAudio
{
	public class Helpers
	{
		private class WrappedLoadObj
		{
			public string audioBundle;

			public string prefabName;

			public Definitions.BundlePrefabLoaded onLoaded;

			public object userData;
		}

		public static bool VOBundlesDownloaded
		{
			get
			{
				return AppShell.Instance.BundleLoader.GetBundleGroupDone(AssetBundleLoader.BundleGroup.VO);
			}
		}

		public static int GetClipIndex(string assetName)
		{
			int num = assetName.LastIndexOf('_');
			if (num > 0 && num != assetName.Length - 1)
			{
				int result = -1;
				string s = assetName.Substring(num + 1);
				if (int.TryParse(s, out result))
				{
					return result;
				}
				return -1;
			}
			return -1;
		}

		public static string GetClipBase(string assetName)
		{
			if (GetClipIndex(assetName) >= 0)
			{
				int length = assetName.LastIndexOf('_');
				return assetName.Substring(0, length);
			}
			return assetName;
		}

		public static LocalizationStrategy GetLocalizationStrategy(AudioAssetType assetType)
		{
			LocalizationStrategy result = LocalizationStrategy.UseLocale;
			if (assetType == AudioAssetType.TutorialVO && Configuration.UseEnglishOnlyTutorialVO)
			{
				result = LocalizationStrategy.EnglishOnly;
			}
			else if (assetType == AudioAssetType.FlavorVO && Configuration.UseEnglishOnlyFlavorVO)
			{
				result = LocalizationStrategy.EnglishOnly;
			}
			return result;
		}

		public static string GetAudioBundleName(SABundle bundle)
		{
			return GetAudioBundleName(bundle, LocalizationStrategy.UseLocale);
		}

		public static string GetAudioBundleName(SABundle bundle, LocalizationStrategy strategy)
		{
			return GetAudioBundleName(bundle.ToString(), strategy);
		}

		public static string GetAudioBundleName(string bundle)
		{
			return GetAudioBundleName(bundle, LocalizationStrategy.UseLocale);
		}

		public static string GetAudioBundleName(string bundle, LocalizationStrategy strategy)
		{
			return WrapAudioBundleName(bundle.ToLower());
		}

		public static string InsertLocale(string bundle, string locale)
		{
			return bundle.Insert(bundle.LastIndexOf('/') + 1, locale + "_");
		}

		public static string WrapAudioBundleName(string bundle)
		{
			return "Audio/" + bundle.ToLower() + "_audio";
		}

		public static string ExtractBundleRoot(string bundle, LocalizationStrategy strategy)
		{
			return UnwrapAudioBundleName(bundle).ToLower();
		}

		public static string UnwrapAudioBundleName(string bundle)
		{
			string text = "_audio";
			if (!bundle.StartsWith("Audio/") || !bundle.EndsWith(text))
			{
				return bundle;
			}
			int num = bundle.Length - text.Length;
			int length = "Audio/".Length;
			return bundle.Substring(length, num - length);
		}

		public static SABundledAsset ResolveBundledAsset(SABundle bundle, string prefabNameOrKey, LocalizationStrategy strategy)
		{
			SABundledAsset bundledAsset = null;
			if (bundle == SABundle.EXTERNALLY_DEFINED)
			{
				TaggedAudioBundleEntry value;
				if (Definitions.TaggedAudioReferences.TryGetValue(prefabNameOrKey, out value))
				{
					bundledAsset = new SABundledAsset();
					bundledAsset.bundle = GetAudioBundleName(value.BundleName, strategy);
					bundledAsset.asset = value.AssetName;
				}
				else
				{
					CspUtils.DebugLog("No tagged audio asset found with name <" + prefabNameOrKey + ">");
				}
			}
			else
			{
				bundledAsset = ResolveBundledAsset(bundle.ToString(), prefabNameOrKey, strategy);
			}
			return bundledAsset;
		}

		public static SABundledAsset ResolveBundledAsset(string bundle, string prefabName, LocalizationStrategy strategy)
		{
			SABundledAsset bundledAsset = new SABundledAsset();
			bundledAsset.bundle = GetAudioBundleName(bundle, strategy);
			bundledAsset.asset = prefabName;
			return bundledAsset;
		}

		public static void LoadPrefabFromBundle(SABundle bundle, string prefabNameOrKey, Definitions.BundlePrefabLoaded onLoaded, object extraData)
		{
			LoadPrefabFromBundle(bundle, prefabNameOrKey, onLoaded, LocalizationStrategy.UseLocale, extraData);
		}

		public static void LoadPrefabFromBundle(SABundle bundle, string prefabNameOrKey, Definitions.BundlePrefabLoaded onLoaded, LocalizationStrategy strategy, object extraData)
		{
			SABundledAsset bundledAsset = ResolveBundledAsset(bundle, prefabNameOrKey, strategy);
			if (bundledAsset != null)
			{
				WrappedLoadObj wrappedLoadObj = new WrappedLoadObj();
				wrappedLoadObj.audioBundle = bundledAsset.bundle;
				wrappedLoadObj.prefabName = bundledAsset.asset;
				wrappedLoadObj.onLoaded = onLoaded;
				wrappedLoadObj.userData = extraData;
				AppShell.Instance.BundleLoader.LoadAsset(wrappedLoadObj.audioBundle, wrappedLoadObj.prefabName, wrappedLoadObj, OnBundlePrefabLoaded);
			}
			else if (onLoaded != null)
			{
				onLoaded(null, extraData);
			}
		}

		private static void OnBundlePrefabLoaded(Object obj, AssetBundle bundle, object extraData)
		{
			WrappedLoadObj wrappedLoadObj = extraData as WrappedLoadObj;
			GameObject audioPrefab = obj as GameObject;
			if (obj == null)
			{
				CspUtils.DebugLog("Failed to load audio prefab <" + wrappedLoadObj.prefabName + "> from bundle <" + wrappedLoadObj.audioBundle + ">");
			}
			else if (wrappedLoadObj.onLoaded != null)
			{
				wrappedLoadObj.onLoaded(audioPrefab, wrappedLoadObj.userData);
			}
		}
	}
}
