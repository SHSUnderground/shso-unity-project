using System.Collections;
using UnityEngine;

public class ScaleWobble : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum ScaleAxis
	{
		X,
		Y,
		Z
	}

	public float endScale = 1f;

	public ScaleAxis scaleAxis = ScaleAxis.Y;

	private void Start()
	{
		StartCoroutine(Go());
	}

	private void SetScale(float scale)
	{
		Vector3 localScale = base.transform.localScale;
		if (scaleAxis == ScaleAxis.X)
		{
			localScale.x = scale;
		}
		else if (scaleAxis == ScaleAxis.Y)
		{
			localScale.y = scale;
		}
		else
		{
			localScale.z = scale;
		}
		base.transform.localScale = localScale;
	}

	private float GetScale()
	{
		if (scaleAxis == ScaleAxis.X)
		{
			Vector3 localScale = base.transform.localScale;
			return localScale.x;
		}
		if (scaleAxis == ScaleAxis.Y)
		{
			Vector3 localScale2 = base.transform.localScale;
			return localScale2.y;
		}
		Vector3 localScale3 = base.transform.localScale;
		return localScale3.z;
	}

	public IEnumerator Go()
	{
		SetScale(0f);
		yield return StartCoroutine(ScaleTo(endScale * 1.4f, 0.1f));
		yield return StartCoroutine(ScaleTo(endScale * 0.9f, 0.2f));
		yield return StartCoroutine(ScaleTo(endScale * 1.05f, 0.1f));
		yield return StartCoroutine(ScaleTo(endScale, 0.05f));
	}

	private IEnumerator ScaleTo(float scale, float time)
	{
		float startScale = GetScale();
		float startTime = Time.time;
		float newScale = startScale;
		while ((scale > startScale && newScale < scale) || (scale < startScale && newScale > scale))
		{
			float t = (Time.time - startTime) / time;
			newScale = t * scale + (1f - t) * startScale;
			SetScale(newScale);
			yield return 0;
		}
		SetScale(scale);
	}
}
