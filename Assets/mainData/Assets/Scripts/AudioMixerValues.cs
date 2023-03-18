public class AudioMixerValues
{
	protected float musicVolume = 0.75f;

	protected float soundFxVolume = 0.75f;

	protected float uiSoundVolume = 0.75f;

	protected float voxVolume = 0.75f;

	public float MusicVolume
	{
		get
		{
			return musicVolume;
		}
		set
		{
			musicVolume = value;
			AppShell.Instance.EventMgr.Fire(null, new AudioMixerMessage(this));
		}
	}

	public float SoundFxVolume
	{
		get
		{
			return soundFxVolume;
		}
		set
		{
			soundFxVolume = value;
			AppShell.Instance.EventMgr.Fire(null, new AudioMixerMessage(this));
		}
	}

	public float UISoundVolume
	{
		get
		{
			return uiSoundVolume;
		}
		set
		{
			uiSoundVolume = value;
			AppShell.Instance.EventMgr.Fire(null, new AudioMixerMessage(this));
		}
	}

	public float VOXVolume
	{
		get
		{
			return voxVolume;
		}
		set
		{
			voxVolume = value;
			AppShell.Instance.EventMgr.Fire(null, new AudioMixerMessage(this));
		}
	}
}
