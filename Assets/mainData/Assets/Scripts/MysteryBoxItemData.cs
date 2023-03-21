using UnityEngine;

public class MysteryBoxItemData
{
	public int rarity = 10000;

	public int ownableTypeID;

	public int quantity;

	public virtual string itemTextureSource
	{
		get
		{
			OwnableDefinition def = OwnableDefinition.getDef(ownableTypeID);
			return def.shoppingIcon;
		}
	}

	public virtual Vector2 itemPictureSize
	{
		get
		{
			OwnableDefinition def = OwnableDefinition.getDef(ownableTypeID);
			//return def.shoppingIconSize;
			//Temporarily hard coded value! (by Doggo)
			return new Vector2(262f, 328f);
		}
	}

	public virtual bool displayInBox
	{
		get
		{
			return true;
		}
	}

	public virtual string name
	{
		get
		{
			return string.Empty;
		}
	}

	public MysteryBoxItemData(int ownable_id, int quan = 1)
	{
		ownableTypeID = ownable_id;
		quantity = quan;
	}

	public static MysteryBoxItemData factory(int ownable_id, int quantity = 1, string catStr = "")
	{
		OwnableDefinition.Category category = OwnableDefinition.Category.Unknown;
		if (catStr == string.Empty)
		{
			OwnableDefinition def = OwnableDefinition.getDef(ownable_id);
			if (def != null)
			{
				category = def.category;
			}
		}
		else
		{
			category = OwnableDefinition.getCategoryFromString(catStr);
		}
		switch (category)
		{
		case OwnableDefinition.Category.Hero:
			return new HeroMysteryBoxItemData(ownable_id, quantity);
		case OwnableDefinition.Category.HQItem:
			return new HQItemMysteryBoxItemData(ownable_id, quantity);
		case OwnableDefinition.Category.Card:
			return new CardMysteryBoxItemData(ownable_id, quantity);
		case OwnableDefinition.Category.BoosterPack:
			return new BoosterMysteryBoxItemData(ownable_id, quantity);
		case OwnableDefinition.Category.HQRoom:
			return new HQRoomMysteryBoxItemData(ownable_id, quantity);
		case OwnableDefinition.Category.Mission:
			return new MissionMysteryBoxItemData(ownable_id, quantity);
		case OwnableDefinition.Category.Quest:
			return new CardQuestMysteryBoxItemData(ownable_id, quantity);
		case OwnableDefinition.Category.Potion:
			return new PotionMysteryBoxItemData(ownable_id, quantity);
		case OwnableDefinition.Category.Badge:
			return new BadgeMysteryBoxItemData(ownable_id, quantity);
		case OwnableDefinition.Category.Sidekick:
			return new SidekickMysteryBoxItemData(ownable_id, quantity);
		case OwnableDefinition.Category.Craft:
			return new SidekickCraftItemData(ownable_id, quantity);
		case OwnableDefinition.Category.SidekickUpgrade:
			return new SidekickUpgradeMysteryBoxItemData(ownable_id, quantity);
		default:
			CspUtils.DebugLog("ERROR:  could not create a MysteryBoxItemData for item category " + category + " with ID " + ownable_id);
			return null;
		}
	}
}
