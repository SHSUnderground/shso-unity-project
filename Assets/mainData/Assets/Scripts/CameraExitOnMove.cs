using UnityEngine;

[AddComponentMenu("Camera/Camera Exit On Move")]
public class CameraExitOnMove : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public CameraLite nextCamera;

	public float blendTime = 1f;

	public bool exitOnMove = true;

	public bool exitOnMouseWheel = true;

	public bool exitOnTarget;

	protected CameraTarget targetComponent;

	protected float safeWindow = 1f;

	private void Start()
	{
		targetComponent = Utils.GetComponent<CameraTarget>(base.gameObject);
	}

	private void Update()
	{
		safeWindow -= Time.deltaTime;
		if (!(safeWindow > 0f))
		{
			if (exitOnMouseWheel && SHSInput.GetMouseWheelDelta() != 0f)
			{
				GotoNextCamera();
			}
			else if (exitOnMove && GetTargetSpeed() > 1f)
			{
				GotoNextCamera();
			}
			else if (exitOnTarget && targetComponent != null && targetComponent.Target != null)
			{
				GotoNextCamera();
			}
		}
	}

	protected void GotoNextCamera()
	{
		CameraLiteManager instance = CameraLiteManager.Instance;
		if (!(instance == null))
		{
			instance.ReplaceCamera(nextCamera, blendTime);
			base.enabled = false;
		}
	}

	protected float GetTargetSpeed()
	{
		if (targetComponent == null)
		{
			return 0f;
		}
		if (targetComponent.Target == null)
		{
			return 0f;
		}
		ShsCharacterController shsCharacterController = targetComponent.Target.gameObject.GetComponent(typeof(ShsCharacterController)) as ShsCharacterController;
		if (shsCharacterController == null)
		{
			shsCharacterController = (targetComponent.Target.root.gameObject.GetComponent(typeof(ShsCharacterController)) as ShsCharacterController);
			if (shsCharacterController == null)
			{
				return 0f;
			}
		}
		Vector3 velocity = shsCharacterController.velocity;
		velocity.y = 0f;
		return velocity.magnitude;
	}
}
