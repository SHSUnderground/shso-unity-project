using UnityEngine;

public class CombatCharacterDespawnedMessage : ShsEventMessage
{
	public GameObject Character;

	public CombatController CharacterCombat;

	public CombatCharacterDespawnedMessage(GameObject Character, CombatController CharacterCombat)
	{
		this.Character = Character;
		this.CharacterCombat = CharacterCombat;
	}
}
