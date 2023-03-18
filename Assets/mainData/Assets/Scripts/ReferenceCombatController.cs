using UnityEngine;

public class ReferenceCombatController : CombatController
{
	public GameObject referenceObject;

	protected CombatController referenceController;

	public bool useRootObject;

	public float extraAttackHeight;

	protected override void Start()
	{
		base.Start();
		networkComponent = (GetComponent(typeof(NetworkComponent)) as NetworkComponent);
		targetHeight += extraAttackHeight;
		if (useRootObject)
		{
			SetReferenceObject(base.transform.root.gameObject);
		}
		else
		{
			SetReferenceObject(referenceObject);
		}
	}

	public void SetReferenceObject(GameObject newRef)
	{
		if (newRef == base.gameObject)
		{
			newRef = null;
		}
		if (newRef == null)
		{
			referenceObject = newRef;
			referenceController = null;
			return;
		}
		referenceController = (newRef.GetComponent(typeof(CombatController)) as CombatController);
		if (referenceController != null)
		{
			referenceObject = newRef;
		}
		else
		{
			referenceObject = null;
		}
	}

	public override void hitByAttackRemote(Vector3 impactPosition, GameObject source, float damage, ImpactData impactData)
	{
		if (referenceController != null)
		{
			referenceController.hitByAttackRemote(impactPosition, source, damage, impactData);
		}
	}

	public override void hitByAttackLocal(Vector3 impactPosition, CombatController sourceCombatController, string attackName, float damage, ImpactData impactData)
	{
		if (referenceController != null)
		{
			referenceController.hitByAttackLocal(impactPosition, sourceCombatController, attackName, damage, impactData);
		}
	}

	public override void hitByAttack(Vector3 impactPosition, CombatController sourceCombatController, GameObject source, float damage, ImpactResultData impactResultData)
	{
		if (referenceController != null)
		{
			referenceController.hitByAttack(impactPosition, sourceCombatController, source, damage, impactResultData);
		}
	}

	public override void killed(GameObject killer, float duration)
	{
		if (!isKilled)
		{
			isKilled = true;
			if (referenceController != null)
			{
				referenceController.killed(killer, duration);
			}
		}
	}

	public override void revive()
	{
		isKilled = false;
		if (referenceController != null)
		{
			referenceController.revive();
		}
	}

	public override bool IsObjectEnemy(GameObject obj)
	{
		if (referenceController != null)
		{
			return referenceController.IsObjectEnemy(obj);
		}
		return false;
	}

	public override bool isHealthInitialized()
	{
		if (referenceController != null)
		{
			return referenceController.isHealthInitialized();
		}
		return false;
	}

	public override float getHealth()
	{
		if (referenceController != null)
		{
			return referenceController.getHealth();
		}
		return 1f;
	}

	public override void addHealth(float amount)
	{
		if (referenceController != null)
		{
			referenceController.addHealth(amount);
		}
	}

	public override void setHealth(float amount)
	{
		if (referenceController != null)
		{
			referenceController.setHealth(amount);
		}
	}

	public override void clearCombatEffects()
	{
		if (referenceController != null)
		{
			referenceController.clearCombatEffects();
		}
	}

	public override void removeCombatEffect(string combatEffectName)
	{
		if (referenceController != null)
		{
			referenceController.removeCombatEffect(combatEffectName);
		}
	}

	public override void createCombatEffect(string newCombatEffectName, CombatController sourceCombatController, bool usePrefabSource)
	{
		if (referenceController != null)
		{
			referenceController.createCombatEffect(newCombatEffectName, sourceCombatController, usePrefabSource);
		}
	}

	public override void SetSecondaryAttack(int newSelection)
	{
		if (referenceController != null)
		{
			referenceController.SetSecondaryAttack(newSelection);
		}
	}
}
