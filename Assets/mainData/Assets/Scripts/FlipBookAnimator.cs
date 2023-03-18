using System;
using UnityEngine;

[AddComponentMenu("Rendering/Flip Book Animator")]
public class FlipBookAnimator : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum AnimationMode
	{
		Sequential,
		Step,
		Controlled
	}

	public enum TweenMode
	{
		Step,
		Linear,
		Smooth,
		ExtraSmooth
	}

	private const string AdvancedMaterialProperty = "_FlipBookData";

	public AnimationMode mode;

	public float time;

	public int currentFrame = -1;

	public MeshRenderer meshRenderer;

	public SkinnedMeshRenderer skinnedMeshRenderer;

	public TrailRenderer trailRenderer;

	public LineRenderer lineRenderer;

	public ParticleRenderer particleRenderer;

	public int materialIndex;

	public string textureName = "_MainTex";

	public int rows = 1;

	public int columns = 1;

	public int frames = 1;

	public float fps;

	public bool preScaled;

	public bool negateVCoord;

	public TweenMode Tween = TweenMode.Linear;

	private Material material;

	private Material oldMaterial;

	private bool materialIsAdvanced;

	private Material RendererMaterial
	{
		get
		{
			if (meshRenderer != null)
			{
				return meshRenderer.materials[materialIndex];
			}
			if (skinnedMeshRenderer != null)
			{
				return skinnedMeshRenderer.materials[materialIndex];
			}
			if (trailRenderer != null)
			{
				return trailRenderer.materials[materialIndex];
			}
			if (lineRenderer != null)
			{
				return lineRenderer.materials[materialIndex];
			}
			return particleRenderer.materials[materialIndex];
		}
		set
		{
			if (meshRenderer != null)
			{
				meshRenderer.materials[materialIndex] = value;
			}
			else if (skinnedMeshRenderer != null)
			{
				skinnedMeshRenderer.materials[materialIndex] = value;
			}
			else if (trailRenderer != null)
			{
				trailRenderer.materials[materialIndex] = value;
			}
			else if (lineRenderer != null)
			{
				lineRenderer.materials[materialIndex] = value;
			}
			else
			{
				particleRenderer.materials[materialIndex] = value;
			}
		}
	}

	private void Update()
	{
		//Discarded unreachable code: IL_025c
		columns = Mathf.Max(columns, 1);
		rows = Mathf.Max(rows, 1);
		frames = Mathf.Clamp(frames, columns * Mathf.Max(rows - 1, 1), columns * rows);
		if (fps == 0f)
		{
			base.enabled = false;
		}
		else
		{
			try
			{
				Validate();
				AttachToRenderer();
				switch (mode)
				{
				case AnimationMode.Sequential:
				{
					float num4 = Mathf.Abs(fps);
					time = (time + Time.deltaTime) % ((float)frames / num4);
					float num5 = time * num4;
					int num6 = (int)num5;
					int num7 = (num6 + 1) % frames;
					float tween2 = num5 - (float)num6;
					if (fps > 0f)
					{
						SetAnimationFrame(num6, num7, tween2);
					}
					else
					{
						SetAnimationFrame(frames - 1 - num6, frames - 1 - num7, tween2);
					}
					break;
				}
				case AnimationMode.Step:
				{
					int num8 = (fps > 0f) ? 1 : (-1);
					int num9 = currentFrame + num8;
					if (num9 < 0)
					{
						num9 += frames * (-num9 / frames + 1);
					}
					int num10 = num9 + num8;
					if (num10 < 0)
					{
						num10 += frames * (-num10 / frames + 1);
					}
					SetAnimationFrame(num9 % frames, num10 % frames, 0f);
					base.enabled = false;
					break;
				}
				case AnimationMode.Controlled:
				{
					float num = Mathf.Abs(time) * Mathf.Abs(fps);
					int num2 = (int)num % frames;
					int num3 = ((int)num + ((fps >= 0f) ? 1 : (-1))) % frames;
					float tween = num - (float)(int)num;
					if (fps * time < 0f)
					{
						num2 = frames - 1 - num2;
						num3 = frames - 1 - num3;
					}
					SetAnimationFrame(num2, num3, tween);
					break;
				}
				}
			}
			catch
			{
				base.enabled = false;
				throw;
			}
		}
	}

	private void SetAnimationFrame(int i, int next, float tween)
	{
		Vector2 vector = new Vector2(1f / (float)columns, 1f / (float)rows);
		material.SetTextureOffset(textureName, GetAnimationOffset(i, vector));
		if (!preScaled)
		{
			material.SetTextureScale(textureName, vector);
		}
		if (materialIsAdvanced)
		{
			Vector2 animationOffset = GetAnimationOffset(next, vector);
			switch (Tween)
			{
			case TweenMode.Step:
				tween = 0f;
				break;
			case TweenMode.Smooth:
				tween = (3f - 2f * tween) * tween * tween;
				break;
			case TweenMode.ExtraSmooth:
				tween = Mathf.Pow(tween, 3f) * ((tween * 6f - 15f) * tween + 10f);
				break;
			}
			Vector4 vector2 = new Vector4(animationOffset.x, animationOffset.y, tween, 0f);
			material.SetVector("_FlipBookData", vector2);
		}
		currentFrame = i;
	}

	private Vector2 GetAnimationOffset(int i, Vector2 size)
	{
		int num = i % columns;
		int num2 = i / columns;
		int num3;
		if (negateVCoord)
		{
			num3 = rows - 1 - num2;
			if (preScaled)
			{
				num3 = (num3 + 1) % rows;
			}
		}
		else
		{
			num3 = num2;
		}
		return new Vector2((float)num * size.x, (float)num3 * size.y);
	}

	private void Validate()
	{
		if (meshRenderer == null && skinnedMeshRenderer == null && trailRenderer == null && particleRenderer == null)
		{
			meshRenderer = Utils.GetComponent<MeshRenderer>(base.gameObject, Utils.SearchChildren);
			skinnedMeshRenderer = Utils.GetComponent<SkinnedMeshRenderer>(base.gameObject, Utils.SearchChildren);
			if (meshRenderer == null && skinnedMeshRenderer == null)
			{
				trailRenderer = Utils.GetComponent<TrailRenderer>(base.gameObject, Utils.SearchChildren);
				if (trailRenderer == null)
				{
					lineRenderer = Utils.GetComponent<LineRenderer>(base.gameObject, Utils.SearchChildren);
					if (lineRenderer == null)
					{
						particleRenderer = Utils.GetComponent<ParticleRenderer>(base.gameObject, Utils.SearchChildren);
						if (particleRenderer == null)
						{
							throw new Exception("Renderer not assigned.  (And not attached to this game object.)");
						}
					}
				}
			}
		}
		if (textureName == null || textureName == string.Empty)
		{
			throw new Exception("Texture not assigned.");
		}
	}

	private void AttachToRenderer()
	{
		Material rendererMaterial = RendererMaterial;
		if (rendererMaterial != material)
		{
			if (oldMaterial == null)
			{
				oldMaterial = rendererMaterial;
			}
			RendererMaterial = new Material(oldMaterial);
			material = RendererMaterial;
			materialIsAdvanced = material.HasProperty("_FlipBookData");
		}
	}
}
