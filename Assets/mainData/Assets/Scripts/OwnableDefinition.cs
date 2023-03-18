using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml.XPath;
using UnityEngine;

public class OwnableDefinition
{
	public enum OwnableLocation
	{
		Store,
		Crafting,
		MysteryBox,
		Achievement,
		Mission,
		NotFound
	}

	public enum Category
	{
		HQItem,
		Mission,
		HQRoom,
		Card,
		BoosterPack,
		Hero,
		Quest,
		Potion,
		TicketPack,
		MoneyBag,
		MysteryBox,
		Badge,
		Fractal,
		Sidekick,
		Title,
		Medallion,
		Bundle,
		Craft,
		Blueprint,
		SidekickUpgrade,
		XP,
		Gear,
		Unknown,
		All
	}

	private static Dictionary<Category, string> _categoryToString;

	private static Dictionary<string, Category> _stringToCategory;

	private static Dictionary<string, OwnableDefinition> _scavengerZoneToActualOwnable;

	public static Dictionary<int, List<int>> HeroIDToBadgeID;

	public static Dictionary<int, int> BadgeIDToHeroID;

	public static Dictionary<string, int> HeroNameToHeroID;

	public static Dictionary<int, string> HeroIDToHeroName;

	public static List<OwnableDefinition> HeroesByName;

	public static List<OwnableDefinition> MissionsByName;

	public static Dictionary<string, int> MissionNameToMissionID;

	public static Dictionary<int, string> MissionIDToMissionName;

	public static List<string> MissionBossNames;

	public int ownableTypeID;

	public string name;

	public Category category;

	public int subscriberOnly;

	public string description;

	public string metadata;

	private string baseIconName;

	public int released;

	public string hints;

	protected Dictionary<string, Keyword> keywords = new Dictionary<string, Keyword>();

	protected List<Keyword> keywordList = new List<Keyword>();

	private static DataWarehouse _scavengerInfo;

	private static List<OwnableDefinition> _ownableList;

	private static Dictionary<Category, List<OwnableDefinition>> _ownablesByCategory;

	private static Dictionary<int, OwnableDefinition> _ownableDefs;

	private static Dictionary<string, HeroDefinition> _heroDefsByName;

	private static Dictionary<int, HeroDefinition> _heroDefsByID;

	private static Dictionary<string, OwnableDefinition> _missionDefsByName;

	public static List<OwnableDefinition> allOwnables
	{
		get
		{
			return _ownableList;
		}
	}

	public string shoppingIcon
	{
		get
		{
			switch (category)
			{
			case Category.Badge:
				if (name.EndsWith("_2"))
				{
					return "shopping_bundle|badge";
				}
				return "shopping_bundle|badge_silver";
			case Category.BoosterPack:
				return "shopping_bundle|L_shopping_booster_pack_" + name;
			case Category.Bundle:
				return "shopping_bundle|bundle_" + ownableTypeID;
			case Category.Card:
				return string.Empty;
			case Category.Craft:
				return iconBase;
			case Category.Fractal:
				return "goodieseffects_bundle|goody_effects_fractal_shopping";
			case Category.Hero:
				return "characters_bundle|expandedtooltip_render_" + name;
			case Category.HQItem:
				if (!AppShell.Instance.ItemDictionary.ContainsKey(string.Empty + ownableTypeID))
				{
					return null;
				}
				return "items_bundle|" + AppShell.Instance.ItemDictionary[string.Empty + ownableTypeID].Icon;
			case Category.HQRoom:
				return "hq_bundle|L_HQflyer_" + name;
			case Category.Medallion:
				return metadata;
			case Category.Mission:
				return "missionflyers_bundle|L_mission_flyer_" + name;
			case Category.MoneyBag:
				if (metadata != string.Empty)
				{
					return "goodieseffects_bundle|goody_effects_fractal_shopping";
				}
				return "items_bundle|moneybag_" + ownableTypeID;
			case Category.MysteryBox:
				return "shopping_bundle|" + name + "_shopping";
			case Category.Potion:
			{
				ExpendableDefinition value = null;
				if (AppShell.Instance.ExpendablesManager.ExpendableTypes.TryGetValue(string.Empty + ownableTypeID, out value))
				{
					return value.ShoppingIcon;
				}
				return string.Empty;
			}
			case Category.Quest:
				return "shopping_bundle|L_shopping_cardquest_easy_box";
			case Category.Sidekick:
			{
				PetData data = PetDataManager.getData(ownableTypeID);
				if (data == null)
				{
					return string.Empty;
				}
				return "shopping_bundle|" + data.shoppingIcon;
			}
			case Category.TicketPack:
				return "shopping_bundle|shopping_ticket_item";
			case Category.Title:
				return "shopping_bundle|titles_shopping";
			case Category.SidekickUpgrade:
				if (name.EndsWith("_2"))
				{
					return "shopping_bundle|badge";
				}
				return "shopping_bundle|badge_silver";
			case Category.XP:
				return "shopping_bundle|L_mysquad_heroinfo_xp_icon";
			default:
				return string.Empty;
			}
		}
	}

