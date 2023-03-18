using System;
using UnityEngine;

[Serializable]
public class SoundEffect : IBaseEffect
{
	public GameObject SoundPrefab;

	public float TimeOffset;

	public float FadeTime = -1f;

	public float Lifetime = -1f;

	public bool duckMusic;

	public EffectScope scope;

	public float chanceToPlay = 1f;

	public bool keepLocalTransform;

	[NonSerialized]
	public ShsAudioSource soundInstance;

	[NonSerialized]
	public bool started;

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
			return SoundPrefab;
		}
		set
		{
			SoundPrefab = (value as GameObject);
		}
	}

	public string GetName()
	{
		if (SoundPrefab != null)
		{
			return SoundPrefab.name;
		}
		return "<Sound>";
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
		if (Lifetime <= 0f)
		{
			return GetAssetLifetime(obj);
		}
		return Lifetime;
	}

	public float GetAssetLifetime(GameObject obj)
	{
		if (SoundPrefab == null)
		{
			return -99f;
		}
		ShsAudioSource component = Utils.GetComponent<ShsAudioSource>(SoundPrefab);
		if (component != null)
		{
			if (component.Clips.Length <= 0)
			{
				return -99f;
			}
			if (component.Loop)
			{
				return -1f;
			}
			return component.LengthOfNextClip();
		}
		CspUtils.DebugLog("No ShsAudioSource attached to <" + ((!(SoundPrefab == null)) ? SoundPrefab.name : "null") + ">");
		return -99f;
	}
}
