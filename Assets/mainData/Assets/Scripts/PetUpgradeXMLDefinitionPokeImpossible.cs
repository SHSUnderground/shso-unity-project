using System.Xml.Serialization;

public class PetUpgradeXMLDefinitionPokeImpossible : PetUpgradeXMLDefinition
{
	[XmlElement(ElementName = "radius")]
	public int radius = 5;
}
