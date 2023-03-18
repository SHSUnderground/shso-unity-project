using UnityEngine;

public class SquadBattleCharacterTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public SquadBattlePlayerEnum attackingPlayer;

	public string attackingCharacterName = "hulk";

	public string secondaryAttackerName = string.Empty;

	public bool delaySecondAttackerSpawn;

	public string blockingCharacterName = string.Empty;

	public bool attackerBecomesKeeper;

	public bool attackerKeeperDestroyed;

	public int damage = 1;

	public bool killingBlow;

	public string attackSequence = "L1,L2,L3,L4,L5;R1;R2;R3";

	public bool repeatSequence;

	public bool doAttack;

	public string __________;

	public SquadBattlePlayerEnum newKeeperPlayer;

	public string newKeeperName = "iron_man";

	public bool addKeeper;

	public string ___________;

	public SquadBattlePlayerEnum removeKeeperPlayer;

	public string removeKeeperName = "iron_man";

	public bool removeKeeper;

	public string ____________;

	public SquadBattlePlayerEnum recoilPlayer;

	public string recoilCharacterName = "human_torch";

	public CombatController.AttackData.RecoilType recoilType = CombatController.AttackData.RecoilType.Large;

	public bool doRecoil;

	protected SquadBattleCharacterController characterManager;

	private void Start()
	{
		doAttack = false;
		addKeeper = false;
		removeKeeper = false;
		doRecoil = false;
		characterManager = (base.gameObject.GetComponent(typeof(SquadBattleCharacterController)) as SquadBattleCharacterController);
	}

	private void Update()
	{
		if (doAttack)
		{
			SquadBattleAction squadBattleAction = new SquadBattleAction();
			squadBattleAction.player = attackingPlayer;
			squadBattleAction.attackingCharacterName = attackingCharacterName;
			squadBattleAction.blockingCharacterName = blockingCharacterName;
			squadBattleAction.attackerBecomesKeeper = attackerBecomesKeeper;
			squadBattleAction.attackerKeeperDestroyed = attackerKeeperDestroyed;
			squadBattleAction.damage = damage;
			squadBattleAction.secondaryAttackingCharacterName = secondaryAttackerName;
			if (killingBlow)
			{
				squadBattleAction.startingHealth = damage;
			}
			SquadBattleAttackPattern squadBattleAttackPattern = squadBattleAction.attackPattern = new SquadBattleAttackPattern();
			squadBattleAttackPattern.RepeatSequence = repeatSequence;
			squadBattleAttackPattern.AttackSequenceString = attackSequence;
			squadBattleAttackPattern.DelaySecondCharacterSpawn = delaySecondAttackerSpawn;
			characterManager.QueueAction(squadBattleAction);
			doAttack = false;
		}
		if (addKeeper)
		{
			characterManager.AddKeeper(newKeeperPlayer, newKeeperName);
			addKeeper = false;
		}
		if (removeKeeper)
		{
			characterManager.RemoveKeeper(removeKeeperPlayer, removeKeeperName);
			removeKeeper = false;
		}
		if (doRecoil)
		{
			doRecoil = false;
			SquadBattleRecoil squadBattleRecoil = new SquadBattleRecoil();
			squadBattleRecoil.player = recoilPlayer;
			squadBattleRecoil.characterName = recoilCharacterName;
			squadBattleRecoil.recoilType = recoilType;
			characterManager.PlayRecoil(squadBattleRecoil);
		}
	}
}
