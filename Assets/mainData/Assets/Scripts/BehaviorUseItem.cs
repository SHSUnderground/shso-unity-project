using System;
using UnityEngine;

public class BehaviorUseItem : BehaviorBase
{
	private HqItem item;

	private int currentActionIndex;

	private float currentActionStartTime;

	private float currentActionDuration;

	private bool doneUsingItem;

	private UseInfo useInfo;

	private Use use;

	private bool playingStand;

	private string currentItemEffectSequence;

	private EffectSequence currentEffectSequence;

	private AIControllerHQ itemUser;

	protected OnBehaviorDone ItemUsed;

	public override bool Paused
	{
		get
		{
			return paused;
		}
		set
		{
			paused = value;
			if (currentEffectSequence != null)
			{
				currentEffectSequence.Paused = value;
			}
			if (item != null)
			{
				item.Paused = value;
			}
		}
	}

	public void Initialize(HqItem itemToUse, DockPoint dockPoint, OnBehaviorDone usedItemCallback)
	{
		item = itemToUse;
		ItemUsed = usedItemCallback;
		doneUsingItem = false;
		if (item.ItemDefinition != null && item.ItemDefinition.UseInfo != null)
		{
			useInfo = item.ItemDefinition.UseInfo;
			use = useInfo.GetUseByDockPointName(dockPoint.Name);
			if (useInfo == null || use == null)
			{
				CspUtils.DebugLog(owningObject.name + " is trying to use a dock point that has no use info! Name:" + dockPoint.Name);
				InterruptUse();
				return;
			}
		}
		if (use.PostureType == Use.Posture.sit)
		{
			Sit();
		}
		HqObject2 component = Utils.GetComponent<HqObject2>(item.gameObject);
		if (component != null)
		{
			HqController2.Instance.AIIsTakingOverItem(component);
			itemUser = Utils.GetComponent<AIControllerHQ>(owningObject);
		}
		if (use.ItemActions != null && use.ItemActions.Count > 0)
		{
			StartAction(currentActionIndex);
			currentActionStartTime = elapsedTime;
		}
	}

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		currentActionIndex = 0;
		doneUsingItem = false;
		playingStand = false;
	}

	public override void behaviorUpdate()
	{
		if (ShouldInterruptUse())
		{
			InterruptUse();
		}
		if (Paused)
		{
			return;
		}
		base.behaviorUpdate();
		if (item.gameObject.rigidbody != null && item.gameObject.rigidbody.velocity.sqrMagnitude > 0.1f)
		{
			InterruptUse();
		}
		if (playingStand && !animationComponent.IsPlaying("sit_stand"))
		{
			doneUsingItem = true;
		}
		if (!doneUsingItem && elapsedTime - currentActionStartTime >= currentActionDuration)
		{
			currentActionIndex++;
			NextAction();
		}
		if (!doneUsingItem && use.PostureType == Use.Posture.jump && charGlobals.motionController.IsOnGround())
		{
			Jump();
		}
		if (doneUsingItem)
		{
			if (ItemUsed != null)
			{
				ItemUsed(owningObject);
			}
		}
		else if (currentEffectSequence == null)
		{
			Idle();
		}
	}

	private void NextAction()
	{
		currentActionDuration = 0f;
		if (currentEffectSequence != null)
		{
			currentEffectSequence.Cancel();
			currentEffectSequence = null;
		}
		if (currentItemEffectSequence != null)
		{
			item.StopSequence(currentItemEffectSequence, owningObject);
			currentItemEffectSequence = null;
		}
		if (use.ItemActions != null && currentActionIndex < use.ItemActions.Count)
		{
			StartAction(currentActionIndex);
			currentActionStartTime = elapsedTime;
			if (use.ItemActions[currentActionIndex].UserSequence == null)
			{
				currentActionDuration = use.ItemActions[currentActionIndex].PadTime;
			}
		}
		else
		{
			InterruptUse();
		}
	}

	protected void InterruptUse()
	{
		if (!Paused && use.PostureType == Use.Posture.sit)
		{
			if (!playingStand)
			{
				Stand();
			}
			else if (!animationComponent.IsPlaying("sit_stand"))
			{
				doneUsingItem = true;
			}
		}
		else
		{
			doneUsingItem = true;
		}
	}

	public override void behaviorEnd()
	{
		base.behaviorEnd();
		if (currentEffectSequence != null && use != null && currentActionIndex < use.ItemActions.Count)
		{
			if (use.ItemActions[currentActionIndex].UserSequence != null && use.ItemActions[currentActionIndex].UserSequence.ItemSequence != null)
			{
				string name = use.ItemActions[currentActionIndex].UserSequence.ItemSequence.Name;
				if (name != null && item != null)
				{
					item.StopSequence(name, owningObject);
				}
			}
			use = null;
			currentEffectSequence.Cancel();
		}
		if (item != null)
		{
			HqTrigger component = Utils.GetComponent<HqTrigger>(item, Utils.SearchChildren);
			if (component != null)
			{
				component.TurnOff();
			}
		}
		item = null;
		useInfo = null;
	}

	public override void behaviorCancel()
	{
		base.behaviorCancel();
		doneUsingItem = true;
		if (currentEffectSequence != null)
		{
			currentEffectSequence.Cancel();
		}
		if (use.PostureType == Use.Posture.sit)
		{
			Stand();
		}
		else if (use.PostureType == Use.Posture.jump)
		{
			Jump();
		}
		if (item != null)
		{
			HqTrigger component = Utils.GetComponent<HqTrigger>(item, Utils.SearchChildren);
			if (component != null)
			{
				component.TurnOff();
			}
		}
		doneUsingItem = true;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return doneUsingItem;
	}

	protected void StartAction(int actionIndex)
	{
		currentActionDuration = 0f;
		currentItemEffectSequence = null;
		if (actionIndex < use.ItemActions.Count)
		{
			if (use.ItemActions[actionIndex].UserSequence != null)
			{
				currentActionDuration = StartUserSequence(actionIndex);
				float a = StartItemSequence(actionIndex);
				currentActionDuration = Mathf.Max(a, currentActionDuration);
			}
			currentActionDuration = Mathf.Max(currentActionDuration, use.ItemActions[actionIndex].MinDuration);
		}
	}

	private float StartItemSequence(int actionIndex)
	{
		float result = 0f;
		if (use.ItemActions[actionIndex].UserSequence.ItemSequence != null && use.ItemActions[actionIndex].UserSequence.ItemSequence.EventName == null)
		{
			string name = use.ItemActions[actionIndex].UserSequence.ItemSequence.Name;
			if (name != null && item != null)
			{
				result = item.StartSequence(name, owningObject, OnEffectEvent, OnEffectDone);
				currentItemEffectSequence = name;
			}
		}
		return result;
	}

	private float StartUserSequence(int actionIndex)
	{
		float result = 0f;
		EffectSequenceList effectSequenceList = owningObject.GetComponent(typeof(EffectSequenceList)) as EffectSequenceList;
		if (effectSequenceList != null)
		{
			GameObject gameObject = effectSequenceList.GetEffectSequencePrefabByName(use.ItemActions[actionIndex].UserSequence.SequenceName) as GameObject;
			if (gameObject != null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
				EffectSequence effectSequence = gameObject2.GetComponent(typeof(EffectSequence)) as EffectSequence;
				effectSequence.Initialize(charGlobals.gameObject, OnEffectDone, OnEffectEvent);
				effectSequence.StartSequence();
				currentEffectSequence = effectSequence;
				result = effectSequence.Lifetime;
			}
		}
		return result;
	}

	protected void OnEffectDone(EffectSequence seq)
	{
		if (!doneUsingItem && use != null && currentActionIndex < use.ItemActions.Count && elapsedTime - currentActionStartTime < currentActionDuration)
		{
			if (seq == currentEffectSequence)
			{
				StartUserSequence(currentActionIndex);
			}
			else
			{
				StartItemSequence(currentActionIndex);
			}
		}
	}

	protected void OnEffectEvent(EffectSequence seq, EventEffect effect)
	{
		if (item != null && (effect.EventName.ToLower() == "triggeron" || effect.EventName.ToLower() == "triggeroff"))
		{
			HqTrigger component = Utils.GetComponent<HqTrigger>(item, Utils.SearchChildren);
			if (component != null)
			{
				if (effect.EventName.ToLower() == "triggeron")
				{
					component.TurnOn();
				}
				else
				{
					component.TurnOff();
				}
			}
		}
		if (currentActionIndex >= use.ItemActions.Count)
		{
			return;
		}
		ItemAction itemAction = use.ItemActions[currentActionIndex];
		if (itemAction.UserSequence != null && itemAction.UserSequence.ItemSequence != null && itemAction.UserSequence.ItemSequence.EventName == effect.EventName)
		{
			string name = itemAction.UserSequence.ItemSequence.Name;
			if (name != null && item != null)
			{
				item.StartSequence(name, owningObject, OnEffectEvent, OnEffectDone);
			}
		}
	}

	protected void Sit()
	{
		if (animationComponent["sit_down"] != null)
		{
			animationComponent["sit_down"].wrapMode = WrapMode.Once;
			animationComponent.Play("sit_down");
		}
	}

	protected void Stand()
	{
		if (!playingStand && animationComponent["sit_stand"] != null)
		{
			animationComponent["sit_stand"].wrapMode = WrapMode.Once;
			animationComponent.Play("sit_stand");
			playingStand = true;
		}
	}

	protected void Jump()
	{
		charGlobals.motionController.jumpPressed();
		if (animationComponent["jump_up"] != null)
		{
			animationComponent["jump_up"].wrapMode = WrapMode.Once;
			animationComponent.Play("jump_up");
		}
	}

	protected void Idle()
	{
		if (use.PostureType == Use.Posture.sit)
		{
			if (HasAnimationCompleted("sit_down") && HasAnimationCompleted("sit_idle") && animationComponent["sit_idle"] != null)
			{
				animationComponent.Play("sit_idle");
			}
		}
		else if (use.PostureType == Use.Posture.stand && HasAnimationCompleted("movement_idle") && animationComponent["movement_idle"] != null)
		{
			animationComponent.Play("movement_idle");
		}
	}

	protected bool ShouldInterruptUse()
	{
		if (item == null || item.gameObject == null)
		{
			return true;
		}
		if (item.IsInAIControl)
		{
			return true;
		}
		HqObject2 component = Utils.GetComponent<HqObject2>(item.gameObject);
		if (item == null || (component != null && !component.IsUsableByAI))
		{
			return true;
		}
		if (itemUser != null && item != null && itemUser.CurrentActivityItem != item)
		{
			return true;
		}
		return false;
	}

	private bool HasAnimationCompleted(string animationName)
	{
		return !animationComponent.isPlaying || (animationComponent.IsPlaying(animationName) && animationComponent[animationName].time >= animationComponent[animationName].length);
	}

	public override void motionLanded()
	{
		if (use != null && use.PostureType == Use.Posture.jump)
		{
			Jump();
		}
	}
}
