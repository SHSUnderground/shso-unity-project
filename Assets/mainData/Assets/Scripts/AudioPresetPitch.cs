using System.Xml.Serialization;

[XmlRoot(ElementName = "preset")]
public class AudioPresetPitch : AudioPreset
{
	protected float pitch;

	protected float pitchVariation;

	[XmlElement(ElementName = "audio_pitch")]
	public float Pitch
	{
		get
		{
			return pitch;
		}
		set
		{
			pitch = value;
		}
	}

	[XmlElement(ElementName = "audio_pitch_rand")]
	public float PitchVariation
	{
		get
		{
			return pitchVariation;
		}
		set
		{
			pitchVariation = value;
		}
	}
}
