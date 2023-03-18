using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Rendering/Depth Camera")]
public class DepthCamera : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private DepthTextureMode oldDepthMode;

	private void OnEnable()
	{
		oldDepthMode = base.camera.depthTextureMode;
		base.camera.depthTextureMode = DepthTextureMode.Depth;
	}

	private void OnDisable()
	{
		base.camera.depthTextureMode = oldDepthMode;
	}
}
