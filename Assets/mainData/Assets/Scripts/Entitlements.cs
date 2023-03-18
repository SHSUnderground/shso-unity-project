using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.XPath;
using UnityEngine;

public class Entitlements : Singleton<Entitlements>
{
	public enum EntitlementFlagEnum
	{
		ShieldPlayAllow,
		ShieldHeroesAllow,
		ShieldHQAllow,
		ShieldPrizeWheelAllow,
		IsPayingCustomer,
		IsPayingSubscriber,
		CardGameAllow,
		OpenChatAllow,
		ArcadeAllow,
		ClientConsoleAllow,
		UnityEditorAllow,
		DebuggingAllow,
		WipMissionsAllow,
		DemoLimitsOn,
		ParentalHqDeny,
		ParentalMissionsDeny,
		ParentalCardGameDeny,
		ParentalFriendingDeny,
		ParentalBlockingDeny,
		ParentalOpenChatDeny,
		ParentalShoppingDeny,
		SubscriptionPagesAllow,
		TutorialVOAllow,
		FlavorVOAllow,
		UseLocalizedTutorialVO,
		UseLocalizedFlavorVO,
		PlayerCountry,
		PlayerLanguage,
		ShoppingCatalog,
		UseExternalShopping,
		SubscriptionType,
		Expiration,
		CatalogCountry,
		DailyRewardStr,
		MaxChallengeLevel,
		MissionsPermitSet,
		GameWorldPermitSet,
		CardGamePermitSet,
		OpenChatPermitSet,
		MySquadPermit,
		FriendsListPermit,
		ShopWindowPermit,
		SettingsWindowPermit,
		PrizeWheelPermit,
		InventoryDragDropMode,
		InventoryCharacterSelect,
		Aggregate
	}

	public enum EntitlementLocationTypeEnum
	{
		Client,
		Server,
		Aggregate
	}

	public abstract class Entitlement
	{
		protected bool isConfigured;

		protected EntitlementLocationTypeEnum type;

		protected string denyReason;

		protected bool negateValue;

		protected EntitlementFlagEnum entitlementFlag;

		protected string EntitlementXmlKey;

		protected bool defaultPermission;

		[CompilerGenerated]
		private bool? _003CPlayerPermission_003Ek__BackingField;

		public bool IsConfigured
		{
			get
			{
				return isConfigured;
			}
			set
			{
				isConfigured = value;
			}
		}

		public EntitlementLocationTypeEnum Type
		{
			get
			{
				return type;
			}
		}

		public string DenyReason
		{
			get
			{
				return denyReason;
			}
		}

		public bool NegateValue
		{
			get
			{
				return negateValue;
			}
		}

		public bool? PlayerPermission
		{
			[CompilerGenerated]
			get
			{
				return _003CPlayerPermission_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CPlayerPermission_003Ek__BackingField = value;
			}
		}

		public EntitlementFlagEnum EntitlementFlag
		{
			get
			{
				return entitlementFlag;
			}
			set
			{
				entitlementFlag = value;
			}
		}

		public bool DefaultPermission
		{
			get
			{
				return defaultPermission;
			}
			set
			{
				defaultPermission = value;
			}
		}

		public virtual bool PermissionCheck()
		{
			if (PlayerPermission.HasValue)
			{
				return (!NegateValue) ? PlayerPermission.Value : (!PlayerPermission.Value);
			}
			return DefaultPermission;
		}

		public virtual bool PermissionCheck(out string ReasonIfDeny)
		{
			ReasonIfDeny = DenyReason;
			if (PlayerPermission.HasValue)
			{
				return (!NegateValue) ? PlayerPermission.Value : (!PlayerPermission.Value);
			}
			return DefaultPermission;
		}
	}

	public class ClientEntitlement : Entitlement
	{
		public ClientEntitlement(EntitlementFlagEnum flag, bool defaultPermit, string denyReason)
		{
			type = EntitlementLocationTypeEnum.Client;
			base.EntitlementFlag = flag;
			base.DefaultPermission = defaultPermit;
			base.PlayerPermission = null;
			isConfigured = true;
			base.denyReason = denyReason;
		}
	}

	public class ServerEntitlement : Entitlement
	{
		[CompilerGenerated]
		private string _003CXmlKey_003Ek__BackingField;

