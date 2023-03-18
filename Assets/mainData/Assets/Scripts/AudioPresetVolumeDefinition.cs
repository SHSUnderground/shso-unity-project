using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

[XmlRoot(ElementName = "audio_preset_volume")]
public class AudioPresetVolumeDefinition : StaticDataDefinition, IStaticDataDefinitionTxt
{
	[XmlIgnore]
	public Dictionary<string, AudioPresetVolume> Presets;

	[XmlArray("preset_list")]
	[XmlArrayItem("preset")]
	public AudioPresetVolume[] _ForXmlParsingOnly
	{
		get
		{
			return null;
		}
		set
		{
			if (Presets == null)
			{
				Presets = new Dictionary<string, AudioPresetVolume>();
			}
			Presets.Clear();
			foreach (AudioPresetVolume audioPresetVolume in value)
			{
				Presets.Add(audioPresetVolume.PresetName, audioPresetVolume);
			}
		}
	}

	public void InitializeFromData(string xml)
	{
		using (StringReader textReader = new StringReader(xml))
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AudioPresetVolumeDefinition));
			AudioPresetVolumeDefinition audioPresetVolumeDefinition = xmlSerializer.Deserialize(textReader) as AudioPresetVolumeDefinition;
			Presets = audioPresetVolumeDefinition.Presets;
		}
	}
}
