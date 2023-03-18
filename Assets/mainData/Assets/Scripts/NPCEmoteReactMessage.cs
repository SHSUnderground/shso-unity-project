using UnityEngine;

public class NPCEmoteReactMessage : ShsEventMessage
{
	public GameObject target;

	public string reactionAnim;

	public NPCEmoteReactMessage(GameObject target, string reactionAnim)
	{
		this.target = target;
		this.reactionAnim = reactionAnim;
	}
}
