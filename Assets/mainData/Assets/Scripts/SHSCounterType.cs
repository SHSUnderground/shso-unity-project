using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHSCounterType : ISHSCounterType
{
	public enum EffectTypeEnum
	{
		EffectSequence,
		Behavior
	}

	public enum CounterUpdateVisibilityEnum
	{
		Silent,
		Local,
		Public
	}

	public enum ReportingMethodEnum
	{
		RoomNotification,
		WebService,
		None
	}

	private SHSCountersManager manager;

	private Dictionary<SHSCounterBank, Dictionary<string, long>> qualifierValues;

	private string name;

	private string id;

	private CounterCycleTypeEnum cycleType;

	private MaxAccumTypeEnum maxAccumType;

	private Dictionary<string, ISHSCounterType> subCounters;

	private long maxAccumulationPerCycle;

	private string cycleCounter;

	private ISHSCounterType parent;

	private CounterUpdateVisibilityEnum updateVisibility;

	private string effectName;

	private EffectTypeEnum effectType;

	public Dictionary<SHSCounterBank, Dictionary<string, long>> QualifierValues
	{
		get
		{
			return qualifierValues;
		}
	}

	public string Name
	{
		get
		{
			return name;
		}
	}

	public string Path
	{
		get
		{
			string text = name;
			for (ISHSCounterType iSHSCounterType = parent; iSHSCounterType != null; iSHSCounterType = iSHSCounterType.Parent)
			{
				text = iSHSCounterType.Name + "." + text;
			}
			return text;
		}
	}

	public string Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public CounterCycleTypeEnum CycleType
	{
		get
		{
			return cycleType;
		}
	}

	public MaxAccumTypeEnum MaxAccumType
	{
		get
		{
			return maxAccumType;
		}
	}

	public Dictionary<string, ISHSCounterType> SubCounters
	{
		get
		{
			return subCounters;
		}
	}

	public long MaxAccumulationPerCycle
	{
		get
		{
			return maxAccumulationPerCycle;
		}
	}

	public string CycleCounter
	{
		get
		{
			return cycleCounter;
		}
	}

	public ISHSCounterType Parent
	{
		get
		{
			return parent;
		}
	}

	public CounterUpdateVisibilityEnum UpdateVisibility
	{
		get
		{
			return updateVisibility;
		}
		set
		{
			updateVisibility = value;
		}
	}

	public string EffectName
	{
		get
		{
			return effectName;
		}
	}

	public EffectTypeEnum EffectType
	{
		get
		{
			return effectType;
		}
	}

	public SHSCounterType(ISHSCounterType Parent, DataWarehouse data, SHSCountersManager mgr)
	{
		subCounters = new Dictionary<string, ISHSCounterType>();
		parent = Parent;
		manager = mgr;
		maxAccumulationPerCycle = long.MaxValue;
		qualifierValues = new Dictionary<SHSCounterBank, Dictionary<string, long>>();
		Configure(data);
	}

	public void AddCounterBank(SHSCounterBank bank)
	{
		if (qualifierValues.ContainsKey(bank))
		{
			CspUtils.DebugLog("Counter type: " + name + " already has a bank " + bank.Id);
			return;
		}
		qualifierValues[bank] = new Dictionary<string, long>();
		foreach (ISHSCounterType value in subCounters.Values)
		{
			value.AddCounterBank(bank);
		}
	}

	public void RemoveCounterBank(SHSCounterBank bank)
	{
		if (!qualifierValues.ContainsKey(bank))
		{
			CspUtils.DebugLog("Counter type: " + name + " does not have aa bank " + bank.Id);
			return;
		}
		qualifierValues.Remove(bank);
		foreach (ISHSCounterType value in subCounters.Values)
		{
			value.RemoveCounterBank(bank);
		}
	}

	public long GetCurrentValue()
	{
		return GetCurrentValue(manager.DefaultCounterBank, "shsp");
	}

	public long GetCurrentValue(SHSCounterBank bank)
	{
		return GetCurrentValue(bank, "shsp");
	}

	public long GetCurrentValue(string qualifierKey)
	{
		return GetCurrentValue(manager.DefaultCounterBank, qualifierKey);
	}

	public long GetCurrentValue(SHSCounterBank bank, string qualifierKey)
	{
		long value;
		if (!qualifierValues[bank].TryGetValue(qualifierKey, out value))
		{
			return 0L;
		}
		return value;
	}

	public void InitCurrentValue(long value)
	{
		InitCurrentValue(manager.DefaultCounterBank, "shsp", value);
	}

	public void InitCurrentValue(SHSCounterBank bank, long value)
	{
		InitCurrentValue(bank, "shsp", value);
	}

	public void InitCurrentValue(string qualifierKey, long value)
	{
		qualifierValues[manager.DefaultCounterBank][qualifierKey] = value;
	}

	public void InitCurrentValue(SHSCounterBank bank, string qualifierKey, long value)
	{
		qualifierValues[bank][qualifierKey] = value;
	}

	public void Configure(DataWarehouse data)
	{
		name = data.GetString("name").ToUpper();
		id = data.GetString("id");
		if (string.IsNullOrEmpty(id))
		{
			CspUtils.DebugLog("Counter " + name + " is missing an id");
		}
		if (!manager.CountersId.ContainsKey(id))
		{
			manager.CountersId.Add(id, this);
		}
		else
		{
			CspUtils.DebugLog("Duplicated counter id " + name + " , " + id);
		}
		cycleType = (CounterCycleTypeEnum)(int)Enum.Parse(typeof(CounterCycleTypeEnum), data.GetString("cycletype"));
		if (cycleType != CounterCycleTypeEnum.Infinite && cycleType != CounterCycleTypeEnum.Inherit)
		{
			cycleCounter = data.GetString("cyclecounter");
		}
		maxAccumType = (MaxAccumTypeEnum)(int)Enum.Parse(typeof(MaxAccumTypeEnum), data.GetString("accumcapmethod"));
		if (maxAccumType == MaxAccumTypeEnum.Fixed)
		{
			maxAccumulationPerCycle = data.TryGetLong("maxaccumpercycle", long.MaxValue);
		}
		updateVisibility = (CounterUpdateVisibilityEnum)(int)Enum.Parse(typeof(CounterUpdateVisibilityEnum), data.GetString("visibility"));
		string @string = data.GetString("effect");
		if (string.IsNullOrEmpty(@string))
		{
			if (updateVisibility != 0)
			{
				CspUtils.DebugLog("Counter award set to be visible, but no effect supplied.");
			}
		}
		else
		{
			effectName = @string;
			effectType = (EffectTypeEnum)(int)Enum.Parse(typeof(EffectTypeEnum), data.GetString("effectType"));
		}
		foreach (DataWarehouse item in data.GetIterator("subcounters/subcounter"))
		{
			ISHSCounterType iSHSCounterType = new SHSCounterType(this, item, manager);
			subCounters[iSHSCounterType.Name] = iSHSCounterType;
		}
	}

	public bool AddCounter()
	{
		return AddCounter(manager.DefaultCounterBank, "shsp", 1L);
	}

	public bool AddCounter(SHSCounterBank bank)
	{
		return AddCounter(bank, "shsp", 1L);
	}

	public bool AddCounter(string qualifierKey)
	{
		return AddCounter(manager.DefaultCounterBank, qualifierKey, 1L);
	}

	public bool AddCounter(SHSCounterBank bank, string qualifierKey)
	{
		return AddCounter(bank, qualifierKey, 1L);
	}

	public bool AddCounter(long Count)
	{
		return AddCounter(manager.DefaultCounterBank, "shsp", Count);
	}

	public bool AddCounter(SHSCounterBank bank, long Count)
	{
		return AddCounter(bank, "shsp", Count);
	}

	public bool AddCounter(string qualifierKey, long Count)
	{
		return AddCounter(manager.DefaultCounterBank, qualifierKey, Count, true);
	}

	public bool AddCounter(SHSCounterBank bank, string qualifierKey, long Count)
	{
		return AddCounter(bank, qualifierKey, Count, true);
	}

	public bool AddCounter(string qualifierKey, long Count, bool reportEach)
	{
		return AddCounter(manager.DefaultCounterBank, qualifierKey, Count, reportEach);
	}

	public bool AddCounter(SHSCounterBank bank, string qualifierKey, long Count, bool reportEach)
	{
		if (bank.ReadOnly)
		{
			CspUtils.DebugLog(string.Format("Can't update ReadOnly counter bank {0}", bank.Id));
			return false;
		}
		long num = 0L;
		if (qualifierValues[bank].ContainsKey(qualifierKey))
		{
			num = qualifierValues[bank][qualifierKey];
		}
		if (num + Count > maxAccumulationPerCycle)
		{
			CspUtils.DebugLog("Maximum accumulation reached for: " + name + " until next cycle.");
			return false;
		}
		qualifierValues[bank][qualifierKey] = num + Count;
		sendEventNotification(qualifierKey, num);
		if (reportEach)
		{
			ReportCounterTypeValues(bank, qualifierKey);
		}
		if (updateVisibility != 0)
		{
			GameObject localPlayer = GameController.GetController().LocalPlayer;
			if (localPlayer != null)
			{
				PlayEffect(localPlayer);
			}
			else
			{
				CspUtils.DebugLog("No current player to play effect on... This is not normal (though in future could be, in which case, annihilate this log message)");
			}
		}
		return true;
	}

	public bool SetCounter(long Count)
	{
		return SetCounter(manager.DefaultCounterBank, "shsp", Count);
	}

	public bool SetCounter(SHSCounterBank bank, long Count)
	{
		return SetCounter(bank, "shsp", Count);
	}

	public bool SetCounter(long Count, ReportingMethodEnum reportingMethod)
	{
		return SetCounter(manager.DefaultCounterBank, "shsp", Count, reportingMethod);
	}

	public bool SetCounter(SHSCounterBank bank, long Count, ReportingMethodEnum reportingMethod)
	{
		return SetCounter(bank, "shsp", Count, reportingMethod);
	}

	public bool SetCounter(string qualifierKey, long Count)
	{
		return SetCounter(manager.DefaultCounterBank, qualifierKey, Count, ReportingMethodEnum.RoomNotification);
	}

	public bool SetCounter(SHSCounterBank bank, string qualifierKey, long Count)
	{
		return SetCounter(bank, qualifierKey, Count, ReportingMethodEnum.RoomNotification);
	}

	public bool SetCounter(string qualifierKey, long Count, ReportingMethodEnum reportingMethod)
	{
		return SetCounter(manager.DefaultCounterBank, qualifierKey, Count, reportingMethod);
	}

	public bool SetCounter(SHSCounterBank bank, string qualifierKey, long Count, ReportingMethodEnum reportingMethod)
	{
		if (bank.ReadOnly)
		{
			CspUtils.DebugLog(string.Format("Can't set ReadOnly counter bank {0}", bank.Id));
			return false;
		}
		if (Count > maxAccumulationPerCycle)
		{
			CspUtils.DebugLog("Maximum accumulation reached for: " + name + " until next cycle.");
			return false;
		}
		long value = 0L;
		qualifierValues[bank].TryGetValue(qualifierKey, out value);
		qualifierValues[bank][qualifierKey] = Count;
		sendEventNotification(qualifierKey, value);
		ReportCounterTypeValues(bank, qualifierKey, reportingMethod);
		return true;
	}

	public void Update(double Time)
	{
	}

	protected void PlayEffect(GameObject currentPlayer)
	{
		if (manager.EffectPrefabBundle != null)
		{
			UnityEngine.Object @object = manager.EffectPrefabBundle.Load(effectName);
			if (@object != null)
			{
				GameObject g = UnityEngine.Object.Instantiate(@object) as GameObject;
				EffectSequence component = Utils.GetComponent<EffectSequence>(g);
				if (!(component != null))
				{
					return;
				}
				if (effectType == EffectTypeEnum.Behavior)
				{
					BehaviorManager behaviorManager = currentPlayer.GetComponent(typeof(BehaviorManager)) as BehaviorManager;
					if (behaviorManager != null)
					{
						BehaviorEffectSequence behaviorEffectSequence = behaviorManager.requestChangeBehavior(typeof(BehaviorEffectSequence), true) as BehaviorEffectSequence;
						if (behaviorEffectSequence != null)
						{
							component.SetParent(currentPlayer);
							behaviorEffectSequence.Initialize(component, null);
						}
						else
						{
							UnityEngine.Object.Destroy(component);
							CspUtils.DebugLog("Nope, effect won't take. destroying.");
						}
					}
					else
					{
						CspUtils.DebugLog("No behavior manager to play counter effect for.");
					}
				}
				else if (effectType == EffectTypeEnum.EffectSequence)
				{
					component.Initialize(currentPlayer, null, null);
					component.StartSequence();
				}
			}
			else
			{
				CspUtils.DebugLog("Couldn't load prefab: " + effectName);
			}
		}
		else
		{
			CspUtils.DebugLog("Object of :" + effectName + " does not have an effect sequence component.");
		}
	}

	public void Reset()
	{
		foreach (SHSCounterBank key in qualifierValues.Keys)
		{
			List<string> list = new List<string>(qualifierValues[key].Keys);
			foreach (string item in list)
			{
				qualifierValues[key][item] = 0L;
				ReportCounterTypeValues(key, item);
			}
		}
		foreach (ISHSCounterType value in subCounters.Values)
		{
			value.Reset();
		}
	}

	private void ReportCounterTypeValues(SHSCounterBank bank)
	{
		ReportCounterTypeValues(bank, "shsp", ReportingMethodEnum.RoomNotification);
	}

	private void ReportCounterTypeValues(SHSCounterBank bank, string qualifierKey)
	{
		ReportCounterTypeValues(bank, qualifierKey, ReportingMethodEnum.RoomNotification);
	}

	private void ReportCounterTypeValues(SHSCounterBank bank, string qualifierKey, ReportingMethodEnum reportingMethod)
	{
		if (bank.ReadOnly)
		{
			CspUtils.DebugLog(string.Format("Can't report ReadOnly counter bank {0}", bank.Id));
			return;
		}
		switch (reportingMethod)
		{
		case ReportingMethodEnum.RoomNotification:
		{
			Hashtable hashtable = new Hashtable();
			hashtable["id"] = Id;
			hashtable["v"] = qualifierValues[bank][qualifierKey].ToString();
			hashtable["h"] = qualifierKey;
			AppShell.Instance.ServerConnection.ReportEvent("set_counter", hashtable);
			break;
		}
		case ReportingMethodEnum.WebService:
		{
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("hero_name", qualifierKey);
			wWWForm.AddField("counter", Id);
			wWWForm.AddField("amount", qualifierValues[bank][qualifierKey].ToString());
			AppShell.Instance.WebService.StartRequest("resources$users/counters_set.py", delegate
			{
			}, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);
			break;
		}
		}
	}

	private void sendEventNotification(string qualifierKey, long prevValue)
	{
		AppShell.Instance.EventMgr.Fire(this, new CounterUpdatedMessage(name, Path, prevValue, qualifierValues[manager.DefaultCounterBank][qualifierKey], qualifierKey, this));
	}
}