	public string shoppingName
	{
		get
		{
			switch (category)
			{
			case Category.Hero:
				return AppShell.Instance.CharacterDescriptionManager[name].CharacterName;
			case Category.MysteryBox:
				return "#MYSTERYBOX_" + name + "_NAME";
			case Category.Mission:
				return "#MIS_" + name + "_NAME";
			case Category.HQRoom:
				return "#HQROOM_" + name;
			case Category.BoosterPack:
				return "#CARDBOOSTER_" + name + "_NAME";
			default:
				return name;
			}
		}
	}

	public string shoppingDesc
	{
		get
		{
			switch (category)
			{
			case Category.Hero:
				return AppShell.Instance.CharacterDescriptionManager[name].LongDescription;
			case Category.MysteryBox:
				return "#MYSTERYBOX_" + name + "_NAME";
			case Category.Mission:
				return "#MIS_" + name + "_NAME";
			default:
				return name;
			}
		}
	}

	public Vector2 shoppingIconSize
	{
		get
		{
			switch (category)
			{
			case Category.Badge:
				return new Vector2(183f, 183f);
			case Category.BoosterPack:
				return new Vector2(260f, 336f);
			case Category.Bundle:
				return new Vector2(183f, 183f);
			case Category.Card:
				return default(Vector2);
			case Category.Craft:
				return new Vector2(90f, 95f);
			case Category.Fractal:
				return new Vector2(183f, 183f);
			case Category.Hero:
				return new Vector2(183f, 183f);
			case Category.HQItem:
				return new Vector2(128f, 128f);
			case Category.HQRoom:
				return new Vector2(128f, 128f);
			case Category.Medallion:
				return new Vector2(64f, 64f);
			case Category.Mission:
				return new Vector2(612f, 792f);
			case Category.MoneyBag:
				return new Vector2(181f, 184f);
			case Category.MysteryBox:
				return new Vector2(183f, 183f);
			case Category.Potion:
				return new Vector2(200f, 200f);
			case Category.Quest:
				return new Vector2(275f, 337f);
			case Category.Sidekick:
			case Category.SidekickUpgrade:
				return new Vector2(183f, 183f);
			case Category.TicketPack:
				return new Vector2(53f, 59f);
			case Category.Title:
				return new Vector2(183f, 183f);
			default:
				return default(Vector2);
			}
		}
	}

	public string iconBase
	{
		get
		{
			switch (category)
			{
			case Category.Craft:
				return "shopping_bundle|" + baseIconName;
			case Category.MysteryBox:
				return "goodieseffects_bundle|goody_effects_" + name;
			case Category.Potion:
			{
				ExpendableDefinition value = null;
				if (AppShell.Instance.ExpendablesManager.ExpendableTypes.TryGetValue(string.Empty + ownableTypeID, out value))
				{
					return value.InventoryIcon;
				}
				return string.Empty;
			}
			case Category.Sidekick:
			{
				PetData data = PetDataManager.getData(ownableTypeID);
				if (data == null)
				{
					return string.Empty;
				}
				return "shopping_bundle|" + data.inventoryIconBase;
			}
			case Category.SidekickUpgrade:
			{
				PetData data = PetDataManager.getData(Convert.ToInt32(metadata));
				if (data == null)
				{
					return string.Empty;
				}
				return "shopping_bundle|" + data.inventoryIconBase;
			}
			default:
				return shoppingIcon;
			}
		}
	}

