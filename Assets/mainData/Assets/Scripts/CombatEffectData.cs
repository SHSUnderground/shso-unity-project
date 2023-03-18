using System.Collections.Generic;
using UnityEngine;

public class CombatEffectData
{
	private enum statWeight
	{
		damage,
		regen,
		power,
		speed,
		armor,
		elemental,
		numStats
	}

	public enum AttackLimiters
	{
		SecondaryOne,
		SecondaryTwo,
		SecondaryThree,
		AttackAll,
		Interact,
		Jump
	}

	public string combatEffectName;

	public string effectPrefabName;

	public string effectRemovePrefabName;

	public string icon;

	public string toolTip;

	public string resisterName;

	public string alertTexture;

	public bool reapplyEffect;

	public ModifierData minimumDuration;

	public ModifierData maximumDuration;

	public string animationDuration;

	public float healthModifier;

	public ModifierData healthRegenModifier;

	public ModifierData powerRegenModifier;

	public float damageMultiplier;

	public float incomingDamageMultiplier;

	public float speedMultiplier;

	public int maximumRecoilResisted;

	public int maximumRecoilAllowed;

	public int minimumAttackRecoil;

	public int maximumAttackRecoil;

	public int recoilInterruptModifier;

	public float powerDamageModifier;

	public float powerReceivedModifier;

	public float pushbackResistanceModifier;

	public float launchResistanceModifier;

	public string bonusTargetCombatEffect;

	public string bonusIncommingEffect;

	public string bonusImpactEffect;

	public bool stealthMode;

	public float maxAttackPushback;

	public float maxAttackLaunch;

	public float maxPushbackDuration;

	public float powerBarTimerDuration;

	public string addComponent;

	public string startMessage;

	public string[] startMessageArgs;

	public string endMessage;

	public string[] endMessageArgs;

	public string startBehavior;

	public string endBehavior;

	public string getupAttack;

	public string replacementCombatEffect;

	public float ablativeHealth;

	public string ablativeEndBehavior;

	public string ablativeReplacementCombatEffect;

	public string attachedProjectile;

	public int attachedProjectileCount;

	public float dotPeriod;

	public string dotAttack;

	public Vector3 scale;

	public Vector3 scaleOffset;

	public float scaleDuration;

	public float scaleRampDuration;

	public string scaleBone;

	public Vector3 appliedVelocity;

	public string gravityWellObject;

	public Vector3 gravityWellOffset;

	public bool usesGravityWell;

	public float gravityNearStrength;

	public float gravityFarStrength;

	public float gravityNearDistance;

	public float gravityFarDistance;

	public float speedStartMultiplier;

	public float speedMidMultiplier;

	public float speedEndMultiplier;

	public float speedMultiplierTime;

	public float speedMultiplierMidTime;

	public int maxRegularAttackChain;

	public int attackLimiter;

	public Dictionary<string, string> animationOverrides;

	public string startScenarioEvent;

	public string endScenarioEvent;

	public string prioritizedAttackName;

	public bool? attacksRepeatable;

	public bool disableInput;

	public float clickboxHeight;

	public float clickboxRadius;

	public float clickboxHeightScale;

	public float clickboxRadiusScale;

	public Vector3 clickboxCenter;

	public bool suppressVO;

	private static readonly string[] GeneratedIcons = new string[6]
	{
		"buff_damage",
		"buff_regen",
		"buff_energy",
		"buff_speed",
		"buff_armor",
		"buff_elemental"
	};

	private static readonly string[] GeneratedToolTips = new string[6]
	{
		"Damage",
		"Health Regeneration",
		"Power Regeneration",
		"Run Speed",
		"Armor",
		"Elemental"
	};

	public bool HasTimerData()
	{
		return minimumDuration.getValue(null, false) > 0f || maximumDuration.getValue(null, false) > 0f || animationDuration != string.Empty;
	}

	public bool HasStatModifierData()
	{
		return healthModifier > 0f || healthRegenModifier.getValue(null, false) != 0f || powerRegenModifier.getValue(null, false) != 0f || damageMultiplier != 1f || speedMultiplier != 1f || incomingDamageMultiplier != 1f || maximumRecoilAllowed != 0 || maximumRecoilResisted != 0 || minimumAttackRecoil != 0 || maximumAttackRecoil != 10 || recoilInterruptModifier != 0 || powerDamageModifier != 0f || powerReceivedModifier != 0f || pushbackResistanceModifier != 0f || launchResistanceModifier != 0f || bonusTargetCombatEffect != null || bonusIncommingEffect != null || bonusImpactEffect != null || stealthMode || maxAttackPushback >= 0f || maxAttackLaunch >= 0f || maxPushbackDuration >= 0f;
	}

	public bool HasPowerBarTimerData()
	{
		return powerBarTimerDuration != 0f;
	}

	public bool HasAddComponentData()
	{
		return !string.IsNullOrEmpty(addComponent);
	}

	public bool HasMessageData()
	{
		return startMessage != string.Empty || endMessage != string.Empty;
	}

	public bool HasBehaviorData()
	{
		return startBehavior != string.Empty || endBehavior != string.Empty;
	}

	public bool HasReplacementData()
	{
		return replacementCombatEffect != string.Empty;
	}

