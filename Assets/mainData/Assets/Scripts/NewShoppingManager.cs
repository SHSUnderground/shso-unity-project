using System;
using System.Collections.Generic;

public class NewShoppingManager
{
	public enum ShoppingCategory
	{
		Featured,
		Hero,
		AgentOnly,
		Badge,
		Craft,
		Mission,
		MysteryBox,
		Potion,
		Card,
		Sidekick,
		Title,
		Gear,
		Bundle,
		Medallion,
		Misc,
		INVALID
	}

	public class NewCatalogJson
	{
		public List<CatalogOwnableJson> items;

		public List<CatalogOwnableSaleJson> sales;
	}

	public class CatalogOwnableJson
	{
		public int catalog_ownable_id;

		public string category;

		public string desc;

		public string name;

		public int ownable_type_id;

		public int price;

		public string price_type;

		public int shard_price;

		public int quantity;

		public int subscriber_only;

		public int display_priority;

		public int visible;

		public int flags;

		public int bag_quantity;

		public string bag_currency_type;

		public CatalogOwnableJson clone()
		{
			CatalogOwnableJson catalogOwnableJson = new CatalogOwnableJson();
			catalogOwnableJson.catalog_ownable_id = catalog_ownable_id;
			catalogOwnableJson.category = category;
			catalogOwnableJson.desc = desc;
			catalogOwnableJson.name = name;
			catalogOwnableJson.ownable_type_id = ownable_type_id;
			catalogOwnableJson.price = price;
			catalogOwnableJson.price_type = price_type;
			catalogOwnableJson.shard_price = shard_price;
			catalogOwnableJson.quantity = quantity;
			catalogOwnableJson.subscriber_only = subscriber_only;
			catalogOwnableJson.display_priority = display_priority;
			catalogOwnableJson.visible = visible;
			catalogOwnableJson.flags = flags;
			return catalogOwnableJson;
		}
	}

	public class CatalogOwnableSaleJson
	{
		public int catalog_ownable_sale_id;

		public int catalog_ownable_id;

		public string description;

		public string end_date;

		public int flags;

		public string name;

		public int ownable_type_id;

		public int price;

		public string price_type;

		public int shard_price;

		public string start_date;

		public int display_priority;
	}

	public static Dictionary<int, CatalogOwnableJson> CatalogOwnableMap = new Dictionary<int, CatalogOwnableJson>();

	public static Dictionary<ShoppingCategory, List<CatalogItem>> categoryContents = new Dictionary<ShoppingCategory, List<CatalogItem>>();

	public static List<CatalogItem> allCatalogItems = new List<CatalogItem>();

