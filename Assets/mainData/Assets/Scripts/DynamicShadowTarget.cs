using UnityEngine;

[RequireComponent(typeof(Renderer))]
[AddComponentMenu("Rendering/Dynamic Shadow Target")]
public class DynamicShadowTarget : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	[HideInInspector]
	public int LatestVisibleFrame;

	public RenderTexture ShadowBuffer;

	private void OnWillRenderObject()
	{
		LatestVisibleFrame = Time.frameCount;
	}

	private void OnEnable()
	{
		DynamicShadowController.RegisterShadowTarget(this);
	}

	private void OnDisable()
	{
		DynamicShadowController.UnregisterShadowTarget(this);
	}
}
