using System;
using UnityEngine;

public class BehaviorFreeFall : BehaviorBase
{
	protected enum State
	{
		Unitialized,
		Falling,
		Landing,
		Done
	}

	protected const string animLand = "jump_land";

	protected Vector3 velocity = Vector3.zero;

	protected State state;

	protected ShsCharacterController characterController;

	protected OnBehaviorDone onBehaviorDone;

	public void Initialize(Vector3 initialVelocity, OnBehaviorDone onDone)
	{
		velocity = initialVelocity;
		onBehaviorDone = onDone;
		state = State.Falling;
	}

	public override void behaviorBegin()
	{
		characterController = charGlobals.characterController;
		if (CapsuleHasLanded())
		{
			state = State.Done;
		}
		else
		{
			state = State.Falling;
		}
	}

	public override void behaviorUpdate()
	{
		switch (state)
		{
		case State.Unitialized:
			CspUtils.DebugLog("BehaviorFreeFall is unitialized");
			break;
		case State.Falling:
			characterController.Move(velocity * Time.deltaTime);
			if (!CapsuleHasLanded())
			{
				Vector3 a = new Vector3(0f, 0f - charGlobals.motionController.gravity, 0f);
				velocity += a * Time.deltaTime;
				break;
			}
			state = State.Landing;
			if (animationComponent["jump_land"] == null)
			{
				state = State.Done;
				break;
			}
			animationComponent.Rewind("jump_land");
			animationComponent.CrossFade("jump_land", 0.25f);
			break;
		case State.Landing:
			if (animationComponent["jump_land"].time >= animationComponent["jump_land"].length)
			{
				state = State.Done;
			}
			break;
		case State.Done:
			charGlobals.motionController.setDestination(owningObject.transform.position);
			if (onBehaviorDone != null)
			{
				onBehaviorDone(owningObject);
			}
			if (charGlobals.behaviorManager.getBehavior() == this)
			{
				charGlobals.behaviorManager.endBehavior();
			}
			break;
		}
		base.behaviorUpdate();
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override bool useMotionControllerRotate()
	{
		return false;
	}

	protected bool CapsuleHasLanded()
	{
		if (characterController.isGrounded)
		{
			return true;
		}
		if ((characterController.collisionFlags & CollisionFlags.Below) != 0)
		{
			return true;
		}
		return false;
	}
}
