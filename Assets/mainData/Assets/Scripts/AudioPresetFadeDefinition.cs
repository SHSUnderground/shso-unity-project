using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

[XmlRoot(ElementName = "audio_preset_fade")]
public class AudioPresetFadeDefinition : StaticDataDefinition, IStaticDataDefinitionTxt
{
	[XmlIgnore]
	public Dictionary<string, AudioPresetFade> Presets;

	[XmlArray("preset_list")]
	[XmlArrayItem("preset")]
	public AudioPresetFade[] _ForXmlParsingOnly
	{
		get
		{
			return null;
		}
		set
		{
			if (Presets == null)
			{
				Presets = new Dictionary<string, AudioPresetFade>();
			}
			Presets.Clear();
			foreach (AudioPresetFade audioPresetFade in value)
			{
				Presets.Add(audioPresetFade.PresetName, audioPresetFade);
			}
		}
	}

	public void InitializeFromData(string xml)
	{
		using (StringReader textReader = new StringReader(xml))
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AudioPresetFadeDefinition));
			AudioPresetFadeDefinition audioPresetFadeDefinition = xmlSerializer.Deserialize(textReader) as AudioPresetFadeDefinition;
			Presets = audioPresetFadeDefinition.Presets;
		}
	}
}