	public NewShoppingManager(IEnumerable<NewCatalogJson> catalogList)
	{
		ShoppingCategory[] array = (ShoppingCategory[])Enum.GetValues(typeof(ShoppingCategory));
		foreach (ShoppingCategory key in array)
		{
			categoryContents.Add(key, new List<CatalogItem>());
		}
		foreach (NewCatalogJson catalog in catalogList)
		{
			Dictionary<int, List<CatalogOwnableSaleJson>> dictionary = new Dictionary<int, List<CatalogOwnableSaleJson>>();
			foreach (CatalogOwnableSaleJson sale in catalog.sales)
			{
				if (sale.catalog_ownable_id == 16787)
				{
					CspUtils.DebugLog("found sale for guardians bundle " + sale.flags);
				}
				if (!dictionary.ContainsKey(sale.catalog_ownable_id))
				{
					dictionary.Add(sale.catalog_ownable_id, new List<CatalogOwnableSaleJson>());
				}
				dictionary[sale.catalog_ownable_id].Add(sale);
			}
			foreach (CatalogOwnableJson item in catalog.items)
			{
				if (!CatalogOwnableMap.ContainsKey(item.ownable_type_id))
				{
					CatalogOwnableMap.Add(item.ownable_type_id, item);
				}
				ShoppingCategory shoppingCategory = ownableCategoryToShoppingCategory(item.category);
				if (shoppingCategory == ShoppingCategory.INVALID)
				{
					if (!(item.category == "skip"))
					{
						CspUtils.DebugLog("Unknown item category: " + item.category);
					}
				}
				else
				{
					CatalogItem catalogItem = null;
					List<CatalogOwnableSaleJson> value;
					if (dictionary.TryGetValue(item.catalog_ownable_id, out value))
					{
						foreach (CatalogOwnableSaleJson item2 in value)
						{
							if (item.subscriber_only == 0 && (item2.flags & 0x20) != 0)
							{
								CspUtils.DebugLog("CLONING SALE DATA " + item2.catalog_ownable_id + " " + item2.catalog_ownable_sale_id);
								CatalogOwnableJson catalogOwnableJson = item.clone();
								catalogOwnableJson.subscriber_only = 1;
								catalogOwnableJson.flags = item2.flags;
								catalogOwnableJson.price = item2.price;
								catalogOwnableJson.price_type = item2.price_type;
								catalogOwnableJson.shard_price = item2.shard_price;
								catalogOwnableJson.name = item2.name;
								if (item2.display_priority != 0)
								{
									catalogOwnableJson.display_priority = item2.display_priority;
								}
								catalogOwnableJson.display_priority += 100000;
								catalogOwnableJson.visible = 1;
								catalogItem = AddItemToCatalog(catalogOwnableJson);
								if (catalogItem != null)
								{
									catalogItem.catalogOwnableSaleID = item2.catalog_ownable_sale_id;
								}
							}
							else
							{
								item.flags = item2.flags;
								item.price = item2.price;
								item.price_type = item2.price_type;
								item.shard_price = item2.shard_price;
								item.name = item2.name;
								if (item2.display_priority != 0)
								{
									item.display_priority = item2.display_priority;
								}
								item.display_priority += 100000;
								item.visible = 1;
								catalogItem = AddItemToCatalog(item);
								if (catalogItem != null)
								{
									catalogItem.catalogOwnableSaleID = item2.catalog_ownable_sale_id;
								}
							}
						}
					}
					else
					{
						AddItemToCatalog(item);
					}
				}
			}
		}
		foreach (List<CatalogItem> value2 in categoryContents.Values)
		{
			value2.Sort(delegate(CatalogItem p1, CatalogItem p2)
			{
				return p2.displayPriority.CompareTo(p1.displayPriority);
			});
		}
	}

	public bool itemForSale(int ownableTypeID)
	{
		foreach (CatalogItem allCatalogItem in allCatalogItems)
		{
			if (allCatalogItem.ownableTypeID == ownableTypeID)
			{
				return true;
			}
		}
		return false;
	}

	public static ShoppingCategory ownableCategoryToShoppingCategory(string category)
	{
		return ownableCategoryToShoppingCategory(OwnableDefinition.getCategoryFromString(category));
	}

	public static ShoppingCategory ownableCategoryToShoppingCategory(OwnableDefinition.Category category)
	{
		switch (category)
		{
		case OwnableDefinition.Category.Badge:
			return ShoppingCategory.Badge;
		case OwnableDefinition.Category.Mission:
			return ShoppingCategory.Mission;
		case OwnableDefinition.Category.Card:
		case OwnableDefinition.Category.BoosterPack:
		case OwnableDefinition.Category.Quest:
			return ShoppingCategory.Card;
		case OwnableDefinition.Category.Potion:
			return ShoppingCategory.Potion;
		case OwnableDefinition.Category.Hero:
			return ShoppingCategory.Hero;
		case OwnableDefinition.Category.MysteryBox:
			return ShoppingCategory.MysteryBox;
		case OwnableDefinition.Category.Medallion:
			return ShoppingCategory.Medallion;
		case OwnableDefinition.Category.Bundle:
			return ShoppingCategory.Bundle;
		case OwnableDefinition.Category.Sidekick:
		case OwnableDefinition.Category.SidekickUpgrade:
			return ShoppingCategory.Sidekick;
		case OwnableDefinition.Category.Title:
			return ShoppingCategory.Title;
		case OwnableDefinition.Category.Craft:
			return ShoppingCategory.Craft;
		case OwnableDefinition.Category.Blueprint:
			return ShoppingCategory.Title;
		case OwnableDefinition.Category.HQItem:
		case OwnableDefinition.Category.HQRoom:
		case OwnableDefinition.Category.TicketPack:
		case OwnableDefinition.Category.MoneyBag:
		case OwnableDefinition.Category.Fractal:
			return ShoppingCategory.Misc;
		case OwnableDefinition.Category.Unknown:
			return ShoppingCategory.INVALID;
		default:
			throw new Exception("Unhandled OwnableDefinition.Category " + category + " in ownableCategoryToShoppingCategory()");
		}
	}

