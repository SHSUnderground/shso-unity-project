using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using UnityEngine;

public class SHSCountersManager
{
	[XmlRoot(ElementName = "counters")]
	public class CounterXml
	{
		[XmlRoot(ElementName = "counter")]
		public class CounterInfo
		{
			public string hero_name;

			public string counter_type;

			public long value;
		}

		[XmlElement("counter")]
		public CounterInfo[] items;
	}

	public delegate void OnCountersLoaded(bool success, string error);

	public const string GENERAL_COUNTER_QUALIFIER = "shsp";

	private bool countersLoaded;

	private AssetBundle effectPrefabBundle;

	private string effectPrefabBundleName;

	private Dictionary<string, ISHSCounterType> countersIdDict;

	private Dictionary<string, ISHSCounterType> countersDict;

	private Dictionary<long, SHSCounterBank> counterBanks;

	private double lastCheckTime;

	private int updateCheckInterval = 5000;

	private SHSCounterBank defaultCounterBank;

	private bool profileIdSync;

	public bool CountersLoaded
	{
		get
		{
			return countersLoaded;
		}
	}

	public AssetBundle EffectPrefabBundle
	{
		get
		{
			return effectPrefabBundle;
		}
	}

	public string EffectPrefabBundleName
	{
		get
		{
			return effectPrefabBundleName;
		}
		set
		{
			effectPrefabBundleName = value;
		}
	}

	public Dictionary<string, ISHSCounterType> CountersId
	{
		get
		{
			return countersIdDict;
		}
	}

	public Dictionary<string, ISHSCounterType> Counters
	{
		get
		{
			return countersDict;
		}
	}

	public Dictionary<long, SHSCounterBank> CounterBanks
	{
		get
		{
			return counterBanks;
		}
	}

	public int UpdateCheckInterval
	{
		get
		{
			return updateCheckInterval;
		}
		set
		{
			updateCheckInterval = value;
		}
	}

	public SHSCounterBank DefaultCounterBank
	{
		get
		{
			return defaultCounterBank;
		}
	}

	public SHSCountersManager()
	{
		countersDict = new Dictionary<string, ISHSCounterType>();
		countersIdDict = new Dictionary<string, ISHSCounterType>();
		counterBanks = new Dictionary<long, SHSCounterBank>();
		lastCheckTime = 0.0;
	}

	public SHSCounterBank CreateCounterBank(long id, bool readOnly)
	{
		if (!countersLoaded)
		{
			CspUtils.DebugLog("Can't create counter bank until counter definitions are loaded.");
			return null;
		}
		if (counterBanks.ContainsKey(id))
		{
			CspUtils.DebugLog("Attempt to create counter bank: " + id + ", but already exists...");
			return null;
		}
		SHSCounterBank sHSCounterBank = new SHSCounterBank(id);
		sHSCounterBank.ReadOnly = readOnly;
		counterBanks[id] = sHSCounterBank;
		foreach (ISHSCounterType value in countersDict.Values)
		{
			value.AddCounterBank(sHSCounterBank);
		}
		return sHSCounterBank;
	}

	public bool RemoveCounterBank(long id)
	{
		SHSCounterBank value;
		if (CounterBanks.TryGetValue(id, out value))
		{
			return RemoveCounterBank(id, value);
		}
		CspUtils.DebugLog("COUNTERBANK: Could not find counter by id: " + id);
		return false;
	}

	public bool RemoveCounterBank(SHSCounterBank bank)
	{
		if (CounterBanks.ContainsKey(bank.Id))
		{
			return RemoveCounterBank(bank.Id, bank);
		}
		CspUtils.DebugLog("COUNTERBANK: Could not find counter by id: " + bank.Id);
		return false;
	}

	public bool RemoveCounterBank(long id, SHSCounterBank bank)
	{
		if (!counterBanks.ContainsKey(id))
		{
			CspUtils.DebugLog("Attempt to create counter bank: " + id + ", but already exists...");
			return false;
		}
		foreach (ISHSCounterType value in countersDict.Values)
		{
			value.RemoveCounterBank(bank);
		}
		counterBanks.Remove(id);
		return true;
	}

