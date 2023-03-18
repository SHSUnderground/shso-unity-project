using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class TimLandMouseLook : MonoBehaviour, ICaptureHandler
{
	public enum RotationAxes
	{
		MouseXAndY,
		MouseX,
		MouseY
	}

	public RotationAxes axes;

	public float sensitivityX = 15f;

	public float sensitivityY = 15f;

	public float minimumX = -360f;

	public float maximumX = 360f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	private float rotationX;

	private float rotationY;

	private GameObject overTarget;

	private GameObject selTarget;

	private Quaternion originalRotation;

	private ICaptureManager manager;

	public ICaptureManager Manager
	{
		get
		{
			return manager;
		}
		set
		{
			manager = value;
		}
	}

	private void SetTarget(GameObject target)
	{
		SetTarget(target, new Color(1f, 1f, 1f, 1f));
	}

	private void SetTarget(GameObject target, Color color)
	{
		CspUtils.DebugLog((!(target == null)) ? (" Now targeting " + target.name) : ("No longer targeting " + selTarget.name));
		SkinnedMeshRenderer skinnedMeshRenderer = ((!(target == null)) ? target : selTarget).GetComponentInChildren(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
		skinnedMeshRenderer.material.color = color;
		selTarget = target;
	}

	private void Update()
	{
		if (!SHSInput.AllowInput(this))
		{
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(SHSInput.mousePosition);
		int layerMask = 512;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, layerMask))
		{
			if (overTarget != hitInfo.collider.gameObject)
			{
				CspUtils.DebugLog("Im now over " + overTarget);
				overTarget = hitInfo.collider.gameObject;
			}
			if (SHSInput.GetMouseButtonUp(SHSInput.MouseButtonType.Left) && selTarget != hitInfo.collider.gameObject)
			{
				if (selTarget != null)
				{
					SetTarget(null);
				}
				SetTarget(hitInfo.collider.gameObject, new Color(0f, 1f, 0f, 1f));
			}
		}
		else
		{
			if (overTarget != null)
			{
				CspUtils.DebugLog("im NOT over " + overTarget.name);
				overTarget = null;
			}
			if (selTarget != null && SHSInput.GetMouseButtonUp(SHSInput.MouseButtonType.Left))
			{
				SetTarget(null);
			}
		}
		if (axes == RotationAxes.MouseXAndY && SHSInput.GetMouseButton(SHSInput.MouseButtonType.Left))
		{
			rotationX += Input.GetAxis("Mouse X") * sensitivityX;
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationX = ClampAngle(rotationX, minimumX, maximumX);
			rotationY = ClampAngle(rotationY, minimumY, maximumY);
			Quaternion rhs = Quaternion.AngleAxis(rotationX, Vector3.up);
			Quaternion rhs2 = Quaternion.AngleAxis(rotationY, Vector3.left);
			base.transform.localRotation = originalRotation * rhs * rhs2;
		}
		else if (axes == RotationAxes.MouseX && Input.GetKey(KeyCode.Space))
		{
			rotationX += Input.GetAxis("Mouse X") * sensitivityX;
			rotationX = ClampAngle(rotationX, minimumX, maximumX);
			Quaternion rhs3 = Quaternion.AngleAxis(rotationX, Vector3.up);
			base.transform.localRotation = originalRotation * rhs3;
		}
		else if (Input.GetKey(KeyCode.Space))
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = ClampAngle(rotationY, minimumY, maximumY);
			Quaternion rhs4 = Quaternion.AngleAxis(rotationY, Vector3.left);
			base.transform.localRotation = originalRotation * rhs4;
		}
	}

	private void Start()
	{
		if ((bool)base.rigidbody)
		{
			base.rigidbody.freezeRotation = true;
		}
		originalRotation = base.transform.localRotation;
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public CaptureHandlerResponse HandleCapture(SHSKeyCode code)
	{
		return CaptureHandlerResponse.Block;
	}

	public virtual void OnCaptureAcquired()
	{
	}

	public virtual void OnCaptureUnacquired()
	{
	}
}
