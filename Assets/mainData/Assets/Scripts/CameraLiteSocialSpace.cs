using System;
using UnityEngine;

[AddComponentMenu("Camera/Camera Lite Social Space")]
public class CameraLiteSocialSpace : CameraLite
{
	public enum BoomStates
	{
		BoomDefault,
		BoomForward,
		BoomIdle,
		BoomBackwards
	}

	[Serializable]
	public class BoomInfo
	{
		public Vector3 boom = Vector3.zero;

		public float distance;

		public float minTimeToEnter = 1f;

		[HideInInspector]
		public float timer;
	}

	public BoomInfo boomForward;

	public BoomInfo boomIdle;

	public BoomInfo boomBackward;

	public float blendFactor = 5f;

	public float deadZone = 0.7f;

	public bool springEnabled = true;

	public float springConstant = 100f;

	public float springDamping = 15f;

	public float springMass = 1f;

	public Vector3 lookAtOffset;

	public bool zoomEnabled;

	public float zoomDistance = 15f;

	public float zoomMin = 10f;

	public float zoomMax = 25f;

	public float zoomWheelSpeed = 750f;

	public float zoomLerpSpeed = 50f;

	public float zoomAutoVelocity = 15f;

	public BoomStates debugBoomOverride;

	protected float camDistance;

	protected float camRelativeVelocity;

	protected Vector3 camVelocity = Vector3.zero;

	protected Vector3 targetLastFrame = Vector3.zero;

	protected float targetMaxSpeed = 1f;

	protected Vector3 targetVelocity = Vector3.zero;

	protected BoomStates boomState;

	protected BoomInfo boomCurrent;

	protected Vector3 boomPosOld;

	protected Vector3 boomPosCur;

	protected float boomTime;

	protected float boomTimeScale;

	protected Vector3 lookAt;

	protected Vector3 lookAtVel;

	protected bool onSpline;

	protected float wheelZoomPosition;

	protected float wheelZoomTime;

	protected float autoZoomPosition;

	protected float autoZoomTime;

	protected float autoZoomVelocity;

	public void Update()
	{
		if (!(target == null))
		{
			targetVelocity = (target.transform.position - targetLastFrame) / Time.deltaTime;
			targetLastFrame = target.transform.position;
		}
	}

	public override void InitFromMgr()
	{
		base.InitFromMgr();
		onSpline = false;
		Reset();
	}

	public override void WakeFromMgr()
	{
		base.WakeFromMgr();
		Reset();
	}

