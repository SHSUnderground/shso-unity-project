using System.Xml.Serialization;

public class PetUpgradeXMLDefinitionAlly : PetUpgradeXMLDefinition
{
	[XmlElement(ElementName = "char")]
	public string character;

	[XmlElement(ElementName = "r2")]
	public int r2;

	[XmlElement(ElementName = "duration")]
	public int duration;

	[XmlElement(ElementName = "numberToSpawn")]
	public int numberToSpawn = 1;

	[XmlElement(ElementName = "deathAnimOverride")]
	public string deathAnimOverride = string.Empty;
}
