using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Brawler/Traps/Teleport")]
public class BrawlerTrapTeleport : BrawlerTrapBase
{
	public enum TeleportMode
	{
		ToObject,
		AwayFromFriends,
		AwayFromEnemies
	}

	protected class CameraCut
	{
		private CharacterGlobals targetCharGlobals;

		public CameraCut(CharacterGlobals targetCharGlobals)
		{
			this.targetCharGlobals = targetCharGlobals;
		}

		public void DoCut()
		{
			if (Utils.IsLocalPlayer(targetCharGlobals))
			{
				targetCharGlobals.motionController.LockTargetCamera(true, true, true, true, 0f);
				CharacterMotionController motionController = targetCharGlobals.motionController;
				motionController.landCallback = (CharacterMotionController.MotionCallback)Delegate.Combine(motionController.landCallback, new CharacterMotionController.MotionCallback(OnLanded));
			}
		}

		private void OnLanded()
		{
			CharacterMotionController motionController = targetCharGlobals.motionController;
			motionController.landCallback = (CharacterMotionController.MotionCallback)Delegate.Remove(motionController.landCallback, new CharacterMotionController.MotionCallback(OnLanded));
			targetCharGlobals.StartCoroutine(ResetOnNextFrame());
		}

		private IEnumerator ResetOnNextFrame()
		{
			yield return 0;
			CameraLiteManager.Instance.GetCurrentCamera().Reset();
		}
	}

	public TeleportMode teleportType = TeleportMode.AwayFromFriends;

	public GameObject TeleportDestination;

	public string TeleportDestinationName = string.Empty;

	public bool snapToGroundOnTeleport = true;

	public bool cutCamera;

	public bool useObjectRotation;

	public override bool OnHitTargetCharacter(CharacterGlobals targetCharGlobals)
	{
		if (base.OnHitTargetCharacter(targetCharGlobals))
		{
			teleportTarget(targetCharGlobals);
			return true;
		}
		return false;
	}

	protected BrawlerTeleportGrid GetTeleportGrid()
	{
		if (TeleportDestination != null)
		{
			BrawlerTeleportGrid componentInChildren = TeleportDestination.GetComponentInChildren<BrawlerTeleportGrid>();
			if (componentInChildren != null)
			{
				return componentInChildren;
			}
		}
		BrawlerTeleportGrid[] array = Utils.FindObjectsOfType<BrawlerTeleportGrid>();
		if (array != null && array.Length > 0)
		{
			BrawlerTeleportGrid brawlerTeleportGrid = array[0];
			BrawlerTeleportGrid[] array2 = array;
			foreach (BrawlerTeleportGrid brawlerTeleportGrid2 in array2)
			{
				if ((brawlerTeleportGrid2.transform.position - base.transform.position).sqrMagnitude < (brawlerTeleportGrid.transform.position - base.transform.position).sqrMagnitude)
				{
					brawlerTeleportGrid = brawlerTeleportGrid2;
				}
			}
			return brawlerTeleportGrid;
		}
		return null;
	}

	protected void teleportTarget(CharacterGlobals targetCharGlobals)
	{
		Vector3 vector = new Vector3(0f, 0f, 0f);
		Quaternion rotation = targetCharGlobals.transform.rotation;
		bool flag = true;
		switch (teleportType)
		{
		case TeleportMode.ToObject:
			if (TeleportDestination == null && !string.IsNullOrEmpty(TeleportDestinationName))
			{
				TeleportDestination = GameObject.Find(TeleportDestinationName);
				if (TeleportDestination == null)
				{
					CspUtils.DebugLog("Teleport destination <" + TeleportDestinationName + "> not found in scene");
				}
			}
			if (TeleportDestination != null)
			{
				vector = TeleportDestination.transform.position;
				if (useObjectRotation)
				{
					rotation = TeleportDestination.transform.rotation;
					flag = false;
				}
			}
			else
			{
				CspUtils.DebugLog("No teleport destination in teleport-to-destination mode!");
			}
			break;
		case TeleportMode.AwayFromEnemies:
		{
			BrawlerTeleportGrid teleportGrid2 = GetTeleportGrid();
			if (teleportGrid2 == null)
			{
				CspUtils.DebugLog("No teleport grid located!");
			}
			else
			{
				vector = teleportGrid2.GetSafeLocation(targetCharGlobals.combatController.faction);
			}
			break;
		}
		case TeleportMode.AwayFromFriends:
		{
			BrawlerTeleportGrid teleportGrid = GetTeleportGrid();
			if (teleportGrid == null)
			{
				CspUtils.DebugLog("No teleport grid located!");
			}
			else
			{
				vector = ((targetCharGlobals.combatController.faction != 0) ? teleportGrid.GetSafeLocation(CombatController.Faction.Player) : teleportGrid.GetSafeLocation(CombatController.Faction.Enemy));
			}
			break;
		}
		}
		if (flag)
		{
			Vector3 forward = targetCharGlobals.transform.position - vector;
			forward.y = 0f;
			rotation = Quaternion.LookRotation(forward);
		}
		if (snapToGroundOnTeleport)
		{
			RaycastHit hitInfo = default(RaycastHit);
			if (Physics.Raycast(vector + new Vector3(0f, 5f, 0f), Vector3.down, out hitInfo, targetCharGlobals.characterController.height + 5f, 804756969))
			{
				teleportTargetToDestination(targetCharGlobals, hitInfo.point + new Vector3(0f, 0.1f, 0f), rotation);
			}
		}
		else
		{
			teleportTargetToDestination(targetCharGlobals, vector, rotation);
		}
	}

	public override void RemoteHitTarget(GameObject target)
	{
		if (!(target == null))
		{
			base.RemoteHitTarget(target);
			CharacterGlobals targetCharGlobals = target.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
			teleportTarget(targetCharGlobals);
		}
	}

	protected void teleportTargetToDestination(CharacterGlobals targetCharGlobals, Vector3 destination, Quaternion rotation)
	{
		if (cutCamera)
		{
			new CameraCut(targetCharGlobals).DoCut();
		}
		else
		{
			targetCharGlobals.motionController.cameraForceFollowTime = 1f;
		}
		targetCharGlobals.motionController.teleportTo(destination);
		targetCharGlobals.motionController.setDestination(targetCharGlobals.transform.position);
		targetCharGlobals.transform.rotation = rotation;
		targetCharGlobals.motionController.updateLookDirection();
	}
}
