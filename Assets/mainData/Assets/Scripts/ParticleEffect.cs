using System;
using UnityEngine;

[Serializable]
public class ParticleEffect : IBaseEffect
{
	public GameObject ParticlePrefab;

	public float TimeOffset;

	public float FadeTime;

	public bool FadeOut = true;

	public float Lifetime;

	public bool DetachedParticle;

	public bool DetachedParticlePermanent;

	public string AttachNodeName = string.Empty;

	public EffectScope scope;

	public bool started;

	public GraphicsOptions.GraphicsQuality minimumQuality;

	public bool ScaleToOwner = true;

	public bool HasAsset
	{
		get
		{
			return true;
		}
	}

	public Type AssetType
	{
		get
		{
			return typeof(GameObject);
		}
	}

	public object Asset
	{
		get
		{
			return ParticlePrefab;
		}
		set
		{
			ParticlePrefab = (value as GameObject);
		}
	}

	public string GetName()
	{
		if (ParticlePrefab != null)
		{
			return ParticlePrefab.name;
		}
		return "<Particle>";
	}

	public float GetTimeOffset()
	{
		return TimeOffset;
	}

	public void SetTimeOffset(float time_offset)
	{
		TimeOffset = time_offset;
	}

	public void SetLifetime(float lifetime)
	{
		Lifetime = lifetime;
	}

	public bool GetAllowsLifetime()
	{
		return true;
	}

	public float GetLifetime(GameObject obj)
	{
		return Lifetime;
	}

	public float GetAssetLifetime(GameObject obj)
	{
		return Lifetime;
	}
}
