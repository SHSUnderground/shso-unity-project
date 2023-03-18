using UnityEngine;

public class ImpactMelee : ImpactBase
{
	protected bool colliderShown;

	protected Vector3 colliderOriginalScale;

	protected Vector3 colliderOriginalPosition;

	public bool colliderEnabled;

	public bool colliderDisabled;

	protected bool colliderActive;

	public override void ImpactBegin(CharacterGlobals newCharGlobals, CombatController.AttackData newAttackData, CombatController newTargetCombatController)
	{
		base.ImpactBegin(newCharGlobals, newAttackData, newTargetCombatController);
		colliderOriginalScale = colliderObject.transform.localScale;
		colliderOriginalPosition = colliderObject.transform.localPosition;
		colliderEnabled = false;
		colliderDisabled = false;
		colliderActive = false;
	}

	public override void ImpactUpdate(float elapsedTime)
	{
		if (colliderEnabled || (impactData.impactStartTime > 0f && elapsedTime >= impactData.impactStartTime && !fired))
		{
			colliderEnabled = false;
			ImpactFired();
			colliderActive = true;
			if (charGlobals.spawnData != null && (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Remote) != 0 && charGlobals.combatController.faction != CombatController.Faction.Enemy)
			{
				return;
			}
			Utils.ActivateTree(colliderObject, true);
			if (impactData.colliderScale.getValue(null, false) != 1f)
			{
				colliderObject.transform.localPosition = colliderOriginalPosition * impactData.colliderScale.getValue(charGlobals.combatController);
				colliderObject.transform.localScale = colliderOriginalScale * impactData.colliderScale.getValue(charGlobals.combatController);
			}
			colliderObject.transform.localPosition += impactData.colliderOffset;
			AttackColliderController attackColliderController = colliderObject.GetComponent(typeof(AttackColliderController)) as AttackColliderController;
			if ((bool)attackColliderController)
			{
				attackColliderController.Initialize(attackData, impactData);
			}
			if (impactData.showCollider || (BrawlerController.Instance != null && BrawlerController.Instance.showAttackColliders))
			{
				MeshRenderer meshRenderer = colliderObject.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
				meshRenderer.enabled = true;
				colliderShown = true;
			}
		}
		else if (colliderActive && !done && (colliderDisabled || (impactData.impactEndTime > 0f && elapsedTime >= impactData.impactEndTime)))
		{
			done = true;
			colliderActive = false;
			if (charGlobals.spawnData != null && charGlobals.spawnData.spawnType == (CharacterSpawn.Type.Remote | CharacterSpawn.Type.Player))
			{
				return;
			}
			Utils.ActivateTree(colliderObject, false);
			colliderObject.transform.localScale = colliderOriginalScale;
			colliderObject.transform.localPosition = colliderOriginalPosition;
			if (colliderShown)
			{
				MeshRenderer meshRenderer2 = colliderObject.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
				meshRenderer2.enabled = false;
			}
		}
		base.ImpactUpdate(elapsedTime);
	}

	public override void ImpactEnd()
	{
		if (colliderObject != null)
		{
			colliderObject.transform.localScale = colliderOriginalScale;
			colliderObject.transform.localPosition = colliderOriginalPosition;
			Utils.ActivateTree(colliderObject, false);
		}
		base.ImpactEnd();
	}
}
