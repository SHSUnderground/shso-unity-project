using System;
using System.Xml.Serialization;

[Serializable]
[XmlRoot(ElementName = "TipTitle")]
public class SmartTipTitle
{
	[XmlElement(ElementName = "TitleKey")]
	public string Key;

	[XmlElement(ElementName = "Icon")]
	public string Icon;
}
