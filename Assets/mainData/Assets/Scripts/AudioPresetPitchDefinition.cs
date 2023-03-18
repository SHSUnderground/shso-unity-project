using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

[XmlRoot(ElementName = "audio_preset_pitch")]
public class AudioPresetPitchDefinition : StaticDataDefinition, IStaticDataDefinitionTxt
{
	[XmlIgnore]
	public Dictionary<string, AudioPresetPitch> Presets;

	[XmlArrayItem("preset")]
	[XmlArray("preset_list")]
	public AudioPresetPitch[] _ForXmlParsingOnly
	{
		get
		{
			return null;
		}
		set
		{
			if (Presets == null)
			{
				Presets = new Dictionary<string, AudioPresetPitch>();
			}
			Presets.Clear();
			foreach (AudioPresetPitch audioPresetPitch in value)
			{
				Presets.Add(audioPresetPitch.PresetName, audioPresetPitch);
			}
		}
	}

	public void InitializeFromData(string xml)
	{
		using (StringReader textReader = new StringReader(xml))
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AudioPresetPitchDefinition));
			AudioPresetPitchDefinition audioPresetPitchDefinition = xmlSerializer.Deserialize(textReader) as AudioPresetPitchDefinition;
			Presets = audioPresetPitchDefinition.Presets;
		}
	}
}
