using UnityEngine;

public class LocalPlayerChangedMessage : ShsEventMessage
{
	public readonly GameObject localPlayer;

	public LocalPlayerChangedMessage(GameObject newLocalPlayer)
	{
		localPlayer = newLocalPlayer;
	}
}
