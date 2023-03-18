using UnityEngine;

public class CombatCharacterCreatedMessage : ShsEventMessage
{
	public GameObject Character;

	public CombatController CharacterCombat;

	public CombatCharacterCreatedMessage(GameObject Character, CombatController CharacterCombat)
	{
		this.Character = Character;
		this.CharacterCombat = CharacterCombat;
	}
}
