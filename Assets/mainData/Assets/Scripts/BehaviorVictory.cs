using System;
using UnityEngine;

internal class BehaviorVictory : BehaviorBase
{
	private const string idleAnimName = "movement_idle";

	private float timeToNextCheer;

	private string[] victoryNames = new string[5]
	{
		"cheer",
		"clap",
		"approve",
		"dance",
		"pose"
	};

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		animationComponent.CrossFade("movement_idle");
		GetNextCheerTime();
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		timeToNextCheer -= Time.deltaTime;
		if (!(timeToNextCheer <= 0f))
		{
			return;
		}
		GetNextCheerTime();
		BehaviorEmote behaviorEmote = charGlobals.behaviorManager.requestChangeBehavior(typeof(BehaviorEmote), false) as BehaviorEmote;
		if (behaviorEmote != null)
		{
			int num = UnityEngine.Random.Range(0, 5);
			EmotesDefinition.EmoteDefinition emoteByCommand = EmotesDefinition.Instance.GetEmoteByCommand(victoryNames[num]);
			if (emoteByCommand == null)
			{
				charGlobals.behaviorManager.endBehavior();
			}
			else if (!behaviorEmote.Initialize(emoteByCommand.id, true, false, 10f))
			{
				charGlobals.behaviorManager.endBehavior();
			}
		}
	}

	public override void behaviorLateUpdate()
	{
		base.behaviorLateUpdate();
	}

	public override void behaviorEnd()
	{
		base.behaviorEnd();
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		if (newBehaviorType == typeof(BehaviorEmote))
		{
			return true;
		}
		if (newBehaviorType == typeof(BehaviorLeveledUp))
		{
			return true;
		}
		return false;
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override void destinationChanged()
	{
		base.destinationChanged();
	}

	public override void motionJumped()
	{
		base.motionJumped();
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override bool useMotionControllerRotate()
	{
		return false;
	}

	public override bool useMotionControllerGravity()
	{
		return false;
	}

	public void StartNextCheer()
	{
		timeToNextCheer = 0f;
	}

	protected void GetNextCheerTime()
	{
		timeToNextCheer = UnityEngine.Random.Range(1f, 3f);
	}
}
