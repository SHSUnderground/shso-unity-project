using System.Xml.Serialization;

public class PetUpgradeXMLDefinitionMove : PetUpgradeXMLDefinition
{
	[XmlElement(ElementName = "double")]
	public int doubleJump;

	[XmlElement(ElementName = "super")]
	public int superJump;

	[XmlElement(ElementName = "glide")]
	public int glide;
}
