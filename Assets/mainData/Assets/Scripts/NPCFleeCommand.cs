using UnityEngine;

public class NPCFleeCommand : NPCCommandBase
{
	public const float PATH_TEST_HEIGHT = 0.25f;

	public const float FLEE_DISTANCE = 5f;

	private BehaviorApproach moveBeh;

	public NPCFleeCommand()
	{
		type = NPCCommandTypeEnum.Flee;
		interruptable = false;
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
			moveBeh.behaviorCancel();
		}
	}

	public override void Resume()
	{
		base.Resume();
		if (isDone)
		{
			return;
		}
		if (target == null)
		{
			CspUtils.DebugLog("Target is null. Can't flee from null.");
			isDone = true;
			return;
		}
		bool flag = false;
		int num = 0;
		moveBeh = (behaviorManager.requestChangeBehavior(typeof(BehaviorApproach), false) as BehaviorApproach);
		if (moveBeh == null)
		{
			return;
		}
		Vector3 position = gameObject.transform.position;
		Vector3 position2 = target.transform.position;
		Vector3 normalized = (position - position2).normalized;
		Vector3 b = normalized * 5f;
		if (ClearLineTo(position, position + b, 0.25f))
		{
			flag = true;
		}
		else
		{
			while (num++ < 10 && !flag)
			{
				b = Random.insideUnitSphere;
				b = new Vector3(b.x, normalized.y, b.z).normalized * 5f;
				if (ClearLineTo(position, position + b, 0.25f))
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			moveBeh.SetNPCAnimMode(true, 2.6f);
			moveBeh.Initialize(position + b, gameObject.transform.rotation, false, moveCompleted, moveCompleted, 0f, 0f, false, true);
		}
		else
		{
			CspUtils.DebugLog("Cant find a path to flee from emoting NPC. Choosing to be scared instead");
			startPostFleeCower();
		}
	}

	public bool ClearLineTo(Vector3 start, Vector3 end, float height)
	{
		start += new Vector3(0f, height, 0f);
		end += new Vector3(0f, height, 0f);
		RaycastHit hitInfo;
		if (Physics.Linecast(start, end, out hitInfo, -268976151))
		{
			return false;
		}
		return true;
	}

	public override void Completed()
	{
		base.Completed();
	}

	public override NPCCommandResultEnum Update()
	{
		return (!isDone) ? NPCCommandResultEnum.InProgress : NPCCommandResultEnum.Completed;
	}

	private void moveCompleted(GameObject objMoving)
	{
		startPostFleeCower();
	}

	private void startPostFleeCower()
	{
		NPCEmoteCommand nPCEmoteCommand = new NPCEmoteCommand();
		nPCEmoteCommand.target = target;
		nPCEmoteCommand.emoteList = "emote_scared";
		isDone = true;
		interruptable = true;
		manager.AddCommand(nPCEmoteCommand, true);
	}

	public override string ToString()
	{
		return type.ToString() + " (Distance:" + 5f + ")";
	}
}
