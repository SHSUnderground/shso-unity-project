using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrawlerPopupAnimator : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum ScaleType
	{
		ScaleNone,
		ScaleLinear,
		ScaleExponential
	}

	protected struct TextMeshPopup
	{
		public TextMesh textMesh;

		public Color textMeshColor;

		public TextMeshPopup(TextMesh textMesh, Color textMeshColor)
		{
			this.textMesh = textMesh;
			this.textMeshColor = textMeshColor;
		}
	}

	protected struct MeshRendererPopup
	{
		public MeshRenderer mesh;

		public Color meshColor;

		public MeshRendererPopup(MeshRenderer mesh, Color meshColor)
		{
			this.mesh = mesh;
			this.meshColor = meshColor;
		}
	}

	public float speed = 1f;

	public float duration = 4f;

	public float fadetime = 1f;

	public ScaleType scaleType;

	public Vector4 maxScale = new Vector4(0.2f, 0.2f, 0.2f, 0f);

	public float scaleDuration = 0.4f;

	public float scaleExponential = 3f;

	protected float startTime;

	protected float scaleStartTime;

	protected Vector4 minScale;

	protected float halfScaleDuration;

	protected Transform parent;

	protected Vector3 offsetFromParent;

	protected List<TextMeshPopup> textMeshList;

	protected List<MeshRendererPopup> meshList;

	public void End()
	{
		EnableRenderers(false);
		parent = null;
		CombatController.addPopup(base.gameObject.name, this);
		base.gameObject.active = false;
	}

	public virtual void SetText(string newPopupText)
	{
		EnableRenderers(true);
	}

	public void AttachToParent(Transform parent)
	{
		this.parent = parent;
		if (parent != null)
		{
			Vector3 position = base.transform.position;
			float x = position.x;
			Vector3 position2 = parent.position;
			float x2 = x - position2.x;
			Vector3 position3 = base.transform.position;
			float z = position3.z;
			Vector3 position4 = parent.position;
			offsetFromParent = new Vector3(x2, 0f, z - position4.z);
		}
	}

	public void EnableRenderers(bool enable)
	{
		foreach (TextMeshPopup textMesh in textMeshList)
		{
			textMesh.textMesh.renderer.enabled = enable;
			if (enable)
			{
				textMesh.textMesh.renderer.material.color = textMesh.textMeshColor;
			}
		}
		foreach (MeshRendererPopup mesh in meshList)
		{
			mesh.mesh.renderer.enabled = enable;
			if (enable)
			{
				mesh.mesh.renderer.material.color = mesh.meshColor;
			}
		}
	}

	public void DelayPopup(float time, CombatController locator, bool useLocator)
	{
		base.enabled = false;
		EnableRenderers(false);
		StartCoroutine(DelayPopupUntilTime(time, locator, useLocator));
	}

	public void StartPulseAnimation()
	{
		if (CanPulse() && textMeshList.Count > 0)
		{
			scaleStartTime = Time.time;
			halfScaleDuration = scaleDuration / 2f;
			Vector3 localScale = base.transform.localScale;
			float x = localScale.x;
			Vector3 localScale2 = base.transform.localScale;
			float y = localScale2.y;
			Vector3 localScale3 = base.transform.localScale;
			float z = localScale3.z;
			TextMeshPopup textMeshPopup = textMeshList[0];
			minScale = new Vector4(x, y, z, textMeshPopup.textMesh.renderer.material.renderQueue);
			maxScale.w = minScale.w * (maxScale.x / minScale.x);
		}
	}

	public void EndPulseAnimation()
	{
		scaleStartTime = 0f;
		halfScaleDuration = 0f;
		base.transform.localScale = new Vector3(minScale.x, minScale.y, minScale.z);
		foreach (TextMeshPopup textMesh in textMeshList)
		{
			textMesh.textMesh.renderer.material.renderQueue = (int)minScale.w;
		}
	}

	public bool CanPulse()
	{
		return scaleType != 0 && scaleDuration > 0f;
	}

	public bool IsFading()
	{
		return Time.time > startTime + (duration - fadetime);
	}

	public bool IsPulsing()
	{
		return scaleStartTime > 0f;
	}

	protected void AddMesh(MeshRenderer mesh, Color meshColor)
	{
		meshList.Add(new MeshRendererPopup(mesh, meshColor));
	}

	protected void AddTextMesh(TextMesh textMesh, Color textMeshColor)
	{
		textMeshList.Add(new TextMeshPopup(textMesh, textMeshColor));
	}

	protected void UpdatePulseAnimation()
	{
		float num = Time.time - scaleStartTime;
		if (num > scaleDuration)
		{
			EndPulseAnimation();
			return;
		}
		float timeElapsed = num;
		if (num >= halfScaleDuration)
		{
			timeElapsed = num - halfScaleDuration;
		}
		Vector3 localScale = base.transform.localScale;
		float x = localScale.x;
		Vector3 localScale2 = base.transform.localScale;
		float y = localScale2.y;
		Vector3 localScale3 = base.transform.localScale;
		float z = localScale3.z;
		TextMeshPopup textMeshPopup = textMeshList[0];
		Vector4 vector = new Vector4(x, y, z, textMeshPopup.textMesh.renderer.material.renderQueue);
		switch (scaleType)
		{
		case ScaleType.ScaleLinear:
			vector = DoLinearScale(ref minScale, ref maxScale, timeElapsed, halfScaleDuration, num < halfScaleDuration);
			break;
		case ScaleType.ScaleExponential:
			vector = DoExponentialScale(ref minScale, ref maxScale, timeElapsed, halfScaleDuration, num < halfScaleDuration);
			break;
		}
		base.transform.localScale = new Vector3(vector.x, vector.y, vector.z);
		foreach (TextMeshPopup textMesh in textMeshList)
		{
			textMesh.textMesh.renderer.material.renderQueue = (int)vector.w;
		}
	}

	protected void UpdateFadeAnimation()
	{
		float num = Mathf.Max(startTime + duration - Time.time, 0f);
		float num2 = num / fadetime;
		foreach (TextMeshPopup textMesh in textMeshList)
		{
			textMesh.textMesh.renderer.material.color = new Color(textMesh.textMeshColor.r, textMesh.textMeshColor.g, textMesh.textMeshColor.b, textMesh.textMeshColor.a * num2);
		}
		foreach (MeshRendererPopup mesh in meshList)
		{
			mesh.mesh.renderer.material.color = new Color(mesh.meshColor.r, mesh.meshColor.g, mesh.meshColor.b, mesh.meshColor.a * num2);
		}
	}

	protected void UpdatePositionAnimation()
	{
		Vector3 position = base.transform.position;
		position.y += speed * Time.deltaTime;
		if (parent != null)
		{
			Vector3 position2 = parent.position;
			position.x = position2.x + offsetFromParent.x;
			Vector3 position3 = parent.position;
			position.z = position3.z + offsetFromParent.z;
		}
		base.transform.position = position;
	}

	protected Vector4 DoLinearScale(ref Vector4 minScale, ref Vector4 maxScale, float timeElapsed, float timeTotal, bool scaleUp)
	{
		Vector4 result = (!scaleUp) ? (maxScale + (minScale - maxScale) / timeTotal * timeElapsed) : (minScale + (maxScale - minScale) / timeTotal * timeElapsed);
		result.x = Mathf.Clamp(result.x, minScale.x, maxScale.x);
		result.y = Mathf.Clamp(result.y, minScale.y, maxScale.y);
		result.z = Mathf.Clamp(result.z, minScale.z, maxScale.z);
		result.w = Mathf.Clamp(result.w, minScale.w, maxScale.w);
		return result;
	}

	protected Vector4 DoExponentialScale(ref Vector4 minScale, ref Vector4 maxScale, float timeElapsed, float timeTotal, bool scaleUp)
	{
		float f = timeElapsed / timeTotal;
		float d = Mathf.Pow(f, scaleExponential);
		Vector4 result = (!scaleUp) ? (maxScale + (minScale - maxScale) * d) : (minScale + (maxScale - minScale) * d);
		result.x = Mathf.Clamp(result.x, minScale.x, maxScale.x);
		result.y = Mathf.Clamp(result.y, minScale.y, maxScale.y);
		result.z = Mathf.Clamp(result.z, minScale.z, maxScale.z);
		result.w = Mathf.Clamp(result.w, minScale.w, maxScale.w);
		return result;
	}

	protected IEnumerator DelayPopupUntilTime(float time, CombatController locator, bool useLocator)
	{
		yield return new WaitForSeconds(time);
		base.enabled = true;
		EnableRenderers(true);
		if (!(locator != null))
		{
			yield break;
		}
		if (useLocator)
		{
			Vector3 location;
			if (CombatController.getPopupLocation(locator.gameObject, out location))
			{
				base.transform.position = location;
			}
		}
		else
		{
			base.transform.position = locator.getPopupLocation();
		}
	}

	protected virtual void Awake()
	{
		textMeshList = new List<TextMeshPopup>();
		meshList = new List<MeshRendererPopup>();
	}

	private void OnEnable()
	{
		startTime = Time.time;
		StartPulseAnimation();
	}

	private void OnDisable()
	{
		if (IsPulsing())
		{
			EndPulseAnimation();
		}
	}

	private void Update()
	{
		if (Time.time > startTime + duration)
		{
			End();
			return;
		}
		if (IsFading())
		{
			UpdateFadeAnimation();
		}
		if (IsPulsing())
		{
			UpdatePulseAnimation();
		}
		UpdatePositionAnimation();
	}
}