	public string iconFullPath
	{
		get
		{
			string iconBase = this.iconBase;
			switch (category)
			{
			case Category.MysteryBox:
			case Category.Sidekick:
			case Category.SidekickUpgrade:
				return iconBase + "_normal";
			default:
				return iconBase;
			}
		}
	}

	public OwnableDefinition()
	{
	}

	public OwnableDefinition(OwnableJson data)
	{
		ownableTypeID = data.id;
		name = data.name;
		category = getCategoryFromString(data.cat);
		subscriberOnly = data.sub;
		metadata = data.meta;
		released = data.rel;
		hints = data.hints;
	}

	static OwnableDefinition()
	{
		_scavengerZoneToActualOwnable = new Dictionary<string, OwnableDefinition>();
		HeroIDToBadgeID = new Dictionary<int, List<int>>();
		BadgeIDToHeroID = new Dictionary<int, int>();
		HeroNameToHeroID = new Dictionary<string, int>();
		HeroIDToHeroName = new Dictionary<int, string>();
		HeroesByName = new List<OwnableDefinition>();
		MissionsByName = new List<OwnableDefinition>();
		MissionNameToMissionID = new Dictionary<string, int>();
		MissionIDToMissionName = new Dictionary<int, string>();
		MissionBossNames = new List<string>();
		_ownableList = new List<OwnableDefinition>();
		_ownablesByCategory = new Dictionary<Category, List<OwnableDefinition>>();
		_ownableDefs = new Dictionary<int, OwnableDefinition>();
		_heroDefsByName = new Dictionary<string, HeroDefinition>();
		_heroDefsByID = new Dictionary<int, HeroDefinition>();
		_missionDefsByName = new Dictionary<string, OwnableDefinition>();
		_categoryToString = new Dictionary<Category, string>();
		_categoryToString[Category.HQItem] = "i";
		_categoryToString[Category.BoosterPack] = "b";
		_categoryToString[Category.Card] = "c";
		_categoryToString[Category.Hero] = "h";
		_categoryToString[Category.HQRoom] = "hq";
		_categoryToString[Category.Mission] = "m";
		_categoryToString[Category.Quest] = "quest";
		_categoryToString[Category.Potion] = "potion";
		_categoryToString[Category.MoneyBag] = "moneybag";
		_categoryToString[Category.TicketPack] = "t";
		_categoryToString[Category.MysteryBox] = "mystery";
		_categoryToString[Category.Badge] = "badge";
		_categoryToString[Category.Fractal] = "fractal";
		_categoryToString[Category.Sidekick] = "sidekick";
		_categoryToString[Category.Medallion] = "mdln";
		_categoryToString[Category.Title] = "title";
		_categoryToString[Category.Bundle] = "bundle";
		_categoryToString[Category.Craft] = "craft";
		_categoryToString[Category.SidekickUpgrade] = "sideup";
		_categoryToString[Category.Gear] = "gear";
		_categoryToString[Category.All] = string.Empty;
		_categoryToString[Category.Unknown] = "unk";
		_stringToCategory = new Dictionary<string, Category>();
		_stringToCategory["i"] = Category.HQItem;
		_stringToCategory["p"] = Category.Potion;
		_stringToCategory["b"] = Category.BoosterPack;
		_stringToCategory["c"] = Category.Card;
		_stringToCategory["card"] = Category.Card;
		_stringToCategory["h"] = Category.Hero;
		_stringToCategory["r"] = Category.HQRoom;
		_stringToCategory["hq"] = Category.HQRoom;
		_stringToCategory["m"] = Category.Mission;
		_stringToCategory["quest"] = Category.Quest;
		_stringToCategory["potion"] = Category.Potion;
		_stringToCategory["t"] = Category.TicketPack;
		_stringToCategory["moneybag"] = Category.MoneyBag;
		_stringToCategory["mystery"] = Category.MysteryBox;
		_stringToCategory["badge"] = Category.Badge;
		_stringToCategory["fractal"] = Category.Fractal;
		_stringToCategory["sidekick"] = Category.Sidekick;
		_stringToCategory["mdln"] = Category.Medallion;
		_stringToCategory["title"] = Category.Title;
		_stringToCategory["bundle"] = Category.Bundle;
		_stringToCategory["craft"] = Category.Craft;
		_stringToCategory["sideup"] = Category.SidekickUpgrade;
		_stringToCategory["gear"] = Category.Gear;
		_stringToCategory[string.Empty] = Category.All;
		_stringToCategory["unk"] = Category.Unknown;
	}

