using System;
using UnityEngine;

[Serializable]
public class EventEffect : IBaseEffect
{
	public float TimeOffset;

	public string EventName = string.Empty;

	public float EventValue;

	public EffectScope scope;

	public bool started;

	public Material material;

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
			return typeof(string);
		}
	}

	public object Asset
	{
		get
		{
			return EventName;
		}
		set
		{
			EventName = (value as string);
		}
	}

	public EventEffect()
	{
	}

	public EventEffect(string name, float value)
	{
		EventName = name;
		EventValue = value;
	}

	public string GetName()
	{
		if (!string.IsNullOrEmpty(EventName))
		{
			return EventName;
		}
		return "<Event>";
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
	}

	public bool GetAllowsLifetime()
	{
		return false;
	}

	public float GetLifetime(GameObject obj)
	{
		return 0f;
	}

	public float GetAssetLifetime(GameObject obj)
	{
		return 0f;
	}
}
