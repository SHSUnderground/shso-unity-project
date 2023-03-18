using System.Collections.Generic;
using UnityEngine;

public class AttackColliderController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool isBeam;

	protected Vector3 originalLocalScale = Vector3.one;

	protected CombatController.AttackData attackData;

	protected CombatController.ImpactData impactData;

	protected CombatController combatController;

	protected Dictionary<int, int> hitTargets;

	private void Start()
	{
		hitTargets = new Dictionary<int, int>();
	}

	private void OnDisable()
	{
		if (hitTargets != null)
		{
			hitTargets.Clear();
		}
	}

	private bool IsValidForDivision(Vector3 v)
	{
		return IsValidForDivision(v.x) && IsValidForDivision(v.y) && IsValidForDivision(v.z);
	}

	private bool IsValidForDivision(float f)
	{
		if (float.IsNaN(f))
		{
			return false;
		}
		if (float.IsInfinity(f))
		{
			return false;
		}
		if (Mathf.Approximately(f, 0f))
		{
			return false;
		}
		return true;
	}

	private bool IsValid(Vector3 v)
	{
		return IsValid(v.x) && IsValid(v.y) && IsValid(v.z);
	}

	private bool IsValid(float f)
	{
		if (float.IsNaN(f))
		{
			return false;
		}
		if (float.IsInfinity(f))
		{
			return false;
		}
		return true;
	}

	private void LateUpdate()
	{
		if (isBeam)
		{
			originalLocalScale = base.transform.localScale;
			if (!IsValid(originalLocalScale))
			{
				CspUtils.DebugLogError("(2) Invalid local scale <" + base.gameObject.name + "> scale ignored <" + originalLocalScale.x + "," + originalLocalScale.y + "," + originalLocalScale.z + ">");
				originalLocalScale = Vector3.zero;
			}
		}
		if (base.gameObject.transform.parent != null)
		{
			Vector3 localScale = base.gameObject.transform.parent.transform.localScale;
			if (IsValidForDivision(localScale))
			{
				Vector3 localScale2 = default(Vector3);
				localScale2.x = originalLocalScale.x / localScale.x;
				localScale2.y = originalLocalScale.y / localScale.y;
				localScale2.z = originalLocalScale.z / localScale.z;
				base.gameObject.transform.localScale = localScale2;
			}
			else
			{
				CspUtils.DebugLogError("Invalid parent scale <" + base.transform.parent.gameObject.name + "> scale ignored <" + localScale.x + "," + localScale.y + "," + localScale.z + ">");
				base.gameObject.transform.localScale = Vector3.one;
			}
		}
	}

	public void Initialize(CombatController.AttackData newAttackData, CombatController.ImpactData newImpactData)
	{
		originalLocalScale = base.transform.localScale;
		if (!IsValid(originalLocalScale))
		{
			CspUtils.DebugLogError("(1) Invalid local scale <" + base.gameObject.name + "> scale ignored <" + originalLocalScale.x + "," + originalLocalScale.y + "," + originalLocalScale.z + ">");
			originalLocalScale = Vector3.one;
		}
		attackData = newAttackData;
		impactData = newImpactData;
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if (this.combatController == null || impactData == null)
		{
			return;
		}
		int instanceID = other.gameObject.GetInstanceID();
		CombatController combatController = other.gameObject.GetComponent(typeof(CombatController)) as CombatController;
		if (impactData.maxHitsPerTarget > 0 && hitTargets.ContainsKey(instanceID) && hitTargets[instanceID] >= impactData.maxHitsPerTarget)
		{
			return;
		}
		if (!impactData.hitsFriends)
		{
			if (other.gameObject == this.combatController.gameObject)
			{
				return;
			}
			SpawnData spawnData = other.gameObject.GetComponent(typeof(SpawnData)) as SpawnData;
			if (spawnData != null && spawnData.spawnType == (CharacterSpawn.Type.Remote | CharacterSpawn.Type.Player))
			{
				if (this.combatController.faction != 0 && checkExtraConditions(other.gameObject) && combatController != null)
				{
					this.combatController.tryShowPopupDamage(combatController, this.combatController.getDamageAmount(combatController, impactData));
				}
				return;
			}
		}
		if (!checkExtraConditions(other.gameObject))
		{
			return;
		}
		if (combatController != null)
		{
			if (hitTargets.ContainsKey(instanceID))
			{
				Dictionary<int, int> dictionary;
				Dictionary<int, int> dictionary2 = dictionary = hitTargets;
				int key;
				int key2 = key = instanceID;
				key = dictionary[key];
				dictionary2[key2] = key + 1;
			}
			else
			{
				hitTargets.Add(instanceID, 1);
			}
			Vector3 colliderPosition = (!impactData.impactResult.pushbackFromCollider || impactData.impactType == CombatController.ImpactData.ImpactType.Beam) ? this.combatController.transform.position : base.transform.position;
			this.combatController.attackHit(colliderPosition, combatController, attackData, impactData);
		}
		else if (other.transform.parent != null)
		{
			SquadBattleSmashable squadBattleSmashable = other.transform.parent.GetComponent(typeof(SquadBattleSmashable)) as SquadBattleSmashable;
			if (squadBattleSmashable != null)
			{
				squadBattleSmashable.TriggerHit(base.transform.root.gameObject);
			}
		}
	}

	protected virtual bool checkExtraConditions(GameObject targetObject)
	{
		if (impactData.minimumDistanceSqr > 0f && (targetObject.transform.position - base.gameObject.transform.position).sqrMagnitude < impactData.minimumDistanceSqr)
		{
			return false;
		}
		if (!string.IsNullOrEmpty(impactData.requiredCombatEffect))
		{
			CombatController combatController = targetObject.GetComponent(typeof(CombatController)) as CombatController;
			if (combatController == null)
			{
				return false;
			}
			if (combatController.currentActiveEffects == null)
			{
				return false;
			}
			if (!combatController.currentActiveEffects.ContainsKey(impactData.requiredCombatEffect))
			{
				return false;
			}
		}
		CharacterGlobals component = targetObject.GetComponent<CharacterGlobals>();
		if (component != null && component.combatController != null && component.combatController.Unattackable)
		{
			return false;
		}
		return true;
	}

	protected virtual void OnTriggerStay(Collider other)
	{
	}

	public void setCombatController(CombatController newCombatController)
	{
		combatController = newCombatController;
	}
}
