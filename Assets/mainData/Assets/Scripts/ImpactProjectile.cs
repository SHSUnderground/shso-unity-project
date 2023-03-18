using System;
using System.Collections.Generic;
using UnityEngine;

public class ImpactProjectile : ImpactBase
{
	protected GameObject projectile;

	protected Vector3 targetPosition;

	protected GameObject targetReticle;

	protected void updateTargetPosition()
	{
		targetPosition = targetCombatController.TargetPosition;
		if (impactData.targetAimOffset != 0f)
		{
			Vector3 vector = charGlobals.transform.position - targetPosition;
			float d = Mathf.Min(vector.magnitude - 0.5f, impactData.targetAimOffset);
			vector = vector.normalized * d;
			targetPosition += vector;
		}
	}

	public override void ImpactBegin(CharacterGlobals newCharGlobals, CombatController.AttackData newAttackData, CombatController newTargetCombatController)
	{
		base.ImpactBegin(newCharGlobals, newAttackData, newTargetCombatController);
		if (targetCombatController != null)
		{
			updateTargetPosition();
		}
		projectile = null;
		if (impactData.projectileTargetReticle != null && impactData.projectileTargetReticle != "false")
		{
			targetReticle = charGlobals.combatController.createEffect(impactData.projectileTargetReticle, null);
			if (targetReticle != null)
			{
				targetReticle.transform.position = targetPosition;
			}
		}
	}

	public override void ImpactUpdate(float elapsedTime)
	{
		if (!fired && impactData.projectileCreateImmediate && projectile == null && elapsedTime > 0.1f)
		{
			CreateProjectile(charGlobals, colliderObject, colliderObject.transform.parent.gameObject);
		}
		if (attackData.trackTarget && !fired && targetCombatController != null)
		{
			updateTargetPosition();
			if (targetReticle != null)
			{
				targetReticle.transform.position = targetPosition;
			}
		}
		if (fireNow || (impactData.firingTime > 0f && elapsedTime > impactData.firingTime && !fired))
		{
			ImpactFired();
			if (!impactData.projectileCreateImmediate)
			{
				CreateProjectile(charGlobals, colliderObject, null);
			}
			LaunchProjectile(targetCombatController);
			if (!impactData.projectileAttached && charGlobals != null && impactData.projectileAdditionalTargets > 0 && charGlobals.networkComponent != null && charGlobals.networkComponent.IsOwner())
			{
				int num = impactData.projectileAdditionalTargets;
				List<CombatController> list = new List<CombatController>();
				Collider[] array = Physics.OverlapSphere(colliderObject.transform.position, attackData.maximumRange, 2101248);
				Collider[] array2 = array;
				foreach (Collider collider in array2)
				{
					CombatController combatController = collider.gameObject.GetComponent(typeof(CombatController)) as CombatController;
					if (combatController != null && combatController != targetCombatController && charGlobals.combatController.IsControllerEnemy(combatController) && charGlobals.combatController.checkRangeToController(combatController, attackData))
					{
						list.Add(combatController);
					}
				}
				while (num > 0 && list.Count > 0)
				{
					int index = UnityEngine.Random.Range(0, list.Count);
					CreateProjectile(charGlobals, colliderObject, null);
					LaunchProjectile(list[index]);
					NetActionProjectile action = new NetActionProjectile(null, list[index].gameObject, attackData.attackName, impactData.index);
					charGlobals.networkComponent.QueueNetAction(action);
					num--;
					list.RemoveAt(index);
				}
			}
		}
		base.ImpactUpdate(elapsedTime);
	}

	public override void ImpactEnd()
	{
		base.ImpactEnd();
		if (projectile != null)
		{
			UnityEngine.Object.Destroy(projectile);
		}
		if (targetReticle != null)
		{
			UnityEngine.Object.Destroy(targetReticle);
		}
	}

