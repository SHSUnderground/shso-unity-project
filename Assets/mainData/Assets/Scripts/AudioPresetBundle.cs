using System.Xml.Serialization;

[XmlRoot(ElementName = "bundle")]
public class AudioPresetBundle
{
	protected static AudioPresetBundleDefinition _LoadingRefPresetDefinitions;

	protected string bundleName;

	protected AudioPresetVolume presetVolume;

	protected AudioPreset3D preset3D;

	protected AudioPresetPitch presetPitch;

	protected AudioPresetFade presetFade;

	protected AudioPresetVoice presetVoice;

	[XmlElement(ElementName = "bundle_name")]
	public string BundleName
	{
		get
		{
			return bundleName;
		}
		set
		{
			bundleName = value;
		}
	}

	[XmlIgnore]
	public AudioPresetVolume PresetVolume
	{
		get
		{
			return presetVolume;
		}
		set
		{
			presetVolume = value;
		}
	}

	[XmlIgnore]
	public AudioPreset3D Preset3D
	{
		get
		{
			return preset3D;
		}
		set
		{
			preset3D = value;
		}
	}

	[XmlIgnore]
	public AudioPresetPitch PresetPitch
	{
		get
		{
			return presetPitch;
		}
		set
		{
			presetPitch = value;
		}
	}

	[XmlIgnore]
	public AudioPresetFade PresetFade
	{
		get
		{
			return presetFade;
		}
		set
		{
			presetFade = value;
		}
	}

	[XmlIgnore]
	public AudioPresetVoice PresetVoice
	{
		get
		{
			return presetVoice;
		}
		set
		{
			presetVoice = value;
		}
	}

	[XmlElement(ElementName = "audio_preset_volume")]
	public string AudioPresetVolumeName
	{
		get
		{
			return (presetVolume != null) ? presetVolume.PresetName : "null";
		}
		set
		{
			if (value != null && _LoadingRefPresetDefinitions.VolumeDefinitions != null)
			{
				if (!_LoadingRefPresetDefinitions.VolumeDefinitions.Presets.TryGetValue(value, out presetVolume))
				{
					CspUtils.DebugLog("Unable to find audio volume preset by name <" + value + ">.");
				}
			}
			else
			{
				CspUtils.DebugLog("Value is null or volume definitions are null while setting audio preset with name <" + value + ">, <" + bundleName + ">.");
			}
		}
	}

	[XmlElement(ElementName = "audio_preset_pitch")]
	public string AudioPresetPitchName
	{
		get
		{
			return (presetPitch != null) ? presetPitch.PresetName : "null";
		}
		set
		{
			if (value != null && _LoadingRefPresetDefinitions.PitchDefinitions != null)
			{
				if (!_LoadingRefPresetDefinitions.PitchDefinitions.Presets.TryGetValue(value, out presetPitch))
				{
					CspUtils.DebugLog("Unable to find audio pitch preset by name <" + value + ">.");
				}
			}
			else
			{
				CspUtils.DebugLog("Value is null or pitch definitions are null while setting audio preset with name <" + value + ">, <" + bundleName + ">.");
			}
		}
	}

	[XmlElement(ElementName = "audio_preset_3d")]
	public string AudioPreset3DName
	{
		get
		{
			return (preset3D != null) ? preset3D.PresetName : "null";
		}
		set
		{
			if (value != null && _LoadingRefPresetDefinitions.ThreeDDefinitions != null)
			{
				if (!_LoadingRefPresetDefinitions.ThreeDDefinitions.Presets.TryGetValue(value, out preset3D))
				{
					CspUtils.DebugLog("Unable to find audio 3D preset by name <" + value + ">.");
				}
			}
			else
			{
				CspUtils.DebugLog("Value is null or 3D definitions are null while setting audio preset with name <" + value + ">, <" + bundleName + ">.");
			}
		}
	}

	[XmlElement(ElementName = "audio_preset_fade")]
	public string AudioPresetFadeName
	{
		get
		{
			return (presetFade != null) ? presetFade.PresetName : "null";
		}
		set
		{
			if (value != null && _LoadingRefPresetDefinitions.FadeDefinitions != null)
			{
				if (!_LoadingRefPresetDefinitions.FadeDefinitions.Presets.TryGetValue(value, out presetFade))
				{
					CspUtils.DebugLog("Unable to find audio fade preset by name <" + value + ">.");
				}
			}
			else
			{
				CspUtils.DebugLog("Value is null or fade definitions are null while setting audio preset with name <" + value + ">, <" + bundleName + ">.");
			}
		}
	}

	[XmlElement(ElementName = "audio_preset_voice")]
	public string AudioPresetVoiceName
	{
		get
		{
			return (presetVoice != null) ? presetVoice.PresetName : "null";
		}
		set
		{
			if (value != null && _LoadingRefPresetDefinitions.VoiceDefinitions != null)
			{
				if (!_LoadingRefPresetDefinitions.VoiceDefinitions.Presets.TryGetValue(value, out presetVoice))
				{
					CspUtils.DebugLog("Unable to find audio voice preset by name <" + value + ">.");
				}
			}
			else
			{
				CspUtils.DebugLog("Value is null or voice definitions are null while setting audio preset with name <" + value + ">, <" + bundleName + ">.");
			}
		}
	}

	public static void SetLoadingPresetDefinitions(AudioPresetBundleDefinition value)
	{
		_LoadingRefPresetDefinitions = value;
	}
}
