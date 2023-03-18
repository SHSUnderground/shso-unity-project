using System.Xml.Serialization;

public class PetUpgradeXMLDefinitionStunPigeon : PetUpgradeXMLDefinition
{
	[XmlElement(ElementName = "radius")]
	public int radius = 7;
}
