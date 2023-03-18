using System.Xml.Serialization;

public class PetUpgradeXMLDefinitionSmartbomb : PetUpgradeXMLDefinition
{
	[XmlElement(ElementName = "char")]
	public string character;

	[XmlElement(ElementName = "attackName")]
	public string attackName;

	[XmlElement(ElementName = "deathAnimOverride")]
	public string deathAnimOverride;
}
