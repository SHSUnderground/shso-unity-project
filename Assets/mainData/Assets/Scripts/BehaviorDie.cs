using System;
using UnityEngine;

public class BehaviorDie : BehaviorBase
{
	protected const float respawnHeight = 6f;

	protected const float respawnKnockdownRange = 7f;

	protected float duration = 5f;

	protected float despawnTime;

	protected float respawnTime;

	protected float reactivateTime;

	protected bool landed;

	protected Vector3 lookTarget;

	protected LodBase lodRenderer;

	protected SkinnedMeshRenderer renderer;

	protected bool rendererEnabled = true;

	protected bool blinkEnabled;

	protected float blinkDuration = 0.05f;

	protected float blinkFrequencyOffset = 0.75f;

	protected float blinkFrequencyStart = 0.3f;

	protected float blinkFrequencyEnd = 0.05f;

	protected float nextBlinkToggle;

	protected float blinkTotalTime;

	protected float blinkStartTime;

	protected string koStars = "death_star_sequence";

	protected bool canBehaviorEnd;

	public bool survivalMode;

	public bool deathAnimOverride;

	private bool survivalInstructionsDisplayed;

	private bool survivalEndLevelEventFired;

	private int cameraPlayerIndex;

	public virtual void Initialize(GameObject attacker, float newDuration)
	{
		duration = newDuration;
		combatController.resetAttackChain();
		if (!combatController.IsPlayer())
		{
			owningObject.layer = 2;
			AppShell.Instance.ServerConnection.Game.ForEachNetEntity(delegate(NetGameManager.NetEntity e)
			{
				if (e.netComp.gameObject.active && owningObject.collider != e.netComp.gameObject.collider)
				{
					Physics.IgnoreCollision(owningObject.collider, e.netComp.gameObject.collider);
				}
			});
		}
		else if (BrawlerStatManager.Active)
		{
			BrawlerStatManager.instance.ReportEmotionalEvent(combatController.gameObject, 0);
		}
		string text = getAnimationName("recoil_death");
		if (charGlobals.motionController.getVerticalVelocity() > 0f)
		{
			landed = false;
			if (animationComponent["recoil_launch"] != null)
			{
				text = "recoil_launch";
			}
		}
		else
		{
			if (animationComponent[text + "_2"] != null)
			{
				text += "_2";
			}
			motionLanded();
		}
		if (charGlobals.spawnData != null && charGlobals.spawnData.spawner != null)
		{
			string overrideDeathAnim = charGlobals.spawnData.spawner.overrideDeathAnim;
			if (!string.IsNullOrEmpty(overrideDeathAnim) && animationComponent[overrideDeathAnim] != null)
			{
				text = overrideDeathAnim;
				deathAnimOverride = true;
			}
		}
		EffectSequence effectSequence = null;
		if (!deathAnimOverride)
		{
			GameObject gameObject = combatController.createEffect(koStars, null);
			OffsetTracker component = gameObject.GetComponent<OffsetTracker>();
			Transform transform = Utils.FindNodeInChildren(owningObject.transform, "Head");
			if (transform != null)
			{
				component.toTrack = transform.gameObject;
			}
			if (component.toTrack == null)
			{
				component.toTrack = owningObject;
			}
			effectSequence = gameObject.GetComponent<EffectSequence>();
		}
		if (animationComponent[text] == null)
		{
			despawnTime = Time.time + duration;
			if (!deathAnimOverride)
			{
				effectSequence.TotalLifetime = duration;
			}
		}
		else
		{
			despawnTime = Time.time + duration + animationComponent[text].length;
			if (!deathAnimOverride)
			{
				effectSequence.TotalLifetime = duration + animationComponent[text].length;
			}
			charGlobals.GetComponent<FacialAnimation>().PersistOnAnimEnd = true;
			animationComponent.Play(text);
			animationComponent[text].wrapMode = WrapMode.ClampForever;
		}
		lookTarget = attacker.transform.position - owningObject.transform.position;
		int childCount = owningObject.transform.GetChildCount();
		for (int i = 0; i < childCount; i++)
		{
			Transform child = owningObject.transform.GetChild(i);
			if (child.name.Contains("AttackEffect"))
			{
				UnityEngine.Object.Destroy(child.gameObject);
			}
		}
		PlayerCombatController playerCombatController = combatController as PlayerCombatController;
		if (playerCombatController != null && CardGameController.Instance == null)
		{
			charGlobals.motionController.LockTargetCamera(false, true, false, false, 10f);
			playerCombatController.setPower(0f);
			combatController.faction = CombatController.Faction.Neutral;
		}
		else if (owningObject == GUIManager.Instance.GetTargetedEnemy())
		{
			GUIManager.Instance.DetachAttackingIndicator();
		}
		if (CardGameController.Instance != null)
		{
			blinkEnabled = false;
		}
		else if (charGlobals.animationComponent[text] != null && (charGlobals.spawnData == null || charGlobals.spawnData.spawner == null || charGlobals.spawnData.spawner.blinkBeforeDespawn))
		{
			blinkEnabled = true;
			lodRenderer = (owningObject.GetComponentInChildren(typeof(LodBase)) as LodBase);
			if (lodRenderer == null)
			{
				renderer = (owningObject.GetComponentInChildren(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer);
			}
			blinkStartTime = charGlobals.animationComponent[text].length * blinkFrequencyOffset;
			blinkTotalTime = charGlobals.animationComponent[text].length * (1f - blinkFrequencyOffset) + duration;
			nextBlinkToggle = 0f;
		}
		else
		{
			blinkEnabled = false;
		}
		if (!deathAnimOverride)
		{
			VOManager.Instance.PlayVO("damage_death", owningObject);
		}
		respawnTime = 0f;
		reactivateTime = 0f;
	}

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		GUIManager.Instance.DetachHealthBarIndicator();
	}

