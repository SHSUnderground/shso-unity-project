using UnityEngine;

public class SquadBattleAction : SquadBattleActionBase
{
	public bool secondarySpawnLocation;

	public bool blocker;

	public SquadBattleAttackPattern attackPattern;

	public GameObject targetCharacter;

	public GameObject secondaryCharacter;

	public SquadBattleAction()
	{
	}

	public SquadBattleAction(SquadBattleActionBase source)
	{
		player = source.player;
		attackingCharacterName = source.attackingCharacterName;
		blockingCharacterName = source.blockingCharacterName;
		attackerBecomesKeeper = source.attackerBecomesKeeper;
		attackerKeeperDestroyed = source.attackerKeeperDestroyed;
		startingHealth = source.startingHealth;
		damage = source.damage;
		attackPattern = new SquadBattleAttackPattern();
		attackPattern.AttackSequenceString = source.attackSequenceString;
	}

	public SquadBattleAction GenerateBlockingAction(GameObject newTarget)
	{
		SquadBattleAction squadBattleAction = new SquadBattleAction();
		squadBattleAction.player = ((player == SquadBattlePlayerEnum.Left) ? SquadBattlePlayerEnum.Right : SquadBattlePlayerEnum.Left);
		squadBattleAction.attackingCharacterName = blockingCharacterName;
		squadBattleAction.damage = 0;
		squadBattleAction.secondarySpawnLocation = true;
		squadBattleAction.targetCharacter = newTarget;
		squadBattleAction.blocker = true;
		return squadBattleAction;
	}
}
