using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderTransitionTrigger : TriggerBase
{
	public bool PlayerOnly = true;

	public bool EnemyOnly;

	public GameObject[] LeavingTargets;

	public string LeavingTriggerMethod = "LeavingTrigger";

	public float LeavingResetTime;

	protected float nextLeavingTriggerTime;

	protected Collider triggerCollider;

	protected override void Awake()
	{
		base.Awake();
		DetermineLeavingTriggerTargets();
	}

	protected void DetermineLeavingTriggerTargets()
	{
		if (LeavingTargets == null || LeavingTargets.Length == 0)
		{
			LeavingTargets = new GameObject[1]
			{
				base.gameObject
			};
		}
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
		if (otherCollider.gameObject.layer != 2)
		{
			SpawnData spawnData = otherCollider.gameObject.GetComponent(typeof(SpawnData)) as SpawnData;
			if (checkCharacterType(spawnData))
			{
				OnTriggered(otherCollider.gameObject);
			}
		}
	}

	protected virtual void OnTriggerExit(Collider otherCollider)
	{
		if (otherCollider.gameObject.layer != 2)
		{
			SpawnData spawnData = otherCollider.gameObject.GetComponent(typeof(SpawnData)) as SpawnData;
			if (checkCharacterType(spawnData))
			{
				triggeringObject = otherCollider.gameObject;
				OnLeavingTriggered();
				triggeringObject = null;
			}
		}
	}

	protected virtual void OnLeavingTriggered()
	{
		OnLeavingTriggeredWithReset(LeavingTargets);
	}

	protected bool OnLeavingTriggeredWithReset(GameObject[] triggerTargets)
	{
		if (OneShot && hasFired)
		{
			return false;
		}
		if (LeavingResetTime > 0f && nextLeavingTriggerTime > Time.time)
		{
			return false;
		}
		if (LeavingResetTime > 0f)
		{
			nextLeavingTriggerTime = Time.time + LeavingResetTime;
		}
		OnTriggeredHelper(triggerTargets);
		return true;
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		GameObject[] leavingTargets = LeavingTargets;
		foreach (GameObject gameObject in leavingTargets)
		{
			if (!(gameObject == null))
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(base.transform.position, gameObject.transform.position);
				Quaternion rotation = gameObject.transform.rotation;
				Matrix4x4 matrix = Gizmos.matrix;
				try
				{
					gameObject.transform.LookAt(base.transform);
					Gizmos.matrix = gameObject.transform.localToWorldMatrix;
					Gizmos.DrawFrustum(gameObject.transform.position, 20f, 0f, 0.3f, 1f);
				}
				finally
				{
					gameObject.transform.rotation = rotation;
					Gizmos.matrix = matrix;
				}
			}
		}
	}
}
