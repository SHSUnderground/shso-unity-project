using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatEffectDataWarehouse
{
	protected static CombatEffectDataWarehouse instance;

	protected Dictionary<string, CombatEffectData> combatEffectDataDictionary;

	protected Dictionary<string, CombatEffectResister> combatEffectResisterDictionary;

	public static CombatEffectDataWarehouse Instance
	{
		get
		{
			return instance;
		}
	}

	public CombatEffectDataWarehouse()
	{
		if (instance != null)
		{
			CspUtils.DebugLog("A second CombatEffectDataWarehouse is being created.  This may lead to instabilities!");
			return;
		}
		instance = this;
		AppShell.Instance.DataManager.LoadGameData("Brawler/combat_effect_data", OnCombatEffectDataLoaded);
	}

	public void OnCombatEffectDataLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		DataWarehouse data = response.Data;
		combatEffectDataDictionary = new Dictionary<string, CombatEffectData>();
		foreach (DataWarehouse item in data.GetIterator("//combat_effect"))
		{
			CombatEffectData combatEffectData = new CombatEffectData();
			combatEffectData.combatEffectName = item.GetString("combat_effect_name");
			combatEffectData.minimumDuration = new ModifierData(item, "minimum_duration", 0f);
			combatEffectData.maximumDuration = new ModifierData(item, "maximum_duration", 0f);
			combatEffectData.animationDuration = item.TryGetString("animation_duration", string.Empty);
			combatEffectData.effectPrefabName = item.TryGetString("effect_prefab_name", null);
			combatEffectData.effectRemovePrefabName = item.TryGetString("effect_remove_prefab_name", null);
			combatEffectData.icon = item.TryGetString("icon", string.Empty);
			combatEffectData.toolTip = item.TryGetString("tool_tip", string.Empty);
			combatEffectData.resisterName = item.TryGetString("resister_name", null);
			combatEffectData.alertTexture = item.TryGetString("alert_texture", string.Empty);
			combatEffectData.reapplyEffect = item.TryGetBool("reapply", true);
			combatEffectData.healthModifier = item.TryGetFloat("health_modifier", 0f);
			combatEffectData.healthRegenModifier = new ModifierData(item, "health_regen_modifier", 0f);
			combatEffectData.powerRegenModifier = new ModifierData(item, "power_regen_modifier", 0f);
			combatEffectData.damageMultiplier = item.TryGetFloat("damage_multiplier", 1f);
			combatEffectData.incomingDamageMultiplier = item.TryGetFloat("incoming_damage_multiplier", 1f);
			combatEffectData.speedMultiplier = item.TryGetFloat("speed_multiplier", 1f);
			combatEffectData.maximumRecoilResisted = item.TryGetInt("recoil_immunity", 0);
			combatEffectData.maximumRecoilAllowed = item.TryGetInt("recoil_limit", 0);
			combatEffectData.minimumAttackRecoil = item.TryGetInt("recoil_enhancement", 0);
			combatEffectData.maximumAttackRecoil = item.TryGetInt("recoil_nerf", 10);
			combatEffectData.recoilInterruptModifier = item.TryGetInt("recoil_interrupt_modifier", 0);
			combatEffectData.pushbackResistanceModifier = item.TryGetFloat("pushback_resistance_modifier", 0f);
			combatEffectData.launchResistanceModifier = item.TryGetFloat("launch_resistance_modifier", 0f);
			combatEffectData.powerDamageModifier = item.TryGetFloat("power_damage_modifier", 0f);
			combatEffectData.powerReceivedModifier = item.TryGetFloat("power_received_modifier", 0f);
			combatEffectData.bonusTargetCombatEffect = item.TryGetString("bonus_target_combat_effect", null);
			combatEffectData.bonusIncommingEffect = item.TryGetString("bonus_incomming_effect", null);
			combatEffectData.bonusImpactEffect = item.TryGetString("bonus_impact_effect", null);
			combatEffectData.stealthMode = item.TryGetBool("stealth_mode", false);
			combatEffectData.maxAttackPushback = item.TryGetFloat("max_attack_pushback", -1f);
			combatEffectData.maxAttackLaunch = item.TryGetFloat("max_attack_launch", -1f);
			combatEffectData.maxPushbackDuration = item.TryGetFloat("max_pushback_duration", -1f);
			combatEffectData.powerBarTimerDuration = item.TryGetFloat("power_bar_timer", 0f);
			combatEffectData.addComponent = item.TryGetString("add_component", null);
			CombatEffectMessage.BuildCombatEffectMessage(item.TryGetString("start_message", string.Empty), out combatEffectData.startMessage, out combatEffectData.startMessageArgs);
			CombatEffectMessage.BuildCombatEffectMessage(item.TryGetString("end_message", string.Empty), out combatEffectData.endMessage, out combatEffectData.endMessageArgs);
			combatEffectData.startBehavior = item.TryGetString("start_behavior", string.Empty);
			combatEffectData.endBehavior = item.TryGetString("end_behavior", string.Empty);
			combatEffectData.getupAttack = item.TryGetString("getup_attack", string.Empty);
			combatEffectData.replacementCombatEffect = item.TryGetString("replacement_combat_effect", string.Empty);
			combatEffectData.ablativeHealth = item.TryGetFloat("ablative_health", 0f);
			combatEffectData.ablativeEndBehavior = item.TryGetString("ablative_end_behavior", string.Empty);
			combatEffectData.ablativeReplacementCombatEffect = item.TryGetString("ablative_replacement_combat_effect", string.Empty);
			combatEffectData.attachedProjectile = item.TryGetString("attached_projectile", string.Empty);
			combatEffectData.attachedProjectileCount = item.TryGetInt("attached_projectile_count", 1);
			combatEffectData.dotPeriod = item.TryGetFloat("dot_period", 0f);
			combatEffectData.dotAttack = item.TryGetString("dot_attack", string.Empty);
			combatEffectData.scale = item.TryGetVector("scale", Vector3.one);
			combatEffectData.scaleOffset = item.TryGetVector("scale_offset", Vector3.zero);
			combatEffectData.scaleDuration = item.TryGetFloat("scale_duration", 0f);
			combatEffectData.scaleRampDuration = item.TryGetFloat("scale_ramp_duration", 0f);
			combatEffectData.scaleBone = item.TryGetString("scale_bone", string.Empty);
			combatEffectData.appliedVelocity = item.TryGetVector("velocity", Vector3.zero);
			combatEffectData.gravityWellObject = item.TryGetString("gravitySource", string.Empty);
			combatEffectData.gravityWellOffset = item.TryGetVector("gravityOffset", Vector3.zero);
			combatEffectData.gravityNearStrength = item.TryGetFloat("gravityNearStrength", 0f);
			combatEffectData.gravityFarStrength = item.TryGetFloat("gravityFarStrength", 0f);
			combatEffectData.gravityNearDistance = item.TryGetFloat("gravityNearDistance", 0f);
			combatEffectData.gravityFarDistance = item.TryGetFloat("gravityFarDistance", 0f);
			if ((combatEffectData.gravityNearStrength != 0f || combatEffectData.gravityFarStrength != 0f) && combatEffectData.gravityFarDistance != combatEffectData.gravityNearDistance)
			{
				combatEffectData.usesGravityWell = true;
			}
			combatEffectData.speedStartMultiplier = item.TryGetFloat("speed_start_multiplier", 1f);
			combatEffectData.speedMidMultiplier = item.TryGetFloat("speed_mid_multiplier", 1f);
			combatEffectData.speedEndMultiplier = item.TryGetFloat("speed_end_multiplier", 1f);
			combatEffectData.speedMultiplierTime = item.TryGetFloat("speed_multiplier_time", 0f);
			combatEffectData.speedMultiplierMidTime = item.TryGetFloat("speed_multiplier_mid_time", 0f);
			combatEffectData.startScenarioEvent = item.TryGetString("start_scenario_event", string.Empty);
			combatEffectData.endScenarioEvent = item.TryGetString("end_scenario_event", string.Empty);
			combatEffectData.prioritizedAttackName = item.TryGetString("prioritized_attack", string.Empty);
			combatEffectData.disableInput = item.TryGetBool("disable_input", false);
			string text = item.TryGetString("attacks_repeatable", string.Empty);
			if (text == string.Empty)
			{
				combatEffectData.attacksRepeatable = null;
			}
			else
			{
				combatEffectData.attacksRepeatable = Convert.ToBoolean(text);
			}
			combatEffectData.clickboxHeight = item.TryGetFloat("clickbox_height", 0f);
			combatEffectData.clickboxRadius = item.TryGetFloat("clickbox_radius", 0f);
			combatEffectData.clickboxHeightScale = item.TryGetFloat("clickbox_height_scale", 1f);
			combatEffectData.clickboxRadiusScale = item.TryGetFloat("clickbox_radius_scale", 1f);
			combatEffectData.clickboxCenter = item.TryGetVector("clickbox_center", new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
			combatEffectData.suppressVO = item.TryGetBool("suppress_vo", false);
			combatEffectData.BuildAttackLimiterData(item);
			combatEffectData.BuildAnimationOverrideData(item);
			combatEffectData.GenerateIconAndToolTip();
			if (combatEffectDataDictionary.ContainsKey(combatEffectData.combatEffectName))
			{
				CombatEffectData combatEffectData2 = combatEffectDataDictionary[combatEffectData.combatEffectName];
				if (combatEffectData.combatEffectName == combatEffectData2.combatEffectName)
				{
					CspUtils.DebugLog("Combat effect <" + combatEffectData.combatEffectName + "> cannot be created because it already exists!");
				}
				else
				{
					CspUtils.DebugLog("Combat effect <" + combatEffectData.combatEffectName + "> cannot be created because it will overwrite <" + combatEffectData2.combatEffectName + ">");
				}
			}
			else
			{
				combatEffectDataDictionary.Add(combatEffectData.combatEffectName, combatEffectData);
			}
		}
		combatEffectResisterDictionary = new Dictionary<string, CombatEffectResister>();
		foreach (DataWarehouse item2 in data.GetIterator("//combat_effect_resister"))
		{
			string text2 = item2.TryGetString("resister_name", string.Empty);
			if (!string.IsNullOrEmpty(text2))
			{
				CombatEffectResister combatEffectResister = new CombatEffectResister();
				combatEffectResister.InitializeFromData(item2);
				combatEffectResisterDictionary.Add(text2, combatEffectResister);
			}
		}
	}

	public CombatEffectData getCombatEffectData(string combatEffectName)
	{
		CombatEffectData value;
		combatEffectDataDictionary.TryGetValue(combatEffectName, out value);
		return value;
	}

	public CombatEffectResister GetCombatEffectResister(string combatEffectResisterName)
	{
		CombatEffectResister value;
		combatEffectResisterDictionary.TryGetValue(combatEffectResisterName, out value);
		return value;
	}
}
