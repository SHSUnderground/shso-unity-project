using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Audio/SHS Music Source %&m")]
public class ShsMusicSource : ShsAudioBase
{
	private const string PRESET_BUNDLE_NAME = "Music";

	public bool PlayOnAwake = true;

	public int loopCount = 1;

	public AudioClipReference introClip;

	public AudioClipReference loopingClip;

	public AudioClipReference outroClip;

	protected AudioSource currentSrc;

	protected AudioSource nextSrc;

	private bool currentWasPlaying;

	private bool nextWasPlaying;

	private bool unloading;

	public override float Volume
	{
		get
		{
			return base.Volume;
		}
		set
		{
			base.Volume = value;
			float mixedVolume = GetMixedVolume();
			if (currentSrc != null)
			{
				currentSrc.volume = mixedVolume;
			}
			if (nextSrc != null)
			{
				nextSrc.volume = mixedVolume;
			}
		}
	}

	protected void Awake()
	{
		currentSrc = Utils.AddComponent<AudioSource>(base.gameObject);
		currentSrc.playOnAwake = false;
		nextSrc = Utils.AddComponent<AudioSource>(base.gameObject);
		nextSrc.playOnAwake = false;
	}

	protected void Start()
	{
		if (!base.IsRegistered)
		{
			RegisterWithAudioManager();
		}
		LoadClips();
	}

	protected void OnEnable()
	{
		if (!base.IsRegistered)
		{
			RegisterWithAudioManager();
		}
		if (currentSrc == null)
		{
			CspUtils.DebugLog("Unable to find an AudioSource on <" + base.gameObject.name + "> that is required by the ShsMusicSource!  Audio won't play.");
		}
		else
		{
			if (!currentSrc.enabled)
			{
				currentSrc.enabled = true;
			}
			if (nextSrc != null && !nextSrc.enabled)
			{
				nextSrc.enabled = true;
			}
		}
		if (PlayOnAwake)
		{
			Play();
		}
	}

	protected void OnUnload()
	{
		unloading = true;
	}

	protected void OnDisable()
	{
		if (base.IsRegistered)
		{
			UnregisterWithAudioManager();
		}
		if (!unloading)
		{
			if (currentSrc != null)
			{
				currentSrc.enabled = false;
			}
			if (nextSrc != null)
			{
				nextSrc.enabled = false;
			}
		}
	}

	protected override void PlayImmediate()
	{
		if (!base.Paused)
		{
			StartCoroutine(StartPlay());
			return;
		}
		if (currentSrc != null && currentWasPlaying)
		{
			currentSrc.Play();
		}
		if (nextSrc != null && nextWasPlaying)
		{
			nextSrc.Play();
		}
	}

	public override void Stop()
	{
		StopAllCoroutines();
		if (currentSrc != null)
		{
			currentSrc.Stop();
		}
		if (nextSrc != null)
		{
			nextSrc.Stop();
		}
	}

	public void Pause()
	{
		if (!base.Paused)
		{
			base.Paused = true;
			if (currentSrc != null)
			{
				currentWasPlaying = currentSrc.isPlaying;
				currentSrc.Pause();
			}
			if (nextSrc != null)
			{
				nextWasPlaying = nextSrc.isPlaying;
				nextSrc.Pause();
			}
		}
	}

	public override bool Is3D()
	{
		return false;
	}

