using ShsAudio;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("Audio/SHS Audio Source %&a")]
public class ShsAudioSource : ShsAudioBase
{
	public enum AudioClipPlayBehaviorEnum
	{
		Random,
		Sequential
	}
	


	public delegate void SoundPreloadedDelegate(ShsAudioSource audioSrcInstance, object extraData);

	public delegate void PrePlayDelegate(ShsAudioSource audioSrcInstance);

	protected const float STEAL_FADE_OUT_TIME = 0.2f;

	public string PrefabName;


	


	public AudioCategoryEnum AudioCategory;

	public bool Loop;

	public bool PlayOnAwake;

	public bool WaitForLoadToPlay;

	public bool UseCustomRolloff;

	public static float AttenuationScale = 1f;

	public static float LocalHeroScale = 1.5f;

	public AudioClipPlayBehaviorEnum PlayBehavior = AudioClipPlayBehaviorEnum.Sequential;

	public AudioClipReference[] Clips = new AudioClipReference[0];

	public string presetBundleName = "None";

    	
	[SerializeField]
	[HideInInspector]
	
    


	protected float currentVolumeVariance;

	protected float pitch = 1f;

	protected float currentPitchVariance;

	protected bool isPlaying;

	protected bool isPersistent;

	protected float storedTimeOffset;

	private bool? _requestsCrossfadeOverride;

	protected AudioManager manager;

	protected AudioSource currentAudioSrc;

	protected AudioSource nextAudioSrc;

	protected int nextSequentialClipIndex;

	protected bool[] alreadyPlayedRandomClips;

	protected int numberPlayedRandomClips;

	protected int clipIndexToPlayNext = -1;

	protected bool shouldCleanupOnDone;

	protected float instanceAttenuationScale = 1f;

	protected float heroScale = 1f;

	protected AudioPresetBundle presetBundle;

	[CompilerGenerated]
	private bool _003CPlayedFromLocalHero_003Ek__BackingField;

	public string PresetBundleName
	{
		get
		{
			return presetBundleName;
		}
		set
		{
			presetBundleName = value;
		}
	}

	public float Pitch
	{
		get
		{
			return pitch;
		}
		set
		{
			pitch = value;
			if (currentAudioSrc != null)
			{
				currentAudioSrc.pitch = GetMixedPitch();
			}
		}
	}

	public bool IsPlaying
	{
		get
		{
			if (!Loop && currentAudioSrc != null)
			{
				isPlaying = currentAudioSrc.isPlaying;
			}
			return isPlaying;
		}
	}

	public bool IsPersistent
	{
		get
		{
			return isPersistent;
		}
		set
		{
			isPersistent = value;
			if (isPersistent)
			{
				base.gameObject.transform.parent = AppShell.Instance.transform;
			}
			else
			{
				base.gameObject.transform.parent = null;
			}
		}
	}

	public float TimeOffset
	{
		get
		{
			if (currentAudioSrc != null)
			{
				return currentAudioSrc.time;
			}
			return storedTimeOffset;
		}
		set
		{
			if (currentAudioSrc != null)
			{
				currentAudioSrc.time = value;
			}
			else
			{
				storedTimeOffset = value;
			}
		}
	}

	public bool PlayedFromLocalHero
	{
		[CompilerGenerated]
		get
		{
			return _003CPlayedFromLocalHero_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CPlayedFromLocalHero_003Ek__BackingField = value;
		}
	}

	public bool RequestsCrossfade
	{
		get
		{
			bool? requestsCrossfadeOverride = _requestsCrossfadeOverride;
			if (!requestsCrossfadeOverride.HasValue)
			{
				return AudioCategory == AudioCategoryEnum.MusicScore;
			}
			bool? requestsCrossfadeOverride2 = _requestsCrossfadeOverride;
			return requestsCrossfadeOverride2.Value;
		}
		set
		{
			_requestsCrossfadeOverride = value;
		}
	}

