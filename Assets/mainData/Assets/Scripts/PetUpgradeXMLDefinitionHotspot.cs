using System.Xml.Serialization;

public class PetUpgradeXMLDefinitionHotspot : PetUpgradeXMLDefinition
{
	[XmlElement(ElementName = "hotSpotType")]
	public string hotSpotType;
}
