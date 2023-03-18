using UnityEngine;

[AddComponentMenu("Camera/Camera Target")]
public class CameraTarget : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Transform Target;

	public static void SwapAllTargets(Transform oldTarget, Transform newTarget, bool resetCameraLite)
	{
		CameraTarget[] array = Utils.FindObjectsOfType<CameraTarget>();
		CameraTarget[] array2 = array;
		foreach (CameraTarget cameraTarget in array2)
		{
			if (!(cameraTarget.Target == oldTarget))
			{
				continue;
			}
			cameraTarget.Target = newTarget;
			if (resetCameraLite)
			{
				CameraLite component = cameraTarget.GetComponent<CameraLite>();
				if (component != null)
				{
					component.Reset();
				}
			}
		}
	}
}
