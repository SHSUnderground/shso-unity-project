using UnityEngine;

public class CardMysteryBoxItemData : MysteryBoxItemData
{
	private Texture2D cardTexture;

	public override string itemTextureSource
	{
		get
		{
			return string.Empty;
		}
	}

	public override string name
	{
		get
		{
			OwnableDefinition def = OwnableDefinition.getDef(ownableTypeID);
			if (def == null)
			{
				return null;
			}
			BattleCard battleCard = new BattleCard(def.name);
			return battleCard.Name;
		}
	}

	public Texture2D itemTexture
	{
		get
		{
			if (cardTexture != null)
			{
				return cardTexture;
			}
			OwnableDefinition def = OwnableDefinition.getDef(ownableTypeID);
			if (def == null)
			{
				return null;
			}
			BattleCard battleCard = new BattleCard(def.name);
			cardTexture = battleCard.FullTexture;
			return cardTexture;
		}
	}

	public CardMysteryBoxItemData(int ownable_id, int quantity)
		: base(ownable_id, quantity)
	{
		Texture2D itemTexture = this.itemTexture;
	}
}
