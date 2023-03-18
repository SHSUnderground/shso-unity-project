using UnityEngine;

[AddComponentMenu("Camera/Camera Lite Spring")]
public class CameraLiteSpring : CameraLite
{
	public Vector3 targetToCamera;

	public float distanceFromTarget;

	public float audioListenerDistance = 13f;

	public bool springEnabled = true;

	public float springConstant = 100f;

	public float springDamping = 15f;

	public float springMass = 1f;

	public bool zoomEnable = true;

	public float zoomSpeed = 1000f;

	public float zoomMin = 2f;

	public float zoomMax = 75f;

	protected Vector3 offset;

	protected Vector3 velocity = Vector3.zero;

	protected bool lastSpringEnabled;

	protected float lastDistance = -1f;

	public override void Reset()
	{
		base.Reset();
		InternalOverride();
	}

	public override void InitFromMgr()
	{
		base.InitFromMgr();
		InternalOverride();
	}

	protected void InternalOverride()
	{
		if (targetToCamera == Vector3.zero)
		{
			if (target != null)
			{
				offset = base.transform.position - target.position;
			}
		}
		else
		{
			offset = targetToCamera;
		}
		if (distanceFromTarget > 0f)
		{
			offset = offset.normalized * distanceFromTarget;
		}
		else
		{
			distanceFromTarget = offset.magnitude;
		}
		targetToCamera = offset;
		lastDistance = distanceFromTarget;
		lastSpringEnabled = springEnabled;
		if (target != null)
		{
			base.transform.position = target.position + offset;
		}
		velocity = Vector3.zero;
	}

	public override void WakeFromMgr()
	{
		base.WakeFromMgr();
		InitFromMgr();
	}

	public override void UpdateFromMgr(float deltaTime)
	{
		if (target == null)
		{
			base.UpdateFromMgr(deltaTime);
			return;
		}
		if (lastDistance != distanceFromTarget && distanceFromTarget > 0f)
		{
			offset = offset.normalized * distanceFromTarget;
			targetToCamera = offset;
		}
		if (!lastSpringEnabled && springEnabled)
		{
			targetToCamera = base.transform.position - target.position;
			offset = targetToCamera;
			distanceFromTarget = offset.magnitude;
		}
		if (zoomEnable)
		{
			float mouseWheelDelta = SHSInput.GetMouseWheelDelta();
			if (mouseWheelDelta != 0f)
			{
				float num = mouseWheelDelta * deltaTime * (0f - zoomSpeed);
				float d = Mathf.Clamp(distanceFromTarget + num, zoomMin, zoomMax);
				offset = offset.normalized * d;
				targetToCamera = offset;
				distanceFromTarget = offset.magnitude;
			}
		}
		if (springEnabled)
		{
			base.transform.position += velocity * deltaTime;
			Vector3 a = target.position + targetToCamera;
			Vector3 a2 = a - base.transform.position;
			Vector3 zero = Vector3.zero;
			zero += springConstant * a2;
			zero -= springDamping * velocity;
			velocity += zero / springMass * deltaTime;
		}
		lastSpringEnabled = springEnabled;
		lastDistance = distanceFromTarget;
		base.UpdateFromMgr(deltaTime);
	}

	public override void SetDistance(float d)
	{
		float num = Mathf.Clamp(d, zoomMin, zoomMax);
		offset = offset.normalized * num;
		targetToCamera = offset;
		distanceFromTarget = offset.magnitude;
		base.SetDistance(num);
	}

	public override void UpdateAudioFromMgr(float deltaTime)
	{
		audioPosition = base.transform.TransformPoint(Vector3.forward * (distanceFromTarget - audioListenerDistance));
		audioRotation = base.transform.rotation * CameraLiteManager.Instance.audioListenerRotation;
	}

	private void Awake()
	{
		InitFromMgr();
	}
}
