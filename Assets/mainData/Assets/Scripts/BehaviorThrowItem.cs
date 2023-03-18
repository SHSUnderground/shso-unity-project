using System;
using UnityEngine;

internal class BehaviorThrowItem : BehaviorBase, IAnimTagListener
{
	protected enum State
	{
		TurningToFaceObject,
		Attaching,
		Rotating,
		Throwing,
		TurningBack,
		Done
	}

	public delegate void OnDoneThrowing(GameObject obj);

	protected GameObject objectToThrow;

	protected Transform attachNode;

	protected OnDoneThrowing DoneThrowing;

	protected bool saveKinematicMode;

	protected bool saveUsesGravity;

	private float throwForce = 10f;

	private float throwForceY = 10f;

	private AIControllerHQ aiController;

	protected State currentState;

	protected Vector3 lookBehind;

	protected bool attached;

	protected bool throwing;

	protected bool objectWasThrown;

	protected float throwStartTime;

	protected Vector3 originalScale;

	void IAnimTagListener.OnMoveStartAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnMoveEndAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnChainAttackAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnCollisionEnableAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnCollisionDisableAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnPinballStartAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnPinballEndAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnMultishotInfoAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnTriggerEffectAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnProjectileFireAnimTag(AnimationEvent evt)
	{
		if (currentState == State.Throwing)
		{
			LetGo();
		}
	}

	public bool Initialize(GameObject objToThrow, OnDoneThrowing doneCallback)
	{
		if (animationComponent["pickup"] == null)
		{
			CspUtils.DebugLog(owningObject.name + " does not have a pickup animation");
			return false;
		}
		attachNode = Utils.FindNodeInChildren(owningObject.transform, charGlobals.characterController.pickupBone);
		if (attachNode == null)
		{
			CspUtils.DebugLog("Cannot find attach node in BehaviorThrowSequence");
			return false;
		}
		animationComponent.Play("pickup");
		objectToThrow = objToThrow;
		HqObject2 component = Utils.GetComponent<HqObject2>(objectToThrow, Utils.SearchChildren);
		if (component != null)
		{
			HqController2.Instance.AIIsTakingOverItem(component);
		}
		DoneThrowing = doneCallback;
		currentState = State.TurningToFaceObject;
		attached = false;
		throwing = false;
		objectWasThrown = false;
		originalScale = objectToThrow.transform.localScale;
		aiController = Utils.GetComponent<AIControllerHQ>(owningObject);
		return true;
	}

