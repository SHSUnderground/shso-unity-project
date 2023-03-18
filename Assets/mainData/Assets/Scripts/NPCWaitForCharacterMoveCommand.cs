using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWaitForCharacterMoveCommand : NPCCommandBase
{
	public bool run = true;

	public float moveSpeed = 6.3f;

	public bool ignorePathingChecks;

	private float lastPathCheck;

	private static int pathingDetectionFlags = 802655721;

	public NPCWaitForCharacterMoveCommand()
	{
		type = NPCCommandTypeEnum.WaitForCharacterMove;
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
	}

	public override void Resume()
	{
		base.Resume();
	}

	public override void Completed()
	{
		base.Completed();
	}

	public override NPCCommandResultEnum Update()
	{
		CspUtils.DebugLog("WaitForChar - Update");
		if (target == null)
		{
			CspUtils.DebugLog("   TARGET IS NULL");
			Object.Destroy(gameObject);
			return NPCCommandResultEnum.Completed;
		}
		if (!target.active)
		{
			CspUtils.DebugLog("   TARGET IS !ACTIVE");
			Object.Destroy(gameObject);
			return NPCCommandResultEnum.Completed;
		}
		if (Time.time - lastPathCheck > 0.5f)
		{
			lastPathCheck = Time.time;
			float sqrMagnitude = (target.transform.position - gameObject.transform.position).sqrMagnitude;
			if (sqrMagnitude > 10f)
			{
				CspUtils.DebugLog("TARGET MOVED, FOLLOW " + sqrMagnitude + " ");
				NPCFollowCharacterCommand nPCFollowCharacterCommand = new NPCFollowCharacterCommand();
				nPCFollowCharacterCommand.target = target;
				manager.AddCommand(nPCFollowCharacterCommand, true);
				return NPCCommandResultEnum.InProgress;
			}
			return NPCCommandResultEnum.InProgress;
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
		CspUtils.DebugLog("NPC FOLLOW COMPLETE - DOING IT AGAIN");
		Resume();
	}

	private void moveFailed(GameObject objMoving)
	{
		manager.ProblemHistory.Add(new KeyValuePair<int, string>(commandSet, " Failed to complete approach; teleporting"));
	}

	public static bool ClearLineTo(Vector3 start, Vector3 end)
	{
		RaycastHit hitInfo;
		if (Physics.Linecast(start, end, out hitInfo, pathingDetectionFlags))
		{
			return false;
		}
		return true;
	}

	public static bool ClearLineTo(Vector3 start, Vector3 end, float height)
	{
		start += new Vector3(0f, height, 0f);
		end += new Vector3(0f, height, 0f);
		RaycastHit hitInfo;
		if (Physics.Linecast(start, end, out hitInfo, pathingDetectionFlags))
		{
			return false;
		}
		return true;
	}

	public static bool ClearLineTo(Vector3 start, float startHeight, Vector3 end, float endHeight)
	{
		start += new Vector3(0f, startHeight, 0f);
		end += new Vector3(0f, endHeight, 0f);
		RaycastHit hitInfo;
		if (Physics.Linecast(start, end, out hitInfo, pathingDetectionFlags))
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
		RaycastHit hitInfo;
		if (Physics.Linecast(position, position2, out hitInfo, pathingDetectionFlags))
		{
			return false;
		}
		return true;
	}

	public override string ToString()
	{
		return base.ToString() + " (" + ((!(target != null)) ? "null" : target.gameObject.name) + ") Run? " + run;
	}
}
