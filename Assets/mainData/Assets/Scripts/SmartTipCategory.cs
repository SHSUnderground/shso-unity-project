using System;
using System.Xml.Serialization;

[Serializable]
public class SmartTipCategory
{
	private string _rawName;

	[XmlIgnore]
	public string Name;

	[XmlElement(ElementName = "Weight")]
	public float Weight;

	[XmlElement(ElementName = "Name")]
	public string xmlRawName
	{
		get
		{
			return _rawName;
		}
		set
		{
			_rawName = value;
			if (!string.IsNullOrEmpty(_rawName))
			{
				Name = _rawName.Split(':')[0];
			}
		}
	}
}
