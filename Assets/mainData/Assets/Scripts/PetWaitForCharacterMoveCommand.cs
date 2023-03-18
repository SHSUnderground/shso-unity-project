using UnityEngine;

public class PetWaitForCharacterMoveCommand : PetCommandBase
{
	public bool run = true;

	public float moveSpeed = 6.3f;

	public bool ignorePathingChecks;

	private BehaviorIdlePet idleBeh;

	private float lastPathCheck;

	private float lastFidget;

	public float fidgetDelay = -1f;

	public int pigeonDeathHack;

	public PetFidgetData fidgetData;

	public PetWaitForCharacterMoveCommand()
	{
		type = PetCommandTypeEnum.WaitForCharacterMove;
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
		lastFidget = Time.time;
		idleBeh = (behaviorManager.requestChangeBehavior(typeof(BehaviorIdlePet), false) as BehaviorIdlePet);
		idleBeh.setAnimName(manager.petData.customIdleAnim);
		if (pigeonDeathHack == 1)
		{
			gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
			gameObject.transform.rotation = gameObject.transform.rotation * new Quaternion(0f, 0f, 1f, 0f) * new Quaternion(0f, 1f, 0f, 0f);
		}
	}

	public override void Completed()
	{
		base.Completed();
	}

	public override PetCommandResultEnum Update()
	{
		if (target == null)
		{
			CspUtils.DebugLog("PetWaitForChar - target is null, removing");
			Object.Destroy(gameObject);
			return PetCommandResultEnum.Completed;
		}
		if (!target.active)
		{
			CspUtils.DebugLog("PetWaitForChar - target is not active, removing");
			Object.Destroy(gameObject);
			return PetCommandResultEnum.Completed;
		}
		if (Time.time - lastPathCheck > 0.5f)
		{
			lastPathCheck = Time.time;
			float sqrMagnitude = (target.transform.position - gameObject.transform.position).sqrMagnitude;
			if (sqrMagnitude > 10f)
			{
				PetFollowCharacterCommand petFollowCharacterCommand = new PetFollowCharacterCommand();
				petFollowCharacterCommand.pigeonDeathHack = pigeonDeathHack;
				petFollowCharacterCommand.target = target;
				manager.AddCommand(petFollowCharacterCommand, true);
				return PetCommandResultEnum.InProgress;
			}
		}
		if (fidgetData != null && Time.time - lastFidget > fidgetDelay)
		{
			if (fidgetData.fidgetEmoteID > 0)
			{
				BehaviorEmote behaviorEmote = behaviorManager.requestChangeBehavior(typeof(BehaviorEmote), false) as BehaviorEmote;
				behaviorEmote.Initialize((sbyte)fidgetData.fidgetEmoteID, false, delegate
				{
					Resume();
				});
			}
			else if (fidgetData.fidgetAnimName != string.Empty)
			{
				BehaviorAnimate behaviorAnimate = behaviorManager.requestChangeBehavior(typeof(BehaviorAnimate), false) as BehaviorAnimate;
				behaviorAnimate.Initialize(fidgetData.fidgetAnimName, delegate
				{
					Resume();
				});
			}
			if (fidgetData.fidgetMassEmoteID != null)
			{
				ForceEmoteMessage forceEmoteMessage = new ForceEmoteMessage(new GoNetId(GoNetId.PLAYER_ID_FLAG, AppShell.Instance.ServerConnection.GetGameUserId()));
				forceEmoteMessage.emoteID = int.Parse(fidgetData.fidgetMassEmoteID);
				AppShell.Instance.ServerConnection.SendGameMsg(forceEmoteMessage);
			}
			if (fidgetData.fidgetPlayerEmoteID != null)
			{
				AppShell.Instance.EventMgr.Fire(this, new EmoteMessage(GameController.GetController().LocalPlayer, (sbyte)int.Parse(fidgetData.fidgetPlayerEmoteID)));
			}
			if (fidgetData.fidgetSoundName != string.Empty)
			{
			}
			lastFidget = Time.time;
			return PetCommandResultEnum.InProgress;
		}
		return (!isDone) ? PetCommandResultEnum.InProgress : PetCommandResultEnum.Completed;
	}

	public override string ToString()
	{
		return base.ToString() + " (" + ((!(target != null)) ? "null" : target.gameObject.name) + ") Run? " + run;
	}
}
