using UnityEngine;

public class HotspotActionInteractiveObjectController : HotspotInteractiveObjectController
{
	public HotspotAction action;

	public DockPoint[] startingPoints;

	public bool allowSimultaneousUse;

	protected bool inUse;

	public bool Occupied
	{
		get
		{
			return !allowSimultaneousUse && inUse;
		}
		protected set
		{
			inUse = value;
		}
	}

	public override bool CanPlayerUse(GameObject player)
	{
		if (Occupied)
		{
			return false;
		}
		return base.CanPlayerUse(player);
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		if (!base.StartWithPlayer(player, onDone))
		{
			return false;
		}
		if (player == null || action == null || Occupied)
		{
			return false;
		}
		CharacterGlobals charGlobals = Utils.GetComponent<CharacterGlobals>(player);
		if (charGlobals == null)
		{
			CspUtils.DebugLog("Player should always have a CharacterGlobals component attached.");
			return false;
		}
		inUse = true;
		if (startingPoints.Length > 0)
		{
			BehaviorApproach behaviorApproach = charGlobals.behaviorManager.requestChangeBehavior<BehaviorApproach>(false);
			if (behaviorApproach != null)
			{
				DockPoint closestDockPoint = GetClosestDockPoint(player);
				behaviorApproach.Initialize(closestDockPoint.transform.position, closestDockPoint.transform.rotation, true, delegate
				{
					PerformAction(charGlobals, onDone);
				}, delegate
				{
					inUse = false;
					if (onDone != null)
					{
						onDone(player, CompletionStateEnum.Canceled);
					}
				}, 0.1f, 0.1f, false, false);
			}
		}
		else
		{
			PerformAction(charGlobals, onDone);
		}
		return true;
	}

	protected void PerformAction(CharacterGlobals player, OnDone onDone)
	{
		action.PerformAction(player, delegate
		{
			inUse = false;
			if (onDone != null)
			{
				if (player != null)
				{
					onDone(player.gameObject, CompletionStateEnum.Success);
				}
				else
				{
					onDone(null, CompletionStateEnum.Success);
				}
			}
			AppShell.Instance.EventMgr.Fire(base.gameObject, new PetTeleportCommand.PetTeleportEndedEvent(player.gameObject));
		});
		if (player.activeSidekick != null && (player.activeSidekick.motionController.hotSpotType & HotSpotType.Style.Teleport) != 0)
		{
			PetCommandManager component = player.activeSidekick.GetComponent<PetCommandManager>();
			component.AddCommand(new PetTeleportCommand(), true);
			PerformAction(player.activeSidekick, onDone);
		}
		reportHotSpotUse(player.gameObject);
	}

	protected DockPoint GetClosestDockPoint(GameObject player)
	{
		DockPoint result = null;
		float num = float.MaxValue;
		DockPoint[] array = startingPoints;
		foreach (DockPoint dockPoint in array)
		{
			float sqrMagnitude = (dockPoint.transform.position - player.transform.position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = dockPoint;
			}
		}
		return result;
	}
}
