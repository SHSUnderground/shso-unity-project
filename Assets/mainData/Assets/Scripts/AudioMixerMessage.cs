public class AudioMixerMessage : ShsEventMessage
{
	public AudioMixerValues MixerSettings;

	public AudioMixerMessage(AudioMixerValues mixerSettings)
	{
		MixerSettings = mixerSettings;
	}
}
