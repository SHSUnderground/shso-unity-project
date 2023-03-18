using UnityEngine;

public class ColorShifter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Color startColor = Color.white;

	public Color endColor = Color.black;

	public float duration = 1f;

	private Renderer childRenderer;

	private float startTime;

	private void Start()
	{
		if (base.renderer != null)
		{
			childRenderer = base.renderer;
		}
		else
		{
			childRenderer = (GetComponentInChildren(typeof(Renderer)) as Renderer);
			if (childRenderer == null)
			{
				CspUtils.DebugLog("ColorShifter: No Renderer component attached to object or any of object's children");
			}
		}
		startTime = Time.time;
	}

	private void Update()
	{
		if (childRenderer != null)
		{
			childRenderer.material.color = startColor + (endColor - startColor) * ((Time.time - startTime) % duration);
		}
	}
}
