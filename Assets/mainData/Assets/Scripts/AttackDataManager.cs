using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class AttackDataManager : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public delegate void AttackDataLoadedCallback();

	protected Dictionary<string, CombatController.AttackData> attackDataDictionary;

	protected Dictionary<string, Queue<AttackDataLoadedCallback>> attackDataRequests;

	protected ImpactMatrix impactMatrix;

	protected XmlSerializer serializer;

	protected static AttackDataManager instance;

	public static AttackDataManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		serializer = new XmlSerializer(typeof(AttackDataRaw));
		attackDataRequests = new Dictionary<string, Queue<AttackDataLoadedCallback>>();
		attackDataDictionary = new Dictionary<string, CombatController.AttackData>();
		impactMatrix = new ImpactMatrix();
		loadAttackData("common_attacks", OnCommonDataLoaded);
		impactMatrix.Load("impact_matrix", null);
	}

	protected void OnCommonDataLoaded()
	{
	}

	protected void OnAttackDataLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		DataWarehouse data = response.Data.GetData("attack_data");
		processAttackData(data, extraData as string);
	}

	protected void processAttackData(DataWarehouse attackData, string identifier)
	{
		AttackDataRaw attackDataRaw = AttackDataRaw.FromXML(attackData.Navigator.OuterXml, serializer);
		if (attackDataRaw.attacks != null)
		{
			for (int i = 0; i < attackDataRaw.attacks.Length; i++)
			{
				AttackRaw attackRaw = attackDataRaw.attacks[i];
				if (attackDataDictionary.ContainsKey(attackRaw.attack_name))
				{
					CspUtils.DebugLog("Duplicate attack name found: " + attackRaw.attack_name + "   All characters must have unique attack names.");
					continue;
				}
				CombatController.AttackData attackData2 = new CombatController.AttackData();
				attackData2.attackName = attackRaw.attack_name;
				attackData2.behaviorName = attackRaw.behavior;
				attackData2.animName = attackRaw.anim;
				attackData2.effectName = attackRaw.effect;
				attackData2.triggeredEffectName = attackRaw.triggered_effect;
				attackData2.stopEffectOnChain = attackRaw.stop_effect_on_chain;
				attackData2.desiredRange = attackRaw.desired_range;
				attackData2.maximumRange = attackRaw.maximum_range;
				attackData2.moveSpeed = attackRaw.move_speed;
				attackData2.moveStartTime = attackRaw.move_start_time;
				attackData2.moveArriveTime = attackRaw.move_arrive_time;
				attackData2.collisionOffTime = attackRaw.collision_off_time;
				attackData2.collisionOnTime = attackRaw.collision_on_time;
				attackData2.transitionStartTime = attackRaw.chain_start_time;
				attackData2.transitionEndTime = attackRaw.chain_end_time;
				attackData2.transitionRequiresImpact = attackRaw.chain_requires_impact;
				attackData2.interruptibleBy = (CombatController.AttackData.RecoilType)attackRaw.interruptible_by;
				attackData2.faceTarget = attackRaw.face_target;
				attackData2.trackTarget = attackRaw.track_target;
				attackData2.allowLateralTranslation = attackRaw.allow_lateral_translation;
				attackData2.attackDuration = attackRaw.attack_duration;
				attackData2.forwardSpeed = attackRaw.forward_speed;
				attackData2.forwardOnHit = attackRaw.forward_on_hit;
				attackData2.powerCost = attackRaw.power_cost;
				attackData2.targetEffectName = attackRaw.target_effect;
				attackData2.environmentalAttack = attackRaw.environmental_attack;
				attackData2.targetlessAttack = attackRaw.targetless_attack;
				attackData2.ignoreHeightDifference = attackRaw.ignore_height_difference;
				attackData2.autoChainAttackName = attackRaw.auto_chain_attack_name;
				attackData2.stopOnEnemyCollision = attackRaw.stop_on_enemy_collision;
				attackData2.hideOnTeleport = attackRaw.hide_on_teleport;
				attackData2.totalTargets = attackRaw.total_targets;
				attackData2.attackDelay = attackRaw.attack_delay;
				attackData2.minimumDelay = attackRaw.minimum_delay;
				attackData2.pinballPause = attackRaw.pinball_pause;
				attackData2.teleportEffect = attackRaw.teleport_effect;
				attackData2.pinballPrefab = attackRaw.pinball_prefab;
				attackData2.linearPinball = attackRaw.linear_pinball;
				attackData2.hasShotInfoTag = attackRaw.has_shot_info_tag;
				attackData2.arcMultiplier = attackRaw.arc_multiplier;
				attackData2.pinballStartNode = attackRaw.pinball_start_node;
				attackData2.pinballTurnDuration = attackRaw.pinball_turn_duration;
				attackData2.pinballCanBeHit = !attackRaw.pinball_cannot_be_hit;
				attackData2.alwaysRenderMaxBeam = attackRaw.always_render_max_beam;
				attackData2.beamHitGeometryEffect = attackRaw.beam_hit_geometry_effect;
				attackData2.beamHitNothingEffect = attackRaw.beam_hit_nothing_effect;
				if (attackRaw.summons != null)
				{
					for (int j = 0; j < attackRaw.summons.Length; j++)
					{
						if (attackRaw.summons[j].summonList != null)
						{
							for (int k = 0; k < attackRaw.summons[j].summonList.Length; k++)
							{
								attackData2.summons.Add(new CombatController.SummonData(attackRaw.summons[j].summonList[k]));
							}
						}
					}
				}
				attackData2.chance = attackRaw.chance;
				int num = 0;
				if (attackRaw.attack_child_names != null)
				{
					num = attackRaw.attack_child_names.Length;
				}
				attackData2.attackChildNames = new string[num];
				for (int l = 0; l < num; l++)
				{
					attackData2.attackChildNames[l] = attackRaw.attack_child_names[l];
				}
				int num2 = 0;
				if (attackRaw.impacts != null)
				{
					num2 = attackRaw.impacts.Length;
				}
				attackData2.impacts = new CombatController.ImpactData[num2];
				for (int m = 0; m < num2; m++)
				{
					ImpactRaw impactRaw = attackRaw.impacts[m];
					CombatController.ImpactData impactData = new CombatController.ImpactData();
					impactData.impactType = (CombatController.ImpactData.ImpactType)impactRaw.impact_type;
					impactData.effectName = impactRaw.effect;
					impactData.impactStartTime = impactRaw.impact_start_time;
					impactData.impactEndTime = impactRaw.impact_end_time;
					impactData.firingTime = impactRaw.firing_time;
					impactData.projectileName = impactRaw.projectile_name;
					impactData.projectileIsEnvironmental = impactRaw.projectile_environmental;
					impactData.projectileCreateImmediate = impactRaw.projectile_create_immediate;
					impactData.projectileLifespan = impactRaw.projectile_lifespan;
					impactData.projectileSpeed = impactRaw.projectile_speed;
					impactData.projectileAimed = impactRaw.projectile_aimed;
					impactData.projectileAttached = impactRaw.projectile_attached;
					impactData.projectileRotateToVelocity = impactRaw.projectile_rotate_to_velocity;
					impactData.projectileExplosionRadius = impactRaw.projectile_explosion_radius;
					impactData.projectileGravity = impactRaw.projectile_gravity;
					impactData.projectileDestroyOnImpact = impactRaw.projectile_destroy_on_impact;
					impactData.projectileDestroyOnCollision = impactRaw.projectile_destroy_on_collision;
					impactData.projectileImpactStickTime = impactRaw.projectile_impact_stick_time;
					impactData.projectileCollisionStickTime = impactRaw.projectile_collision_stick_time;
					impactData.projectileTargetReticle = impactRaw.projectile_target_reticle;
					impactData.projectileAdditionalTargets = impactRaw.projectile_additional_targets;
					impactData.projectileReturnArc = impactRaw.projectile_return_arc;
					impactData.projectileBallistic = impactRaw.projectile_ballistic;
					impactData.projectileBallisticLob = impactRaw.projectile_ballistic_lob;
					impactData.projectileScaledToOwner = impactRaw.projectile_scaled_to_owner;
					impactData.targetAimOffset = impactRaw.target_aim_offset;
					impactData.showCollider = impactRaw.show_collider;
					impactData.colliderScale = new ModifierData(impactRaw.collider_scale, 0f, impactRaw.collider_scale_stat);
					impactData.colliderAngleLimit = impactRaw.collider_angle_limit;
					impactData.colliderOffset = impactRaw.collider_offset;
					impactData.nextImpactDelay = impactRaw.next_impact_delay;
					if (impactRaw.attacker_combat_effect != null)
					{
						for (int n = 0; n < impactRaw.attacker_combat_effect.Length; n++)
						{
							impactData.attackerCombatEffects.Add(impactRaw.attacker_combat_effect[n]);
						}
					}
					impactData.attackerRemoveEffect = impactRaw.attacker_remove_effect;
					impactData.requiredCombatEffect = impactRaw.required_combat_effect;
					impactData.eventName = impactRaw.event_name;
					impactData.eventObjectName = impactRaw.event_object_name;
					impactData.pickupName = impactRaw.pickup_name;
					impactData.hitsFriends = impactRaw.hits_friends;
					impactData.hitsEnemies = impactRaw.hits_enemies;
					impactData.maxHitsPerTarget = impactRaw.maximum_hits_per_target;
					impactData.maximumTargetsHit = impactRaw.maximum_targets_hit;
					impactData.minimumDistanceSqr = Mathf.Pow(impactRaw.minimum_distance, 2f);
					impactData.index = m;
					CombatController.ImpactResultData impactResultData = new CombatController.ImpactResultData();
					impactResultData.damageData = new ModifierData(impactRaw.damage, impactRaw.damage_sf, impactRaw.damage_ss);
					impactResultData.pushbackDuration = new ModifierData(impactRaw.pushback_duration, impactRaw.pushback_duration_sf, impactRaw.pushback_duration_ss);
					impactResultData.pushbackVelocity = new ModifierData(impactRaw.pushback_velocity, impactRaw.pushback_velocity_sf, impactRaw.pushback_velocity_ss);
					impactResultData.pushbackFromCollider = impactRaw.pushback_from_collider;
					impactResultData.knockdownDuration = new ModifierData(impactRaw.recoil_duration, impactRaw.recoil_duration_sf, impactRaw.recoil_duration_ss);
					impactResultData.impactEffectName = impactRaw.impact_effect;
					impactResultData.impactFaceCamera = impactRaw.impact_face_camera;
					impactResultData.impactMatrixType = (ImpactMatrix.Type)impactRaw.impact_matrix_type;
					impactResultData.launchVelocity = new ModifierData(impactRaw.launch_velocity, impactRaw.launch_velocity_sf, impactRaw.launch_velocity_ss);
					impactResultData.targetCombatEffect = impactRaw.target_combat_effect;
					impactResultData.targetRemoveCombatEffect = impactRaw.target_remove_combat_effect;
					impactResultData.rotateTargetToImpact = (impactRaw.rotate_target_to_impact = true);
					impactResultData.forceAttach = impactRaw.force_attach;
					impactResultData.colliderName = impactRaw.collider;
					impactResultData.recoil = (CombatController.AttackData.RecoilType)impactRaw.recoil;
					impactResultData.useRecoilDurationOnAttach = impactRaw.use_recoil_duration_on_attach;
					impactResultData.attachPrefabName = impactRaw.attach_prefab_name;
					impactResultData.attachAnimName = impactRaw.attach_anim_name;
					impactResultData.attachUsesRotation = impactRaw.attach_uses_rotation;
					impactResultData.stunAnimSpeed = impactRaw.stun_anim_speed;
					impactData.impactResult = impactResultData;
					attackData2.impacts[m] = impactData;
					if (impactRaw.summons == null)
					{
						continue;
					}
					for (int num3 = 0; num3 < impactRaw.summons.Length; num3++)
					{
						if (impactRaw.summons[num3].summonList != null)
						{
							for (int num4 = 0; num4 < impactRaw.summons[num3].summonList.Length; num4++)
							{
								impactResultData.summons.Add(new CombatController.SummonData(impactRaw.summons[num3].summonList[num4]));
							}
						}
					}
				}
				attackDataDictionary.Add(attackData2.attackName, attackData2);
			}
		}
		foreach (AttackDataLoadedCallback item in attackDataRequests[identifier])
		{
			item();
		}
		attackDataRequests[identifier].Clear();
	}

	public void loadAttackData(string filename, AttackDataLoadedCallback callback)
	{
		if (!attackDataRequests.ContainsKey(filename))
		{
			attackDataRequests[filename] = new Queue<AttackDataLoadedCallback>();
			attackDataRequests[filename].Enqueue(callback);
			AppShell.Instance.DataManager.LoadGameData("AttackData/" + filename, OnAttackDataLoaded, filename);
		}
		else if (attackDataRequests[filename].Count > 0)
		{
			attackDataRequests[filename].Enqueue(callback);
		}
		else
		{
			callback();
		}
	}

	public void loadAttackData(string identifier, DataWarehouse attackData, AttackDataLoadedCallback callback)
	{
		if (!attackDataRequests.ContainsKey(identifier))
		{
			attackDataRequests[identifier] = new Queue<AttackDataLoadedCallback>();
			attackDataRequests[identifier].Enqueue(callback);
			processAttackData(attackData, identifier);
		}
		else if (attackDataRequests[identifier].Count > 0)
		{
			attackDataRequests[identifier].Enqueue(callback);
		}
		else
		{
			callback();
		}
	}

	public CombatController.AttackData getAttackData(string attackName)
	{
		CombatController.AttackData value;
		attackDataDictionary.TryGetValue(attackName, out value);
		return value;
	}

	public string getImpactMatrixEffect(ImpactMatrix.Type attackType, ImpactMatrix.Type receiveType)
	{
		return impactMatrix.GetEffect(attackType, receiveType);
	}

	public void ClearDataCache()
	{
		CspUtils.DebugLog("You are clearing the attack data cache!  This should only be used in editor-only circumstances where changes have been made to existing assets");
		attackDataDictionary.Clear();
		attackDataRequests.Clear();
	}
}
