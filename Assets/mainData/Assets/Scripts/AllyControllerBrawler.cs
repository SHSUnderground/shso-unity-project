using System.Collections;
using UnityEngine;

public class AllyControllerBrawler : AIControllerBrawler
{
	public string forcedAttackName = string.Empty;

	public bool oneShot;

	public string deathAnimOverride = string.Empty;

	protected override IEnumerator AIMain()
	{
		runningAIRoutine = true;
		yield return 0;
		yield return 0;
		while (combatController == null || combatController.colliderObjects == null || combatController.colliderObjects.Count == 0)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (oneShot)
		{
			combatController.setMaxHealth(1000f);
			combatController.setHealth(1000f);
			charGlobals.behaviorManager.OverrideAnimation("recoil_death", deathAnimOverride);
		}
		bool alive = true;
		while (alive)
		{
			if (spawnTime > 0f && lifeDuration > 0f && Time.time > spawnTime + lifeDuration)
			{
				charGlobals.behaviorManager.OverrideAnimation("recoil_death", deathAnimOverride);
				combatController.dieNow();
				break;
			}
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
								if (Random.Range(0f, 1f) < changeTargetChance)
								{
									targetAssignedTime = Time.time;
									if (findNewTarget() == TargetType.TargetPlayer && !currentTarget.targetNet.IsOwner())
									{
										break;
									}
								}
								if (!combatController.InPursuitOfTarget(GetTarget()) && !combatController.AwaitingPursuitOfTarget(GetTarget()))
								{
									if (oneShot && combatController.successfulAttacks > 0)
									{
										CspUtils.DebugLog("killing ally because oneShot = true and they have successfully attacked");
										combatController.dieNow();
										break;
									}
									if (aiCombatController != null)
									{
										aiCombatController.clearChosenAttack(false);
									}
									if (forcedAttackName == string.Empty)
									{
										if (combatController.successfulAttacks > 0 && combatController.successfulAttacks % 3 == 0 && combatController.HasSecondaryAttack)
										{
											combatController.pursueTarget(GetTarget(), true);
										}
										else
										{
											combatController.pursueTarget(GetTarget(), false);
										}
									}
									else
									{
										combatController.pursueTarget(GetTarget(), true, false, forcedAttackName);
									}
								}
								else if (aiCombatController != null)
								{
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

	public override void HitByEnemy(CombatController enemyCombatController)
	{
		if (enemyCombatController.faction == combatController.faction)
		{
			CspUtils.DebugLog("Ally was HitByEnemy, but the enemy is of the same faction so skip");
		}
		else
		{
			base.HitByEnemy(enemyCombatController);
		}
	}
}
