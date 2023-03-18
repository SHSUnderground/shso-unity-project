using System.Xml.Serialization;

public class PetUpgradeXMLDefinitionPokeStar : PetUpgradeXMLDefinition
{
	[XmlElement(ElementName = "radius")]
	public int radius = 7;
}
