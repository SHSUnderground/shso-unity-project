using System;
using UnityEngine;

internal class BehaviorPowerMove : BehaviorAnimate
{
	protected string powerMoveBuff;

	protected GameObject powerMoveEffect;

	protected GameObject createdEffect;

	protected float knockbackSize;

	protected float knockbackGrowRate;

	protected float knockbackStartTime;

	protected float knockbackEndTime;

	public void PowerMoveInitialize(string emoteName, GameObject newPowerMoveEffect, string newPowerMoveBuff, float newKnockbackStartTime)
	{
		knockbackSize = 1f;
		knockbackGrowRate = 11f;
		knockbackStartTime = newKnockbackStartTime;
		knockbackEndTime = knockbackStartTime + 0.75f;
		Initialize(emoteName, null);
		powerMoveBuff = newPowerMoveBuff;
		powerMoveEffect = newPowerMoveEffect;
		charGlobals.motionController.setDestination(owningObject.transform.position);
		if (powerMoveEffect != null)
		{
			createdEffect = (UnityEngine.Object.Instantiate(powerMoveEffect) as GameObject);
			Utils.AttachGameObject(owningObject, createdEffect);
		}
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		if (!(elapsedTime > knockbackStartTime) || !(elapsedTime < knockbackEndTime))
		{
			return;
		}
		knockbackSize += knockbackGrowRate * Time.deltaTime;
		Collider[] array = Physics.OverlapSphere(owningObject.transform.position, knockbackSize, -1);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			CombatController combatController = collider.gameObject.GetComponent(typeof(CombatController)) as CombatController;
			if (combatController != null && combatController.faction != base.combatController.faction)
			{
				Vector3 b = collider.gameObject.transform.position - owningObject.transform.position;
				b.Normalize();
				b *= knockbackSize + 0.2f;
				CharacterMotionController characterMotionController = collider.gameObject.GetComponent(typeof(CharacterMotionController)) as CharacterMotionController;
				characterMotionController.moveTowards(owningObject.transform.position + b, 0f);
			}
		}
	}

	public override void behaviorEnd()
	{
		if (createdEffect != null)
		{
			UnityEngine.Object.Destroy(createdEffect);
		}
		if (powerMoveBuff != null)
		{
			combatController.createCombatEffect(powerMoveBuff, combatController, true);
		}
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override bool allowUserInput()
	{
		return false;
	}
}
