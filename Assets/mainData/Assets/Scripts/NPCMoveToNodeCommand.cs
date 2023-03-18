using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMoveToNodeCommand : NPCCommandBase
{
	public NPCPathNode targetNode;

	public NPCPathNode currentNode;

	public bool run;

	public float moveSpeed = 2.6f;

	public bool ignorePathingChecks;

	private BehaviorApproach moveBeh;

	private float retryDuration = 5f;

	private float retryRetries = 3f;

	private float retryCount;

	private float retryTimeStarted;

	private bool retryMode;

	private static int pathingDetectionFlags = 802655721;

	private static RaycastHit lastHitInfo;

	public NPCMoveToNodeCommand()
	{
		type = NPCCommandTypeEnum.MoveToNode;
	}

	public override void Start()
	{
		Start(null);
	}

	public override void Start(params object[] initValues)
	{
		Resume();
	}

	public override void Suspend()
	{
		base.Suspend();
		if (moveBeh != null)
		{
			behaviorManager.cancelBehavior();
			behaviorManager.endBehavior();
		}
	}

	public override void Resume()
	{
		base.Resume();
		if (targetNode == null)
		{
			CspUtils.DebugLog("Target Node is null. Can't move to null.");
			isDone = true;
			return;
		}
		float num = Mathf.Max(targetNode.radius - gameObject.GetComponent<CharacterController>().radius, 0.1f);
		if ((targetNode.transform.position - gameObject.transform.position).sqrMagnitude <= num * num)
		{
			CoroutineContainer component = gameObject.GetComponent<CoroutineContainer>();
			component = (component ?? gameObject.AddComponent<CoroutineContainer>());
			component.StartCoroutine(CompleteMoveOnNextFrame(gameObject));
			return;
		}
		int num2 = 0;
		bool flag = false;
		moveBeh = (behaviorManager.requestChangeBehavior(typeof(BehaviorApproach), false) as BehaviorApproach);
		if (moveBeh == null)
		{
			return;
		}
		Vector3 position = gameObject.transform.position;
		Vector3 position2 = targetNode.transform.position;
		Vector3 vector = Vector3.zero;
		while (num2++ < 10 && !flag)
		{
			vector = position2 + VectorConverter.Convert(Random.insideUnitCircle) * num;
			if (ignorePathingChecks || ClearLineTo(position, currentNode.linecastHeight, position2, targetNode.linecastHeight))
			{
				flag = true;
				break;
			}
			string value = "Path Fail (" + position + ") to (" + vector + ") Hit?: " + lastHitInfo.collider.gameObject.name;
			manager.ProblemHistory.Add(new KeyValuePair<int, string>(commandSet, value));
		}
		if (!flag)
		{
			Vector3 normalized = (position2 - position).normalized;
			Vector3 normalized2 = Vector3.Cross(normalized, new Vector3(0f, 1f, 0f)).normalized;
			Vector3 vector2 = position2 + normalized2 * num;
			if (ClearLineTo(position, vector2, targetNode.linecastHeight))
			{
				vector = vector2;
				flag = true;
				manager.ProblemHistory.Add(new KeyValuePair<int, string>(commandSet, " Found usable path to next node"));
			}
			else
			{
				Vector3 vector3 = position2 - normalized2 * num;
				if (ClearLineTo(position, vector3, targetNode.linecastHeight))
				{
					vector = vector3;
					flag = true;
					manager.ProblemHistory.Add(new KeyValuePair<int, string>(commandSet, " Found usable path to next node"));
				}
			}
			if (!flag)
			{
				manager.ProblemHistory.Add(new KeyValuePair<int, string>(commandSet, " No path found. Retrying " + retryCount));
				retryMode = true;
				retryTimeStarted = Time.time;
				return;
			}
		}
		retryMode = false;
		retryCount = 0f;
		moveBeh.SetNPCAnimMode(run, moveSpeed);
		moveBeh.Initialize(vector, targetNode.transform.rotation, false, moveCompleted, moveFailed, 0f, 0f, false, true);
	}

	public override void Completed()
	{
		base.Completed();
	}

	public override NPCCommandResultEnum Update()
	{
		if (retryMode)
		{
			if (retryCount >= retryRetries)
			{
				isDone = true;
				manager.ProblemHistory.Add(new KeyValuePair<int, string>(commandSet, " Giving up trying to get to next path."));
				return NPCCommandResultEnum.Failed;
			}
			if (Time.time - retryTimeStarted > retryDuration)
			{
				retryMode = false;
				retryCount += 1f;
				Resume();
				return NPCCommandResultEnum.InProgress;
			}
		}
		return (!isDone) ? NPCCommandResultEnum.InProgress : NPCCommandResultEnum.Completed;
	}

	private IEnumerator CompleteMoveOnNextFrame(GameObject objMoving)
	{
		yield return 0;
		moveCompleted(objMoving);
	}

	private void moveCompleted(GameObject objMoving)
	{
		if (!aiController.ConfigureCommandsFromNode(targetNode, true))
		{
			NPCMoveToNextNodeCommand command = new NPCMoveToNextNodeCommand();
			manager.AddCommand(command);
		}
		isDone = true;
	}

	private void moveFailed(GameObject objMoving)
	{
		manager.ProblemHistory.Add(new KeyValuePair<int, string>(commandSet, " Failed to complete approach; teleporting"));
	}

	public static bool ClearLineTo(Vector3 start, Vector3 end)
	{
		if (Physics.Linecast(start, end, out lastHitInfo, pathingDetectionFlags))
		{
			return false;
		}
		return true;
	}

	public static bool ClearLineTo(Vector3 start, Vector3 end, float height)
	{
		start += new Vector3(0f, height, 0f);
		end += new Vector3(0f, height, 0f);
		if (Physics.Linecast(start, end, out lastHitInfo, pathingDetectionFlags))
		{
			return false;
		}
		return true;
	}

	public static bool ClearLineTo(Vector3 start, float startHeight, Vector3 end, float endHeight)
	{
		start += new Vector3(0f, startHeight, 0f);
		end += new Vector3(0f, endHeight, 0f);
		if (Physics.Linecast(start, end, out lastHitInfo, pathingDetectionFlags))
		{
			return false;
		}
		return true;
	}

	public static bool ClearLineTo(NPCPathNode start, NPCPathNode end)
	{
		Vector3 position = start.gameObject.transform.position;
		Vector3 position2 = end.gameObject.transform.position;
		position += new Vector3(0f, start.linecastHeight, 0f);
		position2 += position2 + new Vector3(0f, end.linecastHeight, 0f);
		if (Physics.Linecast(position, position2, out lastHitInfo, pathingDetectionFlags))
		{
			return false;
		}
		return true;
	}

	public override string ToString()
	{
		return base.ToString() + " (" + ((!(targetNode != null)) ? "null" : targetNode.gameObject.name) + ") Run? " + run;
	}
}
