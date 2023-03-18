using UnityEngine;

public class CardQuestMysteryBoxItemData : MysteryBoxItemData
{
	public override string itemTextureSource
	{
		get
		{
			CardQuestPart questPart = AppShell.Instance.CardQuestManager.GetQuestPart(ownableTypeID);
			if (questPart != null && questPart.PartType == CardQuestPartsTypeEnum.Hard)
			{
				return "shopping_bundle|L_shopping_cardquest_tough_box";
			}
			return "shopping_bundle|L_shopping_cardquest_easy_box";
		}
	}

	public override Vector2 itemPictureSize
	{
		get
		{
			return new Vector2(275f, 337f);
		}
	}

	public override string name
	{
		get
		{
			OwnableDefinition def = OwnableDefinition.getDef(ownableTypeID);
			if (def == null)
			{
				return string.Empty;
			}
			return def.name;
		}
	}

	public CardQuestMysteryBoxItemData(int ownable_id, int quantity)
		: base(ownable_id, quantity)
	{
	}
}
