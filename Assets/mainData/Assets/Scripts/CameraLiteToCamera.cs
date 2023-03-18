using UnityEngine;

[AddComponentMenu("Camera/Camera Lite To Camera")]
public class CameraLiteToCamera : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public CameraLite lite;

	private void Awake()
	{
		lite.InitFromMgr();
	}

	private void Start()
	{
		lite.WakeFromMgr();
		lite.CopyToCamera(base.camera);
	}

	private void LateUpdate()
	{
		lite.UpdateFromMgr(Time.deltaTime);
		lite.CopyToCamera(base.camera);
	}

	private void OnEnable()
	{
		lite.WakeFromMgr();
		lite.CopyToCamera(base.camera);
	}

	private void OnDisable()
	{
		lite.SleepFromMgr();
	}
}
