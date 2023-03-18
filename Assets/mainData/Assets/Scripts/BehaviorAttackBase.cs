using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorAttackBase : BehaviorBase, IAnimTagListener
{
	public enum AttackCategory
	{
		Primary,
		Secondary,
		Super
	}

	protected CombatController.AttackData attackData;

	protected ShsCharacterController owningCharacterController;

	protected ShsCharacterController targetCharacterController;

	protected CombatController targetCombatController;

	protected Vector3 originalFacing;

	protected string animName;

	protected string baseAnim;

	protected bool continueAttackingTarget;

	public bool canChain;

	protected bool nextAttackAttempted;

	public AttackCategory category;

	protected int motionFrames;

	protected Queue<ImpactBase> impactInstances;

	protected bool mustUseMovementTags;

	protected bool movementEnabled;

	protected bool chainAttackNow;

	protected bool forciblyEnded;

	protected bool targetDead;

	protected bool durationEnded;

	protected bool startAnim;

	public bool firstUpdate;

	public bool suppressBroadcast;

	protected bool chainAttack;

	protected float forwardSpeed;

	protected GameObject createdEffect;

	protected float collisionOffTime = -1f;

	public string AttackName
	{
		get
		{
			return attackData.attackName;
		}
	}

	void IAnimTagListener.OnPinballStartAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnPinballEndAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnMultishotInfoAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnMoveStartAnimTag(AnimationEvent evt)
	{
		if (attackData.moveStartTime == 0f)
		{
			movementEnabled = true;
		}
	}

	void IAnimTagListener.OnMoveEndAnimTag(AnimationEvent evt)
	{
		if (attackData.moveArriveTime == 0f)
		{
			movementEnabled = false;
			if (mustUseMovementTags)
			{
				forwardSpeed = 0f;
				charGlobals.motionController.setForcedVelocityDuration(0f);
			}
		}
	}

	void IAnimTagListener.OnChainAttackAnimTag(AnimationEvent evt)
	{
		if (attackData.transitionStartTime == 0f && attackData.attackDuration == 0f)
		{
			chainAttackNow = true;
		}
	}

	void IAnimTagListener.OnCollisionEnableAnimTag(AnimationEvent evt)
	{
		if (impactInstances != null)
		{
			int animationEventIndex = GetAnimationEventIndex(evt);
			foreach (ImpactBase impactInstance in impactInstances)
			{
				if (impactInstance.index == animationEventIndex)
				{
					ImpactMelee impactMelee = impactInstance as ImpactMelee;
					if (impactMelee != null && impactMelee.impactData.impactStartTime == 0f)
					{
						impactMelee.colliderEnabled = true;
					}
				}
			}
		}
	}

	void IAnimTagListener.OnCollisionDisableAnimTag(AnimationEvent evt)
	{
		if (impactInstances != null)
		{
			int num = 0;
			string stringParameter = evt.stringParameter;
			if (stringParameter != string.Empty)
			{
				num = int.Parse(stringParameter) - 1;
			}
			foreach (ImpactBase impactInstance in impactInstances)
			{
				if (impactInstance.index == num)
				{
					ImpactMelee impactMelee = impactInstance as ImpactMelee;
					if (impactMelee != null && impactMelee.impactData.impactEndTime == 0f)
					{
						impactMelee.colliderDisabled = true;
					}
				}
			}
		}
	}

	void IAnimTagListener.OnProjectileFireAnimTag(AnimationEvent evt)
	{
		int num = GetAnimationEventIndex(evt);
		foreach (ImpactBase impactInstance in impactInstances)
		{
			if (impactInstance.index == num && impactInstance.impactData.firingTime == 0f)
			{
				if (impactInstance.IsFired())
				{
					num++;
				}
				else
				{
					impactInstance.fireNow = true;
				}
			}
		}
	}

	void IAnimTagListener.OnTriggerEffectAnimTag(AnimationEvent evt)
	{
		if (!string.IsNullOrEmpty(attackData.triggeredEffectName))
		{
			DestroyEffect();
			charGlobals.effectsList.TryOneShot(attackData.triggeredEffectName);
		}
	}

	public override void behaviorBegin()
	{
		firstUpdate = false;
		base.behaviorBegin();
	}

	public virtual void Initialize(GameObject newTargetObject, CombatController.AttackData newAttackData, bool newSecondaryAttack, bool newChainAttack, float emoteBroadcastRadius)
	{
		attackData = newAttackData;
		category = (newSecondaryAttack ? AttackCategory.Secondary : AttackCategory.Primary);
		setTarget(newTargetObject);
		chainAttack = newChainAttack;
		if (attackData.forwardOnHit)
		{
			forwardSpeed = 0f;
		}
		else
		{
			forwardSpeed = newAttackData.forwardSpeed;
		}
		if (emoteBroadcastRadius > 0f)
		{
			BroadcastEmote(emoteBroadcastRadius);
		}
		if (attackData.collisionOffTime >= 0f)
		{
			collisionOffTime = attackData.collisionOffTime;
		}
	}

	public virtual void behaviorFirstUpdate()
	{
		mustUseMovementTags = false;
		movementEnabled = false;
		chainAttackNow = false;
		durationEnded = false;
		startAnim = false;
		bool flag = true;
		PlayerCombatController playerCombatController = combatController as PlayerCombatController;
		if (playerCombatController != null)
		{
			flag = playerCombatController.autoChaining;
			if (attackData.powerCost > 0f && (!(charGlobals.spawnData != null) || (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Remote) <= CharacterSpawn.Type.Unknown))
			{
				if (!playerCombatController.checkPower(attackData))
				{
					CspUtils.DebugLog("Attack initialized with insufficient power - this should not occur");
					charGlobals.behaviorManager.endBehavior();
					return;
				}
				playerCombatController.decreasePower(attackData.powerCost);
			}
		}
		combatController.announceAttackBegin(category == AttackCategory.Secondary);
		nextAttackAttempted = false;
		if (!(charGlobals.squadBattleCharacterAI != null))
		{
			if (category == AttackCategory.Secondary || !combatController.advanceAttackChain())
			{
				canChain = false;
				combatController.resetAttackChain();
			}
			else
			{
				canChain = true;
			}
		}
		owningCharacterController = (owningObject.GetComponent(typeof(ShsCharacterController)) as ShsCharacterController);
		if (targetObject != null)
		{
			targetCharacterController = (targetObject.GetComponent(typeof(ShsCharacterController)) as ShsCharacterController);
			targetCombatController = (targetObject.GetComponent(typeof(CombatController)) as CombatController);
			if (charGlobals.spawnData == null)
			{
				continueAttackingTarget = false;
			}
			else if (charGlobals.spawnData.spawnType == CharacterSpawn.Type.LocalPlayer)
			{
				if (targetCombatController.CharGlobals != null && targetObject != GUIManager.Instance.GetTargetedEnemy())
				{
					GUIManager.Instance.DetachAttackingIndicator();
					GUIManager.Instance.AttachAttackingEnemyIndicator(targetObject);
				}
				GUIManager.Instance.AttachHealthBarIndicator(targetObject);
				continueAttackingTarget = (category != AttackCategory.Secondary && canChain && flag);
				if (charGlobals.motionController != null && charGlobals.motionController.carriedThrowable != null)
				{
					continueAttackingTarget = false;
				}
			}
			else if (charGlobals.networkComponent == null || charGlobals.networkComponent.IsOwner())
			{
				continueAttackingTarget = canChain;
			}
			else
			{
				continueAttackingTarget = false;
			}
			originalFacing = targetObject.transform.position - owningObject.transform.position;
		}
		else
		{
			targetCharacterController = null;
			continueAttackingTarget = false;
		}
		animName = attackData.animName;
		baseAnim = animName;
		if (attackData.attackDuration > 0f)
		{
			animationComponent[animName].wrapMode = WrapMode.Loop;
		}
		else if (animationComponent[animName + "_catch"] != null || animationComponent[animName + "_end"] != null)
		{
			animationComponent[animName].wrapMode = WrapMode.ClampForever;
		}
		if (!chainAttack && animationComponent[animName + "_start"] != null)
		{
			animName += "_start";
			startAnim = true;
		}
		if (animationComponent.IsPlaying(animName))
		{
			animationComponent.Stop();
			animationComponent.Rewind();
			animationComponent.Play(animName);
		}
		else
		{
			animationComponent.CrossFade(animName, 0.1f);
		}
		motionFrames = 0;
		charGlobals.motionController.setDestination(owningObject.transform.position);
		targetDead = false;
		impactInstances = new Queue<ImpactBase>();
		for (int i = 0; i < attackData.impacts.Length; i++)
		{
			ImpactBase item = CombatController.ImpactData.CreateImpact(attackData, i, charGlobals, targetCombatController);
			impactInstances.Enqueue(item);
		}
		if (attackData.effectName != null)
		{
			GameObject gameObject = null;
			gameObject = (charGlobals.combatController.effectSequenceSource.GetEffectSequencePrefabByName(attackData.effectName) as GameObject);
			if (gameObject != null)
			{
				createdEffect = (UnityEngine.Object.Instantiate(gameObject) as GameObject);
				createdEffect.SendMessage("AssignCreator", charGlobals, SendMessageOptions.DontRequireReceiver);
				createdEffect.name = "AttackEffect_" + gameObject.name;
				Utils.AttachGameObject(charGlobals.gameObject, createdEffect);
				EffectSequence component = createdEffect.GetComponent<EffectSequence>();
				if (component != null)
				{
					component.ScaleEffectsToOwner = true;
				}
			}
		}
		if (attackData.targetEffectName != null && targetObject != null)
		{
			GameObject gameObject2 = charGlobals.combatController.effectSequenceSource.GetEffectSequencePrefabByName(attackData.targetEffectName) as GameObject;
			if (gameObject2 != null)
			{
				GameObject gameObject3 = UnityEngine.Object.Instantiate(gameObject2) as GameObject;
				gameObject3.SendMessage("AssignCreator", charGlobals, SendMessageOptions.DontRequireReceiver);
				gameObject3.transform.rotation = owningObject.transform.rotation;
				Utils.AttachGameObject(targetObject, gameObject3);
			}
		}
		if (networkComponent != null && networkComponent.IsOwner())
		{
			broadcastAction();
		}
		forciblyEnded = true;
		combatController.currentAttackData = attackData;
	}

	protected virtual void broadcastAction()
	{
		if (!suppressBroadcast)
		{
			NetActionAttack action = new NetActionAttack(owningObject, targetObject, attackData.attackName);
			networkComponent.QueueNetAction(action);
		}
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		if (!firstUpdate)
		{
			firstUpdate = true;
			behaviorFirstUpdate();
			return;
		}
		if (targetObject != null && !targetObject.active)
		{
			targetObject = null;
			targetCombatController = null;
		}
		if (targetCombatController != null && charGlobals.spawnData != null && targetCombatController.isKilled)
		{
			targetDead = true;
			continueAttackingTarget = false;
		}
		if (charGlobals.spawnData == null || (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Local) != 0)
		{
			if (continueAttackingTarget)
			{
				PlayerCombatController x = base.combatController as PlayerCombatController;
				if (x != null && SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Right))
				{
					continueAttackingTarget = false;
				}
			}
			if (canChain && (chainAttackNow || (elapsedTime >= attackData.transitionStartTime && attackData.transitionStartTime != 0f && (elapsedTime < attackData.transitionEndTime || !nextAttackAttempted))))
			{
				chainAttackNow = false;
				nextAttackAttempted = true;
				GameObject gameObject = null;
				bool flag = false;
				BehaviorAttackQueue behaviorAttackQueue = charGlobals.behaviorManager.getQueuedBehavior() as BehaviorAttackQueue;
				if (behaviorAttackQueue != null)
				{
					if (!behaviorAttackQueue.getSecondaryAttack())
					{
						gameObject = behaviorAttackQueue.getTarget();
					}
				}
				else
				{
					BehaviorAttackApproach behaviorAttackApproach = charGlobals.behaviorManager.getQueuedBehavior() as BehaviorAttackApproach;
					if (behaviorAttackApproach != null)
					{
						if (!behaviorAttackApproach.secondaryAttack)
						{
							gameObject = behaviorAttackApproach.getTarget();
						}
					}
					else if (continueAttackingTarget)
					{
						gameObject = targetObject;
					}
				}
				if (charGlobals != null && charGlobals.spawnData != null && (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Player) != 0)
				{
					bool flag2 = false;
					if (gameObject != null)
					{
						CombatController combatController = gameObject.GetComponent(typeof(CombatController)) as CombatController;
						if (combatController.isKilled)
						{
							flag2 = true;
						}
					}
					if ((gameObject == null && targetDead) || flag2)
					{
						if (gameObject == null)
						{
							gameObject = targetObject;
						}
						CharacterGlobals characterGlobals = null;
						if (gameObject != null)
						{
							characterGlobals = (gameObject.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals);
						}
						if (characterGlobals != null)
						{
							CharacterGlobals characterGlobals2 = null;
							float num = 0f;
							Collider[] array = Physics.OverlapSphere(gameObject.transform.position, 5f, 2101248);
							Collider[] array2 = array;
							foreach (Collider collider in array2)
							{
								CharacterGlobals characterGlobals3 = collider.gameObject.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
								if (characterGlobals3 != null && characterGlobals3.combatController.faction == characterGlobals.combatController.faction && !characterGlobals3.combatController.isKilled)
								{
									float sqrMagnitude = (gameObject.transform.position - characterGlobals3.transform.position).sqrMagnitude;
									if (num == 0f || sqrMagnitude < num)
									{
										num = sqrMagnitude;
										characterGlobals2 = characterGlobals3;
									}
								}
							}
							if (characterGlobals2 != null)
							{
								gameObject = characterGlobals2.gameObject;
							}
						}
					}
				}
				if (gameObject != null)
				{
					forciblyEnded = false;
					if (base.combatController.beginChainAttack(gameObject, false, attackData.transitionRequiresImpact))
					{
						return;
					}
				}
				base.combatController.resetAttackChain();
				if (gameObject != null && (base.combatController.beginAttack(gameObject, false) || base.combatController.pursueTarget(gameObject, false)))
				{
					return;
				}
			}
		}
		if (attackData.attackDuration > 0f && elapsedTime >= attackData.attackDuration && animationComponent[animName + "_end"] != null)
		{
			animName += "_end";
			animationComponent.Play(animName);
			durationEnded = true;
			if (forwardSpeed != 0f)
			{
				charGlobals.motionController.setForcedVelocityDuration(0f);
			}
			if (attackData.effectName != null)
			{
				GameObject gameObject2 = null;
				gameObject2 = (charGlobals.combatController.effectSequenceSource.GetEffectSequencePrefabByName(attackData.effectName + "_end") as GameObject);
				if (gameObject2 != null)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate(gameObject2) as GameObject;
					gameObject3.SendMessage("AssignCreator", charGlobals, SendMessageOptions.DontRequireReceiver);
					gameObject3.name = "AttackEffect_" + gameObject2.name;
					Utils.AttachGameObject(charGlobals.gameObject, gameObject3);
				}
			}
		}
		if (checkEndAttack())
		{
			forciblyEnded = false;
			if (targetObject != null && (charGlobals.spawnData == null || charGlobals.spawnData.spawnType == CharacterSpawn.Type.LocalPlayer))
			{
				Vector3 position = targetObject.transform.position;
				float y = position.y;
				Vector3 position2 = owningObject.transform.position;
				if (Mathf.Abs(y - position2.y) < 10f)
				{
					if (!string.IsNullOrEmpty(attackData.autoChainAttackName))
					{
						if (!base.combatController.beginAttack(targetObject, category == AttackCategory.Secondary, false, attackData.autoChainAttackName))
						{
							base.combatController.pursueTarget(targetObject, category == AttackCategory.Secondary, false, attackData.autoChainAttackName);
						}
					}
					else if (continueAttackingTarget && charGlobals.behaviorManager.getQueuedBehavior() == null && !base.combatController.beginAttack(targetObject, false))
					{
						base.combatController.pursueTarget(targetObject, false);
					}
				}
			}
			if (charGlobals.squadBattleCharacterAI == null)
			{
				base.combatController.resetAttackChain();
			}
			charGlobals.behaviorManager.endBehavior();
			return;
		}
		if (targetObject != null)
		{
			if (attackData.faceTarget)
			{
				if (attackData.trackTarget)
				{
					originalFacing = targetObject.transform.position - owningObject.transform.position;
				}
				charGlobals.motionController.rotateTowards(originalFacing);
			}
			if (forwardSpeed == 0f && (movementEnabled || (elapsedTime >= attackData.moveStartTime && elapsedTime < attackData.moveArriveTime)))
			{
				Vector3 vector = validateDesiredPosition(calcDesiredPosition(attackData.desiredRange));
				if (vector != owningObject.transform.position)
				{
					charGlobals.motionController.moveTowards(vector, attackData.moveSpeed);
					charGlobals.motionController.setDestination(vector);
				}
				ChangeToForwardMotion();
			}
		}
		else if (attackData.faceTarget && originalFacing != Vector3.zero)
		{
			charGlobals.motionController.rotateTowards(originalFacing);
		}
		if (collisionOffTime >= 0f && elapsedTime >= collisionOffTime)
		{
			AnimationEvent animationEvent = new AnimationEvent();
			float num2 = 0f;
			if (attackData.collisionOnTime >= 0f && attackData.collisionOnTime > collisionOffTime)
			{
				num2 = attackData.collisionOnTime - elapsedTime;
			}
			else if (attackData.attackDuration > 0f)
			{
				num2 = attackData.attackDuration - elapsedTime;
			}
			animationEvent.stringParameter = num2.ToString();
			charGlobals.motionController.OnEnemyCollisionAnimTag(animationEvent);
			collisionOffTime = -1f;
		}
		if (forwardSpeed != 0f && !durationEnded && (!startAnim || movementEnabled))
		{
			charGlobals.motionController.setForcedVelocity(owningObject.transform.forward * forwardSpeed, 1f);
		}
	}

	protected Vector3 calcDesiredPosition(float range)
	{
		Vector3 b = targetObject.transform.position - owningObject.transform.position;
		float num = range + owningCharacterController.radius;
		num = ((!(targetCharacterController != null)) ? (num + targetCombatController.attackDistance) : (num + targetCharacterController.radius));
		if (b.magnitude > num)
		{
			b.Normalize();
			b *= num;
			return targetObject.transform.position - b;
		}
		return owningObject.transform.position;
	}

	protected Vector3 validateDesiredPosition(Vector3 desiredPosition)
	{
		Vector3 position = targetObject.transform.position;
		float y = position.y;
		Vector3 position2 = owningObject.transform.position;
		if (Mathf.Abs(y - position2.y) < 10f)
		{
			return desiredPosition;
		}
		Vector3 position3 = desiredPosition;
		Vector3 position4 = owningObject.transform.position;
		position3.y = position4.y + 10f;
		RaycastHit hitInfo;
		if (ShsCharacterController.FindGround(position3, 20f, out hitInfo))
		{
			return ShsCharacterController.FindPositionOnGround(owningObject.GetComponent<CharacterController>(), hitInfo.point, ShsCharacterController.GroundOffset);
		}
		return desiredPosition;
	}

	protected virtual bool checkEndAttack()
	{
		if (startAnim)
		{
			if (!animationComponent.IsPlaying(animName))
			{
				startAnim = false;
				animName = attackData.animName;
				animationComponent.Play(animName);
				elapsedTime = 0f;
			}
			return false;
		}
		return (attackData.attackDuration != 0f && !durationEnded) ? (elapsedTime >= attackData.attackDuration) : (!animationComponent.IsPlaying(animName));
	}

	public override void behaviorLateUpdate()
	{
		if (motionFrames < 2)
		{
			motionFrames++;
		}
		else if ((double)elapsedTime > 0.1 && !movementEnabled && (elapsedTime < attackData.moveStartTime || elapsedTime > attackData.moveArriveTime) && (targetObject == null || charGlobals.motionController.EnemyCollisionsDisabled || calcDesiredPosition(0f) != owningObject.transform.position))
		{
			charGlobals.motionController.performRootMotion(attackData.allowLateralTranslation);
		}
		if (impactInstances != null)
		{
			foreach (ImpactBase impactInstance in impactInstances)
			{
				impactInstance.ImpactUpdate(elapsedTime);
			}
		}
	}

	public override float GetBehaviorDuration()
	{
		return animationComponent[attackData.animName].length - elapsedTime;
	}

	public override void behaviorEnd(BehaviorBase newBehavior)
	{
		if (attackData.stopEffectOnChain && !forciblyEnded && (newBehavior is BehaviorAttackBase || newBehavior is BehaviorAttackApproach))
		{
			DestroyEffect(1);
		}
		base.behaviorEnd(newBehavior);
	}

	public override void behaviorEnd()
	{
		if (impactInstances != null)
		{
			foreach (ImpactBase impactInstance in impactInstances)
			{
				impactInstance.ImpactEnd();
			}
		}
		combatController.announceAttackEnd();
		if (!(charGlobals.spawnData != null) || charGlobals.spawnData.spawnType == CharacterSpawn.Type.LocalPlayer)
		{
		}
		if (forwardSpeed != 0f)
		{
			charGlobals.motionController.setForcedVelocityDuration(0f);
		}
		if (forciblyEnded)
		{
			combatController.resetAttackChain();
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < owningObject.transform.GetChildCount(); i++)
			{
				Transform child = owningObject.transform.GetChild(i);
				if (child.gameObject.name.Contains("AttackEffect_"))
				{
					list.Add(child.gameObject);
				}
			}
			foreach (GameObject item in list)
			{
				EffectSequence effectSequence = item.GetComponent(typeof(EffectSequence)) as EffectSequence;
				if (effectSequence != null)
				{
					effectSequence.StopSequence(false);
				}
				else
				{
					UnityEngine.Object.Destroy(item);
				}
			}
		}
		Component[] componentsInChildren = owningObject.GetComponentsInChildren(typeof(EffectSequence));
		Component[] array = componentsInChildren;
		for (int j = 0; j < array.Length; j++)
		{
			EffectSequence effectSequence2 = (EffectSequence)array[j];
			effectSequence2.AnimationInterrupted = true;
		}
		base.behaviorEnd();
	}

	public override bool behaviorEndOnCutScene()
	{
		return true;
	}

	public override bool allowUserInput()
	{
		return true;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		if (newBehaviorType.BaseType == typeof(BehaviorRecoil))
		{
			return true;
		}
		if (typeof(BehaviorAttackSpecial).IsAssignableFrom(newBehaviorType))
		{
			return true;
		}
		if (nextAttackAttempted && (newBehaviorType == typeof(BehaviorAttackApproach) || newBehaviorType == typeof(BehaviorAttackBase)))
		{
			return true;
		}
		if (newBehaviorType == typeof(BehaviorApproach))
		{
			return true;
		}
		return false;
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override void destinationChanged()
	{
		base.destinationChanged();
		if (networkComponent != null && networkComponent.IsOwner())
		{
			NetActionCancel action = new NetActionCancel();
			networkComponent.QueueNetAction(action);
		}
		charGlobals.behaviorManager.clearQueuedBehavior();
		charGlobals.behaviorManager.endBehavior();
	}

	public string getAttackName()
	{
		return attackData.attackName;
	}

	public CombatController.AttackData getAttackData()
	{
		return attackData;
	}

	private static int GetAnimationEventIndex(AnimationEvent evt)
	{
		string stringParameter = evt.stringParameter;
		if (!string.IsNullOrEmpty(stringParameter))
		{
			return int.Parse(stringParameter) - 1;
		}
		return 0;
	}

	public void OnProjectileReturned()
	{
		if (animationComponent[animName + "_catch"] != null)
		{
			animName += "_catch";
			animationComponent[animName].wrapMode = WrapMode.Once;
			animationComponent.CrossFade(animName, 0.1f);
		}
	}

	public void OnProjectileImpact(GameObject target)
	{
		if (animationComponent[animName + "_end"] != null)
		{
			targetObject = target;
			targetCharacterController = (targetObject.GetComponent(typeof(ShsCharacterController)) as ShsCharacterController);
			targetCombatController = (targetObject.GetComponent(typeof(CombatController)) as CombatController);
			foreach (ImpactBase impactInstance in impactInstances)
			{
				impactInstance.ChangeTarget(targetCombatController);
			}
			animName += "_end";
			animationComponent[animName].wrapMode = WrapMode.Once;
			animationComponent.CrossFade(animName, 0.1f);
		}
	}

	public void OnProjectileDestroyed()
	{
		if (animationComponent != null && animName != null && animationComponent[animName] != null && animationComponent[animName].wrapMode == WrapMode.ClampForever && (animationComponent[animName + "_end"] != null || animationComponent[animName + "_catch"] != null))
		{
			animationComponent.CrossFade("movement_idle", 0.5f);
		}
	}

	public override void motionCollided()
	{
		base.motionCollided();
		if (attackData.stopOnEnemyCollision)
		{
			if (attackData.attackDuration > 0f)
			{
				elapsedTime = attackData.attackDuration;
			}
			DestroyEffect();
		}
	}

	public void ChangeToForwardMotion()
	{
		if (attackData.forwardOnHit && combatController.CurrentAttackSuccessful)
		{
			if (attackData.forwardSpeed > 0f)
			{
				forwardSpeed = attackData.forwardSpeed;
			}
			else
			{
				forwardSpeed = attackData.moveSpeed * charGlobals.motionController.speed;
			}
			mustUseMovementTags = true;
		}
	}

	public void DestroyEffect()
	{
		if (createdEffect != null)
		{
			UnityEngine.Object.Destroy(createdEffect);
		}
	}

	public void DestroyEffect(int frameDelay)
	{
		if (createdEffect != null)
		{
			CoroutineContainer.GetInstance(createdEffect).StartCoroutine(DelayedEffectDestruction(frameDelay, createdEffect));
		}
	}

	private IEnumerator DelayedEffectDestruction(int frameDelay, GameObject objToDestroy)
	{
		for (int frame = 0; frame < frameDelay; frame++)
		{
			yield return 0;
		}
		UnityEngine.Object.Destroy(objToDestroy);
	}

	protected void BroadcastEmote(float radius)
	{
		Collider[] array = Physics.OverlapSphere(owningObject.transform.position, radius);
		foreach (Collider collider in array)
		{
			EmoteListener component = Utils.GetComponent<EmoteListener>(collider.gameObject);
			if (component != null)
			{
				component.OnEmoteBroadcast(EmotesDefinition.Instance.GetEmoteByCommand("cheer").id, owningObject);
			}
		}
	}

	public bool AnyImpactsFired()
	{
		if (impactInstances != null)
		{
			foreach (ImpactBase impactInstance in impactInstances)
			{
				if (impactInstance.Fired)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void CeaseAttack()
	{
		canChain = false;
		continueAttackingTarget = false;
	}
}
