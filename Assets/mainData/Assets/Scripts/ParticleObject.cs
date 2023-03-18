using System.Runtime.CompilerServices;
using UnityEngine;

public class ParticleObject
{
	[CompilerGenerated]
	private GameObject _003CInstance_003Ek__BackingField;

	[CompilerGenerated]
	private float _003CLifeTime_003Ek__BackingField;

	[CompilerGenerated]
	private float _003CFadeTime_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CCanFade_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CIsAttached_003Ek__BackingField;

	public GameObject Instance
	{
		[CompilerGenerated]
		get
		{
			return _003CInstance_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CInstance_003Ek__BackingField = value;
		}
	}

	public float LifeTime
	{
		[CompilerGenerated]
		get
		{
			return _003CLifeTime_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CLifeTime_003Ek__BackingField = value;
		}
	}

	public float FadeTime
	{
		[CompilerGenerated]
		get
		{
			return _003CFadeTime_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CFadeTime_003Ek__BackingField = value;
		}
	}

	public bool CanFade
	{
		[CompilerGenerated]
		get
		{
			return _003CCanFade_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CCanFade_003Ek__BackingField = value;
		}
	}

	public bool IsAttached
	{
		[CompilerGenerated]
		get
		{
			return _003CIsAttached_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CIsAttached_003Ek__BackingField = value;
		}
	}

	public ParticleObject(GameObject obj, IBaseEffect creator, bool attached)
	{
		Instance = obj;
		IsAttached = attached;
		if (creator is ParticleEffect)
		{
			ParticleEffect particleEffect = creator as ParticleEffect;
			LifeTime = particleEffect.Lifetime;
			FadeTime = particleEffect.FadeTime;
			CanFade = particleEffect.FadeOut;
		}
		else if (creator is GeneralEffect)
		{
			GeneralEffect generalEffect = creator as GeneralEffect;
			LifeTime = generalEffect.Lifetime;
			FadeTime = 0f;
			CanFade = false;
		}
		else
		{
			LifeTime = 0f;
			FadeTime = 0f;
			CanFade = false;
		}
	}

	public void Destroy()
	{
		if (Instance != null)
		{
			Utils.DelayedDestroyNetworkedChildren(Instance);
			Object.Destroy(Instance);
		}
		Instance = null;
	}
}