	public override void behaviorUpdate()
	{
		if (blinkEnabled && (lodRenderer != null || renderer != null) && elapsedTime > blinkStartTime && respawnTime == 0f && elapsedTime > nextBlinkToggle)
		{
			rendererEnabled = !rendererEnabled;
			toggleRenderer(rendererEnabled);
			if (rendererEnabled)
			{
				float t = (elapsedTime - blinkStartTime) / blinkTotalTime;
				float num = Mathf.Lerp(blinkFrequencyStart, blinkFrequencyEnd, t);
				nextBlinkToggle = elapsedTime + num;
			}
			else
			{
				nextBlinkToggle = elapsedTime + blinkDuration;
			}
		}
		if (!landed)
		{
			charGlobals.motionController.setForcedVelocityDuration(0.2f);
			charGlobals.motionController.rotateTowards(lookTarget);
			if ((double)elapsedTime > 0.5 && charGlobals.motionController.IsOnGround())
			{
				motionLanded();
			}
		}
		else if (duration >= 0f && landed && Time.time >= despawnTime)
		{
			if (BrawlerController.Instance == null || !BrawlerController.Instance.IsCutScenePlaying)
			{
				duration = -1f;
				if (CardGameController.Instance != null)
				{
					respawnTime = Time.time + 1.5f;
					return;
				}
				combatController.playDespawnEffect();
				if (combatController.IsPlayer())
				{
					respawnTime = Time.time + 1.5f;
					toggleRenderer(false);
					return;
				}
				AppShell.Instance.EventMgr.Fire(this, new CombatCharacterDespawnedMessage(owningObject, combatController));
				if (networkComponent != null && networkComponent.IsOwner())
				{
					combatController.dropPickup();
				}
				if (charGlobals.spawnData != null)
				{
					charGlobals.spawnData.Despawn(EntityDespawnMessage.despawnType.defeated, false);
				}
				return;
			}
		}
		else if (reactivateTime == 0f && respawnTime > 0f && Time.time >= respawnTime)
		{
			if (survivalMode)
			{
				if (PlayerCombatController.GetPlayerCount() == PlayerCombatController.GetPlayerKilledCount() && !survivalEndLevelEventFired)
				{
					if (PlayerCombatController.GetPlayerCount() > 1)
					{
						BrawlerController.Instance.HandleStageEnd(2);
					}
					else
					{
						BrawlerController.Instance.HandleStageEnd();
					}
					survivalEndLevelEventFired = true;
				}
				else if (!survivalInstructionsDisplayed && charGlobals.gameObject == GameController.GetController().LocalPlayer && !survivalEndLevelEventFired)
				{
					swapCamera();
					BrawlerController instance = BrawlerController.Instance;
					if (instance != null)
					{
						SHSOkDialogWindow sHSOkDialogWindow = new SHSOkDialogWindow();
						sHSOkDialogWindow.TitleText = "#SURVIVAL_DEFEATED_CAMERA_INSTRUCTIONS";
						GUIManager.Instance.ShowDynamicWindow(sHSOkDialogWindow, GUIControl.ModalLevelEnum.Full);
					}
					survivalInstructionsDisplayed = true;
				}
				return;
			}
			if (CardGameController.Instance != null)
			{
				combatController.revive();
				BehaviorRecoilGetup behaviorRecoilGetup = charGlobals.behaviorManager.forceChangeBehavior(typeof(BehaviorRecoilGetup)) as BehaviorRecoilGetup;
				behaviorRecoilGetup.Initialize(owningObject, owningObject.transform.position, null);
				return;
			}
			charGlobals.motionController.LockTargetCamera(false, true, false, true, 10f);
			FacialAnimation component = charGlobals.GetComponent<FacialAnimation>();
			component.PersistOnAnimEnd = false;
			component.SetFacialExpression(FacialAnimation.Expression.Normal);
			reactivateTime = Time.time + 0.8f;
			combatController.restoreHealth();
			charGlobals.motionController.teleportTo(charGlobals.motionController.transform.position + Vector3.up * 6f);
			charGlobals.motionController.setVerticalVelocity(0f);
			toggleRenderer(true);
			EffectSequence logicalEffectSequence = charGlobals.effectsList.GetLogicalEffectSequence("Transport");
			if (logicalEffectSequence != null)
			{
				logicalEffectSequence.Initialize(owningObject, null, null);
				logicalEffectSequence.StartSequence();
			}
			charGlobals.animationComponent.Play("jump_fall");
			combatController.createCombatEffect("RespawnInvulernable", combatController, true);
			combatController.faction = CombatController.Faction.Player;
		}
		else if (reactivateTime > 0f && Time.time >= reactivateTime)
		{
			charGlobals.behaviorManager.endBehavior();
			knockdownEnemies();
		}
		base.behaviorUpdate();
	}