	public override void behaviorUpdate()
	{
		if (Paused)
		{
			return;
		}
		base.behaviorUpdate();
		if (objectToThrow != null)
		{
			objectToThrow.transform.localScale = originalScale;
			switch (currentState)
			{
			case State.TurningToFaceObject:
			{
				Vector3 from = owningObject.transform.TransformDirection(Vector3.forward);
				from.y = 0f;
				from.Normalize();
				Vector3 to = objectToThrow.transform.position - owningObject.transform.position;
				to.y = 0f;
				to.Normalize();
				if (Vector3.Angle(from, to) > 2.5f)
				{
					Vector3 forward2 = Vector3.Slerp(from, to, elapsedTime);
					owningObject.transform.rotation = Quaternion.LookRotation(forward2);
					charGlobals.motionController.updateLookDirection();
				}
				else
				{
					currentState = State.Attaching;
				}
				break;
			}
			case State.Attaching:
				if (!attached)
				{
					if (objectToThrow.rigidbody != null)
					{
						saveKinematicMode = objectToThrow.rigidbody.isKinematic;
						saveUsesGravity = objectToThrow.rigidbody.useGravity;
						objectToThrow.rigidbody.isKinematic = true;
						objectToThrow.rigidbody.useGravity = false;
						objectToThrow.rigidbody.detectCollisions = false;
						objectToThrow.active = false;
						objectToThrow.active = true;
					}
					Vector3 zero = Vector3.zero;
					if (aiController != null)
					{
						aiController.AttachObject(objectToThrow, charGlobals.characterController.pickupBone);
					}
					if (objectToThrow.renderer != null)
					{
						float y = zero.y;
						Vector3 extents = objectToThrow.renderer.bounds.extents;
						zero.y = y + extents.y / 2f;
					}
					Quaternion rotation = objectToThrow.transform.rotation;
					objectToThrow.transform.parent = attachNode.transform;
					objectToThrow.transform.rotation = rotation;
					objectToThrow.transform.localPosition = zero;
					lookBehind = owningObject.transform.forward * -1f;
					attached = true;
				}
				if (attached && elapsedTime > animationComponent["pickup"].length)
				{
					currentState = State.Rotating;
				}
				break;
			case State.Rotating:
				if (Vector3.Angle(owningObject.transform.forward, lookBehind) > 5f)
				{
					Vector3 forward3 = Vector3.RotateTowards(owningObject.transform.forward, lookBehind, charGlobals.motionController.rotateSpeed * ((float)Math.PI / 180f) * Time.deltaTime, 1000f);
					owningObject.transform.rotation = Quaternion.LookRotation(forward3);
				}
				else
				{
					currentState = State.Throwing;
				}
				break;
			case State.Throwing:
				if (!throwing)
				{
					if (animationComponent["pickup_throw"] != null)
					{
						animationComponent.Play("pickup_throw");
						throwing = true;
						throwStartTime = elapsedTime;
					}
				}
				else if (animationComponent["pickup_throw"] == null || elapsedTime - throwStartTime > animationComponent["pickup_throw"].length)
				{
					currentState = State.TurningBack;
					lookBehind *= -1f;
				}
				break;
			case State.TurningBack:
				if (Vector3.Angle(owningObject.transform.forward, lookBehind) > 5f)
				{
					Vector3 forward = Vector3.RotateTowards(owningObject.transform.forward, lookBehind, charGlobals.motionController.rotateSpeed * ((float)Math.PI / 180f) * Time.deltaTime, 1000f);
					owningObject.transform.rotation = Quaternion.LookRotation(forward);
					break;
				}
				currentState = State.Done;
				if (DoneThrowing != null)
				{
					DoneThrowing(objectToThrow);
				}
				break;
			}
		}
		else if (DoneThrowing != null)
		{
			DoneThrowing(objectToThrow);
		}
	}

	public override void behaviorEnd()
	{
		detachObject();
		currentState = State.Done;
		objectToThrow = null;
		attachNode = null;
		base.behaviorEnd();
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return currentState == State.Done;
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override void behaviorCancel()
	{
		detachObject();
		currentState = State.Done;
	}

	protected void detachObject()
	{
		if (!(objectToThrow != null))
		{
			return;
		}
		HqItem component = Utils.GetComponent<HqItem>(objectToThrow);
		if (aiController != null)
		{
			aiController.DetachObject(objectToThrow);
			if (component != null)
			{
				aiController.ReleaseControl(component);
			}
		}
		else
		{
			objectToThrow.transform.parent = null;
		}
		if (objectToThrow.rigidbody != null && HqController2.Instance.State == typeof(HqController2.HqControllerFlinga))
		{
			objectToThrow.rigidbody.isKinematic = false;
			objectToThrow.rigidbody.useGravity = true;
			objectToThrow.rigidbody.detectCollisions = true;
		}
		objectToThrow.transform.localScale = originalScale;
		if (component != null && component.Room != HqController2.Instance.ActiveRoom)
		{
			Utils.ActivateTree(objectToThrow, false);
			return;
		}
		objectToThrow.active = false;
		objectToThrow.active = true;
	}

	private void LetGo()
	{
		if (!objectWasThrown && objectToThrow.rigidbody != null)
		{
			detachObject();
			Vector3 force = lookBehind.normalized * throwForce;
			force.y += throwForceY;
			objectToThrow.rigidbody.AddForce(force, ForceMode.VelocityChange);
			objectWasThrown = true;
		}
	}
}
