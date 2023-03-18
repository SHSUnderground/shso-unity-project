using System.Xml.Serialization;

public class PetUpgradeXMLDefinitionCooldown : PetUpgradeXMLDefinition
{
	[XmlElement(ElementName = "refresh")]
	public int refresh;

	[XmlElement(ElementName = "effect")]
	public string effect = string.Empty;
}