	public static string simpleZoneName(string input)
	{
		input = input.ToLower();
		if (input.Contains("bugle"))
		{
			return "bugle";
		}
		if (input.Contains("baxter"))
		{
			return "baxter";
		}
		if (input.Contains("asgard"))
		{
			return "asgard";
		}
		if (input.Contains("villain"))
		{
			return "villain";
		}
		return "bugle";
	}

	public static bool isUniqueCategory(Category cat)
	{
		switch (cat)
		{
		case Category.Mission:
		case Category.HQRoom:
		case Category.Hero:
		case Category.Quest:
		case Category.Badge:
		case Category.Sidekick:
		case Category.Title:
		case Category.Medallion:
		case Category.Bundle:
		case Category.Blueprint:
		case Category.SidekickUpgrade:
			return true;
		default:
			return false;
		}
	}

	public static bool isUniqueItem(int ownableTypeID)
	{
		OwnableDefinition def = getDef(ownableTypeID);
		if (def == null)
		{
			CspUtils.DebugLog("isUniqueItem got null OwnableDef for " + ownableTypeID);
			return false;
		}
		return isUniqueCategory(def.category);
	}

	public static string getStringFromCategory(Category category)
	{
		return _categoryToString[category];
	}

	public static Category getCategoryFromString(string str)
	{
		if (_stringToCategory.ContainsKey(str))
		{
			return _stringToCategory[str];
		}
		return Category.Unknown;
	}

	public static int numOwned(int ownableTypeID, UserProfile profile)
	{
		OwnableDefinition def = getDef(ownableTypeID);
		if (def == null)
		{
			return 0;
		}
		string key = string.Empty + ownableTypeID;
		switch (def.category)
		{
		case Category.MoneyBag:
			return 0;
		case Category.Hero:
			if (profile.AvailableCostumes.ContainsKey(def.name))
			{
				return 1;
			}
			break;
		case Category.HQRoom:
			if (profile.AvailableHQRooms.ContainsKey(key))
			{
				return 1;
			}
			break;
		case Category.Mission:
			if (profile.AvailableMissions.ContainsKey(key))
			{
				return 1;
			}
			break;
		case Category.Quest:
			if (profile.AvailableQuests.ContainsKey(key))
			{
				return 1;
			}
			break;
		case Category.Badge:
			if (profile.Badges.ContainsKey(key))
			{
				return 1;
			}
			break;
		case Category.Sidekick:
			if (PetDataManager.ownsPet(ownableTypeID))
			{
				return 1;
			}
			break;
		case Category.Medallion:
			if (profile.Medallions.ContainsKey(key))
			{
				return 1;
			}
			break;
		case Category.Title:
			if (TitleManager.ownsTitle(ownableTypeID))
			{
				return 1;
			}
			break;
		case Category.Bundle:
			if (profile.Bundles.ContainsKey(key))
			{
				return 1;
			}
			break;
		case Category.SidekickUpgrade:
			if (profile.SidekickUpgrades.ContainsKey(key))
			{
				return 1;
			}
			break;
		case Category.Craft:
			if (profile.Crafts.ContainsKey(key))
			{
				return profile.Crafts[key].Quantity;
			}
			break;
		default:
			CspUtils.DebugLog("OwnableDefinition - numOwned() received a category it did not handle " + def.category);
			break;
		}
		return 0;
	}

	public static bool isOwned(int ownableTypeID, UserProfile profile)
	{
		return numOwned(ownableTypeID, profile) >= 1;
	}

	public static bool isUniqueAndOwned(int ownableTypeID, UserProfile profile)
	{
		OwnableDefinition def = getDef(ownableTypeID);
		if (def == null)
		{
			CspUtils.DebugLog("ERROR:  failed to retrieve OwnableDefinition for ID " + ownableTypeID);
			return false;
		}
		if (!isUniqueCategory(def.category))
		{
			return false;
		}
		return isOwned(ownableTypeID, profile);
	}

	public void addKeyword(Keyword keyword)
	{
		keywords.Add(keyword.keyword, keyword);
		keywordList.Add(keyword);
		keywordList.Sort(delegate(Keyword p1, Keyword p2)
		{
			return p1.keyword.CompareTo(p2.keyword);
		});
	}

