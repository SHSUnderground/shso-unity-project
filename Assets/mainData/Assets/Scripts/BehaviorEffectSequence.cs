public class BehaviorEffectSequence : BehaviorBase
{
	protected OnBehaviorDone onBehaviorDone;

	protected EffectSequence sequence;

	protected bool exited;

	public void Initialize(EffectSequence inSequence, OnBehaviorDone onDone)
	{
		sequence = inSequence;
		onBehaviorDone = onDone;
		exited = false;
		sequence.AssignCreator(charGlobals);
		sequence.Initialize(null, OnSequenceDone, OnSequenceEvent);
		sequence.StartSequence();
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override bool useMotionController()
	{
		return false;
	}

	protected void OnSequenceDone(EffectSequence seq)
	{
		ExitBehavior();
	}

	protected void OnSequenceEvent(EffectSequence seq, EventEffect effect)
	{
		if (effect.EventName == "behavior end")
		{
			ExitBehavior();
		}
	}

	private void ExitBehavior()
	{
		if (!exited)
		{
			if (onBehaviorDone != null)
			{
				onBehaviorDone(owningObject);
				onBehaviorDone = null;
			}
			if (charGlobals.behaviorManager.getBehavior() == this)
			{
				charGlobals.behaviorManager.endBehavior();
			}
			exited = true;
		}
	}
}
