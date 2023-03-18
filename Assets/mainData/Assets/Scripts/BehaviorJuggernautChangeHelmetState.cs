using System;

public abstract class BehaviorJuggernautChangeHelmetState : BehaviorBase
{
	protected string changeHelmetAnimName;

	protected string changeHelmetSequence;

	protected string nextBehavior;

	protected bool attackingSuppressed;

	private float endTime;

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		charGlobals.motionController.setDestination(owningObject.transform.position);
		charGlobals.effectsList.TryOneShot(changeHelmetSequence, owningObject);
		animationComponent.Play(changeHelmetAnimName);
		endTime = animationComponent[changeHelmetAnimName].length;
		charGlobals.behaviorManager.ChangeDefaultBehavior(nextBehavior);
		charGlobals.brawlerCharacterAI.attackingSuppressed = attackingSuppressed;
	}

	public override void behaviorUpdate()
	{
		if (elapsedTime > endTime)
		{
			charGlobals.behaviorManager.forceChangeBehavior(Type.GetType(nextBehavior));
		}
		base.behaviorUpdate();
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}

	public override bool useMotionController()
	{
		return false;
	}
}
