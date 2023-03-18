using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable]
[XmlRoot(ElementName = "pet")]
public class PetXMLDefinition
{
	[XmlElement(ElementName = "ownableID")]
	public int ownableID;

	[XmlElement(ElementName = "readableName")]
	public string readableName;

	[XmlElement(ElementName = "displayName")]
	public string displayName;

	[XmlElement(ElementName = "displayDesc")]
	public string displayDesc;

	[XmlElement(ElementName = "bundlePath")]
	public string bundlePath;

	[XmlElement(ElementName = "characterDataPath")]
	public string characterDataPath;

	[XmlElement(ElementName = "scale")]
	public string scale;

	[XmlElement(ElementName = "idleTimeRange")]
	public string idleTimeRange;

	[XmlElement(ElementName = "fidgetEmoteID")]
	public int fidgetEmoteID;

	[XmlElement(ElementName = "fidgetAnimName")]
	public string fidgetAnimName;

	[XmlElement(ElementName = "flyer")]
	public int flyer;

	[XmlElement(ElementName = "shoppingIcon")]
	public string shoppingIcon;

	[XmlElement(ElementName = "inventoryIconBase")]
	public string inventoryIconBase;

	[XmlElement(ElementName = "customRunAnim")]
	public string customRunAnim;

	[XmlElement(ElementName = "customIdleAnim")]
	public string customIdleAnim;

	[XmlElement(ElementName = "pigeonDeathHack")]
	public int pigeonDeathHack;

	[XmlArray(ElementName = "fidgets")]
	[XmlArrayItem("fidget")]
	public List<PetFidgetXMLDefinition> fidgets;

	[XmlArray(ElementName = "hot_spots")]
	[XmlArrayItem("hot_spot")]
	public List<string> hotSpotTypes;

	[XmlArrayItem("pokeimpossible", typeof(PetUpgradeXMLDefinitionPokeImpossible))]
	[XmlArrayItem("cooldown", typeof(PetUpgradeXMLDefinitionCooldown))]
	[XmlArrayItem("brawler_buff", typeof(PetUpgradeXMLDefinitionBrawlerBuff))]
	[XmlArrayItem("brawler_passive_buff", typeof(PetUpgradeXMLDefinitionBrawlerPassiveBuff))]
	[XmlArrayItem("move", typeof(PetUpgradeXMLDefinitionMove))]
	[XmlArrayItem("hotspot", typeof(PetUpgradeXMLDefinitionHotspot))]
	[XmlArrayItem("grab", typeof(PetUpgradeXMLDefinitionGrab))]
	[XmlArrayItem("pokestar", typeof(PetUpgradeXMLDefinitionPokeStar))]
	[XmlArrayItem("stunpigeon", typeof(PetUpgradeXMLDefinitionStunPigeon))]
	[XmlArrayItem("scavengerspawn", typeof(PetUpgradeXMLDefinitionSpawnScavenger))]
	[XmlArrayItem("xpspawn", typeof(PetUpgradeXMLDefinitionSpawnXP))]
	[XmlArrayItem("silverspawn", typeof(PetUpgradeXMLDefinitionSpawnSilver))]
	[XmlArrayItem("ticketspawn", typeof(PetUpgradeXMLDefinitionSpawnTickets))]
	[XmlArrayItem("smartbomb", typeof(PetUpgradeXMLDefinitionSmartbomb))]
	[XmlArray(ElementName = "upgrades")]
	[XmlArrayItem("killtroublebot", typeof(PetUpgradeXMLDefinitionKillTroublebot))]
	[XmlArrayItem("pickupstrength", typeof(PetUpgradeXMLDefinitionPickupStrength))]
	[XmlArrayItem("fractalspawn", typeof(PetUpgradeXMLDefinitionSpawnFractals))]
	[XmlArrayItem("ally", typeof(PetUpgradeXMLDefinitionAlly))]
	public List<PetUpgradeXMLDefinition> upgrades;
}
