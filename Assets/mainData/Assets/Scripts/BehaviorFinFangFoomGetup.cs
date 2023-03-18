using System;

public class BehaviorFinFangFoomGetup : BehaviorBase
{
	protected float endTime;

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		animationComponent.Play("getup");
		endTime = animationComponent["getup"].length;
		charGlobals.behaviorManager.ChangeDefaultBehavior(charGlobals.behaviorManager.defaultBehaviorType);
		charGlobals.effectsList.TryOneShot("getup_sequence", owningObject);
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		if (elapsedTime > endTime)
		{
			charGlobals.behaviorManager.endBehavior();
		}
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}
}
