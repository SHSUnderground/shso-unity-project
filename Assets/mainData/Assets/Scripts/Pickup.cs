using System.Collections;
using UnityEngine;

public class Pickup : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum CollectorFactionType
	{
		Player,
		Enemy,
		Both
	}

	protected const float homingAcceleration = 15f;

	public string pickupID;

	public bool ignoreDespawnTime;

	public CollectorFactionType collectorFaction;

	protected BrawlerController.PickupData pickupData;

	protected float despawnTime;

	protected CombatController pendingTargetCombatController;

	protected GameObject homingObject;

	protected float homingSpeed = 3f;

	private void Start()
	{
		StartCoroutine(GetData());
	}

	protected IEnumerator GetData()
	{
		for (pickupData = BrawlerController.Instance.getPickupData(pickupID); pickupData == null; pickupData = BrawlerController.Instance.getPickupData(pickupID))
		{
			yield return new WaitForSeconds(1f);
		}
		if (!ignoreDespawnTime && pickupData.despawnTime > 0f)
		{
			despawnTime = Time.time + pickupData.despawnTime;
		}
		else
		{
			despawnTime = 0f;
		}
	}

	private void Update()
	{
		if (despawnTime > 0f && Time.time > despawnTime)
		{
			Object.Destroy(base.gameObject);
		}
		if (!(homingObject != null))
		{
			return;
		}
		Vector3 vector = homingObject.transform.position - base.transform.position;
		homingSpeed += 15f * Time.deltaTime;
		float num = homingSpeed * Time.deltaTime;
		if (vector.magnitude < num * 2f)
		{
			if (pickupData.effectName != null)
			{
				GameObject gameObject = BrawlerController.Instance.brawlerBundle.Load(pickupData.effectName) as GameObject;
				if (gameObject != null)
				{
					Object.Instantiate(gameObject, base.gameObject.transform.position, base.gameObject.transform.rotation);
				}
			}
			Object.Destroy(base.gameObject);
		}
		else
		{
			vector = vector.normalized * num;
			base.transform.position += vector;
		}
	}

	public void OnPickup(CombatController targetCombatController)
	{
		StartCoroutine(WaitForData(targetCombatController));
	}

	protected IEnumerator WaitForData(CombatController targetCombatController)
	{
		while (pickupData == null)
		{
			yield return 0;
		}
		HandlePickup(targetCombatController);
	}

	protected void HandlePickup(CombatController targetCombatController)
	{
		if (BrawlerStatManager.Active && targetCombatController.IsPlayer())
		{
			BrawlerStatManager.instance.ReportEmotionalEvent(targetCombatController.gameObject, 2);
		}
		if (pickupData.combatEffectName != null)
		{
			targetCombatController.createCombatEffect(pickupData.combatEffectName, null, true);
		}
		if (pickupData.healthChange > 0f)
		{
			targetCombatController.addHealth(pickupData.healthChange);
		}
		if (pickupData.powerChange > 0f && targetCombatController.IsPlayer())
		{
			PlayerCombatController playerCombatController = targetCombatController as PlayerCombatController;
			if (playerCombatController != null)
			{
				playerCombatController.addPower(pickupData.powerChange);
			}
		}
		AppShell.Instance.EventMgr.Fire(targetCombatController.gameObject, new PickupMessage(targetCombatController.gameObject, base.gameObject));
		homingObject = targetCombatController.gameObject;
		despawnTime = 0f;
	}

	protected void OnOwnershipChange(GameObject go, bool bAssumedOwnership)
	{
		if (bAssumedOwnership)
		{
			if (pendingTargetCombatController == null)
			{
				CspUtils.DebugLog("Unknown pending target in OnOwnershipChange in Pickup");
				return;
			}
			NetActionPickupPickup action = new NetActionPickupPickup(pendingTargetCombatController.gameObject, base.gameObject);
			NetworkComponent networkComponent = pendingTargetCombatController.gameObject.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
			networkComponent.QueueNetAction(action);
			AppShell.Instance.EventMgr.Fire(pendingTargetCombatController.gameObject, new BrawlerPickupMessage(pendingTargetCombatController.gameObject, pickupData.id));
			OnPickup(pendingTargetCombatController);
		}
	}

	protected void OnTriggerEnter(Collider collider)
	{
		if (pendingTargetCombatController != null || pickupData == null)
		{
			return;
		}
		CombatController combatController = collider.gameObject.GetComponent(typeof(CombatController)) as CombatController;
		if (!(combatController != null) || !IsValidCollector(combatController) || combatController.isKilled)
		{
			return;
		}
		SpawnData spawnData = combatController.gameObject.GetComponent(typeof(SpawnData)) as SpawnData;
		if (spawnData != null && (spawnData.spawnType & CharacterSpawn.Type.Remote) != 0)
		{
			return;
		}
		if (pickupData.combatEffectName == null && pickupData.healthChange > 0f && combatController.IsPlayer() && combatController.getHealth() >= combatController.getMaxHealth())
		{
			pendingTargetCombatController = null;
			return;
		}
		if (pickupData.combatEffectName == null && pickupData.powerChange > 0f && combatController.IsPlayer())
		{
			PlayerCombatController playerCombatController = combatController as PlayerCombatController;
			if (playerCombatController != null && playerCombatController.getPower() >= playerCombatController.getMaxPower())
			{
				pendingTargetCombatController = null;
				return;
			}
		}
		pendingTargetCombatController = combatController;
		AppShell.Instance.ServerConnection.Game.TakeOwnership(base.gameObject, OnOwnershipChange);
		OnOwnershipChange(base.gameObject, true); // added by CSP to force health pickups for non hosts.
	}

	public bool IsValidCollector(CombatController collector)
	{
		if (collector == null)
		{
			return false;
		}
		switch (collectorFaction)
		{
		case CollectorFactionType.Player:
			return collector.IsPlayer();
		case CollectorFactionType.Enemy:
			return collector.IsEnemy();
		case CollectorFactionType.Both:
			return collector.IsPlayer() || collector.IsEnemy();
		default:
			return false;
		}
	}
}