	public override void UpdateFromMgr(float deltaTime)
	{
		base.UpdateFromMgr(deltaTime);
		if (target == null)
		{
			return;
		}
		Vector3 vector = base.transform.forward;
		vector.y = 0f;
		vector = Vector3.Normalize(vector);
		camRelativeVelocity = Vector3.Dot(targetVelocity, vector) / targetMaxSpeed;
		if (zoomEnabled)
		{
			float mouseWheelDelta = SHSInput.GetMouseWheelDelta();
			if (mouseWheelDelta != 0f)
			{
				float num = mouseWheelDelta * deltaTime * (0f - zoomWheelSpeed);
				zoomDistance = Mathf.Clamp(zoomDistance + num, zoomMin, zoomMax);
			}
		}
		float f = zoomDistance - wheelZoomPosition;
		float num2 = Mathf.Abs(f);
		if (num2 >= 0.001f)
		{
			float num3 = deltaTime * zoomLerpSpeed;
			if (num2 <= num3)
			{
				wheelZoomPosition = zoomDistance;
			}
			else
			{
				wheelZoomPosition += num3 * Mathf.Sign(f);
			}
		}
		else
		{
			wheelZoomPosition = zoomDistance;
		}
		autoZoomPosition = Mathf.SmoothDamp(autoZoomPosition, boomCurrent.distance, ref autoZoomVelocity, autoZoomTime);
		camDistance = autoZoomPosition + wheelZoomPosition;
		Vector3 boom = boomIdle.boom;
		switch (debugBoomOverride)
		{
		case BoomStates.BoomDefault:
			if (camRelativeVelocity >= deadZone)
			{
				boomIdle.timer = 0f;
				boomBackward.timer = 0f;
				if (boomState != BoomStates.BoomForward)
				{
					boomForward.timer += deltaTime;
					if (boomForward.timer >= boomForward.minTimeToEnter)
					{
						boomForward.timer = 0f;
						UpdateBoom(BoomStates.BoomForward, boomForward);
					}
				}
			}
			else if (camRelativeVelocity <= 0f - deadZone)
			{
				boomIdle.timer = 0f;
				boomForward.timer = 0f;
				if (boomState != BoomStates.BoomBackwards)
				{
					boomBackward.timer += deltaTime;
					if (boomBackward.timer >= boomBackward.minTimeToEnter)
					{
						boomBackward.timer = 0f;
						UpdateBoom(BoomStates.BoomBackwards, boomBackward);
					}
				}
			}
			else if (targetVelocity.sqrMagnitude <= 1E-05f)
			{
				boomForward.timer = 0f;
				boomBackward.timer = 0f;
				if (boomState != BoomStates.BoomIdle)
				{
					boomIdle.timer += deltaTime;
					if (boomIdle.timer >= boomIdle.minTimeToEnter)
					{
						boomIdle.timer = 0f;
						UpdateBoom(BoomStates.BoomIdle, boomIdle);
					}
				}
			}
			boomTime += deltaTime * boomTimeScale;
			boomPosCur = Vector3.Slerp(boomPosOld, boomCurrent.boom, boomTime);
			boom = boomPosCur;
			break;
		case BoomStates.BoomForward:
			boomCurrent = boomForward;
			boom = boomCurrent.boom;
			break;
		case BoomStates.BoomIdle:
			boomCurrent = boomIdle;
			boom = boomCurrent.boom;
			break;
		case BoomStates.BoomBackwards:
			boomCurrent = boomBackward;
			boom = boomCurrent.boom;
			break;
		}
		Vector3 normalized = boom.normalized;
		boom = normalized * camDistance;
		Vector3 vector2 = target.position + boom;
		if (springEnabled)
		{
			base.transform.position += camVelocity * deltaTime;
			Vector3 a = vector2 - base.transform.position;
			Vector3 zero = Vector3.zero;
			zero += springConstant * a;
			zero -= springDamping * camVelocity;
			camVelocity += zero / springMass * deltaTime;
			float d = 1f;
			if (onSpline)
			{
				d = 2f;
			}
			lookAt += lookAtVel * deltaTime;
			a = target.transform.position - lookAt;
			zero = springConstant * a * d;
			zero -= springDamping * lookAtVel;
			lookAtVel += zero / springMass * deltaTime;
			base.transform.rotation = Quaternion.LookRotation((lookAt + lookAtOffset - base.transform.position).normalized);
		}
		else
		{
			base.transform.position = vector2;
			base.transform.LookAt(target);
		}
		SetDistance((target.transform.position - base.transform.position).magnitude);
	}

	public override void SleepFromMgr()
	{
		base.SleepFromMgr();
	}

	public override void SetDistance(float d)
	{
		base.SetDistance(d);
	}

	public override void SplineState(bool starting)
	{
		onSpline = starting;
	}

	public override void Reset()
	{
		base.Reset();
		camDistance = zoomDistance;
		camRelativeVelocity = 0f;
		camVelocity = Vector3.zero;
		targetLastFrame = Vector3.zero;
		targetVelocity = Vector3.zero;
		boomState = BoomStates.BoomIdle;
		boomCurrent = boomIdle;
		targetMaxSpeed = 1f;
		wheelZoomPosition = zoomDistance;
		wheelZoomTime = 1f;
		autoZoomPosition = boomCurrent.distance;
		autoZoomVelocity = 0f;
		autoZoomTime = 0f;
		if (target != null)
		{
			targetLastFrame = target.position;
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(target, Utils.SearchChildren);
			if (component != null)
			{
				targetMaxSpeed = component.speed;
			}
			lookAt = target.position;
			lookAtVel = Vector3.zero;
			boomTime = 1f;
			boomTimeScale = 1f;
			boomPosOld = boomIdle.boom;
			boomPosCur = boomIdle.boom;
			base.transform.position = target.position + boomCurrent.boom;
			base.transform.LookAt(lookAt + lookAtOffset);
		}
	}

	protected void UpdateBoom(BoomStates newBoomState, BoomInfo newBoom)
	{
		boomPosOld = boomPosCur;
		boomCurrent = newBoom;
		boomState = newBoomState;
		autoZoomTime = Mathf.Abs(newBoom.distance - autoZoomPosition) / zoomAutoVelocity;
		float magnitude = (boomCurrent.boom - boomPosCur).magnitude;
		if (magnitude < 0.01f)
		{
			boomTime = 10f;
			boomTimeScale = 1f;
		}
		else
		{
			boomTime = 0f;
			boomTimeScale = blendFactor / magnitude;
		}
	}
}
