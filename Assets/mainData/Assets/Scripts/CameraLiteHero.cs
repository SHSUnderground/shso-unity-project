using UnityEngine;

[AddComponentMenu("Camera/Camera Lite Hero")]
public class CameraLiteHero : CameraLite
{
	public Vector3 boom;

	public Vector3 lookAtOffset;

	public float zoomDistance = 15f;

	public float zoomMin = 10f;

	public float zoomMax = 25f;

	public float zoomWheelSpeed = 750f;

	public float zoomLerpSpeed = 10f;

	public bool springEnabled = true;

	public float springConstant = 100f;

	public float springDamping = 15f;

	public float springMass = 1f;

	protected float camDistance;

	protected Vector3 camVelocity = Vector3.zero;

	protected Vector3 lookAt;

	protected Vector3 lookAtVel;

	public override void InitFromMgr()
	{
		base.InitFromMgr();
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
		float mouseWheelDelta = SHSInput.GetMouseWheelDelta();
		if (mouseWheelDelta != 0f)
		{
			float num = mouseWheelDelta * deltaTime * (0f - zoomWheelSpeed);
			zoomDistance = Mathf.Clamp(zoomDistance + num, zoomMin, zoomMax);
		}
		float num2 = Mathf.Clamp(zoomDistance, zoomMin, zoomMax);
		if (!Mathf.Approximately(camDistance, num2))
		{
			float num3 = deltaTime * zoomLerpSpeed;
			float f = num2 - camDistance;
			if (Mathf.Abs(f) <= num3)
			{
				camDistance = num2;
			}
			else
			{
				camDistance += num3 * Mathf.Sign(f);
			}
		}
		SetDistance(camDistance);
		Vector3 normalized = boom.normalized;
		boom = normalized * camDistance;
		Vector3 vector = target.position + boom;
		if (springEnabled)
		{
			base.transform.position += camVelocity * deltaTime;
			Vector3 a = vector - base.transform.position;
			Vector3 zero = Vector3.zero;
			zero += springConstant * a;
			zero -= springDamping * camVelocity;
			camVelocity += zero / springMass * deltaTime;
			lookAt += lookAtVel * deltaTime;
			a = target.transform.position - lookAt;
			zero = springConstant * a;
			zero -= springDamping * lookAtVel;
			lookAtVel += zero / springMass * deltaTime;
			base.transform.rotation = Quaternion.LookRotation((lookAt + lookAtOffset - base.transform.position).normalized);
		}
		else
		{
			base.transform.position = vector;
			base.transform.LookAt(target);
		}
	}

	public override void SleepFromMgr()
	{
		base.SleepFromMgr();
	}

	public override void SetDistance(float d)
	{
		base.SetDistance(d);
	}

	public override void Reset()
	{
		base.Reset();
		camDistance = zoomDistance;
		camVelocity = Vector3.zero;
		if (target != null)
		{
			lookAt = target.position;
			lookAtVel = Vector3.zero;
			base.transform.position = target.position + boom;
			base.transform.LookAt(lookAt + lookAtOffset);
		}
	}
}
