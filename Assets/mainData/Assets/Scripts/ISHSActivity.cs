using UnityEngine;

public interface ISHSActivity
{
	string Name
	{
		get;
	}

	string Description
	{
		get;
	}

	ActivityStateEnum State
	{
		get;
	}

	bool AutoStartOnZoneLoad
	{
		get;
	}

	bool IsCollectionActivity
	{
		get;
	}

	ActivityResultEnum Result
	{
		get;
	}

	int RequiredDestinyCompletion
	{
		get;
	}

	void Configure(SHSActivityManager manager, DataWarehouse data);

	void Start();

	void Start(GameObject initObject);

	void OnZoneLoaded(string zoneName, int zoneID);

	void OnZoneUnloaded(string zoneName);

	void Update();

	void Complete();

	void Reset();

	void RegisterActivityObject(ActivityObject activityObject);

	void UnRegisterActivityObject(ActivityObject activityObject);

	void OnActivityAction(ActivityObjectActionNameEnum action, ActivityObject activityObject, object extraData);
}