	public AudioPresetBundle PresetBundle
	{
		get
		{
			if ((presetBundle == null || presetBundle.BundleName != presetBundleName) && AppShell.Instance != null && AppShell.Instance.AudioManager != null && AppShell.Instance.AudioManager.PresetBundleDefinitions != null)
			{
				AppShell.Instance.AudioManager.PresetBundleDefinitions.PresetBundles.TryGetValue(presetBundleName, out presetBundle);
			}
			return presetBundle;
		}
		set
		{
			if (value.BundleName != presetBundleName)
			{
				presetBundleName = value.BundleName;
			}
			presetBundle = value;
		}
	}

	public override float Volume
	{
		get
		{
			return base.Volume;
		}
		set
		{
			base.Volume = value;
			if (currentAudioSrc != null)
			{
				currentAudioSrc.volume = GetMixedVolume();
			}
		}
	}

	public override float GetMixedVolume()
	{
		return Mathf.Clamp01(volume + currentVolumeVariance) * GetMixerVolume() * heroScale;
	}

	protected float GetMixedPitch()
	{
		return pitch + currentPitchVariance;
	}

	protected virtual void Awake()
	{
		pitch = 1f;
		base.Paused = false;
		nextSequentialClipIndex = 0;
		clipIndexToPlayNext = -1;
		instanceAttenuationScale = AttenuationScale;
		bool flag = false;
		AudioClipReference[] clips = Clips;
		foreach (AudioClipReference audioClipReference in clips)
		{
			if (!audioClipReference.HasAsset())
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			RequestClipsFromAssetBundles(Clips);
		}
		if (string.IsNullOrEmpty(PrefabName))
		{
			string[] array = base.gameObject.name.Split('(');
			if (array.Length > 0)
			{
				PrefabName = array[0].Trim();
			}
			else
			{
				PrefabName = string.Empty;
			}
		}
	}

	protected virtual void Start()
	{
		if (PresetBundle != null && !base.IsRegistered)
		{
			RegisterWithAudioManager();
		}
		if (UseCustomRolloff)
		{
			return;
		}
		if (PlayedFromLocalHero)
		{
			LocalHeroBoost();
			return;
		}
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (localPlayer != null)
		{
			Transform transform = localPlayer.transform;
			Transform transform2 = base.transform;
			while (transform2 != null && transform2 != transform)
			{
				transform2 = transform2.transform.parent;
			}
			if (transform2 != null)
			{
				LocalHeroBoost();
			}
		}
	}

	protected virtual void OnEnable()
	{
		if (!IsPersistent || !Application.isLoadingLevel)
		{
			if (currentAudioSrc != null && !currentAudioSrc.enabled)
			{
				currentAudioSrc.enabled = true;
			}
			if (nextAudioSrc != null && !nextAudioSrc.enabled)
			{
				nextAudioSrc.enabled = true;
			}
			if (PresetBundle != null && !base.IsRegistered)
			{
				RegisterWithAudioManager();
			}
			if (PlayOnAwake)
			{
				Play();
			}
		}
	}

	protected virtual void OnDisable()
	{
		if (!IsPersistent || !Application.isLoadingLevel)
		{
			if (base.IsRegistered)
			{
				UnregisterWithAudioManager();
			}
			if (currentAudioSrc != null)
			{
				currentAudioSrc.enabled = false;
			}
			if (nextAudioSrc != null)
			{
				nextAudioSrc.enabled = false;
			}
			StopImmediate();
		}
	}

	public override void Play()
	{
		if (base.IsRegistered && RequestsCrossfade)
		{
			AppShell.Instance.AudioManager.DeclarePendingMusic(this);
		}
		base.Play();
	}

