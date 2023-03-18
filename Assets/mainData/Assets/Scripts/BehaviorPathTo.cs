using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorPathTo : BehaviorApproach
{
	public enum PathStatus
	{
		PathNotFound,
		PathingTo,
		Arrived,
		Rotating,
		TimedOut,
		ItemMoved,
		PathError
	}

	protected const float FUDGE = 0.2f;

	protected PathFinder pathFinder;

	protected List<Vector3> currentPath;

	protected List<int> teleportIndices;

	protected int currentPathNodeIndex;

	protected HqItem targetItem;

	protected bool isMoving;

	protected Vector3 originalItemPosition;

	protected PathStatus status;

	public PathStatus Status
	{
		get
		{
			return status;
		}
	}

	public void Initialize(Vector3 position, Quaternion rotation, HqItem item, bool adjustDestination, OnArrive onArrive, OnCanceled onCanceled, float tolerance, float rotateDist, bool waitForRotation, bool lookAtPosition, PathFinder pf, PathFinder.DoesObstacleBlock obstacleCheck)
	{
		status = PathStatus.PathError;
		pathFinder = pf;
		base.targetPosition = position;
		originalItemPosition = item.gameObject.transform.position;
		targetItem = item;
		currentPath = new List<Vector3>();
		Vector3 position2 = owningObject.transform.position;
		Vector3 targetPosition = base.targetPosition;
		targetPosition.y += PathNodeBase.StepHeight + 0.1f;
		position2.y = targetPosition.y;
		if (pathFinder != null)
		{
			List<PathNode> list = pathFinder.FindPath(owningObject.transform.position, base.targetPosition, true, obstacleCheck);
			if (list == null || list.Count < 1)
			{
				status = PathStatus.PathNotFound;
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				currentPath.Add(list[i].transform.position);
				if (i <= 0)
				{
					continue;
				}
				PathNode pathNode = list[i - 1];
				if (pathNode.teleportLinks.Contains(list[i]))
				{
					if (teleportIndices == null)
					{
						teleportIndices = new List<int>();
					}
					teleportIndices.Add(i);
				}
			}
			if (list[list.Count - 1].GapInBetween(base.targetPosition))
			{
				status = PathStatus.PathNotFound;
				return;
			}
			if ((base.targetPosition - currentPath[currentPath.Count - 1]).sqrMagnitude > tolerenceSqr)
			{
				currentPath.Add(base.targetPosition);
			}
			if (adjustDestination && !AdjustDestination())
			{
				status = PathStatus.PathNotFound;
				return;
			}
			currentPathNodeIndex = 0;
			if (currentPath.Count > 0)
			{
				base.targetPosition = currentPath[currentPathNodeIndex];
				charGlobals.motionController.setDestination(base.targetPosition);
				isMoving = true;
				status = PathStatus.PathingTo;
			}
			else
			{
				CspUtils.DebugLog("Could not find a valid destination.");
				charGlobals.motionController.setDestination(owningObject.transform.position);
			}
			Initialize(base.targetPosition, rotation, false, onArrive, onCanceled, tolerance, rotateDist, waitForRotation, lookAtPosition);
		}
		else
		{
			CspUtils.DebugLog("BehaviorPathTo pathfinder is null");
			status = PathStatus.PathNotFound;
		}
	}

	public override void behaviorUpdate()
	{
		if (Paused)
		{
			return;
		}
		if (targetItem == null || targetItem.IsDestroyed)
		{
			status = PathStatus.PathError;
			Canceled();
			return;
		}
		if ((lastPosition - owningObject.transform.position).sqrMagnitude >= 0.0001f)
		{
			stalledTime = Time.time;
		}
		else if (elapsedTime - stalledTime >= 2f)
		{
			status = PathStatus.TimedOut;
			Canceled();
			return;
		}
		if ((targetItem.transform.position - originalItemPosition).sqrMagnitude > 0.1f)
		{
			CspUtils.DebugLog(owningObject.name + " is canceling move to " + targetItem.gameObject.name + " because it has moved " + (targetItem.transform.position - originalItemPosition).sqrMagnitude);
			status = PathStatus.ItemMoved;
			Canceled();
			return;
		}
		if (currentPath != null && status != PathStatus.Arrived)
		{
			DebugDrawPath(currentPath);
			Vector3 vector = owningObject.transform.position - targetPosition;
			if (Mathf.Abs(vector.y) < 2f)
			{
				vector.y = 0f;
			}
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude <= tolerenceSqr)
			{
				currentPathNodeIndex++;
				if (currentPathNodeIndex < currentPath.Count)
				{
					targetPosition = currentPath[currentPathNodeIndex];
					if (teleportIndices != null && teleportIndices.Contains(currentPathNodeIndex))
					{
						CspUtils.DebugLog("Teleporting " + owningObject.name + " to pathnode.");
						charGlobals.motionController.teleportTo(targetPosition);
					}
					else
					{
						charGlobals.motionController.setDestination(targetPosition);
					}
					return;
				}
				status = PathStatus.Arrived;
			}
		}
		lastPosition = owningObject.transform.position;
		if (!isMoving)
		{
			Vector3 vector2 = owningObject.transform.position - targetPosition;
			if (Mathf.Abs(vector2.y) < 2f)
			{
				vector2.y = 0f;
			}
			float sqrMagnitude2 = vector2.sqrMagnitude;
			if (sqrMagnitude2 <= tolerenceSqr + 0.2f)
			{
				Arrived();
				return;
			}
			status = PathStatus.TimedOut;
			CspUtils.DebugLog(owningObject.name + " is canceling BehaviorPathTo: " + targetItem.gameObject.name + " because movement stopped before arriving at destination.");
			Canceled();
			return;
		}
		if (HasArrived())
		{
			Arrived();
			return;
		}
		Vector3 vector3 = owningObject.transform.position - targetPosition;
		vector3.y = 0f;
		if (vector3.sqrMagnitude <= rotateDistSqr)
		{
			Vector3 current = owningObject.transform.TransformDirection(Vector3.forward);
			current.y = 0f;
			current.Normalize();
			if (bLookAtPosition)
			{
				updateTargetLook();
			}
			Vector3 vector4 = Vector3.RotateTowards(current, targetLook, charGlobals.motionController.rotateSpeed * ((float)Math.PI / 180f) * Time.deltaTime, 1000f);
			if (vector4 != Vector3.zero)
			{
				owningObject.transform.rotation = Quaternion.LookRotation(vector4);
				charGlobals.motionController.updateLookDirection();
			}
		}
		updateAnimation();
	}

	public override void behaviorEnd()
	{
		StopIdleSequence();
	}

	public override bool allowUserInput()
	{
		return false;
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
		if (currentPath == null || currentPathNodeIndex == currentPath.Count)
		{
			return false;
		}
		return true;
	}

	protected override bool HasArrived()
	{
		if (currentPath != null && currentPathNodeIndex < currentPath.Count)
		{
			return false;
		}
		return base.HasArrived();
	}

	protected bool AdjustDestination()
	{
		if (targetItem != null && currentPath.Count > 0)
		{
			Vector3 vector = owningObject.transform.position;
			if (currentPath.Count > 1)
			{
				vector = currentPath[currentPath.Count - 2];
			}
			Vector3 a = currentPath[currentPath.Count - 1];
			float num = 0f;
			HqObject2 component = Utils.GetComponent<HqObject2>(targetItem.gameObject);
			if (component != null)
			{
				num = component.localBounds.extents.magnitude;
			}
			else if (targetItem.gameObject.collider != null)
			{
				num = targetItem.gameObject.collider.bounds.extents.magnitude;
			}
			Vector3 a2 = a - vector;
			if (num * num < a2.sqrMagnitude)
			{
				Vector3 b = a2.normalized * num;
				a = vector + (a2 - b);
				currentPath[currentPath.Count - 1] = a;
			}
			else
			{
				currentPath.RemoveAt(currentPath.Count - 1);
			}
			return true;
		}
		return false;
	}

	public override void motionStoppedMoving()
	{
		base.motionStoppedMoving();
		isMoving = false;
	}
}
