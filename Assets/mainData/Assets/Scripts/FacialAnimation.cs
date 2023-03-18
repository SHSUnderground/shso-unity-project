using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("Rendering/Facial Animation")]
public class FacialAnimation : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum Expression
	{
		Normal,
		Happy,
		Surprised,
		Aggressive,
		Squint,
		Blink,
		Custom1,
		Custom2
	}

	public Expression facialExpression;

	public Expression defaultFacialExpression;

	public int faceWidth = 4;

	public int faceHeight = 2;

	protected Expression currentExpression;

	protected List<Material> faceMaterials;

	protected float normalTime;

	protected bool warningSent;

	[CompilerGenerated]
	private bool _003CPersistOnAnimEnd_003Ek__BackingField;

	public bool PersistOnAnimEnd
	{
		[CompilerGenerated]
		get
		{
			return _003CPersistOnAnimEnd_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CPersistOnAnimEnd_003Ek__BackingField = value;
		}
	}

	private void Awake()
	{
		PersistOnAnimEnd = false;
	}

	private void Start()
	{
		GetMaterials();
	}

	private void GetMaterials()
	{
		faceMaterials = new List<Material>();
		Component[] componentsInChildren = GetComponentsInChildren(typeof(Renderer));
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Renderer renderer = (Renderer)array[i];
			Material[] materials = renderer.materials;
			foreach (Material material in materials)
			{
				string text = material.name.ToLower();
				if (text.Contains("face") || text.Contains("head"))
				{
					faceMaterials.Add(material);
					break;
				}
			}
		}
		if (faceMaterials.Count == 0)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (normalTime > 0f)
		{
			if (HqController2.Instance != null && !HqController2.Instance.IsInPlayMode())
			{
				normalTime += Time.deltaTime;
			}
			if (Time.time > normalTime)
			{
				SetFacialExpression(defaultFacialExpression);
			}
		}
	}

	public void SetFacialExpression(Expression newExpression)
	{
		if (currentExpression != newExpression && faceMaterials != null && faceMaterials.Count > 0)
		{
			currentExpression = newExpression;
			facialExpression = newExpression;
			int num = faceWidth;
			int num2 = faceHeight;
			Vector2 zero = Vector2.zero;
			zero.x = (float)((int)newExpression % num) / (float)num;
			zero.y = (float)((int)newExpression / num) / (float)num2;
			foreach (Material faceMaterial in faceMaterials)
			{
				faceMaterial.SetTextureOffset("_MainTex", zero);
			}
		}
	}

	public void ResetToNormal()
	{
		SetFacialExpression(defaultFacialExpression);
		PersistOnAnimEnd = false;
	}

	public void OnMaterialsChanged()
	{
		GetMaterials();
	}

	[AnimTag("face")]
	public void OnFaceAnimTag(AnimationEvent evt)
	{
		string[] array = evt.stringParameter.Split(':');
		string text = array[0];
		float num = 0f;
		if (array.Length > 1)
		{
			num = float.Parse(array[1]);
		}
		switch (text)
		{
		case "normal":
			SetFacialExpression(Expression.Normal);
			break;
		case "happy":
			SetFacialExpression(Expression.Happy);
			break;
		case "surprised":
			SetFacialExpression(Expression.Surprised);
			break;
		case "aggressive":
			SetFacialExpression(Expression.Aggressive);
			break;
		case "squint":
			SetFacialExpression(Expression.Squint);
			break;
		case "blink":
			SetFacialExpression(Expression.Blink);
			break;
		case "custom1":
			SetFacialExpression(Expression.Custom1);
			break;
		case "custom2":
			SetFacialExpression(Expression.Custom2);
			break;
		}
		if (num > 0f)
		{
			if (!PersistOnAnimEnd || num < evt.animationState.length - evt.animationState.time)
			{
				normalTime = Time.time + num;
			}
			else
			{
				normalTime = 0f;
			}
		}
		else if (text != "normal" && !warningSent)
		{
			CspUtils.DebugLog("Facial animations for " + base.gameObject.name + " do not have a duration.  Animations for this character should be reimported and rebundled.");
			CspUtils.DebugLog("onfaceanimtag : " + evt.stringParameter);
			warningSent = true;
		}
	}
}