	protected IEnumerator StartPlay()
	{
		if (loopingClip == null)
		{
			CspUtils.DebugLog("No looping clip specified for <" + base.gameObject.name + "> -- Audio will not be played");
			yield break;
		}
		yield return StartCoroutine(WaitForClip(loopingClip));
		if (introClip != null)
		{
			yield return StartCoroutine(WaitForClip(introClip));
		}
		if (outroClip != null)
		{
			yield return StartCoroutine(WaitForClip(outroClip));
		}
		yield return StartCoroutine(WaitForVolume());
		yield return StartCoroutine(WaitIfPaused());
		if (introClip == null)
		{
			currentSrc.clip = loopingClip.Clip;
			currentSrc.loop = true;
			currentSrc.Play();
		}
		else
		{
			nextSrc.clip = loopingClip.Clip;
			nextSrc.loop = true;
			nextSrc.mute = true;
			nextSrc.Play();
			currentSrc.clip = introClip.Clip;
			currentSrc.loop = false;
			currentSrc.Play();
			while (currentSrc.timeSamples == 0 || nextSrc.timeSamples == 0)
			{
				yield return 0;
			}
			nextSrc.timeSamples = nextSrc.clip.samples - currentSrc.clip.samples + currentSrc.timeSamples;
			while (currentSrc.clip.length - currentSrc.time > Time.deltaTime)
			{
				yield return 0;
			}
			nextSrc.mute = false;
			yield return StartCoroutine(WaitIfPaused());
			while (currentSrc.isPlaying)
			{
				yield return 0;
			}
			yield return 0;
			AudioSource oldCurrentSrc = currentSrc;
			currentSrc = nextSrc;
			nextSrc = oldCurrentSrc;
		}
		StartCoroutine(TrackLoops());
	}

	protected IEnumerator TrackLoops()
	{
		for (int currentLoopCount = 0; currentLoopCount < loopCount - 1; currentLoopCount++)
		{
			int currentSample = 0;
			int lastSample;
			do
			{
				yield return 0;
				lastSample = currentSample;
				currentSample = currentSrc.timeSamples;
			}
			while (currentSample >= lastSample);
		}
		currentSrc.loop = false;
		if (outroClip != null)
		{
			nextSrc.clip = outroClip.Clip;
			nextSrc.loop = true;
			nextSrc.mute = true;
			while (currentSrc.clip.samples - currentSrc.timeSamples >= nextSrc.clip.samples)
			{
				yield return 0;
			}
			yield return StartCoroutine(WaitIfPaused());
			nextSrc.Play();
			while (nextSrc.timeSamples == 0)
			{
				yield return 0;
			}
			nextSrc.timeSamples = nextSrc.clip.samples - currentSrc.clip.samples + currentSrc.timeSamples;
			while (currentSrc.clip.length - currentSrc.time > Time.deltaTime)
			{
				yield return 0;
			}
			nextSrc.mute = false;
			yield return StartCoroutine(WaitIfPaused());
			while (currentSrc.isPlaying)
			{
				yield return 0;
			}
			yield return 0;
			AudioSource oldCurrentSrc = currentSrc;
			currentSrc = nextSrc;
			nextSrc = oldCurrentSrc;
			currentSrc.loop = false;
		}
	}

	protected override float GetMixerVolume()
	{
		return mixerSettings.MusicVolume;
	}

	protected IEnumerator WaitForVolume()
	{
		if (base.IsRegistered)
		{
			while (AppShell.Instance.AudioManager.PresetBundleDefinitions == null)
			{
				yield return 0;
			}
			AudioPresetBundle musicBundle;
			while (!AppShell.Instance.AudioManager.PresetBundleDefinitions.PresetBundles.TryGetValue("Music", out musicBundle))
			{
				yield return 0;
			}
			Volume = musicBundle.PresetVolume.Volume;
		}
	}

	protected IEnumerator WaitForClip(AudioClipReference clipRef)
	{
		float startRequestTime = Time.time;
		while (clipRef.Clip == null && clipRef.WaitingForAssetLoad && startRequestTime + -1f > Time.time)
		{
			yield return 0;
		}
		if (clipRef.Clip == null)
		{
			CspUtils.DebugLog("Unable to start clip because the clip never loaded (or wasn't defined): source=<" + base.gameObject.name + ">, bundle=<" + clipRef.BundleName + ">, asset=<" + clipRef.AssetName + ">.");
		}
	}

	protected void LoadClips()
	{
		List<AudioClipReference> list = new List<AudioClipReference>();
		if (introClip != null)
		{
			list.Add(introClip);
		}
		if (loopingClip != null)
		{
			list.Add(loopingClip);
		}
		if (outroClip != null)
		{
			list.Add(outroClip);
		}
		RequestClipsFromAssetBundles(list);
	}

	protected IEnumerator WaitIfPaused()
	{
		while (base.Paused)
		{
			yield return 0;
		}
	}
}
