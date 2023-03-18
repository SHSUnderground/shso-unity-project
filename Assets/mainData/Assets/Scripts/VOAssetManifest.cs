using System.Collections;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

public class VOAssetManifest : Singleton<VOAssetManifest>
{
	[XmlRoot("vo-asset-manifest")]
	public class VOAssetManifestDefinition : StaticDataDefinition, IStaticDataDefinitionTxt
	{
		[XmlRoot("vo-asset")]
		public class VOAsset
		{
			[CompilerGenerated]
			private string _003CBundle_003Ek__BackingField;

			[CompilerGenerated]
			private string _003CAsset_003Ek__BackingField;

			[XmlAttribute("bundle")]
			public string Bundle
			{
				[CompilerGenerated]
				get
				{
					return _003CBundle_003Ek__BackingField;
				}
				[CompilerGenerated]
				set
				{
					_003CBundle_003Ek__BackingField = value;
				}
			}

			[XmlAttribute("asset")]
			public string Asset
			{
				[CompilerGenerated]
				get
				{
					return _003CAsset_003Ek__BackingField;
				}
				[CompilerGenerated]
				set
				{
					_003CAsset_003Ek__BackingField = value;
				}
			}

			public string CombinedName
			{
				get
				{
					return Bundle + "|" + Asset;
				}
			}
		}

		[XmlRoot("vo-alias")]
		public class VOAlias : VOAsset
		{
			[CompilerGenerated]
			private string _003CBundleReference_003Ek__BackingField;

			[CompilerGenerated]
			private string _003CAssetReference_003Ek__BackingField;

			[XmlAttribute("bundle-ref")]
			public string BundleReference
			{
				[CompilerGenerated]
				get
				{
					return _003CBundleReference_003Ek__BackingField;
				}
				[CompilerGenerated]
				set
				{
					_003CBundleReference_003Ek__BackingField = value;
				}
			}

			[XmlAttribute("asset-ref")]
			public string AssetReference
			{
				[CompilerGenerated]
				get
				{
					return _003CAssetReference_003Ek__BackingField;
				}
				[CompilerGenerated]
				set
				{
					_003CAssetReference_003Ek__BackingField = value;
				}
			}

			public string CombinedReferenceName
			{
				get
				{
					return BundleReference + "|" + AssetReference;
				}
			}
		}

		[CompilerGenerated]
		private VOAsset[] _003CAssets_003Ek__BackingField;

		[CompilerGenerated]
		private VOAlias[] _003CAliases_003Ek__BackingField;

		[XmlArray("vo-assets")]
		[XmlArrayItem("vo-asset")]
		public VOAsset[] Assets
		{
			[CompilerGenerated]
			get
			{
				return _003CAssets_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CAssets_003Ek__BackingField = value;
			}
		}

		[XmlArray("vo-aliases")]
		[XmlArrayItem("vo-alias")]
		public VOAlias[] Aliases
		{
			[CompilerGenerated]
			get
			{
				return _003CAliases_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CAliases_003Ek__BackingField = value;
			}
		}

		public void InitializeFromData(string xml)
		{
			VOAssetManifestDefinition vOAssetManifestDefinition = Utils.XmlDeserialize<VOAssetManifestDefinition>(xml);
			Assets = vOAssetManifestDefinition.Assets;
			Aliases = vOAssetManifestDefinition.Aliases;
		}
	}

	public const string VO_ASSET_MANIFEST_PATH = "Audio/vo_assets";

	protected static VOAssetManifestDefinition voAssetManifestData = new VOAssetManifestDefinition();

	protected TransactionMonitor loadTransaction;

	[CompilerGenerated]
	private bool _003CDataIsLoaded_003Ek__BackingField;

	[CompilerGenerated]
	private Hashtable _003CVOAssets_003Ek__BackingField;

	public bool DataIsLoaded
	{
		[CompilerGenerated]
		get
		{
			return _003CDataIsLoaded_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CDataIsLoaded_003Ek__BackingField = value;
		}
	}

	public Hashtable VOAssets
	{
		[CompilerGenerated]
		get
		{
			return _003CVOAssets_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CVOAssets_003Ek__BackingField = value;
		}
	}

	public VOAssetManifest()
	{
		DataIsLoaded = false;
		VOAssets = new Hashtable();
	}

	public bool DoesVOAssetExist(string bundle, string asset)
	{
		return GetVOAsset(bundle, asset) != null;
	}

	public BundledAsset GetVOAsset(string bundle, string asset)
	{
		string text = bundle + "|" + asset;
		string key = text + "_audio";
		if (!DataIsLoaded)
		{
			CspUtils.DebugLog("Tried to get VO asset " + text + ", but the VO asset manifest has not yet been loaded.");
			return null;
		}
		if (VOAssets.ContainsKey(key))
		{
			return VOAssets[key] as BundledAsset;
		}
		if (VOAssets.ContainsKey(text))
		{
			return VOAssets[text] as BundledAsset;
		}
		return null;
	}

	public void LoadVOAssetManifestData(TransactionMonitor transactionParent, string stepName)
	{
		loadTransaction = transactionParent;
		if (loadTransaction != null && !string.IsNullOrEmpty(stepName))
		{
			loadTransaction.AddStep(stepName, TransactionMonitor.DumpTransactionStatus);
		}
		AppShell.Instance.DataManager.LoadGameData("Audio/vo_assets", OnVOAssetManifestDataLoaded, voAssetManifestData, stepName);
	}

	protected void OnVOAssetManifestDataLoaded(GameDataLoadResponse response, object extraData)
	{
		string text = extraData as string;
		if (string.IsNullOrEmpty(response.Error))
		{
			VOAssetManifestDefinition vOAssetManifestDefinition = response.DataDefinition as VOAssetManifestDefinition;
			VOAssetManifestDefinition.VOAsset[] assets = vOAssetManifestDefinition.Assets;
			foreach (VOAssetManifestDefinition.VOAsset vOAsset in assets)
			{
				VOAssets[vOAsset.CombinedName] = new BundledAsset(vOAsset.Bundle, vOAsset.Asset);
			}
			VOAssetManifestDefinition.VOAlias[] aliases = vOAssetManifestDefinition.Aliases;
			foreach (VOAssetManifestDefinition.VOAlias vOAlias in aliases)
			{
				VOAssets[vOAlias.CombinedName] = new BundledAsset(vOAlias.BundleReference, vOAlias.AssetReference);
			}
			DataIsLoaded = true;
			if (loadTransaction != null && !string.IsNullOrEmpty(text))
			{
				loadTransaction.CompleteStep(text);
			}
		}
		else if (loadTransaction != null && !string.IsNullOrEmpty(text))
		{
			loadTransaction.FailStep(text, response.Error);
		}
	}
}
