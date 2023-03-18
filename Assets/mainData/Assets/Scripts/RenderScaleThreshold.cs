using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Rendering/RenderScaleThreshold")]
public class RenderScaleThreshold : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float minimumLossyScale = 0.01f;

	public float maximumLossyScale = 10f;

	private bool hidden;

	private List<Renderer> hiddenRenderers = new List<Renderer>();

	private void LateUpdate()
	{
		Vector3 lossyScale = base.transform.lossyScale;
		bool flag = lossyScale.x < minimumLossyScale || lossyScale.y < minimumLossyScale || lossyScale.z < minimumLossyScale || lossyScale.x > maximumLossyScale || lossyScale.y > maximumLossyScale || lossyScale.z > maximumLossyScale;
		if (flag && !hidden)
		{
			hidden = true;
			HideRenderers();
		}
		else if (!flag && hidden)
		{
			hidden = false;
			ShowRenderers();
		}
	}

	private void HideRenderers()
	{
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = false;
			hiddenRenderers.Add(renderer);
		}
	}

	private void ShowRenderers()
	{
		foreach (Renderer hiddenRenderer in hiddenRenderers)
		{
			hiddenRenderer.enabled = true;
		}
		hiddenRenderers.Clear();
	}
}