	protected override void PlayImmediate()
	{
		if (base.Paused)
		{
			base.Paused = false;
			currentAudioSrc.Play();
		}
		else
		{
			if (IsPlaying || Clips.GetLength(0) <= 0)
			{
				return;
			}
			if (!base.IsRegistered)
			{
				RegisterWithAudioManager();
			}
			PreselectNextClipToPlay();
			if (AppShell.Instance.AudioManager.CanPlayAudioSource(this))
			{
				shouldCleanupOnDone = true;
				int num = -1;
				num = SelectClipToPlay();
				if (num >= 0)
				{
					StartCoroutine(PlayClip(num));
				}
			}
		}
	}

	public override void Stop()
	{
		StopAllCoroutines();
		if (base.gameObject.active)
		{
			StartCoroutine(StopClip());
		}
		else
		{
			StopImmediate();
		}
	}

	public void Pause()
	{
		base.Paused = true;
		if (currentAudioSrc != null)
		{
			currentAudioSrc.Pause();
		}
	}

	public override bool Is3D()
	{
		return AudioCategory != AudioCategoryEnum.MusicScore && AudioCategory != AudioCategoryEnum.SfxHud && PresetBundleName != "VO" && PresetBundleName != "Amb_BaseLayer";
	}

	public void DestroyWhenFinished()
	{
		SuicideOnStop component = base.gameObject.GetComponent<SuicideOnStop>();
		if (component == null)
		{
			Utils.AddComponent<SuicideOnStop>(base.gameObject);
		}
	}

	public void Steal()
	{
		StopClipWithFade(0.2f);
	}

	public float LengthOfNextClip()
	{
		PreselectNextClipToPlay();
		if (clipIndexToPlayNext < 0)
		{
			return -99f;
		}
		return Clips[clipIndexToPlayNext].Clip.length;
	}

	protected int SelectClipToPlay()
	{
		int result = -1;
		if (clipIndexToPlayNext >= 0)
		{
			result = clipIndexToPlayNext;
			clipIndexToPlayNext = -1;
			return result;
		}
		if (PlayBehavior == AudioClipPlayBehaviorEnum.Random)
		{
			result = ChooseNextRandomClipIndex();
		}
		else if (PlayBehavior == AudioClipPlayBehaviorEnum.Sequential)
		{
			result = ChooseNextSequentialClipIndex();
		}
		else
		{
			CspUtils.DebugLog("Unknown PlayBehavior when trying to invoke Play on game object <" + base.gameObject.name + ">.");
		}
		return result;
	}

	protected void PreselectNextClipToPlay()
	{
		if (clipIndexToPlayNext < 0)
		{
			clipIndexToPlayNext = SelectClipToPlay();
		}
	}

	public void Reset()
	{
		nextSequentialClipIndex = 0;
		numberPlayedRandomClips = 0;
		if (alreadyPlayedRandomClips != null)
		{
			int length = alreadyPlayedRandomClips.GetLength(0);
			for (int i = 0; i < length; i++)
			{
				alreadyPlayedRandomClips[i] = false;
			}
		}
	}

	protected override void RequestClipsFromAssetBundles()
	{
		RequestClipsFromAssetBundles(Clips);
	}

