using System;
using System.Xml.Serialization;

[Serializable]
[XmlRoot("routing-info")]
public class VORoutingInfo
{
	public enum ComparisonResolution
	{
		Ignore,
		Queue,
		Replace
	}

	[XmlElement("priority")]
	public float priority;

	[XmlElement("important")]
	public bool important;

	[XmlElement("cuts-self")]
	public bool cutsSelf;

	[XmlElement("queues-self")]
	public bool queuesSelf;

	public void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		writer.Write(priority);
		writer.Write(important);
		writer.Write(cutsSelf);
		writer.Write(queuesSelf);
	}

	public void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		priority = reader.ReadFloat();
		important = reader.ReadBoolean();
		cutsSelf = reader.ReadBoolean();
		queuesSelf = reader.ReadBoolean();
	}

	public static ComparisonResolution Compare(VORoutingInfo older, VORoutingInfo newer, bool isSameAction)
	{
		if (isSameAction)
		{
			if (newer.cutsSelf)
			{
				return ComparisonResolution.Replace;
			}
			if (newer.queuesSelf)
			{
				return ComparisonResolution.Queue;
			}
		}
		else if (newer.important)
		{
			if ((older.important && newer.priority >= older.priority) || !older.important)
			{
				return ComparisonResolution.Replace;
			}
		}
		else if (!older.important && newer.priority >= older.priority)
		{
			return ComparisonResolution.Replace;
		}
		return ComparisonResolution.Ignore;
	}
}
