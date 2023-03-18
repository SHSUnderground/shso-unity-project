using UnityEngine;

public class BounceTransitionIn : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool animateX = true;

	public bool animateY = true;

	public bool animateAlpha = true;

	private AnimClipManager apm = new AnimClipManager();

	private Vector3 scale = Vector3.one;

	private Renderer alphaRenderer;

	private void Start()
	{
		alphaRenderer = Utils.GetComponent<Renderer>(base.gameObject, Utils.SearchChildren);
		base.transform.localScale = Vector3.zero;
		if (animateX)
		{
			apm.Add(AnimClipBuilder.Custom.Function(SHSAnimations.GenericPaths.BounceTransitionInX(1f, 0f), UpdateX));
		}
		if (animateY)
		{
			apm.Add(AnimClipBuilder.Custom.Function(SHSAnimations.GenericPaths.BounceTransitionInY(1f, 0f), UpdateY));
		}
		if (animateAlpha)
		{
			apm.Add(AnimClipBuilder.Custom.Function(SHSAnimations.GenericPaths.BounceTransitionInAlpha(1f, 0f), UpdateAlpha));
		}
	}

	private void UpdateX(float x)
	{
		scale.x = x;
	}

	private void UpdateY(float y)
	{
		scale.y = y;
	}

	private void UpdateAlpha(float a)
	{
		if (alphaRenderer != null && alphaRenderer.material != null)
		{
			Color color = alphaRenderer.material.color;
			color.a = a;
			alphaRenderer.material.color = color;
		}
	}

	private void Update()
	{
		apm.Update(Time.deltaTime);
		base.transform.localScale = scale;
	}
}