	public void CreateProjectile(CharacterGlobals currentGlobals, GameObject referenceObject, GameObject targetParent)
	{
		if (referenceObject == null)
		{
			referenceObject = colliderObject;
		}
		GameObject gameObject = null;
		if (currentGlobals.motionController.carriedThrowable != null)
		{
			gameObject = currentGlobals.motionController.carriedThrowable.projectilePrefab;
			currentGlobals.motionController.carriedThrowable.ProjectileThrown();
		}
		else if (impactData.projectileName != null)
		{
			if (impactData.projectileIsEnvironmental)
			{
				EnvironmentalObjectReference[] array = UnityEngine.Object.FindObjectsOfType(typeof(EnvironmentalObjectReference)) as EnvironmentalObjectReference[];
				EnvironmentalObjectReference[] array2 = array;
				foreach (EnvironmentalObjectReference environmentalObjectReference in array2)
				{
					if (environmentalObjectReference.objectName == impactData.projectileName)
					{
						gameObject = environmentalObjectReference.reference;
					}
				}
				if (gameObject == null)
				{
					CspUtils.DebugLog("Cannot find specified environmental projectile " + impactData.projectileName);
				}
			}
			else
			{
				gameObject = (currentGlobals.combatController.effectSequenceSource.GetEffectSequencePrefabByName(impactData.projectileName) as GameObject);
			}
		}
		if (gameObject != null)
		{
			if (currentGlobals.motionController.carriedThrowable != null)
			{
				projectile = (UnityEngine.Object.Instantiate(gameObject, currentGlobals.motionController.carriedThrowable.gameObject.transform.position, currentGlobals.motionController.carriedThrowable.gameObject.transform.rotation) as GameObject);
			}
			else
			{
				projectile = (UnityEngine.Object.Instantiate(gameObject, referenceObject.transform.position, referenceObject.transform.rotation) as GameObject);
			}
			if (impactData.projectileAttached || targetParent != null)
			{
				projectile.transform.localPosition = gameObject.transform.localPosition;
				projectile.transform.localRotation = gameObject.transform.localRotation;
				if (targetParent == null)
				{
					Utils.AttachGameObject(referenceObject.transform.parent.gameObject, projectile);
				}
				else
				{
					Utils.AttachGameObject(targetParent, projectile);
				}
			}
		}
		else if (impactData.projectileName != null)
		{
			CspUtils.DebugLog("Failed to spawn projectile " + impactData.projectileName + " for attack " + attackData.attackName);
		}
	}

	public void LaunchProjectile(CombatController targetController)
	{
		if (projectile == null)
		{
			return;
		}
		if (!impactData.projectileAttached && projectile.transform.parent != null)
		{
			projectile.transform.parent = null;
		}
		ProjectileColliderController projectileColliderController = projectile.GetComponent(typeof(ProjectileColliderController)) as ProjectileColliderController;
		if ((bool)projectileColliderController)
		{
			bool newNonImpacting = charGlobals != null && charGlobals.spawnData != null && (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Remote) != 0 && charGlobals.combatController.faction != CombatController.Faction.Enemy;
			projectileColliderController.Initialize(charGlobals, attackData, impactData, newNonImpacting, colliderObject, targetController);
			Vector3 vector;
			if (!impactData.projectileAimed || !(targetController != null))
			{
				vector = ((!impactData.projectileAimed) ? projectile.transform.forward : charGlobals.combatController.transform.forward);
			}
			else
			{
				vector = targetPosition - projectile.transform.position;
				if (impactData.projectileBallistic)
				{
					if (impactData.projectileGravity == 0f)
					{
						CspUtils.DebugLog(attackData.attackName + " has ballistic projectile with no gravity, firing normally");
					}
					else
					{
						float num = impactData.projectileGravity * 0.25f;
						float num2 = num * vector.magnitude / (impactData.projectileSpeed * impactData.projectileSpeed);
						float num3;
						if (num2 > 1f)
						{
							num3 = 45f;
						}
						else
						{
							num3 = 28.64789f * Mathf.Asin(num2);
							if (impactData.projectileBallisticLob)
							{
								num3 = 90f - num3;
								vector.y = 0f;
							}
						}
						CspUtils.DebugLog("angle = " + num3);
						vector.Normalize();
						CspUtils.DebugLog("pushvector before = " + vector);
						vector = Vector3.RotateTowards(vector, Vector3.up, num3 * ((float)Math.PI / 180f), 100f);
						CspUtils.DebugLog("pushvector after = " + vector);
					}
				}
				vector.Normalize();
			}
			if (impactData.projectileSpeed > 0f)
			{
				vector *= impactData.projectileSpeed;
				projectile.rigidbody.AddForce(vector, ForceMode.VelocityChange);
			}
			if (impactData.projectileRotateToVelocity)
			{
				projectile.transform.LookAt(projectile.transform.position - vector);
			}
			if (charGlobals.motionController.carriedThrowable != null)
			{
				projectileColliderController.setRotationalVelocity(charGlobals.motionController.carriedThrowable.GetRotationalVelocity());
			}
			if (targetReticle != null)
			{
				projectileColliderController.setReticle(targetReticle);
			}
		}
		else
		{
			CspUtils.DebugLog("Projectile " + projectile.name + " does not have a Projectile Collider Controlller and will not function");
		}
		if (charGlobals.motionController.carriedThrowable != null && charGlobals.networkComponent.IsOwner())
		{
			NetActionDropThrowable action = new NetActionDropThrowable(true);
			charGlobals.networkComponent.QueueNetAction(action);
		}
		projectile = null;
		targetReticle = null;
	}
}
