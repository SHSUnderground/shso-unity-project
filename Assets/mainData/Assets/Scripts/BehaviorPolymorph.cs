using System;
using UnityEngine;

public class BehaviorPolymorph : BehaviorBase
{
	private GameObject mPolymorph;

	private CharacterGlobals mPolymorphGlobals;

	public void Initialize(GameObject polymorph, GameObject combatTarget)
	{
		mPolymorph = polymorph;
		if (mPolymorph != null)
		{
			mPolymorphGlobals = mPolymorph.GetComponent<CharacterGlobals>();
		}
		Polymorph(combatTarget);
	}

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		animationComponent.Play(getAnimationName("movement_idle"));
	}

	public override void behaviorEnd()
	{
		base.behaviorEnd();
		if (owningObject != null && owningObject.active)
		{
			Revert();
		}
	}

	public override void behaviorCancel()
	{
		base.behaviorCancel();
		if (owningObject != null && owningObject.active)
		{
			Revert();
		}
	}

	public override bool allowUserInput()
	{
		return false;
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

	public override bool useMotionControllerRotate()
	{
		return false;
	}

	private void Polymorph(GameObject combatTarget)
	{
		if (mPolymorph == null)
		{
			CspUtils.DebugLog("Cannot polymorph: Polymorph object not set for behavior");
			return;
		}
		AddEventListeners();
		mPolymorph.transform.position = owningObject.transform.position;
		mPolymorph.transform.rotation = owningObject.transform.rotation;
		if (mPolymorphGlobals != null)
		{
			mPolymorphGlobals.behaviorManager.setMotionEnabled(true);
			if (mPolymorphGlobals.brawlerCharacterAI != null)
			{
				mPolymorphGlobals.brawlerCharacterAI.RunAI(true);
				if (charGlobals.brawlerCharacterAI != null)
				{
					mPolymorphGlobals.brawlerCharacterAI.aggroDistance = charGlobals.brawlerCharacterAI.aggroDistance;
				}
			}
			if (combatTarget != null && mPolymorphGlobals.networkComponent.IsOwner() && !Utils.IsPlayer(owningObject) && mPolymorphGlobals.combatController.colliderObjects != null && mPolymorphGlobals.combatController.colliderObjects.Count > 0)
			{
				mPolymorphGlobals.combatController.pursueTarget(combatTarget, false);
			}
			Vector3 position = mPolymorph.transform.position;
			position.y += 1f;
			RaycastHit hitInfo;
			if (ShsCharacterController.FindGround(position, 2f, out hitInfo))
			{
				position = ShsCharacterController.FindPositionOnGround(mPolymorph.GetComponent<CharacterController>(), hitInfo.point, ShsCharacterController.GroundOffset);
			}
			mPolymorphGlobals.combatController.ShowCombatant(position, owningObject.transform.rotation, false);
			mPolymorphGlobals.characterController.radius = charGlobals.characterController.radius;
			GUIManager.Instance.TransferHealthBarIndicator(owningObject, mPolymorph);
		}
		charGlobals.combatController.HideCombatant(true);
		if (charGlobals.brawlerCharacterAI != null)
		{
			charGlobals.brawlerCharacterAI.RunAI(false);
		}
		charGlobals.motionController.DisableNetUpdates(true);
		if (Utils.IsLocalPlayer(owningObject))
		{
			CameraTargetHelper componentInChildren = owningObject.GetComponentInChildren<CameraTargetHelper>();
			if (componentInChildren != null)
			{
				componentInChildren.SetTarget(mPolymorph.transform);
			}
			if (PlayerOcclusionDetector.Instance.myPlayer == owningObject)
			{
				PlayerOcclusionDetector.Instance.myPlayer = mPolymorph;
			}
		}
		CharacterSpawn.Type spawnType = charGlobals.spawnData.spawnType;
		CharacterSpawn.Type polymorphType = CharacterSpawn.Type.Unknown;
		if (mPolymorphGlobals != null)
		{
			polymorphType = mPolymorphGlobals.spawnData.spawnType;
		}
		AppShell.Instance.EventMgr.Fire(owningObject, new EntityPolymorphMessage(owningObject, mPolymorph, spawnType, polymorphType, false));
	}

	private void Revert()
	{
		if (mPolymorph == null)
		{
			CspUtils.DebugLog("Cannot revert: Polymorph object not set for behavior");
			return;
		}
		RemoveEventListeners();
		if (Utils.IsLocalPlayer(owningObject))
		{
			CameraTargetHelper componentInChildren = mPolymorph.GetComponentInChildren<CameraTargetHelper>();
			if (componentInChildren != null)
			{
				componentInChildren.SetTarget(owningObject.transform);
			}
			if (PlayerOcclusionDetector.Instance.myPlayer == mPolymorph)
			{
				PlayerOcclusionDetector.Instance.myPlayer = owningObject;
			}
		}
		charGlobals.motionController.DisableNetUpdates(false);
		if (mPolymorphGlobals != null)
		{
			mPolymorphGlobals.behaviorManager.setMotionEnabled(false);
			GUIManager.Instance.TransferHealthBarIndicator(mPolymorph, owningObject);
		}
		Vector3 position = mPolymorph.transform.position;
		position.y += 1f;
		RaycastHit hitInfo;
		if (ShsCharacterController.FindGround(position, 2f, out hitInfo))
		{
			position = ShsCharacterController.FindPositionOnGround(owningObject.GetComponent<CharacterController>(), hitInfo.point, ShsCharacterController.GroundOffset);
		}
		charGlobals.combatController.ShowCombatant(position, mPolymorph.transform.rotation, true);
		if (charGlobals.brawlerCharacterAI != null)
		{
			charGlobals.brawlerCharacterAI.RunAI(true);
		}
		if (charGlobals.motionController.IsForcedVelocity())
		{
			charGlobals.motionController.setForcedVelocity(Vector2.zero, 0f);
		}
		CharacterSpawn.Type spawnType = charGlobals.spawnData.spawnType;
		CharacterSpawn.Type originalType = CharacterSpawn.Type.Unknown;
		if (mPolymorphGlobals != null)
		{
			originalType = mPolymorphGlobals.spawnData.spawnType;
		}
		AppShell.Instance.EventMgr.Fire(owningObject, new EntityPolymorphMessage(mPolymorph, owningObject, originalType, spawnType, true));
		mPolymorph = null;
		mPolymorphGlobals = null;
	}

	private void OnStatChange(CharacterStat.StatChangeEvent msg)
	{
		if (!(mPolymorph == null) && msg != null && !(msg.Character != mPolymorph) && msg.StatType == CharacterStats.StatType.Health)
		{
			charGlobals.combatController.setMaxHealth(msg.MaxValue);
			charGlobals.combatController.setHealth(msg.NewValue);
		}
	}

	private void AddEventListeners()
	{
		AppShell.Instance.EventMgr.AddListener<CharacterStat.StatChangeEvent>(OnStatChange);
	}

	private void RemoveEventListeners()
	{
		AppShell.Instance.EventMgr.RemoveListener<CharacterStat.StatChangeEvent>(OnStatChange);
	}
}
