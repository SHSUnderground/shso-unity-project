using System.Xml.Serialization;

public class ChallengeReward
{
	[XmlElement(ElementName = "type")]
	public ChallengeRewardType rewardType;

	[XmlElement(ElementName = "value")]
	public string value;

	[XmlElement(ElementName = "grantmode")]
	public ChallengeGrantMode grantMode;

	[XmlElement(ElementName = "qualifier")]
	public string qualifier;
}
