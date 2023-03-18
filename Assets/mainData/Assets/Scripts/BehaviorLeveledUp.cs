public class BehaviorLeveledUp : BehaviorCelebrate
{
	public override void behaviorBegin()
	{
		base.behaviorBegin();
		PlayEffect("level up sequence");
		PlayEffect("emote_cheer_sequence");
	}

	public override void Initialize(bool sendNetMessages)
	{
		base.sendNetMessages = sendNetMessages;
		if (sendNetMessages && networkComponent != null && networkComponent.IsOwner())
		{
			networkComponent.QueueNetAction(new NetActionLeveledUpStart(owningObject));
		}
	}

	protected override void OnSequenceDone(EffectSequence seq)
	{
		if (seq.name == "emote_cheer_sequence")
		{
			seq.FadeToIdleOnEnd = false;
			DestroyEffect(seq);
			PlayEffect("emote_clap_sequence");
		}
		else if (seq.name == "emote_clap_sequence")
		{
			seq.FadeToIdleOnEnd = false;
			DestroyEffect(seq);
			PlayEffect("emote_dance_sequence");
		}
		else if (seq.name == "emote_dance_sequence")
		{
			seq.FadeToIdleOnEnd = false;
			DestroyEffect(seq);
			PlayEffect("emote_dance_sequence");
		}
	}

	public override void behaviorEnd()
	{
		if (sendNetMessages && networkComponent != null && networkComponent.IsOwner())
		{
			networkComponent.QueueNetAction(new NetActionLeveledUpEnd(owningObject));
		}
		base.behaviorEnd();
	}
}