	public List<Keyword> getKeywords()
	{
		return keywordList;
	}

	public bool hasKeyword(string keywordStr)
	{
		Keyword keyword = Keyword.keywordsByName[keywordStr];
		return hasKeyword(keyword);
	}

	public bool hasKeyword(Keyword keyword)
	{
		return keywords.ContainsKey(keyword.keyword);
	}

	public static void loadScavengerInfo(DataWarehouse xml)
	{
		_scavengerInfo = xml;
	}

	public static List<OwnableDefinition> getOwnablesByCategory(Category category)
	{
		return _ownablesByCategory[category];
	}

	private static void OnCharacterDataLoaded(GameDataLoadResponse response, object extraData)
	{
		HeroDefinition heroDefinition = new HeroDefinition(response.Data);
		heroDefinition.ownableTypeID = HeroNameToHeroID[heroDefinition.name];
		heroDefinition.ownableDef = getDef(heroDefinition.ownableTypeID);
		_heroDefsByName.Add(heroDefinition.name, heroDefinition);
		_heroDefsByID.Add(heroDefinition.ownableTypeID, heroDefinition);
	}

	public static void load(IEnumerable<OwnableJson> ownableList, IEnumerable<KeywordJson> keywordJsonList, IEnumerable<OwnableKeywordJson> ownableKeywordList)
	{
		foreach (int value in Enum.GetValues(typeof(Category)))
		{
			_ownablesByCategory.Add((Category)value, new List<OwnableDefinition>());
		}
		OwnableDefinition ownableDefinition;
		foreach (OwnableJson ownable in ownableList)
		{
			ownableDefinition = new OwnableDefinition(ownable);
			_ownableDefs.Add(ownableDefinition.ownableTypeID, ownableDefinition);
			_ownableList.Add(ownableDefinition);
			_ownablesByCategory[ownableDefinition.category].Add(ownableDefinition);
			switch (ownableDefinition.category)
			{
			case Category.Badge:
			{
				if (ownableDefinition.metadata == null || ownableDefinition.metadata == string.Empty)
				{
					CspUtils.DebugLog("ERROR:  bad badge definition (mission metadata) for ownable ID " + ownableDefinition.ownableTypeID);
				}
				BadgeIDToHeroID.Add(ownableDefinition.ownableTypeID, int.Parse(ownableDefinition.metadata));
				if (!HeroIDToBadgeID.ContainsKey(int.Parse(ownableDefinition.metadata)))
				{
					HeroIDToBadgeID.Add(int.Parse(ownableDefinition.metadata), new List<int>());
					HeroIDToBadgeID[int.Parse(ownableDefinition.metadata)].Add(-1);
					HeroIDToBadgeID[int.Parse(ownableDefinition.metadata)].Add(-1);
				}
				int index = 0;
				if (ownableDefinition.name.Substring(ownableDefinition.name.Length - 2) == "_2")
				{
					index = 1;
				}
				HeroIDToBadgeID[int.Parse(ownableDefinition.metadata)][index] = ownableDefinition.ownableTypeID;
				break;
			}
			case Category.Hero:
				HeroNameToHeroID.Add(ownableDefinition.name, ownableDefinition.ownableTypeID);
				HeroIDToHeroName.Add(ownableDefinition.ownableTypeID, ownableDefinition.name);
				HeroesByName.Add(ownableDefinition);
				AppShell.Instance.DataManager.LoadGameData("Characters/" + ownableDefinition.name, OnCharacterDataLoaded, ownableDefinition.name);
				break;
			case Category.Title:
				TitleManager.addTitle(ownableDefinition.ownableTypeID, ownableDefinition.name);
				break;
			case Category.Medallion:
				if (ownableDefinition.metadata == null || ownableDefinition.metadata == string.Empty)
				{
					CspUtils.DebugLog("ERROR:  bad Medallion definition (missing metadata) for ownable ID " + ownableDefinition.ownableTypeID);
				}
				else
				{
					TitleManager.addMedallion(ownableDefinition.ownableTypeID, ownableDefinition.name, ownableDefinition.metadata);
				}
				break;
			case Category.Mission:
				if (ownableDefinition.metadata != string.Empty && ownableDefinition.released == 1 && !MissionBossNames.Contains(ownableDefinition.metadata))
				{
					MissionBossNames.Add(ownableDefinition.metadata);
				}
				MissionNameToMissionID.Add(ownableDefinition.name, ownableDefinition.ownableTypeID);
				MissionIDToMissionName.Add(ownableDefinition.ownableTypeID, ownableDefinition.name);
				_missionDefsByName.Add(ownableDefinition.name, ownableDefinition);
				if (ownableDefinition.released == 1)
				{
					MissionsByName.Add(ownableDefinition);
				}
				break;
			}
		}
		HeroesByName.Sort(delegate(OwnableDefinition p1, OwnableDefinition p2)
		{
			string characterFamily = AppShell.Instance.CharacterDescriptionManager[p1.name].CharacterFamily;
			string characterFamily2 = AppShell.Instance.CharacterDescriptionManager[p2.name].CharacterFamily;
			return (characterFamily != characterFamily2) ? characterFamily.CompareTo(characterFamily2) : AppShell.Instance.CharacterDescriptionManager[p1.name].CharacterName.CompareTo(AppShell.Instance.CharacterDescriptionManager[p2.name].CharacterName);
		});
		MissionsByName.Sort(delegate(OwnableDefinition p1, OwnableDefinition p2)
		{
			return AppShell.Instance.stringTable.GetString(p1.shoppingName).CompareTo(AppShell.Instance.stringTable.GetString(p2.shoppingName));
		});
		MissionBossNames.Sort(delegate(string p1, string p2)
		{
			return p1.CompareTo(p2);
		});
		foreach (KeywordJson keywordJson in keywordJsonList)
		{
			Keyword keyword = new Keyword(keywordJson);
		}
		foreach (OwnableKeywordJson ownableKeyword in ownableKeywordList)
		{
			if (!Keyword.keywordsByName.ContainsKey(ownableKeyword.keyword))
			{
				CspUtils.DebugLog("ERROR: ownable has a keyword that isn't in the keyword list.  Ownable Type is " + ownableKeyword.ownable_type_id + ", keyword is " + ownableKeyword.keyword);
			}
			else
			{
				ownableDefinition = getDef(ownableKeyword.ownable_type_id);
				if (ownableDefinition == null)
				{
					CspUtils.DebugLog("ERROR: ownable " + ownableKeyword.ownable_type_id + " lacks definition.");
				}
				else
				{
					Keyword keyword = Keyword.keywordsByName[ownableKeyword.keyword];
					ownableDefinition.addKeyword(keyword);
				}
			}
		}
		ownableDefinition = new OwnableDefinition();
		ownableDefinition.category = Category.XP;
		ownableDefinition.name = "XP";
		ownableDefinition.ownableTypeID = -4;
		_ownableDefs.Add(ownableDefinition.ownableTypeID, ownableDefinition);
		_ownableList.Add(ownableDefinition);
		_ownablesByCategory[ownableDefinition.category].Add(ownableDefinition);
		CspUtils.DebugLog("OwnableDefinitions loaded");
	}

