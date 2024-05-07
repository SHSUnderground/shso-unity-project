using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CombatController))]
[RequireComponent(typeof(BehaviorManager))]
[RequireComponent(typeof(CharacterMotionController))]
public class AIControllerBrawler : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected enum TargetType
	{
		TargetPlayer,
		TargetAI,
		TargetNone,
		TargetSame
	}

	[Serializable]
	protected sealed class AITarget
	{
		public GameObject target;

		public NetworkComponent targetNet;

		public CombatController targetCombat;

		public BrawlerHotTarget targetHot;

		public AITarget(GameObject target)
		{
			this.target = target;
			targetNet = target.GetComponent<NetworkComponent>();
			targetCombat = target.GetComponent<CombatController>();
			targetHot = target.GetComponent<BrawlerHotTarget>();
		}
	}

	public float aggroDistance;

	public float changeTargetMinimum;

	public float changeTargetMaximum;

	public float attackFrequency;

	public bool attackingSuppressed;

	public int targetingBaseWeight = 10;

	public float targetingTimeFactor = 1f;

	public float targetingDistanceFactor = 1f;

	protected float targetAssignedTime;

	protected float firstTargetTime;

	protected Dictionary<GameObject, float> lastTargettedTimes;

	protected AITarget currentTarget;

	protected AITarget pendingTarget;

	protected CharacterGlobals charGlobals;

	protected NetworkComponent netComp;

	protected CharacterMotionController motionController;

	protected BehaviorManager behaviorManager;

	protected CombatController combatController;

	protected AICombatController aiCombatController;

	protected bool initialized;

	protected bool wasOwner;

	protected bool lockedOwnership;

	protected bool wasAttacking;

	protected float lastAttackTime = Time.time;

	protected bool runningAIRoutine;

	protected bool startAIMain;

	protected bool runAI = true;

	protected bool gammaApplied;

	private List<CombatController> _potentialTargets;

	protected bool wakeUpOnAggro = true;

	protected bool wakeUpOnEvent = true;

	protected bool wakeUpOnHit = true;

	public float spawnTime = -1f;

	public float lifeDuration = -1f;

	protected bool attacksObjects;

	public bool CanWakeUpOnAggro
	{
		get
		{
			return wakeUpOnAggro;
		}
	}

	public bool CanWakeUpOnEvent
	{
		get
		{
			return wakeUpOnEvent;
		}
	}

	public virtual bool CanPlayAttackVO
	{
		get
		{
			return true;
		}
	}

	public virtual void Start()
	{
		charGlobals = (GetComponent(typeof(CharacterGlobals)) as CharacterGlobals);
		netComp = charGlobals.networkComponent;
		motionController = charGlobals.motionController;
		behaviorManager = charGlobals.behaviorManager;
		combatController = charGlobals.combatController;
		if (combatController != null)
		{
			aiCombatController = (charGlobals.combatController as AICombatController);
		}
		firstTargetTime = 0f;
		lastTargettedTimes = new Dictionary<GameObject, float>();
		_potentialTargets = new List<CombatController>();
		AppShell.Instance.EventMgr.AddListener<BrawlerWakeFromSleepMessage>(OnWakeFromSleep);
		StartAIMainRoutine();
	}

	public void Update()
	{
		if (netComp.IsOwner() && !runningAIRoutine && runAI)
		{
			StartAIMainRoutine();
		}
	}

	public virtual void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<BrawlerWakeFromSleepMessage>(OnWakeFromSleep);
	}

	public virtual void InitializeFromData(DataWarehouse aiMeleeData)
	{
		if (aiMeleeData != null)
		{
			aggroDistance = aiMeleeData.TryGetFloat("aggro_distance", 10f);
			changeTargetMinimum = aiMeleeData.TryGetFloat("change_target_time_minimum", 5f);
			changeTargetMaximum = aiMeleeData.TryGetFloat("change_target_time_maximum", 15f);
			//changeTargetMaximum = 5000f;   // CSP 
			InitializeAttackFrequency(aiMeleeData.TryGetFloat("attack_frequency", 1f));
			targetingBaseWeight = aiMeleeData.TryGetInt("targeting_base_weight", 10);
			targetingTimeFactor = aiMeleeData.TryGetFloat("targeting_time_factor", 1f);
			targetingDistanceFactor = aiMeleeData.TryGetFloat("targeting_distance_factor", 1f);
		}
		else
		{
			aggroDistance = 10f;
			changeTargetMinimum = 5f;
			changeTargetMaximum = 15f;
			//changeTargetMaximum = 5000f;   // CSP
			InitializeAttackFrequency(1f);
			targetingBaseWeight = 10;
			targetingTimeFactor = 1f;
			targetingDistanceFactor = 1f;
		}
	}

	public virtual void InitializeFromCopy(AIControllerBrawler source)
	{
		aggroDistance = source.aggroDistance;
		changeTargetMinimum = source.changeTargetMinimum;
		changeTargetMaximum = source.changeTargetMaximum;
		//changeTargetMaximum = 5000f;  // CSP
		InitializeAttackFrequency(source.attackFrequency);
		targetingBaseWeight = source.targetingBaseWeight;
		targetingTimeFactor = source.targetingTimeFactor;
		targetingDistanceFactor = source.targetingDistanceFactor;
	}

	public virtual void InitializeFromSpawner(CharacterSpawn source)
	{
		if (source.overrideAggroDistance)
		{
			aggroDistance = source.aggroDistance;
		}
		wakeUpOnAggro = source.wakeUpOnAggro;
		wakeUpOnEvent = source.wakeUpOnEvent;
		wakeUpOnHit = source.wakeUpOnHit;
		attacksObjects = source.attacksObjects;
	}

	protected void StartAIMainRoutine()
	{
		startAIMain = true;
		if ((bool)BrawlerController.Instance && BrawlerController.Instance.IsCutScenePlaying)
		{
			RunAI(false);
		}
		else if (combatController != null && combatController.isKilled)
		{
			RunAI(false);
		}
		else if (runAI && !runningAIRoutine)
		{
			StartCoroutine("AIMain");
		}
	}

	protected void StopAIMainRoutine()
	{
		startAIMain = false;
		if (runningAIRoutine)
		{
			StopCoroutine("AIMain");
			runningAIRoutine = false;
		}
	}

	protected virtual IEnumerator AIMain()
	{
		runningAIRoutine = true;
		yield return 0;
		yield return 0;
		while (combatController == null || combatController.colliderObjects == null || combatController.colliderObjects.Count == 0)
		{
			yield return new WaitForSeconds(0.1f);
		}
		int gammaLevel = 0;
		float damageMult = 1f;
		float healthMult = 1f;
		if (!gammaApplied && (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Polymorph) == 0 && FeaturesManager.featureEnabled("irradiation"))
		{
			gammaApplied = true;
			gammaLevel = 4;
			if (gammaLevel > 0)
			{
				damageMult += 0.25f * (float)gammaLevel;
			}
			if (gammaLevel > 0)
			{
				healthMult += 0.5f + 0.5f * (float)gammaLevel;
			}
			if ((charGlobals.spawnData.spawnType & CharacterSpawn.Type.Boss) != 0)
			{
				healthMult /= 1.5f;
			}
		}
		combatController.setMaxHealth(combatController.getMaxHealth() * healthMult);
		combatController.setHealth(combatController.getHealth() * healthMult);
		combatController.damageMultiplier *= damageMult;
		bool alive = true;
		
		while (alive)
		{
			if (spawnTime > 0f && lifeDuration > 0f && Time.time > spawnTime + lifeDuration)
			{
				combatController.dieNow();
				break;
			}
			CspUtils.DebugLog(" " + this.gameObject.name + " IsOwnedBySomeoneElse " + netComp.IsOwner() + 
								" " + netComp.NetOwnerId);  // CSP
			if (netComp.IsOwnedBySomeoneElse())
			{
				break;
			}
			if (netComp.IsOwner() && !wasOwner)
			{
				wasOwner = true;
				yield return new WaitForSeconds(0.5f);
			}
			alive = !combatController.isKilled;
			if (!alive)
			{
				break;
			}
			if (HasTarget() && !currentTarget.targetCombat.isKilled)
			{
				if (netComp.IsOwner() && !IsSleeping())
				{
					if (currentTarget.targetHot != null && currentTarget.targetHot.hotType == BrawlerHotTarget.HotTargetType.HotPickup)
					{
						BehaviorApproach approachBehavior = charGlobals.behaviorManager.getBehavior() as BehaviorApproach;
						if (approachBehavior == null)
						{
							combatController.ApproachTarget(GetTarget());
						}
					}
					else if (!attackingSuppressed)
					{
						BehaviorAttackBase attackBehavior = charGlobals.behaviorManager.getBehavior() as BehaviorAttackBase;
						if (attackBehavior == null)
						{
							wasAttacking = false;
							if (lastAttackTime + attackFrequency <= Time.time)
							{
								float changeTargetChance = (Time.time - (targetAssignedTime + changeTargetMinimum)) / (changeTargetMaximum - changeTargetMinimum);
								float chance = UnityEngine.Random.Range(0f, 1f);

								 if (chance < changeTargetChance)
								 {
								// 	CspUtils.DebugLog("chance=" + chance);  // CSP
								// 	CspUtils.DebugLog("Time.time=" + Time.time);  // CSP
								// 	CspUtils.DebugLog("changeTargetMinimum=" + changeTargetMinimum);  // CSP
								// 	CspUtils.DebugLog("changeTargetMaximum=" + changeTargetMaximum);  // CSP
								// 	CspUtils.DebugLog("targetAssignedTime=" + targetAssignedTime);  // CSP
									

								 	targetAssignedTime = Time.time;
									TargetType fnt = findNewTarget();
									// CSP commented out this block for testing
								 	if (fnt == TargetType.TargetPlayer && !currentTarget.targetNet.IsOwner())
								 	{
										CspUtils.DebugLog("AICB breaking! ");
										CspUtils.DebugLog("breaking ai=" + this.gameObject.name + "target=" + GetTarget().gameObject.name);
										CspUtils.DebugLog("1st= " + (fnt == TargetType.TargetPlayer));
										CspUtils.DebugLog("2nd= " + !currentTarget.targetNet.IsOwner()); 
								 		break;
								 	}
								 }
								if (!combatController.InPursuitOfTarget(GetTarget()) && !combatController.AwaitingPursuitOfTarget(GetTarget()))
								{
									if (aiCombatController != null)
									{
										CspUtils.DebugLog("clearChosenAttack");
										CspUtils.DebugLog("clear ai=" + this.gameObject.name + "target=" + GetTarget().gameObject.name);					
										aiCombatController.clearChosenAttack(false);
									}
									CspUtils.DebugLog("pursueTarget");
									CspUtils.DebugLog("pursue ai=" + this.gameObject.name + "target=" + GetTarget().gameObject.name);
									combatController.pursueTarget(GetTarget(), false);
								}
								else if (aiCombatController != null)
								{
									CspUtils.DebugLog("clearChosenAttack");
									CspUtils.DebugLog("clear ai=" + this.gameObject.name + "target=" + GetTarget().gameObject.name);				
									aiCombatController.clearChosenAttack(true);
								}
							}
						}
						else if (!wasAttacking)
						{
							lastAttackTime = Time.time;
							wasAttacking = true;
						}
					}
				}
			}
			else
			{
				findNewTarget();
			}
			yield return new WaitForSeconds(0.1f);
		}
		runningAIRoutine = false;
	}

	protected TargetType findNewTarget()
	{
		CspUtils.DebugLog("findNewTarget() called!");
		if (lockedOwnership)
		{
			return (!HasTarget()) ? TargetType.TargetNone : TargetType.TargetSame;
		}
		if (HasTarget())
		{
			GameObject target = GetTarget();
			if (lastTargettedTimes.ContainsKey(target))
			{
				lastTargettedTimes[target] = Time.time;
			}
			else
			{
				lastTargettedTimes.Add(target, Time.time);
			}
		}
		Dictionary<int, CombatController> dictionary = new Dictionary<int, CombatController>();
		int num = 0;
		foreach (CombatController potentialTarget in GetPotentialTargets())
		{
			float value;
			if (!lastTargettedTimes.TryGetValue(potentialTarget.gameObject, out value))
			{
				value = firstTargetTime;
			}
			float num2 = Time.time - value;
			float num3 = Vector3.Distance(base.transform.position, potentialTarget.transform.position);
			int num4 = (int)(targetingTimeFactor * num2 - targetingDistanceFactor * num3);
			if (num4 < 0)
			{
				num4 = 0;
			}
			num += targetingBaseWeight + num4;
			dictionary.Add(num, potentialTarget);
		}
		if (dictionary.Count > 0)
		{
			int num5 = UnityEngine.Random.Range(0, num);
			foreach (KeyValuePair<int, CombatController> item in dictionary)
			{
				if (item.Key >= num5)
				{
					if (HasTarget() && item.Value == currentTarget.targetCombat)
					{
						return TargetType.TargetSame;
					}
					return assignTarget(item.Value);
				}
			}
		}
		return TargetType.TargetNone;
	}

	protected void OnOwnershipChange(GameObject go, bool bAssumedOwnership)
	{
		if (!bAssumedOwnership)
		{
			SetTarget(null);
			wasOwner = false;
			return;
		}
		if (firstTargetTime == 0f)
		{
			firstTargetTime = Time.time;
		}
		SetTarget(GetPendingTarget());
		SetPendingTarget(null);
		if (IsSleeping() && wakeUpOnAggro)
		{
			WakeUp();
		}
		if (!runningAIRoutine)
		{
			StartAIMainRoutine();
		}
	}

	protected virtual TargetType assignTarget(CombatController newTargetController)
	{
		CspUtils.DebugLog("newTargetController attacker=" + this.gameObject.name + " target=" +  newTargetController.gameObject.name);

		if (GetTarget() == newTargetController.gameObject)
		{
			CspUtils.DebugLog("return same");
			return TargetType.TargetSame;
		}
		if (netComp.IsOwnedBySomeoneElse())
		{
			CspUtils.DebugLog("return none");
			return TargetType.TargetNone;
		}
		targetAssignedTime = Time.time;
		NetworkComponent component = newTargetController.GetComponent<NetworkComponent>();
		bool flag = Utils.IsPlayer(newTargetController.gameObject);
		if (netComp.NetOwnerId < 0)
		{
			if (component == null || component.IsOwnedBySomeoneElse())
			{
				CspUtils.DebugLog("return none2");
				return TargetType.TargetNone;
			}
			SetPendingTarget(newTargetController.gameObject);
			AppShell.Instance.ServerConnection.Game.TakeOwnership(base.gameObject, OnOwnershipChange);
		}
		else
		{
			behaviorManager.clearQueuedBehavior();
			BehaviorAttackApproach behaviorAttackApproach = behaviorManager.getBehavior() as BehaviorAttackApproach;
			if (behaviorAttackApproach != null)
			{
				behaviorManager.endBehavior();
			}
			BehaviorApproach behaviorApproach = behaviorManager.getBehavior() as BehaviorApproach;
			if (behaviorApproach != null)
			{
				behaviorManager.endBehavior();
			}
			if (flag)
			{
				if (component.NetOwnerId == -2)
				{
					CspUtils.DebugLog("return none3");
					return TargetType.TargetNone;
				}
				if (component.NetOwnerId == -1)
				{
					
					component.NetOwnerId = component.goNetId.ChildId;
					CspUtils.DebugLog("AICB name= " + component.gameObject.name + " AICB NetOwnerId= " + component.NetOwnerId); // CSP
				}
			}
			SetTarget(newTargetController.gameObject);
			if (IsSleeping() && wakeUpOnAggro)
			{
				WakeUp();
			}
			if (flag)
			{
				AssignTargetMessage assignTargetMessage = new AssignTargetMessage(netComp.goNetId);
				assignTargetMessage.target = newTargetController.gameObject;
				AppShell.Instance.ServerConnection.SendGameMsg(assignTargetMessage);
			}
			// CSP component is target, netComp is attacker
			if (component != null && component.NetOwnerId != netComp.NetOwnerId && flag)
			{
				// this block commented out by CSP temporarily, as it seems to be making enemy's ownerID the ID of non-host player.
				//StopAIMainRoutine();
				//motionController.stopGently();
				//wasOwner = false;
				//AppShell.Instance.ServerConnection.Game.TransferOwnership(newTargetController.gameObject, base.gameObject);
			}
			else if (!runningAIRoutine)
			{
				StartAIMainRoutine();
			}
		}
		AppShell.Instance.EventMgr.Fire(this, new CombatCharacterAggroMessage(combatController, newTargetController));
		CspUtils.DebugLog("return end");
		return (!flag) ? TargetType.TargetAI : TargetType.TargetPlayer;
	}

	public virtual void netAssignTarget(GameObject newTarget)
	{
		if (newTarget == null)
		{
			return;
		}
		if (firstTargetTime == 0f)
		{
			firstTargetTime = Time.time;
		}
		if (HasTarget())
		{
			GameObject target = GetTarget();
			if (lastTargettedTimes.ContainsKey(target))
			{
				lastTargettedTimes[target] = Time.time;
			}
			else
			{
				lastTargettedTimes.Add(target, Time.time);
			}
		}
		SetTarget(newTarget);
		if (IsSleeping() && wakeUpOnAggro)
		{
			WakeUp();
		}
		if (!(currentTarget.targetNet == null) && currentTarget.targetNet.IsOwner())
		{
			motionController.teleportToDestination();
			if (!runningAIRoutine)
			{
				StartAIMainRoutine();
			}
		}
	}

	protected virtual void OnBecomeOwner()
	{
		if (!HasTarget() || currentTarget.targetNet == null || !currentTarget.targetNet.IsOwner())
		{
			CspUtils.DebugLog("OnBecomeOwner() calling SetTarget()");
			SetTarget(null);
		}
		if (!runningAIRoutine)
		{
			StartAIMainRoutine();
		}
	}

	public virtual void HitByEnemy(CombatController enemyCombatController)
	{
		if (enemyCombatController != null && enemyCombatController.gameObject != GetTarget() && !netComp.IsOwnedBySomeoneElse() && (!netComp.IsOwner() || (enemyCombatController.CharGlobals != null && enemyCombatController.CharGlobals.networkComponent.IsOwner())) && !enemyCombatController.InStealthMode() && combatController.faction != enemyCombatController.faction)
		{
			assignTarget(enemyCombatController);
		}
	}

	public void RunAI(bool run)
	{
		runAI = run;
		if (runAI)
		{
			if (startAIMain)
			{
				StartAIMainRoutine();
			}
		}
		else if (runningAIRoutine)
		{
			StopCoroutine("AIMain");
			runningAIRoutine = false;
		}
	}

	public virtual void OnCutSceneStart()
	{
		RunAI(false);
	}

	public virtual void OnCutSceneEnd()
	{
		RunAI(true);
	}

	public void EndAttackOnTarget(GameObject target)
	{
		if (!(netComp == null) && netComp.IsOwner() && !(target != GetTarget()))
		{
			if (behaviorManager.getBehavior() is BehaviorAttackApproach)
			{
				behaviorManager.endBehavior();
			}
			TargetType targetType = findNewTarget();
			if (targetType == TargetType.TargetNone || targetType == TargetType.TargetSame)
			{
				SetTarget(null);
				charGlobals.motionController.stopGently();
			}
		}
	}

	public void EndAttackOnFaction(CombatController.Faction faction)
	{
		if (!(netComp == null) && netComp.IsOwner() && HasTarget() && currentTarget.targetCombat.faction == faction)
		{
			if (behaviorManager.getBehavior() is BehaviorAttackApproach)
			{
				behaviorManager.endBehavior();
			}
			TargetType targetType = findNewTarget();
			if (targetType == TargetType.TargetSame || targetType == TargetType.TargetNone)
			{
				SetTarget(null);
				charGlobals.motionController.stopGently();
			}
		}
	}

	public GameObject GetTarget()
	{
		return (currentTarget == null) ? null : currentTarget.target;
	}

	public GameObject GetPendingTarget()
	{
		return (pendingTarget == null) ? null : pendingTarget.target;
	}

	public bool HasTarget()
	{
		return GetTarget() != null;
	}

	public bool HasPendingTarget()
	{
		return GetPendingTarget() != null;
	}

	public bool HasTargetedSomeone()
	{
		if (lastTargettedTimes == null)
		{
			return false;
		}
		return lastTargettedTimes.Count > 0;
	}

	public bool HasTargetInRange()
	{
		List<CombatController> potentialTargets = GetPotentialTargets();
		return potentialTargets.Count > 0;
	}

	public List<CombatController> GetPotentialTargets()
	{
		if (_potentialTargets == null)
		{
			return new List<CombatController>();
		}
		_potentialTargets.Clear();
		CombatController.Faction faction = CombatController.Faction.None;
		if (combatController.faction == CombatController.Faction.Enemy)
		{
			faction = CombatController.Faction.Player;
		}
		else if (combatController.faction == CombatController.Faction.Player)
		{
			faction = CombatController.Faction.Enemy;
		}
		if (faction == CombatController.Faction.None)
		{
			return _potentialTargets;
		}
		List<CombatController> factionList = CombatController.GetFactionList(faction);
		if (factionList != null)
		{
			foreach (CombatController item in factionList)
			{
				if (IsHotTarget(item))
				{
					_potentialTargets.Clear();
					_potentialTargets.Add(item);
					break;
				}
				if (IsPotentialTarget(item))
				{
					_potentialTargets.Add(item);
				}
			}
		}
		return _potentialTargets;
	}

	public bool IsSleeping()
	{
		BehaviorMovement behaviorMovement = charGlobals.behaviorManager.getBehavior() as BehaviorMovement;
		return behaviorMovement != null && behaviorMovement.IsSleeping();
	}

	public void WakeUp()
	{
		if (!wakeUpOnHit)
		{
			combatController.faction = CombatController.Faction.Enemy;
		}
		BehaviorMovement behaviorMovement = charGlobals.behaviorManager.getBehavior() as BehaviorMovement;
		if (behaviorMovement != null)
		{
			behaviorMovement.WakeUp();
			combatController.playWakeupEffect();
			AppShell.Instance.EventMgr.Fire(base.gameObject, new CombatCharacterAwakenedMessage(base.gameObject));
		}
	}

	public void WakeUpOnEvent()
	{
		if (IsSleeping() && wakeUpOnEvent)
		{
			WakeUp();
		}
	}

	public void InitializeAttackFrequency(float frequency)
	{
		lastAttackTime = Time.time - frequency;
		attackFrequency = frequency;
	}

	public void LockOwnership(bool locked)
	{
		lockedOwnership = locked;
	}

	protected void OnWakeFromSleep(BrawlerWakeFromSleepMessage e)
	{
		WakeUpOnEvent();
	}

	private void SetTarget(GameObject target)
	{
		CspUtils.DebugLog("SetTarget called!");

		if (target != null)
		{
			currentTarget = new AITarget(target);
			targetAssignedTime = Time.time;
		}
		else
		{
			currentTarget = null;
		}
	}

	private void SetPendingTarget(GameObject target)
	{
		pendingTarget = ((!(target != null)) ? null : new AITarget(target));
	}

	private bool IsPotentialTarget(CombatController target)
	{
		if (target == null)
		{
			return false;
		}
		if (!target.isHealthInitialized() || target.isKilled)
		{
			return false;
		}
		if (target.InStealthMode() || target.isHidden)
		{
			return false;
		}
		if (target is ObjectCombatController && !attacksObjects)
		{
			return false;
		}
		if ((target.transform.position - base.transform.position).sqrMagnitude > aggroDistance * aggroDistance)
		{
			return false;
		}
		return true;
	}

	private bool IsHotTarget(CombatController target)
	{
		if (target == null)
		{
			return false;
		}
		if (!target.isHealthInitialized() || target.isKilled)
		{
			return false;
		}
		if (target.InStealthMode() || target.isHidden)
		{
			return false;
		}
		BrawlerHotTarget component = target.GetComponent<BrawlerHotTarget>();
		if (component == null)
		{
			return false;
		}
		if (!component.IsHot())
		{
			return false;
		}
		if (!component.IsMoth(combatController))
		{
			return false;
		}
		if (!component.IsMothInHotRadius(combatController))
		{
			return false;
		}
		return true;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, aggroDistance);
	}
}
