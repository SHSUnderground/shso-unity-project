using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Brawler/Traps/Pickup")]
public class BrawlerTrapPickup : BrawlerTrapBase
{
	public enum cameraLockTypes
	{
		none,
		lockDuringAttach,
		lockOnAttach
	}

	public GameObject attachPoint;

	public float pickupDuration;

	public bool ignoreGeometry;

	public cameraLockTypes cameraAttachAction;

	protected float dropTime;

	protected List<CharacterGlobals> heldCharGlobals;

	protected override void Start()
	{
		base.Start();
		heldCharGlobals = new List<CharacterGlobals>();
		dropTime = 0f;
	}

	protected override void Update()
	{
		base.Update();
		foreach (CharacterGlobals heldCharGlobal in heldCharGlobals)
		{
			if (!(heldCharGlobal == null))
			{
				if (ignoreGeometry)
				{
					heldCharGlobal.transform.position = attachPoint.transform.position;
					heldCharGlobal.motionController.setDestination(heldCharGlobal.transform.position);
				}
				else
				{
					Vector3 motion = attachPoint.transform.position - heldCharGlobal.transform.position;
					heldCharGlobal.characterController.Move(motion);
					heldCharGlobal.motionController.setDestination(heldCharGlobal.transform.position);
				}
			}
		}
		if (dropTime > 0f && Time.time > dropTime)
		{
			releaseTargets();
		}
	}

	protected void releaseTargets()
	{
		foreach (CharacterGlobals heldCharGlobal in heldCharGlobals)
		{
			heldCharGlobal.behaviorManager.setMotionEnabled(true);
			if (cameraAttachAction == cameraLockTypes.lockDuringAttach)
			{
				heldCharGlobal.motionController.LockTargetCamera(false, false, false, false, 0f);
			}
		}
		heldCharGlobals.Clear();
		dropTime = 0f;
	}

	protected void OnDisable()
	{
		if (heldCharGlobals.Count > 0)
		{
			releaseTargets();
		}
	}

	public override bool OnHitTargetCharacter(CharacterGlobals targetCharGlobals)
	{
		if (base.OnHitTargetCharacter(targetCharGlobals))
		{
			attachTarget(targetCharGlobals);
			return true;
		}
		return false;
	}

	protected void attachTarget(CharacterGlobals targetCharGlobals)
	{
		if (targetCharGlobals.combatController.recoilResistance >= 8 || (targetCharGlobals.combatController.recoilLimit > 0 && targetCharGlobals.combatController.recoilLimit < 8))
		{
			return;
		}
		if (cameraAttachAction != 0 && targetCharGlobals.combatController as PlayerCombatController != null)
		{
			if (cameraAttachAction == cameraLockTypes.lockOnAttach)
			{
				targetCharGlobals.motionController.LockTargetCamera(false, true, false, true, 5f);
			}
			else
			{
				targetCharGlobals.motionController.LockTargetCamera(false, true, false, false, 10f);
			}
		}
		heldCharGlobals.Add(targetCharGlobals);
		targetCharGlobals.behaviorManager.setMotionEnabled(false);
		targetCharGlobals.motionController.setIsOnGround(false);
		targetCharGlobals.gameObject.transform.position = attachPoint.transform.position;
		if (dropTime == 0f)
		{
			dropTime = Time.time + pickupDuration;
		}
	}

	public override void RemoteHitTarget(GameObject target)
	{
		if (!(target == null))
		{
			base.RemoteHitTarget(target);
			CharacterGlobals targetCharGlobals = target.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
			attachTarget(targetCharGlobals);
		}
	}
}
