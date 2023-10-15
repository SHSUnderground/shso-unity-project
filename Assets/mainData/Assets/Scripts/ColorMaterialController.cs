using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Character/Color Material Controller")]
public class ColorMaterialController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float delay;

	public float duration = -1f;

	public Material overrideMaterial;

	public Material colorMaterial;

	public Material greyscaleMaterial;

	public Material highPassMaterial;

	public bool includeOverride = true;

	public bool includeGreyscale = true;

	public bool includeHighPass = true;

	public bool includeColor = true;

	public string colorRampTexture = "gold_ramp";

	public float colorRampOffset = -0.5f;

	public bool reconnect;

	public Color textureColor;

	public bool useTextureColor;

	public bool includeNonSkinnedRenderers;

	public bool includeParticleRenderers;

	protected bool connected;

	protected Renderer[] meshes;

	protected List<RenderTexture> convertedTextures;

	protected List<Material[]> savedMaterials;

	protected bool awaitingFullScreenChange;

	public bool Connect()
	{
		if (connected)
		{
			return true;
		}
		CreateMaterials();
		connected = true;
		return true;
	}

	public bool Disconnect()
	{
		if (!connected)
		{
			return true;
		}
		if (!awaitingFullScreenChange)
		{
			RestoreMaterials();
		}
		connected = false;
		return true;
	}

	private void CreateMaterials()
	{
		if (colorMaterial == null)
		{
			Texture2D texture = Resources.Load("Shaders/Textures/" + colorRampTexture, typeof(Texture2D)) as Texture2D;
			colorMaterial = new Material(Shader.Find("GoldShader"));
			colorMaterial.SetTexture("_RampTex", texture);
		}
		if (greyscaleMaterial == null)
		{
			greyscaleMaterial = new Material(Shader.Find("GoldShader"));
		}
		if (highPassMaterial == null)
		{
			highPassMaterial = new Material(Shader.Find("HighPassFilter"));
		}
		if (overrideMaterial == null)
		{
			overrideMaterial = new Material(Shader.Find("Specular"));
		}
		ResetResources();
		for (int i = 0; i < meshes.Length; i++)
		{
			Renderer renderer = meshes[i];
			Material[] materials = renderer.materials;
			Material[] array = new Material[materials.Length];
			for (int j = 0; j < materials.Length; j++)
			{
				Material material = materials[j];
				Material material2 = null;
				material2 = ((!(material.shader.name == "Marvel/Base/Diffuse 2-Sided") && !(material.shader.name == "Marvel/Base/Diffuse Cutout 2-Sided") && includeOverride) ? new Material(overrideMaterial) : new Material(material));
				float num = textureColor.a;
				if (material.shader.name == "Marvel/Base/Diffuse Cutout 2-Sided" && material.HasProperty("_Cutoff"))
				{
					float @float = material.GetFloat("_Cutoff");
					if (@float > num)
					{
						num = @float;
					}
				}
				if (useTextureColor)
				{
					material2.color = new Color(textureColor.r, textureColor.g, textureColor.b, num);
				}
				Texture texture2 = material.GetTexture("_MainTex");
				if (texture2 != null)
				{
					RenderTexture renderTexture = new RenderTexture(texture2.width, texture2.height, 0);
					RenderTexture temporary = RenderTexture.GetTemporary(texture2.width, texture2.height, 0);
					if (includeHighPass)
					{
						ShaderUtil.Blit(texture2, temporary, highPassMaterial);
					}
					else
					{
						ShaderUtil.Blit(texture2, temporary);
					}
					if (includeGreyscale)
					{
						ShaderUtil.Blit(texture2, temporary, greyscaleMaterial, 0);
					}
					if (includeColor)
					{
						colorMaterial.SetFloat("_Offset", colorRampOffset);
						ShaderUtil.Blit(temporary, renderTexture, colorMaterial, 1);
					}
					else
					{
						ShaderUtil.Blit(temporary, renderTexture);
					}
					RenderTexture.ReleaseTemporary(temporary);
					material2.SetTexture("_MainTex", renderTexture);
					convertedTextures.Add(renderTexture);
				}
				array[j] = material2;
			}
			savedMaterials.Add(materials);
			renderer.materials = array;
		}
	}

	private void RestoreMaterials()
	{
		if (savedMaterials.Count > 0)
		{
			for (int i = 0; i < meshes.Length; i++)
			{
				if (meshes[i] != null)
				{
					meshes[i].materials = savedMaterials[i];
				}
			}
			savedMaterials.Clear();
		}
		ResetResources();
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

	private IEnumerator TimedConnect(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		Connect();
	}

	private IEnumerator TimedDestroy(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		Object.Destroy(this);
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

	private void Start()
	{
		AppShell.Instance.OnAboutToChangeFullscreenState += OnAboutToChangeFullscreenState;
		AppShell.Instance.OnChangedFullscreenState += OnChangedFullscreenState;
		if (meshes == null || meshes.Length == 0)
		{
			if (includeNonSkinnedRenderers)
			{
				List<Renderer> list;
				if (includeParticleRenderers)
				{
					list = new List<Renderer>(Utils.GetComponents<Renderer>(base.transform.root.gameObject, Utils.SearchChildren));
				}
				else
				{
					list = new List<Renderer>();
					Renderer[] components = Utils.GetComponents<Renderer>(base.transform.root.gameObject, Utils.SearchChildren);
					foreach (Renderer renderer in components)
					{
						if (!(renderer is ParticleRenderer))
						{
							list.Add(renderer);
						}
					}
				}
				meshes = list.ToArray();
			}
			else
			{
				meshes = Utils.GetComponents<SkinnedMeshRenderer>(base.transform.root.gameObject, Utils.SearchChildren);
			}
			if (meshes == null || meshes.Length == 0)
			{
				CspUtils.DebugLogError("Unable to connect gold controller to " + base.transform.root.gameObject + ".  SkinnedMeshRenderer component not found.");
			}
		}
		StartCoroutine(TimedConnect(delay));
		if (duration >= 0f)
		{
			StartCoroutine(TimedDestroy(duration));
		}
	}

	private void Update()
	{
		if (reconnect && connected)
		{
			Disconnect();
			StopAllCoroutines();
			StartCoroutine(TimedConnect(delay));
			if (duration >= 0f)
			{
				StartCoroutine(TimedDestroy(duration));
			}
			reconnect = false;
		}
	}

	private void OnDisable()
	{
		Disconnect();
	}
}
