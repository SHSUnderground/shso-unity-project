using System.Xml.Serialization;

[XmlRoot(ElementName = "preset")]
public class AudioPresetVolume : AudioPreset
{
	protected float volume;

	protected float volumeVariation;

	[XmlElement(ElementName = "audio_volume")]
	public float Volume
	{
		get
		{
			return volume;
		}
		set
		{
			volume = value;
		}
	}

	[XmlElement(ElementName = "audio_volume_rand")]
	public float VolumeVariation
	{
		get
		{
			return volumeVariation;
		}
		set
		{
			volumeVariation = value;
		}
	}
}
