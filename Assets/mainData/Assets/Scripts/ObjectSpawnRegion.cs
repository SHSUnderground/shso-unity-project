using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Brawler/ObjectSpawnRegion")]
public class ObjectSpawnRegion : ObjectRegionBase
{
	public enum SpawnHeight
	{
		Bottom,
		Top,
		Random,
		Target
	}

	public enum HomingTargets
	{
		None,
		Player,
		Enemy,
		Both
	}

	public GameObject objectPrefab;

	public ObjectSpawnSelection[] randomPrefabs;

	public float frequency = 5f;

	public bool startEnabled;

	public int batchSize = 1;

	public int quantity = -1;

	public int maximumSimultaneous = -1;

	public bool resetQuantityOnEnable = true;

	public bool disableAutomatically;

	public bool snapToGround = true;

	public SpawnHeight spawnHeight = SpawnHeight.Target;

	public Vector3 velocity = Vector3.zero;

	public HomingTargets homingTargets;

	public bool matchBatchToTargets;

	public bool removeWhenEmpty;

	public bool cleanListOnUpdate;

	public bool randomizeYRotation;

	public Vector3 appliedRotation = Vector3.zero;

	public string destroyEvent = string.Empty;

	public bool requireLineOfSight;

	protected float totalRandomWeight;

	protected float nextSpawnTime;

	protected int totalSpawned;

	protected BoxCollider boxCollider;

	protected List<GameObject> potentialTargets;

	protected List<GameObject> spawnedObjects;

	protected bool hasSpawned;

	protected int originalChildCount;

	protected bool activated;

	protected override void Start()
	{
		base.Start();
		if (destroyEvent != string.Empty)
		{
			ScenarioEventManager.Instance.SubscribeScenarioEvent(destroyEvent, OnDestroyEvent);
		}
		potentialTargets = new List<GameObject>();
		spawnedObjects = new List<GameObject>();
		boxCollider = (GetComponent(typeof(BoxCollider)) as BoxCollider);
		activated = false;
		totalSpawned = 0;
		ObjectSpawnSelection[] array = randomPrefabs;
		foreach (ObjectSpawnSelection objectSpawnSelection in array)
		{
			totalRandomWeight += objectSpawnSelection.chanceWeight;
		}
		originalChildCount = base.transform.childCount;
		if (startEnabled)
		{
			OnEnableEvent(string.Empty);
		}
		AppShell.Instance.EventMgr.AddListener<EntityFactionChangeMessage>(OnEntityFactionChange);
	}

	protected override void OnEnableEvent(string eventName)
	{
		base.OnEnableEvent(eventName);
		if (resetQuantityOnEnable)
		{
			totalSpawned = 0;
		}
		activated = true;
	}

	protected override void OnDisableEvent(string eventName)
	{
		base.OnDisableEvent(eventName);
		activated = false;
	}

	protected void OnDestroyEvent(string eventName)
	{
		foreach (GameObject spawnedObject in spawnedObjects)
		{
			Object.Destroy(spawnedObject);
		}
		spawnedObjects.Clear();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (ScenarioEventManager.Instance != null && destroyEvent != string.Empty)
		{
			ScenarioEventManager.Instance.UnsubscribeScenarioEvent(destroyEvent, OnDestroyEvent);
		}
		AppShell.Instance.EventMgr.RemoveListener<EntityFactionChangeMessage>(OnEntityFactionChange);
	}

	private void Update()
	{
		if (activated && Time.time >= nextSpawnTime && (quantity <= 0 || quantity > totalSpawned))
		{
			nextSpawnTime = Time.time + frequency;
			spawn();
			if (disableAutomatically)
			{
				activated = false;
			}
		}
		if (removeWhenEmpty && hasSpawned && base.transform.childCount <= originalChildCount)
		{
			Object.Destroy(base.gameObject);
		}
		if (cleanListOnUpdate && hasSpawned)
		{
			cleanLists();
		}
	}

