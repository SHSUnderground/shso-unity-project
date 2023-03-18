using System.Xml.Serialization;

public class PetUpgradeXMLDefinitionPickupStrength : PetUpgradeXMLDefinition
{
	[XmlElement(ElementName = "strength")]
	public float strength;
}
