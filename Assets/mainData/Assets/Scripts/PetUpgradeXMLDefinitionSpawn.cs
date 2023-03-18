using System.Xml.Serialization;

public class PetUpgradeXMLDefinitionSpawn : PetUpgradeXMLDefinition
{
	[XmlElement(ElementName = "objectType")]
	public string objectType = string.Empty;

	[XmlElement(ElementName = "refresh")]
	public int refresh;

	[XmlElement(ElementName = "amount")]
	public int amount;
}
