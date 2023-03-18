using UnityEngine;

public class CombatPlayerKilledMessage : ShsEventMessage
{
	public GameObject Character;

	public CombatController CharacterCombat;

	public CombatController SourceCharacterCombat;

	public CombatPlayerKilledMessage(GameObject Character, CombatController CharacterCombat, CombatController SourceCharacterCombat)
	{
		this.Character = Character;
		this.CharacterCombat = CharacterCombat;
		this.SourceCharacterCombat = SourceCharacterCombat;
	}
}