	protected IEnumerator PlayClip(int clipIndex)
	{
		isPlaying = true;
		float startRequestTime = Time.time;
		AudioClipReference clipRef = Clips[clipIndex];
		bool isLoadedFromBundle = clipRef.Clip == null;
		if (isLoadedFromBundle && clipRef.IsNullBundleReference())
		{
			CspUtils.DebugLog("Asked to play undefined clip in source <" + base.gameObject.name + ">"); // Added an error comment (Aaron)
			CspUtils.DebugLog("Asked to play undefined clip in source <" + base.gameObject.name + ">");
			DonePlaying(true, true);
			yield break;
		}
		float maxWaitTime = 2f;
		if (AudioCategory == AudioCategoryEnum.MusicScore || AudioCategory == AudioCategoryEnum.MusicEmitter)
		{
			maxWaitTime = -1f;
		}
		while (WaitForLoadToPlay && clipRef.Clip == null && clipRef.WaitingForAssetLoad && (maxWaitTime < 0f || startRequestTime + maxWaitTime > Time.time))
		{
			yield return 0;
		}
		if (clipRef.Clip == null)
		{
			if (isLoadedFromBundle && !WaitForLoadToPlay)
			{
				DonePlaying();
				yield break;
			}
			if (isLoadedFromBundle)
			{
				CspUtils.DebugLog("Timed out when trying to play asset <" + clipRef.AssetName + "> in source <" + base.gameObject.name + "> from bundle <" + clipRef.BundleName + ">");
				if (maxWaitTime < 0f)
				{
					CspUtils.DebugLog("Not sure what happened here!  Aborting play.");
					DonePlaying();
					yield break;
				}
			}
			else
			{
				CspUtils.DebugLog("Unknown error occurred when playing audio: source=<" + base.gameObject.name + ">, bundle=<" + clipRef.BundleName + ">, asset=<" + clipRef.AssetName + ">.");
			}
			DonePlaying(true, true);
			yield break;
		}
		float timeToStopWaitingForPresets = Time.time + 1f;
		int frameCountToWaitForPresets = 30;
		while (PresetBundle == null)
		{
			if (timeToStopWaitingForPresets <= Time.time && frameCountToWaitForPresets-- <= 0)
			{
				CspUtils.DebugLog("Unable to start clip because presets were not available: source=<" + base.gameObject.name + ">, preset bundle=<" + PresetBundleName + ">.");
				DonePlaying();
				yield break;
			}
			yield return 0;
		}
		if (!AcquireAudioSource())
		{
			DonePlaying();
			yield break;
		}
		currentAudioSrc.clip = clipRef.Clip;
		currentAudioSrc.loop = false;
		currentAudioSrc.time = 0f;
		currentAudioSrc.playOnAwake = false;
		currentVolumeVariance = Random.Range(0f - PresetBundle.PresetVolume.VolumeVariation, PresetBundle.PresetVolume.VolumeVariation);
		currentPitchVariance = Random.Range(0f - PresetBundle.PresetPitch.PitchVariation, PresetBundle.PresetPitch.PitchVariation);
		currentAudioSrc.pitch = GetMixedPitch();
		if (!UseCustomRolloff)
		{
			currentAudioSrc.minDistance = PresetBundle.Preset3D.AttenuationStartDistance * AttenuationScale;
		}
		if (Loop && Clips.Length == 1)
		{
			currentAudioSrc.loop = true;
			if (!PlayCurrentAudioSource())
			{
				DonePlaying();
				yield break;
			}
		}
		else if (!PlayCurrentAudioSource())
		{
			DonePlaying();
			yield break;
		}
		float fadeInTime = PresetBundle.PresetFade.FadeInTime;
		if (fadeInTime > 0f)
		{
			if (currentAudioSrc.clip.length > 0f && fadeInTime > currentAudioSrc.clip.length)
			{
				fadeInTime = currentAudioSrc.clip.length;
			}
			float fadeStartTime = Time.time;
			float fadeEndTime = fadeStartTime + fadeInTime;
			while (Time.time < fadeEndTime)
			{
				currentAudioSrc.volume = Mathf.SmoothStep(0f, GetMixedVolume(), (Time.time - fadeStartTime) / fadeInTime);
				yield return 0;
			}
		}
		currentAudioSrc.volume = GetMixedVolume();
		if (!Loop || Clips.Length != 1)
		{
			while (currentAudioSrc.isPlaying || base.Paused)
			{
				yield return 0;
			}
			if (Loop)
			{
				SwapAudioSources();
				DonePlaying(false, false);
				Play();
			}
			else
			{
				DonePlaying();
			}
		}
	}

	protected void DonePlaying()
	{
		DonePlaying(false, true);
	}

