using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShsAudioSourceOld : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public AudioGroupEnum AudioCategory = AudioGroupEnum.Music;

	protected float volume = 1f;

	protected float pitch = 1f;

	protected AudioMixerValues mixerSettings;

	public float Volume
	{
		get
		{
			return volume;
		}
		set
		{
			volume = value;
			float num = 1f;
			switch (AudioCategory)
			{
			case AudioGroupEnum.Unknown:
			case AudioGroupEnum.Music:
			case AudioGroupEnum.Ambient:
				num = mixerSettings.MusicVolume;
				break;
			case AudioGroupEnum.Effects:
				num = mixerSettings.SoundFxVolume;
				break;
			case AudioGroupEnum.UI:
				num = mixerSettings.UISoundVolume;
				break;
			}
			base.audio.volume = volume * num;
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
			base.audio.pitch = pitch;
		}
	}

	private void Awake()
	{
		volume = base.audio.volume;
		pitch = base.audio.pitch;
	}

	private void Start()
	{
		mixerSettings = AppShell.Instance.AudioManager.MixerSettings;
		Volume = volume;
	}

	private void OnEnable()
	{
		AppShell.Instance.EventMgr.AddListener<AudioMixerMessage>(OnAudioMixerChanged);
	}

	private void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<AudioMixerMessage>(OnAudioMixerChanged);
	}

	private void OnAudioMixerChanged(AudioMixerMessage e)
	{
		Volume = volume;
	}
}
