using UnityEngine;

public class CombatEffectStatModifier : CombatEffectBase
{
	protected CharacterMotionController motionController;

	protected CombatController combatController;

	protected CharacterStat healthStat;

	protected CharacterStat powerStat;

	protected int statKey;

	protected float powerChangeValue;

	public override void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		GameObject gameObject = base.transform.root.gameObject;
		motionController = (gameObject.GetComponent(typeof(CharacterMotionController)) as CharacterMotionController);
		combatController = (gameObject.GetComponent(typeof(CombatController)) as CombatController);
		CharacterStats characterStats = gameObject.GetComponent(typeof(CharacterStats)) as CharacterStats;
		if (characterStats != null)
		{
			healthStat = characterStats.GetStat(CharacterStats.StatType.Health);
			powerStat = characterStats.GetStat(CharacterStats.StatType.Power);
		}
		if (combatEffectData.healthModifier > 0f)
		{
			healthStat.InitialValue *= combatEffectData.healthModifier;
			healthStat.InitialMaximum *= combatEffectData.healthModifier;
			healthStat.Value *= combatEffectData.healthModifier;
			healthStat.MaximumValue *= combatEffectData.healthModifier;
		}
		if (combatEffectData.healthRegenModifier.getValue(null, false) != 0f && healthStat != null)
		{
			statKey = healthStat.AddTimedUpdate(1f, combatEffectData.healthRegenModifier.getValue(_sourceCombatController));
		}
		if (combatEffectData.speedMultiplier != 1f)
		{
			motionController.addSpeedMultiplier(combatEffectData.speedMultiplier);
		}
		if (combatEffectData.damageMultiplier != 1f)
		{
			combatController.addDamageMultiplier(combatEffectData.damageMultiplier);
		}
		if (combatEffectData.incomingDamageMultiplier != 1f)
		{
			combatController.addIncomingDamageMultiplier(combatEffectData.incomingDamageMultiplier);
		}
		if (combatEffectData.maximumRecoilResisted != 0)
		{
			combatController.recoilResistance += combatEffectData.maximumRecoilResisted;
		}
		if (combatEffectData.maximumRecoilAllowed != 0)
		{
			combatController.recoilLimit += combatEffectData.maximumRecoilAllowed;
		}
		if (combatEffectData.minimumAttackRecoil != 0)
		{
			combatController.recoilEnhancement += combatEffectData.minimumAttackRecoil;
		}
		if (combatEffectData.maximumAttackRecoil != 10)
		{
			combatController.recoilNerf = combatEffectData.maximumAttackRecoil;
		}
		if (combatEffectData.recoilInterruptModifier != 0)
		{
			combatController.recoilInterruptModifier += combatEffectData.recoilInterruptModifier;
		}
		if (combatEffectData.pushbackResistanceModifier != 0f)
		{
			combatController.pushbackResistance += combatEffectData.pushbackResistanceModifier;
		}
		if (combatEffectData.launchResistanceModifier != 0f)
		{
			combatController.launchResistance += combatEffectData.launchResistanceModifier;
		}
		if (combatEffectData.bonusTargetCombatEffect != null)
		{
			combatController.bonusTargetCombatEffects.Add(combatEffectData.bonusTargetCombatEffect);
		}
		if (combatEffectData.bonusIncommingEffect != null)
		{
			combatController.bonusIncommingEffects.Add(combatEffectData.bonusIncommingEffect);
		}
		if (combatEffectData.bonusImpactEffect != null)
		{
			combatController.bonusImpactEffects.Add(combatEffectData.bonusImpactEffect);
		}
		if (combatEffectData.stealthMode)
		{
			combatController.OnEnterStealthMode(newCombatEffectData.combatEffectName);
		}
		if (combatEffectData.maxAttackPushback >= 0f)
		{
			combatController.SetMaxPushbackVelocity(combatEffectData.maxAttackPushback);
		}
		if (combatEffectData.maxAttackLaunch >= 0f)
		{
			combatController.SetMaxLaunchVelocity(combatEffectData.maxAttackLaunch);
		}
		if (combatEffectData.maxPushbackDuration >= 0f)
		{
			combatController.SetMaxPushbackDuration(combatEffectData.maxPushbackDuration);
		}
		PlayerCombatController playerCombatController = combatController as PlayerCombatController;
		if (playerCombatController != null)
		{
			if (combatEffectData.powerDamageModifier != 0f)
			{
				playerCombatController.PowerDamageDealtMultiplier += combatEffectData.powerDamageModifier;
			}
			if (combatEffectData.powerReceivedModifier != 0f)
			{
				playerCombatController.PowerDamageReceivedMultiplier += combatEffectData.powerReceivedModifier;
			}
			if (combatEffectData.powerRegenModifier.getValue(null, false) != 0f)
			{
				CspUtils.DebugLog("initializing power regen stat " + combatEffectData.powerRegenModifier.getValue(null, false) + " " + combatEffectData.powerRegenModifier.getValue(_sourceCombatController));
				powerChangeValue = combatEffectData.powerRegenModifier.getValue(_sourceCombatController);
				powerStat.TimedUpdateChange += powerChangeValue;
				powerStat.TimedUpdateDelay = 1f;
				powerStat.StartTimedUpdates();
			}
		}
	}

	private new void OnRemove(bool doRemoveEffect)
	{
		if (combatEffectData.healthModifier > 0f)
		{
			healthStat.InitialValue /= combatEffectData.healthModifier;
			healthStat.InitialMaximum /= combatEffectData.healthModifier;
			healthStat.MaximumValue /= combatEffectData.healthModifier;
		}
		if (combatEffectData.healthRegenModifier.getValue(null, false) != 0f && healthStat != null)
		{
			healthStat.RemoveTimedUpdate(statKey);
		}
		if (combatEffectData.speedMultiplier != 1f)
		{
			motionController.removeSpeedMultiplier(combatEffectData.speedMultiplier);
		}
		if (combatEffectData.damageMultiplier != 1f)
		{
			combatController.removeDamageMultiplier(combatEffectData.damageMultiplier);
		}
		if (combatEffectData.incomingDamageMultiplier != 1f)
		{
			combatController.removeIncomingDamageMultiplier(combatEffectData.incomingDamageMultiplier);
		}
		if (combatEffectData.maximumRecoilResisted != 0)
		{
			combatController.recoilResistance -= combatEffectData.maximumRecoilResisted;
		}
		if (combatEffectData.maximumRecoilAllowed != 0)
		{
			combatController.recoilLimit -= combatEffectData.maximumRecoilAllowed;
		}
		if (combatEffectData.minimumAttackRecoil != 0)
		{
			combatController.recoilEnhancement -= combatEffectData.minimumAttackRecoil;
		}
		if (combatEffectData.maximumAttackRecoil != 10)
		{
			combatController.recoilNerf = 10;
		}
		if (combatEffectData.recoilInterruptModifier != 0)
		{
			combatController.recoilInterruptModifier -= combatEffectData.recoilInterruptModifier;
		}
		if (combatEffectData.pushbackResistanceModifier != 0f)
		{
			combatController.pushbackResistance -= combatEffectData.pushbackResistanceModifier;
		}
		if (combatEffectData.launchResistanceModifier != 0f)
		{
			combatController.launchResistance -= combatEffectData.launchResistanceModifier;
		}
		if (combatEffectData.bonusTargetCombatEffect != null)
		{
			combatController.bonusTargetCombatEffects.Remove(combatEffectData.bonusTargetCombatEffect);
		}
		if (combatEffectData.bonusIncommingEffect != null)
		{
			combatController.bonusIncommingEffects.Remove(combatEffectData.bonusIncommingEffect);
		}
		if (combatEffectData.bonusImpactEffect != null)
		{
			combatController.bonusImpactEffects.Remove(combatEffectData.bonusImpactEffect);
		}
		if (combatEffectData.stealthMode)
		{
			combatController.OnExitStealthMode();
		}
		if (combatEffectData.maxAttackPushback >= 0f)
		{
			combatController.RemoveMaxPushbackVelocity(combatEffectData.maxAttackPushback);
		}
		if (combatEffectData.maxAttackLaunch >= 0f)
		{
			combatController.RemoveMaxLaunchVelocity(combatEffectData.maxAttackLaunch);
		}
		if (combatEffectData.maxPushbackDuration >= 0f)
		{
			combatController.RemoveMaxPushbackDuration(combatEffectData.maxPushbackDuration);
		}
		PlayerCombatController playerCombatController = combatController as PlayerCombatController;
		if (!(playerCombatController != null))
		{
			return;
		}
		if (combatEffectData.powerDamageModifier != 0f)
		{
			playerCombatController.PowerDamageDealtMultiplier -= combatEffectData.powerDamageModifier;
		}
		if (combatEffectData.powerReceivedModifier != 0f)
		{
			playerCombatController.PowerDamageReceivedMultiplier -= combatEffectData.powerReceivedModifier;
		}
		if (combatEffectData.powerRegenModifier.getValue(null, false) != 0f && powerStat != null)
		{
			powerStat.TimedUpdateChange -= powerChangeValue;
			if (powerStat.TimedUpdateChange == 0f)
			{
				powerStat.StopTimedUpdates();
			}
		}
	}
}
