using System.Xml.Serialization;

[XmlRoot(ElementName = "action")]
public class ItemAction
{
	protected float minDuration;

	protected float padTime;

	protected UserSequence userSequence;

	[XmlElement(ElementName = "min_duration")]
	public float MinDuration
	{
		get
		{
			return minDuration;
		}
		set
		{
			minDuration = value;
		}
	}

	[XmlElement(ElementName = "pad_time")]
	public float PadTime
	{
		get
		{
			return padTime;
		}
		set
		{
			padTime = value;
		}
	}

	[XmlElement(ElementName = "user_sequence")]
	public UserSequence UserSequence
	{
		get
		{
			return userSequence;
		}
		set
		{
			userSequence = value;
		}
	}
}
