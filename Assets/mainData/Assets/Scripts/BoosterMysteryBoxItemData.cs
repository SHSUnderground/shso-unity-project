public class BoosterMysteryBoxItemData : MysteryBoxItemData
{
	public override string itemTextureSource
	{
		get
		{
			OwnableDefinition def = OwnableDefinition.getDef(ownableTypeID);
			if (def == null)
			{
				return string.Empty;
			}
			return "shopping_bundle|L_shopping_booster_pack_" + def.name;
		}
	}

	public BoosterMysteryBoxItemData(int ownable_id, int quantity)
		: base(ownable_id, quantity)
	{
	}
}
