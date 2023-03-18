public class CombatCharacterAggroMessage : ShsEventMessage
{
	public CombatController CharacterCombat;

	public CombatController TargetCharacterCombat;

	public CombatCharacterAggroMessage(CombatController AggroingCharacterCombat, CombatController TargetCharacterCombat)
	{
		CharacterCombat = AggroingCharacterCombat;
		this.TargetCharacterCombat = TargetCharacterCombat;
	}
}
