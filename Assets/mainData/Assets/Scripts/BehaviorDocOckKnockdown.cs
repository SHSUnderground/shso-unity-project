using System;

public class BehaviorDocOckKnockdown : BehaviorBase
{
	protected float endTime;

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		animationComponent.Play("recoil_knockdown");
		endTime = animationComponent["recoil_knockdown"].length;
		charGlobals.behaviorManager.ChangeDefaultBehavior("BehaviorDocOckVulnerable");
		charGlobals.brawlerCharacterAI.attackingSuppressed = true;
	}

	public override void behaviorUpdate()
	{
		if (elapsedTime > endTime)
		{
			charGlobals.behaviorManager.forceChangeBehavior(typeof(BehaviorDocOckVulnerable));
		}
		base.behaviorUpdate();
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}
}
