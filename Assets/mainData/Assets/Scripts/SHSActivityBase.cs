using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSActivityBase : ISHSActivity
{
	protected SHSActivityManager manager;

	protected string currentZone;

	protected int currentZoneID;

	protected DataWarehouse activityConfigurationData;

	protected List<GameObject> blockedGameObjects = new List<GameObject>();

	private string name;

	private string description;

	protected ActivityStateEnum state;

	protected ActivityResultEnum result;

	private bool autoStartOnZoneLoad;

	private bool isCollectionActivity;

	private int requiredDestinyCompletion = -1;

	public string Name
	{
		get
		{
			return name;
		}
	}

	public string Description
	{
		get
		{
			return description;
		}
	}

	public ActivityStateEnum State
	{
		get
		{
			return state;
		}
	}

	public ActivityResultEnum Result
	{
		get
		{
			return result;
		}
	}

	public bool AutoStartOnZoneLoad
	{
		get
		{
			return autoStartOnZoneLoad;
		}
	}

	public bool IsCollectionActivity
	{
		get
		{
			return isCollectionActivity;
		}
	}

	public int RequiredDestinyCompletion
	{
		get
		{
			return requiredDestinyCompletion;
		}
	}

	public virtual void Configure(SHSActivityManager manager, DataWarehouse data)
	{
		this.manager = manager;
		activityConfigurationData = data;
		name = data.GetString("id").ToUpper();
		description = data.GetString("description");
		autoStartOnZoneLoad = data.GetBool("autostartonzoneload");
		isCollectionActivity = data.TryGetString("configuration/state_counter_subkey", string.Empty).Equals("CollectionState");
		requiredDestinyCompletion = data.TryGetInt("configuration/required_destiny_completion", -1);
	}

	public virtual void Start()
	{
		start();
	}

	public virtual void Start(GameObject initObject)
	{
		start();
	}

	private void start()
	{
		if (requiredDestinyCompletion != -1 && AppShell.Instance.Profile != null)
		{
			PlayerAchievementData value;
			AchievementManager.allPlayerData.TryGetValue((int)AppShell.Instance.Profile.UserId, out value);
			if (value == null || value.getData(requiredDestinyCompletion) == null || value.getData(requiredDestinyCompletion).complete == 0)
			{
				state = ActivityStateEnum.AwaitingAchievementData;
				return;
			}
		}
		ActivityStateEnum activityStateEnum = state;
		state = ActivityStateEnum.Started;
		if (activityStateEnum == ActivityStateEnum.AwaitingAchievementData)
		{
			foreach (GameObject blockedGameObject in blockedGameObjects)
			{
				if (!(blockedGameObject == null))
				{
					try
					{
						blockedGameObject.SetActiveRecursively(true);
					}
					catch (Exception ex)
					{
						CspUtils.DebugLog("Exception caught in activity " + this + " SetActiveRecursively " + ex.ToString());
					}
				}
			}
			blockedGameObjects.Clear();
			Reactivate();
		}
		AppShell.Instance.EventMgr.Fire(this, new ActivityStartedMessage(this));
	}

	public virtual void OnZoneLoaded(string zoneName, int zoneID)
	{
		currentZone = zoneName;
		currentZoneID = zoneID;
		if (autoStartOnZoneLoad)
		{
			Start();
		}
	}

	public virtual void OnZoneUnloaded(string zoneName)
	{
		blockedGameObjects.Clear();
	}

	public virtual void Update()
	{
	}

	public virtual void Complete()
	{
		AppShell.Instance.EventMgr.Fire(this, new ActivityCompletedMessage(this, result));
	}

	public virtual void Reset()
	{
	}

	public virtual void Reactivate()
	{
	}

	public virtual void RegisterActivityObject(ActivityObject activityObject)
	{
		if (state == ActivityStateEnum.AwaitingAchievementData)
		{
			activityObject.gameObject.SetActiveRecursively(false);
			blockedGameObjects.Add(activityObject.gameObject);
		}
	}

	public virtual void UnRegisterActivityObject(ActivityObject activityObject)
	{
	}

	public virtual void OnActivityAction(ActivityObjectActionNameEnum action, ActivityObject activityObject, object extraData)
	{
	}
}