		public string XmlKey
		{
			[CompilerGenerated]
			get
			{
				return _003CXmlKey_003Ek__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				_003CXmlKey_003Ek__BackingField = value;
			}
		}

		public ServerEntitlement(EntitlementFlagEnum flag, string xmlKey, bool defaultPermit, string denyReason)
			: this(flag, xmlKey, defaultPermit, denyReason, false)
		{
		}

		public ServerEntitlement(EntitlementFlagEnum flag, string xmlKey, bool defaultPermit, string denyReason, bool negateValue)
		{
			type = EntitlementLocationTypeEnum.Server;
			base.EntitlementFlag = flag;
			base.DefaultPermission = defaultPermit;
			base.PlayerPermission = null;
			XmlKey = xmlKey;
			base.denyReason = denyReason;
			base.negateValue = negateValue;
		}

		public override bool PermissionCheck()
		{
			if (!base.IsConfigured)
			{
				return (!base.NegateValue) ? base.DefaultPermission : (!base.DefaultPermission);
			}
			return base.PermissionCheck();
		}

		public override bool PermissionCheck(out string ReasonIfDeny)
		{
			ReasonIfDeny = base.DenyReason;
			if (!base.IsConfigured)
			{
				return (!base.NegateValue) ? base.DefaultPermission : (!base.DefaultPermission);
			}
			return base.PermissionCheck(out ReasonIfDeny);
		}

		public virtual void SetPermissionFlag(string value)
		{
			base.PlayerPermission = Utils.ParseAsBool(value);
		}
	}

	public class AggregateEntitlement : Entitlement
	{
		private readonly Dictionary<EntitlementFlagEnum, Entitlement> entitlementsSet;

		private List<EntitlementFlagEnum> subFlags;

		public List<EntitlementFlagEnum> SubFlags
		{
			get
			{
				return subFlags;
			}
			set
			{
				subFlags = value;
			}
		}

		public AggregateEntitlement(Dictionary<EntitlementFlagEnum, Entitlement> EntitlementsSet, IEnumerable<EntitlementFlagEnum> flags)
		{
			type = EntitlementLocationTypeEnum.Aggregate;
			entitlementFlag = EntitlementFlagEnum.Aggregate;
			entitlementsSet = EntitlementsSet;
			subFlags = new List<EntitlementFlagEnum>(flags);
		}

		public override bool PermissionCheck()
		{
			foreach (EntitlementFlagEnum subFlag in subFlags)
			{
				if (!entitlementsSet[subFlag].PermissionCheck())
				{
					return false;
				}
			}
			return true;
		}

		public override bool PermissionCheck(out string ReasonIfDeny)
		{
			ReasonIfDeny = string.Empty;
			foreach (EntitlementFlagEnum subFlag in subFlags)
			{
				if (!entitlementsSet[subFlag].PermissionCheck(out ReasonIfDeny))
				{
					return false;
				}
			}
			return true;
		}
	}

	public class ServerMaxEntitlement : ServerEntitlement
	{
		protected int playerMax;

		public int PlayerMax
		{
			get
			{
				return playerMax;
			}
		}

		public ServerMaxEntitlement(EntitlementFlagEnum flag, string xmlKey, bool defaultPermit, string denyReason)
			: base(flag, xmlKey, defaultPermit, denyReason, false)
		{
		}

		public override void SetPermissionFlag(string value)
		{
			int result = 0;
			if (!string.IsNullOrEmpty(value))
			{
				int.TryParse(value, out result);
			}
			base.PlayerPermission = (result != 0);
			playerMax = result;
		}
	}

	public class ServerValueEntitlement : ServerEntitlement
	{
		[CompilerGenerated]
		private string _003CValue_003Ek__BackingField;

		public string Value
		{
			[CompilerGenerated]
			get
			{
				return _003CValue_003Ek__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				_003CValue_003Ek__BackingField = value;
			}
		}

		public ServerValueEntitlement(EntitlementFlagEnum flag, string xmlKey, bool defaultPermit, string denyReason)
			: base(flag, xmlKey, defaultPermit, denyReason)
		{
		}

		public override void SetPermissionFlag(string value)
		{
			Value = value;
		}
	}

	private readonly Dictionary<EntitlementFlagEnum, Entitlement> entitlementsSet;

