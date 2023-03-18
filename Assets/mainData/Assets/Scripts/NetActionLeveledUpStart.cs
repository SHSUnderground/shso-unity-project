using UnityEngine;

public class NetActionLeveledUpStart : NetActionCelebrate
{
	public NetActionLeveledUpStart()
	{
	}

	public NetActionLeveledUpStart(GameObject initObject)
		: base(initObject)
	{
	}

	public override void Process(CharacterGlobals character)
	{
		if (!(character.behaviorManager.getBehavior() is BehaviorLeveledUp))
		{
			BehaviorLeveledUp behaviorLeveledUp = character.behaviorManager.requestChangeBehavior<BehaviorLeveledUp>(true);
			if (behaviorLeveledUp != null)
			{
				behaviorLeveledUp.Initialize(false);
			}
		}
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionLeveledUpStart;
	}

	public override string ToString()
	{
		return "Start " + base.ToString();
	}
}
