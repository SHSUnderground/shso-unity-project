using System.Runtime.CompilerServices;
using System.Xml.Serialization;

[XmlRoot(ElementName = "preset")]
public class AudioPreset3D : AudioPreset
{
	[CompilerGenerated]
	private float _003CAttenuationStartDistance_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CRolloffPrefab_003Ek__BackingField;

	[XmlElement(ElementName = "attenuation_start_distance")]
	public float AttenuationStartDistance
	{
		[CompilerGenerated]
		get
		{
			return _003CAttenuationStartDistance_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CAttenuationStartDistance_003Ek__BackingField = value;
		}
	}

	[XmlElement(ElementName = "rolloff_prefab", IsNullable = true)]
	public string RolloffPrefab
	{
		[CompilerGenerated]
		get
		{
			return _003CRolloffPrefab_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CRolloffPrefab_003Ek__BackingField = value;
		}
	}
}
