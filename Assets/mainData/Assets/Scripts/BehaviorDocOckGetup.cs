using System;

public class BehaviorDocOckGetup : BehaviorAttackBase
{
	public override void behaviorBegin()
	{
		base.behaviorBegin();
		charGlobals.brawlerCharacterAI.attackingSuppressed = false;
		charGlobals.behaviorManager.ChangeDefaultBehavior(charGlobals.behaviorManager.defaultBehaviorType);
		CombatController.AttackData secondaryAttackData = combatController.getSecondaryAttackData(1);
		Initialize(null, secondaryAttackData, true, false, 0f);
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}
}
