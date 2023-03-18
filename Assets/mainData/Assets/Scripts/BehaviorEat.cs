using System;
using UnityEngine;

public class BehaviorEat : BehaviorBase
{
	public delegate void OnEatOver(GameObject objInteractedWith);

	private const float EAT_OBJECT_SCALE_FACTOR = 1f;

	private const float TIME_TO_GRAB = 0.3f;

	private const float TIME_TO_EAT = 1.75f;

	private const float TIME_TO_MOVEFOOD = 0.22f;

	private const string EAT_EMOTE_SEQUENCE = "emote_eat_sequence";

	protected bool allowInput;

	protected bool eating;

	protected bool attached;

	protected bool doneEating;

	protected Vector3 foodStartPosition;

	protected OnEatOver DoneCallback;

	public override void behaviorBegin()
	{
		base.behaviorBegin();
	}

	public void Initialize(GameObject eatTargetObject, bool allowInput, OnEatOver overCallback)
	{
		this.allowInput = allowInput;
		Initialize(eatTargetObject, overCallback);
	}

	public void Initialize(GameObject eatTargetObject, OnEatOver overCallback)
	{
		doneEating = false;
		eating = true;
		attached = false;
		DoneCallback = overCallback;
		if (animationComponent.GetClip("emote_eat") != null)
		{
			setTarget(eatTargetObject);
			if (!charGlobals.effectsList.TryOneShot("emote_eat_sequence", owningObject))
			{
				animationComponent["emote_eat"].wrapMode = WrapMode.Once;
				animationComponent.Play("emote_eat");
			}
			foodStartPosition = eatTargetObject.transform.position;
		}
		else
		{
			UnityEngine.Object.Destroy(eatTargetObject);
		}
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		GameObject target = getTarget();
		float num = Time.time - startTime;
		if (eating && !animationComponent.IsPlaying("emote_eat"))
		{
			if (DoneCallback != null)
			{
				if (target != null && AppShell.Instance != null && AppShell.Instance.Profile != null && animationComponent.gameObject == GameController.GetController().LocalPlayer)
				{
					CspUtils.DebugLog("food 2 " + animationComponent.gameObject);
					AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "eat_food", 1, -10000, -10000, target.name, string.Empty);
				}
				doneEating = true;
				DoneCallback(getTarget());
			}
			charGlobals.behaviorManager.endBehavior();
			return;
		}
		if (!attached && num >= 0.22f && num < 0.3f)
		{
			Transform transform = Utils.FindNodeInChildren(owningObject.transform, "fx_Rpalm");
			target.transform.position = Vector3.Lerp(foodStartPosition, transform.position, (num - 0.22f) / 0.08000001f);
		}
		else if (!attached && num >= 0.3f)
		{
			attached = true;
			if (target != null)
			{
				Transform transform2 = Utils.FindNodeInChildren(owningObject.transform, "fx_Rpalm");
				Utils.AttachGameObject(transform2.gameObject, target);
				target.transform.localPosition = Vector3.zero;
				target.transform.position = target.transform.position + transform2.up * 0.28f;
			}
		}
		if (num >= 1.75f && target != null)
		{
			if (AppShell.Instance != null && AppShell.Instance.Profile != null && animationComponent.gameObject == GameController.GetController().LocalPlayer)
			{
				CspUtils.DebugLog("food 1 " + animationComponent.gameObject);
				AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "eat_food", 1, -10000, -10000, target.name, string.Empty);
			}
			doneEating = true;
			if (DoneCallback != null)
			{
				DoneCallback(getTarget());
			}
			setTarget(null);
		}
	}

	public override bool allowUserInput()
	{
		return doneEating && allowInput;
	}

	public override void destinationChanged()
	{
		base.destinationChanged();
		charGlobals.behaviorManager.endBehavior();
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