	public bool HasAblativeData()
	{
		return ablativeHealth != 0f;
	}

	public bool HasProjectileData()
	{
		return attachedProjectile != string.Empty;
	}

	public bool HasDOTData()
	{
		return dotPeriod > 0f && dotAttack != string.Empty;
	}

	public bool HasScaleData()
	{
		return scale.x != 1f || scale.y != 1f || scale.z != 1f;
	}

	public bool HasVelocityData()
	{
		return appliedVelocity.sqrMagnitude != 0f || usesGravityWell;
	}

	public bool HasSpeedOverTimeData()
	{
		return speedStartMultiplier != 1f || speedMidMultiplier != 1f || speedEndMultiplier != 1f || speedMultiplierTime != 0f;
	}

	public bool HasAttackLimiterData()
	{
		return maxRegularAttackChain >= 0 || attackLimiter != 0;
	}

	public bool HasAnimationOverrideData()
	{
		return animationOverrides != null && animationOverrides.Count > 0;
	}

	public bool HasScenarioEventData()
	{
		return startScenarioEvent != string.Empty || endScenarioEvent != string.Empty;
	}

	public bool HasPriorityAttackData()
	{
		return !string.IsNullOrEmpty(prioritizedAttackName);
	}

	public bool HasAttacksRepeatableData()
	{
		bool? flag = attacksRepeatable;
		return flag.HasValue;
	}

	public bool HasDisableInputData()
	{
		return disableInput;
	}

	public bool HasClickboxSizeData()
	{
		return clickboxHeight > 0f && clickboxRadius > 0f;
	}

	public bool HasClickboxScaleData()
	{
		return clickboxHeightScale != 1f || clickboxRadiusScale != 1f;
	}

	public bool HasClickboxCenterData()
	{
		return clickboxCenter.x != float.MaxValue || clickboxCenter.y != float.MaxValue || clickboxCenter.z != float.MaxValue;
	}

	public bool HasVOData()
	{
		return suppressVO;
	}

	public void GenerateIconAndToolTip()
	{
		if (icon != string.Empty)
		{
			if (icon == "none")
			{
				icon = string.Empty;
				toolTip = string.Empty;
			}
			return;
		}
		float[] array = new float[6]
		{
			damageMultiplier - 1f + (float)(minimumAttackRecoil - (10 - maximumAttackRecoil)) / 10f,
			healthModifier / 1f + healthRegenModifier.getValue(null, false) / 20f,
			powerRegenModifier.getValue(null, false) / 20f + (powerDamageModifier - 1f) + (powerReceivedModifier - 1f),
			speedMultiplier - 1f + ((speedStartMultiplier + speedMidMultiplier + speedEndMultiplier) / 3f - 1f),
			1f - incomingDamageMultiplier + (pushbackResistanceModifier + launchResistanceModifier) / 6f + (float)(maximumRecoilResisted + maximumRecoilAllowed) / 10f,
			0f
		};
		if (bonusTargetCombatEffect != string.Empty && bonusTargetCombatEffect != null)
		{
			array[5] += 1f;
		}
		if (bonusIncommingEffect != string.Empty && bonusIncommingEffect != null)
		{
			array[5] += 1f;
		}
		if (bonusImpactEffect != string.Empty && bonusImpactEffect != null)
		{
			array[5] += 1f;
		}
		if (stealthMode)
		{
			array[5] += 1f;
		}
		if (attachedProjectile != string.Empty && attachedProjectile != null)
		{
			array[5] += 1f;
		}
		int num = 0;
		float num2 = -1f;
		for (int i = 0; i < 6; i++)
		{
			if (array[i] > num2)
			{
				num = i;
				num2 = array[i];
			}
		}
		if (num2 > 0f)
		{
			icon = GeneratedIcons[num];
			toolTip = GeneratedToolTips[num];
		}
	}

	public void BuildAnimationOverrideData(DataWarehouse data)
	{
		if (data.GetCount("animation_override") <= 0)
		{
			animationOverrides = null;
			return;
		}
		animationOverrides = new Dictionary<string, string>();
		foreach (DataWarehouse item in data.GetIterator("animation_override"))
		{
			string @string = item.GetString("base_animation");
			string string2 = item.GetString("override_animation");
			if (!string.IsNullOrEmpty(@string) && !string.IsNullOrEmpty(string2))
			{
				animationOverrides.Add(@string, string2);
			}
		}
	}

	public void BuildAttackLimiterData(DataWarehouse data)
	{
		maxRegularAttackChain = data.TryGetInt("max_regular_attack_chain", -1);
		attackLimiter = 0;
		int count = data.GetCount("secondary_attack_unavailable");
		for (int i = 0; i < count; i++)
		{
			int num = data.GetInt("secondary_attack_unavailable", i) - 1;
			if (num >= 0 && num < 3)
			{
				attackLimiter |= 1 << num;
			}
		}
		if (!data.TryGetBool("allow_attacks", true))
		{
			attackLimiter |= 8;
		}
		if (!data.TryGetBool("allow_interacts", true))
		{
			attackLimiter |= 16;
		}
		if (!data.TryGetBool("allow_jumps", true))
		{
			attackLimiter |= 32;
		}
	}
}
