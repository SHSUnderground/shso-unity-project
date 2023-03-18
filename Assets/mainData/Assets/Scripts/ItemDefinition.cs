using System;
using System.Collections.Generic;
using System.Xml.XPath;

public class ItemDefinition : StaticDataDefinition, IStaticDataDefinition
{
	public enum Materials
	{
		Unspecified,
		Adamantium,
		Ceramic,
		Neutronium,
		Plastic,
		Rubber,
		Steel,
		Stone,
		Wood
	}

	public enum Categories
	{
		Blocks,
		Consumables,
		Enhancements,
		Equipment,
		Gadgets,
		HeroesAndVillains,
		PersonalEffects,
		Scenery,
		Seating,
		Surfaces,
		Heroes,
		BuildingBlocks,
		OtherFurniture,
		Decorations,
		Food,
		Explosives,
		Paints,
		MyToys,
		HeroToys
	}

	public enum Rooms
	{
		None,
		Bridge,
		Lab,
		Rec,
		Dorm,
		Detention,
		Cafeteria,
		Training,
		Trophy,
		Gym
	}

	public const string ICON_PATH = "items_bundle|";

	private const char ASSET_BUNDLE_SEPERATOR = '|';

	private const int STRENGTH_DEFAULT = 0;

	private const int WEIGHT_DEFAULT = 0;

	private const int HUNGER_VALUE_DEFAULT = 0;

	protected string id;

	protected string name;

	protected string description;

	protected string iconAssetBundle;

	protected string icon;

	protected int maxStackSize;

	protected int duration;

	protected string placedObjectAssetBundle;

	protected string placedObjectPrefab;

	protected string heroSet;

	protected List<Categories> categoryList;

	protected Rooms roomAssociation;

	protected int coinCost = int.MaxValue;

	protected int tokenCost = int.MaxValue;

	protected int resaleTokens;

	protected string eventFlag;

	protected DateTime expirationDate;

	protected Dictionary<int, ItemKeyword> itemKeywords;

	protected bool placed;

	protected UseInfo useInfo;

	protected int strength;

	protected int weight;

	protected int hungerValue;

	protected Materials material;

	public string Id
	{
		get
		{
			return id;
		}
	}

	public string Name
	{
		get
		{
			return name;
		}
	}

	public string Description
	{
		get
		{
			return description;
		}
	}

	public string IconAssetBundle
	{
		get
		{
			return iconAssetBundle;
		}
	}

	public string Icon
	{
		get
		{
			return icon;
		}
	}

	public int MaxStackSize
	{
		get
		{
			return maxStackSize;
		}
	}

	public int Duration
	{
		get
		{
			return duration;
		}
	}

	public string PlacedObjectAssetBundle
	{
		get
		{
			return placedObjectAssetBundle;
		}
	}

	public string PlacedObjectPrefab
	{
		get
		{
			return placedObjectPrefab;
		}
	}

	public string HeroSet
	{
		get
		{
			return heroSet;
		}
	}

	public List<Categories> CategoryList
	{
		get
		{
			return categoryList;
		}
	}

	public Rooms RoomAssociation
	{
		get
		{
			return roomAssociation;
		}
	}

	public int CoinCost
	{
		get
		{
			return coinCost;
		}
	}

	public int TokenCost
	{
		get
		{
			return tokenCost;
		}
	}

	public int ResaleTokens
	{
		get
		{
			return resaleTokens;
		}
	}

	public string EventFlag
	{
		get
		{
			return eventFlag;
		}
	}

	public DateTime ExpirationDate
	{
		get
		{
			return expirationDate;
		}
	}

	public Dictionary<int, ItemKeyword> ItemKeywords
	{
		get
		{
			return itemKeywords;
		}
	}

	public UseInfo UseInfo
	{
		get
		{
			return useInfo;
		}
	}

	public int Strength
	{
		get
		{
			return strength;
		}
	}

	public int Weight
	{
		get
		{
			return weight;
		}
	}

	public int HungerValue
	{
		get
		{
			return hungerValue;
		}
	}

