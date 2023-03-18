using System.Collections.Generic;
using UnityEngine;

public class BehaviorApproach : BehaviorMovement
{
	public delegate void OnArrive(GameObject objMoving);

	public delegate void OnCanceled(GameObject objMoving);

	protected const float maxStalledTime = 2f;

	protected bool bCancelOnMove = true;

	protected bool bWaitForRotation;

	protected bool bLookAtPosition;

	protected OnArrive onArrive;

	protected bool bArrived;

	protected OnCanceled onCancel;

	protected bool bCancelled;

	protected Vector3 targetPosition;

	protected Quaternion targetRotation;

	protected Vector3 targetLook;

	protected float tolerenceSqr;

	protected float rotateDistSqr;

	protected bool previousCatchupSetting;

	protected EffectSequence approachSequence;

	public void Initialize(Vector3 newTargetPosition, Quaternion rotation, bool bCancelOnMove, OnArrive onArrive, OnCanceled onCanceled, float tolerance, float rotateDist, bool waitForRotation, bool lookAtPosition)
	{
		Initialize(newTargetPosition, rotation, bCancelOnMove, onArrive, onCanceled, tolerance, rotateDist, waitForRotation, lookAtPosition, true);
	}

	public void Initialize(Vector3 newTargetPosition, Quaternion rotation, bool bCancelOnMove, OnArrive onArrive, OnCanceled onCanceled, float tolerance, float rotateDist, bool waitForRotation, bool lookAtPosition, bool disableNet)
	{
		if (newTargetPosition == Vector3.zero)
		{
			CspUtils.DebugLog("BehaviorApproach.Initialize is being called with <0,0,0> target position.");
		}
		this.bCancelOnMove = false;
		this.onArrive = onArrive;
		bArrived = false;
		onCancel = onCanceled;
		bWaitForRotation = waitForRotation;
		targetRotation = rotation;
		bLookAtPosition = lookAtPosition;
		float minimumMoveDistance = CharacterMotionController.minimumMoveDistance;
		if (tolerance > minimumMoveDistance)
		{
			tolerenceSqr = tolerance * tolerance;
		}
		else
		{
			tolerenceSqr = minimumMoveDistance * minimumMoveDistance;
		}
		rotateDistSqr = rotateDist * rotateDist;
		updateTargetLook();
		targetPosition = newTargetPosition;
		Vector3 position = owningObject.transform.position;
		targetPosition.y = position.y;
		lastPosition = owningObject.transform.position;
		charGlobals.motionController.setDestination(targetPosition);
		previousCatchupSetting = charGlobals.motionController.checkForNetCatchups;
		if (disableNet)
		{
			charGlobals.motionController.checkForNetCatchups = false;
		}
		this.bCancelOnMove = bCancelOnMove;
		stalledTime = Time.time;
	}

	public override void behaviorBegin()
	{
		approachSequence = charGlobals.effectsList.PlaySequence("approach_sequence");
		base.behaviorBegin();
	}

	public override void behaviorUpdate()
	{
		if (charGlobals.motionController.getDestination() != targetPosition)
		{
			Vector3 position = owningObject.transform.position;
			targetPosition.y = position.y;
			charGlobals.motionController.setDestination(targetPosition);
		}
		if (HasArrived())
		{
			Arrived();
			return;
		}
		Vector3 vector = owningObject.transform.position - targetPosition;
		vector.y = 0f;
		if (vector.sqrMagnitude <= rotateDistSqr)
		{
			Vector3 from = owningObject.transform.TransformDirection(Vector3.forward);
			from.y = 0f;
			from.Normalize();
			if (bLookAtPosition)
			{
				updateTargetLook();
			}
			Vector3 forward = Vector3.Slerp(from, targetLook, elapsedTime);
			owningObject.transform.rotation = Quaternion.LookRotation(forward);
			charGlobals.motionController.updateLookDirection();
		}
		if (Time.time - stalledTime >= 2f)
		{
			Canceled();
		}
		else
		{
			base.behaviorUpdate();
		}
	}

	public override void behaviorCancel()
	{
		base.behaviorCancel();
		charGlobals.motionController.stopGently();
		charGlobals.motionController.checkForNetCatchups = previousCatchupSetting;
	}

	public override void behaviorEnd()
	{
		base.behaviorEnd();
		if (approachSequence != null)
		{
			Object.Destroy(approachSequence.gameObject);
		}
		if (!bArrived)
		{
			Canceled();
		}
	}

