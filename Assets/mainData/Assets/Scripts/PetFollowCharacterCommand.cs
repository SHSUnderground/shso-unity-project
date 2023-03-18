using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetFollowCharacterCommand : PetCommandBase
{
	public bool run = true;

	public float moveSpeed = 6.3f;

	public bool ignorePathingChecks;

	private BehaviorApproachPet moveBeh;

	private float retryDuration = 0.5f;

	private float retryRetries = 3f;

	private float retryCount;

	private float retryTimeStarted;

	private bool retryMode;

	private int linecastOffset = 15;

	public int pigeonDeathHack;

	private float lastPathCheck;

	private bool forceComplete;

	private static int pathingDetectionFlags = 802655721;

	private static RaycastHit lastHitInfo;

	public PetFollowCharacterCommand()
	{
		type = PetCommandTypeEnum.FollowCharacter;
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
			CspUtils.DebugLog("Pet Follow Character target is null");
			isDone = true;
			return;
		}
		float num = Mathf.Max(gameObject.GetComponent<CharacterController>().radius, 2f);
		if ((target.transform.position - gameObject.transform.position).sqrMagnitude <= num * num)
		{
			forceComplete = true;
			return;
		}
		int num2 = 0;
		bool flag = false;
		base.behaviorManager.runAnimOverride = manager.petData.customRunAnim;
		base.behaviorManager.idleAnimOverride = manager.petData.customIdleAnim;
		moveBeh = (base.behaviorManager.requestChangeBehavior(typeof(BehaviorApproachPet), false) as BehaviorApproachPet);
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
					retryMode = true;
					retryTimeStarted = Time.time;
					return;
				}
			}
			BehaviorManager behaviorManager = target.GetComponent(typeof(BehaviorManager)) as BehaviorManager;
			float sqrMagnitude = (target.transform.position - gameObject.transform.position).sqrMagnitude;
			if (sqrMagnitude > 70f)
			{
				CharacterMotionController characterMotionController = target.GetComponent(typeof(CharacterMotionController)) as CharacterMotionController;
				if (behaviorManager.currentBehaviorName != "BehaviorSpline" && characterMotionController.IsOnGround())
				{
					gameObject.transform.position = target.transform.position;
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
			moveBeh.setPetAnimMode(moveSpeed, manager.petData.customRunAnim, manager.petData.customIdleAnim);
			moveBeh.Initialize(vector, target.transform.rotation, false, moveCompleted, moveFailed, 0f, 0f, false, true);
			lastPathCheck = Time.time;
		}
		else
		{
			CspUtils.DebugLog("PetFollowCharacterCommand - Behavior Manager is nulL!");
		}
	}

	public override void Completed()
	{
		base.Completed();
	}

	public override PetCommandResultEnum Update()
	{
		if (forceComplete)
		{
			return PetCommandResultEnum.Completed;
		}
		if (pigeonDeathHack == 1)
		{
			gameObject.transform.rotation = gameObject.transform.rotation * new Quaternion(0f, 0f, 1f, 0f) * new Quaternion(0f, 1f, 0f, 0f);
		}
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
				return PetCommandResultEnum.InProgress;
			}
			if (Time.time - retryTimeStarted > retryDuration)
			{
				retryMode = false;
				retryCount += 1f;
				Resume();
				return PetCommandResultEnum.InProgress;
			}
		}
		else if (Time.time - lastPathCheck > 0.5f)
		{
			Resume();
		}
		return (!isDone) ? PetCommandResultEnum.InProgress : PetCommandResultEnum.Completed;
	}

	protected T GetInteractiveObject<T>(GameObject o) where T : Component
	{
		while (o != null)
		{
			T component = Utils.GetComponent<T>(o);
			if ((Object)component != (Object)null)
			{
				return component;
			}
			if (o.transform.parent == null)
			{
				break;
			}
			o = o.transform.parent.gameObject;
		}
		return (T)null;
	}

	private IEnumerator CompleteMoveOnNextFrame(GameObject objMoving)
	{
		yield return 0;
		PetWaitForCharacterMoveCommand command = new PetWaitForCharacterMoveCommand();
		command.target = target;
		manager.AddCommand(command, true);
	}

	private void moveCompleted(GameObject objMoving)
	{
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

	public override string ToString()
	{
		return base.ToString() + " (" + ((!(target != null)) ? "null" : target.gameObject.name) + ") Run? " + run;
	}
}