	protected void DonePlaying(bool retryIfLooping, bool broadcastFinish)
	{
		isPlaying = false;
		if (shouldCleanupOnDone)
		{
			shouldCleanupOnDone = false;
			AppShell.Instance.AudioManager.DonePlayingAudioSource(this);
			if (retryIfLooping && Loop && PlayBehavior == AudioClipPlayBehaviorEnum.Random)
			{
				Play();
			}
			else if (broadcastFinish)
			{
				base.gameObject.SendMessage("OnAudioFinished", this, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	protected bool AcquireAudioSource()
	{
		if (currentAudioSrc == null)
		{
			if (UseCustomRolloff)
			{
				currentAudioSrc = base.gameObject.GetComponent<AudioSource>();
				if (currentAudioSrc == null)
				{
					LogStartFailure("\"Use Custom Rolloff\" is checked but no AudioSource is attached.");
					return false;
				}
				return currentAudioSrc != null;
			}
			AudioSource component = base.gameObject.GetComponent<AudioSource>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			string text = PresetBundle.Preset3D.RolloffPrefab ?? "default";
			Object @object = Resources.Load("Audio/RolloffPresets/" + text);
			if (@object == null)
			{
				LogStartFailure(string.Format("rolloff preset \"{0}\" was not found", text));
				return false;
			}
			GameObject gameObject = Object.Instantiate(@object, base.transform.position, base.transform.rotation) as GameObject;
			gameObject.transform.parent = base.transform;
			currentAudioSrc = gameObject.GetComponent<AudioSource>();
			if (currentAudioSrc == null)
			{
				LogStartFailure(string.Format("no AudioSource component is attached to rolloff preset \"{0}\"", text));
				Object.Destroy(gameObject);
				return false;
			}
		}
		return true;
	}

	protected void LogStartFailure(string reason)
	{
		CspUtils.DebugLog(string.Format("Unable to start clip <{0}> because {1}", base.gameObject.name, reason));
	}

	protected IEnumerator StopClip()
	{
		float fadeOutTime = 0f;
		if (PresetBundle != null && PresetBundle.PresetFade != null)
		{
			fadeOutTime = PresetBundle.PresetFade.FadeOutTime;
		}
		if (IsPlaying && fadeOutTime > 0f)
		{
			yield return StopClipWithFade(fadeOutTime);
		}
		else
		{
			StopImmediate();
		}
	}

	protected IEnumerator StopClipWithFade(float fadeTime)
	{
		if (!currentAudioSrc.loop && fadeTime > currentAudioSrc.clip.length - currentAudioSrc.time)
		{
			fadeTime = currentAudioSrc.clip.length - currentAudioSrc.time;
		}
		float fadeStartTime = Time.time;
		float fadeEndTime = fadeStartTime + fadeTime;
		while (Time.time < fadeEndTime)
		{
			float newVolume = Mathf.SmoothStep(GetMixedVolume(), 0f, (Time.time - fadeStartTime) / fadeTime);
			if (newVolume < currentAudioSrc.volume)
			{
				currentAudioSrc.volume = newVolume;
			}
			yield return 0;
		}
		StopImmediate();
	}

	public void StopImmediate()
	{
		if (currentAudioSrc != null)
		{
			currentAudioSrc.Stop();
		}
		if (nextAudioSrc != null)
		{
			nextAudioSrc.Stop();
		}
		DonePlaying();
		StopAllCoroutines();
	}

	protected int ChooseNextSequentialClipIndex()
	{
		int result = nextSequentialClipIndex;
		nextSequentialClipIndex = (nextSequentialClipIndex + 1) % Clips.GetLength(0);
		return result;
	}

	protected int ChooseNextRandomClipIndex()
	{
		int length = Clips.GetLength(0);
		if (length <= 1)
		{
			return 0;
		}
		if (alreadyPlayedRandomClips == null || alreadyPlayedRandomClips.GetLength(0) != length)
		{
			alreadyPlayedRandomClips = new bool[length];
			numberPlayedRandomClips = 0;
			for (int i = 0; i < length; i++)
			{
				alreadyPlayedRandomClips[i] = false;
			}
		}
		int num = Random.Range(0, length - numberPlayedRandomClips);
		int num2 = -1;
		for (int j = 0; j < length; j++)
		{
			if (!alreadyPlayedRandomClips[j])
			{
				if (num == 0)
				{
					num2 = j;
					alreadyPlayedRandomClips[num2] = true;
					numberPlayedRandomClips++;
					break;
				}
				num--;
			}
		}
		if (numberPlayedRandomClips >= length)
		{
			for (int k = 0; k < length; k++)
			{
				if (k != num2)
				{
					alreadyPlayedRandomClips[k] = false;
				}
				else
				{
					alreadyPlayedRandomClips[k] = true;
				}
			}
			numberPlayedRandomClips = 1;
		}
		if (num2 < 0)
		{
			CspUtils.DebugLog("Unexpected state when choosing a random clip in <" + base.gameObject.name + ">.  Playing clip zero.");
			Reset();
			num2 = ChooseNextRandomClipIndex();
		}
		return num2;
	}

	protected void SwapAudioSources()
	{
		AudioClip clip = currentAudioSrc.clip;
		if (nextAudioSrc == null)
		{
			nextAudioSrc = Utils.AddComponent<AudioSource>(currentAudioSrc.gameObject);
			nextAudioSrc.playOnAwake = false;
		}
		currentAudioSrc.clip = nextAudioSrc.clip;
		nextAudioSrc.clip = clip;
	}

	protected override void RegisterWithAudioManager()
	{
		if (PresetBundle != null)
		{
			base.RegisterWithAudioManager();
		}
		if (base.IsRegistered)
		{
			Volume = PresetBundle.PresetVolume.Volume;
		}
	}

	protected override float GetMixerVolume()
	{
		float result = 1f;
		switch (AudioManager.GetGroupForCategory(AudioCategory))
		{
		case AudioGroupEnum.Music:
			result = mixerSettings.MusicVolume;
			break;
		case AudioGroupEnum.Unknown:
		case AudioGroupEnum.Ambient:
		case AudioGroupEnum.Effects:
			result = mixerSettings.SoundFxVolume;
			break;
		case AudioGroupEnum.UI:
			result = mixerSettings.SoundFxVolume;
			break;
		case AudioGroupEnum.VOX:
			result = mixerSettings.VOXVolume;
			break;
		}
		return result;
	}

	protected void LocalHeroBoost()
	{
		heroScale = LocalHeroScale;
		instanceAttenuationScale = 1f;
		if (currentAudioSrc != null)
		{
			currentAudioSrc.minDistance = PresetBundle.Preset3D.AttenuationStartDistance;
		}
		Volume = Volume;
	}

	protected bool PlayCurrentAudioSource()
	{
		bool flag = true;
		if (RequestsCrossfade)
		{
			IsPersistent = true;
			flag = AppShell.Instance.AudioManager.RequestCrossfade(this);
		}
		if (flag)
		{
			currentAudioSrc.Play();
			if (storedTimeOffset != 0f)
			{
				currentAudioSrc.time = storedTimeOffset;
				storedTimeOffset = 0f;
			}
		}
		return flag;
	}

	public static ShsAudioSource PlayFromPrefab(GameObject sfxPrefab, Transform location, PrePlayDelegate onPrePlay)
	{
		ShsAudioSource shsAudioSource = null;
		if (sfxPrefab != null)
		{
			GameObject g = Object.Instantiate(sfxPrefab, location.position, location.rotation) as GameObject;
			shsAudioSource = Utils.GetComponent<ShsAudioSource>(g);
			if (onPrePlay != null)
			{
				onPrePlay(shsAudioSource);
			}
			if (shsAudioSource != null && !shsAudioSource.PlayOnAwake)
			{
				shsAudioSource.Play();
			}
		}
		return shsAudioSource;
	}

	public static ShsAudioSource PlayFromPrefab(GameObject sfxPrefab, Transform location)
	{
		return PlayFromPrefab(sfxPrefab, location, null);
	}

	public static ShsAudioSource PlayFromPrefab(GameObject sfxPrefab)
	{
		return PlayFromPrefab(sfxPrefab, (!(sfxPrefab != null)) ? null : sfxPrefab.transform);
	}

	public static ShsAudioSource PlayAutoSound(GameObject sfxPrefab, Transform location)
	{
		ShsAudioSource shsAudioSource = PlayFromPrefab(sfxPrefab, location);
		if (shsAudioSource != null)
		{
			shsAudioSource.DestroyWhenFinished();
		}
		return shsAudioSource;
	}

	public static ShsAudioSource PlayAutoSound(GameObject sfxPrefab)
	{
		return PlayAutoSound(sfxPrefab, (!(sfxPrefab != null)) ? null : sfxPrefab.transform);
	}

	public static void PreloadSound(GameObject sfxPrefab, GameObject parent, SoundPreloadedDelegate onFinished, object extraData)
	{
		if (sfxPrefab == null)
		{
			CspUtils.DebugLog("Asked to preload a null audio prefab");
			return;
		}
		ShsAudioSource component = sfxPrefab.GetComponent<ShsAudioSource>();

///////////// this block quick fix by CSP, because above line was not finding component. ////////////////////////////////////////////////////////////////////
		Component[] components = sfxPrefab.GetComponents<Component>() as Component[];
        if(components.Length > 0) {
                //Print all the components to console
                foreach(Component comp in components) {
                    CspUtils.DebugLog ("component="+comp);
					if (comp is ShsAudioSource)
						component = (ShsAudioSource)comp;
				}
        }  
//////////////////////////////////////////////////////////////////////////////////

		if (component == null)
		{
			CspUtils.DebugLog(string.Format("Asked to preload audio prefab <{0}>, but no ShsAudioSource was found", sfxPrefab.name));
			return;
		}
		bool playOnAwake = component.PlayOnAwake;
		component.PlayOnAwake = false;
		GameObject gameObject = Object.Instantiate(sfxPrefab) as GameObject;
		gameObject.transform.parent = ((!(parent == null)) ? parent.transform : null);
		ShsAudioSource component2 = gameObject.GetComponent<ShsAudioSource>();

///////////// this block quick fix by CSP, because above line was not finding component. ////////////////////////////////////////////////////////////////////
		components = gameObject.GetComponents<Component>() as Component[];
        if(components.Length > 0) {
                //Print all the components to console
                foreach(Component comp in components) {
                    CspUtils.DebugLog ("component="+comp);
					if (comp is ShsAudioSource)
						component2 = (ShsAudioSource)comp;
				}
        }  
//////////////////////////////////////////////////////////////////////////////////

		component2.RequestClipsFromAssetBundles(component2.Clips);
		gameObject.AddComponent<CoroutineContainer>().StartCoroutine(WaitForClipsToLoad(component2, onFinished, extraData));
		component.PlayOnAwake = playOnAwake;
		component2.PlayOnAwake = playOnAwake;
	}

	private static IEnumerator WaitForClipsToLoad(ShsAudioSource audioSrc, SoundPreloadedDelegate onFinished, object extraData)
	{
		bool loaded2 = false;
		do
		{
			yield return 0;
			loaded2 = true;
			AudioClipReference[] clips = audioSrc.Clips;
			foreach (AudioClipReference clip in clips)
			{
				if (clip.WaitingForAssetLoad)
				{
					// CSP temporarily commented out next 2 lines.
					//CspUtils.DebugLog("########waiting for clip to load: " + clip.Clip.name);
					//loaded2 = false;
				}
			}
		}
		while (!loaded2);
		AudioClipReference[] clips2 = audioSrc.Clips;
		foreach (AudioClipReference clip2 in clips2)
		{
			clip2.BundleName = SABundle.None;
			clip2.AssetName = null;
		}
		if (onFinished != null)
		{
			onFinished(audioSrc, extraData);
		}
	}
}
