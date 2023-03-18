using System.Collections.Generic;
using UnityEngine;

public class SHSHedgeDropActivity : SHSActivityBase
{
	private List<ActivityObject> activeObjects = new List<ActivityObject>();

	public override void Reset()
	{
		foreach (ActivityObject activeObject in activeObjects)
		{
			if (activeObject != null)
			{
				activeObject.Despawn();
			}
		}
		activeObjects.Clear();
		base.Reset();
	}

	public override void OnZoneLoaded(string zoneName, int zoneID)
	{
		ActivitySpawnPoint[] array = Object.FindObjectsOfType(typeof(ActivitySpawnPoint)) as ActivitySpawnPoint[];
		ActivitySpawnPoint[] array2 = array;
		foreach (ActivitySpawnPoint activitySpawnPoint in array2)
		{
			if (activitySpawnPoint.activity.ToLower() == "hedgedropactivity")
			{
				activitySpawnPoint.activityReference = this;
			}
		}
		base.OnZoneLoaded(zoneName, zoneID);
	}

	public override void OnZoneUnloaded(string zoneName)
	{
		Reset();
		base.OnZoneUnloaded(zoneName);
	}

	public override void RegisterActivityObject(ActivityObject activityObject)
	{
		activeObjects.Add(activityObject);
		activityObject.Activate();
		base.RegisterActivityObject(activityObject);
	}

	public override void UnRegisterActivityObject(ActivityObject activityObject)
	{
		if (activeObjects.Contains(activityObject))
		{
			activeObjects.Remove(activityObject);
		}
		base.UnRegisterActivityObject(activityObject);
	}
}
