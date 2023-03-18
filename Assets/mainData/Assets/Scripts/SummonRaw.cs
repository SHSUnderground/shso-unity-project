using System.Xml.Serialization;

public class SummonRaw
{
	[XmlElement(ElementName = "name")]
	public string name;

	[XmlElement(ElementName = "duration")]
	public int duration;

	[XmlElement(ElementName = "power_attack")]
	public int powerAttack;
}