	public Dictionary<EntitlementFlagEnum, Entitlement> EntitlementsSet
	{
		get
		{
			return entitlementsSet;
		}
	}

	public Entitlements()
	{
		entitlementsSet = new Dictionary<EntitlementFlagEnum, Entitlement>();
		entitlementsSet[EntitlementFlagEnum.ShieldPlayAllow] = new ServerEntitlement(EntitlementFlagEnum.ShieldPlayAllow, "shieldplayallow", false, "Shield Play Not Allowed.");
		entitlementsSet[EntitlementFlagEnum.ShieldHeroesAllow] = new ServerEntitlement(EntitlementFlagEnum.ShieldHeroesAllow, "shieldheroesallow", false, "Can't use Shield Heroes.");
		entitlementsSet[EntitlementFlagEnum.ShieldHQAllow] = new ServerEntitlement(EntitlementFlagEnum.ShieldHQAllow, "shieldhqallow", false, "Can't use Shield HQ Rooms.");
		entitlementsSet[EntitlementFlagEnum.ShieldPrizeWheelAllow] = new ServerEntitlement(EntitlementFlagEnum.ShieldPrizeWheelAllow, "shieldprizewheelallow", false, "Can't use PrizeWheel.");
		entitlementsSet[EntitlementFlagEnum.IsPayingCustomer] = new ServerEntitlement(EntitlementFlagEnum.IsPayingCustomer, "ispayingcustomer", false, "Not a paying customer.");
		entitlementsSet[EntitlementFlagEnum.IsPayingSubscriber] = new ServerEntitlement(EntitlementFlagEnum.IsPayingSubscriber, "ispayingsubscriber", false, "Not a paying Subscriber.");
		entitlementsSet[EntitlementFlagEnum.ArcadeAllow] = new ServerEntitlement(EntitlementFlagEnum.ArcadeAllow, "arcadeallow", false, "Arcade not available.");
		entitlementsSet[EntitlementFlagEnum.CardGameAllow] = new ServerEntitlement(EntitlementFlagEnum.CardGameAllow, "cardgameallow", false, "Card Game not available.");
		entitlementsSet[EntitlementFlagEnum.OpenChatAllow] = new ServerEntitlement(EntitlementFlagEnum.OpenChatAllow, "openchatallow", false, "Open Chat not available.");
		entitlementsSet[EntitlementFlagEnum.ClientConsoleAllow] = new ServerEntitlement(EntitlementFlagEnum.ClientConsoleAllow, "clientconsoleallow", false, "Console access not available.");
		entitlementsSet[EntitlementFlagEnum.UnityEditorAllow] = new ServerEntitlement(EntitlementFlagEnum.UnityEditorAllow, "unityeditorallow", false, "Editor usage not approved.");
		entitlementsSet[EntitlementFlagEnum.DebuggingAllow] = new ServerEntitlement(EntitlementFlagEnum.DebuggingAllow, "debuggingallow", false, "Editor usage not approved.");
		entitlementsSet[EntitlementFlagEnum.WipMissionsAllow] = new ServerEntitlement(EntitlementFlagEnum.WipMissionsAllow, "wipallow", false, "Work in Progress Missions not enabled.");
		entitlementsSet[EntitlementFlagEnum.DemoLimitsOn] = new ServerEntitlement(EntitlementFlagEnum.DemoLimitsOn, "demolimitson", false, "No Demo Limits", true);
		entitlementsSet[EntitlementFlagEnum.ParentalHqDeny] = new ServerEntitlement(EntitlementFlagEnum.ParentalHqDeny, "parentalhqdeny", false, "Parental Restriction", true);
		entitlementsSet[EntitlementFlagEnum.ParentalMissionsDeny] = new ServerEntitlement(EntitlementFlagEnum.ParentalMissionsDeny, "parentalmissionsdeny", false, "Parental Restriction", true);
		entitlementsSet[EntitlementFlagEnum.ParentalCardGameDeny] = new ServerEntitlement(EntitlementFlagEnum.ParentalCardGameDeny, "parentalcardgamedeny", false, "Parental Restriction", true);
		entitlementsSet[EntitlementFlagEnum.ParentalFriendingDeny] = new ServerEntitlement(EntitlementFlagEnum.ParentalFriendingDeny, "parentalfriendingdeny", false, "Parental Restriction", true);
		entitlementsSet[EntitlementFlagEnum.ParentalBlockingDeny] = new ServerEntitlement(EntitlementFlagEnum.ParentalBlockingDeny, "parentalblockingdeny", false, "Parental Restriction", true);
		entitlementsSet[EntitlementFlagEnum.ParentalOpenChatDeny] = new ServerEntitlement(EntitlementFlagEnum.ParentalOpenChatDeny, "parentalopenchatdeny", false, "Parental Restriction", true);
		entitlementsSet[EntitlementFlagEnum.ParentalShoppingDeny] = new ServerEntitlement(EntitlementFlagEnum.ParentalShoppingDeny, "parentalshoppingdeny", false, "Parental Restriction", true);
		entitlementsSet[EntitlementFlagEnum.SubscriptionPagesAllow] = new ServerEntitlement(EntitlementFlagEnum.SubscriptionPagesAllow, "subscriptionpages", false, "Subscription Pages");
		entitlementsSet[EntitlementFlagEnum.TutorialVOAllow] = new ServerEntitlement(EntitlementFlagEnum.TutorialVOAllow, "tutorialvoallow", true, "Tutorial VO not enabled.");
		entitlementsSet[EntitlementFlagEnum.FlavorVOAllow] = new ServerEntitlement(EntitlementFlagEnum.FlavorVOAllow, "flavorvoallow", true, "Flavor VO not enabled.");
		entitlementsSet[EntitlementFlagEnum.UseLocalizedTutorialVO] = new ServerEntitlement(EntitlementFlagEnum.UseLocalizedTutorialVO, "uselocalizedtutorialvo", true, "English tutorial VO only.");
		entitlementsSet[EntitlementFlagEnum.UseLocalizedFlavorVO] = new ServerEntitlement(EntitlementFlagEnum.UseLocalizedFlavorVO, "uselocalizedflavorvo", true, "English flavor VO only.");
		entitlementsSet[EntitlementFlagEnum.PlayerCountry] = new ServerValueEntitlement(EntitlementFlagEnum.PlayerCountry, "playercountry", true, string.Empty);
		entitlementsSet[EntitlementFlagEnum.PlayerLanguage] = new ServerValueEntitlement(EntitlementFlagEnum.PlayerLanguage, "playerlanguage", true, string.Empty);
		entitlementsSet[EntitlementFlagEnum.ShoppingCatalog] = new ServerValueEntitlement(EntitlementFlagEnum.ShoppingCatalog, "shoppingcatalog", true, string.Empty);
		entitlementsSet[EntitlementFlagEnum.UseExternalShopping] = new ServerEntitlement(EntitlementFlagEnum.UseExternalShopping, "useexternalshopping", false, string.Empty);
		entitlementsSet[EntitlementFlagEnum.CatalogCountry] = new ServerValueEntitlement(EntitlementFlagEnum.CatalogCountry, "catalogcountry", true, string.Empty);
		entitlementsSet[EntitlementFlagEnum.SubscriptionType] = new ServerValueEntitlement(EntitlementFlagEnum.SubscriptionType, "subscriptiontype", true, string.Empty);
		entitlementsSet[EntitlementFlagEnum.Expiration] = new ServerValueEntitlement(EntitlementFlagEnum.Expiration, "expiration", true, string.Empty);
		entitlementsSet[EntitlementFlagEnum.DailyRewardStr] = new ServerValueEntitlement(EntitlementFlagEnum.DailyRewardStr, "dailyrewardstr", true, string.Empty);
		entitlementsSet[EntitlementFlagEnum.MaxChallengeLevel] = new ServerMaxEntitlement(EntitlementFlagEnum.MaxChallengeLevel, "maxchallengelevel", false, "Challenge System Disabled");
		entitlementsSet[EntitlementFlagEnum.MySquadPermit] = new ClientEntitlement(EntitlementFlagEnum.MySquadPermit, true, "My Squad not available at this time.");
		entitlementsSet[EntitlementFlagEnum.FriendsListPermit] = new ClientEntitlement(EntitlementFlagEnum.FriendsListPermit, true, "Friends List not permitted at this time.");
		entitlementsSet[EntitlementFlagEnum.ShopWindowPermit] = new ClientEntitlement(EntitlementFlagEnum.ShopWindowPermit, true, "Shopping not available at this time.");
		entitlementsSet[EntitlementFlagEnum.InventoryDragDropMode] = new ClientEntitlement(EntitlementFlagEnum.InventoryDragDropMode, true, "Drag Drop not available at this time.");
		entitlementsSet[EntitlementFlagEnum.InventoryCharacterSelect] = new ClientEntitlement(EntitlementFlagEnum.InventoryCharacterSelect, false, "Character Selection not available at this time.");
		entitlementsSet[EntitlementFlagEnum.PrizeWheelPermit] = new ClientEntitlement(EntitlementFlagEnum.PrizeWheelPermit, true, "Prize Wheel is not available at this time.");
		entitlementsSet[EntitlementFlagEnum.MissionsPermitSet] = new AggregateEntitlement(entitlementsSet, new EntitlementFlagEnum[1]
		{
			EntitlementFlagEnum.ParentalMissionsDeny
		});
		entitlementsSet[EntitlementFlagEnum.CardGamePermitSet] = new AggregateEntitlement(entitlementsSet, new EntitlementFlagEnum[2]
		{
			EntitlementFlagEnum.CardGameAllow,
			EntitlementFlagEnum.ParentalCardGameDeny
		});
		entitlementsSet[EntitlementFlagEnum.OpenChatPermitSet] = new AggregateEntitlement(entitlementsSet, new EntitlementFlagEnum[2]
		{
			EntitlementFlagEnum.OpenChatAllow,
			EntitlementFlagEnum.ParentalOpenChatDeny
		});
	}

