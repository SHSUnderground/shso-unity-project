using UnityEngine;

public class CombatCharacterAwakenedMessage : ShsEventMessage
{
	public GameObject Character;

	public CombatCharacterAwakenedMessage(GameObject Character)
	{
		this.Character = Character;
	}
}
