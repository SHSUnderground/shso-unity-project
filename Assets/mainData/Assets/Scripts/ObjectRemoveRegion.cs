using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Brawler/ObjectRemoveRegion")]
public class ObjectRemoveRegion : ObjectRegionBase
{
	public float radius = 5f;

	public float frequency = 0.5f;

	public int maximumRemovals = 2;

	public GameObject removeObject;

	public GameObject activationObject;

	public GameObject replaceObject;

	protected float removeTime;

	protected GameObject spawnedActivationObject;

	protected override void Start()
	{
		base.Start();
	}

	private void Update()
	{
		if (removeTime > 0f && Time.time >= removeTime)
		{
			performRemoval();
			if (frequency > 0f)
			{
				removeTime = Time.time + frequency;
			}
			else
			{
				removeTime = 0f;
			}
		}
	}

	protected void performRemoval()
	{
		List<GameObject> list = new List<GameObject>();
		Collider[] array = Physics.OverlapSphere(base.transform.position, radius);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			Transform transform = collider.transform;
			while (transform != null)
			{
				if ((transform.gameObject.name == removeObject.name || transform.gameObject.name == removeObject.name + "(Clone)") && checkConstraints(transform.position, false))
				{
					list.Add(transform.gameObject);
					break;
				}
				transform = transform.parent;
			}
		}
		int num = 0;
		while (num < maximumRemovals && list.Count > 0)
		{
			int index = Random.Range(0, list.Count);
			GameObject gameObject = list[index];
			list.RemoveAt(index);
			if (!(gameObject != null))
			{
				continue;
			}
			NetworkComponent networkComponent = gameObject.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
			if (!(networkComponent == null) && !AppShell.Instance.ServerConnection.IsGameHost())
			{
				continue;
			}
			if (replaceObject != null)
			{
				GameObject newObject = Object.Instantiate(replaceObject, gameObject.transform.position, replaceObject.transform.rotation) as GameObject;
				if (netComp != null)
				{
					netComp.AnnounceObjectSpawn(newObject, "ObjectRemoveRegion", replaceObject.name);
				}
			}
			if (netComp != null)
			{
				DeleteEntityMessage msg = new DeleteEntityMessage(networkComponent.goNetId);
				AppShell.Instance.ServerConnection.SendGameMsg(msg);
			}
			Object.Destroy(gameObject);
			num++;
		}
	}

	public GameObject RemoteSpawn(Vector3 spawnLoc, Quaternion spawnRot, GoNetId newID, string prefabName, GameObject parent)
	{
		GameObject original;
		if (prefabName == replaceObject.name)
		{
			original = replaceObject;
		}
		else
		{
			if (!(prefabName == activationObject.name))
			{
				CspUtils.DebugLog("Unknown prefab (" + prefabName + ") for RemoteSpawn on " + base.gameObject.name);
				return null;
			}
			original = activationObject;
		}
		GameObject gameObject = Object.Instantiate(original, spawnLoc, replaceObject.transform.rotation) as GameObject;
		if (prefabName == activationObject.name)
		{
			spawnedActivationObject = gameObject;
		}
		if (newID.IsValid())
		{
			NetworkComponent networkComponent = gameObject.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
			if (networkComponent != null)
			{
				networkComponent.goNetId = newID;
			}
		}
		return gameObject;
	}

	protected override void OnEnableEvent(string eventName)
	{
		base.OnEnableEvent(eventName);
		if ((netComp == null || AppShell.Instance.ServerConnection.IsGameHost()) && activationObject != null)
		{
			GameObject newObject = Object.Instantiate(activationObject, base.transform.position, base.transform.rotation) as GameObject;
			if (netComp != null)
			{
				netComp.AnnounceObjectSpawn(newObject, "ObjectRemoveRegion", activationObject.name);
			}
		}
		removeTime = Time.time;
	}

	protected override void OnDisableEvent(string eventName)
	{
		base.OnDisableEvent(eventName);
		removeTime = 0f;
		if (spawnedActivationObject != null)
		{
			Object.Destroy(spawnedActivationObject);
		}
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(base.transform.position, radius);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "ObjectRemoveIcon.png");
	}
}
