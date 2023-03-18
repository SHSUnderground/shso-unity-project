using System.Collections;
using UnityEngine;

public class StatusBubble : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Renderer icon;

	public readonly float kFadeRate = 2f;

	public float IconAlpha
	{
		get
		{
			float result;
			if (icon != null)
			{
				Color color = icon.material.color;
				result = color.a;
			}
			else
			{
				result = -1f;
			}
			return result;
		}
		set
		{
			if (icon != null)
			{
				Color color = icon.material.color;
				color.a = value;
				icon.material.color = color;
			}
		}
	}

	private void Awake()
	{
		IconAlpha = 0f;
	}

	public void SetIconTexture(Texture newIcon)
	{
		if (icon != null)
		{
			icon.material.SetTexture("_MainTex", newIcon);
		}
	}

	public void SetIconTextureByName(string newIcon)
	{
		Texture texture = GUIManager.Instance.BundleManager.LoadAsset("gameworld_bundle", newIcon) as Texture;
		if (texture != null)
		{
			SetIconTexture(texture);
		}
		else
		{
			CspUtils.DebugLog("Could not find icon texture <" + newIcon + "> when trying to set player status icon.");
		}
	}

	public void Update()
	{
		base.transform.rotation = Camera.main.transform.rotation;
	}

	public void FadeIn()
	{
		StopAllCoroutines();
		StartCoroutine(Fade(1f));
	}

	public void FadeOut()
	{
		StopAllCoroutines();
		StartCoroutine(Fade(-1f));
	}

	private IEnumerator Fade(float direction)
	{
		bool done = false;
		while (!done)
		{
			yield return 0;
			float newAlpha = IconAlpha + direction * kFadeRate * Time.deltaTime;
			if (direction < 0f && newAlpha <= 0f)
			{
				IconAlpha = 0f;
				done = true;
			}
			else if (direction > 0f && newAlpha >= 1f)
			{
				IconAlpha = 1f;
				done = true;
			}
			else
			{
				IconAlpha = newAlpha;
			}
		}
	}
}
