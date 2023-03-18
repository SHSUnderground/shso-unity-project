using UnityEngine;

public class NetActionChallengeUpEnd : NetActionCelebrate
{
	public NetActionChallengeUpEnd()
	{
	}

	public NetActionChallengeUpEnd(GameObject initObject)
		: base(initObject)
	{
	}

	public override void Process(CharacterGlobals character)
	{
		if (character.behaviorManager.getBehavior() is BehaviorChallengeUp)
		{
			character.behaviorManager.endBehavior();
		}
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionChallengeUpEnd;
	}

	public override string ToString()
	{
		return "End " + base.ToString();
	}
}
