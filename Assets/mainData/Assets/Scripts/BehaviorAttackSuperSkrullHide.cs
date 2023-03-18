using System;
using UnityEngine;

public class BehaviorAttackSuperSkrullHide : BehaviorAttackBase
{
	private Renderer characterRenderer;

	private bool wentAway;

	public override void Initialize(GameObject newTargetObject, CombatController.AttackData newAttackData, bool newSecondaryAttack, bool chainAttack, float emoteBroadcastRadius)
	{
		base.Initialize(newTargetObject, newAttackData, newSecondaryAttack, chainAttack, emoteBroadcastRadius);
		characterRenderer = (owningObject.GetComponentInChildren(typeof(SkinnedMeshRenderer)) as Renderer);
		wentAway = false;
	}

	public override void behaviorBegin()
	{
		base.behaviorBegin();
	}

	public override void behaviorUpdate()
	{
		if (!wentAway && elapsedTime >= animationComponent[attackData.animName].length)
		{
			animationComponent.Stop();
			goAway();
		}
		base.behaviorUpdate();
	}

	public override void behaviorEnd()
	{
		base.behaviorEnd();
		comeBack();
		if (networkComponent == null || networkComponent.IsOwner())
		{
			CombatController.AttackData secondaryAttackData = combatController.getSecondaryAttackData(1);
			combatController.createAttackBehavior(targetObject, secondaryAttackData, true, false);
		}
	}

	protected void goAway()
	{
		wentAway = true;
		characterRenderer.enabled = false;
		charGlobals.combatController.targetHeightMinimumEnabled = false;
		charGlobals.combatController.targetHeight -= 100f;
		charGlobals.motionController.teleportTo(charGlobals.transform.position + Vector3.up * 100f);
	}

	protected void comeBack()
	{
		InvisibleWomanFadeController invisibleWomanFadeController = charGlobals.GetComponent(typeof(InvisibleWomanFadeController)) as InvisibleWomanFadeController;
		if (invisibleWomanFadeController != null)
		{
			invisibleWomanFadeController.Fade(true);
		}
		if (wentAway)
		{
			wentAway = false;
			characterRenderer.enabled = true;
			charGlobals.combatController.targetHeightMinimumEnabled = true;
			charGlobals.combatController.targetHeight += 100f;
			charGlobals.motionController.teleportTo(charGlobals.transform.position + Vector3.down * 100f);
		}
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override bool useMotionControllerGravity()
	{
		return false;
	}
}