	public void LoadConfiguration(DataWarehouse data)
	{
		long num = -1L;
		if (AppShell.Instance.Profile != null)
		{
			num = AppShell.Instance.Profile.UserId;
		}
		defaultCounterBank = new SHSCounterBank(num);
		defaultCounterBank.ReadOnly = false;
		counterBanks[num] = defaultCounterBank;
		foreach (DataWarehouse item in data.GetIterator("//counters/counter"))
		{
			ISHSCounterType iSHSCounterType = new SHSCounterType(null, item, this);
			if (countersDict.ContainsKey(iSHSCounterType.Name))
			{
				CspUtils.DebugLog("Duplicate Counter Type detected when loading configuration file: " + iSHSCounterType.Name);
			}
			else
			{
				countersDict.Add(iSHSCounterType.Name, iSHSCounterType);
			}
		}
		foreach (ISHSCounterType value in countersDict.Values)
		{
			value.AddCounterBank(defaultCounterBank);
		}
		if (!string.IsNullOrEmpty(effectPrefabBundleName))
		{
			AppShell.Instance.BundleLoader.FetchAssetBundle(effectPrefabBundleName, delegate(AssetBundleLoadResponse response, object extraData)
			{
				if (response.Error != null)
				{
					CspUtils.DebugLog("Can't load counter effects bundle: " + response.Error);
				}
				else
				{
					effectPrefabBundle = response.Bundle;
				}
			}, null, false);
		}
	}

	public void LoadAllPersisted(string userId, OnCountersLoaded callback)
	{
		AppShell.Instance.WebService.StartRequest("resources$users/" + userId + "/counters", delegate(ShsWebResponse response)
		{
			ProcessCounters(response, callback);
		}, null, ShsWebService.ShsWebServiceType.RASP);
	}

	public SHSCounterBank ProcessCounters(long remotePlayerId, List<RemotePlayerProfileJsonCounterData> jsonData, OnCountersLoaded callback)
	{
		SHSCounterBank sHSCounterBank = CreateCounterBank(remotePlayerId, true);
		foreach (RemotePlayerProfileJsonCounterData jsonDatum in jsonData)
		{
			ISHSCounterType value = null;
			if (countersIdDict.TryGetValue(jsonDatum.type.ToString(), out value))
			{
				value.InitCurrentValue(sHSCounterBank, jsonDatum.hero, jsonDatum.n);
			}
			else
			{
				CspUtils.DebugLog("Unknown counter " + jsonDatum.type + " received from server.");
			}
		}
		if (callback != null)
		{
			callback(true, string.Empty);
		}
		return sHSCounterBank;
	}