	public void Configure(DataWarehouse entitlementData)
	{
		List<ServerEntitlement> list = Enumerable.ToList(entitlementsSet.Values).FindAll(delegate(Entitlement e)
		{
			return e is ServerEntitlement;
		}).ConvertAll(delegate(Entitlement e)
		{
			return e as ServerEntitlement;
		});
		foreach (XPathNavigator value in entitlementData.GetValues("//entitlements/entitlement"))
		{
			string text = value.GetAttribute("name", string.Empty).ToLowerInvariant();
			string attribute = value.GetAttribute("value", string.Empty);
			bool flag = false;
			foreach (ServerEntitlement item in list)
			{
				if (item.XmlKey == text)
				{
					item.SetPermissionFlag(attribute);
					item.IsConfigured = true;
					flag = true;
				}
			}
			if (!flag)
			{
				CspUtils.DebugLog("Found: " + text + " server entitlement, but no corresponding client entitlement to match it.");
			}
		}
		ServerValueEntitlement serverValueEntitlement = Singleton<Entitlements>.instance.EntitlementsSet[EntitlementFlagEnum.ShoppingCatalog] as ServerValueEntitlement;
	}

	public void ConfigureEntitlement(EntitlementFlagEnum flag, bool value)
	{
		Entitlement value2;
		if (entitlementsSet.TryGetValue(flag, out value2))
		{
			value2.PlayerPermission = value;
			value2.IsConfigured = true;
		}
		else
		{
			CspUtils.DebugLog("Couldn't find entitlement in set for " + flag);
		}
	}

	public bool PermissionCheck(EntitlementFlagEnum permitFlag)
	{
		if (!entitlementsSet.ContainsKey(permitFlag))
		{
			CspUtils.DebugLogWarning("Can't find flag: " + permitFlag + " in permission set. Defaulting to no permission.");
			return false;
		}
		return entitlementsSet[permitFlag].PermissionCheck();
	}

	public bool MaxCheck(EntitlementFlagEnum maxFlag, int value)
	{
		if (!entitlementsSet.ContainsKey(maxFlag))
		{
			CspUtils.DebugLogWarning("Can't find flag: " + maxFlag + " in permission set. Defaulting to false for max check.");
			return false;
		}
		ServerMaxEntitlement serverMaxEntitlement = entitlementsSet[maxFlag] as ServerMaxEntitlement;
		if (serverMaxEntitlement == null)
		{
			CspUtils.DebugLogWarning("Flag: " + maxFlag + " is not a max entitlement. Defaulting to false for max check.");
			return false;
		}
		return value > 0 && serverMaxEntitlement.PlayerMax >= value;
	}
}
