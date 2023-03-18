using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class StringTable
{
	protected class ExtraData
	{
		public TransactionMonitor transaction;

		public int bundleIdx;

		public ExtraData(TransactionMonitor transaction, int bundleIdx)
		{
			this.transaction = transaction;
			this.bundleIdx = bundleIdx;
		}
	}

	[Serializable]
	[XmlType("strings", AnonymousType = true)]
	[XmlRoot(Namespace = "", IsNullable = false)]
	public class StringArray
	{
		private StringEntry[] stringField;

		[XmlElement("string")]
		public StringEntry[] Items
		{
			get
			{
				return stringField;
			}
			set
			{
				stringField = value;
			}
		}
	}

	[Serializable]
	public class StringEntry
	{
		private string idField;

		private string valueField;

		[XmlElement]
		public string value
		{
			get
			{
				return valueField;
			}
			set
			{
				valueField = value;
			}
		}

		[XmlAttribute]
		public string id
		{
			get
			{
				return idField;
			}
			set
			{
				idField = value;
			}
		}
	}

	public delegate void OnStringsLoaded(bool success, string error);

	public const char LocalizedStringPrefix = '#';

	protected Dictionary<string, string> strings;

	protected AssetBundle[] bundles;

	protected TransactionMonitor loadTransaction;

	protected OnStringsLoaded onLoaded;

	public string this[string id]
	{
		get
		{
			return GetString(id);
		}
	}

	public Dictionary<string, string> Entries
	{
		get
		{
			return strings;
		}
	}

	public StringTable()
	{
		strings = new Dictionary<string, string>();
		loadTransaction = null;
		onLoaded = null;
	}

	public string GetString(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return string.Empty;
		}
		if (id[0] != '#')
		{
			return id;
		}
		string value;
		if (strings.TryGetValue(id.ToUpperInvariant(), out value))
		{
			return value;
		}
		return id;
	}

	public void Load(string locale, OnStringsLoaded onLoadedCallback)
	{
		if (loadTransaction != null)
		{
			loadTransaction = null;
			if (bundles.Length > 0)
			{
				bundles = null;
			}
		}
		if (onLoaded != null)
		{
			onLoaded = null;
		}
		strings.Clear();
		onLoaded = onLoadedCallback;
		string[] array = LocaleMapper.LocaleToDirectories(locale, false);
		bundles = new AssetBundle[array.Length];
		loadTransaction = TransactionMonitor.CreateTransactionMonitor("StringTable_loadTransaction", OnStringBundlesLoaded, 120f, null);
		for (int i = 0; i < array.Length; i++)
		{
			string name = "load_" + i.ToString();
			string bundlePath = array[i] + locale.ToLower() + "_strings";
			loadTransaction.AddStep(name);
			loadTransaction.AddStepBundle(name, bundlePath);
			AppShell.Instance.BundleLoader.FetchAssetBundle(bundlePath, OnAssetBundleLoaded, new ExtraData(loadTransaction, i));
		}
	}

	protected void OnAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		ExtraData extraData2 = (ExtraData)extraData;
		if (loadTransaction == extraData2.transaction)
		{
			if (response.Error != null)
			{
				CspUtils.DebugLog("Error loading string table " + response.Path + ": " + response.Error);
			}
			bundles[extraData2.bundleIdx] = response.Bundle;
			loadTransaction.CompleteStep("load_" + extraData2.bundleIdx);
		}
	}

	protected void OnStringBundlesLoaded(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		loadTransaction = null;
		if (exit == TransactionMonitor.ExitCondition.Success && ProcessAssetBundles(out error))
		{
			if (onLoaded != null)
			{
				onLoaded(true, null);
			}
		}
		else if (onLoaded != null)
		{
			onLoaded(false, error);
		}
	}

	protected bool ProcessAssetBundles(out string error)
	{
		for (int i = 0; i < bundles.Length; i++)
		{
			if (!(bundles[i] == null))
			{
				TextAsset textAsset = bundles[i].Load("strings", typeof(TextAsset)) as TextAsset;
				if (!(textAsset == null))
				{
					using (TextReader textReader = new StringReader(textAsset.text))
					{
						XmlSerializer xmlSerializer = new XmlSerializer(typeof(StringArray));
						StringArray stringArray = xmlSerializer.Deserialize(textReader) as StringArray;
						StringEntry[] items = stringArray.Items;
						foreach (StringEntry stringEntry in items)
						{
							if (!strings.ContainsKey(stringEntry.id))
							{
								strings.Add(stringEntry.id, stringEntry.value);
							}
						}
					}
				}
			}
		}
		error = null;
		return true;
	}
}
