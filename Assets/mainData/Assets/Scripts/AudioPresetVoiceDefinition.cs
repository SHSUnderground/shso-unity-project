using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

[XmlRoot(ElementName = "audio_preset_voice")]
public class AudioPresetVoiceDefinition : StaticDataDefinition, IStaticDataDefinitionTxt
{
	[XmlIgnore]
	public Dictionary<string, AudioPresetVoice> Presets;

	[XmlArrayItem("preset")]
	[XmlArray("preset_list")]
	public AudioPresetVoice[] _ForXmlParsingOnly
	{
		get
		{
			return null;
		}
		set
		{
			if (Presets == null)
			{
				Presets = new Dictionary<string, AudioPresetVoice>();
			}
			Presets.Clear();
			foreach (AudioPresetVoice audioPresetVoice in value)
			{
				Presets.Add(audioPresetVoice.PresetName, audioPresetVoice);
			}
		}
	}

	public void InitializeFromData(string xml)
	{
		using (StringReader textReader = new StringReader(xml))
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AudioPresetVoiceDefinition));
			AudioPresetVoiceDefinition audioPresetVoiceDefinition = xmlSerializer.Deserialize(textReader) as AudioPresetVoiceDefinition;
			Presets = audioPresetVoiceDefinition.Presets;
		}
	}
}
