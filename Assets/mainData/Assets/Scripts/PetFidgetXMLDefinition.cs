using System;
using System.Xml.Serialization;

[Serializable]
[XmlRoot(ElementName = "fidget")]
public class PetFidgetXMLDefinition
{
	[XmlElement(ElementName = "fidgetEmoteID")]
	public string fidgetEmoteID;

	[XmlElement(ElementName = "fidgetAnimName")]
	public string fidgetAnimName;

	[XmlElement(ElementName = "fidgetSoundName")]
	public string fidgetSoundName;

	[XmlElement(ElementName = "fidgetMassEmoteID")]
	public string fidgetMassEmoteID;

	[XmlElement(ElementName = "fidgetPlayerEmoteID")]
	public string fidgetPlayerEmoteID;
}
