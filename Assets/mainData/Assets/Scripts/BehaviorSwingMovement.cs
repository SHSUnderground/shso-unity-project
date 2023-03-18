using System;
using UnityEngine;

public class BehaviorSwingMovement : BehaviorBase
{
	protected enum SwingState
	{
		PreSwing,
		Swinging,
		Falling
	}

	protected SwingState currentState;

	protected GameObject SwingEffectInstance;

	protected SwingData swingData;

	protected bool Initialized;

	protected LineRenderer lineRenderer;

	protected LaserBeamScript laserBeamScript;

	protected Vector3 pivotPosition;

	protected bool IsMaxLevel;

	public override void behaviorBegin()
	{
		Initialized = false;
		base.behaviorBegin();
		currentState = SwingState.PreSwing;
		string text = charGlobals.motionController.SwingData;
		if (!string.IsNullOrEmpty(text))
		{
			GameObject gameObject = charGlobals.effectsList.GetEffectSequencePrefabByName(text) as GameObject;
			if (gameObject != null)
			{
				swingData = Utils.GetComponent<SwingData>(gameObject);
				if (swingData != null)
				{
					if (animationComponent[swingData.Animation] == null)
					{
						CspUtils.DebugLog("No swing animation for character: " + owningObject.gameObject.name);
						return;
					}
					if (animationComponent[swingData.EndAnimation] == null)
					{
						CspUtils.DebugLog("No swing end animation for character: " + owningObject.gameObject.name);
						return;
					}
					Initialized = true;
					CreateRope();
				}
			}
		}
		lineRenderer = null;
		HeroPersisted heroPersisted = null;
	}

	public override void behaviorUpdate()
	{
		if (!Initialized)
		{
			charGlobals.behaviorManager.endBehavior();
			return;
		}
		if (lineRenderer == null)
		{
			InitializeLineRenderer();
		}
		switch (currentState)
		{
		case SwingState.PreSwing:
			if (elapsedTime >= swingData.PauseTime)
			{
				animationComponent[swingData.Animation].speed = 1f;
				charGlobals.motionController.setForcedVelocity(owningObject.transform.forward * swingData.Speed, swingData.Time);
				currentState = SwingState.Swinging;
			}
			break;
		case SwingState.Swinging:
			if (elapsedTime >= swingData.Time + swingData.PauseTime)
			{
				if (SwingEffectInstance != null)
				{
					UnityEngine.Object.Destroy(SwingEffectInstance);
					SwingEffectInstance = null;
				}
				if (!string.IsNullOrEmpty(swingData.EndAnimation))
				{
					animationComponent.CrossFade(swingData.EndAnimation, 0.3f);
				}
				charGlobals.motionController.setForcedVelocity(owningObject.transform.forward * (swingData.Speed * 0.5f), swingData.FallTime);
				if (IsMaxLevel)
				{
					charGlobals.motionController.DoubleJumping = false;
				}
				currentState = SwingState.Falling;
			}
			else
			{
				float verticalVelocity = swingData.Curve.Evaluate((elapsedTime - swingData.PauseTime) / swingData.Time) * swingData.VerticalVelocity;
				charGlobals.motionController.setVerticalVelocity(verticalVelocity);
				if (lineRenderer != null)
				{
					lineRenderer.SetPosition(1, lineRenderer.transform.InverseTransformPoint(pivotPosition));
				}
			}
			break;
		case SwingState.Falling:
			if (charGlobals.motionController.IsOnGround())
			{
				charGlobals.motionController.setWasOnGround(false);
				charGlobals.behaviorManager.endBehavior();
				return;
			}
			break;
		}
		base.behaviorUpdate();
	}

	private void InitializeLineRenderer()
	{
		if (!(lineRenderer == null))
		{
			return;
		}
		lineRenderer = Utils.GetComponent<LineRenderer>(owningObject, Utils.SearchChildren);
		if (lineRenderer != null && lineRenderer.transform.parent != null)
		{
			GameObject gameObject = lineRenderer.transform.parent.gameObject;
			laserBeamScript = Utils.GetComponent<LaserBeamScript>(gameObject, Utils.SearchChildren);
			if (laserBeamScript != null)
			{
				laserBeamScript.BeamLength = swingData.LineLength;
				pivotPosition = lineRenderer.transform.TransformPoint(Vector3.forward * laserBeamScript.BeamLength);
			}
			Color white = Color.white;
			Color end = new Color(1f, 1f, 1f, 0f);
			lineRenderer.SetColors(white, end);
		}
	}

	private void CreateRope()
	{
		if (!(swingData != null))
		{
			return;
		}
		if (!string.IsNullOrEmpty(swingData.Animation))
		{
			animationComponent[swingData.Animation].speed *= 2f;
			animationComponent.CrossFade(swingData.Animation, 0.1f);
			Initialized = true;
		}
		if (swingData.Effect != null)
		{
			SwingEffectInstance = (UnityEngine.Object.Instantiate(swingData.Effect) as GameObject);
			if (SwingEffectInstance != null)
			{
				Utils.AttachGameObject(owningObject, SwingEffectInstance);
			}
		}
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return currentState == SwingState.Falling || newBehaviorType != typeof(BehaviorEmote);
	}

	public override bool useMotionController()
	{
		return currentState == SwingState.Falling;
	}

	public override bool useMotionControllerGravity()
	{
		return currentState == SwingState.Falling;
	}

	public override void behaviorEnd()
	{
		if (SwingEffectInstance != null)
		{
			UnityEngine.Object.Destroy(SwingEffectInstance);
		}
		charGlobals.motionController.setForcedVelocityDuration(0f);
		base.behaviorEnd();
	}

	public override void motionCollided()
	{
		elapsedTime = float.MaxValue;
	}

	public override bool allowUserInput()
	{
		return currentState != SwingState.Swinging;
	}

	public override void userInputOverride()
	{
		if (SHSInput.GetButtonDown("Jump") && currentState == SwingState.Swinging)
		{
			if (IsMaxLevel)
			{
				charGlobals.motionController.DoubleJumping = false;
			}
			currentState = SwingState.Falling;
			charGlobals.motionController.setForcedVelocity(owningObject.transform.forward * 10f, 0.5f);
			charGlobals.motionController.setVerticalVelocity(15f);
			if (!string.IsNullOrEmpty(swingData.EndAnimation))
			{
				animationComponent.CrossFade(swingData.EndAnimation, 0.5f);
			}
			lineRenderer.SetWidth(0f, 0f);
		}
	}
}
