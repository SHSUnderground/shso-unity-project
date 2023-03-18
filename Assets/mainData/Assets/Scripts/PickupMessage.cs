using UnityEngine;

public class PickupMessage : ShsEventMessage
{
	public GameObject Character;

	public GameObject Pickup;

	public PickupMessage(GameObject character, GameObject pickup)
	{
		Character = character;
		Pickup = pickup;
	}
}
