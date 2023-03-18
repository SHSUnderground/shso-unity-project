using System.Collections.Generic;
using UnityEngine;

public class LightsOutMaster : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Color darkAmbient = new Color(1f, 1f, 1f, 1f);

	public Color darkDirectional = new Color(1f, 1f, 1f, 1f);

	public Color darkLightMap = new Color(1f, 1f, 1f, 1f);

	public bool lightsOn;

	protected AmbientOverride ambientLight;

	protected Light dynamicLight;

	protected List<Material> lightMapMaterials;

	protected Color oldAmbientColor = new Color(1f, 1f, 1f, 1f);

	protected Color oldCharacterColor = new Color(1f, 1f, 1f, 1f);

	private void Start()
	{
		AmbientOverride[] array = Utils.FindObjectsOfType<AmbientOverride>();
		if (array.Length > 0)
		{
			ambientLight = array[0];
			oldAmbientColor = ambientLight.ambientColor;
		}
		Light[] array2 = Utils.FindObjectsOfType<Light>();
		if (array2.Length > 0)
		{
			dynamicLight = array2[0];
			oldCharacterColor = dynamicLight.color;
		}
		lightMapMaterials = new List<Material>();
		MeshRenderer[] array3 = Utils.FindObjectsOfType<MeshRenderer>();
		MeshRenderer[] array4 = array3;
		foreach (MeshRenderer meshRenderer in array4)
		{
			for (int j = 0; j < meshRenderer.materials.Length; j++)
			{
				Material material = meshRenderer.materials[j];
				Shader shader = null;
				switch (material.shader.name)
				{
				case "Marvel/Lightmap/Vertex Color":
					shader = Shader.Find("Marvel/Lightmap/Vertex Color - Tintable");
					break;
				case "Marvel/Lightmap/Diffuse":
					shader = Shader.Find("Marvel/Lightmap/Diffuse - Tintable");
					break;
				}
				if (shader != null)
				{
					meshRenderer.materials[j] = new Material(meshRenderer.materials[j]);
					meshRenderer.materials[j].shader = shader;
					lightMapMaterials.Add(meshRenderer.materials[j]);
				}
			}
		}
		UpdateLightingEffect();
	}

	private void Update()
	{
	}

	public void UpdateLightingEffect()
	{
		Vector4 vector = new Vector4(darkLightMap.r, darkLightMap.g, darkLightMap.b, darkLightMap.a);
		if (lightsOn)
		{
			if (dynamicLight != null)
			{
				dynamicLight.color = oldCharacterColor;
			}
			if (ambientLight != null)
			{
				ambientLight.ambientColor = oldAmbientColor;
			}
			foreach (Material lightMapMaterial in lightMapMaterials)
			{
				lightMapMaterial.SetVector("_Tint", Vector4.one);
			}
		}
		else
		{
			if (dynamicLight != null)
			{
				dynamicLight.color = darkDirectional;
			}
			if (ambientLight != null)
			{
				ambientLight.ambientColor = darkAmbient;
			}
			foreach (Material lightMapMaterial2 in lightMapMaterials)
			{
				lightMapMaterial2.SetVector("_Tint", vector);
			}
		}
	}
}
