using UnityEngine;

[AddComponentMenu("Camera/Camera Lite")]
public class CameraLite : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float nearClipPlane = 0.3f;

	public float farClipPlane = 1000f;

	public float fieldOfView = 45f;

	public float focalLength = 15f;

	public float dofOffset;

	public Vector3 audioPosition = Vector3.zero;

	public Quaternion audioRotation = Quaternion.identity;

	protected Transform target;

	protected CameraTarget targetComponent;

	protected Vector3 targetOffset = Vector3.zero;

	public virtual void SetFromCameraData(CameraData data)
	{
		nearClipPlane = data.nearClipPlane;
		farClipPlane = data.farClipPlane;
		fieldOfView = data.fieldOfView;
		focalLength = data.focalLength;
		dofOffset = data.dofOffset;
		base.transform.position = data.position;
		base.transform.rotation = data.rotation;
		CspUtils.DebugLog("**CameraLite base.transform.position=" + base.transform.position);
		CspUtils.DebugLog("base.name = " + base.name);
		audioPosition = data.audioPosition;
		audioRotation = data.audioRotation;
	}

	public virtual void SetFromCameraLite(CameraLite cam)
	{
		nearClipPlane = cam.nearClipPlane;
		farClipPlane = cam.farClipPlane;
		fieldOfView = cam.fieldOfView;
		focalLength = cam.focalLength;
		dofOffset = cam.dofOffset;
		base.transform.position = cam.transform.position;
		base.transform.rotation = cam.transform.rotation;
		CspUtils.DebugLog("**CameraLite base.transform.position=" + base.transform.position);
		CspUtils.DebugLog("base.name = " + base.name);
		audioPosition = cam.audioPosition;
		audioRotation = cam.audioRotation;
	}

	public virtual void CopyToCamera(Camera cam)
	{
		CspUtils.DebugLog("**CameraLite base.transform.position=" + base.transform.position);
		CspUtils.DebugLog("cam.name = " + cam.name);
		CspUtils.DebugLog("base.name = " + base.name);
		cam.transform.position = base.transform.position;
		cam.transform.rotation = base.transform.rotation;
		cam.fieldOfView = fieldOfView;
		AOSceneCamera component = Utils.GetComponent<AOSceneCamera>(cam);
		if (component != null)
		{
			component.DOFTarget = focalLength + dofOffset;
		}
		cam.nearClipPlane = nearClipPlane;
		cam.farClipPlane = farClipPlane;
	}

	public virtual void InitFromMgr()
	{
	}

	public virtual void WakeFromMgr()
	{
		InternalReset();
	}

	public virtual void UpdateFromMgr(float deltaTime)
	{
		if (target != null)
		{
			targetOffset = base.transform.position - target.position;
		}
	}

	public virtual void UpdateAudioFromMgr(float deltaTime)
	{
		audioPosition = base.transform.TransformPoint(CameraLiteManager.Instance.audioListenerOffset);
		audioRotation = base.transform.rotation * CameraLiteManager.Instance.audioListenerRotation;
	}

	public virtual void SleepFromMgr()
	{
	}

	public virtual void SetDistance(float d)
	{
		focalLength = d;
	}

	public virtual float GetDistance()
	{
		return focalLength;
	}

	public virtual void SplineState(bool starting)
	{
	}

	public virtual void Reset()
	{
		InternalReset();
	}

	public virtual void SetTarget(Transform newTarget)
	{
		CameraTargetHelper componentInChildren = newTarget.gameObject.GetComponentInChildren<CameraTargetHelper>();
		if (componentInChildren != null)
		{
			target = componentInChildren.transform;
			CspUtils.DebugLog("**CameraLite componentInChildren.transform.gameObject.name=" + componentInChildren.transform.gameObject.name);
			CspUtils.DebugLog("**CameraLite target.transform=" + target.transform);
		}
		else
		{
			CspUtils.DebugLog("**CameraLite newTarget.gameObject=" + newTarget.gameObject.name);
			target = newTarget;
		}
	}

	public Transform GetTarget()
	{
		return target;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "CameraDataIcon.png");
	}

	private void OnDrawGizmosSelected()
	{
		Color color = new Color(0.46f, 0.8f, 0.843f);
		if ((bool)target)
		{
			Gizmos.color = color;
			Gizmos.DrawLine(base.transform.position, target.position);
		}
		Gizmos.color = color;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawFrustum(base.transform.position, fieldOfView, farClipPlane, nearClipPlane, 1f);
	}

	protected void InternalReset()
	{
		targetComponent = (base.gameObject.GetComponent(typeof(CameraTarget)) as CameraTarget);
		if (targetComponent != null)
		{
			target = targetComponent.Target;
		}
		else {
			CspUtils.DebugLog("**CameraLite base.gameObject=" + base.gameObject.name);
		}
	}
}
