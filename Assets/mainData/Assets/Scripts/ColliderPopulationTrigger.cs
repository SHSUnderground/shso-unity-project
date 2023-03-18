using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderPopulationTrigger : TriggerBase
{
	public bool PlayerOnly = true;

	public bool EnemyOnly;

	public bool LocalOnly;

	public int PopulationGoal = 1;

	public bool FireOnGoalMet = true;

	public bool FireOnGoalLost;

	protected Collider triggerCollider;

	protected Dictionary<GameObject, GameObject> objectsInside;

	protected override void Awake()
	{
		base.Awake();
		objectsInside = new Dictionary<GameObject, GameObject>();
	}

	private void Start()
	{
		triggerCollider = (GetComponent(typeof(Collider)) as Collider);
	}

	protected bool checkCharacterType(SpawnData spawnData)
	{
		if (!PlayerOnly && !EnemyOnly)
		{
			return true;
		}
		if (spawnData == null)
		{
			return false;
		}
		if (LocalOnly && (spawnData.spawnType & CharacterSpawn.Type.Local) == 0)
		{
			return false;
		}
		if (PlayerOnly && (spawnData.spawnType & CharacterSpawn.Type.Player) != 0)
		{
			return true;
		}
		if (EnemyOnly && (spawnData.spawnType & CharacterSpawn.Type.Player) == 0)
		{
			return true;
		}
		return false;
	}

	protected virtual void OnTriggerEnter(Collider otherCollider)
	{
		if (otherCollider.gameObject.layer == 2)
		{
			return;
		}
		SpawnData spawnData = otherCollider.gameObject.GetComponent(typeof(SpawnData)) as SpawnData;
		if (checkCharacterType(spawnData))
		{
			int count = objectsInside.Count;
			objectsInside.Add(otherCollider.gameObject, otherCollider.gameObject);
			if (FireOnGoalMet && count < PopulationGoal && objectsInside.Count >= PopulationGoal)
			{
				OnTriggered(otherCollider.gameObject);
			}
		}
	}

	protected virtual void OnTriggerExit(Collider otherCollider)
	{
		if (otherCollider.gameObject.layer == 2)
		{
			return;
		}
		SpawnData spawnData = otherCollider.gameObject.GetComponent(typeof(SpawnData)) as SpawnData;
		if (checkCharacterType(spawnData))
		{
			int count = objectsInside.Count;
			try
			{
				objectsInside.Remove(otherCollider.gameObject);
			}
			catch
			{
				CspUtils.DebugLog("Attempting to remove a game object <" + otherCollider.gameObject.name + "> from collider when it wasn't in it!  Something is out of sync.");
			}
			if (FireOnGoalLost && count >= PopulationGoal && objectsInside.Count < PopulationGoal)
			{
				OnTriggered(otherCollider.gameObject);
			}
		}
	}

	public List<GameObject> GetObjectsInside()
	{
		return new List<GameObject>(objectsInside.Values);
	}
}
