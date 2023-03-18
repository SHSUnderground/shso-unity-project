using System;

[Serializable]
public class NPCEmoteCommand : NPCCommandBase
{
	public string emoteList = string.Empty;

	public NPCEmoteCommand()
	{
		interruptable = false;
		type = NPCCommandTypeEnum.Emote;
	}

	public override void Start()
	{
		Start(null);
	}

	public override void Start(params object[] initValues)
	{
		base.Start(initValues);
		if (string.IsNullOrEmpty(emoteList) || !AIControllerNPC.ReactionTable.Contains(emoteList))
		{
			isDone = true;
			return;
		}
		BehaviorAnimate behaviorAnimate = behaviorManager.requestChangeBehavior(typeof(BehaviorAnimate), false) as BehaviorAnimate;
		behaviorAnimate.Initialize(emoteList, delegate
		{
			isDone = true;
		});
	}

	public override NPCCommandResultEnum Update()
	{
		return (!isDone) ? NPCCommandResultEnum.InProgress : NPCCommandResultEnum.Completed;
	}

	public override string ToString()
	{
		return type.ToString() + ": (" + emoteList + ")";
	}
}
