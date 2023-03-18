using UnityEngine;

public class CharacterGlobals : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Animation animationComponent;

	public ShsCharacterController characterController;

	public CharacterMotionController motionController;

	public BehaviorManager behaviorManager;

	public CombatController combatController;

	public NetworkComponent networkComponent;

	public EffectSequenceList effectsList;

	public SpawnData spawnData;

	public CharacterStats stats;

	public SquadBattleCharacterAI squadBattleCharacterAI;

	public AIControllerBrawler brawlerCharacterAI;

	public DebugHistory debugHistory;

	public CharacterDefinition definitionData;

	public PolymorphController polymorphController;

	public CharacterGlobals activeSidekick;

	public void Awake()
	{
		characterController = base.gameObject.GetComponentInChildren<ShsCharacterController>();
		behaviorManager = base.gameObject.GetComponentInChildren<BehaviorManager>();
		animationComponent = base.gameObject.GetComponentInChildren<Animation>();
		motionController = base.gameObject.GetComponentInChildren<CharacterMotionController>();
		combatController = base.gameObject.GetComponentInChildren<CombatController>();
		networkComponent = base.gameObject.GetComponentInChildren<NetworkComponent>();
		effectsList = base.gameObject.GetComponentInChildren<EffectSequenceList>();
		spawnData = base.gameObject.GetComponentInChildren<SpawnData>();
		stats = base.gameObject.GetComponentInChildren<CharacterStats>();
		squadBattleCharacterAI = base.gameObject.GetComponentInChildren<SquadBattleCharacterAI>();
		brawlerCharacterAI = base.gameObject.GetComponentInChildren<AIControllerBrawler>();
		debugHistory = base.gameObject.GetComponentInChildren<DebugHistory>();
		polymorphController = base.gameObject.GetComponentInChildren<PolymorphController>();
		activeSidekick = null;
		if (characterController == null)
		{
			CspUtils.DebugLog("CharacterGlobals characterController is null");
		}
	}
}
