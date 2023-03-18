using System;
using UnityEngine;

[Serializable]
public class AnimationEffect : IBaseEffect
{
	public string Animation = string.Empty;

	public float TimeOffset;

	public float BlendTime = 0.1f;

	public WrapMode Mode = WrapMode.ClampForever;

	public EffectScope scope;

	public bool started;

	public bool persistFace;

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
			return Animation;
		}
		set
		{
			Animation = (value as string);
		}
	}

	public string GetName()
	{
		if (!string.IsNullOrEmpty(Animation))
		{
			return Animation;
		}
		return "<Animation>";
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
		return GetAssetLifetime(obj);
	}

	public float GetAssetLifetime(GameObject obj)
	{
		if (obj == null)
		{
			return 0f;
		}
		switch (Mode)
		{
		case WrapMode.Default:
		case WrapMode.Once:
		case WrapMode.ClampForever:
		{
			Animation animation = obj.GetComponentInChildren(typeof(Animation)) as Animation;
			if (animation == null)
			{
				return 0f;
			}
			AnimationClip clip = animation.GetClip(Animation);
			if (clip == null)
			{
				return 0f;
			}
			if (Mode == WrapMode.Default && (clip.wrapMode == WrapMode.Loop || clip.wrapMode == WrapMode.PingPong))
			{
				return -1f;
			}
			return clip.length;
		}
		case WrapMode.Loop:
		case WrapMode.PingPong:
			return -1f;
		default:
			return 0f;
		}
	}
}
