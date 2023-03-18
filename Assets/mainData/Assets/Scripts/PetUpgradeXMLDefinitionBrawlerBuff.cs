using System.Xml.Serialization;

public class PetUpgradeXMLDefinitionBrawlerBuff : PetUpgradeXMLDefinitionBrawler
{
	[XmlElement(ElementName = "buff")]
	public string buff = string.Empty;
}
