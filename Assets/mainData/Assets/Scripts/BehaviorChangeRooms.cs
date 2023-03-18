using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorChangeRooms : BehaviorMovement
{
	public enum PathStatus
	{
		PathNotFound,
		PathingTo,
		PathCanceled,
		TimedOut,
		Arrived
	}

	public delegate void OnChangedRooms(GameObject objMoving);

	public delegate void OnCanceled(GameObject objMoving);

	protected const float maxStalledTime = 2f;

	protected OnChangedRooms onChangedRooms;

	protected OnCanceled onCanceled;

	private HqRoom2 nextRoom;

	private Vector3 exitDestination;

	private List<PathNode> currentPath;

	private int currentPathNodeIndex;

	private PathFinder pathFinder;

	protected PathStatus pathStatus;

	public PathStatus Status
	{
		get
		{
			return pathStatus;
		}
	}

	public override bool Paused
	{
		get
		{
			return base.Paused;
		}
		set
		{
			base.Paused = value;
			float speed = 1f;
			if (value)
			{
				speed = 0f;
			}
			else if (currentPath != null && currentPathNodeIndex < currentPath.Count)
			{
				charGlobals.motionController.setDestination(currentPath[currentPathNodeIndex].transform.position);
			}
			foreach (AnimationState item in owningObject.animation)
			{
				item.speed = speed;
			}
		}
	}

	public void Initialize(HqRoom2 currentRoom, HqRoom2 nextRoom, PathFinder pf, PathFinder.DoesObstacleBlock obstacleCheck, OnChangedRooms onChangedRooms, OnCanceled onCanceled)
	{
		this.nextRoom = nextRoom;
		this.onChangedRooms = onChangedRooms;
		this.onCanceled = onCanceled;
		pathFinder = pf;
		pathStatus = PathStatus.PathingTo;
		if (!(currentRoom != null))
		{
			return;
		}
		exitDestination = currentRoom.GetDoor(owningObject.transform.position);
		if (pathFinder != null)
		{
			currentPath = pathFinder.FindPath(owningObject.transform.position, exitDestination, true, obstacleCheck);
			currentPathNodeIndex = 0;
			if (currentPath != null)
			{
				charGlobals.motionController.setDestination(currentPath[currentPathNodeIndex].transform.position);
				return;
			}
			pathStatus = PathStatus.PathNotFound;
			exitDestination = Vector3.zero;
		}
		else
		{
			CspUtils.DebugLog("Could not find a pathfinder for " + owningObject.gameObject.name);
			pathStatus = PathStatus.PathNotFound;
			exitDestination = Vector3.zero;
		}
	}

	public override void behaviorBegin()
	{
		base.behaviorBegin();
	}

	public override void behaviorUpdate()
	{
		if (Paused)
		{
			return;
		}
		base.behaviorUpdate();
		if (elapsedTime - stalledTime >= 2f)
		{
			pathStatus = PathStatus.TimedOut;
			CspUtils.DebugLog("Canceling BehaviorChangeRooms because stalled!");
			Canceled();
			return;
		}
		AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(owningObject);
		if (component.CurrentRoom.RoomState != HqRoom2.AccessState.Unlocked)
		{
			CspUtils.DebugLog("Canceling BehaviorChangeRooms because current room is locked!");
			pathStatus = PathStatus.PathCanceled;
			Canceled();
		}
		else if (exitDestination == Vector3.zero)
		{
			CspUtils.DebugLog("Canceling BehaviorChangeRooms because exitDestination is zero!");
			pathStatus = PathStatus.PathNotFound;
			Canceled();
		}
		else
		{
			if (currentPath == null)
			{
				return;
			}
			DebugDrawPath(currentPath);
			float sqrMagnitude = (owningObject.transform.position - currentPath[currentPathNodeIndex].transform.position).sqrMagnitude;
			if (!(sqrMagnitude <= 1f))
			{
				return;
			}
			currentPathNodeIndex++;
			if (currentPathNodeIndex == currentPath.Count)
			{
				Vector3 randomDoor = nextRoom.RandomDoor;
				charGlobals.motionController.teleportTo(randomDoor);
				if (component != null)
				{
					if (component.CurrentRoom == HqController2.Instance.ActiveRoom)
					{
						component.CurrentRoom.PlayTransporterEffect(exitDestination);
					}
					component.CurrentRoom = nextRoom;
				}
				if (onChangedRooms != null)
				{
					pathStatus = PathStatus.Arrived;
					onChangedRooms(owningObject);
					if (charGlobals.behaviorManager.getBehavior() == this)
					{
						charGlobals.behaviorManager.endBehavior();
					}
				}
			}
			else
			{
				charGlobals.motionController.setDestination(currentPath[currentPathNodeIndex].transform.position);
			}
		}
	}

	public override void behaviorEnd()
	{
		motionStoppedMoving();
		currentPath = null;
		nextRoom = null;
		base.behaviorEnd();
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		if (pathStatus == PathStatus.PathingTo)
		{
			return false;
		}
		return true;
	}

	public override void destinationChanged()
	{
		charGlobals.behaviorManager.endBehavior();
	}

	protected void Canceled()
	{
		pathStatus = PathStatus.PathCanceled;
		CspUtils.DebugLog("Canceling BehaviorChangeRooms!");
		if (onCanceled != null)
		{
			onCanceled(owningObject);
		}
		if (charGlobals.behaviorManager.getBehavior() == this)
		{
			charGlobals.behaviorManager.endBehavior();
		}
	}

	public override bool useMotionController()
	{
		if (Paused)
		{
			return false;
		}
		return base.useMotionController();
	}

	public override bool useMotionControllerRotate()
	{
		if (Paused)
		{
			return false;
		}
		return base.useMotionControllerRotate();
	}
}
