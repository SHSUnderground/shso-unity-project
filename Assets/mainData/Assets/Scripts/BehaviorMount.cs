using System;
using UnityEngine;

internal class BehaviorMount : BehaviorBase
{
	protected enum States
	{
		Uninitialized,
		Mounting,
		Landing,
		Done
	}

	protected States state;

	protected ShsCharacterController charController;

	protected CharacterMotionController motionController;

	protected Vector3 mountSourceUp;

	protected Vector3 toMountSource;

	protected Vector3 lookVector;

	protected Vector3 startPosition;

	protected float time;

	protected float mountTime = 1f;

	protected float jumpHeight = 3f;

	protected bool isFalling;

	protected float landTimer;

	protected Vector3 destination;

	protected OnBehaviorDone doneCallback;

	protected string jumpAnim;

	protected string fallAnim;

	protected string landAnim;

	protected string mountSequenceName;

	public bool Initialize(DockPoint launchPoint, OnBehaviorDone onMountDone, float jumpHeight, string mountAnim1, string mountAnim2, string mountAnim3, string mountSequenceName, float time)
	{
		doneCallback = onMountDone;
		destination = launchPoint.transform.position;
		this.jumpHeight = jumpHeight;
		isFalling = false;
		jumpAnim = mountAnim1;
		fallAnim = mountAnim2;
		landAnim = mountAnim3;
		mountTime = time;
		this.mountSequenceName = mountSequenceName;
		InitStateMounting();
		return true;
	}

	public override void behaviorUpdate()
	{
		switch (state)
		{
		case States.Mounting:
			StateMounting();
			break;
		case States.Landing:
			StateLanding();
			break;
		case States.Done:
			CheckEnd();
			break;
		default:
			CspUtils.DebugLog("Unexpected state");
			EndMountState();
			break;
		}
		base.behaviorUpdate();
	}

	protected void InitStateMounting()
	{
		charController = charGlobals.characterController;
		motionController = charGlobals.motionController;
		motionController.setForcedVelocityAllow(false);
		combatController.setRecoilAllow(false);
		startPosition = owningObject.transform.position;
		mountSourceUp = default(Vector3);
		toMountSource = default(Vector3);
		float vectorLength = 0f;
		MountableObjectController.InitMountVectors(owningObject, destination, ref mountSourceUp, ref toMountSource, ref vectorLength);
		lookVector = new Vector3(toMountSource.x, 0f, toMountSource.z);
		state = States.Mounting;
		time = 0f;
		if (!string.IsNullOrEmpty(jumpAnim))
		{
			animationComponent.Rewind(jumpAnim);
			animationComponent.Play(jumpAnim);
		}
		if (!string.IsNullOrEmpty(mountSequenceName))
		{
			charGlobals.effectsList.TryOneShot(mountSequenceName, owningObject);
		}
	}

	protected void StateMounting()
	{
		time += Time.deltaTime / mountTime;
		if (time >= 1f)
		{
			EndMountState();
			return;
		}
		Vector3 position = owningObject.transform.position;
		Vector3 position2 = MountableObjectController.EvalPosition(time, jumpHeight, startPosition, toMountSource);
		if (!isFalling)
		{
			float num = position2.y - position.y;
			if (num < 0f)
			{
				isFalling = true;
				if (!string.IsNullOrEmpty(fallAnim))
				{
					animationComponent.Rewind(fallAnim);
					animationComponent.Play(fallAnim);
				}
			}
		}
		owningObject.transform.position = position2;
	}

	protected void EndMountState()
	{
		if (motionController != null)
		{
			motionController.updateLookDirection();
			motionController.setDestination(destination);
		}
		if (!string.IsNullOrEmpty(landAnim))
		{
			animationComponent.Rewind(landAnim);
			animationComponent.Play(landAnim);
			landTimer = 0f;
			state = States.Landing;
		}
		else
		{
			state = States.Done;
		}
	}

	protected void StateLanding()
	{
		landTimer -= Time.deltaTime;
		if (landTimer < 0f)
		{
			state = States.Done;
		}
	}

	protected void CheckEnd()
	{
		motionController.setForcedVelocityAllow(true);
		combatController.setRecoilAllow(true);
		if (doneCallback != null)
		{
			doneCallback(owningObject);
		}
		if (charGlobals.behaviorManager.getBehavior() == this)
		{
			charGlobals.behaviorManager.endBehavior();
		}
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override bool useMotionControllerGravity()
	{
		return false;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}
}