	protected void cleanLists()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < spawnedObjects.Count; i++)
		{
			if (spawnedObjects[i] == null)
			{
				list.Add(i);
			}
		}
		list.Reverse();
		if (list.Count > 0)
		{
			foreach (int item in list)
			{
				spawnedObjects.RemoveAt(item);
			}
		}
		list.Clear();
		for (int j = 0; j < potentialTargets.Count; j++)
		{
			if (potentialTargets[j] == null)
			{
				list.Add(j);
			}
		}
		list.Reverse();
		if (list.Count > 0)
		{
			foreach (int item2 in list)
			{
				potentialTargets.RemoveAt(item2);
			}
		}
	}

	protected void spawn()
	{
		if (netComp != null && !AppShell.Instance.ServerConnection.IsGameHost())
		{
			return;
		}
		cleanLists();
		int num = batchSize;
		if (matchBatchToTargets)
		{
			num = potentialTargets.Count;
		}
		if (maximumSimultaneous > 0 && num > maximumSimultaneous - spawnedObjects.Count)
		{
			num = maximumSimultaneous - spawnedObjects.Count;
		}
		List<GameObject> list = new List<GameObject>(potentialTargets);
		while (num > 0)
		{
			if (list.Count > 0)
			{
				GameObject gameObject = selectTarget(list);
				if (gameObject != null)
				{
					Vector3 position = gameObject.transform.position;
					if (spawnHeight == SpawnHeight.Top)
					{
						Vector3 size = boxCollider.size;
						float num2 = size.y * 0.5f;
						Vector3 center = boxCollider.center;
						float num3 = num2 + center.y;
						Vector3 position2 = base.transform.position;
						position.y = num3 + position2.y;
					}
					else if (spawnHeight == SpawnHeight.Bottom)
					{
						Vector3 size2 = boxCollider.size;
						float num4 = size2.y * -0.5f;
						Vector3 center2 = boxCollider.center;
						float num5 = num4 + center2.y;
						Vector3 position3 = base.transform.position;
						position.y = num5 + position3.y;
					}
					else if (spawnHeight == SpawnHeight.Random)
					{
						Vector3 center3 = boxCollider.center;
						float y = center3.y;
						Vector3 size3 = boxCollider.size;
						float min = 0f - size3.y * 0.5f;
						Vector3 size4 = boxCollider.size;
						float num6 = y + Random.Range(min, size4.y * 0.5f);
						Vector3 position4 = base.transform.position;
						position.y = num6 + position4.y;
					}
					localSpawn(position);
					list.Remove(gameObject);
				}
			}
			else
			{
				localSpawn(randomSpawnLocation());
			}
			num--;
		}
	}

	protected void localSpawn(Vector3 spawnLoc)
	{
		GameObject gameObject = pickSpawn();
		Quaternion rotation = (!randomizeYRotation) ? gameObject.transform.rotation : Quaternion.Euler(0f, Random.Range(0, 360), 0f);
		GameObject gameObject2 = Object.Instantiate(gameObject, spawnLoc, rotation) as GameObject;
		spawnedObjects.Add(gameObject2);
		totalSpawned++;
		hasSpawned = true;
		if (potentialTargets.Count > 0)
		{
			gameObject2.transform.rotation = base.gameObject.transform.rotation;
			gameObject2.transform.parent = base.gameObject.transform;
		}
		else
		{
			Utils.AttachGameObject(base.gameObject, gameObject2);
		}
		if (appliedRotation != Vector3.zero)
		{
			gameObject2.transform.rotation = Quaternion.Euler(appliedRotation);
		}
		RaycastHit hitInfo;
		if (snapToGround && Physics.Raycast(gameObject2.transform.position, Vector3.down, out hitInfo, 100f, -271086103))
		{
			gameObject2.transform.position = hitInfo.point;
		}
		if (velocity != Vector3.zero)
		{
			if (gameObject2.rigidbody == null)
			{
				CspUtils.DebugLog("Object spawner " + base.gameObject.name + " tried to give velocity to object without rigidbody: " + gameObject.name);
			}
			else
			{
				gameObject2.rigidbody.velocity = velocity;
			}
		}
		if (AppShell.Instance.ServerConnection != null && netComp != null)
		{
			netComp.AnnounceObjectSpawn(gameObject2, "ObjectSpawnRegion", gameObject.name);
		}
	}

	protected GameObject pickSpawn()
	{
		if (randomPrefabs.Length == 0)
		{
			return objectPrefab;
		}
		if (objectPrefab != null)
		{
			CspUtils.DebugLog("Object spawner " + base.gameObject.name + " has a non-random prefab " + objectPrefab.name + " that is being ignored");
		}
		float num = Random.Range(0f, totalRandomWeight);
		float num2 = 0f;
		ObjectSpawnSelection[] array = randomPrefabs;
		foreach (ObjectSpawnSelection objectSpawnSelection in array)
		{
			num2 += objectSpawnSelection.chanceWeight;
			if (num2 >= num)
			{
				return objectSpawnSelection.objectPrefab;
			}
		}
		CspUtils.DebugLog("Unable to pick a prefab in object spawner " + base.gameObject.name);
		return null;
	}

	public GameObject findPrefab(string prefabName)
	{
		if (objectPrefab != null && objectPrefab.name == prefabName)
		{
			return objectPrefab;
		}
		ObjectSpawnSelection[] array = randomPrefabs;
		foreach (ObjectSpawnSelection objectSpawnSelection in array)
		{
			if (objectSpawnSelection.objectPrefab.name == prefabName)
			{
				return objectSpawnSelection.objectPrefab;
			}
		}
		CspUtils.DebugLog("Unknown prefab " + prefabName + " for object spawner " + base.gameObject.name);
		return null;
	}

	public GameObject RemoteSpawn(Vector3 spawnLoc, Quaternion spawnRot, GoNetId newID, string prefabName, GameObject parent)
	{
		totalSpawned++;
		hasSpawned = true;
		GameObject gameObject = findPrefab(prefabName);
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2 = Object.Instantiate(gameObject, spawnLoc, spawnRot) as GameObject;
		gameObject2.transform.parent = base.gameObject.transform;
		gameObject2.transform.localScale = gameObject.transform.localScale;
		spawnedObjects.Add(gameObject2);
		if (newID.IsValid())
		{
			NetworkComponent networkComponent = gameObject2.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
			if (networkComponent != null)
			{
				networkComponent.goNetId = newID;
			}
		}
		if (velocity != Vector3.zero)
		{
			if (gameObject2.rigidbody == null)
			{
				CspUtils.DebugLog("Object spawner " + base.gameObject.name + " tried to give velocity to object without rigidbody: " + gameObject.name);
			}
			else
			{
				gameObject2.rigidbody.velocity = velocity;
			}
		}
		return gameObject2;
	}

	protected GameObject selectTarget(List<GameObject> targets)
	{
		GameObject gameObject = null;
		while (gameObject == null)
		{
			int index = Random.Range(0, targets.Count);
			gameObject = targets[index];
			if (!checkConstraints(gameObject.transform.position, false))
			{
				targets.RemoveAt(index);
				if (targets.Count == 0)
				{
					break;
				}
				gameObject = null;
			}
		}
		return gameObject;
	}

	protected Vector3 randomSpawnLocation()
	{
		if (boxCollider == null)
		{
			return Vector3.zero;
		}
		Vector3 zero = Vector3.zero;
		int num = 0;
		while (true)
		{
			Vector3 center = boxCollider.center;
			float x = center.x;
			Vector3 size = boxCollider.size;
			float min = 0f - size.x * 0.5f;
			Vector3 size2 = boxCollider.size;
			zero.x = x + Random.Range(min, size2.x * 0.5f);
			if (spawnHeight == SpawnHeight.Top)
			{
				Vector3 size3 = boxCollider.size;
				float num2 = size3.y * 0.5f;
				Vector3 center2 = boxCollider.center;
				zero.y = num2 + center2.y;
			}
			else if (spawnHeight == SpawnHeight.Bottom)
			{
				Vector3 size4 = boxCollider.size;
				float num3 = size4.y * -0.5f;
				Vector3 center3 = boxCollider.center;
				zero.y = num3 + center3.y;
			}
			else if (spawnHeight == SpawnHeight.Random)
			{
				Vector3 center4 = boxCollider.center;
				float y = center4.y;
				Vector3 size5 = boxCollider.size;
				float min2 = 0f - size5.y * 0.5f;
				Vector3 size6 = boxCollider.size;
				zero.y = y + Random.Range(min2, size6.y * 0.5f);
			}
			else
			{
				Vector3 center5 = boxCollider.center;
				zero.y = center5.y;
			}
			Vector3 center6 = boxCollider.center;
			float z = center6.z;
			Vector3 size7 = boxCollider.size;
			float min3 = 0f - size7.z * 0.5f;
			Vector3 size8 = boxCollider.size;
			zero.z = z + Random.Range(min3, size8.z * 0.5f);
			bool flag = false;
			if (requireLineOfSight)
			{
				flag = Physics.Linecast(base.transform.position + Vector3.up, base.transform.position + zero + Vector3.up, 804756969);
			}
			if (!flag && checkConstraints(zero + base.gameObject.transform.position, false))
			{
				break;
			}
			if (++num > 10)
			{
				zero = Vector3.zero;
				break;
			}
		}
		return zero;
	}

	protected void OnTriggerEnter(Collider other)
	{
		if (homingTargets != 0 && !(other.gameObject == null))
		{
			CombatController combatController = other.gameObject.GetComponent(typeof(CombatController)) as CombatController;
			if (!(combatController == null) && ((homingTargets != HomingTargets.Enemy && combatController.faction == CombatController.Faction.Player) || (homingTargets != HomingTargets.Player && combatController.faction == CombatController.Faction.Enemy)))
			{
				potentialTargets.Add(other.gameObject);
			}
		}
	}

	protected void OnTriggerExit(Collider other)
	{
		if (homingTargets != 0 && !(other.gameObject == null) && potentialTargets.Contains(other.gameObject))
		{
			potentialTargets.Remove(other.gameObject);
		}
	}

	private void OnEntityFactionChange(EntityFactionChangeMessage msg)
	{
		if (msg.go == null)
		{
			return;
		}
		if (potentialTargets.Contains(msg.go))
		{
			if ((homingTargets == HomingTargets.Enemy && msg.newFaction != CombatController.Faction.Enemy) || (homingTargets == HomingTargets.Player && msg.newFaction != 0))
			{
				potentialTargets.Remove(msg.go);
			}
		}
		else if (((homingTargets == HomingTargets.Enemy && msg.newFaction == CombatController.Faction.Enemy) || (homingTargets == HomingTargets.Player && msg.newFaction == CombatController.Faction.Player)) && checkConstraints(msg.go.transform.position, false) && (!(boxCollider != null) || boxCollider.bounds.Contains(msg.go.transform.position)))
		{
			potentialTargets.Add(msg.go);
		}
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		if (velocity != Vector3.zero)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(base.transform.position, base.transform.position + velocity);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "ObjectSpawnIcon.png");
	}
}
