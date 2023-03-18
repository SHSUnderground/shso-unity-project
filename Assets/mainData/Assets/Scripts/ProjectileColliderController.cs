using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileColliderController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject destruction_effect;

	public bool spawnDestructionEffectOnImpact = true;

	public bool spawnDestructionEffectOnCollision = true;

	public bool destructionEffectSnapToGround;

	public float targetHomingSpeed;

	public GameObject spawnOnAttacker;

	public bool spawnOnAttackerOnImpact = true;

	public bool spawnOnAttackerOnCollision = true;

	protected CombatController.AttackData attackData;

	protected CombatController.ImpactData impactData;

	protected bool nonImpacting;

	protected float endTime = -1f;

	protected bool impacted;

	protected bool disabled;

	protected CharacterGlobals charGlobals;

	protected GameObject emitter;

	protected CombatController targetController;

	protected Vector3 rotationalVelocity;

	protected List<LineRenderer> lineRenderers;

	protected GameObject lineRenderPositionFinder;

	protected GameObject reticle;

	private ThrowableGround throwable;

	protected virtual void Start()
	{
		lineRenderers = new List<LineRenderer>();
		Component[] componentsInChildren = GetComponentsInChildren(typeof(LineRenderer));
		Component[] array = componentsInChildren;
		foreach (Component component in array)
		{
			LineRenderer lineRenderer = component as LineRenderer;
			lineRenderers.Add(lineRenderer);
			lineRenderer.SetVertexCount(2);
			lineRenderer.SetPosition(0, Vector3.zero);
			lineRenderer.SetPosition(1, Vector3.zero);
		}
		if (lineRenderers.Count > 0)
		{
			lineRenderPositionFinder = new GameObject("LineRenderPositionFinder");
			lineRenderPositionFinder.transform.parent = base.transform;
		}
		if (charGlobals != null)
		{
			if (charGlobals.motionController.carriedThrowable != null && !string.IsNullOrEmpty(charGlobals.motionController.carriedThrowable.groundComponent.DestroyEvent))
			{
				throwable = charGlobals.motionController.carriedThrowable.groundComponent;
			}
			if (impactData != null && impactData.projectileScaledToOwner)
			{
				Vector3 localScale = base.transform.localScale;
				base.transform.localScale = Utils.FindNodeInChildren(charGlobals.transform, "export_node", true).localScale;
				ColliderUtil.UnScale(base.gameObject, localScale);
			}
		}
	}

	protected virtual void Update()
	{
		if (endTime > -1f && Time.time > endTime)
		{
			endTimeReached();
		}
		if (disabled)
		{
			return;
		}
		if (impactData != null && impactData.projectileGravity > 0f)
		{
			base.gameObject.transform.rigidbody.AddForce(new Vector3(0f, (0f - impactData.projectileGravity) * Time.deltaTime * 30f, 0f), ForceMode.Acceleration);
			if (impactData.projectileRotateToVelocity)
			{
				base.gameObject.transform.LookAt(base.gameObject.transform.position - base.gameObject.transform.rigidbody.velocity);
			}
		}
		if (targetController != null && targetHomingSpeed > 0f)
		{
			Vector3 target = (targetController.TargetPosition - base.transform.position).normalized * impactData.projectileSpeed;
			Vector3 force = Vector3.RotateTowards(base.rigidbody.velocity, target, targetHomingSpeed * Time.deltaTime * ((float)Math.PI / 180f), 1000f);
			base.rigidbody.velocity = Vector3.zero;
			base.rigidbody.AddForce(force, ForceMode.VelocityChange);
		}
		if (rotationalVelocity.x != 0f || rotationalVelocity.y != 0f || rotationalVelocity.z != 0f)
		{
			base.gameObject.transform.Rotate(rotationalVelocity * Time.deltaTime, Space.Self);
		}
	}

	protected void endTimeReached()
	{
		if (endTime > 0f && impactData.projectileExplosionRadius > 0f)
		{
			SphereCollider sphereCollider = base.gameObject.GetComponent(typeof(SphereCollider)) as SphereCollider;
			if ((bool)sphereCollider)
			{
				sphereCollider.radius = impactData.projectileExplosionRadius;
			}
			else
			{
				CspUtils.DebugLog(base.gameObject.name + " has an explosion radius defined but no sphere collider");
			}
			disabled = false;
			endTime = 0f;
		}
		else
		{
			destroyProjectile();
		}
	}

	protected void destroyProjectile()
	{
		if (destruction_effect != null && ((spawnDestructionEffectOnImpact && impacted) || (spawnDestructionEffectOnCollision && !impacted)))
		{
			spawnObject(destruction_effect, null, base.gameObject.transform.position, destructionEffectSnapToGround);
		}
		if (spawnOnAttacker != null)
		{
			doSpawnOnAttacker();
		}
		if (charGlobals != null)
		{
			charGlobals.combatController.ProjectileDestroyed();
		}
		if (reticle != null)
		{
			UnityEngine.Object.Destroy(reticle);
		}
		if (throwable != null)
		{
			Utils.ActivateTree(throwable.gameObject, true);
			AppShell.Instance.ServerConnection.Game.ReleaseOwnership(throwable.gameObject);
		}
		if (impactData.impactResult.summons != null && impactData.impactResult.summons.Count > 0 && BrawlerController.Instance.LocalPlayer == charGlobals.gameObject)
		{
			double num = Math.PI * 2.0 / (double)impactData.impactResult.summons.Count;
			double num2 = 2.0;
			double num3 = impactData.impactResult.summons.Count - 1;
			foreach (CombatController.SummonData summon in impactData.impactResult.summons)
			{
				double num4 = Math.Sin(num * num3) * num2;
				double num5 = Math.Cos(num * num3) * num2;
				num3 -= 1.0;
				CharacterSpawn.SpawnAlly(summon.name, base.gameObject.transform.localPosition + new Vector3((float)num4, 0f, (float)num5), charGlobals.combatController.faction, summon.powerAttack, summon.duration, false, string.Empty, string.Empty);
			}
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	protected void doSpawnOnAttacker()
	{
		if (!(charGlobals == null) && !(spawnOnAttacker == null) && ((impacted && spawnOnAttackerOnImpact) || (!impacted && spawnOnAttackerOnCollision)))
		{
			spawnObject(spawnOnAttacker, charGlobals.gameObject, charGlobals.transform.position, false);
		}
	}

	protected void spawnObject(GameObject prefab, GameObject parent, Vector3 spawnLocation, bool snapToGround)
	{
		NetworkComponent x = prefab.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
		if (x == null || (charGlobals != null && (charGlobals.networkComponent == null || charGlobals.networkComponent.IsOwner())))
		{
			RaycastHit hitInfo;
			if (snapToGround && Physics.Raycast(spawnLocation, Vector3.down, out hitInfo, 100f, 804756969))
			{
				spawnLocation = hitInfo.point;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, spawnLocation, prefab.transform.rotation) as GameObject;
			if (gameObject != null && parent != null)
			{
				gameObject.transform.parent = parent.transform;
			}
			if (gameObject != null && x != null && charGlobals != null && charGlobals.networkComponent != null)
			{
				charGlobals.networkComponent.AnnounceObjectSpawn(gameObject, "CombatController", prefab.name, parent);
			}
		}
	}

	protected virtual void LateUpdate()
	{
		if (lineRenderers.Count > 0)
		{
			lineRenderPositionFinder.transform.position = emitter.transform.position;
			foreach (LineRenderer lineRenderer in lineRenderers)
			{
				lineRenderer.SetPosition(0, lineRenderPositionFinder.transform.localPosition);
			}
		}
	}

	protected virtual void OnTriggerEnter(Collider collider)
	{
		if (disabled || charGlobals == null || collider.gameObject.layer == 22 || !(collider.gameObject != charGlobals.gameObject))
		{
			return;
		}
		CombatController combatController = collider.gameObject.GetComponent(typeof(CombatController)) as CombatController;
		if (combatController == null && collider.gameObject.transform.parent != null)
		{
			GameObject gameObject = collider.gameObject.transform.parent.gameObject;
			combatController = (gameObject.GetComponent(typeof(CombatController)) as CombatController);
			if (combatController != null && gameObject.GetComponent(typeof(Collider)) as Collider != null)
			{
				combatController = null;
			}
		}
		if (combatController != null)
		{
			CharacterGlobals characterGlobals = collider.gameObject.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
			if (characterGlobals != null && characterGlobals.combatController != null && characterGlobals.combatController.Unattackable)
			{
				return;
			}
			if (!(characterGlobals != null) || !(characterGlobals.spawnData != null) || characterGlobals.spawnData.spawnType == CharacterSpawn.Type.LocalPlayer)
			{
			}
			if (combatController.isKilled)
			{
				return;
			}
			if (!nonImpacting)
			{
				bool flag = true;
				if (characterGlobals != null && characterGlobals.spawnData != null && characterGlobals.spawnData.spawnType == (CharacterSpawn.Type.Remote | CharacterSpawn.Type.Player))
				{
					if (charGlobals.combatController.faction == CombatController.Faction.Player)
					{
						return;
					}
					if (checkExtraConditions(combatController, impactData))
					{
						charGlobals.combatController.tryShowPopupDamage(combatController, charGlobals.combatController.getDamageAmount(combatController, impactData));
					}
					flag = false;
				}
				if ((characterGlobals == null && !impactData.projectileDestroyOnCollision) || !checkExtraConditions(combatController, impactData))
				{
					return;
				}
				if (flag)
				{
					charGlobals.combatController.attackHit(base.gameObject.transform.position, combatController, attackData, impactData);
				}
			}
			if (impactData.projectileImpactStickTime > 0f)
			{
				endTime = Time.time + impactData.projectileImpactStickTime;
				base.rigidbody.velocity = Vector3.zero;
				base.transform.parent = collider.transform;
				disabled = true;
			}
			else if (impactData.projectileDestroyOnImpact && endTime > 0f)
			{
				endTime = Time.time;
				disabled = true;
			}
			impacted = true;
		}
		else
		{
			if (collider.isTrigger || collider.gameObject.name == "__blocker_hack")
			{
				return;
			}
			if (collider.transform.parent != null)
			{
				SquadBattleSmashable squadBattleSmashable = collider.transform.parent.GetComponent(typeof(SquadBattleSmashable)) as SquadBattleSmashable;
				if (squadBattleSmashable != null)
				{
					squadBattleSmashable.TriggerHit(base.gameObject);
				}
			}
			if (endTime > Time.time)
			{
				if (impactData.projectileCollisionStickTime > 0f)
				{
					endTime = Time.time + impactData.projectileCollisionStickTime;
					base.rigidbody.velocity = Vector3.zero;
					disabled = true;
				}
				else if (impactData.projectileDestroyOnCollision)
				{
					endTime = Time.time;
				}
			}
		}
	}

	protected bool checkExtraConditions(CombatController targetCombatController, CombatController.ImpactData impactData)
	{
		if (!string.IsNullOrEmpty(impactData.requiredCombatEffect))
		{
			if (targetCombatController.currentActiveEffects == null)
			{
				return false;
			}
			if (!targetCombatController.currentActiveEffects.ContainsKey(impactData.requiredCombatEffect))
			{
				return false;
			}
		}
		return true;
	}

	public void Initialize(CharacterGlobals newCharGlobals, CombatController.AttackData newAttackData, CombatController.ImpactData newImpactData, bool newNonImpacting, GameObject newEmitter, CombatController newTargetController)
	{
		charGlobals = newCharGlobals;
		attackData = newAttackData;
		impactData = newImpactData;
		nonImpacting = newNonImpacting;
		rotationalVelocity = Vector3.zero;
		endTime = Time.time + impactData.projectileLifespan;
		emitter = newEmitter;
		targetController = newTargetController;
	}

	public void setRotationalVelocity(Vector3 newRotationalVelocity)
	{
		rotationalVelocity = newRotationalVelocity;
	}

	public void setReticle(GameObject newReticle)
	{
		reticle = newReticle;
	}
}
