using System;

public class BehaviorGenericGetup : BehaviorBase
{
	protected float endTime;

	public string chainToAttack = string.Empty;

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		animationComponent.Play("recoil_getup");
		endTime = animationComponent["recoil_getup"].length;
		charGlobals.behaviorManager.ChangeDefaultBehavior(charGlobals.behaviorManager.defaultBehaviorType);
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		if (!(elapsedTime > endTime))
		{
			return;
		}
		CombatController.AttackData attackData = null;
		if (chainToAttack != string.Empty)
		{
			attackData = combatController.getAttackDataByName(chainToAttack);
			if (attackData == null)
			{
				CspUtils.DebugLog("Attack: " + chainToAttack + " was not found on " + owningObject.name);
			}
			AICombatController aICombatController = combatController as AICombatController;
			if (aICombatController != null && !aICombatController.AttackAvailable(attackData))
			{
				attackData = null;
			}
		}
		if (attackData == null)
		{
			charGlobals.behaviorManager.endBehavior();
		}
		else
		{
			charGlobals.combatController.createAttackBehavior(null, attackData, false, true);
		}
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}
}
