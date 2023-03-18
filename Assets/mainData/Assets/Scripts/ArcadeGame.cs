using System.Runtime.CompilerServices;
using System.Xml.Serialization;

[XmlRoot(ElementName = "game")]
public class ArcadeGame
{
	[CompilerGenerated]
	private string _003CName_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CDescription_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CKeyword_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CDisplayImage_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CHelpImage_003Ek__BackingField;

	[XmlElement(ElementName = "name")]
	public string Name
	{
		[CompilerGenerated]
		get
		{
			return _003CName_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CName_003Ek__BackingField = value;
		}
	}

	[XmlElement(ElementName = "description")]
	public string Description
	{
		[CompilerGenerated]
		get
		{
			return _003CDescription_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CDescription_003Ek__BackingField = value;
		}
	}

	[XmlElement(ElementName = "keyword")]
	public string Keyword
	{
		[CompilerGenerated]
		get
		{
			return _003CKeyword_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CKeyword_003Ek__BackingField = value;
		}
	}

	[XmlElement(ElementName = "img")]
	public string DisplayImage
	{
		[CompilerGenerated]
		get
		{
			return _003CDisplayImage_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CDisplayImage_003Ek__BackingField = value;
		}
	}

	[XmlElement(ElementName = "helperimg")]
	public string HelpImage
	{
		[CompilerGenerated]
		get
		{
			return _003CHelpImage_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CHelpImage_003Ek__BackingField = value;
		}
	}
}
