using System;
using UnityEngine;

public class BehaviorTurnTo : BehaviorMovement
{
	public delegate void TurnedDelegate(GameObject objTurning);

	protected TurnedDelegate onTurned;

	protected Vector3 targetPosition;

	protected Vector3 targetLook;

	public void Initialize(Vector3 position, TurnedDelegate onTurned)
	{
		this.onTurned = onTurned;
		targetPosition = position;
		targetLook = position - owningObject.transform.position;
		targetLook.y = 0f;
		targetLook.Normalize();
	}

	public override void behaviorUpdate()
	{
		if (IsFacing())
		{
			Turned();
			return;
		}
		updateRotation();
		base.behaviorUpdate();
	}

	private bool updateRotation()
	{
		Vector3 current = owningObject.transform.TransformDirection(Vector3.forward);
		current.y = 0f;
		current.Normalize();
		targetLook = targetPosition - owningObject.transform.position;
		targetLook.y = 0f;
		targetLook.Normalize();
		Vector3 vector = Vector3.RotateTowards(current, targetLook, charGlobals.motionController.rotateSpeed * ((float)Math.PI / 180f) * Time.deltaTime, 1000f);
		owningObject.transform.rotation = Quaternion.LookRotation(vector);
		charGlobals.motionController.setDestination(owningObject.transform.position, vector);
		return false;
	}

	public override bool allowUserInput()
	{
		return false;
	}

	protected virtual bool IsFacing()
	{
		Vector3 lhs = owningObject.transform.TransformDirection(Vector3.forward);
		lhs.y = 0f;
		lhs.Normalize();
		targetLook = targetPosition - owningObject.transform.position;
		targetLook.y = 0f;
		targetLook.Normalize();
		if (Vector3.Dot(lhs, targetLook) > 0.95f)
		{
			return true;
		}
		return false;
	}

	protected void Turned()
	{
		if (onTurned != null)
		{
			onTurned(owningObject);
		}
		if (charGlobals.behaviorManager.getBehavior() == this)
		{
			charGlobals.behaviorManager.endBehavior();
		}
	}
}
