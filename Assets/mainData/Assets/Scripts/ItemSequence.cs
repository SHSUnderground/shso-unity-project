using System.Xml.Serialization;

[XmlRoot(ElementName = "item_sequence")]
public class ItemSequence
{
	protected string sequenceName;

	protected string eventName;

	[XmlElement("name")]
	public string Name
	{
		get
		{
			return sequenceName;
		}
		set
		{
			sequenceName = value;
		}
	}

	[XmlElement("event")]
	public string EventName
	{
		get
		{
			return eventName;
		}
		set
		{
			eventName = value;
		}
	}
}
