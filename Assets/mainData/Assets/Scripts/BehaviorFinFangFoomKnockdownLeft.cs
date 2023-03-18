using System;

public class BehaviorFinFangFoomKnockdownLeft : BehaviorFinFangFoomKnockdown
{
	protected float endTime;

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		charGlobals.combatController.removeCombatEffect("FinFangFoomInvulnerable");
		animationComponent.Play("left_hand_recoil");
		endTime = animationComponent["left_hand_recoil"].length;
		charGlobals.behaviorManager.ChangeDefaultBehavior("BehaviorFinFangFoomVulnerable");
		charGlobals.effectsList.OneShot("fff_knockdown_sequence", charGlobals.gameObject);
	}

	public override void behaviorUpdate()
	{
		if (elapsedTime > endTime)
		{
			charGlobals.behaviorManager.forceChangeBehavior(typeof(BehaviorFinFangFoomVulnerable));
		}
		base.behaviorUpdate();
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}
}
