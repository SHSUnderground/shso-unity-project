using UnityEngine;

public class SkyCamera : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum FollowType
	{
		FollowNone,
		FollowFixed,
		FollowSpring,
		FollowLerp
	}

	public enum ZoomEnum
	{
		ZoomOff,
		ZoomFOV,
		ZoomDistance
	}

	public Transform target;

	public Vector3 offset = Vector3.zero;

	public float distance;

	public FollowType followType = FollowType.FollowSpring;

	public float springConstant = 100f;

	public float springDamping = 15f;

	public float springMass = 1f;

	public float lerpTime = 1f;

	public float lerpMaxSpeed = 10f;

	public ZoomEnum zoomType = ZoomEnum.ZoomFOV;

	public float zoomSpeed = 1000f;

	public float zoomMin = 20f;

	public float zoomMax = 75f;

	protected Vector3 offsetToTarget = Vector3.zero;

	protected Vector3 velocity = Vector3.zero;

	protected float lastDistance;

	protected FollowType lastFollowType = FollowType.FollowSpring;

	private static bool IsNearZero(float a)
	{
		return Mathf.Abs(a) <= 0.001f;
	}

	private static bool IsNearEqual(Vector3 a, Vector3 b)
	{
		return IsNearZero(a.x - b.x) && IsNearZero(a.y - b.y) && IsNearZero(a.z - b.z);
	}

	public void CameraInit()
	{
		if (offset == Vector3.zero)
		{
			if ((bool)target)
			{
				offsetToTarget = base.transform.position - target.position;
			}
			else
			{
				offsetToTarget = base.transform.position;
			}
		}
		else
		{
			offsetToTarget = offset;
		}
		if (distance > 0f)
		{
			offsetToTarget = offsetToTarget.normalized * distance;
		}
		else
		{
			distance = offsetToTarget.magnitude;
		}
		offset = offsetToTarget;
		lastDistance = distance;
		lastFollowType = followType;
		if ((bool)target)
		{
			base.transform.position = target.position + offsetToTarget;
		}
	}

	private void Awake()
	{
		CameraInit();
	}

	private void Update()
	{
		if (lastDistance != distance && distance > 0f)
		{
			offsetToTarget = offsetToTarget.normalized * distance;
		}
		if (lastFollowType == FollowType.FollowNone && lastFollowType != followType)
		{
			offsetToTarget = base.transform.position - target.position;
			offset = offsetToTarget;
			distance = offsetToTarget.magnitude;
		}
		switch (zoomType)
		{
		case ZoomEnum.ZoomFOV:
		{
			float mouseWheelDelta2 = SHSInput.GetMouseWheelDelta();
			if (mouseWheelDelta2 != 0f)
			{
				float num2 = mouseWheelDelta2 * Time.deltaTime * (0f - zoomSpeed);
				if (base.camera.orthographic)
				{
					base.camera.orthographicSize = Mathf.Clamp(base.camera.orthographicSize + num2, zoomMin, zoomMax);
				}
				else
				{
					base.camera.fov = Mathf.Clamp(base.camera.fov + num2, zoomMin, zoomMax);
				}
			}
			break;
		}
		case ZoomEnum.ZoomDistance:
		{
			float mouseWheelDelta = SHSInput.GetMouseWheelDelta();
			if (mouseWheelDelta != 0f)
			{
				float num = mouseWheelDelta * Time.deltaTime * (0f - zoomSpeed);
				float d = Mathf.Clamp(offsetToTarget.magnitude + num, zoomMin, zoomMax);
				offsetToTarget = offsetToTarget.normalized * d;
				offset = offsetToTarget;
				distance = d;
			}
			break;
		}
		}
		lastDistance = distance;
		lastFollowType = followType;
	}

	private void LateUpdate()
	{
		if ((bool)target)
		{
			Vector3 vector = base.transform.position;
			Vector3 vector2 = target.position + offsetToTarget;
			switch (followType)
			{
			case FollowType.FollowNone:
				return;
			case FollowType.FollowFixed:
				vector = vector2;
				break;
			case FollowType.FollowSpring:
			{
				vector += velocity * Time.deltaTime;
				Vector3 a = vector2 - vector;
				Vector3 a2 = springConstant * a;
				a2 -= springDamping * velocity;
				velocity += a2 / springMass * Time.deltaTime;
				break;
			}
			case FollowType.FollowLerp:
				vector.x = Mathf.SmoothDamp(vector.x, vector2.x, ref velocity.x, lerpTime, lerpMaxSpeed);
				vector.y = Mathf.SmoothDamp(vector.y, vector2.y, ref velocity.y, lerpTime, lerpMaxSpeed);
				vector.z = Mathf.SmoothDamp(vector.z, vector2.z, ref velocity.z, lerpTime, lerpMaxSpeed);
				break;
			}
			base.transform.position = vector;
		}
	}
}
