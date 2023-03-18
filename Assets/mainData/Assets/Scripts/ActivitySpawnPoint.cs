using UnityEngine;

public class ActivitySpawnPoint : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string activity = string.Empty;

	public string activityBundle;

	public string activityPrefab;

	[HideInInspector]
	public bool used;

	[HideInInspector]
	public ISHSActivity activityReference;

	[HideInInspector]
	protected GameObject activityGO;

	public virtual void Awake()
	{
		foreach (Transform item in base.transform)
		{
			Object.Destroy(item.gameObject);
		}
	}

	public virtual void OnDisable()
	{
	}

	public virtual void Start()
	{
	}

	public void SpawnActivityObject()
	{
		if (used)
		{
			CspUtils.DebugLog("Asked to spawn from this node (" + this + "), but already has a spawned activity object");
			return;
		}
		used = true;
		if (activityBundle != null && activityBundle != string.Empty && activityPrefab != null && activityPrefab != string.Empty)
		{
			AppShell.Instance.BundleLoader.FetchAssetBundle(activityBundle, OnActivityObjectSpawned);
			return;
		}
		CspUtils.DebugLog("Prefab and bundle not specified. Can't spawn activity.");
		used = false;
	}

	protected virtual void OnActivityObjectSpawned(AssetBundleLoadResponse response, object extraData)
	{
		if (response.Error != null)
		{
			CspUtils.DebugLog("Error loading bundle: " + activityBundle);
			used = false;
			return;
		}
		Object @object = response.Bundle.Load(activityPrefab);
		if (@object == null)
		{
			CspUtils.DebugLog("Unable to load prefab <" + activityPrefab + "> from bundle <" + activityBundle + ">!");
		}
		activityGO = (Object.Instantiate(@object, base.gameObject.transform.position, base.gameObject.transform.rotation) as GameObject);
		activityGO.transform.parent = base.transform;
		ActivityObject activityObject = Utils.GetComponent<ActivityObject>(activityGO);
		if (activityObject == null)
		{
			CspUtils.DebugLog("Activity spawner spawned: " + activityGO + " which does not have an activity component.");
			CspUtils.DebugLog("  so we're gonna add one");
			activityObject = activityGO.AddComponent<ActivityObject>();
		}
		activityObject.spawner = this;
		activityReference.RegisterActivityObject(activityObject);
		AppShell.Instance.EventMgr.AddListener<ActivityObjectDespawnMessage>(activityGO, OnActivityObjectDespawned);
	}

	private void OnActivityObjectDespawned(ActivityObjectDespawnMessage msg)
	{
		used = false;
		AppShell.Instance.EventMgr.RemoveListener<ActivityObjectDespawnMessage>(msg.go, OnActivityObjectDespawned);
	}
}
