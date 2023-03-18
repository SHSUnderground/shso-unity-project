public class BehaviorChallengeUp : BehaviorCelebrate
{
	public override void behaviorBegin()
	{
		base.behaviorBegin();
		PlayEffect("challenge_system_sequence");
		PlayEffect("emote_cheer_sequence");
	}

	public override void Initialize(bool sendNetMessages)
	{
		base.sendNetMessages = sendNetMessages;
		if (sendNetMessages && networkComponent != null && networkComponent.IsOwner())
		{
			networkComponent.QueueNetAction(new NetActionChallengeUpStart(owningObject));
		}
	}

	protected override void OnSequenceDone(EffectSequence seq)
	{
		if (seq.name == "emote_cheer_sequence")
		{
			DestroyEffect(seq);
			PlayEffect("emote_clap_sequence");
		}
		else if (seq.name == "emote_clap_sequence")
		{
			DestroyEffect(seq);
			PlayEffect("emote_dance_sequence");
		}
		else if (seq.name == "emote_dance_sequence")
		{
			DestroyEffect(seq);
			PlayEffect("emote_dance_sequence");
		}
	}

	public override void behaviorEnd()
	{
		if (sendNetMessages && networkComponent != null && networkComponent.IsOwner())
		{
			networkComponent.QueueNetAction(new NetActionChallengeUpEnd(owningObject));
		}
		base.behaviorEnd();
	}
}
