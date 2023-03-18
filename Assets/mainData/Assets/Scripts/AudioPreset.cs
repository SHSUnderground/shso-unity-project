using System.Xml.Serialization;

public abstract class AudioPreset
{
	protected string presetName;

	[XmlElement(ElementName = "preset_name")]
	public string PresetName
	{
		get
		{
			return presetName;
		}
		set
		{
			presetName = value;
		}
	}
}
