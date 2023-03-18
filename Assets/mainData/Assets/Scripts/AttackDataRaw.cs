using System.IO;
using System.Xml.Serialization;

[XmlType(AnonymousType = true)]
[XmlRoot(ElementName = "attack_data", Namespace = "", IsNullable = false)]
public class AttackDataRaw
{
	[XmlElement(ElementName = "attack")]
	public AttackRaw[] attacks;

	public static AttackDataRaw FromXML(string xml)
	{
		//Discarded unreachable code: IL_0015
		using (StringReader reader = new StringReader(xml))
		{
			return FromXML(reader);
		}
	}

	public static AttackDataRaw FromXML(MemoryStream stream)
	{
		//Discarded unreachable code: IL_001d
		stream.Position = 0L;
		using (StreamReader reader = new StreamReader(stream))
		{
			return FromXML(reader);
		}
	}

	public static AttackDataRaw FromXML(TextReader reader)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(AttackDataRaw));
		return xmlSerializer.Deserialize(reader) as AttackDataRaw;
	}

	public static AttackDataRaw FromXML(string xml, XmlSerializer serializer)
	{
		//Discarded unreachable code: IL_001b
		using (StringReader textReader = new StringReader(xml))
		{
			return serializer.Deserialize(textReader) as AttackDataRaw;
		}
	}
}
