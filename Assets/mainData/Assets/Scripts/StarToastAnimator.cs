using System.Collections;
using UnityEngine;

public class StarToastAnimator : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float lifetime = 2f;

	protected string text;

	public Color frontColor;

	public Color middleColor;

	public Color backColor;

	protected MeshRenderer frontMesh;

	protected MeshRenderer middleMesh;

	protected MeshRenderer backMesh;

	private TextMesh[] meshes;

	private MeshRenderer[] textrenderers;

	private void Start()
	{
		text = AppShell.Instance.stringTable["#notification_10_stars"];
		Shader shader = Shader.Find("GUI/Text Shader Z");
		textrenderers = Utils.GetComponents<MeshRenderer>(base.gameObject, Utils.SearchChildren);
		meshes = Utils.GetComponents<TextMesh>(base.gameObject, Utils.SearchChildren);
		TextMesh[] array = meshes;
		foreach (TextMesh textMesh in array)
		{
			textMesh.text = text;
		}
		MeshRenderer[] array2 = textrenderers;
		foreach (MeshRenderer meshRenderer in array2)
		{
			meshRenderer.material.shader = shader;
			if (meshRenderer.gameObject.name.Contains("front"))
			{
				frontMesh = meshRenderer;
				meshRenderer.material.color = frontColor;
			}
			else if (meshRenderer.gameObject.name.Contains("middle"))
			{
				middleMesh = meshRenderer;
				meshRenderer.material.color = backColor;
			}
			else if (meshRenderer.gameObject.name.Contains("back"))
			{
				backMesh = meshRenderer;
				meshRenderer.material.color = backColor;
			}
			else
			{
				CspUtils.DebugLog("Star Toast Animator, found mystery meshrenderer called '" + meshRenderer.gameObject.name + "'!");
			}
		}
		StartCoroutine(AnimateColor());
		StartCoroutine(AnimatePosition());
		StartCoroutine(AutoKill());
	}

	private IEnumerator AnimateColor()
	{
		float alpha = 1f;
		while (true)
		{
			SetAlpha(alpha);
			yield return new WaitForSeconds(0.1f);
			SetAlpha(alpha);
			yield return new WaitForSeconds(0.1f);
			alpha = Mathf.Max(0f, alpha - 0.2f / lifetime);
			SetAlpha(alpha);
		}
	}

	private void SetAlpha(float newAlpha)
	{
		frontMesh.material.color = new Color(frontColor.r, frontColor.g, frontColor.b, newAlpha);
		middleMesh.material.color = new Color(middleColor.r, middleColor.g, middleColor.b, newAlpha);
		backMesh.material.color = new Color(backColor.r, backColor.g, backColor.b, newAlpha);
	}

	private IEnumerator AnimatePosition()
	{
		float delta2 = 3f;
		while (true)
		{
			Vector3 position = base.transform.position;
			position.y += delta2 * Time.deltaTime;
			delta2 -= 2f * Time.deltaTime;
			delta2 = Mathf.Max(delta2, 0f);
			base.transform.position = position;
			yield return 0;
		}
	}

	private IEnumerator AutoKill()
	{
		yield return new WaitForSeconds(lifetime);
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}
}
