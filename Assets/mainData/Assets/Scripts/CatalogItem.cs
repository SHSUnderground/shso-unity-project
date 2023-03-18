using System.Runtime.CompilerServices;
using UnityEngine;

public class CatalogItem
{
	public enum Flag
	{
		IsNew = 1,
		OnSale = 2,
		OneWeek = 4,
		OneDay = 8,
		Featured = 0x10,
		SubscriberOnly = 0x20,
		EarlyAccess = 0x40
	}

	public NewShoppingManager.CatalogOwnableJson jsonData;

	public int catalogOwnableSaleID = -1;

	[CompilerGenerated]
	private int _003Cflags_003Ek__BackingField;

	[CompilerGenerated]
	private int _003Cquantity_003Ek__BackingField;

	[CompilerGenerated]
	private string _003Cdescription_003Ek__BackingField;

	[CompilerGenerated]
	private string _003Cname_003Ek__BackingField;

	[CompilerGenerated]
	private NewShoppingManager.ShoppingCategory _003CshoppingCategory_003Ek__BackingField;

	[CompilerGenerated]
	private OwnableDefinition _003CownableDef_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CdisplayPriority_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CgoldPrice_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CshardPrice_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CagentOnly_003Ek__BackingField;

	public int flags
	{
		[CompilerGenerated]
		get
		{
			return _003Cflags_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003Cflags_003Ek__BackingField = value;
		}
	}

	public int quantity
	{
		[CompilerGenerated]
		get
		{
			return _003Cquantity_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003Cquantity_003Ek__BackingField = value;
		}
	}

	public string description
	{
		[CompilerGenerated]
		get
		{
			return _003Cdescription_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003Cdescription_003Ek__BackingField = value;
		}
	}

	public string name
	{
		[CompilerGenerated]
		get
		{
			return _003Cname_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003Cname_003Ek__BackingField = value;
		}
	}

	public NewShoppingManager.ShoppingCategory shoppingCategory
	{
		[CompilerGenerated]
		get
		{
			return _003CshoppingCategory_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CshoppingCategory_003Ek__BackingField = value;
		}
	}

	public OwnableDefinition ownableDef
	{
		[CompilerGenerated]
		get
		{
			return _003CownableDef_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CownableDef_003Ek__BackingField = value;
		}
	}

	public int ownableTypeID
	{
		get
		{
			return jsonData.ownable_type_id;
		}
	}

	public int catalogOwnableID
	{
		get
		{
			return jsonData.catalog_ownable_id;
		}
	}

	public int displayPriority
	{
		[CompilerGenerated]
		get
		{
			return _003CdisplayPriority_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CdisplayPriority_003Ek__BackingField = value;
		}
	}

	public int goldPrice
	{
		[CompilerGenerated]
		get
		{
			return _003CgoldPrice_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CgoldPrice_003Ek__BackingField = value;
		}
	}

	public int shardPrice
	{
		[CompilerGenerated]
		get
		{
			return _003CshardPrice_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CshardPrice_003Ek__BackingField = value;
		}
	}

	public bool agentOnly
	{
		[CompilerGenerated]
		get
		{
			return _003CagentOnly_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CagentOnly_003Ek__BackingField = value;
		}
	}

	public bool visible
	{
		get
		{
			return jsonData.visible == 1;
		}
	}

	public bool isValid
	{
		get
		{
			if (string.IsNullOrEmpty(name))
			{
				CspUtils.DebugLog("CatalogItem " + catalogOwnableID + " has a null name");
				return false;
			}
			if (name.Substring(0, 1) != "#")
			{
				CspUtils.DebugLog("CatalogItem " + catalogOwnableID + " has a non-stringID name: " + name);
				return false;
			}
			return true;
		}
	}

	public int numberOwned
	{
		get
		{
			return OwnableDefinition.numOwned(ownableTypeID, AppShell.Instance.Profile);
		}
	}

	public Vector2 iconSize
	{
		get
		{
			return ownableDef.shoppingIconSize;
		}
	}

	public string iconPath
	{
		get
		{
			return ownableDef.shoppingIcon;
		}
	}

	public CatalogItem(NewShoppingManager.CatalogOwnableJson json)
	{
		jsonData = json;
		ownableDef = OwnableDefinition.getDef(ownableTypeID);
		if (ownableDef == null) {  // if block added by CSP
			CspUtils.DebugLogError("ownableDef equals NULL for ownableTypeID=" + ownableTypeID);
			return;
		}
		flags = json.flags;
		quantity = json.quantity;
		description = json.desc;
		name = json.name;
		if (name.Length == 0 || name.Substring(0, 1) != "#")
		{
			name = ownableDef.shoppingName;
		}
		shoppingCategory = NewShoppingManager.ownableCategoryToShoppingCategory(ownableDef.category);
		displayPriority = json.display_priority;
		goldPrice = json.price;
		shardPrice = json.shard_price;
		agentOnly = (json.subscriber_only == 1);
	}

	public bool canPurchase(UserProfile profile)
	{
		return canPurchase(profile, 0, false);
	}

	public bool canPurchase(UserProfile profile, int sessionPurchaseCount, bool withShards)
	{
		if (profile == null)
		{
			return false;
		}
		if (OwnableDefinition.isUniqueAndOwned(ownableTypeID, profile))
		{
			return false;
		}
		int num = goldPrice * quantity;
		int num2 = shardPrice * quantity;
		Entitlements.ServerValueEntitlement serverValueEntitlement = Singleton<Entitlements>.instance.EntitlementsSet[Entitlements.EntitlementFlagEnum.SubscriptionType] as Entitlements.ServerValueEntitlement;
		if (serverValueEntitlement.Value == "12")
		{
			num = (int)((float)num * 0.9001f);
			num2 = (int)((float)num2 * 0.9001f);
		}
		if (withShards)
		{
			return profile.Shards >= num2;
		}
		return profile.Gold >= num;
	}
}
