using System.Xml.Serialization;

public class ChallengeInfoParameters
{
	[XmlElement(ElementName = "name")]
	public string key;

	[XmlElement(ElementName = "value")]
	public string value;
}
