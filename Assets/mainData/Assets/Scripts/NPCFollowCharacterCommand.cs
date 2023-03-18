using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFollowCharacterCommand : NPCCommandBase
{
	public bool run = true;

	public float moveSpeed = 6.3f;

	public bool ignorePathingChecks;

	private BehaviorApproach moveBeh;

	private float retryDuration = 0.5f;

	private float retryRetries = 3f;

	private float retryCount;

	private float retryTimeStarted;

	private bool retryMode;

	private int linecastOffset = 15;

	private float lastPathCheck;

	private static int pathingDetectionFlags = 802655721;

	private static RaycastHit lastHitInfo;

	public NPCFollowCharacterCommand()
	{
		type = NPCCommandTypeEnum.FollowCharacter;
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
		if (target == null)
		{
			CspUtils.DebugLog("NPC Follow Character target is null");
			isDone = true;
			return;
		}
		float num = Mathf.Max(gameObject.GetComponent<CharacterController>().radius, 1f);
		if ((target.transform.position - gameObject.transform.position).sqrMagnitude <= num * num)
		{
			CoroutineContainer component = gameObject.GetComponent<CoroutineContainer>();
			component = (component ?? gameObject.AddComponent<CoroutineContainer>());
			component.StartCoroutine(CompleteMoveOnNextFrame(gameObject));
			return;
		}
		int num2 = 0;
		bool flag = false;
		moveBeh = (behaviorManager.requestChangeBehavior(typeof(BehaviorApproach), false) as BehaviorApproach);
		if (moveBeh != null)
		{
			Vector3 position = gameObject.transform.position;
			Vector3 position2 = target.transform.position;
			Vector3 vector = Vector3.zero;
			while (num2++ < 10 && !flag)
			{
				vector = position2 + VectorConverter.Convert(Random.insideUnitCircle) * num;
				if (ignorePathingChecks || ClearLineTo(position, linecastOffset, position2, linecastOffset))
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
				if (ClearLineTo(position, vector2, linecastOffset))
				{
					vector = vector2;
					flag = true;
					manager.ProblemHistory.Add(new KeyValuePair<int, string>(commandSet, " Found usable path to next node"));
				}
				else
				{
					Vector3 vector3 = position2 - normalized2 * num;
					if (ClearLineTo(position, vector3, linecastOffset))
					{
						vector = vector3;
						flag = true;
						manager.ProblemHistory.Add(new KeyValuePair<int, string>(commandSet, " Found usable path to next node"));
					}
				}
				if (!flag)
				{
					manager.ProblemHistory.Add(new KeyValuePair<int, string>(commandSet, " No path found. Retrying " + retryCount));
					CspUtils.DebugLog("Can't find a clear path from: " + position + " to the next node: " + target.transform.position);
					retryMode = true;
					retryTimeStarted = Time.time;
					return;
				}
			}
			float sqrMagnitude = (target.transform.position - gameObject.transform.position).sqrMagnitude;
			if (sqrMagnitude > 70f)
			{
				CharacterMotionController characterMotionController = target.GetComponent(typeof(CharacterMotionController)) as CharacterMotionController;
				if (characterMotionController.IsOnGround())
				{
					CspUtils.DebugLog("distance is " + sqrMagnitude + " teleporting");
					gameObject.transform.position = target.transform.position;
				}
				else
				{
					CspUtils.DebugLog("cannot teleport - target is not on the ground");
				}
			}
			else if (sqrMagnitude > 5f)
			{
				run = true;
			}
			else
			{
				run = false;
			}
			retryMode = false;
			retryCount = 0f;
			moveBeh.SetNPCAnimMode(true, moveSpeed);
			moveBeh.Initialize(vector, target.transform.rotation, false, moveCompleted, moveFailed, 0f, 0f, false, true);
			lastPathCheck = Time.time;
		}
		else
		{
			CspUtils.DebugLog("NPCFollowCharacterCommand - Behavior Manager is nulL!");
		}
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
				CspUtils.DebugLog("exceeded retry count to path to target, teleporting");
				CharacterMotionController characterMotionController = target.GetComponent(typeof(CharacterMotionController)) as CharacterMotionController;
				if (characterMotionController.IsOnGround())
				{
					gameObject.transform.position = target.transform.position;
					retryMode = false;
				}
				return NPCCommandResultEnum.InProgress;
			}
			if (Time.time - retryTimeStarted > retryDuration)
			{
				retryMode = false;
				retryCount += 1f;
				Resume();
				return NPCCommandResultEnum.InProgress;
			}
		}
		else if (Time.time - lastPathCheck > 0.5f)
		{
			ShsCharacterController shsCharacterController = gameObject.GetComponent(typeof(ShsCharacterController)) as ShsCharacterController;
			CspUtils.DebugLog("NPCFollow - update path " + shsCharacterController.height + " " + shsCharacterController.radius);
			Resume();
		}
		return (!isDone) ? NPCCommandResultEnum.InProgress : NPCCommandResultEnum.Completed;
	}

	private IEnumerator CompleteMoveOnNextFrame(GameObject objMoving)
	{
		yield return 0;
		NPCWaitForCharacterMoveCommand command = new NPCWaitForCharacterMoveCommand();
		command.target = target;
		manager.AddCommand(command, true);
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
		return base.ToString() + " (" + ((!(target != null)) ? "null" : target.gameObject.name) + ") Run? " + run;
	}
}
