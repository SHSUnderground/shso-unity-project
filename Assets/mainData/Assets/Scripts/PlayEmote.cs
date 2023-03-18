using UnityEngine;

public class PlayEmote : InteractiveObjectController
{
	public string emote = "laugh";

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		player.GetComponent<BehaviorManager>().requestChangeBehavior<BehaviorEmote>(false).Initialize(EmotesDefinition.Instance.GetEmoteByCommand(emote).id);
		if (onDone != null)
		{
			onDone(player, CompletionStateEnum.Success);
		}
		return true;
	}
}
