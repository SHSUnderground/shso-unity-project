using System.Xml.Serialization;

public class SummonsRaw
{
	[XmlElement(ElementName = "summon")]
	public SummonRaw[] summonList;
}
