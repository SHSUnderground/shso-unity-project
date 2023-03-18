using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldMaterialController : CharacterMaterialController, IComponentTimeInit
{
	public float delay = 0.25f;

	public float duration = 30f;

	public Material overrideMaterial;

	public Material goldMat;

	public Material greyscaleMat;

	public Material highPassMat;

	public Material highPassAdd;

	public bool includeGreyscale = true;

	public float goldRampOffset = -0.5f;

	public bool connect;

	public bool disconnect;

	protected bool connected;

	protected SkinnedMeshRenderer[] skinnedMeshes;

	protected List<RenderTexture> convertedTextures;

	protected List<Material[]> savedMaterials;

	protected EffectSequence sequenceOnApply;

	protected EffectSequence sequenceOnRevert;

	protected GameObject sequenceParent;

	protected EffectSequence sequenceInstance;

	protected bool awaitingFullScreenChange;

	public void OnEnable()
	{
		if (skinnedMeshes == null)
		{
			skinnedMeshes = Utils.GetComponents<SkinnedMeshRenderer>(base.gameObject, Utils.SearchChildren);
			if (skinnedMeshes == null || skinnedMeshes.Length == 0)
			{
				CspUtils.DebugLogError("Unable to connect gold controller to " + base.gameObject.name + ".  SkinnedMeshRenderer component not found.");
			}
		}
	}

	public void OnDisable()
	{
		Disconnect();
	}

	public void SetDelay(float time)
	{
		delay = time;
	}

	public void SetDuration(float time)
	{
		duration = time;
	}

	public void Start()
	{
		if (AppShell.Instance != null)
		{
			AppShell.Instance.OnAboutToChangeFullscreenState += OnAboutToChangeFullscreenState;
			AppShell.Instance.OnChangedFullscreenState += OnChangedFullscreenState;
		}
		StartCoroutine(TimedConnect(delay));
		StartCoroutine(TimedDestroy(duration));
	}

	public void SetApplySequence(EffectSequence sequence)
	{
		sequenceOnApply = sequence;
	}

	public void SetRevertSequence(EffectSequence sequence)
	{
		sequenceOnRevert = sequence;
	}

	public void SetSequenceOwner(GameObject owner)
	{
		sequenceParent = owner;
	}

	private IEnumerator TimedConnect(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		Connect();
	}

	private IEnumerator TimedDestroy(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		Object.DestroyImmediate(this);
	}

	public override bool Connect()
	{
		if (!base.Connect())
		{
			return false;
		}
		if (connected)
		{
			return true;
		}
		CreateMaterials();
		sequenceInstance = EffectSequence.PlayOneShot(sequenceOnApply, (!(sequenceParent != null)) ? base.gameObject : sequenceParent);
		connected = true;
		if (AppShell.Instance != null && AppShell.Instance.EventMgr != null)
		{
			AppShell.Instance.EventMgr.Fire(base.gameObject, new EntityEngoldenMessage(base.gameObject, true));
		}
		return true;
	}

	public override bool Disconnect()
	{
		if (!base.Disconnect())
		{
			return false;
		}
		if (!connected)
		{
			return true;
		}
		if (!awaitingFullScreenChange)
		{
			RestoreMaterials();
		}
		if (sequenceInstance != null)
		{
			Object.Destroy(sequenceInstance.gameObject);
		}
		EffectSequence.PlayOneShot(sequenceOnRevert, (!(sequenceParent != null)) ? base.gameObject : sequenceParent);
		if (AppShell.Instance != null && AppShell.Instance.EventMgr != null)
		{
			AppShell.Instance.EventMgr.Fire(base.gameObject, new EntityEngoldenMessage(base.gameObject, false));
		}
		connected = false;
		Object.Destroy(this);
		return true;
	}

	private void CreateMaterials()
	{
		if (goldMat == null)
		{
			Texture2D texture = Resources.Load("Shaders/Textures/gold_ramp", typeof(Texture2D)) as Texture2D;
			goldMat = new Material(Shader.Find("GoldShader"));
			goldMat.SetTexture("_RampTex", texture);
		}
		if (greyscaleMat == null)
		{
			greyscaleMat = new Material(Shader.Find("GoldShader"));
		}
		if (highPassMat == null)
		{
			highPassMat = new Material(Shader.Find("HighPassFilter"));
		}
		if (highPassAdd == null)
		{
			highPassAdd = new Material(Shader.Find("HighPassAdd"));
		}
		if (overrideMaterial == null)
		{
			overrideMaterial = new Material(Shader.Find("Specular"));
		}
		ResetResources();
		for (int i = 0; i < skinnedMeshes.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = skinnedMeshes[i];
			Material[] materials = skinnedMeshRenderer.materials;
			Material[] array = new Material[materials.Length];
			for (int j = 0; j < materials.Length; j++)
			{
				Material material = materials[j];
				Material material2 = null;
				material2 = ((!(material.shader.name == "Marvel/Base/Diffuse 2-Sided") && !(material.shader.name == "Marvel/Base/Diffuse Cutout 2-Sided")) ? new Material(overrideMaterial) : new Material(material));
				Texture texture2 = material.GetTexture("_MainTex");
				if (texture2 != null)
				{
					RenderTexture renderTexture = new RenderTexture(texture2.width, texture2.height, 0);
					RenderTexture temporary = RenderTexture.GetTemporary(texture2.width, texture2.height, 0);
					ShaderUtil.Blit(texture2, temporary, highPassMat);
					if (includeGreyscale)
					{
						ShaderUtil.Blit(texture2, temporary, greyscaleMat, 0);
					}
					goldMat.SetFloat("_Offset", goldRampOffset);
					ShaderUtil.Blit(temporary, renderTexture, goldMat, 1);
					RenderTexture.ReleaseTemporary(temporary);
					material2.SetTexture("_MainTex", renderTexture);
					convertedTextures.Add(renderTexture);
				}
				array[j] = material2;
			}
			savedMaterials.Add(materials);
			skinnedMeshRenderer.materials = array;
		}
	}

	private void RestoreMaterials()
	{
		if (savedMaterials.Count > 0)
		{
			for (int i = 0; i < skinnedMeshes.Length; i++)
			{
				if (skinnedMeshes[i] != null)
				{
					skinnedMeshes[i].materials = savedMaterials[i];
				}
			}
			savedMaterials.Clear();
		}
		ResetResources();
	}

	private void ResetResources()
	{
		if (savedMaterials != null)
		{
			foreach (Material[] savedMaterial in savedMaterials)
			{
				Material[] array = savedMaterial;
				foreach (Material obj in array)
				{
					Object.Destroy(obj);
				}
			}
			savedMaterials.Clear();
		}
		else
		{
			savedMaterials = new List<Material[]>();
		}
		if (convertedTextures != null)
		{
			foreach (RenderTexture convertedTexture in convertedTextures)
			{
				Object.Destroy(convertedTexture);
			}
			convertedTextures.Clear();
		}
		else
		{
			convertedTextures = new List<RenderTexture>();
		}
	}

	private void OnAboutToChangeFullscreenState(bool newState)
	{
		if (connected)
		{
			awaitingFullScreenChange = true;
			RestoreMaterials();
		}
	}

	private void OnChangedFullscreenState(bool newState)
	{
		awaitingFullScreenChange = false;
		if (connected)
		{
			CreateMaterials();
		}
	}

	private static RenderTexture DestroyTexture(RenderTexture texture)
	{
		if (texture != null)
		{
			Object.Destroy(texture);
		}
		return null;
	}

	private static RenderTexture[] DestroyTextureArray(RenderTexture[] textureArray)
	{
		if (textureArray != null)
		{
			foreach (RenderTexture texture in textureArray)
			{
				DestroyTexture(texture);
			}
		}
		return null;
	}
}