	protected void updateTargetLook()
	{
		if (bLookAtPosition)
		{
			if (targetObject != null)
			{
				targetLook = targetObject.transform.position - owningObject.transform.position;
			}
			else
			{
				targetLook = targetPosition - owningObject.transform.position;
			}
		}
		else
		{
			targetLook = targetRotation * Vector3.forward;
		}
		targetLook.y = 0f;
		targetLook.Normalize();
	}

	public override bool useMotionControllerRotate()
	{
		float sqrMagnitude = (owningObject.transform.position - targetPosition).sqrMagnitude;
		if (sqrMagnitude <= rotateDistSqr)
		{
			return false;
		}
		return true;
	}

	public override bool allowUserInput()
	{
		return bCancelOnMove;
	}

	public override void destinationChanged()
	{
		if (bCancelOnMove && !bArrived)
		{
			Vector3 vector = charGlobals.motionController.getDestination() - targetPosition;
			vector.y = 0f;
			if (vector.sqrMagnitude > tolerenceSqr)
			{
				Canceled();
			}
		}
	}

	public override void motionJumped()
	{
		if (bCancelOnMove)
		{
			Canceled();
		}
	}

	protected virtual bool HasArrived()
	{
		Vector3 position = owningObject.transform.position;
		position.y = targetPosition.y;
		if ((targetPosition - position).sqrMagnitude > tolerenceSqr)
		{
			return false;
		}
		if (bWaitForRotation)
		{
			Vector3 lhs = owningObject.transform.TransformDirection(Vector3.forward);
			lhs.y = 0f;
			lhs.Normalize();
			if (Vector3.Dot(lhs, targetLook) < 0.999f)
			{
				return false;
			}
		}
		if (!charGlobals.motionController.IsOnGround())
		{
			return false;
		}
		return true;
	}

	protected void Arrived()
	{
		bArrived = true;
		charGlobals.motionController.setDestination(owningObject.transform.position, false);
		motionStoppedMoving();
		charGlobals.motionController.checkForNetCatchups = previousCatchupSetting;
		if (onArrive != null)
		{
			onArrive(owningObject);
		}
		if (charGlobals.behaviorManager.getBehavior() == this)
		{
			charGlobals.behaviorManager.endBehavior();
		}
	}

	protected void Canceled()
	{
		if (!bCancelled)
		{
			bCancelled = true;
			charGlobals.motionController.checkForNetCatchups = previousCatchupSetting;
			if (onCancel != null)
			{
				onCancel(owningObject);
			}
			if (charGlobals.behaviorManager.getBehavior() == this)
			{
				charGlobals.behaviorManager.endBehavior();
			}
		}
	}

	public static void SetupApproachChain(CharacterGlobals player, bool cancelOnMove, OnArrive onArrived, OnCanceled onCanceled, IEnumerator<Transform> waypoints)
	{
		SetupApproachChain(player, cancelOnMove, onArrived, onCanceled, 0.1f, 2f, true, false, waypoints);
	}

	public static void SetupApproachChain(CharacterGlobals player, bool cancelOnMove, OnArrive onArrived, OnCanceled onCanceled, float tolerance, float rotateDistance, bool waitForRotation, bool lookAtPosition, IEnumerator<Transform> waypoints)
	{
		if (waypoints == null || player == null)
		{
			if (onCanceled != null)
			{
				onCanceled(player.gameObject);
			}
			return;
		}
		if (!waypoints.MoveNext())
		{
			if (onArrived != null)
			{
				onArrived(player.gameObject);
			}
			return;
		}
		Transform current = waypoints.Current;
		BehaviorApproach behaviorApproach = player.behaviorManager.requestChangeBehavior<BehaviorApproach>(false);
		if (behaviorApproach == null)
		{
			if (onCanceled != null)
			{
				onCanceled(player.gameObject);
			}
		}
		else
		{
			behaviorApproach.Initialize(current.position, current.rotation, cancelOnMove, delegate(GameObject arrivedPlayer)
			{
				if (arrivedPlayer == null)
				{
					if (onCanceled != null)
					{
						onCanceled(arrivedPlayer);
					}
				}
				else
				{
					SetupApproachChain(player, cancelOnMove, onArrived, onCanceled, tolerance, rotateDistance, waitForRotation, lookAtPosition, waypoints);
				}
			}, onCanceled, tolerance, rotateDistance, waitForRotation, lookAtPosition);
		}
	}
}
