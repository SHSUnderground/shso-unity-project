using System;
using System.Xml.Serialization;

[Serializable]
[XmlRoot(ElementName = "feature")]
public class FeatureDefinition
{
	[XmlElement(ElementName = "name")]
	public string name;

	[XmlElement(ElementName = "enabled")]
	public string enabled;
}