	public static void convertScavengerInfo()
	{
		DataWarehouse scavengerInfo = _scavengerInfo;
		XPathNavigator value = scavengerInfo.GetValue("scavenger_data");
		XPathNodeIterator xPathNodeIterator = value.SelectChildren("item_info", string.Empty);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(ScavengerXMLDefinition));
		ScavengerXMLDefinition scavengerXMLDefinition;
		while (true)
		{
			if (!xPathNodeIterator.MoveNext())
			{
				return;
			}
			string outerXml = xPathNodeIterator.Current.OuterXml;
			scavengerXMLDefinition = (xmlSerializer.Deserialize(new StringReader(outerXml)) as ScavengerXMLDefinition);
			if (scavengerXMLDefinition != null)
			{
				OwnableDefinition def = getDef(scavengerXMLDefinition.ownableTypeID);
				if (def == null)
				{
					break;
				}
				def.baseIconName = scavengerXMLDefinition.icon;
				if (scavengerXMLDefinition.zoneInfo.Length > 0)
				{
					_scavengerZoneToActualOwnable[scavengerXMLDefinition.zoneInfo] = def;
				}
			}
		}
		CspUtils.DebugLog("Error in loadScavengerInfo: target ownableTypeID was not found in the ownables list: " + scavengerXMLDefinition.ownableTypeID + " - from " + scavengerXMLDefinition.readableName);
	}

	public static OwnableDefinition getOwnableForScavengerZone(string zoneName, int generalOwnableID)
	{
		if (_scavengerZoneToActualOwnable.ContainsKey(zoneName + "," + generalOwnableID))
		{
			return _scavengerZoneToActualOwnable[zoneName + "," + generalOwnableID];
		}
		if (_scavengerZoneToActualOwnable.ContainsKey("all," + generalOwnableID))
		{
			return _scavengerZoneToActualOwnable["all," + generalOwnableID];
		}
		CspUtils.DebugLog("ERROR:  could not find actual ownable type for scavenger general type " + generalOwnableID + " in zone " + zoneName);
		return null;
	}

	public static HeroDefinition getHeroDef(int id)
	{
		if (_heroDefsByID.ContainsKey(id))
		{
			return _heroDefsByID[id];
		}
		return null;
	}

	public static HeroDefinition getHeroDef(string xmlName)
	{
		if (_heroDefsByName.ContainsKey(xmlName))
		{
			return _heroDefsByName[xmlName];
		}
		return null;
	}

	public static Dictionary<string, OwnableDefinition> getMissionDefs()
	{
		return _missionDefsByName;
	}

	public static OwnableDefinition getMissionDef(string missionName)
	{
		if (_missionDefsByName.ContainsKey(missionName))
		{
			return _missionDefsByName[missionName];
		}
		return null;
	}

	public static OwnableDefinition getDef(int id)
	{
		OwnableDefinition value;
		if (_ownableDefs.TryGetValue(id, out value))
		{
			return value;
		}
		return null;
	}

	public GUISimpleControlWindow getIcon(Vector2 desiredSize, bool includeTooltip = false)
	{
		Vector2 size = ShoppingWindow.constrain(this.shoppingIconSize, desiredSize);
		float x = size.x;
		Vector2 shoppingIconSize = this.shoppingIconSize;
		float num = x / shoppingIconSize.x;
		GUISimpleControlWindow gUISimpleControlWindow = new GUISimpleControlWindow();
		gUISimpleControlWindow.SetSize(size);
		gUISimpleControlWindow.IsVisible = true;
		GUIDrawTexture gUIDrawTexture;
		if (includeTooltip)
		{
			gUIDrawTexture = new GUIImageWithEvents();
			gUIDrawTexture.ToolTip = new GUIControl.NamedToolTipInfo(shoppingName);
		}
		else
		{
			gUIDrawTexture = new GUIImage();
		}
		gUIDrawTexture.SetSize(size, GUIControl.AutoSizeTypeEnum.Absolute, GUIControl.AutoSizeTypeEnum.Absolute);
		gUIDrawTexture.Position = Vector2.zero;
		gUIDrawTexture.TextureSource = shoppingIcon;
		gUIDrawTexture.IsVisible = true;
		gUISimpleControlWindow.Add(gUIDrawTexture);
		if (category == Category.Quest)
		{
			CardQuestPart questPart = AppShell.Instance.CardQuestManager.GetQuestPart(ownableTypeID);
			if (questPart != null)
			{
				GUIImage gUIImage = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(200f, 218f) * num, new Vector2(91f * num, 88f * num));
				gUIImage.TextureSource = "characters_bundle|" + questPart.Sponsor + "_HUD_default";
				gUIImage.IsVisible = true;
				gUISimpleControlWindow.Add(gUIImage);
			}
			else
			{
				CspUtils.DebugLog("Quest Part:" + ownableTypeID + " is not listed in the card quest collection.");
			}
		}
		else if (category == Category.Badge)
		{
			num += 0.15f;
			OwnableDefinition def = getDef(int.Parse(metadata));
			GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(103f, 103f) * num, new Vector2(0f, 0f));
			gUIImage2.TextureSource = "characters_bundle|token_" + def.name + string.Empty;
			Vector2 size2 = gUIDrawTexture.Size;
			float num2 = size2.x / 2f;
			Vector2 size3 = gUIImage2.Size;
			float x2 = num2 - size3.x / 2f;
			Vector2 size4 = gUIDrawTexture.Size;
			float num3 = size4.y / 2f;
			Vector2 size5 = gUIImage2.Size;
			gUIImage2.SetPosition(GUIControl.DockingAlignmentEnum.TopLeft, GUIControl.AnchorAlignmentEnum.TopLeft, GUIControl.OffsetType.Absolute, new Vector2(x2, num3 - size5.y / 2f));
			gUIImage2.IsVisible = true;
			gUISimpleControlWindow.Add(gUIImage2);
		}
		else if (category == Category.SidekickUpgrade)
		{
			num += 0.15f;
			OwnableDefinition def2 = getDef(int.Parse(metadata));
			GUIImage gUIImage3 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(103f, 103f) * num, new Vector2(0f, 0f));
			gUIImage3.TextureSource = "characters_bundle|token_" + def2.name + string.Empty;
			Vector2 size6 = gUIDrawTexture.Size;
			float num4 = size6.x / 2f;
			Vector2 size7 = gUIImage3.Size;
			float x3 = num4 - size7.x / 2f;
			Vector2 size8 = gUIDrawTexture.Size;
			float num5 = size8.y / 2f;
			Vector2 size9 = gUIImage3.Size;
			gUIImage3.SetPosition(GUIControl.DockingAlignmentEnum.TopLeft, GUIControl.AnchorAlignmentEnum.TopLeft, GUIControl.OffsetType.Absolute, new Vector2(x3, num5 - size9.y / 2f));
			gUIImage3.IsVisible = true;
			gUISimpleControlWindow.Add(gUIImage3);
		}
		return gUISimpleControlWindow;
	}

	public static void goToOwnableLocation(int ownableID)
	{
		OwnableLocation location = getLocation(ownableID);
		OwnableDefinition def = getDef(ownableID);
		switch (location)
		{
		case OwnableLocation.Store:
			new ShoppingWindow(ownableID).launch();
			break;
		case OwnableLocation.Crafting:
			CraftingWindow.requestCraftingWindow(ownableID);
			break;
		case OwnableLocation.MysteryBox:
			if (def.hints.Length > 0)
			{
				List<int> list = new List<int>();
				string[] array = def.hints.Split(',');
				string[] array2 = array;
				foreach (string s in array2)
				{
					list.Add(int.Parse(s));
				}
				new ShoppingWindow(NewShoppingManager.ShoppingCategory.MysteryBox, list).launch();
			}
			else
			{
				new ShoppingWindow(NewShoppingManager.ShoppingCategory.MysteryBox).launch();
			}
			break;
		default:
			CspUtils.DebugLog("goToOwnableLocation couldn't find a location for ownable " + ownableID + ".  Proabably out of catalog? opening store to that ownable's category");
			new ShoppingWindow(NewShoppingManager.ownableCategoryToShoppingCategory(def.category)).launch();
			break;
		}
	}

	public static OwnableLocation getLocation(int ownableID)
	{
		NewShoppingManager.ShoppingCategory shoppingCategory = NewShoppingManager.itemIDToCategory(ownableID);
		if (shoppingCategory != NewShoppingManager.ShoppingCategory.INVALID)
		{
			return OwnableLocation.Store;
		}
		OwnableDefinition def = getDef(ownableID);
		if (def == null)
		{
			CspUtils.DebugLog("ownable definition not found, returning OwnableLocation.NotFound");
			return OwnableLocation.NotFound;
		}
		if (def.hasKeyword(Keyword.keywordsByName["mysterybox_only"]))
		{
			return OwnableLocation.MysteryBox;
		}
		if (def.hasKeyword(Keyword.keywordsByName["crafted_only"]))
		{
			return OwnableLocation.Crafting;
		}
		if (def.hasKeyword(Keyword.keywordsByName["achievement_only"]))
		{
			CspUtils.DebugLog("ACHIEVEMENT ONLY ITEM NEED TO LAUNCH ACHIEVEMENT WINDOW");
			return OwnableLocation.Achievement;
		}
		if (def.hasKeyword(Keyword.keywordsByName["mission_only"]))
		{
			CspUtils.DebugLog("MISSION ONLY ITEM NEED TO LAUNCH SHOP WINDOW");
			return OwnableLocation.Mission;
		}
		CspUtils.DebugLog("findOwnableLocation couldn't find the ownable " + ownableID + ".  Proabably out of catalog?");
		return OwnableLocation.NotFound;
	}
}
