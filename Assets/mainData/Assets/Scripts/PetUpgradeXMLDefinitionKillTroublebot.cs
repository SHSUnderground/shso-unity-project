using System.Xml.Serialization;

public class PetUpgradeXMLDefinitionKillTroublebot : PetUpgradeXMLDefinition
{
	[XmlElement(ElementName = "radius")]
	public int radius = 7;
}
