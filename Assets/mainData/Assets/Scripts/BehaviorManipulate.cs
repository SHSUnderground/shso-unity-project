using System;
using UnityEngine;

public class BehaviorManipulate : BehaviorBase
{
	private const float MANIPULATE_BLEND_TIME = 0.5f;

	private const string MANIPULATE_EMOTE_SEQUENCE = "emote_manipulate_sequence";

	private const string MANIPULATE_ANIMATION = "emote_manipulate_loop";

	protected bool manipulating;

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		manipulating = true;
		if (animationComponent.GetClip("emote_manipulate_loop") != null)
		{
			animationComponent["emote_manipulate_loop"].wrapMode = WrapMode.Loop;
			animationComponent.Rewind("emote_manipulate_loop");
			animationComponent.CrossFade("emote_manipulate_loop", 0.5f, PlayMode.StopAll);
			manipulating = true;
			GameObject gameObject = charGlobals.effectsList.GetEffectSequencePrefabByName("emote_manipulate_sequence") as GameObject;
			if (gameObject != null)
			{
				GameObject child = UnityEngine.Object.Instantiate(gameObject) as GameObject;
				Utils.AttachGameObject(charGlobals.gameObject, child);
			}
		}
		else
		{
			CspUtils.DebugLog("No manipulate animation, unable to perform manipulate behavior.");
			charGlobals.behaviorManager.endBehavior();
		}
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
	}

	public override void behaviorEnd()
	{
		if (manipulating)
		{
			if (animationComponent["emote_manipulate_loop"] != null)
			{
				animationComponent["emote_manipulate_loop"].wrapMode = WrapMode.Once;
				animationComponent.Blend("emote_manipulate_loop", 0f, 0.5f);
			}
			manipulating = false;
		}
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}
}
