using System;

public class BehaviorGenericKnockdown : BehaviorBase
{
	protected float endTime;

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		animationComponent.Play("recoil_knockdown");
		endTime = animationComponent["recoil_knockdown"].length;
		charGlobals.behaviorManager.ChangeDefaultBehavior("BehaviorGenericVulnerable");
		VOManager.Instance.PlayVO("damage_knockdown", owningObject);
	}

	public override void behaviorUpdate()
	{
		if (elapsedTime > endTime)
		{
			charGlobals.behaviorManager.forceChangeBehavior(typeof(BehaviorGenericVulnerable));
		}
		base.behaviorUpdate();
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}
}
