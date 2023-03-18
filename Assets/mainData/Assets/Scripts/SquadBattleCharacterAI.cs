using CardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadBattleCharacterAI : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected const float attackTimeoutDelay = 5f;

	protected const float cameraResetDelay = 0.33f;

	protected const float multiCharacterApproachAngle = 45f;

	protected const float soloCharacterApproachAngle = -15f;

	protected const float emoteTimeoutDelay = 15f;

	public bool isVillain;

	public SquadBattleCharacterLocator homeLocator;

	public Vector3 homeLocation;

	public Vector3 homeFacing;

	public SquadBattlePlayerEnum owningPlayer;

	public CharacterLocatorManager locatorManager;

	protected SquadBattleAction currentAction;

	protected bool begunAction;

	protected bool arrived;

	protected bool actionReady;

	protected bool waitingForAction;

	protected bool secondAttackerActing;

	protected int attackSequenceIndex;

	protected int attackEnumerator;

	protected int queuedDamage;

	protected int damageRemaining;

	protected bool isFatalAttack;

	public bool isDeathBlow;

	protected float attackTimeoutTimestamp;

	protected CameraLiteManagerSquadBattle cameraMan;

	protected float cameraResetTimestamp;

	protected float cameraChangeTime;

	protected CharacterGlobals charGlobals;

	protected CharacterGlobals secondaryAttacker;

	protected float endEmoteTimestamp;

	protected bool didWeSloMo;

	protected bool begunBlocking;

	protected bool makeTargetBlock;

	protected bool victorious;

	protected bool defeated;

	protected bool hasDied;

	private static Dictionary<string, bool> SloMoBlacklist;

	private static string[] sloMoBlacklist;

	private static string[] ignoreIntroForCardGame;

	private static string[] angry_behaviors;

	private static string[] happy_behaviors;

	private static KeyValuePair<int, string>[] intro_emotes;

	static SquadBattleCharacterAI()
	{
		sloMoBlacklist = new string[7]
		{
			"wolverine",
			"wolverine_jeans",
			"wolverine_samurai",
			"hulk_gladiator",
			"elektra",
			"deadpool",
			"thor_ultimate"
		};
		ignoreIntroForCardGame = new string[1]
		{
			"dr_doom_future_foundation"
		};
		angry_behaviors = new string[9]
		{
			"angry",
			"disapprove",
			"taunt",
			"blush",
			"confused",
			"sad",
			"sad",
			"sad",
			"scared"
		};
		happy_behaviors = new string[11]
		{
			"approve",
			"cheer",
			"cheer",
			"cheer",
			"clap",
			"laugh",
			"pose",
			"point",
			"pemote1",
			"pemote2",
			"pemote3"
		};
		intro_emotes = new KeyValuePair<int, string>[16]
		{
			new KeyValuePair<int, string>(15, "pemote1"),
			new KeyValuePair<int, string>(15, "pemote2"),
			new KeyValuePair<int, string>(15, "pemote3"),
			new KeyValuePair<int, string>(5, "angry"),
			new KeyValuePair<int, string>(3, "disapprove"),
			new KeyValuePair<int, string>(7, "taunt"),
			new KeyValuePair<int, string>(3, "approve"),
			new KeyValuePair<int, string>(5, "cheer"),
			new KeyValuePair<int, string>(4, "clap"),
			new KeyValuePair<int, string>(5, "greet"),
			new KeyValuePair<int, string>(5, "laugh"),
			new KeyValuePair<int, string>(5, "point"),
			new KeyValuePair<int, string>(1, "shock"),
			new KeyValuePair<int, string>(8, "pose"),
			new KeyValuePair<int, string>(1, "sneeze"),
			new KeyValuePair<int, string>(3, "think")
		};
		SloMoBlacklist = new Dictionary<string, bool>();
		string[] array = sloMoBlacklist;
		foreach (string key in array)
		{
			SloMoBlacklist.Add(key, true);
		}
	}

	private void Awake()
	{
		currentAction = null;
	}

	private void Start()
	{
		charGlobals = (GetComponent(typeof(CharacterGlobals)) as CharacterGlobals);
		cameraMan = (CameraLiteManager.Instance as CameraLiteManagerSquadBattle);
		victorious = false;
		defeated = false;
		waitingForAction = false;
	}

	private void Update()
	{
		if (currentAction != null && actionReady)
		{
			if (!begunAction)
			{
				if (charGlobals.behaviorManager.getBehavior() is BehaviorMovementSquadBattle)
				{
					BeginAction();
				}
			}
			else
			{
				continueAttack();
			}
		}
		else if (victorious || defeated)
		{
			UpdateEndEmote();
		}
		else if (attackTimeoutTimestamp > 0f && Time.time > attackTimeoutTimestamp)
		{
			if (!arrived)
			{
				Arrived();
			}
			else
			{
				charGlobals.behaviorManager.endBehavior();
			}
		}
	}

	protected void BeginAction()
	{
		begunAction = true;
		if (secondaryAttacker == null && (bool)currentAction.secondaryCharacter)
		{
			secondaryAttacker = (currentAction.secondaryCharacter.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals);
			if (secondaryAttacker == null)
			{
				CspUtils.DebugLog("Could not get CharacterGlobals for secondary character");
			}
			else
			{
				secondaryAttacker.squadBattleCharacterAI.moveToTargetOffset(currentAction.targetCharacter, (currentAction.player != 0) ? (-45f) : 45f, true);
			}
		}
		damageRemaining = currentAction.damage;
		if (currentAction.blocker)
		{
			begunBlocking = true;
			SquadBattleCharacterAI squadBattleCharacterAI = currentAction.targetCharacter.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
			squadBattleCharacterAI.HitBlocker(base.gameObject);
			BehaviorBlock behaviorBlock = charGlobals.behaviorManager.ChangeDefaultBehavior("BehaviorBlock") as BehaviorBlock;
			behaviorBlock.Initialize(currentAction.targetCharacter, false);
			charGlobals.behaviorManager.endBehavior();
		}
		else if (currentAction.blockingCharacterName != string.Empty)
		{
			startBlock();
		}
		else if (damageRemaining > 0)
		{
			startAttack();
		}
	}

	protected void UpdateEndEmote()
	{
		if (!(charGlobals.behaviorManager.getBehavior() is BehaviorMovementSquadBattle))
		{
			return;
		}
		if (endEmoteTimestamp == -1f)
		{
			endEmoteTimestamp = Time.time + UnityEngine.Random.Range(0.1f, 1.4f);
		}
		else if (endEmoteTimestamp == 0f)
		{
			if (defeated && locatorManager.getAvatar() == base.gameObject)
			{
				CameraLiteSelection cameraLiteSelection = cameraMan.ArenaCamera();
				cameraMan.ReplaceCamera(cameraLiteSelection.camera, 0f);
			}
			endEmoteTimestamp = Time.time + UnityEngine.Random.Range(3f, 5f);
		}
		else if (Time.time > endEmoteTimestamp)
		{
			if (victorious)
			{
				DoSomethingHappy();
			}
			else if (defeated)
			{
				DoSomethingAngry();
			}
			endEmoteTimestamp = -1f;
		}
	}

	public void DoSomethingHappy()
	{
		string empty = string.Empty;
		if (isVillain)
		{
			if (charGlobals.animationComponent["emote_taunt"] == null)
			{
				return;
			}
			empty = "taunt";
		}
		else
		{
			int num = UnityEngine.Random.Range(0, happy_behaviors.Length);
			empty = happy_behaviors[num];
		}
		BehaviorEmote behaviorEmote = charGlobals.behaviorManager.requestChangeBehavior(typeof(BehaviorEmote), true) as BehaviorEmote;
		if (!behaviorEmote.Initialize(EmotesDefinition.Instance.GetEmoteByCommand(empty).id))
		{
			charGlobals.behaviorManager.endBehavior();
		}
	}

	public void DoSomethingAngry()
	{
		BehaviorEmote behaviorEmote = charGlobals.behaviorManager.requestChangeBehavior(typeof(BehaviorEmote), true) as BehaviorEmote;
		string empty = string.Empty;
		if (isVillain)
		{
			empty = "taunt";
		}
		else
		{
			int num = UnityEngine.Random.Range(0, angry_behaviors.Length);
			empty = angry_behaviors[num];
		}
		behaviorEmote.Initialize(EmotesDefinition.Instance.GetEmoteByCommand(empty).id);
	}

	public void HitBlocker(GameObject blockingCharacter)
	{
		currentAction.damage = 1;
		currentAction.targetCharacter = blockingCharacter;
		currentAction.blockingCharacterName = string.Empty;
		begunBlocking = false;
		begunAction = false;
		makeTargetBlock = false;
	}

	public void beginSuper(int powerAttackIndex)
	{
		PlayerCombatController playerCombatController = charGlobals.combatController as PlayerCombatController;
		if (playerCombatController == null)
		{
			CspUtils.DebugLog(charGlobals.gameObject.name + " tried to player super attack, but does not have a PlayerCombatController");
			return;
		}
		string heroupAttackName = playerCombatController.GetHeroupAttackName(powerAttackIndex);
		CombatController.AttackData attackData = AttackDataManager.Instance.getAttackData(heroupAttackName);
		if (attackData == null)
		{
			CspUtils.DebugLog("Specified super attack " + heroupAttackName + " does not exist!");
			return;
		}
		Type type = Type.GetType(attackData.behaviorName);
		BehaviorAttackBase behaviorAttackBase = charGlobals.behaviorManager.requestChangeBehavior(type, true) as BehaviorAttackBase;
		behaviorAttackBase.Initialize(null, attackData, false, false, 0f);
	}

	protected void startAttack()
	{
		if (makeTargetBlock)
		{
			CharacterGlobals characterGlobals = currentAction.targetCharacter.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
			BehaviorBlock behaviorBlock = characterGlobals.behaviorManager.ChangeDefaultBehavior("BehaviorBlock") as BehaviorBlock;
			behaviorBlock.Initialize(base.gameObject, true);
			characterGlobals.behaviorManager.endBehavior();
			characterGlobals.squadBattleCharacterAI.ClearDefeat();
		}
		cameraChangeTime = Time.time;
		attackCamera(false);
		didWeSloMo = false;
		secondAttackerActing = currentAction.attackPattern.attackSequence[attackSequenceIndex].isSecondaryAttacker;
		CharacterGlobals currentAttacker = (!secondAttackerActing) ? charGlobals : secondaryAttacker;
		if (secondAttackerActing)
		{
			charGlobals.squadBattleCharacterAI.moveToTargetOffset(currentAction.targetCharacter, (currentAction.player != 0) ? 45f : (-45f), true);
		}
		else if (secondaryAttacker != null)
		{
			secondaryAttacker.squadBattleCharacterAI.moveToTargetOffset(currentAction.targetCharacter, (currentAction.player != 0) ? (-45f) : 45f, true);
		}
		performAttack(currentAttacker);
		if (currentAction.attackPattern.RepeatSequence || currentAction.attackPattern.attackSequence.Count > 1)
		{
			queuedDamage = 1;
		}
		else
		{
			queuedDamage = damageRemaining;
		}
		if (isFatalAttack && damageRemaining == queuedDamage)
		{
			deathBlow();
		}
	}

	protected void continueAttack()
	{
		if (secondAttackerActing && secondaryAttacker == null)
		{
			return;
		}
		CharacterGlobals currentAttacker = (!secondAttackerActing) ? charGlobals : secondaryAttacker;
		if (damageRemaining > 0 && currentAction.blockingCharacterName == string.Empty)
		{
			BehaviorBase behavior = currentAttacker.behaviorManager.getBehavior();
			bool flag = behavior is BehaviorMovementSquadBattle;
			if (!flag)
			{
				float behaviorDuration = behavior.GetBehaviorDuration();
				if (behaviorDuration > 0f && behaviorDuration < 0.5f)
				{
					flag = true;
				}
			}
			BehaviorAttackBase behaviorAttackBase = behavior as BehaviorAttackBase;
			bool flag2 = behaviorAttackBase != null || behavior is BehaviorEmote;
			BehaviorBase queuedBehavior = currentAttacker.behaviorManager.getQueuedBehavior();
			if (queuedDamage > 0 && queuedBehavior == null && (flag2 || (attackTimeoutTimestamp > 0f && Time.time > attackTimeoutTimestamp)))
			{
				damageRemaining -= queuedDamage;
				queuedDamage = 0;
				if (damageRemaining == 0)
				{
					if (isFatalAttack)
					{
						deathBlow();
					}
					return;
				}
				if (isFatalAttack && !currentAction.attackPattern.RepeatSequence && remainingAttacksHarmless())
				{
					deathBlow();
				}
				if (behaviorAttackBase != null)
				{
					behaviorAttackBase.canChain = IsCurrentActionChaining();
				}
			}
			bool flag3 = attackSequenceIndex + 1 < currentAction.attackPattern.attackSequence.Count && secondAttackerActing != currentAction.attackPattern.attackSequence[attackSequenceIndex + 1].isSecondaryAttacker;
			if (queuedBehavior != null || (((behaviorAttackBase == null && !flag3) || !IsCurrentActionChaining()) && !flag && (!(attackTimeoutTimestamp > 0f) || !(Time.time > attackTimeoutTimestamp))))
			{
				return;
			}
			attackSequenceIndex++;
			attackEnumerator++;
			if ((float)UnityEngine.Random.Range(0, 30) < Time.time - cameraChangeTime - 2f)
			{
				attackCamera(true);
				cameraChangeTime = Time.time;
			}
			bool flag4 = false;
			if (attackSequenceIndex == currentAction.attackPattern.attackSequence.Count)
			{
				attackSequenceIndex = 0;
			}
			else if (attackSequenceIndex == currentAction.attackPattern.attackSequence.Count - 1 && !currentAction.attackPattern.RepeatSequence)
			{
				flag4 = true;
			}
			bool flag5 = secondAttackerActing;
			secondAttackerActing = currentAction.attackPattern.attackSequence[attackSequenceIndex].isSecondaryAttacker;
			if (flag5 != secondAttackerActing)
			{
				float num = (!secondAttackerActing) ? (-45f) : 45f;
				if (currentAction.player == SquadBattlePlayerEnum.Left)
				{
					num *= -1f;
				}
				currentAttacker.squadBattleCharacterAI.moveToTargetOffset(currentAction.targetCharacter, num, false);
				currentAttacker = ((!secondAttackerActing) ? charGlobals : secondaryAttacker);
			}
			if (flag4)
			{
				queuedDamage = damageRemaining;
			}
			else
			{
				queuedDamage = 1;
			}
			if (secondAttackerActing && currentAttacker == null)
			{
				currentAction.secondaryCharacter = locatorManager.getCharacter(currentAction.secondaryAttackingCharacterName);
				if (currentAction.secondaryCharacter == null)
				{
					locatorManager.spawnTemporaryCharacter(currentAction.secondaryAttackingCharacterName, currentAction, currentAction.targetCharacter, true, delegate(GameObject spawnedCharacter)
					{
						currentAction.secondaryCharacter = spawnedCharacter;
						SquadBattleCharacterAI squadBattleCharacterAI2 = spawnedCharacter.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
						squadBattleCharacterAI2.WaitForAction();
						secondaryAttacker = (spawnedCharacter.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals);
						currentAttacker = secondaryAttacker;
						attackCamera(false);
						performAttack(currentAttacker);
					});
				}
				else
				{
					SquadBattleCharacterAI squadBattleCharacterAI = currentAction.secondaryCharacter.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
					squadBattleCharacterAI.WaitForAction();
					secondaryAttacker = (currentAction.secondaryCharacter.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals);
					currentAttacker = secondaryAttacker;
					attackCamera(false);
				}
			}
			if (currentAttacker != null)
			{
				performAttack(currentAttacker);
			}
		}
		else
		{
			if (begunBlocking)
			{
				return;
			}
			if (currentAction.blockingCharacterName != string.Empty)
			{
				startBlock();
				return;
			}
			BehaviorMovementSquadBattle behaviorMovementSquadBattle = currentAttacker.behaviorManager.getBehavior() as BehaviorMovementSquadBattle;
			if (behaviorMovementSquadBattle != null)
			{
				if (cameraResetTimestamp == 0f)
				{
					cameraResetTimestamp = Time.time + 0.33f;
				}
				else if (Time.time > cameraResetTimestamp)
				{
					EndAttack();
				}
				return;
			}
			BehaviorAttackApproach behaviorAttackApproach = currentAttacker.behaviorManager.getBehavior() as BehaviorAttackApproach;
			if (behaviorAttackApproach != null && attackTimeoutTimestamp > 0f && Time.time > attackTimeoutTimestamp)
			{
				currentAttacker.behaviorManager.clearQueuedBehavior();
				currentAttacker.behaviorManager.endBehavior();
				EndAttack();
			}
		}
	}

	protected void deathBlow()
	{
		isDeathBlow = true;
		if (secondaryAttacker != null)
		{
			secondaryAttacker.squadBattleCharacterAI.isDeathBlow = true;
		}
	}

	protected bool remainingAttacksHarmless()
	{
		int num = attackSequenceIndex + 1;
		if (num == currentAction.attackPattern.attackSequence.Count)
		{
			return true;
		}
		while (currentAction.attackPattern.attackSequence[num].isHarmless())
		{
			num++;
			if (num == currentAction.attackPattern.attackSequence.Count)
			{
				return true;
			}
		}
		return false;
	}

	protected void performAttack(CharacterGlobals currentAttacker)
	{
		attackTimeoutTimestamp = Time.time + 5f;
		SquadBattleAttackPattern.AttackSequenceEntry attackSequenceEntry = currentAction.attackPattern.attackSequence[attackSequenceIndex];
		if (attackSequenceEntry.attackType == SquadBattleAttackPattern.AttackSequenceEntry.AttackSequenceType.Super)
		{
			currentAttacker.squadBattleCharacterAI.beginSuper(currentAction.attackPattern.attackSequence[attackSequenceIndex].attackIndex);
		}
		else if (attackSequenceEntry.attackType == SquadBattleAttackPattern.AttackSequenceEntry.AttackSequenceType.Emote || attackSequenceEntry.attackType == SquadBattleAttackPattern.AttackSequenceEntry.AttackSequenceType.PowerEmote)
		{
			BehaviorEmote behaviorEmote = currentAttacker.behaviorManager.requestChangeBehavior(typeof(BehaviorEmote), true) as BehaviorEmote;
			behaviorEmote.Initialize((sbyte)attackSequenceEntry.attackIndex);
		}
		else
		{
			BehaviorAttackQueue behaviorAttackQueue = currentAttacker.behaviorManager.requestChangeBehavior(typeof(BehaviorAttackQueue), true) as BehaviorAttackQueue;
			bool newSecondaryAttack = SetAttack(currentAttacker);
			behaviorAttackQueue.Initialize(currentAction.targetCharacter, newSecondaryAttack);
		}
	}

	protected bool SetAttack(CharacterGlobals character)
	{
		bool flag = currentAction.attackPattern.attackSequence[attackSequenceIndex].attackType == SquadBattleAttackPattern.AttackSequenceEntry.AttackSequenceType.Right;
		if (flag)
		{
			character.combatController.SetSecondaryAttack(currentAction.attackPattern.attackSequence[attackSequenceIndex].attackIndex);
		}
		else
		{
			character.combatController.currentAttack = currentAction.attackPattern.attackSequence[attackSequenceIndex].attackIndex;
		}
		return flag;
	}

	public void Killed(GameObject source)
	{
		if (!hasDied)
		{
			hasDied = true;
			defeated = true;
			CspUtils.DebugLog(base.gameObject.name + " killed, defeated");
			ActionDone();
			charGlobals.combatController.killed(base.gameObject, 1f);
			StartCoroutine(CoSloMo(base.gameObject));
		}
	}

	public void inflictDamage(GameObject target)
	{
		SquadBattleCharacterAI squadBattleCharacterAI = target.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
		if (!squadBattleCharacterAI.begunBlocking && !didWeSloMo)
		{
			bool flag = false;
			if (isDeathBlow)
			{
				squadBattleCharacterAI.Killed(base.gameObject);
				victorious = true;
			}
			else if (UnityEngine.Random.Range(0, 100) <= 10)
			{
				flag = true;
			}
			if (!isFatalAttack && flag && !SloMoBlacklist.ContainsKey(base.name) && currentAction != null && currentAction.targetCharacter != null)
			{
				StartCoroutine(CoSloMo(currentAction.targetCharacter));
			}
		}
	}

	protected IEnumerator CoSloMo(GameObject target)
	{
		CardGameController.Instance.AudioManager.Play(CardGameAudioManager.SFX.SlomoStart);
		CameraLiteSelection characterCam = cameraMan.CharacterCamera();
		characterCam.camera.SetTarget(target.transform);
		CameraLiteOffset clo = characterCam.camera as CameraLiteOffset;
		clo.SetReverse(owningPlayer == SquadBattlePlayerEnum.Right);
		float oldDistance = characterCam.camera.GetDistance();
		characterCam.camera.SetDistance(oldDistance * 0.75f);
		cameraMan.ReplaceCamera(characterCam.camera, 0f);
		float killSloMoTime = Time.time + 0.4f;
		didWeSloMo = true;
		Time.timeScale = 0.25f;
		while (Time.time < killSloMoTime)
		{
			yield return 0;
		}
		Time.timeScale = 1f;
		CardGameController.Instance.AudioManager.Play(CardGameAudioManager.SFX.SlomoEnd);
		CspUtils.DebugLog(base.gameObject.name + " ending slomo, defeated = " + defeated);
		if (defeated)
		{
			characterCamera();
		}
		else
		{
			attackCamera(true);
		}
	}

	protected void attackCamera(bool forceCut)
	{
		CameraLiteSelection cameraLiteSelection = cameraMan.CharacterCamera();
		CameraLiteOffset cameraLiteOffset = cameraLiteSelection.camera as CameraLiteOffset;
		CameraLiteSelection cameraLiteSelection2 = cameraMan.CombatCamera();
		CameraLiteMultiTarget cameraLiteMultiTarget = cameraLiteSelection2.camera.gameObject.GetComponent(typeof(CameraLiteMultiTarget)) as CameraLiteMultiTarget;
		List<GameObject> list = new List<GameObject>();
		list.Add(base.gameObject);
		list.Add(currentAction.targetCharacter);
		if (currentAction.secondaryCharacter != null)
		{
			list.Add(secondaryAttacker.gameObject);
		}
		cameraLiteMultiTarget.assignTargets(list);
		cameraLiteOffset = (cameraLiteSelection2.camera as CameraLiteOffset);
		cameraLiteOffset.SetReverse(owningPlayer == SquadBattlePlayerEnum.Right);
		cameraMan.ReplaceCamera(cameraLiteSelection2.camera, (!forceCut) ? cameraLiteSelection2.blendTime : 0f);
	}

	protected void startBlock()
	{
		EndAttack();
	}

	public void EndAttack()
	{
		attackTimeoutTimestamp = 0f;
		homeLocation = Vector3.zero;
		begunBlocking = false;
		if (!victorious && !defeated)
		{
			if (currentAction != null && currentAction.targetCharacter != null && currentAction.damage >= 4)
			{
				SquadBattleCharacterController.Instance.HighDamageStreak(owningPlayer, base.gameObject, (!(secondaryAttacker == null)) ? secondaryAttacker.gameObject : null, currentAction.targetCharacter);
			}
			CameraLiteSelection cameraLiteSelection = cameraMan.ArenaCamera();
			cameraMan.ReplaceCamera(cameraLiteSelection.camera, 0f);
		}
		if (currentAction != null && currentAction.targetCharacter != null)
		{
			SquadBattleCharacterAI squadBattleCharacterAI = currentAction.targetCharacter.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
			squadBattleCharacterAI.ActionDone();
		}
		currentAction = null;
		waitingForAction = false;
		if (secondaryAttacker != null)
		{
			secondaryAttacker.squadBattleCharacterAI.EndAttack();
			secondaryAttacker = null;
		}
		charGlobals.combatController.clearCombatEffects();
		SquadBattleCharacterController.Instance.ActionComplete();
	}

	protected void HitByEnemy(CombatController enemy)
	{
		if (currentAction != null && currentAction.blocker && enemy.gameObject == currentAction.targetCharacter)
		{
			CombatController.AttackData attackData = AttackDataManager.Instance.getAttackData("CardGameBlockBlowback");
			CombatController.ImpactData impactData = attackData.impacts[0];
			BehaviorManager behaviorManager = enemy.gameObject.GetComponent(typeof(BehaviorManager)) as BehaviorManager;
			behaviorManager.endBehavior();
			enemy.hitByAttack(base.gameObject.transform.position, charGlobals.combatController, charGlobals.gameObject, 0f, impactData.impactResult);
		}
	}

	public void ActionDone()
	{
		currentAction = null;
		charGlobals.behaviorManager.ChangeDefaultBehavior(charGlobals.behaviorManager.defaultBehaviorType);
		BehaviorBlock behaviorBlock = charGlobals.behaviorManager.getBehavior() as BehaviorBlock;
		if (behaviorBlock != null)
		{
			begunBlocking = false;
			behaviorBlock.endBlock();
		}
	}

	public void SetHomeLocator(SquadBattleCharacterLocator newLocator)
	{
		homeLocator = newLocator;
	}

	public void StartNewAction(SquadBattleAction newAction)
	{
		if (currentAction != null)
		{
			CspUtils.DebugLog("Tried to start a new action before completing previous - ignoring new action");
			return;
		}
		currentAction = newAction;
		attackSequenceIndex = 0;
		attackEnumerator = 0;
		isDeathBlow = false;
		isFatalAttack = false;
		secondAttackerActing = false;
		if (currentAction.attackPattern != null)
		{
			int num = 0;
			Stack<int> stack = new Stack<int>();
			foreach (SquadBattleAttackPattern.AttackSequenceEntry item in currentAction.attackPattern.attackSequence)
			{
				if (item.damageRequirement > currentAction.damage)
				{
					stack.Push(num);
				}
				num++;
			}
			while (stack.Count > 0)
			{
				currentAction.attackPattern.attackSequence.RemoveAt(stack.Pop());
			}
		}
		CspUtils.DebugLog(base.gameObject.name + " got SquadBattleCharacterAI.StartNewAction() -- damage=" + currentAction.damage + ", health=" + currentAction.startingHealth);
		if (!currentAction.preventKO && currentAction.damage >= currentAction.startingHealth)
		{
			isFatalAttack = true;
		}
		else
		{
			isFatalAttack = false;
		}
		if (currentAction.emoteString.Length > 0)
		{
			KeeperNoDamageEmote(currentAction);
		}
		else if (currentAction.blocker)
		{
			actionReady = true;
			attackTimeoutTimestamp = Time.time + 5f;
		}
		else if (!waitingForAction)
		{
			ApproachTarget(currentAction.targetCharacter, true);
		}
		waitingForAction = false;
		begunAction = false;
		begunBlocking = false;
		makeTargetBlock = true;
		cameraResetTimestamp = 0f;
		endEmoteTimestamp = 0f;
	}

	public void KeeperNoDamageEmote(SquadBattleAction action)
	{
		if (charGlobals.behaviorManager.getBehavior() is BehaviorMovementSquadBattle)
		{
			BehaviorEmote behaviorEmote = charGlobals.behaviorManager.requestChangeBehavior(typeof(BehaviorEmote), true) as BehaviorEmote;
			behaviorEmote.Initialize(EmotesDefinition.Instance.GetEmoteByCommand(action.emoteString).id);
			characterCamera();
		}
	}

	public void WaitForAction()
	{
		waitingForAction = true;
	}

	public void moveToTargetOffset(GameObject targetCharacter, float angle, bool approachAllowed)
	{
		SquadBattleCharacterAI squadBattleCharacterAI = targetCharacter.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
		Vector3 a = Quaternion.AngleAxis(angle, Vector3.up) * ((!(squadBattleCharacterAI != null) || !(squadBattleCharacterAI.homeLocator != null)) ? targetCharacter.transform.forward : squadBattleCharacterAI.homeLocator.transform.forward);
		Vector3 vector = targetCharacter.transform.position + a * 5f;
		if (!approachAllowed)
		{
			float num = Vector3.Distance(targetCharacter.transform.position, base.transform.position);
			if (num > Vector3.Distance(targetCharacter.transform.position, vector))
			{
				return;
			}
		}
		if (!(Vector3.Distance(base.transform.position, vector) < 1f))
		{
			Vector3 newFacing = targetCharacter.transform.position - vector;
			newFacing.Normalize();
			charGlobals.motionController.setDestination(vector, newFacing);
			if (!(charGlobals.behaviorManager.getBehavior() is BehaviorMovementSquadBattle))
			{
				homeLocation = vector;
				homeFacing = newFacing;
			}
		}
	}

	public void ApproachTarget(GameObject targetCharacter, bool isSolo)
	{
		waitingForAction = true;
		actionReady = false;
		arrived = false;
		attackTimeoutTimestamp = Time.time + 5f;
		if (isSolo)
		{
			moveToTargetOffset(targetCharacter, (owningPlayer != 0) ? 15f : (-15f), true);
			characterCamera();
		}
	}

	public void characterCamera()
	{
		characterCamera(0f, false);
	}

	public void characterCamera(float extraRotation, bool forceCut)
	{
		if (cameraMan != null)
		{
			CameraLiteSelection cameraLiteSelection = cameraMan.CharacterCamera();
			if (cameraLiteSelection == null)
			{
				CspUtils.DebugLog("No character camera available");
				return;
			}
			cameraLiteSelection.camera.SetTarget(base.gameObject.transform);
			CameraLiteOffset cameraLiteOffset = cameraLiteSelection.camera as CameraLiteOffset;
			cameraLiteOffset.SetReverse(owningPlayer == SquadBattlePlayerEnum.Right);
			cameraLiteOffset.SetExtraRotation(extraRotation);
			cameraMan.ReplaceCamera(cameraLiteSelection.camera, (!forceCut) ? cameraLiteSelection.blendTime : 0f);
		}
	}

	public void SpectatorCamera()
	{
		if (cameraMan != null)
		{
			CameraLiteSelection cameraLiteSelection = cameraMan.ArenaCamera();
			cameraMan.ReplaceCamera(cameraLiteSelection.camera, -1f);
		}
	}

	public void Arrived()
	{
		attackTimeoutTimestamp = 0f;
		arrived = true;
		actionReady = true;
	}

	public bool HasAction()
	{
		return currentAction != null || waitingForAction;
	}

	public bool IsCurrentActionChaining()
	{
		return currentAction.attackPattern.attackSequence[attackSequenceIndex].chain;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (!(hit.rigidbody != null) || !(hit.transform.parent != null))
		{
			return;
		}
		SquadBattleSmashable squadBattleSmashable = hit.transform.parent.GetComponent(typeof(SquadBattleSmashable)) as SquadBattleSmashable;
		if (!squadBattleSmashable)
		{
			return;
		}
		if (charGlobals.motionController.IsForcedVelocity() || charGlobals.behaviorManager.getBehavior() is BehaviorRecoil)
		{
			squadBattleSmashable.TriggerHit(base.gameObject);
			return;
		}
		charGlobals.motionController.jumpPressed();
		if (homeLocator != null)
		{
			Vector3 vector = charGlobals.motionController.getDestination() - base.transform.position;
			vector.y = 0f;
			if (vector.sqrMagnitude < 0.1f)
			{
				charGlobals.motionController.setDestination(homeLocator.transform.position, homeLocator.transform.forward);
			}
		}
	}

	public void Victory()
	{
		victorious = true;
	}

	public void Defeat()
	{
		defeated = true;
	}

	public void ClearDefeat()
	{
		defeated = false;
		hasDied = false;
		endEmoteTimestamp = 0f;
	}

	public void Intro()
	{
		StartCoroutine(IntroSequence());
	}

	protected IEnumerator IntroSequence()
	{
		CspUtils.DebugLog("owningPlayer1 = " + owningPlayer);
		if (owningPlayer == SquadBattlePlayerEnum.Right)
		{
			yield return new WaitForSeconds(1f);
		}
		CameraLiteSelection introCam = cameraMan.IntroCamera();
		introCam.camera.SetTarget(base.transform);
		CameraLiteOffset clo = introCam.camera as CameraLiteOffset;
		clo.SetReverse(owningPlayer == SquadBattlePlayerEnum.Right);
		cameraMan.ReplaceCamera(introCam.camera, -1f);
		while (!CardGameController.Instance.StartTransaction.IsCompleted)
		{
			yield return 0;
		}
		if (owningPlayer == SquadBattlePlayerEnum.Left)
		{
			yield return new WaitForSeconds(2f);
		}
		bool introEmote = DoIntroBehavior();
		float timeout = Time.time + (float)((!introEmote) ? 4 : 30);
		while (Time.time < timeout && !(charGlobals.behaviorManager.getBehavior() is BehaviorMovementSquadBattle))
		{
			yield return 0;
		}
		yield return new WaitForSeconds(0.5f);
		CspUtils.DebugLog("owningPlayer2 = " + owningPlayer);
		if (owningPlayer == SquadBattlePlayerEnum.Left)
		{
			AppShell.Instance.EventMgr.Fire(this, new CardGameEvent.ShowVersusIcon());
			SquadBattleCharacterController.Instance.SpawnRightAvatar();
			yield break;
		}
		SquadBattleCharacterController.Instance.Bow();
		CameraLiteSelection arenaCam = cameraMan.ArenaCamera();
		cameraMan.ReplaceCamera(arenaCam.camera, -1f);
		AppShell.Instance.EventMgr.Fire(this, new CardGameEvent.IntroSequenceFinished());
		RoomAgent.Instance.Resume();
		CardGameController.Instance.Hud.UpdateScreenSize();
		CardGameController.Instance.Hud.ShowPanels(2f);
	}

	public bool DoIntroBehavior()
	{
		BehaviorEmote behaviorEmote = charGlobals.behaviorManager.requestChangeBehavior(typeof(BehaviorEmote), false) as BehaviorEmote;
		if (Array.IndexOf(ignoreIntroForCardGame, base.name) == -1 && behaviorEmote != null && behaviorEmote.Initialize(EmotesDefinition.Instance.GetEmoteByCommand("intro").id))
		{
			return true;
		}
		if (isVillain)
		{
			CombatController.AttackData attackData = charGlobals.combatController.getAttackData(1);
			charGlobals.combatController.createAttackBehavior(null, attackData, false, true);
		}
		else
		{
			playRandomEmote(intro_emotes);
		}
		return false;
	}

	public void DoFidget()
	{
	}

	protected void playRandomEmote(KeyValuePair<int, string>[] emotes)
	{
		string command = "none";
		int num = UnityEngine.Random.Range(0, 100);
		int num2 = 0;
		for (int i = 0; i < emotes.Length; i++)
		{
			KeyValuePair<int, string> keyValuePair = emotes[i];
			num2 += keyValuePair.Key;
			if (num < num2)
			{
				command = keyValuePair.Value;
				break;
			}
		}
		BehaviorEmote behaviorEmote = charGlobals.behaviorManager.requestChangeBehavior(typeof(BehaviorEmote), true) as BehaviorEmote;
		if (!behaviorEmote.Initialize(EmotesDefinition.Instance.GetEmoteByCommand(command).id))
		{
			charGlobals.behaviorManager.endBehavior();
		}
	}

	public bool CanBow()
	{
		return charGlobals.animationComponent["emote_bow"] != null;
	}

	public void Bow()
	{
		BehaviorEmote behaviorEmote = charGlobals.behaviorManager.requestChangeBehavior(typeof(BehaviorEmote), true) as BehaviorEmote;
		behaviorEmote.Initialize(EmotesDefinition.Instance.GetEmoteByCommand("bow").id);
	}
}
