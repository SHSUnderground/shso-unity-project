using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

[Serializable]
[XmlRoot(ElementName = "challenge")]
public class ChallengeInfo
{
	[XmlElement(ElementName = "id")]
	public int ChallengeId;

	[XmlElement(ElementName = "bypassItem")]
	public int BypassItem;

	[XmlElement(ElementName = "classname")]
	public string ClassName;

	[XmlElement(ElementName = "name")]
	public string Name;

	[XmlElement(ElementName = "desc")]
	public string Description;

	[XmlElement(ElementName = "icon_path")]
	public string IconPath;

	[XmlElement(ElementName = "message_type")]
	public string MessageType = string.Empty;

	[XmlElement(ElementName = "authority")]
	public ChallengeValidationEnum Authority;

	[XmlElement(ElementName = "force_client_events")]
	public bool ForceClientEvents;

	[XmlArrayItem("parameter")]
	[XmlArray(ElementName = "parameters")]
	public List<ChallengeInfoParameters> Parameters;

	[XmlElement(ElementName = "reward")]
	public ChallengeReward Reward;

	[XmlElement(ElementName = "display")]
	[XmlAnyElement]
	public XmlElement DisplayNode;
}
