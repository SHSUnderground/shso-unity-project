using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CombatController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	
        
	public enum Faction
	{
		None = -1,
		Player,
		Enemy,
		Neutral,
		Environment,
		All,
		Count
	}

	public enum CombatState
	{
		Killed = 1,
		Hidden = 2,
		Charmed = 4,
		FactionHits = 8,
		CharmBreakable = 0x10,
		AttacksRestricted = 0x20,
		InteractsRestricted = 0x40,
		JumpRestricted = 0x80
	}

	protected class DelayedImpact
	{
		public CombatController targetCombatController;

		public AttackData sourceAttack;

		public float timeToImpact;

		public int impactIndex;

		public DelayedImpact(CombatController TargetCombatController, AttackData SourceAttack, float TimeToImpact, int ImpactIndex)
		{
			targetCombatController = TargetCombatController;
			sourceAttack = SourceAttack;
			timeToImpact = TimeToImpact;
			impactIndex = ImpactIndex;
		}
	}

	public class AttackData
	{
		public enum RecoilType
		{
			None,
			Small,
			Large,
			Getup,
			Knockdown,
			Launch,
			Dance,
			Stun,
			Attach,
			Uninterruptible,
			NumRecoils
		}

		public static Type[] RecoilBehaviorType = new Type[10]
		{
			null,
			typeof(BehaviorRecoilSmall),
			typeof(BehaviorRecoilLarge),
			typeof(BehaviorRecoilGetup),
			typeof(BehaviorRecoilKnockdown),
			typeof(BehaviorRecoilLaunch),
			typeof(BehaviorRecoilStun),
			typeof(BehaviorRecoilStun),
			typeof(BehaviorRecoilAttach),
			null
		};

		public string attackName;

		public string behaviorName;

		public string animName;

		public string effectName;

		public string triggeredEffectName;

		public bool stopEffectOnChain;

		public float powerCost;

		public float desiredRange;

		public float maximumRange;

		public float moveSpeed;

		public float moveStartTime;

		public float moveArriveTime;

		public float collisionOffTime;

		public float collisionOnTime;

		public float transitionStartTime;

		public float transitionEndTime;

		public bool transitionRequiresImpact;

		public RecoilType interruptibleBy;

		public bool faceTarget;

		public bool trackTarget;

		public bool allowLateralTranslation;

		public float attackDuration;

		public float forwardSpeed;

		public bool forwardOnHit;

		public string targetEffectName;

		public bool environmentalAttack;

		public bool targetlessAttack;

		public bool ignoreHeightDifference;

		public string autoChainAttackName;

		public bool stopOnEnemyCollision;

		public bool hideOnTeleport;

		public int totalTargets;

		public float attackDelay;

		public float minimumDelay;

		public string teleportEffect;

		public float pinballPause;

		public string pinballPrefab;

		public bool linearPinball;

		public bool hasShotInfoTag;

		public float arcMultiplier;

		public string pinballStartNode;

		public float pinballTurnDuration;

		public bool pinballCanBeHit;

		public bool alwaysRenderMaxBeam;

		public string beamHitGeometryEffect;

		public string beamHitNothingEffect;

		public List<SummonData> summons = new List<SummonData>();

		public float chance;

		public string[] attackChildNames;

		public ImpactData[] impacts;

		public bool isInterruptibleBy(RecoilType reactionType, int recoilModifier)
		{
			return interruptibleBy <= reactionType + recoilModifier;
		}
	}

	public class ImpactData
	{
		public enum ImpactType
		{
			Melee,
			Projectile,
			Beam,
			Target,
			Object
		}

		public static Type[] ImpactTypeClass = new Type[5]
		{
			typeof(ImpactMelee),
			typeof(ImpactProjectile),
			typeof(ImpactBeam),
			typeof(ImpactTarget),
			typeof(ImpactObject)
		};

		public int index;

		public ImpactType impactType;

		public string effectName;

		public string impactEffectName;

		public float firingTime;

		public string projectileName;

		public bool projectileIsEnvironmental;

		public bool projectileCreateImmediate;

		public float projectileSpeed;

		public float projectileLifespan;

		public bool projectileAimed;

		public bool projectileAttached;

		public bool projectileRotateToVelocity;

		public float projectileExplosionRadius;

		public float projectileGravity;

		public bool projectileDestroyOnImpact;

		public bool projectileDestroyOnCollision;

		public float projectileImpactStickTime;

		public float projectileCollisionStickTime;

		public string projectileTargetReticle;

		public int projectileAdditionalTargets;

		public float projectileReturnArc;

		public bool projectileBallistic;

		public bool projectileBallisticLob;

		public bool projectileScaledToOwner;

		public float targetAimOffset;

		public float impactStartTime;

		public float impactEndTime;

		public bool showCollider;

		public ModifierData colliderScale;

		public float colliderAngleLimit;

		public List<string> attackerCombatEffects = new List<string>();

		public string attackerRemoveEffect;

		public string requiredCombatEffect;

		public string eventObjectName;

		public string eventName;

		public string pickupName;

		public ImpactResultData impactResult;

		public bool hitsFriends;

		public bool hitsEnemies;

		public int maxHitsPerTarget;

		public int maximumTargetsHit;

		public float minimumDistanceSqr;

		public Vector3 colliderOffset;

		public float nextImpactDelay;

		public static ImpactBase CreateImpact(AttackData attackData, int impactIndex, CharacterGlobals charGlobals, CombatController targetCombatController)
		{
			ImpactData impactData = attackData.impacts[impactIndex];
			Type type = ImpactTypeClass[(int)impactData.impactType];
			ImpactBase impactBase = Activator.CreateInstance(type) as ImpactBase;
			impactBase.index = impactIndex;
			impactBase.impactData = impactData;
			impactBase.ImpactBegin(charGlobals, attackData, targetCombatController);
			return impactBase;
		}
	}

	public class SummonData
	{
		public string name;

		public int duration;

		public int powerAttack;

		public SummonData(SummonRaw data)
		{
			name = data.name;
			duration = data.duration;
			powerAttack = data.powerAttack;
		}
	}

	[Serializable]
	public class ImpactResultData
	{
		public float damage;

		public ModifierData damageData;

		public string colliderName;

		public string impactEffectName;

		public bool impactFaceCamera;

		public ImpactMatrix.Type impactMatrixType;

		public bool pushbackFromCollider;

		public ModifierData pushbackVelocity;

		public ModifierData pushbackDuration;

		public ModifierData knockdownDuration;

		public ModifierData launchVelocity = new ModifierData(0f);

		public string targetCombatEffect;

		public string targetRemoveCombatEffect;

		public bool rotateTargetToImpact;

		public AttackData.RecoilType recoil;

		public bool forceAttach;

		public bool useRecoilDurationOnAttach;

		public string attachPrefabName;

		public string attachAnimName;

		public bool attachUsesRotation;

		public float stunAnimSpeed;

		public List<SummonData> summons = new List<SummonData>();
	}

	public delegate void Attacked(CharacterGlobals attacker);

	protected const float targetHeightMinimum = 0.6f;

	public Faction faction = Faction.None;

	private Faction _oldFaction = Faction.None;

	private static List<CombatController>[] _factionLists = new List<CombatController>[5];

	public float attackDistance = 0.5f;

	public float minimumTargetableDistance;

	public static bool displayDamageInfo = false;

	public static bool displayPopups = true;

	protected float[] multiplayerDamageModifier = new float[4]
	{
		1f,
		0.9f,
		0.8f,
		0.7f
	};

	public int maximumSecondaryAttackChain = 5;

	public int selectedSecondaryAttack = 1;

	public int currentLChain;

	public int characterLevel = 1;

	protected float heightOffsetTolerance = 1f;

	public static int MAX_L_CHAINS = 5;

	public string pickupBone;

	public Dictionary<string, GameObject> colliderObjects;

	protected List<List<string>> attackChainNames;

	protected List<List<AttackData>> attackChain;

	protected string[][] secondaryAttackChainNames;

	protected AttackData[][] secondaryAttackChain;

	protected AttackData[] currentSecondaryAttackChain;

	protected bool started;

	public int currentAttack = 1;

	public Dictionary<string, CombatEffectBase> currentActiveEffects;

	protected string startupCombatEffect;

	protected CharacterGlobals charGlobals;

	protected BehaviorManager behaviorManager;

	protected ShsCharacterController characterController;

	protected CharacterMotionController motionController;

	protected NetworkComponent networkComponent;

	public EffectSequenceList effectSequenceSource;

	protected CharacterStat health;

	protected CharacterStat attackPower;

	protected CharacterStat specialPower;

	public int successfulAttacks;

	protected CombatState currentCombatState;

	public bool isKilled;

	public CombatController charmer;

	private bool _unattackable;

	private float _unattackableFailsafeTime;

	protected string despawnEffectName;

	protected string deathEffectName;

	protected string characterImpactEffectName;

	protected string knockdownEffectName;

	protected string launchLandEffectName;

	protected string stunEffectName;

	protected string pickupThrowableEffectName;

	protected string wakeupEffectName;

	protected string getupEffectName;

	protected string targetBone = "Pelvis";

	public GameObject characterImpactEffect;

	protected GameObject impactMatrixEffect;

	protected GameObject knockdownEffect;

	protected GameObject launchLandEffect;

	protected GameObject stunEffect;

	protected GameObject pickupThrowableEffect;

	protected GameObject wakeupEffect;

	protected GameObject getupEffect;

	protected ImpactMatrix.Type impactMatrixType;

	protected string[] healthEffectNames;

	protected GameObject[] healthEffects;

	protected int currentHealthEffectIndex = -1;

	protected GameObject currentHealthEffect;

	protected AttackData.RecoilType[] recoilInterruptibleBy;

	public string dropTableName;

	public AttackData currentAttackData;

	public AttackData childAttackChosen;

	public float damageMultiplier = 1f;

	protected List<float> damageIncreaseMultipliers;

	protected List<float> damageDecreaseMultipliers;

	public float incomingDamageMultiplier = 1f;

	protected List<float> incomingDamageIncreaseMultipliers;

	protected List<float> incomingDamageDecreaseMultipliers;

	public int recoilResistance;

	public int recoilLimit;

	public int recoilEnhancement;

	public int recoilNerf = 10;

	public int recoilInterruptModifier;

	public float pushbackResistance;

	public float launchResistance;

	public List<string> bonusTargetCombatEffects;

	public List<string> bonusIncommingEffects;

	public List<string> bonusImpactEffects;

	private List<float> _maxPushbackList;

	private List<float> _maxLaunchList;

	private List<float> _maxPushbackDurationList;

	private List<int> _maxRegularAttackChainList;

	private List<int> _lockedSecondaryAttackList;

	private float _maxPushbackVelocity = float.MaxValue;

	private float _maxLaunchVelocity = float.MaxValue;

	private int _maxRegularAttackChain = int.MaxValue;

	private float _maxPushbackDuration = float.MaxValue;

	public bool targetHeightMinimumEnabled = true;

	protected Transform targetTransform;

	public float targetHeight;

	private float _EmoteBroadcastRadius;

	public bool ignoreHeightDifference;

	private Attacked _OnPrimaryAttack;

	private Attacked _OnSecondaryAttack;

	public bool useMouseOver = true;

	protected static Dictionary<string, List<BrawlerPopupAnimator>> availablePopups;

	protected static List<PopupLocator> availableLocators;

	protected float lastPopupTime;

	public static float popupDelay = 0.15f;

	protected string stealthCombatEffectActive = string.Empty;

	protected bool currentAttackSuccessful;

	protected bool recoilAllowed = true;

	protected List<DelayedImpact> impactsToFire;

	public string prestigeEffectID = "622040";

	[CompilerGenerated]
	private ImpactMatrix.Type _003CDefaultImpactMatrixType_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CMouseOver_003Ek__BackingField;

	public bool HasSecondaryAttack
	{
		get
		{
			return secondaryAttackChain.Length > 0;
		}
	}

	public CharacterGlobals CharGlobals
	{
		get
		{
			return charGlobals;
		}
	}

	public bool isHidden
	{
		get
		{
			return InCombatState(CombatState.Hidden);
		}
	}

	public bool IsCharmed
	{
		get
		{
			return InCombatState(CombatState.Charmed);
		}
	}

	public bool IsAttackRestricted
	{
		get
		{
			return InCombatState(CombatState.AttacksRestricted);
		}
	}

	public bool IsInteractRestricted
	{
		get
		{
			return InCombatState(CombatState.InteractsRestricted);
		}
	}

	public bool IsJumpRestricted
	{
		get
		{
			return InCombatState(CombatState.JumpRestricted);
		}
	}

	public bool Unattackable
	{
		get
		{
			if (_unattackable && Time.time > _unattackableFailsafeTime)
			{
				CspUtils.DebugLog("Unattackable timeout reached.  Setting " + base.gameObject.name + " back to attackable.");
				_unattackable = false;
			}
			return _unattackable;
		}
		set
		{
			_unattackable = value;
			if (_unattackable)
			{
				_unattackableFailsafeTime = Time.time + 30f;
			}
		}
	}

	public ImpactMatrix.Type DefaultImpactMatrixType
	{
		[CompilerGenerated]
		get
		{
			return _003CDefaultImpactMatrixType_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CDefaultImpactMatrixType_003Ek__BackingField = value;
		}
	}

	public ImpactMatrix.Type ImpactMatrixType
	{
		get
		{
			return ImpactMatrix.Resolve(impactMatrixType, DefaultImpactMatrixType);
		}
		set
		{
			impactMatrixType = value;
		}
	}

	public Vector3 TargetPosition
	{
		get
		{
			Vector3 position = targetTransform.position;
			position.y += targetHeight;
			if (targetHeightMinimumEnabled)
			{
				float y = position.y;
				Vector3 position2 = base.transform.position;
				if (y - position2.y < 0.6f)
				{
					Vector3 position3 = base.transform.position;
					position.y = position3.y + 0.6f;
				}
			}
			return position;
		}
	}

	public float EmoteBroadcastRadius
	{
		get
		{
			return _EmoteBroadcastRadius;
		}
		set
		{
			_EmoteBroadcastRadius = value;
		}
	}

	public Attacked OnPrimaryAttack
	{
		get
		{
			return _OnPrimaryAttack;
		}
		set
		{
			_OnPrimaryAttack = value;
		}
	}

	public Attacked OnSecondaryAttack
	{
		get
		{
			return _OnSecondaryAttack;
		}
		set
		{
			_OnSecondaryAttack = value;
		}
	}

	public bool MouseOver
	{
		[CompilerGenerated]
		get
		{
			return _003CMouseOver_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CMouseOver_003Ek__BackingField = value;
		}
	}

	public bool CurrentAttackSuccessful
	{
		get
		{
			return currentAttackSuccessful;
		}
	}

	public float getAttackPower()
	{
		if (attackPower == null)
		{
			return 0f;
		}
		return attackPower.Value;
	}

	public float getSpecialPower()
	{
		if (specialPower == null)
		{
			return 0f;
		}
		CspUtils.DebugLog("getSpecialPower " + characterLevel + " " + specialPower.Value + " " + specialPower.level + " " + specialPower.levelScaling);
		return specialPower.Value;
	}

	public virtual void InitializeFromData(DataWarehouse combatData, DataWarehouse attackData)
	{
		bonusTargetCombatEffects = new List<string>();
		bonusIncommingEffects = new List<string>();
		bonusImpactEffects = new List<string>();
		damageIncreaseMultipliers = new List<float>();
		damageDecreaseMultipliers = new List<float>();
		incomingDamageIncreaseMultipliers = new List<float>();
		incomingDamageDecreaseMultipliers = new List<float>();
		_maxPushbackList = new List<float>();
		_maxLaunchList = new List<float>();
		_maxPushbackDurationList = new List<float>();
		_maxRegularAttackChainList = new List<int>();
		_lockedSecondaryAttackList = new List<int>();
		currentActiveEffects = new Dictionary<string, CombatEffectBase>();
		string text = combatData.TryGetString("faction", "Enemy");
		switch (text)
		{
		case "Player":
			faction = Faction.Player;
			break;
		case "Enemy":
			faction = Faction.Enemy;
			break;
		case "Neutral":
			faction = Faction.Neutral;
			break;
		default:
			CspUtils.DebugLog("Unknown faction <" + text + ">");
			faction = Faction.Enemy;
			break;
		}
		ignoreHeightDifference = combatData.TryGetBool("ignore_height", false);
		attackChainNames = new List<List<string>>();
		for (int i = 0; i < MAX_L_CHAINS; i++)
		{
			attackChainNames.Add(new List<string>());
		}
		int num = 0;
		foreach (DataWarehouse item in combatData.GetIterator("attack"))
		{
			num = item.TryGetInt("chain", 0);
			attackChainNames[num].Add(item.GetString("name"));
		}
		int count = combatData.GetCount("secondary_attack");
		secondaryAttackChainNames = new string[count][];
		for (int j = 0; j < count; j++)
		{
			DataWarehouse data = combatData.GetData("secondary_attack", j);
			string[] array = null;
			int count2 = data.GetCount("rank");
			if (count2 > 0)
			{
				array = new string[count2];
				for (int k = 0; k < count2; k++)
				{
					DataWarehouse data2 = data.GetData("rank", k);
					int @int = data2.GetInt("number");
					if (@int >= 0 && @int < count2)
					{
						array[@int] = data2.GetString("name");
					}
					else
					{
						CspUtils.DebugLog("Rank number supplied for secondary attack is invalid");
					}
				}
				for (int l = 0; l < count2; l++)
				{
					if (array[l] == null)
					{
						CspUtils.DebugLog("No secondary attack rank name supplied for <" + base.gameObject.name + "> at rank <" + l + ">");
						array[l] = string.Empty;
					}
				}
			}
			else
			{
				array = new string[1]
				{
					data.TryGetString("name", string.Empty)
				};
			}
			secondaryAttackChainNames[j] = array;
		}
		int num2 = 10;
		recoilInterruptibleBy = new AttackData.RecoilType[num2];
		recoilInterruptibleBy[1] = (AttackData.RecoilType)combatData.TryGetInt("recoil_small_interruptible_by", 2);
		recoilInterruptibleBy[2] = (AttackData.RecoilType)combatData.TryGetInt("recoil_large_interruptible_by", 4);
		recoilInterruptibleBy[3] = (AttackData.RecoilType)combatData.TryGetInt("recoil_getup_interruptible_by", 4);
		recoilInterruptibleBy[4] = (AttackData.RecoilType)combatData.TryGetInt("recoil_knockdown_interruptible_by", num2);
		recoilInterruptibleBy[5] = (AttackData.RecoilType)combatData.TryGetInt("recoil_launch_interruptible_by", num2);
		recoilInterruptibleBy[6] = (AttackData.RecoilType)combatData.TryGetInt("recoil_dance_interruptible_by", 1);
		recoilInterruptibleBy[7] = (AttackData.RecoilType)combatData.TryGetInt("recoil_stun_interruptible_by", num2);
		recoilInterruptibleBy[9] = (AttackData.RecoilType)combatData.TryGetInt("recoil_stun_interruptible_by", num2);
		recoilResistance = combatData.TryGetInt("recoil_resistance", 0);
		recoilLimit = combatData.TryGetInt("recoil_limit", 0);
		pushbackResistance = combatData.TryGetFloat("pushback_resistance", 0f);
		launchResistance = combatData.TryGetFloat("launch_resistance", 0f);
		characterImpactEffectName = combatData.TryGetString("impact_effect", null);
		knockdownEffectName = combatData.TryGetString("knockdown_effect", null);
		launchLandEffectName = combatData.TryGetString("launch_land_effect", null);
		despawnEffectName = combatData.TryGetString("despawn_effect", "despawn_sequence");
		deathEffectName = combatData.TryGetString("death_effect", "death_effect_sequence");
		stunEffectName = combatData.TryGetString("stun_effect", null);
		pickupThrowableEffectName = combatData.TryGetString("pickup_throwable_effect", null);
		wakeupEffectName = combatData.TryGetString("wakeup_effect", null);
		getupEffectName = combatData.TryGetString("getup_effect", null);
		targetBone = combatData.TryGetString("target_bone", "Pelvis");
		DefaultImpactMatrixType = (ImpactMatrix.Type)combatData.TryGetInt("default_impact_matrix_type", 4);
		ImpactMatrixType = (ImpactMatrix.Type)combatData.TryGetInt("impact_matrix_type", 0);
		startupCombatEffect = combatData.TryGetString("startup_combat_effect", null);
		int count3 = combatData.GetCount("health_effect");
		if (count3 > 0)
		{
			healthEffectNames = new string[count3];
			healthEffects = new GameObject[count3];
			for (int m = 0; m < count3; m++)
			{
				healthEffectNames[m] = combatData.GetString("health_effect", m);
				healthEffects[m] = null;
			}
			AppShell.Instance.EventMgr.AddListener<CharacterStat.StatChangeEvent>(OnStatChange);
		}
		dropTableName = combatData.TryGetString("drop_table", "minion_drop_a");
		CharacterStats characterStats = base.gameObject.GetComponentInChildren(typeof(CharacterStats)) as CharacterStats;
		if (characterStats != null)
		{
			health = characterStats.GetStat(CharacterStats.StatType.Health);
			attackPower = characterStats.GetStat(CharacterStats.StatType.AttackPower);
			specialPower = characterStats.GetStat(CharacterStats.StatType.SpecialPower);
		}
		if (AttackDataManager.Instance != null)
		{
			if (attackData != null)
			{
				string text2 = combatData.TryGetString("//name", string.Empty);
				if (text2 != string.Empty)
				{
					AttackDataManager.Instance.loadAttackData(text2, attackData, loadAttackChain);
				}
				else
				{
					CspUtils.DebugLog("Can't load attack data for un-named objects!");
				}
			}
			else
			{
				loadAttackChain();
			}
		}
		AppShell.Instance.EventMgr.AddListener<EntityPolymorphMessage>(OnEntityPolymorph);
		AppShell.Instance.EventMgr.AddListener<EntityFactionChangeMessage>(OnEntityFactionChange);
	}

	public virtual void InitializeFromCopy(CombatController source)
	{
		bonusTargetCombatEffects = new List<string>();
		bonusIncommingEffects = new List<string>();
		bonusImpactEffects = new List<string>();
		damageIncreaseMultipliers = new List<float>();
		damageDecreaseMultipliers = new List<float>();
		incomingDamageIncreaseMultipliers = new List<float>();
		incomingDamageDecreaseMultipliers = new List<float>();
		_maxPushbackList = new List<float>();
		_maxLaunchList = new List<float>();
		_maxPushbackDurationList = new List<float>();
		_maxRegularAttackChainList = new List<int>();
		_lockedSecondaryAttackList = new List<int>();
		currentActiveEffects = new Dictionary<string, CombatEffectBase>();
		faction = source.faction;
		ignoreHeightDifference = source.ignoreHeightDifference;
		attackChainNames = source.attackChainNames;
		attackChain = source.attackChain;
		secondaryAttackChainNames = source.secondaryAttackChainNames;
		secondaryAttackChain = source.secondaryAttackChain;
		currentSecondaryAttackChain = source.currentSecondaryAttackChain;
		selectedSecondaryAttack = source.selectedSecondaryAttack;
		recoilInterruptibleBy = source.recoilInterruptibleBy;
		recoilResistance = source.recoilResistance;
		recoilLimit = source.recoilLimit;
		pushbackResistance = source.pushbackResistance;
		launchResistance = source.launchResistance;
		characterImpactEffectName = source.characterImpactEffectName;
		knockdownEffectName = source.knockdownEffectName;
		launchLandEffectName = source.launchLandEffectName;
		despawnEffectName = source.despawnEffectName;
		deathEffectName = source.deathEffectName;
		stunEffectName = source.stunEffectName;
		pickupThrowableEffectName = source.pickupThrowableEffectName;
		wakeupEffectName = source.wakeupEffectName;
		getupEffectName = source.getupEffectName;
		targetBone = source.targetBone;
		DefaultImpactMatrixType = source.DefaultImpactMatrixType;
		impactMatrixType = source.impactMatrixType;
		startupCombatEffect = source.startupCombatEffect;
		if (source.healthEffectNames != null)
		{
			healthEffectNames = source.healthEffectNames;
			healthEffects = new GameObject[healthEffectNames.Length];
			AppShell.Instance.EventMgr.AddListener<CharacterStat.StatChangeEvent>(OnStatChange);
		}
		dropTableName = source.dropTableName;
		colliderObjects = new Dictionary<string, GameObject>();
		if (source.colliderObjects != null)
		{
			foreach (GameObject value in source.colliderObjects.Values)
			{
				Transform transform = Utils.FindNodeInChildren(base.gameObject.transform, value.gameObject.name);
				GameObject gameObject = transform.gameObject;
				AttackColliderController attackColliderController = gameObject.GetComponent(typeof(AttackColliderController)) as AttackColliderController;
				if (attackColliderController == null)
				{
					CspUtils.DebugLog("Attack collider " + gameObject.name + " has no Attack Collider Controller - skipping");
				}
				else
				{
					attackColliderController.setCombatController(this);
					Utils.ActivateTree(gameObject, false);
					colliderObjects.Add(gameObject.name, gameObject);
				}
			}
		}
		CharacterStats characterStats = base.gameObject.GetComponentInChildren(typeof(CharacterStats)) as CharacterStats;
		health = characterStats.GetStat(CharacterStats.StatType.Health);
		AppShell.Instance.EventMgr.AddListener<EntityPolymorphMessage>(OnEntityPolymorph);
		AppShell.Instance.EventMgr.AddListener<EntityFactionChangeMessage>(OnEntityFactionChange);
	}

	protected virtual void Start()
	{
		if (started)
		{
			return;
		}
		started = true;
		impactsToFire = new List<DelayedImpact>();
		charGlobals = (GetComponent(typeof(CharacterGlobals)) as CharacterGlobals);
		if (charGlobals == null)
		{
			GetTargetTransform();
			return;
		}
		if (startupCombatEffect != null && base.gameObject.active)
		{
			createCombatEffect(startupCombatEffect, this, true);
		}
		behaviorManager = charGlobals.behaviorManager;
		characterController = charGlobals.characterController;
		effectSequenceSource = charGlobals.effectsList;
		motionController = charGlobals.motionController;
		networkComponent = charGlobals.networkComponent;
		AppShell.Instance.EventMgr.Fire(base.gameObject, new CombatCharacterCreatedMessage(base.gameObject, this));
		if (colliderObjects == null)
		{
			colliderObjects = new Dictionary<string, GameObject>();
		}
		isKilled = false;
		GetTargetTransform();
	}

	public void PrecachedInitialize()
	{
		Start();
	}

	public void OnMouseRolloverEnter(object data)
	{
		MouseRollover mouseRollover = data as MouseRollover;
		if (mouseRollover.allowUserInput && mouseRollover.charGlobals.combatController.IsObjectEnemy(base.gameObject) && useMouseOver)
		{
			GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Interactable);
			GUIManager.Instance.AttachMouseoverEnemyIndicator(base.gameObject);
			MouseOver = true;
		}
	}

	public void OnMouseRolloverExit()
	{
		MouseOver = false;
		GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Normal);
		GUIManager.Instance.DetachMouseoverIndicator();
	}

	public void OnMouseLeftClick()
	{
		GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Attack);
		GUIManager.Instance.DetachMouseoverIndicator();
	}

	public void OnMouseLeftRelease()
	{
		GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Interactable);
		GUIManager.Instance.DetachMouseoverIndicator();
	}

	protected virtual void Update()
	{
		if (_oldFaction != faction)
		{
			ChangeFaction(faction);
		}
		int num = 0;
		while (num < impactsToFire.Count)
		{
			DelayedImpact delayedImpact = impactsToFire[num];
			if (delayedImpact.targetCombatController == null || delayedImpact.targetCombatController.isKilled)
			{
				impactsToFire.RemoveAt(num);
				continue;
			}
			delayedImpact.timeToImpact -= Time.deltaTime;
			if (delayedImpact.timeToImpact <= 0f)
			{
				attackHit(delayedImpact.targetCombatController.transform.position, delayedImpact.targetCombatController, delayedImpact.sourceAttack, delayedImpact.sourceAttack.impacts[delayedImpact.impactIndex]);
				impactsToFire.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
	}

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
		if (healthEffectNames != null)
		{
			AppShell.Instance.EventMgr.RemoveListener<CharacterStat.StatChangeEvent>(OnStatChange);
		}
		AppShell.Instance.EventMgr.RemoveListener<EntityPolymorphMessage>(OnEntityPolymorph);
		AppShell.Instance.EventMgr.RemoveListener<EntityFactionChangeMessage>(OnEntityFactionChange);
		if (MouseOver)
		{
			OnMouseRolloverExit();
		}
		RemoveFromFactionList();
	}

	protected void OnStatChange(CharacterStat.StatChangeEvent e)
	{
		if (e.Character != base.gameObject || e.StatType != CharacterStats.StatType.Health)
		{
			return;
		}
		if (e.NewValue <= 0f)
		{
			if (currentHealthEffect != null)
			{
				UnityEngine.Object.Destroy(currentHealthEffect);
			}
			return;
		}
		float num = (e.MaxValue - e.NewValue) / e.MaxValue;
		int num2 = (int)((float)(healthEffectNames.Length + 1) * num) - 1;
		if (num2 == currentHealthEffectIndex)
		{
			return;
		}
		currentHealthEffectIndex = num2;
		if (currentHealthEffect != null)
		{
			UnityEngine.Object.Destroy(currentHealthEffect);
		}
		if (currentHealthEffectIndex >= 0)
		{
			if (healthEffects[currentHealthEffectIndex] == null)
			{
				healthEffects[currentHealthEffectIndex] = (charGlobals.effectsList.GetEffectSequencePrefabByName(healthEffectNames[currentHealthEffectIndex]) as GameObject);
			}
			if (healthEffects[currentHealthEffectIndex] != null)
			{
				currentHealthEffect = (UnityEngine.Object.Instantiate(healthEffects[currentHealthEffectIndex]) as GameObject);
				Utils.AttachGameObject(base.gameObject, currentHealthEffect);
			}
		}
	}

	protected virtual void OnEntityPolymorph(EntityPolymorphMessage e)
	{
		if (behaviorManager != null)
		{
			BehaviorBase behavior = behaviorManager.getBehavior();
			if (behavior != null && behavior.getTarget() == e.original)
			{
				behavior.setTarget(null);
			}
		}
		if (charGlobals != null && charGlobals.brawlerCharacterAI != null)
		{
			charGlobals.brawlerCharacterAI.EndAttackOnTarget(e.original);
		}
		if (e.revert)
		{
			return;
		}
		CombatController component = e.original.GetComponent<CombatController>();
		if (component == this)
		{
			CombatController component2 = e.polymorph.GetComponent<CombatController>();
			if (component2 != null)
			{
				component2.setMinHealth(getMinHealth());
				component2.setMaxHealth(getMaxHealth());
				component2.setHealth(getHealth());
			}
		}
	}

	protected void OnEntityFactionChange(EntityFactionChangeMessage msg)
	{
		if (!(charGlobals == null) && charGlobals.brawlerCharacterAI != null && faction == msg.newFaction)
		{
			charGlobals.brawlerCharacterAI.EndAttackOnTarget(msg.go);
		}
	}

	public virtual void loadAttackChain()
	{
		if (attackChain != null || attackChainNames == null)
		{
			return;
		}
		if (attackChainNames[0].Count == 0 && SocialSpaceController.Instance == null)
		{
			Animation animation = base.gameObject.GetComponent(typeof(Animation)) as Animation;
			if (animation != null)
			{
				if (animation["attack_1"] != null)
				{
					CspUtils.DebugLog(base.gameObject.name + " has no attacks defined.  Adding generic_villain_attack.");
					attackChainNames[0].Add("generic_villain_attack");
				}
				else if (animation["attack_L1"] != null)
				{
					CspUtils.DebugLog(base.gameObject.name + " has no attacks defined.  Adding generic_hero_attack.");
					attackChainNames[0].Add("generic_hero_attack");
				}
				else
				{
					CspUtils.DebugLog(base.gameObject.name + " has no attacks defined.  Adding generic_idle_attack.");
					attackChainNames[0].Add("generic_idle_attack");
				}
			}
		}
		attackChain = new List<List<AttackData>>();
		for (int i = 0; i < MAX_L_CHAINS; i++)
		{
			attackChain.Add(new List<AttackData>());
		}
		secondaryAttackChain = new AttackData[secondaryAttackChainNames.Length][];
		currentSecondaryAttackChain = new AttackData[secondaryAttackChainNames.Length];
		int num = 0;
		for (int j = 0; j < MAX_L_CHAINS; j++)
		{
			for (int k = 0; k < attackChainNames[j].Count; k++)
			{
				attackChain[j].Add(AttackDataManager.Instance.getAttackData(attackChainNames[j][k]));
			}
		}
		for (num = 0; num < secondaryAttackChainNames.Length; num++)
		{
			string[] array = secondaryAttackChainNames[num];
			secondaryAttackChain[num] = new AttackData[array.Length];
			int num2 = 0;
			string[] array2 = array;
			foreach (string attackName in array2)
			{
				secondaryAttackChain[num][num2++] = AttackDataManager.Instance.getAttackData(attackName);
			}
			currentSecondaryAttackChain[num] = GetSecondaryAttackDataAtRank(num, 1);
		}
		attackChainNames = null;
		secondaryAttackChainNames = null;
		SetSecondaryAttack(selectedSecondaryAttack);
	}

	public void addCollider(string colliderPrefabName, string bone, string name, Vector3 offset, float scale)
	{
		Transform transform = Utils.FindNodeInChildren(base.gameObject.transform, bone);
		if (transform == null)
		{
			CspUtils.DebugLog(base.gameObject.name + " failed to attach collider " + name + " on bone " + bone);
		}
		else if (effectSequenceSource != null)
		{
			CspUtils.DebugLog("addCollider colliderPrefabName=" + colliderPrefabName);
			GameObject original = effectSequenceSource.GetEffectSequencePrefabByName(colliderPrefabName) as GameObject;
			CspUtils.DebugLog("addCollider original=" + original);
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			gameObject.name = name;
			gameObject.transform.localPosition += offset;
			gameObject.transform.localScale *= scale;
			Utils.AttachGameObject(transform.gameObject, gameObject);
			AttackColliderController attackColliderController = gameObject.GetComponent(typeof(AttackColliderController)) as AttackColliderController;
			attackColliderController.setCombatController(this);
			Utils.ActivateTree(gameObject, false);
			colliderObjects.Add(name, gameObject);
		}
		else
		{
			CspUtils.DebugLog("Could not add collider \"" + name + "\" on bone \"" + bone + "\": No EffectSequenceList found");
		}
	}

	public bool advanceAttackChain()
	{
		if (attackChain == null)
		{
			return false;
		}
		currentAttack++;
		if (currentAttack > GetMaxRegularAttackChain() || currentAttack > attackChain[currentLChain].Count)
		{
			return false;
		}
		return true;
	}

	public void resetAttackChain()
	{
		currentAttack = 1;
	}

	public void announceAttackBegin(bool secondaryAttack)
	{
		SendMessage("AttackBegin", (!secondaryAttack) ? currentAttack : (-currentAttack), SendMessageOptions.DontRequireReceiver);
	}

	public void announceAttackEnd()
	{
		SendMessage("AttackEnd", null, SendMessageOptions.DontRequireReceiver);
	}

	public bool pursueTarget(GameObject targetObject, bool secondaryAttack)
	{
		return pursueTarget(targetObject, secondaryAttack, false, null);
	}

	public bool pursueTarget(GameObject targetObject, bool secondaryAttack, bool ignoreRange, string attackName)
	{
		CombatController combatController = targetObject.GetComponent(typeof(CombatController)) as CombatController;
		if (combatController.isKilled)
		{
			return false;
		}
		if (charGlobals.combatController.beginAttack(targetObject, secondaryAttack, ignoreRange, attackName))
		{
			return true;
		}
		BehaviorAttackApproach behaviorAttackApproach = behaviorManager.requestChangeBehavior(typeof(BehaviorAttackApproach), true) as BehaviorAttackApproach;
		if (behaviorAttackApproach != null)
		{
			behaviorAttackApproach.setTarget(targetObject);
			behaviorAttackApproach.Initialize(secondaryAttack, attackName);
			return true;
		}
		return false;
	}

	public bool InPursuitOfTarget(GameObject targetObject)
	{
		BehaviorAttackApproach behaviorAttackApproach = behaviorManager.getBehavior() as BehaviorAttackApproach;
		return behaviorAttackApproach != null && behaviorAttackApproach.getTarget() == targetObject;
	}

	public bool AwaitingPursuitOfTarget(GameObject targetObject)
	{
		BehaviorAttackApproach behaviorAttackApproach = behaviorManager.getQueuedBehavior() as BehaviorAttackApproach;
		return behaviorAttackApproach != null && behaviorAttackApproach.getTarget() == targetObject;
	}

	public bool InPursuit()
	{
		return behaviorManager.HasTarget<BehaviorAttackApproach>();
	}

	public bool AwaitingPursuit()
	{
		return behaviorManager.HasAwaitingTarget<BehaviorAttackApproach>();
	}

	public bool InAttack()
	{
		return behaviorManager.HasTarget<BehaviorAttackBase>();
	}

	public bool InCombat()
	{
		return InAttack() || InPursuit() || AwaitingPursuit();
	}

	public GameObject GetCombatTarget()
	{
		BehaviorBase behavior = behaviorManager.getBehavior();
		return (behavior == null) ? null : behavior.getTarget();
	}

	public bool ApproachTarget(GameObject targetObject)
	{
		CombatController combatController = targetObject.GetComponent(typeof(CombatController)) as CombatController;
		if (combatController.isKilled)
		{
			return false;
		}
		BehaviorApproach behaviorApproach = behaviorManager.requestChangeBehavior<BehaviorApproach>(true);
		if (behaviorApproach != null)
		{
			behaviorApproach.setTarget(targetObject);
			behaviorApproach.Initialize(targetObject.transform.position, Quaternion.identity, false, null, null, 0.1f, 100f, false, true, false);
			return true;
		}
		return false;
	}

	public bool checkRange(GameObject targetObject, AttackData attackData)
	{
		CombatController combatController = targetObject.GetComponent(typeof(CombatController)) as CombatController;
		if (combatController == null)
		{
			return false;
		}
		return checkRangeToController(combatController, attackData);
	}

	public bool checkRangeToController(CombatController enemyCombatController, AttackData attackData)
	{
		ShsCharacterController shsCharacterController = enemyCombatController.GetComponent(typeof(ShsCharacterController)) as ShsCharacterController;
		if (enemyCombatController.isKilled)
		{
			return false;
		}
		if (!ignoreHeightDifference && !attackData.ignoreHeightDifference)
		{
			Vector3 vector = enemyCombatController.transform.position - base.gameObject.transform.position;
			if (Mathf.Abs(vector.y) > 1f)
			{
				return false;
			}
		}
		Vector3 vector2;
		if (!ignoreHeightDifference && !enemyCombatController.ignoreHeightDifference && !attackData.ignoreHeightDifference)
		{
			float num = 0f;
			num = ((!(characterController != null)) ? (num + heightOffsetTolerance) : (num + characterController.height * 1.3f));
			num = ((!(shsCharacterController != null)) ? (num + enemyCombatController.heightOffsetTolerance) : (num + shsCharacterController.height * 1.3f));
			num *= 0.5f;
			vector2 = enemyCombatController.TargetPosition - TargetPosition;
			if (Math.Abs(vector2.y) > num)
			{
				return false;
			}
		}
		vector2 = enemyCombatController.transform.position - base.transform.position;
		float num2 = attackData.maximumRange;
		if (CardGameController.Instance != null && attackData.forwardSpeed == 0f)
		{
			ImpactData[] impacts = attackData.impacts;
			foreach (ImpactData impactData in impacts)
			{
				if (impactData.impactType == ImpactData.ImpactType.Melee && impactData.colliderScale.getValue(null, false) * 0.5f < num2)
				{
					num2 = impactData.colliderScale.getValue(null, false) * 0.45f;
					if (num2 < 0.45f)
					{
						num2 = 0.45f;
					}
				}
			}
		}
		float num3 = num2 + characterController.radius;
		num3 = ((!(shsCharacterController != null)) ? (num3 + enemyCombatController.attackDistance) : (num3 + shsCharacterController.radius));
		num3 *= num3;
		if (vector2.sqrMagnitude > num3)
		{
			return false;
		}
		Vector3 targetPosition = TargetPosition;
		RaycastHit hitInfo;
		if (Physics.Raycast(targetPosition, vector2, out hitInfo, vector2.magnitude, -275280407))
		{
			return false;
		}
		return true;
	}

	public bool createAttackBehavior(GameObject targetObject, AttackData newAttackData, bool secondaryAttack, bool force)
	{
		Type type = Type.GetType(newAttackData.behaviorName);
		if (type == null)
		{
			CspUtils.DebugLog("Unknown attack behavior " + newAttackData.behaviorName + " specified in attack " + newAttackData.attackName);
			return false;
		}
		if (secondaryAttack && Utils.IsLocalPlayer(charGlobals))
		{
			Type typeFromHandle = typeof(BehaviorAttackSpecial);
			if (!typeFromHandle.IsAssignableFrom(type))
			{
				CspUtils.DebugLog("secondary attack <" + newAttackData.attackName + "> with behavior <" + type + "> for local player <" + base.gameObject.name + "> should be <" + typeFromHandle + "> - forcing change");
				type = typeFromHandle;
			}
		}
		BehaviorAttackBase behaviorAttackBase = (!force) ? (behaviorManager.requestChangeBehavior(type, false) as BehaviorAttackBase) : (behaviorManager.forceChangeBehavior(type) as BehaviorAttackBase);
		if (behaviorAttackBase != null)
		{
			behaviorAttackBase.Initialize(targetObject, newAttackData, secondaryAttack, false, EmoteBroadcastRadius);
			if (targetObject != null)
			{
				AttackerLimiter component = Utils.GetComponent<AttackerLimiter>(targetObject);
				if (component != null)
				{
					component.SetAttacker(base.gameObject);
				}
			}
			return true;
		}
		return false;
	}

	public virtual bool IsAttackAvailable(bool secondaryAttack)
	{
		return true;
	}

	public virtual bool IsAttackAvailable(string attackName)
	{
		return true;
	}

	public virtual bool IsEnemyAttackable(GameObject targetObject)
	{
		CombatController component = targetObject.GetComponent<CombatController>();
		if (component != null && component.Unattackable)
		{
			return false;
		}
		AttackerLimiter component2 = targetObject.GetComponent<AttackerLimiter>();
		return !(component2 != null) || !component2.InUse;
	}

	public virtual bool beginAttack(GameObject targetObject, bool secondaryAttack)
	{
		return beginAttack(targetObject, secondaryAttack, false);
	}

	public virtual bool beginAttack(GameObject targetObject, bool secondaryAttack, bool ignoreRange)
	{
		return beginAttack(targetObject, secondaryAttack, ignoreRange, null);
	}

	public virtual void checkForInitialSummons(AttackData attackData)
	{
		if (attackData.summons.Count > 0)
		{
			double num = Math.PI * 2.0 / (double)attackData.summons.Count;
			double num2 = 2.0;
			double num3 = attackData.summons.Count - 1;
			foreach (SummonData summon in attackData.summons)
			{
				double num4 = Math.Sin(num * num3) * num2;
				double num5 = Math.Cos(num * num3) * num2;
				num3 -= 1.0;
				CspUtils.DebugLog("CombatController - Attack summon " + summon.name);
				Vector3 vector = new Vector3((float)num4, 0f, (float)num5);
				if (Physics.Linecast(base.gameObject.transform.position + Vector3.up, vector + base.gameObject.transform.position + Vector3.up))
				{
					vector = Vector3.zero;
				}
				CharacterSpawn.SpawnAlly(summon.name, base.gameObject.transform.localPosition + vector, faction, summon.powerAttack, summon.duration, false, string.Empty, string.Empty);
			}
		}
	}

	public virtual bool beginAttack(GameObject targetObject, bool secondaryAttack, bool ignoreRange, string attackName)
	{
		AttackData attackData = null;
		attackData = ((!string.IsNullOrEmpty(attackName)) ? AttackDataManager.Instance.getAttackData(attackName) : getCurrentAttackData(secondaryAttack, false, targetObject));
		if (attackData == null)
		{
			return false;
		}
		if (HasChildAttack(attackData))
		{
			if (childAttackChosen == null || !IsChildAttack(attackData, childAttackChosen))
			{
				childAttackChosen = GetChildAttack(attackData);
			}
			attackData = childAttackChosen;
		}
		if (IsAttackRestricted)
		{
			return false;
		}
		if (targetObject != null && !ignoreRange && !checkRange(targetObject, attackData))
		{
			return false;
		}
		childAttackChosen = null;
		Type type = Type.GetType(attackData.behaviorName);
		if (behaviorManager.currentBehaviorInterruptible(type))
		{
			if (!createAttackBehavior(targetObject, attackData, secondaryAttack, false))
			{
				return false;
			}
			checkForInitialSummons(attackData);
		}
		else if (!secondaryAttack || !string.IsNullOrEmpty(attackName))
		{
			BehaviorAttackQueue behaviorAttackQueue = behaviorManager.requestChangeBehavior(typeof(BehaviorAttackQueue), true) as BehaviorAttackQueue;
			if (behaviorAttackQueue != null)
			{
				behaviorAttackQueue.Initialize(targetObject, secondaryAttack, attackName);
			}
		}
		AppShell.Instance.EventMgr.Fire(this, new BrawlerUnstuckMessage());
		if (secondaryAttack && OnSecondaryAttack != null)
		{
			OnSecondaryAttack(charGlobals);
		}
		else if (!secondaryAttack && OnPrimaryAttack != null)
		{
			OnPrimaryAttack(charGlobals);
		}
		if (InStealthMode())
		{
			ExitStealthMode();
		}
		successfulAttacks++;
		currentAttackSuccessful = false;
		return true;
	}

	public bool beginChainAttack(GameObject targetObject, bool secondaryAttack, bool requiresImpact)
	{
		AttackData attackData = getCurrentAttackData(secondaryAttack, true, targetObject);
		if (!checkRange(targetObject, attackData))
		{
			return false;
		}
		if (requiresImpact && !currentAttackSuccessful)
		{
			return false;
		}
		Type type = Type.GetType(attackData.behaviorName);
		if (type == null)
		{
			CspUtils.DebugLog("Unknown attack behavior " + attackData.behaviorName + " specified in attack " + attackData.attackName);
			return false;
		}
		BehaviorAttackBase behaviorAttackBase = behaviorManager.forceChangeBehavior(type) as BehaviorAttackBase;
		if (behaviorAttackBase != null)
		{
			behaviorAttackBase.Initialize(targetObject, attackData, secondaryAttack, true, EmoteBroadcastRadius);
		}
		return true;
	}

	public GameObject getColliderObject(string name)
	{
		GameObject value = null;
		if (!colliderObjects.TryGetValue(name, out value))
		{
			CspUtils.DebugLog("Requested collider object " + name + " not found in CombatController!");
		}
		return value;
	}

	public bool attackHit(Vector3 colliderPosition, CombatController targetCombatController, AttackData attackData, ImpactData impactData)
	{
		targetCombatController.gameObject.SendMessage("OnAttacked", charGlobals, SendMessageOptions.DontRequireReceiver);
		if (targetCombatController.faction == Faction.Neutral)
		{
			return false;
		}
		if (targetCombatController.faction == faction)
		{
			if (BrawlerController.Instance != null)
			{
				if (!impactData.hitsFriends)
				{
					if (BrawlerController.Instance.pvpEnabled() && IsPlayer())
					{
						CspUtils.DebugLog("pvp should hit here");
					}
					else if (!IsPlayer() || !targetCombatController.InCombatState(CombatState.FactionHits))
					{
						return false;
					}
				}
			}
			else if (CardGameController.Instance != null && (!IsPlayer() || !targetCombatController.InCombatState(CombatState.FactionHits)))
			{
				return false;
			}
		}
		if (!impactData.hitsEnemies && targetCombatController.faction != faction)
		{
			return false;
		}
		if (attackData.environmentalAttack != (targetCombatController.faction == Faction.Environment))
		{
			return false;
		}
		float num = getDamageAmount(targetCombatController, impactData);
		tryShowPopupDamage(targetCombatController, num);
		if (this as PlayerCombatController != null && PlayerCombatController.PlayerCount > 0 && PlayerCombatController.PlayerCount < 5)
		{
			num *= multiplayerDamageModifier[PlayerCombatController.PlayerCount - 1];
		}
		if (impactData.impactType == ImpactData.ImpactType.Projectile)
		{
			BehaviorAttackBase behaviorAttackBase = behaviorManager.getBehavior() as BehaviorAttackBase;
			if (behaviorAttackBase != null)
			{
				behaviorAttackBase.OnProjectileImpact(targetCombatController.gameObject);
			}
		}
		if (impactData.nextImpactDelay > 0f)
		{
			DelayedImpact item = new DelayedImpact(targetCombatController, attackData, impactData.nextImpactDelay, impactData.index + 1);
			impactsToFire.Add(item);
		}
		targetCombatController.hitByAttackLocal(colliderPosition, this, attackData.attackName, num, impactData);
		onDealtDamage(targetCombatController, num, attackData);
		currentAttackSuccessful = true;
		CspUtils.DebugLog("attackHit returning true! damage=" + num);
		return true;
	}

	public virtual void hitByAttackRemote(Vector3 impactPosition, GameObject source, float damage, ImpactData impactData)
	{
		CharacterGlobals characterGlobals = source.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
		if (characterGlobals == null)
		{
			CspUtils.DebugLog("Source character has no globals in hitByAttackRemote");
			return;
		}
		if (characterGlobals.networkComponent.goNetId.ChildId != AppShell.Instance.ServerConnection.GetGameUserId())
		{
			if (impactData.impactType == ImpactData.ImpactType.Projectile)
			{
				BehaviorAttackBase behaviorAttackBase = characterGlobals.behaviorManager.getBehavior() as BehaviorAttackBase;
				if (behaviorAttackBase != null)
				{
					behaviorAttackBase.OnProjectileImpact(base.gameObject);
				}
			}
			if (impactData.impactResult.impactEffectName != string.Empty)
			{
				playImpactEffectFromSource(characterGlobals.combatController, impactData.impactResult.impactEffectName, impactData.impactResult.impactFaceCamera);
			}
			playImpactEffect();
		}
		else if (isKilled)
		{
			return;
		}
		hitByAttack(impactPosition + base.transform.position, characterGlobals.combatController, source, damage, impactData.impactResult);
	}

	public virtual void hitByAttackLocal(Vector3 impactPosition, CombatController sourceCombatController, string attackName, float damage, ImpactData impactData)
	{
		if (impactData.impactResult.impactEffectName != string.Empty && impactData.impactResult.impactEffectName != null)
		{
			playImpactEffectFromSource(sourceCombatController, impactData.impactResult.impactEffectName, impactData.impactResult.impactFaceCamera);
		}
		playImpactEffect();
		if (networkComponent != null)
		{
			if (!networkComponent.IsOwnedBySomeoneElse())
			{
				NetActionImpact action = new NetActionImpact(impactPosition - base.transform.position, sourceCombatController.gameObject, attackName, impactData.index, damage);
				networkComponent.QueueNetAction(action);
			}
			else
			{
				RemoteImpactMessage msg = new RemoteImpactMessage(networkComponent.goNetId, impactPosition - base.transform.position, sourceCombatController.gameObject, attackName, impactData.index, damage);
				AppShell.Instance.ServerConnection.SendGameMsg(msg, networkComponent.NetOwnerId);
				if (health.Value - damage > health.MinimumValue)
				{
					//return;     // CSP commented out, seems to be keeping combatants from taking damage.
				}
			}
		}
		hitByAttack(impactPosition, sourceCombatController, sourceCombatController.gameObject, damage, impactData.impactResult);
	}

	public virtual void hitByAttack(Vector3 impactPosition, CombatController sourceCombatController, GameObject source, float damage, ImpactResultData impactResultData)
	{
		AppShell.Instance.EventMgr.Fire(this, new CombatCharacterHitMessage(base.gameObject, this, sourceCombatController, damage));
		if (sourceCombatController != null)
		{
			if (sourceCombatController.bonusImpactEffects.Count > 0)
			{
				foreach (string bonusImpactEffect in sourceCombatController.bonusImpactEffects)
				{
					playImpactEffectFromSource(sourceCombatController, bonusImpactEffect, true);
				}
			}
			if (IsCharmed && sourceCombatController.IsPlayer() && sourceCombatController.faction == faction && InCombatState(CombatState.CharmBreakable))
			{
				BreakCharm();
			}
			bool flag = true;
			if (IsCharmed)
			{
				flag = (faction != sourceCombatController.faction);
			}
			if (flag)
			{
				SendMessage("HitByEnemy", sourceCombatController, SendMessageOptions.DontRequireReceiver);
			}
		}
		playImpactMatrixEffect(impactResultData.impactMatrixType, sourceCombatController);
		if (!string.IsNullOrEmpty(impactResultData.targetCombatEffect))
		{
			createCombatEffect(impactResultData.targetCombatEffect, sourceCombatController, true);
		}
		if (!string.IsNullOrEmpty(impactResultData.targetRemoveCombatEffect))
		{
			removeCombatEffect(impactResultData.targetRemoveCombatEffect);
		}
		if (sourceCombatController != null)
		{
			foreach (string bonusTargetCombatEffect in sourceCombatController.bonusTargetCombatEffects)
			{
				createCombatEffect(bonusTargetCombatEffect, sourceCombatController, true);
			}
		}
		if (impactResultData.summons.Count > 0)
		{
			foreach (SummonData summon in impactResultData.summons)
			{
				CspUtils.DebugLog("CombatController - IMPACT summon " + summon.name);
				CharacterSpawn.SpawnAlly(summon.name, base.gameObject.transform.localPosition, sourceCombatController.faction, summon.powerAttack, summon.duration, false, string.Empty, string.Empty);
			}
		}
		Vector3 a = base.gameObject.transform.position - impactPosition;
		a.y = 0f;
		a.Normalize();
		if (impactResultData.recoil != AttackData.RecoilType.Attach)
		{
			float num = 0f;
			if (impactResultData != null && impactResultData.pushbackVelocity != null)
			{
				num = impactResultData.pushbackVelocity.getValue(sourceCombatController);
			}
			float num2 = 0f;
			if (impactResultData != null && impactResultData.launchVelocity != null)
			{
				num2 = impactResultData.launchVelocity.getValue(sourceCombatController);
			}
			float num3 = 0f;
			if (impactResultData != null && impactResultData.pushbackDuration != null)
			{
				num3 = impactResultData.pushbackDuration.getValue(sourceCombatController);
			}
			if (num < 0f)
			{
				num = 0f - num;
				a *= -1f;
			}
			if (sourceCombatController != null)
			{
				if (num > sourceCombatController.GetMaxPushbackVelocity())
				{
					num = sourceCombatController.GetMaxPushbackVelocity();
				}
				if (num2 > sourceCombatController.GetMaxLaunchVelocity())
				{
					num2 = sourceCombatController.GetMaxLaunchVelocity();
				}
				if (num3 > sourceCombatController.GetMaxPushbackDuration())
				{
					num3 = sourceCombatController.GetMaxPushbackDuration();
				}
			}
			num -= pushbackResistance;
			if (num > 0f)
			{
				motionController.setForcedVelocity(a * num, num3);
			}
			int num4 = 1;
			if (num2 < 0f)
			{
				num4 = -1;
			}
			num2 = Math.Abs(num2) - launchResistance;
			if (num2 > 0f)
			{
				num2 *= (float)num4;
				motionController.setVerticalVelocity(num2);
			}
		}
		if (recoilAllowed)
		{
			if (impactResultData.recoil == AttackData.RecoilType.None && CardGameController.Instance != null)
			{
				impactResultData.recoil = AttackData.RecoilType.Small;
			}
			if (impactResultData.recoil != 0)
			{
				AttackData.RecoilType recoil = impactResultData.recoil;
				if (sourceCombatController != null)
				{
					if (sourceCombatController.recoilEnhancement > (int)recoil)
					{
						recoil = (AttackData.RecoilType)sourceCombatController.recoilEnhancement;
					}
					if (sourceCombatController.recoilNerf < (int)recoil)
					{
						recoil = (AttackData.RecoilType)sourceCombatController.recoilNerf;
					}
				}
				if (recoilResistance < (int)recoil)
				{
					if (recoilLimit > 0 && (int)recoil > recoilLimit)
					{
						recoil = (AttackData.RecoilType)recoilLimit;
					}
					string currentAttackName = behaviorManager.getCurrentAttackName();
					bool flag2 = true;
					if (currentAttackName == "attackInvalid")
					{
						BehaviorRecoil currentRecoil = behaviorManager.getCurrentRecoil();
						if (currentRecoil != null && currentRecoil.recoilType() != 0 && recoilInterruptibleBy[(int)currentRecoil.recoilType()] > recoil)
						{
							flag2 = false;
							currentRecoil.tookDamage(damage);
						}
					}
					else
					{
						AttackData attackData = AttackDataManager.Instance.getAttackData(currentAttackName);
						flag2 = attackData.isInterruptibleBy(recoil, recoilInterruptModifier);
					}
					if (flag2)
					{
						BehaviorAttackApproach behaviorAttackApproach = null;
						BehaviorAttackBase behaviorAttackBase = null;
						if (charGlobals.spawnData != null && charGlobals.spawnData.spawnType == CharacterSpawn.Type.LocalPlayer)
						{
							behaviorAttackApproach = (behaviorManager.getBehavior() as BehaviorAttackApproach);
							behaviorAttackBase = (behaviorManager.getBehavior() as BehaviorAttackBase);
						}
						Type newBehaviorType = AttackData.RecoilBehaviorType[(int)recoil];
						BehaviorRecoil behaviorRecoil = behaviorManager.requestChangeBehavior(newBehaviorType, false) as BehaviorRecoil;
						if (behaviorRecoil != null)
						{
							behaviorRecoil.Initialize(source, impactPosition, impactResultData);
							if (behaviorAttackApproach != null)
							{
								BehaviorAttackQueue behaviorAttackQueue = behaviorManager.requestChangeBehavior(typeof(BehaviorAttackQueue), true) as BehaviorAttackQueue;
								if (behaviorAttackQueue != null)
								{
									behaviorAttackQueue.Initialize(behaviorAttackApproach.getTarget(), behaviorAttackApproach.secondaryAttack);
								}
							}
							else if (behaviorAttackBase != null)
							{
								BehaviorAttackQueue behaviorAttackQueue2 = behaviorManager.requestChangeBehavior(typeof(BehaviorAttackQueue), true) as BehaviorAttackQueue;
								if (behaviorAttackQueue2 != null)
								{
									behaviorAttackQueue2.Initialize(behaviorAttackBase.getTarget(), behaviorAttackBase.category == BehaviorAttackBase.AttackCategory.Secondary);
								}
							}
						}
					}
				}
			}
		}
		takeDamage(damage, source);
	}

	public virtual float getDamageAmount(CombatController targetCombatController, ImpactData impactData)
	{
		float value = impactData.impactResult.damageData.getValue(this);
		return value * damageMultiplier * targetCombatController.incomingDamageMultiplier;
	}

	public virtual void tryShowPopupDamage(CombatController targetCombatController, float damage)
	{
		if (charGlobals.squadBattleCharacterAI == null && (canShowPopupDamage() || targetCombatController.canShowPopupDamage()))
		{
			bool player = faction != Faction.Player;
			if (faction == Faction.Neutral && networkComponent != null)
			{
				player = !networkComponent.goNetId.IsPlayerId();
			}
			targetCombatController.showPopupDamage(damage, player);
		}
	}

	public virtual void showPopupDamage(float damage, bool player)
	{
		if (!displayPopups || damage < 1f)
		{
			return;
		}
		string popupPrefab = (!player) ? "OtherDamagePopup" : "PlayerDamagePopup";
		GameObject gameObject = createPopup(getPopupLocation(), popupPrefab, ((int)damage).ToString());
		if (gameObject != null)
		{
			BrawlerPopupAnimator component = gameObject.GetComponent<BrawlerPopupAnimator>();
			if (Time.time - lastPopupTime < popupDelay)
			{
				lastPopupTime += popupDelay;
				component.DelayPopup(lastPopupTime - Time.time, this, false);
			}
			else
			{
				lastPopupTime = Time.time;
			}
			if (!player)
			{
				component.AttachToParent(base.gameObject.transform);
			}
		}
	}

	public void showPopupScore(float amount)
	{
		if (!displayPopups || amount < 1f)
		{
			return;
		}
		Vector3 location;
		getPopupLocation(base.gameObject, out location);
		//GameObject gameObject = createPopup(location, "ScorePopup", ((int)amount).ToString());  // CSP
		GameObject gameObject = null;  // CSP - workaround that keeps pink medalicon from showing on enemy kills.
		if (gameObject != null)
		{
			BrawlerPopupAnimator component = gameObject.GetComponent<BrawlerPopupAnimator>();
			component.DelayPopup(1f, this, true);
			ScorePopupAnimator scorePopupAnimator = component as ScorePopupAnimator;
			if (BrawlerController.Instance != null && scorePopupAnimator != null)
			{
				scorePopupAnimator.SetMedal(BrawlerController.Instance.GetCurrentMedalName());
			}
		}
	}

	public Vector3 getPopupLocation()
	{
		Vector3 position = base.transform.position;
		if (characterController != null)
		{
			position.y += characterController.height;
		}
		else if (base.renderer != null)
		{
			float y = position.y;
			Vector3 max = base.renderer.bounds.max;
			position.y = y + max.y;
		}
		return position;
	}

	public bool canShowPopupDamage()
	{
		return Utils.IsLocalPlayer(base.gameObject) || (IsCharmed && charmer != null && Utils.IsLocalPlayer(charmer.gameObject)) || (charGlobals != null && (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Ally) != 0 && (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Local) != 0);
	}

	public void dieNow()
	{
		if ((charGlobals.spawnData.spawnType & CharacterSpawn.Type.Player) == 0 || networkComponent.IsOwner())
		{
			killed(null, 0.4f);
		}
	}

	protected virtual void takeDamage(float damage, GameObject source)
	{
		if (networkComponent == null && source != null)
		{
			SquadBattleCharacterAI squadBattleCharacterAI = source.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
			if (squadBattleCharacterAI != null)
			{
				squadBattleCharacterAI.inflictDamage(base.gameObject);
			}
			return;
		}
		if (charGlobals.spawnData != null && (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Boss) != 0)
		{
			damage /= (float)PlayerCombatController.PlayerCount;
		}
		if (currentActiveEffects.Keys.Count > 0)
		{
			string[] array = new string[currentActiveEffects.Keys.Count];
			currentActiveEffects.Keys.CopyTo(array, 0);
			string[] array2 = array;
			foreach (string key in array2)
			{
				if (currentActiveEffects.ContainsKey(key))
				{
					CombatEffectBase combatEffectBase = currentActiveEffects[key];
					if (combatEffectBase.gameObject != null)
					{
						combatEffectBase.gameObject.SendMessage("OnDamage", damage / incomingDamageMultiplier, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}
		if (health == null || !(health.Value > health.MinimumValue))
		{
			if (health != null)
				CspUtils.DebugLog("TakeDamage RETURN health.Value=" + health.Value + " health.MinimumValue=" + health.MinimumValue);
			return;
		}
		float value = health.Value;
		health.Value -= damage;
		if (displayDamageInfo)
		{
			CspUtils.DebugLog(base.gameObject.name + " is taking : " + damage + " damage.  Incoming multiplier is: " + incomingDamageMultiplier + ". " + source.name + " dealt this.  Health was: " + value + ", is now: " + health.Value);
		}
		if (health.Value != health.MinimumValue)
		{
			CspUtils.DebugLog("TakeDamage RETURN health.Value=" + health.Value + " health.MinimumValue=" + health.MinimumValue);			
			return;
		}
		CharacterGlobals characterGlobals = source.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
		if (characterGlobals != null && characterGlobals.networkComponent != null && !characterGlobals.networkComponent.IsOwner() && characterGlobals.combatController.faction == Faction.Player)
		{
			health.Value = 1f;
		}
		else if (!networkComponent.IsOwner() && (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Player) != 0)
		{
			health.Value = 1f;
		}
		else if ((charGlobals.spawnData.spawnType & CharacterSpawn.Type.Player) != 0)
		{
			if (networkComponent.IsOwner())
			{
				killed(source, 2f);
			}
		}
		else
		{
			CspUtils.DebugLog("TakeDamage KILLED");
			killed(source, 0.4f);
		}
	}

	public virtual void killed(GameObject killer, float duration)
	{
		if (isKilled)
		{
			return;
		}
		isKilled = true;
		if (health != null)
		{
			health.Value = health.MinimumValue;
		}
		if (charGlobals.polymorphController != null)
		{
			if (charGlobals.polymorphController.IsAPolymorph())
			{
				CharacterGlobals originalCharacter = charGlobals.polymorphController.GetOriginalCharacter();
				originalCharacter.combatController.killed(killer, duration);
				return;
			}
			if (charGlobals.polymorphController.IsPolymorphed())
			{
				charGlobals.polymorphController.EndPolymorphOnDeath();
			}
		}
		Transform transform = Utils.FindNodeInChildren(base.transform, "ClickBox");
		if (transform != null)
		{
			transform.gameObject.active = false;
		}
		CombatController sourceCharacterCombat = (!(killer == null)) ? (killer.GetComponent(typeof(CombatController)) as CombatController) : null;
		CharacterGlobals characterGlobals = (!(killer == null)) ? (killer.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals) : null;
		if (!IsPlayer())
		{
			if (killer != null && characterGlobals != null && characterGlobals.spawnData != null && (characterGlobals.spawnData.spawnType & CharacterSpawn.Type.Ally) != 0)
			{
				sourceCharacterCombat = null;
			}
			AppShell.Instance.EventMgr.Fire(base.gameObject, new CombatCharacterKilledMessage(base.gameObject, this, sourceCharacterCombat));
		}
		else
		{
			AppShell.Instance.EventMgr.Fire(base.gameObject, new CombatPlayerKilledMessage(base.gameObject, this, sourceCharacterCombat));
		}
		if (charGlobals.spawnData != null && (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Boss) != 0)
		{
			AppShell.Instance.EventMgr.Fire(base.gameObject, new BossAIControllerBrawler.BossDieEvent(base.gameObject));
			if (BossAIControllerBrawler.BossCount == 0)
			{
				AppShell.Instance.EventMgr.Fire(base.gameObject, new BossAIControllerBrawler.BossBattleEndEvent(base.gameObject));
			}
		}
		bool flag = charGlobals.spawnData != null && charGlobals.spawnData.spawnType == CharacterSpawn.Type.LocalPlayer;
		bool flag2 = charGlobals.spawnData != null && (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Player) != 0;
		if (flag || (characterGlobals != null && characterGlobals.networkComponent != null && characterGlobals.networkComponent.IsOwner() && ((characterGlobals.spawnData.spawnType & CharacterSpawn.Type.Player) != 0 || (characterGlobals.spawnData.spawnType & CharacterSpawn.Type.Ally) != 0)) || (characterGlobals == null && (!flag2 || flag)))
		{
			if (characterGlobals == null && killer != null && killer.GetComponent(typeof(NetworkComponent)) == null)
			{
				killer = null;
			}
			NetActionDie action = new NetActionDie(base.gameObject, killer, duration);
			networkComponent.QueueNetActionIgnoringOwnership(action);
		}
		BehaviorDie behaviorDie = (!(charGlobals.spawnData != null) || (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Boss) == 0 || BossAIControllerBrawler.BossCount != 0 || charGlobals.spawnData.spawner.overrideBossDeathSequence) ? (behaviorManager.forceChangeBehavior(typeof(BehaviorDie)) as BehaviorDie) : (behaviorManager.forceChangeBehavior(typeof(BehaviorDieBoss)) as BehaviorDieBoss);
		if (killer == null)
		{
			killer = base.gameObject;
		}
		ActiveMission activeMission = (ActiveMission)AppShell.Instance.SharedHashTable["ActiveMission"];
		if (activeMission != null && activeMission.IsSurvivalMode)
		{
			behaviorDie.survivalMode = true;
		}
		behaviorDie.Initialize(killer, duration);
		if (charGlobals.spawnData != null && !string.IsNullOrEmpty(charGlobals.spawnData.spawner.overrideDeathSequenceName))
		{
			createEffect(charGlobals.spawnData.spawner.overrideDeathSequenceName, base.gameObject);
		}
		else
		{
			createEffect(deathEffectName, base.gameObject);
		}
		if (displayPopups && !behaviorDie.deathAnimOverride)
		{
			GameObject gameObject = createEffect("death_KO_sequence", null);
			Vector3 position = base.transform.position;
			if (characterController != null)
			{
				position.y += characterController.height / 2f;
			}
			gameObject.transform.position = position;
		}
	}

	public virtual void revive()
	{
		isKilled = false;
	}

	public virtual bool IsObjectEnemy(GameObject obj)
	{
		CombatController combatController = obj.GetComponent(typeof(CombatController)) as CombatController;
		if ((bool)combatController)
		{
			return IsControllerEnemy(combatController);
		}
		return false;
	}

	public virtual bool IsControllerEnemy(CombatController targetCombatController)
	{
		if (targetCombatController.faction == Faction.Neutral)
		{
			return false;
		}
		if (targetCombatController.faction == Faction.Environment != getCurrentAttackData(false, false).environmentalAttack)
		{
			return false;
		}
		if (BrawlerController.Instance == null)
		{
			return false;
		}
		if (faction == targetCombatController.faction && !BrawlerController.Instance.pvpEnabled() && !targetCombatController.InCombatState(CombatState.FactionHits))
		{
			return false;
		}
		if (targetCombatController.minimumTargetableDistance > 0f)
		{
			float magnitude = (targetCombatController.transform.position - base.gameObject.transform.position).magnitude;
			if (magnitude > targetCombatController.minimumTargetableDistance)
			{
				return false;
			}
		}
		return true;
	}

	public virtual bool IsPlayer()
	{
		return false;
	}

	public virtual bool IsEnemy()
	{
		if (IsPlayer())
		{
			return false;
		}
		return faction == Faction.Enemy || IsCharmed;
	}

	public void dropPickup()
	{
		if ((faction == Faction.Player && charGlobals != null && (charGlobals.polymorphController == null || !charGlobals.polymorphController.IsPolymorphed())) || dropTableName == null || !(dropTableName != string.Empty))
		{
			return;
		}
		string text = BrawlerController.Instance.pickDrop(dropTableName);
		if (text != null && BrawlerController.Instance.allowPickupSpawn)
		{
			Vector3 newPosition = base.gameObject.transform.position;
			Vector3 position = base.gameObject.transform.position;
			position.y += 1.5f;
			RaycastHit hitInfo;
			if (Physics.Raycast(position, Vector3.down, out hitInfo, 100f, 804756969))
			{
				newPosition = hitInfo.point;
			}
			BrawlerController.Instance.spawnPickup(text, newPosition, GoNetId.Invalid);
		}
	}

	public virtual bool isHealthInitialized()
	{
		return health != null;
	}

	public virtual float getHealth()
	{
		if (health == null)
		{
			return 1f;
		}
		return health.Value;
	}

	public virtual float getMinHealth()
	{
		if (health == null)
		{
			return 1f;
		}
		return health.MinimumValue;
	}

	public virtual float getMaxHealth()
	{
		if (health == null)
		{
			return 1f;
		}
		return health.MaximumValue;
	}

	public virtual void restoreHealth()
	{
		if (health != null)
		{
			health.Value = health.MaximumValue;
		}
		revive();
		NetGameManager game = AppShell.Instance.ServerConnection.Game;
		if (game != null && networkComponent != null && networkComponent.IsOwner())
		{
			NetActionPositionFull action = new NetActionPositionFull(base.gameObject);
			networkComponent.QueueNetAction(action);
		}
	}

	public virtual void addHealth(float amount)
	{
		health.Value += amount;
		if (health.Value > health.MaximumValue)
		{
			health.Value = health.MaximumValue;
		}
	}

	public virtual void setHealth(float amount)
	{
		if (health != null)
		{
			health.Value = amount;
		}
	}

	public virtual void setMinHealth(float amount)
	{
		if (health != null)
		{
			health.MinimumValue = amount;
		}
	}

	public virtual void setMaxHealth(float amount)
	{
		if (health != null)
		{
			health.MaximumValue = amount;
		}
	}

	protected virtual void onDealtDamage(CombatController TargetController, float damage, AttackData attackData)
	{
	}

	public AttackData getAttackData(int index)
	{
		if (attackChain[currentLChain].Count < index)
		{
			CspUtils.DebugLog(base.gameObject.name + " tried to get attack data for nonexistant attack " + index);
			return attackChain[0][0];
		}
		return attackChain[currentLChain][index - 1];
	}

	public AttackData getAttackDataByName(string attackName)
	{
		foreach (AttackData item in attackChain[currentLChain])
		{
			if (item.attackName == attackName)
			{
				return item;
			}
		}
		AttackData[] array = currentSecondaryAttackChain;
		foreach (AttackData attackData in array)
		{
			if (attackData.attackName == attackName)
			{
				return attackData;
			}
		}
		return null;
	}

	public AttackData getSecondaryAttackData(int index)
	{
		if (currentSecondaryAttackChain.Length < index)
		{
			CspUtils.DebugLog(base.gameObject.name + " tried to get attack data for nonexistant secondary attack " + index + " (d1)");
			if (currentSecondaryAttackChain.Length > 0)
			{
				return currentSecondaryAttackChain[0];
			}
			return attackChain[currentLChain][0];
		}
		return currentSecondaryAttackChain[index - 1];
	}

	public AttackData GetSecondaryAttackDataAtRank(int index, int rank)
	{
		rank--;
		if (secondaryAttackChain.Length <= index)
		{
			CspUtils.DebugLog(base.gameObject.name + " tried to get attack data for nonexistant secondary attack " + index + " (d2)");
			index = ((secondaryAttackChain.Length <= 0) ? (-1) : 0);
		}
		if (index < 0)
		{
			return attackChain[currentLChain][0];
		}
		if (secondaryAttackChain[index].Length <= rank)
		{
			CspUtils.DebugLog(base.gameObject.name + " tried to get attack data for nonexistant secondary attack " + index + " at rank " + rank + " (d3)");
			rank = ((secondaryAttackChain[index].Length <= 0) ? (-1) : 0);
		}
		if (rank < 0)
		{
			return attackChain[currentLChain][0];
		}
		return secondaryAttackChain[index][rank];
	}

	public virtual AttackData getCurrentAttackData(bool secondaryAttack, bool chainAttack)
	{
		return getCurrentAttackData(secondaryAttack, chainAttack, null);
	}

	public virtual AttackData getCurrentAttackData(bool secondaryAttack, bool chainAttack, GameObject target)
	{
		if (motionController.carriedThrowable != null)
		{
			return AttackDataManager.Instance.getAttackData(motionController.carriedThrowable.attackName);
		}
		if (secondaryAttack)
		{
			return getSecondaryAttackData(selectedSecondaryAttack);
		}
		return getAttackData(currentAttack);
	}

	public virtual List<AttackData> getAllAttackData()
	{
		List<AttackData> list = new List<AttackData>();
		foreach (List<AttackData> item in attackChain)
		{
			list.AddRange(item);
		}
		AttackData[][] array = secondaryAttackChain;
		foreach (AttackData[] collection in array)
		{
			list.AddRange(collection);
		}
		return list;
	}

	protected AttackData GetChildAttack(AttackData attack)
	{
		float num = 0f;
		AttackData[] array = new AttackData[attack.attackChildNames.Length];
		for (int i = 0; i < attack.attackChildNames.Length; i++)
		{
			array[i] = AttackDataManager.Instance.getAttackData(attack.attackChildNames[i]);
			num += array[i].chance;
		}
		float num2 = UnityEngine.Random.Range(0f, num);
		num = 0f;
		AttackData[] array2 = array;
		foreach (AttackData attackData in array2)
		{
			num += attackData.chance;
			if (num2 < num)
			{
				return attackData;
			}
		}
		return attack;
	}

	protected bool HasChildAttack(AttackData attack)
	{
		return attack.attackChildNames != null && attack.attackChildNames.Length > 0;
	}

	protected bool IsChildAttack(AttackData parentAttack, AttackData childAttack)
	{
		if (parentAttack == null || childAttack == null)
		{
			return false;
		}
		string[] attackChildNames = parentAttack.attackChildNames;
		foreach (string b in attackChildNames)
		{
			if (childAttack.attackName == b)
			{
				return true;
			}
		}
		return false;
	}

	public GameObject createEffect(string effectName, GameObject newParent)
	{
		return createEffect(effectName, newParent, null, false);
	}

	public GameObject createEffect(string effectName, GameObject newParent, bool remote)
	{
		return createEffect(effectName, newParent, null, remote);
	}

	public GameObject createEffect(string effectName, GameObject newParent, EffectSequenceList effectSequenceSource)
	{
		return createEffect(effectName, newParent, effectSequenceSource, false);
	}

	public GameObject createEffect(string effectName, GameObject newParent, EffectSequenceList effectSequenceSource, bool remote)
	{
		if (effectName == null)
		{
			return null;
		}
		if (CardGameController.Instance != null && CardGameController.EffectBlacklist.ContainsKey(effectName))
		{
			return null;
		}
		if (effectSequenceSource == null)
		{
			effectSequenceSource = this.effectSequenceSource;
			if (effectSequenceSource == null)
			{
				this.effectSequenceSource = GetComponent<EffectSequenceList>();
				effectSequenceSource = this.effectSequenceSource;
			}
		}
		GameObject gameObject = effectSequenceSource.TryGetEffectSequencePrefabByName(effectName) as GameObject;
		if (gameObject != null)
		{
			NetworkComponent x = gameObject.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
			if (remote || x == null || (charGlobals != null && (charGlobals.networkComponent == null || charGlobals.networkComponent.IsOwner())))
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, gameObject.transform.localPosition, gameObject.transform.localRotation) as GameObject;
				if (gameObject2 != null && newParent != null)
				{
					Utils.AttachGameObject(newParent, gameObject2);
				}
				if (!remote && gameObject2 != null && x != null && charGlobals != null && charGlobals.networkComponent != null)
				{
					charGlobals.networkComponent.AnnounceObjectSpawn(gameObject2, "CombatController", gameObject.name, (!(newParent == null)) ? newParent.transform.root.gameObject : null);
				}
				return gameObject2;
			}
		}
		else
		{
			EffectSequence effectSequence = GUIManager.Instance.LoadEffectSequence(effectName);
			if (effectSequence != null)
			{
				effectSequence.SetParent(newParent);
				effectSequence.Initialize(null, null, null);
				effectSequence.StartSequence();
			}
			else
			{
				CspUtils.DebugLog("createEffect sequence failed because " + effectName + " could not be located by TryGetEffectSequencePrefabByName()");
			}
		}
		return null;
	}

	public virtual void clearCombatEffects()
	{
		if (currentActiveEffects.Values.Count > 0)
		{
			string[] array = new string[currentActiveEffects.Keys.Count];
			currentActiveEffects.Keys.CopyTo(array, 0);
			string[] array2 = array;
			foreach (string key in array2)
			{
				currentActiveEffects[key].removeCombatEffect(false);
			}
			currentActiveEffects.Clear();
		}
	}

	public void removeCombatEffectRemote(string combatEffectName)
	{
		removeCombatEffectInternal(combatEffectName);
	}

	public virtual void removeCombatEffect(string combatEffectName)
	{
		if (networkComponent == null || !networkComponent.IsOwnedBySomeoneElse())
		{
			if (networkComponent != null && networkComponent.IsOwner())
			{
				NetActionCombatEffect action = new NetActionCombatEffect(combatEffectName, true, null, false);
				networkComponent.QueueNetAction(action);
			}
			removeCombatEffectInternal(combatEffectName);
		}
	}

	protected virtual void removeCombatEffectInternal(string combatEffectName)
	{
		if (currentActiveEffects.ContainsKey(combatEffectName))
		{
			currentActiveEffects[combatEffectName].removeCombatEffect(true);
			currentActiveEffects.Remove(combatEffectName);
		}
	}

	public virtual void createCombatEffectRemote(string newCombatEffectName, GameObject source, bool usePrefabSource)
	{
		CombatController sourceCombatController = null;
		if (source != null)
		{
			sourceCombatController = (source.GetComponent(typeof(CombatController)) as CombatController);
		}
		createCombatEffectInternal(newCombatEffectName, sourceCombatController, usePrefabSource);
	}

	public virtual void createCombatEffect(string newCombatEffectName, CombatController sourceCombatController, bool usePrefabSource)
	{
		if (!(networkComponent == null) && networkComponent.IsOwnedBySomeoneElse())
		{
			return;
		}
		if (networkComponent != null && networkComponent.IsOwner())
		{
			GameObject newSource = null;
			if (sourceCombatController != null)
			{
				newSource = sourceCombatController.gameObject;
			}
			NetActionCombatEffect action = new NetActionCombatEffect(newCombatEffectName, false, newSource, usePrefabSource);
			networkComponent.QueueNetAction(action);
		}
		createCombatEffectInternal(newCombatEffectName, sourceCombatController, usePrefabSource);
	}

	protected virtual void createCombatEffectInternal(string newCombatEffectName, CombatController sourceCombatController, bool usePrefabSource)
	{
		if (BrawlerController.Instance == null && CardGameController.Instance == null)
		{
			return;
		}
		CombatEffectData combatEffectData = CombatEffectDataWarehouse.Instance.getCombatEffectData(newCombatEffectName);
		if (combatEffectData == null)
		{
			CspUtils.DebugLog("Failed to find combat effect data " + newCombatEffectName);
			return;
		}
		if (!string.IsNullOrEmpty(combatEffectData.resisterName))
		{
			CombatEffectResister combatEffectResister = CombatEffectDataWarehouse.Instance.GetCombatEffectResister(combatEffectData.resisterName);
			if (combatEffectResister == null)
			{
				CspUtils.DebugLog("Failed to find combat effect resister: " + combatEffectData.resisterName);
			}
			else if (combatEffectResister.IsResister(base.gameObject))
			{
				if (combatEffectResister.ResisterHasEffect())
				{
					createEffect(combatEffectResister.ResisterEffect, base.gameObject);
				}
				return;
			}
		}
		if (currentActiveEffects.ContainsKey(newCombatEffectName))
		{
			if (!combatEffectData.reapplyEffect)
			{
				return;
			}
			currentActiveEffects[newCombatEffectName].removeCombatEffect(false);
			currentActiveEffects.Remove(newCombatEffectName);
		}
		GameObject gameObject = new GameObject(newCombatEffectName);
		Utils.AttachGameObject(base.gameObject, gameObject);
		CombatEffectBase combatEffectBase = gameObject.AddComponent(typeof(CombatEffectBase)) as CombatEffectBase;
		combatEffectBase.storedLocation = currentActiveEffects;
		currentActiveEffects.Add(newCombatEffectName, combatEffectBase);
		currentActiveEffects[newCombatEffectName].Initialize(combatEffectData, sourceCombatController);
		currentActiveEffects[newCombatEffectName].RegisterIcon();
		if (combatEffectData.HasTimerData())
		{
			CombatEffectTimer combatEffectTimer = gameObject.AddComponent(typeof(CombatEffectTimer)) as CombatEffectTimer;
			combatEffectTimer.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasAblativeData())
		{
			CombatEffectAblative combatEffectAblative = gameObject.AddComponent(typeof(CombatEffectAblative)) as CombatEffectAblative;
			combatEffectAblative.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasStatModifierData())
		{
			CombatEffectStatModifier combatEffectStatModifier = gameObject.AddComponent(typeof(CombatEffectStatModifier)) as CombatEffectStatModifier;
			combatEffectStatModifier.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasPowerBarTimerData())
		{
			CombatEffectPowerBarTimer combatEffectPowerBarTimer = gameObject.AddComponent(typeof(CombatEffectPowerBarTimer)) as CombatEffectPowerBarTimer;
			combatEffectPowerBarTimer.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasAddComponentData())
		{
			CombatEffectAddComponent combatEffectAddComponent = gameObject.AddComponent<CombatEffectAddComponent>();
			combatEffectAddComponent.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasMessageData())
		{
			CombatEffectMessage combatEffectMessage = gameObject.AddComponent(typeof(CombatEffectMessage)) as CombatEffectMessage;
			combatEffectMessage.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasBehaviorData())
		{
			CombatEffectBehavior combatEffectBehavior = gameObject.AddComponent(typeof(CombatEffectBehavior)) as CombatEffectBehavior;
			combatEffectBehavior.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasReplacementData())
		{
			CombatEffectReplacer combatEffectReplacer = gameObject.AddComponent(typeof(CombatEffectReplacer)) as CombatEffectReplacer;
			combatEffectReplacer.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasProjectileData())
		{
			CombatEffectAttachedProjectile combatEffectAttachedProjectile = gameObject.AddComponent(typeof(CombatEffectAttachedProjectile)) as CombatEffectAttachedProjectile;
			combatEffectAttachedProjectile.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasDOTData())
		{
			CombatEffectDOT combatEffectDOT = gameObject.AddComponent(typeof(CombatEffectDOT)) as CombatEffectDOT;
			combatEffectDOT.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasScaleData())
		{
			CombatEffectScale combatEffectScale = gameObject.AddComponent(typeof(CombatEffectScale)) as CombatEffectScale;
			combatEffectScale.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasVelocityData())
		{
			CombatEffectVelocity combatEffectVelocity = gameObject.AddComponent(typeof(CombatEffectVelocity)) as CombatEffectVelocity;
			combatEffectVelocity.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasSpeedOverTimeData())
		{
			CombatEffectSpeedOverTime combatEffectSpeedOverTime = gameObject.AddComponent(typeof(CombatEffectSpeedOverTime)) as CombatEffectSpeedOverTime;
			combatEffectSpeedOverTime.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasAttackLimiterData())
		{
			CombatEffectAttackLimiter combatEffectAttackLimiter = gameObject.AddComponent(typeof(CombatEffectAttackLimiter)) as CombatEffectAttackLimiter;
			combatEffectAttackLimiter.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasAnimationOverrideData())
		{
			CombatEffectAnimationOverride combatEffectAnimationOverride = gameObject.AddComponent(typeof(CombatEffectAnimationOverride)) as CombatEffectAnimationOverride;
			combatEffectAnimationOverride.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasScenarioEventData())
		{
			CombatEffectScenarioEvent combatEffectScenarioEvent = gameObject.AddComponent(typeof(CombatEffectScenarioEvent)) as CombatEffectScenarioEvent;
			combatEffectScenarioEvent.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasPriorityAttackData())
		{
			CombatEffectPriorityAttack combatEffectPriorityAttack = gameObject.AddComponent<CombatEffectPriorityAttack>();
			combatEffectPriorityAttack.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasAttacksRepeatableData())
		{
			CombatEffectAttacksRepeatable combatEffectAttacksRepeatable = gameObject.AddComponent<CombatEffectAttacksRepeatable>();
			combatEffectAttacksRepeatable.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasDisableInputData())
		{
			CombatEffectDisableInput combatEffectDisableInput = gameObject.AddComponent<CombatEffectDisableInput>();
			combatEffectDisableInput.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasClickboxSizeData())
		{
			CombatEffectClickboxSize combatEffectClickboxSize = gameObject.AddComponent<CombatEffectClickboxSize>();
			combatEffectClickboxSize.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasClickboxScaleData())
		{
			CombatEffectClickboxScale combatEffectClickboxScale = gameObject.AddComponent<CombatEffectClickboxScale>();
			combatEffectClickboxScale.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasClickboxCenterData())
		{
			CombatEffectClickboxCenter combatEffectClickboxCenter = gameObject.AddComponent<CombatEffectClickboxCenter>();
			combatEffectClickboxCenter.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.HasVOData())
		{
			CombatEffectVO combatEffectVO = gameObject.AddComponent<CombatEffectVO>();
			combatEffectVO.Initialize(combatEffectData, sourceCombatController);
		}
		if (combatEffectData.effectPrefabName != null)
		{
			if (sourceCombatController != null && sourceCombatController != this && usePrefabSource)
			{
				sourceCombatController.createEffect(combatEffectData.effectPrefabName, gameObject);
			}
			else
			{
				createEffect(combatEffectData.effectPrefabName, gameObject);
			}
		}
	}

	public GameObject playGetupEffect()
	{
		if (getupEffectName == null)
		{
			return null;
		}
		if (getupEffect == null)
		{
			getupEffect = (effectSequenceSource.GetEffectSequencePrefabByName(getupEffectName) as GameObject);
		}
		if (getupEffect == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(getupEffect) as GameObject;
		Utils.AttachGameObject(base.gameObject, gameObject);
		return gameObject;
	}

	public GameObject playWakeupEffect()
	{
		if (wakeupEffectName == null)
		{
			return null;
		}
		if (wakeupEffect == null)
		{
			wakeupEffect = (effectSequenceSource.GetEffectSequencePrefabByName(wakeupEffectName) as GameObject);
		}
		if (wakeupEffect == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(wakeupEffect) as GameObject;
		Utils.AttachGameObject(base.gameObject, gameObject);
		return gameObject;
	}

	public GameObject playPickupThrowableEffect()
	{
		if (pickupThrowableEffectName == null)
		{
			return null;
		}
		if (pickupThrowableEffect == null)
		{
			pickupThrowableEffect = (effectSequenceSource.GetEffectSequencePrefabByName(pickupThrowableEffectName) as GameObject);
		}
		if (pickupThrowableEffect == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(pickupThrowableEffect) as GameObject;
		Utils.AttachGameObject(base.gameObject, gameObject);
		return gameObject;
	}

	public GameObject playStunEffect()
	{
		if (stunEffectName == null)
		{
			return null;
		}
		if (stunEffect == null)
		{
			stunEffect = (effectSequenceSource.GetEffectSequencePrefabByName(stunEffectName) as GameObject);
		}
		if (stunEffect == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(stunEffect) as GameObject;
		Utils.AttachGameObject(base.gameObject, gameObject);
		return gameObject;
	}

	public virtual void playImpactEffectFromSource(CombatController sourceCombatController, string impactName, bool faceCamera)
	{
		GameObject gameObject = sourceCombatController.effectSequenceSource.GetEffectSequencePrefabByName(impactName) as GameObject;
		if (gameObject != null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
			gameObject2.SendMessage("AssignCreator", charGlobals, SendMessageOptions.DontRequireReceiver);
			gameObject2.SendMessage("AssignInstigator", sourceCombatController.CharGlobals, SendMessageOptions.DontRequireReceiver);
			gameObject2.transform.position = TargetPosition;
			if (faceCamera)
			{
				gameObject2.transform.LookAt(Camera.main.transform);
			}
			gameObject2.transform.parent = base.transform;
		}
		else
		{
			CspUtils.DebugLog("Specified impact effect '" + impactName + "' does not exist in attack effect list for " + sourceCombatController.gameObject.name);
		}
	}

	public void playBonusIncommingEffects()
	{
		foreach (string bonusIncommingEffect in bonusIncommingEffects)
		{
			GameObject gameObject = effectSequenceSource.GetEffectSequencePrefabByName(bonusIncommingEffect) as GameObject;
			if (gameObject != null)
			{
				GameObject child = UnityEngine.Object.Instantiate(gameObject) as GameObject;
				Utils.AttachGameObject(base.gameObject, child);
			}
		}
	}

	public virtual GameObject playImpactEffect()
	{
		playBonusIncommingEffects();
		if (characterImpactEffect == null && characterImpactEffectName == null)
		{
			return null;
		}
		if (characterImpactEffect == null)
		{
			characterImpactEffect = (effectSequenceSource.GetEffectSequencePrefabByName(characterImpactEffectName) as GameObject);
		}
		if (characterImpactEffect == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(characterImpactEffect) as GameObject;
		Utils.AttachGameObject(base.gameObject, gameObject);
		return gameObject;
	}

	public void playImpactMatrixEffect(ImpactMatrix.Type attackType, CombatController source)
	{
		attackType = ImpactMatrix.Resolve(attackType, (!(source != null)) ? ImpactMatrix.Type.Silent : source.DefaultImpactMatrixType);
		string text = AttackDataManager.Instance.getImpactMatrixEffect(attackType, ImpactMatrixType);
		if (text == null)
		{
			return;
		}
		if (impactMatrixEffect == null)
		{
			impactMatrixEffect = (effectSequenceSource.GetEffectSequencePrefabByName(text) as GameObject);
		}
		if (impactMatrixEffect != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(impactMatrixEffect) as GameObject;
			gameObject.SendMessage("AssignCreator", charGlobals, SendMessageOptions.DontRequireReceiver);
			if (source != null)
			{
				gameObject.SendMessage("AssignInstigator", source.CharGlobals, SendMessageOptions.DontRequireReceiver);
			}
			Utils.AttachGameObject(base.gameObject, gameObject);
		}
	}

	public virtual void playKnockdownEffect()
	{
		if (knockdownEffectName != null)
		{
			if (knockdownEffect == null)
			{
				knockdownEffect = (effectSequenceSource.GetEffectSequencePrefabByName(knockdownEffectName) as GameObject);
			}
			if (!(knockdownEffect == null))
			{
				GameObject child = UnityEngine.Object.Instantiate(knockdownEffect) as GameObject;
				Utils.AttachGameObject(base.gameObject, child);
			}
		}
	}

	public void playLaunchLandEffect()
	{
		if (launchLandEffectName != null)
		{
			if (launchLandEffect == null)
			{
				launchLandEffect = (effectSequenceSource.GetEffectSequencePrefabByName(launchLandEffectName) as GameObject);
			}
			if (!(launchLandEffect == null))
			{
				GameObject child = UnityEngine.Object.Instantiate(launchLandEffect) as GameObject;
				Utils.AttachGameObject(base.gameObject, child);
			}
		}
	}

	public void playDespawnEffect()
	{
		if (despawnEffectName != null)
		{
			GameObject gameObject = createEffect(despawnEffectName, null);
			gameObject.transform.position = base.gameObject.transform.position;
			gameObject.transform.rotation = base.gameObject.transform.rotation;
		}
	}

	public void SelectSecondaryAttack(KeyCode code)
	{
		int num = 0;
		switch (code)
		{
		case KeyCode.Alpha1:
			num = 1;
			break;
		case KeyCode.Alpha2:
			num = 2;
			break;
		case KeyCode.Alpha3:
			num = 3;
			break;
		case KeyCode.Alpha4:
			num = 4;
			break;
		case KeyCode.Alpha5:
			num = 5;
			break;
		}
		if (num > 0 && num <= maximumSecondaryAttackChain)
		{
			SetSecondaryAttack(num);
		}
	}

	public virtual void SetSecondaryAttack(int newSelection)
	{
		if (newSelection > maximumSecondaryAttackChain)
		{
			newSelection = maximumSecondaryAttackChain;
		}
		if (newSelection < 1)
		{
			newSelection = 1;
		}
		selectedSecondaryAttack = newSelection;
	}

	protected bool IsSecondaryAttack(AttackData attack, out int index)
	{
		index = 0;
		AttackData[][] array = secondaryAttackChain;
		foreach (AttackData[] array2 in array)
		{
			AttackData[] array3 = array2;
			foreach (AttackData attackData in array3)
			{
				if (attackData == attack)
				{
					return true;
				}
			}
			index++;
		}
		return false;
	}

	protected virtual bool IsPrimaryAttack(AttackData attack, out int index)
	{
		index = 0;
		foreach (AttackData item in attackChain[currentLChain])
		{
			if (item == attack)
			{
				return true;
			}
			index++;
		}
		return false;
	}

	public void ReturningProjectileArrived()
	{
		BehaviorAttackBase behaviorAttackBase = behaviorManager.getBehavior() as BehaviorAttackBase;
		if (behaviorAttackBase != null)
		{
			behaviorAttackBase.OnProjectileReturned();
		}
	}

	public void ProjectileDestroyed()
	{
		BehaviorAttackBase behaviorAttackBase = behaviorManager.getBehavior() as BehaviorAttackBase;
		if (behaviorAttackBase != null)
		{
			behaviorAttackBase.OnProjectileDestroyed();
		}
	}

	public virtual void GetTargetTransform()
	{
		targetTransform = Utils.FindNodeInChildren(base.gameObject.transform, targetBone);
		targetHeight = 0f;
		if (targetTransform == null)
		{
			targetTransform = base.gameObject.transform;
			if (characterController != null)
			{
				targetHeight = characterController.height / 2f;
			}
		}
	}

	public virtual GameObject RemoteSpawn(Vector3 spawnLoc, Quaternion spawnRot, GoNetId newID, string prefabName, GameObject parent)
	{
		GameObject gameObject = createEffect(prefabName, parent, true);
		if (parent != null)
		{
			gameObject.transform.localPosition = spawnLoc;
		}
		else
		{
			gameObject.transform.position = spawnLoc;
		}
		if (newID.IsValid())
		{
			NetworkComponent networkComponent = gameObject.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
			if (networkComponent != null)
			{
				networkComponent.goNetId = newID;
			}
		}
		return gameObject;
	}

	protected float recalcDamageMultiplier(List<float> increaseMultipliers, List<float> decreaseMultipliers)
	{
		float num = 1f;
		foreach (float increaseMultiplier in increaseMultipliers)
		{
			float num2 = increaseMultiplier;
			if (num2 > num)
			{
				num = num2;
			}
		}
		float num3 = 1f;
		foreach (float decreaseMultiplier in decreaseMultipliers)
		{
			float num4 = decreaseMultiplier;
			if (num4 < num3)
			{
				num3 = num4;
			}
		}
		return num * num3;
	}

	public void addDamageMultiplier(float multiplier)
	{
		if (multiplier > 1f)
		{
			damageIncreaseMultipliers.Add(multiplier);
		}
		else
		{
			damageDecreaseMultipliers.Add(multiplier);
		}
		damageMultiplier = recalcDamageMultiplier(damageIncreaseMultipliers, damageDecreaseMultipliers);
	}

	public void removeDamageMultiplier(float multiplier)
	{
		if (multiplier > 1f)
		{
			damageIncreaseMultipliers.Remove(multiplier);
		}
		else
		{
			damageDecreaseMultipliers.Remove(multiplier);
		}
		damageMultiplier = recalcDamageMultiplier(damageIncreaseMultipliers, damageDecreaseMultipliers);
	}

	public void addIncomingDamageMultiplier(float multiplier)
	{
		if (multiplier > 1f)
		{
			incomingDamageIncreaseMultipliers.Add(multiplier);
		}
		else
		{
			incomingDamageDecreaseMultipliers.Add(multiplier);
		}
		incomingDamageMultiplier = recalcDamageMultiplier(incomingDamageIncreaseMultipliers, incomingDamageDecreaseMultipliers);
	}

	public void removeIncomingDamageMultiplier(float multiplier)
	{
		if (multiplier > 1f)
		{
			incomingDamageIncreaseMultipliers.Remove(multiplier);
		}
		else
		{
			incomingDamageDecreaseMultipliers.Remove(multiplier);
		}
		incomingDamageMultiplier = recalcDamageMultiplier(incomingDamageIncreaseMultipliers, incomingDamageDecreaseMultipliers);
	}

	public bool InStealthMode()
	{
		return stealthCombatEffectActive != string.Empty;
	}

	public void ExitStealthMode()
	{
		if (stealthCombatEffectActive != string.Empty)
		{
			removeCombatEffect(stealthCombatEffectActive);
		}
	}

	public void OnEnterStealthMode(string stealthCombatEffectName)
	{
		stealthCombatEffectActive = stealthCombatEffectName;
		AIControllerBrawler[] array = Utils.FindObjectsOfType<AIControllerBrawler>();
		foreach (AIControllerBrawler aIControllerBrawler in array)
		{
			aIControllerBrawler.EndAttackOnTarget(base.gameObject);
		}
	}

	public void OnExitStealthMode()
	{
		stealthCombatEffectActive = string.Empty;
	}

	public void setRecoilAllow(bool allowed)
	{
		recoilAllowed = allowed;
	}

	public Faction GetEnemyFaction()
	{
		if (faction == Faction.Player)
		{
			return Faction.Enemy;
		}
		if (faction == Faction.Enemy)
		{
			return Faction.Player;
		}
		return Faction.None;
	}

	public void ChangeFaction(Faction newFaction)
	{
		if (_oldFaction == newFaction || newFaction == Faction.Count)
		{
			return;
		}
		if (_oldFaction != Faction.None && _factionLists[(int)_oldFaction] != null)
		{
			_factionLists[(int)_oldFaction].Remove(this);
		}
		Faction oldFaction = _oldFaction;
		_oldFaction = (faction = newFaction);
		if (faction != Faction.None)
		{
			if (_factionLists[(int)faction] == null)
			{
				_factionLists[(int)faction] = new List<CombatController>();
			}
			if (!_factionLists[(int)faction].Contains(this))
			{
				_factionLists[(int)faction].Add(this);
			}
		}
		if (oldFaction != Faction.None && newFaction != Faction.None)
		{
			AppShell.Instance.EventMgr.Fire(base.gameObject, new EntityFactionChangeMessage(base.gameObject, oldFaction, newFaction));
		}
	}

	private void RemoveFromFactionList()
	{
		if (_oldFaction != Faction.None && _oldFaction != Faction.Count && _factionLists[(int)_oldFaction] != null)
		{
			_factionLists[(int)_oldFaction].Remove(this);
		}
		_oldFaction = Faction.None;
	}

	public static void ClearFactionLists()
	{
		List<CombatController>[] factionLists = _factionLists;
		foreach (List<CombatController> list in factionLists)
		{
			if (list != null)
			{
				list.Clear();
			}
		}
	}

	public static List<CombatController> GetFactionList(Faction desired)
	{
		return (desired == Faction.None || desired == Faction.Count) ? null : _factionLists[(int)desired];
	}

	public static bool FactionExists(Faction desired)
	{
		List<CombatController> factionList = GetFactionList(desired);
		return factionList != null && factionList.Count > 0;
	}

	public void SetMaxPushbackVelocity(float max)
	{
		_maxPushbackList.Add(max);
		_maxPushbackVelocity = GetMinimum(_maxPushbackList, float.MaxValue);
	}

	public void RemoveMaxPushbackVelocity(float max)
	{
		_maxPushbackList.Remove(max);
		_maxPushbackVelocity = GetMinimum(_maxPushbackList, float.MaxValue);
	}

	public void SetMaxLaunchVelocity(float max)
	{
		_maxLaunchList.Add(max);
		_maxLaunchVelocity = GetMinimum(_maxLaunchList, float.MaxValue);
	}

	public void RemoveMaxLaunchVelocity(float max)
	{
		_maxLaunchList.Remove(max);
		_maxLaunchVelocity = GetMinimum(_maxLaunchList, float.MaxValue);
	}

	public void SetMaxPushbackDuration(float max)
	{
		_maxPushbackDurationList.Add(max);
		_maxPushbackDuration = GetMinimum(_maxPushbackDurationList, float.MaxValue);
	}

	public void RemoveMaxPushbackDuration(float max)
	{
		_maxPushbackDurationList.Remove(max);
		_maxPushbackDuration = GetMinimum(_maxPushbackDurationList, float.MaxValue);
	}

	public void SetMaxRegularAttackChain(int max)
	{
		_maxRegularAttackChainList.Add(max);
		_maxRegularAttackChain = GetMinimum(_maxRegularAttackChainList, int.MaxValue);
	}

	public void RemoveMaxRegularAttackChain(int max)
	{
		_maxRegularAttackChainList.Remove(max);
		_maxRegularAttackChain = GetMinimum(_maxRegularAttackChainList, int.MaxValue);
	}

	public void LockSecondaryAttack(int attack)
	{
		_lockedSecondaryAttackList.Add(attack);
	}

	public void RemoveSecondaryAttackLock(int attack)
	{
		_lockedSecondaryAttackList.Remove(attack);
	}

	public float GetMaxPushbackVelocity()
	{
		return _maxPushbackVelocity;
	}

	public float GetMaxLaunchVelocity()
	{
		return _maxLaunchVelocity;
	}

	public float GetMaxPushbackDuration()
	{
		return _maxPushbackDuration;
	}

	public int GetMaxRegularAttackChain()
	{
		return _maxRegularAttackChain;
	}

	public bool IsSecondaryAttackLocked(int attack)
	{
		return _lockedSecondaryAttackList.Contains(attack);
	}

	private T GetMinimum<T>(List<T> valueList, T min) where T : IComparable
	{
		foreach (T value in valueList)
		{
			if (value.CompareTo(min) < 0)
			{
				min = value;
			}
		}
		return min;
	}

	public void SetCombatState(CombatState state)
	{
		currentCombatState |= state;
	}

	public void ClearCombatState(CombatState state)
	{
		currentCombatState &= ~state;
	}

	public bool InCombatState(CombatState state)
	{
		return (currentCombatState & state) != 0;
	}

	public void Charm(Faction newFaction, bool canFactionDamage, bool canCharmBreak, CombatController charmer)
	{
		ChangeFaction(newFaction);
		SetCombatState(CombatState.Charmed);
		if (canFactionDamage)
		{
			SetCombatState(CombatState.FactionHits);
		}
		if (canCharmBreak)
		{
			SetCombatState(CombatState.CharmBreakable);
		}
		this.charmer = charmer;
		if (charGlobals.brawlerCharacterAI != null)
		{
			charGlobals.brawlerCharacterAI.EndAttackOnFaction(newFaction);
		}
	}

	public void EndCharm(Faction oldFaction)
	{
		ChangeFaction(oldFaction);
		ClearCombatState(CombatState.Charmed);
		ClearCombatState(CombatState.FactionHits);
		ClearCombatState(CombatState.CharmBreakable);
		charmer = null;
		if (charGlobals.brawlerCharacterAI != null)
		{
			charGlobals.brawlerCharacterAI.EndAttackOnFaction(oldFaction);
		}
	}

	public void BreakCharm()
	{
		charGlobals.polymorphController.EndPolymorphToFaction();
	}

	public void HideCombatant(bool updateTargetHeight)
	{
		if (!InCombatState(CombatState.Hidden))
		{
			SetCombatState(CombatState.Hidden);
			if (updateTargetHeight)
			{
				targetHeightMinimumEnabled = false;
				targetHeight -= 100f;
			}
			Vector3 vector = base.gameObject.transform.position + Vector3.up * 100f;
			if (charGlobals != null)
			{
				charGlobals.motionController.setDestination(vector);
				charGlobals.motionController.teleportTo(vector);
			}
			else
			{
				base.gameObject.transform.position = vector;
			}
		}
	}

	public void ShowCombatant(Vector3 position, Quaternion rotation, bool updateTargetHeight)
	{
		ClearCombatState(CombatState.Hidden);
		base.gameObject.transform.rotation = rotation;
		if (updateTargetHeight)
		{
			targetHeightMinimumEnabled = true;
			targetHeight += 100f;
		}
		if (charGlobals != null)
		{
			charGlobals.motionController.setDestination(position);
			charGlobals.motionController.teleportTo(position);
			charGlobals.motionController.updateLookDirection();
		}
		else
		{
			base.gameObject.transform.position = position;
		}
	}

	[AnimTag("movestart")]
	public void OnMoveStartAnimTag(AnimationEvent evt)
	{
		IAnimTagListener animTagListener = behaviorManager.getBehavior() as IAnimTagListener;
		if (animTagListener != null)
		{
			animTagListener.OnMoveStartAnimTag(evt);
		}
	}

	[AnimTag("moveend")]
	public void OnMoveEndAnimTag(AnimationEvent evt)
	{
		IAnimTagListener animTagListener = behaviorManager.getBehavior() as IAnimTagListener;
		if (animTagListener != null)
		{
			animTagListener.OnMoveEndAnimTag(evt);
		}
	}

	[AnimTag("chainattack")]
	public void OnChainAttackAnimTag(AnimationEvent evt)
	{
		IAnimTagListener animTagListener = behaviorManager.getBehavior() as IAnimTagListener;
		if (animTagListener != null)
		{
			animTagListener.OnChainAttackAnimTag(evt);
		}
	}

	[AnimTag("collisionenable")]
	public void OnCollisionEnableAnimTag(AnimationEvent evt)
	{
		IAnimTagListener animTagListener = behaviorManager.getBehavior() as IAnimTagListener;
		if (animTagListener != null)
		{
			animTagListener.OnCollisionEnableAnimTag(evt);
		}
	}

	[AnimTag("collisiondisable")]
	public void OnCollisionDisableAnimTag(AnimationEvent evt)
	{
		IAnimTagListener animTagListener = behaviorManager.getBehavior() as IAnimTagListener;
		if (animTagListener != null)
		{
			animTagListener.OnCollisionDisableAnimTag(evt);
		}
	}

	[AnimTag("projectilefire")]
	public void OnProjectileFireAnimTag(AnimationEvent evt)
	{
		IAnimTagListener animTagListener = behaviorManager.getBehavior() as IAnimTagListener;
		if (animTagListener != null)
		{
			animTagListener.OnProjectileFireAnimTag(evt);
		}
	}

	[AnimTag("pinball_start")]
	public void OnPinballStartAnimTag(AnimationEvent evt)
	{
		IAnimTagListener animTagListener = behaviorManager.getBehavior() as IAnimTagListener;
		if (animTagListener != null)
		{
			animTagListener.OnPinballStartAnimTag(evt);
		}
	}

	[AnimTag("pinball_end")]
	public void OnPinballEndAnimTag(AnimationEvent evt)
	{
		IAnimTagListener animTagListener = behaviorManager.getBehavior() as IAnimTagListener;
		if (animTagListener != null)
		{
			animTagListener.OnPinballEndAnimTag(evt);
		}
	}

	[AnimTag("multishot_info")]
	public void OnMultishotInfoAnimTag(AnimationEvent evt)
	{
		IAnimTagListener animTagListener = behaviorManager.getBehavior() as IAnimTagListener;
		if (animTagListener != null)
		{
			animTagListener.OnMultishotInfoAnimTag(evt);
		}
	}

	[AnimTag("trigger_effect")]
	public void OnTriggerEffectAnimTag(AnimationEvent evt)
	{
		IAnimTagListener animTagListener = behaviorManager.getBehavior() as IAnimTagListener;
		if (animTagListener != null)
		{
			animTagListener.OnTriggerEffectAnimTag(evt);
		}
	}

	[AnimTag("attackable")]
	public void OnAttackableAnimTag(AnimationEvent evt)
	{
		Unattackable = false;
	}

	[AnimTag("unattackable")]
	public void OnUnattackableAnimTag(AnimationEvent evt)
	{
		Unattackable = true;
	}

	protected static BrawlerPopupAnimator getPopup(string popupName)
	{
		if (availablePopups != null && availablePopups.ContainsKey(popupName))
		{
			BrawlerPopupAnimator brawlerPopupAnimator = null;
			while (brawlerPopupAnimator == null && availablePopups[popupName].Count > 0)
			{
				brawlerPopupAnimator = availablePopups[popupName][0];
				availablePopups[popupName].RemoveAt(0);
			}
			if (brawlerPopupAnimator == null)
			{
				return null;
			}
			brawlerPopupAnimator.gameObject.active = true;
			return brawlerPopupAnimator;
		}
		return null;
	}

	public static void addPopup(string popupName, BrawlerPopupAnimator newPopup)
	{
		int num = popupName.IndexOf("(Clone)");
		if (num > 0)
		{
			popupName = popupName.Substring(0, num);
		}
		if (availablePopups == null)
		{
			availablePopups = new Dictionary<string, List<BrawlerPopupAnimator>>();
		}
		if (!availablePopups.ContainsKey(popupName))
		{
			availablePopups.Add(popupName, new List<BrawlerPopupAnimator>());
		}
		availablePopups[popupName].Add(newPopup);
	}

	public static GameObject createPopup(Vector3 popupLocation, string popupPrefab, string popupText)
	{
		BrawlerPopupAnimator brawlerPopupAnimator = getPopup(popupPrefab);
		GameObject gameObject;
		if (brawlerPopupAnimator == null)
		{
			gameObject = (UnityEngine.Object.Instantiate(Resources.Load("GUI/3D/" + popupPrefab), popupLocation, Quaternion.identity) as GameObject);
			brawlerPopupAnimator = (gameObject.GetComponent(typeof(BrawlerPopupAnimator)) as BrawlerPopupAnimator);
		}
		else
		{
			gameObject = brawlerPopupAnimator.gameObject;
			gameObject.transform.position = popupLocation;
		}
		brawlerPopupAnimator.SetText(popupText);
		return gameObject;
	}

	public static void addPopupLocator(PopupLocator locator)
	{
		if (availableLocators == null)
		{
			availableLocators = new List<PopupLocator>();
		}
		if (!availableLocators.Contains(locator))
		{
			availableLocators.Add(locator);
		}
	}

	public static void removePopupLocator(PopupLocator locator)
	{
		if (availableLocators != null)
		{
			availableLocators.Remove(locator);
			if (availableLocators.Count == 0)
			{
				availableLocators = null;
			}
		}
	}

	public static bool getPopupLocation(GameObject receiver, out Vector3 location)
	{
		if (availableLocators != null && availableLocators.Count > 0)
		{
			location = availableLocators[0].transform.position;
			return true;
		}
		if (receiver == null)
		{
			location = default(Vector3);
			return false;
		}
		PolymorphController component = receiver.GetComponent<PolymorphController>();
		if (component != null && component.IsPolymorphed())
		{
			return getPopupLocation(component.polymorphObject, out location);
		}
		location = receiver.transform.position;
		CharacterController component2 = receiver.GetComponent<CharacterController>();
		if (component2 != null)
		{
			location.y += component2.height;
		}
		else if (receiver.renderer != null)
		{
			float y = location.y;
			Vector3 max = receiver.renderer.bounds.max;
			location.y = y + max.y;
		}
		return true;
	}
}