	protected void ProcessCounters(ShsWebResponse response, OnCountersLoaded callback)
	{
		HttpStatusCode status = (HttpStatusCode)response.Status;
		HttpStatusCode httpStatusCode = status;
		if (httpStatusCode == HttpStatusCode.OK)
		{
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(CounterXml));
				CounterXml counterXml = (CounterXml)xmlSerializer.Deserialize(new StringReader(response.Body));
				if (counterXml.items != null)
				{
					CounterXml.CounterInfo[] items = counterXml.items;
					foreach (CounterXml.CounterInfo counterInfo in items)
					{
						ISHSCounterType value = null;
						if (countersIdDict.TryGetValue(counterInfo.counter_type, out value))
						{
							value.InitCurrentValue(DefaultCounterBank, counterInfo.hero_name, counterInfo.value);
						}
						else
						{
							CspUtils.DebugLog("Unknown counter " + counterInfo.counter_type + " received from server.");
						}
					}
				}
				countersLoaded = true;
				AppShell.Instance.EventMgr.Fire(null, new UserCounterDataLoadedMessage());
				if (callback != null)
				{
					callback(true, null);
				}
			}
			catch (Exception message)
			{
				CspUtils.DebugLog(message);
				if (callback != null)
				{
					callback(false, "error parsing counters");
				}
			}
		}
		else if (callback != null)
		{
			callback(false, status.ToString());
		}
	}

	public void AddCounter(string counterType)
	{
		AddCounter(DefaultCounterBank, counterType);
	}

	public void AddCounter(SHSCounterBank bank, string counterType)
	{
		AddCounter(bank, counterType, "shsp");
	}

	public void AddCounter(string counterType, string qualifierKey)
	{
		AddCounter(DefaultCounterBank, counterType, qualifierKey, true);
	}

	public void AddCounter(SHSCounterBank bank, string counterType, string qualifierKey)
	{
		AddCounter(bank, counterType, qualifierKey, true);
	}

	public void AddCounter(string counterType, string qualifierKey, bool reportEach)
	{
		AddCounter(DefaultCounterBank, counterType, qualifierKey, 1L, reportEach);
	}

	public void AddCounter(SHSCounterBank bank, string counterType, string qualifierKey, bool reportEach)
	{
		if (reportEach)
		{
			AddCounter(bank, counterType, qualifierKey, 1L, true);
		}
		else
		{
			AddCounter(bank, counterType, qualifierKey, 1L, false);
		}
	}

	public void AddCounter(string counterType, long quantity)
	{
		AddCounter(DefaultCounterBank, counterType, "shsp", quantity);
	}

	public void AddCounter(SHSCounterBank bank, string counterType, long quantity)
	{
		AddCounter(bank, counterType, "shsp", quantity);
	}

	public void AddCounter(string counterType, string qualifierKey, long quantity)
	{
		AddCounter(DefaultCounterBank, counterType, qualifierKey, quantity, true);
	}

	public void AddCounter(SHSCounterBank bank, string counterType, string qualifierKey, long quantity)
	{
		AddCounter(bank, counterType, qualifierKey, quantity, true);
	}

	public void AddCounter(string counterType, string qualifierKey, long quantity, bool reportEach)
	{
		AddCounter(DefaultCounterBank, counterType, qualifierKey, quantity, reportEach);
	}

	public void AddCounter(SHSCounterBank bank, string counterType, string qualifierKey, long quantity, bool reportEach)
	{
		ISHSCounterType counterType2 = getCounterType(counterType);
		if (counterType2 != null)
		{
			if (reportEach)
			{
				counterType2.AddCounter(bank, qualifierKey, quantity);
			}
			else
			{
				counterType2.AddCounter(bank, qualifierKey, quantity, false);
			}
		}
	}

	private ISHSCounterType getCounterType(string CounterTypePath)
	{
		string[] array = CounterTypePath.ToUpper().Split('.');
		if (array.Length == 0)
		{
			CspUtils.DebugLog("counter type: " + CounterTypePath + " not valid.");
			return null;
		}
		if (!countersDict.ContainsKey(array[0]))
		{
			return null;
		}
		ISHSCounterType iSHSCounterType = countersDict[array[0]];
		if (array.Length == 1)
		{
			return iSHSCounterType;
		}
		for (int i = 1; i < array.Length; i++)
		{
			if (!iSHSCounterType.SubCounters.ContainsKey(array[i]))
			{
				CspUtils.DebugLog("Sub Counter: " + array[i] + " doesn't exist for counter: " + iSHSCounterType.Name);
				return null;
			}
			iSHSCounterType = iSHSCounterType.SubCounters[array[i]];
		}
		return iSHSCounterType;
	}

	public ISHSCounterType GetCounter(string counterType)
	{
		return getCounterType(counterType);
	}

	public long GetCounters(string counterType)
	{
		return GetCounters(DefaultCounterBank, counterType);
	}

	public long GetCounters(SHSCounterBank bank, string counterType)
	{
		ISHSCounterType counterType2 = getCounterType(counterType);
		if (counterType2 != null)
		{
			return counterType2.GetCurrentValue(bank);
		}
		return 0L;
	}

	public void Reset()
	{
		foreach (ISHSCounterType value in countersDict.Values)
		{
			value.Reset();
		}
	}

	public void Update()
	{
		if (!profileIdSync)
		{
			if (defaultCounterBank != null && defaultCounterBank.Id != -1)
			{
				profileIdSync = true;
				return;
			}
			if (AppShell.Instance.Profile != null && defaultCounterBank != null)
			{
				long userId = AppShell.Instance.Profile.UserId;
				SHSCounterBank sHSCounterBank = defaultCounterBank;
				sHSCounterBank.Id = userId;
				counterBanks.Remove(-1L);
				counterBanks[userId] = sHSCounterBank;
				profileIdSync = true;
			}
		}
		double time = ServerTime.time;
		if (!(time - lastCheckTime < (double)updateCheckInterval))
		{
			lastCheckTime = time;
			foreach (ISHSCounterType value in countersDict.Values)
			{
				value.Update(time);
			}
		}
	}
}
