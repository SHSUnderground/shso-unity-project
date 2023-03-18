using UnityEngine;

public class HQRoomRespawnMessage : ShsEventMessage
{
	public readonly GameObject respawnObj;

	public HQRoomRespawnMessage(GameObject obj)
	{
		respawnObj = obj;
	}
}
