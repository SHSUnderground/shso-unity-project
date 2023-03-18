using System;
using UnityEngine;

public class BehaviorAttackQueue : BehaviorBase
{
	protected bool secondaryAttack;

	protected string attackName;

	protected bool IsAttackAvailable()
	{
		if (attackName != null)
		{
			return combatController.IsAttackAvailable(attackName);
		}
		return combatController.IsAttackAvailable(secondaryAttack);
	}

	protected void startAttack()
	{
		if (targetObject == null || !IsAttackAvailable())
		{
			charGlobals.behaviorManager.endBehavior();
		}
		else if (!combatController.beginAttack(targetObject, secondaryAttack, false, attackName) && !combatController.pursueTarget(targetObject, secondaryAttack, false, attackName))
		{
			charGlobals.behaviorManager.endBehavior();
		}
	}

	public override void behaviorBegin()
	{
		if (targetObject != null && IsAttackAvailable())
		{
			startAttack();
		}
		base.behaviorBegin();
	}

	public virtual void Initialize(GameObject newTargetObject, bool newSecondaryAttack)
	{
		Initialize(newTargetObject, newSecondaryAttack, null);
	}

	public virtual void Initialize(GameObject newTargetObject, bool newSecondaryAttack, string newAttackName)
	{
		secondaryAttack = newSecondaryAttack;
		attackName = newAttackName;
		setTarget(newTargetObject);
	}

	public bool getSecondaryAttack()
	{
		return secondaryAttack;
	}

	public override void behaviorUpdate()
	{
		startAttack();
		base.behaviorUpdate();
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return true;
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override bool behaviorEndOnCutScene()
	{
		return true;
	}
}
