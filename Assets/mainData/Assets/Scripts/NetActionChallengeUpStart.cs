using UnityEngine;

public class NetActionChallengeUpStart : NetActionCelebrate
{
	public NetActionChallengeUpStart()
	{
	}

	public NetActionChallengeUpStart(GameObject initObject)
		: base(initObject)
	{
	}

	public override void Process(CharacterGlobals character)
	{
		if (!(character.behaviorManager.getBehavior() is BehaviorChallengeUp))
		{
			BehaviorChallengeUp behaviorChallengeUp = character.behaviorManager.requestChangeBehavior<BehaviorChallengeUp>(true);
			if (behaviorChallengeUp != null)
			{
				behaviorChallengeUp.Initialize(false);
			}
		}
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionChallengeUpStart;
	}

	public override string ToString()
	{
		return "Start " + base.ToString();
	}
}
