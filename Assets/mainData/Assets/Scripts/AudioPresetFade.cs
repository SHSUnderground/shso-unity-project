using System.Xml.Serialization;

[XmlRoot(ElementName = "preset")]
public class AudioPresetFade : AudioPreset
{
	protected float fadeInTime;

	protected float fadeOutTime;

	[XmlElement(ElementName = "audio_fadein_time")]
	public float FadeInTime
	{
		get
		{
			return fadeInTime;
		}
		set
		{
			fadeInTime = value;
		}
	}

	[XmlElement(ElementName = "audio_fadeout_time")]
	public float FadeOutTime
	{
		get
		{
			return fadeOutTime;
		}
		set
		{
			fadeOutTime = value;
		}
	}
}
