using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorWander : BehaviorMovement
{
	protected enum CurrentState
	{
		Wandering,
		Idle
	}

	public delegate void OnWanderEnd();

	protected const float STUCK_TIMEOUT = 0.5f;

	protected const float MIN_MOVE_DIST = 0.05f;

	protected const float ARRIVED_DISTANCE = 0.1f;

	protected const float STEP_HEIGHT = 0.35f;

	protected const int MIN_WAIT_TIME = 1;

	protected const int MAX_WAIT_TIME = 3;

	protected Vector3 currentWanderPoint;

	protected List<Vector3> currentPath;

	protected int currentPathNodeIndex;

	protected Vector3 previousPosition;

	protected float lastTimeMoved;

	protected CurrentState wanderState;

	protected PathFinder pathfinder;

	protected PathFinder.DoesObstacleBlock obstacleCheckCallback;

	protected GameObject cameraObject;

	protected OnWanderEnd wanderEndCallback;

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
			foreach (AnimationState item in owningObject.animation)
			{
				item.speed = speed;
			}
		}
	}

	public virtual void Initialize(OnWanderEnd wanderEnd, PathFinder pf, PathFinder.DoesObstacleBlock obstacleCheck)
	{
		wanderEndCallback = wanderEnd;
		pathfinder = pf;
		obstacleCheckCallback = obstacleCheck;
		wanderState = CurrentState.Idle;
		currentWanderPoint = Vector3.zero;
		if (currentPath == null)
		{
			currentPath = new List<Vector3>();
		}
		cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
		ChooseDestination();
	}

	public override void behaviorEnd()
	{
		motionStoppedMoving();
		base.behaviorEnd();
		if (currentPath != null)
		{
			currentPath.Clear();
		}
	}

	public override void behaviorUpdate()
	{
		if (Paused)
		{
			return;
		}
		base.behaviorUpdate();
		Vector3 position = owningObject.transform.position;
		position.y += 0.35f;
		if (currentPath != null && currentPath.Count > 0)
		{
			DebugDrawPath(currentPath);
		}
		else if (currentWanderPoint != Vector3.zero)
		{
			Debug.DrawLine(position, currentWanderPoint, Color.green);
		}
		float magnitude = (owningObject.transform.position - previousPosition).magnitude;
		previousPosition = owningObject.transform.position;
		if (magnitude > 0.05f)
		{
			lastTimeMoved = Time.time;
		}
		if (wanderState == CurrentState.Wandering)
		{
			if (currentPath != null && currentPathNodeIndex < currentPath.Count)
			{
				float magnitude2 = (currentPath[currentPathNodeIndex] - position).magnitude;
				if (magnitude2 < 0.1f || (magnitude < 0.05f && Time.time - lastTimeMoved > 0.5f))
				{
					currentPathNodeIndex++;
					if (currentPathNodeIndex < currentPath.Count)
					{
						charGlobals.motionController.setDestination(currentPath[currentPathNodeIndex]);
						previousPosition = owningObject.transform.position;
					}
					else
					{
						wanderState = CurrentState.Idle;
						charGlobals.motionController.setDestination(owningObject.transform.position);
					}
				}
			}
			else if (currentWanderPoint != Vector3.zero && Time.time - lastTimeMoved > 0.5f)
			{
				wanderState = CurrentState.Idle;
				charGlobals.motionController.setDestination(owningObject.transform.position);
				CspUtils.DebugLog("Stuck!");
			}
		}
		if (IsDone() && wanderEndCallback != null)
		{
			wanderEndCallback();
		}
	}

	protected virtual bool IsDone()
	{
		if (wanderState == CurrentState.Idle && TryToFaceCamera())
		{
			return true;
		}
		return false;
	}

	protected virtual bool TryToFaceCamera()
	{
		Vector3 vector = cameraObject.transform.position - owningObject.transform.position;
		vector.y = 0f;
		if (Vector3.Angle(owningObject.transform.forward, vector) > 5f)
		{
			Vector3 forward = Vector3.RotateTowards(owningObject.transform.forward, vector, charGlobals.motionController.rotateSpeed * ((float)Math.PI / 180f) * Time.deltaTime, 1000f);
			owningObject.transform.rotation = Quaternion.LookRotation(forward);
			return false;
		}
		return true;
	}

	protected virtual void ChooseDestination()
	{
		if (pathfinder != null && pathfinder.PickRandomDestination(owningObject.transform.position, out currentWanderPoint))
		{
			DebugDrawClearPath(currentPath);
			List<PathNode> list = pathfinder.FindPath(owningObject.transform.position, currentWanderPoint, true, obstacleCheckCallback);
			if (list != null)
			{
				currentPath.Clear();
				foreach (PathNode item2 in list)
				{
					Vector3 position = item2.transform.position;
					float x = position.x;
					Vector3 position2 = item2.transform.position;
					float y = position2.y + 0.35f;
					Vector3 position3 = item2.transform.position;
					Vector3 item = new Vector3(x, y, position3.z);
					currentPath.Add(item);
				}
				if (currentPath.Count > 0)
				{
					currentPathNodeIndex = 0;
					charGlobals.motionController.setDestination(currentPath[currentPathNodeIndex]);
					wanderState = CurrentState.Wandering;
					previousPosition = owningObject.transform.position;
					return;
				}
			}
		}
		currentPathNodeIndex = 0;
		currentPath.Add(GetRandomPoint());
		charGlobals.motionController.setDestination(currentPath[currentPathNodeIndex]);
		wanderState = CurrentState.Wandering;
		previousPosition = owningObject.transform.position;
	}

	protected Vector3 GetRandomPoint()
	{
		Vector3 vector = owningObject.transform.forward * 3f;
		Vector3 position = owningObject.transform.position;
		Vector3 result = owningObject.transform.position;
		position.y += PathNodeBase.StepHeight;
		int num = UnityEngine.Random.Range(0, 360);
		for (int i = 0; i < 360; i += 10)
		{
			Vector3 vector2 = position + vector;
			vector2.y += PathNodeBase.StepHeight;
			RaycastHit hitInfo;
			if (Physics.Linecast(position, vector2, out hitInfo, 4694016) && hitInfo.collider.gameObject != owningObject)
			{
				num += i;
				Quaternion rotation = Quaternion.AngleAxis(num, Vector3.up);
				vector = rotation * vector;
				continue;
			}
			result = vector2;
			break;
		}
		return result;
	}

	public override void destinationChanged()
	{
		charGlobals.behaviorManager.endBehavior();
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

	private void DebugDrawClearPath(List<Vector3> path)
	{
		if (path != null)
		{
			for (int i = 0; i < path.Count; i++)
			{
			}
		}
	}
}
