using UnityEngine;

internal class HQAIRoomChanged : ShsEventMessage
{
	public GameObject ai;

	public HqRoom2 room;

	public HQAIRoomChanged(GameObject ai, HqRoom2 room)
	{
		this.ai = ai;
		this.room = room;
	}
}
