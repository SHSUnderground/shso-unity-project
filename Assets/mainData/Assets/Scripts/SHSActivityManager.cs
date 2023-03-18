using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SHSActivityManager
{
	private AssetBundle effectPrefabBundle;

	private string effectPrefabBundleName = "SocialSpace/gameworld_activity_objects";

	private Dictionary<string, ISHSActivity> activitiesDict;

	private bool activeZoneLoaded;

	private string activeZone = string.Empty;

	private int activeZoneID = -1;

	private bool queuedZoneLoad;

	private bool initialized;

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

	public string ActiveZone
	{
		get
		{
			return activeZone;
		}
	}

	public int ActiveZoneID
	{
		get
		{
			return activeZoneID;
		}
	}

	public bool Initialized
	{
		get
		{
			return initialized;
		}
	}

	public bool HasActivitiesInProgress
	{
		get
		{
			if (activeZoneLoaded)
			{
				foreach (ISHSActivity value in activitiesDict.Values)
				{
					if (value.Result == ActivityResultEnum.InProgress)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public ISHSActivity this[string name]
	{
		get
		{
			string key = name.ToUpper();
			if (activitiesDict.ContainsKey(key))
			{
				return activitiesDict[key];
			}
			return null;
		}
	}

	public SHSActivityManager(SHSCountersManager counterManager)
	{
		activitiesDict = new Dictionary<string, ISHSActivity>();
	}

	public bool RemoveActivity(string activity)
	{
		return activitiesDict.Remove("activity");
	}

	public void Start()
	{
		AppShell.Instance.EventMgr.AddListener<ZoneLoadedMessage>(OnZoneLoaded);
		AppShell.Instance.EventMgr.AddListener<ZoneUnloadedMessage>(OnZoneUnloaded);
		AppShell.Instance.EventMgr.AddListener<AchievementCompleteMessage>(OnAchievementCompleteMessage);
		AppShell.Instance.EventMgr.AddListener<AchievementDataLoadedMessage>(OnAchievementDataLoaded);
		if (!string.IsNullOrEmpty(effectPrefabBundleName))
		{
			AppShell.Instance.BundleLoader.FetchAssetBundle(effectPrefabBundleName, delegate(AssetBundleLoadResponse response, object extraData)
			{
				if (response.Error != null)
				{
					CspUtils.DebugLog("Can't load activity bundle: " + response.Error);
				}
				else
				{
					effectPrefabBundle = response.Bundle;
				}
			}, null, false);
		}
	}

	public void OnZoneLoaded(ZoneLoadedMessage msg)
	{
		activeZoneLoaded = true;
		activeZone = msg.zoneName;
		activeZoneID = msg.zoneID;
		if (initialized)
		{
			foreach (ISHSActivity value in activitiesDict.Values)
			{
				value.OnZoneLoaded(msg.zoneName, msg.zoneID);
			}
		}
		else
		{
			queuedZoneLoad = !initialized;
		}
	}

	public void OnZoneUnloaded(ZoneUnloadedMessage msg)
	{
		activeZoneLoaded = false;
		activeZone = string.Empty;
		foreach (ISHSActivity value in activitiesDict.Values)
		{
			value.OnZoneUnloaded(msg.zoneName);
		}
	}

	private void OnAchievementCompleteMessage(AchievementCompleteMessage msg)
	{
		foreach (ISHSActivity value in activitiesDict.Values)
		{
			if (value.RequiredDestinyCompletion == msg.achievementID)
			{
				value.Start();
			}
		}
	}

	private void OnAchievementDataLoaded(AchievementDataLoadedMessage msg)
	{
		if (msg.data.playerID == AppShell.Instance.Profile.UserId)
		{
			foreach (ISHSActivity value in activitiesDict.Values)
			{
				if (!(value is SHSRallyActivity))
				{
					value.Start();
				}
			}
		}
	}

	public void Update()
	{
		if (activeZoneLoaded)
		{
			foreach (ISHSActivity value in activitiesDict.Values)
			{
				if (value.State != 0)
				{
					value.Update();
				}
			}
		}
	}

	public void LoadConfiguration(DataWarehouse data)
	{
		foreach (DataWarehouse item in data.GetIterator("//activities/activity"))
		{
			string @string = item.GetString("activityclass");
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Type type = executingAssembly.GetType(@string);
			if (type == null)
			{
				CspUtils.DebugLog("Can't get type: " + @string + " from executing assembly.");
			}
			else if (!typeof(ISHSActivity).IsAssignableFrom(type))
			{
				CspUtils.DebugLog("Type: " + type.ToString() + " does not implement the ISHSActivity interface.");
			}
			else
			{
				ISHSActivity iSHSActivity = (ISHSActivity)Activator.CreateInstance(type);
				iSHSActivity.Configure(this, item);
				if (activitiesDict.ContainsKey(iSHSActivity.Name))
				{
					CspUtils.DebugLog("Duplicate Activity name detected: " + iSHSActivity.Name);
				}
				else
				{
					activitiesDict[iSHSActivity.Name] = iSHSActivity;
				}
			}
		}
		initialized = true;
		if (queuedZoneLoad)
		{
			queuedZoneLoad = false;
			OnZoneLoaded(new ZoneLoadedMessage(activeZone, activeZoneID));
		}
	}

	public ISHSActivity GetActivity(string name)
	{
		string key = name.ToUpper();
		if (activitiesDict.ContainsKey(key))
		{
			return activitiesDict[key];
		}
		return null;
	}

	public void ResetCollectionActivities()
	{
		foreach (ISHSActivity value in activitiesDict.Values)
		{
			if (value.IsCollectionActivity)
			{
				value.Reset();
			}
		}
	}
}
