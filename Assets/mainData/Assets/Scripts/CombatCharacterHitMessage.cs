using UnityEngine;

public class CombatCharacterHitMessage : ShsEventMessage
{
	public GameObject Character;

	public CombatController CharacterCombat;

	public CombatController SourceCharacterCombat;

	public float Damage;

	public CombatCharacterHitMessage(GameObject Character, CombatController CharacterCombat, CombatController SourceCharacterCombat, float Damage)
	{
		this.Character = Character;
		this.CharacterCombat = CharacterCombat;
		this.SourceCharacterCombat = SourceCharacterCombat;
		this.Damage = Damage;
	}
}
