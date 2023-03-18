using UnityEngine;

public class NetActionLeveledUpEnd : NetActionCelebrate
{
	public NetActionLeveledUpEnd()
	{
	}

	public NetActionLeveledUpEnd(GameObject initObject)
		: base(initObject)
	{
	}

	public override void Process(CharacterGlobals character)
	{
		if (character.behaviorManager.getBehavior() is BehaviorLeveledUp)
		{
			character.behaviorManager.endBehavior();
		}
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionLeveledUpEnd;
	}

	public override string ToString()
	{
		return "End " + base.ToString();
	}
}
