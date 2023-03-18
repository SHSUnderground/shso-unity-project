using System;
using UnityEngine;

[Serializable]
public class GeneralEffect : IBaseEffect
{
	public GameObject Prefab;

	public float TimeOffset;

	public float Lifetime;

	public bool looping;

	public string AttachNodeName = string.Empty;

	public EffectScope scope;

	public bool started;

	public GraphicsOptions.GraphicsQuality minimumQuality;

	public bool ScaleToOwner = true;

	public float chanceToPlay = 1f;

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
			return Prefab;
		}
		set
		{
			Prefab = (value as GameObject);
		}
	}

	public string GetName()
	{
		if (Prefab != null)
		{
			return Prefab.name;
		}
		return "<General>";
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

	public bool IsLooping()
	{
		if (looping)
		{
			return true;
		}
		if (Prefab != null)
		{
			IGeneralEffect firstIGeneralEffect = GetFirstIGeneralEffect(Prefab);
			if (firstIGeneralEffect != null)
			{
				return firstIGeneralEffect.IsLooping();
			}
		}
		return true;
	}

	public IGeneralEffect GetFirstIGeneralEffect(GameObject obj)
	{
		if (obj == null)
		{
			return null;
		}
		MonoBehaviour[] components = obj.GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour monoBehaviour in components)
		{
			if (typeof(IGeneralEffect).IsAssignableFrom(monoBehaviour.GetType()))
			{
				return (IGeneralEffect)monoBehaviour;
			}
		}
		return null;
	}
}