	public override void motionLanded()
	{
		landed = true;
		if (animationComponent.IsPlaying("recoil_launch") && animationComponent["recoil_launch_land"] != null)
		{
			animationComponent.CrossFade("recoil_launch_land", 0.1f);
			despawnTime += animationComponent["recoil_launch_land"].length;
		}
		combatController.playLaunchLandEffect();
	}

	public override void behaviorEnd()
	{
		toggleRenderer(true);
		base.behaviorEnd();
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}

	public override bool allowUserInput()
	{
		return false;
	}

	private void swapCamera()
	{
		PlayerCombatController[] array = Utils.FindObjectsOfType<PlayerCombatController>();
		cameraPlayerIndex = (cameraPlayerIndex + 1) % array.Length;
		int num = cameraPlayerIndex;
		PlayerCombatController playerCombatController = array[cameraPlayerIndex];
		while (playerCombatController.isKilled)
		{
			cameraPlayerIndex = (cameraPlayerIndex + 1) % array.Length;
			playerCombatController = array[cameraPlayerIndex];
			if (num == cameraPlayerIndex)
			{
				return;
			}
		}
		CharacterSpawn.ConnectCameras(playerCombatController.gameObject);
	}

	public override void userInputOverride()
	{
		if (survivalMode && SHSInput.GetButtonDown("Jump"))
		{
			swapCamera();
		}
	}

	public override bool allowForcedInterrupt(Type newBehaviorType)
	{
		if (newBehaviorType == typeof(BehaviorVictory))
		{
			return true;
		}
		if (combatController.getHealth() <= 0f || combatController.isKilled)
		{
			return false;
		}
		return base.allowForcedInterrupt(newBehaviorType);
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override void behaviorLateUpdate()
	{
		base.behaviorLateUpdate();
		if (elapsedTime > 0.3f)
		{
			charGlobals.motionController.performRootMotion();
		}
	}

	protected void toggleRenderer(bool visible)
	{
		if (lodRenderer != null)
		{
			lodRenderer.SetVisible(visible);
		}
		if (renderer != null)
		{
			renderer.enabled = visible;
		}
	}

	protected void knockdownEnemies()
	{
		if (charGlobals != null && !charGlobals.networkComponent.IsOwner())
		{
			return;
		}
		CombatController.AttackData attackData = AttackDataManager.Instance.getAttackData("PlayerRespawnAttack");
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(CharacterGlobals));
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			CharacterGlobals characterGlobals = (CharacterGlobals)array2[i];
			if (characterGlobals != null && Vector3.Distance(characterGlobals.transform.position, owningObject.transform.position) < 7f && !(characterGlobals.behaviorManager.getBehavior() is BehaviorRecoil))
			{
				SpawnData spawnData = characterGlobals.spawnData;
				if (spawnData != null && characterGlobals.combatController != null && characterGlobals.combatController.faction == CombatController.Faction.Enemy && ((spawnData.spawnType & CharacterSpawn.Type.AI) != 0 || (spawnData.spawnType & CharacterSpawn.Type.Boss) != 0))
				{
					combatController.attackHit(charGlobals.transform.position, characterGlobals.combatController, attackData, attackData.impacts[0]);
				}
			}
		}
	}
}
