using System.Xml.Serialization;

public class ExpendHandlerParameters
{
	[XmlElement(ElementName = "name")]
	public string Key;

	[XmlElement(ElementName = "value")]
	public string[] Value;
}
