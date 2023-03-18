using System;
using UnityEngine;

public class BehaviorInteract : BehaviorBase
{
	public delegate void OnIteractionOver(GameObject objInteractedWith);

	public delegate void OnInteractionEvent(GameObject objInteractedWith, string eventName, float value);

	protected const float facingAngleMin = 10f;

	protected bool doneFacing;

	protected GameObject objectToInteractWith;

	protected EffectSequence effectSequence;

	protected OnIteractionOver DoneCallback;

	protected OnInteractionEvent InteractEventCallback;

	protected bool effectSequenceFinished;

	protected GameObject effectObject;

	public override bool Paused
	{
		get
		{
			return base.Paused;
		}
		set
		{
			base.Paused = value;
			if (effectSequence != null)
			{
				effectSequence.Paused = value;
			}
		}
	}

	public void Initialize(GameObject gameObject, OnIteractionOver overCallback, OnInteractionEvent eventCallback, sbyte emoteId)
	{
		effectSequenceFinished = true;
		objectToInteractWith = gameObject;
		DoneCallback = overCallback;
		InteractEventCallback = eventCallback;
		doneFacing = false;
		if (emoteId == 0)
		{
			return;
		}
		EffectSequenceList effectSequenceList = owningObject.GetComponent(typeof(EffectSequenceList)) as EffectSequenceList;
		if (effectSequenceList == null)
		{
			return;
		}
		EmotesDefinition.EmoteDefinition emoteById = EmotesDefinition.Instance.GetEmoteById(emoteId);
		if (emoteById == null)
		{
			return;
		}
		EffectSequence effectSequence = null;
		if (emoteById.isLogicalSequence)
		{
			effectSequence = effectSequenceList.GetLogicalEffectSequence(emoteById.sequenceName);
		}
		else
		{
			GameObject gameObject2 = effectSequenceList.GetEffectSequencePrefabByName(emoteById.sequenceName) as GameObject;
			if (gameObject2 != null)
			{
				effectObject = (UnityEngine.Object.Instantiate(gameObject2) as GameObject);
				effectSequence = (effectObject.GetComponent(typeof(EffectSequence)) as EffectSequence);
				effectSequence.SetParent(charGlobals.gameObject);
			}
		}
		if (!(effectSequence == null))
		{
			this.effectSequence = effectSequence;
			effectSequence.Initialize(null, OnEffectDone, OnEffectEvent);
		}
	}

	public override void behaviorBegin()
	{
		base.behaviorBegin();
	}

	public override void behaviorEnd()
	{
		base.behaviorEnd();
		if (effectSequence != null)
		{
			UnityEngine.Object.Destroy(effectSequence);
			effectSequence = null;
		}
		if (effectObject != null)
		{
			UnityEngine.Object.Destroy(effectObject);
			effectObject = null;
		}
		objectToInteractWith = null;
	}

	public override void behaviorCancel()
	{
		base.behaviorCancel();
		if (effectSequence != null)
		{
			UnityEngine.Object.Destroy(effectSequence);
			effectSequence = null;
		}
		if (effectObject != null)
		{
			UnityEngine.Object.Destroy(effectObject);
			effectObject = null;
		}
		objectToInteractWith = null;
	}

	public override void behaviorUpdate()
	{
		if (Paused)
		{
			return;
		}
		base.behaviorUpdate();
		if (IsFacing(objectToInteractWith))
		{
			if (!doneFacing)
			{
				doneFacing = true;
				if (effectSequence != null)
				{
					effectSequence.StartSequence();
					effectSequenceFinished = false;
				}
				else if (DoneCallback != null)
				{
					DoneCallback(objectToInteractWith);
				}
			}
		}
		else if (objectToInteractWith != null)
		{
			Vector3 target = objectToInteractWith.gameObject.transform.position - owningObject.transform.position;
			target.y = 0f;
			target.Normalize();
			Vector3 forward = Vector3.RotateTowards(owningObject.transform.forward, target, charGlobals.motionController.rotateSpeed * ((float)Math.PI / 180f) * Time.deltaTime, 1000f);
			owningObject.transform.rotation = Quaternion.LookRotation(forward);
		}
	}

	private void OnEffectDone(EffectSequence seq)
	{
		effectSequenceFinished = true;
		if (DoneCallback != null)
		{
			DoneCallback(objectToInteractWith);
		}
	}

	private void OnEffectEvent(EffectSequence seq, EventEffect effect)
	{
		if (InteractEventCallback != null)
		{
			InteractEventCallback(objectToInteractWith, effect.EventName, effect.EventValue);
		}
	}

	public override bool useMotionControllerRotate()
	{
		return false;
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		if (newBehaviorType == typeof(BehaviorInteract))
		{
			return false;
		}
		return true;
	}

	public override bool allowUserInput()
	{
		return false;
	}

	protected virtual bool IsFacing(GameObject gameObject)
	{
		if (gameObject == null)
		{
			return true;
		}
		Vector3 from = owningObject.transform.TransformDirection(Vector3.forward);
		Vector3 to = gameObject.transform.position - owningObject.transform.position;
		from.y = 0f;
		to.y = 0f;
		float num = Vector3.Angle(from, to);
		if (num <= 10f)
		{
			return true;
		}
		return false;
	}
}
