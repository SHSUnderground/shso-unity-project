using System.Runtime.CompilerServices;
using UnityEngine;

public class LocallyOwnedVO : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private static float _desiredListenerDistance = 1f;

	private static AudioListener _cachedListener;

	[CompilerGenerated]
	private ResolvedVOAction _003CVOAction_003Ek__BackingField;

	[CompilerGenerated]
	private ShsAudioSource _003CAudioSource_003Ek__BackingField;

	public static float DesiredListenerDistance
	{
		get
		{
			return _desiredListenerDistance;
		}
		set
		{
			_desiredListenerDistance = value;
		}
	}

	public static AudioListener Listener
	{
		get
		{
			if (_cachedListener == null)
			{
				_cachedListener = (Object.FindObjectOfType(typeof(AudioListener)) as AudioListener);
			}
			return _cachedListener;
		}
	}

	public ResolvedVOAction VOAction
	{
		[CompilerGenerated]
		get
		{
			return _003CVOAction_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CVOAction_003Ek__BackingField = value;
		}
	}

	public ShsAudioSource AudioSource
	{
		[CompilerGenerated]
		get
		{
			return _003CAudioSource_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CAudioSource_003Ek__BackingField = value;
		}
	}

	private void Start()
	{
		if (VOAction == null || VOAction.Emitter == null || Listener == null)
		{
			Object.Destroy(this);
		}
		if (AudioSource != null)
		{
			AudioSource.PlayedFromLocalHero = true;
		}
	}

	private void LateUpdate()
	{
		if (VOAction != null && !(VOAction.Emitter == null) && !(Listener == null))
		{
			Vector3 b = (VOAction.Emitter.transform.position - Listener.transform.position).normalized * DesiredListenerDistance;
			base.transform.position = Listener.transform.position + b;
		}
	}
}
