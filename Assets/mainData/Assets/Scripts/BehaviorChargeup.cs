using System;
using UnityEngine;

public class BehaviorChargeup : BehaviorBase
{
	private const float timeBetweenIncreases = 0.5f;

	protected float nextIncreaseTime;

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		charGlobals.motionController.stopGently();
		charGlobals.animationComponent.Play("block");
		charGlobals.combatController.selectedSecondaryAttack = 1;
		nextIncreaseTime = Time.time + 0.5f;
	}

	public override void behaviorUpdate()
	{
		if (!SHSInput.GetMouseButton(SHSInput.MouseButtonType.Right))
		{
			GameObject gameObject = null;
			RaycastHit hit;
			if (Utils.FindObjectUnderCursor(out hit))
			{
				gameObject = hit.collider.gameObject;
				if (gameObject.layer == 14)
				{
					gameObject = gameObject.transform.parent.gameObject;
				}
			}
			if (gameObject == null || !charGlobals.combatController.IsObjectEnemy(gameObject) || !charGlobals.combatController.beginAttack(gameObject, true))
			{
				charGlobals.behaviorManager.endBehavior();
			}
		}
		else
		{
			if (Time.time > nextIncreaseTime && charGlobals.combatController.selectedSecondaryAttack < charGlobals.combatController.maximumSecondaryAttackChain)
			{
				charGlobals.combatController.selectedSecondaryAttack++;
				nextIncreaseTime = Time.time + 0.5f;
				charGlobals.combatController.createEffect("despawn_sequence", owningObject);
			}
			base.behaviorUpdate();
		}
	}

	public override bool allowUserInput()
	{
		return true;
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		if (newBehaviorType.BaseType == typeof(BehaviorRecoil))
		{
			return false;
		}
		return true;
	}
}
