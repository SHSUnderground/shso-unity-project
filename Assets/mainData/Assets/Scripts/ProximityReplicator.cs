using UnityEngine;

public class ProximityReplicator : ScenarioEventHandlerEnableBase
{
	public float initialDelay;

	public float frequency;

	public float randomDelay;

	public float distance;

	public float proximityLimit;

	public bool startEnabled = true;

	public bool snapToGround = true;

	protected float replicateTime;

	protected NetworkComponent netComp;

	protected ObjectSpawnRegion spawnRegion;

	protected GameObject prefab;

	protected override void Start()
	{
		base.Start();
		if (startEnabled)
		{
			OnEnableEvent(null);
		}
		netComp = (GetComponent(typeof(NetworkComponent)) as NetworkComponent);
		if (base.transform.parent != null)
		{
			spawnRegion = (base.transform.parent.GetComponent(typeof(ObjectSpawnRegion)) as ObjectSpawnRegion);
		}
		if (spawnRegion != null)
		{
			string prefabName = base.name.Replace("(Clone)", string.Empty);
			prefab = spawnRegion.findPrefab(prefabName);
		}
		else
		{
			prefab = base.gameObject;
		}
	}

	protected override void OnEnableEvent(string eventName)
	{
		replicateTime = Time.time + initialDelay + Random.Range(0f, randomDelay);
	}

	protected override void OnDisableEvent(string eventName)
	{
		replicateTime = 0f;
	}

	private void Update()
	{
		if (replicateTime > 0f && Time.time >= replicateTime)
		{
			replicate();
			replicateTime = Time.time + frequency + Random.Range(0f, randomDelay);
		}
	}

	protected void replicate()
	{
		if (netComp != null && !AppShell.Instance.ServerConnection.IsGameHost())
		{
			return;
		}
		float angle = Random.Range(0f, 360f);
		Vector3 a = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
		Vector3 vector = base.transform.position + a * distance;
		if (snapToGround)
		{
			RaycastHit hitInfo;
			if (!Physics.Raycast(vector + Vector3.up, Vector3.down, out hitInfo, 2f, 804756969))
			{
				return;
			}
			vector = hitInfo.point;
		}
		if (spawnRegion != null && !spawnRegion.checkConstraints(vector, true))
		{
			return;
		}
		if (proximityLimit > 0f)
		{
			Collider[] array = Physics.OverlapSphere(vector, proximityLimit);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				Transform transform = collider.transform;
				while (transform != null)
				{
					if (transform.gameObject.name == base.gameObject.name && Vector3.Distance(transform.position, vector) < proximityLimit)
					{
						return;
					}
					transform = transform.parent;
				}
			}
		}
		GameObject gameObject = Object.Instantiate(prefab, vector, prefab.transform.rotation) as GameObject;
		if (spawnRegion != null)
		{
			gameObject.transform.parent = spawnRegion.transform;
			netComp.AnnounceObjectSpawn(gameObject, "ProximityReplicator", prefab.name);
		}
	}

	public GameObject RemoteSpawn(Vector3 spawnLoc, Quaternion spawnRot, GoNetId newID, string prefabName, GameObject parent)
	{
		GameObject gameObject = Object.Instantiate(prefab, spawnLoc, prefab.transform.rotation) as GameObject;
		gameObject.transform.parent = spawnRegion.transform;
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
}
