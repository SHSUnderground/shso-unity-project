using System.Xml.Serialization;

public class PetUpgradeXMLDefinitionGrab : PetUpgradeXMLDefinition
{
	[XmlElement(ElementName = "activity_name")]
	public string activityName;

	[XmlElement(ElementName = "radius")]
	public int radius;
}
