using ShsAudio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class AudioManager
{
	public class MuteContext
	{
		public Dictionary<string, AudioPresetVolume> originalVolumePresets = new Dictionary<string, AudioPresetVolume>();

		public bool muted = true;
	}

	public delegate void AudioManagerInitializedDelegate();

	public const string AUDIO_CATEGORY_VOICE_LIMITING_PATH = "Audio/audio_category_limiting";

	protected static Dictionary<AudioCategoryEnum, AudioGroupEnum> CategoryToGroupMap;

	public AudioMixerValues MixerSettings;

	public AudioPresetBundleDefinition PresetBundleDefinitions;

	protected Dictionary<int, ShsAudioBase> registeredAudioSources;

	protected Dictionary<string, VoiceLimitingPlayInfo> prefabAudioLimitingDictionary;

	protected Dictionary<AudioCategoryEnum, VoiceLimitingPlayInfo> categoryAudioLimitingDictionary;

	protected List<ShsAudioSource> sourcesPlayedThisFrame = new List<ShsAudioSource>();

	protected ShsAudioBase currentMusic;

	protected ShsAudioBase pendingMusic;

	protected CrossfadeController crossfadeController = new CrossfadeController();

	protected List<MuteContext> activeMutes = new List<MuteContext>();

	[CompilerGenerated]
	private bool _003CPlayedStartupMusic_003Ek__BackingField;

	public ShsAudioBase[] RegisteredAudioSources
	{
		get
		{
			ShsAudioBase[] array = new ShsAudioBase[registeredAudioSources.Count];
			registeredAudioSources.Values.CopyTo(array, 0);
			return array;
		}
	}

	public ShsAudioBase CurrentMusic
	{
		get
		{
			return currentMusic;
		}
	}

	public bool PlayedStartupMusic
	{
		[CompilerGenerated]
		get
		{
			return _003CPlayedStartupMusic_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CPlayedStartupMusic_003Ek__BackingField = value;
		}
	}

	public AudioManager()
	{
		registeredAudioSources = new Dictionary<int, ShsAudioBase>();
		prefabAudioLimitingDictionary = new Dictionary<string, VoiceLimitingPlayInfo>();
		categoryAudioLimitingDictionary = new Dictionary<AudioCategoryEnum, VoiceLimitingPlayInfo>();
		MixerSettings = new AudioMixerValues();
		if (ShsPlayerPrefs.HasKey(ShsPlayerPrefs.Keys.AudioEffectsVolume))
		{
			MixerSettings.SoundFxVolume = ShsPlayerPrefs.GetFloat(ShsPlayerPrefs.Keys.AudioEffectsVolume);
		}
		if (ShsPlayerPrefs.HasKey(ShsPlayerPrefs.Keys.AudioMusicVolume))
		{
			MixerSettings.MusicVolume = ShsPlayerPrefs.GetFloat(ShsPlayerPrefs.Keys.AudioMusicVolume);
		}
		if (ShsPlayerPrefs.HasKey(ShsPlayerPrefs.Keys.AudioVOXVolume))
		{
			MixerSettings.VOXVolume = ShsPlayerPrefs.GetFloat(ShsPlayerPrefs.Keys.AudioVOXVolume);
		}
		AppShell.Instance.EventMgr.AddListener<AudioMixerMessage>(OnAudioMixerChanged);
		PlayedStartupMusic = false;
	}

	static AudioManager()
	{
		CategoryToGroupMap = new Dictionary<AudioCategoryEnum, AudioGroupEnum>();
		CategoryToGroupMap[AudioCategoryEnum.Unassigned] = AudioGroupEnum.Effects;
		CategoryToGroupMap[AudioCategoryEnum.SfxHero] = AudioGroupEnum.Effects;
		CategoryToGroupMap[AudioCategoryEnum.SfxVillain] = AudioGroupEnum.Effects;
		CategoryToGroupMap[AudioCategoryEnum.SfxEmitter] = AudioGroupEnum.Effects;
		CategoryToGroupMap[AudioCategoryEnum.SfxHud] = AudioGroupEnum.UI;
		CategoryToGroupMap[AudioCategoryEnum.SfxGeneral] = AudioGroupEnum.Effects;
		CategoryToGroupMap[AudioCategoryEnum.MusicScore] = AudioGroupEnum.Music;
		CategoryToGroupMap[AudioCategoryEnum.MusicEmitter] = AudioGroupEnum.Music;
		CategoryToGroupMap[AudioCategoryEnum.Vox] = AudioGroupEnum.VOX;
	}

	public static AudioGroupEnum GetGroupForCategory(AudioCategoryEnum category)
	{
		return CategoryToGroupMap[category];
	}

	public void Initialize(TransactionMonitor transactionMonitor, AudioManagerInitializedDelegate onInitialized)
	{
		LoadPresetDefinitions();
		Definitions.LoadTaggedAudioReferences(delegate
		{
			if (onInitialized != null)
			{
				onInitialized();
			}
		}, transactionMonitor, null);
		AppShell.Instance.BundleLoader.FetchAssetBundle(Helpers.GetAudioBundleName(SABundle.Global_Persistent), OnPersistentBundleLoaded, null, false);
	}

	private void OnPersistentBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if (string.IsNullOrEmpty(response.Error) && currentMusic == null && !GameController.GetController().isTestScene && AppShell.Instance.startupMusic != null)
		{
			ShsAudioSource.PlayAutoSound(AppShell.Instance.startupMusic.gameObject);
			PlayedStartupMusic = true;
		}
	}

	public void LoadPresetDefinitions()
	{
		PresetBundleDefinitions = new AudioPresetBundleDefinition();
		AppShell.Instance.DataManager.LoadGameData("Audio/audio_category_limiting", OnCategoryVoiceLimitingLoaded);
	}

	public AudioMixerValues RegisterAudioSource(ShsAudioBase audioSource)
	{
		registeredAudioSources[audioSource.gameObject.GetInstanceID()] = audioSource;
		return MixerSettings;
	}

	public void UnregisterAudioSource(ShsAudioBase audioSource)
	{
		registeredAudioSources.Remove(audioSource.gameObject.GetInstanceID());
	}

	public bool CanPlayAudioSource(ShsAudioSource audioSource)
	{
		bool flag = true;
		if (string.IsNullOrEmpty(audioSource.PrefabName))
		{
			CspUtils.DebugLog("Voice limiting by prefab name not possible without prefab name, letting source play. Source <" + audioSource.gameObject.name + ", , priority=" + audioSource.PresetBundle.PresetVoice.Priority + ", maxplaybacks=" + audioSource.PresetBundle.PresetVoice.MaxPlaybacks + ", stealing=" + audioSource.PresetBundle.PresetVoice.StealBehavior.ToString() + ">.");
			return true;
		}
		if (sourcesPlayedThisFrame.Contains(audioSource))
		{
			CspUtils.DebugLog("Source already played this frame. Source <" + audioSource.gameObject.name + ", , priority=" + audioSource.PresetBundle.PresetVoice.Priority + ", maxplaybacks=" + audioSource.PresetBundle.PresetVoice.MaxPlaybacks + ", stealing=" + audioSource.PresetBundle.PresetVoice.StealBehavior.ToString() + ">.");
			return false;
		}
		if (audioSource.PresetBundleName == "VFX_Hero")
		{
			if (VOManager.Instance != null && VOManager.Instance.IsEmitterInUse(audioSource.transform.root.gameObject))
			{
				return false;
			}
			audioSource.gameObject.AddComponent<VOTransitionHack>();
		}
		VoiceLimitingPlayInfo value = null;
		VoiceLimitingPlayInfo value2 = null;
		if (!prefabAudioLimitingDictionary.TryGetValue(audioSource.PrefabName, out value2))
		{
			value2 = new VoiceLimitingPlayInfo(audioSource.PrefabName, audioSource.PresetBundle.PresetVoice.MaxPlaybacks);
			prefabAudioLimitingDictionary.Add(audioSource.PrefabName, value2);
		}
		if (value2.MaxPlaybacks != audioSource.PresetBundle.PresetVoice.MaxPlaybacks)
		{
			CspUtils.DebugLog("Voice limiting inconsistency between MaxPlaybacks on audio source and earlier versions of the same prefab.  Source <" + audioSource.gameObject.name + ", , prefab name=" + audioSource.PrefabName + ", maxplaybacks=" + audioSource.PresetBundle.PresetVoice.MaxPlaybacks + ">. Earlier prefab data <, maxplaybacks=" + value2.MaxPlaybacks + ">.");
		}
		if (!CheckAudioSourceForVoiceLimiting(value2, audioSource))
		{
			flag = false;
		}
		else if (categoryAudioLimitingDictionary.TryGetValue(audioSource.AudioCategory, out value))
		{
			if (!CheckAudioSourceForVoiceLimiting(value, audioSource))
			{
				flag = false;
			}
		}
		else
		{
			CspUtils.DebugLog("Voice limiting by category not possible without category limiting data, letting source play. Source <" + audioSource.gameObject.name + ", , category=" + audioSource.AudioCategory.ToString() + ", priority=" + audioSource.PresetBundle.PresetVoice.Priority + ", maxplaybacks=" + audioSource.PresetBundle.PresetVoice.MaxPlaybacks + ", stealing=" + audioSource.PresetBundle.PresetVoice.StealBehavior.ToString() + ">.");
		}
		if (flag)
		{
			if (value != null)
			{
				value.AddAudioSource(audioSource);
			}
			if (value2 != null)
			{
				value2.AddAudioSource(audioSource);
			}
			if (AppShell.Instance.gameObject.active)
			{
				AppShell.Instance.StartCoroutine(PreventDuplicatePlays(audioSource));
			}
		}
		return flag;
	}

	protected IEnumerator PreventDuplicatePlays(ShsAudioSource source)
	{
		sourcesPlayedThisFrame.Add(source);
		yield return 0;
		sourcesPlayedThisFrame.Remove(source);
	}

	protected bool CheckAudioSourceForVoiceLimiting(VoiceLimitingPlayInfo voiceLimitingInfo, ShsAudioSource audioSource)
	{
		voiceLimitingInfo.PruneNullSources();
		if (voiceLimitingInfo.MaxPlaybacks > voiceLimitingInfo.PlayingCount)
		{
			return true;
		}
		ShsAudioSource shsAudioSource = voiceLimitingInfo.FindLowestPriorityAudioSource(audioSource.PresetBundle.PresetVoice.StealBehavior);
		if (shsAudioSource != null)
		{
			if (shsAudioSource.PresetBundle.PresetVoice.Priority > audioSource.PresetBundle.PresetVoice.Priority)
			{
				shsAudioSource.Steal();
				voiceLimitingInfo.RemoveAudioSource(shsAudioSource);
				return true;
			}
			if (shsAudioSource.PresetBundle.PresetVoice.Priority == audioSource.PresetBundle.PresetVoice.Priority)
			{
				if (audioSource.PresetBundle.PresetVoice.StealBehavior != 0)
				{
					shsAudioSource.Steal();
					voiceLimitingInfo.RemoveAudioSource(shsAudioSource);
					return true;
				}
				CspUtils.DebugLog("Voice limiting caused audio source to 'fail' to play (no lower priority sources).  Voice Limiting <limiting group=" + voiceLimitingInfo.GroupName + ", maxplaybacks=" + voiceLimitingInfo.MaxPlaybacks + ">, Source <" + audioSource.gameObject.name + ", prefab name=" + audioSource.PrefabName + ", category=" + audioSource.AudioCategory.ToString() + ", priority=" + audioSource.PresetBundle.PresetVoice.Priority + ", maxplaybacks=" + audioSource.PresetBundle.PresetVoice.MaxPlaybacks + ", stealing=" + audioSource.PresetBundle.PresetVoice.StealBehavior.ToString() + ">.");
				return false;
			}
			CspUtils.DebugLog("Voice limiting caused audio source to 'fail' to play (only higher priority sources).  Voice Limiting <limiting group=" + voiceLimitingInfo.GroupName + ", maxplaybacks=" + voiceLimitingInfo.MaxPlaybacks + ">, Source <" + audioSource.gameObject.name + ", prefab name=" + audioSource.PrefabName + ", category=" + audioSource.AudioCategory.ToString() + ", priority=" + audioSource.PresetBundle.PresetVoice.Priority + ", maxplaybacks=" + audioSource.PresetBundle.PresetVoice.MaxPlaybacks + ", stealing=" + audioSource.PresetBundle.PresetVoice.StealBehavior.ToString() + ">.");
			return false;
		}
		CspUtils.DebugLog("Voice limiting inconsistency - no lowest priority source, but playback count below max!  Voice Limiting <limiting group=" + voiceLimitingInfo.GroupName + ", maxplaybacks=" + voiceLimitingInfo.MaxPlaybacks + ">, New audio source <" + audioSource.gameObject.name + ", " + audioSource.PrefabName + "> not allowed to play.");
		return false;
	}

	public void DonePlayingAudioSource(ShsAudioSource audioSource)
	{
		VoiceLimitingPlayInfo value = null;
		if (prefabAudioLimitingDictionary.TryGetValue(audioSource.PrefabName, out value))
		{
			value.RemoveAudioSource(audioSource);
		}
		if (categoryAudioLimitingDictionary.TryGetValue(audioSource.AudioCategory, out value))
		{
			value.RemoveAudioSource(audioSource);
		}
		sourcesPlayedThisFrame.Remove(audioSource);
		if (audioSource == currentMusic)
		{
			currentMusic = null;
		}
		if (audioSource == pendingMusic)
		{
			pendingMusic = null;
		}
	}

	private void OnAudioMixerChanged(AudioMixerMessage e)
	{
		foreach (ShsAudioBase value in registeredAudioSources.Values)
		{
			value.OnAudioMixerChanged(e);
		}
	}

	public AudioGroupEnum GetGroupForSource(ShsAudioSource audioSource)
	{
		return GetGroupForCategory(audioSource.AudioCategory);
	}

	public void RegisterCrossfadeController(CrossfadeController controller)
	{
		CspUtils.DebugLog("Registering crossfade controller: " + controller);
		crossfadeController = controller;
		if (crossfadeController == null)
		{
			CspUtils.DebugLog("Asked to register a null crossfade controller -- please use UnregisterCrossfadeController instead.");
			crossfadeController = new CrossfadeController();
		}
	}

	public void UnregisterCrossfadeController(CrossfadeController controller)
	{
		CspUtils.DebugLog("Unregistering crossfade controller: " + controller);
		if (controller != crossfadeController)
		{
			CspUtils.DebugLog("Asked to unregister an out-of-date crossfade controller; ignoring unregistration.");
			return;
		}
		if (controller != null)
		{
			controller.OnUnregistered();
		}
		crossfadeController = new CrossfadeController();
	}

	public void DeclarePendingMusic(ShsAudioBase audioSource)
	{
		if (pendingMusic != null && pendingMusic != audioSource)
		{
			pendingMusic.Stop();
		}
		pendingMusic = audioSource;
	}

	public bool RequestCrossfade(ShsAudioBase audioSource)
	{
		if (pendingMusic != null && audioSource != pendingMusic)
		{
			pendingMusic.Stop();
		}
		pendingMusic = null;
		ShsAudioBase shsAudioBase = crossfadeController.RequestCrossfade(currentMusic, audioSource);
		if (shsAudioBase != currentMusic)
		{
			MusicChangedMessage msg = new MusicChangedMessage(currentMusic, shsAudioBase);
			currentMusic = shsAudioBase;
			AppShell.Instance.EventMgr.Fire(this, msg);
			return true;
		}
		return false;
	}

	protected void OnCategoryVoiceLimitingLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
		}
		else
		{
			foreach (DataWarehouse item in response.Data.GetIterator("//category"))
			{
				string value = item.TryGetString("name", null);
				AudioCategoryEnum audioCategoryEnum = (AudioCategoryEnum)(int)Enum.Parse(typeof(AudioCategoryEnum), value);
				int maxPlaybacks = item.TryGetInt("max_playbacks", 0);
				categoryAudioLimitingDictionary.Add(audioCategoryEnum, new VoiceLimitingPlayInfo(audioCategoryEnum.ToString(), maxPlaybacks));
			}
		}
	}

	public void DumpToLog()
	{
		CspUtils.DebugLog("**** Dumping the AudioManager state:");
		string text = string.Empty;
		foreach (KeyValuePair<int, ShsAudioBase> registeredAudioSource in registeredAudioSources)
		{
			ShsAudioBase value = registeredAudioSource.Value;
			if (value != null && value.gameObject != null)
			{
				string text2 = text;
				text = text2 + "(" + value.gameObject.name + ":" + ((!(value.audio != null) || !(value.audio.clip != null)) ? string.Empty : value.audio.clip.name) + "), ";
			}
			else
			{
				CspUtils.DebugLog("Dead ShsAudioBase with ID <" + registeredAudioSource.Key + "> still registered with the AudioManager.");
			}
		}
		CspUtils.DebugLog("**** Registered Audio Sources = " + text);
	}

	public MuteContext MuteAllExcept(string[] presetBundleNames)
	{
		MuteContext muteContext = new MuteContext();
		foreach (KeyValuePair<string, AudioPresetBundle> presetBundle in PresetBundleDefinitions.PresetBundles)
		{
			if (Array.IndexOf(presetBundleNames, presetBundle.Key) == -1)
			{
				AudioPresetVolume value = presetBundle.Value.PresetVolume;
				foreach (MuteContext activeMute in activeMutes)
				{
					if (activeMute.originalVolumePresets.TryGetValue(presetBundle.Key, out value))
					{
						break;
					}
				}
				muteContext.originalVolumePresets[presetBundle.Key] = value;
				presetBundle.Value.AudioPresetVolumeName = "volume_00";
			}
		}
		foreach (ShsAudioBase value2 in registeredAudioSources.Values)
		{
			ShsAudioSource shsAudioSource = value2 as ShsAudioSource;
			if (shsAudioSource != null && Array.IndexOf(presetBundleNames, shsAudioSource.PresetBundleName) == -1)
			{
				shsAudioSource.Volume = 0f;
			}
		}
		activeMutes.Add(muteContext);
		return muteContext;
	}

	public bool UnMute(MuteContext mutedState)
	{
		if (mutedState == null || !mutedState.muted)
		{
			return false;
		}
		mutedState.muted = false;
		if (!activeMutes.Contains(mutedState))
		{
			CspUtils.DebugLog("Mute Context was not created by the Audio Manager");
			return false;
		}
		activeMutes.Remove(mutedState);
		foreach (KeyValuePair<string, AudioPresetVolume> originalVolumePreset in mutedState.originalVolumePresets)
		{
			bool flag = false;
			foreach (MuteContext activeMute in activeMutes)
			{
				if (activeMute.originalVolumePresets.ContainsKey(originalVolumePreset.Key))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				PresetBundleDefinitions.PresetBundles[originalVolumePreset.Key].PresetVolume = originalVolumePreset.Value;
			}
		}
		foreach (ShsAudioBase value in registeredAudioSources.Values)
		{
			ShsAudioSource shsAudioSource = value as ShsAudioSource;
			if (shsAudioSource != null)
			{
				shsAudioSource.Volume = shsAudioSource.PresetBundle.PresetVolume.Volume;
			}
		}
		return true;
	}

	public void OnSceneTransition()
	{
		if (activeMutes.Count <= 0)
		{
			return;
		}
		CspUtils.DebugLog("There are " + activeMutes.Count + " outstanding mutes!  Resetting...");
		while (activeMutes.Count > 0)
		{
			if (!UnMute(activeMutes[0]))
			{
				activeMutes.RemoveAt(0);
			}
		}
	}
}