	private CatalogItem AddItemToCatalog(CatalogOwnableJson jsonItem)
	{
		ShoppingCategory key = ownableCategoryToShoppingCategory(jsonItem.category);
		CatalogItem catalogItem = new CatalogItem(jsonItem);
		if (jsonItem.visible == 1)
		{
			if (jsonItem.flags != 0)
			{
				categoryContents[ShoppingCategory.Featured].Add(catalogItem);
			}
			if (jsonItem.subscriber_only == 1)
			{
				categoryContents[ShoppingCategory.AgentOnly].Add(catalogItem);
			}
			else
			{
				categoryContents[key].Add(catalogItem);
			}
			allCatalogItems.Add(catalogItem);
			return catalogItem;
		}
		return null;
	}

	public static int findSimilarUnownedItem(string itemName, ShoppingCategory defaultCategory)
	{
		itemName = itemName.ToLowerInvariant();
		CatalogItem catalogItem = allCatalogItems.Find(delegate(CatalogItem a)
		{
			//return a.name.ToLowerInvariant().Contains(itemName) && a.numberOwned == 0 && a.shoppingCategory == defaultCategory;
			return false;   // CSP above line wasn't working with Kingpin Mayhem Mission, temporary workaround.
		});
		if (catalogItem != null)
		{
			return catalogItem.ownableTypeID;
		}
		CatalogItem catalogItem2 = getList(defaultCategory).Find(delegate(CatalogItem a)
		{
			return a.numberOwned == 0;
		});
		if (catalogItem2 != null)
		{
			return catalogItem2.ownableTypeID;
		}
		return -1;
	}

	public static List<CatalogItem> getList(ShoppingCategory category)
	{
		return categoryContents[category];
	}

	public static ShoppingCategory itemIDToCategory(int ownableTypeID)
	{
		if (CatalogOwnableMap.ContainsKey(ownableTypeID))
		{
			CatalogOwnableJson catalogOwnableJson = CatalogOwnableMap[ownableTypeID];
			if (catalogOwnableJson.visible == 0)
			{
				foreach (CatalogItem allCatalogItem in allCatalogItems)
				{
					if (allCatalogItem.ownableTypeID == ownableTypeID && allCatalogItem.visible)
					{
						CspUtils.DebugLog(" " + allCatalogItem.agentOnly + " " + allCatalogItem.flags + " " + allCatalogItem.jsonData.category);
						if (allCatalogItem.agentOnly)
						{
							return ShoppingCategory.AgentOnly;
						}
						if ((allCatalogItem.flags & 0x10) != 0)
						{
							return ShoppingCategory.Featured;
						}
						return ownableCategoryToShoppingCategory(allCatalogItem.jsonData.category);
					}
				}
				CspUtils.DebugLog("itemIDToCategory found visible 0");
				return ShoppingCategory.INVALID;
			}
			return ownableCategoryToShoppingCategory(catalogOwnableJson.category);
		}
		return ShoppingCategory.INVALID;
	}
}
