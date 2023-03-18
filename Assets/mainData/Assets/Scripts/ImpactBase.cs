using UnityEngine;

public class ImpactBase
{
	public int index;

	public GameObject colliderObject;

	public CombatController.ImpactData impactData;

	public GameObject createdEffect;

	protected bool fired;

	protected bool done;

	public bool fireNow;

	protected CharacterGlobals charGlobals;

	protected CombatController targetCombatController;

	protected CombatController.AttackData attackData;

	public bool Fired
	{
		get
		{
			return fired;
		}
	}

	public virtual void ImpactBegin(CharacterGlobals newCharGlobals, CombatController.AttackData newAttackData, CombatController newTargetCombatController)
	{
		charGlobals = newCharGlobals;
		attackData = newAttackData;
		targetCombatController = newTargetCombatController;
		fired = false;
		fireNow = false;
		done = false;
		if (impactData.impactResult.colliderName != null)
		{
			colliderObject = charGlobals.combatController.getColliderObject(impactData.impactResult.colliderName);
			if (colliderObject == null)
			{
				CspUtils.DebugLog("Collider object " + impactData.impactResult.colliderName + " not found for attack " + attackData.attackName);
			}
		}
		if (impactData.effectName == null)
		{
			return;
		}
		GameObject gameObject = null;
		gameObject = (charGlobals.combatController.effectSequenceSource.GetEffectSequencePrefabByName(impactData.effectName) as GameObject);
		if (gameObject != null)
		{
			createdEffect = (Object.Instantiate(gameObject) as GameObject);
			createdEffect.SendMessage("AssignCreator", charGlobals, SendMessageOptions.DontRequireReceiver);
			if (colliderObject != null)
			{
				Utils.AttachGameObject(colliderObject.transform.parent.gameObject, createdEffect);
			}
			else
			{
				Utils.AttachGameObject(charGlobals.gameObject, createdEffect);
			}
			EffectSequence effectSequence = createdEffect.GetComponent(typeof(EffectSequence)) as EffectSequence;
			if (effectSequence != null)
			{
				effectSequence.SetParent(charGlobals.gameObject);
			}
		}
		else
		{
			CspUtils.DebugLog("Unable to find " + impactData.effectName + "  in the FX bundles.");
		}
	}

	public virtual void CreateAttackerEffect()
	{
		foreach (string attackerCombatEffect in impactData.attackerCombatEffects)
		{
			charGlobals.combatController.createCombatEffect(attackerCombatEffect, charGlobals.combatController, true);
		}
	}

	public virtual void RemoveAttackerEffect()
	{
		if (impactData.attackerRemoveEffect != null)
		{
			charGlobals.combatController.removeCombatEffect(impactData.attackerRemoveEffect);
		}
	}

	public virtual void ImpactFired()
	{
		fired = true;
		fireNow = false;
		CreateAttackerEffect();
		RemoveAttackerEffect();
		if (impactData.eventName != null)
		{
			if (impactData.eventObjectName != null)
			{
				GameObject gameObject = GameObject.Find(impactData.eventObjectName);
				if (gameObject == null)
				{
					CspUtils.DebugLog(attackData.attackName + " cannot find event object " + impactData.eventObjectName);
				}
				else
				{
					gameObject.SendMessage(impactData.eventName, charGlobals.gameObject, SendMessageOptions.RequireReceiver);
				}
			}
			else
			{
				ScenarioEventManager.Instance.FireScenarioEvent(impactData.eventName, true);
			}
		}
		if (impactData.pickupName != null && charGlobals != null && charGlobals.networkComponent != null && charGlobals.networkComponent.IsOwner())
		{
			BrawlerController.Instance.spawnPickup(impactData.pickupName, colliderObject.transform.position, GoNetId.Invalid);
		}
	}

	public virtual void ImpactUpdate(float elapsedTime)
	{
	}

	public virtual void ImpactEnd()
	{
		if (createdEffect != null)
		{
			Object.Destroy(createdEffect);
		}
	}

	public void ChangeTarget(CombatController newTargetController)
	{
		targetCombatController = newTargetController;
	}

	public bool IsFired()
	{
		return fired;
	}
}
