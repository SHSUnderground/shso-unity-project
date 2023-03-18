using UnityEngine;

public class CombatCharacterKilledMessage : ShsEventMessage
{
	public GameObject Character;

	public CombatController CharacterCombat;

	public CombatController SourceCharacterCombat;

	public CombatCharacterKilledMessage(GameObject Character, CombatController CharacterCombat, CombatController SourceCharacterCombat)
	{
		this.Character = Character;
		this.CharacterCombat = CharacterCombat;
		this.SourceCharacterCombat = SourceCharacterCombat;
	}
}
