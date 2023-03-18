using System.Collections.Generic;
using UnityEngine;

public class BehaviorAttackMultiShot : BehaviorAttackSpecial, IAnimTagListener
{
	protected enum MultiShotMode
	{
		hiding,
		waitForPinballTag,
		attacking,
		turning,
		showing,
		pinballReturn,
		finishing,
		waiting
	}

	protected enum RotationMode
	{
		none,
		usePinballRotation,
		facePinball
	}

	private CombatController.Faction oldFaction = CombatController.Faction.Neutral;

	private CombatController.Faction oppositeFaction = CombatController.Faction.Neutral;

	private bool hideOnTeleport;

	private float teleportStartTime;

	private bool hidden;

	private bool multiShotInit;

	private bool shotInfoInTags;

	private bool canBeHit = true;

	private MultiShotMode currentMode;

	private float modeTimer;

	private List<CombatController> targets;

	private float motionPauseStart;

	private float motionPauseFinish;

	private float attackHitDelay;

	private float defaultAnimSpeed = 1f;

	private float defaultAnimLength;

	private float travelAnimSpeed = 1f;

	private float timeDelayPercent;

	private float travelPercent = 1f;

	private float attackHitPercent = 1f;

	private float attackTime = 0.5f;

	private float nextTargetAt = 0.5f;

	private float minimumNextTarget;

	private bool newImpact = true;

	private int totalTargets = -1;

	private int targetIndex;

	private string teleportEffect = string.Empty;

	private GameObject pinballEffect;

	private int impactCounter;

	private bool hasImpact;

	private GameObject pinball;

	private Animation pinballAnimation;

	private bool linearPinball = true;

	private Vector3 startDirection = new Vector3(-1f, 0f, 0f);

	private Vector3 endDirection = new Vector3(1f, 0f, 0f);

	private Vector3 startPoint = new Vector3(0f, 0f, 0f);

	private Vector3 endPoint = new Vector3(0f, 0f, 0f);

	private GameObject pinballStartPoint;

	private float arcDistance;

	private float arcMultiplier;

	private RotationMode facingMode = RotationMode.facePinball;

	private float turnLengthSeconds;

	private Vector3 originalTurnDirection = Vector3.zero;

	private Vector3 previousPinballPoint = new Vector3(0f, 0f, 0f);

	private float estimatedSegmentLength = 1f;

	private bool hostMode = true;

	private bool netFirstPoint = true;

	private GameObject netCurrentTarget;

	private GameObject netNextTarget;

	protected float MotionStartPauseDuration
	{
		get
		{
			return motionPauseStart / defaultAnimSpeed;
		}
	}

	protected float MotionFinishPauseDuration
	{
		get
		{
			return motionPauseFinish / defaultAnimSpeed;
		}
	}

	protected float MotionPauseDuration
	{
		get
		{
			return MotionStartPauseDuration + MotionFinishPauseDuration;
		}
	}

	protected float MotionTravelDuration
	{
		get
		{
			return nextTargetAt - MotionPauseDuration;
		}
	}

	protected float TotalAnimationDuration
	{
		get
		{
			return defaultAnimLength / defaultAnimSpeed;
		}
	}

	protected float TravelAnimationDuration
	{
		get
		{
			return TotalAnimationDuration - MotionPauseDuration;
		}
	}

	protected GameObject CurrentTarget
	{
		get
		{
			if (hostMode)
			{
				if (targetIndex < targets.Count && targets[targetIndex] != null)
				{
					return targets[targetIndex].gameObject;
				}
				return pinballStartPoint;
			}
			return netNextTarget;
		}
	}

	void IAnimTagListener.OnMoveStartAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnMoveEndAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnChainAttackAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnCollisionEnableAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnCollisionDisableAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnProjectileFireAnimTag(AnimationEvent evt)
	{
		teleportStartTime = modeTimer;
	}

	void IAnimTagListener.OnPinballStartAnimTag(AnimationEvent evt)
	{
		animationComponent[baseAnim].speed = travelAnimSpeed;
		if (pinballAnimation != null)
		{
			pinballAnimation[pinballAnimation.clip.name].speed = travelAnimSpeed;
		}
	}

