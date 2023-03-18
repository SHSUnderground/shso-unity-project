using UnityEngine;

public class BrawlerPickupMessage : ShsEventMessage
{
	public GameObject Character;

	public string PickupId;

	public BrawlerPickupMessage(GameObject Character, string pickupId)
	{
		this.Character = Character;
		PickupId = pickupId;
	}
}
