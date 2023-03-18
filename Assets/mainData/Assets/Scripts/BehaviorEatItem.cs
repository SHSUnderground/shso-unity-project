using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorEatItem : BehaviorInteract
{
	private delegate void EventHandler(GameObject objEaten, float value);

	public delegate void OnEatOver(GameObject obj);

	public delegate void OnItemEaten(GameObject obj);

	private Dictionary<string, EventHandler> eventHandlers;

	private OnEatOver doneEating;

	private OnItemEaten itemEaten;

	protected bool finishedEating;

	protected Vector3 foodStartPosition;

	public void Initialize(GameObject eatTargetObject, OnItemEaten itemEatenCallback, OnEatOver overCallback)
	{
		if (owningObject.collider != null)
		{
			Collider[] components = Utils.GetComponents<Collider>(eatTargetObject, Utils.SearchChildren);
			Collider[] array = components;
			foreach (Collider collider in array)
			{
				Physics.IgnoreCollision(collider, owningObject.collider);
			}
		}
		if (eatTargetObject.rigidbody != null)
		{
			eatTargetObject.rigidbody.isKinematic = true;
			eatTargetObject.rigidbody.useGravity = false;
			eatTargetObject.rigidbody.detectCollisions = false;
		}
		itemEaten = itemEatenCallback;
		doneEating = overCallback;
		foodStartPosition = eatTargetObject.transform.position;
		EmotesDefinition.EmoteDefinition emoteByCommand = EmotesDefinition.Instance.GetEmoteByCommand("eat");
		if (emoteByCommand != null)
		{
			eventHandlers = new Dictionary<string, EventHandler>();
			eventHandlers["MoveFood"] = OnMoveFoodEvent;
			eventHandlers["GrabFood"] = OnGrabFoodEvent;
			eventHandlers["EatFood"] = OnEatFoodEvent;
			Initialize(eatTargetObject, OnDoneEating, OnEatEvent, emoteByCommand.id);
		}
		Vector3 vector = eatTargetObject.transform.position - owningObject.transform.position;
		vector.y = 0f;
		vector.Normalize();
		if (vector != Vector3.zero)
		{
			owningObject.transform.rotation = Quaternion.LookRotation(vector);
		}
		finishedEating = false;
	}

	private void OnEatEvent(GameObject objEaten, string eventName, float value)
	{
		if (eventHandlers.ContainsKey(eventName))
		{
			eventHandlers[eventName](objEaten, value);
		}
	}

	private void OnMoveFoodEvent(GameObject objEaten, float value)
	{
		Transform transform = Utils.FindNodeInChildren(owningObject.transform, "fx_Rpalm");
		objectToInteractWith.transform.position = transform.position;
	}

	private void OnGrabFoodEvent(GameObject objEaten, float value)
	{
		AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(owningObject);
		if (objectToInteractWith != null && component != null)
		{
			component.AttachObject(objectToInteractWith, "fx_Rpalm");
			objectToInteractWith.transform.localPosition = Vector3.zero;
			if (objectToInteractWith.transform.parent != null)
			{
				objectToInteractWith.transform.position = objectToInteractWith.transform.position + objectToInteractWith.transform.parent.transform.up * 0.28f;
			}
		}
		EatenEffect.Play(objEaten, owningObject);
	}

	private void OnEatFoodEvent(GameObject objEaten, float value)
	{
		finishedEating = true;
		if (itemEaten != null)
		{
			itemEaten(objEaten);
		}
		detachItem();
		objectToInteractWith = null;
	}

	private void OnDoneEating(GameObject objInteractedWith)
	{
		if (doneEating != null)
		{
			doneEating(objInteractedWith);
		}
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return finishedEating;
	}

	protected override bool IsFacing(GameObject gameObject)
	{
		return true;
	}

	protected void detachItem()
	{
		AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(owningObject);
		if (!(objectToInteractWith != null) || !(component != null))
		{
			return;
		}
		component.DetachObject(objectToInteractWith);
		HqItem component2 = Utils.GetComponent<HqItem>(objectToInteractWith, Utils.SearchChildren);
		if (component2 != null)
		{
			if (component2 == component.CurrentActivityItem)
			{
				component.ReleaseCurrentItem();
			}
			else
			{
				component2.ReleaseControl(component);
			}
		}
		if (objectToInteractWith.rigidbody != null && HqController2.Instance.State == typeof(HqController2.HqControllerFlinga))
		{
			objectToInteractWith.rigidbody.isKinematic = false;
			objectToInteractWith.rigidbody.useGravity = true;
			objectToInteractWith.rigidbody.detectCollisions = true;
		}
	}

	public override void behaviorEnd()
	{
		detachItem();
		finishedEating = true;
		base.behaviorEnd();
	}

	public override void behaviorCancel()
	{
		detachItem();
		finishedEating = true;
		base.behaviorCancel();
	}
}