	void IAnimTagListener.OnPinballEndAnimTag(AnimationEvent evt)
	{
		animationComponent[baseAnim].speed = defaultAnimSpeed;
		if (pinballAnimation != null)
		{
			pinballAnimation[pinballAnimation.clip.name].speed = defaultAnimSpeed;
		}
	}

	void IAnimTagListener.OnMultishotInfoAnimTag(AnimationEvent evt)
	{
		if (!multiShotInit)
		{
			multiShotInit = true;
			defaultAnimSpeed = animationComponent[baseAnim].speed;
			defaultAnimLength = animationComponent[baseAnim].length;
			string[] array = evt.stringParameter.Split(':');
			if (array.Length >= 3)
			{
				motionPauseStart = float.Parse(array[0]);
				motionPauseFinish = defaultAnimLength - float.Parse(array[1]);
				attackHitPercent = float.Parse(array[2]) / defaultAnimLength;
			}
		}
	}

	protected void ShowCharacter()
	{
		hidePinball();
		if (hidden)
		{
			Utils.ActivateSkinnedMeshRenderers(owningObject, true);
			hidden = false;
			if (teleportEffect != null && teleportEffect != string.Empty)
			{
				combatController.createEffect(teleportEffect, owningObject);
			}
		}
	}

	protected void HideCharacter()
	{
		if (!hidden && hideOnTeleport)
		{
			Utils.ActivateSkinnedMeshRenderers(owningObject, false);
			hidden = true;
			if (teleportEffect != null && teleportEffect != string.Empty)
			{
				combatController.createEffect(teleportEffect, owningObject);
			}
		}
	}

	protected void CreatePinball()
	{
		if (attackData.pinballPrefab != null && attackData.pinballPrefab != string.Empty)
		{
			pinball = combatController.createEffect(attackData.pinballPrefab, null);
			pinball.transform.position = pinballStartPoint.transform.position;
			pinballAnimation = pinball.GetComponent<Animation>();
			linearPinball = attackData.linearPinball;
		}
	}

	protected void UpdatePinballAnimation()
	{
		timeDelayPercent = MotionStartPauseDuration / nextTargetAt;
		travelPercent = MotionTravelDuration / nextTargetAt;
		CalculateArcDistance();
		travelAnimSpeed = TravelAnimationDuration / MotionTravelDuration;
		if (hideOnTeleport)
		{
			animationComponent.Rewind();
			animationComponent[baseAnim].speed = defaultAnimSpeed;
			animationComponent.Play(baseAnim);
		}
		if (pinballAnimation != null)
		{
			pinballAnimation.Rewind();
			pinballAnimation[pinballAnimation.clip.name].speed = defaultAnimSpeed;
			pinballAnimation.Play(pinballAnimation.clip.name);
		}
		PlayPinballEffect();
	}

	protected void UpdateTargetPoints(bool allPoints)
	{
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		if (targets.Count <= 0 || pinball == null)
		{
			if (pinball != null && networkComponent != null)
			{
				NetActionMultishotUpdate action = new NetActionMultishotUpdate(1f, pinballStartPoint, pinballStartPoint);
				networkComponent.QueueNetAction(action);
			}
			nextTargetAt = 1f;
			return;
		}
		while (targetIndex < targets.Count && !IsPinballTarget(targets[targetIndex]))
		{
			targets.RemoveAt(targetIndex);
		}
		Vector3 a = endPoint;
		gameObject = ((targetIndex >= targets.Count) ? pinballStartPoint : targets[targetIndex].gameObject);
		endPoint = gameObject.transform.position;
		gameObject2 = pinballStartPoint;
		if (targetIndex + 1 < targets.Count)
		{
			gameObject2 = targets[targetIndex + 1].gameObject;
		}
		Vector3 position = gameObject2.transform.position;
		if (allPoints)
		{
			if (targetIndex == 0)
			{
				startPoint = pinballStartPoint.transform.position;
				previousPinballPoint = startPoint;
				startDirection = (endPoint - startPoint).normalized;
			}
			else
			{
				startDirection = (a - startPoint).normalized;
				startPoint = a;
			}
		}
		endDirection = (position - endPoint).normalized;
		estimatedSegmentLength = (endPoint - startPoint).magnitude;
		if (allPoints)
		{
			nextTargetAt = Mathf.Max(estimatedSegmentLength / attackTime, minimumNextTarget) + MotionPauseDuration;
			UpdatePinballAnimation();
			if (networkComponent != null)
			{
				NetActionMultishotUpdate action2 = new NetActionMultishotUpdate(nextTargetAt, (!(gameObject == pinballStartPoint)) ? gameObject : owningObject, (!(gameObject2 == pinballStartPoint)) ? gameObject2 : owningObject);
				networkComponent.QueueNetAction(action2);
			}
		}
	}

