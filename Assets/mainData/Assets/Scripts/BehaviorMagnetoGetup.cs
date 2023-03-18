using System;

public class BehaviorMagnetoGetup : BehaviorRecoilGetup
{
	public override void behaviorBegin()
	{
		base.behaviorBegin();
		charGlobals.brawlerCharacterAI.attackingSuppressed = false;
		charGlobals.behaviorManager.ChangeDefaultBehavior(charGlobals.behaviorManager.defaultBehaviorType);
		Initialize(owningObject, owningObject.transform.position, null);
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}
}
