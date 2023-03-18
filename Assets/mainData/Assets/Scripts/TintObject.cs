using UnityEngine;

public class TintObject : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum TintFadeType
	{
		NoTintFade,
		TintFadeIn,
		TintFadeOut
	}

	protected static string[] incompatibleShaders = new string[1]
	{
		"Marvel/Base/Diffuse Cutout 2-Sided"
	};

	public bool tintsMeshes = true;

	public bool tintsSkinnedMeshes = true;

	public Color tint;

	public float tintFadeTime;

	public TintFadeType tintFadeType;

	protected Color untint;

	protected float tintFadeStartTime;

	public void StartFade(TintFadeType fadeType)
	{
		tintFadeStartTime = Time.time;
		tintFadeType = fadeType;
		if (tintFadeType == TintFadeType.TintFadeIn)
		{
			DoTint(ref untint);
		}
		else
		{
			DoTint(ref tint);
		}
	}

	public void EndFade()
	{
		if (tintFadeType == TintFadeType.TintFadeOut)
		{
			DoTint(ref untint);
		}
		else
		{
			DoTint(ref tint);
		}
		tintFadeStartTime = 0f;
		tintFadeType = TintFadeType.NoTintFade;
	}

	public void UnTint()
	{
		DoTint(ref untint);
	}

	private void Start()
	{
		untint = new Vector4(1f, 1f, 1f, 1f);
		StartFade(tintFadeType);
	}

	private void OnDisable()
	{
		DoTint(ref untint);
	}

	private void Update()
	{
		if (tintFadeType != 0)
		{
			float num = Time.time - tintFadeStartTime;
			if (num > tintFadeTime)
			{
				EndFade();
			}
			else if (tintFadeType == TintFadeType.TintFadeIn)
			{
				Color colorToApply = untint + (tint - untint) * (num / tintFadeTime);
				DoTint(ref colorToApply);
			}
			else
			{
				Color colorToApply2 = tint + (untint - tint) * (num / tintFadeTime);
				DoTint(ref colorToApply2);
			}
		}
	}

	private void DoTint(ref Color colorToApply)
	{
		if (!(base.transform.root != null) || !(base.transform.root.gameObject.GetComponentInChildren<NoTint>() == null))
		{
			return;
		}
		if (tintsMeshes)
		{
			MeshRenderer[] components = Utils.GetComponents<MeshRenderer>(base.transform.root.gameObject, Utils.SearchChildren);
			if (components != null)
			{
				MeshRenderer[] array = components;
				foreach (MeshRenderer tintMe in array)
				{
					TintRenderer(tintMe, ref colorToApply);
				}
			}
		}
		if (!tintsSkinnedMeshes)
		{
			return;
		}
		SkinnedMeshRenderer[] components2 = Utils.GetComponents<SkinnedMeshRenderer>(base.transform.root.gameObject, Utils.SearchChildren);
		if (components2 != null)
		{
			SkinnedMeshRenderer[] array2 = components2;
			foreach (SkinnedMeshRenderer tintMe2 in array2)
			{
				TintRenderer(tintMe2, ref colorToApply);
			}
		}
	}

	private void TintRenderer(Renderer tintMe, ref Color colorToApply)
	{
		for (int i = 0; i < tintMe.materials.Length; i++)
		{
			Material material = tintMe.materials[i];
			bool flag = true;
			string[] array = incompatibleShaders;
			foreach (string a in array)
			{
				if (a == material.shader.name)
				{
					flag = false;
					break;
				}
			}
			if (flag && material.HasProperty("_Color"))
			{
				material.SetColor("_Color", colorToApply);
			}
		}
	}
}
