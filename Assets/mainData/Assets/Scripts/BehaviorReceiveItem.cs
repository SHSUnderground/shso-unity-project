public class BehaviorReceiveItem : BehaviorCelebrate
{
	public void Initialize(string baseSequenceName)
	{
		PlayEffect(baseSequenceName);
		PlayEffect("emote_cheer_sequence");
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
}
