using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

[XmlRoot(ElementName = "audio_preset_3d")]
public class AudioPreset3DDefinition : StaticDataDefinition, IStaticDataDefinitionTxt
{
	[XmlIgnore]
	public Dictionary<string, AudioPreset3D> Presets;

	[XmlArray("preset_list")]
	[XmlArrayItem("preset")]
	public AudioPreset3D[] _ForXmlParsingOnly
	{
		get
		{
			return null;
		}
		set
		{
			if (Presets == null)
			{
				Presets = new Dictionary<string, AudioPreset3D>();
			}
			Presets.Clear();
			foreach (AudioPreset3D audioPreset3D in value)
			{
				Presets.Add(audioPreset3D.PresetName, audioPreset3D);
			}
		}
	}

	public void InitializeFromData(string xml)
	{
		using (StringReader textReader = new StringReader(xml))
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AudioPreset3DDefinition));
			AudioPreset3DDefinition audioPreset3DDefinition = xmlSerializer.Deserialize(textReader) as AudioPreset3DDefinition;
			Presets = audioPreset3DDefinition.Presets;
		}
	}
}