	public bool CanAIUse
	{
		get
		{
			if (hungerValue > 0)
			{
				return true;
			}
			if (useInfo == null)
			{
				return false;
			}
			if (useInfo.Uses == null || useInfo.Uses.Count == 0)
			{
				return false;
			}
			return true;
		}
	}

	public Materials Material
	{
		get
		{
			return material;
		}
	}

	public void InitializeFromData(DataWarehouse data)
	{
		XPathNavigator value = data.GetValue("name");
		value.MoveToParent();
		id = data.TryGetString("id", value.LocalName);
		name = AppShell.Instance.stringTable[data.GetString("name")];
		description = AppShell.Instance.stringTable[data.GetString("description")];
		string @string = data.GetString("icon");
		string[] array = @string.Split('|');
		if (array.Length > 1)
		{
			iconAssetBundle = array[0];
			icon = array[1];
		}
		else
		{
			icon = array[0];
		}
		maxStackSize = data.GetInt("max_stack");
		duration = data.GetInt("duration");
		@string = data.GetString("placed_object");
		array = @string.Split('|');
		if (array.Length > 1)
		{
			placedObjectAssetBundle = array[0];
			placedObjectPrefab = array[1];
		}
		else
		{
			placedObjectPrefab = array[0];
		}
		heroSet = data.TryGetString("hero_set", null);
		if (heroSet == "none")
		{
			heroSet = null;
		}
		string text = data.TryGetString("material", null);
		if (text != null)
		{
			try
			{
				material = (Materials)(int)Enum.Parse(typeof(Materials), text);
			}
			catch
			{
				CspUtils.DebugLog("Invalid material specified in xml:" + text);
			}
		}
		categoryList = new List<Categories>();
		DataWarehouse data2 = data.GetData("category_list");
		foreach (DataWarehouse item in data2.GetIterator("category"))
		{
			try
			{
				categoryList.Add((Categories)(int)Enum.Parse(typeof(Categories), item.GetString("text()")));
			}
			catch (ArgumentException ex)
			{
				CspUtils.DebugLog("Exception occurred while parsing the category of item <" + id + ", " + name + ">: <" + ex + ">.");
			}
		}
		@string = data.TryGetString("room_association", null);
		if (@string != null && @string != string.Empty)
		{
			try
			{
				roomAssociation = (Rooms)(int)Enum.Parse(typeof(Rooms), @string, true);
			}
			catch (ArgumentException ex2)
			{
				CspUtils.DebugLog("Exception occurred while parsing the room association of item <" + id + ", " + name + ">: <" + ex2 + ">.");
			}
		}
		coinCost = data.GetInt("cost/coins");
		tokenCost = data.GetInt("cost/tokens");
		resaleTokens = data.GetInt("cost/resale");
		eventFlag = data.TryGetString("restrictions/event_flag", null);
		@string = data.TryGetString("restrictions/expiration_date", null);
		if (@string != null && @string != string.Empty)
		{
			try
			{
				expirationDate = DateTime.Parse(@string);
			}
			catch (FormatException ex3)
			{
				CspUtils.DebugLog("Exception occurred while parsing the expiration date of item <" + id + ", " + name + ">: <" + ex3 + ">.");
			}
		}
		DataWarehouse dataWarehouse = data.TryGetData("use_info", null);
		if (dataWarehouse != null)
		{
			useInfo = new UseInfo(dataWarehouse);
		}
		strength = data.TryGetInt("strength", 0);
		weight = data.TryGetInt("weight", 0);
		hungerValue = data.TryGetInt("hunger_value", 0);
		DataWarehouse dataWarehouse2 = data.TryGetData("ai_keywords", null);
		itemKeywords = new Dictionary<int, ItemKeyword>();
		if (dataWarehouse2 != null)
		{
			foreach (DataWarehouse item2 in dataWarehouse2.GetIterator("ai_keyword"))
			{
				int @int = item2.GetInt("ai_keyword_id");
				int int2 = item2.GetInt("ai_keyword_strength");
				ItemKeyword value2 = new ItemKeyword(@int, int2);
				itemKeywords.Add(@int, value2);
			}
		}
	}
}
