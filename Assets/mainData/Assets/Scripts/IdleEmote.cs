public class IdleEmote
{
	public EmoteList emoteList;

	public FRange initialWaitTimeSeconds = new FRange(0f, 0f);

	public FRange waitTimeSeconds = new FRange(0f, 0f);

	public FRange emoteTimeSeconds;

	public float elapsed;

	private float nextWaitTime;

	public IdleEmote(EmoteList emoteList, FRange emoteTime, FRange initialWaitTime, FRange subsequentWaitTime)
	{
		this.emoteList = emoteList;
		emoteTimeSeconds = emoteTime;
		initialWaitTimeSeconds = initialWaitTime;
		waitTimeSeconds = subsequentWaitTime;
		nextWaitTime = initialWaitTimeSeconds.RandomValue;
	}

	public bool Update(float deltaTime)
	{
		elapsed += deltaTime;
		if (elapsed >= nextWaitTime)
		{
			elapsed = 0f;
			nextWaitTime = waitTimeSeconds.RandomValue;
			return true;
		}
		return false;
	}

	public void Emote(BehaviorManager behaviorManager)
	{
		sbyte id = EmotesDefinition.Instance.GetEmoteByCommand(emoteList.GetNextEmote()).id;
		BehaviorEmote behaviorEmote = behaviorManager.requestChangeBehavior(typeof(BehaviorEmote), false) as BehaviorEmote;
		behaviorEmote.Initialize(id, false, (emoteTimeSeconds == null) ? (-1f) : emoteTimeSeconds.RandomValue);
	}
}
