using System;
using UnityEngine;

internal class BehaviorPickupThrowable : BehaviorPickup
{
	protected ThrowableCarry carryObjComponent;

	protected ThrowableGround groundObjComponent;

	public bool Initialize(ThrowableGround throwableGroundComponent)
	{
		if (throwableGroundComponent == null)
		{
			CspUtils.DebugLog("BehaviorPickupThrowable was passed a null object");
			return false;
		}
		groundObjComponent = throwableGroundComponent;
		throwableGroundComponent.PickupCharacter = owningObject;
		Vector3 eulerAngles = owningObject.transform.rotation.eulerAngles;
		throwableGroundComponent.originalCharacterRotation = eulerAngles.y;
		throwableGroundComponent.originalRotation = throwableGroundComponent.transform.rotation;
		GameObject gameObject = UnityEngine.Object.Instantiate(throwableGroundComponent.carryPrefab, throwableGroundComponent.transform.position, throwableGroundComponent.transform.rotation) as GameObject;
		carryObjComponent = (gameObject.GetComponent(typeof(ThrowableCarry)) as ThrowableCarry);
		if (carryObjComponent != null)
		{
			carryObjComponent.Initialize(throwableGroundComponent, charGlobals.combatController.playPickupThrowableEffect());
			if (!Initialize(gameObject, null))
			{
				return false;
			}
			Utils.ActivateTree(throwableGroundComponent.gameObject, false);
			if (!string.IsNullOrEmpty(throwableGroundComponent.DestroyEvent))
			{
				ScenarioEventManager.Instance.SubscribeScenarioEvent(throwableGroundComponent.DestroyEvent, OnDestroyGroundComponentEvent);
			}
			charGlobals.motionController.dropThrowable();
			charGlobals.motionController.carriedThrowable = carryObjComponent;
			if (networkComponent.IsOwner())
			{
				NetActionPickupThrowable action = new NetActionPickupThrowable(owningObject, throwableGroundComponent.gameObject);
				networkComponent.QueueNetAction(action);
				charGlobals.motionController.positionSent(true);
			}
			return true;
		}
		CspUtils.DebugLog("Cannot find ThrowableCarry component on " + gameObject.name + " in BehaviorPickupThrowable");
		return false;
	}

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		charGlobals.motionController.setDestination(owningObject.transform.position, false);
	}

	public override void behaviorEnd()
	{
		base.behaviorEnd();
		if (!pickupCompleted)
		{
			charGlobals.motionController.dropThrowable();
		}
		else
		{
			combatController.createCombatEffect("ThrowablePickupRecoilResistance", combatController, false);
		}
		carryObjComponent = null;
		attachNode = null;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		if (newBehaviorType.BaseType == typeof(BehaviorRecoil))
		{
			return false;
		}
		return true;
	}

	protected void OnDestroyGroundComponentEvent(string eventName)
	{
		if (charGlobals.motionController.carriedThrowable != null && charGlobals.motionController.carriedThrowable.groundComponent == groundObjComponent)
		{
			charGlobals.motionController.dropThrowable();
		}
		UnityEngine.Object.Destroy(groundObjComponent.gameObject);
		ScenarioEventManager.Instance.UnsubscribeScenarioEvent(eventName, OnDestroyGroundComponentEvent);
	}
}
