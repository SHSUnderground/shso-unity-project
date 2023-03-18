using System.Xml.Serialization;

[XmlRoot(ElementName = "user_sequence")]
public class UserSequence
{
	protected string sequenceName;

	protected ItemSequence itemSequence;

	[XmlElement(ElementName = "sequence_name")]
	public string SequenceName
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

	[XmlElement(ElementName = "item_sequence")]
	public ItemSequence ItemSequence
	{
		get
		{
			return itemSequence;
		}
		set
		{
			itemSequence = value;
		}
	}
}
