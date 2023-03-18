using System.Xml.Serialization;

public class PetUpgradeXMLDefinitionBrawlerPassiveBuff : PetUpgradeXMLDefinitionBrawler
{
	[XmlElement(ElementName = "buff")]
	public string buff = string.Empty;
}
