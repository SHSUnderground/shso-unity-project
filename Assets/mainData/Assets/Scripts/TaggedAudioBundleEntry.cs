using System.Runtime.CompilerServices;
using System.Xml.Serialization;

[XmlRoot(ElementName = "audio-reference")]
public class TaggedAudioBundleEntry
{
	[CompilerGenerated]
	private string _003CKey_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CBundleName_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CAssetName_003Ek__BackingField;

	[XmlElement(ElementName = "key")]
	public string Key
	{
		[CompilerGenerated]
		get
		{
			return _003CKey_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CKey_003Ek__BackingField = value;
		}
	}

	[XmlElement(ElementName = "bundle")]
	public string BundleName
	{
		[CompilerGenerated]
		get
		{
			return _003CBundleName_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CBundleName_003Ek__BackingField = value;
		}
	}

	[XmlElement(ElementName = "asset")]
	public string AssetName
	{
		[CompilerGenerated]
		get
		{
			return _003CAssetName_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CAssetName_003Ek__BackingField = value;
		}
	}
}
