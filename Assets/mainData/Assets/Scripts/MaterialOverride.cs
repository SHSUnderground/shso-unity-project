using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MaterialOverride
{
	[Serializable]
	public class PropertyAnimation
	{
		public string propertyName;

		public AnimationCurve curve;
	}

	[Serializable]
	public class RenderStep
	{
		[Serializable]
		public class RenderInstruction
		{
			public Material material;

			public int shaderPassIndex = -1;
		}

		public RenderInstruction[] instructions;

		public RenderStep()
		{
			instructions = new RenderInstruction[1]
			{
				new RenderInstruction()
			};
		}

		public RenderTexture Render(Texture sourceTexture, bool temporary)
		{
			RenderTexture renderTexture = GetRenderTexture(sourceTexture, temporary);
			RenderInstruction[] array = instructions;
			foreach (RenderInstruction renderInstruction in array)
			{
				if (renderInstruction.material != null)
				{
					ShaderUtil.Blit(sourceTexture, renderTexture, renderInstruction.material, renderInstruction.shaderPassIndex);
					continue;
				}
				CspUtils.DebugLog("Invalid RenderInstruction; performing empty blit.");
				ShaderUtil.Blit(sourceTexture, renderTexture);
			}
			if (instructions == null || instructions.Length == 0)
			{
				CspUtils.DebugLog("Invalid RenderStep (no render instructions); performing empty blit.");
				ShaderUtil.Blit(sourceTexture, renderTexture);
			}
			return renderTexture;
		}

		public RenderTexture GetRenderTexture(Texture sourceTexture, bool temporary)
		{
			if (temporary)
			{
				return RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height, 0);
			}
			return new RenderTexture(sourceTexture.width, sourceTexture.height, 0);
		}
	}

	public int materialIndexStart;

	public int materialIndexEnd = -1;

	public Material overrideMaterial;

	public bool useTint = true;

	public ColorAnimation tint;

	public PropertyAnimation[] propertyAnims;

	public RenderStep[] renderSteps;

	private List<UnityEngine.Object> createdResources = new List<UnityEngine.Object>();

	public void ClearResources()
	{
		foreach (UnityEngine.Object createdResource in createdResources)
		{
			UnityEngine.Object.Destroy(createdResource);
		}
		createdResources.Clear();
	}

	public void ApplyToRenderers(IEnumerable renderers)
	{
		foreach (Renderer renderer in renderers)
		{
			Material[] array = new Material[renderer.materials.Length];
			DRange overrideRange = GetOverrideRange(renderer);
			for (int i = 0; i < renderer.materials.Length; i++)
			{
				if (overrideRange.Contains(i))
				{
					Material item = array[i] = CreateOverrideMaterial(renderer.materials[i]);
					createdResources.Add(item);
				}
				else
				{
					array[i] = renderer.materials[i];
				}
			}
			renderer.materials = array;
		}
	}

	public void AnimateTo(float time, IEnumerable renderers)
	{
		Color color = default(Color);
		Dictionary<string, float> dictionary = null;
		if (useTint)
		{
			color = tint.GetColor(time);
		}
		if (propertyAnims.Length > 0)
		{
			dictionary = new Dictionary<string, float>();
			PropertyAnimation[] array = propertyAnims;
			foreach (PropertyAnimation propertyAnimation in array)
			{
				dictionary[propertyAnimation.propertyName] = propertyAnimation.curve.Evaluate(time);
			}
		}
		foreach (Renderer renderer in renderers)
		{
			DRange overrideRange = GetOverrideRange(renderer);
			for (int j = 0; j < renderer.materials.Length; j++)
			{
				if (overrideRange.Contains(j))
				{
					if (useTint)
					{
						renderer.materials[j].color = color;
					}
					if (dictionary != null)
					{
						foreach (KeyValuePair<string, float> item in dictionary)
						{
							if (renderer.materials[j].HasProperty(item.Key))
							{
								renderer.materials[j].SetFloat(item.Key, item.Value);
							}
						}
					}
				}
			}
		}
	}

	protected Material CreateOverrideMaterial(Material originalMaterial)
	{
		Material material;
		if (overrideMaterial != null)
		{
			material = new Material(overrideMaterial);
			material.name = originalMaterial.name + "_" + overrideMaterial.name;
		}
		else
		{
			material = new Material(originalMaterial);
		}
		Texture texture = originalMaterial.GetTexture("_MainTex");
		RenderTexture renderTexture = null;
		for (int i = 0; i < renderSteps.Length; i++)
		{
			bool flag = i == renderSteps.Length - 1;
			texture = renderSteps[i].Render(texture, !flag);
			if (renderTexture != null)
			{
				RenderTexture.ReleaseTemporary(renderTexture);
			}
			if (flag)
			{
				createdResources.Add(texture);
			}
			else
			{
				renderTexture = (texture as RenderTexture);
			}
		}
		material.SetTexture("_MainTex", texture);
		return material;
	}

	protected DRange GetOverrideRange(Renderer renderer)
	{
		return new DRange((materialIndexStart >= 0) ? materialIndexStart : (renderer.materials.Length + materialIndexStart), (materialIndexEnd >= 0) ? materialIndexEnd : (renderer.materials.Length + materialIndexEnd));
	}
}