	protected void CalculateArcDistance()
	{
		arcDistance = charGlobals.motionController.gravity * (nextTargetAt * nextTargetAt) * (travelPercent * travelPercent) * 0.25f * arcMultiplier;
	}

	protected void UpdatePinballPosition()
	{
		if (!(pinball != null))
		{
			return;
		}
		float num = Mathf.Clamp01((modeTimer - nextTargetAt * timeDelayPercent) / (nextTargetAt * travelPercent));
		float num2 = 0f;
		num2 = ((!(num < 0.5f)) ? (2f * (1f - num)) : (2f * num));
		num2 = 1f - (1f - num2) * (1f - num2);
		num2 *= arcDistance;
		if (linearPinball)
		{
			pinball.transform.position = startPoint * (1f - num) + endPoint * num;
			Vector3 forward = endPoint - startPoint;
			forward.y = 0f;
			pinball.transform.rotation = Quaternion.LookRotation(forward);
		}
		else
		{
			Vector3 a = startPoint - startDirection * estimatedSegmentLength;
			Vector3 a2 = startPoint;
			Vector3 vector = endPoint;
			Vector3 b = endPoint + endDirection * estimatedSegmentLength;
			pinball.transform.position = 0.5f * (2f * a2 + (-a + vector) * num + (2f * a - 5f * a2 + 4f * vector - b) * (num * num) + (-a + 3f * a2 - 3f * vector + b) * (num * num * num));
			Vector3 forward2 = pinball.transform.position - previousPinballPoint;
			forward2.y = 0f;
			if (forward2.magnitude > 0.01f)
			{
				pinball.transform.rotation = Quaternion.LookRotation(forward2);
				previousPinballPoint = pinball.transform.position;
			}
		}
		pinball.transform.position += new Vector3(0f, num2, 0f);
	}

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		animationComponent.animateOnlyIfVisible = false;
		targets = new List<CombatController>();
		AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(OnEnemyDespawned);
		if (networkComponent == null)
		{
			hostMode = true;
		}
		else
		{
			hostMode = networkComponent.IsOwner();
		}
	}

	public override void behaviorFirstUpdate()
	{
		base.behaviorFirstUpdate();
		hideOnTeleport = attackData.hideOnTeleport;
		totalTargets = attackData.totalTargets;
		attackTime = attackData.attackDelay;
		nextTargetAt = attackTime;
		minimumNextTarget = attackData.minimumDelay;
		shotInfoInTags = attackData.hasShotInfoTag;
		motionPauseFinish = attackData.pinballPause;
		teleportEffect = attackData.teleportEffect;
		arcMultiplier = attackData.arcMultiplier;
		turnLengthSeconds = attackData.pinballTurnDuration;
		canBeHit = attackData.pinballCanBeHit;
		oldFaction = base.combatController.faction;
		if (owningObject == GameController.GetController().LocalPlayer)
		{
			oldFaction = CombatController.Faction.Player;
		}
		if (!canBeHit)
		{
			base.combatController.faction = CombatController.Faction.Neutral;
		}
		oppositeFaction = CombatController.Faction.Enemy;
		if (oldFaction == CombatController.Faction.Enemy)
		{
			oppositeFaction = CombatController.Faction.Player;
		}
		if (attackData.pinballPrefab == null || attackData.pinballPrefab == string.Empty)
		{
			facingMode = RotationMode.none;
		}
		else if (!hideOnTeleport)
		{
			facingMode = RotationMode.facePinball;
		}
		else
		{
			facingMode = RotationMode.usePinballRotation;
		}
		if (startAnim)
		{
			teleportStartTime = animationComponent[animName].length / animationComponent[animName].speed;
		}
		if (!string.IsNullOrEmpty(attackData.pinballStartNode))
		{
			Transform transform = Utils.FindNodeInChildren(owningObject.transform, attackData.pinballStartNode);
			pinballStartPoint = ((!(transform != null)) ? owningObject : transform.gameObject);
		}
		else
		{
			pinballStartPoint = owningObject;
		}
		defaultAnimLength = animationComponent[animName].length;
		modeTimer = 0f;
		CombatController[] array = Utils.FindObjectsOfType<CombatController>();
		int num = 0;
		if (array != null && array.Length > 0)
		{
			CombatController[] array2 = array;
			foreach (CombatController combatController in array2)
			{
				if (combatController as ObjectCombatController != null || !IsPinballTarget(combatController))
				{
					continue;
				}
				targets.Add(combatController);
				if (totalTargets != -1)
				{
					num++;
					if (num >= totalTargets)
					{
						break;
					}
				}
			}
		}
		if (targets.Count == 0)
		{
			BehaviorEmote behaviorEmote = charGlobals.behaviorManager.forceChangeBehavior(typeof(BehaviorEmote)) as BehaviorEmote;
			if (behaviorEmote != null)
			{
				EmotesDefinition.EmoteDefinition emoteByCommand = EmotesDefinition.Instance.GetEmoteByCommand("confused");
				if (emoteByCommand == null)
				{
					charGlobals.behaviorManager.endBehavior();
				}
				else if (!behaviorEmote.Initialize(emoteByCommand.id, true, true, 10f))
				{
					charGlobals.behaviorManager.endBehavior();
				}
			}
		}
		if (attackData.impacts != null && attackData.impacts.Length > 0)
		{
			hasImpact = true;
		}
	}

	public override void behaviorEnd()
	{
		ShowCharacter();
		AppShell.Instance.EventMgr.RemoveListener<EntityDespawnMessage>(OnEnemyDespawned);
		combatController.faction = oldFaction;
		base.behaviorEnd();
	}

	protected void UpdateRotation()
	{
		if (facingMode == RotationMode.none)
		{
			return;
		}
		if (currentMode == MultiShotMode.hiding)
		{
			if (targets != null && targets.Count > 0)
			{
				Vector3 target = targets[0].transform.position - owningObject.transform.position;
				target.y = 0f;
				Vector3 forward = Vector3.RotateTowards(owningObject.transform.forward, target, 4.468043f * Time.deltaTime, 1000f);
				owningObject.transform.rotation = Quaternion.LookRotation(forward);
				charGlobals.motionController.forceNewFacing(owningObject.transform.forward);
			}
		}
		else if (pinball != null)
		{
			if (facingMode != RotationMode.facePinball)
			{
				owningObject.transform.rotation = pinball.transform.rotation;
			}
			charGlobals.motionController.forceNewFacing(owningObject.transform.forward);
		}
	}

	protected void UpdateTurning()
	{
		if (pinball != null && CurrentTarget == null)
		{
			if (originalTurnDirection == Vector3.zero)
			{
				originalTurnDirection = pinball.transform.forward;
			}
			Vector3 to = CurrentTarget.transform.position - pinball.transform.position;
			Vector3 forward = Vector3.Slerp(originalTurnDirection, to, modeTimer / turnLengthSeconds);
			pinball.transform.rotation = Quaternion.LookRotation(forward);
		}
		if (modeTimer >= turnLengthSeconds)
		{
			currentMode = MultiShotMode.attacking;
			if (hostMode)
			{
				UpdateTargetPoints(true);
			}
			modeTimer = 0f;
			originalTurnDirection = Vector3.zero;
		}
	}

	protected void hostUpdate()
	{
		if (currentMode == MultiShotMode.hiding)
		{
			UpdateTargetPoints(false);
			if (modeTimer >= teleportStartTime)
			{
				HideCharacter();
				modeTimer = 0f;
				CreatePinball();
				currentMode = MultiShotMode.waitForPinballTag;
				animationComponent[baseAnim].time = 0f;
				animationComponent.Play(baseAnim);
			}
		}
		if (currentMode == MultiShotMode.waitForPinballTag && (!shotInfoInTags || multiShotInit))
		{
			currentMode = MultiShotMode.attacking;
			UpdateTargetPoints(true);
		}
		if (currentMode == MultiShotMode.attacking)
		{
			if (targets == null || targetIndex >= targets.Count)
			{
				currentMode = MultiShotMode.pinballReturn;
				modeTimer = 0f;
				if (targetIndex == 0)
				{
					modeTimer = 1f;
				}
				pinball.BroadcastMessage("OnPinballReturnBegan", this, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				if (modeTimer >= nextTargetAt * attackHitPercent - attackHitDelay && newImpact)
				{
					newImpact = false;
					if (hasImpact)
					{
						combatController.attackHit(targets[targetIndex].transform.position, targets[targetIndex], attackData, attackData.impacts[impactCounter]);
					}
				}
				if (modeTimer >= nextTargetAt)
				{
					targetIndex++;
					impactCounter = (impactCounter + 1) % attackData.impacts.Length;
					modeTimer = 0f;
					newImpact = true;
					currentMode = MultiShotMode.turning;
				}
			}
			if (currentMode == MultiShotMode.attacking)
			{
				if (newImpact)
				{
					UpdateTargetPoints(false);
				}
				UpdatePinballPosition();
			}
		}
		if (currentMode == MultiShotMode.turning)
		{
			UpdateTurning();
		}
		if (currentMode == MultiShotMode.pinballReturn)
		{
			if (pinball != null && modeTimer < nextTargetAt)
			{
				UpdatePinballPosition();
			}
			else
			{
				currentMode = MultiShotMode.showing;
				hidePinball();
			}
		}
		if (currentMode == MultiShotMode.showing)
		{
			ShowCharacter();
			currentMode = MultiShotMode.finishing;
		}
		if (currentMode == MultiShotMode.finishing)
		{
			Finish();
			currentMode = MultiShotMode.waiting;
		}
		UpdateRotation();
	}

	protected void UpdateNetTargetPoints(bool allPoints)
	{
		if (netCurrentTarget == null)
		{
			netCurrentTarget = pinballStartPoint;
		}
		Vector3 a = endPoint;
		RaycastHit hitInfo;
		if (ShsCharacterController.FindGround(netCurrentTarget.transform.position, 100f, out hitInfo))
		{
			endPoint = hitInfo.point;
		}
		else
		{
			endPoint = netCurrentTarget.transform.position;
		}
		if (netFirstPoint)
		{
			a = endPoint;
		}
		if (allPoints)
		{
			if (netFirstPoint)
			{
				startPoint = pinballStartPoint.transform.position;
				previousPinballPoint = startPoint;
				startDirection = (endPoint - startPoint).normalized;
				netFirstPoint = false;
			}
			else
			{
				startDirection = (a - startPoint).normalized;
				startPoint = a;
				modeTimer = 0f;
			}
			UpdatePinballAnimation();
		}
		if (netNextTarget != null)
		{
			Vector3 position = netNextTarget.transform.position;
			endDirection = (position - endPoint).normalized;
		}
		estimatedSegmentLength = (endPoint - startPoint).magnitude;
	}

	public void GetNewTargets(float attackTime, GameObject target, GameObject nextTarget)
	{
		if (target == owningObject)
		{
			target = pinballStartPoint;
		}
		if (nextTarget == owningObject)
		{
			nextTarget = pinballStartPoint;
		}
		nextTargetAt = attackTime;
		netCurrentTarget = target;
		netNextTarget = nextTarget;
		if (netCurrentTarget == null)
		{
			netCurrentTarget = pinballStartPoint;
		}
		if (netNextTarget == null)
		{
			netNextTarget = pinballStartPoint;
		}
		UpdateNetTargetPoints(true);
	}

	protected void clientUpdate()
	{
		if (networkComponent == null || networkComponent.IsOwner())
		{
			hostMode = true;
			CspUtils.DebugLog("unexpectedly owner in clientupdate");
			if (pinball != null)
			{
				modeTimer = 1f;
				currentMode = MultiShotMode.pinballReturn;
				pinball.BroadcastMessage("OnPinballReturnBegan", this, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				Finish();
			}
			return;
		}
		if (currentMode == MultiShotMode.hiding && netCurrentTarget != null)
		{
			UpdateNetTargetPoints(false);
			if (modeTimer >= teleportStartTime)
			{
				HideCharacter();
				modeTimer = 0f;
				CreatePinball();
				currentMode = MultiShotMode.waitForPinballTag;
				animationComponent[baseAnim].time = 0f;
				animationComponent.Play(baseAnim);
				return;
			}
		}
		if (currentMode == MultiShotMode.waitForPinballTag && (!shotInfoInTags || multiShotInit))
		{
			currentMode = MultiShotMode.attacking;
		}
		if (currentMode == MultiShotMode.attacking)
		{
			UpdateNetTargetPoints(false);
			if (modeTimer < nextTargetAt)
			{
				UpdatePinballPosition();
			}
			else if (netCurrentTarget == pinballStartPoint)
			{
				currentMode = MultiShotMode.showing;
				hidePinball();
			}
			else
			{
				currentMode = MultiShotMode.turning;
				modeTimer = 0f;
			}
		}
		if (currentMode == MultiShotMode.turning)
		{
			UpdateTurning();
		}
		if (currentMode == MultiShotMode.showing)
		{
			ShowCharacter();
			currentMode = MultiShotMode.finishing;
		}
		if (currentMode == MultiShotMode.finishing)
		{
			Finish();
			currentMode = MultiShotMode.waiting;
		}
		UpdateRotation();
	}

	protected void hidePinball()
	{
		if (pinball != null)
		{
			if (hidden)
			{
				owningObject.transform.rotation = pinball.transform.rotation;
				charGlobals.motionController.setNewFacing(owningObject.transform.forward);
			}
			Object.Destroy(pinball);
		}
	}

	public override void behaviorUpdate()
	{
		if (!firstUpdate)
		{
			base.behaviorUpdate();
			return;
		}
		base.behaviorUpdate();
		modeTimer += Time.deltaTime;
		if (hostMode)
		{
			hostUpdate();
		}
		else
		{
			clientUpdate();
		}
	}

	protected void OnEnemyDespawned(EntityDespawnMessage e)
	{
		CombatController component = e.go.GetComponent<CombatController>();
		if (targets.Contains(component))
		{
			int num = targets.IndexOf(component);
			if (num < targetIndex)
			{
				targets[num] = null;
			}
			else
			{
				targets.RemoveAt(num);
			}
		}
	}

	protected bool IsPinballTarget(CombatController target)
	{
		float num = attackData.maximumRange * attackData.maximumRange;
		float sqrMagnitude = (target.transform.position - owningObject.transform.position).sqrMagnitude;
		return sqrMagnitude <= num && target.faction == oppositeFaction;
	}

	protected void PlayPinballEffect()
	{
		if (!(pinball != null) || attackData.effectName == null)
		{
			return;
		}
		string name = attackData.effectName + "_pinball";
		GameObject gameObject = null;
		gameObject = (charGlobals.combatController.effectSequenceSource.GetEffectSequencePrefabByName(name) as GameObject);
		if (gameObject != null)
		{
			if (pinballEffect != null)
			{
				Object.Destroy(pinballEffect);
			}
			pinballEffect = (Object.Instantiate(gameObject) as GameObject);
			pinballEffect.SendMessage("AssignCreator", charGlobals, SendMessageOptions.DontRequireReceiver);
			pinballEffect.name = "AttackEffect_" + gameObject.name;
			Utils.AttachGameObject(pinball, pinballEffect);
			EffectSequence component = pinballEffect.GetComponent<EffectSequence>();
			if (component != null)
			{
				component.ScaleEffectsToOwner = true;
			}
		}
	}

	protected void Finish()
	{
		if (animationComponent[baseAnim + "_end"] != null)
		{
			animName = baseAnim + "_end";
			animationComponent.Play(animName);
			durationEnded = true;
			startAnim = false;
			if (attackData.forwardSpeed != 0f)
			{
				charGlobals.motionController.setForcedVelocityDuration(0f);
			}
			if (attackData.effectName != null)
			{
				GameObject gameObject = null;
				gameObject = (charGlobals.combatController.effectSequenceSource.GetEffectSequencePrefabByName(attackData.effectName + "_end") as GameObject);
				if (gameObject != null)
				{
					GameObject gameObject2 = Object.Instantiate(gameObject) as GameObject;
					gameObject2.SendMessage("AssignCreator", charGlobals, SendMessageOptions.DontRequireReceiver);
					gameObject2.name = "AttackEffect_" + gameObject.name;
					Utils.AttachGameObject(charGlobals.gameObject, gameObject2);
				}
			}
		}
		else
		{
			charGlobals.behaviorManager.endBehavior();
		}
	}
}
