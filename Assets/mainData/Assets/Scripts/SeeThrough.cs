using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Rendering/See Through")]
[RequireComponent(typeof(Camera))]
public class SeeThrough : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	[Serializable]
	private class FadedInfo
	{
		public Material[] origMaterials;

		public int Layer;

		public Renderer Target;

		public float secsTillReset;

		public Color tintColor;

		public float colorDir = 1f;

		protected float fadeHoldTime = 0.5f;

		protected AnimationCurve fadeOutCurve;

		protected AnimationCurve fadeInCurve;

		protected AnimationCurve fadeInAndOutCurve;

		protected AnimationCurve currentCurve;

		protected float curveAnimTime;

		protected float totalAnimTime;

		public FadedInfo(Renderer r, Color color, AnimationCurve fadeOutCurve, AnimationCurve fadeInCurve, AnimationCurve fadeInAndOutCurve, float fadeHoldTime)
		{
			this.fadeHoldTime = fadeHoldTime;
			this.fadeOutCurve = fadeOutCurve;
			this.fadeInCurve = fadeInCurve;
			this.fadeInAndOutCurve = fadeInAndOutCurve;
			SelectAnimationCurve(this.fadeOutCurve);
			origMaterials = r.materials;
			Layer = r.gameObject.layer;
			Target = r;
			secsTillReset = fadeHoldTime;
			tintColor = color;
		}

		public void Thump()
		{
			SelectAnimationCurve(fadeInAndOutCurve);
		}

		protected void SelectAnimationCurve(AnimationCurve newCurve)
		{
			if (newCurve != null)
			{
				currentCurve = newCurve;
				curveAnimTime = 0f;
				if (currentCurve.length > 1)
				{
					totalAnimTime = currentCurve[currentCurve.length - 1].time;
				}
				else
				{
					totalAnimTime = 1f;
				}
			}
		}

		public bool Update(float deltaTime)
		{
			if (Target == null)
			{
				return false;
			}
			curveAnimTime += deltaTime;
			if (currentCurve == fadeInCurve && curveAnimTime > totalAnimTime)
			{
				ResetTarget();
				return false;
			}
			curveAnimTime = Mathf.Clamp(curveAnimTime, 0f, totalAnimTime);
			secsTillReset -= deltaTime;
			if (secsTillReset <= 0f && currentCurve != fadeInCurve)
			{
				SelectAnimationCurve(fadeInCurve);
			}
			float t = Mathf.Clamp(currentCurve.Evaluate(curveAnimTime), 0f, 1f);
			Material[] materials = Target.materials;
			foreach (Material material in materials)
			{
				if (material.HasProperty("_ColorFade"))
				{
					Color color = tintColor;
					color.a = Mathf.Lerp(1f, tintColor.a, t);
					material.SetColor("_ColorFade", color);
					material.SetColor("_ColorAmbient", RenderSettings.ambientLight);
				}
			}
			return true;
		}

		public void TickleTime()
		{
			colorDir = 1f;
			secsTillReset = fadeHoldTime;
		}

		public void ResetTarget()
		{
			if (!(Target != null))
			{
				return;
			}
			Target.materials = origMaterials;
			Target.gameObject.layer = Layer;
			Component[] components = Target.gameObject.GetComponents<Component>();
			foreach (Component component in components)
			{
				if (component is IFadeInformable)
				{
					((IFadeInformable)component).Restored();
				}
			}
		}
	}

	private Dictionary<Renderer, FadedInfo> ActiveObjects = new Dictionary<Renderer, FadedInfo>();

	public Color TintColor = new Color(0.9f, 0.9f, 1f, 0.5f);

	public LayerMask CullingMask = -1;

	public int FadedLayer = 1;

	public bool FadeChildren = true;

	public float fadeHoldTime = 0.5f;

	public AnimationCurve fadeInCurve;

	public AnimationCurve fadeOutCurve;

	public AnimationCurve fadeInAndOutCurve;

	public Color EditTintColor
	{
		get
		{
			return TintColor;
		}
		set
		{
			if (!Utils.ColorEqual(TintColor, value))
			{
				EditFlush(true, false);
			}
			TintColor = value;
		}
	}

	public LayerMask EditCullingMask
	{
		get
		{
			return CullingMask;
		}
		set
		{
			CullingMask = EditFlush(CullingMask, value);
		}
	}

	public int EditFadedLayer
	{
		get
		{
			return FadedLayer;
		}
		set
		{
			FadedLayer = EditFlush(FadedLayer, value);
		}
	}

	public bool EditFadeChildren
	{
		get
		{
			return FadeChildren;
		}
		set
		{
			FadeChildren = EditFlush(FadeChildren, value);
		}
	}

	public float EditFadeHoldTime
	{
		get
		{
			return fadeHoldTime;
		}
		set
		{
			fadeHoldTime = EditFlush(fadeHoldTime, value);
		}
	}

	private void Start()
	{
		base.enabled = (PlayerPrefs.GetInt("FadeOption", 1) == 1);
	}

	private void OnDisable()
	{
		FlushActiveObjects();
	}

	private void Update()
	{
		IEnumerable<Renderer> transparentObjects = GetTransparentObjects();
		if (transparentObjects != null)
		{
			foreach (Renderer item in transparentObjects)
			{
				FadedInfo value;
				if (ActiveObjects.TryGetValue(item, out value))
				{
					value.TickleTime();
				}
				else
				{
					ActiveObjects.Add(item, Transparentize(item));   // commented out by CSP - this seems to be a workaround for pink shader problem.
				}
			}
		}
		List<Renderer> list = new List<Renderer>(10);
		foreach (KeyValuePair<Renderer, FadedInfo> activeObject in ActiveObjects)
		{
			if (!activeObject.Value.Update(Time.deltaTime))
			{
				list.Add(activeObject.Key);
			}
		}
		foreach (Renderer item2 in list)
		{
			ActiveObjects.Remove(item2);
		}
	}

	private IEnumerable<Renderer> GetTransparentObjects()
	{
		CameraTarget component = Utils.GetComponent<CameraTarget>(base.gameObject);
		if (component == null || component.Target == null)
		{
			return null;
		}
		IEnumerable<Renderer> renderers = GetRenderers2(PlayerOcclusionDetector.Instance);
		return Utils.Distinct(renderers);
	}

	protected IEnumerable<Renderer> GetRenderers(PlayerOcclusionDetector pod)
	{
		IEnumerable<Transform> enumerable = Utils.Map(pod.GetCollidingObjects(), delegate(GameObject hit)
		{
			return hit.transform;
		});
		IEnumerable<Transform> enumerable3;
		if (FadeChildren)
		{			
			IEnumerable<Transform> enumerable2 = Utils.Expand<Transform, Transform>(enumerable, new Converter<Transform, IEnumerable<Transform>>(Utils.WalkTree));
			enumerable3 = enumerable2;
		}
		else
		{
			enumerable3 = enumerable;
		}
		IEnumerable<Transform> roots = enumerable3;
		IEnumerable<Transform> enumerable4;
		if (FadeChildren)
		{
			IEnumerable<Transform> enumerable2 = Utils.Expand<Transform, Transform>(roots, new Converter<Transform, IEnumerable<Transform>>(SeeThrough.WalkStepKids));			
			enumerable4 = enumerable2;
		}
		else
		{
			enumerable4 = enumerable;
		}
		IEnumerable<Transform> input = enumerable4;
		return SkipNulls(Utils.Map(input, delegate(Transform child)
		{
			return child.renderer;
		}));
	}

	public static IEnumerable<Transform> WalkStepKids(Transform root)
	{
		StepChildren kids = Utils.GetComponent<StepChildren>(root.gameObject);
		if (kids != null)
		{
			foreach (GameObject i in kids.stepKids)
			{
				if (i != null && i.transform != null)
				{
					yield return i.transform;
				}
			}
		}
		yield return root;
	}

	public static IEnumerable<Renderer> SkipNulls(IEnumerable<Renderer> input)
	{
		foreach (Renderer r in input)
		{
			if (r != null)
			{
				yield return r;
			}
		}
	}

	protected IEnumerable<Renderer> GetRenderers2(PlayerOcclusionDetector pod)
	{
		foreach (GameObject go in pod.GetCollidingObjects())
		{
			if (go != null)
			{
				Renderer[] renders = go.GetComponentsInChildren<Renderer>();
				Renderer[] array = renders;
				foreach (Renderer r in array)
				{
					if (r != null)
					{
						yield return r;
					}
				}
			}
		}
	}

	protected Shader GetReplacementShader(Shader inShader)
	{
		Shader shader = null;
		switch (inShader.name)
		{
		case "Marvel/Base/Self-Illuminated":
			return Shader.Find("Marvel/Base/Self-Illuminated - Transparent");
		case "Marvel/Lightmap/Vertex Color":
			return Shader.Find("Marvel/Lightmap/Vertex Color - Transparent");
		case "Marvel/Lightmap/Diffuse":
			return Shader.Find("Marvel/Lightmap/Diffuse - Transparent");
		case "Marvel/Lightmap/Self-Illuminated-Tex":
			return Shader.Find("Marvel/Lightmap/Self-Illuminated-Tex - Transparent");
		case "Marvel/AO/Vertex Color":
			return Shader.Find("Marvel/AO/Vertex Color - Transparent");
		case "Marvel/AO/Diffuse":
			return Shader.Find("Marvel/AO/Diffuse - Transparent");
		case "Marvel/Base/Self-Illuminated-Tex":
			return Shader.Find("Marvel/Base/Self-Illuminated-Tex - Transparent");
		case "Particles/Alpha Blended":
			return inShader;
		default:
			return inShader;
		}
	}

	public void CollisionWithFaded(GameObject collidingObject)
	{
		CspUtils.DebugLog("BOING! Player hit building: " + collidingObject.name);
		Renderer renderer = collidingObject.renderer;
		foreach (KeyValuePair<Renderer, FadedInfo> activeObject in ActiveObjects)
		{
			if (activeObject.Key == renderer)
			{
				FadedInfo value = activeObject.Value;
				if (value != null)
				{
					value.Thump();
				}
				break;
			}
		}
	}

	private FadedInfo Transparentize(Renderer r)
	{
		FadedInfo result = new FadedInfo(r, TintColor, fadeOutCurve, fadeInCurve, fadeInAndOutCurve, fadeHoldTime);
		Material[] array = new Material[r.materials.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new Material(r.materials[i]);
			array[i].shader = GetReplacementShader(array[i].shader);
		}
		r.materials = array;
		r.gameObject.layer = FadedLayer;
		Component[] components = r.gameObject.GetComponents<Component>();
		foreach (Component component in components)
		{
			if (component is IFadeInformable)
			{
				((IFadeInformable)component).Faded();
			}
		}
		return result;
	}

	private void FlushActiveObjects()
	{
		EditFlush(true, false);
	}

	private T EditFlush<T>(T oldValue, T newValue) where T : struct
	{
		if (oldValue.Equals(newValue))
		{
			return newValue;
		}
		foreach (FadedInfo value in ActiveObjects.Values)
		{
			value.ResetTarget();
		}
		ActiveObjects.Clear();
		return newValue;
	}
}
