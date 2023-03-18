using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProximityTimer : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public CombatController.Faction activatedBy;

	public float timer = 5f;

	public GameObject activationPrefab;

	public GameObject completionPrefab;

	public float activationCascadeRadius;

	public float completionCascadeRadius;

	public string activationScenarioEvent;

	public string completionScenarioEvent;

	protected float completionTime;

	protected bool owner;

	protected bool completed;

	private void Start()
	{
	}

	private void Update()
	{
		if (owner && completionTime > 0f && Time.time >= completionTime)
		{
			Completed();
		}
	}

	protected void OnTriggerEnter(Collider other)
	{
		if (!(completionTime > 0f))
		{
			CombatController combatController = other.gameObject.GetComponent(typeof(CombatController)) as CombatController;
			if (combatController != null && combatController.faction == activatedBy)
			{
				Activate();
			}
		}
	}

	protected void OnOwnershipChange(GameObject go, bool bAssumedOwnership)
	{
		if (bAssumedOwnership)
		{
			owner = true;
		}
	}

	public void Activate()
	{
		if (completionTime > 0f)
		{
			return;
		}
		completionTime = Time.time + timer;
		if (activationScenarioEvent != null)
		{
			ScenarioEventManager.Instance.FireScenarioEvent(activationScenarioEvent, true);
		}
		if (activationPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(activationPrefab) as GameObject;
			Utils.AttachGameObject(base.gameObject, gameObject);
			EffectSequence effectSequence = gameObject.GetComponent(typeof(EffectSequence)) as EffectSequence;
			if (effectSequence != null)
			{
				effectSequence.Initialize(base.gameObject, null, null);
			}
		}
		AppShell.Instance.ServerConnection.Game.TakeOwnership(base.gameObject, OnOwnershipChange);
	}

	protected void Completed()
	{
		if (completed)
		{
			return;
		}
		completed = true;
		if (activationCascadeRadius > 0f)
		{
			Collider[] array = Physics.OverlapSphere(base.gameObject.transform.position, activationCascadeRadius);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				ProximityTimer proximityTimer = collider.GetComponent(typeof(ProximityTimer)) as ProximityTimer;
				if (proximityTimer != null)
				{
					proximityTimer.Activate();
				}
			}
		}
		if (completionCascadeRadius > 0f)
		{
			Collider[] array3 = Physics.OverlapSphere(base.gameObject.transform.position, completionCascadeRadius);
			Collider[] array4 = array3;
			foreach (Collider collider2 in array4)
			{
				ProximityTimer proximityTimer2 = collider2.GetComponent(typeof(ProximityTimer)) as ProximityTimer;
				if (proximityTimer2 != null)
				{
					proximityTimer2.Completed();
				}
			}
		}
		if (completionScenarioEvent != null)
		{
			ScenarioEventManager.Instance.FireScenarioEvent(completionScenarioEvent, false);
		}
		GameObject newObject = Object.Instantiate(completionPrefab, base.gameObject.transform.position, base.gameObject.transform.rotation) as GameObject;
		NetworkComponent networkComponent = GetComponent(typeof(NetworkComponent)) as NetworkComponent;
		if (AppShell.Instance.ServerConnection != null && networkComponent != null)
		{
			networkComponent.AnnounceObjectSpawn(newObject, "ProximityTimer", completionPrefab.name);
		}
		Object.Destroy(base.gameObject);
	}

	public GameObject RemoteSpawn(Vector3 spawnLoc, Quaternion spawnRot, GoNetId newID, string prefabName, GameObject parent)
	{
		GameObject gameObject = Object.Instantiate(completionPrefab, spawnLoc, Quaternion.identity) as GameObject;
		if (newID.IsValid())
		{
			NetworkComponent networkComponent = gameObject.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
			if (networkComponent != null)
			{
				networkComponent.goNetId = newID;
			}
		}
		Object.Destroy(base.gameObject);
		return gameObject;
	}
}
