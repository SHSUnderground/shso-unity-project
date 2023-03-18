using System.Xml.Serialization;

public class PetUpgradeXMLDefinition
{
	[XmlElement(ElementName = "icon")]
	public string icon = string.Empty;

	[XmlElement(ElementName = "name")]
	public string name = string.Empty;

	[XmlElement(ElementName = "uses")]
	public int uses = SpecialAbility.PASSIVE_USES;

	[XmlElement(ElementName = "requiredOwnable")]
	public int requiredOwnable = -1;

	[XmlElement(ElementName = "displaySpace")]
	public string displaySpace = "social";
}
