using System;
using System.Xml.Serialization;

[Serializable]
[XmlRoot(ElementName = "item_info")]
public class ScavengerXMLDefinition
{
	[XmlElement(ElementName = "ownableTypeID")]
	public int ownableTypeID;

	[XmlElement(ElementName = "readableName")]
	public string readableName;

	[XmlElement(ElementName = "icon")]
	public string icon;

	[XmlElement(ElementName = "zone_info")]
	public string zoneInfo;
}
