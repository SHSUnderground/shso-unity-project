using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FadeOutSound : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float FadeDuration = 2f;

	public bool StopOnFinish = true;

	[CompilerGenerated]
	private ShsAudioBase _003CSource_003Ek__BackingField;

	[CompilerGenerated]
	private float _003CStartTime_003Ek__BackingField;

	[CompilerGenerated]
	private float _003CInitialVolume_003Ek__BackingField;

	public ShsAudioBase Source
	{
		[CompilerGenerated]
		get
		{
			return _003CSource_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CSource_003Ek__BackingField = value;
		}
	}

	public float StartTime
	{
		[CompilerGenerated]
		get
		{
			return _003CStartTime_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CStartTime_003Ek__BackingField = value;
		}
	}

	public float InitialVolume
	{
		[CompilerGenerated]
		get
		{
			return _003CInitialVolume_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CInitialVolume_003Ek__BackingField = value;
		}
	}

	public float CurrentVolume
	{
		get
		{
			return Source.Volume;
		}
		protected set
		{
			Source.Volume = value;
		}
	}

	public static FadeOutSound StartFade(ShsAudioBase source, float fadeDuration, bool stopOnFinish)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		FadeOutSound fadeOutSound = source.gameObject.AddComponent<FadeOutSound>();
		fadeOutSound.Source = source;
		fadeOutSound.FadeDuration = fadeDuration;
		fadeOutSound.StopOnFinish = stopOnFinish;
		return fadeOutSound;
	}

	public void Start()
	{
		if (Source == null)
		{
			Source = base.gameObject.GetComponent<ShsAudioBase>();
		}
		if (Source != null)
		{
			StartCoroutine(FadeRoutine());
		}
		else
		{
			CspUtils.DebugLog("Could not fade out <" + base.gameObject.name + ">: no audio source found.");
		}
	}

	private IEnumerator FadeRoutine()
	{
		StartTime = Time.time;
		InitialVolume = CurrentVolume;
		while (Time.time <= StartTime + FadeDuration)
		{
			float t = (Time.time - StartTime) / FadeDuration;
			CurrentVolume = Mathf.SmoothStep(InitialVolume, 0f, t);
			yield return 0;
		}
		CurrentVolume = 0f;
		if (StopOnFinish)
		{
			Source.Stop();
		}
	}
}
