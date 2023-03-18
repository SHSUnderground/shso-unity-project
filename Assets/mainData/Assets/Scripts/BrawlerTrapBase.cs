using UnityEngine;

[AddComponentMenu("Brawler/Traps/Base")]
public class BrawlerTrapBase : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum TrapTargetTypes
	{
		Players,
		Enemies,
		Both
	}

	public TrapTargetTypes targetsAffected;

	public bool affectsObjects;

	public float collisionEnableTime;

	public float collisionDisableTime;

	public float collisionRehitFrequency;

	public float restartTime;

	public float removeTime;

	public bool removeOnImpact;

	public bool removeOnCollision;

	public GameObject spawnOnRemoval;

	public Vector3 spawnOnRemovalRotation = Vector3.zero;

	public CombatController.ImpactResultData impactResultData;

	public float damageMultiplierMax;

	public float damageMultiplierDelay;

	public bool dontDestroyTarget;

	public string eventOnTrapName;

	protected TrapCollider trapCollider;

	protected float startTimeStamp;

	protected float removeTimeStamp;

	protected float currentDamageMultiplier;

	protected NetworkComponent netComp;

	protected virtual void Start()
	{
		currentDamageMultiplier = 1f;
		netComp = (GetComponent(typeof(NetworkComponent)) as NetworkComponent);
		if (removeTime > 0f)
		{
			removeTimeStamp = Time.time + removeTime;
		}
		else
		{
			removeTimeStamp = 0f;
		}
	}

	protected virtual void OnEnable()
	{
		startTimeStamp = Time.time;
		if (!(trapCollider == null))
		{
			return;
		}
		trapCollider = (GetComponentInChildren(typeof(TrapCollider)) as TrapCollider);
		if (trapCollider == null)
		{
			CspUtils.DebugLog("No trap collider found in children of Trap (" + base.gameObject.name + ") - this object is now being removed");
			Object.Destroy(base.gameObject);
			return;
		}
		trapCollider.SetOwningTrap(this);
		if (collisionEnableTime > 0f)
		{
			trapCollider.gameObject.active = false;
		}
	}

	protected virtual void Update()
	{
		if (collisionDisableTime > 0f && Time.time >= startTimeStamp + collisionDisableTime)
		{
			if (trapCollider.gameObject.active)
			{
				trapCollider.gameObject.active = false;
			}
		}
		else if (Time.time >= startTimeStamp + collisionEnableTime && !trapCollider.gameObject.active)
		{
			trapCollider.gameObject.active = true;
		}
		if (restartTime > 0f && Time.time >= startTimeStamp + restartTime)
		{
			Utils.ActivateTree(base.gameObject, false);
			Utils.ActivateTree(base.gameObject, true);
			trapCollider.gameObject.active = false;
		}
		if (removeTimeStamp > 0f && Time.time >= removeTimeStamp)
		{
			removeTrap();
		}
		if (damageMultiplierDelay > 0f && damageMultiplierMax > 0f && currentDamageMultiplier != damageMultiplierMax)
		{
			float num = (Time.time - startTimeStamp) / damageMultiplierDelay;
			if (num > 1f)
			{
				num = 1f;
			}
			if (damageMultiplierMax > 1f)
			{
				currentDamageMultiplier = 1f + num * (damageMultiplierMax - 1f);
			}
			else
			{
				currentDamageMultiplier = 1f - num * (1f - damageMultiplierMax);
			}
		}
	}

	public GameObject RemoteSpawn(Vector3 spawnLoc, Quaternion spawnRot, GoNetId newID, string prefabName, GameObject parent)
	{
		GameObject gameObject = Object.Instantiate(spawnOnRemoval, spawnLoc, spawnRot) as GameObject;
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

	public virtual void OnCollision(Collider other)
	{
		if (removeOnCollision && ((1 << other.gameObject.layer) & 0x2FF79DE9) != 0 && other.gameObject.layer != 22)
		{
			removeTrap();
		}
	}

	public virtual bool OnHitTarget(CombatController targetCombatController)
	{
		CspUtils.DebugLog(base.gameObject.name + " OnHitTargetCharacter : " + targetCombatController.gameObject);
		if ((targetsAffected == TrapTargetTypes.Players && targetCombatController.faction != 0) || (targetsAffected == TrapTargetTypes.Enemies && targetCombatController.faction != CombatController.Faction.Enemy))
		{
			return false;
		}
		float num = 0f;
		if (this.impactResultData != null && this.impactResultData.damageData != null)
		{
			num = this.impactResultData.damageData.getValue(null, false) * currentDamageMultiplier * targetCombatController.incomingDamageMultiplier;
		}
		else if (this.impactResultData.damage != 0f)
		{
			num = this.impactResultData.damage;
		}
		if (this.impactResultData == null)
		{
			CspUtils.DebugLog("impactResultData is null");
		}
		else if (this.impactResultData.damageData == null)
		{
			CspUtils.DebugLog("impactResultData.damage is null");
		}
		else
		{
			CspUtils.DebugLog("damage is " + num);
		}
		if (targetCombatController.canShowPopupDamage() || !Utils.IsPlayer(targetCombatController.gameObject))
		{
			targetCombatController.showPopupDamage(num, true);
		}
		if (dontDestroyTarget && !targetCombatController.isKilled)
		{
			if (targetCombatController.getHealth() - num > 0f)
			{
				CombatController.ImpactResultData impactResultData = new CombatController.ImpactResultData();
				impactResultData.targetCombatEffect = string.Empty;
				impactResultData.damage = num;
				targetCombatController.hitByAttack(trapCollider.transform.position, null, base.gameObject, num, impactResultData);
				targetCombatController.hitByAttack(trapCollider.transform.position, null, base.gameObject, 0f, this.impactResultData);
			}
		}
		else
		{
			targetCombatController.hitByAttack(trapCollider.transform.position, null, base.gameObject, num, this.impactResultData);
		}
		if (AppShell.Instance.ServerConnection != null && netComp != null && netComp.goNetId.IsValid())
		{
			TrapImpactMessage msg = new TrapImpactMessage(netComp.goNetId, targetCombatController.gameObject);
			AppShell.Instance.ServerConnection.SendGameMsg(msg);
		}
		if (removeOnImpact)
		{
			removeTrap();
		}
		DispatchTrapEvent(false);
		return true;
	}

	public virtual bool OnHitTargetCharacter(CharacterGlobals targetCharGlobals)
	{
		return OnHitTarget(targetCharGlobals.combatController);
	}

	public virtual void RemoteHitTarget(GameObject target)
	{
		if (!(target == null))
		{
			CombatController combatController = target.GetComponent(typeof(CombatController)) as CombatController;
			if (combatController != null)
			{
				combatController.hitByAttack(trapCollider.transform.position, null, base.gameObject, impactResultData.damageData.getValue(null, false) * currentDamageMultiplier, impactResultData);
			}
			if (removeOnImpact)
			{
				removeTrap();
			}
		}
	}

	protected void removeTrap()
	{
		if (spawnOnRemoval != null)
		{
			Quaternion rotation = base.transform.rotation;
			if (spawnOnRemovalRotation != Vector3.zero)
			{
				rotation = Quaternion.Euler(spawnOnRemovalRotation);
			}
			bool flag = true;
			if (AppShell.Instance.ServerConnection != null && netComp != null && spawnOnRemoval.GetComponent(typeof(NetworkComponent)) != null)
			{
				flag = false;
				if (netComp.IsOwner() || (!netComp.IsOwnedBySomeoneElse() && AppShell.Instance.ServerConnection.IsGameHost()))
				{
					GameObject newObject = Object.Instantiate(spawnOnRemoval, base.transform.position, rotation) as GameObject;
					netComp.AnnounceObjectSpawn(newObject, "BrawlerTrapBase", spawnOnRemoval.name);
				}
			}
			if (flag)
			{
				Object.Instantiate(spawnOnRemoval, base.transform.position, rotation);
			}
		}
		Utils.DelayedDestroy(base.gameObject);
	}

	protected void DispatchTrapEvent(bool remote)
	{
		if (!string.IsNullOrEmpty(eventOnTrapName))
		{
			ScenarioEventManager.Instance.FireScenarioEvent(eventOnTrapName, remote);
		}
	}
}
