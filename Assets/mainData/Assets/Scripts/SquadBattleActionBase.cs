public class SquadBattleActionBase
{
	public SquadBattlePlayerEnum player;

	public string attackingCharacterName = string.Empty;

	public string secondaryAttackingCharacterName = string.Empty;

	public string blockingCharacterName = string.Empty;

	public bool attackerBecomesKeeper;

	public bool attackerKeeperDestroyed;

	public int startingHealth = 100;

	public int damage;

	public bool preventKO;

	public string attackSequenceString = "L1";

	public string emoteString = string.Empty;
}
